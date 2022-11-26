using CCLiar;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis.Calls {
	public sealed class ReplaceNpcIdsCall : CCList, ILoadable {
		private static string[] array;

		public ReplaceNpcIdsCall() : base(x => Array.IndexOf(array, x) != -1, args => {
			object npcId = args.Get<object>(0) is int ? args.Get<int>(0) : args.Get<List<int>>(0, _ => _ == null);
			string newName = args.Get<string>(1, _ => string.IsNullOrWhiteSpace(_));
			GameCulture.CultureName culture = args.Get<GameCulture.CultureName>(2);

			if (npcId is int _int) {
				Call(_int, newName, culture);
				return Wikithis.GotoSuccessReturn();
			}
			else if (npcId is List<int> lists) {
				Call(lists, newName, culture);
				return Wikithis.GotoSuccessReturn();
			}
			throw new NotImplementedException("Unexpected behavior happened!");
		}, new ICCKey[]
		{
			new CCOrKey<int, List<int>>(),
			new CCKey<string>(),
			new CCOptionalKey<GameCulture.CultureName?>(() => GameCulture.CultureName.English),
		}) {
			array = new string[]
			{
				"2",
				"npcidreplacement",
				"npcidreplacements",
				"npcidsreplacement",
				"npcidsreplacements",
				"replacenpcid",
				"replacenpcids"
			};
		}

		public static void Call(int npcId, string name, GameCulture.CultureName culture = GameCulture.CultureName.English) => Wikithis.ReplaceNpcIds(name, culture, npcId);

		public static void Call(int[] npcId, string name, GameCulture.CultureName culture = GameCulture.CultureName.English) => Wikithis.ReplaceNpcIds(name, culture, npcId);

		public static void Call(List<int> npcId, string name, GameCulture.CultureName culture = GameCulture.CultureName.English) => Wikithis.ReplaceNpcIds(name, culture, npcId.ToArray());

		public static void Call(IEnumerable<int> npcId, string name, GameCulture.CultureName culture = GameCulture.CultureName.English) => Wikithis.ReplaceNpcIds(name, culture, npcId.ToArray());

		public void Unload() => array = null;

		public void Load(Mod mod) {
		}
	}
}
