namespace Wikithis
{
	public interface IWiki<in TEntry, TKey> : IWiki
	{
		void AddEntry(TEntry entry, WikiEntry<TKey> wikiEntry);

		WikiEntry<TKey> GetEntry(TKey key);

		bool HasEntry(TKey key);

		bool HasEntryAndIsValid(TKey key) => HasEntry(key) && Wikithis.CheckURLValid(GetEntry(key).Search);

		bool IsValid(TKey key) => HasEntry(key) && Wikithis.CheckURLValid(GetEntry(key).Search);

		void MessageIfDoesntExists(TKey key);
	}
}
