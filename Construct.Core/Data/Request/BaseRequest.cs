using System.Reflection;
using Construct.Core.Data.Attribute;
using Construct.Core.Data.Response;

namespace Construct.Core.Data.Request
{
    public abstract class BaseRequest
    {
        /// <summary>
        /// Returns a validation error response for the request if one exists.
        /// If none exists, null is returned.
        /// </summary>
        /// <returns>Returns a validation error response.</returns>
        public IResponse GetValidationErrorResponse()
        {
            // Iterate over the properties and return an error for empty non-empty properties.
            foreach (var property in this.GetType().GetProperties())
            {
                // Return if a non-empty constraint is violated.
                var nonEmptyAttribute = property.GetCustomAttribute<NotEmptyAttribute>(true);
                var value = property.GetValue(this);
                if (nonEmptyAttribute != null && (Equals(value, "") || Equals(value, null)))
                {
                    return new GenericStatusResponse(nonEmptyAttribute.Response);
                }
            }
            
            // Return null (no validation error).
            return null;
        }
    }
}