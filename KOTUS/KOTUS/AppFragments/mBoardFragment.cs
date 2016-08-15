/* FileName: mBoard.cs
 * Purpose: Create a message board of all messages in the current group
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Parse;

namespace KOTUS
{
	public class mBoardFragment : Fragment
	{


		private Button newMessage;//button to create a new button
		private IList <MessageObject> message_List = new List<MessageObject>{};//list of messageobjects
		private ListView list_View_Messages;//Listview of message
		private IList <string> messages;//list of string of messages
		private IList <string> display_list;//list to be displayed
		private View rootView;//view to be seen

		/* Name: create_Message_Board
		 * Purpose: to create the message baord from a lsit
		 * Parameters: none
		 * Return: void
		 */
		private async void create_Message_Board(){
			//find the current group parse object
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("Group");
			ParseObject currentGroup = await query.GetAsync(App.CurrentGroupID);

			//find the lsit of messages associated with it
			messages = currentGroup.Get<IList<string>>("messages");

			//create a new message object for each then add it to the list
			foreach (string message in messages) {
				ParseQuery<ParseObject> query_name = ParseObject.GetQuery ("Message");
				ParseObject current_query = await query_name.GetAsync (message);
				MessageObject current_object = new MessageObject (current_query.Get<string> ("Title"), current_query.Get<string> ("Content"),
					current_query.Get<DateTime> ("DateTime"), current_query.Get<string> ("Author"), 
					current_query.Get<string> ("AuthorID"), current_query.Get<string> ("GroupID"), current_query.ObjectId);
				message_List.Add (current_object);
			}

			display_list = new List<string>{ };

			//Create the view of each tab of each message
			foreach (MessageObject message in message_List) {
				display_list.Add (message.getTitle() + "\n" + message.getAuthor() + "  " + message.getDateTime());
			}
			//create adapter from list 
			list_View_Messages.Adapter = new ArrayAdapter<string>(rootView.Context, Android.Resource.Layout.SimpleListItem1, display_list);
		}

		/* Name: OnCreateView
		 * Purpose: view to be seen when created
		 * Parameters: LayoutInflator - p0 inflator for layour
		 * 				ViewGroup - p1 group view of layout
		 * 				bundle - p2 to create when fragment called
		 * Return: void
		 */
		public override View OnCreateView (LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			//set view with mboard axml layout
			rootView = p0.Inflate (Resource.Layout.mBoard, p1, false);
			var parent_activity = (GroupActivityWindow) this.Activity;

			//Set title to current group name
			rootView.FindViewById<TextView> (Resource.Id.textView1).Text = App.CurrentGroupName;

			//associate current listview
			list_View_Messages = rootView.FindViewById<ListView> (Resource.Id.listView1);

			//Calls create-Message_Board to store all Group message titles within message_Titles
			//and according DateTimes within message_DateTImes
			create_Message_Board ();

			//when item in list is clicked will take to individual view of message
			list_View_Messages.ItemClick += async (sender, e) => {
				App.CurrentMessageID = message_List [e.Position].getMessageID ();
				Fragment fragment = new LoadingFragment ();
				Intent intent = new Intent(Activity, typeof(MessageViewActivity));
				StartActivity(intent);
			};

			//find newMessage button id in axml file
			newMessage = rootView.FindViewById<Button> (Resource.Id.button1);

			//newMessage being clicked will take user to writemessageActivity
			newMessage.Click += delegate {
				Fragment fragment = new LoadingFragment();
				Intent intent = new Intent(Activity, typeof(MessageWriteActivity));
				StartActivity(intent);
			};

			//return view
			return rootView;

		}


		/* Name: MessageObject
		 * Purpose: to create a message object
		 */
		public class MessageObject{
			// private member functions
			private string Title;//title of message
			private string Content;//content within message
			private DateTime datetime;//datetime message created
			private string Author;//author of message
			private string AuthorId;//author's id
			private string GroupId;//group message is in
			private string messageID;//messages object id

			/* Name: MessageObject
			 * Purpose: Constructor to create object
			 * Parameters: all above
			 * Return: void
			 */
			public MessageObject(string Title, string Content, DateTime datetime, string Author, string AuthorId, string GroupId, string messageID){
				this.Title = Title;
				this.Content = Content;
				this.datetime = datetime;
				this.Author = Author;
				this.AuthorId = AuthorId;
				this.GroupId = GroupId;
				this.messageID = messageID;
			}

			/* Name: getMessageId
			 * Purpose:to get message id
			 * Parameters:none
			 * Return: string of messageID
			 */
			public string getMessageID(){
				return messageID;
			}

			/* Name: setTitle
			 * Purpose: set title
			 * Parameters: string - Title
			 * Return:void
			 */
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