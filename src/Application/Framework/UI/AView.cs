using Raylib_cs;
using System.Diagnostics;
using System.Numerics;

namespace FractalViewer.Application.UI;

public abstract class AView {
	
	public static Vector2 screenSize;

	//* Used for changing the camera that's actively being drawn in the perspective from. Used for dynamically changing the view
	//* Used for storing a default camera
	public Extras.Camera2D camera;
	public List<ScreenObject> pipeline = new List<ScreenObject>();
	public float fTimeElapsed = 0.0f;


	public AView(Extras.Camera2D camera) {
		this.camera = camera;
	}
	public void AddToPipeline(ScreenObject interactable) {
		pipeline.Add(interactable);
	}

	public void UpdateAssets(float dt) {
		foreach (ScreenObject asset in pipeline) {
			asset.Update(dt);
		}
	}
	public void DrawAssets() {
		foreach (ScreenObject asset in pipeline) {
			asset.Draw();
		}
	}
	public virtual void Update(float dt) {}
	public virtual void Draw() {}

	
	public virtual void Release() {}
}