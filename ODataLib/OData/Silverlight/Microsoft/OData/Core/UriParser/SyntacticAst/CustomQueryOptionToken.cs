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
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a query option.
    /// </summary>
    internal sealed class CustomQueryOptionToken : QueryToken
    {
        /// <summary>
        /// The name of the query option.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The value of the query option.
        /// </summary>
        private readonly string value;

        /// <summary>
        /// Create a new CustomQueryOptionToken given name and value.
        /// </summary>
        /// <param name="name">The name of the query option.</param>
        /// <param name="value">The value of the query option.</param>
        public CustomQueryOptionToken(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.CustomQueryOption; }
        }

        /// <summary>
        /// The name of the query option.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// The value of the query option.
        /// </summary>
        public string Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
