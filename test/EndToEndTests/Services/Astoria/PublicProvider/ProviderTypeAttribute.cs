//---------------------------------------------------------------------
// <copyright file="ProviderTypeAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System;

    /// <summary>
    /// A custom attribute to mark whhich provider the service operation belongs to
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ProviderTypeAttribute : Attribute
    {
        public string Type { get; set; }
    }
}
