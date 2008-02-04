/* FileItem.cs
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
using System.IO;
using System.Collections;

using Mono.Unix;

using Do.Addins;

namespace Do.Universe
{
	/// <summary>
	/// FileItem is an item describing a file. FileItem subclasses
	/// can be created and registered with FileItem for instantiation
	/// in the factory method FileItem.Create.
	/// </summary>
	public class FileItem : IFileItem, IOpenableItem
	{
		
		/// <summary>
		/// Abbreviates an absolute path by replacing $HOME with ~.
		/// </summary>
		/// <param name="uri">
		/// A <see cref="System.String"/> containing a path.
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the abbreviated path.
		/// </returns>
		public static string ShortPath (string path)
		{
			string home;
			
			path = path ?? "";
			home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			path = path.Replace (home, "~");
			return path;
		}
		
		public static bool IsExecutable (IFileItem fi)
		{
			if (fi == null) return false;
			return IsExecutable (fi.Path);
		}

		public static bool IsExecutable (string path)
		{
			UnixFileInfo info;

			if (System.IO.Directory.Exists (path)) return false;

			info = new UnixFileInfo (path);
			return (info.FileAccessPermissions & FileAccessPermissions.UserExecute) != 0;
		}
		
		public static bool IsHidden (IFileItem fi)
		{
			if (fi == null) return false;
			return IsHidden (fi.Path);
		}

		public static bool IsHidden (string path)
		{
			FileInfo info;

			if (path.EndsWith ("~")) return true;

			info = new FileInfo (path);
			return (info.Attributes & FileAttributes.Hidden) != 0;
		}

		public static bool IsDirectory (IFileItem fi)
		{
			return IsDirectory (fi.Path);
		}


		public static bool IsDirectory (string path)
		{
			return Directory.Exists (path);
		}
		
		protected string path;
		
		/// <summary>
		/// Create a new FileItem for a given file.
		/// </summary>
		/// <param name="path">
		/// A <see cref="System.String"/> containing an absolute path to a file.
		/// </param>
		public FileItem (string path)
		{	
			this.path = path;
		}
		
		public virtual string Name
		{
			get {
				return System.IO.Path.GetFileName (Path);
			}
		}
		
		public virtual string Description
		{
			get {
				string short_path;
				
				short_path = ShortPath (Path);
				if (short_path == "~")
					// Sowing only "~" looks too abbreviated.
					return Path;
				else
					return short_path;
			}
		}
		
		public virtual string Icon
		{
			get {
				string icon;

				icon = MimeType;
				try {
					if (icon == "x-directory/normal") {
						icon = "folder";
					} else if (icon.StartsWith ("image")) {
						icon = "gnome-mime-image";
					} else {
						icon = icon.Replace ('/', '-');
						icon = string.Format ("gnome-mime-{0}", icon);
					}
				} catch (NullReferenceException) {
					icon = "file";
				}
				return icon;
			}
		}
		
		public string Path
		{
			get {
				return path;
			}
		}
		
		public string URI
		{
			get {
				return "file://" + Path;
			}
		}
		
		public string MimeType
		{
			get {
				return Gnome.Vfs.Global.GetMimeType (Path);
			}
		}

		public virtual void Open ()
		{
			string escaped_name;

			escaped_name = Path
				.Replace (" ", "\\ ")
				.Replace ("'", "\\'");
			Util.Environment.Open (escaped_name);
		}
	}
	
}
