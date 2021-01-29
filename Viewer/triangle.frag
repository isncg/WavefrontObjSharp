#version 330 core
in vec3 normal;
out vec4 result;

uniform vec3 color;

void main()
{
    result = vec4(color+normal*0.1, 1.0);
} 