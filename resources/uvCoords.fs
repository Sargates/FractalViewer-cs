#version 330
in vec2 fragTexCoord;
in vec4 fragColor;

out vec4 finalColor;

uniform vec2 screenSize;

void main() {
	vec2 uv = fragTexCoord.xy;
	finalColor = vec4(fragTexCoord, 0., 1.);
}