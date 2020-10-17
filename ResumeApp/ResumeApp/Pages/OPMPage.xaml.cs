using ResumeApp.Classes;
using ResumeApp.Classes.Adapters;
using ResumeApp.Classes.Bluetooth;
using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Classes.Devices;
using ResumeApp.Events;
using ResumeApp.Interfaces;
using ResumeApp.ViewModels;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OPMPage : BaseContentPage
    {
        public OPMViewModel VM;
        public OPM PowerMeter;
        private SynchronizationContext Main;

        public OPMPage()
        {
            InitializeComponent();
            VM = (OPMViewModel)this.BindingContext;
            MainFrame.Content = MainGrid;
            Title = "OPM";
            BluetoothButton.IconImageSource = ImageSource.FromStream(() => new MemoryStream(PlatformSpecificInterface.ResizeImage(ResumeApp.Resources.Resources.BluetoothIcon, PlatformSpecificInterface.IconWidth, PlatformSpecificInterface.IconHeight)));
            SettingsButton.IconImageSource = ImageSource.FromStream(() => new MemoryStream(PlatformSpecificInterface.ResizeImage(ResumeApp.Resources.Resources.SettingsIcon, PlatformSpecificInterface.IconWidth, PlatformSpecificInterface.IconHeight)));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                DataStorage.DictionaryOfColorCodes.TryGetValue("None", out SelectedColorCode);
            }
            catch
            {
                SelectedColorCode = DataStorage.DictionaryOfColorCodes.First().Value;
            }
            Main = SynchronizationContext.Current;
            RefreshListView();
            if (PowerMeter != null)
                if (!PowerMeter.Live)
                    PowerMeter.StartStream(ReadingChanged, Main, Offload);
            FirstReading = true;
        }

        private void ReadingChanged(object sender, ReadingChangedEventArgs e)
        {
            if (FirstReading)
            {
                FirstReading = false;
                OutputLabel.Text = "Connected to SNP OPM";
            }
            ReadingLabel.Text = e.reading.DisplayValue + " " + e.reading.DisplayWavelength;
            ReadingFrame.BorderColor = e.reading.ToneDetected == "None" ? (Color)Application.Current.Resources["Green"] : (Color)Application.Current.Resources["DarkGray"];
            if (e.reading.ToneDetected != "" && e.reading.ToneDetected != "None")
            {
                ToneLabel.TextColor = Color.Red;
                ToneLabel.Text = "Tone:" + e.reading.ToneDetected;
            }
            else
            {
                ToneLabel.TextColor = Color.White;
                ToneLabel.Text = "Tone:None";
            }
        }

        private void Offload(object sender, OffLoadEventArgs e)
        {
            ImportReadings(e.readings);
        }

        public override void BackButtonPressed()
        {
            if (PowerMeter != null)
                try
                {
                    PowerMeter.StopStream();
                }
                catch
                {
                }
            base.BackButtonPressed();
        }

        public void UpdateStatus(string text)
        {
            MainThread.BeginInvokeOnMainThread(() => { OutputLabel.Text = text; });
        }

        public Color SKColorToColor(SKColor value)
        {
            SKColor Value = (SKColor)value;
            double Red = Scale(Value.Red, 0, 1);
            double Blue = Scale(Value.Blue, 0, 1);
            double Green = Scale(Value.Green, 0, 1);
            double Alpha = Scale(Value.Alpha, 0, 1);
            return new Color(Red, Green, Blue, Alpha);
        }

        public void SetDevice(IAdapter adapter, IDevice device)
        {
            if (PowerMeter != null)
                PowerMeter.Dispose();
            OutputLabel.Text = "Connecting To OPM";
            PowerMeter = new SNPOPM(new BluetoothComInterface(adapter, device));
        }

        private bool FirstReading = false;

        private void ToastDisplayEvent(object sender, DisplayEventArgs e)
        {
            ShowToast(e.DisplayText);
        }

        private void DisplayEvent(object sender, DisplayEventArgs e)
        {
            UpdateStatus(e.DisplayText);
        }

        public new double Scale(double val, double min, double max)
        {
            double m = (max - min) / (255);
            double c = min;
            return val * m + c;
        }

        public void ImportReadings(List<Reading> readingsToImport)
        {
            foreach (Reading r in readingsToImport)
            {
                VM.ReadingListSource.Add(new ReadingListAdapter(r));
                DataStorage.Store(r);
            }
            RefreshListView();
        }

        private void RefreshListView()
        {
            int temp = VM.ReadingListSource.IndexOf(ReadingList.SelectedItem as ReadingListAdapter);
            int Index = -1;
            if (temp >= 0)
                Index = VM.ReadingListSource[temp].Reading.Index;
            ObservableCollection<ReadingListAdapter> newadapter = new ObservableCollection<ReadingListAdapter>();
            DataStorage.ListOfReadings.Sort();
            for (int x = 0; x < DataStorage.ListOfReadings.Count; x++)
            {
                if ((Convert.ToInt32(SettingsManagerInterface.GetString("Min", "-100")) == 0 && Convert.ToInt32(SettingsManagerInterface.GetString("Max", "100")) == 0) || (Convert.ToInt32(SettingsManagerInterface.GetString("Min", "-100")) == -100 && Convert.ToInt32(SettingsManagerInterface.GetString("Max", "100")) == 100))
                {
                    DataStorage.ListOfReadings[x].ColorEnabled = false;
                }
                else
                {
                    DataStorage.ListOfReadings[x].ColorEnabled = true;
                    var measurement = Convert.ToSingle(DataStorage.ListOfReadings[x].Measurement);
                    DataStorage.ListOfReadings[x].PassFail = measurement >= Convert.ToInt32(SettingsManagerInterface.GetString("Min", "-100")) && measurement <= Convert.ToInt32(SettingsManagerInterface.GetString("Max", "100"));
                }
                newadapter.Add(new ReadingListAdapter(DataStorage.ListOfReadings[x]) { Selected = DataStorage.ListOfReadings[x].Index == Index });
            }
            VM.ReadingListSource = newadapter;
            VM.RefreshListView();
        }

        public void ChangeWavelength(int wL)
        {
            if (PowerMeter != null)
                if (!ReadingLabel.Text.Contains("No OPM"))
                {
                    PowerMeter.ChangeWaveLength((int)wL);
                }
        }

        private void ReadingList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            RefreshListView();
        }

        private void ModeButton_Clicked(object sender, EventArgs e)
        {
            if (PowerMeter != null)
                if (!(ReadingLabel.Text.Contains("No OPM") || ReadingLabel.Text.Contains("*") || ReadingLabel.Text.Contains("--") || ReadingLabel.Text.Contains("+")))
                    PowerMeter.DBDBM();
        }

        private void WaveButton_Clicked(object sender, EventArgs e)
        {
            if (PowerMeter != null)
            {
                WavelengthSelectionGrid.IsVisible = true;
                OverallGrid.RaiseChild(WavelengthSelectionGrid);
            }
        }

        private void SixteenElevenButton_Clicked(object sender, EventArgs e)
        {
            WavelengthSelectionGrid.IsVisible = false;
            ChangeWavelength(1611);
        }

        private void SixteenTwentyFiveButton_Clicked(object sender, EventArgs e)
        {
            WavelengthSelectionGrid.IsVisible = false;
            ChangeWavelength(1625);
        }

        private void ThirteenHundredButton_Clicked(object sender, EventArgs e)
        {
            WavelengthSelectionGrid.IsVisible = false;
            ChangeWavelength(1300);
        }

        private void ThirteenTenButton_Clicked(object sender, EventArgs e)
        {
            WavelengthSelectionGrid.IsVisible = false;
            ChangeWavelength(1310);
        }

        private void EightFiftyButton_Clicked(object sender, EventArgs e)
        {
            WavelengthSelectionGrid.IsVisible = false;
            ChangeWavelength(850);
        }

        private void FifteenFiftyButton_Clicked(object sender, EventArgs e)
        {
            WavelengthSelectionGrid.IsVisible = false;
            ChangeWavelength(1550);
        }

        private void FourteenNintyButton_Clicked(object sender, EventArgs e)
        {
            WavelengthSelectionGrid.IsVisible = false;
            ChangeWavelength(1490);
        }

        private void SetRefButton_Clicked(object sender, EventArgs e)
        {
            if (PowerMeter != null)
                if (!(ReadingLabel.Text.Contains("No OPM") || ReadingLabel.Text.Contains("*") || ReadingLabel.Text.Contains("--") || ReadingLabel.Text.Contains("+")))
                    PowerMeter.SetReference(ReadingLabel.Text.ToLower());
        }

        private void RenameButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if ((ReadingList.SelectedItem as ReadingListAdapter) != null)
                {
                    string selectedID = "";
                    foreach (ReadingListAdapter reading in VM.ReadingListSource)
                        if (reading.Selected)
                            selectedID = reading.Reading.ID;
                    string Temp = "";
                    Temp = PlatformSpecificInterface.GatherThroughPopupOkCancel("Please Enter A New Name For The Selected Reading", "Rename Selected Reading", "", selectedID);
                    if (Temp != "")
                        foreach (ReadingListAdapter reading in VM.ReadingListSource)
                            if (reading.Selected)
                                reading.Reading.ID = Temp;
                    List<Reading> temp = new List<Reading>();
                    foreach (ReadingListAdapter reading in VM.ReadingListSource)
                    {
                        temp.Add(reading.Reading);
                    }
                    DataStorage.Update(new List<Reading>(temp));
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        RefreshListView();
                    });
                }
            });
        }

        private void OverWriteButton_Clicked(object sender, EventArgs e)
        {
            if (ReadingList.SelectedItem != null)
                SaveReading((ReadingList.SelectedItem as ReadingListAdapter).Reading.Index, true);
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if ((ReadingList.SelectedItem as ReadingListAdapter) != null)
            {
                int temp = (ReadingList.SelectedItem as ReadingListAdapter).Reading.Index;
                for (int x = 0; x < DataStorage.ListOfReadings.Count; x++)
                    if (DataStorage.ListOfReadings[x].Index == temp)
                    {
                        DataStorage.ListOfReadings.RemoveAt(x);
                        x--;
                    }
                if (VM.ReadingListSource.Count > 0)
                    ReadingList.SelectedItem = (VM.ReadingListSource[0]);
                RefreshListView();
            }
        }

        private void SaveReading(int index, bool overwriteExisting)
        {
            //Convert our strings into a "Reading" and add it to the list
            SelectedColorCode.Index = index;
            var readingDouble = Convert.ToDouble(PowerMeter.CurrentMeasurement);
            bool PassOrFail;
            bool colorEnabled;
            if (Convert.ToInt32(SettingsManagerInterface.GetString("Min", "-100")) == -100 || Convert.ToInt32(SettingsManagerInterface.GetString("Max", "100")) == 100)
            {
                PassOrFail = false;
                colorEnabled = false;
            }
            else if (readingDouble >= Convert.ToInt32(SettingsManagerInterface.GetString("Min", "-100")) && readingDouble <= Convert.ToInt32(SettingsManagerInterface.GetString("Max", "100")))
            {
                PassOrFail = true;
                colorEnabled = true;
            }
            else
            {
                PassOrFail = false;
                colorEnabled = true;
            }
            //create reading and add 1 to its index (since it is a new one)
            Reading reading;
            if (SelectedColorCode.Index < SelectedColorCode.Count - 1)
            {
                reading = new Reading(PowerMeter.CurrentMeasurement, PassOrFail, PowerMeter.CurrentMode, PowerMeter.CurrentWavelength, SelectedColorCode.CurrentSector + "_" + SelectedColorCode.CurrentGroupingColor + "_" + SelectedColorCode.CurrentIndividualColor + "_" + SelectedColorCode.CurrentCore, DateTime.Now, index, PowerMeter.CurrentReference, PowerMeter.CurrentTone) { ColorEnabled = colorEnabled };
                if (overwriteExisting)
                    for (int x = 0; x < DataStorage.ListOfReadings.Count; x++)
                        if (DataStorage.ListOfReadings[x].Index == index)
                        {
                            DataStorage.ListOfReadings.RemoveAt(x);
                            x--;
                        }
                DataStorage.ListOfReadings.Add(reading);
            }
            else
            {
                reading = new Reading(PowerMeter.CurrentMeasurement, PassOrFail, PowerMeter.CurrentMode, PowerMeter.CurrentWavelength, (index + 1).ToString(), DateTime.Now, index, PowerMeter.CurrentReference, PowerMeter.CurrentTone) { ColorEnabled = colorEnabled };
                if (overwriteExisting)
                    for (int x = 0; x < DataStorage.ListOfReadings.Count; x++)
                        if (DataStorage.ListOfReadings[x].Index == index)
                        {
                            DataStorage.ListOfReadings.RemoveAt(x);
                            x--;
                        }
                DataStorage.ListOfReadings.Add(reading);
            }
            RefreshListView();
            DataStorage.Update();
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (PowerMeter != null && !FirstReading)
                SaveReading(DataStorage.ListOfReadings.Count, false);
        }

        private void BluetoothButton_Clicked(object sender, EventArgs e)
        {
            BLEOPMPage temp = new BLEOPMPage(this);
            temp.SetParentPage(this);
            NavigateTo(temp);
        }

        private void SettingsButton_Clicked(object sender, EventArgs e)
        {
            NavigateTo(new OPMSettingsPage(this));
        }
    }
}