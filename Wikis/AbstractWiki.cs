using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis.Wikis;

public interface IWiki : IModType {
	IEnumerable<KeyValuePair<object, object>> Entries { get; }

	void Initialize();

	void AddEntry(object key, object value);

	object GetEntry(object key);
}

public abstract class AbstractWiki<TKey> : ModType, IWiki {
	private readonly Dictionary<TKey, IWikiEntry<TKey>> entries = new();

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

		GameCulture.CultureName culture = Wikithis.CultureLoaded;
		bool doesntContainsOthers = Wikithis.dataForMods.GetOrCreateValue(mod).URLs.ContainsKey(culture);
		if (!doesntContainsOthers)
			culture = GameCulture.CultureName.English;

		if (Wikithis.dataForMods.GetOrCreateValue(mod).URLs.TryGetValue(culture, out string value) && Wikithis.WikiUrlRegex.IsMatch(value)) {
			return Wikithis.WikiStrRegex.Replace(value, name);
		}

		throw new NotSupportedException("Update URL to new format!");
	}

	protected sealed override void Register() {
		Wikithis.wikis.Add(this);
		ModTypeLookup<IWiki>.Register(this);
	}
}
