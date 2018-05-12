using System;
using OpenTK.Graphics.OpenGL;
namespace Renderer.BufferObjects
{
    sealed public class VertexBufferObject<vertex> where vertex: struct
    {
        private readonly int vertexSize;
        private readonly int vertexCount;
        private readonly int handle;
        private vertex[] vertices = null;

        public VertexBufferObject(int vertexSize, vertex[] vertices)
        {
            this.vertexSize = vertexSize;
            this.vertices = vertices;
            this.vertexCount = this.vertices.Length;
            this.handle = GL.GenBuffer();
        }
        public void CreateBuffer(BufferUsageHint hint = BufferUsageHint.StreamDraw)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.handle);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexSize * vertexCount), vertices, hint);
        }
        public void UnBind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        internal void BindBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
        }
        internal void Draw(PrimitiveType primitiveType)
        {
            GL.DrawArrays(primitiveType, 0, vertexCount);
        }
        public void DeleteBuffer()
        {
            GL.DeleteBuffer(handle);
        }
    }
}