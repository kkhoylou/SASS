
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

namespace KOTUS
{
	[Activity (Label = "GroupDashActivity")]			
	public class GroupDashActivity : Activity
	{

		private Button admin_dash_button;
		private Button invite_member_button;
		private Button calendar_button;
		private Button message_board_button;
		private Button file_upload_button;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.GroupDash);

			admin_dash_button = FindViewById<Button> (Resource.Id.adminButton);
			admin_dash_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Admin Dash Button Pressed!!");
				//StartActivity(typeof(AdminDashActivity)); 
			};

			invite_member_button = FindViewById<Button> (Resource.Id.inviteButton);
			invite_member_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Invite Memeber Button Pressed!!");
				//StartActivity(typeof(inviteMember)); 
			};

			calendar_button = FindViewById<Button> (Resource.Id.calendarButton);
			calendar_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Calendar Button Pressed!!");
				StartActivity(typeof(CalendarEvent)); 
			};

			message_board_button = FindViewById<Button> (Resource.Id.messageButton);
			message_board_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Message Board Button Pressed!!");
				//StartActivity(typeof(messageBoard)); 
			};

			file_upload_button = FindViewById<Button> (Resource.Id.fileButton);
			file_upload_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("File Upload Button Pressed!!");
				//StartActivity(typeof(fileUpload)); 
			};
		}
	}
}

