#version 330 core
uniform sampler2D _tex;

in vec2 uv;

out vec4 result;

void main()
{
	result = texture2D(_tex, uv.st);
}