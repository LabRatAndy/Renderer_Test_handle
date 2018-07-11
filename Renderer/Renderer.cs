using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Renderer.BufferObjects;
using System.Diagnostics;
using OpenTK;
namespace Renderer
{
    /// <summary>
    /// The Renderer class
    /// </summary>
    public partial class Renderer
    {
        private ShaderManager shaderMgr = null;
        private CameraManager cameraMgr = null;
        private TextureManager textureMgr = null;
        private RendererSettings currentSettings;
        private RendererSettings newSettings = null;
        private bool settingschanged = false;
        private bool depthteston = false;
        public Renderer()
        {
            shaderMgr = ShaderManager.Instance;
            cameraMgr = CameraManager.Instance;
            textureMgr = TextureManager.Instance;
            GL.Disable(EnableCap.DepthTest);
            currentSettings = new RendererSettings();
            currentSettings.ApplySettings();
        }
        /// <summary>
        /// turns shader compile and link error exception throwing on or off will always return -1 as shader index if compile or link fails
        /// </summary>
        public bool DebugMode
        {
            get { return shaderMgr.DebugMode; }
            set { shaderMgr.DebugMode = value; }
        }

        /// <summary>
        /// Adds a shader for use by the renderer, if a name is given then name can be used to refer to it, in the future as can the returned index
        /// -1 is returned on compile error of the shader
        /// </summary>
        /// <param name="vertexShaderSource">absolute path of the vertex shader source code file</param>
        /// <param name="fragmentShaderSource">absolute path of the fragment shader source code file</param>
        /// <param name="name">optional name of the shader default is null</param>
        /// <param name="initialise">optional flag to initialise the shader default is true</param>
        /// <returns></returns>
        public int AddShader(string vertexShaderSource, string fragmentShaderSource,string name = null,bool initialise = true)
        {
            Shader shader = new Shader(vertexShaderSource, fragmentShaderSource);
            if(initialise)
            {
                if (!shader.InitialiseShader()) return -1;
                return shaderMgr.AddShader(shader, initialise, name);
            }
            return shaderMgr.AddShader(shader, initialise, name);
        }
        /// <summary>
        /// returns the shader object
        /// </summary>
        /// <param name="index">index given by add shader method</param>
        /// <returns></returns>
        public Shader GetShader(int index)
        {
            return shaderMgr.GetShader(index);
        }
        /// <summary>
        /// returns the shader object   
        /// </summary>
        /// <param name="name">the name given to shader in addshader </param>
        /// <returns></returns>
        public Shader GetShader(string name)
        {
            return shaderMgr.GetShader(name);
        }
        /// <summary>
        /// removes a shader 
        /// </summary>
        /// <param name="index">the index returned by add shader</param>
        public void RemoveShader(int index)
        {
            shaderMgr.RemoveShader(index);
        }
        /// <summary>
        /// removes a shader
        /// </summary>
        /// <param name="name">name of shader given when it was added</param>
        public void RemoveShader(string name)
        {
            shaderMgr.RemoveShader(name);
        }
        /// <summary>
        /// Adds a camera to the renderer, returns an index that can be used to access, returns -1 if there is an error.
        /// </summary>
        /// <param name="camera">camera object to add</param>
        /// <param name="cameraName">name of the camera</param>
        /// <returns></returns>
        public int AddCamera(Camera camera, string cameraName)
        {
            if(camera == null) return -1;
            if(string.IsNullOrWhiteSpace(cameraName)) return -1;
            return cameraMgr.AddCamera(camera, cameraName);
        }
        /// <summary>
        /// Removes a camera from the renderer
        /// </summary>
        /// <param name="name">name of camera to remove</param>
        public void RemoveCamera(string name)
        {
            cameraMgr.RemoveCamera(name);
        }
        /// <summary>
        /// Removes a camera from the renderer
        /// </summary>
        /// <param name="index">index of camera to remove</param>
        public void RemoveCamera(int index)
        {
            cameraMgr.RemoveCamera(index);
        }
        /// <summary>
        /// Sets the camera the renderer will use
        /// </summary>
        /// <param name="name">name of the camera</param>
        public void SetActiveCamera(string name)
        {
            cameraMgr.SetActiveCamera(name);
        }
        /// <summary>
        /// Sets the camera the renderer will use
        /// </summary>
        /// <param name="index">index of the camera to use</param>
        public void SetActiveCamera(int index)
        {
            cameraMgr.SetActiveCamera(index);
        }
        /// <summary>
        /// Applies the mouse movement to the current active camera
        /// </summary>
        /// <param name="xDelta">the amount moved along x axis</param>
        /// <param name="yDelta">the amount moved along y axis</param>
        public void OnMouseMove(float xDelta,float yDelta)
        {
            cameraMgr.GetActiveCamera().ProcessMouseMovement(xDelta, yDelta, true);
        }
        /// <summary>
        /// Appies the mouse wheel movement to the current active camera
        /// </summary>
        /// <param name="movementOffset">the amount of the mouse wheel moved</param>
        public void OnMouseWheelMove(float movementOffset)
        {
            cameraMgr.GetActiveCamera().ProcessMouseScroll(movementOffset);
        }
        /// <summary>
        /// Processess the camera movement made via keyboard
        /// </summary>
        /// <param name="movement">movement to make</param>
        /// <param name="timeDelta">Time delta that the key was pressed for</param>
        public void OnMoveCamera(CameraMovement movement,float timeDelta)
        {
            cameraMgr.GetActiveCamera().ProcessKeyboard(movement, timeDelta);
        }
        /// <summary>
        /// adds a texture for the renderer to be able to use 
        /// </summary>
        /// <param name="image">byte array containing image data</param>
        /// <param name="width">width of texture in pixels</param>
        /// <param name="height">height of texture in pixels</param>
        /// <param name="name">string that can be used to name texture (can be null or empty)</param>
        /// <returns>an index that can be used to access the texture</returns>
        public int AddTexture(byte[] image, int width, int height, string name)
        {
            Texture texture = new Texture(image, width, height);
            return textureMgr.AddTexture(texture, name);
        }
        /// <summary>
        /// adds a texture for the renderer to be able to use
        /// </summary>
        /// <param name="textureFileName">string that is the path to the image file for the texture</param>
        /// <param name="name">string that can be used to name texture (can be null or empty)</param>
        /// <returns>an index that can be used to access the texture</returns>
        public int AddTexture(string textureFileName,string name)
        {
            Image image = Image.FromFile(textureFileName);
            Bitmap teximage = new Bitmap(image);
            Texture texture = new Texture(teximage);
            teximage.Dispose();
            image.Dispose();
            return textureMgr.AddTexture(texture, name);
        }
        /// <summary>
        /// Removes a texture so the renderer cannot use it
        /// </summary>
        /// <param name="name">name of the texture to remove as  given in add texture</param>
        public void RemoveTexture(string name)
        {
            textureMgr.RemoveTexture(name);
        }
        /// <summary>
        /// Removes a texture so the renderer cannot use it
        /// </summary>
        /// <param name="index">index of the texture as returned by addtexture</param>
        public void RemoveTexture(int index)
        {
            textureMgr.RemoveTexture(index);
        }
        /// <summary>
        /// creates a new empty texture unit
        /// </summary>
        /// <param name="name">name of the texture unit that can be used to access it</param>
        /// <returns>an index which can be used to access the texture unit</returns>
        public int CreateTextureUnit(string name)
        {
            return textureMgr.CreateTextureUnit(name);
        }
        /// <summary>
        /// adds textures to the texture unit(atomically with an array)
        /// </summary>
        /// <param name="unitIndex">index of texture unit as returned by createtextureunit</param>
        /// <param name="texturestoadd">int array of texture indices to add(as returned by addtexture)</param>
        public void AddTextureToTextureUnit(int unitIndex,int[] texturestoadd)
        {
            for (int n = 0; n < texturestoadd.Length; n++)
            {
                textureMgr.AddTextureToTextureUnit(unitIndex, texturestoadd[n]);
            }
        }
        /// <summary>
        /// adds textures to the texture unit(atomically with an array)
        /// </summary>
        /// <param name="unitName">name of unit to add textures to</param>
        /// <param name="texturestoadd">int array of texture indices to add(as returned by addtexture)</param>
        public void AddTextureToTextureUnit(string unitName, int[] texturestoadd)
        {
            int index = textureMgr.FindTextureUnitIndexFromName(unitName);
            for (int n = 0; n < texturestoadd.Length; n++)
            {
                textureMgr.AddTextureToTextureUnit(index, texturestoadd[n]);
            }
        }
        /// <summary>
        /// adds textures to the texture unit(singularlly)
        /// </summary>
        /// <param name="unitIndex">index of texture unit as returned by createtextureunit</param>
        /// <param name="texturetoadd">index of texture to add(as returned by addtexture)</param>
        public void AddTextureToTextureUnit(int unitIndex, int texturetoadd)
        {
            textureMgr.AddTextureToTextureUnit(unitIndex, texturetoadd);
        }
        /// <summary>
        /// adds a texture to the texture unit
        /// </summary>
        /// <param name="unitName">name of the texture unit</param>
        /// <param name="texturetoadd">index of texture to add(as returned by addtexture)</param>
        public void AddTextureToTextureUnit(string unitName, int texturetoadd)
        {
            int index = textureMgr.FindTextureUnitIndexFromName(unitName);
            textureMgr.AddTextureToTextureUnit(index, texturetoadd);
        }
        /// <summary>
        /// adds a texture to the texture unit
        /// </summary>
        /// <param name="unitName">name of the texture unit</param>
        /// <param name="texturetoadd"> name of the texture to add</param>
        public void AddTextureToTextureUnit(string unitName, string texturetoadd)
        {
            textureMgr.AddTextureToTextureUnit(unitName, texturetoadd);
        }
        /// <summary>
        /// Removes a Texture unit (note it will leave the textures themselves)
        /// </summary>
        /// <param name="index">index of textureunit to remove as returned by createtextureunit</param>
        public void RemoveTextureUnit(int index)
        {
            textureMgr.RemoveTextureUnit(index);
        }
        /// <summary>
        ///  Removes a Texture unit (note it will leave the textures themselves)
        /// </summary>
        /// <param name="name">name of the texture unit to remove</param>
        public void RemoveTextureUnit(string name)
        {
            textureMgr.RemoveTextureUnit(name);
        }
        /// <summary>
        /// Initialises a texture as a skybox cubemap
        /// </summary>
        /// <param name="name">The name of the skybox texture</param>
        /// <returns>The index of the texture</returns>
        public int CreateSkyBox(string name)
        {
            Texture skyBox = new Texture();
            return textureMgr.AddTexture(skyBox, name);
        }
        /// <summary>
        /// Adds a texture to the skybox cubemap texture
        /// </summary>
        /// <param name="side">The side of the cubemap the image represents </param>
        /// <param name="textureFileName"> the filename of the texture</param>
        /// <param name="textureIndex">the index of the cubemap skybox texture retruned by createskybox</param>
        public void AddSkyBoxTexture(SkyBoxTextureSide side, string textureFileName, int textureIndex)
        {
            Texture skyBox = textureMgr.GetTexture(textureIndex);
            Image image = Image.FromFile(textureFileName);
            Bitmap data = new Bitmap(image);
            switch (side)
            {
                case SkyBoxTextureSide.Back:
                    skyBox.LoadCubeMapSide(data, TextureTarget.TextureCubeMapPositiveZ);
                    break;
                case SkyBoxTextureSide.Bottom:
                    skyBox.LoadCubeMapSide(data, TextureTarget.TextureCubeMapNegativeY);
                    break;
                case SkyBoxTextureSide.Front:
                    skyBox.LoadCubeMapSide(data, TextureTarget.TextureCubeMapNegativeZ);
                    break;
                case SkyBoxTextureSide.Left:
                    skyBox.LoadCubeMapSide(data, TextureTarget.TextureCubeMapNegativeX);
                    break;
                case SkyBoxTextureSide.Right:
                    skyBox.LoadCubeMapSide(data, TextureTarget.TextureCubeMapPositiveX);
                    break;
                case SkyBoxTextureSide.Top:
                    skyBox.LoadCubeMapSide(data, TextureTarget.TextureCubeMapPositiveY);
                    break;
            }
            data.Dispose();
            image.Dispose();
        }
        /// <summary>
        /// Gets and Sets the settings for the renderer. Note: they only take effect at start of next render iteration
        /// unless ApplySettingsNow is called
        /// </summary>
        public RendererSettings Settings
        {
            get { return currentSettings; }
            set 
            {
                newSettings = value;
                settingschanged = true;
            }
        }
        /// <summary>
        /// applies the changed settings 
        /// </summary>
        public void ApplySettingsNow()
        {
            if (!settingschanged) return;
            currentSettings = newSettings;
            currentSettings.ApplySettings();
            settingschanged = false;
        }
        public void RenderObject(VertexBufferObject<Vertex> item, VertexArrayObject<Vertex> vao, int shaderIndex)
        {
            if (settingschanged) ApplySettingsNow();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shaderMgr.InitialiseAShader(shaderIndex);
            Shader shader = shaderMgr.GetShader(shaderIndex);
            shader.Use();
            vao.Bind();
            item.BindBuffer();
            item.Draw(PrimitiveType.Triangles);
        }
        public void RenderObject(VertexBufferObject<Vertex>vbo, VertexArrayObject<Vertex> vao,int shaderIndex, Matrix4 modelTransform)
        {
            ApplySettingsNow();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shaderMgr.InitialiseAShader(shaderIndex);
            Uniform.Matrix4Uniform model = new Uniform.Matrix4Uniform("model");
            model.Matrix = modelTransform;
            Uniform.Matrix4Uniform view = new Uniform.Matrix4Uniform("view");
            view.Matrix = cameraMgr.GetActiveCamera().GetVeiwMatrix();
            Uniform.Matrix4Uniform projection = new Uniform.Matrix4Uniform("projection");
            projection.Matrix = GetProjectionMatrix();
            Shader shader = shaderMgr.GetShader(shaderIndex);
            shader.Use();
            model.Set(shader);
            view.Set(shader);
            projection.Set(shader);
            vao.Bind();
            vbo.BindBuffer();
            vbo.Draw(PrimitiveType.Triangles);
        }
        public void RenderObject(VertexBufferObject<Vertex>vbo, VertexArrayObject<Vertex>vao, int shaderIndex, Matrix4 modelTransform,int textureIndex)
        {
            ApplySettingsNow();
            if(!depthteston)
            {
                GL.Enable(EnableCap.DepthTest);
                depthteston = true;
            }
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shaderMgr.InitialiseAShader(shaderIndex);
            Uniform.Matrix4Uniform model = new Uniform.Matrix4Uniform("model");
            model.Matrix = modelTransform;
            Uniform.Matrix4Uniform view = new Uniform.Matrix4Uniform("view");
            view.Matrix = cameraMgr.GetActiveCamera().GetVeiwMatrix();
            Uniform.Matrix4Uniform projection = new Uniform.Matrix4Uniform("projection");
            projection.Matrix = GetProjectionMatrix();
            Shader shader = shaderMgr.GetShader(shaderIndex);
            shader.Use();
            model.Set(shader);
            view.Set(shader);
            projection.Set(shader);
            GL.ActiveTexture(TextureUnit.Texture0);
            textureMgr.GetTexture(textureIndex).BindTexture();
            textureMgr.GetTexture(textureIndex).ActivateTexture(TextureUnit.Texture0, shader, "theTexture", 0);
            vao.Bind();
            vbo.BindBuffer();
            vbo.Draw(PrimitiveType.Triangles);
        }
        public void RenderObject()
        {
            // set up
            float[] vertices = new float[] {-0.5f,-0.5f,0.0f,0.5f,-0.5f,0.0f,0.0f,0.5f,0.0f};
            int vao;
            VertexBufferObject<float> vbo;
            GL.GenVertexArrays(1, out vao);
            vbo = new VertexBufferObject<float>(sizeof(float), vertices);
            GL.BindVertexArray(vao);
            vbo.BindBuffer();
            vbo.CreateBuffer(BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            //actual renderering
            GL.ClearColor(OpenTK.Graphics.Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shaderMgr.InitialiseAShader("test1");
            shaderMgr.GetShader("test1").Use();
            GL.BindVertexArray(vao);
            vbo.Draw(PrimitiveType.Triangles);
            GL.BindVertexArray(0);
        }
        /// <summary>
        /// Creates a VAO and VBO object from an array of vertices, the shader, an array of attributes
        /// </summary>
        /// <param name="vertices"> the vertex array</param>
        /// <param name="shaderIndex">index of the shader being used for that object</param>
        /// <param name="vbo">vbo object for output</param>
        /// <param name="vao">vao object for output</param>
        /// <param name="attributes">attribute or attributes(in an array if more than one)</param>
        public void  CreateBufferObjects(Vertex[] vertices, int shaderIndex, out VertexBufferObject<Vertex>vbo,out VertexArrayObject<Vertex>vao,
             params BufferObjects.VertexAttribute[] attributes)
        {
            VertexArrayObject<Vertex> thevao = new VertexArrayObject<Vertex>();
            VertexBufferObject<Vertex> thevbo = new VertexBufferObject<Vertex>(Vertex.Size, vertices);
            thevbo.CreateBuffer();
            thevao.SetAttributes(thevbo, shaderMgr.GetShader(shaderIndex), attributes);
            vbo = thevbo;
            vao = thevao;
        }
        /// <summary>
        /// Creates a VAO and VBO object from an array of vertices, the shader, an array of attributes
        /// </summary>
        /// <param name="vertices"> the vertex array</param>
        /// <param name="shaderIndex">index of the shader being used for that object</param>
        /// <param name="vbo">vbo object for output</param>
        /// <param name="vao">vao object for output</param>
        /// <param name="attributes">attribute or attributes(in an array if more than one)</param>
        public void CreateBufferObjects(Vertex[] vertices, string shaderName, out VertexBufferObject<Vertex>vbo, out VertexArrayObject<Vertex>vao,
            params BufferObjects.VertexAttribute[] attributes)
        {
            VertexArrayObject<Vertex> thevao = new VertexArrayObject<Vertex>();
            VertexBufferObject<Vertex> thevbo = new VertexBufferObject<Vertex>(Vertex.Size, vertices);
            thevbo.CreateBuffer();
            thevao.SetAttributes(thevbo, shaderMgr.GetShader(shaderName), attributes);
            vbo = thevbo;
            vao = thevao;
        }
        /// <summary>
        /// Calculates the projection matrix
        /// </summary>
        /// <returns>projection matrix</returns>
        private Matrix4 GetProjectionMatrix()
        {
            // field of veiw 45 deg = 0.785398
            ApplySettingsNow();
            float aspect = Settings.ScreenWidth/Settings.ScreenHeight;
            return Matrix4.CreatePerspectiveFieldOfView(0.785398f, aspect, Settings.NearClipDistance, Settings.FarClipDistance);
        }
    }
}