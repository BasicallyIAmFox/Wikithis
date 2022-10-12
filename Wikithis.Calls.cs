using CCLiar;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis
	{
		private static string _callMessageCache;

		[Obsolete("Use Calls.AddModUrlCall.Call(...) instead.")]
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

			if (!WikiUrlRegex.IsMatch(domain))
			{
				Instance.Logger.Info($"'{mod.Name}' doesn't matches new format! Recommended to switch to new format. (Culture: {culture.Value})");
			}
		}

		[Obsolete("Use Calls.ReplaceItemIdsCall.Call(...) instead.")]
		public static void ReplaceItemIds(string name, GameCulture.CultureName? culture, params int[] ids)
		{
			if (ids.Length == 1)
			{
				//_itemIdNameReplace.TryAdd((ids[0], culture.Value), name);
			}
			else
			{
				foreach (int i in ids)
				{
					//_itemIdNameReplace.TryAdd((i, culture.Value), name);
				}
			}
		}

		[Obsolete("Use Calls.ReplaceNpcIdsCall.Call(...) instead.")]
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

		internal static bool GotoSuccessReturn()
		{
			_callMessageCache = null;
			return true;
		}

		public override object Call(params object[] args)
		{
			if (Main.dedServ)
				return "Got called on server-side!";

			try
			{
				string message = (args[0] as string)?.ToLower();
				int? messageOverload = args[0] as int?;
				const int index = 1;

				_callMessageCache = messageOverload.HasValue ? messageOverload.Value.ToString() : message;

				CCList list = CCList<Wikithis>.GetList(_callMessageCache);
				if (list == null)
					throw new ArgumentOutOfRangeException($"Key '{_callMessageCache}' is invalid! Make sure you haven't done a typo!");
				return list.Build(args[index..]);
			}
			catch (Exception e)
			{
				Logger.Error($"Call Error: {e.StackTrace} {e.Message}");
			}

			return null;
		}
	}
}
