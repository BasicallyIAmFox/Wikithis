using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis
	{
		private void NPCURL(ILContext il)
		{
			ILCursor c = new(il);
			try
			{
				// OLD IL
				/*
				c.GotoNext(i => i.MatchCall(typeof(NPCLoader).GetMethod(nameof(NPCLoader.ModifyHoverBoundingBox), BindingFlags.Public | BindingFlags.Static)))
					.GotoNext(MoveType.After, i => i.MatchStloc(14))
					.Emit(OpCodes.Ldsfld, typeof(Main).GetField(nameof(Main.npc)))
					.Emit(OpCodes.Ldloc, 10)
					.Emit(OpCodes.Ldelem_Ref)
					.Emit(OpCodes.Ldloc, 14);
				c.EmitDelegate<Action<NPC, bool>>((npc, hovers) =>
				{
					if (WikithisConfig.Config.CanWikiNPCs && hovers && WikithisSystem.WikiKeybind.JustPressed)
					{
						OpenWikiPage(npc, false);
					}
				});
				*/

				int npcIndex = 0;
				int hovers = 0;

				c.GotoNext(i => i.MatchLdloc(out npcIndex),
					i => i.MatchLdelemRef(),
					i => i.MatchLdloca(out _),
					i => i.MatchCall(typeof(NPCLoader).GetMethod(nameof(NPCLoader.ModifyHoverBoundingBox), BindingFlags.Public | BindingFlags.Static)));

				c.GotoNext(MoveType.After,
					i => i.MatchLdcI4(1),
					i => i.MatchStloc(out hovers),
					i => i.MatchLdloc(hovers),
					i => i.MatchBrfalse(out _));

				c.GotoNext(i => i.MatchLdsfld<Main>(nameof(Main.SmartInteractShowingGenuine)),
					i => i.MatchBrfalse(out _));

				c.Emit(OpCodes.Ldsfld, typeof(Main).GetField(nameof(Main.npc), BindingFlags.Public | BindingFlags.Static));
				c.Emit(OpCodes.Ldloc, npcIndex);
				c.Emit(OpCodes.Ldelem_Ref);
				c.Emit(OpCodes.Ldloc, hovers);
				c.EmitDelegate<Action<NPC, bool>>((npc, hovers) =>
				{
					if (WikithisConfig.Config.CanWikiNPCs && hovers && WikithisSystem.WikiKeybind.JustPressed)
					{
						OpenWikiPage(npc, false);
					}
				});
			}
			catch (Exception e)
			{
				Logger.Error($"IL Error: {e.Message} {e.StackTrace}");
			}
		}
	}
}
