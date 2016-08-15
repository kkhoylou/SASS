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
	[Activity (Label = "New Message", NoHistory = true, Theme="@style/MyCustomTheme")]            
	public class MessageWriteActivity : Activity
	{

		private EditText messageTitle;
		private EditText messageContent;
		private Button cancelMessage;
		private Button createMessage;

		/* Name: create_new_Message()
         * Purpose: Creates a new message Parse Object, assigning it a title,
         *             content, author, datetime, and GroupID and saves it
         * Return: Void, saves it in Parse
         */
		private async void create_new_Message(){
			var message = new ParseObject ("Message");
			message["Title"] = messageTitle.Text;
			message ["Content"] = messageContent.Text;
			message ["Author"] = ParseUser.CurrentUser.Username;
			message ["AuthorID"] = ParseUser.CurrentUser.ObjectId;
			message ["DateTime"] = DateTime.Now;
			message ["GroupID"] = App.CurrentGroupID;


			try{
				await message.SaveAsync();
				ParseQuery<ParseObject> query = ParseObject.GetQuery("Group");
				ParseObject currentGroup = await query.GetAsync(App.CurrentGroupID);
				currentGroup.AddToList("messages",message.ObjectId.ToString());
				await currentGroup.SaveAsync();
				StartActivity(typeof(GroupActivityWindow));

			} catch(ParseException e){
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
					});
			}
		}


		/* Name: checkSubject()
         * Purpose: determines if title is null or not
         * Parameters: None
         * Return: bool - true if not null, false if empty or over 25 character
         */
		private bool checkSubject(){

			if (messageTitle.Length () > 0 && messageTitle.Length () < 35) {
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
			if (messageContent.Length () > 0) {
				return true;
			} else {
				return false;
			}
		}

		/* Name: OnCreate()
         * Purpose: Creates the "Write Message" window, where the user create a new message and post to the message board.
         * Parameters: Bundle bundle
         * Return: void
         */
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//Sets Screen to newMessage.axml
			SetContentView (Resource.Layout.MessageWrite);

			//Show Group Name
			TextView groupNameTextView = FindViewById<TextView> (Resource.Id.textView3);
			groupNameTextView.Text = App.CurrentGroupName;

			//Sets Text Editors to according ones in axml
			messageTitle = FindViewById<EditText> (Resource.Id.title);
			messageContent = FindViewById<EditText> (Resource.Id.content);

			//Initiliazes cancelMessage Button to cancel in axml
			cancelMessage = FindViewById<Button> (Resource.Id.button1);

			//If cancel pressed takes user back to messageBoard
			cancelMessage.Click += delegate {
				StartActivity(typeof(GroupActivityWindow));
			};

			//Initializes createMessage to create in axml
			createMessage = FindViewById<Button> (Resource.Id.create);

			//If pressed will create_new_message or if text fields are null will give user an error
			//message stating that texts fields are null
			createMessage.Click += delegate {

				if(checkContent() && checkSubject()){
					create_new_Message();
				}
				else{
					
				}
			};
		}


	}
}