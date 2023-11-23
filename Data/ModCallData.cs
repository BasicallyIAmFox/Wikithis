﻿//
//    Copyright 2023 BasicallyIAmFox
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

using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Localization;

namespace Wikithis.Data;

public sealed class ModCallData {
	// ReSharper disable once InconsistentNaming
	public Dictionary<GameCulture.CultureName, string> URLs { get; set; }
	public Asset<Texture2D> PersonalAsset { get; set; }
}
