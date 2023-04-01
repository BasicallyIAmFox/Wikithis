using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Wikithis.Wikis;

namespace Wikithis;

internal sealed class WikithisItem : GlobalItem {
	private const float scaleValue = 2f / 3f;

	public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset) {
		bool isAvailable = Wikithis.GetWiki<ItemWiki>().Entries.TryGetValue((short)item.netID, out var wikiEntry);
		if (line.Mod == Mod.Name && line.Name == $"{nameof(Wikithis)}:Wiki") {
			Asset<Texture2D> texture;
			bool defaultTexture = false;
			if (!isAvailable) {
				Main.instance.LoadItem(ItemID.WireCutter);
				texture = TextureAssets.Item[ItemID.WireCutter];
			}
			else if (item.ModItem?.Mod != null && Wikithis.dataForMods.GetOrCreateValue(item.ModItem.Mod).PersonalAsset != null) {
				texture = Wikithis.dataForMods.GetOrCreateValue(item.ModItem.Mod).PersonalAsset;
			}
			else {
				texture = TextureAssets.BestiaryMenuButton;
				defaultTexture = true;
			}

			Vector2 scale = new(scaleValue, scaleValue);
			Vector2 origin = new(defaultTexture ? 0f : -((30f - texture.Width()) / 2f), defaultTexture ? 0f : -((TextureAssets.BestiaryMenuButton.Height() - texture.Height()) / 2f));

			Main.spriteBatch.Draw(texture.Value, new Vector2(line.X, line.Y), new Rectangle(0, 0, defaultTexture ? 30 : texture.Width(), texture.Height()), Color.White, 0f, origin, scale, 0, 0f);
			Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, line.OverrideColor ?? line.Color, Color.Black, line.Origin);
			return false;
		}
		if (WikithisSystem.WikiKeybind.JustPressed && line.Mod == "Terraria" && line.Name == "ItemName" && isAvailable) {
			wikiEntry.OpenWikiPage(false);
		}
		return true;
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
		if (!WikithisConfig.Config.TooltipsEnabled)
			return;

		string text;
		bool tryGet = Wikithis.GetWiki<ItemWiki>().Entries.TryGetValue((short)item.netID, out _);
		if (tryGet) {
			text = Language.GetTextValue($"Mods.{Mod.Name}.Click", TooltipHotkeyString(WikithisSystem.WikiKeybind));
		}
		else {
			text = Language.GetTextValue($"Mods.{Mod.Name}.NoWiki");
		}
		tooltips.Add(new(Mod, "Wikithis:Wiki", $"    {text}") {
			OverrideColor = !tryGet ? Color.Lerp(Color.LightGray, Color.Pink, 0.5f) : Color.LightGray
		});
	}

	private static string TooltipHotkeyString(ModKeybind keybind) {
		if (Main.dedServ || keybind == null)
			return string.Empty;

		List<string> assignedKeys = keybind.GetAssignedKeys();
		if (assignedKeys.Count == 0)
			return "[NONE]";

		return string.Join(" / ", assignedKeys);

		/*StringBuilder stringBuilder = new(16);
		stringBuilder.Append(assignedKeys[0]);
		for (int index = 1; index < assignedKeys.Count; ++index)
			stringBuilder.Append(" / ").Append(assignedKeys[index]);

		return stringBuilder.ToString();*/
	}
}
