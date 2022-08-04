using Terraria;

namespace Wikithis
{
	public readonly struct WikiEntry<TKey>
	{
		#region Fields
		public readonly TKey Key;

		public readonly string Search;
		#endregion

		#region Constructors
		public WikiEntry(TKey key, string searchStr)
		{
			Key = key;
			Search = searchStr;
		}
		#endregion

		#region Methods
		public void OpenWikiPage(bool checkForKeybind = true)
		{
			if (Search != string.Empty & (!checkForKeybind || WikithisSystem.WikiKeybind.JustReleased))
			{
				Utils.OpenToURL(Wikithis.AprilFools && !WikithisSystem.RickRolled ? Wikithis.RickRoll : Search);
				if (Wikithis.AprilFools && !WikithisSystem.RickRolled)
					WikithisSystem.RickRolled = true;
			}
		}
		#endregion
	}
}
