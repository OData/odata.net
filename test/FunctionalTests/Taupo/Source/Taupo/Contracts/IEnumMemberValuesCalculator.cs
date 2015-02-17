//---------------------------------------------------------------------
// <copyright file="IEnumMemberValuesCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Computes values for enum members in the model with unspecified values.
    /// </summary>
    [ImplementationSelector("EnumMemberValuesCalculator", DefaultImplementation = "Default", HelpText = "Computes values for enum members in the model with unspecified values.")]
    public interface IEnumMemberValuesCalculator
    {
        /// <summary>
        /// Computes values for enum members in the model with unspecified values.
        /// </summary>
        /// <param name="enumType">An enum type.</param>
        /// <returns>List of enum members with all values resolved.</returns>
        IEnumerable<EnumMember> CalculateEnumMemberValues(EnumType enumType);
    }
}
