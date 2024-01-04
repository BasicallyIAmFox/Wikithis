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

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Wikithis.Data;

namespace Wikithis.Wikis;

public sealed class ItemWiki : AbstractWiki<short, WikiEntry<short>> {
	private static void AddReplacements() {
		var replacements = Wikithis.ItemUrlReplacements as IDictionary<(short, GameCulture.CultureName), string>;
		Debug.Assert(replacements != null);

		AddBrainScramblerReplacements(replacements);
	}

	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	private static void AddBrainScramblerReplacements(IDictionary<(short, GameCulture.CultureName), string> replacements) {
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Brain_Scrambler_(item)");
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.German), "https://terraria.wiki.gg/de/wiki/Gehirnverwirrer_(Gegenstand)");
		// Italian wiki doesn't has a wiki page for Brain Scrambler.
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Embrouilleur_(objet)");
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Spanish), "https://terraria.wiki.gg/es/wiki/Destrozacerebros_(objeto)");
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Запутыватель_мозгов_(предмет)");
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E6%89%B0%E8%84%91%E5%99%A8");
		// Portuguese wiki doesn't has a wiki page for Brain Scrambler.
		// Polish wiki doesn't has a wiki page for Brain Scrambler.
	}

	protected override void Initialize() {
		AddReplacements();

		LoaderUtils.ForEachAndAggregateExceptions(ContentSamples.ItemsByType.Values
			.Where(x => !ItemID.Sets.Deprecated[x.type] && x.ModItem?.Mod.Name != "ModLoader" && x.type != ItemID.None), item => {
				string key = item.ModItem is null
					? $"ItemName.{ItemID.Search.GetName(item.type)}"
					: item.ModItem.DisplayName.Key;

				string name = LanguageManager.GetTextValue(key);

				string url = null;
				if (Wikithis.ItemUrlReplacements.TryGetValue(((short)item.type, Wikithis.CurrentCulture), out string urlReplacement))
					url = urlReplacement;
				else if (Wikithis.ItemUrlReplacements.TryGetValue(((short)item.type, GameCulture.CultureName.English), out urlReplacement))
					url = urlReplacement;

				url ??= DefaultSearchStr(name, item.ModItem?.Mod);
				AddEntry((short)item.type, new WikiEntry<short>((short)item.type, url));
			});
	}
}
