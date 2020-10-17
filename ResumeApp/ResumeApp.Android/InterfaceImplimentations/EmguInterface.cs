using NativeFeatures.Droid;
using ResumeApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(Emguinterface))]

namespace NativeFeatures.Droid
{
    public class Emguinterface : IEmgu
    {
        public Emguinterface()
        {
        }
    }
}