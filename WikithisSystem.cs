using Terraria.ModLoader;

namespace Wikithis;

public sealed class WikithisSystem : ModSystem {
	public static ModKeybind WikiKeybind { get; private set; }

	public override void Load() => WikiKeybind = KeybindLoader.RegisterKeybind(Mod, "WikiKeybind", "O");

	public override void PostAddRecipes() => Wikithis.SetupWikiPages();

	public override void Unload() => WikiKeybind = null;
}