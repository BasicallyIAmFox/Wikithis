﻿using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	[Autoload(false)]
	public sealed class SealedWiki : Wiki<object, IConvertible>
	{
		private readonly Action<Func<IConvertible, bool>, Action<object, IConvertible, string>, Func<string, Mod, string>> initialize;
		private readonly Action<IDictionary<(Mod, GameCulture.CultureName), string>, GameCulture.CultureName, object> noExists;
		private readonly string name;

		public override string Name => name;

		public SealedWiki(string name, Func<object, IConvertible> func, Action<Func<IConvertible, bool>, Action<object, IConvertible, string>, Func<string, Mod, string>> initialize, Action<IDictionary<(Mod, GameCulture.CultureName), string>, GameCulture.CultureName, object> noExists = null) : base(func)
		{
			this.name = name;
			this.initialize = initialize;
			this.noExists = noExists;
		}

		public override void Initialize() => initialize(x => HasEntry(x), (x, y, z) => AddEntry(x, new WikiEntry<IConvertible>(y, z)), (x, y) => Wikithis.DefaultSearchStr(x, y));

		public override void MessageIfDoesntExists(IConvertible key)
		{
			if (noExists != null)
			{
				IDictionary<(Mod, GameCulture.CultureName), string> mtu = Wikithis._modToURL;
				noExists(mtu, Wikithis.CultureLoaded, key);
				return;
			}
			base.MessageIfDoesntExists(key);
		}
	}
}