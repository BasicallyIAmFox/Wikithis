using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Wikithis
{
	public abstract class Wiki<TEntry, TKey> : ModType, IWiki<TEntry, TKey>
	{
		#region Fields
		private readonly Dictionary<TKey, WikiEntry<TKey>> _entries = new();

		private readonly Func<TEntry, TKey> _getKeyFunc;
		#endregion

		#region Constructors
		protected Wiki() { _getKeyFunc = null; }

		public Wiki(Func<TEntry, TKey> getKeyFunc) => _getKeyFunc = getKeyFunc;
		#endregion

		#region Methods
		public abstract void Initialize();

		public void AddEntry(TEntry entry, WikiEntry<TKey> wikiEntry) => _entries.TryAdd(_getKeyFunc(entry), wikiEntry);

		public WikiEntry<TKey> GetEntry(TKey key) => _entries.GetValueOrDefault(key);

		public bool HasEntry(TKey key) => _entries.ContainsKey(key);
		#endregion

		#region TML Methods
		public sealed override void SetStaticDefaults() { }
		public sealed override void SetupContent() { }
		protected sealed override void Register() => Wikithis.Wikis.Add(FullName, this);
		#endregion
	}
}
