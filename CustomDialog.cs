using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Net;
using Spasa.Droid.Resources.layout;
using Android.AccessibilityServices;
using Android.Support.V4.Content;
using Android;

namespace Spasa.Droid
{
    class CustomDialog : Dialog
    {
        public CustomDialog(Activity activity) : base(activity)
        {

        }
        string timestamp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature((int)WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Confirmation);
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
			



            TextView confirmationText = FindViewById<TextView>(Resource.Id.confirmationTxt);

            confirmationText.Text = "You are about to purchase R" + PublicVariables.amount + " " + PublicVariables.product
                + " for " + "account: " + PublicVariables.AccountNo + "\n";





            Button cancel = (Button)FindViewById(Resource.Id.cancelButton);
            cancel.Click += (e, a) =>
            {
                Dismiss();
            };

            Button ConfirmBtn = FindViewById<Button>(Resource.Id.confirmButton);

			ConfirmBtn.Click += async delegate
			{
				ProgressDialog p = new ProgressDialog(Context);
				p.SetMessage("Processing Payment. Please wait...");
				p.Show();

				var w = new WebService();
				try
				{ await w.GetCashier(); }
				catch(Exception e)
				{
					confirmationText.Text = "Payment Failed" + "\n" + e.Message;
					cancel.Visibility = ViewStates.Gone;
					ConfirmBtn.Click += delegate
					{
						Dismiss();
					};
				}

				try
				{ await w.MakePayment(); }
				catch (Exception e)
				{
					confirmationText.Text = "Payment Failed"+ "\n"+e.Message;
					cancel.Visibility = ViewStates.Gone;
					ConfirmBtn.Click += delegate
					{
						Dismiss();
					};

				}

				p.Dismiss();
				var amount = double.Parse(PublicVariables.amount);

				timestamp = GetTimestamp(DateTime.Now);

				PublicVariables.Content = "Success\n" +
				"Amount Purchased R: " + PublicVariables.amount + "\n" +
				"Product: " + PublicVariables.product + "\n" +
				//"Customer: " + PublicVariables.CustomerName + "\n" +
				"Account Number: "+"\n" + PublicVariables.AccountNo + "\n" +
				"Cashier: " + PublicVariables.CashierName + " " + PublicVariables.CashierID + "\n" +
				"Municipality: " + PublicVariables.MunicipalityName + "\n" +
				"New Balance: R" + PublicVariables.NewCustomerBalance+ "\n"+
                "Timestamp: " + timestamp;

				
				if(PublicVariables.TransactionErrorCode != "0")
				{
					confirmationText.Text = "Payment Failed, ErrorCode is: "+ PublicVariables.TransactionErrorMessage;
					cancel.Visibility = ViewStates.Gone;
					ConfirmBtn.Click += delegate
					{
						Dismiss();
					};

				}

				else
				{

					w = new WebService();
					await w.GetCashier();

					confirmationText.Text = PublicVariables.Content + "\n" + "\n" + "Print Receipts?";
					cancel.Visibility = ViewStates.Gone;
					ConfirmBtn.Click += delegate
					 {
						 try
						 {
							 new ContentPrint().run();
							 new ContentPrint().run();
						 }

						 catch (Exception)
						 {

							 Toast.MakeText(Application.Context, "Print Error", ToastLength.Long).Show();
							 
						 }
						 Dismiss();
					 };

				}
			
            };

        }

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }


       
    }


    }

