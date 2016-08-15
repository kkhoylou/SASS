using System;
using System.Globalization;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Parse;

//Group Calendar Events List

namespace KOTUS
{
	public class GroupCalendarFragment: Fragment{

		private Button createEventButton; // the create event button
		private View rootView; // the layout view
		private ListView eventsList; // the list of events for selected date
		private CalendarView calendar; // the calendar display

		private IList<string> events_list;
		private IList<GroupEventObject> group_event_list = new List<GroupEventObject>{};
		private IList<string> display_list;


		//Grab all the events created and list them
		private async void parse_get_all_group_events(){
			ParseQuery<ParseObject> query = ParseObject.GetQuery("Group");
			ParseObject currentGroup = await query.GetAsync(App.CurrentGroupID);
			events_list = currentGroup.Get<IList<string>>("events");

			foreach (string current_event in events_list)
			{
				ParseQuery<ParseObject> query_name = ParseObject.GetQuery("GroupEvent");
				ParseObject current_query = await query_name.GetAsync(current_event);
				GroupEventObject current_object = new GroupEventObject (current_query.Get<string>("eventName"), current_query.Get<string>("description"),
					current_query.Get<string>("location"), current_query.Get<string>("dateDisplayStart"), current_query.Get<string>("dateDisplayEnd"),
					current_query.Get<string> ("timeDisplayStart"), current_query.Get<string>("timeDisplayEnd"), current_query.ObjectId);

				group_event_list.Add (current_object);

			}

			getTodaysEvents(current_date);
		}

		private void getTodaysEvents (DateTime date){
			display_list = new List<string>{ };
			// build the list

			foreach (GroupEventObject current_event in group_event_list) {
				if (current_event.isInTimeFrame (current_date.ToString())) {
					display_list.Add (current_event.getDateStart() +  " " + current_event.getTimeStart() + " - " +
						current_event.getDateEnd() + " " + current_event.getTimeEnd() + " - " +  current_event.getTitle() + ": " + current_event.getDescription());
				}
			}

			// display the list of events
			eventsList.Adapter = new ArrayAdapter<string>(rootView.Context, Android.Resource.Layout.SimpleListItem1, display_list);
		}
			

		//Set up the fragment for Group Calendar, grabs/list all events and create event
		private DateTime current_date;
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			rootView = p0.Inflate(Resource.Layout.Calendar, p1, false);
			var parent_activity = (GroupActivityWindow) this.Activity;

			eventsList = rootView.FindViewById<ListView> (Resource.Id.listView1);
			calendar = rootView.FindViewById<CalendarView> (Resource.Id.calendarView1);
			current_date = UnixTimeStampToDateTime (calendar.Date);

			parse_get_all_group_events();

			// when date is changed update the eventsList
			calendar.DateChange += delegate {
				current_date = UnixTimeStampToDateTime(calendar.Date);
				getTodaysEvents(current_date);
			};

			createEventButton = rootView.FindViewById<Button> (Resource.Id.button1);
			createEventButton.Click+= delegate {
				// call the parent function button action to switch activities
				parent_activity.group_calendar_button_action();
			};

			eventsList.ItemClick += async (sender, e) => {
				//perform action on click
				App.CurrentEventID = group_event_list[e.Position].getEventID();
				EditEventActivity.fromUser = false;
				parent_activity.group_calendar_view_action();

			};

			Activity.Title = "Group Calendar";
			return rootView;
		}

		public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
		{
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}
	}

	public class GroupEventObject{
		// private member functions
		private string dateDisplayEnd;
		private string dateDisplayStart;
		private string description;
		private string eventName;
		private string location;
		private string timeDisplayEnd;
		private string timeDisplayStart;
		private string groupName;
		private string eventID;

		private DateTime startDate;
		private DateTime endDate;

		// public member methods
		public GroupEventObject(string eventName, string description, string location, string dateStart, string dateEnd, string timeStart, string timeEnd, string eventID){
			this.eventName = eventName;
			this.description = description;
			this.location = location;
			this.dateDisplayStart = dateStart;
			this.dateDisplayEnd = dateEnd;
			this.timeDisplayStart = timeStart;
			this.timeDisplayEnd = timeEnd;
			this.eventID = eventID;

			// parse the dates
			startDate = DateTime.ParseExact (dateStart, "MM/dd/yyyy", CultureInfo.InvariantCulture);
			endDate = DateTime.ParseExact (dateEnd, "MM/dd/yyyy", CultureInfo.InvariantCulture);

		}
			
		public string getEventID(){
			return eventID;
		}
		public void setGroupName(string groupName){
			this.groupName = groupName;
		}

		public string getGroupName(){
			return groupName;
		}

		// checks if a date is between the start and end date inclusive
		// -1 is before 0 is equal 1 is after
		public bool isInTimeFrame(string date){
			string formatted_date_string = CalendarEvent.formatDateString (date);
			formatted_date_string = formatted_date_string.Substring (0, 10);
			DateTime date_to_compare = DateTime.ParseExact (formatted_date_string, "MM/dd/yyyy", CultureInfo.InvariantCulture);

			int start = startDate.CompareTo (date_to_compare);
			int end = endDate.CompareTo (date_to_compare);

			if (start == -1 && end == -1) {
				//Console.WriteLine ("1");
				return false;
			} else if (start == -1 && end == 0) {
				//Console.WriteLine ("2");
				return true;
			} else if (start == -1 && end == 1) {
				//Console.WriteLine ("3");
				return true;
			} else if (start == 0 && end == -1) {
				//Console.WriteLine ("4");
				return true;
			} else if (start == 0 && end == 0) {
				//Console.WriteLine ("5");
				return true;
			} else if (start == 0 && end == 1) {
				//Console.WriteLine ("6");
				return true;
			} else if (start == 1 && end == -1) {
				//Console.WriteLine ("7");
				return true;
			} else if (start == 1 && end == 0) {
				//Console.WriteLine ("8");
				return true;
			} else if (start == 1 && end == 1) {
				//Console.WriteLine ("9");
				return false;
			} else {
				//Console.WriteLine ("10");
				return true;
			}
				
		}

		public string getTitle(){
			return this.eventName;
		}

		public string getLocation(){
			return this.location;
		}

		public string getDescription(){
			return this.description;
		}

		public string getDateStart(){
			return this.dateDisplayStart;
		}

		public string getDateEnd(){
			return this.dateDisplayEnd;
		}

		public string getTimeStart(){
			return this.timeDisplayStart;
		}

		public string getTimeEnd(){
			return this.timeDisplayEnd;
		}
	}
}