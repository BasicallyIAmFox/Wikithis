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

		public override string Usage => $"/wikithis <npc|item> [name of type]\n{Language.GetTextValue($"Mods.{nameof(Wikithis)}.WikithisInput")}";

		public override string Description => Language.GetTextValue($"Mods.{nameof(Wikithis)}.WikithisDesc");

		public override CommandType Type => CommandType.Chat;

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (caller.Player.whoAmI != Main.myPlayer)
				return;

			if (args.Length == 0)
			{
				foreach (string s in Usage.Split('\n'))
				{
					caller.Reply(s, Color.OrangeRed);
				}
				return;
			}

			Array.Resize(ref args, 2);
			if (args[0] is string type)
			{
				type = type.ToLowerInvariant();

				if (type == "npc" && args[1] is string name)
				{
					if (!int.TryParse(args[1], out int npcType))
					{
						string getName = name.Replace('_', ' ');

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
							caller.Reply($"{input} <-- Unknown NPC type!", Color.OrangeRed);
							return;
						}
					}

					if (npcType == NPCID.None || npcType <= NPCID.NegativeIDCount || npcType >= NPCLoader.NPCCount)
					{
						caller.Reply($"{input} <-- Unknown NPC ID!", Color.OrangeRed);
						return;
					}
					Wikithis.OpenWikiPage(ContentSamples.NpcsByNetId[npcType], false);
				}
				else if (type == "item" && args[1] is string name2)
				{
					if (!int.TryParse(args[1], out int itemType))
					{
						string getName = name2.Replace('_', ' ');

						Item item = new();
						for (int k = 0; k < ItemLoader.ItemCount; k++)
						{
							item.SetDefaults(k, true);
							if (getName != item.Name)
								continue;

							itemType = k;
							break;
						}

						if (itemType == ItemID.None)
						{
							caller.Reply($"{input} <-- Unknown item type!", Color.OrangeRed);
							return;
						}
					}

					if (itemType <= ItemID.None || itemType >= ItemLoader.ItemCount)
					{
						caller.Reply($"{input} <-- Unknown item ID!", Color.OrangeRed);
						return;
					}
					Wikithis.OpenWikiPage(ContentSamples.ItemsByType[itemType], false);
				}
				else if (type == "npc" && args[1] is null)
				{
					caller.Reply($"{input} <-- Unknown NPC name!", Color.OrangeRed);
				}
				else if (type == "item" && args[1] is null)
				{
					caller.Reply($"{input} <-- Unknown item name!", Color.OrangeRed);
				}
				else
				{
					caller.Reply($"{input} <-- Unknown type! Available types: [c/{Color.Yellow.Hex3()}:npc], [c/{Color.Yellow.Hex3()}:item].", Color.OrangeRed);
				}
			}
		}
	}
}