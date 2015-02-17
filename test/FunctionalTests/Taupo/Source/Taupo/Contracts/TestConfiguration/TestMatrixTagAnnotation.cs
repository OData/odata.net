//---------------------------------------------------------------------
// <copyright file="TestMatrixTagAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Annotation for a Tag
    /// </summary>
    public class TestMatrixTagAnnotation : TestConfigurationMatrixAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the TestMatrixTagAnnotation class.
        /// </summary>
        /// <param name="tag">Name of Tag</param>
        public TestMatrixTagAnnotation(string tag)
        {
            ExceptionUtilities.CheckArgumentNotNull(tag, "tag");
            
            this.Tag = tag;
        }

        /// <summary>
        /// Gets the Tag
        /// </summary>
        public string Tag { get; private set; }
    }
}
