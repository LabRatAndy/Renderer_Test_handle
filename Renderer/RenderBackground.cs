using OpenTK.Graphics.OpenGL;
using OpenTK;
using Renderer.BufferObjects;
using System;

namespace Renderer
{
    // implementation of background renderer
    public partial class Renderer
    {
        private VertexBufferObject<Vertex>  BGvbo= null;
        private VertexArrayObject<Vertex> BGvao = null;
        private ElementBufferObject BGSideWallebo = null;
        private ElementBufferObject BGCapsebo = null;
        public struct BackGroundData
        {
            int imagewidth;
            int imageheight;
            bool keepAspectRatio;
            int imageRepations;
            int imageDistance;
            int backgroundShaderIndex;
            int backgroundTexture;
            byte fogRed;
            byte fogGreen;
            byte fogBlue;
            float fogStart;
            float fogend;
            public int ImageWidth
            {
                get { return imagewidth; }
                set { imagewidth = value; }
            }
            public int ImageHeight
            {
                get { return imageheight; }
                set { imageheight = value; }
            }
            public int ImageRepations
            {
                get { return imageRepations; }
                set { imageRepations = value; }
            }
            public bool KeepAspectRatio
            {
                get { return keepAspectRatio; }
                set { keepAspectRatio = value; }
            }
            public int ImageDistance
            {
                get { return imageDistance; }
                set { imageDistance = value; }
            }
            public int BackgroundShaderIndex
            {
                get { return backgroundShaderIndex;}
                set { backgroundShaderIndex = value;}
            }
            public int BackgroundTextureIndex
            {
                get { return backgroundTexture; }
                set { backgroundTexture = value; }
            }
            public byte FogRed
            {
                get { return fogRed; }
                set { fogRed = value; }
            }
            public byte FogGreen
            {
                get { return fogGreen; }
                set { fogGreen = value; }
            }
            public byte FogBlue
            {
                get { return fogBlue; }
                set { fogBlue = value; }
            }
            public float FogStart
            {
                get { return fogStart; }
                set { fogStart = value; }
            }
            public float FogEnd
            {
                get { return fogend; }
                set { fogend = value; }
            }
        }
        private BackGroundData BGData;
        public BackGroundData BackgroundData
        {
            get { return BGData; }
            set { BGData = value; }
        }
        public void RenderBackGround()
        {
            if (BGvbo == null) CreateSkyBox();
            if(depthteston)
            {
                GL.Disable(EnableCap.DepthTest);
            }
            Shader shader = shaderMgr.GetShader(BGData.BackgroundShaderIndex);
            if (shader == null) throw new Exception("background shader not initalised");
            shader.Use();
            Uniform.Matrix4Uniform view = new Uniform.Matrix4Uniform("view");
            Uniform.Matrix4Uniform projection = new Uniform.Matrix4Uniform("projection");
            view.Matrix = cameraMgr.GetActiveCamera().GetViewMatrix();
            projection.Matrix = GetProjectionMatrix();
            view.Set(shader);
            projection.Set(shader);
            GL.ActiveTexture(TextureUnit.Texture0);
            textureMgr.GetTexture(BGData.BackgroundTextureIndex).BindTexture();
            textureMgr.GetTexture(BGData.BackgroundTextureIndex).ActivateTexture(TextureUnit.Texture0, shader, "skyboxtexture", 0);
            BGvao.Bind();
            BGvbo.BindBuffer();
            BGvbo.Draw(PrimitiveType.Triangles);
            BGvbo.UnBind();
            BGvao.UnBind();
            GL.Enable(EnableCap.DepthTest);
            depthteston = true;
        }
        private void BackgroundFog(Shader shader)
        {
            Uniform.IntUniform fogOn = new Uniform.IntUniform("FogOn");
            Uniform.FloatUniform fogStart = new Uniform.FloatUniform("FogStart");
            Uniform.FloatUniform fogEnd = new Uniform.FloatUniform("FogEnd");
            Uniform.Colour4Uniform fogColour = new Uniform.Colour4Uniform("FogColour");
            Uniform.FloatUniform backgroundDistance = new Uniform.FloatUniform("BackgroundDistance");
            const float fogdistance = 600.0f;
            const float scale = 0.5f;            
            float ratio = (float)BGData.ImageDistance/fogdistance;
            fogOn.Value = 1;
            fogStart.Value = BGData.FogStart * ratio * scale;
            fogEnd.Value = BGData.FogEnd * ratio * scale;
            OpenTK.Graphics.Color4 colour = new OpenTK.Graphics.Color4(BGData.FogRed, BGData.FogGreen, BGData.FogBlue, 255);
            fogColour.Colour = colour;
            backgroundDistance.Value = (float)BGData.ImageDistance;
            fogOn.Set(shader);
            fogStart.Set(shader);
            fogEnd.Set(shader);
            fogColour.Set(shader);
            backgroundDistance.Set(shader);
        }
        private void CreateBackgroundCone()
        {
            Vertex[] vertices = new Vertex[66];
            Vector3[] top = new Vector3[32];
            Vector3[] bottom = new Vector3[32];
            float y0, y1;
            double angleValue = 2.61799387799149 - 3.14159265358979 / 32.0d;
            const double angleIncrement = 6.28318530717958 / 32.0d;
            const float scale = 0.5f;
            float textureStart = 0.5f*(float)BGData.ImageRepations/32.0f;
            float textureIncrement = -(float)BGData.ImageRepations / 32.0f;
            float textureX = textureStart;
            //calculate the Y value for top(y1 and bottom(y0)
            if(BGData.KeepAspectRatio)
            {
                double hh = Math.PI * BGData.ImageDistance * (double)BGData.ImageHeight / ((double)BGData.ImageWidth * (double)BGData.ImageRepations);
                y0 = (float)(-0.5 * hh);
                y1 = (float)(1.5 * hh);
            }
            else
            {
                y0 = (float)(-0.125 * BGData.ImageDistance);
                y1 = (float)(0.373 * BGData.ImageDistance);
            }
            //calculate x and z values and create vectors for positions
            for (int n = 0; n < 32; n++)
            {
                float x = (float)(BGData.ImageDistance * Math.Cos(angleValue));
                float z = (float)(BGData.ImageDistance * Math.Sin(angleValue));
                top[n] = new Vector3(scale * x, scale * y1, scale * z);
                bottom[n] = new Vector3(scale * x, scale * y0, scale * z);
                angleValue += angleIncrement;
            }
            //calculate texture coords and assemble vertices
            int vertexCount = 0;
            for (int n = 0; n < 32; n++)
            {
                Vector2 topTexCoord = new Vector2(textureX, 0.005f);
                vertices[vertexCount] = new Vertex(top[n], topTexCoord);
                vertexCount++;
                Vector2 bottomTexCoord = new Vector2(textureX, 0.995f);
                vertices[vertexCount] = new Vertex(bottom[n], bottomTexCoord);
                vertexCount++;
                textureX += textureIncrement;
            }
            //vertex 64 and 65 are the centre vertex or the top and bottom cap respectivly
            Vector2 topCapTexCoords = new Vector2(textureStart + 0.5f * textureIncrement, 0.1f);
            Vector2 bottomCapTexCoords = new Vector2(textureStart + 0.5f * textureIncrement, 0.9f);
            Vector3 topCentreCoord = new Vector3(0.0f, y1, 0.0f);
            Vector3 bottomCentreCoord = new Vector3(0.0f, y0, 0.0f);
            vertices[64] = new Vertex(topCentreCoord, topCapTexCoords);
            vertices[65] = new Vertex(bottomCentreCoord, bottomCapTexCoords);
            CreateBackgroundConeBuffers(vertices);            
        }
        private void CreateBackgroundConeBuffers(Vertex[] vertices)
        {
            //first set up the attributes
            BufferObjects.VertexAttribute[] attribute = new BufferObjects.VertexAttribute[3];
            attribute[0] = new BufferObjects.VertexAttribute("position", 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float,
                Vertex.Size, 0);
            attribute[1] = new BufferObjects.VertexAttribute("normal", 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float,
                Vertex.Size, Vector3.SizeInBytes);
            attribute[2] = new BufferObjects.VertexAttribute("texcoord", 2, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float,
                Vertex.Size, 2 * Vector3.SizeInBytes);
            BGvao = new VertexArrayObject<Vertex>();
            BGvbo = new VertexBufferObject<Vertex>(Vertex.Size, vertices);
            BGvbo.CreateBuffer(BufferUsageHint.StaticDraw);
            BGvao.SetAttributes(BGvbo, shaderMgr.GetShader("BackgroundNoFog"), attribute);
            BGvao.Bind();
            // create the EBO 
            int[] EBOindices = new int[]
#region  
            {0,1,3,2,
            2,3,5,4,
            4,5,7,6,
            6,7,9,8,
            8,9,11,10,
            10,11,13,12,
            12,13,15,14,
            14,15,17,16,
            16,17,19,18,
            18,19,21,20,
            20,21,23,22,
            22,23,25,24,
            24,25,27,26,
            26,27,29,28,
            28,29,31,30,
            30,31,33,32,
            32,33,35,34,
            34,35,37,36,
            36,37,39,38,
            38,39,41,40,
            40,41,43,42,
            42,43,45,44,
            44,45,47,46,
            46,47,49,48,
            48,49,51,50,
            50,51,53,52,
            52,53,55,54,
            54,55,57,56,
            56,57,59,58,
            58,59,61,60,
            60,61,63,62,
            62,63,1,0};
#endregion
            //side walls
            BGSideWallebo = new ElementBufferObject(EBOindices);
            BGSideWallebo.CreateBuffer(BufferUsageHint.StaticDraw);
            EBOindices = null;
            //caps
            EBOindices = new int[]
#region
            {
                0,2,64,
                2,4,64,
                4,6,64,
                6,8,64,
                8,10,64,
                10,12,64,
                12,14,64,
                14,16,64,
                16,18,64,
                18,20,64,
                20,22,64,
                22,24,64,
                24,26,64,
                26,28,64,
                28,30,64,
                30,32,64,
                32,34,64,
                34,36,64,
                36,38,64,
                38,40,64,
                40,42,64,
                42,44,64,
                44,46,64,
                46,48,64,
                48,50,64,
                50,52,64,
                52,54,64,
                54,56,64,
                56,58,64,
                58,60,64,
                60,62,64,
                62,0,64,
                65,3,1,
                65,5,3,
                65,7,5,
                65,9,7,
                65,11,9,
                65,13,11,
                65,15,13,
                65,17,15,
                65,19,17,
                65,21,19,
                65,23,21,
                65,25,23,
                65,27,25,
                65,29,27,
                65,31,29,
                65,33,31,
                65,35,33,
                65,37,35,
                65,39,37,
                65,41,39,
                65,43,41,
                65,45,43,
                65,47,45,
                65,49,47,
                65,51,49,
                65,53,51,
                65,55,53,
                65,57,55,
                65,59,57,
                65,61,59,
                65,63,61,
                65,1,63,
            };
#endregion     
            BGCapsebo = new ElementBufferObject(EBOindices);
            BGCapsebo.CreateBuffer(BufferUsageHint.StaticDraw);
            BGvao.UnBind();
            BGvbo.UnBind();
            BGSideWallebo.Unbind();
            BGCapsebo.Unbind();
        }
        /// <summary>
        /// creates the skybox vertices vbo vao etc
        /// </summary>
        private void CreateSkyBox()
        {
            Vertex[] vertices = new Vertex[36];
            #region
            vertices[0] = new Vertex(new Vector3(-10.0f, 10.0f, -10.0f));
            vertices[1] = new Vertex(new Vector3(-10.0f, -10.0f, -10.0f));
            vertices[2] = new Vertex(new Vector3(10.0f, -10.0f, -10.0f));
            vertices[3] = new Vertex(new Vector3(10.0f, -10.0f, -10.0f));
            vertices[4] = new Vertex(new Vector3(10.0f, 10.0f, -10.0f));
            vertices[5] = new Vertex(new Vector3(-10.0f, 10.0f, -10.0f));

            vertices[6] = new Vertex(new Vector3(-10.0f, -10.0f, 10.0f));
            vertices[7] = new Vertex(new Vector3(-10.0f, -10.0f, -10.0f));
            vertices[8] = new Vertex(new Vector3(-10.0f, 10.0f, -10.0f));
            vertices[9] = new Vertex(new Vector3(-10.0f, 10.0f, -10.0f));
            vertices[10] = new Vertex(new Vector3(-10.0f, 10.0f, 10.0f));
            vertices[11] = new Vertex(new Vector3(-10.0f, -10.0f, 10.0f));

            vertices[12] = new Vertex(new Vector3(10.0f, -10.0f, -10.0f));
            vertices[13] = new Vertex(new Vector3(10.0f, -10.0f, 10.0f));
            vertices[14] = new Vertex(new Vector3(10.0f, 10.0f, 10.0f));
            vertices[15] = new Vertex(new Vector3(10.0f, 10.0f, 10.0f));
            vertices[16] = new Vertex(new Vector3(10.0f, 10.0f, -10.0f));
            vertices[17] = new Vertex(new Vector3(10.0f, -10.0f, -10.0f));

            vertices[18] = new Vertex(new Vector3(-10.0f, -10.0f, 10.0f));
            vertices[19] = new Vertex(new Vector3(-10.0f, 10.0f, 10.0f));
            vertices[20] = new Vertex(new Vector3(10.0f, 10.0f, 10.0f));
            vertices[21] = new Vertex(new Vector3(10.0f, 10.0f, 10.0f));
            vertices[22] = new Vertex(new Vector3(10.0f, -10.0f, 10.0f));
            vertices[23] = new Vertex(new Vector3(-10.0f, -10.0f, 10.0f));

            vertices[24] = new Vertex(new Vector3(-10.0f, 10.0f, -10.0f));
            vertices[25] = new Vertex(new Vector3(10.0f, 10.0f, -10.0f));
            vertices[26] = new Vertex(new Vector3(10.0f, 10.0f, 10.0f));
            vertices[27] = new Vertex(new Vector3(10.0f, 10.0f, 10.0f));
            vertices[28] = new Vertex(new Vector3(-10.0f, 10.0f, 10.0f));
            vertices[29] = new Vertex(new Vector3(-10.0f, 10.0f, -10.0f));

            vertices[30] = new Vertex(new Vector3(-10.0f, -10.0f, -10.0f));
            vertices[31] = new Vertex(new Vector3(-10.0f, -10.0f, 10.0f));
            vertices[32] = new Vertex(new Vector3(10.0f, -10.0f, -10.0f));
            vertices[33] = new Vertex(new Vector3(10.0f, -10.0f, -10.0f));
            vertices[34] = new Vertex(new Vector3(-10.0f, -10.0f, 10.0f));
            vertices[35] = new Vertex(new Vector3(10.0f, -10.0f, 10.0f));
            #endregion
            BufferObjects.VertexAttribute[] attribute = new VertexAttribute[3];
            attribute[0] = new VertexAttribute("position", 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, Vertex.Size, 0);
            attribute[1] = new VertexAttribute("normal", 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, Vertex.Size, Vector3.SizeInBytes);
            attribute[2] = new VertexAttribute("texcoord", 2, VertexAttribPointerType.Float, Vertex.Size, Vector2.SizeInBytes);
            BGvao = new VertexArrayObject<Vertex>();
            BGvbo = new VertexBufferObject<Vertex>(Vertex.Size, vertices);
            BGvbo.CreateBuffer(BufferUsageHint.StaticDraw);
            BGvao.SetAttributes(BGvbo, shaderMgr.GetShader("SkyBox"), attribute);
            BGvao.UnBind();
            BGvbo.UnBind();
        }
    }
}