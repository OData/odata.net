//---------------------------------------------------------------------
// <copyright file="TestInitializeAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if SILVERLIGHT
namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    using System;

    /// <summary>
    /// Stub attribute so that MStest attributes files compile
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestInitializeAttribute : Attribute
    {
    }
}
#endif
