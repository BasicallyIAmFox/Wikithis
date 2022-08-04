using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	internal class WikithisCommand : ModCommand
	{
		public override string Command => "wikithis";

		public override string Usage => $"/wikithis <npc|item> [name of type]\n{Language.GetTextValue($"Mods.{Mod.Name}.WikithisInput")}";

		public override string Description => Language.GetTextValue($"Mods.{Mod.Name}.WikithisDesc");

		public override CommandType Type => CommandType.Chat;

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (caller.Player.whoAmI == Main.myPlayer && args.Length == 0)
			{
				foreach (string s in Usage.Split('\n'))
				{
					Main.NewText(s, Color.OrangeRed);
				}
				return;
			}

			Array.Resize(ref args, 2);
			if (caller.Player.whoAmI == Main.myPlayer && args[0] is string type)
			{
				type = type.ToLower();
				if (type == "npc" && args[1] is string name)
				{
					if (!int.TryParse(args[1], out int npcType))
					{
						string getName = args[1].Replace("_", " ");
						NPC npc = new();
						for (int k = NPCID.NegativeIDCount + 1; k < NPCLoader.NPCCount; k++)
						{
							npc.SetDefaults(k);
							if (getName != npc.GivenOrTypeName)
								continue;

							npcType = k;
							break;
						}

						if (npcType == 0)
						{
							Main.NewText($"{input} <-- Unknown NPC type!", Color.OrangeRed);
							throw new UsageException($"Unknown NPC: {name}");
						}
					}

					if (npcType <= 0 || npcType >= NPCLoader.NPCCount)
					{
						Main.NewText($"{input} <-- Unknown NPC ID!", Color.OrangeRed);
						throw new UsageException($"Unknown NPC ID: {npcType}");
					}
					Wikithis.OpenWikiPage(Mod, ContentSamples.NpcsByNetId[npcType]);
				}
				else if (type == "item" && args[1] is string name2)
				{
					name = name2;
					if (!int.TryParse(args[1], out int itemType))
					{
						string getName = args[1].Replace("_", " ");
						Item item = new();
						for (int k = 0; k < ItemLoader.ItemCount; k++)
						{
							item.SetDefaults(k, true);
							if (getName != item.Name)
								continue;

							itemType = k;
							break;
						}

						if (itemType == 0)
						{
							Main.NewText($"{input} <-- Unknown item type!", Color.OrangeRed);
							throw new UsageException($"Unknown item: {name}");
						}
					}

					if (itemType <= 0 || itemType >= ItemLoader.ItemCount)
					{
						Main.NewText($"{input} <-- Unknown item ID!", Color.OrangeRed);
						throw new UsageException($"Unknown item ID: {itemType}");
					}
					Wikithis.OpenWikiPage(Mod, ContentSamples.ItemsByType[itemType]);
				}
				else if (type == "npc" && args[1] is null)
				{
					Main.NewText($"{input} <-- Unknown NPC name!", Color.OrangeRed);
				}
				else if (type == "item" && args[1] is null)
				{
					Main.NewText($"{input} <-- Unknown item name!", Color.OrangeRed);
				}
				else
				{
					Main.NewText($"{input} <-- Unknown type! Available types: [c/{Color.Yellow.Hex3()}:npc], [c/{Color.Yellow.Hex3()}:item].", Color.OrangeRed);
					throw new UsageException("Unknown type: " + type);
				}
			}
		}
	}

	// crossplatform warning
	/*internal class WikithisPlayer : ModPlayer
	{
		public override void OnEnterWorld(Player player)
		{
			if (!Main.dedServ && !Platform.IsWindows)
				Main.NewText(Language.GetTextValue("Mods.Wikithis.UnsupportedWarning"), Color.OrangeRed);
		}
	}*/
}