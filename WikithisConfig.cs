using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Wikithis;

public sealed class WikithisConfig : ModConfig {
	public sealed override ConfigScope Mode => ConfigScope.ClientSide;
	public static WikithisConfig Config => ModContent.GetInstance<WikithisConfig>();

	[DefaultValue(true)]
	public bool OpenSteamBrowser;

	[DefaultValue(true)]
	public bool TooltipsEnabled;

	[DefaultValue(false)]
	public bool CanWikiNPCs;
}
