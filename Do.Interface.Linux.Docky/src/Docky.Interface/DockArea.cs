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
using System.Text.RegularExpressions;

using Cairo;
using Gdk;
using Gtk;

using Do.Platform;
using Do.Interface;
using Do.Universe;
using Do.Interface.CairoUtils;
using Do;

using Docky.Utilities;
using Docky.Interface.Renderers;

namespace Docky.Interface
{
	
	
	public class DockArea : Gtk.DrawingArea
	{
		public const int BaseAnimationTime = 150;
		const int VerticalBuffer = 5;
		const int HorizontalBuffer = 7;
		const int BounceTime = 700;
		const int InsertAnimationTime = BaseAnimationTime*5;
		const int WindowHeight = 300;
		const int IconBorderWidth = 2;
		const string HighlightFormat = "<span foreground=\"#5599ff\">{0}</span>";
		
		Gdk.Point cursor;
		
		DateTime last_click = DateTime.UtcNow;
		DateTime enter_time = DateTime.UtcNow;
		DateTime last_render = DateTime.UtcNow;
		DateTime interface_change_time = DateTime.UtcNow;
		
		bool cursor_is_handle = false;
		
		int monitor_width;
		int drag_start_y = 0;
		int drag_start_icon_size = 0;
		uint animation_timer = 0;
		
		DockWindow window;
		DockItemProvider item_provider;
		Surface backbuffer, input_area_buffer, dock_icon_buffer;
		
		#region public properties
		public bool InputInterfaceVisible { get; set; }
		
		public int Width {
			get { return monitor_width; }
		}
		
		public int Height { 
			get { return WindowHeight; } 
		}
		
		public int DockWidth {
			get {
				return DockItems.Sum (di => di.Width + 2*IconBorderWidth) + 2*HorizontalBuffer;
			}
		}
		
