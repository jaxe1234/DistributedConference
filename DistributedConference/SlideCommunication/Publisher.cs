using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;

namespace SlideCommunication
{
    public class Publisher : Consumer
    {
        private int _lastPage = 1;

        private ISpace ExposedSpace { get; }
        private ISpace ConcealedSpace { get; }
        public Publisher(ISpace space, ISpace exposedSpace, ISpace concealedSpace) : base(space)
        {
            ExposedSpace = exposedSpace;
            ConcealedSpace = concealedSpace;
        }

        protected override Action GetHostAction()
        {
            return () => Parallel.Invoke(FrameRequest, SlideControl);
        }

        public void SyncIncomingUser(string identifier)
        {
            Task.Run(() => {
                var key = GetSharedSecret(identifier);
                var tuple = ExposedSpace.Get("ControlConsumerToken", identifier, typeof(string));
                var token = tuple.Get<string>(2);
                var rtoken = new RequestToken(token, key);

                var colTuple = ConcealedSpace.Query("ActiveCollection", typeof(string), typeof(int));
                var collectionId = colTuple.Get<string>(1);
                var pages = colTuple.Get<int>(2);

                ExposedSpace.Put("SlideSync", collectionId, rtoken.ResponseToken, identifier, _lastPage, pages);
            });
        }

        private void FrameRequest()
        {
            while (true)
            {
                var tuple = ConcealedSpace.Get("UnauthenticatedFramePayload", typeof(string), typeof(string), typeof(FramePayload));
                var username = tuple.Get<string>(1);
                var token = tuple.Get<string>(2);
                var payload = tuple.Get<FramePayload>(3);
                var key = GetSharedSecret(username);
                var hash = ProjectUtilities.NameHashingTool.GetSHA256String(key + token);
                ExposedSpace.Put("FramePayload", username, token, hash, payload);
            }
        }

        private string GetSharedSecret(string identifier)
        {
            return Space.QueryP("SessionSecret", typeof(string), identifier)?.Get<string>(1);
        }

        private void SlideControl()
        {
            while (true)
            {
                var slideChangeTuple = ConcealedSpace.Get("SlideChange", typeof(string), typeof(int), typeof(int));
                var collectionId = slideChangeTuple.Get<string>(1);
                var page = slideChangeTuple.Get<int>(2);
                var pages = slideChangeTuple.Get<int>(3);
                var tokens = ExposedSpace.GetAll("ControlConsumerToken", typeof(string), typeof(string));
                
                foreach (var tuple in tokens)
                {
                    var token = tuple.Get<string>(2);
                    var username = tuple.Get<string>(1);
                    var key = GetSharedSecret(username);
                    var resposeToken = ProjectUtilities.NameHashingTool.GetSHA256String(key + token);
                    ExposedSpace.Put("SlideChange", collectionId, resposeToken, username, page, pages);
                }

                _lastPage = page;
            }
        }
    }
}
