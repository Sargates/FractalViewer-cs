#version 330
in vec2 fragTexCoord;
in vec4 fragColor;

out vec4 finalColor;

uniform mat4 worldToLocal;
uniform int maxIterations;
uniform float zoom;
uniform vec2 screenSize;


vec2 complexMultiply(vec2 a, vec2 b) {
	return vec2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);
}

void main() {

	vec4 colors[7];

	colors[0] = vec4(0.12, 0.12, 0.12, 1);
	colors[1] = vec4(1, 0, 0, 1);
	colors[2] = vec4(1,.5, 0, 1);
	colors[3] = vec4(1, 1, 0, 1);
	colors[4] = vec4(0, 1, 0, 1);
	colors[5] = vec4(0, 0, 1, 1);
	colors[6] = vec4(1, 0, 1, 1);

	vec2 temp = 2.0*fragTexCoord-1.0;


	vec2 worldPos = (inverse(worldToLocal) * vec4(fragTexCoord*(screenSize/2), 0, 0)).xy;


	vec2 uv = worldPos*zoom; // Multiply by zoom so that things don't get translated why zoom (not exactly sure why this is required)
	vec2 c = uv.xy;
	c /= vec2(800.0, 800.0); // Divide out to scale uv coord up to reasonable scale (vector components are pixel 1:1)
	// 800 is hardcoded in the program as well so don't change it here
	vec2 z = vec2(0.0, 0.0);

	int i;
	for(i = 0; i < maxIterations; i++) {
		if(dot(z, z) > 4.0) {
			break;
		}
		z = complexMultiply(z, z) + c;
	}

	if(i == maxIterations) {
		finalColor = vec4(0.0, 0.0, 0.0, 1.0); // Black for the Mandelbrot set
	} else {
		float interp = float(i) / float(maxIterations);
		int index = int(float(colors.length())*sqrt(interp));
		// vec4 colorMix = mix(colors)
		finalColor = colors[index];
	}
}
