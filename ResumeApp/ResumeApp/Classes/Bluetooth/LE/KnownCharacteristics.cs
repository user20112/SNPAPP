using Newtonsoft.Json.Linq;
using ResumeApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ResumeApp.Classes.Bluetooth.LE
{
    public struct KnownCharacteristic
    {
        public Guid ID;
        public string Name;
    }

    public static class KnownCharacteristics
    {
        private static readonly object myLock = new object();
        private static Dictionary<Guid, KnownCharacteristic> items;

        static KnownCharacteristics()
        {
        }

        public static IList<KnownCharacteristic> GetCharacteristics()
        {
            return items.Values.ToList();
        }

        public static void LoadItemsFromJson()
        {
            items = new Dictionary<Guid, KnownCharacteristic>();
            //TODO: switch over to CharacteristicStack.Text when it gets bound.
            KnownCharacteristic characteristic;
            string itemsJson = ResourceLoader.GetEmbeddedResourceString(typeof(KnownCharacteristics).GetTypeInfo().Assembly, "KnownCharacteristics.json");
            var json = JValue.Parse(itemsJson);
            foreach (var item in json.Children())
            {
                JProperty prop = item as JProperty;
                characteristic = new KnownCharacteristic()
                { Name = prop.Value.ToString(), ID = Guid.ParseExact(prop.Name, "d") };
                items.Add(characteristic.ID, characteristic);
            }
        }

        public static KnownCharacteristic Lookup(Guid id)
        {
            lock (myLock)
            {
                if (items == null)
                    LoadItemsFromJson();
            }
            if (items.ContainsKey(id))
                return items[id];
            else
                return new KnownCharacteristic { Name = "Unknown", ID = Guid.Empty };
        }
    }
}