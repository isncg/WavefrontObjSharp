#version 330 core

layout (location = 0) in vec3 pos;
layout (location = 1) in vec3 norm;
layout (location = 2) in vec2 texcoord;

out vec3 normal;
out vec2 uv;

void main()
{
    gl_Position = vec4(pos, 1.0);
    normal = norm;
	uv = texcoord;
}