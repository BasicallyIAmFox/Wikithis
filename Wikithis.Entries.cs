using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	partial class Wikithis
	{
		public static Dictionary<Type, Dictionary<string, WikiEntry>> WikiEntries = new();

		public static void AddEntries<T>(IEnumerable<T> all, Func<T, string> getKey, Func<T, string> getSearchStr, Func<T, bool> skipIf = null, (Func<T, int>, Dictionary<(int, GameCulture.CultureName), string>)? replacements = null)
		{
			if (!WikiEntries.ContainsKey(typeof(T)))
				WikiEntries.Add(typeof(T), new Dictionary<string, WikiEntry>());

			Dictionary<string, WikiEntry> wikiEntries = WikiEntries[typeof(T)];
			foreach (T i in all)
			{
				if (skipIf != null && skipIf.Invoke(i))
					continue;

				string key = getKey(i);
				if (replacements.HasValue && replacements.Value.Item2.TryGetValue((replacements.Value.Item1(i), CultureLoaded), out string key2))
					key = key2;

				if (wikiEntries.ContainsKey(key))
					continue;

				wikiEntries.Add(key, new WikiEntry(getKey(i), getSearchStr(i)));
			}
		}

		public static string DefaultSearchStr(string name, Mod mod)
		{
			name = name.Replace(' ', '_');
			name = name.Replace("'", "%27");

			if (mod == null)
			{
				const int l = 25; // length of "https://terraria.wiki.gg/wiki/"

				string url = $"https://terraria.wiki.gg/wiki/{name}";
				if (CultureLoaded == GameCulture.CultureName.Polish)
					url = url.Insert(l, "pl/");
				else if (CultureLoaded == GameCulture.CultureName.Italian)
					url += "/it";
				else if (CultureLoaded == GameCulture.CultureName.French)
					url = url.Insert(l, "fr/");
				else if (CultureLoaded == GameCulture.CultureName.Spanish)
					url = url.Insert(l, "pl/");
				else if (CultureLoaded == GameCulture.CultureName.Russian)
					url = url.Insert(l, "ru/");
				else if (CultureLoaded == GameCulture.CultureName.German)
					url = url.Insert(l, "de/");
				else if (CultureLoaded == GameCulture.CultureName.Portuguese)
					url = url.Insert(l, "pt/");
				else if (CultureLoaded == GameCulture.CultureName.Chinese)
					url = url.Insert(l, "zh/");

				return url;
			}
			else
			{
				string url = string.Empty;
				bool success = false;

				GameCulture.CultureName culture = CultureLoaded;

				bool doesntContainsOthers = ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(mod, culture), out _);
				if (!doesntContainsOthers)
					culture = GameCulture.CultureName.English;

				if (ModToURL.TryGetValue((mod, culture), out string value))
				{
					success = true;
					url = value;
				}

				if (!success)
					return string.Empty;

				string[] urls = url.Split('$');
				string[] urls2 = url.Split('♛');
				string result = $"https://{urls[0]}/wiki";

				if (urls.Length >= 2)
				{
					foreach (string v in urls.AsSpan(1))
					{
						result += $"/{v}";
					}
				}

				result += $"/{name}";
				if (urls2.Length > 1)
					result += $"/{urls2[1]}";

				return CheckURLValid(result) ? result : string.Empty;
			}
		}
	}

	public readonly struct WikiEntry
	{
		/// <summary>
		/// Full name of an item, helps to find Mod.
		/// </summary>
		public readonly string FullName;

		/// <summary>
		/// URL
		/// </summary>
		public readonly string Search;

		/// <summary>
		/// From what mod entry is.
		/// </summary>
		public readonly Mod Mod;

		public WikiEntry(string fullName, string searchStr)
		{
			FullName = fullName;
			Search = searchStr;

			Mod = FullName?.Split('/').Length > 1 ? ModLoader.GetMod(FullName?.Split('/')[0]) : null;
		}

		public void OpenWikiPage(bool checkForKeybind = false)
		{
			if (Search != string.Empty & (!checkForKeybind || WikithisSystem.WikiKeybind.JustReleased))
			{
				Utils.OpenToURL(Wikithis.AprilFools && !WikithisSystem.RickRolled ? Wikithis.RickRoll : Search);
				if (Wikithis.AprilFools && !WikithisSystem.RickRolled)
					WikithisSystem.RickRolled = true;
			}
		}
	}
}
