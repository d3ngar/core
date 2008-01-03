/* InternalItemSource.cs
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
using System.Collections.Generic;

using Do.Core;

namespace Do.Universe
{
	public class InternalItemSource : IItemSource
	{
		public static readonly ProxyItem LastItem = new ProxyItem ("Last Item",
		                                                           "The last item used in a command.",
		                                                           "undo");
		private List<IItem> items;
		
		public InternalItemSource ()
		{
			items = new List<IItem> ();
			items.Add (LastItem);
			items.Add (new SelectedTextItem ());
		}
		
		public Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
				};
			}
		}
		
		public string Name
		{
			get { return "Internal GNOME Do Items"; }
		}
		
		public string Description
		{
			get { return "Special items relevant to the inner-workings of GNOME Do."; }
		}
		
		public string Icon
		{
			get { return "gnome-system"; }
		}
		
		
		public void UpdateItems ()
		{
		}
		
		public ICollection<IItem> Items
		{
			get { return items; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
	}
}