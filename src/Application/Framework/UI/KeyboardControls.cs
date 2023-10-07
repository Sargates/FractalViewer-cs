using Raylib_cs;

namespace FractalViewer.Core;

public static class KeyboardControls {

	private static int KeyModifiers; // Left/Right [Ctrl, Shift, Alt, Super]
	private static List<KeyboardKey> KeysPressed = new List<KeyboardKey>(); // Left/Right [Ctrl, Shift, Alt, Super]
	// private static Dictionary<(KeyboardKey, int), Action<float>> binds = new Dictionary<(KeyboardKey, int), Action<float>>();
	//!!! MAke these private again, turned public for testing
	private static Dictionary<(KeyboardKey, int), Action<float>> holdBinds = new Dictionary<(KeyboardKey, int), Action<float>>();
	//!!! MAke these private again, turned public for testing`
	private static Dictionary<(KeyboardKey, int), Action<float>> risingEdgeBinds = new Dictionary<(KeyboardKey, int), Action<float>>();


	public static void BindKeyHold(KeyboardKey key, Action<float> @delegate, params KeyboardKey[] modifiers) {
		// ! Add check to make sure `key` is not `KeyboardKey.KEY_NULL`
		int modsForBind = 0;
		if (modifiers.Length != 0) {
			for (int i=0; i<8; i++) {
				if (modifiers.Any((KeyboardKey x) => (int)x == i+340)) modsForBind |= 1 << i;
				else modsForBind &= ~(1 << i);
			}
		}
		holdBinds.Add((key, modsForBind), @delegate);

	}
	public static void BindKeyRisingEdge(KeyboardKey key, Action<float> @delegate, params KeyboardKey[] modifiers) {
		// ! Add check to make sure `key` is not `KeyboardKey.KEY_NULL`
		int modsForBind = 0;
		if (modifiers.Length != 0) {
			for (int i=0; i<8; i++) {
				if (modifiers.Any((KeyboardKey x) => (int)x == i+340)) modsForBind |= 1 << i;
				else modsForBind &= ~(1 << i);
			}
		}
		risingEdgeBinds.Add((key, modsForBind), @delegate);

	}
	public static void UnbindKey(KeyboardKey key, params KeyboardKey[] modifiers) {
		// ! Add check to make sure `key` is not `KeyboardKey.KEY_NULL`

		int modsForBind = 0;
		if (modifiers.Length != 0) {
			for (int i=0; i<8; i++) {
				if (modifiers.Any((KeyboardKey x) => (int)x == i+340)) modsForBind |= 1 << i;
				else modsForBind &= ~(1 << i);
			}
		}
		if (risingEdgeBinds.Remove((key, modsForBind))) return;
		if (holdBinds.Remove((key, modsForBind))) return;
		Console.WriteLine("Tried to unbind key that wasn't bound");
	}


	public static void CheckKeyModifiers() {
		// KEY_LEFT_SHIFT = 340
		// KEY_LEFT_CONTROL = 341
		// KEY_LEFT_ALT = 342
		// KEY_LEFT_SUPER = 343
		// KEY_RIGHT_SHIFT = 344
		// KEY_RIGHT_CONTROL = 345
		// KEY_RIGHT_ALT = 346
		// KEY_RIGHT_SUPER = 347


		for (int i=340; i<348; i++) {
			//* Set the respective bit in `KeyModifiers`
			if (Raylib.IsKeyDown((KeyboardKey)i)) {
				KeyModifiers |= 1 << (i-340);
				Console.WriteLine((KeyboardKey)i);
			}
			else KeyModifiers &= ~(1 << (i-340));
		}
	}
	public static void ExecKeyBinds(float dt) {


		KeyboardKey risingKey;
		while ((risingKey=(KeyboardKey)Raylib.GetKeyPressed()) != KeyboardKey.KEY_NULL) {
			CheckKeyModifiers(); // Update key modifiers
			// Console.WriteLine($"{risingKey}, {KeyModifiers}");
			if (risingEdgeBinds.ContainsKey((risingKey, KeyModifiers))) risingEdgeBinds[(risingKey, KeyModifiers)].Invoke(dt);
			KeysPressed.Add(risingKey);
		}

		List<KeyboardKey> keysToRemove = new List<KeyboardKey>();
		foreach (KeyboardKey heldKey in KeysPressed) {
			// Console.WriteLine($"{heldKey}, {KeyModifiers}");
			if (Raylib.IsKeyUp(heldKey)) {
				keysToRemove.Add(heldKey);
				continue;
			}
			if (holdBinds.ContainsKey((heldKey, KeyModifiers))) holdBinds[(heldKey, KeyModifiers)].Invoke(dt);
		}

		foreach (KeyboardKey keyToRemove in keysToRemove) KeysPressed.Remove(keyToRemove);
		keysToRemove.Clear();
	}
}