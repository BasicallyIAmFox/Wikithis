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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Wikithis.Wikis;

namespace Wikithis;

// ReSharper disable once UnusedType.Local
public sealed class WikithisItem : GlobalItem {
	private const string TooltipName = $"{nameof(Wikithis)}:Wiki"; // Do not change.

	public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset) {
		bool isAvailable = Wikithis.GetWiki<ItemWiki>().Entries.TryGetValue((short)item.netID, out var wikiEntry) && wikiEntry.IsValid();

		if (DrawIcon(item, line)) {
			return false;
		}

		if (WikithisSystem.WikiKeybind.JustPressed && line is { Mod: "Terraria", Name: "ItemName" } && isAvailable) {
			wikiEntry.OpenWikiPage(false);
		}

		return true;
	}

	private bool DrawIcon(Item item, DrawableTooltipLine line) {
		if (line.Mod != Mod.Name || line.Name is not TooltipName)
			return false;

		var texture = TextureAssets.BestiaryMenuButton;
		var position = new Vector2(line.X, line.Y);
		var sourceRect = new Rectangle(0, 0, 30, texture.Height());
		var origin = new Vector2(0f, 0f);
		var scale = new Vector2(2f / 3f);

		if (!Wikithis.GetWiki<ItemWiki>().HasValidEntry((short)item.netID)) {
			Main.instance.LoadItem(ItemID.WireCutter);
			texture = TextureAssets.Item[ItemID.WireCutter];
			sourceRect.Width = texture.Width();
		}
		else if (item.ModItem?.Mod != null && Wikithis.ModData.GetOrCreateValue(item.ModItem.Mod)!.PersonalAsset != null) {
			texture = Wikithis.ModData.GetOrCreateValue(item.ModItem.Mod)!.PersonalAsset;
			sourceRect.Width = texture.Width();
		}

		if (texture != TextureAssets.BestiaryMenuButton) {
			origin.X = -((30f - texture.Width()) / 2f);
			origin.Y = -((TextureAssets.BestiaryMenuButton.Height() - texture.Height()) / 2f);
		}

		Main.spriteBatch.Draw(texture.Value, position, sourceRect, Color.White, 0f, origin, scale, 0, 0f);
		Utils.DrawBorderStringFourWay(Main.spriteBatch, line.Font, line.Text, position.X, position.Y, line.OverrideColor ?? line.Color, Color.Black, line.Origin);
		return true;
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
		if (!WikithisConfig.Config.TooltipsEnabled)
			return;

		bool tryGet = Wikithis.GetWiki<ItemWiki>().HasValidEntry((short)item.netID);
		string text = tryGet
			? Language.GetTextValue("Mods.Wikithis.Click", TooltipHotkeyString(WikithisSystem.WikiKeybind))
			: Language.GetTextValue("Mods.Wikithis.NoWiki");

		tooltips.Add(new TooltipLine(Mod, TooltipName, Language.GetTextValue("Mods.Wikithis.TextFormatting", text)) {
			OverrideColor = !tryGet
				? Color.Lerp(Color.LightGray, Color.Pink, 0.5f)
				: Color.LightGray
		});
	}

	private static string TooltipHotkeyString(ModKeybind keybind) {
		if (Main.dedServ || keybind == null)
			return string.Empty;

		var assignedKeys = keybind.GetAssignedKeys();
		return assignedKeys.Count == 0 ? "[NONE]" : string.Join(" / ", assignedKeys);
	}
}
