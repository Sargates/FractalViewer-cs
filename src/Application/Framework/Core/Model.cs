using Raylib_cs;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using FractalViewer.Application.UI;

namespace FractalViewer.Application;
public class Model {

	public View view;
	
	public Model(View view) {
		this.view = view;
	}
}