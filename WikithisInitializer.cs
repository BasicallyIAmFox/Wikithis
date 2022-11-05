using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Wikithis
{
	internal static class WikithisInitializer
	{
		public static Dictionary<string, Asset<Texture2D>> a = new();

		public static void InitializeEverything()
		{
			InitializeTextures();
		}

		private static void InitializeTextures()
		{
			foreach (Mod mod in ModLoader.Mods)
			{
				if (Wikithis.ModToTexture.TryGetValue(mod, out var asset)) { }
				asset ??= TextureAssets.BestiaryMenuButton;
				a.TryAdd(mod.Name, asset);
			}
			a.TryAdd("Terraria", TextureAssets.BestiaryMenuButton);
		}
	}
}