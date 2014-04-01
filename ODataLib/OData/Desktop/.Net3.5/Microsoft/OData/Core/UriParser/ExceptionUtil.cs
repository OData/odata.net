//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    /// <summary>
    /// Helper class for throwing exceptions during URI parsing.
    /// </summary>
    internal static class ExceptionUtil
    {
        /// <summary>Creates a new "Resource Not Found" exception.</summary>
        /// <param name="identifier">segment identifier information for which resource was not found.</param>
        /// <returns>A new exception to indicate the requested resource cannot be found.</returns>
        internal static ODataException CreateResourceNotFound(string identifier)
        {
            // 404: Not Found
            return ResourceNotFoundError(Strings.RequestUriProcessor_ResourceNotFound(identifier));
        }

        /// <summary>Creates a new "Resource Not Found" exception.</summary>
        /// <param name="errorMessage">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate the requested resource cannot be found.</returns>
        internal static ODataException ResourceNotFoundError(string errorMessage)
        {
            // 404: Not Found
            return new ODataUnrecognizedPathException(errorMessage);
        }

        /// <summary>Creates a new exception to indicate a syntax error.</summary>
        /// <returns>A new exception to indicate a syntax error.</returns>
        internal static ODataException CreateSyntaxError()
        {
            return CreateBadRequestError(Strings.RequestUriProcessor_SyntaxError);
        }

        /// <summary>
        /// Creates a new exception to indicate BadRequest error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate a bad request error.</returns>
        internal static ODataException CreateBadRequestError(string message)
        {
            // 400 - Bad Request
            return new ODataException(message);
        }

        /// <summary>Checks the specific value for syntax validity.</summary>
        /// <param name="valid">Whether syntax is valid.</param>
        /// <remarks>This helper method is used to keep syntax check code more terse.</remarks>
        internal static void ThrowSyntaxErrorIfNotValid(bool valid)
        {
            if (!valid)
            {
                throw CreateSyntaxError();
            }
        }

        /// <summary>Checks the specifid value for syntax validity.</summary>
        /// <param name="resourceExists">Whether syntax is valid.</param>
        /// <param name="identifier">segment indentifier for which the resource was null.</param>
        /// <remarks>This helper method is used to keep syntax check code more terse.</remarks>
        internal static void ThrowIfResourceDoesNotExist(bool resourceExists, string identifier)
        {
            if (!resourceExists)
            {
                throw CreateResourceNotFound(identifier);
            }
        }
    }
}
