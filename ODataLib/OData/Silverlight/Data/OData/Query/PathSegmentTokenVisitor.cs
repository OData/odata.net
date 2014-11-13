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
