#version 330 core
uniform sampler2D tex;

in vec3 normal;
in vec2 uv;

out vec4 result;

void main()
{
	result = texture2D(tex, uv.st);
}