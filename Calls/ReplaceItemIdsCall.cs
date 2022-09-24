using CCLiar;
using System;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis.Calls
{
	public sealed class ReplaceItemIdsCall : CCList, ILoadable
	{
		private static string[] array;

		public ReplaceItemIdsCall() : base(x => Array.IndexOf(array, x) != -1, args =>
		{
			int itemId = args.Get<int>(0);
			string newName = args.Get<string>(1);
			GameCulture.CultureName culture = args.Get<GameCulture.CultureName>(2);

			Wikithis.ReplaceItemIds(newName, culture, itemId);
			return Wikithis.GotoSuccessReturn();
		}, new ICCKey[]
		{
			new CCKey<int>(),
			new CCKey<string>(),
			new CCOptionalKey<GameCulture.CultureName?>(() => GameCulture.CultureName.English),
		})
		{
			array = new string[]
			{
				"1",
				"itemidreplacement",
				"itemidreplacements",
				"itemidsreplacement",
				"itemidsreplacements",
				"replaceitemid",
				"replaceitemids"
			};
		}

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}
