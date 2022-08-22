using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	internal class WikithisCommand : ModCommand
	{
		public override bool IsLoadingEnabled(Mod mod) => false; // needs LOTS of work...

		public override string Command => "wikithis";

		public override string Usage => $"/wikithis <{string.Join("|", Wikithis._commandAvailableTypes.Keys)}> [key of type]\n{Language.GetTextValue($"Mods.{Mod.Name}.WikithisInput")}";

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

				Dictionary<string, IWiki<object, IConvertible>> availableTypes = Wikithis._commandAvailableTypes;

				if (availableTypes.ContainsKey(type))
				{
					if (args[1] is IConvertible name)
					{
						var typeWiki = availableTypes[type];
						if (!typeWiki.HasEntry(name))
						{
							caller.Reply($"{input} <-- Unknown key!", Color.OrangeRed);
							return;
						}

						typeWiki.GetEntry(name).OpenWikiPage(false);
					}
					else
					{
						caller.Reply($"{input} <-- Unknown key!", Color.OrangeRed);
					}
				}
				else
				{
					string joiner = string.Join($"], [c/{Color.Yellow.Hex3()}:", availableTypes.Keys);
					caller.Reply($"{input} <-- Unknown type! Available types: {joiner}.]", Color.OrangeRed);
				}
			}
		}
	}
}