//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
