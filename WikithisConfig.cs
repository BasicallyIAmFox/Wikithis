using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Wikithis;

public sealed class WikithisConfig : ModConfig {
	public override ConfigScope Mode => ConfigScope.ClientSide;
	public static WikithisConfig Config => ModContent.GetInstance<WikithisConfig>();

#if TML_2022_09
	[Label("$Mods.Wikithis.Configs.WikithisConfig.OpenSteamBrowser.Label")]
	[Tooltip("$Mods.Wikithis.Configs.WikithisConfig.OpenSteamBrowser.Tooltip")]
#endif
	[DefaultValue(true)]
	public bool OpenSteamBrowser;

#if TML_2022_09
	[Label("$Mods.Wikithis.Configs.WikithisConfig.TooltipsEnabled.Label")]
	[Tooltip("$Mods.Wikithis.Configs.WikithisConfig.TooltipsEnabled.Tooltip")]
#endif
	[DefaultValue(true)]
	public bool TooltipsEnabled;

#if TML_2022_09
	[Label("$Mods.Wikithis.Configs.WikithisConfig.CanWikiNPCs.Label")]
	[Tooltip("$Mods.Wikithis.Configs.WikithisConfig.CanWikiNPCs.Tooltip")]
#endif
	[DefaultValue(false)]
	public bool CanWikiNPCs;
}
