// Bezel.cs
// 
// Copyright (C) 2008 GNOME-Do
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

using Cairo;
using Gdk;
using Gtk;

using Do.Addins;
using Do.Universe;

namespace Do.UI
{
	
	
	public class Bezel : Gtk.Window, IDoWindow
	{
		BezelDrawingArea bda;
		BezelResultsWindow results_window;
		IDoController controller;
		PositionWindow pw;
		
		public Pane CurrentPane {
			get { return bda.Focus; }
			set { bda.Focus = value; }
		}
		
		public Bezel(IDoController controller) : base (Gtk.WindowType.Toplevel)
		{
			this.controller = controller;
			Build ();
		}
		
		void Build ()
		{
			Decorated = false;
			AppPaintable = true;
			KeepAbove = true;
//			DoubleBuffered = false;
			
			TypeHint = WindowTypeHint.Splashscreen;
			SetColormap ();
			
			bda = new BezelDrawingArea ();
			Add (bda);
			bda.Show ();
			
			results_window = new BezelResultsWindow ();
			results_window.WidthRequest = 325;
			
			pw = new PositionWindow (this, results_window);
		}
		
		protected override bool OnButtonPressEvent (EventButton evnt)
		{
			Gdk.Point global_point = new Gdk.Point ((int) evnt.XRoot, (int) evnt.YRoot);
			Gdk.Point local_point = new Gdk.Point ((int) evnt.X, (int) evnt.Y);
			
			switch (bda.GetPointLocation (local_point)) {
			case PointLocation.Close:
			case PointLocation.Outside:
				controller.ButtonPressOffWindow ();
				break;
			case PointLocation.Preferences:
				Addins.Util.Appearance.PopupMainMenuAtPosition (global_point.X, global_point.Y);
//				// Have to re-grab the pane from the menu.
				Addins.Util.Appearance.PresentWindow (this);
				break;
			}

			return base.OnButtonPressEvent (evnt);
		}
		
		protected override bool OnKeyPressEvent (EventKey evnt)
		{
			KeyPressEvent (evnt);

			return base.OnKeyPressEvent (evnt);
		}
		
		protected override bool OnExposeEvent (EventExpose evnt)
		{
			Cairo.Context cr = Gdk.CairoHelper.Create (GdkWindow);
//			cr.Color = new Cairo.Color (0, 0, 0, 0);
			cr.Operator = Cairo.Operator.Source;
			cr.Paint ();
			return base.OnExposeEvent (evnt);
		}

		
		protected virtual void SetColormap ()
		{
			Gdk.Colormap  colormap;

			colormap = Screen.RgbaColormap;
			if (colormap == null) {
				colormap = Screen.RgbColormap;
				Console.Error.WriteLine ("No alpha support.");
			}
			
			Colormap = colormap;
			colormap.Dispose ();
		}

		public void Summon ()
		{
			int width, height;
			GetSize (out width, out height);
			
			pw.UpdatePosition (0, Pane.First, new Gdk.Rectangle ((int) (width / 2) - (int) (325/2), 0, 0, 0));
			Show ();
			Util.Appearance.PresentWindow (this);
		}

		public void Vanish ()
		{
			Hide ();
		}

		public void Reset ()
		{
			bda.Clear ();
		}

		public void Grow ()
		{
			bda.ThirdPaneVisible = true;
		}

		public void Shrink ()
		{
			bda.ThirdPaneVisible = false;
		}

		public void GrowResults ()
		{
			results_window.Show ();
		}

		public void ShrinkResults ()
		{
			results_window.Hide ();
		}

		public void SetPaneContext (Pane pane, IUIContext context)
		{
			bda.BezelSetPaneObject (pane, context.Selection);
			bda.BezelSetQuery      (pane, context.Query);
			bda.BezelSetTextMode   (pane, context.LargeTextDisplay);
			if (context.LargeTextModeType == TextModeType.Explicit) {
				bda.BezelSetEntryMode (pane, true);
			} else {
				bda.BezelSetEntryMode (pane, false);
			}
			
			bda.Draw ();
			
			if (CurrentPane == pane)
				results_window.Context = context;
		}

		public void ClearPane (Pane pane)
		{
			bda.BezelSetPaneObject (pane, null);
			bda.BezelSetQuery (pane, string.Empty);
			bda.BezelSetEntryMode (pane, false);
			
			bda.Draw ();
		}

		
		public new event DoEventKeyDelegate KeyPressEvent;
	}
}
