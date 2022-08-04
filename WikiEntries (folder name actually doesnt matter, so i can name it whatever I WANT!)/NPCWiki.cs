using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Wikithis
{
	public class NPCWiki : Wiki<NPC, int>
	{
		public NPCWiki() : base(new Func<NPC, int>((x) => x.type))
		{
		}

		public override void Initialize()
		{
			foreach (NPC npc in ContentSamples.NpcsByNetId.Values)
			{
				if (HasEntry(npc.type))
					continue;

				string name = npc.netID < NPCID.Count
					? Language.GetTextValue($"NPCName.{Wikithis.GetInternalName(npc.type, 1)}")
					: Language.GetTextValue($"Mods.{npc.ModNPC.Mod.Name}.NPCName.{npc.ModNPC.Name}");

				if (Wikithis.NpcIdNameReplace.TryGetValue((npc.netID, Wikithis.CultureLoaded), out string name2))
					name = name2;

				AddEntry(npc, new(npc.netID, Wikithis.DefaultSearchStr(name, npc.ModNPC?.Mod)));
			}
		}
	}
}
