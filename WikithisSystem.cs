using Terraria.ModLoader;

namespace Wikithis
{
	internal class WikithisSystem : ModSystem
	{
		public static ModKeybind WikiKeybind { get; private set; }
		internal static bool RickRolled { get; set; }

		public override void OnWorldLoad() => RickRolled = false;

		public override void OnWorldUnload() => RickRolled = false;

		public override void Load() => WikiKeybind = KeybindLoader.RegisterKeybind(Mod, "$Mods.Wikithis.WikiKeybind", "O");

		public override void PostAddRecipes() => Wikithis.SetupWikiPages();

		public override void Unload() => WikiKeybind = null;
	}
}