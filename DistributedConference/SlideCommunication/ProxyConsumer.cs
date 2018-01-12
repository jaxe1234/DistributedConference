using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;

namespace SlideCommunication
{
    public class ProxyConsumer : Consumer
    {
        private ISpace ExposedSpace { get; }
        private ISpace ConcealedSpace { get; }
        public ProxyConsumer(ISpace space, ISpace exposedSpace, ISpace concealedSpace) : base(space)
        {
            ExposedSpace = exposedSpace;
            ConcealedSpace = concealedSpace;
        }

        protected override Action GetHostAction(HostingMode mode)
        {
            switch (mode)
            {
                case HostingMode.Idle:
                    return (() => { });
                default:
                    return () => Parallel.Invoke(FrameRequest, SlideControl);
            }
        }

        private void FrameRequest()
        {
            while (true)
            {
                var response = ConcealedSpace.Get("UnvalidatedResponse", typeof(string), typeof(string), RequestType.FrameRequest, typeof(List<FramePayload>)).Fields;
                var token = response[1] as string;
                var username = response[2] as string;
                var key = Space.QueryP("SessionSecret", typeof(string), username)?.Get<string>(1);
                response[0] = "Response";
                response[1] = NamingTools.NamingTool.GetSHA256String(key + token);
                ExposedSpace.Put(response);
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
                    var token = tuple.Get<string>(1);
                    var username = tuple.Get<string>(1);
                    var key = Space.QueryP("SessionSecret", typeof(string), username)?.Get<string>(1);
                    ExposedSpace.Put("SlideChange", key, username, page);
                }
            }
        }
    }
}
