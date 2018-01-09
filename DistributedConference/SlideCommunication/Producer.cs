using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class Producer
    {
        private ISpace Space { get; set; }
        private string Username { get; set; }
        private string SessionKey { get
            {
                var t = Private.QueryP("SessionKey", typeof(string));
                if (t == null)
                {
                    ConnectToSession();
                    t = Private.Query("SessionKey", typeof(string));
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
            Space.Put("SlideRequestPayload", SessionKey, pagesToRecieve.ToList());
            Space.Put("Request", RequestType.FrameRequest, SessionKey);
            var t1 = Space.Get("Response", SessionKey, RequestType.FrameRequest, typeof(List<FramePayload>));
            var recievedPages = new Queue<FramePayload>(t1.Get<List<FramePayload>>(3).AsEnumerable());
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
            Space.Put("Request", RequestType.EstablishSession, Username);
            var tuple = Space.Get("Response", typeof(string), RequestType.EstablishSession);
            Private.Put("SessionKey", tuple.Get<string>(1));
        }
    }
}
