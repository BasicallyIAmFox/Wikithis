using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Wikithis.Wikithis;

namespace Wikithis
{
	// TODO: UNDERSTAND THE CODEBASE; make it actually readable and organized?
	public partial class Wikithis : Mod
	{
		internal const string RickRoll = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

		internal static bool AprilFools { get; private set; }
		internal static Dictionary<(Mod, GameCulture.CultureName), string> ModToURL { get; private set; }
		internal static Dictionary<Mod, Asset<Texture2D>> ModToTexture { get; private set; }

		internal static Dictionary<(int, GameCulture.CultureName), string> ItemIdNameReplace { get; private set; }
		internal static Dictionary<(int, GameCulture.CultureName), string> NpcIdNameReplace { get; private set; }
		internal static Dictionary<(int, GameCulture.CultureName), string> TileIdNameReplace { get; private set; }

		internal static GameCulture.CultureName CultureLoaded { get; private set; }

		public Wikithis()
		{
			ModToURL = new();
			ModToTexture = new();

			ItemIdNameReplace = new();
			NpcIdNameReplace = new();
			TileIdNameReplace = new();

			AprilFools = DateTime.Now.Day == 1 && DateTime.Now.Month == 4;
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

		private void NPCURL(ILContext il)
		{
			ILCursor c = new(il);
			try
			{
				c.GotoNext(i => i.MatchCall(typeof(NPCLoader).GetMethod(nameof(NPCLoader.ModifyHoverBoundingBox), BindingFlags.Public | BindingFlags.Static)))
					.GotoNext(i => i.MatchStloc(14));

				c.Index++;
				c.Emit(OpCodes.Ldsfld, typeof(Main).GetField(nameof(Main.npc)))
					.Emit(OpCodes.Ldloc, 10)
					.Emit(OpCodes.Ldelem_Ref);
				c.Emit(OpCodes.Ldloc, 14);
				c.EmitDelegate<Action<NPC, bool>>((npc, hovers) =>
				{
					if (WikithisConfig.Config.CanWikiNPCs && hovers && WikithisSystem.WikiKeybind.JustPressed)
					{
						OpenWikiPage(this, npc);
					}
				});
			}
			catch (Exception e)
			{
				Logger.Error($"IL Error: {e.Message} {e.StackTrace}");
			}
		}

		public override object Call(params object[] args)
		{
			Array.Resize(ref args, 4);
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

					if (culture != GameCulture.CultureName.English && ModToURL.ContainsKey(new(mod, GameCulture.CultureName.English)))
					{
						ModToURL.TryAdd((mod, culture.Value), domain);
					}
					else if (culture != GameCulture.CultureName.English && !ModToURL.ContainsKey(new(mod, GameCulture.CultureName.English)))
					{
						throw new Exception("English (default; main) key wasn't present in Dictionary, yet translations are being added!");
					}
					else
					{
						ModToURL.TryAdd((mod, culture.Value), domain);
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
						ItemIdNameReplace.TryAdd((id2 ?? id[0], culture.Value), name);
					}
					else
					{
						foreach (int i in id)
						{
							ItemIdNameReplace.TryAdd((i, culture.Value), name);
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
						NpcIdNameReplace.TryAdd((id2 ?? id[0], culture.Value), name);
					}
					else
					{
						foreach (int i in id)
						{
							NpcIdNameReplace.TryAdd((i, culture.Value), name);
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
				else if (messageOverload.HasValue && messageOverload.Value == 4)
				{
				}
				else if (messageOverload.HasValue && messageOverload.Value == 5)
				{
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

		public override void Unload()
		{
			ModToURL = null;
			ModToTexture = null;

			WikiEntries = null;

			ItemIdNameReplace = null;
			NpcIdNameReplace = null;

			AprilFools = false;
			CultureLoaded = 0;

			if (Main.dedServ)
				return;

			IL.Terraria.Main.DrawMouseOver -= NPCURL;
			//On.Terraria.Main.DrawMouseOver -= TileURL;
		}

		internal static string TooltipHotkeyString(ModKeybind keybind)
		{
			if (Main.dedServ || keybind == null)
				return "";

			List<string> assignedKeys = keybind.GetAssignedKeys(InputMode.Keyboard);
			if (assignedKeys.Count == 0)
				return "[NONE]";

			StringBuilder stringBuilder = new(16);
			stringBuilder.Append(assignedKeys[0]);
			for (int index = 1; index < assignedKeys.Count; ++index)
				stringBuilder.Append(" / ").Append(assignedKeys[index]);

			return stringBuilder.ToString();
		}
	}

	// actually makes you able to use urls on items
	internal class WikithisItem : GlobalItem
	{
		public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
		{
			bool exists = !WikiEntries[typeof(Item)].ContainsKey(item.type.ToString()) || WikiEntries[typeof(Item)].ContainsKey(item.type.ToString()) && !CheckURLValid(WikiEntries[typeof(Item)][item.type.ToString()].Search);
			if (line.Mod == Mod.Name && line.Name == "Wikithis:Wiki")
			{
				Asset<Texture2D> texture = TextureAssets.BestiaryMenuButton;
				if (item.ModItem != null && ModToTexture.TryGetValue(item.ModItem.Mod, out Asset<Texture2D> value))
					texture = value;

				if (exists)
				{
					Main.instance.LoadItem(ItemID.WireCutter);
					texture = TextureAssets.Item[ItemID.WireCutter];
				}

				Vector2 scale = new(2f / 3f, 2f / 3f);
				if (texture.Width() + texture.Height() > 60)
					scale = new(20f / (exists ? texture.Width() : 30f), 20f / texture.Height());
				Vector2 origin = new(!exists ? 0f : -((30f - texture.Width()) / 2f), !exists ? 0f : -((TextureAssets.BestiaryMenuButton.Height() - texture.Height()) / 2f));

				Main.spriteBatch.Draw(texture.Value, new Vector2(line.X, line.Y), new Rectangle(0, 0, exists ? texture.Width() : 30, texture.Height()), Color.White, 0f, origin, scale, 0, 0f);
				Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, line.OverrideColor ?? line.Color, Color.Black, line.Origin);
				return false;
			}
			if (!exists && WikithisSystem.WikiKeybind.JustPressed && line.Mod == "Terraria" && line.Name == "ItemName")
			{
				OpenWikiPage(Mod, item);
			}
			return true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (!WikithisConfig.Config.TooltipsEnabled)
				return;

			bool exists = !WikiEntries[typeof(Item)].ContainsKey(item.type.ToString()) || WikiEntries[typeof(Item)].ContainsKey(item.type.ToString()) && !CheckURLValid(WikiEntries[typeof(Item)][item.type.ToString()].Search);
			string text = Language.GetTextValue($"Mods.{Mod.Name}.Click", TooltipHotkeyString(WikithisSystem.WikiKeybind));
			if (exists)
				text = Language.GetTextValue($"Mods.{Mod.Name}.NoWiki");

			tooltips.Add(new(Mod, "Wikithis:Wiki", $"    {text}")
			{
				OverrideColor = !exists ? Color.LightGray : Color.Lerp(Color.LightGray, Color.Pink, 0.5f)
			});

			if (ModLoader.TryGetMod("CalamityMod", out Mod calamity) && item.ModItem?.Mod.Name == calamity?.Name && calamity?.Version <= new Version(2, 0, 0, 3))
			{
				tooltips.Add(new(Mod, "Wikithis:WikiClam", "Stop saying that mod is bad because it doesnt supports Calamity.")
				{
					OverrideColor = Color.Lerp(Color.LightGray, Color.Pink, 0.5f)
				});
				tooltips.Add(new(Mod, "Wikithis:WikiClam2", "Calamity *will* support Wikithis in next update. Thanks.")
				{
					OverrideColor = Color.Lerp(Color.LightGray, Color.Pink, 0.5f)
				});
			}
		}
	}

	// setups all urls
	internal class WikithisSystem : ModSystem
	{
		public static ModKeybind WikiKeybind { get; private set; }
		internal static bool RickRolled { get; set; }

		public override void OnWorldLoad() => RickRolled = false;

		public override void OnWorldUnload() => RickRolled = false;

		public override void Load() => WikiKeybind = KeybindLoader.RegisterKeybind(Mod, "Check wiki page on item/NPC", "O");

		public override void PostAddRecipes() => SetupWikiPages();

		public override void Unload() => WikiKeybind = null;
	}

	// self-explanatory
	internal class WikithisCommand : ModCommand
	{
		public override string Command => "wikithis";

		public override string Usage => $"/wikithis <npc|item> [name of type]\n{Language.GetTextValue($"Mods.{Mod.Name}.WikithisInput")}";

		public override string Description => Language.GetTextValue($"Mods.{Mod.Name}.WikithisDesc");

		public override CommandType Type => CommandType.Chat;

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (caller.Player.whoAmI == Main.myPlayer && args.Length == 0)
			{
				foreach (string s in Usage.Split('\n'))
				{
					Main.NewText(s, Color.OrangeRed);
				}
				return;
			}

			Array.Resize(ref args, 2);
			if (caller.Player.whoAmI == Main.myPlayer && args[0] is string type)
			{
				type = type.ToLower();
				if (type == "npc" && args[1] is string name)
				{
					if (!int.TryParse(args[1], out int npcType))
					{
						string getName = args[1].Replace("_", " ");
						NPC npc = new();
						for (int k = NPCID.NegativeIDCount + 1; k < NPCLoader.NPCCount; k++)
						{
							npc.SetDefaults(k);
							if (getName != npc.GivenOrTypeName)
								continue;

							npcType = k;
							break;
						}

						if (npcType == 0)
						{
							Main.NewText($"{input} <-- Unknown NPC type!", Color.OrangeRed);
							throw new UsageException($"Unknown NPC: {name}");
						}
					}

					if (npcType <= 0 || npcType >= NPCLoader.NPCCount)
					{
						Main.NewText($"{input} <-- Unknown NPC ID!", Color.OrangeRed);
						throw new UsageException($"Unknown NPC ID: {npcType}");
					}
					OpenWikiPage(Mod, ContentSamples.NpcsByNetId[npcType]);
				}
				else if (type == "item" && args[1] is string name2)
				{
					name = name2;
					if (!int.TryParse(args[1], out int itemType))
					{
						string getName = args[1].Replace("_", " ");
						Item item = new();
						for (int k = 0; k < ItemLoader.ItemCount; k++)
						{
							item.SetDefaults(k, true);
							if (getName != item.Name)
								continue;

							itemType = k;
							break;
						}

						if (itemType == 0)
						{
							Main.NewText($"{input} <-- Unknown item type!", Color.OrangeRed);
							throw new UsageException($"Unknown item: {name}");
						}
					}

					if (itemType <= 0 || itemType >= ItemLoader.ItemCount)
					{
						Main.NewText($"{input} <-- Unknown item ID!", Color.OrangeRed);
						throw new UsageException($"Unknown item ID: {itemType}");
					}
					OpenWikiPage(Mod, ContentSamples.ItemsByType[itemType]);
				}
				else if (type == "npc" && args[1] is null)
				{
					Main.NewText($"{input} <-- Unknown NPC name!", Color.OrangeRed);
				}
				else if (type == "item" && args[1] is null)
				{
					Main.NewText($"{input} <-- Unknown item name!", Color.OrangeRed);
				}
				else
				{
					Main.NewText($"{input} <-- Unknown type! Available types: [c/{Color.Yellow.Hex3()}:npc], [c/{Color.Yellow.Hex3()}:item].", Color.OrangeRed);
					throw new UsageException("Unknown type: " + type);
				}
			}
		}
	}

	// crossplatform warning
	/*internal class WikithisPlayer : ModPlayer
	{
		public override void OnEnterWorld(Player player)
		{
			if (!Main.dedServ && !Platform.IsWindows)
				Main.NewText(Language.GetTextValue("Mods.Wikithis.UnsupportedWarning"), Color.OrangeRed);
		}
	}*/
}