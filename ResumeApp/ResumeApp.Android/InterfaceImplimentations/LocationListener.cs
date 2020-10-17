using Android;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using System;

namespace ResumeApp.Droid.PlatformSpecific
{
    public class LocationListener : AppCompatActivity, ILocationListener
    {
        public bool isRequestingLocationUpdates;
        public string latitude;
        public string longitude;
        public string provider;
        private static readonly int RC_LAST_LOCATION_PERMISSION_CHECK = 1000;
        private static readonly int RC_LOCATION_UPDATES_PERMISSION_CHECK = 1100;
        private readonly LocationManager locationManager;
        private readonly View rootLayout;

        public LocationListener()
        {
            rootLayout = MainActivity.Instance.CurrentFocus;
            locationManager = (LocationManager)MainActivity.Instance.GetSystemService(LocationService);
        }

        ~LocationListener()
        {
            locationManager.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location location)
        {
            latitude = location.Latitude.ToString("0.000");
            longitude = location.Longitude.ToString("0.000");
            provider = location.Provider;
            Console.WriteLine(location.ToString());
        }

        public void OnProviderDisabled(string provider)
        {
            isRequestingLocationUpdates = false;
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK || requestCode == RC_LOCATION_UPDATES_PERMISSION_CHECK)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK)
                    {
                        GetLastLocationFromDevice();
                    }
                    else
                    {
                        isRequestingLocationUpdates = true;
                        StartRequestingLocationUpdates();
                    }
                }
                else
                {//PermissionNotGranted
                    Snackbar.Make(rootLayout, "Need Location Permissions for GPS", Snackbar.LengthIndefinite)
                            .SetAction("Ok", delegate { FinishAndRemoveTask(); })
                            .Show();
                    return;
                }
            }
            else
            {
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            if (status == Availability.OutOfService)
            {
                StopRequestingLocationUpdates();
                isRequestingLocationUpdates = false;
            }
        }

        public void StartLocationStreaming()
        {
            if (ContextCompat.CheckSelfPermission(MainActivity.Instance, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                StartRequestingLocationUpdates();
                isRequestingLocationUpdates = true;
            }
            else
            {
                RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
                StartRequestingLocationUpdates();
                isRequestingLocationUpdates = true;
            }
        }

        public void StopLocationStreaming()
        {
            isRequestingLocationUpdates = false;
            StopRequestingLocationUpdates();
        }

        private void GetLastLocationFromDevice()
        {
            var criteria = new Criteria { PowerRequirement = Power.Medium };
            var bestProvider = locationManager.GetBestProvider(criteria, true);
            var location = locationManager.GetLastKnownLocation(bestProvider);
            if (location != null)
            {
                latitude = location.Latitude.ToString();
                longitude = location.Longitude.ToString();
                provider = location.Provider;
            }
            else
            {
                latitude = "Location Unavailable";
                longitude = "Location Unavailable";
                provider = bestProvider;
            }
        }

        private void RequestLocationPermission(int requestCode)
        {
            isRequestingLocationUpdates = false;
            if (ActivityCompat.ShouldShowRequestPermissionRationale(MainActivity.Instance, Manifest.Permission.AccessFineLocation))
            {
                Snackbar.Make(rootLayout, "Requires Location Permissions", Snackbar.LengthIndefinite)
                        .SetAction("Ok",
                                   delegate
                                   {
                                       ActivityCompat.RequestPermissions(MainActivity.Instance, new[] { Manifest.Permission.AccessFineLocation }, requestCode);
                                   })
                        .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, requestCode);
            }
        }

        private void StartRequestingLocationUpdates()
        {
            isRequestingLocationUpdates = true;
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 200, 1, this);
        }

        private void StopRequestingLocationUpdates()
        {
            locationManager.RemoveUpdates(this);
            isRequestingLocationUpdates = false;
        }
    }
}