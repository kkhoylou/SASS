using System;

using Android.App;
using Android.OS;
using Android.Widget;

using System.Collections.Generic;

using Parse;



namespace KOTUS
{
	[Activity (Label = "Edit Event", NoHistory = true, Theme="@style/MyCustomTheme")]			
	public class EditEventActivity : Activity
	{
		public static bool fromUser = false;

		private TextView dateDisplayStart;
		private TextView dateDisplayEnd;
		private Button pickDateStart;
		private Button pickDateEnd; 
		private DateTime dateStart;
		private DateTime dateEnd; 
		private TextView timeDisplayStart;
		private TextView timeDisplayEnd;
		private Button pickTimeStart;
		private Button pickTimeEnd; 
		private EditText titleInput;
		private EditText location;
		private EditText description; 
		private int hourStart;
		private int minuteStart;
		private int hourEnd;
		private int minuteEnd; 

		private const int CALENDAR_POSITION = 2;

		const int DATE_START_DIALOG_ID = 0;
		const int DATE_END_DIALOG_ID = 1;
		const int TIME_START_DIALOG_ID = 2; 
		const int TIME_END_DIALOG_ID = 3; 

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.EditCalenderEvent); 

			// capture our View elements
			dateDisplayStart = FindViewById<TextView> (Resource.Id.dateDisplayStart);
			dateDisplayEnd = FindViewById<TextView> (Resource.Id.dateDisplayEnd);
			pickDateStart = FindViewById<Button> (Resource.Id.pickDateStart);
			pickDateEnd = FindViewById<Button> (Resource.Id.pickDateEnd);
			timeDisplayStart = FindViewById<TextView> (Resource.Id.timeDisplayStart); 
			timeDisplayEnd = FindViewById<TextView> (Resource.Id.timeDisplayEnd);	
			pickTimeStart = FindViewById<Button> (Resource.Id.pickTimeStart); 
			pickTimeEnd = FindViewById<Button> (Resource.Id.pickTimeEnd);
			titleInput = FindViewById<EditText>(Resource.Id.title); 
			location = FindViewById<EditText> (Resource.Id.locationText);
			description = FindViewById<EditText> (Resource.Id.DescriptionText);

			// add a click event handler to the button
			pickDateStart.Click += delegate { ShowDialog(DATE_START_DIALOG_ID); };
			pickDateEnd.Click += delegate { ShowDialog(DATE_END_DIALOG_ID); };

			// Add a click listener to the button
			pickTimeStart.Click += (o, e) => ShowDialog (TIME_START_DIALOG_ID);
			pickTimeEnd.Click += (o, e) => ShowDialog(TIME_END_DIALOG_ID);


			// get the current date
			dateStart = DateTime.Today;
			dateEnd = DateTime.Today;


			// Get the current time
			hourStart = DateTime.Now.Hour;
			minuteStart = DateTime.Now.Minute;



			hourEnd = DateTime.Now.Hour;
			minuteEnd = DateTime.Now.Minute; 

			// display the current date (this method is below)
			UpdateDisplay ();

			parse_get_event_content ();

			Button edit = FindViewById<Button> (Resource.Id.editEvent); 
			edit.Click += delegate {
				parse_edit_event(titleInput.Text); 
			};
			Button cancel = FindViewById<Button> (Resource.Id.cancel);
			cancel.Click+= delegate {
				if(fromUser == false){
					GroupActivityWindow.return_to_activity = true;
					GroupActivityWindow.return_positon = CALENDAR_POSITION;
					StartActivity(typeof(GroupActivityWindow));
				} else {
					// return to user dash
					StartActivity(typeof(UserActivityWindow));
				}
			};

