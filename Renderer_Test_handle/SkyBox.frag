# version 330 core
in vec3 TexCoords;
uniform samplerCube skyboxtexture;
out vec4 frag_color;
void Main()
{
	frag_color = texture(skyboxtexture, TexCoords);
}