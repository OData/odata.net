//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
