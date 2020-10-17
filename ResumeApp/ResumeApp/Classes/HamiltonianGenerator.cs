using System;
using Xamarin.Forms;

namespace ResumeApp.Classes
{
    public class HamiltonianGenerator
    {
        public int[] Path;
        private int Height;
        private int Width;
        private Random random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Generate a hamiltonian cycle for the height and width given
        /// </summary>
        /// <param name="height">Must be even and atleast 2</param>
        /// <param name="width">Must be even adn atleast 2</param>
        public HamiltonianGenerator(int height, int width, bool Simple)
        {
            Height = height;
            Width = width;
            Path = new int[height * width];
            Generate(Simple);
        }

        private void Generate(bool Simple)
        {
            int CurPos = 0;
            int CurRow = 0;
            int CurColumn = 0;
            bool OddWidth = Width % 2 != 0;
            bool OddHeight = Height % 2 != 0;
            Path[CurColumn] = CurPos++;
            bool GoingLeft = true;
            while (CurRow < Height)
            {
                if (GoingLeft)
                {
                    CurColumn = 1;
                    while (CurColumn < Width)
                    {
                        Path[CurColumn++ + CurRow * Width] = CurPos++;
                    }
                }
                else
                {
                    CurColumn = Width - 1;
                    while (CurColumn > 0)
                    {
                        Path[CurColumn-- + CurRow * Width] = CurPos++;
                    }
                }
                GoingLeft = !GoingLeft;//turn around each loop
                CurRow++;
            }
            for (int y = Height - 1; y > 0; y--)
            {
                Path[y * Width] = CurPos++;
            }
            if (!Simple)
                Randomize();
        }

        private void Randomize()
        {
            //at this point we should have the following we want it to look a bit more randomized;
            /*
             0---|
             ||---
             |---|
             ||---
             |---|
             |----
             */
            if (Width >= 6 && Height >= 6)
            {//  if its less than 6 just let it be zig zag its to small for me to care
                int NumberOfRandomizations = Width + Height / 2;
                while (NumberOfRandomizations > 0)
                {
                    int PickedWidth = (int)(random.NextDouble() * Width - 3) + 1;
                    int PickedHeight = (int)(random.NextDouble() * Height - 3) + 1;
                    Point JointPoint1 = new Point(PickedWidth, PickedHeight);
                    Point JointPoint2 = new Point(PickedWidth + 1, PickedHeight);
                    Point JointPoint3 = new Point(PickedWidth, PickedHeight + 1);
                    Point JointPoint4 = new Point(PickedWidth + 1, PickedHeight + 1);
                    ConnectPoints(JointPoint1, JointPoint2);
                    ConnectPoints(JointPoint3, JointPoint4);
                    PickedWidth = (int)(random.NextDouble() * Width - 3) + 1;
                    PickedHeight = (int)(random.NextDouble() * Height - 3) + 1;
                    Point JointPoint5 = new Point(PickedWidth, PickedHeight);
                    Point JointPoint6 = new Point(PickedWidth, PickedHeight);
                    Point JointPoint7 = new Point(PickedWidth, PickedHeight);
                    Point JointPoint8 = new Point(PickedWidth, PickedHeight);
                }
            }
        }

        private void ConnectPoints(Point Point1, Point Point2)
        {
            int StartIndex = GetIndex(Point1.X, Point2.Y);
            Point Next = GetNextStep(Point1);
            while (GetIndex(Next) != 0)
            {
            }
        }

        public Point GetNextStep(int index)
        {
            index++;
            if (index == Width * Height) return new Point(0, 0);
            for (int x = 0; x < Width * Height; x++)
            {
                if (Path[x] == index)
                    return new Point(x % Width, x / Width);
            }
            return new Point(-1, -1);
        }

        public Point GetNextStep(int x, int y)
        {
            int Index = GetIndex(x, y) + 1;
            if (Index == Width * Height - 1) return new Point(0, 0);//if it is the last index return the first.
            if (Index == GetIndex(x + 1, y)) new Point(x + 1, y);
            if (Index == GetIndex(x, y + 1)) new Point(x, y + 1);
            if (Index == GetIndex(x, y - 1)) new Point(x, y - 1);
            if (Index == GetIndex(x - 1, y)) new Point(x - 1, y);
            return new Point(-1, -1);
        }

        public Point GetNextStep(Point point)
        {
            int Index = GetIndex(point.X, point.Y) + 1;
            if (Index == Width * Height - 1) return new Point(0, 0);//if it is the last index return the first.
            if (Index == GetIndex(point.X + 1, point.Y)) new Point(point.X + 1, point.Y);
            if (Index == GetIndex(point.X, point.Y + 1)) new Point(point.X, point.Y + 1);
            if (Index == GetIndex(point.X, point.Y - 1)) new Point(point.X, point.Y - 1);
            if (Index == GetIndex(point.X - 1, point.Y)) new Point(point.X - 1, point.Y);
            return new Point(-1, -1);
        }

        public int GetIndex(int x, int y)
        {
            try
            {
                return Path[x + Width * y];
            }
            catch
            {
                return -1;
            }
        }

        public int GetIndex(double x, double y)
        {
            try
            {
                return Path[(int)x + Width * (int)y];
            }
            catch
            {
                return -1;
            }
        }

        public int GetIndex(Point point)
        {
            try
            {
                return Path[(int)point.X + Width * (int)point.Y];
            }
            catch
            {
                return -1;
            }
        }
    }
}