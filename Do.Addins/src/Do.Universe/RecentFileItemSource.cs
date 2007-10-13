// RecentFileItemSource.cs created with MonoDevelop
// User: dave at 2:18 PM 9/13/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

namespace Do.Universe
{
	
	public class RecentFileItemSource : IItemSource
	{
		
		List<IItem> files;
		bool recentUpdated;
		
		public RecentFileItemSource()
		{
			files = new List<IItem> ();
			Gtk.RecentManager.Default.Changed += OnRecentChanged;
			
			recentUpdated = false;
			ForceUpdateItems ();
		}
		
		protected void OnRecentChanged (object sender, EventArgs args)
		{
			recentUpdated = true;
			ForceUpdateItems ();
		}
		
		public string Name {
			get { return "Recent Files"; }
		}
		
		public string Description {
			get { return "Finds recently-opened files."; }
		}
		
		public string Icon {
			get { return "document"; }
		}
		
		public ICollection<IItem> Items {
			get { return files; }
		}
		
		public bool UpdateItems ()
		{
			bool tmp;
		
			tmp = recentUpdated;
			recentUpdated = false;
			return tmp;
		}
		
		protected virtual void ForceUpdateItems ()
		{
			/*
			foreach (IntPtr info_ptr in Gtk.RecentManager.Default.Items) {
				Console.WriteLine ("Recent items source adding item: {0}", info);
				files.Add (new FileItem (info.DisplayName, info.Uri));
			}
			*/
		}
		
	}
}