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

/*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis;

public sealed class EnglishLanguageManager {
	private readonly LanguageManager languageManager;

	public EnglishLanguageManager() {
		languageManager = LanguageManager_ctor();

		[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
		extern static LanguageManager LanguageManager_ctor();
	}

	public void Load() {
		var culture = GameCulture.FromCultureName(GameCulture.CultureName.English);

		// Not calling `LoadFilesForCulture` just to not call LocalizationLoader.LoadModTranslations
		void LoadVanillaTranslations() {
			string[] languageFilesForCulture = GetLanguageFilesForCulture(languageManager, culture);
			foreach (string text in languageFilesForCulture) {
				try {
					string text2 = Utils.ReadEmbeddedResource(text);
					if (text2 == null || text2.Length < 2)
						throw new FormatException();

					languageManager.LoadLanguageFromFileTextJson(text2, canCreateCategories: true);
				}
				catch (Exception) {
					if (Debugger.IsAttached)
						Debugger.Break();

					Console.WriteLine("Failed to load language file: " + text);
					break;
				}
			}
		}

		void LoadModTranslations(LanguageManager lang) {
			foreach (var mod in ModLoader.Mods) {
				foreach (var (key, value) in LoadTranslations(mod, culture)) {
					lang.GetText(key).SetValue(value);
				}
			}

			static extern List<(string key, string value)> LoadTranslations(Mod mod, GameCulture culture);
		}

		LoadVanillaTranslations();
		LoadModTranslations(languageManager);
		ProcessCopyCommandsInTexts(languageManager);
	}

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(GetLanguageFilesForCulture))]
	private static extern string[] GetLanguageFilesForCulture(LanguageManager languageManager, GameCulture culture);

	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(ProcessCopyCommandsInTexts))]
	private static extern void ProcessCopyCommandsInTexts(LanguageManager languageManager);
}
*/
