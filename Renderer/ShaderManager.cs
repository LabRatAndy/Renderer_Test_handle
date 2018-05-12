using System;
using System.Collections.Generic;
namespace Renderer
{
    /// <summary>
    /// A singleton class to manage shaders for the renderer need to access the Instance parameter to create and/or get the only instance
    /// </summary>
    internal sealed class ShaderManager
    {
        /// <summary>
        /// class to hold metadata about the shaders like is it initalised more could be added later
        /// </summary>
        private class MetaShaderData
        {
            private readonly Shader theShader;
            private readonly bool isInitalised;
            private readonly string shaderName;
            internal MetaShaderData(Shader theShader,bool isInitialised, string shaderName)
            {
                this.theShader = theShader;
                this.isInitalised = isInitialised;
                this.shaderName = shaderName;
            }
            internal Shader TheShader
            {
                get { return theShader; }
            }
            internal bool IsInitialised
            {
                get { return isInitalised; }
            }
            internal string ShaderName
            {
                get { return shaderName; }
            }
        }
        //member variables
        private List<MetaShaderData> shaderList = null;
        private bool debugMode = false;
        private static readonly Lazy<ShaderManager> lazy = new Lazy<ShaderManager>(() => new ShaderManager());
        /// <summary>
        /// returnes the instance of the shadermanager object note there is only one
        /// </summary>
        internal static ShaderManager Instance
        {
            get { return lazy.Value; }
        }
        private ShaderManager()
        {
            shaderList = new List<MetaShaderData>();
        }
        /// <summary>
        /// Method to add a shader 
        /// </summary>
        /// <param name="newShader">The shader object to add</param>
        /// <param name="isInitialised">has the shader been initialsised or not default true</param>
        /// <param name="shaderName">A string that is the name of the shader default is null</param>
        /// <returns>the index assigned by the shader manager</returns>
        internal int AddShader(Shader newShader, bool isInitialised = true,string shaderName = null)
        {
            MetaShaderData shaderData = new MetaShaderData(newShader, isInitialised, shaderName);
            shaderList.Add(shaderData);
            return shaderList.IndexOf(shaderData);
        }
        /// <summary>
        /// Returns the shader specified by the index 
        /// </summary>
        /// <param name="index">the index of the shader as returned by AddShader method</param>
        /// <returns>the shader object or null if an invalid index provided or the shader is not initialised</returns>
        internal Shader GetShader(int index)
        {
            if (index < 0 || index > shaderList.Count) return null;
            if (shaderList[index].IsInitialised == false) return null;
            return shaderList[index].TheShader;
        }
        /// <summary>
        /// Returns the shader of the specified name
        /// </summary>
        /// <param name="name">string with the name of the shader to return</param>
        /// <returns>the shader object or null if the shader does not exist of the shader is not initialised</returns>
        internal Shader GetShader(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            foreach(var item in shaderList)
            {
                if (item.ShaderName == name)
                {
                    return item.TheShader;
                }
            }
            return null;
        }
        /// <summary>
        /// removes the shader that is at the index provided invalid values will just be ignored 
        /// </summary>
        /// <param name="index">the index of the shader as returned by the AddShader method</param>
        internal void RemoveShader(int index)
        {
            shaderList.RemoveAt(index);
        }
        /// <summary>
        /// remove the shader of the name provided 
        /// </summary>
        /// <param name="name">the name of the shader to remove</param>
        internal void RemoveShader(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) return;
            int n;
            for (n = 0; n < shaderList.Count; n++)
            {
                if(shaderList[n].ShaderName == name)
                {
                    shaderList.RemoveAt(n);
                    return;
                }
            }
        }
        /// <summary>
        /// Initialise a shader for use if not already initialised
        /// </summary>
        /// <param name="index">the index of the shader to initailese as given by the addshader method </param>
        /// <returns>returns true if successful or the shader is already initialied</returns>
        internal bool InitialiseAShader(int index)
        {
            if (shaderList[index].IsInitialised) return true;
            return shaderList[index].TheShader.InitialiseShader();
        }
        /// <summary>
        /// Initialise a Shader for use if not already initialised
        /// </summary>
        /// <param name="name">the name of the shader to initialise</param>
        /// <returns>returns true if successful or the shader is already initialied</returns>
        internal bool InitialiseAShader(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            int n;
            for (n = 0; n < shaderList.Count; n++)
            {
                if (shaderList[n].ShaderName == name) break;
            }
            if (shaderList[n].IsInitialised) return true;
            return shaderList[n].TheShader.InitialiseShader();
        }
        /// <summary>
        /// Initialises all Shaders for use if they are not already
        /// </summary>
        /// <returns>returns true if successful or the shader is already initialied</returns>
        internal bool InitaliseAllShaders()
        {
            for (int n = 0; n < shaderList.Count; n++)
            {
                if(shaderList[n].IsInitialised)
                {
                    if (shaderList[n].TheShader.InitialiseShader() == false) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Finds the shader name feom a given index
        /// </summary>
        /// <param name="index">the index of the shader as given by addshader method</param>
        /// <returns>the name of the shader in the metadata object returns null if no name associated or invalid index given</returns>
        internal string GetShaderName(int index)
        {
            if (index < 0 || index > shaderList.Count) return null;
            return shaderList[index].ShaderName;
        }
        /// <summary>
        /// Finds the shader index from a shader name
        /// </summary>
        /// <param name="name">the name of the shader to find </param>
        /// <returns>index of the shader -1 if name not found</returns>
        internal int GetShaderIndexFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return -1;
            for (int n = 0; n < shaderList.Count; n++)
            {
                if(shaderList[n].ShaderName == name)
                {
                    return n;
                }
            }
            return -1;
        }
        /// <summary>
        /// Turns throwing a shader compilation exception on or off
        /// </summary>
        internal bool DebugMode
        {
            get { return debugMode; }
            set { debugMode = value; }
        }
    }
}