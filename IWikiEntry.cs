using System.Diagnostics;

namespace Wikithis
{
	public interface IWikiEntry
	{
		internal static Stopwatch weJustOpenedWiki = null;

		string Search { get; }

		void OpenWikiPage(bool checkForKeybind = true);
	}
}
