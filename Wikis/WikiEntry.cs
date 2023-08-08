using System.Diagnostics;
using Steamworks;
using Terraria;

namespace Wikithis.Wikis;

public interface IWikiEntry<out TKey> {
	TKey Key { get; }

	bool IsValid();

	void OpenWikiPage(bool checkForKeybind = true);
}

public readonly struct WikiEntry<TKey> : IWikiEntry<TKey> {
	private static readonly Stopwatch watch = Stopwatch.StartNew();

	public TKey Key { get; }
	public string Search { get; }

	public WikiEntry(TKey key, string search) {
		Key = key;
		Search = search;
	}

	public bool IsValid() => !string.IsNullOrEmpty(Search);

	public void OpenWikiPage(bool checkForKeybind = true) {
		if (!IsValid()) {
			return;
		}

		if ((!checkForKeybind || WikithisSystem.WikiKeybind.JustReleased) && watch.Elapsed.TotalSeconds >= 0.1) {
			watch.Restart();

			if (WikithisConfig.Config.OpenSteamBrowser) {
				try {
					SteamFriends.ActivateGameOverlayToWebPage(Search);
				}
				catch {
					Utils.OpenToURL(Search);
				}
			}
			else {
				Utils.OpenToURL(Search);
			}
		}
	}
}
