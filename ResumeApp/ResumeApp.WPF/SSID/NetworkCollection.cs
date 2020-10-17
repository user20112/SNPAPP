using System.Collections;
using System.Collections.Generic;

namespace ResumeApp.WPF.SSID
{
    public class NetworkCollection : IEnumerable<Network>
    {
        private readonly IEnumerable networkEnumerable;

        internal NetworkCollection(IEnumerable networkEnumerable)
        {
            this.networkEnumerable = networkEnumerable;
        }

        public IEnumerator<Network> GetEnumerator()
        {
            foreach (INetwork network in networkEnumerable)
            {
                yield return new Network(network);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (INetwork network in networkEnumerable)
            {
                yield return new Network(network);
            }
        }
    }
}