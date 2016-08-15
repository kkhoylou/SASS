using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Text.RegularExpressions;

namespace KOTUS
{
	public class ForgotPassFragment: Fragment{

		/* Name: OnCreateView
		 * Purpose: Create the view for forgot password
		 * Parameters: LayoutInflator p0 - used to create lyout
		 * 				ViewGroup p1 - GroupView to use
		 * 				Bundle p2 - Bundle to use within
		 * Return: view of forgot password
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			//Initilaize to forgotpass layout
			var rootView = p0.Inflate(Resource.Layout.ForgotPass, p1, false);
			Activity.Title = "Forgot My Password";
			var parent_activity = (WelcomeActivity) this.Activity;

			//initialize button and edittext of emial
			Button button = rootView.FindViewById<Button> (Resource.Id.button1);
			EditText email = rootView.FindViewById<EditText> (Resource.Id.editText1);

			//WHen clicked will send an email to email if valid
			button.Click+= delegate {
				Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
				Match match = regex.Match (email.Text);
				if (match.Success) {
					parent_activity.parse_forgot_pass(email.Text);
				} // dont let invalid email addresses be checked
			};

			return rootView;
		}
	}
}