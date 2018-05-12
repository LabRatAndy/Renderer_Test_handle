#version 330 core
in vec2 TexCoord;
out vec4 color;
uniform sampler2D background;
uniform int FogOn;
uniform float FogStart;
uniform float FogEnd;
uniform vec3 FogColour;
uniform float BackgroundDistance;

void main()
{
	float fogFactor = 1.0 - clamp((FogEnd-BackgroundDistance)/(FogEnd-FogStart),0.0,1.0);
	vec3 textColour = texture(background,TexCoord).rgb;
	vec3 endColour = vec3(0,0,0);
	endColour = (1.0 - fogFactor) * FogColour + fogFactor * textColour;
	color = vec4(endColour,1.0);
}