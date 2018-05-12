using System;

namespace Renderer
{
    public class ShaderException : Exception
    {
        public enum ExceptionType
        {
            VertexCompileError,
            FragmentCompileError,
            ShaderLinkerError
        }
        private ExceptionType type;
        private string errorText = null;
        public ShaderException(ExceptionType type, string errorText)
        {
            this.type = type;
            this.errorText = errorText;
        }
        public ExceptionType Type
        {
            get { return type; }
        }
        public string ErrorText
        {
            get { return errorText; }
        }

    }
}