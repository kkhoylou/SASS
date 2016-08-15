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

namespace KOTUS
{
	public class messageAdapter : BaseAdapter   {
		
		List<string> messages;
		Activity context;

		public messageAdapter(Activity context, List<string> messages) : base() {
			this.context = context;
			this.messages = messages;
		}

		public override long GetItemId(int position){
			return position;
		}

		public override Java.Lang.Object GetItem(int position) {
				return messages[position];
		}
		public override int Count {
			get { return messages.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent){
			View view = convertView; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
				view = context.LayoutInflater.Inflate(Resource.Layout.customMessageRow, null);
			view.FindViewById<TextView>(Resource.Id.title).Text = messages[position];
			view.FindViewById<TextView>(Resource.Id.author).Text = "author";
			view.FindViewById<TextView> (Resource.Id.datetime).Text = "datetime";
			return view;
		}
	}

	

/*	public class expandableMessageAdapter : BaseExpandableListAdapter {

		protected Dictionary<String,List<String>> map;

		readonly Activity context;

		public expandableMessageAdapter(Activity context, List<String> titles, Dictionary<String,List<String>> map) : base()
		{
			this.context = context;
			this.titles = titles;
			this.map = map;
		}
		protected List<String> titles { get; set;}
		/*public override object GetChild(int groupPosition, int childPosition){
			return this.titles.get(this.map.get(groupPosition)).get(childPosititon);
		}

		public override View GetGroupView(int groupposition, bool isExpanded, View convertView, ViewGroup parent)
		{
			View message_Title = convertView;
			if (message_Title == null) { // no view to re-use, create new
				LayoutInflater inflater = (LayoutInflater)this.context.GetSystemService (Context.LayoutInflaterService);
				message_Title = inflater.Inflate (Resource.Layout.customMessageRow, null);
			}
			message_Title.FindViewById<TextView> (Resource.Id.title).Text = "title";
			message_Title.FindViewById<TextView> (Resource.Id.author).Text = "author";
			message_Title.FindViewById<TextView> (Resource.Id.datetime).Text = "datetime";
			return message_Title;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{


			//String childText = "hello";
			View row = convertView;
			if (row == null) {
				//row = context.LayoutInflater.Inflate (Resource.Layout.customReplyRow, null);
				LayoutInflater inflater = (LayoutInflater)this.context.GetSystemService (Context.LayoutInflaterService);
				row = inflater.Inflate (Resource.Layout.customReplyRow, null);
			}

			//GetChildViewHelper (groupPosition, childPosition, out hey, out ho);
			row.FindViewById<TextView> (Resource.Id.content).Text = "content";
			row.FindViewById<TextView> (Resource.Id.author).Text = "author";


			return row;
		}

		public string this[int position]
		{
			get { return titles [position]; }
		}

		public int Count
		{
			get { return titles.Count; }
		}

		public override int GetChildrenCount (int groupPosition)
		{
			//char letter = (char)(65 + groupPosition);
			//TODO
			return 1;
		}

		public override int GroupCount {
			get {
				//TODO
				return 0;
			}
		}
	

		#region implemented abstract members of BaseExpandableListAdapter

		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			throw new NotImplementedException ();
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return "GroupName";
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			return true;
		}

		public override bool HasStableIds {
			get {
				return false;
			}
		}

		#endregion

	}*/
}