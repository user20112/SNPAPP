using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;

namespace ResumeApp.Classes
{
    public class CharacteristicListViewAdapter
    {
        public ICharacteristic BaseCharacteristic;
        public string CharacteristicName = "";
        public string CurrentValue = "";

        public CharacteristicListViewAdapter(ICharacteristic baseCharacteristic)
        {
            BaseCharacteristic = baseCharacteristic;
            BaseCharacteristic.ValueUpdated += ValueUpdated;
            CharacteristicName = baseCharacteristic.ID.ToString();
        }

        private void ValueUpdated(object sender, CharacteristicReadEventArgs e)
        {
            CurrentValue = e.Characteristic.StringValue;
        }
    }
}