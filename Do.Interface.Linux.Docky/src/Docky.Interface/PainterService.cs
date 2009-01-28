// PainterService.cs
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

using Docky.Core;

namespace Docky.Interface
{
	
	
	public class PainterService : IPainterService
	{
		List<IDockPainter> painters;
		DockArea parent;
		
		#region IPainterService implementation 
		
		public bool RequestShow (IDockPainter painter)
		{
			return parent.RequestShowPainter (painter);
		}
		
		public bool RequestHide (IDockPainter painter)
		{
			return parent.RequestHidePainter (painter);
		}

		#endregion 
		

		
		public PainterService (DockArea parent)
		{
			this.parent = parent;
			painters = new List<IDockPainter> ();
		}

		public void BuildPainters ()
		{
			painters.Add (new Painters.SummonModeRenderer ());
		}

		#region IDisposable implementation 
		
		public void Dispose ()
		{
			parent = null;
		}
		
		#endregion 
		
	}
}
