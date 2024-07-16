//
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

using System.Text.RegularExpressions;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Wikithis.Wikis;

namespace Wikithis;

public sealed partial class Wikithis : Mod {
	public static Regex WikiUrlRegex { get; private set; } = _WikiUrlRegex();
	public static Regex WikiStrRegex { get; private set; } = _WikiStrRegex();

	[GeneratedRegex(@".*\/\{.*\}.*", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
	private static partial Regex _WikiUrlRegex();
	[GeneratedRegex(@"\{.*\}", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
	private static partial Regex _WikiStrRegex();

	public static GameCulture.CultureName CurrentCulture { get; private set; }
	//private static Lazy<EnglishLanguageManager> EnglishLanguageManager { get; } = new();

	public static Wikithis Instance { get; private set; }

	public Wikithis() {
		Instance = this;
	}

	public override void Load() {
		CurrentCulture = Language.ActiveCulture.Name switch {
			"de-DE" => GameCulture.CultureName.German,
			"es-ES" => GameCulture.CultureName.Spanish,
			"fr-FR" => GameCulture.CultureName.French,
			"it-IT" => GameCulture.CultureName.Italian,
			"pl-PL" => GameCulture.CultureName.Polish,
			"pt-BR" => GameCulture.CultureName.Portuguese,
			"ru-RU" => GameCulture.CultureName.Russian,
			"zh-Hans" => GameCulture.CultureName.Chinese,
			_ => GameCulture.CultureName.English
		};

		if (Main.dedServ)
			return;

		//if (WikithisConfig.Config.AlwaysOpenEnglishWiki)
		//	CurrentCulture = GameCulture.CultureName.English;

		IL_Main.HoverOverNPCs += ClickableNPCsWithMouse;

#if DEBUG
		WikithisTests.TestModCalls();
#endif
	}

	internal static void SetupWikiPages() {
		if (Main.dedServ)
			return;

		var languageManager = LanguageManager.Instance;

		//if (WikithisConfig.Config.AlwaysOpenEnglishWiki)
		//	languageManager = EnglishLanguageManager.Value;

		foreach (var wiki in ModContent.GetContent<IWiki>())
			wiki.Initialize(languageManager);
	}

	public override void Unload() {
		Instance = null;

		WikiUrlRegex = null;
		WikiStrRegex = null;

		itemReplacements?.Clear();
		itemReplacements = null;

		npcReplacements?.Clear();
		npcReplacements = null;

		ModData?.Clear();
		ModData = null;

		CurrentCulture = 0;
		_callMessageCache = null;

		if (Main.dedServ)
			return;

		IL_Main.HoverOverNPCs -= ClickableNPCsWithMouse;
	}

	public static T GetWiki<T>() where T : class, IWiki {
		return ModContent.GetInstance<T>();
	}
}
