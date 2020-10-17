using ResumeApp.Interfaces;
using ResumeApp.WPF.InterfaceImplimentations;

[assembly: Xamarin.Forms.Dependency(typeof(EmguInterface))]

namespace ResumeApp.WPF.InterfaceImplimentations
{
    public class EmguInterface : IEmgu
    {
    }
}