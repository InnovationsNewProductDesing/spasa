using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Media;
using Android.Nfc;
using Android.Nfc.Tech;
using System;

namespace Spasa.Droid
{
    [Activity(Label = "MainActivity",Theme = "@style/Theme.AppCompat.NoActionBar"
               )]
    public class MainActivity : Activity
    {
        NfcAdapter mNfcAdapter;
        PendingIntent pendingIntent;
        Tag tag;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);

            TextView CashierName = FindViewById<TextView>(Resource.Id.cashierNameTxt);
            TextView MunicipalName = FindViewById<TextView>(Resource.Id.MunicipalityName);
            EditText amountetxt = FindViewById<EditText>(Resource.Id.amountetxt);
            RadioButton electricityRadio = FindViewById<RadioButton>(Resource.Id.electricityRadioButton);
            RadioButton waterRadio = FindViewById<RadioButton>(Resource.Id.waterRadioButton);
            Button Nextbtn = FindViewById<Button>(Resource.Id.nextButton);
            EditText AccNoetxt = FindViewById<EditText>(Resource.Id.AccNoetxt);
			TextView BalanceTxtView = FindViewById<TextView>(Resource.Id.balanceTxtView);

            CashierName.Text = "Cashier: " + PublicVariables.CashierID;
            MunicipalName.Text = PublicVariables.MunicipalityName;
			
			BalanceTxtView.Text = PublicVariables.VendorName + " Balance: R" + PublicVariables.CashierBalance;


			amountetxt.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                PublicVariables.amount = e.Text.ToString();
            };

            
            AccNoetxt.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                PublicVariables.AccountNo = e.Text.ToString();
            };


            electricityRadio.Click+= (object sender, System.EventArgs e) => 
            {
                PublicVariables.product = "electricity";
				PublicVariables.ProductCode = "E";
            };

            waterRadio.Click += (object sender, System.EventArgs e) =>
            {
                PublicVariables.product = "water";
				PublicVariables.ProductCode = "W";


			};



            Nextbtn.Click += delegate {

				if (string.IsNullOrEmpty(PublicVariables.product))
				{
					Toast.MakeText(this, "Please select Water or Electricity", ToastLength.Long).Show();
				}
				else if (string.IsNullOrEmpty(PublicVariables.amount))
				{
					Toast.MakeText(this, "Please enter a purchase amount", ToastLength.Long).Show();

				}

				else if (string.IsNullOrEmpty(PublicVariables.AccountNo))
				{
					Toast.MakeText(this, "Please enter an account number or swipe customer card", ToastLength.Long).Show();

				}
				else
				{
					CustomDialog customDialog = new CustomDialog(this);
					customDialog.Show();
				}
            };

        }

        protected override void OnNewIntent(Intent intent)
        {

            base.OnNewIntent(intent);
            tag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);

            ReadFromTag(tag, intent);

        }

        protected override void OnResume()
        {
            base.OnResume();
            mNfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop), 0);

            mNfcAdapter.EnableForegroundDispatch(this, pendingIntent, null, null);

        }

        protected override void OnPause()
        {
            base.OnPause();
            mNfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            mNfcAdapter.DisableForegroundDispatch(this);
        }


        
        public void ReadFromTag(Tag tag, Intent intent)
        {
            var messages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);

            Ndef ndef = Ndef.Get(tag);

            try
            {

                ndef.Connect();

                    NdefMessage[] ndefMessages = new NdefMessage[messages.Length];
                    for (int i = 0; i < messages.Length; i++)
                    {
                        ndefMessages[i] = (NdefMessage)messages[i];
                    }
                    NdefRecord record = ndefMessages[0].GetRecords()[0];

                    byte[] payload = record.GetPayload();

                    var msg = System.Text.Encoding.UTF8.GetString(payload);
					msg = msg.Remove(0, 3);
                    
                    EditText AccNoetxt = FindViewById<EditText>(Resource.Id.AccNoetxt);
                    AccNoetxt.Text = msg;
                    PublicVariables.AccountNo = msg;
				

                    ndef.Close();

                
            }

            catch (Exception e)
            {
                Toast.MakeText(this, "Message: " + e.Message, ToastLength.Long).Show();
            }



        }






    }
}

