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

public sealed class ItemWiki : AbstractWiki<short> {
	private static void AddBrainScramblerReplacements() {
		Wikithis.itemReplacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Brain_Scrambler_(item)");
		Wikithis.itemReplacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.German), "https://terraria.wiki.gg/de/wiki/Gehirnverwirrer_(Gegenstand)");
		// Italian wiki doesn't has a wiki page for Brain Scrambler.
		Wikithis.itemReplacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Embrouilleur_(objet)");
		Wikithis.itemReplacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Spanish), "https://terraria.wiki.gg/es/wiki/Destrozacerebros_(objeto)");
		Wikithis.itemReplacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Запутыватель_мозгов_(предмет)");
		Wikithis.itemReplacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E6%89%B0%E8%84%91%E5%99%A8");
		// Portuguese wiki doesn't has a wiki page for Brain Scrambler.
		// Polish wiki doesn't has a wiki page for Brain Scrambler.
	}

	public sealed override void Initialize() {
		AddBrainScramblerReplacements();

		LoaderUtils.ForEachAndAggregateExceptions(ContentSamples.ItemsByType.Values
			.Where(x => !Entries.ContainsKey((short)x.type) && !ItemID.Sets.Deprecated[x.type] && x.ModItem?.Mod.Name != "ModLoader" && x.type != ItemID.None), item => {
				string name = /*item.ModItem?.DisplayName?.Value ?? */Lang.GetItemNameValue(item.type);

				string url = null;
				if (Wikithis.itemReplacements.TryGetValue(((short)item.type, Wikithis.CurrentCulture), out string urlReplacement)) {
					url = urlReplacement;
				}
				else if (Wikithis.itemReplacements.TryGetValue(((short)item.type, GameCulture.CultureName.English), out urlReplacement)) {
					url = urlReplacement;
				}

				url ??= DefaultSearchStr(name, item.ModItem?.Mod);
				AddEntry((short)item.type, new WikiEntry<short>((short)item.type, url));
			});
	}
}
