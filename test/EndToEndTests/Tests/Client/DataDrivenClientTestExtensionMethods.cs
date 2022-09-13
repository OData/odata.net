//---------------------------------------------------------------------
// <copyright file="DataDrivenClientTestExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.Test.DataDriven;
    using Microsoft.Test.OData.Framework.Client;

    /// <summary>
    /// Extension methods for running multi-variation client tests.
    /// </summary>
    public static class DataDrivenClientTestExtensionMethods
    {
        /// <summary>
        /// Runs a client test with both Atom and Json OData formats.
        /// </summary>
        /// <typeparam name="TContext">The DataServiceContext type.</typeparam>
        /// <param name="testBase">The test class running the test.</param>
        /// <param name="createContext">A delegate to create the context.</param>
        /// <param name="test">The test action to execute.</param>
        public static void RunOnAtomAndJsonFormats<TContext>(
            this EndToEndTestBase testBase,
            Func<DataServiceContextWrapper<TContext>> createContext,
            Action<DataServiceContextWrapper<TContext>> test) where TContext : DataServiceContext
        {
            var jsonContext = createContext();
            jsonContext.ContextLabel = "Json";
            jsonContext.Format.UseJson();

            testBase.InvokeDataDrivenTest(test, DataDrivenTest.CreateData(jsonContext));
        }
    }
}
