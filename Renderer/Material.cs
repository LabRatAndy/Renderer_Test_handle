using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Renderer;
namespace Renderer
{
    public class Material
    {
        private bool usesColour = false;
        private Vector4 colour;
        private bool usesEmissiveColour = false;
        private Vector3 emissiveColour;
        private bool usesTransparentTextures = false;
        private Vector3 transparentColour;
        private int textureIndex = -1;
        public int TextureIndex
        {
            get { return textureIndex; }
            set { textureIndex = value; }
        }
        public Vector4 Colour
        {
            get { return colour; }
            set { colour = value; }
        }
        public Vector3 EmissiveColour
        {
            get { return emissiveColour; }
            set { emissiveColour = value; }
        }
        public Vector3 TransparentColour
        {
            get { return transparentColour; }
            set { transparentColour = value; }
        }
        public bool UsesEmissiveColour
        {
            get { return usesEmissiveColour; }
            set { usesEmissiveColour = value; }
        }
        public bool UsesTranparentTextures
        {
            get { return usesTransparentTextures; }
            set { usesTransparentTextures = value; }
        }
        public bool UsesColour
        {
            get { return usesColour; }
            set { usesColour = value; }
        }
        public Material(Color colour, Color emissiveColour, Color transparentColour,int textureIndex)
        {
            this.usesColour = true;
            this.colour = ConvertColour4ToVector(colour);
            this.usesEmissiveColour = true;
            this.emissiveColour = ConvertColour3ToVector(emissiveColour);
            this.usesTransparentTextures = true;
            this.transparentColour = ConvertColour3ToVector(transparentColour);
            this.textureIndex = textureIndex;
        }
        public Material(Color colour,Color transparentColour, int textureIndex)
        {
            this.usesColour = true;
            this.colour = ConvertColour4ToVector(colour);
            this.usesTransparentTextures = true;
            this.transparentColour = ConvertColour3ToVector(transparentColour);
            this.textureIndex = textureIndex;
        }
        public Material(Color colour,Color emissiveColour)
        {
            this.usesColour = true;
            this.colour = ConvertColour4ToVector(colour);
            this.usesEmissiveColour = true;
            this.emissiveColour = ConvertColour3ToVector(emissiveColour);
        }
        public Material(int textureIndex, Color transparentColour)
        {
            this.usesTransparentTextures = true;
            this.textureIndex = textureIndex;
        }
        public Material(int textureIndex)
        {
            this.textureIndex = textureIndex;
        }
        public Material(Color colour)
        {
            this.usesColour = true;
            this.colour = ConvertColour4ToVector(colour);
        }
        public Material(Vector3 emissiveColour)
        {
            this.usesEmissiveColour = true;
            this.emissiveColour = emissiveColour;
        }
        public void Set(Shader shader)
        {
            int usesColourhandle, colourHandle, usesEmissiveHandle, emissiveHandle, usesTransparentHandle, transparentHandle;
            usesColourhandle = shader.GetUniformLocation("material.usesColour");
            usesEmissiveHandle = shader.GetUniformLocation("material.usesEmissiveColour");
            usesTransparentHandle = shader.GetUniformLocation("material.usesTransparentColour");
            if (this.usesColour == true)
            {
                colourHandle = shader.GetUniformLocation("material.colour");
                GL.Uniform1(usesColourhandle, 1);
                GL.Uniform4(colourHandle, ref colour);
            }
            else GL.Uniform1(usesColourhandle, 0);
            if (this.usesEmissiveColour == true)
            {
                emissiveHandle = shader.GetUniformLocation("material.emissiveColour");
                GL.Uniform1(usesEmissiveHandle, 1);
                GL.Uniform3(emissiveHandle, ref emissiveColour);
            }
            else GL.Uniform1(usesEmissiveHandle, 0);
            if (this.usesTransparentTextures == true)
            {
                transparentHandle = shader.GetUniformLocation("material.transparentColour");
                GL.Uniform1(usesTransparentHandle, 1);
                GL.Uniform3(transparentHandle, ref transparentColour);
            }
            else GL.Uniform1(usesTransparentHandle, 0);
            // set up texture if needed
            if (textureIndex == -1) return;
            Texture texture = TextureManager.Instance.GetTexture(textureIndex);
            texture.ActivateTexture(TextureUnit.Texture0, shader, "material.texture", 0);
        }
        public void UnbindTexture()
        {
            TextureManager.Instance.GetTexture(textureIndex).UnbindTexture();
        }
        private Vector4 ConvertColour4ToVector(Color theColour)
        {
            float red = (float)(theColour.R / (byte)255);
            float green = (float)(theColour.G / (byte)255);
            float blue = (float)(theColour.B / (byte)255);
            float alpha = (float)(theColour.A / (byte)255);
            return new Vector4(red, green, blue, alpha);
        }
        private Vector3 ConvertColour3ToVector(Color theColour)
        {
            float red = (float)(theColour.R / (byte)255);
            float green = (float)(theColour.G / (byte)255);
            float blue = (float)(theColour.B /(byte)255);
            return new Vector3(red, green, blue);
        }
    }
}