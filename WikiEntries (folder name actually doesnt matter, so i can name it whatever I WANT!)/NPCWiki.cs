﻿using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Wikithis
{
	public class NPCWiki : Wiki<NPC, int>
	{
		public NPCWiki() : base(new Func<NPC, int>((x) => x.netID))
		{
		}

		public override void Initialize()
		{
			foreach (NPC npc in ContentSamples.NpcsByNetId.Values)
			{
				if (HasEntry(npc.type) || npc.netID <= NPCID.None)
					continue;

				string name = npc.netID < NPCID.Count
					? Language.GetTextValue($"NPCName.{Wikithis.GetInternalName(npc.netID, 1)}")
					: Language.GetTextValue($"Mods.{npc.ModNPC.Mod.Name}.NPCName.{npc.ModNPC.Name}");

				if (Wikithis.NpcIdNameReplace.TryGetValue((npc.netID, Wikithis.CultureLoaded), out string name2))
					name = name2;

				AddEntry(npc, new WikiEntry<int>(npc.netID, Wikithis.DefaultSearchStr(name, npc.ModNPC?.Mod)));
			}
		}

		public override void MessageIfDoesntExists(int key)
		{
			NPC npc = ContentSamples.NpcsByNetId[key];

			bool bl = Wikithis.ModToURL.ContainsKey((npc.ModNPC?.Mod, Wikithis.CultureLoaded));
			if (!bl)
				bl = Wikithis.ModToURL.ContainsKey((npc.ModNPC?.Mod, GameCulture.CultureName.English));

			Wikithis.Instance.Logger.Info("Key: " + key.ToString());
			Wikithis.Instance.Logger.Info("Type: " + npc.type.ToString());
			Wikithis.Instance.Logger.Info("Net ID: " + npc.netID.ToString());
			Wikithis.Instance.Logger.Info("Name: " + npc.GivenOrTypeName);
			Wikithis.Instance.Logger.Info("Vanilla: " + (npc.ModNPC == null).ToString());
			Wikithis.Instance.Logger.Info("Mod: " + npc.ModNPC?.Mod.Name);
			Wikithis.Instance.Logger.Info("Domain in dictionary: " + (npc.ModNPC != null ? bl.ToString() : "False"));
		}
	}
}
