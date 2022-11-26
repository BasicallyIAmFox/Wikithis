using System;
using Terraria;

namespace Wikithis {
	public readonly struct WikiEntry<TKey> : IWikiEntry where TKey : IConvertible {
		/// <summary>
		/// The key, identifier of an entry.
		/// </summary>
		public readonly TKey Key { get; }

		/// <summary>
		/// URL of an entry.
		/// </summary>
		public readonly string Search { get; }

		public WikiEntry(TKey key, string searchStr) {
			Key = key;
			Search = searchStr;
		}

		/// <summary>
		/// Opens URL of an entry.
		/// </summary>
		/// <param name="checkForKeybind"></param>
		public void OpenWikiPage(bool checkForKeybind = true) {
			if (!Main.instance.IsActive)
				return;

			if (new TimeSpan(IWikiEntry.weJustOpenedWiki?.ElapsedTicks ?? 0).TotalSeconds >= 0.5)
				IWikiEntry.weJustOpenedWiki = null;

			if (IWikiEntry.weJustOpenedWiki == null && !string.IsNullOrEmpty(Search) && (!checkForKeybind || WikithisSystem.WikiKeybind.JustReleased)) {
				Utils.OpenToURL(Search);

				IWikiEntry.weJustOpenedWiki = new();
				IWikiEntry.weJustOpenedWiki.Start();
			}
		}
	}
}
