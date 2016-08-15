/*
 * FileName: AddMemberFragment.cs
 * Purpose: to add a new member to the current group the user is in
 * 
 * 
 */
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Parse;

namespace KOTUS
{
	public class AddMemberFragment: Fragment{


		private EditText add_email;//text to add email
		private Button submit_button;//button to confirm adding person
		private View rootView;//view to link fragment to activity

		public const int RETURN_TO_DASH_SELECTION = 5;

		/* Name: parse_add_user_to_group
		 * Purpose: add user in parse
		 * Parameters: string - email of person to be added
		 * Return: void
		 */
		private async void parse_add_user_to_group(string email){
			// find user with that email
			try{
				//sets user and sets email
				var new_user = await (from user in ParseUser.Query
					where user.Get<string>("email") == email
					select user).FindAsync();

				//sets user to this current group and adds user to the group
				foreach (ParseUser found_user in new_user){
					//found_user.ObjectId;
					System.Console.WriteLine(found_user.ObjectId);

					//create parse object of current group
					ParseQuery<ParseObject> query_name = ParseObject.GetQuery("Group");
					ParseObject current_group = await query_name.GetAsync(App.CurrentGroupID);

					//Add user to group user list
					current_group.AddToList("userList", email);
					await current_group.SaveAsync ();

					var invite = new ParseObject ("Invites");
					invite["groupID"] = App.CurrentGroupID;
					invite["userID"] = found_user.ObjectId;
					await invite.SaveAsync();
				}
			}catch(ParseException e){
				// couldnt find user
				System.Console.WriteLine("ERROR");
			}
		}

		/* Name: OnCreateView
		 * Purpose: Create view for addmember fragment
		 * Parameters: LayoutInflater p0 - layout to use, viewgroup - p1 view to be used
		 * 				Bundle p2 - bundle to create when activity started
		 * Return: view to place in activity
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			//Set title and view to be seen and place within groupactivity
			rootView = p0.Inflate(Resource.Layout.ForgotPass, p1, false);
			Activity.Title = "Add Member";
			var parent_activity = (GroupActivityWindow) this.Activity;

			//set header of view
			TextView header = rootView.FindViewById<TextView> (Resource.Id.textView1);
			header.Text = "Enter User Email:";

			//set top text and button
			add_email = rootView.FindViewById<EditText> (Resource.Id.editText1);
			submit_button = rootView.FindViewById<Button> (Resource.Id.button1);

			//when submit clicked will add user in parse and return to dash board
			submit_button.Click+= delegate {
				string user_to_find = add_email.Text;
				parse_add_user_to_group(user_to_find);
				parent_activity.group_dash_button_actions(RETURN_TO_DASH_SELECTION);
			};

			//view to be returned to screen
			return rootView;
		}
	}
}