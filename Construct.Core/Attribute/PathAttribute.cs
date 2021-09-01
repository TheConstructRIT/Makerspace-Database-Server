using System;

namespace Construct.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PathAttribute : System.Attribute
    {
        /// <summary>
        /// Path of the request handler.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Creates the Path attribute.
        /// </summary>
        /// <param name="path">Path of the request handler.</param>
        public PathAttribute(string path)
        {
            this.Path = path;
        }
    }
}