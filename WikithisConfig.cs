﻿//
//    Copyright 2023-2024 BasicallyIAmFox
//
//    Licensed under the Apache License, Version 2.0 (the "License")
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

// ReSharper disable UnassignedField.Global

namespace Wikithis;

public sealed class WikithisConfig : ModConfig {
	public override ConfigScope Mode => ConfigScope.ClientSide;
	public static WikithisConfig Config => ModContent.GetInstance<WikithisConfig>();

	[DefaultValue(true)]
	public bool OpenSteamBrowser;

	/* Temporarily disabled
	[DefaultValue(false)]
	[ReloadRequired]
	public bool AlwaysOpenEnglishWiki;
	*/

	[DefaultValue(true)]
	public bool TooltipsEnabled;

	[DefaultValue(false)]
	// ReSharper disable once InconsistentNaming
	public bool CanWikiNPCs;
}
