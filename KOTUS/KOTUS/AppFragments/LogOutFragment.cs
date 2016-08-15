using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace KOTUS
{
	public class LogoutFragment: Fragment{

		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			var rootView = p0.Inflate (Resource.Layout.Loading, p1, false);
			var parent_activity = (UserActivityWindow) this.Activity;

			rootView.FindViewById<ProgressBar> (Resource.Id.progressBar1).Activated = true;

			parent_activity.parseLogOut ();

			Activity.Title = "Log Out";
			return rootView;
		}
	}
}