			Button delete = FindViewById<Button> (Resource.Id.delete);
			delete.Click+= delegate {
				parse_delete_event();
			};
		}

		// updates the date in the TextView
		private void UpdateDisplay ()
		{
			dateDisplayStart.Text = dateStart.ToString ("d");
			dateDisplayEnd.Text = dateEnd.ToString ("d");

			string timeStart = string.Format ("{0}:{1}", hourStart, minuteStart.ToString ().PadLeft (2, '0'));
			string timeEnd = string.Format ("{0}:{1}", hourEnd, minuteEnd.ToString ().PadLeft (2, '0'));

			timeDisplayStart.Text = timeStart;
			timeDisplayEnd.Text = timeEnd; 

		}

		private void StartTimePickerCallback (object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			hourStart = e.HourOfDay;
			minuteStart = e.Minute;

			UpdateDisplay ();
		}

		private void EndTimePickerCallback (object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			hourEnd = e.HourOfDay;
			minuteEnd = e.Minute;
			UpdateDisplay ();
		}

		// the event received when the user "sets" the date in the dialog
		void OnStartDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			this.dateStart = e.Date;
			UpdateDisplay ();
		}

		// the event received when the user "sets" the date in the dialog
		void OnEndDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			this.dateEnd = e.Date; 
			UpdateDisplay ();
		}

		protected override Dialog OnCreateDialog (int id)
		{

			switch (id) {
			case DATE_START_DIALOG_ID:
				return new DatePickerDialog (this, OnStartDateSet, dateStart.Year, dateStart.Month - 1, dateStart.Day); 
			case DATE_END_DIALOG_ID:
				return new DatePickerDialog (this, OnEndDateSet, dateEnd.Year, dateEnd.Month - 1, dateEnd.Day); 
			case TIME_START_DIALOG_ID:
				return new TimePickerDialog (this, StartTimePickerCallback, hourStart, minuteStart, true);
			case TIME_END_DIALOG_ID:
				return new TimePickerDialog (this, EndTimePickerCallback, hourEnd, minuteEnd, true);
			}
			return null;
		}

		public static string formatDateString(string input_date){
			string[] parsed_string = input_date.Split ('/');
			//string[] end = dateDisplayEnd.Text.Split ('/');

			if (parsed_string [0].Length == 1) {
				parsed_string [0] = "0" + parsed_string [0];
			}

			if (parsed_string [1].Length == 1) {
				parsed_string [1] = "0" + parsed_string [1];
			}

			return parsed_string [0] + "/" + parsed_string [1] + "/" + parsed_string [2];

		}

		public async void parse_get_event_content(){
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("GroupEvent");
			var current_event = await query.GetAsync(App.CurrentEventID);

			dateDisplayStart.Text = current_event.Get<string> ("dateDisplayStart");
			dateDisplayEnd.Text = current_event.Get<string> ("dateDisplayEnd");
			timeDisplayStart.Text = current_event.Get<string> ("timeDisplayStart");
			timeDisplayEnd.Text = current_event.Get<string> ("timeDisplayEnd");
			titleInput.Text = current_event.Get<string> ("title");
			location.Text = current_event.Get<string> ("location");
			description.Text = current_event.Get<string> ("description");

		}

		public async void parse_delete_event(){
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("GroupEvent");
			var current_event = await query.GetAsync (App.CurrentEventID);
			await current_event.DeleteAsync();

			ParseQuery<ParseObject> query_name = ParseObject.GetQuery("Group");
			ParseObject groupName = await query_name.GetAsync(App.CurrentGroupID);
			IList <string> current_list = groupName.Get<IList<string>> ("events");
			IList <string> new_list = new List<string> { };
			foreach (string event_id in current_list) {
				if (event_id == App.CurrentEventID) {
					continue;
				} else {
					new_list.Add (event_id);
				}
			}

			groupName ["events"] = new_list;
			//groupName.RemoveAllFromList ("Message");

			await groupName.SaveAsync ();

			if (fromUser == false) {
				GroupActivityWindow.return_to_activity = true;
				GroupActivityWindow.return_positon = CALENDAR_POSITION;
				StartActivity (typeof(GroupActivityWindow));
			} else {
				StartActivity (typeof(UserActivityWindow));
			}
		}

		public async void parse_edit_event(string groupEvent_name){
			dateDisplayStart.Text = formatDateString(dateDisplayStart.Text); 
			dateDisplayEnd.Text = formatDateString (dateDisplayEnd.Text);

			//var groupEvent = new ParseObject ("GroupEvent");
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("GroupEvent");
			var groupEvent = await query.GetAsync(App.CurrentEventID);
			groupEvent["eventName"] = groupEvent_name;

			groupEvent ["dateDisplayStart"] = dateDisplayStart.Text; 
			groupEvent ["dateDisplayEnd"] = dateDisplayEnd.Text;
			groupEvent ["timeDisplayStart"] = timeDisplayStart.Text;
			groupEvent ["timeDisplayEnd"] = timeDisplayEnd.Text;
			groupEvent ["title"] = titleInput.Text; 
			groupEvent ["location"] = location.Text; 
			groupEvent ["description"] = description.Text;
			groupEvent ["groupID"] = App.CurrentGroupID;

			try{
				await groupEvent.SaveAsync();
				if(fromUser == false){
					GroupActivityWindow.return_to_activity = true;
					GroupActivityWindow.return_positon = CALENDAR_POSITION;
					StartActivity(typeof(GroupActivityWindow));
				} else {
					StartActivity(typeof(UserActivityWindow));
				}
			}catch(ParseException e){
				// do nothing
			}
		}
	}
}
