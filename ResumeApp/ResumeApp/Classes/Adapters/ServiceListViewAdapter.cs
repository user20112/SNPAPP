using ResumeApp.Classes.Bluetooth.LE;
using System;
using System.Collections.ObjectModel;

namespace ResumeApp.Classes.Adapters
{
    public class ServiceListViewAdapter
    {
        public IService BaseService;
        public string ServiceName = "";
        public ObservableCollection<CharacteristicListViewAdapter> Characteristics = new ObservableCollection<CharacteristicListViewAdapter>();

        public ServiceListViewAdapter(IService baseService)
        {
            BaseService = baseService;
            ServiceName = baseService.ID.ToString();
            BaseService.CharacteristicsDiscovered += CharacteristicsDiscovered;
            BaseService.DiscoverCharacteristics();
        }

        private void CharacteristicsDiscovered(object sender, EventArgs e)
        {
            for (int x = 0; x < BaseService.Characteristics.Count; x++)
            {
                Characteristics.Add(new CharacteristicListViewAdapter(BaseService.Characteristics[x]));
            }
        }
    }
}