using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis
	{
		private static FieldInfo _loadMods;
		private static MethodInfo _subProgressText;

		private static void SetLoadingText(string text) => _subProgressText.Invoke(_loadMods.GetValue(null), new object[] { text });

		private static string GetInternalName(int id, int num = 0)
		{
			if (num == 1) return NPCID.Search.GetName(id);
			else if (num == 2) return TileID.Search.GetName(id);
			else if (num < 0 && num >= 3) throw new NotImplementedException();
			return ItemID.Search.GetName(id);
		}

		internal static void SetupWikiPages(Mod mod)
		{
			_loadMods = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.Interface")!.GetField("loadMods", BindingFlags.NonPublic | BindingFlags.Static)!;
			_subProgressText = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIProgress")!.GetProperty("SubProgressText", BindingFlags.Public | BindingFlags.Instance)!.GetSetMethod()!;

			Stopwatch sw = new();
			sw.Start();

			SetupWikiPages_Item(mod);
			SetupWikiPages_NPC(mod);
			SetupWikiPages_Tiles(mod);

			sw.Stop();
			Console.WriteLine(sw.Elapsed.TotalSeconds);

			SetLoadingText(string.Empty);

			_loadMods = null;
			_subProgressText = null;
		}

		private static void Construct(int i, string name, IModType type, Dictionary<int, string> dictionary)
		{
			name = name.Replace(' ', '_');
			name = name.Replace("'", "%27");

			if (type == null)
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

				dictionary.TryAdd(i, url);
			}
			else
			{
				string url = string.Empty;
				bool success = false;

				GameCulture.CultureName culture = CultureLoaded;

				bool doesntContainsOthers = ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(type.Mod, culture), out _);
				if (!doesntContainsOthers)
					culture = GameCulture.CultureName.English;

				if (ModToURL.TryGetValue((type.Mod, culture), out string value))
				{
					success = true;
					url = value;
				}

				if (!success)
					return;

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

				if (CheckURLValid(result))
				{
					dictionary.TryAdd(i, result);
				}
			}
		}

		private static void SetupWikiPages_Item(Mod mod)
		{
			for (int i = 1; i < ItemLoader.ItemCount; i++)
			{
				if (ContentSamples.ItemsByType.TryGetValue(i, out Item item))
				{
					if (ItemID.Sets.Deprecated[item.type] || item.ModItem?.Mod.Name == "ModLoader")
						continue;

					string n = Language.GetTextValue("ItemName." + GetInternalName(item.type));

					string name = item.type < ItemID.Count ? n : Language.GetTextValue($"Mods.{item.ModItem.Mod.Name}.ItemName.{item.ModItem.Name}");
					if (ItemIdNameReplace.TryGetValue((item.type, CultureLoaded), out string name2))
						name = name2;

					SetLoadingText(Language.GetTextValue($"Mods.{mod.Name}.LoadingItem", name));

					Construct(i, name, item.ModItem, ItemToURL);
				}
			}
		}

		private static void SetupWikiPages_NPC(Mod mod)
		{
			for (int i = 1; i < NPCLoader.NPCCount; i++)
			{
				if (ContentSamples.NpcsByNetId.TryGetValue(i, out NPC npc))
				{
					if (npc.ModNPC?.Mod.Name == "ModLoader")
						continue;

					string n = Language.GetTextValue("NPCName." + GetInternalName(npc.netID, 1));

					string name = npc.netID < NPCID.Count ? n : Language.GetTextValue($"Mods.{npc.ModNPC.Mod.Name}.NPCName.{npc.ModNPC.Name}");
					if (NpcIdNameReplace.TryGetValue((npc.netID, CultureLoaded), out string name2))
						name = name2;

					SetLoadingText(Language.GetTextValue($"Mods.{mod.Name}.LoadingNPC", name));

					Construct(i, name, npc.ModNPC, NPCToURL);
				}
			}
		}

		private static void SetupWikiPages_Tiles(Mod mod)
		{
			for (int i = 0; i < TileLoader.TileCount; i++)
			{
				string n = Lang._mapLegendCache[MapHelper.TileToLookup(i, 0)].Value;
				if (TileIdNameReplace.TryGetValue((i, CultureLoaded), out string name2))
					n = name2;

				if (n == string.Empty)
					continue;

				SetLoadingText(Language.GetTextValue($"Mods.{mod.Name}.LoadingItem", n));
				
				Construct(i, n, ModContent.GetModTile(i), TileToURL);
			}

			for (int i = 1; i < ItemLoader.ItemCount; i++)
			{
				if (ContentSamples.ItemsByType.TryGetValue(i, out Item item))
				{
					if (ItemID.Sets.Deprecated[item.type] || item.createTile == -1 || TileToURL.ContainsKey(item.createTile) || item.ModItem?.Mod.Name == "ModLoader")
						continue;

					string n = Language.GetTextValue("ItemName." + GetInternalName(item.type));
					if (ItemIdNameReplace.TryGetValue((item.type, CultureLoaded), out string name2))
						n = name2;

					string n2 = Language.GetTextValue($"Mods.{item.ModItem?.Mod.Name}.ItemName.{item.ModItem?.Name}");
					if (Lang.GetMapObjectName(MapHelper.TileToLookup(item.createTile, 0)) != string.Empty)
						n2 = Lang.GetMapObjectName(MapHelper.TileToLookup(item.createTile, 0));

					string name = item.type < ItemID.Count ? n : n2;

					SetLoadingText(Language.GetTextValue($"Mods.{mod.Name}.LoadingItem", name));

					Construct(item.createTile, name, item.ModItem, TileToURL);
				}
			}
		}

		internal static void OpenWikiPage(Mod mod, Item item)
		{
			if (ItemToURL.ContainsKey(item.type) && CheckURLValid(ItemToURL[item.type]))
			{
				Utils.OpenToURL(AprilFools && !WikithisSystem.RickRolled ? RickRoll : ItemToURL[item.type]);
				if (AprilFools && !WikithisSystem.RickRolled)
					WikithisSystem.RickRolled = true;
			}
			else
			{
				Main.NewText(Language.GetTextValue($"Mods.{mod.Name}.Error"), Color.OrangeRed);

				bool bl = ModToURL.ContainsKey((item.ModItem?.Mod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey((item.ModItem?.Mod, GameCulture.CultureName.English));

				mod.Logger.Error("Tried to get wiki page, but failed!");
				mod.Logger.Info("Type: " + item.type.ToString());
				mod.Logger.Info("Name: " + item.Name);
				mod.Logger.Info("Vanilla: " + (item.ModItem == null).ToString());
				mod.Logger.Info("Mod: " + item.ModItem?.Mod.Name);
				mod.Logger.Info("Domain in dictionary: " + (item.ModItem != null ? bl.ToString() : "False"));
			}
		}

		internal static void OpenWikiPage(Mod mod, NPC npc)
		{
			if (NPCToURL.ContainsKey(npc.type) && CheckURLValid(NPCToURL[npc.type]))
			{
				Utils.OpenToURL(AprilFools && !WikithisSystem.RickRolled ? RickRoll : NPCToURL[npc.type]);
				if (AprilFools && !WikithisSystem.RickRolled)
					WikithisSystem.RickRolled = true;
			}
			else
			{
				Main.NewText(Language.GetTextValue($"Mods.{mod.Name}.Error"), Color.OrangeRed);

				bool bl = ModToURL.ContainsKey((npc.ModNPC?.Mod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey((npc.ModNPC?.Mod, GameCulture.CultureName.English));

				mod.Logger.Error("Tried to get wiki page, but failed!");
				mod.Logger.Info("Type: " + npc.type.ToString());
				mod.Logger.Info("Name: " + npc.GivenOrTypeName);
				mod.Logger.Info("Vanilla: " + (npc.ModNPC == null).ToString());
				mod.Logger.Info("Mod: " + npc.ModNPC?.Mod.Name);
				mod.Logger.Info("Domain in dictionary: " + (npc.ModNPC != null ? bl.ToString() : "False"));
			}
		}

		internal static void OpenWikiPage(Mod mod, ushort type)
		{
			if (TileToURL.ContainsKey(type) && CheckURLValid(TileToURL[type]))
			{
				Utils.OpenToURL(AprilFools && !WikithisSystem.RickRolled ? RickRoll : TileToURL[type]);
				if (AprilFools && !WikithisSystem.RickRolled)
					WikithisSystem.RickRolled = true;
			}
			else
			{
				Main.NewText(Language.GetTextValue($"Mods.{mod.Name}.Error"), Color.OrangeRed);

				Mod tileMod = ModContent.GetModTile(type)?.Mod;
				bool bl = ModToURL.ContainsKey((tileMod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey((tileMod, GameCulture.CultureName.English));

				mod.Logger.Error("Tried to get wiki page, but failed!");
				mod.Logger.Info("Type: " + type.ToString());
				mod.Logger.Info("Vanilla: " + (tileMod == null).ToString());
				mod.Logger.Info("Mod: " + tileMod?.Name);
				mod.Logger.Info("Domain in dictionary: " + (tileMod != null ? bl.ToString() : "False"));
			}
		}
	}
}