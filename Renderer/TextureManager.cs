using System;
using System.Collections.Generic;

namespace Renderer
{
    internal sealed class TextureManager
    {
        private class TextureMetaData
        {
            private readonly Texture theTexture;
            private readonly string texturename;
            internal TextureMetaData(Texture theTexture, string texturename)
            {
                this.texturename = texturename;
                this.theTexture = theTexture;
            }
            internal Texture TheTexture
            {
                get { return theTexture; }
            }
            internal string TextureName
            {
                get { return texturename; }
            }
        }
        private class TextureUnitMetaData
        {
            private int index;
            private List<int> texturesindicesinunit;
            private readonly string name;
            private bool readytouse;
            internal TextureUnitMetaData(string name)
            {
                this.index = -1;
                this.name = name;
                texturesindicesinunit = new List<int>(32);
                readytouse = false;
            }
            internal int Index
            {
                get { return index; }
                set { if (index == -1) index = value; }
            }
            internal List<int> TexturesInUnit
            {
                get { return texturesindicesinunit; }
            }
            internal bool ReadyToUse
            {
                get { return readytouse; }
            }
            internal string Name
            {
                get { return name; }
            }
            internal void AddTextureToUnit(int textureIndex)
            {
                if (this.texturesindicesinunit.Count > 31) return;
                texturesindicesinunit.Add(textureIndex);
            }
            internal void IsReady()
            {
                if (texturesindicesinunit.Count > 1) readytouse = true;
                else readytouse = false;
            }
        }
        private List<TextureMetaData> textureList = null;
        private List<TextureUnitMetaData> textureUnitList = null;
        private static Lazy<TextureManager> lazy = new Lazy<TextureManager>(() => new TextureManager());
        /// <summary>
        /// returns the instance of the texture manager
        /// </summary>
        internal static TextureManager Instance
        {
            get { return lazy.Value; }
        }
        private TextureManager()
        {
            textureList = new List<TextureMetaData>();
            textureUnitList = new List<TextureUnitMetaData>();
        }
        /// <summary>
        /// Adds at texture to the texture manager and returns an index that can be used to access the texture 
        /// </summary>
        /// <param name="texture">The texture object to add</param>
        /// <param name="texturename">The name of the texture defaults to empty string</param>
        /// <returns>index of the texture</returns>
        internal int AddTexture(Texture texture,string texturename)
        {
            if (string.IsNullOrWhiteSpace(texturename)) texturename = string.Empty;
            TextureMetaData data = new TextureMetaData(texture, texturename);
            textureList.Add(data);
            return textureList.IndexOf(data);
        }
        /// <summary>
        /// returns the texture with the specified index
        /// </summary>
        /// <param name="index">index of texture to return</param>
        /// <returns>the requested texture</returns>
        internal Texture GetTexture(int index)
        {
            return textureList[index].TheTexture;
        }
        /// <summary>
        /// returns the texture with the specified name
        /// </summary>
        /// <param name="name">the name of the texture</param>
        /// <returns>the requested texture</returns>
        internal Texture GetTexture(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("name argument is null or empty");
            for(int n = 0; n<textureList.Count;n++)
            {
                if(!string.IsNullOrEmpty(textureList[n].TextureName))
                {
                    if (textureList[n].TextureName == name) return textureList[n].TheTexture;
                }
            }
            return null;
        }
        /// <summary>
        /// Removes the texture at the given index
        /// </summary>
        /// <param name="index">index of the texture to remove</param>
        internal void RemoveTexture(int index)
        {
            if (index < 0 || index > textureList.Count) return;
            textureList.RemoveAt(index);
        }
        /// <summary>
        /// Removes the texture with the given name 
        /// </summary>
        /// <param name="name">name of texture to remove</param>
        internal void RemoveTexture(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name argument is null or empty");
            for (int n = 0; n < textureList.Count; n++)
            {
                if(!string.IsNullOrWhiteSpace(textureList[n].TextureName))
                {
                    if(textureList[n].TextureName == name)
                    {
                        textureList.RemoveAt(n);
                        return;
                    }
                }
            }
        }
        internal int CreateTextureUnit(string name)
        {
            TextureUnitMetaData newTextureUnit = new TextureUnitMetaData(name);
            textureUnitList.Add(newTextureUnit);
            int index = textureUnitList.IndexOf(newTextureUnit);
            newTextureUnit.Index = index;
            return index;
        }
        internal void AddTextureToTextureUnit(int textureunitIndex, int textureIndex)
        {
            if (textureUnitList.Count < textureunitIndex) throw new IndexOutOfRangeException("Texture Unit Index invalid");
            if (textureList.Count < textureIndex) throw new IndexOutOfRangeException("Texture index is invalid");
            if (textureUnitList[textureunitIndex].TexturesInUnit.Count > 31) throw new Exception("Max textures in one unit is 32");
            textureUnitList[textureunitIndex].AddTextureToUnit(textureIndex);
        }
        internal void AddTextureToTextureUnit(string textureUnitName,string textureName)
        {
            if (string.IsNullOrWhiteSpace(textureUnitName)) throw new ArgumentException("texture unit name is null or empty");
            if (string.IsNullOrWhiteSpace(textureName)) throw new ArgumentException("texture name is null or empty");
            int textureUnitIndex = FindTextureUnitIndexFromName(textureUnitName);
            int textureIndex = FindTextureIndexFromName(textureName);
            if (textureUnitList[textureUnitIndex].TexturesInUnit.Count > 31) throw new Exception("Max textures in one unit is 32");
            textureUnitList[textureUnitIndex].AddTextureToUnit(textureIndex);
        }
        internal void RemoveTextureUnit(int index)
        {
            textureUnitList.RemoveAt(index);
        }
        internal void RemoveTextureUnit(string name)
        {
            int index = FindTextureUnitIndexFromName(name);
            textureUnitList.RemoveAt(index);
        }
        internal int[] GetTextureUnitTextureIndices(int index)
        {
            if (index > textureUnitList.Count) throw new Exception("invalid index");
            return textureUnitList[index].TexturesInUnit.ToArray();
        }
        internal int[] GetTextureUnitTextureIndices(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is null or empty");
            int index = FindTextureUnitIndexFromName(name);
            return textureUnitList[index].TexturesInUnit.ToArray();
        }
        internal int FindTextureIndexFromName(string name)
        {
            int index = -1;
            for (int n = 0; n < textureList.Count; n++)
            {
                if(!string.IsNullOrWhiteSpace(textureList[n].TextureName))
                {
                    if (textureList[n].TextureName == name) index = n;
                }
            }
            return index;
        }
        internal int FindTextureUnitIndexFromName(string name)
        {
            int index = -1;
            for (int n = 0; n < textureUnitList.Count; n++)
            {
                if(!string.IsNullOrWhiteSpace(textureUnitList[n].Name))
                {
                    if (textureUnitList[n].Name == name) index = n;
                }
            }
            return index;
        }
    }
}