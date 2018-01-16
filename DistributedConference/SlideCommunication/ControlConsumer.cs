using System;
using dotSpace.Interfaces.Space;
using NamingTools;
using dotSpace.Objects.Network;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class ControlConsumer : Consumer
    {
        public ISlideShow SlideShower { get; }
        private ClientSession Session { get; }

        private RequestToken _lastToken;
        private string _lastCollection;

        public int NumberOfPages { get; private set; }

        public ControlConsumer(ClientSession session, ISlideShow slideShow) : base(session.Space)
        {
            Session = session;
            SlideShower = slideShow;
        }

        protected override Action GetHostAction()
        {
            return Listen;
        }

        private void SyncWithHost()
        {
            SendToken();
            //("SlideSync", collectionId, rtoken.ResponseToken, identifier, _lastPage, pages)
            var tuple = Space.Get("SlideSync", typeof(string), _lastToken.ResponseToken, Session.Username, typeof(int), typeof(int));
            NumberOfPages = tuple.Get<int>(5);
            var page = tuple.Get<int>(4);
            var collectionId = tuple.Get<string>(1);
            if (string.IsNullOrEmpty(_lastCollection) || collectionId != _lastCollection)
            {
                SetupRequest(collectionId);
            }
            var ftuple = Session.LocalSpace.Query("Frame", page, typeof(FramePayload));
            var payload = ftuple.Get<FramePayload>(2);
            SlideShower.UpdateSlide(payload);
        }

        private void SendToken()
        {
            if (_lastToken != null)
            {
                Space.GetAll("ControlConsumerToken", Session.Username, _lastToken.Token);
            }
            _lastToken = Session.CreateToken();
            Space.Put("ControlConsumerToken", Session.Username, _lastToken.Token);
        }

        private void Listen()
        {
            SyncWithHost();
            while (true)
            {
                SendToken();
                var tuple = Space.Get("SlideChange", typeof(string),  _lastToken.ResponseToken, Session.Username, typeof(int), typeof(int));
                var page = tuple.Get<int>(4);
                NumberOfPages = tuple.Get<int>(5);
                var collectionId = tuple.Get<string>(1);

                if (_lastCollection != collectionId)
                {
                    SetupRequest(collectionId);
                }

                var ftuple = Session.LocalSpace.Query("Frame", page, typeof(FramePayload));
                var payload = ftuple.Get<FramePayload>(2);
                SlideShower.UpdateSlide(payload);
            }
        }

        private void SetupRequest(string collectionId)
        {
            Session.LocalSpace.GetAll("Frame", typeof(int), typeof(FramePayload));
            _lastCollection = collectionId;
            SlideShower.NewCollection(NumberOfPages);
            foreach (var page in Enumerable.Range(1, NumberOfPages))
            {
                Space.Put("FramePayloadRequest", collectionId, Session.Username, page);
            }
        }
    }
}
