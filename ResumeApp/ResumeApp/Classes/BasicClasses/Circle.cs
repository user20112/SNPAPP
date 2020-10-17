using SkiaSharp;

namespace ResumeApp.Classes
{
    public class Circle
    {
        public SKPoint Center;//  center of the circle
        public double Radius;//  how wide is the circle

        public Circle(double radius, SKPoint center)
        {
            Center = center;
            Radius = radius;
        }
    }
}