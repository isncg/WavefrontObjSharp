#version 330 core

in vec3 world_pos;
in vec3 world_normal;
in vec2 uv;

layout (location = 0) out vec4 out_color;
layout (location = 1) out vec4 out_pos;
layout (location = 2) out vec4 out_normal;
layout (location = 3) out vec4 out_uv;

uniform sampler2D _tex;

void main()
{
	out_color  = texture2D(_tex, uv.st);
	out_pos    = vec4(1.0, 1.0, 1.0, 1.0);//vec4(world_pos, 1.0);
	out_normal = vec4(world_normal, 1.0);
	out_uv     = vec4(1.0, 0.0, 0.0, 1.0);
} 