using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Wikithis.Wikis;

public sealed class NPCWiki : AbstractWiki<short> {
	public override void Initialize() {
		foreach (var npc in ContentSamples.NpcsByNetId.Values
			.Where(x => !Entries.ContainsKey((short)x.netID) && x.netID != ItemID.None)) {
			try {
				string name = npc.ModNPC?.DisplayName?.Value ?? Lang.GetNPCNameValue(npc.type);
				if (Wikithis.npcReplacements.TryGetValue(((short)npc.netID, Wikithis.CultureLoaded), out string nameReplacement)) {
					name = nameReplacement;
				}
				else if (Wikithis.npcReplacements.TryGetValue(((short)npc.netID, GameCulture.CultureName.English), out nameReplacement)) {
					name = nameReplacement;
				}

				AddEntry((short)npc.netID, new WikiEntry<short>((short)npc.netID, DefaultSearchStr(name, npc.ModNPC?.Mod)));
			}
			catch {
			}
		}
	}
}
