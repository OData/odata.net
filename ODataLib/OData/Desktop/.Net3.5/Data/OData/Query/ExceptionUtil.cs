//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
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
            DebugUtils.CheckNoExternalCallers();

            // 404: Not Found
            return ResourceNotFoundError(Strings.RequestUriProcessor_ResourceNotFound(identifier));
        }

        /// <summary>Creates a new "Resource Not Found" exception.</summary>
        /// <param name="errorMessage">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate the requested resource cannot be found.</returns>
        internal static ODataException ResourceNotFoundError(string errorMessage)
        {
            DebugUtils.CheckNoExternalCallers();

            // 404: Not Found
            return new ODataUnrecognizedPathException(errorMessage);
        }

        /// <summary>Creates a new exception to indicate a syntax error.</summary>
        /// <returns>A new exception to indicate a syntax error.</returns>
        internal static ODataException CreateSyntaxError()
        {
            DebugUtils.CheckNoExternalCallers();
            return CreateBadRequestError(Strings.RequestUriProcessor_SyntaxError);
        }

        /// <summary>
        /// Creates a new exception to indicate BadRequest error.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <returns>A new exception to indicate a bad request error.</returns>
        internal static ODataException CreateBadRequestError(string message)
        {
            DebugUtils.CheckNoExternalCallers();

            // 400 - Bad Request
            return new ODataException(message);
        }

        /// <summary>Checks the specific value for syntax validity.</summary>
        /// <param name="valid">Whether syntax is valid.</param>
        /// <remarks>This helper method is used to keep syntax check code more terse.</remarks>
        internal static void ThrowSyntaxErrorIfNotValid(bool valid)
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            if (!resourceExists)
            {
                throw CreateResourceNotFound(identifier);
            }
        }
    }
}
