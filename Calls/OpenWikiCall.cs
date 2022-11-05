using CCLiar;
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
			string fullname = args.Get<string>(0, _ => string.IsNullOrWhiteSpace(_));
			IConvertible key = args.Get<IConvertible>(1);
			bool withKeybind = args.Get<bool>(2);

			return Call(fullname, key, withKeybind);
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

		public static bool Call(string fullname, IConvertible key, bool withKeybind = true)
		{
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
				goto gotoFailed;
			}

		tryOpenNormal:
			if (isItemOrNpc is not null)
			{
				if (key is not IConvertible key2)
					throw new InvalidCastException();

				int keyy = key2.ToInt32(null);
				Wikithis.Wikis[$"{nameof(Wikithis)}/{name}"].GetEntry(keyy)?.OpenWikiPage(withKeybind);

				goto gotoSuccess;
			}

			if (Wikithis.Wikis.TryGetValue(name, out IWiki value))
			{
				value.GetEntry(key)?.OpenWikiPage(withKeybind);
				goto gotoSuccess;
			}

		gotoFailed:
			return Wikithis.GotoFailedReturn();
		gotoSuccess:
			return Wikithis.GotoSuccessReturn();
		}

		public static bool Call(Mod mod, string name, IConvertible key, bool withKeybind = true) => Call($"{mod.Name}/{name}", key, withKeybind);

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}
