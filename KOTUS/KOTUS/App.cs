using System;
using Android.App;
using Android.Runtime;
using Parse;

namespace KOTUS
{
	[Application]
	public class App : Application
	{
		// Public Static Strings to be used throughout the app to reference the current user
		public static string UsersName = "";
		public static string UserFirstName = "";
		public static string UsersLastName = "";

		// Public Static Strings to be used throughout the app to reference the current group
		public static string CurrentGroupID = "";
		public static string CurrentGroupName = "";

		// Public Static String to be used throughout the app to reference the current message
		public static string CurrentMessageID = "";

		// Public Static String to be used throughout the app to reference the current event
		public static string CurrentEventID = "";


		// Function required by Parse API, though its functionality is not used for our app
		public App (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		// Function required by Parse API, allows us to initialize the parse client to our API Keys app wide
		public override void OnCreate ()
		{
			base.OnCreate ();

			// Initialize the Parse client with your Application ID and .NET Key found on
			// your Parse dashboard
			Console.WriteLine("Initializing Parse Client");
			ParseClient.Initialize ("essILhBBMMu4mcWdHW7Qtb4pZJdBU415083KdFXT", "qMWZn2Zjmln0OIMXjJltHBR3HW7cD1yR5J2IVdvk");

		}
	}
}