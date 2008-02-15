//  MailtoAction.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Mono.Unix;

using Do.Addins;

namespace Do.Universe
{
	public class MailtoAction : AbstractAction
	{
		public override string Name
		{
			get { return Catalog.GetString ("Email"); }
		}
		
		public override string Description
		{
			get { return Catalog.GetString ("Compose a new email to a friend."); }
		}
		
		public override string Icon
		{
			get { return "email"; }
		}
		
		public override Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ContactItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			ContactItem contact = item as ContactItem;
			foreach (string detail in contact.Details)
				if (detail.StartsWith ("email"))
					return true;
			return false;
		}
		
		public override IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{
			string to;
			
			to = "";
			foreach (IItem item in items) {
				if (item is ContactItem) {
					ContactItem contact = item as ContactItem;
					string email = contact["email"];
					
					if (null == email) {
						foreach (string detail in contact.Details) {
							if (detail.StartsWith ("email")) {
								email = contact[detail];
								break;
							}
						}
					}
					to += email + ",";
				}
			}
			Util.Environment.Open ("mailto:" + to);
			return null;
		}
	}
}