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
    public class FrameConsumer : Consumer
    {
        private ClientSession Session { get; }
        private ISpace Private { get; set; }

        public FrameConsumer(ClientSession session) : base(session.Space)
        {
            Session = session;
            Private = Session.LocalSpace;
        }

        public void Listen()
        {
            while (true)
            {
                var tuple = Space.Get("FramePayload", Session.Username, typeof(string), typeof(string), typeof(FramePayload));
                var stoken = tuple.Get<string>(2);
                var hash = tuple.Get<string>(3);
                var token = Session.CreateToken(stoken);
                if (hash == token.ResponseToken)
                {
                    var payload = tuple.Get<FramePayload>(4);
                    Private.Put("Frame", payload.PageNumber, payload.Bitstream);
                }
            }
        }

        protected override Action GetHostAction()
        {
            return Listen;
        }
    }
}
