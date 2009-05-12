//  
//  Copyright (C) 2009 GNOME Do
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Gdk;
using Cairo;
using Mono.Unix;

using Do.Platform;
using Do.Universe;
using Do.Interface.Wink;
using Do.Universe.Common;
using Do.Interface;
using Do.Interface.CairoUtils;

using Docky.Interface.Menus;
using Docky.Utilities;

using Wnck;

namespace Docky.Interface
{
	
	
	public abstract class WnckDockItem : AbstractDockItem
	{
		const string ErrorMessage = "Docky could not move the file to the requested Directory.  " + 
			"Please check file name and permissions and try again";
		
		// a bad hack, but it works
		static string [] blacklist = new [] {
			"CopyToClipboardAction", 
			"WindowFocusAction",
		};
		
		int last_raised;
		bool? accepting_drops;
		
		DateTime last_scroll = new DateTime (0);
		TimeSpan scroll_rate = new TimeSpan (0, 0, 0, 0, 200);
		
		public abstract IEnumerable<Wnck.Window> Windows { get; }
		
		public abstract Item Item { get; }
		
		protected IEnumerable<Wnck.Window> VisibleWindows {
			get { return Windows.Where (w => !w.IsSkipTasklist); }
		}
		
		protected bool HasVisibleApps {
			get {
				if (Windows == null)
					return false;
				return VisibleWindows.Any ();
			}
		}	
		
		public override bool ContainsFocusedWindow {
			get { return VisibleWindows.Any (w => w.IsActive); }
		}
		
		public override bool IsAcceptingDrops {
			get {
				if (!accepting_drops.HasValue) {
					accepting_drops = (Item is IFileItem && Directory.Exists ((Item as IFileItem).Path)) || (Item is IApplicationItem);
				}
				return accepting_drops.Value;
			}
		}
		
		public WnckDockItem() : base ()
		{
			last_raised = 0;
		}
		
		protected IEnumerable<Act> ActionsForItem (Item item) 
		{
			return Services.Core.GetActionsForItemOrderedByRelevance (item, false)
				    .Where (act => !blacklist.Contains (act.GetType ().Name))
					.OrderByDescending (act => act.GetType ().Name != "WindowCloseAction")
					.ThenByDescending (act => act.GetType ().Name != "WindowMaximizeAction")
					.ThenByDescending (act => act.GetType ().Name != "WindowMinimizeAction")
					.ThenBy (act => act.Name.Length)
					.ThenBy (act => act.Name);
		}
		
		protected abstract void Launch ();
		
		public override void Scrolled (Gdk.ScrollDirection direction)
		{
			if (WindowCount < 1 || (DateTime.UtcNow - last_scroll) < scroll_rate) return;
			
			last_scroll = DateTime.UtcNow;
			
			// This block will make sure that if we're scrolling on an app that is already active
			// that when we scroll we move on the next window instead of appearing to do nothing
			Wnck.Window focused = VisibleWindows.Where (w => w.IsActive).FirstOrDefault ();
			if (focused != null) {
				for (; last_raised < WindowCount - 1; last_raised++) {
					if (VisibleWindows.ElementAt (last_raised).Pid == focused.Pid)
						break;
				}
			}
			
			switch (direction) {
			case ScrollDirection.Up:
			case ScrollDirection.Right: 
				last_raised++; 
				break;
			case ScrollDirection.Down:
			case ScrollDirection.Left: 
				last_raised--; 
				break;
			}
			
			KeepLastRaiseInBounds ();
			VisibleWindows.ElementAt (last_raised).CenterAndFocusWindow ();
		}
		
		public override void Clicked (uint button, Gdk.ModifierType state, PointD position)
		{
			if (!Windows.Any () || !HasVisibleApps || button == 2) {
				AnimationType = ClickAnimationType.Bounce;
				Launch ();
			} else if (button == 1) {
				AnimationType = ClickAnimationType.Darken;
				WindowUtils.PerformLogicalClick (Windows);
			} else {
				AnimationType = ClickAnimationType.None;
			}
		
			base.Clicked (button, state, position);
		}

		protected bool DetermineUrgencyStatus  ()
		{
			return VisibleWindows.Any (w => !w.IsSkipTasklist && w.NeedsAttention ());
		}
		
		void KeepLastRaiseInBounds ()
		{
			if (WindowCount <= last_raised)
				last_raised = 0;
			else if (last_raised < 0)
				last_raised = WindowCount - 1;
		}
		
		public override bool ReceiveItem (string item)
		{
			bool result = false;
			if (!IsAcceptingDrops)
				return result;
			
			if (item.StartsWith ("file://"))
				item = item.Substring ("file://".Length);
			
			if (File.Exists (item)) {
				if (Item is IApplicationItem) {
					try {
						(Item as IApplicationItem).LaunchWithFiles (Do.Platform.Services.UniverseFactory.NewFileItem (item).Cons (null));
						SetLastClick ();
						AnimationType = ClickAnimationType.Bounce;
						result = true;
					} catch {
						Services.Notifications.Notify ("Docky Error", "Docky could not launch application");
					}
				} else {
					try {
						File.Move (item, System.IO.Path.Combine ((Item as IFileItem).Path, System.IO.Path.GetFileName (item)));
						result = true;
					} catch { 
						Services.Notifications.Notify ("Docky Error", ErrorMessage);
					}
				}
			} else if (Directory.Exists (item)) {
				try {
					Directory.Move (item, System.IO.Path.Combine ((Item as IFileItem).Path, System.IO.Path.GetFileName (item)));
					result = true;
				} catch { 
					Services.Notifications.Notify ("Docky Error", ErrorMessage);
				}
			}
			return result;
		}
	}
}
