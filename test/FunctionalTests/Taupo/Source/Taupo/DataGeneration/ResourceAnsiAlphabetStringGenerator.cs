//---------------------------------------------------------------------
// <copyright file="ResourceAnsiAlphabetStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Reflection;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Generates ANSI strings based on ANSI alphabet defined in an embedded resource.
    /// </summary>
    internal class ResourceAnsiAlphabetStringGenerator : ResourceAlphabetStringGenerator, IAnsiStringGenerator
    {
        /// <summary>
        /// Initializes a new instance of the ResourceAnsiAlphabetStringGenerator class.
        /// </summary>
        /// <param name="assembly">The assembly containing the resource.</param>
        /// <param name="resourceName">Name of the resource.</param>
        public ResourceAnsiAlphabetStringGenerator(Assembly assembly, string resourceName)
            : base(assembly, resourceName)
        {
        }
    }
}
