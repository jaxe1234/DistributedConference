using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;

namespace SlideCommunication
{
    public class PublishTransformer : Consumer
    {
        private ISpace ExposedSpace { get; }
        private ISpace ConcealedSpace { get; }
        public PublishTransformer(ISpace space, ISpace exposedSpace, ISpace concealedSpace) : base(space)
        {
            ExposedSpace = exposedSpace;
            ConcealedSpace = concealedSpace;
        }

        protected override Action GetHostAction()
        {
            return () => Parallel.Invoke(FrameRequest, SlideControl);
        }

        private void FrameRequest()
        {
            while (true)
            {
                var tuple = ConcealedSpace.Get("UnauthenticatedFramePayload", typeof(string), typeof(string), typeof(FramePayload));
                var username = tuple.Get<string>(1);
                var token = tuple.Get<string>(2);
                var payload = tuple.Get<FramePayload>(3);
                var key = Space.QueryP("SessionSecret", typeof(string), username)?.Get<string>(1);
                var hash = NamingTools.NameHashingTool.GetSHA256String(key + token);
                ExposedSpace.Put("FramePayload", username, token, hash, payload);
            }
        }

        private void SlideControl()
        {
            while (true)
            {
                var slideChangeTuple = ConcealedSpace.Get("SlideChange", typeof(string), typeof(int));
                var page = slideChangeTuple.Get<int>(2);
                var tokens = ExposedSpace.GetAll("SlideChangeToken", typeof(string), typeof(string));
                
                foreach (var tuple in tokens)
                {
                    var token = tuple.Get<string>(2);
                    var username = tuple.Get<string>(1);
                    var key = Space.QueryP("SessionSecret", typeof(string), username)?.Get<string>(1);
                    var resposeToken = NamingTools.NameHashingTool.GetSHA256String(key + token);
                    ExposedSpace.Put("SlideChange", resposeToken, username, page);
                }
            }
        }
    }
}
