//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Visitors
{
    using System;
    using Microsoft.OData.Core.UriParser.Syntactic;

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
