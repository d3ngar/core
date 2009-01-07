// AbstractMenuButtonArgs.cs
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
using Mono.Unix;

namespace Docky.Interface
{
	public abstract class AbstractMenuButtonArgs
	{
		const int MaxDescriptionCharacters = 50;
		
		public EventHandler Handler {
			get {
				return (sender, e) => Action ();
			}
		}
		
		public string Description {
			get; private set;
		}
		
		public string Icon {
			get; private set;
		}
		
		public bool Sensitive {
			get; private set; 
		}
		
		public AbstractMenuButtonArgs (string description, string icon, bool sensitive)
		{
			Description = GLib.Markup.EscapeText (Catalog.GetString (description));
			Icon = icon;
			Sensitive = sensitive;
		}
		
		public abstract void Action ();
	}
}
