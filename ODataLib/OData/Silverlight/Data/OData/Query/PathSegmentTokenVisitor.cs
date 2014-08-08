//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System;

    /// <summary>
    /// Visitor interface for walking the Syntactic Tree.
    /// </summary>
    /// <typeparam name="T">Generic type produced by the visitor.</typeparam>
    internal abstract class PathSegmentTokenVisitor<T> : IPathSegmentTokenVisitor<T>
    {
        /// <summary>
        /// Visit an SystemToken
        /// </summary>
        /// <param name="tokenIn">The System token to visit</param>
        /// <returns>A user defined class</returns>
        public virtual T Visit(SystemToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an NonSystemToken
        /// </summary>
        /// <param name="tokenIn">The System token to visit</param>
        /// <returns>A user defined class</returns>
        public virtual T Visit(NonSystemToken tokenIn)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Visitor interface for walking the Syntactic Tree.
    /// </summary>
    internal abstract class PathSegmentTokenVisitor : IPathSegmentTokenVisitor
    {
        /// <summary>
        /// Visit an SystemToken
        /// </summary>
        /// <param name="tokenIn">The System token to visit</param>
        public virtual void Visit(SystemToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an NonSystemToken
        /// </summary>
        /// <param name="tokenIn">The System token to visit</param>
        public virtual void Visit(NonSystemToken tokenIn)
        {
            throw new NotImplementedException();
        }
    }
}
