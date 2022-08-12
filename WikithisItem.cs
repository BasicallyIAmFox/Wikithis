using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	internal class WikithisItem : GlobalItem
	{
		public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
		{
			IWiki<Item, int> wiki = Wikithis.Wikis[$"Wikithis/{nameof(ItemWiki)}"] as IWiki<Item, int>;
			bool wrong = !wiki.IsValid(item.type);
			if (wrong && Wikithis.DelegateWikis.TryGetValue(item.ModItem?.Mod.Name ?? "Terraria", out var delegates) && delegates.pageExists(item, item.type))
			{
				wrong = false;
			}

			if (line.Mod == Mod.Name && line.Name == "Wikithis:Wiki")
			{
				Asset<Texture2D> texture = TextureAssets.BestiaryMenuButton;
				if (item.ModItem != null && Wikithis.ModToTexture.TryGetValue(item.ModItem.Mod, out Asset<Texture2D> value))
					texture = value;

				if (wrong)
				{
					Main.instance.LoadItem(ItemID.WireCutter);
					texture = TextureAssets.Item[ItemID.WireCutter];
				}

				Vector2 scale = new(2f / 3f, 2f / 3f);
				Vector2 origin = new(!wrong ? 0f : -((30f - texture.Width()) / 2f), !wrong ? 0f : -((TextureAssets.BestiaryMenuButton.Height() - texture.Height()) / 2f));

				Main.spriteBatch.Draw(texture.Value, new Vector2(line.X, line.Y), new Rectangle(0, 0, wrong ? texture.Width() : 30, texture.Height()), Color.White, 0f, origin, scale, 0, 0f);
				Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, line.OverrideColor ?? line.Color, Color.Black, line.Origin);
				return false;
			}
			if (!wrong && WikithisSystem.WikiKeybind.JustPressed && line.Mod == "Terraria" && line.Name == "ItemName")
			{
				Wikithis.OpenWikiPage(item, false);
			}
			return true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (!WikithisConfig.Config.TooltipsEnabled)
				return;

			IWiki<Item, int> wiki = Wikithis.Wikis[$"Wikithis/{nameof(ItemWiki)}"] as IWiki<Item, int>;
			bool wrong = !wiki.IsValid(item.type);
			if (wrong && Wikithis.DelegateWikis.TryGetValue(item.ModItem?.Mod.Name ?? "Terraria", out var delegates) && delegates.pageExists(item, item.type))
			{
				wrong = false;
			}

			string text = Language.GetTextValue($"Mods.{Mod.Name}.Click", Wikithis.TooltipHotkeyString(WikithisSystem.WikiKeybind));
			if (wrong)
				text = Language.GetTextValue($"Mods.{Mod.Name}.NoWiki");

			tooltips.Add(new(Mod, "Wikithis:Wiki", $"    {text}")
			{
				OverrideColor = !wrong ? Color.LightGray : Color.Lerp(Color.LightGray, Color.Pink, 0.5f)
			});

			if (ModLoader.TryGetMod("CalamityMod", out Mod calamity) && item.ModItem?.Mod.Name == calamity?.Name && calamity?.Version <= new Version(2, 0, 0, 3))
			{
				tooltips.Add(new(Mod, "Wikithis:WikiClam", "Stop saying that mod is bad because it doesnt supports Calamity.")
				{
					OverrideColor = Color.Lerp(Color.LightGray, Color.Pink, 0.5f)
				});
				tooltips.Add(new(Mod, "Wikithis:WikiClam2", "Calamity *will* support Wikithis in next update. Thanks.")
				{
					OverrideColor = Color.Lerp(Color.LightGray, Color.Pink, 0.5f)
				});
			}
		}
	}
}