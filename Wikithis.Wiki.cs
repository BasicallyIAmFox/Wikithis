using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis
	{
		private static void SetLoadingText(string text)
		{
			var i = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.Interface")!.GetField("loadMods", BindingFlags.NonPublic | BindingFlags.Static)!;
			var p = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIProgress")!.GetProperty("SubProgressText", BindingFlags.Public | BindingFlags.Instance)!.GetSetMethod()!;

			p.Invoke(i.GetValue(null), new object[] { text });
		}

		private static string GetInternalName<Tid, TType>(TType id) where Tid : class where TType : struct, IComparable, IComparable<TType>, IConvertible, IEquatable<TType>, ISpanFormattable, IFormattable
		{
			FieldInfo field = typeof(Tid).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.GetValue(null).Equals(id)).FirstOrDefault();
			return field?.Name;
		}

		internal static void SetupWikiPages(Mod mod)
		{
			SetupWikiPages_Item(mod);
			SetupWikiPages_NPC(mod);
			SetLoadingText(string.Empty);
		}

		private static void SetupWikiPages_Item(Mod mod)
		{
			//Dictionary<Mod, ValueTuple<List<string>, int>> ModGotMax = new();

			for (int i = 1; i < ItemLoader.ItemCount; i++)
			{
				if (ContentSamples.ItemsByType.TryGetValue(i, out Item item))
				{
					if (ItemID.Sets.Deprecated[item.type])
						continue;
					if (item.ModItem?.Mod.Name == "ModLoader")
						continue;

					string n = Language.GetTextValue("ItemName." + GetInternalName<ItemID, short>((short)item.type));

					string name = item.type < ItemID.Count ? n : Language.GetTextValue($"Mods.{item.ModItem.Mod.Name}.ItemName.{item.ModItem.Name}");
					if (ItemIdNameReplace.TryGetValue((item.type, CultureLoaded), out string name2))
						name = name2;

					SetLoadingText(Language.GetTextValue($"Mods.{mod.Name}.LoadingItem", name));

					name = name.Replace(' ', '_');
					name = name.Replace("'", "%27");

					if (item.ModItem == null)
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

						ItemToURL.TryAdd(i, url);
						continue;
					}
					else
					{
						string url = string.Empty;
						bool success = false;

						GameCulture.CultureName culture = CultureLoaded;
						bool doesntContainsOthers = ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(item.ModItem.Mod, CultureLoaded), out _);
						if (!doesntContainsOthers)
							culture = GameCulture.CultureName.English;
						if (ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(item.ModItem.Mod, culture), out string value))
						{
							success = true;
							url = value;
						}

						if (!success)
							continue;

						string[] urls = url.Split('$');
						string[] urls2 = url.Split('%');
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
							ItemToURL.TryAdd(i, result);

							//ModGotMax.TryAdd(item.ModItem.Mod, (new(), item.ModItem.Mod.GetContent<ModItem>().Count()));
							//ModGotMax[item.ModItem.Mod].Item1.Add(item.Name);
						}
						else
						{
							//Mod.Logger.Info(Language.GetTextValue($"Mods.{Mod.Name}.CreationErrors.Item", Lang.GetItemNameValue(item.type)));
							continue;
						}
					}
				}
			}

			/*mod.Logger.Info("=== Analysis of wiki-linked items (START) ===");
			foreach (KeyValuePair<Mod, ValueTuple<List<string>, int>> modgotit in ModGotMax)
			{
				List<string> gottem = new();
				modgotit.Key.GetContent<ModItem>().ToList().ForEach(x => gottem.Add(x.Item.Name));
				foreach (string s in modgotit.Value.Item1)
					gottem.Remove(s);

				Console.WriteLine(modgotit.Key != null ? modgotit.Key.Name : "Terraria");
				Console.WriteLine($"{modgotit.Value.Item1.Count}/{modgotit.Value.Item2}");
				Console.WriteLine(gottem.Count > 0 ? string.Join(", ", gottem) : "Everything was succesfully wiki-linked!");
				Console.WriteLine(string.Empty);
			}
			mod.Logger.Info("=== Analysis of wiki-linked items (END) ===");*/
		}

		private static void SetupWikiPages_NPC(Mod mod)
		{
			for (int i = 1; i < NPCLoader.NPCCount; i++)
			{
				if (ContentSamples.NpcsByNetId.TryGetValue(i, out NPC npc))
				{
					if (npc.ModNPC?.Mod.Name == "ModLoader")
						continue;

					string n = Language.GetTextValue("NPCName." + GetInternalName<NPCID, short>((short)npc.netID));

					string name = npc.netID < NPCID.Count ? n : Language.GetTextValue($"Mods.{npc.ModNPC.Mod.Name}.NPCName.{npc.ModNPC.Name}");
					if (NpcIdNameReplace.TryGetValue((npc.netID, CultureLoaded), out string name2))
						name = name2;

					SetLoadingText(Language.GetTextValue($"Mods.{mod.Name}.LoadingNPC", name));

					name = name.Replace(' ', '_');
					name = name.Replace("'", "%27");

					if (npc.ModNPC == null)
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

						NPCToURL.TryAdd(i, url);
						continue;
					}
					else
					{
						string url = "";
						bool success = false;

						GameCulture.CultureName culture = CultureLoaded;
						bool doesntContainsOthers = ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(npc.ModNPC.Mod, CultureLoaded), out _);
						if (!doesntContainsOthers)
							culture = GameCulture.CultureName.English;
						if (ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(npc.ModNPC.Mod, culture), out string value))
						{
							success = true;
							url = value;
						}

						if (!success)
							continue;

						string[] urls = url.Split('$');
						string[] urls2 = url.Split('々');
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
							NPCToURL.TryAdd(i, result);
						}
						else
						{
							//Mod.Logger.Info(Language.GetTextValue($"Mods.{Mod.Name}.CreationErrors.NPC", Lang.GetNPCNameValue(npc.netID)));
							continue;
						}
					}
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

				bool bl = ModToURL.ContainsKey(new ValueTuple<Mod, GameCulture.CultureName>(item.ModItem?.Mod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey(new ValueTuple<Mod, GameCulture.CultureName>(item.ModItem?.Mod, GameCulture.CultureName.English));

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

				bool bl = ModToURL.ContainsKey(new ValueTuple<Mod, GameCulture.CultureName>(npc.ModNPC?.Mod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey(new ValueTuple<Mod, GameCulture.CultureName>(npc.ModNPC?.Mod, GameCulture.CultureName.English));

				mod.Logger.Error("Tried to get wiki page, but failed!");
				mod.Logger.Info("Type: " + npc.type.ToString());
				mod.Logger.Info("Name: " + npc.GivenOrTypeName);
				mod.Logger.Info("Vanilla: " + (npc.ModNPC == null).ToString());
				mod.Logger.Info("Mod: " + npc.ModNPC?.Mod.Name);
				mod.Logger.Info("Domain in dictionary: " + (npc.ModNPC != null ? bl.ToString() : "False"));
			}
		}
	}
}