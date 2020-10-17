using ResumeApp.Classes;
using ResumeApp.Classes.Devices;
using ResumeApp.Interfaces;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnalysisPage : BaseContentPage
    {
        private Scope scope;

        public AnalysisPage()
        {
            InitializeComponent();
            Title = "Analysis";
            List<string> coms = PlatformSpecificSerialManager.Scan();
            PlatformSpecificSerialManager.Load(coms[0]);
            scope = new SNPScope(new SerialComInterface(PlatformSpecificSerialManager));
        }
    }
}