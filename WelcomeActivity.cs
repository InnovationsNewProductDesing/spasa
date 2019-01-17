
using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Print;
using Android.Widget;
using Android.Media;
using Com.Common.Pos.Api.Printer;
using Android.Hardware.Camera2;

namespace Spasa.Droid
{
    [Activity(Label = "Spasa", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.NoActionBar",
               LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]




    public class WelcomeActivity : Activity 
    {


        NfcAdapter mNfcAdapter;
        PendingIntent pendingIntent;
        Tag tag;
		string msisdn;
		string municipality;

		protected override void OnCreate(Bundle savedInstanceState)
        {


            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.Welcome);

            MediaPlayer media = MediaPlayer.Create(this, Resource.Raw.pleaseswipecard);
            media.Start();
			
            Button LoginBtn = FindViewById<Button>(Resource.Id.LoginButton);
            EditText pinCode = FindViewById<EditText>(Resource.Id.CashierPINetxt);
			EditText IdField = FindViewById<EditText>(Resource.Id.CashierIDetxt);
			ImageView SettingsBtn = FindViewById<ImageView>(Resource.Id.settingsButton);
			var MSISDN = Application.Context.GetSharedPreferences("msisdn", Android.Content.FileCreationMode.Private);
			msisdn = MSISDN.GetString("MSISDN", null);

			if (!string.IsNullOrEmpty(msisdn))
			{
				PublicVariables.MSISDN = msisdn;
			}

			var Municipality = Application.Context.GetSharedPreferences("municipality", Android.Content.FileCreationMode.Private);
			municipality = Municipality.GetString("Municipality", null);

			if (!string.IsNullOrEmpty(municipality))
			{
				PublicVariables.MunicipalityName = municipality;
			}

			SettingsBtn.Click += delegate
			{
				Settings settings = new Settings(this);
				settings.Show();
				
			};

			IdField.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
			{
				PublicVariables.CashierID = e.Text.ToString();
			};

			pinCode.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
			{
				PublicVariables.CashierPIN = e.Text.ToString();
			};

			LoginBtn.Click += async delegate
            {
				if(string.IsNullOrEmpty(PublicVariables.CashierID) || string.IsNullOrEmpty(PublicVariables.CashierPIN ))
				{
					Toast.MakeText(this, "Please enter valid Cashier ID & PIN", ToastLength.Long).Show();

				}

				else if (string.IsNullOrEmpty(PublicVariables.MunicipalityName) || string.IsNullOrEmpty(PublicVariables.MSISDN))
				{
					Toast.MakeText(this, "Please enter valid settings", ToastLength.Long).Show();
					Settings settings = new Settings(this);
					settings.Show();

				}
				else
				{
					ProgressDialog p = new ProgressDialog(this);
					p.SetMessage("Logging in...");
					p.Show();

					WebService w = new WebService();
					await w.GetCashier();
					var loginResult = WebService.Message;

					if (loginResult == "Success")
					{
						Intent Activityintent = new Intent(this, typeof(MainActivity));

						StartActivity(Activityintent);
					}

					else
					{

						Toast.MakeText(this, loginResult, ToastLength.Long).Show();
					}
					p.Dismiss();
				}
			};



            mNfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            if (mNfcAdapter == null)
            {
                return;
            }
            pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop), 0);

        }

		private void SettingsBtn_Click(object sender, EventArgs e)
		{
			throw new NotImplementedException();
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
            pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop), 0);

            mNfcAdapter.EnableForegroundDispatch(this, pendingIntent, null, null);

        }

        protected override void OnPause()
        {
            base.OnPause();
            mNfcAdapter.DisableForegroundDispatch(this);
        }

		

        public void ReadFromTag(Tag tag, Intent intent)
        {
            var messages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);

            Ndef ndef = Ndef.Get(tag);

            try
            {


                ndef.Connect();

                if (messages != null)
                {
                    NdefMessage[] ndefMessages = new NdefMessage[messages.Length];
                    for (int i = 0; i < messages.Length; i++)
                    {
                        ndefMessages[i] = (NdefMessage)messages[i];
                    }
                    NdefRecord record = ndefMessages[0].GetRecords()[0];

                    byte[] payload = record.GetPayload();

                    var msg = System.Text.Encoding.UTF8.GetString(payload);
					msg = msg.Remove(0, 3);

					EditText IdField = FindViewById<EditText>(Resource.Id.CashierIDetxt);

					IdField.Text = msg;

                    ndef.Close();

                }
            }

            catch (Exception e)
            {
                Toast.MakeText(this, "Message: " + e.Message, ToastLength.Long).Show();
            }



        }




    }
}








        
    

