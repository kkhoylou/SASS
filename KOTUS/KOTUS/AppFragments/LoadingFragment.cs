using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Text.RegularExpressions;

namespace KOTUS
{
	public class LoadingFragment: Fragment{
		/* Name: OnCreateView
		 * Purpose: create the view for loading 
		 * Parameters: LayoutInflater: inflate the layout file 
		 *             ViewGroup: container for the layout file
		 *             Bundle: save instance state of the layout file
		 * Return: void
		 */
		public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
		{
			// inflate the layout of the loading page
			var rootView = p0.Inflate(Resource.Layout.Loading, p1, false);
		
			// activity title
			Activity.Title = "Loading";
			return rootView;
		}
	}
}