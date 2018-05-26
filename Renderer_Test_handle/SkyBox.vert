﻿# version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texcoords;
out vec3 TexCoords;
uniform mat4 projection;
uniform mat4 view;
void main()
{
	TexCoords = position;
	gl_Position = projection * view * vec4(position, 1.0);
}