using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace ResumeApp.GTK
{
    internal class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var dllDirectory = @"C:\Program Files (x86)\Mono\bin";
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory);
            Run();
        }

        private static void Run()
        {
            Gtk.Application.Init();
            Forms.Init();
            var app = new App();
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("");
            window.Show();
            Gtk.Application.Run();
        }
    }
}