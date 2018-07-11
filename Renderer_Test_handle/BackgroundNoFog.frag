/* Created with CodeXL */
# version 330 core
in vec2 TexCoord;
out vec4 color;
uniform sampler2D background;
void main(void)
{
	color = texture(background,TexCoord);
}