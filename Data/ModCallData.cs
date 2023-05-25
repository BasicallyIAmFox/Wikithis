using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;

namespace Wikithis.Data;

public sealed class ModCallData {
	public Dictionary<GameCulture.CultureName, string> URLs { get; set; }
	public Asset<Texture2D> PersonalAsset { get; set; }
}
