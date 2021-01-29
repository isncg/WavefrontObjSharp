﻿#version 330 core
layout (location = 0) in vec3 pos;
layout (location = 1) in vec3 norm;
uniform mat4 mvp;

out vec3 normal;

void main()
{
    gl_Position = mvp * vec4(pos, 1.0);
    normal = norm;
}