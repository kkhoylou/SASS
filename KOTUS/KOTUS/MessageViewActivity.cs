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
	[Activity (Label = "View Message", NoHistory = true, Theme="@style/MyCustomTheme")]			
	public class MessageViewActivity : Activity
	{
		private Button editMessage;
		private Button deleteMessage;
		private TextView titleText;
		private TextView authorText;
		private TextView dateText;
		private TextView contentText;
		private string title;
		private string author;
		private DateTime creationTime;
		private string content;

		/* Name: getMessageContent()
         * Purpose: Queries Parse for the content and title of a post in order to set the values to be viewed.
         * Parameters: string messageID - the messageID used to find the desired message in parse
         * Return: void
         */
		private async void getMessageContent(string messageID){

			ParseQuery<ParseObject> query = ParseObject.GetQuery ("Message");
			var message = await query.GetAsync(messageID);

			title = message.Get<string>("Title");
			author = message.Get<string>("Author");
			creationTime = message.Get<DateTime>("DateTime");
			content = message.Get<string>("Content");

			await message.FetchAsync();

			titleText.Text = title;
			authorText.Text = author;
			dateText.Text = creationTime.ToString();
			contentText.Text = content;
			return;
		}

		/* Name: deleteMessageAsync()
         * Purpose: Queries Parse for the content and title of a post in order for the object to be deleted.
         * Parameters: string messageID - the messageID used to find the desired message in parse
         * Return: void
         */
		private async void deleteMessageAsync(string messageId){
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("Message");
			var message = await query.GetAsync (messageId);
			await message.DeleteAsync();

			ParseQuery<ParseObject> query_name = ParseObject.GetQuery("Group");
			ParseObject groupName = await query_name.GetAsync(App.CurrentGroupID);
			IList <string> currentList = groupName.Get<IList<string>> ("messages");
			IList <string> newList = new List<string> { };
			foreach (string id in currentList) {
				if (id == messageId) {
					continue;
				} else {
					newList.Add (id);
				}
			}

			groupName ["messages"] = newList;
			//groupName.RemoveAllFromList ("Message");

			await groupName.SaveAsync ();

			StartActivity(typeof(GroupActivityWindow));

		}

		/* Name: OnCreate()
         * Purpose: Creates the "View Message" window, where the user can view a post.
         * Parameters: Bundle bundle
         * Return: void
         */
		protected override void OnCreate (Bundle bundle)
		{	
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MessageView);
			//Need to put in this MessageID for "hello"
			authorText = (TextView)FindViewById (Resource.Id.author);
			contentText = (TextView)FindViewById (Resource.Id.content);
			titleText = (TextView)FindViewById (Resource.Id.title);
			dateText = (TextView)FindViewById (Resource.Id.date);

			getMessageContent (App.CurrentMessageID);

			//edit message button handling
			editMessage = FindViewById<Button> (Resource.Id.edit);
			editMessage.Click += delegate {
				StartActivity (typeof(MessageEditActivity));
			};
				
			//delete message button handling
			deleteMessage = FindViewById<Button> (Resource.Id.delete);
			deleteMessage.Click += delegate {
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(this);
				builder.SetTitle("Message Shall be Deleted");
				builder.SetMessage("Are you sure you would like to delete this message? Tap OK to continue.");
				builder.SetCancelable(true);
				builder.SetPositiveButton("OK",delegate {
					//need to put in this MessageID for hello
					deleteMessageAsync(App.CurrentMessageID);  
				});
				builder.SetNegativeButton("Cancel",delegate {
					//do nothing
				});
				builder.Show();
				//StartActivity(typeof(GroupActivityWindow));
			};
		}
	}
}