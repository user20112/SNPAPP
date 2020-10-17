using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.DataStorage;
using ResumeApp.Interfaces;
using SkiaSharp;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ResumeApp.Classes
{
    public class BaseContentPage : ContentPage
    {
        public bool OnPage = false;
        public ColorCode SelectedColorCode;
        public IEmgu EmguInterface = DependencyService.Get<IEmgu>();
        public IFileSave FileSaveInterface = DependencyService.Get<IFileSave>();
        public IHIDDevice HIDDeviceInterface = DependencyService.Get<IHIDDevice>();
        public IPlatformSpecific PlatformSpecificInterface = DependencyService.Get<IPlatformSpecific>();
        public ISerialCom PlatformSpecificSerialManager = DependencyService.Get<ISerialCom>();
        public ISettingsManager SettingsManagerInterface = DependencyService.Get<ISettingsManager>();
        public IAdapter Adapter = DependencyService.Get<IAdapter>();
        public IVideoSource VideoManager = DependencyService.Get<IVideoSource>();
        public IDataStorage DataStorage;
        public IWifiManager WifiManager = DependencyService.Get<IWifiManager>();
        public SKRect toastRectangle;
        public bool NavigationButtonAlreadyClicked = false;
        private bool isLoaded = false;
        public string SelectedTheme = "";
        public SynchronizationContext Main = SynchronizationContext.Current;
        public bool SupportsWifi { get { return WifiManager != null; } }
        public bool SupportsBLE { get { return Adapter != null; } }
        public bool SupportsHID { get { return HIDDeviceInterface != null; } }
        public bool SupportsSerial { get { return PlatformSpecificSerialManager != null; } }
        public bool SupportsEmgu { get { return EmguInterface != null; } }
        public bool SupportsVideo { get { return VideoManager != null; } }
        public bool SupportsLocation { get { return PlatformSpecificInterface.SupportsLocation; } }

        public virtual void BackButtonPressed()
        {
        }

        public BaseContentPage()
        {
            NavigationPage.SetHasNavigationBar(this, true);
            SelectedTheme = SettingsManagerInterface.GetString("SelectedString", "Blue");
            switch (SelectedTheme)
            {
                case "Blue":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["LightBlue"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["Blue"];
                    break;

                case "Orange":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Orange"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkOrange"];
                    break;

                case "GrayScale":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Black"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["Black"];
                    break;
            }
        }

        public void NavigateBack()
        {
            if (!NavigationButtonAlreadyClicked)
            {
                NavigationButtonAlreadyClicked = true;
                Navigation.PopAsync();
            }
        }

        public async void NavigateTo(ContentPage page)
        {
            if (!NavigationButtonAlreadyClicked)
            {
                NavigationButtonAlreadyClicked = true;
                await Navigation.PushAsync(page);
            }
        }

        public virtual void OnBackButton()
        {
        }

        public virtual void OnReAppearing()
        {
        }

        public virtual void OnFirstAppearing()
        {
        }

        public void ShowToast(string message)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                PlatformSpecificInterface.ShowToast(toastRectangle, message);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            OnPage = true;
            PositionInNavStack = Navigation.NavigationStack.IndexOf(this);
            DataStorage = new Json(FileSaveInterface, SettingsManagerInterface, true);
            NavigationButtonAlreadyClicked = false;
            toastRectangle = new SKRect((float)Application.Current.MainPage.Width / 4, (float)Application.Current.MainPage.Height / 28 * 22, (float)Application.Current.MainPage.Width / 4 * 3, (float)Application.Current.MainPage.Height / 28 * 27);
            SelectedTheme = SettingsManagerInterface.GetString("SelectedString", "Blue");
            switch (SelectedTheme)
            {
                case "Blue":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["LightBlue"];
                    break;

                case "Orange":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Orange"];
                    break;

                case "GrayScale":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Black"];
                    break;
            }
            if (!isLoaded)
            {
                isLoaded = true;
                OnFirstAppearing();
            }
            else
            {
                OnReAppearing();
            }
        }

        private int PositionInNavStack = 0;

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            OnPage = false;
            if (PositionInNavStack == Navigation.NavigationStack.Count - 1)
            {
                BackButtonPressed();
            }
        }
    }
}