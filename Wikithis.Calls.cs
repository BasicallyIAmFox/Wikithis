using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Wikithis.Data;
using Wikithis.Wikis;

namespace Wikithis;

public partial class Wikithis {
	private static string _callMessageCache;

	internal static List<IWiki> wikis = new();
	internal static Dictionary<(short, GameCulture.CultureName), string> itemReplacements = new();
	internal static Dictionary<(short, GameCulture.CultureName), string> npcReplacements = new();

	public static IReadOnlyList<IWiki> Wikis => wikis;
	public static IReadOnlyDictionary<(short, GameCulture.CultureName), string> ItemUrlReplacements => itemReplacements;
	public static IReadOnlyDictionary<(short, GameCulture.CultureName), string> NpcUrlReplacements => npcReplacements;

	internal static ConditionalWeakTable<Mod, ModCallData> dataForMods = new();

	public override object Call(params object[] args) {
		if (Main.dedServ)
			return "Got called on server-side!";

		try {
			string message = (args[0] as string)?.ToLower();
			message ??= (args[0] as int?).Value.ToString();

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

						var data = dataForMods.GetOrCreateValue(mod);
						data.URLs ??= new();

						if (args[3] is GameCulture.CultureName language) {
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
						if (args[1] is int id) {
							if (args[2] is not string url) {
								throw new ArgumentException(GetArgumentNotMatchingTypeReason<string>(2));
							}
							itemReplacements.Add(((short)id, language), url);
						}
						else if (args[1] is IEnumerable<int> ids) {
							if (args[2] is string url) {
								foreach (var i in ids) {
									itemReplacements.Add(((short)i, language), url);
								}
							}
							else if (args[2] is IEnumerable<string> urls) {
								var asArrayIds = ids.ToArray();
								var asArrayUrls = urls.ToArray();
								if (asArrayIds.Length != asArrayUrls.Length) {
									throw new IndexOutOfRangeException("Arrays at index 1 and 2 don't match length of each other");
								}
								for (int i = 0; i < asArrayIds.Length; i++) {
									itemReplacements.Add(((short)i, language), asArrayUrls[i]);
								}
							}
							else {
								throw new ArgumentException($"Argument at index 1 doesn't matches type: {typeof(string).FullName} or {typeof(IEnumerable<string>).FullName}");
							}
						}
						else {
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
						if (args.Length == 4) {
							language = (args[3] as GameCulture.CultureName?).Value;
						}
						if (args[1] is int id) {
							if (args[2] is not string url) {
								throw new ArgumentException(GetArgumentNotMatchingTypeReason<string>(2));
							}
							npcReplacements.Add(((short)id, language), url);
						}
						else if (args[1] is IEnumerable<int> ids) {
							if (args[2] is not string url) {
								throw new ArgumentException(GetArgumentNotMatchingTypeReason<string>(2));
							}
							else if (args[2] is IEnumerable<string> urls) {
								var asArrayIds = ids.ToArray();
								var asArrayUrls = urls.ToArray();
								if (asArrayIds.Length != asArrayUrls.Length) {
									throw new IndexOutOfRangeException("Arrays at index 1 and 2 don't match length of each other");
								}
								for (int i = 0; i < asArrayIds.Length; i++) {
									npcReplacements.Add(((short)i, language), asArrayUrls[i]);
								}
							}
							else {
								throw new ArgumentException($"Argument at index 1 doesn't matches type: {typeof(string).FullName} or {typeof(IEnumerable<string>).FullName}");
							}
						}
						else {
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

						dataForMods.GetOrCreateValue(mod).PersonalAsset = asset;
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
		}
		catch (Exception e) {
			Logger.Error($"Call Error: {e.Message}");
		}
		return null;

		string GetArgumentExceptionReason(int expected) {
			return $"Arguments length isn't equal to {expected}. Got: {args.Length}";
		}
		string GetArgumentNotMatchingTypeReason<T>(int index) {
			return $"Argument at index {index} doesn't matches type: {typeof(T).FullName}";
		}
		string GetArgumentCannotHaveValueReason<T>(int index, T value) {
			return $"Argument at index {index} cannot be: {value}";
		}
	}
}
