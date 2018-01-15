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

        private ISpace ConcealedSpace { get; set; }
        public FrameTransformer(ISpace space, ISpace concealedSpace) : base(space)
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
                var request = Space.Get("FramePayloadRequest", CollectionIdentifier, typeof(string), typeof(int));
                var username = request.Get<string>(2);
                var page = request.Get<int>(3);
                var token = Guid.NewGuid().ToString();
                var tuple = PrivateSpace.QueryP("Frame", page, typeof(FramePayload));
                var payload = tuple.Get<FramePayload>(2);
                ConcealedSpace.Put("UnauthenticatedFramePayload", username, token, payload);
            }
        }

        private void FlushFrames()
        {
            Space.GetAll("ActiveCollection", typeof(string), typeof(List<int>));
            PrivateSpace.GetAll("Frame", typeof(int), typeof(FramePayload));
        }

        private void SetupSlides(IEnumerable<byte[]> imageBitstreams)
        {
            FlushFrames();
            CollectionIdentifier = Guid.NewGuid().ToString();
            Space.Put("ActiveCollection", CollectionIdentifier, Enumerable.Range(1, imageBitstreams.Count()).ToList() );
            var i = 1;
            foreach (var bs in imageBitstreams)
            {
                PrivateSpace.Put("Frame", i, new FramePayload { PageNumber = i++, Bitstream = bs });
            }
        }
    }
}
