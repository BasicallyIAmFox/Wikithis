//
//    Copyright 2023-2025 BasicallyIAmFox
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
		AddClothiersPrincessDressReplacements(replacements);
		AddGuideToCritterCompanionshipReplacements(replacements);
		AddGuideToEnvironmentalPreservationReplacements(replacements);
		AddGuideToPeacefulCoexistenceReplacements(replacements);
	}

	private static void AddBrainScramblerReplacements(IDictionary<(short, GameCulture.CultureName), string> replacements) {
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Brain_Scrambler_(item)");
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.German), "https://terraria.wiki.gg/de/wiki/Gehirnverwirrer_(Gegenstand)");
		// Italian
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Embrouilleur_(objet)");
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Spanish), "https://terraria.wiki.gg/es/wiki/Destrozacerebros_(objeto)");
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Запутыватель_мозгов_(предмет)");
		replacements.TryAdd((ItemID.BrainScrambler, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E6%89%B0%E8%84%91%E5%99%A8");
		// Portuguese
		// Polish
	}

	private static void AddClothiersPrincessDressReplacements(IDictionary<(short, GameCulture.CultureName), string> replacements) {
		replacements.TryAdd((ItemID.PrincessDress, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Princess_Dress_(Clothier)");
		// Deutsch
		// Italian
		replacements.TryAdd((ItemID.PrincessDress, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Robe_de_princesse");
		// Spanish
		replacements.TryAdd((ItemID.PrincessDress, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Платье_принцессы_(от_портного)");
		replacements.TryAdd((ItemID.PrincessDress, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E5%85%AC%E4%B8%BB%E8%A3%99%EF%BC%88%E6%9C%8D%E8%A3%85%E5%95%86%EF%BC%89");
		// Portuguese
		// Polish
	}

	private static void AddGuideToCritterCompanionshipReplacements(IDictionary<(short, GameCulture.CultureName), string> replacements) {
		replacements.TryAdd((ItemID.DontHurtCrittersBook, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Guide_to_Critter_Companionship");
		// Deutsch
		// Italian
		replacements.TryAdd((ItemID.DontHurtCrittersBook, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Guide_des_cr%C3%A9atures_de_compagnie");
		// Spanish
		replacements.TryAdd((ItemID.DontHurtCrittersBook, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Пособие_по_обращению_с_животными");
		replacements.TryAdd((ItemID.DontHurtCrittersBook, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E5%B0%8F%E5%8A%A8%E7%89%A9%E5%8F%8B%E8%B0%8A%E6%8C%87%E5%8D%97");
		replacements.TryAdd((ItemID.DontHurtCrittersBook, GameCulture.CultureName.Portuguese), "https://terraria.wiki.gg/ru/wiki/Guia_de_Acompanhamento_dos_Animais");
		// Polish

		replacements.TryAdd((ItemID.DontHurtCrittersBookInactive, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Guide_to_Critter_Companionship_(Inactive)");
		// Deutsch
		// Italian
		replacements.TryAdd((ItemID.DontHurtCrittersBookInactive, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Guide_des_cr%C3%A9atures_de_compagnie_(Inactif)");
		// Spanish
		replacements.TryAdd((ItemID.DontHurtCrittersBookInactive, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Пособие_по_обращению_с_животными");
		replacements.TryAdd((ItemID.DontHurtCrittersBookInactive, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E5%B0%8F%E5%8A%A8%E7%89%A9%E5%8F%8B%E8%B0%8A%E6%8C%87%E5%8D%97%EF%BC%88%E5%81%9C%E7%94%A8%EF%BC%89");
		replacements.TryAdd((ItemID.DontHurtCrittersBookInactive, GameCulture.CultureName.Portuguese), "https://terraria.wiki.gg/ru/wiki/Guia_de_Acompanhamento_dos_Animais");
		// Polish
	}

	private static void AddGuideToEnvironmentalPreservationReplacements(IDictionary<(short, GameCulture.CultureName), string> replacements) {
		replacements.TryAdd((ItemID.DontHurtNatureBook, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Guide_to_Environmental_Preservation");
		// Deutsch
		// Italian
		replacements.TryAdd((ItemID.DontHurtNatureBook, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Guide_de_pr%C3%A9servation_environnementale");
		// Spanish
		replacements.TryAdd((ItemID.DontHurtNatureBook, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Пособие_по_охране_окружающей_среды");
		replacements.TryAdd((ItemID.DontHurtNatureBook, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E7%8E%AF%E5%A2%83%E4%BF%9D%E6%8A%A4%E6%8C%87%E5%8D%97");
		// Portuguese
		// Polish

		replacements.TryAdd((ItemID.DontHurtNatureBookInactive, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Guide_to_Environmental_Preservation_(Inactive)");
		// Deutsch
		// Italian
		replacements.TryAdd((ItemID.DontHurtNatureBookInactive, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Guide_de_pr%C3%A9servation_environnementale_(Inactif)");
		// Spanish
		replacements.TryAdd((ItemID.DontHurtNatureBookInactive, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Пособие_по_охране_окружающей_среды");
		replacements.TryAdd((ItemID.DontHurtNatureBookInactive, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E7%8E%AF%E5%A2%83%E4%BF%9D%E6%8A%A4%E6%8C%87%E5%8D%97%EF%BC%88%E5%81%9C%E7%94%A8%EF%BC%89");
		// Portuguese
		// Polish
	}

	private static void AddGuideToPeacefulCoexistenceReplacements(IDictionary<(short, GameCulture.CultureName), string> replacements) {
		replacements.TryAdd((ItemID.DontHurtComboBook, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Guide_to_Peaceful_Coexistence");
		// Deutsch
		// Italian
		replacements.TryAdd((ItemID.DontHurtComboBook, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Guide_de_coexistence_paisible");
		// Spanish
		replacements.TryAdd((ItemID.DontHurtComboBook, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Пособие_по_мирному_сосуществованию");
		replacements.TryAdd((ItemID.DontHurtComboBook, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E5%92%8C%E5%B9%B3%E5%85%B1%E5%A4%84%E6%8C%87%E5%8D%97");
		// Portuguese
		// Polish

		replacements.TryAdd((ItemID.DontHurtComboBookInactive, GameCulture.CultureName.English), "https://terraria.wiki.gg/wiki/Guide_to_Peaceful_Coexistence_(Inactive)");
		// Deutsch
		// Italian
		replacements.TryAdd((ItemID.DontHurtComboBookInactive, GameCulture.CultureName.French), "https://terraria.wiki.gg/fr/wiki/Guide_de_coexistence_paisible");
		// Spanish
		replacements.TryAdd((ItemID.DontHurtComboBookInactive, GameCulture.CultureName.Russian), "https://terraria.wiki.gg/ru/wiki/Пособие_по_мирному_сосуществованию");
		replacements.TryAdd((ItemID.DontHurtComboBookInactive, GameCulture.CultureName.Chinese), "https://terraria.wiki.gg/zh/wiki/%E5%92%8C%E5%B9%B3%E5%85%B1%E5%A4%84%E6%8C%87%E5%8D%97%EF%BC%88%E5%81%9C%E7%94%A8%EF%BC%89");
		// Portuguese
		// Polish
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
