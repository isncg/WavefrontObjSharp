#version 330 core
layout (location = 0) in vec3 pos;
layout (location = 1) in vec3 norm;
layout (location = 2) in vec2 texcoord;
uniform mat4 _mvp;

out vec3 world_pos;
out vec3 world_normal;
out vec2 uv;

void main()
{
    gl_Position = _mvp * vec4(pos, 1.0);
	world_pos = pos;
    world_normal = norm;
	uv = texcoord;
}