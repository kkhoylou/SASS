using System;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Parse;
using Android.Support.V4.Widget;
using Android.Content.Res;
using System.Collections.Generic;

namespace KOTUS
{
			

	/*
	 * UserActivityWindow allows for logged in, non-group activities to be 
	 * displayed behind the navigation drawer
	 */
	[Activity (Theme="@style/MyCustomTheme")]
	public class UserActivityWindow : Activity
	{

		private static bool RETURN_TO_CAL = false; // shound the activity, when loaded, display the calendar

		private const int DASHBOARD_POSITION = 0; // define the current drawer position for the dashboard
		private const int CREATE_GROUP_POSITION = 1; // define the current drawer position for create group
		private const int CALENDAR_POSITION =2; // define the current drawer position for the calendar
		private const int LOGOUT_POSITION = 3; // define the current drawer position for user logout

		// Expanding Drawer Components
		// These all will contain data about how the drawer is presented and acts
		private DrawerLayout _drawer; // the drawer class itself
		private MyActionBarDrawerToggle _drawerToggle; // interfaces with user toggling
		private ListView _drawerList; // interfaces with the View component for the drawer in the UI
		private string _drawerTitle; // contains the currently displayed drawer selection
		private string _title; // contains the currently displayed AppBar title
		private string[] _drawerOptions; // contains the list of selectable drawer options

		// parseLogOut logs the current user out of the app
		// resets all static user info
		// redirects the user to the welcome screen
		public async void parseLogOut(){
			try{
				App.CurrentGroupID = "";
				App.CurrentGroupName = "";
				App.UsersName = "";
				App.UserFirstName = "";
				App.UsersLastName = "";
				await ParseUser.LogOutAsync();
				StartActivity (typeof(WelcomeActivity));
			}catch (ParseException e){
				// logout failed.. bfd
				StartActivity (typeof(WelcomeActivity));
				this.Finish ();
			}
		}

		// Defines the behavior of the activity window on return from the calendar
		public void calendarViewAction(){
			RETURN_TO_CAL = true;
			StartActivity (typeof(EditEventActivity));
		}

		// parseCreateGroup creates a new group object in parse using the parse API
		// grabs input from the View and stores it then saves it to parse
		// then redirects the user to the group activity window
		public async void parseCreateGroup(/*string ID,*/ string group_name){
			var group = new ParseObject ("Group");
			group["groupName"] = group_name;
			//group["privacy"] = privacy;
			//group["groupID"] = ID;
			IList<string> userList = new List<string>{ParseUser.CurrentUser.Username};
			IList<string> eventsList = new List<string>{ };
			IList<string> messagesList = new List<string> { };
			group ["events"] = eventsList;
			group ["userList"] = userList;
			group ["messages"] = messagesList;
			group ["admin"] = ParseUser.CurrentUser.Username;
			try{
				await group.SaveAsync();
				App.CurrentGroupID = group.ObjectId.ToString();
				App.CurrentGroupName = group_name;
				ParseUser.CurrentUser.AddToList("groupList", group.ObjectId.ToString());
				await ParseUser.CurrentUser.SaveAsync ();
				StartActivity(typeof(GroupActivityWindow));
			}catch(ParseException e){
				// do nothing
			}
		}

		// opens the group activity window
		public void openGroup(){
			StartActivity(typeof(GroupActivityWindow));
		}
			

		// defines the drawer display and fragment display system
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view to be the drawer
			SetContentView (Resource.Layout.DrawerLayout);

			// set the current page title
			_title = _drawerTitle = Title;

			// get the list of drawer strings
			_drawerOptions = Resources.GetStringArray (Resource.Array.DashOptArr);

			// set the dashboard title to include the users first name
			_drawerOptions [DASHBOARD_POSITION] = App.UsersName + _drawerOptions [DASHBOARD_POSITION];

			// find the drawer in the View layer
			_drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			_drawerList = FindViewById<ListView>(Resource.Id.left_drawer);

			// modify the appearance of the drawer in the View layer
			_drawer.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

			// send the drawer strings to the View layer
			_drawerList.Adapter = new ArrayAdapter<string>(this,
				Resource.Layout.DrawerListItem, _drawerOptions);

			// define what to do when the user taps on a drawer item
			_drawerList.ItemClick += (sender, args) => SelectItem(args.Position);
			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);

			//DrawerToggle is the animation that happens with the indicator next to the
			//ActionBar icon.
			_drawerToggle = new MyActionBarDrawerToggle(this, _drawer,
				Resource.Drawable.ic_drawer_light,
				Resource.String.DrawerOpen,
				Resource.String.DrawerClose);

			// Can alternatively use _drawer.DrawerClosed here instead of _drawerToggle
			_drawerToggle.DrawerClosed += delegate
			{
				ActionBar.Title = _title;
				InvalidateOptionsMenu();
			};

			// Can alternatively use _drawer.DrawerOpened here instead of _drawerToggle
			_drawerToggle.DrawerOpened += delegate
			{
				ActionBar.Title = _drawerTitle;
				InvalidateOptionsMenu();
			};

			// set the current drawer listener to use
			_drawer.SetDrawerListener(_drawerToggle);

			// define action for activity start
			if (null == savedInstanceState){
				if (RETURN_TO_CAL == false) {
					SelectItem (0);
				} else {
					SelectItem (CALENDAR_POSITION);
				}
		
			}
		}

		// define which fragment to display based on selected item position
		private void SelectItem(int position)
		{
			Fragment fragment = null;

			switch (position) {
			case LOGOUT_POSITION:
				fragment = new LogoutFragment ();
				break;
			case DASHBOARD_POSITION:
				fragment = new UserDashboardFragment ();
				break;
			case CREATE_GROUP_POSITION:
				fragment = new CreateGroupFragment ();
				break;
			case CALENDAR_POSITION:
				fragment = new UserCalendarFragment ();
				break;
			};

			FragmentManager.BeginTransaction()
				.Replace(Resource.Id.content_frame, fragment)
				.Commit();

			_drawerList.SetItemChecked(position, true);
			ActionBar.Title = _title = _drawerOptions[position];
			_drawer.CloseDrawer(_drawerList);
		}

		// sync the state of the toggled drawer
		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			_drawerToggle.SyncState();
		}

		// update when configuration changes
		public override void OnConfigurationChanged(Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			_drawerToggle.OnConfigurationChanged(newConfig);
		}

		// if any icons are added to the title bar, we can define their actions here
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			//MenuInflater.Inflate(Resource.Menu.main, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		// what to do while the drawer is open
		public override bool OnPrepareOptionsMenu(IMenu menu)
		{
			var drawerOpen = _drawer.IsDrawerOpen(Resource.Id.left_drawer);
			//menu.FindItem(Resource.Id.action_websearch).SetVisible(!drawerOpen);
			return base.OnPrepareOptionsMenu(menu);
		}

		// what to do with drawer when items are selected
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			if (_drawerToggle.OnOptionsItemSelected(item))
				return true;

			switch (item.ItemId)
			{
			default:
				return base.OnOptionsItemSelected(item);
			}
		}

	}
}

