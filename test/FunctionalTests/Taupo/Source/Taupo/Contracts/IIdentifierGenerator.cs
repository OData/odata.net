//---------------------------------------------------------------------
// <copyright file="IIdentifierGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Generates unique identifiers for use in code.
    /// </summary>
    [ImplementationSelector("IdentifierGenerator", IsTransient = true, HelpText = "Generates unique identifiers in code.", DefaultImplementation = "Consecutive")]
    public interface IIdentifierGenerator
    {
        /// <summary>
        /// Generates an identifier for a given class.
        /// </summary>
        /// <param name="identifierClass">A string that describes class of identifiers (such as 'Class','Property','EntityType') - identifiers are guaranteed to be unique within the class</param>
        /// <returns>Generated identifier.</returns>
        string GenerateIdentifier(string identifierClass);
    }
}
