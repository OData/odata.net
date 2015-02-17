//---------------------------------------------------------------------
// <copyright file="ToCompareAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;

    /// <summary>
    /// To compare attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ToCompareAttribute : Attribute
    {
    }
}
