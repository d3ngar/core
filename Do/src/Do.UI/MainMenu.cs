/* MainMenu.cs
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
using Gtk;
using Mono.Unix;

using Do;
using Do.Core;

namespace Do.UI
{
	public class MainMenu
	{
		static MainMenu instance;

		public static MainMenu Instance {
			get {
				return instance ??
					instance = new MainMenu ();
			}
		}

		Menu menu;
		int mainMenuX, mainMenuY;

		public MainMenu ()
		{
			MenuItem item;

			menu = new Menu ();

			// Preferences menu item
			item = new ImageMenuItem  ("_Preferences");
			(item as ImageMenuItem).Image = new Image (Stock.Preferences, IconSize.Menu);
			// menu.Add (item);
			item.CanFocus = false;
			item.Sensitive = false;

			// About menu item
			item = new ImageMenuItem  (Catalog.GetString ("_About Do"));
			(item as ImageMenuItem).Image = new Image (Stock.About, IconSize.Menu);
			menu.Add (item);
			item.CanFocus = false;
			item.Activated += OnMainMenuAboutClicked;

			item = new ImageMenuItem  (Catalog.GetString ("Plugin _Manager"));
			//TODO: See if we can find a better item to represent the addin manager
			(item as ImageMenuItem).Image = new Image (Stock.Preferences, IconSize.Menu);
			menu.Add (item);
			item.CanFocus = false;
			item.Activated += OnMainMenuPluginManagerClicked;
			
			// Quit menu item
			item = new ImageMenuItem (Catalog.GetString ("_Quit"));
			(item as ImageMenuItem).Image = new Image (Stock.Quit, IconSize.Menu);
			menu.Add (item);
			item.Activated += OnMainMenuQuitClicked;

			menu.ShowAll ();
		}

		protected void OnMainMenuQuitClicked (object o, EventArgs args)
		{
			Do.Controller.Vanish ();
			Application.Quit ();
		}

		protected void OnMainMenuPluginManagerClicked (object o, EventArgs args)
		{
			Do.Controller.ShowPluginManager ();
		}
		
		protected void OnMainMenuOpenPluginFolderClicked (object o, EventArgs args)
		{
			Do.Controller.Vanish ();
			Util.Environment.Open (Paths.UserPlugins);
		}

		protected void OnMainMenuAboutClicked (object o, EventArgs args)
		{
			Do.Controller.ShowAbout ();
		}

		public void PopupAtPosition (int x, int y)
		{
			mainMenuX = x;
			mainMenuY = y;	
			menu.Popup (null, null, PositionMainMenu, 3, Gtk.Global.CurrentEventTime);
		}

		private void PositionMainMenu (Menu menu, out int x, out int y, out bool push_in)
		{
			x = mainMenuX;
			y = mainMenuY;
			push_in = true;
		}
	}
}
