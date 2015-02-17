//---------------------------------------------------------------------
// <copyright file="TestMatrixNamedValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Annotation that allows custom primitive name and value
    /// </summary>
    public class TestMatrixNamedValueAnnotation : TestConfigurationMatrixAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the TestMatrixNamedValueAnnotation class.
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="value">Value of the custom parameter</param>
        public TestMatrixNamedValueAnnotation(string name, string value)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(value, "value");

            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the Value of the annotation
        /// </summary>
        public string Value { get; private set; }
    }
}
