using ResumeApp.Classes;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovingWebPage : BaseContentPage
    {
        private List<WebNode> WebNodes = new List<WebNode>();
        private Random rand = new Random(DateTime.Now.Millisecond);
        private double MaxSpeed = .1;
        private static int WebSize = 32;
        private SKSurface Surface;
        private static int PixelSize = 640;
        private SKPaint NodePaint;
        private SKPaint LinePaint;
        private static int PixelsPerPoint = PixelSize / WebSize;

        public MovingWebPage()
        {
            InitializeComponent();
            Surface = SKSurface.Create(new SKImageInfo(PixelSize, PixelSize));
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                Task.Run(() =>
                {
                    if (!RequestAdd)
                    {
                        RequestAdd = true;
                        while (Updating)
                            Thread.Sleep(50);

                        for (int x = 0; x < 5; x++)
                        {
                            WebNodes.Add(new WebNode(new Point(rand.NextDouble() * WebSize, rand.NextDouble() * WebSize), new Point(rand.NextDouble() * MaxSpeed, rand.NextDouble() * MaxSpeed * -1)));
                            WebNodes.Add(new WebNode(new Point(rand.NextDouble() * WebSize, rand.NextDouble() * WebSize), new Point(rand.NextDouble() * MaxSpeed * -1, rand.NextDouble() * MaxSpeed * -1)));
                            WebNodes.Add(new WebNode(new Point(rand.NextDouble() * WebSize, rand.NextDouble() * WebSize), new Point(rand.NextDouble() * MaxSpeed * -1, rand.NextDouble() * MaxSpeed)));
                            WebNodes.Add(new WebNode(new Point(rand.NextDouble() * WebSize, rand.NextDouble() * WebSize), new Point(rand.NextDouble() * MaxSpeed, rand.NextDouble() * MaxSpeed)));
                        }
                        RequestAdd = false;
                    }
                });
            };
            WebDisplay.GestureRecognizers.Add(tapGestureRecognizer);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(() => { UpdateWebLoop(); });
        }

        public void UpdateWebLoop()
        {
            while (OnPage)
            {
                try
                {
                    if (WebNodes.Count == 0)
                    {
                        for (int x = 0; x < 30; x++)
                        {
                            WebNodes.Add(new WebNode(new Point(rand.NextDouble() * WebSize, rand.NextDouble() * WebSize), new Point(rand.NextDouble() * MaxSpeed, rand.NextDouble() * MaxSpeed * -1)));
                            WebNodes.Add(new WebNode(new Point(rand.NextDouble() * WebSize, rand.NextDouble() * WebSize), new Point(rand.NextDouble() * MaxSpeed * -1, rand.NextDouble() * MaxSpeed * -1)));
                            WebNodes.Add(new WebNode(new Point(rand.NextDouble() * WebSize, rand.NextDouble() * WebSize), new Point(rand.NextDouble() * MaxSpeed * -1, rand.NextDouble() * MaxSpeed)));
                            WebNodes.Add(new WebNode(new Point(rand.NextDouble() * WebSize, rand.NextDouble() * WebSize), new Point(rand.NextDouble() * MaxSpeed, rand.NextDouble() * MaxSpeed)));
                        }
                    }
                    if (!RequestAdd)
                    {
                        Updating = true;
                        UpdateWeb();
                        UpdateWebImage();
                        Updating = false;
                    }
                }
                catch
                {
                }
            }
        }

        private bool RequestAdd = false;
        private bool Updating = false;

        public void UpdateWebImage()
        {
            SKCanvas canvas = Surface.Canvas;
            Surface.Canvas.Clear(Color.Gray.ToSKColor());
            NodePaint = new SKPaint() { Style = SKPaintStyle.Fill, Color = ((Color)Application.Current.Resources["TraceColor"]).ToSKColor() };
            LinePaint = new SKPaint() { Style = SKPaintStyle.Stroke, Color = ((Color)Application.Current.Resources["TraceColor"]).ToSKColor() };
            Task[] tasks = new Task[WebNodes.Count];
            int x = 0;
            foreach (WebNode node in WebNodes)
            {
                tasks[x++] = Task.Run(() =>
                {
                    canvas.DrawCircle((float)node.Position.X * PixelsPerPoint, (float)node.Position.Y * PixelsPerPoint, 4, NodePaint);
                    foreach (WebNode node2 in WebNodes)
                    {
                        double DistanceSquared = Math.Abs(node.Position.Y - node2.Position.Y) * Math.Abs(node.Position.Y - node2.Position.Y) + Math.Abs(node.Position.X - node2.Position.X) * Math.Abs(node.Position.X - node2.Position.X);
                        if (DistanceSquared < LinkLength.Value * LinkLength.Value)
                        {
                            canvas.DrawLine((float)node.Position.X * PixelsPerPoint, (float)node.Position.Y * PixelsPerPoint, (float)node2.Position.X * PixelsPerPoint, (float)node2.Position.Y * PixelsPerPoint, LinePaint);
                        }
                    }
                });
            }
            Task.WaitAll(tasks);
            WebDisplay.UpdateImage(Surface.Snapshot());
        }

        public void UpdateWeb()
        {
            foreach (WebNode node in WebNodes)
            {
                node.Position.X += node.Velocity.X * NodeSpeed.Value;
                node.Position.Y += node.Velocity.Y * NodeSpeed.Value;
                if (node.Position.X < 0)
                    node.Position.X = WebSize;
                if (node.Position.X > WebSize)
                    node.Position.X = 0;
                if (node.Position.Y < 0)
                    node.Position.Y = WebSize;
                if (node.Position.Y > WebSize)
                    node.Position.Y = 0;
            }
        }

        private void LinkLength_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / 1);
            LinkLength.Value = newStep * 1;
            LinkLabel.Text = (newStep * 1).ToString();
        }

        private void NodeSpeed_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / 1);
            NodeSpeed.Value = newStep * 1;
            SpeedLabel.Text = (newStep * 1).ToString();
        }
    }
}