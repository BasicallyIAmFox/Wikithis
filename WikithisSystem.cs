using Terraria.ModLoader;

namespace Wikithis
{
	internal class WikithisSystem : ModSystem
	{
		public static ModKeybind WikiKeybind { get; private set; }

		public override void Load() => WikiKeybind = KeybindLoader.RegisterKeybind(Mod, "Check wiki page on item/NPC", "O");

		public override void PostAddRecipes() => Wikithis.SetupWikiPages();

		public override void Unload() => WikiKeybind = null;
	}
}