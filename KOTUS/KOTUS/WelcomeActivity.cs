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
	[Activity (Icon = "@drawable/icon", NoHistory=true)]
	public class WelcomeActivity : Activity
	{
		private const int LOGIN_POSITION = 0;
		private const int CREATE_ACCOUNT_POSITION = 1;
		private const int FORGOT_PASSWORD_POSITION = 2;
		private const int LOADING = 999;

		// Expanding Drawer Components
		private DrawerLayout _drawer;
		private MyActionBarDrawerToggle _drawerToggle;
		private ListView _drawerList;
		private string _drawerTitle;
		private string _title;
		private string[] _drawerOptions;

		public async void parse_create_user(string first_name, string last_name, string email, string phone, string pass){
			var user = new ParseUser()
			{
				Username = email,
				Password = pass,
				Email = email
			};

			user["phone"] = phone;
			user ["firstName"] = first_name;
			user ["lastName"] = last_name;
			//Im tyring to make an empty groupList so whenever a user creates/joins a group
			//the group id will be added to this list
			IList<string> groupList = new List<string>{};
			user ["groupList"] = groupList;

			try{
				await user.SignUpAsync();
				parse_login_user(email, pass);

			} catch (ParseException e){
				// user account could not be created
				if (e.Code == ParseException.ErrorCode.EmailTaken || e.Code == ParseException.ErrorCode.UsernameTaken) {
					RunOnUiThread (() => {
						AlertDialog.Builder builder;
						builder = new AlertDialog.Builder (this);
						builder.SetTitle ("Error");
						builder.SetMessage ("E-Mail address already in use!");
						builder.SetCancelable (false);
						builder.SetPositiveButton ("OK", delegate {
							// do nothing
						});
						builder.Show ();
					}
					);
				} else {
					RunOnUiThread(() =>
						{
							AlertDialog.Builder builder;
							builder = new AlertDialog.Builder(this);
							builder.SetTitle("Error");
							builder.SetMessage("Unable to create account. Please try again.");
							builder.SetCancelable(false);
							builder.SetPositiveButton("OK", delegate {
								// do nothing
							});
							builder.Show();
						}
					);
				}
			}
		}

		public async void parse_login_user(string username, string password){
			try
			{
				Fragment fragment = new LoadingFragment();


				FragmentManager.BeginTransaction()
					.Replace(Resource.Id.content_frame, fragment)
					.Commit();

				await ParseUser.LogInAsync(username, password);
				// Login was successful.
				try{
					ParseQuery<ParseUser> query = ParseUser.Query;
					ParseUser user = await query.GetAsync(ParseUser.CurrentUser.ObjectId);
					App.UsersName = user.Get<string>("firstName") + "'s ";
					App.UserFirstName = user.Get<string>("firstName");
					App.UsersLastName = user.Get<string>("lastName");
					StartActivity(typeof(UserActivityWindow)); 

				}catch(ParseException e){
					// do nothing
					Console.WriteLine("ERROR - Cannot fetch user's name");
					StartActivity(typeof(UserActivityWindow)); 
				}


			}
			catch (ParseException e)
			{
				// The login failed. Check the error to see why.
				RunOnUiThread(() =>
					{
						AlertDialog.Builder builder;
						builder = new AlertDialog.Builder(this);
						builder.SetTitle("Error");
						builder.SetMessage("Unable to log in. Please try again.");
						builder.SetCancelable(false);
						builder.SetPositiveButton("OK", delegate {
							SelectItem(LOGIN_POSITION);
						});
						builder.Show();
					}
				);
			}
		}

		public async void parse_forgot_pass(string email){
			try{
				await ParseUser.RequestPasswordResetAsync(email);
				RunOnUiThread(() =>
					{
						AlertDialog.Builder builder;
						builder = new AlertDialog.Builder(this);
						builder.SetTitle("Forgot Password");
						builder.SetMessage("Password Reset Email Sent!");
						builder.SetCancelable(false);
						builder.SetPositiveButton("OK", delegate {
							SelectItem(LOGIN_POSITION);
						});
						builder.Show();
					}
				);
			} catch (ParseException e){
				// email not found...
			}
		}

		private void SelectItem(int position)
		{
			Fragment fragment = null;

			switch (position) {
			case LOGIN_POSITION:
				fragment = new LoginFragment ();
				break;
			case CREATE_ACCOUNT_POSITION:
				fragment = new SignUpFragment ();
				break;
			case FORGOT_PASSWORD_POSITION:
				fragment = new ForgotPassFragment ();
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
			//MenuInflater.Inflate(Resource.Menu.main, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnPrepareOptionsMenu(IMenu menu)
		{
			var drawerOpen = _drawer.IsDrawerOpen(Resource.Id.left_drawer);
			//menu.FindItem(Resource.Id.action_websearch).SetVisible(!drawerOpen);
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
			
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			if (ParseUser.CurrentUser != null) {
				StartActivity (typeof(UserActivityWindow));
			}

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.DrawerLayout);

			_title = _drawerTitle = Title;
			_drawerOptions = Resources.GetStringArray (Resource.Array.LoginArray);
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

			if (null == savedInstanceState)
				SelectItem(0);

		}
	}
}


