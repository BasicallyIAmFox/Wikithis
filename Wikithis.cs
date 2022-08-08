using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis : Mod
	{
		internal const string RickRoll = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
		internal static bool AprilFools { get; private set; }
		internal static Dictionary<string, IWiki> _wikis { get; private set; }
		internal static Dictionary<(Mod, GameCulture.CultureName), string> _modToURL { get; private set; }
		internal static Dictionary<Mod, Asset<Texture2D>> ModToTexture { get; private set; }
		internal static Dictionary<(int, GameCulture.CultureName), string> _itemIdNameReplace { get; private set; }
		internal static Dictionary<(int, GameCulture.CultureName), string> _npcIdNameReplace { get; private set; }
		public static GameCulture.CultureName CultureLoaded { get; private set; }
		private static Wikithis instance;

		public static IDictionary<string, IWiki> Wikis => _wikis;
		public static IDictionary<(Mod, GameCulture.CultureName), string> ModToURL => _modToURL;
		public static IDictionary<(int, GameCulture.CultureName), string> ItemIdNameReplace => _itemIdNameReplace;
		public static IDictionary<(int, GameCulture.CultureName), string> NpcIdNameReplace => _npcIdNameReplace;

		public static Wikithis Instance { get => instance; private set => instance = value; }

		public Wikithis()
		{
			_wikis = new();

			_modToURL = new();
			ModToTexture = new();

			_itemIdNameReplace = new();
			_npcIdNameReplace = new();

			AprilFools = DateTime.Now.Day == 1 && DateTime.Now.Month == 4;
			Instance = this;
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
			foreach (IWiki wiki in Wikis.Values)
			{
				wiki.Initialize();
			}
		}

		public override void Unload()
		{
			_wikis = null;

			_modToURL = null;
			ModToTexture = null;

			_itemIdNameReplace = null;
			_npcIdNameReplace = null;

			AprilFools = false;
			CultureLoaded = 0;

			if (Main.dedServ)
				return;

			IL.Terraria.Main.DrawMouseOver -= NPCURL;
			Instance = null;
		}

		/// <summary>
		/// Checks if URL is valid.
		/// </summary>
		/// <param name="s">The URL.</param>
		/// <returns></returns>
		public static bool CheckURLValid(string s) => Uri.TryCreate(s, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;

		internal static string GetInternalName(int id, int num = 0) => num == 0 ? ItemID.Search.GetName(id) : NPCID.Search.GetName(id);

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
			name = name.Replace(' ', '_');
			name = name.Replace("'", "%27");

			if (mod == null)
			{
				const int l = 25; // length of "https://terraria.wiki.gg/wiki/"

				string url = $"https://terraria.wiki.gg/wiki/{name}";
				if (CultureLoaded == GameCulture.CultureName.Polish)
					url = url.Insert(l, "pl/");
				else if (CultureLoaded == GameCulture.CultureName.Italian)
					url += "/it";
				else if (CultureLoaded == GameCulture.CultureName.French)
					url = url.Insert(l, "fr/");
				else if (CultureLoaded == GameCulture.CultureName.Spanish)
					url = url.Insert(l, "pl/");
				else if (CultureLoaded == GameCulture.CultureName.Russian)
					url = url.Insert(l, "ru/");
				else if (CultureLoaded == GameCulture.CultureName.German)
					url = url.Insert(l, "de/");
				else if (CultureLoaded == GameCulture.CultureName.Portuguese)
					url = url.Insert(l, "pt/");
				else if (CultureLoaded == GameCulture.CultureName.Chinese)
					url = url.Insert(l, "zh/");

				return url;
			}
			else
			{
				string url = string.Empty;
				bool success = false;

				GameCulture.CultureName culture = CultureLoaded;

				bool doesntContainsOthers = ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(mod, culture), out _);
				if (!doesntContainsOthers)
					culture = GameCulture.CultureName.English;

				if (ModToURL.TryGetValue((mod, culture), out string value))
				{
					success = true;
					url = value;
				}

				if (!success)
					return string.Empty;

				string[] urls = url.Split('$');
				string[] urls2 = url.Split('♛');
				string result = $"https://{urls[0]}/wiki";

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
		}

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
		public static void OpenWikiPage<TEntry, TKey>(TKey key, IWiki<TEntry, TKey> wiki, bool checkForKeybind = true, bool forceCheck = true)
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

		internal static void OpenWikiPage(Item item, bool forceCheck = true) => OpenWikiPage(item.type, Wikis[$"Wikithis/{nameof(ItemWiki)}"] as IWiki<Item, int>, false, forceCheck);

		internal static void OpenWikiPage(NPC npc, bool forceCheck = true) => OpenWikiPage(npc.netID, Wikis[$"Wikithis/{nameof(NPCWiki)}"] as IWiki<NPC, int>, false, forceCheck);

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