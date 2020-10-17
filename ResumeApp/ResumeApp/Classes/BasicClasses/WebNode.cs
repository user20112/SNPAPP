using Xamarin.Forms;

namespace ResumeApp.Classes
{
    public class WebNode
    {
        public Point Position;
        public Point Velocity;

        public WebNode(Point position, Point velocity)
        {
            Position = position;
            Velocity = velocity;
        }
    }
}