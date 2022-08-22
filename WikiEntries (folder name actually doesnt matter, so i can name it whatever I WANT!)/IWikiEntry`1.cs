namespace Wikithis
{
	public interface IWikiEntry<in TKey>
	{
		string Search { get; }

		void OpenWikiPage(bool checkForKeybind = true);
	}
}
