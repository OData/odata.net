//---------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Helper class for T4 Template, provide uniform API for different platforms
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// When overridden in a derived class, returns an array of custom attributes
        /// applied to this member and identified by System.Type.
        /// </summary>
        /// <param name="type">
        /// The type to query custom attributes on.
        /// </param>
        /// <param name="attributeType">
        /// The type of attribute to search for. Only attributes that are assignable
        /// to this type are returned.
        /// </param>
        /// <param name="inherit">
        /// true to search this member's inheritance chain to find the attributes; otherwise,
        /// false. This parameter is ignored for properties and events; see Remarks.
        /// </param>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerable{T}" /> of custom attributes applied to this member, or an array with zero
        /// elements if no attributes assignable to attributeType have been applied.
        /// </returns>
        public static IEnumerable<object> GetCustomAttributes(Type type, Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit);
        }
    }
}
