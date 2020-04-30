#version 330

out vec4 outputColor;

in vec2 texCoord;
flat in vec4 lighting;

uniform sampler2D texture0;

void main()
{
    outputColor = texture(texture0, texCoord) * lighting;
    if (outputColour.a == 0) {
        discard;
    }

}