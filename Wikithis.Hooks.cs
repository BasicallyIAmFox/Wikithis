using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Wikithis.Wikis;

namespace Wikithis;

public partial class Wikithis {
	// IL Diff
	#region 1.4.3
	/*
		C#:
			// Goto #1
			NPCLoader.ModifyHoverBoundingBox(npc[k], ref boundingBox);
			bool flag = mouseRectangle.Intersects(value);
			bool flag2 = flag || (SmartInteractShowingGenuine && SmartInteractNPC == k);
			// Goto #2
			if (flag2 && ((npc[k].type != 85 && npc[k].type != 341 && npc[k].type != 629 && npc[k].aiStyle != 87) || npc[k].ai[0] != 0f) && npc[k].type != 488)
			...
		[+] FastDelegateInvokers.InvokeVoidVal2<NPC, bool>(nPC, flag2, DynamicReferenceManager.GetValueTUnsafe<Delegate>(17, 7050241));
			// Goto #3
			bool flag3 = SmartInteractShowingGenuine && SmartInteractNPC == k;
		IL:
			// Goto #1
			IL_055a: ldsfld class Terraria.NPC[] Terraria.Main::npc
			IL_055f: ldloc.s 10
			IL_0561: ldelem.ref
			IL_0562: ldloca.s 12
			IL_0564: call void Terraria.ModLoader.NPCLoader::ModifyHoverBoundingBox(class Terraria.NPC, valuetype [FNA]Microsoft.Xna.Framework.Rectangle&)

			IL_0569: ldloca.s 0
			IL_056b: ldloc.s 12
			IL_056d: call instance bool [FNA]Microsoft.Xna.Framework.Rectangle::Intersects(valuetype [FNA]Microsoft.Xna.Framework.Rectangle)
			IL_0572: stloc.s 13

			IL_0574: ldloc.s 13
			IL_0576: brtrue.s IL_058d
			IL_0578: ldsfld bool Terraria.Main::SmartInteractShowingGenuine
			IL_057d: brfalse.s IL_058a
			IL_057f: ldsfld int32 Terraria.Main::SmartInteractNPC
			IL_0584: ldloc.s 10
			IL_0586: ceq
			IL_0588: br.s IL_058e
			// (no C# code)
			IL_058a: ldc.i4.0
			IL_058b: br.s IL_058e

			// Goto #2
			IL_058d: ldc.i4.1
			IL_058e: stloc.s 14
			IL_0590: ldloc.s 14
			IL_0592: brfalse IL_0a28

			...

		[+] IL_0000: ldloc V_2
		[+] IL_0000: ldloc V_6
		[+] IL_0000: ldc.i4 17
		[+] IL_0000: ldc.i4 7050241
		[+] IL_0000: call T MonoMod.Utils.DynamicReferenceManager::GetValueTUnsafe<System.Delegate>(System.Int32,System.Int32)
		[+] IL_0000: call System.Void MonoMod.Cil.FastDelegateInvokers::InvokeVoidVal2<Terraria.NPC,System.Boolean>(T0,T1,MonoMod.Cil.FastDelegateInvokers/VoidVal2`2<T0,T1>)
	
			// Goto #3
			IL_0611: ldsfld bool Terraria.Main::SmartInteractShowingGenuine
			IL_0616: brfalse.s IL_0623
			IL_0618: ldsfld int32 Terraria.Main::SmartInteractNPC
			IL_061d: ldloc.s 10
			IL_061f: ceq
			IL_0621: br.s IL_0624
	 */
	#endregion
	#region 1.4.4
	/*
		C#:
			// Goto #1
			NPCLoader.ModifyHoverBoundingBox(nPC, ref boundingBox);
			bool flag = mouseRectangle.Intersects(value);
			bool flag2 = flag || (SmartInteractShowingGenuine && SmartInteractNPC == i);
			// Goto #2
			if (flag2 && ((nPC.type != 85 && nPC.type != 341 && nPC.type != 629 && nPC.aiStyle != 87) || nPC.ai[0] != 0f) && nPC.type != 488)
			...
		[+] FastDelegateInvokers.InvokeVoidVal2<NPC, bool>(nPC, flag2, DynamicReferenceManager.GetValueTUnsafe<Delegate>(17, 7050241));
			// Goto #3
			bool flag3 = SmartInteractShowingGenuine && SmartInteractNPC == i;
		IL:
			// Goto #1
			IL_012d: ldloc.2
			IL_012e: ldloca.s 4
			IL_0130: call void Terraria.ModLoader.NPCLoader::ModifyHoverBoundingBox(class Terraria.NPC, valuetype [FNA]Microsoft.Xna.Framework.Rectangle&)

			IL_0135: ldarga.s mouseRectangle
			IL_0137: ldloc.s 4
			IL_0139: call instance bool [FNA]Microsoft.Xna.Framework.Rectangle::Intersects(valuetype [FNA]Microsoft.Xna.Framework.Rectangle)
			IL_013e: stloc.s 5

			IL_0140: ldloc.s 5
			IL_0142: brtrue.s IL_0158
			IL_0144: ldsfld bool Terraria.Main::SmartInteractShowingGenuine
			IL_0149: brfalse.s IL_0155
			IL_014b: ldsfld int32 Terraria.Main::SmartInteractNPC
			IL_0150: ldloc.1
			IL_0151: ceq
			IL_0153: br.s IL_0159
			// (no C# code)
			IL_0155: ldc.i4.0
			IL_0156: br.s IL_0159

			// Goto #2
			IL_0158: ldc.i4.1
			IL_0159: stloc.s 6
			IL_015b: ldloc.s 6
			IL_015d: brfalse IL_054d

			...

			IL_025b: br IL_054d

		[+] IL_0000: ldloc V_2
		[+] IL_0000: ldloc V_6
		[+] IL_0000: ldc.i4 17
		[+] IL_0000: ldc.i4 7050241
		[+] IL_0000: call T MonoMod.Utils.DynamicReferenceManager::GetValueTUnsafe<System.Delegate>(System.Int32,System.Int32)
		[+] IL_0000: call System.Void MonoMod.Cil.FastDelegateInvokers::InvokeVoidVal2<Terraria.NPC,System.Boolean>(T0,T1,MonoMod.Cil.FastDelegateInvokers/VoidVal2`2<T0,T1>)
	
			// Goto #3
			IL_0260: ldsfld bool Terraria.Main::SmartInteractShowingGenuine
			IL_0265: brfalse.s IL_0271
			IL_0267: ldsfld int32 Terraria.Main::SmartInteractNPC
			IL_026c: ldloc.1
			IL_026d: ceq
			IL_026f: br.s IL_0272
	 */
	#endregion
	private static void NPCURL(ILContext il) {
		ILCursor c = new(il);
		try {
			int npcIndex = 0;
			int hovers = 0;

			// Goto #1
			c.GotoNext(i => i.MatchLdloc(out npcIndex),
#if TML_2022_09
				i => i.MatchLdelemRef(),
#endif
				i => i.MatchLdloca(out _),
				i => i.MatchCall(typeof(NPCLoader).GetMethod(nameof(NPCLoader.ModifyHoverBoundingBox), BindingFlags.Public | BindingFlags.Static)));

			// Goto #2
			c.GotoNext(MoveType.After,
				i => i.MatchLdcI4(1),
				i => i.MatchStloc(out hovers),
				i => i.MatchLdloc(hovers),
				i => i.MatchBrfalse(out _));

			// Goto #3
			c.GotoNext(i => i.MatchLdsfld<Main>(nameof(Main.SmartInteractShowingGenuine)),
				i => i.MatchBrfalse(out _));

#if TML_2022_09
			c.Emit(OpCodes.Ldsfld, typeof(Main).GetField(nameof(Main.npc), BindingFlags.Public | BindingFlags.Static));
			c.Emit(OpCodes.Ldloc, npcIndex);
			c.Emit(OpCodes.Ldelem_Ref);
			c.Emit(OpCodes.Ldloc, hovers);
#else
			c.Emit(OpCodes.Ldloc, npcIndex);
			c.Emit(OpCodes.Ldloc, hovers);
#endif
			c.EmitDelegate<Action<NPC, bool>>((npc, hovers) => {
				if (WikithisConfig.Config.CanWikiNPCs && hovers && WikithisSystem.WikiKeybind.JustPressed && GetWiki<NPCWiki>().Entries.TryGetValue((short)npc.netID, out var entry)) {
					entry.OpenWikiPage(false);
				}
			});
		}
#if TML_2022_09
		catch (Exception exception) {
			Instance.Logger.Error($"Failed to patch {il.Body.Method.FullName}. Stack trace: {exception.Message}");
		}
#else
		catch {
			MonoModHooks.DumpIL(Instance, il);
		}
#endif
	}
}
