//---------------------------------------------------------------------
// <copyright file="ETagFieldAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.E2E.TestCommon.Common.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ETagFieldAttribute : Attribute
{
}
