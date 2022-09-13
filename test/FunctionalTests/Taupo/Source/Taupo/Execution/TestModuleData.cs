//---------------------------------------------------------------------
// <copyright file="TestModuleData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A facade over <see cref="TestModule"/> that exposes only data
    /// about the module, instead of a way to execute it.
    /// </summary>
    [Serializable]
    public class TestModuleData : TestItemData
    {
        private TestParameterInfo[] availableParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestModuleData"/> class.
        /// </summary>
        /// <param name="testModule">The <see cref="TestModule"/> this
        /// <see cref="TestModuleData"/> class wraps.</param>
        public TestModuleData(TestModule testModule) :
            base(testModule)
        {
            ExceptionUtilities.CheckArgumentNotNull(testModule, "testModule");

            this.availableParameters = testModule.GetAvailableParameters();
        }

        /// <summary>
        /// Gets the available parameters for this test module.
        /// </summary>
        /// <returns>Array of <see cref="TestParameterInfo"/> that describes allowed test parameters for the test module.</returns>
        public TestParameterInfo[] GetAvailableParameters()
        {
            return this.availableParameters;
        }
    }
}
