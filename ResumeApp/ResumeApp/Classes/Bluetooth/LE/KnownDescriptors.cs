using Newtonsoft.Json.Linq;
using ResumeApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ResumeApp.Classes.Bluetooth.LE
{
    public struct KnownDescriptor
    {
        public Guid ID;
        public string Name;
    }

    public static class KnownDescriptors
    {
        private static readonly object bleLock = new object();
        private static Dictionary<Guid, KnownDescriptor> items;

        static KnownDescriptors()
        { }

        public static IList<KnownDescriptor> GetDescriptors()
        {
            return items.Values.ToList();
        }

        public static void LoadItemsFromJson()
        {
            items = new Dictionary<Guid, KnownDescriptor>();
            KnownDescriptor descriptor;
            string itemsJson = ResourceLoader.GetEmbeddedResourceString(typeof(KnownDescriptors).GetTypeInfo().Assembly, "KnownDescriptors.json");
            var json = JValue.Parse(itemsJson);
            foreach (var item in json.Children())
            {
                JProperty prop = item as JProperty;
                descriptor = new KnownDescriptor() { Name = prop.Value.ToString(), ID = Guid.ParseExact(prop.Name, "d") };
                items.Add(descriptor.ID, descriptor);
            }
        }

        public static KnownDescriptor Lookup(Guid id)
        {
            lock (bleLock)
            {
                if (items == null)
                    LoadItemsFromJson();
            }
            if (items.ContainsKey(id))
                return items[id];
            else
                return new KnownDescriptor { Name = "Unknown", ID = Guid.Empty };
        }
    }
}