using CCLiar;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.ModLoader;

namespace Wikithis.Calls
{
	public sealed class AddWikiTextureCall : CCList, ILoadable
	{
		private static string[] array;

		public AddWikiTextureCall() : base(x => Array.IndexOf(array, x) != -1, args =>
		{
			var mod = args.Get<Mod>(0, _ => _ == null);
			var texture = args.Get<Asset<Texture2D>>(1, _ => _ == null);

			return Call(mod, texture);
		}, new ICCKey[]
		{
			new CCKey<Mod>(),
			new CCKey<Asset<Texture2D>>(),
		})
		{
			array = new string[]
			{
				"3",
				"addwikitexture",
				"wikitexture",
				"addwiki"
			};
		}

		public static bool Call(Mod mod, Asset<Texture2D> asset)
		{
			Wikithis.ModToTexture.TryAdd(mod, asset);
			return Wikithis.GotoSuccessReturn();
		}

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}
