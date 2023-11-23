//
//    Copyright 2023 BasicallyIAmFox
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis.Wikis;

public interface IWiki : IModType, ILoadable {
	IEnumerable<KeyValuePair<object, object>> Entries { get; }

	void Initialize(LanguageManager languageManager);

	void AddEntry(object key, object value);

	object GetEntry(object key);
}

public interface IWiki<TKey, TEntry> : IWiki where TEntry : IWikiEntry<TKey> {
	new IEnumerable<KeyValuePair<TKey, TEntry>> Entries { get; }

	IEnumerable<KeyValuePair<object, object>> IWiki.Entries => Entries.Select(static pair => new KeyValuePair<object, object>(pair.Key, pair.Value));

	void AddEntry(TKey key, TEntry value);

	TEntry GetEntry(TKey key);

	void IWiki.AddEntry(object key, object value) {
		AddEntry((TKey)key, (TEntry)value);
	}

	object IWiki.GetEntry(object key) {
		return GetEntry((TKey)key);
	}
}

[Autoload(Side = ModSide.Client)]
public abstract class AbstractWiki<TKey, TEntry> : ModType, IWiki<TKey, TEntry> where TEntry : IWikiEntry<TKey> {
	private ImmutableDictionary<TKey, TEntry>.Builder _entriesBuilder = ImmutableDictionary.CreateBuilder<TKey, TEntry>();
	private ImmutableDictionary<TKey, TEntry> _entries;

	public IReadOnlyDictionary<TKey, TEntry> Entries => _entries;
	
	IEnumerable<KeyValuePair<TKey, TEntry>> IWiki<TKey, TEntry>.Entries => Entries;
	
	public LanguageManager LanguageManager { get; private set; }

	void IWiki.Initialize(LanguageManager languageManager) {
		LanguageManager = languageManager;
		
		Initialize();

		_entries = _entriesBuilder.ToImmutable();
		_entriesBuilder.Clear();
		_entriesBuilder = null;
	}
	
	public abstract void Initialize();

	public void AddEntry(TKey key, TEntry value) {
		_entriesBuilder.TryAdd(key, value);
	}

	public TEntry GetEntry(TKey key) {
		return _entries[key];
	}

	protected string DefaultSearchStr(string name, Mod mod) {
		name = name.Replace(' ', '_').Replace("'", "%27");

		if (mod == null) {
			const int l = 25; // length of "https://terraria.wiki.gg/wiki/"

			string url = $"https://terraria.wiki.gg/wiki/{name}";
			if (Wikithis.CurrentCulture == GameCulture.CultureName.Italian)
				url += "/it";
			else if (Wikithis.CurrentCulture != GameCulture.CultureName.English)
				url = url.Insert(l, LanguageManager.ActiveCulture.Name[..2] + '/');

			return url;
		}

		var modData = Wikithis.ModData.GetOrCreateValue(mod)!;

		if (modData.URLs == null)
			return string.Empty;

		var culture = Wikithis.CurrentCulture;
		bool doesntContainsOthers = modData.URLs.ContainsKey(culture);
		if (!doesntContainsOthers)
			culture = GameCulture.CultureName.English;

		if (!modData.URLs.TryGetValue(culture, out string value))
			return string.Empty;
		
		if (Wikithis.WikiUrlRegex.IsMatch(value))
			return Wikithis.WikiStrRegex.Replace(value, name);

		Throw(mod);
		return string.Empty;

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void Throw(Mod mod) {
			throw new NotSupportedException($"{mod.Name} has old Wiki URL. Please update it to get rid of this exception.");
		}
	}

	public override void Unload() {
		_entries = null;
	}

	protected sealed override void Register() {
		ModTypeLookup<IWiki>.Register(this);
	}
}
