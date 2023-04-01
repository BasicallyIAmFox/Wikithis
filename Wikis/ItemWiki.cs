using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Wikithis.Wikis;

public sealed class ItemWiki : AbstractWiki<short> {
	public override void Initialize() {
		foreach (var item in ContentSamples.ItemsByType.Values
			.Where(x => !Entries.ContainsKey((short)x.type) && !ItemID.Sets.Deprecated[x.type] && x.ModItem?.Mod.Name != "ModLoader" && x.type != ItemID.None)) {
			try {
				string name = item.ModItem?.DisplayName?.Value ?? Lang.GetItemNameValue(item.type);
				if (Wikithis.itemReplacements.TryGetValue(((short)item.type, Wikithis.CultureLoaded), out string nameReplacement)) {
					name = nameReplacement;
				}
				else if (Wikithis.itemReplacements.TryGetValue(((short)item.type, GameCulture.CultureName.English), out nameReplacement)) {
					name = nameReplacement;
				}

				AddEntry((short)item.type, new WikiEntry<short>((short)item.type, DefaultSearchStr(name, item.ModItem?.Mod)));
			}
			catch {
			}
		}
	}
}
