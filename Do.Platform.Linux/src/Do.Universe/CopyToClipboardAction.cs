/* CopyToClipboardAction.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
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
using System.Linq;
using System.Collections.Generic;

using Gtk;
using Gdk;

using Mono.Unix;

using Do.Universe;

namespace Do.Universe.Linux
{

	public class CopyToClipboardAction : Act
	{

		protected override string Name {
			get { return Catalog.GetString ("Copy to Clipboard"); }
		}
		
		protected override string Description {
			get { return Catalog.GetString ("Copy current text to clipboard"); }
		}
		
		protected override string Icon {
			get { return "edit-paste"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (Item); }
		}
		
		protected override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string text = "";
			Item item = items.First ();

			if (item is ITextItem)
				text = (item as ITextItem).Text;
			else
				text = string.Format ("{0} - {1}", item.NameSafe, item.DescriptionSafe);
			
			Clipboard.Get (Gdk.Selection.Clipboard).Text =
				Clipboard.Get (Gdk.Selection.Primary).Text = text;

			return null;
		}
	}
}
