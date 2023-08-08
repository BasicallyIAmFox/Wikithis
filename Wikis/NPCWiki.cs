using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Core;

namespace Wikithis.Wikis;

public sealed class NPCWiki : AbstractWiki<short> {
	public sealed override void Initialize() {
		LoaderUtils.ForEachAndAggregateExceptions(ContentSamples.NpcsByNetId.Values
			.Where(x => !Entries.ContainsKey((short)x.netID) && x.netID != ItemID.None),
			npc => {
#if TML_2022_09
				string name = npc.ModNPC != null
					? Language.GetTextValue($"Mods.{npc.ModNPC.Mod.Name}.NPCName.{npc.ModNPC.Name}")
					: Lang.GetNPCNameValue(npc.netID);

				if (Wikithis.npcReplacements.TryGetValue(((short)npc.netID, Wikithis.CultureLoaded), out string nameReplacement)) {
					name = nameReplacement;
				}
				else if (Wikithis.npcReplacements.TryGetValue(((short)npc.netID, GameCulture.CultureName.English), out nameReplacement)) {
					name = nameReplacement;
				}

				string url = DefaultSearchStr(name, npc.ModNPC?.Mod);
#else
				string name = npc.ModNPC?.DisplayName?.Value ?? Lang.GetNPCNameValue(npc.netID);

				string url = null;
				if (Wikithis.npcReplacements.TryGetValue(((short)npc.netID, Wikithis.CurrentCulture), out string urlReplacement)) {
					url = urlReplacement;
				}
				else if (Wikithis.npcReplacements.TryGetValue(((short)npc.netID, GameCulture.CultureName.English), out urlReplacement)) {
					url = urlReplacement;
				}

				url ??= DefaultSearchStr(name, npc.ModNPC?.Mod);
#endif
				if (url != null) {
					AddEntry((short)npc.netID, new WikiEntry<short>((short)npc.netID, url));
				}
			});
	}
}
