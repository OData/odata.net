//---------------------------------------------------------------------
// <copyright file="SingleResultAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Use this attribute on a DataService service operation method 
    /// to indicate than the IQueryable returned should contain a single element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SingleResultAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.SingleResultAttribute" /> class. </summary>
        public SingleResultAttribute()
        {
        }
    }
}
