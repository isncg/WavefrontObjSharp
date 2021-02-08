#version 330 core
in vec3 normal;
in vec2 uv;

out vec4 result;

uniform vec3 color;
uniform sampler2D tex;

void main()
{
	vec4 ambientColor = vec4(0.1,0.1,0.1,1);
	float diffuseFactor = dot(normalize(normal), vec3(1.0,1.0,1.0));
	vec4 diffuseColor;
	if(diffuseFactor>0)
		diffuseColor = vec4(diffuseFactor, diffuseFactor, diffuseFactor, 1.0);
	else
		diffuseColor = vec4(0, 0, 0, 0);
    result = texture2D(tex, uv.st)*(ambientColor + diffuseColor);
} 