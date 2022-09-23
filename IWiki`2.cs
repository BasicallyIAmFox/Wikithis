using System;

namespace Wikithis
{
	public interface IWiki<in TEntry, in TKey> : IWiki where TEntry : notnull where TKey : notnull, IConvertible
	{
		/// <summary>
		/// Tries to get <typeparamref name="TEntry"/> entry using <paramref name="key"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>A <seealso cref="WikiEntry{TKey}"/> instance. If entry doesn't exists, it returns default value.</returns>
		IWikiEntry<TKey> GetEntry(TKey key);

		/// <summary>
		/// Checks if entry for <typeparamref name="TKey"/> key exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool HasEntry(TKey key);

		/// <summary>
		/// Checks if entry for <paramref name="key"/> exists and is valid.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool IsValid(TKey key) => HasEntry(key) && Wikithis.CheckURLValid(GetEntry(key).Search);

		/// <summary>
		/// Sends a message if <typeparamref name="TEntry"/> entry is invalid or doesn't exists.
		/// <br>This method mostly meant for debugging.</br>
		/// </summary>
		/// <param name="key">Key of entry.</param>
		void MessageIfDoesntExists(TKey key);
	}
}
