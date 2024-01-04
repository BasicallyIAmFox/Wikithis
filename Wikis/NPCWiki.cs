//
//    Copyright 2023-2024 BasicallyIAmFox
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
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Wikithis.Data;

namespace Wikithis.Wikis;

// ReSharper disable once InconsistentNaming
public sealed class NPCWiki : AbstractWiki<short, WikiEntry<short>> {
	protected override void Initialize() {
		LoaderUtils.ForEachAndAggregateExceptions(ContentSamples.NpcsByNetId.Values
			.Where(x => x.netID != NPCID.None),
			npc => {
				string key = npc.ModNPC is null
					? $"ItemName.{NPCID.Search.GetName(npc.type)}"
					: npc.ModNPC.DisplayName.Key;

				string name = LanguageManager.GetTextValue(key);

				string url = null;
				if (Wikithis.NpcUrlReplacements.TryGetValue(((short)npc.netID, Wikithis.CurrentCulture), out string urlReplacement))
					url = urlReplacement;
				else if (Wikithis.NpcUrlReplacements.TryGetValue(((short)npc.netID, GameCulture.CultureName.English), out urlReplacement))
					url = urlReplacement;

				url ??= DefaultSearchStr(name, npc.ModNPC?.Mod);
				AddEntry((short)npc.netID, new WikiEntry<short>((short)npc.netID, url));
			});
	}
}
