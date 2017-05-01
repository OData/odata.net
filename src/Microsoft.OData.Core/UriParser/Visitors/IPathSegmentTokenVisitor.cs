//---------------------------------------------------------------------
// <copyright file="IPathSegmentTokenVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// Visitor interface for walking the Path Tree.
    /// </summary>
    /// <typeparam name="T">Return type for the visitor methods on this visitor.</typeparam>
    public interface IPathSegmentTokenVisitor<T>
    {
        /// <summary>
        /// Visit an SystemToken
        /// </summary>
        /// <param name="tokenIn">The SystemToken to visit</param>
        /// <returns>A user defined class</returns>
        T Visit(SystemToken tokenIn);

        /// <summary>
        /// Visit an NonSystemToken
        /// </summary>
        /// <param name="tokenIn">The NonSystemToken to visit</param>
        /// <returns>A user defined class</returns>
        T Visit(NonSystemToken tokenIn);
    }

    /// <summary>
    /// Visitor interface for walking the Path Tree.
    /// </summary>
    public interface IPathSegmentTokenVisitor
    {
        /// <summary>
        /// Visit an SystemToken
        /// </summary>
        /// <param name="tokenIn">The SystemToken to visit</param>
        void Visit(SystemToken tokenIn);

        /// <summary>
        /// Visit an NonSystemToken
        /// </summary>
        /// <param name="tokenIn">The NonSystemToken to visit</param>
        void Visit(NonSystemToken tokenIn);
    }
}