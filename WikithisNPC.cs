//
//    Copyright 2023-2025 BasicallyIAmFox
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

using Terraria;
using Terraria.ModLoader;
using Wikithis.Wikis;

namespace Wikithis;

public sealed class WikithisNPC : GlobalNPC {
	public override bool PreHoverInteract(NPC npc, bool mouseIntersects) {
		if (WikithisConfig.Config.CanWikiNPCs && mouseIntersects && WikithisSystem.WikiKeybind.JustPressed && Wikithis.GetWiki<NPCWiki>().Entries.TryGetValue((short)npc.netID, out var entry)) {
			entry.OpenWikiPage(false);
		}

		return true;
	}
}
