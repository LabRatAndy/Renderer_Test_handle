using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Renderer
{
    public class Shader
    {
        private string vertexShaderPath;
        private string fragmentShaderPath;
        private int programID;

        public Shader(string vertexShaderPath, string fragmentShaderPath)
        {
            this.vertexShaderPath = vertexShaderPath;
            this.fragmentShaderPath = fragmentShaderPath;
        }
        public bool InitialiseShader()
        {
            string shaderCode = null;
            try
            {
                shaderCode = System.IO.File.ReadAllText(vertexShaderPath);
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
                return false;
            }
            int sucess;
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, shaderCode);
            GL.CompileShader(vertex);
            GL.GetShader(vertex, ShaderParameter.CompileStatus, out sucess);
            if (sucess == 0)
            {

                string error;
                GL.GetShaderInfoLog(vertex, out error);
#if DEBUG
                Debug.WriteLine(error);
#endif
                if(ShaderManager.Instance.DebugMode == true)
                {
                    throw new ShaderException(ShaderException.ExceptionType.VertexCompileError, error);
                }
                return false;
            }
           sucess = 0;
            shaderCode = null;
            try
            {
                shaderCode = System.IO.File.ReadAllText(fragmentShaderPath);
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
                return false;
            }
            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, shaderCode);
            GL.CompileShader(fragment);
            GL.GetShader(fragment, ShaderParameter.CompileStatus, out sucess);
            if (sucess == 0)
            {

                string error;
                GL.GetShaderInfoLog(fragment, out error);
#if DEBUG
                Debug.WriteLine(error);
#endif
                if(ShaderManager.Instance.DebugMode == true)
                {
                    throw new ShaderException(ShaderException.ExceptionType.FragmentCompileError, error);
                }
                return false;
            }
            sucess = 0;
            shaderCode = null;
            programID = GL.CreateProgram();
            GL.AttachShader(programID, vertex);
            GL.AttachShader(programID, fragment);
            GL.LinkProgram(programID);
            GL.GetProgram(programID, GetProgramParameterName.LinkStatus, out sucess);
            if(sucess == 0)
            {

                string error;
                GL.GetProgramInfoLog(programID,out error);
#if DEBUG
                Debug.WriteLine(error);
#endif
                if (ShaderManager.Instance.DebugMode == true)
                {
                    throw new ShaderException(ShaderException.ExceptionType.ShaderLinkerError, error);
                }
                return false;
            }
            GL.DetachShader(programID, vertex);
            GL.DetachShader(programID, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
            fragmentShaderPath = null;
            vertexShaderPath = null;
            return true;
        }
        public void Use()
        {
            GL.UseProgram(programID);
        }
        public int GetAttributeLocation(string name)
        {
            return GL.GetAttribLocation(programID, name);
        }
        public int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(programID, name);
        }
        /// <summary>
        /// deletes the shader program to hopefully prevent memory leaks on the openGL side
        /// </summary>
        public void Dispose()
        {
            GL.DeleteProgram(programID);
        }
    }
}
