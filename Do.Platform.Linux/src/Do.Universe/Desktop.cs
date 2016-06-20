// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace Gnome {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public class Desktop {
		[DllImport("gnome-desktop-3")]
		static extern IntPtr gnome_desktop_thumbnail_path_for_uri(IntPtr uri, int size);

		public static string ThumbnailPathForUri(string uri, Gnome.DesktopThumbnailSize size) {
			IntPtr native_uri = GLib.Marshaller.StringToPtrGStrdup (uri);
			IntPtr raw_ret = gnome_desktop_thumbnail_path_for_uri(native_uri, (int) size);
			string ret = GLib.Marshaller.PtrToStringGFree(raw_ret);
			GLib.Marshaller.Free (native_uri);
			return ret;
		}
#endregion
	}
}
