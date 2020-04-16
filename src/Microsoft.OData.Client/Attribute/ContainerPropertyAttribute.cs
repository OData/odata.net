//---------------------------------------------------------------------
// <copyright file="ContainerPropertyAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Used in a class representing an open entity or complex types 
    /// to indicate that the property should be used as the container 
    /// for dynamic properties during serialization and materialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ContainerPropertyAttribute : Attribute
    {
    }
}
