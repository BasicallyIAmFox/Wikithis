using Steamworks;
using Terraria;

namespace Wikithis.Wikis;

public interface IWikiEntry<TKey> {
	TKey Key { get; }

	void OpenWikiPage(bool checkForKeybind = true);
}

public readonly record struct WikiEntry<TKey>(TKey Key, string Search) : IWikiEntry<TKey> {
	public void OpenWikiPage(bool checkForKeybind = true) {
		if (string.IsNullOrEmpty(Search)) {
			return;
		}

		if ((!checkForKeybind || WikithisSystem.WikiKeybind.JustReleased) && Main.instance.IsActive) {
			if (!WikithisConfig.Config.OpenSteamBrowser) {
				Utils.OpenToURL(Search);
				return;
			}
			try {
				SteamFriends.ActivateGameOverlayToWebPage(Search);
			}
			catch {
				Utils.OpenToURL(Search);
			}
		}
	}
}
