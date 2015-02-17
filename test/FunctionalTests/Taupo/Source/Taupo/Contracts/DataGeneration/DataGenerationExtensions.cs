//---------------------------------------------------------------------
// <copyright file="DataGenerationExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Data generation extension methods.
    /// </summary>
    public static class DataGenerationExtensions
    {
        /// <summary>
        /// Calculates minimum value of the data generation hints with specified type.
        /// </summary>
        /// <typeparam name="THint">Data generation hint type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="hints">Data generation hints.</param>
        /// <param name="defaultValue">Default value to use when hints enumeration does not contain a hint of the specified type.</param>
        /// <returns>Minimum value for the specified hint or default value if the hints enumeration does not contain a hint of the specified type.</returns>
        public static TValue Min<THint, TValue>(this IEnumerable<DataGenerationHint> hints, TValue defaultValue) 
            where TValue : IComparable<TValue>
            where THint : DataGenerationHint<TValue>
        {
            var hintsOfSpecifcType = hints.OfType<THint>();
            if (hintsOfSpecifcType.Any())
            {
                return hintsOfSpecifcType.Min(h => h.Value);
            }

            return defaultValue;
        }

        /// <summary>
        /// Calculates maximum value of the data generation hints with specified type.
        /// </summary>
        /// <typeparam name="THint">Data generation hint type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="hints">Data generation hints.</param>
        /// <param name="defaultValue">Default value to use when hints enumeration does not contain a hint of the specified type.</param>
        /// <returns>Maximum value for the specified hint or default value if the hints enumeration does not contain a hint of the specified type.</returns>
        public static TValue Max<THint, TValue>(this IEnumerable<DataGenerationHint> hints, TValue defaultValue)
            where TValue : IComparable<TValue>
            where THint : DataGenerationHint<TValue>
        {
            var hintsOfSpecifcType = hints.OfType<THint>();
            if (hintsOfSpecifcType.Any())
            {
                return hintsOfSpecifcType.Max(h => h.Value);
            }

            return defaultValue;
        }

        internal static void AddMemberData(this IList<NamedValue> namedValues, string memberName, object memberData)
        {
            var memberNamedValues = memberData as IEnumerable<NamedValue>;
            if (memberNamedValues != null)
            {
                foreach (var nv in memberNamedValues)
                {
                    namedValues.Add(new NamedValue(memberName + "." + nv.Name, nv.Value));
                }
            }
            else
            {
                namedValues.Add(new NamedValue(memberName, memberData));
            }
        }
    }
}
