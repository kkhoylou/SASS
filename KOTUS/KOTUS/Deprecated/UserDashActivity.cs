
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

using Parse;

namespace KOTUS
{
	[Activity (Label = "User Dashboard")]			
	public class UserDashActivity : Activity
	{
		private Button event_create_button;
		private Button browse_group_button;
		private Button group_list_button;
		private Button group_create_button;
		private Button user_signout_button;


		string[] groups = new string[] { "Vegetables","Fruits","Flower Buds","Legumes","Bulbs","Tubers" };
		//List groups = ParseUser.CurrentUser.Get<Array>("groupList"); // get list of groups for this user and initialize it here.

		private ListView group_lists;


		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = groups[e.Position];
			Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
		}



		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// set the current layout
			SetContentView (Resource.Layout.UserDash);


			/*
			var query = from User in ParseObject.GetQuery("groupList")
				where User.Get<Array> 
				select User;*/

			group_lists = FindViewById<ListView> (Resource.Id.listView1);

			group_lists.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, groups);

			/*
			group_lists.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e) {
				Group_lists_ItemClick (sender, e);
			};*/


			group_list_button = FindViewById<Button> (Resource.Id.button4);
			group_list_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Group List Button Pressed!!");
				//StartActivity(typeof(GroupList)); 
			};

			event_create_button = FindViewById<Button> (Resource.Id.button3);
			event_create_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Create An Event Button Pressed!!");
				//StartActivity(typeof(CreateEvent)); 
			};

			browse_group_button = FindViewById<Button> (Resource.Id.button1);
			browse_group_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Browse A Group Button Pressed!!");
				//StartActivity(typeof(BrowsingGroupList)); 
			};

			group_create_button = FindViewById<Button> (Resource.Id.button2);
			group_create_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Create A Group Button Pressed!!");
				StartActivity(typeof(CreateGroup)); 
			};

			user_signout_button = FindViewById<Button> (Resource.Id.button5);
			user_signout_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Group List Button Pressed!!");
				//StartActivity(typeof(GroupList)); 
			};
		}

		void Group_lists_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			
		}
	}
}
	
