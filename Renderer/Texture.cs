using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace Renderer
{
    public class Texture
    {
        private int handle;
        
        public Texture(byte[] image, int width, int height)
        {
            GL.GenTextures(1, out handle);
            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, image);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public Texture(Bitmap image)
        {
            GL.GenTextures(1, out handle);
            GL.BindTexture(TextureTarget.Texture2D, handle);
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            image.UnlockBits(data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public void BindTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
        public void ActivateTexture(TextureUnit unit,Shader shader,string samplername,int samplerindex)
        {
            GL.ActiveTexture(unit);
            BindTexture();
            GL.Uniform1(shader.GetUniformLocation(samplername), samplerindex);
        }
        public void UnbindTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

    }
}