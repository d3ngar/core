/* ItemSource.cs
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
using System.Linq;
using System.Collections.Generic;

namespace Do.Universe
{
	/// <summary>
	/// A source of Items.
	/// Example: A "EpiphanyBookmarkItemSource" could provide Items representing
	/// Epiphany web browser bookmarks.
	/// </summary>
	public abstract class ItemSource : Element
	{
		
		/// <value>
		/// Item sub-types provided/supported by this source. These include any
		/// types of items provided by the Items property, and the types of items
		/// that this source will provide children for.  Please provide types as
		/// close as possible in ancestry to the static types of items this source
		/// provides/supports (e.g.  FirefoxBookmarkItem instead of Item or
		/// BookmarkItem).
		/// </value>
		public abstract IEnumerable<Type> SupportedItemTypes { get; }
		
		/// <value>
		/// The Items provided by this source.
		/// null is ok---it signifies that no items are provided.
		/// </value>
		protected virtual IEnumerable<Item> Items {
			get { yield break; }
		}
		
		/// <summary>
		/// Provides a collection of children of an item. Item is guaranteed to be a
		/// subtype of a type in SupportedItemTypes.
		/// null is ok---it signifies that no children are provided for the Item
		/// argument.
		/// </summary>
		protected virtual IEnumerable<Item> ChildrenOfItem (Item item)
		{
			yield break;
		}
		
		/// <summary>
		/// When called, the source should make sure the collection of Items
		/// returned on subsequent accesses of the Items property is up to date.
		/// Example: Re-read bookmarks from the filesystem or check for new email,
		/// etc.
		/// </summary>
		protected virtual void UpdateItems ()
		{
		}

		#region Safe alternatives
		
		public void UpdateItemsSafe ()
		{
			try {
				UpdateItems ();
			} catch (Exception e) {
				LogSafeError ("UpdateItems", e);
			}
		}
		
		public IEnumerable<Item> ItemsSafe
		{
			get {
				IEnumerable<Item> items = null;
				
				try {
					// Strictly evaluate the IEnumerable before we leave the try block.
					items = Items.ToArray ();
				} catch (Exception e) {
					items = null;
					LogSafeError ("Items", e);
				} finally {
					items = items ?? Enumerable.Empty<Item> ();
				}

				return items;
			}
		}

		public IEnumerable<Item> ChildrenOfItemSafe (Item item)
		{
			IEnumerable<Item> children = null;

			item = ProxyItem.Unwrap (item);

			if (!SupportedItemTypes.Any (type => type.IsInstanceOfType (item)))
			    return Enumerable.Empty<Item> ();

			try {
				// Strictly evaluate the IEnumerable before we leave the try block.
				children = ChildrenOfItem (item).ToArray ();
			} catch (Exception e) {
				children = null;
				LogSafeError ("ChildrenOfItem", e);
			} finally {
				children = children ?? Enumerable.Empty<Item> ();
			}
			return children;
		}
		#endregion
	}

}
