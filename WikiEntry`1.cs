using Terraria;

namespace Wikithis
{
	public readonly struct WikiEntry<TKey> : IWikiEntry<TKey> where TKey : notnull
	{
		/// <summary>
		/// The key, identifier of an entry.
		/// </summary>
		public readonly TKey Key { get; }

		/// <summary>
		/// URL of an entry.
		/// </summary>
		public readonly string Search { get; }

		public WikiEntry(TKey key, string searchStr)
		{
			Key = key;
			Search = searchStr;
		}

		/// <summary>
		/// Opens URL of an entry.
		/// </summary>
		/// <param name="checkForKeybind"></param>
		public void OpenWikiPage(bool checkForKeybind = true)
		{
			if (!string.IsNullOrEmpty(Search) && (!checkForKeybind || WikithisSystem.WikiKeybind.JustReleased))
			{
				Utils.OpenToURL(Search);
			}
		}
	}
}
