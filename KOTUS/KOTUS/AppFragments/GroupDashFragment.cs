using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace KOTUS
{
	public class GroupDashFragment: Fragment{

		private const int ADMIN_DASH_SELECTION = 0;
		private const int INVITE_MEMBER_SELECTION = 1;
		private const int CALENDAR_SELECTION = 2;
		private const int MESSAGE_BOARD_SELECTION = 3;
		private const int FILE_UPLOAD_SELECTION = 4;
		private const int MEMBERS_LIST_SELECTION = 6;

		/* Name: OnCreateView
		 * Purpose: Create view for groupDash fragment
		 * Parameters: LayoutInflater p0 - layout to use, viewgroup - p1 view to be used
		 * 				Bundle p2 - bundle to create when activity started
		 * Return: view to place in activity
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			var rootView = p0.Inflate (Resource.Layout.GroupDash, p1, false);
			var parent_activity = (GroupActivityWindow) this.Activity;

			TextView group_title_text = rootView.FindViewById<TextView> (Resource.Id.groupName);
			// This is where you can set the page to show the groups name
			// just replace the "" with some mechanism for determining the current group name
			group_title_text.Text = App.CurrentGroupName;

			Button invite_member_button = rootView.FindViewById<Button> (Resource.Id.inviteButton);
			invite_member_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Invite Memeber Button Pressed!!");
				parent_activity.group_dash_button_actions(INVITE_MEMBER_SELECTION);
			};

			Button calendar_button = rootView.FindViewById<Button> (Resource.Id.calendarButton);
			calendar_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Calendar Button Pressed!!");
				parent_activity.group_dash_button_actions(CALENDAR_SELECTION);
			};

			Button message_board_button = rootView.FindViewById<Button> (Resource.Id.messageButton);
			message_board_button.Click += delegate {
				// this is where you tell it what to do when the button is pressed
				Console.WriteLine("Message Board Button Pressed!!");
				parent_activity.group_dash_button_actions(MESSAGE_BOARD_SELECTION);
			};

			Button group_members_button = rootView.FindViewById<Button> (Resource.Id.membersButton);
			group_members_button.Click += delegate {
				Console.WriteLine("Group Members Button Pressed!");
				parent_activity.group_dash_button_actions(MEMBERS_LIST_SELECTION);
			};

			Activity.Title = "Group Dashboard";
			return rootView;
		}

	}
}