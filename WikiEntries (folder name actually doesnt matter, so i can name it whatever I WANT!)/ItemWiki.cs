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
				if (HasEntry(item.type) || ItemID.Sets.Deprecated[item.type] || item.ModItem?.Mod.Name == "ModLoader" || item.type == ItemID.None)
					continue;

				string name = item.type < ItemID.Count
					? Language.GetTextValue($"ItemName.{Wikithis.GetInternalName(item.type)}")
					: Language.GetTextValue($"Mods.{item.ModItem.Mod.Name}.ItemName.{item.ModItem.Name}");

				if (Wikithis.ItemIdNameReplace.TryGetValue((item.type, Wikithis.CultureLoaded), out string name2))
					name = name2;

				AddEntry(item, new(item.type, Wikithis.DefaultSearchStr(name, item.ModItem?.Mod)));
			}
		}

		public override void MessageIfDoesntExists(int key)
		{
			Item item = ContentSamples.ItemsByType[key];

			bool bl = Wikithis.ModToURL.ContainsKey((item.ModItem?.Mod, Wikithis.CultureLoaded));
			if (!bl)
				bl = Wikithis.ModToURL.ContainsKey((item.ModItem?.Mod, GameCulture.CultureName.English));

			Wikithis.Instance.Logger.Info("Key: " + key.ToString());
			Wikithis.Instance.Logger.Info("Type: " + item.type.ToString());
			Wikithis.Instance.Logger.Info("Name: " + item.Name);
			Wikithis.Instance.Logger.Info("Vanilla: " + (item.ModItem == null).ToString());
			Wikithis.Instance.Logger.Info("Mod: " + item.ModItem?.Mod.Name);
			Wikithis.Instance.Logger.Info("Domain in dictionary: " + (item.ModItem != null ? bl.ToString() : "False"));
		}
	}
}
