using Raylib_cs;
using System.IO;
using System.Numerics;
using System.Diagnostics;



using FractalViewer.Application.UI;
using FractalViewer.Application.Helpers;
using FractalViewer.Core;

namespace FractalViewer.Application;


public class MainController { // I would use `AppController` but OmniSharp's autocomplete keeps giving me `AppContext`

	private static MainController? instance; // singleton
	public static MainController Instance {
		get {
			if (instance == null) {
				instance = new MainController();
			}
			Debug.Assert(instance != null);
			return instance;
		}
	}

	Extras.Camera2D cam;
	public Model model;
	public View view;
	public Random random = new Random();
	public dynamic appSettings {
		get {
			return ApplicationSettings.Get();
		}
	}


	public float fTimeElapsed = 0.0f;

	public int mouseButtonsClicked=0; // 0b1111
	public int PressedKey=0;
	
	public bool IsRightReleased => (mouseButtonsClicked & 8) == 8;
	public bool IsRightPressed => (mouseButtonsClicked & 4) == 4;
	public bool IsLeftReleased => (mouseButtonsClicked & 2) == 2;
	public bool IsLeftPressed => (mouseButtonsClicked & 1) == 1;

	private MainController() {
		// Used to initialize precomputed move data
		// Type staticClassInfo = typeof(PrecomputedMoveData);
		// var staticClassConstructorInfo = staticClassInfo.TypeInitializer;  
		// staticClassConstructorInfo?.Invoke(null,null);

		
		View.screenSize = new Vector2(appSettings.uiScreenWidth, appSettings.uiScreenHeight);
		Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
		Raylib.SetTraceLogLevel(TraceLogLevel.LOG_WARNING); // Ignore Raylib Errors unless fatal
		// Raylib.SetTraceLogLevel(TraceLogLevel.LOG_FATAL); // Ignore Raylib Errors unless fatal
		Raylib.InitWindow((int)View.screenSize.X, (int)View.screenSize.Y, "Controller");
		Raylib.InitAudioDevice();
		Raylib.SetMasterVolume(appSettings.uiSoundVolume);


		cam = new Extras.Camera2D {
			target = new Vector2(0, 0),
			offset = new Vector2(View.screenSize.X / 2f, View.screenSize.Y / 2f),
			rotation = 0,
			zoom = 1.0f
		};

		view = new View(cam);
		model = new Model(view);
	}

	public void MainLoop() {
		float dt = 0f;

		BindKeys();
		

		while (!Raylib.WindowShouldClose()) {
			dt = Raylib.GetFrameTime();
			fTimeElapsed += dt;
			view.fTimeElapsed = fTimeElapsed;
			UpdateMouseButtons();

			if (Raylib.IsWindowResized()) {
				view.camera.offset = new Vector2(Raylib.GetScreenWidth() / 2f, Raylib.GetScreenHeight() / 2f);
				appSettings.uiScreenWidth = Raylib.GetScreenWidth();
				appSettings.uiScreenHeight = Raylib.GetScreenHeight();
				View.screenSize = new Vector2(appSettings.uiScreenWidth, appSettings.uiScreenHeight);
			}

			Raylib.BeginDrawing();
			Raylib.ClearBackground(new Color(22, 22, 22, 255));

			view.Update(dt);
			KeyboardControls.ExecKeyBinds(dt);
			HandleMouseInput(dt);
			view.Draw();


			//* Draw menu here

			Raylib.DrawFPS(10, 10);
			Raylib.EndDrawing();
		}

		Raylib.CloseAudioDevice();
		Raylib.CloseWindow();
		view.Release();
		

		// SaveApplicationSettings(); // 
	}

	public void HandleMouseInput(float dt) {
		//* Precondition: Mouse button has changed state

		float wheelDelta = Raylib.GetMouseWheelMove();
		if (wheelDelta > 0) {
			View.ActiveCamera.zoom *= View.tenthRootOfTwo;
		} else if (wheelDelta < 0) {
			View.ActiveCamera.zoom /= View.tenthRootOfTwo;
		}
	}

	public void BindKeys() {
		KeyboardControls.BindKeyHold(KeyboardKey.KEY_A, delegate(float dt) {
			View.ActiveCamera.target -= 240*new Vector2(1, 0) * dt / View.ActiveCamera.zoom;
		});
		KeyboardControls.BindKeyHold(KeyboardKey.KEY_D, delegate(float dt) {
			View.ActiveCamera.target += 240*new Vector2(1, 0) * dt / View.ActiveCamera.zoom;
		});
		KeyboardControls.BindKeyHold(KeyboardKey.KEY_W, delegate(float dt) {
			View.ActiveCamera.target -= 240*new Vector2(0, 1) * dt / View.ActiveCamera.zoom;
		});
		KeyboardControls.BindKeyHold(KeyboardKey.KEY_S, delegate(float dt) {
			View.ActiveCamera.target += 240*new Vector2(0, 1) * dt / View.ActiveCamera.zoom;
		});
		KeyboardControls.BindKeyRisingEdge(KeyboardKey.KEY_P, delegate(float dt) {
			// Console.WriteLine(View.ActiveCamera.target);
			// Console.WriteLine(View.ActiveCamera.zoom);
			// Console.WriteLine();
			// Console.WriteLine(Raylib.GetShaderLocationAttrib(View.mandelbrotShader, "localToWorld"));

			Vector2 newPos = Raylib.GetScreenToWorld2D(new Vector2(0, 0), View.ActiveCamera);


			Matrix4x4 localToWorld;

			if (Matrix4x4.Invert(Raylib.GetCameraMatrix2D(View.ActiveCamera), out localToWorld)) {
				Console.WriteLine(localToWorld);
			} else Console.WriteLine("Failed to invert matrix");

			Console.WriteLine(localToWorld == Raylib.GetCameraMatrix2D(View.ActiveCamera));

			Vector4 temp = Vector4.Transform(new Vector4(0, 0, 0, 1), localToWorld);
			Vector2 newVec2 = new Vector2(temp.X, temp.Y);

			Console.WriteLine(newVec2/View.screenSize);
			Console.WriteLine(newPos/View.screenSize);
			// Console.WriteLine(newPos);
			Console.WriteLine(Raylib.GetCameraMatrix2D(View.ActiveCamera));

			// foreach (var pair in KeyboardControls.risingEdgeBinds) {
			// 	Console.WriteLine($"{pair.Key.Item1}, {pair.Key.Item2} : {pair.Value}");
			// }
		});
		
	}

	public void UpdateMouseButtons() {
		mouseButtonsClicked = 0;
		mouseButtonsClicked += Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_RIGHT) ? 8 : 0;
		mouseButtonsClicked += Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT)  ? 4 : 0;
		mouseButtonsClicked += Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)  ? 2 : 0;
		mouseButtonsClicked += Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)   ? 1 : 0;
	}

	public void SaveApplicationSettings() {
		// if (! File.Exists(FileHelper.GetResourcePath("settings.txt"))) { return; }
		using (StreamWriter writer = new StreamWriter(FileHelper.GetResourcePath("settings.txt"))) {
			foreach (var pair in appSettings._dictionary) {
				writer.WriteLine($"{pair.Key}={pair.Value}");
			}
		}
	}
}