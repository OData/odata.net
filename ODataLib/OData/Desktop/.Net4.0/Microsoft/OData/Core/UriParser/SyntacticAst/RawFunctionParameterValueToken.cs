//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    using System;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    /// <summary>
    /// A token to represent a raw function parameter value that has not yet been parsed further.
    /// </summary>
    internal sealed class RawFunctionParameterValueToken : QueryToken
    {
        /// <summary>
        /// Creates a RawFunctionParameterValue
        /// </summary>
        /// <param name="rawText">the raw text of this parameter value.</param>
        public RawFunctionParameterValueToken(string rawText)
        {
            this.RawText = rawText;
        }

        /// <summary>
        /// Gets the raw text of the value.
        /// </summary>
        public string RawText { get; private set; }

        /// <summary>
        /// Gets the kind of this token
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.RawFunctionParameterValue; }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}
