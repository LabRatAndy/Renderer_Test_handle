/* Created with CodeXL */
# version 330 core
in vec3 TexCoords;
uniform samplerCube skyboxtexture;
out vec4 frag_color;
void main(void)
{
	frag_color = texture(skyboxtexture,TexCoords);
}