using Terraria.ModLoader;

namespace Wikithis;

public sealed class WikithisSystem : ModSystem {
	public static ModKeybind WikiKeybind { get; private set; }

	public override void Load() {
		WikiKeybind = KeybindLoader.RegisterKeybind(
			Mod,
#if TML_2022_09
			"Check wiki page on item/NPC"
#else
			"WikiKeybind"
#endif
			, "O");
	}

	public override void PostAddRecipes() => Wikithis.SetupWikiPages();

	public override void Unload() => WikiKeybind = null;
}