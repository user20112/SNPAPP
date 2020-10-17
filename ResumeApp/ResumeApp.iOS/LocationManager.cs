using CoreLocation;
using System;

namespace ResumeApp.iOS
{
    public class LocationManager
    {
        protected CLLocationManager locMgr;
        private bool started = false;

        public LocationManager()
        {
            this.locMgr = new CLLocationManager();
            this.locMgr.RequestWhenInUseAuthorization();
        }

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        public CLLocationManager LocMgr
        {
            get { return this.locMgr; }
        }

        public void StartLocationUpdates()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                if (!started)
                {
                    LocMgr.DesiredAccuracy = 1;
                    LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                    {
                        LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                    };
                    started = true;
                }
                LocMgr.StartUpdatingLocation();
            }
        }

        public void StopLocationUpdates()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                LocMgr.StopUpdatingLocation();
            }
        }
    }

    public class LocationUpdatedEventArgs : EventArgs
    {
        private readonly CLLocation location;

        public LocationUpdatedEventArgs(CLLocation location)
        {
            this.location = location;
        }

        public CLLocation Location
        {
            get { return location; }
        }
    }
}