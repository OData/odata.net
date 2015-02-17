//---------------------------------------------------------------------
// <copyright file="EnumMemberValuesCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Computes values for enum members in the model with unspecified values.
    /// </summary>
    [ImplementationName(typeof(IEnumMemberValuesCalculator), "Default", HelpText = "Computes values for enum members in the model with unspecified values.")]
    public class EnumMemberValuesCalculator : IEnumMemberValuesCalculator
    {
        /// <summary>
        /// Computes values for enum members in the model with unspecified values.
        /// </summary>
        /// <param name="enumType">An enum type.</param>
        /// <returns>List of enum members with all values resolved.</returns>
        public IEnumerable<EnumMember> CalculateEnumMemberValues(EnumType enumType)
        {
            var enumMembers = new List<EnumMember>();
            long nextValue = 0;

            // Unspecified values are calculated based on the last specified value.
            foreach (var enumMember in enumType.Members)
            {
                object value = null;
                if (enumMember.Value != null)
                {
                    value = enumMember.Value;
                    nextValue = Convert.ToInt64(enumMember.Value, CultureInfo.InvariantCulture) + 1;
                }
                else
                {
                    value = nextValue;
                    nextValue++;
                }

                var resolvedEnumMember = this.CalculateValue(enumType, enumMember.Name, value);
                enumMembers.Add(resolvedEnumMember);
            }

            return enumMembers;
        }

        private EnumMember CalculateValue(EnumType enumType, string name, object value)
        {
            EnumMember enumMember;
             
            if (enumType.UnderlyingType == null)
                {
                    enumMember = new EnumMember(name, Convert.ChangeType(value, typeof(int), CultureInfo.InvariantCulture));
                }
                else
                {
                    enumMember = new EnumMember(name, Convert.ChangeType(value, enumType.UnderlyingType, CultureInfo.InvariantCulture));
                }

            return enumMember;
        }
    }
}
