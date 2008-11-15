// DockArea.cs
// 
// Copyright (C) 2008 GNOME Do
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;
using Gdk;
using Gtk;

using Do.Addins;
using Do.Universe;
using Do.UI;
using Do.Addins.CairoUtils;

using MonoDock.Util;

namespace MonoDock.UI
{
	
	
	public class DockArea : Gtk.DrawingArea
	{
		const int BounceTime = 700;
		const int BaseAnimationTime = 150;
		const int YBuffer = 3;
		const int XBuffer = 5;
		const int PaneSize = 168;
		
		IList<DockItem> dock_items;
		Gdk.Point cursor;
		DateTime enter_time = DateTime.Now;
		DateTime last_render = DateTime.Now;
		Gdk.Rectangle minimum_dock_size = new Gdk.Rectangle (-1, -1, -1, -1);
		
		DockState state;
		Surface backbuffer, input_area_buffer;
		DockWindow window;
		PixbufSurfaceCache large_icon_cache, small_icon_cache;
		
		
		#region Public properties
		public int Width {
			get {
				return (dock_items.Count * IconSize) + Preferences.ZoomSize + 2*XBuffer;
			}
		}
		
		public int Height {
			get {
				return 2*IconSize + 40;
			}
		}
		
		public int DockHeight {
			get {
				Console.WriteLine (MinimumDockArea.Height);
				return MinimumDockArea.Height;
			}
		}
		
		public bool InputInterfaceVisible {
			get {
				return input_interface;
			}
		}
		
		public Pane CurrentPane {
			get {
				return State.CurrentPane;
			}
			set {
				State.CurrentPane = value;
				AnimatedDraw ();
			}
		}
		
		public bool ThirdPaneVisible { get; set; }
		#endregion
		
		double ZoomIn {
			get {
				if (!CursorIsOverDockArea)
					return Math.Max (0, 1-DateTime.Now.Subtract (enter_time).TotalMilliseconds/150.0);
				return Math.Min (1, DateTime.Now.Subtract (enter_time).TotalMilliseconds/150.0);
			}
		}
		
		int ZoomPixels {
			get {
				return (int) (ZoomSize*ZoomIn);
			}
		}
		
		int ZoomSize {
			get {
				return Preferences.ZoomSize;
			}
		}
		
		double DockIconOpacity {
			get {
				double total_time = (DateTime.Now - interface_change_time).TotalMilliseconds;
				if (total_time > BaseAnimationTime) {
					if (input_interface)
						return 0;
					return 1;
				}
				
				if (input_interface) {
					return 1-(total_time/BaseAnimationTime);
				} else {
					return total_time/BaseAnimationTime;
				}
			}
		}
		
		string HighlightFormat { 
			get { 
				return "<span underline=\"single\">{0}</span>";
			} 
		}
		
		double InputAreaOpacity {
			get {
				return 1-DockIconOpacity;
			}
		}
		
		double InputAreaSlideStatus {
			get {
				return Math.Min (1,(DateTime.Now - State.CurrentPaneTime).TotalMilliseconds / BaseAnimationTime);
			}
		}
		
		int IconBorderWidth {get{ return 4; }}
		
		int IconSize { get { return DockItem.IconSize + IconBorderWidth; } }
		
		int GapSize { get { return GetDockArea ().Width - 10 - PaneSize*3; } }
		
		Gdk.Point Cursor {
			get {
				return cursor;
			}
			set {
				bool tmp = CursorIsOverDockArea;
				cursor = value;
				if (CursorIsOverDockArea != tmp) {
					if (CursorIsOverDockArea) {
						window.SetInputMask (0);
					} else {
						window.SetInputMask (Height-IconSize);
					}
					enter_time = DateTime.Now;
					AnimatedDraw ();
				}
			}
		}
		
		Gdk.Rectangle MinimumDockArea {
			get {
				if (minimum_dock_size.X == -1)
					minimum_dock_size = new Gdk.Rectangle ((ZoomSize/2)-XBuffer, Height-IconSize-2*YBuffer, Width - ZoomSize, IconSize+2*YBuffer);
				return minimum_dock_size;
			}
		}
		
		PixbufSurfaceCache LargeIconCache {
			get {
				return large_icon_cache ?? large_icon_cache = new PixbufSurfaceCache (10, 128, 128);
			}
		}
		
		PixbufSurfaceCache SmallIconCache {
			get {
				return small_icon_cache ?? small_icon_cache = new PixbufSurfaceCache (40, 64, 64);
			}
		}
		
		DockState State {
			get { return state ?? state = new DockState (); }
		}
		
		#region Animation properties
		bool CursorIsOverDockArea {
			get {
				Gdk.Rectangle rect = MinimumDockArea;
				rect.Inflate (0, 55);
				return rect.Contains (Cursor); 
			}
		}
		
