using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis
	{
		private static string _callMessageCache = null;

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
					"0",
					"AddModURL"
				};
				string[] second = new string[]
				{
					"1",
					"ItemIdReplacement",
					"ItemIdReplacements",
					"ItemIdsReplacement",
					"ItemIdsReplacements",
					"ReplaceItemId",
					"ReplaceItemIds"
				};
				string[] third = new string[]
				{
					"2",
					"NpcIdReplacement",
					"NpcIdReplacements",
					"NpcIdsReplacement",
					"NpcIdsReplacements",
					"ReplaceNpcId",
					"ReplaceNpcIds"
				};
				string[] fourth = new string[]
				{
					"3",
					"AddWikiTexture",
					"WikiTexture",
					"AddWiki"
				};
				string[] fifth = new string[]
				{
					"4",
					"CustomWiki"
				};
				string[] sixth = new string[]
				{
					"5",
					"OpenWiki",
					"OpenCustomWiki"
				};
				string[] seventh = new string[]
				{
					"6",
					"DelegateWiki"
				};

				_callMessageCache = messageOverload.HasValue ? messageOverload.Value.ToString() : message;

				if (first.Any(x => x.ToLower() == _callMessageCache))
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
					goto successReturn;
				}
				else if (second.Any(x => x.ToLower() == _callMessageCache))
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
					goto successReturn;
				}
				else if (third.Any(x => x.ToLower() == _callMessageCache))
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
					goto successReturn;
				}
				else if (fourth.Any(x => x.ToLower() == _callMessageCache))
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
					goto successReturn;
				}
				else if (fifth.Any(x => x.ToLower() == _callMessageCache))
				{
					Mod mod = args[index + 0] as Mod;
					string name = args[index + 1] as string;
					var key = args[index + 2] as Func<object, IConvertible>;
					var initialize = args[index + 3] as Action<Func<IConvertible, bool>, Action<object, IConvertible, string>, Func<string, Mod, string>>;
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
					goto successReturn;
				}
				else if (sixth.Any(x => x.ToLower() == _callMessageCache))
				{
					int num = args[index + 0] is Mod ? 1 : 0;

					string name;
					IConvertible key = args[index + 1 + num] as IConvertible;
					bool? withKeybind = args[index + 2 + num] as bool?;
					withKeybind ??= true;

					string nameOfArgument = string.Empty;

					if (args[index + 0] is Mod mod)
					{
						name = args[index + 1] as string;

						if (mod == null)
							nameOfArgument = nameof(mod);
						if (name == null)
							nameOfArgument = nameof(name);

						if (nameOfArgument != string.Empty)
							throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

						goto normalGoto;
					}
					else if (args[index + 0] is string fullname)
					{
						if (nameOfArgument != string.Empty)
							throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

						string[] split = fullname.Split('/');
						if (split is null || split.Length is <= 1 or >= 3)
							throw new ArgumentNullException($"Call Error: The {fullname} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null or length less or more than 2.");

						mod = ModLoader.GetMod(split[0]);
						name = split[1];

						goto normalGoto;
					}
					throw new NotImplementedException();

				normalGoto:
					bool? isItemOrNpc = null;
					if (mod.Name is "Wikithis")
						isItemOrNpc = name == "ItemWiki";

					if (DelegateWikis.TryGetValue(mod == null ? "Terraria" : mod.Name, out var delegates) && delegates.pageExists(name, key))
					{
						if (delegates.openPage(name, key))
						{
							goto tryOpenNormal;
						}
						goto successReturn;
					}

				tryOpenNormal:
					if (isItemOrNpc is not null)
					{
						if (key is not IConvertible key2)
							throw new NotImplementedException();

						int keyy = key2.ToInt32(null);

						if (isItemOrNpc is true)
							(Wikis["Wikithis/ItemWiki"] as IWiki<Item, int>).GetEntry(keyy).OpenWikiPage(withKeybind.Value);
						else
							(Wikis["Wikithis/NPCWiki"] as IWiki<NPC, int>).GetEntry(keyy).OpenWikiPage(withKeybind.Value);

						goto successReturn;
					}

					if (Wikis.TryGetValue(name, out IWiki value))
					{
						(value as IWiki<object, IConvertible>).GetEntry(key).OpenWikiPage(withKeybind.Value);
					}

					goto successReturn;
				}
				else if (seventh.Any(x => x.ToLower() == _callMessageCache))
				{
					string mod = args[index + 0] as string;
					var noWiki = args[index + 1] as Func<object, object, bool>;
					var action = args[index + 2] as Func<object, object, bool>;

					string nameOfArgument = string.Empty;
					if (mod == null)
						nameOfArgument = nameof(mod);
					if (noWiki == null)
						nameOfArgument = nameof(noWiki);
					if (action == null)
						nameOfArgument = nameof(action);

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					_delegateWikis.Add(mod, (noWiki, action));
					goto successReturn;
				}
				else if (messageOverload.HasValue && messageOverload.Value == 7)
				{
#if !DEBUG
					throw new NotImplementedException("You not supposed to use this call... yet.");
#endif
					goto successReturn;
				}
				else
				{
#pragma warning disable CA2208
					throw new ArgumentOutOfRangeException(nameof(messageOverload));
#pragma warning restore CA2208
				}

			successReturn:
				_callMessageCache = null;
				return success;
			}
			catch (Exception e)
			{
				Logger.Error($"Call Error: {e.StackTrace} {e.Message}");
			}

			return null;
		}
	}
}
