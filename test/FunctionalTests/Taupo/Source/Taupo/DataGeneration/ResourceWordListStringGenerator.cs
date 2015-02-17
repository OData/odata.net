//---------------------------------------------------------------------
// <copyright file="ResourceWordListStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Generates strings based word list in an embedded resource.
    /// </summary>
    internal class ResourceWordListStringGenerator : AlphabetStringGeneratorBase
    {
        private string[] alphabet;
        private string resourceName;
        private Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the ResourceWordListStringGenerator class.
        /// </summary>
        /// <param name="assembly">The assembly containing the resource.</param>
        /// <param name="resourceName">Name of the resource.</param>
        public ResourceWordListStringGenerator(Assembly assembly, string resourceName)
        {
            ExceptionUtilities.CheckArgumentNotNull(assembly, "assembly");
            ExceptionUtilities.CheckArgumentNotNull(resourceName, "resourceName");

            this.assembly = assembly;
            this.resourceName = resourceName;
        }

        /// <summary>
        /// Gets the alphabet to use for string generation.
        /// </summary>
        /// <returns>
        /// Array of strings comprising the alphabet.
        /// </returns>
        protected override string[] GetAlphabet()
        {
            this.EnsureAlphabetLoaded();

            return this.alphabet;
        }

        /// <summary>
        /// Ensures that the alphabet has been loaded.
        /// </summary>
        private void EnsureAlphabetLoaded()
        {
            if (this.alphabet == null)
            {
                using (Stream stream = this.assembly.GetManifestResourceStream(this.resourceName))
                {
                    StreamReader streamReader = new StreamReader(stream);
                    List<string> allLines = new List<string>();
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        allLines.Add(line);
                    }

                    this.alphabet = allLines.ToArray();
                }
            }
        }
    }
}
