
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace KOTUS
{
	[Activity (Theme = "@style/Theme.Splash", Label = "KOTUS", MainLauncher = true, NoHistory = true, Icon = "@drawable/icon")]			
	public class SplashActivity : Activity
	{
		/* Name: OnCreate
		 * Purpose: Create the slpash activity when app is createed and then goes to welcome activity
		 * Parameters: Bundle - bundle to be used when activity started
		 * Return: void
		 */
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Thread.Sleep(2500); // Simulate a long loading process on app startup.
			// when we have a lot of data to load or uncompress we can replace the thread sleep with that instead
			StartActivity(typeof(WelcomeActivity));
		}
	}
}

