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

using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Wikithis.Data;

// ReSharper disable StringLiteralTypo

namespace Wikithis;

partial class Wikithis {
	private static string _callMessageCache;

	// ReSharper disable once InconsistentNaming
	private static Dictionary<(short, GameCulture.CultureName), string> itemReplacements = new();
	// ReSharper disable once InconsistentNaming
	private static Dictionary<(short, GameCulture.CultureName), string> npcReplacements = new();

	public static IReadOnlyDictionary<(short, GameCulture.CultureName), string> ItemUrlReplacements => itemReplacements;
	public static IReadOnlyDictionary<(short, GameCulture.CultureName), string> NpcUrlReplacements => npcReplacements;

	public static ConditionalWeakTable<Mod, ModCallData> ModData { get; private set; } = new();

	private static object CallInternal(params object[] args) {
		string message = (args[0] as string)?.ToLower();
		message ??= ((int?)args[0]).Value.ToString();

		_callMessageCache = message;

		switch (_callMessageCache) {
			case "0":
			case "addmodurl":
			case "addurl":
			case "url": {
					if (args.Length is < 3 or > 4)
						throw new ArgumentOutOfRangeException(GetArgumentExceptionReason(3));
					if (args[1] is not Mod mod)
						throw new ArgumentException(GetArgumentNotMatchingTypeReason<Mod>(1));
					if (args[2] is not string url)
						throw new ArgumentException(GetArgumentNotMatchingTypeReason<string>(2));

					var data = ModData.GetOrCreateValue(mod)!;
					data.URLs ??= new Dictionary<GameCulture.CultureName, string>();

					if (args.Length >= 4 && args[3] is GameCulture.CultureName language) {
						if (language is GameCulture.CultureName.English) {
							throw new ArgumentException(GetArgumentCannotHaveValueReason(3, language));
						}

						data.URLs[language] = url;
					}
					else {
						data.URLs[GameCulture.CultureName.English] = url;
					}

					if (!WikiUrlRegex.IsMatch(url)) {
						Instance.Logger.Info($"'{mod.Name}' doesn't matches new format! Recommended to switch to new format");
					}

					return true;
				}
			case "1":
			case "itemidreplacement":
			case "itemidreplacements":
			case "itemidsreplacement":
			case "itemidsreplacements":
			case "replaceitemid":
			case "replaceitemids":
			case "itemreplacement":
			case "itemreplacements":
			case "itemsreplacement":
			case "itemsreplacements":
			case "replaceitem": {
					if (args.Length is < 3 or > 4)
						throw new ArgumentOutOfRangeException(GetArgumentExceptionReason(3));

					var language = GameCulture.CultureName.English;
					if (args.Length == 4) {
						language = (args[3] as GameCulture.CultureName?) ?? GameCulture.CultureName.English;
					}

					switch (args[1]) {
						case int id: {
								if (args[2] is not string url)
									throw new ArgumentException(GetArgumentNotMatchingTypeReason<string>(2));

								itemReplacements.TryAdd(((short)id, language), url);
								break;
							}
						case IEnumerable<int> ids when args[2] is string url: {
								foreach (int i in ids)
									itemReplacements.TryAdd(((short)i, language), url);

								break;
							}
						case IEnumerable<int> ids when args[2] is IEnumerable<string> urls: {
								int[] asArrayIds = ids.ToArray();
								string[] asArrayUrls = urls.ToArray();
								if (asArrayIds.Length != asArrayUrls.Length)
									throw new IndexOutOfRangeException("Arrays at index 1 and 2 must match length of each other");

								for (int i = 0; i < asArrayIds.Length; i++)
									itemReplacements.TryAdd(((short)asArrayIds[i], language), asArrayUrls[i]);

								break;
							}
						case IEnumerable<int>:
							throw new ArgumentException($"Argument at index 2 doesn't matches type: {typeof(string).FullName} or {typeof(IEnumerable<string>).FullName}");
						default:
							throw new ArgumentException($"Argument at index 1 doesn't matches type: {typeof(int).FullName} or {typeof(IEnumerable<int>).FullName}");
					}

					return true;
				}
			case "2":
			case "npcidreplacement":
			case "npcidreplacements":
			case "npcidsreplacement":
			case "npcidsreplacements":
			case "replacenpcid":
			case "replacenpcids":
			case "npcreplacement":
			case "npcreplacements":
			case "npcsreplacement":
			case "npcsreplacements":
			case "replacenpc": {
					if (args.Length is < 3 or > 4)
						throw new ArgumentOutOfRangeException(GetArgumentExceptionReason(3));

					var language = GameCulture.CultureName.English;
					if (args.Length == 4)
						language = ((GameCulture.CultureName?)args[3]).Value;

					switch (args[1]) {
						case int id: {
								if (args[2] is not string url) {
									throw new ArgumentException(GetArgumentNotMatchingTypeReason<string>(2));
								}

								npcReplacements.TryAdd(((short)id, language), url);
								break;
							}
						case IEnumerable<int> ids when args[2] is string url: {
								foreach (int cid in ids) {
									npcReplacements.TryAdd(((short)cid, language), url);
								}

								break;
							}
						case IEnumerable<int> ids when args[2] is IEnumerable<string> urls: {
								int[] asArrayIds = ids.ToArray();
								string[] asArrayUrls = urls.ToArray();
								if (asArrayIds.Length != asArrayUrls.Length) {
									throw new IndexOutOfRangeException("Arrays at index 1 and 2 don't match length of each other");
								}

								for (int i = 0; i < asArrayIds.Length; i++) {
									npcReplacements.TryAdd(((short)asArrayIds[i], language), asArrayUrls[i]);
								}

								break;
							}
						case IEnumerable<int>:
							throw new ArgumentException($"Argument at index 1 doesn't matches type: {typeof(string).FullName} or {typeof(IEnumerable<string>).FullName}");
						default:
							throw new ArgumentException($"Argument at index 1 doesn't matches type: {typeof(int).FullName} or {typeof(IEnumerable<int>).FullName}");
					}

					return true;
				}
			case "3":
			case "addwikitexture":
			case "wikitexture":
			case "addwiki": {
					if (args.Length != 3)
						throw new ArgumentOutOfRangeException(GetArgumentExceptionReason(3));
					if (args[1] is not Mod mod)
						throw new ArgumentException(GetArgumentNotMatchingTypeReason<Mod>(1));
					if (args[2] is not Asset<Texture2D> asset)
						throw new ArgumentException(GetArgumentNotMatchingTypeReason<Asset<Texture2D>>(2));

					ModData.GetOrCreateValue(mod)!.PersonalAsset = asset;
					return true;
				}
			case "4":
			case "customwiki":
			case "addcustomwiki":

			case "5":
			case "openwiki":
			case "opencustomwiki":

			case "6":
			case "delegatewiki":
			case "adddelegatewiki": {
					throw new NotSupportedException("Support was dropped for this mod call. If you need this to keep existing for some reason, please open an issue on github.");
				}
		}

		throw new KeyNotFoundException($"There no calls matching key \"{_callMessageCache}\"");

		string GetArgumentExceptionReason(int expected) => $"Arguments length isn't equal to {expected}. Got: {args.Length}";

		string GetArgumentNotMatchingTypeReason<T>(int index) => $"Argument at index {index} doesn't matches type: {typeof(T).FullName}";

		string GetArgumentCannotHaveValueReason<T>(int index, T value) => $"Argument at index {index} cannot be: {value}";
	}

	public override object Call(params object[] args) {
		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (Main.dedServ)
			return "Got called on server-side!";

#if !DEBUG
		try {
			return CallInternal(args);
		}
		catch (Exception e) {
			Logger.Error($"Call Error: {e.Message}");
		}

		return null;
#else
		return CallInternal(args);
#endif
	}
}
