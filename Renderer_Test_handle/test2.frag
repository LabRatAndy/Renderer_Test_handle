#version 330 core
in vec3 Normal;
in vec2 TexCoord;
out vec4 color;
uniform sampler2D theTexture;
void main()
{
    color = texture(theTexture, TexCoord);
}