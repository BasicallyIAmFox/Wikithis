using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Core;

namespace Wikithis.Wikis;

public sealed class ItemWiki : AbstractWiki<short> {
	public sealed override void Initialize() {
		LoaderUtils.ForEachAndAggregateExceptions(ContentSamples.ItemsByType.Values
			.Where(x => !Entries.ContainsKey((short)x.type) && !ItemID.Sets.Deprecated[x.type] && x.ModItem?.Mod.Name != "ModLoader" && x.type != ItemID.None),
			item => {
#if TML_2022_09
				string name = item.ModItem != null
					? Language.GetTextValue($"Mods.{item.ModItem.Mod.Name}.ItemName.{item.ModItem.Name}")
					: Lang.GetItemNameValue(item.type);
#else
				string name = item.ModItem?.DisplayName?.Value ?? Lang.GetItemNameValue(item.type);
#endif
				if (Wikithis.itemReplacements.TryGetValue(((short)item.type, Wikithis.CultureLoaded), out string nameReplacement)) {
					name = nameReplacement;
				}
				else if (Wikithis.itemReplacements.TryGetValue(((short)item.type, GameCulture.CultureName.English), out nameReplacement)) {
					name = nameReplacement;
				}

				string url = DefaultSearchStr(name, item.ModItem?.Mod);
				if (url != null) {
					AddEntry((short)item.type, new WikiEntry<short>((short)item.type, url));
				}
			});
	}
}
