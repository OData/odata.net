//---------------------------------------------------------------------
// <copyright file="ValidationTestInvalidModelAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests;

[AttributeUsage(AttributeTargets.Method)]
public class ValidationTestInvalidModelAttribute : Attribute
{
}
