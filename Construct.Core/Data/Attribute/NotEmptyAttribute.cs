using System;

namespace Construct.Core.Data.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotEmptyAttribute : System.Attribute
    {
        /// <summary>
        /// Response to use when the property is empty.
        /// </summary>
        public string Response { get; }

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="response">Response to use when the property is empty.</param>
        public NotEmptyAttribute(string response)
        {
            this.Response = response;
        }
    }
}