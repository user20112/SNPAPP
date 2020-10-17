using Q42.HueApi;
using Q42.HueApi.Interfaces;
using ResumeApp.Classes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HuePage : BaseContentPage
    {
        private IEnumerable<Q42.HueApi.Models.Bridge.LocatedBridge> Bridges;
        private ILocalHueClient Client;
        private IEnumerable<Light> Lights;
        private const string HueKey = "GPogpO2djadqFKcSUALsuC1Y3rHkOLz8ckVuRyv-";
        private ResumeApp.ViewModels.HueViewModel VM;

        public HuePage()
        {
            InitializeComponent();
            VM = BindingContext as ResumeApp.ViewModels.HueViewModel;
            InitializeHue();
        }

        public async void InitializeHue()
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            Bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            Client = new LocalHueClient("192.168.0.10");
            Client.Initialize(HueKey);
            Lights = await Client.GetLightsAsync();
            foreach (Light light in Lights)
                VM.LightsPickerSource.Add(light.Name);
            LightsPicker.SelectedIndex = 0;
        }

        public async void SetLight(List<string> LightNames, bool On, byte r = 255, byte g = 255, byte b = 255, byte Brightness = 255)
        {
            for (int x = 0; x < LightNames.Count; x++)
            {
                foreach (Light light in Lights)
                {
                    if (light.Name == LightNames[x])
                        LightNames[x] = light.Id;
                }
            }
            LightCommand command = new LightCommand() { On = On, Brightness = Brightness };
            Tuple<double, double> Color = ConvertRGBToXY(r, g, b);
            command.SetColor(Color.Item1, Color.Item2);
            await Client.SendCommandAsync(command, LightNames);
        }

        public Tuple<double, double> ConvertRGBToXY(byte r, byte g, byte b)
        {
            //Apply a gamma correction to the RGB values, which makes the color more vivid and more the like the color displayed on the screen of your device
            double R = (r > 0.04045) ? Math.Pow((r + 0.055) / (1.0 + 0.055), 2.4) : (r / 12.92);
            double G = (g > 0.04045) ? Math.Pow((g + 0.055) / (1.0 + 0.055), 2.4) : (g / 12.92);
            double B = (b > 0.04045) ? Math.Pow((b + 0.055) / (1.0 + 0.055), 2.4) : (b / 12.92);
            //RGB values to XYZ using the Wide RGB D65 conversion formula
            var X = R * 0.664511 + G * 0.154324 + B * 0.162028;
            var Y = R * 0.283881 + G * 0.668433 + B * 0.047685;
            var Z = R * 0.000088 + G * 0.072310 + B * 0.986039;
            //Calculate the xy values from the XYZ values
            var x = (X / (X + Y + Z));
            var y = (Y / (X + Y + Z));
            if (x == double.NaN)
                x = 0;
            if (y == double.NaN)
                x = 0;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            return new Tuple<double, double>(x, y);
        }

        private void OnButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(() =>
                {
                    SetLight(new List<string> { LightsPicker.SelectedItem as string }, true, (byte)RSlider.Value, (byte)GSlider.Value, (byte)BSlider.Value);
                });
        }

        private void OffButton_Clicked(object sender, EventArgs e)
        {
            SetLight(new List<string> { LightsPicker.SelectedItem as string }, false);
        }

        private bool Running = false;
        private bool Incrementing = true;
        private int LastR = 80;

        private void RunTestProgram_Clicked(object sender, EventArgs e)
        {
            Running = true;
            Task.Run(() =>
            {
                while (Running)
                {
                    if (Incrementing)
                    {
                        LastR += 2;
                        SetLight(new List<string> { LightsPicker.SelectedItem as string }, true, (byte)LastR, 25, 50);
                        if (LastR > 130)
                            Incrementing = !Incrementing;
                    }
                    else
                    {
                        LastR -= 2;
                        SetLight(new List<string> { LightsPicker.SelectedItem as string }, true, (byte)LastR, 25, 50);
                        if (LastR < 60)
                            Incrementing = !Incrementing;
                    }
                    Thread.Sleep(100);
                }
            });
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Running = false;
        }
        private void StopTestProgram_Clicked(object sender, EventArgs e)
        {
            Running = false;
        }
    }
}