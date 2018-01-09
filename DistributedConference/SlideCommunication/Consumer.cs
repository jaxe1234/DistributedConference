using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlideCommunication
{
    public class Consumer : IDisposable
    {
        private ISpace Space { get; set; }
        private ISpace PrivateSpace { get; set; }
        
        private CancellationTokenSource Cacelation { get; set; }

        public Consumer(ISpace space, byte[][] imageBitstreams)
        {
            Space = space;
            PrivateSpace = new SequentialSpace();
            Cacelation = new CancellationTokenSource();

            SetupSlides(imageBitstreams); 

            var task = Task.Run(() => Listen(), Cacelation.Token);
        }

        private void Listen()
        {
            while (true)
            {
                // (Request, (SessionKey/Username), RequestType)
                var request = Space.Get("Request", typeof(RequestType), typeof(string));
                var identifier = request.Get<string>(2);
                var requestType = request.Get<RequestType>(1);

                string username = null;
                var authenticated = false;
                if (requestType != RequestType.EstablishSession)
                {
                    var t = PrivateSpace.QueryP("SessionKey", identifier, typeof(string));
                    authenticated = t != null;
                    username = t.Get<string>(2);
                }

                switch (requestType)
                {
                    case (RequestType.EstablishSession):
                        {
                            var key = NamingTools.RepoUtility.UniqueString(64);
                            var t1 = PrivateSpace.QueryP("SessionKey", typeof(string), identifier);
                            var t2 = PrivateSpace.QueryP("SessionKey", key, typeof(string));
                            if (t1 == null && t2 == null)
                            {
                                PrivateSpace.Put("SessionKey", key, identifier);
                                Space.Put("Response", key, RequestType.EstablishSession);
                            }
                            break;
                        }
                    case (RequestType.TerminateSession):
                        {
                            if (authenticated)
                            {
                                var t1 = PrivateSpace.GetP("SessionKey", identifier, username);
                                if (t1 == null)
                                {
                                    PrivateSpace.GetP("SessionKey", identifier, typeof(string));
                                    PrivateSpace.GetP("SessionKey", typeof(string), identifier);
                                }
                            }
                        }
                        break;
                    case (RequestType.FrameRequest):
                        {
                            var pageTuple = Space.GetP("SlideRequestPayload", identifier, typeof(List<int>));
                            var pages = pageTuple.Get<List<int>>(2);
                            var frames = pages
                                .Select(
                                    p => PrivateSpace.QueryP("Frame", p, typeof(FramePayload))?.Get<FramePayload>(2)
                                ).ToList();
                            Space.Put("Response", identifier, RequestType.FrameRequest, frames);
                        }
                        break;
                }
            }
        }

        private void SetupSlides(byte[][] imageBitstreams)
        {
            int i = 1;
            foreach (var bs in imageBitstreams)
            {
                PrivateSpace.Put("Frame", i, new FramePayload { PageNumber = i++, Bitstream = bs });
            }
        }

        public void Dispose()
        {
            Cacelation.Cancel();
        }
    }
}
