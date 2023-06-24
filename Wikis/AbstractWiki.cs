using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis.Wikis;

public interface IWiki : IModType, ILoadable {
	IEnumerable<KeyValuePair<object, object>> Entries { get; }

	void Initialize();

	void AddEntry(object key, object value);

	object GetEntry(object key);
}

public abstract class AbstractWiki<TKey> : ModType, IWiki {
	private Dictionary<TKey, IWikiEntry<TKey>> entries = new();

	public IReadOnlyDictionary<TKey, IWikiEntry<TKey>> Entries => entries;

	IEnumerable<KeyValuePair<object, object>> IWiki.Entries => Entries.Cast<KeyValuePair<object, object>>();

	public abstract void Initialize();

	public void AddEntry(TKey key, IWikiEntry<TKey> value) => entries.TryAdd(key, value);
	public IWikiEntry<TKey> GetEntry(TKey key) => entries[key];

	void IWiki.AddEntry(object key, object value) => AddEntry((TKey)key, (IWikiEntry<TKey>)value);
	object IWiki.GetEntry(object key) => GetEntry((TKey)key);

	protected static string DefaultSearchStr(string name, Mod mod) {
		name = name.Replace(' ', '_').Replace("'", "%27");

		string url;
		if (mod == null) {
			const int l = 25; // length of "https://terraria.wiki.gg/wiki/"

			url = $"https://terraria.wiki.gg/wiki/{name}";
			if (Wikithis.CultureLoaded == GameCulture.CultureName.Italian)
				url += "/it";
			else if (Wikithis.CultureLoaded != GameCulture.CultureName.English)
				url = url.Insert(l, Language.ActiveCulture.Name[..2] + '/');

			return url;
		}

#if !TML_2022_09
		GameCulture.CultureName culture = Wikithis.CultureLoaded;
		bool doesntContainsOthers = Wikithis.ModData.GetOrCreateValue(mod).URLs.ContainsKey(culture);
		if (!doesntContainsOthers)
			culture = GameCulture.CultureName.English;

		if (Wikithis.ModData.GetOrCreateValue(mod).URLs.TryGetValue(culture, out string value)) {
			if (Wikithis.WikiUrlRegex.IsMatch(value)) {
				return Wikithis.WikiStrRegex.Replace(value, name);
			}
			Throw(mod);
		}

		return string.Empty;

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void Throw(Mod mod) {
			throw new NotSupportedException($"{mod.Name} has old Wiki URL. Please update it to get rid of this exception.");
		}
#else
		string result;
		bool success = false;
		url = string.Empty;

		GameCulture.CultureName culture = Wikithis.CultureLoaded;

		if (Wikithis.ModData.GetOrCreateValue(mod).URLs == null) return string.Empty;

		bool doesntContainsOthers = Wikithis.ModData.GetOrCreateValue(mod).URLs.TryGetValue(culture, out _);
		if (!doesntContainsOthers)
			culture = GameCulture.CultureName.English;

		if (Wikithis.ModData.GetOrCreateValue(mod).URLs.TryGetValue(culture, out string value)) {
			success = true;
			url = value;
		}

		if (success && Wikithis.WikiUrlRegex.IsMatch(value))
			return result = Wikithis.WikiStrRegex.Replace(value, name);

		if (!success)
			return string.Empty;

		string[] urls = url.Split('$');
		string[] urls2 = url.Split('♛');
		result = $"https://{urls[0]}/wiki";

		if (urls.Length >= 2) {
			foreach (string v in urls.AsSpan(1)) {
				result += $"/{v}";
			}
		}

		result += $"/{name}";
		if (urls2.Length > 1)
			result += $"/{urls2[1]}";

		return result;
#endif
	}

	public override void Unload() {
		entries.Clear();
		entries = null;
	}

	protected sealed override void Register() {
		ModTypeLookup<IWiki>.Register(this);
	}
}
