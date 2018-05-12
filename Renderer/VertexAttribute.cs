using System;
using OpenTK.Graphics.OpenGL;

namespace Renderer.BufferObjects
{
    sealed public class VertexAttribute
    {
        private readonly string name;
        private readonly int size;
        private readonly VertexAttribPointerType type;
        private readonly bool normalise;
        private readonly int stride;
        private readonly int offset;
        
        public VertexAttribute(string name, int size, VertexAttribPointerType type, int stride,int offset, bool normalise = false)
        {
            this.name = name;
            this.size = size;
            this.type = type;
            this.stride = stride;
            this.offset = offset;
            this.normalise = normalise;
        }
        public void Set(Shader shader)
        {
            int attributeHandle = shader.GetAttributeLocation(name);
            GL.VertexAttribPointer(attributeHandle, size, type, normalise, stride, offset);
            GL.EnableVertexAttribArray(attributeHandle);
        }
    }
}