using Terraria;

namespace Wikithis
{
	public readonly struct WikiEntry<TKey>
	{
		#region Fields
		/// <summary>
		/// The key, identifier of an entry.
		/// </summary>
		public readonly TKey Key;

		/// <summary>
		/// URL of an entry.
		/// </summary>
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
		/// <summary>
		/// Opens URL of an entry.
		/// </summary>
		/// <param name="checkForKeybind"></param>
		public void OpenWikiPage(bool checkForKeybind = true)
		{
			if (Search != null & Search != string.Empty & (!checkForKeybind || WikithisSystem.WikiKeybind.JustReleased))
			{
				Utils.OpenToURL(Wikithis.AprilFools && !WikithisSystem.RickRolled ? Wikithis.RickRoll : Search);
				if (Wikithis.AprilFools && !WikithisSystem.RickRolled)
					WikithisSystem.RickRolled = true;
			}
		}
		#endregion
	}
}
