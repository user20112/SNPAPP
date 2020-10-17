using ResumeApp.Classes;
using ResumeApp.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage : BaseContentPage
    {
        public VideoPage()
        {
            InitializeComponent();
            Title = "Video";
        }

        private List<Tuple<string, List<VideoType>>> Devices = new List<Tuple<string, List<VideoType>>>();
        private string CurrentCamera = "";

        protected override void OnAppearing()
        {
            base.OnAppearing();
            VideoManager.FrameReady += FrameReady;
            DetectCameraLoop();
        }

        private void FrameReady(object sender, FrameReadyEventArgs e)
        {
            CameraImage.UpdateImage(e.Image);
        }

        public bool AlreadyDetecting = false;

        public void DetectCameraLoop()
        {
            Task.Run(() =>
            {
                if (!AlreadyDetecting)
                {
                    AlreadyDetecting = true;
                    while (OnPage)
                    {
                        List<string> devices = VideoManager.Scan();
                        if (devices.Count > Devices.Count)
                        {
                            foreach (string x in devices)
                            {
                                bool found = false;
                                foreach (Tuple<string, List<VideoType>> y in Devices)
                                {
                                    if (y.Item1 == x)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    AddCamera(x);
                                }
                            }
                        }
                        if (devices.Count < Devices.Count)
                        {
                            for (int x = 0; x < Devices.Count; x++)
                            {
                                bool found = false;
                                int Index = 0;
                                for (int y = 0; y < devices.Count; y++)
                                {
                                    if (devices[y] == Devices[x].Item1)
                                    {
                                        found = true;
                                        break;
                                    }
                                    Index++;
                                }
                                if (!found)
                                    RemoveCamera(devices[Index]);
                                for (int z = 0; z < Devices.Count; z++)
                                    if (Devices[z].Item1 == devices[Index])
                                        Devices.RemoveAt(z);
                            }
                        }
                    }
                    AlreadyDetecting = false;
                }
            });
        }

        private int SelectedRes = 0;
        private int SelectedIndex = 0;

        private void AddCamera(string x)
        {
            Devices.Add(new Tuple<string, List<VideoType>>(x, VideoManager.ScanDevice(x)));
            if (CurrentCamera == "")
            {
                foreach (VideoType videoType in Devices[0].Item2)
                {
                    if (videoType.Encoding.ToLower().Contains("mjpg") || videoType.Encoding.ToLower().Contains("jpg") || videoType.Encoding.ToLower().Contains("jpeg"))
                    {
                        StartCamera(x, videoType, 0);
                        break;
                    }
                }
            }
        }

        private void StartCamera(string Device, VideoType type, int index)
        {
            VideoManager.StopStream();
            CurrentCamera = Device;
            SelectedIndex = index;
            VideoManager.Load(Device, type.FrameRate, type.Height, type.Width, type.Encoding);
            VideoManager.StartStream();
        }

        private void RemoveCamera(string v)
        {
            if (CurrentCamera == v)
            {//disconect from camera
            }
            else
            {
                for (int z = 0; z < Devices.Count; z++)
                    if (Devices[z].Item1 == v)
                        Devices.RemoveAt(z);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void ImagesButton_Clicked(object sender, System.EventArgs e)
        {
        }

        private void CaptureCameraButton_Clicked(object sender, System.EventArgs e)
        {
        }

        private void SwitchResolution_Clicked(object sender, System.EventArgs e)
        {
        }

        private void SwitchCameraButton_Clicked(object sender, System.EventArgs e)
        {
            if (SelectedIndex++ == Devices.Count - 1)
                SelectedIndex = 0;

            foreach (VideoType videoType in Devices[SelectedIndex].Item2)
            {
                if (videoType.Encoding.ToLower().Contains("mjpg") || videoType.Encoding.ToLower().Contains("jpg") || videoType.Encoding.ToLower().Contains("jpeg"))
                {
                    StartCamera(Devices[SelectedIndex].Item1, videoType, SelectedIndex);
                    break;
                }
            }
        }
    }
}