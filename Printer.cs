using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Common.Pos.Api.Printer;
using Java.Lang;

namespace Spasa.Droid
{
    public class ContentPrint : Java.Lang.Thread
    {
        [Override()]
        public void run()
        {
            UsbThermalPrinter ThermalPrinter = new UsbThermalPrinter(Android.App.Application.Context);

            Bitmap Logo = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.logobw);



            base.Run();
            ThermalPrinter.Start(0);
            ThermalPrinter.Reset();
            ThermalPrinter.SetAlgin(0);
            ThermalPrinter.SetLeftIndent(1);
            ThermalPrinter.SetLineSpace(1);
            ThermalPrinter.SetFontSize(1);
            ThermalPrinter.EnlargeFontSize(2, 2);
            ThermalPrinter.SetGray(1);
            ThermalPrinter.PrintLogo(Logo, true);
            ThermalPrinter.AddString(PublicVariables.Content);
            ThermalPrinter.PrintString();
            ThermalPrinter.ClearString();
            ThermalPrinter.WalkPaper(20);
			
            ThermalPrinter.Stop();
        }
    }
}