using Raylib_cs;
using System.Numerics;

namespace FractalViewer.Application.UI;

public class View : AView {

	public static readonly float tenthRootOfTwo = (float)Math.Pow(2.0, 1.0/10.0);
	public static Extras.Camera2D ActiveCamera = new Extras.Camera2D(); //* Used to shut the compiler up
	public static Texture2D texture;
	public static Shader mandelbrotShader;
	private static bool isCamSet=false;

	public View(Extras.Camera2D cam) : base(cam) {
		if (! isCamSet) { ActiveCamera = cam; isCamSet = true; }
		mandelbrotShader = Raylib.LoadShader("resources/template.vs", "resources/mandelbrot.fs");
		// mandelbrotShader = Raylib.LoadShader("resources/template.vs", "resources/uvCoords.fs");
		texture = Raylib.LoadTexture("resources/black.png");
		Console.WriteLine("Shader ID: "+mandelbrotShader.id);

		int maxIterations = 500;
		int maxIterationsLocation = Raylib.GetShaderLocation(mandelbrotShader, "maxIterations");
		Console.WriteLine("maxIterations location: " + maxIterationsLocation);
		Raylib.SetShaderValue(mandelbrotShader, maxIterationsLocation, maxIterations, ShaderUniformDataType.SHADER_UNIFORM_INT);

		int worldToLocalLocation = Raylib.GetShaderLocation(mandelbrotShader, "worldToLocal");
		Console.WriteLine("worldToLocal location: " + worldToLocalLocation);

		int screenSizeLocation = Raylib.GetShaderLocation(mandelbrotShader, "screenSize");
		Raylib.SetShaderValue(mandelbrotShader, screenSizeLocation, View.screenSize, ShaderUniformDataType.SHADER_UNIFORM_VEC2);
		Console.WriteLine("screenSize location: " + screenSizeLocation);

	}

	public override void Update(float dt) {
		UpdateAssets(dt);
	}


	public override void Draw() {
		Raylib.SetShaderValueMatrix(mandelbrotShader, Raylib.GetShaderLocation(mandelbrotShader, "worldToLocal"), Raylib.GetCameraMatrix2D(ActiveCamera));
		Raylib.SetShaderValue(mandelbrotShader, Raylib.GetShaderLocation(mandelbrotShader, "zoom"), ActiveCamera.zoom, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
		Raylib.BeginMode2D(ActiveCamera);
		Raylib.BeginShaderMode(mandelbrotShader);

		Rectangle drawingRect = new Rectangle {
			x = ActiveCamera.target.X-ActiveCamera.offset.X/ActiveCamera.zoom,
			y = ActiveCamera.target.Y-ActiveCamera.offset.Y/ActiveCamera.zoom,
			width = 2*ActiveCamera.offset.X/ActiveCamera.zoom,
			height = 2*ActiveCamera.offset.Y/ActiveCamera.zoom,
		}; //* Scaled and translate a rectangle to cover each corner of the camera in worldspace

		Raylib.DrawTexturePro(texture, drawingRect, drawingRect, Vector2.Zero, 0, Color.BLACK);

		Raylib.EndShaderMode();
		Raylib.EndMode2D();

		Raylib.DrawRectangleV((screenSize-new Vector2(4, 4))/2, new Vector2(4, 4), Color.BLACK);
		Raylib.DrawRectangleV((screenSize-new Vector2(2, 2))/2, new Vector2(2, 2), Color.BROWN);

		Raylib.DrawText($"    Zoom: {View.ActiveCamera.zoom}", 10, 30, 20, Color.GREEN);
		Raylib.DrawText($"Position: {View.ActiveCamera.target/800.0f}", 10, 50, 20, Color.GREEN);

	}


	public override void Release() {
		Raylib.UnloadShader(mandelbrotShader);
		Raylib.UnloadTexture(texture);
	} //* For any unsafe memory allocations that need to be released (Pointers, textures, etc.)





	public static Vector2 GetWorldMousePosition() {
		return Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), ActiveCamera);
	}

	public Vector2 GetWorldMousePosition(Camera2D cam) {
		return Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), cam);
	}


}