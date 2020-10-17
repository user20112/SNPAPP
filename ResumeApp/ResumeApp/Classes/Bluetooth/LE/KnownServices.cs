using Newtonsoft.Json.Linq;
using ResumeApp.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ResumeApp.Classes.Bluetooth.LE
{
    public struct KnownService
    {
        public Guid ID;
        public string Name;
    }

    public static class KnownServices
    {
        private static readonly object myLock = new object();
        private static Dictionary<Guid, KnownService> items;

        static KnownServices()
        {
        }

        public static void LoadItemsFromJson()
        {
            items = new Dictionary<Guid, KnownService>();
            KnownService service;
            string itemsJson = ResourceLoader.GetEmbeddedResourceString(typeof(KnownServices).GetTypeInfo().Assembly, "KnownServices.json");
            var json = JValue.Parse(itemsJson);
            foreach (var item in json.Children())
            {
                JProperty prop = item as JProperty;
                service = new KnownService() { Name = prop.Value.ToString(), ID = Guid.ParseExact(prop.Name, "d") };
                items.Add(service.ID, service);
            }
        }

        public static KnownService Lookup(Guid id)
        {
            lock (myLock)
            {
                if (items == null)
                    LoadItemsFromJson();
            }
            if (items.ContainsKey(id))
                return items[id];
            else
                return new KnownService { Name = "Unknown", ID = Guid.Empty };
        }
    }
}