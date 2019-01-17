using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Spasa.Droid
{
	public class Settings : Dialog
	{
		


		public Settings(Activity activity) : base(activity)
		{

		}
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			RequestWindowFeature((int)WindowFeatures.NoTitle);
			SetContentView(Resource.Layout.Settings);
			Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
			
			Button SettingsConfirmBtn = FindViewById<Button>(Resource.Id.SettingsConfirmButton);
			Button SettingsBackBtn = FindViewById<Button>(Resource.Id.SettingsCancelButton);
			EditText MsisdnEtxt = FindViewById<EditText>(Resource.Id.SettingsMSISDNEtxt);
			EditText MunicipalityEtxt = FindViewById<EditText>(Resource.Id.SettingsMunicipalityEtxt);

			if (!String.IsNullOrEmpty(PublicVariables.MSISDN))
			{
				MsisdnEtxt.Text = PublicVariables.MSISDN;
			}

			if (!String.IsNullOrEmpty(PublicVariables.MunicipalityName))
			{
				MunicipalityEtxt.Text = PublicVariables.MunicipalityName;
			}



				
			MsisdnEtxt.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
			{
				PublicVariables.MSISDN = e.Text.ToString();
			};


			MunicipalityEtxt.TextChanged += (object sender, Android.Text.TextChangedEventArgs e1) =>
			{
				PublicVariables.MunicipalityName = e1.Text.ToString();
			};




			SettingsConfirmBtn.Click += delegate
		{

			if (string.IsNullOrEmpty(PublicVariables.MSISDN) || PublicVariables.MSISDN.Length < 10)
			{
				Toast.MakeText(Application.Context, "Please enter a valid MSISDN", ToastLength.Long).Show();
			}


			if (PublicVariables.MSISDN.StartsWith("0"))
			{
				PublicVariables.MSISDN = "27" + PublicVariables.MSISDN.Remove(0,1);
			}
			if (PublicVariables.MSISDN.StartsWith("+"))
			{
				PublicVariables.MSISDN.Remove(0, 1);
			}
			

			

			if (string.IsNullOrEmpty(PublicVariables.MunicipalityName))
			{
				Toast.MakeText(Application.Context, "Please enter a Municipality", ToastLength.Long).Show();

			}
			

			var MSISDN = Application.Context.GetSharedPreferences("msisdn", Android.Content.FileCreationMode.Private);
			var UserEdit = MSISDN.Edit();
			UserEdit.PutString("MSISDN", PublicVariables.MSISDN);
			UserEdit.Commit();


			var Municipality = Application.Context.GetSharedPreferences("municipality", Android.Content.FileCreationMode.Private);
			var UserEdit1 = Municipality.Edit();
			UserEdit1.PutString("Municipality", PublicVariables.MunicipalityName);
			UserEdit1.Commit();

			Dismiss();
			Console.WriteLine("Municipality: " + PublicVariables.MunicipalityName 
				+ "\n" + "MSISDN: " + PublicVariables.MSISDN);
			Toast.MakeText(Application.Context, "Settings Saved", ToastLength.Long).Show();


		};

			SettingsBackBtn.Click += (e, a) =>
			{
				Dismiss();
			};


		}

		
	}
}