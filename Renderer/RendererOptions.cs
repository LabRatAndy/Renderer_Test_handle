using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Renderer
{
    public enum TextureWrapingMode
    {
        Repeat,
        MirroredRepeat,
        ClampToEdge,
        ClampToBoarder
    }
    public enum MipmapInterpolationMode
    {
        NearestMipmapLevelNearestNeightbour,
        NearestMipmapLevelLiner,
        LinerMipmapNearestNeightbour,
        LinerMipmapLiner
    }
    public enum TextureFilteringMode
    {
        Linear,
        NearestNeighbour
    }
    /// <summary>
    /// Holds all the settings that the renderer needs in order to render the scene
    /// </summary>
    public class RendererSettings
    {
        private OpenTK.Graphics.Color4 clearColour;
        private float nearClipDistance;
        private float farClipDistance;
        private int screenheight;
        private int screenwidth;
        private TextureWrapingMode texWrappingMode = TextureWrapingMode.Repeat;
        private MipmapInterpolationMode interpolationMode = MipmapInterpolationMode.LinerMipmapLiner;
        private TextureFilteringMode filteringMode = TextureFilteringMode.NearestNeighbour;

        
        public RendererSettings()
        {
            this.clearColour = OpenTK.Graphics.Color4.Black;
            this.nearClipDistance = 0.1f;
            this.farClipDistance = 600.0f;
            this.screenheight = 640;
            this.screenwidth = 800;
        }
        /// <summary>
        /// sets the clear colour used when buffers are cleared 
        /// </summary>
        public OpenTK.Graphics.Color4 ClearColour
        {
            get { return clearColour; }
            set { clearColour = value; }
        }
        /// <summary>
        /// Sets the bit mask for the clear settings
        /// </summary>
        //public ClearBufferMask ClearBufferMask
        //{
          //  get { return clearBufferMask; }
            //set { clearBufferMask = value; }
        //}
        /// <summary>
        /// Sets the nearest distance that will be rendered by the renderer
        /// </summary>
        public float NearClipDistance
        {
            get { return nearClipDistance; }
            set { nearClipDistance = value; }
        }
        /// <summary>
        /// Sets the furthest distance that will be rendered by the renderer
        /// </summary>
        public float FarClipDistance
        {
            get { return farClipDistance; }
            set { farClipDistance = value; }
        }
        /// <summary>
        /// Height of the render window
        /// </summary>
        public int ScreenHeight
        {
            get { return screenheight; }
            set { screenheight = value; }
        }
        /// <summary>
        /// Width of the render window
        /// </summary>
        public int ScreenWidth
        {
            get { return screenwidth; }
            set { screenwidth = value; }
        }
        public TextureWrapingMode TexWrappingMode
        {
            get { return texWrappingMode; }
            set { texWrappingMode = value; }
        }
        public MipmapInterpolationMode InterpolationMode
        {
            get { return interpolationMode; }
            set { interpolationMode = value; }
        }
        public TextureFilteringMode FilteringMode
        {
            get { return filteringMode; }
            set { filteringMode = value; }
        }
        /// <summary>
        /// Runs the command to apply the setting supplied
        /// </summary>
        internal void ApplySettings()
        {
            GL.ClearColor(this.clearColour);
            switch (texWrappingMode)
            {
                case TextureWrapingMode.Repeat:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(TextureWrapMode.Repeat));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(TextureWrapMode.Repeat));
                    break;
                case TextureWrapingMode.MirroredRepeat:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(TextureWrapMode.MirroredRepeat));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(TextureWrapMode.MirroredRepeat));
                    break;
                case TextureWrapingMode.ClampToEdge:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(TextureWrapMode.ClampToEdge));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(TextureWrapMode.ClampToEdge));
                    break;
                case TextureWrapingMode.ClampToBoarder:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(TextureWrapMode.ClampToBorder));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(TextureWrapMode.ClampToBorder));
                    break;
            }
            switch(interpolationMode)
            {
                case MipmapInterpolationMode.LinerMipmapLiner:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(All.LinearMipmapLinear));
                    break;
                case MipmapInterpolationMode.LinerMipmapNearestNeightbour:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(All.LinearMipmapNearest));
                    break;
                case MipmapInterpolationMode.NearestMipmapLevelLiner:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(All.NearestMipmapLinear));
                    break;
                case MipmapInterpolationMode.NearestMipmapLevelNearestNeightbour:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, Convert.ToInt32(All.NearestMipmapNearest));
                    break;
            }
            switch(filteringMode)
            {
                case TextureFilteringMode.NearestNeighbour:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(All.Nearest));
                    break;
                case TextureFilteringMode.Linear:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, Convert.ToInt32(All.Linear));
                    break;
            }

        }
    }
}