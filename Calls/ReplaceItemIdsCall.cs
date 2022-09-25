using CCLiar;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis.Calls
{
	public sealed class ReplaceItemIdsCall : CCList, ILoadable
	{
		private static string[] array;

		public ReplaceItemIdsCall() : base(x => Array.IndexOf(array, x) != -1, args =>
		{
			object itemId = args.Get<object>(0) is int ? args.Get<int>(0) : args.Get<List<int>>(0);
			string newName = args.Get<string>(1);
			GameCulture.CultureName culture = args.Get<GameCulture.CultureName>(2);

			List<int> items = new();
			if (itemId is int _i)
				items.Add(_i);
			else if (itemId is List<int> _l)
				items.AddRange(_l);
			Wikithis.ReplaceItemIds(newName, culture, items.ToArray());
			return Wikithis.GotoSuccessReturn();
		}, new ICCKey[]
		{
			new CCOrKey<int, List<int>>(),
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
