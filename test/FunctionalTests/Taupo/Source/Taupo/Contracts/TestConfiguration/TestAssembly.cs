//---------------------------------------------------------------------
// <copyright file="TestAssembly.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents the assembly that a list of tests are run from
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable exists for easy creation of objects")]
    public class TestAssembly : IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the TestAssembly class.
        /// </summary>
        /// <param name="mainAssembly">Main Assembly that the test paths are in</param>
        /// <param name="platformType">Platform Type</param>
        public TestAssembly(string mainAssembly, PlatformType platformType)
        {
            ExceptionUtilities.CheckArgumentNotNull(mainAssembly, "mainAssembly");

            this.MainAssembly = mainAssembly;
            this.PlatformType = platformType;
            this.ReferencedAssemblies = new List<string>();
        }

        /// <summary>
        /// Gets the Main assembly tests are found in
        /// </summary>
        public string MainAssembly { get; private set; }

        /// <summary>
        /// Gets the Platform tests run on
        /// </summary>
        public PlatformType PlatformType { get; private set; }

        /// <summary>
        /// Gets other associated assemblies
        /// </summary>
        public IList<string> ReferencedAssemblies { get; private set; }

        /// <summary>
        /// Adds a referenced Assembly to  the list of referenced assemblies
        /// </summary>
        /// <param name="referencedAssembly">reference assembly to add</param>
        public void Add(string referencedAssembly)
        {
            ExceptionUtilities.CheckArgumentNotNull(referencedAssembly, "referencedAssembly");

            this.ReferencedAssemblies.Add(referencedAssembly);
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw new TaupoNotSupportedException(ExceptionUtilities.EnumerableNotImplementedExceptionMessage);
        }
    }
}
