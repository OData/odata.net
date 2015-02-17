//---------------------------------------------------------------------
// <copyright file="AssemblyCleanup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public static class AssemblyCleanup
    {
        [AssemblyCleanup]
        public static void Cleanup()
        {
            LocalWebServerHelper.AssemblyCleanup();
        }
    }
}
