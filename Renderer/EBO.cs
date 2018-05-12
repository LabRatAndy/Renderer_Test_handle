using System;
using OpenTK.Graphics.OpenGL;
namespace Renderer.BufferObjects
{
    sealed public class ElementBufferObject
    {
        private int handle;
        private int[] indices = null;

        public ElementBufferObject(int[] indices)
        {
            handle = GL.GenBuffer();
            this.indices = indices;
        }
        public void CreateBuffer(BufferUsageHint hint = BufferUsageHint.StreamDraw)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle);
            GL.BufferData(BufferTarget.ElementArrayBuffer,(IntPtr) indices.Length, indices, hint);
        }
        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle);
        }
        public void Draw(PrimitiveType primativeType)
        {
            GL.DrawElements(primativeType, indices.Length, DrawElementsType.UnsignedInt, indices);
        }
        public void DeleteBuffer()
        {
            GL.DeleteBuffer(handle);
            indices = null;
            handle = 0;
        }
    }
}