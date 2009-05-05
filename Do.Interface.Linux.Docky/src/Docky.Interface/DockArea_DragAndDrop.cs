// DockArea_DragAndDrop.cs
// 
// Copyright (C) 2009 GNOME Do
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

using Gdk;
using Gtk;

using Do.Platform;
using Do.Interface;
using Do.Universe;
using Do.Interface.CairoUtils;
using Do;

using Docky.Core;
using Docky.Utilities;

namespace Docky.Interface
{
	
	
	internal partial class DockArea
	{
		enum DragEdge {
			None = 0,
			Top,
			Left,
			Right,
		}
		
		const int DragHotZoneSize = 8;

		bool drag_resizing;
		bool gtk_drag_source_set;
		
		int drag_start_icon_size;
		
		DragEdge drag_edge;

		Gdk.Point drag_start_point;
		
		Gdk.CursorType cursor_type;

		Gdk.Window drag_proxy;
		
		Gdk.DragContext drag_context;

		IEnumerable<string> uri_list;
		
		DragState DragState { get; set; }

		bool GtkDragging { get; set; }
		
		bool PreviewIsDesktopFile { get; set; }

		bool CursorNearTopDraggableEdge {
			get {
				return MinimumDockArea.Contains (Cursor) && CurrentDockItem is SeparatorItem;
			}
		}
		
		bool CursorNearLeftEdge {
			get {
				return CursorIsOverDockArea && Math.Abs (Cursor.X - MinimumDockArea.X) < DragHotZoneSize;
			}
		}
		
		bool CursorNearRightEdge {
			get {
				return CursorIsOverDockArea && Math.Abs (Cursor.X - (MinimumDockArea.X + MinimumDockArea.Width)) < DragHotZoneSize;
			}
		}
		
		bool CursorNearDraggableEdge {
			get {
				return CursorNearTopDraggableEdge || 
					   CursorNearRightEdge || 
					   CursorNearLeftEdge;
			}
		}
		
		DragEdge CurrentDragEdge {
			get {
				if (CursorNearTopDraggableEdge)
					return DragEdge.Top;
				else if (CursorNearLeftEdge)
					return DragEdge.Left;
				else if (CursorNearRightEdge)
					return DragEdge.Right;
				return DragEdge.None;
			}
		}

		void RegisterGtkDragSource ()
		{
			gtk_drag_source_set = true;
			TargetEntry te = new TargetEntry ("text/uri-list", TargetFlags.OtherApp | TargetFlags.App, 0);
			Gtk.Drag.SourceSet (this, Gdk.ModifierType.Button1Mask, new [] {te}, DragAction.Copy);
		}
		
		void RegisterGtkDragDest ()
		{
			TargetEntry dest_te = new TargetEntry ("text/uri-list", 0, 0);
			Gtk.Drag.DestSet (this, 0, new [] {dest_te}, Gdk.DragAction.Copy);
		}
		
		void UnregisterGtkDragSource ()
		{
			gtk_drag_source_set = false;
			Gtk.Drag.SourceUnset (this);
		}

		void SetDragProxy ()
		{
			if ((CursorModifier & ModifierType.Button1Mask) != ModifierType.Button1Mask || CursorIsOverDockArea) {
				if (drag_proxy == null)
					return;
				drag_proxy = null;
				RegisterGtkDragDest ();
			} else {
				Gdk.Point local_cursor = Cursor.RelativePointToRootPoint (window);
	
				IEnumerable<Gdk.Window> windows = WindowStack;

				foreach (Gdk.Window w in windows.Reverse ()) {
					if (w == null || w == window.GdkWindow || !w.IsVisible)
						continue;
					
					Gdk.Rectangle rect;
					int depth;
					w.GetGeometry (out rect.X, out rect.Y, out rect.Width, out rect.Height, out depth);
					if (rect.Contains (local_cursor)) {
						if (w == drag_proxy)
							break;
						
						drag_proxy = w;
						Gtk.Drag.DestSetProxy (this, w, DragProtocol.Xdnd, true);
						break;
					}
				}
			}
		}

		void DragCursorUpdate ()
		{
			if (GtkDragging && (CursorModifier & ModifierType.Button1Mask) != ModifierType.Button1Mask) {
				GtkDragging = false;
			}
			SetDragProxy ();
		}

		protected override bool OnDragMotion (Gdk.DragContext context, int x, int y, uint time)
		{
			GtkDragging = true;
			
			do {
				if (DragState.DragItem == null || DragState.IsFinished || 
				    !DockItems.Contains (DragState.DragItem) || !CursorIsOverDockArea)
					continue;
				
				int draggedPosition = DockItems.IndexOf (DragState.DragItem);
				int currentPosition = PositionProvider.IndexAtPosition (Cursor);
				if (draggedPosition == currentPosition || currentPosition == -1)
					continue;
				
				DockServices.ItemsService.MoveItemToPosition (draggedPosition, currentPosition);
			} while (false);
			
			AnimatedDraw ();
			
			if (drag_context != context) {
				Gdk.Atom target = Gtk.Drag.DestFindTarget (this, context, null);
				Gtk.Drag.GetData (this, context, target, Gtk.Global.CurrentEventTime);
				drag_context = context;
			}
			
			Gdk.Drag.Status (context, DragAction.Copy, time);
			base.OnDragMotion (context, x, y, time);
			return true;
		}
		
