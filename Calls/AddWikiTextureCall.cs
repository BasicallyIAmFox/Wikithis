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
			var mod = args.Get<Mod>(0);
			var texture = args.Get<Asset<Texture2D>>(1);

			Wikithis.ModToTexture.TryAdd(mod, texture);
			return Wikithis.GotoSuccessReturn();
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

		public void Unload() => array = null;

		public void Load(Mod mod)
		{
		}
	}
}
