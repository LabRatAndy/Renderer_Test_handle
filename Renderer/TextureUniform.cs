using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace  Renderer.Uniform
{
    sealed internal class TextureUniform
    {
        private readonly string name;
        private int theTextureUnit;

        internal int TheTextureUnit
        {
            get { return theTextureUnit; }
            set { theTextureUnit = value; }
        }
        internal TextureUniform(string name)
        {
            this.name = name;
        }
        internal void Set(Shader shader)
        {
            int uniformhandle = shader.GetUniformLocation(name);
            GL.Uniform1(uniformhandle, theTextureUnit);
        }
    }
}