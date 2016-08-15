using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace KOTUS
{
	public class GroupLogoutFragment: Fragment{

		/* Name: OnCreateView
		 * Purpose: Create the view for forgot password
		 * Parameters: LayoutInflator p0 - used to create lyout
		 * 				ViewGroup p1 - GroupView to use
		 * 				Bundle p2 - Bundle to use within
		 * Return: view of forgot password
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			var rootView = p0.Inflate (Resource.Layout.Loading, p1, false);
			var parent_activity = (GroupActivityWindow) this.Activity;

			rootView.FindViewById<ProgressBar> (Resource.Id.progressBar1).Activated = true;

			parent_activity.parse_log_out ();

			Activity.Title = "Log Out";
			return rootView;
		}
	}
}