		public int DockHeight {
			get {
				return DockPreferences.AutoHide ? 0 : MinimumDockArea.Height;
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
		
		public bool ThirdPaneVisible { 
			get { return State.ThirdPaneVisible; }
			set { 
				if (State.ThirdPaneVisible == value)
					return;
				State.ThirdPaneVisible = value;
				AnimatedDraw ();
			}
		}
		#endregion
		
		IStatistics Statistics { get; set; }
		
		new DockState State { get; set; }
		
		IDockItem[] DockItems { 
			get { return item_provider.DockItems.ToArray (); } 
		}
		
		#region Zoom Properties
		/// <value>
		/// Returns the zoom in percentage (0 through 1)
		/// </value>
		double ZoomIn {
			get {
				if (CursorIsOverDockArea)
					return Math.Min (1, (DateTime.UtcNow - enter_time).TotalMilliseconds/BaseAnimationTime);
				return Math.Max (0, 1 - (DateTime.UtcNow - enter_time).TotalMilliseconds/BaseAnimationTime);
			}
		}
		
		/// <value>
		/// Returns the width of the zoom (ZoomIn * ZoomSize)
		/// </value>
		int ZoomPixels {
			get {
				return (int) (DockPreferences.ZoomSize*ZoomIn);
			}
		}
		
		#endregion
		
		//// <value>
		/// The overall offset of the dock as a whole
		/// </value>
		int VerticalOffset {
			get {
				double offset = 0;
				if (!DockPreferences.AutoHide || cursor_is_handle)
					return 0;
				
				if (CursorIsOverDockArea) {
					offset = 1 - Math.Min (1,(DateTime.UtcNow - enter_time).TotalMilliseconds / BaseAnimationTime);
					return (int) (offset*MinimumDockArea.Height);
					
				} else {
					offset = Math.Min (Math.Min (1,(DateTime.UtcNow - enter_time).TotalMilliseconds / BaseAnimationTime),
					                  Math.Min (1, (DateTime.UtcNow - interface_change_time).TotalMilliseconds / BaseAnimationTime));
					
					if (InputInterfaceVisible)
						offset = 1 - offset;
					return (int) (offset*MinimumDockArea.Height);
				}
			}
		}
		
		/// <value>
		/// Determins the opacity of the icons on the normal dock
		/// </value>
		double DockIconOpacity {
			get {
				double total_time = (DateTime.UtcNow - interface_change_time).TotalMilliseconds;
				if (BaseAnimationTime < total_time) {
					if (InputInterfaceVisible)
						return 0;
					return 1;
				}
				
				if (InputInterfaceVisible) {
					return 1 - (total_time/BaseAnimationTime);
				} else {
					return total_time/BaseAnimationTime;
				}
			}
		}
		
		double InputAreaOpacity {
			get { return 1 - DockIconOpacity; }
		}
		
		int IconSize { 
			get { return DockPreferences.IconSize; } 
		}
		
		/// <value>
		/// The current cursor as known to the dock.
		/// </value>
		Gdk.Point Cursor {
			get {
				return cursor;
			}
			set {
				bool tmp = CursorIsOverDockArea;
				cursor = value;
				
				if (CursorIsOverDockArea != tmp) {
					SetParentInputMask ();
					enter_time = DateTime.UtcNow;
					AnimatedDraw ();
				}
			}
		}
		
		Gdk.Point StickIconCenter {
			get {
				Gdk.Rectangle rect = GetDockArea ();
				return new Gdk.Point (rect.X + rect.Width - 7, rect.Y + 8);
			}
		}
		
		Gdk.Rectangle MinimumDockArea {
			get {
				int x_offset = (Width - DockWidth)/2;
				return new Gdk.Rectangle (x_offset, Height-IconSize - 2*VerticalBuffer, DockWidth, IconSize + 2*VerticalBuffer);
			}
		}
		
		#region Animation properties
		bool CursorIsOverDockArea {
			get {
				Gdk.Rectangle rect = MinimumDockArea;
				rect.Inflate (0, 55);
				return rect.Contains (Cursor); 
			}
		}
		
		bool IconInsertionAnimationNeeded {
			get {
				return DockItems.Any (di => (DateTime.UtcNow - di.DockAddItem).TotalMilliseconds < InsertAnimationTime);
			}
		}
		
		bool PaneChangeAnimationNeeded {
			get {
				return (DateTime.UtcNow - State.CurrentPaneTime).TotalMilliseconds < BaseAnimationTime;
			}
		}
		
		bool ZoomAnimationNeeded {
			get {
				bool is_zoomed_fully_in = CursorIsOverDockArea && ZoomIn == 1;
				bool is_zoomed_fully_out = !CursorIsOverDockArea && ZoomIn == 0;
				return !(is_zoomed_fully_in || is_zoomed_fully_out); 
			}
		}
		
		bool OpenAnimationNeeded {
			get { 
				return (DateTime.UtcNow - enter_time).TotalMilliseconds < BaseAnimationTime ||
					(DateTime.UtcNow - interface_change_time).TotalMilliseconds < BaseAnimationTime;
			}
		}
		
		bool BounceAnimationNeeded {
			get { return (DateTime.UtcNow - last_click).TotalMilliseconds < BounceTime; }
		}
		
		bool InputModeChangeAnimationNeeded {
			get { return (DateTime.UtcNow - interface_change_time).TotalMilliseconds < BaseAnimationTime; }
		}
		
		bool InputModeSlideAnimationNeeded {
			get { return (DateTime.UtcNow - State.LastCursorChange).TotalMilliseconds < BaseAnimationTime; }
		}
		
		bool ThirdPaneVisibilityAnimationNeeded {
			get { return (DateTime.UtcNow - State.ThirdChangeTime).TotalMilliseconds < BaseAnimationTime; }
		}
		
		bool AnimationNeeded {
			get { 
				return OpenAnimationNeeded || 
					   ZoomAnimationNeeded || 
					   BounceAnimationNeeded || 
					   InputModeChangeAnimationNeeded || 
					   InputModeSlideAnimationNeeded || 
					   IconInsertionAnimationNeeded || 
					   PaneChangeAnimationNeeded || 
					   ThirdPaneVisibilityAnimationNeeded; 
			}
		}
		#endregion
		
		public DockArea (DockWindow window, IStatistics statistics) : base ()
		{
			Statistics = statistics;
			this.window = window;
			item_provider = new DockItemProvider (statistics);
			State = new DockState ();
			
			Cursor = new Gdk.Point (-1, -1);
			
			Gdk.Rectangle geo;
			geo = Screen.GetMonitorGeometry (0);
			
			monitor_width = geo.Width;
			SetSizeRequest (geo.Width, Height);
			
			this.SetCompositeColormap ();
			
			AddEvents ((int) EventMask.PointerMotionMask | 
			           (int) EventMask.LeaveNotifyMask |
			           (int) EventMask.ButtonPressMask | 
			           (int) EventMask.ButtonReleaseMask);
			
			DoubleBuffered = false;
			
			RegisterEvents ();
			
			GLib.Timeout.Add (20, () => {
				//must be done after construction is complete
				SetParentInputMask ();
				return false;
			});
		}
		
		void RegisterEvents ()
		{
			item_provider.DockItemsChanged += OnDockItemsChanged;
			
			ItemMenu.Instance.RemoveClicked += OnItemMenuRemoveClicked;
			
			ItemMenu.Instance.Hidden += OnItemMenuHidden;
		}
		
		void AnimatedDraw ()
		{
			if (0 < animation_timer)
				return;
			
			QueueDraw ();
			
			animation_timer = GLib.Timeout.Add (16, delegate {
				QueueDraw ();
				if (AnimationNeeded)
					return true;
				
				//reset the timer to 0 so that the next time AnimatedDraw is called we fall back into
				//the draw loop.
				animation_timer = 0;
				return false;
			});
		}
		
		void DrawDrock (Context cr)
		{
			Gdk.Rectangle dockArea = GetDockArea ();
			DockBackgroundRenderer.RenderDockBackground (cr, dockArea);
			
			if (InputAreaOpacity > 0) {
				if (input_area_buffer == null)
					input_area_buffer = cr.Target.CreateSimilar (cr.Target.Content, Width, Height);
				
				using (Context input_cr = new Context (input_area_buffer)) {
					input_cr.AlphaFill ();
					Renderers.SummonModeRenderer.RenderSummonMode (input_cr, State, dockArea, VerticalBuffer);
				}
				
				cr.SetSource (input_area_buffer);
				cr.PaintWithAlpha (InputAreaOpacity);
			}
			
			if (DockIconOpacity > 0) {
				if (dock_icon_buffer == null)
					dock_icon_buffer = cr.Target.CreateSimilar (cr.Target.Content, Width, Height);
				
				using (Context input_cr = new Context (dock_icon_buffer)) {
					input_cr.AlphaFill ();
					DrawIcons (input_cr);
					
					if (CursorIsOverDockArea)
						DrawThumbnailIcon (input_cr);
				}
				
				cr.SetSource (dock_icon_buffer, 0, IconSize * (1-DockIconOpacity));
				cr.PaintWithAlpha (DockIconOpacity);
			}
		}
		
		void DrawIcons (Context cr)
		{
			for (int i=0; i<DockItems.Length; i++)
				DrawIcon (cr, i);
		}
		
		void DrawIcon (Context cr, int icon)
		{
			int center;
			double zoom;
			
			IconPositionedCenterX (icon, out center, out zoom);
			
			double insertion_ms = (DateTime.UtcNow - DockItems[icon].DockAddItem).TotalMilliseconds;
			if (insertion_ms < InsertAnimationTime) {
				zoom *= insertion_ms/InsertAnimationTime;
			}
			
			double x = (1/zoom)*(center - zoom*IconSize/2);
			double y = (1/zoom)*(Height-(zoom*IconSize)) - VerticalBuffer;
			
			int total_ms = (int) (DateTime.UtcNow - DockItems[icon].LastClick).TotalMilliseconds;
			if (total_ms < BounceTime) {
				y -= Math.Abs (20*Math.Sin (total_ms*Math.PI/(BounceTime/2)));
			}
			
			double scale = zoom/DockPreferences.IconQuality;
			if (DockItems[icon].Scalable) {
				cr.Scale (scale, scale);
				cr.SetSource (DockItems[icon].GetIconSurface (), x*DockPreferences.IconQuality, y*DockPreferences.IconQuality);
				cr.Paint ();
				cr.Scale (1/scale, 1/scale);
			} else {
				cr.SetSource (DockItems[icon].GetIconSurface (), x*zoom, Height-DockItems[icon].Height-(MinimumDockArea.Height-DockItems[icon].Height)/2);
				cr.Paint ();
			}
			
			if (DockItems[icon].DrawIndicator) {
				cr.MoveTo (center, Height - 6);
				cr.LineTo (center+4, Height);
				cr.LineTo (center-4, Height);
				cr.ClosePath ();
				
				cr.Color = new Cairo.Color (1, 1, 1, .7);
				cr.Fill ();
			}
			
			if (DockItemForX (Cursor.X) == icon && CursorIsOverDockArea && DockItems[icon].GetTextSurface () != null) {
				cr.SetSource (DockItems[icon].GetTextSurface (), IconNormalCenterX (icon)-(DockPreferences.TextWidth/2), Height-2*IconSize-28);
				cr.Paint ();
			}
		}
		
		void DrawThumbnailIcon (Context cr)
		{
			Gdk.Point center = StickIconCenter;
			
			double opacity = 1.0/Math.Abs (center.X-Cursor.X)*30 - .2;
			
			cr.Arc (center.X, center.Y, 3.5, 0, Math.PI*2);
			cr.LineWidth = 1;
			cr.Color = new Cairo.Color (1, 1, 1, opacity);
			cr.Stroke ();
			
			if (!DockPreferences.AutoHide) {
				cr.Arc (center.X, center.Y, 1.5, 0, Math.PI*2);
				cr.Color = new Cairo.Color (1, 1, 1, opacity);
				cr.Fill ();
			}
		}
		
		int IconNormalCenterX (int icon)
		{
			//the first icons center is at dock X + border + IconBorder + half its width
			if (!DockItems.Any ())
				return 0;
			int start_x = MinimumDockArea.X + HorizontalBuffer + IconBorderWidth + (DockItems[0].Width/2);
			for (int i=0; i<icon; i++)
				start_x += DockItems[i].Width + 2*IconBorderWidth;
			return start_x;
		}
		
		int DockItemForX (int x)
		{
			int start_x = MinimumDockArea.X + HorizontalBuffer;
			for (int i=0; i<DockItems.Length; i++) {
				if (x >= start_x && x <= start_x+DockItems[i].Width+2*IconBorderWidth)
					return i;
				start_x += DockItems[i].Width + 2*IconBorderWidth;
			}
			return -1;
		}
		
		void IconPositionedCenterX (int icon, out int x, out double zoom)
		{
			int center = IconNormalCenterX (icon);
			int offset = Math.Min (Math.Abs (Cursor.X - center), ZoomPixels/2);
			
			if (ZoomPixels/2 == 0)
				zoom = 1;
			else {
				zoom = DockPreferences.ZoomPercent - (offset/(double)(ZoomPixels/2))*(DockPreferences.ZoomPercent-1);
				zoom = (zoom-1)*ZoomIn+1;
			}
			
			offset = (int) ((offset*Math.Sin ((Math.PI/4)*zoom)) * (DockPreferences.ZoomPercent-1));
			
			if (Cursor.X > center) {
				center -= offset;
			} else {
				center += offset;
			}
			x = center;
		}
		
		Gdk.Rectangle GetDockArea ()
		{
			if (!CursorIsOverDockArea && ZoomIn == 0 && InputAreaOpacity == 0)
				return MinimumDockArea;

			int start_x, end_x;
			double start_zoom, end_zoom;
			IconPositionedCenterX (0, out start_x, out start_zoom);
			IconPositionedCenterX (DockItems.Length - 1, out end_x, out end_zoom);
			
			int x = start_x - (int)(start_zoom*(IconSize/2)) - HorizontalBuffer;
			int end = end_x + (int)(end_zoom*(IconSize/2)) + HorizontalBuffer;
			
			return new Gdk.Rectangle (x, Height-IconSize-2*VerticalBuffer, end-x, IconSize+2*VerticalBuffer);
		}
		
		void OnDockItemsChanged (IEnumerable<IDockItem> items)
		{
			SetIconRegions ();
			AnimatedDraw ();
		}
		
		void OnItemMenuRemoveClicked (Gdk.Point point)
		{
			int item = DockItemForX (point.X);
			item_provider.RemoveItem (item);
		}
		
		void OnItemMenuHidden (object o, System.EventArgs args)
		{
			int x, y;
			Display.GetPointer (out x, out y);
			
			Gdk.Rectangle geo;
			window.GetPosition (out geo.X, out geo.Y);
			
			x -= geo.X;
			y -= geo.Y;
			
			Cursor = new Gdk.Point (x, y);
			AnimatedDraw ();
		}
		
		#region Drag To Code
		protected override bool OnDragMotion (Gdk.DragContext context, int x, int y, uint time_)
		{
			Cursor = new Gdk.Point (x, y);
			AnimatedDraw ();
			return base.OnDragMotion (context, x, y, time_);
		}

		protected override void OnDragDataReceived (Gdk.DragContext context, int x, int y, Gtk.SelectionData selectionData, uint info, uint time)
		{
			string data = System.Text.Encoding.UTF8.GetString ( selectionData.Data );
			//sometimes we get a null at the end, and it crashes us
			data = data.TrimEnd ('\0'); 
			
			string[] uriList = Regex.Split (data, "\r\n");
			uriList.Where (uri => uri.StartsWith ("file://"))
				.ForEach (uri => item_provider.AddCustomItemFromFile (uri.Substring (7)));
			
			base.OnDragDataReceived (context, x, y, selectionData, info, time);
		}
		#endregion
		
		protected override bool OnExposeEvent(EventExpose evnt)
		{
			bool ret_val = base.OnExposeEvent (evnt);
			if (!IsDrawable)
				return ret_val;
			last_render = DateTime.UtcNow;
			
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
			cr2.SetSource (backbuffer, 0, VerticalOffset);
			cr2.Operator = Operator.Source;
			cr2.Paint ();
			(cr2 as IDisposable).Dispose ();
			
			return ret_val;
		}
 
		protected override bool OnMotionNotifyEvent(EventMotion evnt)
		{
			bool tmp = CursorIsOverDockArea;
			Cursor = new Gdk.Point ((int) evnt.X, (int) evnt.Y);
			
			if (cursor_is_handle && !((evnt.State & ModifierType.Button1Mask) == ModifierType.Button1Mask))
				EndDrag ();
			
			if (Math.Abs (Cursor.Y - MinimumDockArea.Y) < 5 || (cursor_is_handle && (evnt.State & ModifierType.Button1Mask) == ModifierType.Button1Mask)) {
				int item = DockItemForX (Cursor.X);
				if (!cursor_is_handle && item > 0 && DockItems[item] is SeparatorItem) {
					GdkWindow.Cursor = new Gdk.Cursor (CursorType.TopSide);
					cursor_is_handle = true;
					drag_start_y = Cursor.Y;
					drag_start_icon_size = DockPreferences.IconSize;
				}
			}
			if (cursor_is_handle && (evnt.State & ModifierType.Button1Mask) == ModifierType.Button1Mask) {
				DockPreferences.IconSize = drag_start_icon_size + (drag_start_y - Cursor.Y);
			}
			
			if (tmp != CursorIsOverDockArea || CursorIsOverDockArea && DateTime.UtcNow.Subtract (last_render).TotalMilliseconds > 20) 
				AnimatedDraw ();
			return base.OnMotionNotifyEvent (evnt);
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			bool ret_val = base.OnButtonPressEvent (evnt);
			
			// lets not do anything in this case
			if (cursor_is_handle) {
				EndDrag ();
				return ret_val;
			}
			
			// we are hovering over the pin icon
			Gdk.Rectangle stick_rect = new Gdk.Rectangle (StickIconCenter.X-4, StickIconCenter.Y-4, 8, 8);
			if (stick_rect.Contains (Cursor)) {
				DockPreferences.AutoHide = !DockPreferences.AutoHide;
				window.SetStruts ();
				AnimatedDraw ();
				return ret_val;
			}
			
			int item = DockItemForX ((int) evnt.X); //sometimes clicking is not good!
			if (item < 0 || item >= DockItems.Length || !CursorIsOverDockArea || InputInterfaceVisible)
				return ret_val;
			
			//handling right clicks
			if (evnt.Button == 3) {
				if (item_provider.GetIconSource (DockItems[item]) == IconSource.Custom || item_provider.GetIconSource (DockItems[item]) == IconSource.Statistics)
					ItemMenu.Instance.PopupAtPosition ((int) evnt.XRoot, (int) evnt.YRoot);
				return ret_val;
			}
			
			//send off the clicks
			DockItems[item].Clicked (evnt.Button, window.Controller);
			if (DockItems[item].LastClick > last_click)
				last_click = DockItems[item].LastClick;
			AnimatedDraw ();
			
			return ret_val;
		}
		
		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			Cursor = new Gdk.Point ((int) evnt.X, (int) evnt.Y);
			ModifierType leave_mask = ModifierType.Button1Mask | ModifierType.Button2Mask | 
				ModifierType.Button3Mask | ModifierType.Button4Mask | ModifierType.Button5Mask;
			
			if (CursorIsOverDockArea && (int) (evnt.State & leave_mask) == 0 && evnt.Mode == CrossingMode.Normal)
				Cursor = new Gdk.Point ((int) evnt.X, -1);
			return base.OnLeaveNotifyEvent (evnt);
		}
		
		protected override void OnRealized ()
		{
			base.OnRealized ();
			if (IsRealized)
				GdkWindow.SetBackPixmap (null, false);
		}
		
		protected override void OnStyleSet (Gtk.Style previous_style)
		{
			if (IsRealized)
					GdkWindow.SetBackPixmap (null, false);
			base.OnStyleSet (previous_style);
		}
		
		void EndDrag ()
		{
			GdkWindow.Cursor = new Gdk.Cursor (CursorType.LeftPtr);
			cursor_is_handle = false;
		}
		
		void SetIconRegions ()
		{
			Gdk.Rectangle pos;
			window.GetPosition (out pos.X, out pos.Y);
			
			for (int i=0; i<DockItems.Length; i++) {
				int x = IconNormalCenterX (i);
				DockItems[i].SetIconRegion (new Gdk.Rectangle (pos.X + (x-IconSize/2), pos.Y + (Height-VerticalBuffer-IconSize), IconSize, IconSize));
			}
		}
		
		void SetParentInputMask ()
		{
			if (CursorIsOverDockArea) {
				window.SetInputMask (GetDockArea ().Height*2 + 10);
			} else {
				if (DockPreferences.AutoHide)
					window.SetInputMask (1);
				else
					window.SetInputMask (GetDockArea ().Height);
			}
		}
		
		public void SetPaneContext (IUIContext context, Pane pane)
		{
			State.SetContext (context, pane);
			AnimatedDraw ();
		}
		
		public void ShowInputInterface ()
		{
			interface_change_time = DateTime.UtcNow;
			InputInterfaceVisible = true;
			
			AnimatedDraw ();
		}
		
		public void HideInputInterface ()
		{
			interface_change_time = DateTime.UtcNow;
			InputInterfaceVisible = false;
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