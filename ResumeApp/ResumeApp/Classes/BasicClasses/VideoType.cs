namespace ResumeApp.Classes
{
    public class VideoType
    {
        public string FriendlyName;
        public int FrameRate;
        public int Height;
        public int Width;
        public string Encoding;

        public VideoType(string friendlyName, int frameRate, int height, int width, string encoding)
        {
            FriendlyName = friendlyName;
            FrameRate = frameRate;
            Height = height;
            Width = width;
            Encoding = encoding;
        }
    }
}