		bool PaneChangeAnimationNeeded {
			get {
				return (DateTime.Now - State.CurrentPaneTime).TotalMilliseconds < BaseAnimationTime;
			}
		}
		
		bool ZoomAnimationNeeded {
			get { return !((CursorIsOverDockArea && ZoomIn == 1) || (!CursorIsOverDockArea && ZoomIn == 0)); }
		}
		
		DateTime last_click = DateTime.Now;
		bool BounceAnimationNeeded {
			get { return (DateTime.Now - last_click).TotalMilliseconds < BounceTime; }
		}
		
		bool InputModeChangeAnimationNeeded {
			get { return (DateTime.Now - interface_change_time).TotalMilliseconds < BaseAnimationTime; }
		}
		
		bool AnimationNeeded {
			get { return PaneChangeAnimationNeeded || ZoomAnimationNeeded || BounceAnimationNeeded || InputModeChangeAnimationNeeded; }
		}
		#endregion
		
		public DockArea(DockWindow window, IEnumerable<DockItem> baseItems) : base ()
		{
			this.window = window;
			dock_items = new List<DockItem> (baseItems);
			cursor = new Gdk.Point (-1, -1);
			
			SetSizeRequest (Width, Height);
			this.SetCompositeColormap ();
			
			AddEvents ((int) Gdk.EventMask.PointerMotionMask | (int) Gdk.EventMask.LeaveNotifyMask |
			           (int) Gdk.EventMask.ButtonPressMask | (int) Gdk.EventMask.ButtonReleaseMask);
			
			DoubleBuffered = false;
		}
		
		uint timer = 0;
		void AnimatedDraw ()
		{
			if (timer > 0)
				return;
			
			QueueDraw ();
			
			timer = GLib.Timeout.Add (20, delegate {
				QueueDraw ();
				if (AnimationNeeded)
					return true;
				
				timer = 0;
				return false;
			});
		}
		
		void DrawDrock (Context cr)
		{
			Gdk.Rectangle dock_area = GetDockArea ();
			cr.Rectangle (dock_area.X+.5, dock_area.Y+.5, dock_area.Width-1, dock_area.Height);
			cr.Color = new Cairo.Color (.1, .1, .1, .7);
			cr.FillPreserve ();
			
			cr.Color = new Cairo.Color (1, 1, 1, .4);
			cr.LineWidth = 1;
			cr.Stroke ();
			
			if (InputAreaOpacity > 0) {
				if (input_area_buffer == null)
					input_area_buffer = cr.Target.CreateSimilar (cr.Target.Content, Width, Height);
				
				using (Context input_cr = new Context (input_area_buffer)) {
					input_cr.AlphaFill ();
					DrawInputArea (input_cr);
				}
				
				cr.SetSource (input_area_buffer);
				cr.PaintWithAlpha (InputAreaOpacity);
			}
			
			if (DockIconOpacity > 0) {
				DrawIcons (cr);
			}
		}
		
		void DrawIcons (Context cr)
		{
			for (int i=0; i<dock_items.Count; i++) {
				int center;
				double zoom;
				
				IconPositionedCenterX (i, out center, out zoom);
				
				double x = (1/zoom)*(center - zoom*IconSize/2);
				double y = (1/zoom)*(Height-(zoom*IconSize)) + IconBorderWidth/2 - YBuffer;
				
				int total_ms = (int) (DateTime.Now - dock_items[i].LastClick).TotalMilliseconds;
				if (total_ms < BounceTime) {
					y -= Math.Abs (20*Math.Sin (total_ms*Math.PI/(BounceTime/2)));
				}
				
				cr.Scale (zoom/DockItem.IconQuality, zoom/DockItem.IconQuality);
				cr.Rectangle (x*DockItem.IconQuality, y*DockItem.IconQuality, IconSize*DockItem.IconQuality, IconSize*DockItem.IconQuality);
				Gdk.CairoHelper.SetSourcePixbuf (cr, dock_items[i].Pixbuf, x*DockItem.IconQuality, y*DockItem.IconQuality);
				cr.PaintWithAlpha (DockIconOpacity);
				cr.Scale (1/(zoom/DockItem.IconQuality), 1/(zoom/DockItem.IconQuality));
				
				if (DockItemForX (Cursor.X) == i && CursorIsOverDockArea) {
					cr.SetSource (dock_items[i].GetTextSurface (), IconNormalCenterX (i)-(DockItem.TextWidth/2), 15);
					cr.Paint ();
				}
			}
		}
		
		#region Input Area drawing code
		void DrawInputArea (Context cr)
		{
			DrawPane (cr, Pane.First);
			DrawResultIcons (cr, Pane.First);
			
			DrawPane (cr, Pane.Second);
			DrawResultIcons (cr, Pane.Second);
			
			if (ThirdPaneVisible) {
				DrawPane (cr, Pane.Third);
				DrawResultIcons (cr, Pane.Third);
			}
		}
		
