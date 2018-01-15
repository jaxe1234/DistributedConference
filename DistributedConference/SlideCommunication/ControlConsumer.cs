using System;
using dotSpace.Interfaces.Space;
using NamingTools;
using dotSpace.Objects.Network;
using System.Linq;
using System.Collections.Generic;

namespace SlideCommunication
{
    public class ControlConsumer : Consumer
    {
        public ISlideShow SlideShower { get; }
        private ClientSession Session { get; }

        private RequestToken _lastToken;

        public ControlConsumer(ClientSession session, ISlideShow slideShow) : base(session.Space)
        {
            Session = session;
            SlideShower = slideShow;
            SendToken();
        }

        protected override Action GetHostAction()
        {
            return Listen;
        }

        private void SendToken()
        {
            _lastToken = Session.CreateToken();
            Space.Put("SlideChangeToken", Session.Username, _lastToken.Token);
        }

        private void Listen()
        {
            SetupRequest();
            while (true)
            {
                var tuple = Space.Get("SlideChange", _lastToken.ResponseToken, Session.Username, typeof(int));
                var page = tuple.Get<int>(3);
                var ftuple = Session.LocalSpace.Query("Frame", page, typeof(byte[]));
                var bitstream = ftuple.Get<byte[]>(2);
                SlideShower.UpdateSlide(bitstream);
                SendToken();
            }
        }

        private void SetupRequest()
        {
            var tuple = Space.Query("ActiveCollection", typeof(string), typeof(List<int>));
            var id = tuple.Get<string>(1);
            var pages = tuple.Get<List<int>>(2);
            foreach (var page in pages)
            {
                Space.Put("FramePayloadRequest", id, Session.Username, page);
            }
        }
    }
}
