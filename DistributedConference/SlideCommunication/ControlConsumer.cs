using System;
using dotSpace.Interfaces.Space;
using NamingTools;
using dotSpace.Objects.Network;
using System.Linq;

namespace SlideCommunication
{
    public class ControlConsumer : Consumer
    {
        public ISlideShow SlideShower { get; }
        private FrameProducer Producer { get; }

        public ControlConsumer(FrameProducer producer, ISlideShow slideShow) : base(producer.Space)
        {
            Producer = producer;
            SlideShower = slideShow;
        }

        private string Identifier { get; }

        protected override Action GetHostAction()
        {
            return Listen;
        }

        private void Listen()
        {
            while (true)
            {
                var token = Producer.CreateToken();
                Space.Put("SlideChangeToken", Identifier, token.Token);
                var tuple = Space.Get("SlideChange", token.ResponseToken, Identifier, typeof(int));
                var page = tuple.Get<int>(3);
                SlideShower.UpdateSlide(Producer.GetFrames(page).FirstOrDefault());
            }
        }
    }
}
