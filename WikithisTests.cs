//
//    Copyright 2023-2024 BasicallyIAmFox
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

#if DEBUG
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.ExceptionServices;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis;

internal static class WikithisTests {
	private sealed class TestManager {
		private readonly ImmutableList<AbstractTest>.Builder _values;

		public TestManager() {
			_values = ImmutableList.CreateBuilder<AbstractTest>();
		}

		public void Add(AbstractTest test) {
			test.Id = _values.Count;
			_values.Add(test);
		}

		public void RunTests() {
			int success = 0;
			int total = 0;

			_values.ToImmutable().ForEach(i => {
				try {
					if (i.Run() == AbstractTest.SuccessValue)
						success++;
				}
				catch {
					// ignored
				}

				total++;
			});

			Wikithis.Instance.Logger.Info($"{success}/{total} tests succeeded!");
		}
	}
	private abstract class AbstractTest {
		public const int SuccessValue = 0;
		public const int FailureValue = 1;

		protected Action Action { get; }

		protected AbstractTest(Action action) {
			Action = action;
		}

		public int Id { get; set; }

		public abstract int Run();

		protected void Fail() {
			Wikithis.Instance.Logger.Error($"Failed test #{Id + 1}!");
		}
	}
	private sealed class SuccessfulTest : AbstractTest {
		public SuccessfulTest(Action action) : base(action) {
		}

		public override int Run() {
			try {
				Action();
				return SuccessValue;
			}
			catch {
				Fail();
			}

			return FailureValue;
		}
	}
	private sealed class ThrowExceptionTest : AbstractTest {
		private Type ExceptionType { get; }

		public ThrowExceptionTest(Action action, Type exceptionType = null) : base(action) {
			ExceptionType = exceptionType;
		}

		public override int Run() {
			try {
				Action();
				goto Fail;
			}
			catch (Exception e) {
				if (ExceptionType != null && e.GetType() != ExceptionType) {
					goto Fail;
				}
			}

			return SuccessValue;

		Fail:
			Fail();
			return FailureValue;
		}
	}

	public static void TestModCalls() {
		const string divider = "==========";

		using var disableFirstChanceExceptions = new Hook(
			typeof(Logging).FindMethod("FirstChanceExceptionHandler")!,
			(Action<object, FirstChanceExceptionEventArgs> _, object _, FirstChanceExceptionEventArgs _) => {
			}
		);
		disableFirstChanceExceptions.Apply();

		if (ModLoader.TryGetMod("Wikithis", out var mod)) {
			TestAddModUrl();
			TestReplaceIDs();
			TestAddWikiTexture();
			mod.Logger.Info(divider);
		}
		else {
			Wikithis.Instance.Logger.Error("Failed to run all tests somehow. How did this happened?");
		}

		disableFirstChanceExceptions.Undo();

		return;

		void TestAddModUrl() {
			const string url = "https://terrariamods.wiki.gg/wiki/Confection_Rebaked";

			var manager = new TestManager();
			manager.Add(new ThrowExceptionTest(() => mod.Call("0", mod))); // Too little args
			manager.Add(new ThrowExceptionTest(() => mod.Call("0", mod, url, GameCulture.CultureName.English, null))); // Too many args
			manager.Add(new ThrowExceptionTest(() => mod.Call("0", "mod", url))); // 2nd argument supposed to be Mod instance
			manager.Add(new ThrowExceptionTest(() => mod.Call("0", mod, 545))); // 3rd argument supposed to be a string aka wiki URL
			manager.Add(new ThrowExceptionTest(() => mod.Call("0", mod, url, GameCulture.CultureName.English))); // Cannot have English as language
			manager.Add(new SuccessfulTest(() => mod.Call("0", mod, url, GameCulture.CultureName.Polish)));
			manager.Add(new SuccessfulTest(() => mod.Call("0", mod, url)));

			mod.Logger.Info(divider);
			mod.Logger.Info("Testing AddModUrl mod call");
			mod.Logger.Info(divider);
			manager.RunTests();
		}

		void TestReplaceIDs() {
			const string url = "https://terrariamods.wiki.gg/wiki/Confection_Rebaked/Roller_Cookie";

			var manager = new TestManager();
			for (int i = 1; i <= 2; i++) {
				int capturedI = i;
				manager.Add(new ThrowExceptionTest(() => mod.Call(capturedI, 1))); // Too little args
				manager.Add(new ThrowExceptionTest(() => mod.Call(capturedI, 1, url, GameCulture.CultureName.English, null))); // Too many args
				manager.Add(new ThrowExceptionTest(() => mod.Call(capturedI, "1", url))); // 2nd argument supposed to be an Identifier
				manager.Add(new ThrowExceptionTest(() => mod.Call(capturedI, 1, new List<string> { url }))); // 3rd argument supposed to be a string aka wiki page URL
				manager.Add(new ThrowExceptionTest(() => mod.Call(capturedI, new List<int>(), new List<string> { url }))); // Lengths of collections are supposed to be same
				manager.Add(new SuccessfulTest(() => mod.Call(capturedI, 1, url)));
				manager.Add(new SuccessfulTest(() => mod.Call(capturedI, new List<int> { 1 }, url)));
				manager.Add(new SuccessfulTest(() => mod.Call(capturedI, new List<int> { 1 }, new List<string> { url })));
			}

			mod.Logger.Info(divider);
			mod.Logger.Info("Testing ReplaceItemID and ReplaceNPCId mod calls");
			mod.Logger.Info(divider);
			manager.RunTests();
		}

		void TestAddWikiTexture() {
			var manager = new TestManager();
			manager.Add(new ThrowExceptionTest(() => mod.Call("3"))); // There should be atleast 3 arguments
			manager.Add(new ThrowExceptionTest(() => mod.Call("3", "mod"))); // 2nd argument supposed to be Mod instance
			manager.Add(new ThrowExceptionTest(() => mod.Call("3", mod, ModContent.Request<Texture2D>("Wikithis/icon").Value))); // 3rd argument supposed to be Asset<Texture2D>
			manager.Add(new SuccessfulTest(() => mod.Call("3", mod, ModContent.Request<Texture2D>("Wikithis/icon"))));

			mod.Logger.Info(divider);
			mod.Logger.Info("Testing AddWikiTexture mod call");
			mod.Logger.Info(divider);
			manager.RunTests();
		}
	}
}
#endif
