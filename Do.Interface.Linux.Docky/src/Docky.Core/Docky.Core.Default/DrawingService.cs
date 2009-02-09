// DrawingService.cs
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

using Docky.Interface;

namespace Docky.Core.Default
{
	
	public class DrawingService  : IDrawingService
	{
		#region IDrawingService implementation 
		
		public Pango.Layout GetThemedLayout ()
		{
			return new Pango.Layout (DockWindow.Window.CreatePangoContext ());
		}
		
		public void RenderTextAtPoint (Cairo.Context cr, string text, Gdk.Point point, int maxWidth, Pango.Alignment align)
		{
			Pango.Layout layout = GetThemedLayout ();
			layout.Width = Pango.Units.FromPixels (maxWidth);
			layout.SetMarkup (text);
			layout.Ellipsize = Pango.EllipsizeMode.End;
			
			Pango.Rectangle rect1, rect2;
			layout.GetExtents (out rect1, out rect2);
			
			int transX = point.X - Pango.Units.ToPixels (rect2.Height) / 2;
			cr.Translate (transX, point.Y);
			Pango.CairoHelper.LayoutPath (cr, layout);
			cr.Color = new Cairo.Color (1, 1, 1);
			cr.Fill ();
			cr.Translate (0 - transX, 0 - point.Y);
		}
		
		#endregion 

		#region IDisposable implementation 
		
		public void Dispose ()
		{
		}
		
		#endregion 
		
	}
}
