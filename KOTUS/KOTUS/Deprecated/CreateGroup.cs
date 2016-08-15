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
using Android.Util;

using Parse;

namespace KOTUS
{
	[Activity (Label = "Create Group")]
	public class CreateGroup : Activity
	{
		private Button create_group_button;
		private EditText group_name;
		//private string privacy;

		private async void create_group_in_parse(string ID/*, string privacy*/ ){
	
			var group = new ParseObject ("Group");
			group["groupName"] = group_name.Text;
			//group["privacy"] = privacy;
			group["groupID"] = ID;
			IList<string> userList = new List<string>{ParseUser.CurrentUser.Username};
			group ["userList"] = userList;
			group ["admin"] = ParseUser.CurrentUser.Username;

			await group.SaveAsync();
		
			ParseUser.CurrentUser.AddToList("groupList",ID);
			await ParseUser.CurrentUser.SaveAsync ();
		}

		/*private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			privacy = String.Format ("{0}", spinner.GetItemAtPosition (e.Position));
		}*/

		private bool check_groupName(){
			//check if valid name inputted
			if (group_name.Text.Length == 0) {
				return false;
			} else
				return true;
		}

		// I COPIED THIS FROM STACK OVERFLOW TO CREATE A RANDOM ID
		string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") {
			if (length < 0) throw new ArgumentOutOfRangeException("length", "length cannot be less than zero.");
			if (string.IsNullOrEmpty(allowedChars)) throw new ArgumentException("allowedChars may not be empty.");

			const int byteSize = 0x100;
			var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
			if (byteSize < allowedCharSet.Length) throw new ArgumentException(String.Format("allowedChars may contain no more than {0} characters.", byteSize));

			// Guid.NewGuid and System.Random are not particularly random. By using a
			// cryptographically-secure random number generator, the caller is always
			// protected, regardless of use.
			using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider()) {
				var result = new StringBuilder();
				var buf = new byte[128];
				while (result.Length < length) {
					rng.GetBytes(buf);
					for (var i = 0; i < buf.Length && result.Length < length; ++i) {
						// Divide the byte into allowedCharSet-sized groups. If the
						// random value falls into the last group and the last group is
						// too small to choose from the entire allowedCharSet, ignore
						// the value in order to avoid biasing the result.
						var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
						if (outOfRangeStart <= buf[i]) continue;
						result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
					}
				}
				return result.ToString();
			}
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// set the current layout
			SetContentView (Resource.Layout.GroupCreate);

			create_group_button = FindViewById<Button> (Resource.Id.groupCreateButton);
			group_name = FindViewById<EditText> (Resource.Id.groupName);
			/*Spinner privacy_setting = FindViewById<Spinner> (Resource.Id.privacy_setting);

			privacy_setting.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource (
				              this, Resource.Array.privacy_array, Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			privacy_setting.Adapter = adapter;*/

			create_group_button.Click += delegate {
				Console.WriteLine ("Create Group Button Pressed!!");

				//Check if valid Group Name then create group, parse stuff, and generate group id, got to Group Dash
				if(check_groupName()){
					//create group id number
					var groupID = RandomString(12);
					//add object to parse
					create_group_in_parse (groupID/*, privacy*/);
					//go to group dashboard
					StartActivity (typeof(GroupDashActivity)); 				
				} else {
					//error message comes up
					group_name.SetError ("Create a valid Group Name!",  Resources.GetDrawable(Resource.Drawable.error));	
				}					
			};			

		}
	}
}
				
