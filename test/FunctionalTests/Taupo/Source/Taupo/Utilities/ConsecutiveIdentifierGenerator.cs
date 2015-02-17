//---------------------------------------------------------------------
// <copyright file="ConsecutiveIdentifierGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Generates unique identifiers by concatenating given prefix with increasing numbers.
    /// </summary>
    [ImplementationName(typeof(IIdentifierGenerator), "Consecutive", HelpText = "Consecutive identifier generator.")]
    public class ConsecutiveIdentifierGenerator : IIdentifierGenerator
    {
        private Dictionary<string, int> lastIdentifierGenerated = new Dictionary<string, int>();

        /// <summary>
        /// Generates identifier.
        /// </summary>
        /// <param name="identifierClass">A string that describes class of identifiers (such as 'Class','Property','EntityType') - identifiers are guaranteed to be unique within the class</param>
        /// <returns>Generated identifier.</returns>
        /// <remarks>If <paramref name="identifierClass"/> is null, 'Identifier' is used instead.</remarks>
        public string GenerateIdentifier(string identifierClass)
        {
            identifierClass = identifierClass ?? "Identifier";

            int lastID;

            if (!this.lastIdentifierGenerated.TryGetValue(identifierClass, out lastID))
            {
                lastID = 0;
            }

            int newID = lastID + 1;
            this.lastIdentifierGenerated[identifierClass] = newID;
            return identifierClass + newID;
        }
    }
}