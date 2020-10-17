using ResumeApp.Classes;
using ResumeApp.Classes.Adapters;
using ResumeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OPMSettingsPage : BaseContentPage
    {
        public new OPMPage Parent;
        private readonly OPMSettingsViewModel VM;

        public OPMSettingsPage(OPMPage parent) : base()
        {
            Parent = parent;
            InitializeComponent();
            Title = "OPM Settings";
            VM = (OPMSettingsViewModel)BindingContext;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DataStorage.DictionaryOfOptions.TryGetValue("Color Code", out string SelectedColorCode);
            if (SelectedColorCode == null)
                SelectedColorCode = "None";
            foreach (KeyValuePair<string, ColorCode> entry in DataStorage.DictionaryOfColorCodes)
                VM.ColorCodePickerSource.Add(entry.Key);
            ColorCodePicker.SelectedItem = SelectedColorCode;
            if (Convert.ToInt32(SettingsManagerInterface.GetString("Min", "-100")) != -100)
                MinEntry.Text = SettingsManagerInterface.GetString("Min", "-100");
            else
                MinEntry.Text = "";
            if (Convert.ToInt32(SettingsManagerInterface.GetString("Max", "100")) != 100)
                MaxEntry.Text = SettingsManagerInterface.GetString("max", "100");
            else
                MaxEntry.Text = "";
        }

        protected override void OnDisappearing()
        {
            try
            {
                DataStorage.UpdateOption("Color Code", ColorCodePicker.SelectedItem as string);
                try
                {
                    int minValue = -100;
                    int maxValue = 100;
                    if (MinEntry.Text != "")
                        minValue = Convert.ToInt32(MinEntry.Text);
                    if (MaxEntry.Text != "")
                        maxValue = Convert.ToInt32(MaxEntry.Text);
                    if (minValue < maxValue)
                    {
                        SettingsManagerInterface.SetString(minValue.ToString(), "Min");
                        SettingsManagerInterface.SetString(maxValue.ToString(), "Max");
                    }
                    else throw new ArgumentException();
                }
                catch (Exception)
                {
                    ShowToast("Invalid Entry Into Min Max Feild");
                }
            }
            catch (Exception ex)
            {
            }
            base.OnDisappearing();
        }

        private void ClearButton_Clicked(object sender, EventArgs e)
        {
            MinEntry.Text = "";
            MaxEntry.Text = "";
        }

        private void ColorCodePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataStorage.DictionaryOfColorCodes.TryGetValue(((Picker)sender).SelectedItem as string, out SelectedColorCode);
            RefreshListView();
        }

        private void RefreshListView()
        {
            VM.RefreshListView();
        }

        private void RenameButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (PlatformSpecificInterface.PopupBool("Warning This Will Rename All Existing Readings", "Warning", "Yes", "No"))
                {
                    for (int x = 0; x < Parent.VM.ReadingListSource.Count; x++)
                    {
                        if (x < SelectedColorCode.Count)
                        {
                            Parent.VM.ReadingListSource[x].Reading.ID = SelectedColorCode.GetIndex(x);
                        }
                        else
                        {
                            Parent.VM.ReadingListSource[x].Reading.ID = (x + 1).ToString();
                        }
                    }
                    List<Reading> temp = new List<Reading>();
                    foreach (ReadingListAdapter reading in Parent.VM.ReadingListSource)
                    {
                        temp.Add(reading.Reading);
                    }
                    DataStorage.Update(temp);
                }
            });
        }
    }
}