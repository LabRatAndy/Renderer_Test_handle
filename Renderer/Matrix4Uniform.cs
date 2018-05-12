using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
namespace Renderer.Uniform
{
    sealed internal class Matrix4Uniform
    {
        private readonly string name;
        private Matrix4 matrix;

        internal Matrix4 Matrix
        {
            get { return this.matrix; }
            set { this.matrix = value; }
        }
        internal Matrix4Uniform(string name)
        {
            this.name = name;
        }
        internal void Set(Shader shader)
        {
            int uniformhandle = shader.GetUniformLocation(this.name);
            GL.UniformMatrix4(uniformhandle, false, ref matrix);
        }
    }
    sealed internal class FloatUniform
    {
        private readonly string name;
        private float value;
        internal float Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        internal FloatUniform(string name)
        {
            this.name = name;
        }
        internal void Set(Shader shader)
        {
            int uniformHandle = shader.GetUniformLocation(this.name);
            GL.Uniform1(uniformHandle, this.value);
        }
    }
    sealed internal class IntUniform
    {
        private readonly string name;
        private int value;
        internal int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        internal IntUniform(string name)
        {
            this.name = name;
        }
        internal void Set(Shader shader)
        {
            int uniformHandle = shader.GetUniformLocation(this.name);
            GL.Uniform1(uniformHandle, this.value);
        }
    }
    sealed internal class Vector4Uniform
    {
        private readonly string name;
        private Vector4 value;
        internal Vector4 Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        internal Vector4Uniform(string name)
        {
            this.name = name;
        }
        internal void Set(Shader shader)
        {
            int uniformHandle = shader.GetUniformLocation(this.name);
            GL.Uniform4(uniformHandle, ref this.value);
        }
    }
    sealed internal class Colour4Uniform
    {
        private readonly string name;
        private OpenTK.Graphics.Color4 colour;
        internal OpenTK.Graphics.Color4 Colour
        {
            get { return this.colour; }
            set { this.colour = value; }
        }
        internal Colour4Uniform(string name)
        {
            this.name = name;
        }
        internal void Set(Shader shader)
        {
            int uniformHandle = shader.GetUniformLocation(this.name);
            GL.Uniform4(uniformHandle, this.colour);
        }
    }
    sealed internal class Vector3Uniform
    {
        private readonly string name;
        private Vector3 value;
        internal Vector3 Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        internal Vector3Uniform(string name)
        {
            this.name = name;
        }
        internal void Set(Shader shader)
        {
            int uniformHandle = shader.GetUniformLocation(this.name);
            GL.Uniform3(uniformHandle, ref this.value);
        }
    }
    sealed internal class Vector2Uniform
    {
        private readonly string name;
        private Vector2 value;
        internal Vector2 Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        internal Vector2Uniform(string name)
        {
            this.name = name;
        }
        internal void Set(Shader shader)
        {
            int uniformHandle = shader.GetUniformLocation(this.name);
            GL.Uniform2(uniformHandle, ref this.value);
        }
    }
}