using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;

namespace SlideCommunication
{
    public class FrameConsumer : Consumer
    {
        private ISpace ConcealedSpace { get; set; }
        public FrameConsumer(ISpace space, ISpace concealedSpace) : base(space)
        {
            ConcealedSpace = concealedSpace;
        } 

        public IEnumerable<byte[]> Bitstreams { set { SetupSlides(value); } }

        protected override Action GetHostAction()
        {
            return Broadcast;
        }

        private void Broadcast()
        {
            while (true)
            {
                var request = Space.Get("Request", RequestType.FrameRequest, typeof(string), typeof(string));
                var token = request.Get<string>(2);
                var username = request.Get<string>(3);
                var pageTuple = Space.GetP("SlideRequestPayload", token, typeof(List<int>));
                var pages = pageTuple.Get<List<int>>(2);
                var frames = pages
                    .Select(
                        p => PrivateSpace.QueryP("Frame", p, typeof(FramePayload))?.Get<FramePayload>(2)
                    ).ToList();
                ConcealedSpace.Put("UnvalidatedResponse", token, username, RequestType.FrameRequest, frames);
            }
        }

        private void FlushFrames()
        {
            PrivateSpace.GetAll("Frame", typeof(int), typeof(FramePayload));
        }

        private void SetupSlides(IEnumerable<byte[]> imageBitstreams)
        {
            FlushFrames();
            int i = 1;
            foreach (var bs in imageBitstreams)
            {
                PrivateSpace.Put("Frame", i, new FramePayload { PageNumber = i++, Bitstream = bs });
            }
        }
    }
}
