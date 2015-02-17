//---------------------------------------------------------------------
// <copyright file="GeneratedCode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Base class for all code generated using <see cref="CodeBuilder"/>.
    /// </summary>
    public abstract class GeneratedCode
    {
        /// <summary>
        /// Initializes a new instance of the GeneratedCode class.
        /// </summary>
        protected GeneratedCode()
        {
            this.Log = Logger.Null;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        [InjectDependency]
        public Logger Log { get; set; }

        /// <summary>
        /// Gets or sets the assertion handler.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }
    }
}
