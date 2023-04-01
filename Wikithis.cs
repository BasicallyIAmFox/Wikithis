using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Wikithis.Wikis;

namespace Wikithis;

public partial class Wikithis : Mod {
	internal static Regex WikiUrlRegex = new(@".*\/\{.*\}.*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	internal static Regex WikiStrRegex = new(@"\{.*\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static GameCulture.CultureName CultureLoaded { get; private set; }

	private static Wikithis instance;
	public static Wikithis Instance { get => instance; private set => instance = value; }

	public Wikithis() {
		Instance = this;
	}

	public override void Load() {
		CultureLoaded = (Language.ActiveCulture.Name == "en-US") ? GameCulture.CultureName.English :
			((Language.ActiveCulture.Name == "de-DE") ? GameCulture.CultureName.German :
			((Language.ActiveCulture.Name == "es-ES") ? GameCulture.CultureName.Spanish :
			((Language.ActiveCulture.Name == "fr-FR") ? GameCulture.CultureName.French :
			((Language.ActiveCulture.Name == "it-IT") ? GameCulture.CultureName.Italian :
			((Language.ActiveCulture.Name == "pl-PL") ? GameCulture.CultureName.Polish :
			((Language.ActiveCulture.Name == "pt-BR") ? GameCulture.CultureName.Portuguese :
			((Language.ActiveCulture.Name == "ru-RU") ? GameCulture.CultureName.Russian :
			((Language.ActiveCulture.Name == "zh-Hans") ? GameCulture.CultureName.Chinese : GameCulture.CultureName.English))))))));

		if (Main.dedServ) {
			return;
		}

		IL_Main.HoverOverNPCs += NPCURL;
	}

	internal static void SetupWikiPages() {
		if (Main.dedServ) {
			return;
		}

		Task.Run(() => {
			foreach (IWiki wiki in wikis) {
				wiki.Initialize();
			}
		});
	}

	public override void Unload() {
		itemReplacements.Clear();
		itemReplacements = null;

		npcReplacements.Clear();
		npcReplacements = null;

		dataForMods.Clear();
		dataForMods = null;

		wikis.Clear();
		wikis = null;

		CultureLoaded = 0;
		_callMessageCache = null;

		if (Main.dedServ) {
			return;
		}

		IL_Main.HoverOverNPCs -= NPCURL;
		Instance = null;
	}

	public static T GetWiki<T>() where T : class, IWiki => ModContent.GetInstance<T>();
}
