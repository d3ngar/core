// FileItem.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
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
using IO = System.IO;
using System.Collections.Generic;

using Gnome;
using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace Do.Universe.Linux {

	/// <summary>
	/// FileItem is an item describing a file. FileItem subclasses
	/// can be created and registered with FileItem for instantiation
	/// in the factory method FileItem.Create.
	/// </summary>
	internal class FileItem : Item, IFileItem, IOpenableItem {

		static FileItem ()
		{
			Gnome.Vfs.Vfs.Initialize ();
		}

		/// <summary>
		/// Abbreviates an absolute path by replacing $HOME with ~.
		/// </summary>
		/// <param name="uri">
		/// A <see cref="System.String"/> containing a path.
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the abbreviated path.
		/// </returns>
		public static string DisplayPath (string path)
		{
			if (null == path) throw new ArgumentNullException ();

			return path.Replace (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "~");
		}

		string name, description, icon;
		
		/// <summary>
		/// Create a new FileItem for a given file.
		/// </summary>
		/// <param name="path">
		/// A <see cref="System.String"/> containing an absolute path to a file.
		/// </param>
		public FileItem (string path)
		{	
			if (null == path) throw new ArgumentNullException ("path");

			Path = path;
			name = IO.Path.GetFileName (Path);
			// Showing only "~" looks too abbreviated.
			description = DisplayPath (Path) == "~"
				? Path
				: DisplayPath (Path);
		}

		public string Path { get; private set; }
		public override string Name { get { return name; } }
		public override string Description { get { return description; } }

		public string Uri {
			get {
				return System.Uri.EscapeUriString ("file://" + Path);
			}
		}

		public string MimeType {
			get { return Gnome.Vfs.Global.GetMimeType (Path); }
		}

		public override string Icon {
			get {
				if (null != icon) return icon;

				// TODO Filenames with spaces are not
				string large_thumb = Thumbnail.PathForUri (Uri, ThumbnailSize.Large);
				string normal_thumb = Thumbnail.PathForUri (Uri, ThumbnailSize.Normal);
				// Generating the thumbnail ourself is too slow for large files.
				// Suggestion: generate thumbnails asynchronously. Banshee's
				// notion of job queues may be useful.
				if (IO.File.Exists (large_thumb)) {
					icon = large_thumb;	
				} else if (IO.File.Exists (normal_thumb)) {
					icon = normal_thumb;	
				} else {
					try {
						if (MimeType == "x-directory/normal") {
							icon = "folder";
						} else if (MimeType.StartsWith ("image")) {
							icon = "gnome-mime-image";
						} else {
							icon = MimeType.Replace ('/', '-');
							icon = string.Format ("gnome-mime-{0}", icon);
						}
					} catch (NullReferenceException) {
						icon = "gtk-file";
					}
				}
				return icon;
			}
		}

		public virtual void Open ()
		{
			Services.Environment.OpenPath (Path);
		}

	}
}
