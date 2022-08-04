using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Wikithis
{
	public class ItemWiki : Wiki<Item, int>
	{
		public ItemWiki() : base(new Func<Item, int>((x) => x.type))
		{
		}

		public override void Initialize()
		{
			foreach (Item item in ContentSamples.ItemsByType.Values)
			{
				if (HasEntry(item.type) || ItemID.Sets.Deprecated[item.type] || item.ModItem?.Mod.Name == "ModLoader")
					continue;

				string name = item.type < ItemID.Count
					? Language.GetTextValue($"ItemName.{Wikithis.GetInternalName(item.type)}")
					: Language.GetTextValue($"Mods.{item.ModItem.Mod.Name}.ItemName.{item.ModItem.Name}");

				if (Wikithis.ItemIdNameReplace.TryGetValue((item.type, Wikithis.CultureLoaded), out string name2))
					name = name2;

				AddEntry(item, new(item.type, Wikithis.DefaultSearchStr(name, item.ModItem?.Mod)));
			}
		}
	}
}
