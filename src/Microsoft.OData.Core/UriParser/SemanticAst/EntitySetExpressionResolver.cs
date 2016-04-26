//---------------------------------------------------------------------
// <copyright file="EntitySetExpressionResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class that knows how to resolve an IEdmExpression to find its associated EntitySet.
    /// This functionality is needed to determine what a EntitySets a FunctionImport applies to.
    /// </summary>
    internal static class EntitySetExpressionResolver
    {
        /// <summary>
        /// Resolves an IEdmExpression to an IEdmEntitySet.
        /// </summary>
        /// <param name="expression">Expression to resolve.</param>
        /// <returns>The resolved EntitySet.</returns>
        internal static IEdmEntitySet ResolveEntitySetFromExpression(IEdmExpression expression)
        {
            if (expression == null)
            {
                return null;
            }

            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.EntitySetReference:
                    return ((IEdmEntitySetReferenceExpression)expression).ReferencedEntitySet;
                default:
                    // TODO: we should support all the other options
                    throw new NotSupportedException(
                        ODataErrorStrings.Nodes_NonStaticEntitySetExpressionsAreNotSupportedInThisRelease);
            }
        }
    }
}