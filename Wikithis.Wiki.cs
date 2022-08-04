using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis
	{
		private static string GetInternalName(int id, int num = 0)
		{
			if (num == 0) return ItemID.Search.GetName(id);
			else if (num == 1) return NPCID.Search.GetName(id);
			else if (num == 2) return TileID.Search.GetName(id);
			throw new NotImplementedException();
		}

		internal static void SetupWikiPages()
		{
			AddEntries(ContentSamples.ItemsByType.Values, x => x.type.ToString(), x =>
			{
				string name = x.type < ItemID.Count ? Language.GetTextValue("ItemName." + GetInternalName(x.type)) : Language.GetTextValue($"Mods.{x.ModItem.Mod.Name}.ItemName.{x.ModItem.Name}");
				if (ItemIdNameReplace.TryGetValue((x.type, CultureLoaded), out string name2))
					name = name2;

				return DefaultSearchStr(name, x.ModItem?.Mod);
			}, x => ItemID.Sets.Deprecated[x.type] || x.ModItem?.Mod.Name == "ModLoader",
			(x => x.type, ItemIdNameReplace));

			AddEntries(ContentSamples.NpcsByNetId.Values, x => x.type.ToString(), x =>
			{
				string name = x.netID < NPCID.Count ? Language.GetTextValue("NPCName." + GetInternalName(x.netID, 1)) : Language.GetTextValue($"Mods.{x.ModNPC.Mod.Name}.NPCName.{x.ModNPC.Name}");
				if (ItemIdNameReplace.TryGetValue((x.type, CultureLoaded), out string name2))
					name = name2;

				return DefaultSearchStr(name, x.ModNPC?.Mod);
			}, x => x.ModNPC?.Mod.Name == "ModLoader",
			(x => x.type, NpcIdNameReplace));
		}

		internal static void OpenWikiPage(Mod mod, Item item)
		{
			if (WikiEntries[typeof(Item)].ContainsKey(item.type.ToString()) && CheckURLValid(WikiEntries[typeof(Item)][item.type.ToString()].Search))
			{
				WikiEntries[typeof(Item)][item.type.ToString()].OpenWikiPage(false);
			}
			else
			{
				Main.NewText(Language.GetTextValue($"Mods.{mod.Name}.Error"), Color.OrangeRed);

				bool bl = ModToURL.ContainsKey((item.ModItem?.Mod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey((item.ModItem?.Mod, GameCulture.CultureName.English));

				mod.Logger.Error("Tried to get wiki page, but failed!");
				mod.Logger.Info("Type: " + item.type.ToString());
				mod.Logger.Info("Name: " + item.Name);
				mod.Logger.Info("Vanilla: " + (item.ModItem == null).ToString());
				mod.Logger.Info("Mod: " + item.ModItem?.Mod.Name);
				mod.Logger.Info("Domain in dictionary: " + (item.ModItem != null ? bl.ToString() : "False"));
			}
		}

		internal static void OpenWikiPage(Mod mod, NPC npc)
		{
			if (WikiEntries[typeof(NPC)].ContainsKey(npc.netID.ToString()) && CheckURLValid(WikiEntries[typeof(NPC)][npc.netID.ToString()].Search))
			{
				WikiEntries[typeof(NPC)][npc.netID.ToString()].OpenWikiPage(false);
			}
			else
			{
				Main.NewText(Language.GetTextValue($"Mods.{mod.Name}.Error"), Color.OrangeRed);

				bool bl = ModToURL.ContainsKey((npc.ModNPC?.Mod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey((npc.ModNPC?.Mod, GameCulture.CultureName.English));

				mod.Logger.Error("Tried to get wiki page, but failed!");
				mod.Logger.Info("Type: " + npc.type.ToString());
				mod.Logger.Info("Name: " + npc.GivenOrTypeName);
				mod.Logger.Info("Vanilla: " + (npc.ModNPC == null).ToString());
				mod.Logger.Info("Mod: " + npc.ModNPC?.Mod.Name);
				mod.Logger.Info("Domain in dictionary: " + (npc.ModNPC != null ? bl.ToString() : "False"));
			}
		}
	}
}