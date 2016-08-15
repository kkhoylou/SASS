using System;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Parse;
using Android.Support.V4.Widget;
using Android.Content.Res;

using System.Linq;
using System.Text;

using Android.Content;
using Android.Runtime;

namespace KOTUS
{
	[Activity (Theme="@style/MyCustomTheme")]			
	public class GroupActivityWindow : Activity
	{
		public static bool return_to_activity = false;
		public static int return_positon = GROUP_DASH_POSITION;

		private const int DASHBOARD_POSITION = 0;
		private const int GROUP_DASH_POSITION = 1;
		private const int CALENDAR_POSITION = 2;
		private const int MESSAGE_BOARD_POSITION = 3;
		private const int ADD_MEMBER_POSITION = 4;
		private const int MEMBER_LIST_POSITION = 5;
		private const int LOGOUT_POSITION = 6;

		public const int ADMIN_DASH_SELECTION = 0; // might not be used
		public const int INVITE_MEMBER_SELECTION = 1;
		public const int CALENDAR_SELECTION = 2;
		public const int MESSAGE_BOARD_SELECTION = 3;
		public const int FILE_UPLOAD_SELECTION = 4;
		public const int RETURN_TO_DASH_SELECTION = 5;
		public const int MEMBER_LIST_SELECTION = 6;

		// Expanding Drawer Components
		private DrawerLayout _drawer;
		private MyActionBarDrawerToggle _drawerToggle;
		private ListView _drawerList;
		private string _drawerTitle;
		private string _title;
		private string[] _drawerOptions;

		/**
		 * Logs user out of app, sets parse info to default for
		 * the next user.
		 * Returns to WelcomeActivity
		 */ 
		public async void parse_log_out(){
			try{
				App.UsersName = "";
				App.UserFirstName = "";
				App.UsersLastName = "";
				App.CurrentGroupID = "";
				App.CurrentGroupName = "";

				await ParseUser.LogOutAsync();
				StartActivity (typeof(WelcomeActivity));
			}catch (ParseException e){
				// logout failed.. bfd
				StartActivity (typeof(WelcomeActivity));
				this.Finish ();
			}
		}

		/** 
		 * Creates a pop up that lists the users info
		 * i.e. Name, email, phone number
		 */
		public void group_member_list_alert(string alert_text){
			RunOnUiThread(() =>
				{
					AlertDialog.Builder builder;
					builder = new AlertDialog.Builder(this);
					builder.SetTitle("User Info");
					builder.SetMessage(alert_text);
					builder.SetCancelable(false);
					builder.SetPositiveButton("Close", delegate {
						
					});
					builder.Show();
				}
			);
		}

		/**
		 * Starts CalendarEvent activity on click
		 */ 
		public void group_calendar_button_action(){
			StartActivity (typeof(CalendarEvent));
		}

		/**
		 * Starts EditEventActivity activity on click
		 */ 
		public void group_calendar_view_action(){
			StartActivity(typeof(EditEventActivity));
		}

		/**
		 * Creates a new fragment for whatever button selected 
		 * It then begins the transaction
		 */ 
		public void group_dash_button_actions(int selection){
			Fragment fragment = null;

			switch (selection) {
			//Invite Member
			case INVITE_MEMBER_SELECTION:
				fragment = new AddMemberFragment ();
				ActionBar.Title = _title = _drawerOptions[ADD_MEMBER_POSITION];
				_drawerList.SetItemChecked(ADD_MEMBER_POSITION, true);
				break;
			//Calendar
			case CALENDAR_SELECTION:
				fragment = new GroupCalendarFragment ();
				ActionBar.Title = _title = _drawerOptions[CALENDAR_POSITION];
				_drawerList.SetItemChecked(CALENDAR_POSITION, true);
				break;
			//Message Board
			case MESSAGE_BOARD_SELECTION:
				fragment = new mBoardFragment ();
				ActionBar.Title = _title = _drawerOptions[MESSAGE_BOARD_POSITION];
				_drawerList.SetItemChecked(MESSAGE_BOARD_POSITION, true);
				break;
			//Member List
			case MEMBER_LIST_SELECTION:
				fragment = new GroupMembersFragment ();
				ActionBar.Title = _title = _drawerOptions [MEMBER_LIST_POSITION];
				_drawerList.SetItemChecked (MEMBER_LIST_POSITION, true);
				break;
			//Return to Dashboard
			case RETURN_TO_DASH_SELECTION:
				fragment = new GroupDashFragment ();
				ActionBar.Title = _title = _drawerOptions[GROUP_DASH_POSITION];
				_drawerList.SetItemChecked(GROUP_DASH_POSITION, true);
				break;
			};

			FragmentManager.BeginTransaction()
				.Replace(Resource.Id.content_frame, fragment)
				.Commit();
		}

