using ResumeApp.Classes.Bluetooth.LE;
using ResumeApp.Events;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ResumeApp
{
    public static class Extensions
    {
        public static Color ToColor(this SKColor value)
        {
            {
                SKColor Value = value;
                double Red = Scale(Value.Red, 0, 1);
                double Blue = Scale(Value.Blue, 0, 1);
                double Green = Scale(Value.Green, 0, 1);
                double Alpha = Scale(Value.Alpha, 0, 1);
                return new Color(Red, Green, Blue, Alpha);
            }
        }

        public static SKColor ToSKColor(this Color value)
        {
            byte Alpha = (byte)(value.A * 255);
            byte Red = (byte)(value.R * 255);
            byte Blue = (byte)(value.B * 255);
            byte Green = (byte)(value.G * 255);
            return new SKColor(Red, Green, Blue, Alpha);
        }

        public static double Scale(double val, double min, double max)
        {
            double m = (max - min) / (255);
            double c = min;
            return val * m + c;
        }

        public static Task<IDevice> ConnectAsync(this IAdapter adapter, IDevice device)
        {
            if (device.State == DeviceState.Connected)
                return Task.FromResult<IDevice>(null);
            var tcs = new TaskCompletionSource<IDevice>();
            void NewDeviceConnected(object sender, BluetoothDeviceConnectionEventArgs e)
            {
                Debug.WriteLine("CCC: " + e.Device.ID + " " + e.Device.State);
                if (e.Device.ID == device.ID)
                {
                    adapter.DeviceConnected -= NewDeviceConnected;
                    tcs.SetResult(e.Device);
                }
            }
            adapter.DeviceConnected += NewDeviceConnected;
            adapter.ConnectToDevice(device);
            return tcs.Task;
        }

        public static string CSVProof(this string s)
        {
            string[] temp;//holds the split values
            char[] separators = { '/', '\\', '?', '%', '*', ':', '|', '"', '<', '>', '.', ',' };//all illegal characters for fat32 plus comma ( we use it for things.
            temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);//split on those characters and agregate them in the array
            return String.Join("", temp);//combine them with an empty string to remove those characters from the original string.
        }

        public static string FileProof(this string s)
        {
            s = s.Replace(' ', '_');
            string[] temp;//holds the split values
            char[] separators = { '/', '\\', '?', '%', '*', ':', '|', '"', '<', '>', '.', ' ', ',' };//all illegal characters for fat32 plus comma ( we use it for things.
            temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);//split on those characters and agregate them in the array
            return String.Join("", temp);//combine them with an empty string to remove those characters from the original string.
        }

        public static int Find(this byte[] buff, byte[] search)
        {
            // enumerate the buffer but don't overstep the bounds
            for (int start = 0; start < buff.Length - search.Length; start++)
            {
                // we found the first character
                if (buff[start] == search[0])
                {
                    int next;
                    // traverse the rest of the bytes
                    for (next = 1; next < search.Length; next++)
                    {
                        // if we don't match, bail
                        if (buff[start + next] != search[next])
                            break;
                    }
                    if (next == search.Length)
                        return start;
                }
            }
            // not found
            return -1;
        }

        public static Task<ICharacteristic> GetCharacteristicAsync(this IService service, Guid id)
        {
            if (service.Characteristics.Count > 0)
            {
                return Task.FromResult(service.Characteristics.First(x => x.ID == id));
            }
            var tcs = new TaskCompletionSource<ICharacteristic>();
            void NewCharacteristicsDiscovered(object sender, EventArgs e)
            {
                service.CharacteristicsDiscovered -= NewCharacteristicsDiscovered;
                try
                {
                    var s = service.Characteristics.First(x => x.ID == id);
                    tcs.SetResult(s);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
            service.CharacteristicsDiscovered += NewCharacteristicsDiscovered;
            service.DiscoverCharacteristics();
            return tcs.Task;
        }

        public static Task<IService> GetServiceAsync(this IDevice device, Guid id)
        {
            if (device.Services.Count > 0)
            {
                return Task.FromResult(device.Services.First(x => x.ID == id));
            }
            var tcs = new TaskCompletionSource<IService>();
            void NewServicesDiscovered(object sender, EventArgs e)
            {
                device.ServicesDiscovered -= NewServicesDiscovered;
                try
                {
                    var s = device.Services.First(x => x.ID == id);
                    tcs.SetResult(s);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
            device.ServicesDiscovered += NewServicesDiscovered;
            device.DiscoverServices();
            return tcs.Task;
        }

        public static string PartialFromUuid(this Guid uuid)
        {
            // opposite of the UuidFromPartial method
            string id = uuid.ToString();
            if (id.Length > 8)
            {
                id = id.Substring(4, 4);
            }
            return "0x" + id;
        }

        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using var ms = new MemoryStream();
            byte[] buffer = new byte[bufferSize];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                ms.Write(buffer, 0, count);
            return ms.ToArray();
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<List<T>> Split<T>(this List<T> MainList, int nSize = 19)
        {
            for (int i = 0; i < MainList.Count; i += nSize)
            {
                yield return MainList.GetRange(i, Math.Min(nSize, MainList.Count - i));
            }
        }

        public static Guid UuidFromPartial(this Int32 @partial)
        {
            //this sometimes returns only the significant bits, e.g.
            //180d or whatever. so we need to add the full string
            string id = @partial.ToString("X").PadRight(4, '0');
            if (id.Length == 4)
            {
                id = "0000" + id + "-0000-1000-8000-00805f9b34fb";
            }
            return Guid.ParseExact(id, "d");
        }
    }
}