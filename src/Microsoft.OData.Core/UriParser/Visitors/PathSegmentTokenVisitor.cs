//---------------------------------------------------------------------
// <copyright file="PathSegmentTokenVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
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