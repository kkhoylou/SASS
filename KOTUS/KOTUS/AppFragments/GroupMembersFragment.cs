using Android.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.OS;
using Parse;

namespace KOTUS
{
	public class GroupMembersFragment: Fragment{

		//private fields for Group member fragment 
		private View rootView; // view to be seen 
		private IList<UserObjectClass> user_object_list = new List<UserObjectClass> {}; //list of user object
		private IList<string> user_name_list = new List<string> {}; //list of user name
		private TextView heading; // the text button 
		private ListView list; // the listview UI

		/* Name: parse_get_group_members
		 * Purpose: pull details about group members from parse and list them in the member list
		 * Parameters: None
		 * Return: void
		 */
		private async void parse_get_group_members(){
		
			try{
				//retrieve parseObject for the group member list
				ParseQuery<ParseObject> query_name = ParseObject.GetQuery("Group");
				ParseObject current_query = await query_name.GetAsync(App.CurrentGroupID);
				IList<string> group_user_list = current_query.Get<IList<string>>("userList");

				// for each email in the group_user_list, find user by email address 
				foreach (string email in group_user_list){
					var user_lookup = await (from user in ParseUser.Query
						where user.Get<string>("email") == email
						select user).FindAsync();

					// for each user in parse, create an user object for each user.
					foreach (ParseUser current_user in user_lookup){
						UserObjectClass current_user_object = new UserObjectClass(current_user.Get<string>("firstName"),
							current_user.Get<string>("lastName"), current_user.Get<string>("email"), 
							current_user.Get<string>("phone"), current_user.ObjectId);

						// add user object onto the list view
						user_object_list.Add(current_user_object);
						user_name_list.Add(current_user_object.getFullName());
					}
				}

				// create user list
				list.Adapter = new ArrayAdapter<string>(rootView.Context, Android.Resource.Layout.SimpleListItem1, user_name_list);
				list.ItemClick += async (sender, e) => {
					//perform action on click, show name, email, and phone number 
					var parent_activity = (GroupActivityWindow) this.Activity;
					parent_activity.group_member_list_alert("Name: " + user_object_list[e.Position].getFullName() + 
						"\nE-Mail: " + user_object_list[e.Position].getEMail() + "\nPhoneNumber: " +
						user_object_list[e.Position].getPhoneNumber());

				};
				
			}catch(ParseException e){
				// if couldnt find user, write ERROR
				System.Console.WriteLine ("ERROR");
			}
		}
		/* Name: OnCreateView
		 * Purpose: create the view for group member list
		 * Parameters: LayoutInflater: inflate the layout file 
		 *             ViewGroup: container for the layout file
		 *             Bundle: save instance state of the layout file
		 * Return: void
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			// inflate the layout of the loading page
			rootView = p0.Inflate (Resource.Layout.UserDash, p1, false);

			// modify the text and list UI for the member list
			rootView.FindViewById<TextView>(Resource.Id.textView1).Text = "Group Members:";
			list = rootView.FindViewById<ListView> (Resource.Id.listView1);

			// listing all the group member from parse database.
			parse_get_group_members ();


			Activity.Title = "Group Members";
			return rootView;
		}
	}

	public class UserObjectClass{
		// private fields of the user object class
		private string firstName;
		private string lastName;
		private string email;
		private string phoneNumber;
		private string userID;

		/* 
		 * Name: UserObjectClass()
		 * Purpose: create an user object
		 * Parameters: string firstName: first name for the user
		 *             string lastName: last name of the user
		 *             string email: email of the user
		 *             string phoneNumber: phone number of the user
		 *             string UserID: user ID 
		 * Return: string
		 */
		public UserObjectClass(string firstName, string lastName, string email, string phoneNumber, string userID){
			this.firstName = firstName;
			this.lastName = lastName;
			this.email = email;
			this.phoneNumber = phoneNumber;
			this.userID = userID;
		}
		/* 
		 * Name: getFirstName()
		 * Purpose: get the first name
		 * Parameters: None
		 * Return: string
		 */
		public string getFirstName(){
			return firstName;
		}

		/* 
		 * Name: getLastName()
		 * Purpose: get the last name
		 * Parameters: None
		 * Return: string
		 */
		public string getLastName(){
			return lastName;
		}

		/* 
		 * Name: getFullName()
		 * Purpose: get the full name
		 * Parameters: None
		 * Return: string
		 */
		public string getFullName(){
			return firstName + " " + lastName;
		}

		/* 
		 * Name: getEmail()
		 * Purpose: get the email address
		 * Parameters: None
		 * Return: string
		 */
		public string getEMail(){
			return email;
		}

		/* 
		 * Name: getPhoneNumber()
		 * Purpose: get the phone number
		 * Parameters: None
		 * Return: string
		 */
		public string getPhoneNumber(){
			return phoneNumber;
		}

		/* 
		 * Name: getUserID()
		 * Purpose: get UserID
		 * Parameters: None
		 * Return: string
		 */
		public string getUserID(){
			return userID;
		}

	}
}