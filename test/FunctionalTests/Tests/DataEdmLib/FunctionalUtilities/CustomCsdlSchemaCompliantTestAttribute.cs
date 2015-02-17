//---------------------------------------------------------------------
// <copyright file="CustomCsdlSchemaCompliantTestAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class CustomCsdlSchemaCompliantTestAttribute : Attribute
    {
    }
}
