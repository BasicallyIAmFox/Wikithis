using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Wikithis;

public sealed class WikithisConfig : ModConfig {
	public override ConfigScope Mode => ConfigScope.ClientSide;
	internal static WikithisConfig Config => ModContent.GetInstance<WikithisConfig>();

	[Label("$Mods.Wikithis.Config.OpenSteamBrowser.Label")]
	[Tooltip("$Mods.Wikithis.Config.OpenSteamBrowser.Tooltip")]
	[DefaultValue(true)]
	public bool OpenSteamBrowser;

	[Label("$Mods.Wikithis.Config.TooltipsEnabled.Label")]
	[Tooltip("$Mods.Wikithis.Config.TooltipsEnabled.Tooltip")]
	[DefaultValue(true)]
	public bool TooltipsEnabled;

	[Label("$Mods.Wikithis.Config.CanWikiNPCs.Label")]
	[Tooltip("$Mods.Wikithis.Config.CanWikiNPCs.Tooltip")]
	[DefaultValue(false)]
	public bool CanWikiNPCs;
}
