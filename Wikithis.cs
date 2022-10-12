using CCLiar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Wikithis.Calls;

namespace Wikithis
{
	public partial class Wikithis : Mod
	{
		internal static Regex WikiUrlRegex = new(@".*\/\{.*\}.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		internal static Regex WikiStrRegex = new(@"\{.*\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		internal static Dictionary<string, IWiki> _wikis { get; private set; } = new();
		internal static Dictionary<(Mod, GameCulture.CultureName), string> _modToURL { get; private set; } = new();
		internal static Dictionary<Mod, Asset<Texture2D>> ModToTexture { get; private set; } = new();
		internal static Dictionary<(int, GameCulture.CultureName), string> _itemIdNameReplace { get; private set; } = new();
		internal static Dictionary<(int, GameCulture.CultureName), string> _npcIdNameReplace { get; private set; } = new();
		internal static Dictionary<string, (Func<object, object, bool> pageExists, Func<object, object, bool> openPage)> _delegateWikis { get; private set; } = new();
		internal static Dictionary<string, IWiki<object, IConvertible>> _commandAvailableTypes { get; private set; } = new();

		public static IReadOnlyDictionary<string, IWiki> Wikis => _wikis;
		public static IReadOnlyDictionary<(Mod, GameCulture.CultureName), string> ModToURL => _modToURL;
		public static IReadOnlyDictionary<(int, GameCulture.CultureName), string> ItemIdNameReplace => _itemIdNameReplace;
		public static IReadOnlyDictionary<(int, GameCulture.CultureName), string> NpcIdNameReplace => _npcIdNameReplace;
		public static IReadOnlyDictionary<string, (Func<object, object, bool> pageExists, Func<object, object, bool> openPage)> DelegateWikis => _delegateWikis;
		public static GameCulture.CultureName CultureLoaded { get; private set; }

		private static Wikithis instance;
		public static Wikithis Instance { get => instance; private set => instance = value; }

		public Wikithis()
		{
			Instance = this;

			if (Main.dedServ)
				return;

			CCList<Wikithis>.Initialize();
			CCList<Wikithis>.Register(new AddModUrlCall());
			CCList<Wikithis>.Register(new AddWikiTextureCall());
			CCList<Wikithis>.Register(new CustomWikiCall());
			CCList<Wikithis>.Register(new DelegateWikiCall());
			CCList<Wikithis>.Register(new OpenWikiCall());
			CCList<Wikithis>.Register(new ReplaceItemIdsCall());
			CCList<Wikithis>.Register(new ReplaceNpcIdsCall());
		}

		public override void Load()
		{
			CultureLoaded = (Language.ActiveCulture.Name == "en-US") ? GameCulture.CultureName.English :
				((Language.ActiveCulture.Name == "de-DE") ? GameCulture.CultureName.German :
				((Language.ActiveCulture.Name == "es-ES") ? GameCulture.CultureName.Spanish :
				((Language.ActiveCulture.Name == "fr-FR") ? GameCulture.CultureName.French :
				((Language.ActiveCulture.Name == "it-IT") ? GameCulture.CultureName.Italian :
				((Language.ActiveCulture.Name == "pl-PL") ? GameCulture.CultureName.Polish :
				((Language.ActiveCulture.Name == "pt-BR") ? GameCulture.CultureName.Portuguese :
				((Language.ActiveCulture.Name == "ru-RU") ? GameCulture.CultureName.Russian :
				((Language.ActiveCulture.Name == "zh-Hans") ? GameCulture.CultureName.Chinese : GameCulture.CultureName.English))))))));

			if (Main.dedServ)
				return;

			IL.Terraria.Main.DrawMouseOver += NPCURL;
		}

		internal static void SetupWikiPages()
		{
			foreach (KeyValuePair<string, IWiki> pair in Wikis)
			{
				var wiki = pair.Value as IWiki<object, IConvertible>;
				var typeName = pair.Key.Split('/')[1].ToLowerInvariant();
				if (typeName.EndsWith("wiki"))
					typeName = typeName[..typeName.IndexOf("wiki")];

				_commandAvailableTypes.Add(typeName, wiki);
			}

			foreach (IWiki wiki in Wikis.Values)
			{
				wiki.Initialize();
			}
		}

		public override void Unload()
		{
			_wikis = null;
			_delegateWikis = null;

			_modToURL = null;
			ModToTexture = null;

			_itemIdNameReplace = null;
			_npcIdNameReplace = null;

			CultureLoaded = 0;

			_callMessageCache = null;
			_commandAvailableTypes = null;

			if (Main.dedServ)
				return;

			IL.Terraria.Main.DrawMouseOver -= NPCURL;
			CCList<Wikithis>.Clear();
			Instance = null;
		}

		/// <summary>
		/// Checks if URL is valid.
		/// </summary>
		/// <param name="s">The URL.</param>
		/// <returns></returns>
		public static bool CheckURLValid(string s) => Uri.TryCreate(s, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;

		/// <summary>
		/// Makes default URL link.
		/// <br>If <paramref name="mod"/> is null, then it will use default Terraria wiki.</br>
		/// <br>If <paramref name="mod"/> is not null, then it will try to get domain from ModToURL dictionary.</br>
		/// <br>If ModToURL doesn't contains <paramref name="mod"/>, then stops creating URL.</br>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="mod"></param>
		/// <returns></returns>
		public static string DefaultSearchStr(string name, Mod mod)
		{
			name = name.Replace(' ', '_').Replace("'", "%27");

			string url;
			if (mod == null)
			{
				const int l = 25; // length of "https://terraria.wiki.gg/wiki/"

				url = $"https://terraria.wiki.gg/wiki/{name}";
				if (CultureLoaded == GameCulture.CultureName.Italian)
					url += "/it";
				else if (CultureLoaded != GameCulture.CultureName.English)
					url = url.Insert(l, Language.ActiveCulture.Name[..2] + '/');

				return url;
			}

			string result;
			bool success = false;
			url = string.Empty;

			GameCulture.CultureName culture = CultureLoaded;

			bool doesntContainsOthers = ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(mod, culture), out _);
			if (!doesntContainsOthers)
				culture = GameCulture.CultureName.English;

			if (ModToURL.TryGetValue((mod, culture), out string value))
			{
				success = true;
				url = value;
			}

			if (success && WikiUrlRegex.IsMatch(value))
			{
				return CheckURLValid(result = WikiStrRegex.Replace(value, name)) ? result : string.Empty;
			}

			if (!success)
				return string.Empty;

			string[] urls = url.Split('$');
			string[] urls2 = url.Split('♛');
			result = $"https://{urls[0]}/wiki";

			if (urls.Length >= 2)
			{
				foreach (string v in urls.AsSpan(1))
				{
					result += $"/{v}";
				}
			}

			result += $"/{name}";
			if (urls2.Length > 1)
				result += $"/{urls2[1]}";

			return CheckURLValid(result) ? result : string.Empty;
		}

		/// <summary>
		/// Used to open item wiki page.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="forceCheck"></param>
		public static void OpenWikiPage(Item item, bool forceCheck = true) => OpenWikiPage(entry => entry.ModItem?.Mod, item, item.type, Wikis[$"{nameof(Wikithis)}/{nameof(ItemWiki)}"] as IWiki<Item, int>, false, forceCheck);

		/// <summary>
		/// Used to open NPC wiki page.
		/// </summary>
		/// <param name="npc"></param>
		/// <param name="forceCheck"></param>
		public static void OpenWikiPage(NPC npc, bool forceCheck = true) => OpenWikiPage(entry => entry.ModNPC?.Mod, npc, npc.netID, Wikis[$"{nameof(Wikithis)}/{nameof(NPCWiki)}"] as IWiki<NPC, int>, false, forceCheck);

		/// <summary>
		/// If user has pressed keybind, then next steps are taken:
		///	<br>1. Check if <paramref name="key"/> is valid. If true, then takes to last step. Otherwise takes two next steps.</br>
		///	<br>2. Gets entry by <paramref name="key"/> key.</br>
		///	<br>3. Opens wiki page from entry. Next step is ignored.</br>
		///	<br>4. Types in chat that error happened.</br>
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="key"></param>
		/// <param name="wiki"></param>
		/// <param name="checkForKeybind"></param>
		/// <param name="forceCheck"></param>
		public static void OpenWikiPage<TEntry, TKey>(Func<TEntry, Mod> getMod, TEntry entry, TKey key, IWiki<TEntry, TKey> wiki, bool checkForKeybind = true, bool forceCheck = true) where TEntry : notnull where TKey : notnull, IConvertible
		{
			if (forceCheck && !WikithisSystem.WikiKeybind.JustReleased)
				return;

			if (DelegateWikis.TryGetValue(getMod(entry)?.Name ?? "Terraria", out var delegates) && delegates.pageExists(entry, key))
			{
				if (delegates.openPage(entry, key))
					goto orig;
				return;
			}

		orig:
			if (wiki.IsValid(key))
			{
				wiki.GetEntry(key).OpenWikiPage(checkForKeybind);
			}
			else
			{
				Main.NewText(Language.GetTextValue("Mods.Wikithis.Error"), Color.OrangeRed);

				Instance.Logger.Error("Tried to get wiki page, but failed!");
				wiki.MessageIfDoesntExists(key);
			}
		}

		[Obsolete("Use OpenWikiPage<TEntry, TKey>(Func<TEntry, Mod> getMod, TEntry entry, ...) instead.", true)]
		public static void OpenWikiPage<TEntry, TKey>(TKey key, IWiki<TEntry, TKey> wiki, bool checkForKeybind = true, bool forceCheck = true) where TEntry : notnull where TKey : notnull, IConvertible
		{
			if (forceCheck && !WikithisSystem.WikiKeybind.JustReleased)
				return;

			if (wiki.IsValid(key))
			{
				wiki.GetEntry(key).OpenWikiPage(checkForKeybind);
			}
			else
			{
				Main.NewText(Language.GetTextValue("Mods.Wikithis.Error"), Color.OrangeRed);

				Instance.Logger.Error("Tried to get wiki page, but failed!");
				wiki.MessageIfDoesntExists(key);
			}
		}

		internal static string TooltipHotkeyString(ModKeybind keybind)
		{
			if (Main.dedServ || keybind == null)
				return string.Empty;

			List<string> assignedKeys = keybind.GetAssignedKeys();
			if (assignedKeys.Count == 0)
				return "[NONE]";

			StringBuilder stringBuilder = new(16);
			stringBuilder.Append(assignedKeys[0]);
			for (int index = 1; index < assignedKeys.Count; ++index)
				stringBuilder.Append(" / ").Append(assignedKeys[index]);

			return stringBuilder.ToString();
		}
	}
}