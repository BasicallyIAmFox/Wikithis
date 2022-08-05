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
		protected Wiki() => _getKeyFunc = null;

		public Wiki(Func<TEntry, TKey> getKeyFunc) => _getKeyFunc = getKeyFunc;
		#endregion

		#region Methods
		/// <summary>
		/// Used to link all <typeparamref name="TEntry"/> with their correspondent wiki pages.
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		/// Adds <typeparamref name="TEntry"/> entry in list.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="wikiEntry"></param>
		public void AddEntry(TEntry entry, WikiEntry<TKey> wikiEntry) => _entries.TryAdd(_getKeyFunc(entry), wikiEntry);

		/// <summary>
		/// Tries to get <typeparamref name="TEntry"/> entry using <paramref name="key"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>A <seealso cref="WikiEntry{TKey}"/> instance. If entry doesn't exists, it returns default value.</returns>
		public WikiEntry<TKey> GetEntry(TKey key) => _entries.GetValueOrDefault(key);

		/// <summary>
		/// Checks if entry for <typeparamref name="TKey"/> key exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool HasEntry(TKey key) => _entries.ContainsKey(key);

		/// <summary>
		/// Sends a message if <typeparamref name="TEntry"/> entry is invalid or doesn't exists.
		/// <br>This method mostly meant for debugging.</br>
		/// </summary>
		/// <param name="key">Key of entry.</param>
		public virtual void MessageIfDoesntExists(TKey key) => Wikithis.Instance.Logger.Info("My modder forgot to put message! Sorry!");
		#endregion

		#region TML Methods
		public sealed override void SetStaticDefaults() { }

		public sealed override void SetupContent() { }

		protected sealed override void Register() => Wikithis._wikis.Add(FullName, this);
		#endregion
	}
}
