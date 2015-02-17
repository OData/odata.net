//---------------------------------------------------------------------
// <copyright file="IAnonymousTypeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Creates and stores anonymous types.
    /// </summary>
    [ImplementationSelector("AnonymousTypeBuilder", DefaultImplementation = "CSharpILGen", HelpText = "Anonymous type generator")]
    public interface IAnonymousTypeBuilder
    {
        /// <summary>
        /// Returns an anonymous type that contains members with given names. Returned type is actually a generic type definition.
        /// </summary>
        /// <param name="memberNames">Names of members of the anonymous type.</param>
        /// <returns>Anonymous type containing given members.</returns>
        Type GetAnonymousType(params string[] memberNames);
    }
}