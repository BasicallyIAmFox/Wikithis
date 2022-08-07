using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public override object Call(params object[] args)
		{
			Array.Resize(ref args, 6);
			const string success = "Success";

			try
			{
				string message = (args[0] as string)?.ToLower();
				int? messageOverload = args[0] as int?;
				const int index = 1;

				string[] first = new string[]
				{
					"AddModURL"
				};
				string[] second = new string[]
				{
					"ItemIdReplacement",
					"ItemIdReplacements",
					"ItemIdsReplacement",
					"ItemIdsReplacements",
					"ReplaceItemId",
					"ReplaceItemIds"
				};
				string[] third = new string[]
				{
					"NpcIdReplacement",
					"NpcIdReplacements",
					"NpcIdsReplacement",
					"NpcIdsReplacements",
					"ReplaceNpcId",
					"ReplaceNpcIds"
				};
				string[] fourth = new string[]
				{
					"AddWikiTexture",
					"WikiTexture",
					"AddWiki"
				};
				string[] fifth = new string[]
				{
					"CustomWiki"
				};
				string[] sixth = new string[]
				{
					"OpenCustomWiki"
				};

				if (first.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 0)
				{
					Mod mod = args[index + 0] as Mod;
					string domain = args[index + 1] as string;
					GameCulture.CultureName? culture = args[index + 2] as GameCulture.CultureName?;

					string nameOfArgument = string.Empty;
					if (mod == null)
						nameOfArgument = nameof(mod);
					if (domain == null)
						nameOfArgument = nameof(domain);
					if (culture == null)
						culture = GameCulture.CultureName.English;

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					if (culture != GameCulture.CultureName.English && _modToURL.ContainsKey(new(mod, GameCulture.CultureName.English)))
					{
						_modToURL.TryAdd((mod, culture.Value), domain);
					}
					else if (culture != GameCulture.CultureName.English && !_modToURL.ContainsKey(new(mod, GameCulture.CultureName.English)))
					{
						throw new Exception("English (default; main) key wasn't present in Dictionary, yet translations are being added!");
					}
					else
					{
						_modToURL.TryAdd((mod, culture.Value), domain);
					}
					return success;
				}
				else if (second.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 1)
				{
					int? id2 = args[index + 0] as int?;
					List<int> id = args[index + 0] as List<int>;
					string name = args[index + 1] as string;
					GameCulture.CultureName? culture = args[index + 2] as GameCulture.CultureName?;

					/*if (id != null && id.Any(x => x <= 0 || x >= ItemLoader.ItemCount))
					{
						throw new IndexOutOfRangeException($"Call Error: The {id} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned one of its elements a less/equal to zero or bigger than/equal to maximum Item count.");
					}
					else if (id != null && id.Any(x => x > 0 || x < ItemID.Count))
					{
						throw new IndexOutOfRangeException($"Call Error: The {id} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned vanilla item ID.");
					}*/

					string nameOfArgument = string.Empty;
					if (id == null && id2 == null)
						nameOfArgument = nameof(id);
					if (name == null)
						nameOfArgument = nameof(name);
					if (culture == null)
						culture = GameCulture.CultureName.English;

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					if (id == null || id?.Count == 1)
					{
						_itemIdNameReplace.TryAdd((id2 ?? id[0], culture.Value), name);
					}
					else
					{
						foreach (int i in id)
						{
							_itemIdNameReplace.TryAdd((i, culture.Value), name);
						}
					}
					return success;
				}
				else if (third.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 2)
				{
					int? id2 = args[index + 0] as int?;
					List<int> id = args[index + 0] as List<int>;
					string name = args[index + 1] as string;
					GameCulture.CultureName? culture = args[index + 2] as GameCulture.CultureName?;

					/*if (id != null && id.Any(x => x <= 0 || x >= NPCLoader.NPCCount))
					{
						throw new IndexOutOfRangeException($"Call Error: The {id} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned one of its elements a less/equal to zero or bigger than/equal to maximum NPC count.");
					}
					else if (id != null && id.Any(x => x >= NPCID.NegativeIDCount || x < NPCID.Count))
					{
						throw new IndexOutOfRangeException($"Call Error: The {id} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned vanilla NPC ID.");
					}*/

					string nameOfArgument = string.Empty;
					if (id == null && id2 == null)
						nameOfArgument = nameof(id);
					if (name == null)
						nameOfArgument = nameof(name);
					if (culture == null)
						culture = GameCulture.CultureName.English;

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					if (id == null || id?.Count == 1)
					{
						_npcIdNameReplace.TryAdd((id2 ?? id[0], culture.Value), name);
					}
					else
					{
						foreach (int i in id)
						{
							_npcIdNameReplace.TryAdd((i, culture.Value), name);
						}
					}
					return success;
				}
				else if (fourth.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 3)
				{
					Mod mod = args[index + 0] as Mod;
					Asset<Texture2D> texture = args[index + 1] as Asset<Texture2D>;

					string nameOfArgument = string.Empty;
					if (mod == null)
						nameOfArgument = nameof(mod);
					if (texture == null)
						nameOfArgument = nameof(texture);

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					ModToTexture.TryAdd(mod, texture);
					return success;
				}
				else if (fifth.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 4)
				{
					Mod mod = args[index + 0] as Mod;
					string name = args[index + 1] as string;
					Func<object, object> key = args[index + 2] as Func<object, object>;
					var initialize = args[index + 3] as Action<Func<object, bool>, Action<object, object, string>, Func<string, Mod, string>>;
					var noExists = args[index + 4] as Action<IDictionary<(Mod, GameCulture.CultureName), string>, GameCulture.CultureName, object>;

					string nameOfArgument = string.Empty;
					if (mod == null)
						nameOfArgument = nameof(mod);
					if (name == null)
						nameOfArgument = nameof(name);
					if (key == null)
						nameOfArgument = nameof(key);
					if (initialize == null)
						nameOfArgument = nameof(initialize);

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					mod.AddContent(new SealedWiki(name, key, initialize, noExists));
					return success;
				}
				else if (sixth.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 5)
				{
					Mod mod = args[index + 0] as Mod;
					string name = args[index + 1] as string;
					object key = args[index + 2];
					bool? withKeybind = args[index + 3] as bool?;
					withKeybind ??= true;

					string nameOfArgument = string.Empty;
					if (mod == null)
						nameOfArgument = nameof(mod);
					if (name == null)
						nameOfArgument = nameof(name);

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					if (Wikis.TryGetValue($"{mod.Name}/{name}", out IWiki value))
						(value as IWiki<object, object>).GetEntry(key).OpenWikiPage(withKeybind.Value);

					return success;
				}
				else if (messageOverload.HasValue && messageOverload.Value == 6)
				{
				}
				else
				{
#pragma warning disable CA2208
					throw new ArgumentOutOfRangeException(nameof(messageOverload));
#pragma warning restore CA2208
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Call Error: {e.StackTrace} {e.Message}");
			}

			return null;
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
		public static void OpenWikiPage<TEntry, TKey>(TKey key, IWiki<TEntry, TKey> wiki, bool checkForKeybind = true)
		{
			if (!WikithisSystem.WikiKeybind.JustReleased)
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

		internal static void OpenWikiPage(Mod mod, Item item) => OpenWikiPage(item.type, Wikis[$"Wikithis/{nameof(ItemWiki)}"] as IWiki<Item, int>, false);

		internal static void OpenWikiPage(Mod mod, NPC npc) => OpenWikiPage(npc.netID, Wikis[$"Wikithis/{nameof(NPCWiki)}"] as IWiki<NPC, int>, false);

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