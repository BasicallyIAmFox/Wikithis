namespace Wikithis
{
	public interface IWiki
	{
		void Initialize();

		IWikiEntry GetEntry(object key);

		bool HasEntry(object key);

		bool IsValid(object key) => HasEntry(key) && Wikithis.CheckURLValid(GetEntry(key).Search);

		void MessageIfDoesntExists(object key);
	}
}
