﻿//---------------------------------------------------------------------
// <copyright file="SearchFieldAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.TestCommon.Helpers
{
    /// <summary>
    /// Adds the attribute on the properties which support $search.
    /// </summary>
    /// <remarks>
    /// DOES NOT add the attribute on the navigation properties. We do not support this scenario now. See <see cref="SearchHelper"/> for more.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class SearchFieldAttribute : Attribute
    {
    }
}
