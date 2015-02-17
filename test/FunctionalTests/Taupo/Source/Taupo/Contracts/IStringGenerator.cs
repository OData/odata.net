//---------------------------------------------------------------------
// <copyright file="IStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Generates random strings of a specified length.
    /// </summary>
    public interface IStringGenerator
    {
        /// <summary>
        /// Generates random string of specified length
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="minLength">Minimum length of the generated string.</param>
        /// <param name="maxLength">Maximum length of the generated string.</param>
        /// <returns>Generated string.</returns>
        string GenerateString(IRandomNumberGenerator random, int minLength, int maxLength);
    }
}
