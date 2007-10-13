// IItem.cs created with MonoDevelop
// User: dave at 12:54 PM 8/29/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

namespace Do.Universe
{
	public interface IItem : IObject
	{
	}
	
	public interface ITextItem : IItem
	{
		string Text { get; }
	}
	
	public interface IURLItem : IItem
	{
		string URL { get; }
	}
	
	public interface IURIItem : IItem
	{
		string URI { get; }
	}

	public interface IRunnableItem : IItem
	{
		void Run ();
	}

	public interface IOpenableItem : IItem
	{
		void Open ();
	}
	
	public interface IFileItem : IURIItem
	{
		string MimeType { get; }
	}
}