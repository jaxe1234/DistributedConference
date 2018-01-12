using System.Windows.Shapes;

namespace SlideCommunication
{
    public interface ISlideShow
    {
        void GotoSlide(int page);
        void GrantHostStatus();
        void RevokeHostStatus();
        void GrantControl();
        void RevokeControl();
        void Draw(Shape figure, System.Drawing.Point position);
    }
}