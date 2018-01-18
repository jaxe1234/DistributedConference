using System.Windows.Shapes;

namespace SlideCommunication
{
    public interface ISlideShow
    {
        void UpdateSlide(FramePayload payload);
        bool InControl { get; set; }
        bool IsHost { get; set; }
        void NewCollection(int pages);
        void Draw(Shape figure, System.Drawing.Point position);
    }
}