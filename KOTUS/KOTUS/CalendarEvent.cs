using System;

using Android.App;
using Android.OS;
using Android.Widget;

using Parse;

//Calendar Events Page

namespace KOTUS
{
	[Activity (Label = "Add Event", NoHistory = true, Theme="@style/MyCustomTheme")]			
	public class CalendarEvent : Activity
	{
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
			SetContentView (Resource.Layout.CreateCalenderEvent); 

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
			Button button = FindViewById<Button> (Resource.Id.createEvent); 
			button.Click += delegate {
				parse_create_event(titleInput.Text); 
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

		//set method for time
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

		//formats date string from parse to be processed by the datetime class
		public static string formatDateString(string input_date){
			string[] parsed_string = input_date.Split ('/');

			if (parsed_string [0].Length == 1) {
				parsed_string [0] = "0" + parsed_string [0];
			}

			if (parsed_string [1].Length == 1) {
				parsed_string [1] = "0" + parsed_string [1];
			}

			return parsed_string [0] + "/" + parsed_string [1] + "/" + parsed_string [2];

		}

		//storing input variables in parse to be used in list
		public async void parse_create_event(string groupEvent_name){
			dateDisplayStart.Text = formatDateString(dateDisplayStart.Text); 
			dateDisplayEnd.Text = formatDateString (dateDisplayEnd.Text);

			var groupEvent = new ParseObject ("GroupEvent");
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
				ParseQuery<ParseObject> query = ParseObject.GetQuery("Group");
				ParseObject currentGroup = await query.GetAsync(App.CurrentGroupID);
				currentGroup.AddToList("events", groupEvent.ObjectId.ToString());
				await currentGroup.SaveAsync();

				GroupActivityWindow.return_to_activity = true;
				GroupActivityWindow.return_positon = CALENDAR_POSITION;
				StartActivity(typeof(GroupActivityWindow));
			}catch(ParseException e){
				// do nothing
			}
		}
	}
}
