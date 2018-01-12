using System.Windows.Shapes;

namespace SlideCommunication
{
    public interface ISlideShow
    {
        void UpdateSlide(byte[] image);
        void GrantHostStatus();
        void RevokeHostStatus();
        void GrantControl();
        void RevokeControl();
        void Draw(Shape figure, System.Drawing.Point position);
    }
}