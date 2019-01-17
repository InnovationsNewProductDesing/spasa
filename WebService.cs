using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Support.V4.View;

namespace Spasa.Droid
{
	class WebService
	{
		public string SessionID;
		public static string Message;

		public string GenerateSessionID()
		{
			Random random = new Random();
			string r = "";
			int i;
			for (i = 1; i < 11; i++)
			{
				r += random.Next(0, 9).ToString();
			}
			return r;
		}


		public async Task GetCashier()
		{
			var Id = PublicVariables.CashierID;
			var Pin = PublicVariables.CashierPIN;
			var msisdn = PublicVariables.MSISDN;
			SessionID = GenerateSessionID();

			try
			{
				var client = new RestClient("http://app.spasa.co.za/mobileapp.ashx?" +
					"msisdn="+ msisdn+
	 				"&sessionid="+SessionID +
					"&func=Cashier" +
					"&CashierID=" +Id+
					"&CashierPIN="+Pin);
				var request = new RestRequest(Method.GET);
				var response = client.Execute(request);
				
				if (!string.IsNullOrEmpty(response.Content))
				{
					var ErrorCodeStart = response.Content.IndexOf("Error") + 7;
					var ErrorCode = response.Content.Substring(ErrorCodeStart, 1);
					var ErrorMsgStart = response.Content.IndexOf("Message") + 10;
					var ErrorMsgEnd = response.Content.IndexOf("VendorName") - 4;
					var ErrorMsgLength = ErrorMsgEnd - ErrorMsgStart;
					var ErrorMsg = response.Content.Substring(ErrorMsgStart, ErrorMsgLength);

					var BalanceStart = response.Content.IndexOf("Balance") + 9;
					var BalanceEnd = response.Content.IndexOf("}");
					var BalanceLength = BalanceEnd - BalanceStart;
					var Balance = response.Content.Substring(BalanceStart, BalanceLength);
					PublicVariables.CashierBalance = double.Parse(Balance);

					var VendorStart = response.Content.IndexOf("VendorName") + 13;
					var VendorEnd = response.Content.IndexOf("Balance") - 4;
					var VendorLength = VendorEnd - VendorStart;
					PublicVariables.VendorName = response.Content.Substring(VendorStart, VendorLength);


					if (ErrorCode == "0")
					{
						Message = "Success";
					}
					else
						Message = ErrorMsg;

				}

				else
					Message = "An Error has occured, please try again";
			

			}

			catch (Exception e)
			{
				Message = e.Message;
			}

		}

		


		//public async Task GetCustomer()
		//{
		//	var client = new RestClient("http://app.spasa.co.za/mobileapp.ashx?" +
		//		"msisdn="+PublicVariables.MSISDN +
		//		"&sessionid="+SessionID +
		//		"&func=Customer" +
		//		"&accountnumber=10000000001E");

		//	var request = new RestRequest(Method.GET);
		//	IRestResponse response = client.Execute(request);
		//	var msg = response.Content;
		//	Console.WriteLine("Content: "+msg);

		//	var ErrorCodeStart = msg.IndexOf("Error") + 7;
		//	var ErrorCode = msg.Substring(ErrorCodeStart, 1);

		//	var CreditStart = msg.IndexOf("Credit") + 9;
		//	var CreditEnd = msg.IndexOf("SupplierName") - 4;
		//	var CreditLength = CreditEnd - CreditStart;
		//	var Credit = msg.Substring(CreditStart, CreditLength);
		//	var CreditDouble = double.Parse(Credit);
		//	PublicVariables.CustomerErrorCode = ErrorCode;
		//	PublicVariables.CustomerCredit = CreditDouble;
			



		//}

		public async Task MakePayment()
		{



			var client = new RestClient("http://app.spasa.co.za/mobileapp.ashx?" +
				"msisdn="+PublicVariables.MSISDN +
				"&sessionid="+SessionID +
				"&func=Purchase" +
				"&accountnumber="+PublicVariables.AccountNo + PublicVariables.ProductCode +
				"&Amount="+PublicVariables.amount);
				var request = new RestRequest(Method.GET);
				IRestResponse response = client.Execute(request);
				var msg = response.Content;

				var ErrorCodeStart = msg.IndexOf("Error") + 7;
				var ErrorCode = msg.Substring(ErrorCodeStart, 1);
				PublicVariables.TransactionErrorCode = ErrorCode;

				var ErrorMessageStart = msg.IndexOf("Message") + 10;
				var ErrorMessageEnd = msg.IndexOf("CustomerBalance") - 4;
				var ErrorMessageLength = ErrorMessageEnd - ErrorMessageStart;
				var ErrorMessage = msg.Substring(ErrorMessageStart, ErrorMessageLength);
				PublicVariables.TransactionErrorMessage = ErrorMessage;

			var CustomerBalanceStart = msg.IndexOf("CustomerBalance") + 17;
			var CustomerBalanceEnd = msg.IndexOf("AvailableBalance") - 3;
			var CustomerBalanceLength = CustomerBalanceEnd - CustomerBalanceStart;
			var CustomerBalance = msg.Substring(CustomerBalanceStart, CustomerBalanceLength);
			var CustomerBalanceDouble = double.Parse(CustomerBalance);
			PublicVariables.NewCustomerBalance = CustomerBalanceDouble;

			

		}



	}
}