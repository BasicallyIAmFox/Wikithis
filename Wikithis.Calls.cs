using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis
	{
		public static void AddModURL(Mod mod, string domain, GameCulture.CultureName? culture)
		{
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
		}

		public static void ReplaceItemIds(string name, GameCulture.CultureName? culture, params int[] ids)
		{
			if (ids.Length == 1)
			{
				_itemIdNameReplace.TryAdd((ids[0], culture.Value), name);
			}
			else
			{
				foreach (int i in ids)
				{
					_itemIdNameReplace.TryAdd((i, culture.Value), name);
				}
			}
		}

		public static void ReplaceNpcIds(string name, GameCulture.CultureName? culture, params int[] ids)
		{
			if (ids.Length == 1)
			{
				_npcIdNameReplace.TryAdd((ids[0], culture.Value), name);
			}
			else
			{
				foreach (int i in ids)
				{
					_npcIdNameReplace.TryAdd((i, culture.Value), name);
				}
			}
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
				string[] seventh = new string[]
				{
					"DelegateWiki"
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

					AddModURL(mod, domain, culture);
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

					ReplaceItemIds(name, culture, id.ToArray());
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

					ReplaceNpcIds(name, culture, id.ToArray());
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
					if (DelegateWikis.TryGetValue(mod, out var delegates) && delegates.pageExists(name))
					{
						delegates.openPage(name);
					}
					else if (Wikis.TryGetValue($"{mod.Name}/{name}", out IWiki value))
					{
						(value as IWiki<object, object>).GetEntry(key).OpenWikiPage(withKeybind.Value);
					}
					return success;
				}
				else if (seventh.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 6)
				{
					Mod mod = args[index + 0] as Mod;
					Func<object, bool> noWiki = args[index + 1] as Func<object, bool>;
					Action<object> action = args[index + 2] as Action<object>;

					string nameOfArgument = string.Empty;
					if (mod == null)
						nameOfArgument = nameof(mod);
					if (noWiki == null)
						nameOfArgument = nameof(noWiki);
					if (action == null)
						nameOfArgument = nameof(action);

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					DelegateWikis.Add(mod, (noWiki, action));
					return success;
				}
				else if (messageOverload.HasValue && messageOverload.Value == 7)
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
	}
}
