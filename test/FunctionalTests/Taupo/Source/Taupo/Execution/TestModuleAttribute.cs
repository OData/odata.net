//---------------------------------------------------------------------
// <copyright file="TestModuleAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;

    /// <summary>
    /// Annotates a class to be a top-level test module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TestModuleAttribute : TestItemBaseAttribute
    {
    }
}
