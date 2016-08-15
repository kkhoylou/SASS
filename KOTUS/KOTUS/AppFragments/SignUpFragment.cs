using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Text.RegularExpressions;

namespace KOTUS
{
	public class SignUpFragment : Fragment
	{
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			var rootView = p0.Inflate(Resource.Layout.signUp, p1, false);
			Activity.Title = "Create Account";
			var parent_activity = (WelcomeActivity) this.Activity;
			// find layout items
			EditText first_name = rootView.FindViewById<EditText> (Resource.Id.firstName);
			EditText last_name = rootView.FindViewById<EditText> (Resource.Id.lastName);
			EditText email = rootView.FindViewById<EditText> (Resource.Id.userEmail);
			EditText password = rootView.FindViewById<EditText> (Resource.Id.userPass);
			EditText confirm_pass = rootView.FindViewById<EditText> (Resource.Id.confirmPass);
			EditText phone_number = rootView.FindViewById<EditText> (Resource.Id.phone);
			Button sign_up = rootView.FindViewById<Button> (Resource.Id.button1);

			sign_up.Click += delegate {

				// check that email address is valid
				bool email_valid;
				if(check_email(email.Text)){
					email_valid = true;
				}else{
					email.Error = "Invalid Email Address";
					email_valid = false;
				}

				// check that name is valid
				bool name_valid;
				bool first_check = false;
				bool second_check = false;
				if (first_name.Text.Length > 1) {
					first_check = true;
				} else {
					first_check = false;
					first_name.Error = "First name must be more than 1 charachter";
				}

				if (last_name.Text.Length > 1) {
					second_check = true;
				} else {
					second_check = false;
					last_name.Error = "Last name must be more than 1 character";
				}
				if (first_check && second_check) {
					name_valid = true;
				} else {
					name_valid = false;
				}

				// check that phone number is valid
				bool phone_valid;
				if(check_phone_number(phone_number.Text)){
					phone_valid = true;
				}else{
					phone_valid =false;
					phone_number.Error = "Phone number must be 10 or 11 characters";
				}

				// check that passwords are valid
				bool pass_valid;
				if(check_passes_are_valid(password.Text)){
					pass_valid = true;
				}else{
					pass_valid = false;
					password.Error = "Password must have between 8-12 characters.";
					confirm_pass.Error = "Password must have between 8-12 characters.";
				}

				// check that the passwords match
				bool pass_match;
				if(check_passwords_match(password.Text, confirm_pass.Text)){
					pass_match = true;
				}else{
					password.Error = "Passwords must match";
					pass_match = false;
				}

				// if all valid then create the user
				if(email_valid && name_valid && phone_valid && pass_match && pass_valid){ // check that the user can be created
					// create user account
					parent_activity.parse_create_user(first_name.Text, last_name.Text, email.Text, phone_number.Text, password.Text);
				} // All errors are presented in the respective check functions

			};

			return rootView;
		}

		// returns true if the email address is of a valid format
		// COMPLETED
		public bool check_email(string email){
			Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
			Match match = regex.Match (email);
			if (match.Success) {
				return true;
			} else {
				return false;
			}
		}

		public bool check_passes_are_valid(string password){
			//password must be between 8-12 characters
			if (password != null && password.Length < 8 || password.Length > 12) {
								return false;
			} else { // no problems here
				return true;
			}
		}

		// this function checks that the passwords that were entered both match
	
		public bool check_passwords_match(string pass, string confirm){
			if (pass.Equals(confirm)){
				return true;
			} else{
				return false;
			}
		}

		// this will check if the entered phone number is valid
		// returns true if the phone number is valid
	
		public bool check_phone_number(string phone){
			if (phone.Length < 10 || phone.Length > 11) {
				return false;
			} else {
				return true;
			}
		}


	}
}

