//
//    Copyright 2023-2024 BasicallyIAmFox
//
//    Licensed under the Apache License, Version 2.0 (the "License")
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using Steamworks;
using System;
using System.Diagnostics;
using Terraria;

namespace Wikithis.Wikis;

public interface IWikiEntry<out TKey> {
	TKey Key { get; }

	string Search { get; }

	bool IsValid();

	void OpenWikiPage(bool checkForKeybind = true);
}

internal static class WikiEntry {
	private static readonly Stopwatch Watch = new();

	public static void StartTicking() {
		Watch.Start();
	}

	public static void RestartTicking() {
		Watch.Restart();
	}

	public static TimeSpan GetElapsedTime() {
		return Watch.Elapsed;
	}
}

public readonly struct WikiEntry<TKey> : IWikiEntry<TKey> {
	static WikiEntry() {
		WikiEntry.StartTicking();
	}

	public TKey Key { get; }
	public string Search { get; }

	public WikiEntry(TKey key, string search) {
		Key = key;
		Search = search;
	}

	public bool IsValid() {
		return !string.IsNullOrEmpty(Search);
	}

	public void OpenWikiPage(bool checkForKeybind = true) {
		if (!IsValid()) {
			return;
		}

		if (checkForKeybind && !WikithisSystem.WikiKeybind.JustReleased || WikiEntry.GetElapsedTime().TotalSeconds < 0.1)
			return;

		WikiEntry.RestartTicking();

		if (WikithisConfig.Config.OpenSteamBrowser)
			try {
				SteamFriends.ActivateGameOverlayToWebPage(Search);
			}
			catch {
				Utils.OpenToURL(Search);
			}
		else
			Utils.OpenToURL(Search);
	}
}
