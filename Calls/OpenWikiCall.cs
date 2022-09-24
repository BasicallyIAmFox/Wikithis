﻿using CCLiar;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Wikithis.Calls
{
	public sealed class OpenWikiCall : CCList, ILoadable
	{
		private static string[] array;

		public OpenWikiCall() : base(x => Array.IndexOf(array, x) != -1, args =>
		{
			string fullname = args.Get<string>(0);
			IConvertible key = args.Get<IConvertible>(1);
			bool withKeybind = args.Get<bool>(2);
			string name = fullname?.Split('/')[1];

			bool? isItemOrNpc = null;
			if (fullname?.Split('/')[0] is nameof(Wikithis))
				isItemOrNpc = name == nameof(ItemWiki);

			if (Wikithis.DelegateWikis.TryGetValue(fullname?.Split('/')[0] ?? "Terraria", out var delegates) && delegates.pageExists(name, key))
			{
				if (delegates.openPage(name, key))
				{
					goto tryOpenNormal;
				}
				return Wikithis.GotoSuccessReturn();
			}

		tryOpenNormal:
			if (isItemOrNpc is not null)
			{
				if (key is not IConvertible key2)
					throw new NotImplementedException();

				int keyy = key2.ToInt32(null);

				if (isItemOrNpc is true)
					(Wikithis.Wikis[$"{nameof(Wikithis)}/{nameof(ItemWiki)}"] as IWiki<Item, int>)?.GetEntry(keyy).OpenWikiPage(withKeybind);
				else
					(Wikithis.Wikis[$"{nameof(Wikithis)}/{nameof(NPCWiki)}"] as IWiki<NPC, int>)?.GetEntry(keyy).OpenWikiPage(withKeybind);

				return Wikithis.GotoSuccessReturn();
			}

			if (Wikithis.Wikis.TryGetValue(name, out IWiki value))
			{
				(value as IWiki<object, IConvertible>)?.GetEntry(key).OpenWikiPage(withKeybind);
			}
			return Wikithis.GotoSuccessReturn();
		}, new ICCKey[]
		{
			new CCKey<string>(),
			new CCKey<IConvertible>(),
			new CCOptionalKey<bool?>(() => true),
		})
		{
			array = new string[]
			{
				"5",
				"openwiki",
				"opencustomwiki"
			};
		}

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}