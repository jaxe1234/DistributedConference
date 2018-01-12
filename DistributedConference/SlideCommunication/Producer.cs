using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class Producer
    {
        private ISpace Space { get; set; }
        private string Username { get; set; }

        private string SessionSecret { get
            {
                var t = Private.QueryP("SessionSecret", typeof(string));
                if (t == null)
                {
                    ConnectToSession();
                    t = Private.Query("SessionSecret", typeof(string));
                }
                return t.Get<string>(1);
            }
        }

        private ISpace Private { get; set; }

        public Producer(ISpace space, string username)
        {
            Space = space;
            Private = new SequentialSpace();
            Username = username;
        }

        public IEnumerable<byte[]> GetFrames(params int[] pages)
        {
            var frames = pages
                .Select(
                    i => Private
                        .QueryP("Frame", typeof(int), typeof(FramePayload))
                        ?.Get<FramePayload>(2) ?? new FramePayload { PageNumber = i }
                    ).ToList();
            var pagesToRecieve = frames
                .Where(p => p.Bitstream == null)
                .Select(p => p.PageNumber).ToList();
            var token = new RequestToken(SessionSecret);
            Space.Put("SlideRequestPayload", token.Token, pagesToRecieve.ToList());
            Space.Put("Request", RequestType.FrameRequest, token.Token, Username);
            var t1 = Space.Get("Response", token.ResponseToken, Username, RequestType.FrameRequest, typeof(List<FramePayload>));
            var recievedPages = new Queue<FramePayload>(t1.Get<List<FramePayload>>(4).AsEnumerable());
            foreach (var p in frames)
            {
                if (p.Bitstream == null)
                {
                    p.Bitstream = recievedPages.Dequeue().Bitstream;
                    Private.Put("Frame", p.PageNumber, p);
                }
            }
            return frames.Select(a => a.Bitstream);
        }

        private void ConnectToSession()
        {
            using (var me = new ECDiffieHellmanCng())
            {
                Space.Put("Request", HubRequestType.EstablishSession, Username, Convert.ToBase64String(me.PublicKey.ToByteArray()));
                var tuple = Space.Get("Response", Username, HubRequestType.EstablishSession, typeof(string));
                var herKey = CngKey.Import(Convert.FromBase64String(tuple.Get<string>(3)), CngKeyBlobFormat.EccPublicBlob);
                var ourKey = Convert.ToBase64String(me.DeriveKeyMaterial(herKey));
                Private.Put("SessionSecret", ourKey);
            }
        }
    }
}
