using CCLiar;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis.Calls
{
	public sealed class CustomWikiCall : CCList, ILoadable
	{
		private static string[] array;

		public CustomWikiCall() : base(x => Array.IndexOf(array, x) != -1, args =>
		{
			var mod = args.Get<Mod>(0, _ => _ == null);
			var name = args.Get<string>(1, _ => string.IsNullOrWhiteSpace(_));
			var key = args.Get<Func<object, IConvertible>>(2, _ => _ == null);
			var initialize = args.Get<Action<Func<IConvertible, bool>, Action<object, IConvertible, string>, Func<string, Mod, string>>>(3, _ => _ == null);
			var noExists = args.Get<Action<IDictionary<(Mod, GameCulture.CultureName), string>, GameCulture.CultureName, object>>(4, _ => _ == null);

			return Call(mod, name, key, initialize, noExists);
		}, new ICCKey[]
		{
			new CCKey<Mod>(),
			new CCKey<string>(),
			new CCKey<Func<object, IConvertible>>(),
			new CCKey<Action<Func<IConvertible, bool>, Action<object, IConvertible, string>, Func<string, Mod, string>>>(),
			new CCKey<Action<IDictionary<(Mod, GameCulture.CultureName), string>, GameCulture.CultureName, object>>(),
		})
		{
			array = new string[]
			{
				"4",
				"customwiki",
				"addcustomwiki"
			};
		}

		public static bool Call(Mod mod, string name, Func<object, IConvertible> key, Action<Func<IConvertible, bool>, Action<object, IConvertible, string>, Func<string, Mod, string>> init, Action<IDictionary<(Mod, GameCulture.CultureName), string>, GameCulture.CultureName, object> noExists)
		{
			mod.AddContent(new SealedWiki(name, key, init, noExists));
			return Wikithis.GotoSuccessReturn();
		}

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}
