/* DoClassicWindow.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using Mono.Unix;

using Do.Universe;
using Gdk;
using Gtk;

namespace Do.Addins.UI
{
	
	
	public class DoClassicWindow : Gtk.Window, IDoWindow
	{
		GlossyRoundedFrame frame;
		SymbolDisplayLabel label;
		ResultsWindow resultsWindow;
		HBox resultsHBox;
		IconBox[] iconbox;
		GConf.Client gconfClient;
		
		const int DefaultIconBoxIconSize = 128;
		const uint DefaultIconBoxPadding = 6;
		const int DefaultIconBoxRadius = 20;

		const double WindowTransparency = 0.91;
		
		Pane currentPane;
		
		public event OnSelectionChanged SelectionChanged;

		public bool IsSummonable {
			get {
				throw new NotImplementedException();
			}
		}

		public Pane CurrentPane {
			get {
				return currentPane;
			}
			set {
				currentPane = value;
			}
		}
		
		int IconBoxIconSize {
			get {
				return DefaultIconBoxIconSize;
			}
		}

		uint IconBoxPadding {
			get {
				return DefaultIconBoxPadding;
			}
		}

		int IconBoxRadius {
			get {
				return DefaultIconBoxRadius;
			}
		}
		
		public DoClassicWindow() : base ("")
		{
		}
		
		protected void Build ()
		{
			VBox      vbox;
			Alignment align;
			Gtk.Image settings_icon;

			AppPaintable = true;
			KeepAbove = true;
			Decorated = false;
			// This typehint gets the window to raise all the way to top.
			TypeHint = WindowTypeHint.Splashscreen;

			try {
				SetIconFromFile ("/usr/share/icons/gnome/scalable/actions/system-run.svg");
			} catch { }
			SetColormap ();

			resultsWindow = new ResultsWindow ();
			resultsWindow.SelectionChanged += OnResultsWindowSelectionChanged;

			currentPane = Pane.First;

			frame = new GlossyRoundedFrame ();
			frame.DrawFill = true;
			frame.FillColor = BackgroundColor;
			frame.FillAlpha = WindowTransparency;
			frame.Radius = Screen.IsComposited ? IconBoxRadius : 0;
			Add (frame);
			frame.Show ();

			vbox = new VBox (false, 0);
			frame.Add (vbox);
			vbox.BorderWidth = IconBoxPadding;
			vbox.Show ();

			settings_icon = new Gtk.Image (GetType().Assembly, "settings-triangle.png");

			align = new Alignment (1.0F, 0.0F, 0, 0);
			align.SetPadding (3, 0, 0, IconBoxPadding);
			align.Add (settings_icon);
			vbox.PackStart (align, false, false, 0);
			settings_icon.Show ();
			align.Show ();

			resultsHBox = new HBox (false, (int) IconBoxPadding * 2);
			resultsHBox.BorderWidth = IconBoxPadding;
			vbox.PackStart (resultsHBox, false, false, 0);
			resultsHBox.Show ();

			iconbox = new IconBox[3];

			iconbox[0] = new IconBox (IconBoxIconSize);
			iconbox[0].IsFocused = true;
			iconbox[0].Radius = IconBoxRadius;
			resultsHBox.PackStart (iconbox[0], false, false, 0);
			iconbox[0].Show ();

			iconbox[1] = new IconBox (IconBoxIconSize);
			iconbox[1].IsFocused = false;
			iconbox[1].Radius = IconBoxRadius;
			resultsHBox.PackStart (iconbox[1], false, false, 0);
			iconbox[1].Show ();

			iconbox[2] = new IconBox (IconBoxIconSize);
			iconbox[2].IsFocused = false;
			iconbox[2].Radius = IconBoxRadius;
			resultsHBox.PackStart (iconbox[2], false, false, 0);
			// iconbox[2].Show ();

			align = new Alignment (0.5F, 0.5F, 1, 1);
			align.SetPadding (0, 2, 0, 0);
			label = new SymbolDisplayLabel ();
			align.Add (label);
			vbox.PackStart (align, false, false, 0);
			label.Show ();
			align.Show ();

			ScreenChanged += OnScreenChanged;
			ConfigureEvent += OnConfigureEvent;

			Reposition ();
		}
		
		private void OnResultsWindowSelectionChanged (object sender,
				ResultsWindowSelectionEventArgs args)
		{
			SelectionChanged (this, args);
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
		}
		
		private Gdk.Color BackgroundColor
		{
			get {
				byte r, g, b;
				Gdk.Color bgColor;

				bgColor = Gtk.Rc.GetStyle (this).Backgrounds[(int) StateType.Selected];
				r = (byte) ((bgColor.Red) >> 8);
				g = (byte) ((bgColor.Green) >> 8);
				b = (byte) ((bgColor.Blue) >> 8);
				
				// Useful for making overbright themes less ugly. Still trying
				// to find a happy balance between 50 and 90...
				byte maxLum = 60;
				RGBToHSV(ref r, ref g, ref b);
				b = Math.Min (b, maxLum);
				HSVToRGB(ref r, ref g, ref b);
				
				return new Gdk.Color (r, g, b);
			}
		}
		
		private void OnScreenChanged (object sender, EventArgs args)
		{
			SetColormap ();
		}
		
		void OnConfigureEvent (object sender, ConfigureEventArgs args)
		{
			Reposition ();
		}
		
		public void Reposition ()
		{
			int monitor;
			Gdk.Rectangle geo, main, results;
			
			GetPosition (out main.X, out main.Y);
			GetSize (out main.Width, out main.Height);
			monitor = Screen.GetMonitorAtPoint (main.X, main.Y);
			geo = Screen.GetMonitorGeometry (monitor);
			main.X = (geo.Width - main.Width) / 2;
			main.Y = (int)((geo.Height - main.Height) / 2.5);
			Move (main.X, main.Y);

			resultsWindow.GetSize (out results.Width, out results.Height);
			results.Y = main.Y + main.Height;
			results.X = main.X + (IconBoxIconSize + 60) * (int) currentPane + IconBoxRadius;
			resultsWindow.Move (results.X, results.Y);
		}

		public void Summon ()
		{
			throw new NotImplementedException();
		}

		public void Vanish ()
		{
			throw new NotImplementedException();
		}

		public void Reset ()
		{
			throw new NotImplementedException();
		}

		public void DisplayObjects (Do.Addins.SearchContext context)
		{
			throw new NotImplementedException();
		}

		public void ResultsWindowNext ()
		{
			throw new NotImplementedException();
		}

		public void ResultsWindowPrev ()
		{
			throw new NotImplementedException();
		}

		public void HideResultWindow ()
		{
			throw new NotImplementedException();
		}

		public void Grow ()
		{
			throw new NotImplementedException();
		}

		public void Shrink ()
		{
			throw new NotImplementedException();
		}

		public void DisplayInPane (Pane pane, IObject item)
		{
			throw new NotImplementedException();
		}

		public void DisplayInLabel (IObject item)
		{
			throw new NotImplementedException();
		}

		public void SetPaneHighlight (Pane pane, string highlight)
		{
			throw new NotImplementedException();
		}

		public void ClearPane (Pane pane)
		{
			throw new NotImplementedException();
		}
		
		private void RGBToHSV (ref byte r, ref byte g, ref byte b)
		{
			// Ported from Murrine Engine.
			double red, green, blue;
			double hue = 0, lum, sat;
			double max, min;
			double delta;
			
			red = (double) r;
			green = (double) g;
			blue = (double) b;
			
			max = Math.Max (red, Math.Max (blue, green));
			min = Math.Min (red, Math.Min (blue, green));
			delta = max - min;
			lum = max / 255.0 * 100.0;
			
			if (Math.Abs (delta) < 0.0001) {
				lum = 0;
				sat = 0;
			} else {
				sat = (delta / max) * 100;
				
				if (red == max)   hue = (green - blue) / delta;
				if (green == max) hue = 2 + (blue - red) / delta;
				if (blue == max)  hue = 4 + (red - green) / delta;
				
				hue *= 60;
				if (hue <= 0) hue += 360;
			}
			r = (byte) hue;
			g = (byte) sat;
			b = (byte) lum;
		}
		
		private void HSVToRGB (ref byte hue, ref byte sat, ref byte val)
		{
			double h, s, v;
			double r = 0, g = 0, b = 0;

			h = (double) hue;
			s = (double) sat / 100;
			v = (double) val / 100;

			if (s == 0) {
				r = v;
				g = v;
				b = v;
			} else {
				int secNum;
				double fracSec;
				double p, q, t;
				
				secNum = (int) Math.Floor(h / 60);
				fracSec = h/60 - secNum;

				p = v * (1 - s);
				q = v * (1 - s*fracSec);
				t = v * (1 - s*(1 - fracSec));

				switch (secNum) {
					case 0:
						r = v;
						g = t;
						b = p;
						break;
					case 1:
						r = q;
						g = v;
						b = p;
						break;
					case 2:
						r = p;
						g = v;
						b = t;
						break;
					case 3:
						r = p;
						g = q;
						b = v;
						break;
					case 4:
						r = t;
						g = p;
						b = v;
						break;
					case 5:
						r = v;
						g = p;
						b = q;
						break;
				}
			}
			hue = Convert.ToByte(r*255);
			sat = Convert.ToByte(g*255);
			val = Convert.ToByte(b*255);
		}
	}
}
