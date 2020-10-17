using ResumeApp.Classes;
using ResumeApp.CustomElements;
using ResumeApp.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ResumeApp.Pages
{
    public partial class MainPage : BaseContentPage
    {
        private DynamicListView ProjectsListView = new DynamicListView();
        private HorizontalListView CapabilityListView = new HorizontalListView();
        public static readonly List<HomePageItem> HomePageItems = new List<HomePageItem>();
        private int x = 0;
        private bool TestingThemes = false;

        public void UpdateThemeTest()
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                switch (x++)
                {
                    case 0:
                        Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["LightBlue"];
                        Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["Blue"];
                        ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.BlueIcon)));
                        break;

                    case 1:
                        Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Orange"];
                        Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkOrange"];
                        ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.OrangeIcon)));
                        break;

                    case 2:
                        Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Black"];
                        Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["Black"];
                        ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.NoirIcon)));
                        break;

                    case 3:
                        Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Pink"];
                        Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkPink"];
                        ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.NeonIcon)));
                        break;

                    case 4:
                        Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Green"];
                        Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkGreen"];
                        ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.GreenIcon)));
                        break;

                    case 5:
                        Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Red"];
                        Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkRed"];
                        ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.RedIcon)));
                        break;

                    default:
                        x = 0;
                        break;
                }
            });
        }

        public MainPage() : base()
        {
            InitializeComponent();
            Title = "Home";
            CapabilityListView.ItemWidth = 75;
            CapabilityListView.ItemHeight = 90;
            ProjectsListView.Columns = 3;
            ProjectsListView.HorizontalOptions = LayoutOptions.Center;
            ProjectsListView.ItemHeight = 90;
            ProjectsListView.ItemWidth = 75;
            HomePageItems.Add(new HomePageItem(Base64Resources.SerialIcon, "Serial", "SerialPage", Color.Gray, typeof(SerialPage), "Shows basic Serial comunication on Android WPF and UWP. Not all android phones support Serial USB Devices in which case it will be unable to find any plugged in. IOS is not supported due to the cost in hardware and software leading it to just not be feasible for a small project like this."));
            HomePageItems.Add(new HomePageItem(Base64Resources.USBIcon, "USB", "USBPage", Color.Gray, typeof(USBPage), "Shows HID Read and Write on Android UWP and WPF. IOS is not supported on this due to the cost of getting ios usb approval and Usb hardware requireing apple approved chips"));
            HomePageItems.Add(new HomePageItem(Base64Resources.WifiIcon, "Wifi", "WifiPage", Color.Gray, typeof(WifiPage), ""));
            HomePageItems.Add(new HomePageItem(Base64Resources.BluetoothIcon, "BLE", "BLEPage", Color.Gray, typeof(BLEPage), "Scans for Bluetooth devices and allowing enumerating Services and characteristics as well as allow BLE Read Write operations. Supported on UWP Android And IOS WPF i couldnt find a good BLE api and it realisticly would be covered by UWP"));
            HomePageItems.Add(new HomePageItem(Base64Resources.EmguIcon, "Emgu", "EmguPage", Color.Gray, typeof(EmguPage), "Shows some basic Shape Detection in Emgu and some other Image processing useing Platform specific emgu implimentations and a SkiaSharp Abstraction for forms."));
            HomePageItems.Add(new HomePageItem(Base64Resources.OPMIcon, "OPM", "OPMPage", Color.Gray, typeof(OPMPage), "A pretty similar page to the ODM OPM Page i designed the plan is to use this for my own OPM build on the EPS32 microcontroller when i finish it. Current Hangup on that is the hardware defect causing code deploys to fail ( i need to solder or hook up a 10 uf capacitor on the enable pin)."));
            HomePageItems.Add(new HomePageItem(Base64Resources.AnalysisIcon, "Analysis", "AnalysisPage", Color.Gray, typeof(AnalysisPage), "Analyzes a fiber using the SNP Video Scope"));
            HomePageItems.Add(new HomePageItem(Base64Resources.VideoIcon, "Video", "VideoPage", Color.Gray, typeof(VideoPage), "Streams from a camera on UWP WPF IOS and Android. Android Uses the Camera 2 API WPF uses Windows Media Foundation or Directshow UWP uses -- and IOS uses --"));
            HomePageItems.Add(new HomePageItem(Base64Resources.SnakeIcon, "Snake", "SnakePage", Color.Gray, typeof(SnakePage), "Hamiltonian Snake AI! can complete snake perfectly by generateing a hamiltonian cycle (Path through each block exactly once ending where it started) and pruneing branches that dont contain the apple or a piece of the snake ( plus 3 nodes as otherwise when it grows it could run into itself on picking up an apple.)"));
            HomePageItems.Add(new HomePageItem(Base64Resources.ScheduleIcon, "Schedule", "SchedulePage", Color.Gray, typeof(SchedulePage), "a basic Shedule Manager that uses SQLLite to store it on the device. This is used as a Use project for the SQL Capability page."));
            HomePageItems.Add(new HomePageItem(Base64Resources.SQLLiteIcon, "SQLLite", "SQLLitePage", Color.Gray, typeof(SQLLitePage), "Shows the ability to store data in a SQL Lite database on UWP WPF Android and IOS."));
            HomePageItems.Add(new HomePageItem(GrabSNPIcon(), "SNPMonitor", "SNPMonitorPage", Color.Gray, typeof(SNPMonitorPage), "Shows a simulation of the SNP Project i made for Osram Sylvania As well as a description of the reason for the application and how the implimentation was done."));
            HomePageItems.Add(new HomePageItem(Base64Resources.MovingWebIcon, "MovingWeb", "MovingWebPage", Color.Gray, typeof(MovingWebPage), "Shows a bunch of nodes moving in random directions. Nodes are connected when close. this length can be adjusted and you can add more nodes by tapping the canvas."));
            HomePageItems.Add(new HomePageItem(Base64Resources.ActiveMQIcon, "ActiveMQ", "ActiveMQPage", Color.Gray, typeof(ActiveMQPage), "Shows ActiveMQ Capability by sending MQTT Stomp and OpenWire communications to the ActiveMQ server hosted on devlinpaddock.online. This server is also used by the Local Chat project."));
            HomePageItems.Add(new HomePageItem(Base64Resources.LocalChat, "Local Chat", "LocalChatPage", Color.Gray, typeof(LocalChatPage), "Uses ActiveMQ and GPS Data to deterime zip code and publish messages to a topic named for the zipcode. any other devices with the app open within that zip code will receive that topic message creating a sort of Local Chat"));
            HomePageItems.Add(new HomePageItem(Base64Resources.Twitter, "Twitter", "TwitterPage", Color.Gray, typeof(TwitterPage), "Usses the TweetSharp API to publish messages to my twitter account."));
            HomePageItems.Add(new HomePageItem(Base64Resources.Hue, "Hue", "HuePage", Color.Gray, typeof(HuePage), "Uses Q42.HueApi to control hue lights in my house."));
            if (SupportsSerial) AddCapability(HomePageItems[0]);//SerialPage
            if (SupportsHID) AddCapability(HomePageItems[1]);//USBPage
            if (SupportsWifi) AddCapability(HomePageItems[2]);//WifiPage
            if (SupportsBLE) AddCapability(HomePageItems[3]);//BLEPage
            //if (SupportsEmgu) AddCapability(HomePageItems[4]);//EmguPage
            AddMisc(HomePageItems[5]);//OPMPAge
            //AddMisc(HomePageItems[6]);//AnalysisPage
            if (SupportsVideo) AddCapability(HomePageItems[7]);//VideoPage
            AddMisc(HomePageItems[8]);//SnakePage
            //AddMisc(HomePageItems[9]);//SchedulePage
            //AddMisc(HomePageItems[10]);//SQLLite
            //AddMisc(HomePageItems[11]);//SNPMonitor
            AddMisc(HomePageItems[12]);//MovingWeb
            AddCapability(HomePageItems[13]);//ActiveMQ
            //AddMisc(HomePageItems[14]);//LocalChat
            AddCapability(HomePageItems[15]);//Twitter
            AddCapability(HomePageItems[16]);//HuePage
            ProjectsFrame.Content = ProjectsListView;
            CapabilityFrame.Content = CapabilityListView;
            UpdateTheme();
            if (TestingThemes)
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(4000);
                        Console.WriteLine(x);
                        UpdateThemeTest();
                    }
                });
        }

        private void UpdateTheme()
        {
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

                case "Neon":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Pink"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkPink"];
                    break;

                case "Green":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Green"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkGreen"];
                    break;

                case "Red":
                    Application.Current.Resources["TraceColor"] = (Color)Application.Current.Resources["Red"];
                    Application.Current.Resources["NavigationPrimary"] = (Color)Application.Current.Resources["DarkRed"];
                    break;
            }
        }

        private string GrabSNPIcon()
        {
            switch (SelectedTheme)
            {
                default:
                    return Base64Resources.BlueIcon;

                case "Orange":
                    return Base64Resources.OrangeIcon;

                case "GrayScale":
                    return Base64Resources.NoirIcon;

                case "Green":
                    return Base64Resources.GreenIcon;

                case "Neon":
                    return Base64Resources.NeonIcon;

                case "Red":
                    return Base64Resources.RedIcon;
            }
        }

        private void AddCapability(HomePageItem item)
        {
            CapabilityListView.AddView(GenerateLayoutFromHomepageItem(item));
        }

        private void AddMisc(HomePageItem item)
        {
            ProjectsListView.AddView(GenerateLayoutFromHomepageItem(item));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SettingsButton.IconImageSource = ImageSource.FromStream(() => new MemoryStream(PlatformSpecificInterface.ResizeImage(ResumeApp.Resources.Resources.SettingsIcon, PlatformSpecificInterface.IconWidth, PlatformSpecificInterface.IconHeight)));
            AboutButton.IconImageSource = ImageSource.FromStream(() => new MemoryStream(PlatformSpecificInterface.ResizeImage(ResumeApp.Resources.Resources.InfoIcon, PlatformSpecificInterface.IconWidth, PlatformSpecificInterface.IconHeight)));
            switch (SelectedTheme)
            {
                default:
                    ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.BlueIcon)));
                    break;

                case "Orange":
                    ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.OrangeIcon)));
                    break;

                case "GrayScale":
                    ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.NoirIcon)));
                    break;

                case "Green":
                    ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.GreenIcon)));
                    break;

                case "Neon":
                    ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.NeonIcon)));
                    break;

                case "Red":
                    ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(Base64Resources.RedIcon)));
                    break;
            }
        }

        private View GenerateLayoutFromHomepageItem(HomePageItem item)
        {
            Image image = new Image
            {
                Source = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String((string)item.Base64Icon)))
            };
            Label label = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 10,
                Text = item.Label
            };
            RoundedGrid grid = new RoundedGrid
            {
                CornerRadius = 15,
                BackgroundColor = item.BackgroundColor
            };
            grid.OnTap.Add(new TapGestureRecognizer() { CommandParameter = item, Command = new Command<HomePageItem>((x) => ItemClicked(x)) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(.9, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.Children.Add(image, 0, 0);
            grid.Children.Add(label, 0, 1);
            return grid;
        }

        private void ItemClicked(HomePageItem x)
        {
            NavigateTo((ContentPage)Activator.CreateInstance(x.Page));
        }

        private void SettingsButton_Clicked(object sender, System.EventArgs e)
        {
            NavigateTo(new SettingsPage());
        }

        private void AboutButton_Clicked(object sender, System.EventArgs e)
        {
            NavigateTo(new AboutPage());
        }
    }
}