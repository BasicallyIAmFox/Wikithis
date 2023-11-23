//
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

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Wikithis.Wikis;
using BindingFlags = System.Reflection.BindingFlags;

namespace Wikithis;

public sealed partial class Wikithis : Mod {
	public static Regex WikiUrlRegex { get; private set; } = new(@".*\/\{.*\}.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	public static Regex WikiStrRegex { get; private set; } = new(@"\{.*\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static GameCulture.CultureName CurrentCulture { get; private set; }

	public static Wikithis Instance { get; private set; }

	private static LanguageManager _englishLanguageManager;

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

		if (WikithisConfig.Config.AlwaysOpenEnglishWiki)
			CurrentCulture = GameCulture.CultureName.English;

		IL_Main.HoverOverNPCs += ClickableNPCsWithMouse;

#if DEBUG
		WikithisTests.TestModCalls();
#endif
	}

	internal static void SetupWikiPages() {
		if (Main.dedServ)
			return;

		// I don't like this at all... but do I really have other choice?
		var languageManager = LanguageManager.Instance;
		
		if (WikithisConfig.Config.AlwaysOpenEnglishWiki) {
			if (_englishLanguageManager == null) {
				_englishLanguageManager =
					typeof(LanguageManager)
						.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Array.Empty<Type>())!
						.Invoke(null) as LanguageManager;
				
				Debug.Assert(_englishLanguageManager != null);
			}

			_englishLanguageManager!.SetLanguage(GameCulture.DefaultCulture);
			languageManager = _englishLanguageManager;

			Thread.CurrentThread.CurrentCulture = _englishLanguageManager.ActiveCulture.CultureInfo;
			Thread.CurrentThread.CurrentUICulture = _englishLanguageManager.ActiveCulture.CultureInfo;
		}

		foreach (var wiki in ModContent.GetContent<IWiki>())
			wiki.Initialize(languageManager);

		Thread.CurrentThread.CurrentCulture = Language.ActiveCulture.CultureInfo;
		Thread.CurrentThread.CurrentUICulture = Language.ActiveCulture.CultureInfo;
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
