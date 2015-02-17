//---------------------------------------------------------------------
// <copyright file="IMemberDataGenerators.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Contains data generators for members.
    /// </summary>
    public interface IMemberDataGenerators
    {
        /// <summary>
        /// Gets a data generator for the specified member.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <returns>A data generator for the specified member.</returns>
        IDataGenerator GetMemberDataGenerator(string memberName);
    }
}
