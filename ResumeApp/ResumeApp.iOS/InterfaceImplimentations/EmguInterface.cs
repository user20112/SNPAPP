using ResumeApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(NativeFeatures.iOS.Emguinterface))]

namespace NativeFeatures.iOS
{
    public class Emguinterface : IEmgu
    {
        public Emguinterface()
        {
        }
    }
}