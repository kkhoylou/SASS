using Android.App;
using Android.Views;
using Android.OS;
using Parse;
using System;
using System.Collections.Generic;
using Android.Widget;

//Creates fragment for the User Calendar
//Grabs 

namespace KOTUS
{
	public class UserCalendarFragment: Fragment{

		//Grab view elements
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			rootView = p0.Inflate(Resource.Layout.UserCalendar, p1, false);
			var parent_activity = (UserActivityWindow) this.Activity;

			eventsList = rootView.FindViewById<ListView> (Resource.Id.listView1);
			calendar = rootView.FindViewById<CalendarView> (Resource.Id.calendarView1);
			current_date = GroupCalendarFragment.UnixTimeStampToDateTime(calendar.Date);
			parse_get_user_groups (); // grab all groups that the user belongs to

			calendar.DateChange += delegate {
				current_date = GroupCalendarFragment.UnixTimeStampToDateTime(calendar.Date);
				getTodaysEvents(current_date);
			};

			eventsList.ItemClick += async (sender, e) => {
				//perform action on click
				App.CurrentEventID = group_event_list[e.Position].getEventID();
				EditEventActivity.fromUser = true;
				parent_activity.calendarViewAction();

				//Console.WriteLine(group_name_list[e.Position]);
			};

			Activity.Title = "Calendar";
			return rootView;
		}

		private View rootView;
		private ListView eventsList;
		private CalendarView calendar; // the calendar display
		private IList<string> events_list; // stores all events to be shown to the user
		private IList<string> group_I_list; // stores the users groupids
		//private IList<string> group_name_list = new List<string>{}; // stores the users group names
		private IList<GroupEventObject> group_event_list = new List<GroupEventObject> {};
		private DateTime current_date;
		private IList<string> display_list = new List<string>{};

		//parse : grabs the object ID's for the group list
		private async void parse_get_user_groups(){
			try{
				ParseQuery<ParseUser> query = ParseUser.Query;
				ParseUser user = await query.GetAsync(ParseUser.CurrentUser.ObjectId);
				group_I_list = user.Get<IList<string>>("groupList");


				foreach (string groupID in group_I_list){
					parse_get_all_group_events(groupID);
				}

			}catch(Exception e){
				// do nothing
				System.Console.WriteLine("ERROR");
			}
		}

		//grab all the group events
		private async void parse_get_all_group_events(string group_id){
			ParseQuery<ParseObject> query = ParseObject.GetQuery("Group");
			ParseObject currentGroup = await query.GetAsync(group_id);
			string current_group_name = currentGroup.Get<string> ("groupName");
			events_list = currentGroup.Get<IList<string>>("events");

			foreach (string current_event in events_list)
			{
				ParseQuery<ParseObject> query_name = ParseObject.GetQuery("GroupEvent");
				ParseObject current_query = await query_name.GetAsync(current_event);
				GroupEventObject current_object = new GroupEventObject (current_query.Get<string>("eventName"), current_query.Get<string>("description"),
					current_query.Get<string>("location"), current_query.Get<string>("dateDisplayStart"), current_query.Get<string>("dateDisplayEnd"),
					current_query.Get<string> ("timeDisplayStart"), current_query.Get<string>("timeDisplayEnd"), current_query.ObjectId);

				current_object.setGroupName (current_group_name);
				group_event_list.Add (current_object);
			}

			getTodaysEvents(current_date);
		}

		//grabs the events for the current day
		private void getTodaysEvents (DateTime date){
			display_list = new List<string>{ };
			// build the list

			foreach (GroupEventObject current_event in group_event_list) {
				if (current_event.isInTimeFrame (current_date.ToString())) {
					//display_list.Add (current_event.getGroupName() + ": " + current_event.getTitle());
					display_list.Add (current_event.getGroupName() + ": " + current_event.getDateStart() +  " " +
						current_event.getTimeStart() + " - " + current_event.getDateEnd() + " " + 
						current_event.getTimeEnd() + " - " +  current_event.getTitle());
				}
			}

			// display the list of events
			eventsList.Adapter = new ArrayAdapter<string>(rootView.Context, Android.Resource.Layout.SimpleListItem1, display_list);
		}

	}
}