/* SecurePreferencesService.cs 
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

using Do.Platform;

namespace Do.Platform.Default
{
	
	public class SecurePreferencesService : ISecurePreferencesService
	{

		#region ISecurePreferencesService

		public bool Set<T> (string key, T val)
		{
			Log.Debug ("Default ISecurePreferencesService cannot set key \"{0}\".", key);
			return false;
		}
		
		public bool TryGet<T> (string key, out T val)
		{
			Log.Debug ("Default ISecurePreferencesService cannot get key \"{0}\".", key);
			val = default (T);
			return false;
		}

		public string AbsolutePathForKey (string key)
		{
			Log.Debug ("Default ISecurePreferencesService cannot find absolute path for key \"{0}\".", key);
			return "";
		}

		#endregion ISecurePreferencesService
	}
}
