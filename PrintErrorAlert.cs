using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Spasa.Droid.Resources.layout
{
    [Activity(Label = "PrintErrorAlert")]
    public class PrintErrorAlert : Dialog

    {
        public PrintErrorAlert(Activity activity) : base(activity)
        {

        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PrintError);

            Button ReprintBtn = FindViewById<Button>(Resource.Id.ReprintButton);

            ReprintBtn.Click +=  delegate
            {

               new ContentPrint().run();

            };



        }
    }
}