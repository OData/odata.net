//---------------------------------------------------------------------
// <copyright file="NavigationPropertyInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Class is used for Reflection Provider to give more information on navigation properties found on an EntityType
    /// </summary>
    internal class NavigationPropertyInfo
    {
        /// <summary>
        /// Initializes a new instance of the NavigationPropertyInfo class
        /// </summary>
        /// <param name="pi">PropertyInfo of the navigation property</param>
        /// <param name="collectionElementType">If its a collection it gives the element type, if its not generic but is a collection, object should be used</param>
        internal NavigationPropertyInfo(PropertyInfo pi, Type collectionElementType)
        {
            this.PropertyInfo = pi;
            this.CollectionElementType = collectionElementType;
        }

        /// <summary>
        /// Gets the PropertyInfo of the navigation property
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Gets the CollectionElementType of the navigation property
        /// </summary>
        public Type CollectionElementType { get; private set; }
    }
}