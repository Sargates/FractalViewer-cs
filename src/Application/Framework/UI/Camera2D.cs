using System.Numerics;

namespace FractalViewer.Extras;
public class Camera2D {
	public Vector2 offset;
	public Vector2 target;
	public float rotation;
	public float zoom;

	public Camera2D() {
		offset = new Vector2(0, 0);
		target = new Vector2(0, 0);
		rotation = 0;
		zoom = 1;
	}
	public Camera2D(Vector2 offset, Vector2 target, float rotation, float zoom) {
		this.offset = offset;
		this.target = target;
		this.rotation = rotation;
		this.zoom = zoom;
	}

	public static implicit operator Raylib_cs.Camera2D(Camera2D cam) { return new Raylib_cs.Camera2D(cam.offset, cam.target, cam.rotation, cam.zoom); }
	// public static implicit operator Camera2D(Raylib_cs.Camera2D cam) { return new Camera2D(cam.offset, cam.target, cam.rotation, cam.zoom); }
}