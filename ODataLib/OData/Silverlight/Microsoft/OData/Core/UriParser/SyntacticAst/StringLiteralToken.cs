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
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a string literal value.
    /// </summary>
    [DebuggerDisplay("StringLiteralToken ({text})")]
    internal sealed class StringLiteralToken : QueryToken
    {
        /// <summary>
        /// Raw text value for this token.
        /// </summary>
        private readonly string text;

        /// <summary>
        /// Constructor for the StringLiteralToken
        /// </summary>
        /// <param name="text">The text value for this token</param>
        internal StringLiteralToken(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.StringLiteral; }
        }

        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        internal string Text
        {
            get 
            {
                return this.text;
            }
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