		/**
		 * The group activity is created
		 */ 
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your application here
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.DrawerLayout);

			_title = _drawerTitle = Title;
			_drawerOptions = Resources.GetStringArray (Resource.Array.GroupOptArr);

			_drawerOptions [DASHBOARD_POSITION] = App.UsersName + _drawerOptions [DASHBOARD_POSITION];

			_drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			_drawerList = FindViewById<ListView>(Resource.Id.left_drawer);

			_drawer.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

			_drawerList.Adapter = new ArrayAdapter<string>(this,
				Resource.Layout.DrawerListItem, _drawerOptions);
			_drawerList.ItemClick += (sender, args) => SelectItem(args.Position);

			ActionBar.SetDisplayHomeAsUpEnabled(true);
			ActionBar.SetHomeButtonEnabled(true);

			//DrawerToggle is the animation that happens with the indicator next to the
			//ActionBar icon. You can choose not to use this.
			_drawerToggle = new MyActionBarDrawerToggle(this, _drawer,
				Resource.Drawable.ic_drawer_light,
				Resource.String.DrawerOpen,
				Resource.String.DrawerClose);

			//You can alternatively use _drawer.DrawerClosed here
			_drawerToggle.DrawerClosed += delegate
			{
				ActionBar.Title = _title;
				InvalidateOptionsMenu();
			};

			//You can alternatively use _drawer.DrawerOpened here
			_drawerToggle.DrawerOpened += delegate
			{
				ActionBar.Title = _drawerTitle;
				InvalidateOptionsMenu();
			};

			_drawer.SetDrawerListener(_drawerToggle);

			if (null == savedInstanceState) {
				if (return_to_activity) {
					SelectItem (return_positon);
					return_to_activity = false;
					return_positon = GROUP_DASH_POSITION;
				} else {
					SelectItem (1);
				}
			}
		}
		/**
		 * The item becomes selected based on the position 
		 */
		private void SelectItem(int position)
		{
			Fragment fragment = null;

			switch (position) {
			case LOGOUT_POSITION:
				fragment = new GroupLogoutFragment ();
				break;
			case DASHBOARD_POSITION:
				fragment = new LoadingFragment ();
				StartActivity (typeof(UserActivityWindow));
				break;
			case CALENDAR_POSITION:
				fragment = new GroupCalendarFragment();
				break;
			case MESSAGE_BOARD_POSITION:
				fragment = new mBoardFragment ();
				break;
			case GROUP_DASH_POSITION:
				fragment = new GroupDashFragment ();
				break;
			case ADD_MEMBER_POSITION:
				fragment = new AddMemberFragment ();
				break;
			case MEMBER_LIST_POSITION:
				fragment = new GroupMembersFragment ();
				break;
			};

			FragmentManager.BeginTransaction()
				.Replace(Resource.Id.content_frame, fragment)
				.Commit();

			_drawerList.SetItemChecked(position, true);
			ActionBar.Title = _title = _drawerOptions[position];
			_drawer.CloseDrawer(_drawerList);
		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			base.OnPostCreate(savedInstanceState);
			_drawerToggle.SyncState();
		}

		public override void OnConfigurationChanged(Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			_drawerToggle.OnConfigurationChanged(newConfig);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnPrepareOptionsMenu(IMenu menu)
		{
			var drawerOpen = _drawer.IsDrawerOpen(Resource.Id.left_drawer);
			return base.OnPrepareOptionsMenu(menu);
		}


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