		protected override bool OnDragDrop (Gdk.DragContext context, int x, int y, uint time)
		{
			int index = PositionProvider.IndexAtPosition (Cursor);
			if (CursorIsOverDockArea && index >= 0 && index < DockItems.Count) {
				foreach (string uri in uri_list) {
					if (CurrentDockItem != null && CurrentDockItem.IsAcceptingDrops && !uri.EndsWith (".desktop")) {
						CurrentDockItem.ReceiveItem (uri);
					} else {
						Gdk.Point center = PositionProvider.IconUnzoomedPosition (index);
						if (center.X < Cursor.X && index < DockItems.Count - 1)
							index++;
						DockServices.ItemsService.AddItemToDock (uri, index);
					}
				}
			}
			
			Gtk.Drag.Finish (context, true, true, time);
			return base.OnDragDrop (context, x, y, time);
		}

		protected override void OnDragDataReceived (Gdk.DragContext context, int x, int y, 
		                                            Gtk.SelectionData selectionData, uint info, uint time)
		{
			IEnumerable<string> uriList;
			try {
				string data = System.Text.Encoding.UTF8.GetString (selectionData.Data);
				data = System.Uri.UnescapeDataString (data);
				//sometimes we get a null at the end, and it crashes us
				data = data.TrimEnd ('\0'); 
				
				uriList = Regex.Split (data, "\r\n")
					.Where (uri => uri.StartsWith ("file://"))
					.Select (uri => uri.Substring ("file://".Length));
			} catch {
				uriList = Enumerable.Empty<string> ();
			}
			
			
			uri_list = uriList;
				
			PreviewIsDesktopFile = !uriList.Any () || uriList.Any (s => s.EndsWith (".desktop"));
			
			base.OnDragDataReceived (context, x, y, selectionData, info, time);
		}
		
		protected override void OnDragBegin (Gdk.DragContext context)
		{
			GtkDragging = true;
			// the user might not end the drag on the same horizontal position they start it on
			int item = PositionProvider.IndexAtPosition (Cursor);

			if (item != -1 && DockServices.ItemsService.ItemCanBeMoved (item))
				DragState = new DragState (Cursor, DockItems [item]);
			else
				DragState = new DragState (Cursor, null);
			
			Gdk.Pixbuf pbuf;
			if (DragState.DragItem == null) {
				pbuf = IconProvider.PixbufFromIconName ("gtk-remove", DockPreferences.IconSize);
			} else {
				pbuf = DragState.DragItem.GetDragPixbuf ();
			}
				
			if (pbuf != null)
				Gtk.Drag.SetIconPixbuf (context, pbuf, pbuf.Width / 2, pbuf.Height / 2);
			base.OnDragBegin (context);
		}
		
		protected override void OnDragEnd (Gdk.DragContext context)
		{
			if (CursorIsOverDockArea) {
				int currentPosition = PositionProvider.IndexAtPosition (Cursor);
				if (currentPosition != -1)
					DockServices.ItemsService.DropItemOnPosition (DragState.DragItem, currentPosition);
			} else {
				DockServices.ItemsService.RemoveItem (DragState.DragItem);
			}
			
			DragState.IsFinished = true;
			GtkDragging = false;
			SetDragProxy ();
			
			AnimatedDraw ();
			base.OnDragEnd (context);
		}
		
		void BuildDragAndDrop ()
		{
			cursor_type = CursorType.LeftPtr;

			DragState = new DragState (Cursor, null);
			DragState.IsFinished = true;
		}
		
		void ConfigureCursor ()
		{
			// we do this so that our custom drag isn't destroyed by gtk's drag
			if (gtk_drag_source_set && CursorNearDraggableEdge) {
				UnregisterGtkDragSource ();

				if (cursor_type != CursorType.SbVDoubleArrow && CursorNearTopDraggableEdge) {
					SetCursor (CursorType.SbVDoubleArrow);
					
				} else if (cursor_type != CursorType.LeftSide && CursorNearLeftEdge) {
					SetCursor (CursorType.LeftSide);
					
				} else if (cursor_type != CursorType.RightSide && CursorNearRightEdge) {
					SetCursor (CursorType.RightSide);
				}
				
			} else if (!gtk_drag_source_set && !drag_resizing && !CursorNearDraggableEdge) {
				if (!PainterOverlayVisible)
					RegisterGtkDragSource ();
				if (cursor_type != CursorType.LeftPtr)
					SetCursor (CursorType.LeftPtr);
			}
		}
		
		void SetCursor (Gdk.CursorType type)
		{
			cursor_type = type;
			Gdk.Cursor tmp_cursor = new Gdk.Cursor (type);
			GdkWindow.Cursor = tmp_cursor;
			tmp_cursor.Dispose ();
		}

		void StartDrag ()
		{
			drag_start_point = Cursor;
			drag_start_icon_size = DockPreferences.IconSize;
			drag_resizing = true;
			drag_edge = CurrentDragEdge;
		}
		
		void EndDrag ()
		{
			drag_edge = DragEdge.None;
			drag_resizing = false;
			SetIconRegions ();
			window.SetStruts ();
			
			AnimatedDraw ();
			
			ResetCursorTimer ();
			
			Reconfigure ();
		}
		
		void HandleDragMotion ()
		{
			int movement = 0;
			switch (drag_edge) {
			case DragEdge.Top:
				int delta = drag_start_point.Y - Cursor.Y;
				if (DockPreferences.Orientation == DockOrientation.Top)
					delta = 0 - delta;
				DockPreferences.IconSize = Math.Min (drag_start_icon_size + delta, DockPreferences.MaxIconSize);
				return;
			case DragEdge.Left:
				movement = drag_start_point.X - Cursor.X;
				break;
			case DragEdge.Right:
				movement = Cursor.X - drag_start_point.X;
				break;
			}

			if (movement > IconSize / 2 + 2) {
				DockPreferences.AutomaticIcons++;
			} else if (movement < 0 - (IconSize / 2 + 2)) {
				DockPreferences.AutomaticIcons--;
			} else {
				return;
			}
			
			drag_start_point = Cursor;
		}
	}
}
