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
using Android.Util;

namespace KOTUS
{
	public class CreateGroupFragment: Fragment{

		/* Name: OnCreateView
		 * Purpose: Create view for createGroup fragment
		 * Parameters: LayoutInflater p0 - layout to use, viewgroup - p1 view to be used
		 * 				Bundle p2 - bundle to create when activity started
		 * Return: view to place in activity
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			var rootView = p0.Inflate (Resource.Layout.GroupCreate, p1, false);
			var parent_activity = (UserActivityWindow) this.Activity;
			Button create_group_button = rootView.FindViewById<Button> (Resource.Id.groupCreateButton);
			EditText group_name = rootView.FindViewById<EditText> (Resource.Id.groupName);

			create_group_button.Click += delegate {
				Console.WriteLine ("Create Group Button Pressed!!");

				//Check if valid Group Name then create group, parse stuff, and generate group id, got to Group Dash
				if(check_groupName(group_name.Text)){
					parent_activity.parseCreateGroup (group_name.Text);				

				} else {
					//error message comes up
					group_name.SetError ("Create a valid Group Name!",  Resources.GetDrawable(Resource.Drawable.error));	
				}					
			};			

			Activity.Title = "Create Group";
			return rootView;
		}

		/* Name: check_groupName
		 * Purpose: check if valid group name
		 * Parameters: string group_name - name of group
		 * Return: true if length of group name is greater than 0
		 * 			false if equals 0
		 */
		private bool check_groupName(string group_name){
			//check if valid name inputted
			if (group_name.Length == 0) {
				return false;
			} else
				return true;
		}
	}
}