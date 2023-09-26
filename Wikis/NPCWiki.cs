//
//    Copyright 2023 BasicallyIAmFox
//
//    Licensed under the Apache License, Version 2.0 (the "License")
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Core;

namespace Wikithis.Wikis;

public sealed class NPCWiki : AbstractWiki<short> {
	public sealed override void Initialize() {
		LoaderUtils.ForEachAndAggregateExceptions(ContentSamples.NpcsByNetId.Values
			.Where(x => !Entries.ContainsKey((short)x.netID) && x.netID != NPCID.None),
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
