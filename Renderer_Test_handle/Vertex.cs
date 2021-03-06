﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
namespace Program
{
    public struct Vertex
    {
        private Vector3 position;
        private Vector3 normal;
        private Vector2 texcoords;
        private const int size = sizeof(float)*8;
        public Vertex()
        {
            position = new Vector3();
            normal = new Vector3();
            texcoords = new Vector2();
        }
        public Vertex(Vector3 position,Vector3 normal, Vector2 texcoords)
        {
            this.position = position;
            this.normal = normal;
            this.texcoords = texcoords;
        }
        public Vertex(Vector3 position)
        {
            this.position = position;
            normal = new Vector3();
            texcoords = new Vector2();
        }
        public Vertex(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
            this.texcoords = new Vector2();
        }
        public Vertex(Vector3 position, Vector2 texcoords)
        {
            this.position = position;
            this.texcoords = texcoords;
            this.normal = new Vector3();
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }
        public Vector2 TexCoords
        {
            get { return texcoords; }
            set { texcoords = value; }
        }
    }
}