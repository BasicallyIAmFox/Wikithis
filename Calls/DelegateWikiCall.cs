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
			string mod = args.Get<string>(0);
			var noWiki = args.Get<Func<object, object, bool>>(1);
			var action = args.Get<Func<object, object, bool>>(2);

			Wikithis._delegateWikis.Add(mod, (noWiki, action));
			return Wikithis.GotoSuccessReturn();
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

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}
