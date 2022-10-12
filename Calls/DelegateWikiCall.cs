using CCLiar;
using System;
using Terraria.ModLoader;

namespace Wikithis.Calls
{
	public sealed class DelegateWikiCall : CCList, ILoadable
	{
		private static string[] array;

		public DelegateWikiCall() : base(x => Array.IndexOf(array, x) != -1, args =>
		{
			string mod = args.Get<string>(0, _ => string.IsNullOrWhiteSpace(_));
			var noWiki = args.Get<Func<object, object, bool>>(1, _ => _ == null);
			var action = args.Get<Func<object, object, bool>>(2, _ => _ == null);

			return Call(mod, noWiki, action);
		}, new ICCKey[]
		{
			new CCKey<string>(),
			new CCKey<Func<object, object, bool>>(),
			new CCKey<Func<object, object, bool>>(),
		})
		{
			array = new string[]
			{
				"6",
				"delegatewiki",
				"adddelegatewiki",
			};
		}

		public static bool Call(Mod mod, Func<object, object, bool> noWiki, Func<object, object, bool> action) => Call(mod.Name, noWiki, action);

		public static bool Call(string mod, Func<object, object, bool> noWiki, Func<object, object, bool> action)
		{
			Wikithis._delegateWikis.Add(mod, (noWiki, action));
			return Wikithis.GotoSuccessReturn();
		}

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}
