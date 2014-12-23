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

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// The class representing the URI Template parsing result.
    /// </summary>
    /// <remarks>
    /// A URI Template is a string wrapped in brackets, it is capable for describing a range of Uniform Resource Identifiers.
    /// The URI Template can be used for both building and parsing URI.
    /// Set the EnableUriTemplateParsing property of <see cref="ODataUriParser"/> to true, to enable parsing the Uri template.
    /// 
    /// For example, in the following URI
    /// http://example.org/service/Customers({CID}) 
    /// {CID} is a validate Uri template for Customers' ID segment, in the parsing result of <see cref="UriTemplateExpression"/>,
    /// <see cref="LiteralText"/> would be the original literal "{CID}", and <see cref="ExpectedType"/> would be the actual type for Customers' ID.
    /// 
    /// See RFC6570 for detail.
    /// </remarks>
    public sealed class UriTemplateExpression
    {
        /// <summary>
        /// The original text for the Uri template.
        /// </summary>
        public string LiteralText { get; internal set; }

        /// <summary>
        /// The expected type of the object which the Uri template stands for.
        /// </summary>
        public IEdmTypeReference ExpectedType { get; internal set; }
    }
}
