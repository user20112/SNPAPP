using System;
using Xamarin.Forms;

namespace ResumeApp.Classes
{
    public class HomePageItem
    {
        public string Base64Icon { get; set; }
        public string Label { get; set; }
        public string PageName { get; set; }
        public Color BackgroundColor { get; set; }
        public Type Page { get; set; }
        public string Description { get; set; }
        public bool Expanded { get; set; } = false;

        public HomePageItem(string base64Icon, string label, string pageName, Color backgroundColor, Type pageToDisplay, string description)
        {
            Description = description;
            Page = pageToDisplay;
            Base64Icon = base64Icon;
            Label = label;
            PageName = pageName;
            BackgroundColor = backgroundColor;
        }
    }
}