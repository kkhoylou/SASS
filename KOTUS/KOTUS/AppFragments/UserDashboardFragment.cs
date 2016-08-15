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
	public class UserDashboardFragment: Fragment{


		/*
		 * This method checks whether the user was invited to a group and subsequently
		 * adds the user to the group's member list and adds the group to the
		 * group list in User class in Parse.
		 */
		private async void parse_check_invites(){
			try{
				var query = from invites in ParseObject.GetQuery("Invites")
						where invites.Get<string>("userID") == Parse.ParseUser.CurrentUser.ObjectId
					select invites;
				IEnumerable<ParseObject> results = await query.FindAsync();

				/*
				var invite_list = await (from invites in ParseObject.Query
					where invites.Get<string>("userID") == Parse.ParseUser.CurrentUser.ObjectId
					select invites).FindAsync();
				*/
				/*ParseQuery<ParseObject> query_name = ParseObject.GetQuery("Invites");
				ParseObject invite = await query_name.GetAsync("");
				*/

				foreach (ParseObject invite in results){
					string group_to_add = invite.Get<string>("groupID");
					await invite.DeleteAsync();
					ParseUser.CurrentUser.AddToList("groupList", group_to_add);
					await ParseUser.CurrentUser.SaveAsync ();
				}

				parse_get_user_groups();

			}catch (ParseException e){
				
			}
		}

		/*
		 * This method gets the list of groups that the currently logged in user is in and
		 * assigns it to the local variable to be used in this UserDashboardFragment and shown
		 * in the app.
		 */
		private async void parse_get_user_groups(){
			try{
				ParseQuery<ParseUser> query = ParseUser.Query;
				ParseUser user = await query.GetAsync(ParseUser.CurrentUser.ObjectId);
				group_I_list = user.Get<IList<string>>("groupList");

				Console.WriteLine("SYSTEM - User's Current Groups");
				foreach (string group in group_I_list)
				{
					ParseQuery<ParseObject> query_name = ParseObject.GetQuery("Group");
					ParseObject groupName = await query_name.GetAsync(group);
					group_name_list.Add(groupName.Get<string>("groupName"));
					Console.WriteLine(group);
				}

				group_lists = rootView.FindViewById<ListView>(Resource.Id.listView1);
				group_lists.Adapter = new ArrayAdapter<string>(rootView.Context, Android.Resource.Layout.SimpleListItem1, group_name_list);
				group_lists.ItemClick += async (sender, e) => {
					//perform action on click
					App.CurrentGroupID = group_I_list[e.Position];
					App.CurrentGroupName = group_name_list[e.Position];
					((UserActivityWindow) this.Activity).openGroup();
					//Console.WriteLine(group_name_list[e.Position]);
				};
			}catch(Exception e){
				// do nothing
				System.Console.WriteLine("ERROR");
			}
		}


		private View rootView;
		private ListView group_lists;
		private IList<string> group_I_list;
		private IList<string> group_name_list = new List<string>{};

		//private string[] groups = new string[] { "Vegetables","Fruits","Flower Buds","Legumes","Bulbs","Tubers" };
		private string[] groups;

		/*
		 * Assign layout design to the code.
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			rootView = p0.Inflate (Resource.Layout.UserDash, p1, false);
			//var parent_activity = (UserActivityWindow) this.Activity;


			//parse_get_user_groups ();
			parse_check_invites();



			Activity.Title = "Dashboard";
			return rootView;
		}

		/*
		 * This method determines what the user taps on the list of groups
		 * being displayed on the User Dashboard. Whatever group gets tapped
		 * on is the group that will be used to pass into starting the
		 * Group Dashbaord activity of the app.
		 */
		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = groups[e.Position];
			Android.Widget.Toast.MakeText(this.Activity, t, Android.Widget.ToastLength.Short).Show();
		}

	}
}