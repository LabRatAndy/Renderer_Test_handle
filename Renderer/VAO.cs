using System;
using OpenTK.Graphics.OpenGL;

namespace Renderer.BufferObjects
{
    sealed public class VertexArrayObject<vertex> where vertex : struct
    {
        private readonly int handle;

        public VertexArrayObject()
        {
            GL.GenVertexArrays(1, out this.handle);
            this.Bind();
        }
        public void SetAttributes(VertexBufferObject<vertex> vbo, Shader shader, params VertexAttribute[] attributes)
        {
            this.Bind();
            vbo.BindBuffer();
            foreach (var attribute in attributes) attribute.Set(shader);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        internal void Bind()
        {
            GL.BindVertexArray(handle);
        }
        internal void UnBind()
        {
            GL.BindVertexArray(0);
        }
    }
}