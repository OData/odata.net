//---------------------------------------------------------------------
// <copyright file="ResourceExpressionType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    /// <summary>Enum for resource expression types</summary>
    internal enum ResourceExpressionType
    {
        /// <summary>ResourceSet Expression</summary>
        RootResourceSet = 10000,

        /// <summary>Single resource expression, used to represent singleton.</summary>
        RootSingleResource,

        /// <summary>Resource Navigation Expression</summary>
        ResourceNavigationProperty,

        /// <summary>Resource Navigation Expression to Singleton</summary>
        ResourceNavigationPropertySingleton,

        /// <summary>Take Query Option Expression</summary>
        TakeQueryOption,

        /// <summary>Skip Query Option Expression</summary>
        SkipQueryOption,

        /// <summary>OrderBy Query Option Expression</summary>
        OrderByQueryOption,

        /// <summary>Filter Query Option Expression</summary>
        FilterQueryOption,

        /// <summary>Reference to a bound component of the resource set path</summary>
        InputReference,

        /// <summary>Projection Query Option Expression</summary>
        ProjectionQueryOption,

        /// <summary>Expand Query Option Expression</summary>
        ExpandQueryOption,

        /// <summary>Apply Query Option Expression</summary>
        ApplyQueryOption
    }
}
