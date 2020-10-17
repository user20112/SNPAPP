using SkiaSharp;

namespace ResumeApp.Interfaces
{
    public interface IPlatformSpecific
    {
        bool SupportsLocation { get; }
        int DPI { get; set; }
        int IconHeight { get; set; }
        int IconWidth { get; set; }
        bool Location { get; set; }
        string Platform { get; set; }
        int ScreenHeight { get; set; }
        int ScreenWidth { get; set; }

        bool CheckCameraPermission();

        void RequestCameraPermission();

        string GatherThroughPopupOkCancel(string message, string title, string HintText, string DefaultText);

        string GetAppVersion();

        string GetBuildVersion();

        string GetDeviceVersion();

        string GetLat();

        string GetLong();

        string GetSSID();

        bool IsBluetoothOn();

        bool IsLocationOn();

        void PlacePhoneCall(string number);

        bool PopupBool(string message, string title, string PositiveButtonText, string NegativeButtonText);

        string PopupTrinary(string message, string title, string[] Buttons);

        void RequestBluetoothPermission();

        void RequestLocationPermission();

        byte[] ResizeImage(byte[] imageData, float width, float height);

        void SendTextEmail(string Topic, string data, string recipient);

        void ShareFile(string path);

        void ShowToast(SKRect rectangleToDraw, string message);

        byte[] SKImageToByte(SKImage Image);
    }
}