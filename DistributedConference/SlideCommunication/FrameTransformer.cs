using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;

namespace SlideCommunication
{
    public class FrameTransformer : Consumer
    {
        public string CollectionIdentifier { get; private set; }
        public ISlideShow SlideShower { get; set; }

        private ISpace ConcealedSpace { get; set; }
        public FrameTransformer(ISpace space, ISpace concealedSpace, ISlideShow slideShower) : base(space)
        {
            ConcealedSpace = concealedSpace;
            SlideShower = slideShower;
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
                var request = Space.Get("FramePayloadRequest", CollectionIdentifier, typeof(string), typeof(int));
                var username = request.Get<string>(2);
                var page = request.Get<int>(3);
                var token = Guid.NewGuid().ToString();
                var tuple = ConcealedSpace.QueryP("Frame", page, typeof(FramePayload));
                var payload = tuple.Get<FramePayload>(2);
                ConcealedSpace.Put("UnauthenticatedFramePayload", username, token, payload);
            }
        }

        private void FlushFrames()
        {
            ConcealedSpace.GetAll("ActiveCollection", typeof(string), typeof(int));
            ConcealedSpace.GetAll("Frame", typeof(int), typeof(FramePayload));
        }

        private void SetupSlides(IEnumerable<byte[]> imageBitstreams)
        {
            FlushFrames();
            CollectionIdentifier = Guid.NewGuid().ToString();
            ConcealedSpace.Put("ControlLock", CollectionIdentifier, imageBitstreams.Count());
            ConcealedSpace.Put("ActiveCollection", CollectionIdentifier, imageBitstreams.Count() );
            var i = 1;
            foreach (var bs in imageBitstreams)
            {
                ConcealedSpace.Put("Frame", i, new FramePayload { PageNumber = i++, Bitstream = bs });
            }
        }
    }
}
