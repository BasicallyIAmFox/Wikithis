namespace Wikithis
{
	public interface IWikiEntry<in TKey> where TKey : notnull
	{
		string Search { get; }

		void OpenWikiPage(bool checkForKeybind = true);
	}
}
