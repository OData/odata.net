//---------------------------------------------------------------------
// <copyright file="DataServiceEntityAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace System.Data.Services.Common
{
    using System;

    /// <summary>Marks a class as an entity type in WCF Data Services.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DataServiceEntityAttribute : System.Attribute
    {
        /// <summary>Creates a new instance of the <see cref="T:System.Data.Services.Common.DataServiceEntityAttribute" /> class.</summary>
        public DataServiceEntityAttribute()
        {
        }
    }
}
