#version 330 core
uniform sampler2D _tex;
uniform vec4 step_min;
uniform vec4 step_max;
in vec2 uv;

out vec4 result;

void main()
{
	result = smoothstep(step_min, step_max, texture2D(_tex, uv.st));
}