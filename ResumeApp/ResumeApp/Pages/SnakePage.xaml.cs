using ResumeApp.Classes;
using ResumeApp.Classes.Enums;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SnakePage : BaseContentPage
    {
        private int PixelSize = 640;
        private int size = 32;
        private HamiltonianGenerator HamiltonianGenerator;
        private Point SnakeHead;
        private SKSurface Surface;
        private SKPaint BackgroundPaint = new SKPaint() { Style = SKPaintStyle.Fill, Color = Color.Gray.ToSKColor() };
        private SKPaint BorderPaint;
        private SKPaint SnakePaint = new SKPaint() { Style = SKPaintStyle.Fill, Color = Color.Green.ToSKColor() };
        private SKPaint ApplePaint = new SKPaint() { Style = SKPaintStyle.Fill, Color = Color.Red.ToSKColor() };
        private List<Point> SnakePoints;
        private Point ApplePoint;
        private int ToIncrease;
        private Random rand = new Random(DateTime.Now.Millisecond);
        private int IncreaseOnApple = 3;

        public SnakePage()
        {
            InitializeComponent();
            BorderPaint = new SKPaint() { Style = SKPaintStyle.Stroke, Color = ((Color)Application.Current.Resources["TraceColor"]).ToSKColor() };
            ApplePoint = new Point((int)(size / 2), (int)(size / 2));
            HamiltonianGenerator = new HamiltonianGenerator(size, size, true);
            Surface = SKSurface.Create(new SKImageInfo(PixelSize, PixelSize));
            ToIncrease = 5;
            SnakeHead = new Point(0, 0);
            SnakePoints = new List<Point>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(() => { SnakeUpdateLoop(); });
        }

        private bool Updateing = false;

        public void SnakeUpdateLoop()
        {
            while (OnPage)
            {
                if (SnakeHead.X < 0 || SnakeHead.X > size || SnakeHead.Y < 0 || SnakeHead.Y > size)
                {
                    UpdateButton_Clicked(null, null);
                }
                if (!Updateing)
                {
                    try
                    {
                        if (ApplePoint == SnakeHead)
                        {
                            ApplePoint = new Point(-1, -1);
                            while (ApplePoint.X == -1)
                            {
                                int x = (int)(rand.NextDouble() * size);
                                int y = (int)(rand.NextDouble() * size);
                                bool found = false;
                                foreach (Point point in SnakePoints)
                                    if (point.X == x && point.Y == y)
                                    {
                                        found = true;
                                        break;
                                    }
                                if (!found)
                                    ApplePoint = new Point(x, y);
                            }
                            ToIncrease += IncreaseOnApple;
                        }
                        DirectionEnum NextStep = DirectionEnum.Down;
                        int CurIndex = HamiltonianGenerator.GetIndex(SnakeHead);
                        List<Tuple<int, bool, DirectionEnum>> options = new List<Tuple<int, bool, DirectionEnum>>();
                        options.Add(new Tuple<int, bool, DirectionEnum>(HamiltonianGenerator.GetIndex(SnakeHead.X - 1, SnakeHead.Y), IndexBetweanContainsAppleOrTail(SnakeHead, new Point(SnakeHead.X - 1, SnakeHead.Y)), DirectionEnum.Left));
                        options.Add(new Tuple<int, bool, DirectionEnum>(HamiltonianGenerator.GetIndex(SnakeHead.X + 1, SnakeHead.Y), IndexBetweanContainsAppleOrTail(SnakeHead, new Point(SnakeHead.X + 1, SnakeHead.Y)), DirectionEnum.Right));
                        options.Add(new Tuple<int, bool, DirectionEnum>(HamiltonianGenerator.GetIndex(SnakeHead.X, SnakeHead.Y + 1), IndexBetweanContainsAppleOrTail(SnakeHead, new Point(SnakeHead.X, SnakeHead.Y + 1)), DirectionEnum.Down));
                        options.Add(new Tuple<int, bool, DirectionEnum>(HamiltonianGenerator.GetIndex(SnakeHead.X, SnakeHead.Y - 1), IndexBetweanContainsAppleOrTail(SnakeHead, new Point(SnakeHead.X, SnakeHead.Y - 1)), DirectionEnum.Up));
                        options = options.OrderByDescending(x => x.Item1).ToList();
                        bool Chosen = false;
                        int X = 0;
                        if (CurIndex != size * size - 1)
                        {
                            while (!Chosen)
                            {
                                if (options[X].Item1 > CurIndex && !options[X].Item2)
                                {
                                    NextStep = options[X].Item3;
                                    Chosen = true;
                                    break;
                                }
                                X++;
                            }
                        }
                        else
                            NextStep = DirectionEnum.Up;
                        UpdateSnakePosition(NextStep);
                        UpdateSnakeImage();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void UpdateSnakePosition(DirectionEnum DirectionToWalk)
        {
            if (ToIncrease > 0)
                ToIncrease--;
            else
                SnakePoints.RemoveAt(0);
            SnakePoints.Add(SnakeHead);
            switch (DirectionToWalk)
            {
                case DirectionEnum.Down: SnakeHead = new Point(SnakeHead.X, SnakeHead.Y + 1); break;
                case DirectionEnum.Up: SnakeHead = new Point(SnakeHead.X, SnakeHead.Y - 1); break;
                case DirectionEnum.Left: SnakeHead = new Point(SnakeHead.X - 1, SnakeHead.Y); break;
                case DirectionEnum.Right: SnakeHead = new Point(SnakeHead.X + 1, SnakeHead.Y); break;
            }
        }

        public bool IndexBetweanContainsAppleOrTail(Point point1, Point point2)
        {
            int StartIndex = HamiltonianGenerator.GetIndex(point1);
            int EndIndex = HamiltonianGenerator.GetIndex(point2);
            if (EndIndex == StartIndex + 1)
                return false;
            int CurIndex = StartIndex;
            bool HitTaleOrApple = false;
            while (CurIndex < EndIndex + IncreaseOnApple)
            {
                Point point = HamiltonianGenerator.GetNextStep(CurIndex++);
                foreach (Point snakepoint in SnakePoints)
                    if (snakepoint.X == point.X && snakepoint.Y == point.Y)
                    {
                        HitTaleOrApple = true;
                        break;
                    }
                if (point.X == ApplePoint.X && point.Y == ApplePoint.Y)
                    HitTaleOrApple = true;
                if (HitTaleOrApple)
                    break;
            }
            return HitTaleOrApple;
        }

        public void UpdateSnakeImage()
        {
            BorderPaint = new SKPaint() { Style = SKPaintStyle.Stroke, Color = ((Color)Application.Current.Resources["TraceColor"]).ToSKColor() };
            SKCanvas canvas = Surface.Canvas;
            canvas.Clear(Color.Green.ToSKColor());
            int PixelsPerSize = PixelSize / size;
            canvas.DrawRect(new SKRect(0, 0, PixelSize, PixelSize), BackgroundPaint);
            canvas.DrawRect(new SKRect((float)(SnakeHead.X * PixelsPerSize + 2), (float)(SnakeHead.Y * PixelsPerSize + 2), (float)(SnakeHead.X * PixelsPerSize + PixelsPerSize) - 2, (float)(SnakeHead.Y * PixelsPerSize + PixelsPerSize) - 2), SnakePaint);
            canvas.DrawRect(new SKRect((float)(SnakeHead.X * PixelsPerSize), (float)(SnakeHead.Y * PixelsPerSize), (float)(SnakeHead.X * PixelsPerSize + PixelsPerSize), (float)(SnakeHead.Y * PixelsPerSize + PixelsPerSize)), BorderPaint);
            foreach (Point SnakePoint in SnakePoints)
            {
                canvas.DrawRect(new SKRect((float)(SnakePoint.X * PixelsPerSize + 2), (float)(SnakePoint.Y * PixelsPerSize + 2), (float)(SnakePoint.X * PixelsPerSize + PixelsPerSize) - 2, (float)(SnakePoint.Y * PixelsPerSize + PixelsPerSize) - 2), SnakePaint);
                canvas.DrawRect(new SKRect((float)(SnakePoint.X * PixelsPerSize), (float)(SnakePoint.Y * PixelsPerSize), (float)(SnakePoint.X * PixelsPerSize + PixelsPerSize), (float)(SnakePoint.Y * PixelsPerSize + PixelsPerSize)), BorderPaint);
            }
            canvas.DrawRect(new SKRect((float)(ApplePoint.X * PixelsPerSize + 2), (float)(ApplePoint.Y * PixelsPerSize + 2), (float)(ApplePoint.X * PixelsPerSize + PixelsPerSize) - 2, (float)(ApplePoint.Y * PixelsPerSize + PixelsPerSize) - 2), ApplePaint);
            canvas.DrawRect(new SKRect((float)(ApplePoint.X * PixelsPerSize), (float)(ApplePoint.Y * PixelsPerSize), (float)(ApplePoint.X * PixelsPerSize + PixelsPerSize), (float)(ApplePoint.Y * PixelsPerSize + PixelsPerSize)), BorderPaint);
            SnakeDisplay.UpdateImage(Surface.Snapshot());
        }

        private void UpdateButton_Clicked(object sender, System.EventArgs e)
        {
            Updateing = true;
            size = (int)SizeSlider.Value;
            HamiltonianGenerator = new HamiltonianGenerator(size, size, true);
            ApplePoint = new Point((int)(size / 2), (int)(size / 2));
            ToIncrease = 5;
            SnakeHead = new Point(0, 0);
            SnakePoints = new List<Point>();
            Updateing = false;
        }

        private void SizeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / 2);
            SizeSlider.Value = newStep * 2;
            SizeLabel.Text = (newStep * 2).ToString();
        }
    }
}