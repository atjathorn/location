using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System;
using System.Collections.Generic;
using Android.Util;
using System.Linq;
using Android.Runtime;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using App2;
using System.IO;

namespace App2
{
    [Activity(Label = "test", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener
    {

        static readonly string TAG = "X:" + typeof(MainActivity).Name;
        TextView _addressText;
        Location _currentLocation;
        LocationManager _locationManager;
        static string sendText1;
        string _locationProvider;
        TextView _locationText;

        public class data
        {
            // Add e.g. strings, int, DateTime,... for each datafield in your database.
            public string name { get; set; } // These strings should get the same name as your databasefields.
            public string pass { get; set; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _addressText = FindViewById<TextView>(Resource.Id.address_text);
            _locationText = FindViewById<TextView>(Resource.Id.location_text);
            FindViewById<TextView>(Resource.Id.get_address_button).Click += AddressButton_OnClick;

            InitializeLocationManager();
        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }
        
        async void AddressButton_OnClick(object sender, EventArgs eventArgs)
        {
            
            

            if (_currentLocation == null)
            {
                _addressText.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }
            SendToPhp(); 
           // Address address = await ReverseGeocodeCurrentLocation();
           // DisplayAddress(address);
            Toast.MakeText(this, sendText1,ToastLength.Short).Show();
        }
        
        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                _addressText.Text = deviceAddress.ToString();
            }
            else
            {
                _addressText.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }

        public async void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                sendText1 = _locationText.Text;
                Address address = await ReverseGeocodeCurrentLocation();
                DisplayAddress(address);
                
            }
            
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {

        }
        
       
        private void SendToPhp()
        {
            try
            {
                // Create a new data object
                data DataObj = new data();
                DataObj.name = sendText1;

                // Serialize your data object.
                string JSONString = JsonClass.JSONSerialize<data>(DataObj);

                // Set your Url for your php-file on your webserver.
                string url = "http://192.168.175.2/test/insert.php";

                // Create your WebRequest
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

                myRequest.Method = "POST";

                string postData = JSONString;

                byte[] pdata = Encoding.UTF8.GetBytes(postData);

                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentLength = pdata.Length;

                Stream myStream = myRequest.GetRequestStream();
                myStream.Write(pdata, 0, pdata.Length);


                // Get response from your php file.
                WebResponse myResponse = myRequest.GetResponse();

                Stream responseStream = myResponse.GetResponseStream();

                StreamReader streamReader = new StreamReader(responseStream);

                // Pass the response to a string and display it in a toast message.
                string result = streamReader.ReadToEnd();

                Toast.MakeText(this, result, ToastLength.Long).Show();

                // Close your streams.
                streamReader.Close();
                responseStream.Close();
                myResponse.Close();
                myStream.Close();
            }
            catch (WebException ex)
            {
                string _exception = ex.ToString();
                Toast.MakeText(this, _exception, ToastLength.Long).Show();
                System.Console.WriteLine("--->" + _exception);
            }
        }
        
    }

}


