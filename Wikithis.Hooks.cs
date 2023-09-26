//
//    Copyright 2023 BasicallyIAmFox
//
//    Licensed under the Apache License, Version 2.0 (the "License")
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Wikithis.Wikis;

namespace Wikithis;

partial class Wikithis {
	#region 1.4.4
	/*
		// Goto #1
		NPCLoader.ModifyHoverBoundingBox(nPC, ref boundingBox);
		bool flag = mouseRectangle.Intersects(value);
		bool flag2 = flag || (SmartInteractShowingGenuine && SmartInteractNPC == i);
		// Goto #2
		if (flag2 && ((nPC.type != 85 && nPC.type != 341 && nPC.type != 629 && nPC.aiStyle != 87) || nPC.ai[0] != 0f) && nPC.type != 488)
		...
	[+] // Our code
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

	[+] IL_0000: // Our code
	
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

			c.Emit(OpCodes.Ldloc, npcIndex);
			c.Emit(OpCodes.Ldloc, hovers);
			c.EmitDelegate<Action<NPC, bool>>((npc, hovers) => {
				if (WikithisConfig.Config.CanWikiNPCs && hovers && WikithisSystem.WikiKeybind.JustPressed && GetWiki<NPCWiki>().Entries.TryGetValue((short)npc.netID, out var entry)) {
					entry.OpenWikiPage(false);
				}
			});
		}
		catch {
			MonoModHooks.DumpIL(Instance, il);
		}
	}
}
