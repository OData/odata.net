//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.UriParser
{
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Helper class for throwing exceptions during URI parsing.
    /// </summary>
    internal static class ExceptionUtil
    {
        /// <summary>Creates a new "Resource Not Found" exception.</summary>
        /// <param name="identifier">segment identifier information for which resource was not found.</param>
        /// <returns>A new exception to indicate the requested resource cannot be found.</returns>
        internal static ODataException CreateResourceNotFoundError(string identifier)
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

        /// <summary>
        /// Throws if the type is not related to the type of the given set.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="secondType">Second type, which should be related to the first type.</param>
        /// <param name="segmentName">The segment that is checking this.</param>
        internal static void ThrowIfTypesUnrelated(IEdmType type, IEdmType secondType, string segmentName)
        {
            if (!UriEdmHelpers.IsRelatedTo(type.AsElementType(), secondType))
            {
                throw new ODataException(Strings.PathParser_TypeMustBeRelatedToSet(type, secondType, segmentName));
            }
        }
    }
}