		void DrawPane (Context cr, Pane pane)
		{
			int start_x = GetXForPane (pane);
			cr.SetRoundedRectanglePath (start_x, Height - 162, 158, 158, 20);
			if (pane == CurrentPane)
				cr.Color = new Cairo.Color (.15, .15, .15, .8);
			else
				cr.Color = new Cairo.Color (0, 0, 0, .4);
			cr.FillPreserve ();
			
			cr.Color = new Cairo.Color (1, 1, 1, .8);
			cr.Stroke ();
			
			if (State.GetPaneItem (pane) != null) {
				if (!LargeIconCache.ContainsKey (State.GetPaneItem (pane).Icon)) {
					LargeIconCache.AddPixbufSurface (State.GetPaneItem (pane).Icon, State.GetPaneItem (pane).Icon);
				}
				cr.SetSource (LargeIconCache.GetSurface (State.GetPaneItem (pane).Icon), start_x+15, Height - (PaneSize - 10));
				cr.Paint ();
				
				string text = GLib.Markup.EscapeText (State.GetPaneItem (pane).Name);
				text = Do.Addins.Util.FormatCommonSubstrings (text, State.GetPaneQuery (pane), HighlightFormat);
				BezelTextUtils.RenderLayoutText (cr, text, start_x + 5, Height-25, PaneSize-20, this);
			}
		}
		
		void DrawResultIcons (Context cr, Pane pane)
		{
			if ((pane != State.CurrentPane && pane != State.PreviousPane) || State.GetPaneResults (pane) == null)
				return;
			
			int num_icons = GapSize/IconSize;
			int pane_x = GetXForPane (pane);
			int cursor = State.GetPaneCursor (pane)+1;
			double transit_state = InputAreaSlideStatus;
			
			if (pane == State.PreviousPane)
				transit_state = 1-transit_state;
			
			cr.Rectangle (pane_x+PaneSize, 0, GapSize*transit_state, Height);
			cr.Clip ();
			for (int i=cursor; i<num_icons+cursor && i<State.GetPaneResults (pane).Count; i++) {
				IObject item = State.GetPaneResults (pane)[i];
				if (!SmallIconCache.ContainsKey (item.Icon))
					SmallIconCache.AddPixbufSurface (item.Icon, item.Icon);
				
				cr.SetSource (SmallIconCache.GetSurface (item.Icon), pane_x + PaneSize + IconSize*(i-cursor)*transit_state, Height-IconSize-YBuffer);
				cr.Paint ();
			}
			cr.ResetClip ();
		}
		
		int GetXForPane (Pane pane)
		{
			Gdk.Rectangle dock_area = GetDockArea ();
			
			double transit_state = InputAreaSlideStatus;
			if (transit_state == 1 || pane == Pane.First) {
				switch (pane) {
				case Pane.First:
					return dock_area.X + 10;
				case Pane.Second:
					if (CurrentPane == Pane.First)
						return dock_area.X + 10 + PaneSize + GapSize;
					return dock_area.X + 10 + PaneSize;
				default:
					if (CurrentPane == Pane.Third)
						return dock_area.X + 10 + 2*PaneSize;
					return dock_area.X + 10 + GapSize + 2*PaneSize;
				}
			}
			
			//It should be impossible to get to this point with a Pane.First
			if (pane == Pane.First)
				throw new Exception ("You have beat the programmer");
			
			if (pane == Pane.Second) {
				if (State.PreviousPane == Pane.First) {
					return dock_area.X + 10 + PaneSize + (int) (GapSize*(1-transit_state));
				} else if (State.CurrentPane == Pane.First) {
					return dock_area.X + 10 + PaneSize + (int) (GapSize*transit_state);
				} else {
					return dock_area.X + 10 + PaneSize;
				}
			} else {
				if (State.CurrentPane == Pane.Third) {
					return dock_area.X + 10 + (int) (GapSize*(1-transit_state)) + 2*PaneSize;
				} else if (State.PreviousPane == Pane.Third) {
					return dock_area.X + 10 + (int) (GapSize*transit_state) + 2*PaneSize;
				} else {
					return dock_area.X + 10 + GapSize + 2*PaneSize;
				}
			}
			
		}
		
		#endregion
		
		int IconNormalCenterX (int icon)
		{
			return MinimumDockArea.X + XBuffer + (IconSize/2) + (IconSize * icon);
		}
		
		int DockItemForX (int x)
		{
			x -= XBuffer;
			return (x-MinimumDockArea.X)/IconSize;
		}
		
