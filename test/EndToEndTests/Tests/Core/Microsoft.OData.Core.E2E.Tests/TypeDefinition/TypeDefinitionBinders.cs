//---------------------------------------------------------------------
// <copyright file="TypeDefinitionBinders.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq.Expressions;
using Microsoft.AspNetCore.OData.Query.Expressions;

namespace Microsoft.OData.Core.E2E.Tests.TypeDefinition;

public class CustomFilterBinder : FilterBinder
{
    /// This method overrides the base implementation to handle the "LifeTimeInSeconds" property specifically:
    /// - If the property being accessed is "LifeTimeInSeconds", it returns the property access expression without converting it to Int64.
    /// - For other properties, it uses the default behavior from the base class.
    /// </remarks>
    public override Expression BindPropertyAccessQueryNode(UriParser.SingleValuePropertyAccessNode propertyAccessNode, QueryBinderContext context)
    {
        // Check if the property being accessed is "LifeTimeInSeconds"
        if (propertyAccessNode.Property.Name == "LifeTimeInSeconds")
        {
            // Return the property access expression without converting it to Int64
            return Expression.Property(
                base.Bind(propertyAccessNode.Source, context),
                propertyAccessNode.Property.Name
            );
        }

        // For other properties, use the default behavior
        return base.BindPropertyAccessQueryNode(propertyAccessNode, context);
    }
}

public class CustomOrderByBinder : OrderByBinder
{
    /// <remarks>
    /// This method overrides the base implementation to handle the "LifeTimeInSeconds" property specifically:
    /// - If the property being accessed is "LifeTimeInSeconds", it returns the property access expression without converting it to Int64.
    /// - For other properties, it uses the default behavior from the base class.
    /// </remarks>
    public override Expression BindPropertyAccessQueryNode(UriParser.SingleValuePropertyAccessNode propertyAccessNode, QueryBinderContext context)
    {
        // Check if the property being accessed is "LifeTimeInSeconds"
        if (propertyAccessNode.Property.Name == "LifeTimeInSeconds")
        {
            // Return the property access expression without converting it to Int64
            return Expression.Property(
                base.Bind(propertyAccessNode.Source, context),
                propertyAccessNode.Property.Name
            );
        }

        // For other properties, use the default behavior
        return base.BindPropertyAccessQueryNode(propertyAccessNode, context);
    }
}
