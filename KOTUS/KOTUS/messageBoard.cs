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


using Parse;


namespace KOTUS
{
	[Activity (Label = "Message Board")]//Maybe add MainLauncher =true    ,      Icon = "@drawable/icon"    
	public class messageBoard : Activity
	{

		private Button newMessage;
		private IList <MessageObject> message_List = new List<MessageObject>{};
		private ListView list_View_Messages;
		private IList <string> messages;
		private IList <string> display_list;


		private async void create_Message_Board(){
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("Group");
			ParseObject currentGroup = await query.GetAsync(App.CurrentGroupID);
			messages = currentGroup.Get<IList<string>>("messages");

			foreach (string message in messages) {
				ParseQuery<ParseObject> query_name = ParseObject.GetQuery ("Message");
				ParseObject current_query = await query_name.GetAsync (message);
				MessageObject current_object = new MessageObject (current_query.Get<string> ("Title"), current_query.Get<string> ("Content"),
					                                  current_query.Get<DateTime> ("DateTime"), current_query.Get<string> ("Author"), 
					current_query.Get<string> ("AuthorID"), current_query.Get<string> ("GroupID"), current_query.ObjectId);
				message_List.Add (current_object);
			}

			display_list = new List<string>{ };
			// build the list

			foreach (MessageObject message in message_List) {
				//if (me.isInTimeFrame (current_date.ToString())) {
				display_list.Add (message.getTitle() + "\n" + message.getAuthor() + "  " + message.getDateTime());
				//}
			}
			list_View_Messages.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, display_list);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//Set ContentView to messageBoard.axml
			SetContentView (Resource.Layout.messageBoard);

			TextView groupNameTextView = FindViewById<TextView> (Resource.Id.textView1);
			groupNameTextView.Text = App.CurrentGroupName;
			list_View_Messages = FindViewById<ListView> (Resource.Id.listView1);
			//Calls create-Message_Board to store all Group message titles within message_Titles
			//and according DateTimes within message_DateTImes
			create_Message_Board ();

			//find newMessage button id in axml file
			newMessage = FindViewById<Button> (Resource.Id.button1);

			//newMessage being clicked will take user to writemessageActivity
			newMessage.Click += delegate {
				Intent intent = new Intent(this,typeof(writeMessageActivity));
				intent.PutExtra("groupID",App.CurrentGroupID);
				intent.PutExtra("groupName",App.CurrentGroupName);
				StartActivity (intent);
			};

			list_View_Messages.ItemClick += async (sender, e) => {
				App.CurrentMessageID = message_List [e.Position].getMessageID ();
				StartActivity (typeof(MessageViewActivity));
			};
		}

		public class MessageObject{
		// private member functions
			private string Title;
			private string Content;
			private DateTime datetime;
			private string Author;
			private string AuthorId;
			private string GroupId;
			private string messageID;

			// public memeber methods
			public MessageObject(string Title, string Content, DateTime datetime, string Author, string AuthorId, string GroupId, string messageID){
				this.Title = Title;
				this.Content = Content;
				this.datetime = datetime;
				this.Author = Author;
				this.AuthorId = AuthorId;
				this.GroupId = GroupId;
				this.messageID = messageID;
			}

			public string getMessageID(){
				return messageID;
			}

		public void setTitle(string Title){
			this.Title = Title;
		}

		public string getTitle(){
			return Title;
		}

		public string getContent(){
			return this.Content;
		}

		public DateTime getDateTime(){
			return this.datetime;
		}

		public string getAuthor(){
			return this.Author;
		}

		public string getAuthorId(){
			return this.AuthorId;
		}

		public string getGroupId(){
			return this.GroupId;
		}
	}
	}
}