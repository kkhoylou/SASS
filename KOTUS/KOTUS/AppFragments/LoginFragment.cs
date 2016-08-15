using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Text.RegularExpressions;

namespace KOTUS
{
	public class LoginFragment: Fragment{

		/* Name: OnCreateView
		 * Purpose: Create the view for forgot password
		 * Parameters: LayoutInflator p0 - used to create lyout
		 * 				ViewGroup p1 - GroupView to use
		 * 				Bundle p2 - Bundle to use within
		 * Return: view of forgot password
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			//sets layour to login layout
			var rootView = p0.Inflate(Resource.Layout.Login, p1, false);
			rootView.FindViewById<Button> (Resource.Id.button1).Click += delegate {
				var parent_activity = (WelcomeActivity) this.Activity;

				//creates edit text for email field and user_password field
				EditText user_email_field = rootView.FindViewById<EditText>(Resource.Id.editText1);
				EditText user_password_field = rootView.FindViewById<EditText>(Resource.Id.editText2);

				//Checks to determind if email and password are valid and if so let's user into parse
				Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
				Match match = regex.Match (user_email_field.Text);
				if (!match.Success || user_password_field.Text.Length == 0) {
					if (!match.Success) {
						user_email_field.Error = "Invalid email address";
					}

					if (user_password_field.Text.Length == 0) {
						user_password_field.Error = "Please enter your password";
					}
				} else {
					parent_activity.parse_login_user(user_email_field.Text, user_password_field.Text);
				}
			};
			Activity.Title = "Log In";
			return rootView;
		}
	}
}