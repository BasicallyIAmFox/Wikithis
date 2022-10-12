using CCLiar;
using System;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis.Calls
{
	public sealed class AddModUrlCall : CCList, ILoadable
	{
		private static string[] array;

		public AddModUrlCall() : base(x => Array.IndexOf(array, x) != -1, args =>
		{
			Mod mod = args.Get<Mod>(0, _ => _ == null);
			string domain = args.Get<string>(1, _ => string.IsNullOrWhiteSpace(_));
			GameCulture.CultureName culture = args.Get<GameCulture.CultureName>(2);

			Call(mod, domain, culture);
			return Wikithis.GotoSuccessReturn();
		}, new ICCKey[]
		{
			new CCKey<Mod>(),
			new CCKey<string>(),
			new CCOptionalKey<GameCulture.CultureName?>(() => GameCulture.CultureName.English),
		})
		{
			array = new string[]
			{
				"0",
				"addmodurl"
			};
		}

		public static void Call(Mod mod, string domain, GameCulture.CultureName culture = GameCulture.CultureName.English) => Wikithis.AddModURL(mod, domain, culture);

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}
