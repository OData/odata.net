//---------------------------------------------------------------------
// <copyright file="HasStreamAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Services.Common
{
    using System;

    /// <summary>Indicates that a class that is an entity type has a default binary data stream. </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class HasStreamAttribute : Attribute
    {
    }
}
