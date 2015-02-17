//---------------------------------------------------------------------
// <copyright file="ApiWordsAnsiStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Reflection;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Implementation of <see cref="IAnsiStringGenerator"/> which generates ANSI strings consisting of API words.
    /// </summary>
    public class ApiWordsAnsiStringGenerator : ApiWordsStringGenerator, IAnsiStringGenerator
    {
        /// <summary>
        /// Initializes a new instance of the ApiWordsAnsiStringGenerator class.
        /// </summary>
        /// <param name="assemblies">The assemblies to use to initialize the word pool.</param>
        public ApiWordsAnsiStringGenerator(params Assembly[] assemblies)
            : base(assemblies)
        {
        }
    }
}
