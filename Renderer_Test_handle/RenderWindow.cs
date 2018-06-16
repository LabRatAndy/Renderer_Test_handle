using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;

namespace Program
{
    [Flags]
    internal enum cameraMoveflag
    {
        None = 0,
        CameraForward = 1,
        CameraBackward = 2,
        CameraLeft = 4,
        CameraRight = 8,
        CameraUp = 16,
        CameraDown = 32,
        CameraPanLeft = 64,
        CameraPanRight = 128,
        CameraTiltUP = 256,
        CameraTiltDown = 512
    }
    internal struct CameraMovementData
    {
        internal cameraMoveflag flag;
        internal long CForwardTicks;
        internal long CBackwardTicks;
        internal long CleftTicks;
        internal long CRightTicks;
        internal long CUpTicks;
        internal long CDownTicks;
        internal long CPanLeft;
        internal long CPanRight;
        internal long CTiltUp;
        internal long CTiltDown;
    }
    internal class RenderWindow: GameWindow
    {
        //lists of vbos and vaos to go here
        private Renderer.Renderer renderer = null;
        private Renderer.BufferObjects.VertexBufferObject<Renderer.Vertex> testItem = null;
        private Renderer.BufferObjects.VertexArrayObject<Renderer.Vertex> testvao = null;
        private int shaderindex;
        private int textureIndex;
        private int backgroundIndex;
        private Renderer.Camera camera = null;
        private CameraMovementData cameraMovement;
        private Renderer.Renderer.BackGroundData backgroundData;
        internal RenderWindow() :base(800,640,GraphicsMode.Default,"Render Window",GameWindowFlags.Default,DisplayDevice.Default,3,0,GraphicsContextFlags.ForwardCompatible)
        {
            renderer = new Renderer.Renderer();
            Renderer.RendererSettings settings = new Renderer.RendererSettings();
            renderer.Settings = settings;
            OpenTK.Graphics.OpenGL.GL.Viewport(0, 0, 800, 640);
            cameraMovement.flag = cameraMoveflag.None;
            cameraMovement.CBackwardTicks = 0;
            cameraMovement.CDownTicks = 0;
            cameraMovement.CForwardTicks = 0;
            cameraMovement.CleftTicks = 0;
            cameraMovement.CPanLeft = 0;
            cameraMovement.CPanRight = 0;
            cameraMovement.CRightTicks = 0;
            cameraMovement.CTiltDown = 0;
            cameraMovement.CTiltUp = 0;
            cameraMovement.CUpTicks = 0;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            renderer.Settings.ScreenHeight = this.Height;
            renderer.Settings.ScreenWidth = this.Width;
            camera = new Renderer.Camera(new Vector3(0.0f, 0.0f, -25.0f), new Vector3(0.0f, 1.0f, 0.0f));
            renderer.AddCamera(camera, "default");
            renderer.SetActiveCamera("default");
            //code for loading to be placed here set up and initialise the shaders
            string vertex = System.IO.Directory.GetCurrentDirectory() + @"\test2.vert";
            string fragment = System.IO.Directory.GetCurrentDirectory() + @"\test2.frag";
            shaderindex = renderer.AddShader(vertex, fragment, "test2", true);
            if(shaderindex == -1)
            {
                //shader compile error
                MessageBox.Show("Error loading shader. Program will Close", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Exit();
                return;
            }
            vertex = System.IO.Directory.GetCurrentDirectory() + @"\SkyBox.vert";
            fragment = System.IO.Directory.GetCurrentDirectory() + @"\SkyBox.frag";
            shaderindex = renderer.AddShader(vertex, fragment, "SkyBox", true);
            if (shaderindex == -1)
            {
                //shader compile error
                MessageBox.Show("Error loading shader. Program will Close", "Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Exit();
                return;
            }
            Renderer.BufferObjects.VertexAttribute[] attribute = new Renderer.BufferObjects.VertexAttribute[3];
            attribute[0] = new Renderer.BufferObjects.VertexAttribute("position", 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float,
                Renderer.Vertex.Size, 0);
            attribute[1] = new Renderer.BufferObjects.VertexAttribute("normal", 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float,
                Renderer.Vertex.Size, Vector3.SizeInBytes);
            attribute[2] = new Renderer.BufferObjects.VertexAttribute("texcoord", 2, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float,
                Renderer.Vertex.Size, 2 * Vector3.SizeInBytes);
            Renderer.Vertex[] vertices = CreateCube();
            renderer.CreateBufferObjects(vertices, shaderindex, out testItem, out testvao, attribute);
            string textureFile = System.IO.Directory.GetCurrentDirectory() + @"\brick2.png";
            textureIndex = renderer.AddTexture(textureFile, "texture1");
            textureFile = System.IO.Directory.GetCurrentDirectory() + @"\background.png";
            textureIndex = renderer.CreateSkyBox("background");
            renderer.AddSkyBoxTexture(Renderer.SkyBoxTextureSide.Back, textureFile, textureIndex);
            renderer.AddSkyBoxTexture(Renderer.SkyBoxTextureSide.Bottom, textureFile, textureIndex);
            renderer.AddSkyBoxTexture(Renderer.SkyBoxTextureSide.Front, textureFile, textureIndex);
            renderer.AddSkyBoxTexture(Renderer.SkyBoxTextureSide.Left, textureFile, textureIndex);
            renderer.AddSkyBoxTexture(Renderer.SkyBoxTextureSide.Right, textureFile, textureIndex);
            renderer.AddSkyBoxTexture(Renderer.SkyBoxTextureSide.Top, textureFile, textureIndex);
            backgroundData.BackgroundTextureIndex = textureIndex;
            backgroundData.ImageHeight = 512;
            backgroundData.ImageWidth = 1024;
            backgroundData.KeepAspectRatio = true;
            backgroundData.ImageRepations = 6;
            backgroundData.ImageDistance = 600;
            backgroundData.FogStart = 1.0f;
            backgroundData.FogEnd = 0.0f;
            backgroundData.FogRed = 155;
            backgroundData.FogGreen = 155;
            backgroundData.FogBlue = 155;
            backgroundData.BackgroundShaderIndex = shaderindex;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            //call renerer resize function
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            //any required logic to go here
            ProcessCameraMovement();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //add renerer calls to draw the objects
            renderer.BackgroundData = this.backgroundData;
           OpenTK.Graphics.OpenGL.GL.Clear(OpenTK.Graphics.OpenGL.ClearBufferMask.ColorBufferBit|OpenTK.Graphics.OpenGL.ClearBufferMask.DepthBufferBit);
            renderer.RenderBackGround();
            //renderer.RenderObject(testItem, testvao, shaderindex, Matrix4.Identity,textureIndex);
            //renderer.RenderObject();
            SwapBuffers();
        }
        protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
        {
            if (e.KeyChar == 'q') Exit(); 
        }
        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch(e.Key)
            {
                case Key.W:
                    cameraMovement.flag |= cameraMoveflag.CameraForward;
                    cameraMovement.CForwardTicks = DateTime.Now.Ticks;
                    break;
                case Key.S:
                    cameraMovement.flag |= cameraMoveflag.CameraBackward;
                    cameraMovement.CBackwardTicks = DateTime.Now.Ticks;
                    break;
                case Key.A:
                    cameraMovement.flag |= cameraMoveflag.CameraLeft;
                    cameraMovement.CleftTicks = DateTime.Now.Ticks;
                    break; 
                case Key.D:
                    cameraMovement.flag |= cameraMoveflag.CameraRight;
                    cameraMovement.CRightTicks = DateTime.Now.Ticks;
                    break;
                case Key.Keypad8:
                    cameraMovement.flag |= cameraMoveflag.CameraUp;
                    cameraMovement.CUpTicks = DateTime.Now.Ticks;
                    break;
                case Key.Keypad2:
                    cameraMovement.flag |= cameraMoveflag.CameraDown;
                    cameraMovement.CDownTicks = DateTime.Now.Ticks;
                    break;
                case Key.Left:
                    cameraMovement.flag |= cameraMoveflag.CameraPanLeft;
                    cameraMovement.CPanLeft = DateTime.Now.Ticks;
                    break;
                case Key.Right:
                    cameraMovement.flag |= cameraMoveflag.CameraPanRight;
                    cameraMovement.CPanRight = DateTime.Now.Ticks;
                    break;
                case Key.Up:
                    cameraMovement.flag |= cameraMoveflag.CameraTiltUP;
                    cameraMovement.CTiltUp = DateTime.Now.Ticks;
                    break;
                case Key.Down:
                    cameraMovement.flag |= cameraMoveflag.CameraTiltDown;
                    cameraMovement.CTiltDown = DateTime.Now.Ticks;
                    break;
            }
        }
        protected override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            switch (e.Key)
            {
                case Key.W:
                    cameraMovement.flag &= ~cameraMoveflag.CameraForward;
                    cameraMovement.CForwardTicks = 0;
                    break;
                case Key.S:
                    cameraMovement.flag &= ~cameraMoveflag.CameraBackward;
                    cameraMovement.CBackwardTicks = 0;
                    break;
                case Key.A:
                    cameraMovement.flag &= ~cameraMoveflag.CameraLeft;
                    cameraMovement.CleftTicks = 0;
                    break;
                case Key.D:
                    cameraMovement.flag &= ~cameraMoveflag.CameraRight;
                    cameraMovement.CRightTicks = 0;
                    break;
                case Key.Keypad8:
                    cameraMovement.flag &= ~cameraMoveflag.CameraUp;
                    cameraMovement.CUpTicks = 0;
                    break;
                case Key.Keypad2:
                    cameraMovement.flag &= ~cameraMoveflag.CameraDown;
                    cameraMovement.CDownTicks = 0;
                    break;
                case Key.Left:
                    cameraMovement.flag &= ~cameraMoveflag.CameraPanLeft;
                    cameraMovement.CPanLeft = 0;
                    break;
                case Key.Right:
                    cameraMovement.flag &= ~cameraMoveflag.CameraPanRight;
                    cameraMovement.CPanRight = 0;
                    break;
                case Key.Up:
                    cameraMovement.flag &= ~cameraMoveflag.CameraTiltUP;
                    cameraMovement.CTiltUp = 0;
                    break;
                case Key.Down:
                    cameraMovement.flag &= ~cameraMoveflag.CameraTiltDown;
                    cameraMovement.CTiltDown = 0;
                    break;
            }
        }
        protected override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            renderer.OnMouseMove((float)e.XDelta, (float)e.YDelta);
        }
        protected override void OnMouseWheel(OpenTK.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            renderer.OnMouseWheelMove(e.DeltaPrecise);
        }
        private byte[] LoadTexture(string filename)
        {

            Image image = Image.FromFile(filename);           
            Bitmap texture = new Bitmap(image);
            #region old texture loading code
            int pixelDataSize = texture.Width * texture.Height * 4;
            Byte[] pixelData = new byte[pixelDataSize];
            int offset = 0;
            for (int y = 0; y < texture.Width; y++)
            {
                for (int x = 0; x < texture.Height; x++)
                {
                    offset = y * texture.Width * 4 + x * 4;
                    System.Drawing.Color pixel = texture.GetPixel(x, y);
                    pixelData[offset] = pixel.R;
                    pixelData[offset + 1] = pixel.B;
                    pixelData[offset + 2] = pixel.G;
                    pixelData[offset + 3] = pixel.A;
                }
            }
            texture.Dispose();
            image.Dispose();
            return pixelData;
            #endregion

        }
        private Renderer.Vertex[] CreateCube()
        {
            Renderer.Vertex[] verticies = new Renderer.Vertex[36];
           //face 1
            verticies[0] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(0.0f, 0.0f));
            verticies[1] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(1.0f, 0.0f));
            verticies[2] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(1.0f, 1.0f));
            verticies[3] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(1.0f, 1.0f));
            verticies[4] = new Renderer.Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(0.0f, 1.0f));
            verticies[5] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(0.0f, 0.0f));
            //face 2
            verticies[6] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(0.0f, 0.0f));
            verticies[7] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(1.0f, 0.0f));
            verticies[8] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(1.0f, 1.0f));
            verticies[9] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector2(1.0f, 1.0f));
            verticies[10] = new Renderer.Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(0.0f, 1.0f));
            verticies[11] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.0f, 0.0f, -1.0f), new Vector2(0.0f, 0.0f));
           //face 3
            verticies[12] = new Renderer.Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(1.0f, 0.0f));
            verticies[13] = new Renderer.Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(1.0f, 1.0f));
            verticies[14] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(0.0f, 1.0f));
            verticies[15] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(0.0f, 1.0f));
            verticies[16] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(0.0f, 0.0f));
            verticies[17] = new Renderer.Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector2(1.0f, 0.0f));
            //face 4
            verticies[18] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(1.0f, 0.0f));
            verticies[19] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(1.0f, 1.0f));
            verticies[20] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(0.0f, 1.0f));
            verticies[21] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(0.0f, 1.0f));
            verticies[22] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(0.0f, 0.0f));
            verticies[23] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector2(1.0f, 0.0f));
            //face 5
            verticies[24] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(0.0f, 1.0f));
            verticies[25] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(1.0f, 1.0f));
            verticies[26] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(1.0f, 0.0f));
            verticies[27] = new Renderer.Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(1.0f, 0.0f));
            verticies[28] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(0.0f, 0.0f));
            verticies[29] = new Renderer.Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f, 0.0f), new Vector2(0.0f, 1.0f));
            //face 6
            verticies[30] = new Renderer.Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(0.0f, 1.0f));
            verticies[31] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(1.0f, 1.0f));
            verticies[32] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(1.0f, 0.0f));
            verticies[33] = new Renderer.Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(1.0f, 0.0f));
            verticies[34] = new Renderer.Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(0.0f, 0.0f));
            verticies[35] = new Renderer.Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(0.0f, 1.0f));
            return verticies;
        }
        private void ProcessCameraMovement()
        {
            long now = DateTime.Now.Ticks;
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraBackward))
            {
                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraBackward, GetTimeDelta(cameraMovement.CBackwardTicks,now));
                    cameraMovement.CBackwardTicks = now;
            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraDown))
            {

                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraDown, GetTimeDelta(cameraMovement.CDownTicks,now));
                    cameraMovement.CDownTicks = now;
            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraForward))
            {

                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraForward, GetTimeDelta(cameraMovement.CForwardTicks,now));
                    cameraMovement.CForwardTicks = now;
            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraLeft))
            {
                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraLeft, GetTimeDelta(cameraMovement.CleftTicks,now));
                    cameraMovement.CleftTicks = now;
            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraPanLeft))
            {
                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraPanLeft, GetTimeDelta(cameraMovement.CPanLeft,now));
                    cameraMovement.CPanLeft = now;

            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraPanRight))
            {
                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraPanRight, GetTimeDelta(cameraMovement.CPanRight,now));
                    cameraMovement.CPanRight = now;
            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraRight))
            {

                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraRight, GetTimeDelta(cameraMovement.CRightTicks,now));
                    cameraMovement.CRightTicks = now;
            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraTiltDown))
            {
                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraTiltDown, GetTimeDelta(cameraMovement.CTiltDown, now));
                    cameraMovement.CTiltDown = now;
            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraTiltUP))
            {
                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraTiltUp, GetTimeDelta(cameraMovement.CTiltUp, now));
                    cameraMovement.CTiltUp = now;
            }
            if(cameraMovement.flag.HasFlag(cameraMoveflag.CameraUp))
            {
                    renderer.OnMoveCamera(Renderer.CameraMovement.CameraUp, GetTimeDelta(cameraMovement.CUpTicks, now));
                    cameraMovement.CUpTicks = now;
            }
        }
        private float GetTimeDelta(long startticks,long currentTicks)
        {
            return (float) (currentTicks-startticks) / 100000;
        }
    }
}