		void IconPositionedCenterX (int icon, out int x, out double zoom)
		{
			int center = IconNormalCenterX (icon);
			int offset = Math.Min (Math.Abs (Cursor.X - center), ZoomPixels/2);
			
			if (ZoomPixels/2 == 0)
				zoom = 1;
			else {
				zoom = 2 - (offset/(double)(ZoomPixels/2));
				zoom = (zoom-1)*ZoomIn+1;
			}
			
			offset = (int) (offset*Math.Sin ((Math.PI/4)*zoom));
			
			if (Cursor.X > center) {
				center -= offset;
			} else {
				center += offset;
			}
			x = center;
		}
		
		Gdk.Rectangle GetDockArea ()
		{
			if (!CursorIsOverDockArea && ZoomIn == 0)
				return MinimumDockArea;

			int start_x, end_x;
			double start_zoom, end_zoom;
			IconPositionedCenterX (0, out start_x, out start_zoom);
			IconPositionedCenterX (dock_items.Count - 1, out end_x, out end_zoom);
			
			int x = start_x - (int)(start_zoom*(IconSize/2)) - XBuffer;
			int end = end_x + (int)(end_zoom*(IconSize/2)) + XBuffer;
			
			return new Gdk.Rectangle (x, Height-IconSize-2*YBuffer, end-x, IconSize+2*YBuffer);
		}
		
		protected override bool OnExposeEvent(EventExpose evnt)
		{
			bool ret_val = base.OnExposeEvent (evnt);
			if (!IsDrawable)
				return ret_val;
			last_render = DateTime.Now;
			
			if (backbuffer == null) {
				Context tmp = Gdk.CairoHelper.Create (GdkWindow);
				backbuffer = tmp.Target.CreateSimilar (tmp.Target.Content, Width, Height);
				(tmp as IDisposable).Dispose ();
			}
			
			Context cr = new Cairo.Context (backbuffer);
			cr.AlphaFill ();
			cr.Operator = Operator.Over;
			
			DrawDrock (cr);
			(cr as IDisposable).Dispose ();
			
			Context cr2 = Gdk.CairoHelper.Create (GdkWindow);
			cr2.SetSource (backbuffer, 0, 0);
			cr2.Operator = Operator.Source;
			cr2.Paint ();
			(cr2 as IDisposable).Dispose ();
			
			return ret_val;
		}
 
		protected override bool OnMotionNotifyEvent(EventMotion evnt)
		{
			bool tmp = CursorIsOverDockArea;
			Cursor = new Gdk.Point ((int) evnt.X, (int) evnt.Y);
			
			if (tmp != CursorIsOverDockArea || CursorIsOverDockArea && DateTime.Now.Subtract (last_render).TotalMilliseconds > 20) 
				AnimatedDraw ();
			return base.OnMotionNotifyEvent (evnt);
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			int item = DockItemForX ((int) evnt.X);
			if (item < 0 || item >= dock_items.Count)
				return base.OnButtonReleaseEvent (evnt);
			if ((DateTime.Now - dock_items[item].LastClick).TotalMilliseconds > BounceTime) {
				last_click = dock_items[item].LastClick = DateTime.Now;
				IItem doItem = dock_items[item].IObject as IItem;
				if (doItem != null)
					window.Controller.PerformDefaultAction (dock_items[item].IObject as IItem);
				AnimatedDraw ();
			}
			return base.OnButtonReleaseEvent (evnt);
		}

		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			Cursor = new Gdk.Point ((int) evnt.X, (int) evnt.Y);
			return base.OnLeaveNotifyEvent (evnt);
		}

		
		public void SetIcons (IEnumerable<DockItem> items)
		{
			dock_items.Clear ();
			foreach (DockItem i in items)
				dock_items.Add (i);
			minimum_dock_size = new Gdk.Rectangle (-1, -1, -1, -1);
			input_area_buffer.Destroy ();
			input_area_buffer = null;
			
			backbuffer.Destroy ();
			backbuffer = null;
			
			SetSizeRequest (Width, Height);
		}
		
		public void SetPaneContext (IUIContext context, Pane pane)
		{
			State[pane] = context.Selection;
			State.SetPaneQuery (context.Query, pane);
			State.SetPaneResults (context.Results, pane);
			State.SetPaneCursor (context.Cursor, pane);
			AnimatedDraw ();
		}
		
		DateTime interface_change_time = DateTime.Now;
		bool input_interface = false;
		public void ShowInputInterface ()
		{
			interface_change_time = DateTime.Now;
			input_interface = true;
			
			AnimatedDraw ();
		}
		
		public void HideInputInterface ()
		{
			interface_change_time = DateTime.Now;
			input_interface = false;
			
			AnimatedDraw ();
		}
		
		public void Reset ()
		{
			State.Clear ();
			AnimatedDraw ();
		}
		
		public void ClearPane (Pane pane)
		{
			State.ClearPane (pane);
		}
	}
}
