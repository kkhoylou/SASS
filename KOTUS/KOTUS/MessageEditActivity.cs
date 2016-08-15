using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Parse;

namespace KOTUS
{
	[Activity (Label = "Edit Message", NoHistory = true, Theme="@style/MyCustomTheme")]			
	public class MessageEditActivity : Activity
	{
		private Button submitButton;
		private Button cancelButton;
		private EditText editContent;
		private EditText editTitle;
		private TextView authorText;
		private TextView dateText;
		private bool messageEditSuccess;
		private string title;
		private string author;
		private DateTime creationTime;
		private string content;

		/* Name: getMessageContent()
         * Purpose: Queries Parse for the content and title of a post in order to set the values that can be edited.
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

			authorText.Text = author;
			editContent.Text = content;
			editTitle.Text = title;
			dateText.Text = creationTime.ToString();
		}

		/* Name: SetMessageContent()
         * Purpose: Creates a new message Parse Object, assigning it a title,
         *             content, author, datetime, and GroupID and saves it
         * Return: Void, saves it in Parse
         */
		private async void setMessageContent(string messageID){

			ParseQuery<ParseObject> query = ParseObject.GetQuery ("Message");
			var message = await query.GetAsync(messageID);

			message["Title"] = title;
			message ["Content"] = content;
			message ["DateTime"] = DateTime.Now;

			try{
				await message.SaveAsync();
				messageEditSuccess = true;
				StartActivity(typeof(GroupActivityWindow));

			} catch(ParseException e){
				messageEditSuccess = false;
			}
		}


		/* Name: checkSubject()
         * Purpose: determines if title is null or not
         * Parameters: None
         * Return: bool - true if not null, false if empty or over 25 character
         */
		private bool checkSubject(){

			if (editTitle.Length () > 0 && editTitle.Length () < 25) {
				return true;
			} else {
				return false;
			}
		}

		/* Name: checkContent()
         * Purpose: determines if content is null or not
         * Parameters: None
         * Return: bool - true if not null, false if empty or over 200 characters
         */
		private bool checkContent(){
			if (editContent.Length () > 0 && editContent.Length () < 200) {
				return true;
			} else {
				return false;
			}
		}

		/* Name: OnCreate()
         * Purpose: Creates the "Edit Message" window, where the user can change the content and title of a post.
         * Parameters: Bundle bundle
         * Return: void
         */
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MessageEdit);

			authorText = FindViewById <TextView>(Resource.Id.author);
			dateText = FindViewById <TextView> (Resource.Id.date);
			editContent = FindViewById <EditText> (Resource.Id.editContent);
			editTitle = FindViewById <EditText> (Resource.Id.editTitle);
			submitButton = FindViewById<Button> (Resource.Id.submit);
			cancelButton = FindViewById<Button> (Resource.Id.cancel);

			getMessageContent (App.CurrentMessageID);

			//Live updates title string as it is edited.
			editContent.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
				content = e.Text.ToString();
			};

			//Live updates content string as it it edited.
			editTitle.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
				title = e.Text.ToString();
			};

			//Handles submit button press
			submitButton.Click += delegate {
				//Makes sure title and content are present. If so, sends values to parse to update backend and returns user to message board.
				if(checkContent() && checkSubject()){
					setMessageContent(App.CurrentMessageID);
				}
				//Throws error dialogue box if nothing has been entered. 
				else{
					RunOnUiThread(() =>
						{
							AlertDialog.Builder builder;
							builder = new AlertDialog.Builder(this);
							builder.SetTitle("Error");
							builder.SetMessage("Title and/or Content field are null.");
							builder.SetCancelable(false);
							builder.SetPositiveButton("OK", delegate {
								// do nothing
							});
							builder.Show();
						}
					);
				}
			};
			//Handles cancel button click.
			cancelButton.Click += delegate {
				StartActivity (typeof(MessageViewActivity));
			};
		}
	}
}