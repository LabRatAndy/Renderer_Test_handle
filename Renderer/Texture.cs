using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using System;

namespace Renderer
{
    public class Texture
    {
        private int handle;
        private readonly bool cubemap = false;

        public Texture()
        {
            cubemap = true;
            GL.GenTextures(1, out handle);
        }
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
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, LoadTexture(image));
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public void BindTexture()
        {
            if (cubemap == true) GL.BindTexture(TextureTarget.TextureCubeMap, handle);
            else GL.BindTexture(TextureTarget.Texture2D, handle);
        }
        public void ActivateTexture(TextureUnit unit,Shader shader,string samplername,int samplerindex)
        {
            GL.ActiveTexture(unit);
            BindTexture();
            GL.Uniform1(shader.GetUniformLocation(samplername), samplerindex);
        }
        public void UnbindTexture()
        {
            if (cubemap != true) GL.BindTexture(TextureTarget.Texture2D, 0);
            else GL.BindTexture(TextureTarget.TextureCubeMap, 0);
        }
        public void LoadCubeMapSide(Bitmap immage, TextureTarget side)
        {
            if (cubemap != true) throw new Exception("Can only be called on a cubemap / Skybox");
            if (side == TextureTarget.TextureCubeMapNegativeX | side == TextureTarget.TextureCubeMapNegativeY | side == TextureTarget.TextureCubeMapNegativeZ |
                side == TextureTarget.TextureCubeMapPositiveX | side == TextureTarget.TextureCubeMapPositiveY | side == TextureTarget.TextureCubeMapPositiveZ)
            {
                GL.BindTexture(TextureTarget.TextureCubeMap, handle);
                GL.TexImage2D(side, 0, PixelInternalFormat.Rgba, immage.Width, immage.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, LoadTexture(immage));
            }
            else throw new Exception("Side is not a valid texture target enum for a cube map");
            GL.TexParameterI(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Linear });
            GL.TexParameterI(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMinFilter.Linear });
            GL.TexParameterI(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, new int[] { (int)TextureParameterName.ClampToEdge });
            GL.TexParameterI(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, new int[] { (int)TextureParameterName.ClampToEdge });
            GL.TexParameterI(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, new int[] { (int)TextureParameterName.ClampToEdge });
        }
        private byte[] LoadTexture(Bitmap image)
        {
            int pixelDataSize = image.Width * image.Height * 4;
            byte[] pixelData = new byte[pixelDataSize];
            int offset = 0;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    pixelData[offset] = pixel.R;
                    pixelData[offset++] = pixel.G;
                    pixelData[offset++] = pixel.B;
                    pixelData[offset++] = pixel.A;
                }
            }
            return pixelData;
        }

    }
}