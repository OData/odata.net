//---------------------------------------------------------------------
// <copyright file="EnumHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Enum helper
    /// </summary>
    public static class EnumHelper
    {
        private const int MaxHashElements = 100;

        private static readonly ConcurrentDictionary<IEdmEnumType, HashEntry> fieldInfoHash = new ConcurrentDictionary<IEdmEnumType, HashEntry>(4, EnumHelper.MaxHashElements);

        /// <summary>
        /// Parse an enum literal value to integer. The literal value can be Enum member name (e.g. "Red"), underlying value (e.g. "2"), or combined values (e.g. "Red, Green, Blue", "1,2,4").
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input string value</param>
        /// <param name="ignoreCase">true if case insensitive, false if case sensitive</param>
        /// <param name="parseResult">parse result</param>
        /// <returns>true if parse succeeds, false if parse fails</returns>
        public static bool TryParseEnum(this IEdmEnumType enumType, ReadOnlySpan<char> input, bool ignoreCase, out long parseResult)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumType));
            }

            parseResult = 0L;
            if (input.IsEmpty)
            {
                return false;
            }

            MemoryExtensions.SpanSplitEnumerator<char> values = input.Split(',');
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            ulong num = 0L;
            int count = 0;
            while (values.MoveNext())
            {
                count++;
                if (!enumType.IsFlags && count > 1)
                {
                    return false;// non-flags can only have 1 value (without comma)
                }

                // For example, Get the 'Red' from 'Red,Yellow,...'
                ReadOnlySpan<char> currentValue = input[values.Current].Trim();

                if (long.TryParse(currentValue, out long longNum))
                {
                    // allow any number value, don't validate it against enum definition.
                    num |= (ulong)longNum;
                }
                else
                {
                    bool found = false;
                    foreach (var member in enumType.Members)
                    {
                        if (currentValue.Equals(member.Name, comparison))
                        {
                            num |= (ulong)member.Value.Value;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        return false;
                    }
                }
            }

            parseResult = (long)num;
            return true;
        }

        /// <summary>
        /// Convert enum int value to string
        /// </summary>
        /// <param name="type">edm enum type reference</param>
        /// <param name="value">input int value</param>
        /// <returns>string literal of the enum value</returns>
        public static string ToStringLiteral(this IEdmEnumTypeReference type, Int64 value)
        {
            if (type != null)
            {
                // parse the value to string literal
                IEdmEnumType enumType = type.Definition as IEdmEnumType;
                if (enumType != null)
                {
                    return enumType.IsFlags ? enumType.ToStringWithFlags(value) : enumType.ToStringNoFlags(value);
                }
            }

            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// For enum with flags, use a sequential search for bit masks, and then check if any residual
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input integer value</param>
        /// <returns>string separated by comma</returns>
        private static string ToStringWithFlags(this IEdmEnumType enumType, Int64 value)
        {
            string[] strArray;
            ulong[] numArray;
            ulong num = (ulong)value;
            enumType.GetCachedValuesAndNames(out numArray, out strArray, true, true);
            int index = numArray.Length - 1;
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            ulong num3 = num;
            const int Zero = 0;
            const ulong UlongZero = 0L;

            while (index >= Zero)
            {
                if ((index == Zero) && (numArray[index] == UlongZero))
                {
                    break;
                }

                if ((num & numArray[index]) == numArray[index])
                {
                    num -= numArray[index];
                    if (!flag)
                    {
                        builder.Insert(Zero, ", ");
                    }

                    builder.Insert(Zero, strArray[index]);
                    flag = false;
                }

                index--;
            }

            if (num != UlongZero)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            if (num3 != UlongZero)
            {
                return builder.ToString();
            }

            if ((numArray.Length > Zero) && (numArray[Zero] == UlongZero))
            {
                return strArray[Zero];
            }

            return Zero.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// For enum without flags, use a binary search
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input integer value</param>
        /// <returns>string</returns>
        private static string ToStringNoFlags(this IEdmEnumType enumType, Int64 value)
        {
            ulong[] values;
            string[] names;
            enumType.GetCachedValuesAndNames(out values, out names, true, true);
            ulong num = (ulong)value;
            int index = Array.BinarySearch(values, num);
            return index >= 0 ? names[index] : value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get cached values and names from hash table
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="values">output values</param>
        /// <param name="names">output names</param>
        /// <param name="getValues">true if get values, false otherwise</param>
        /// <param name="getNames">true if get names, false otherwise</param>
        private static void GetCachedValuesAndNames(this IEdmEnumType enumType, out ulong[] values, out string[] names, bool getValues, bool getNames)
        {
            HashEntry hashEntry = GetHashEntry(enumType);
            values = hashEntry.Values;
            if (values != null)
            {
                getValues = false;
            }

            names = hashEntry.Names;
            if (names != null)
            {
                getNames = false;
            }

            if (!getValues && !getNames)
            {
                return;
            }

            GetEnumValuesAndNames(enumType, ref values, ref names, getValues, getNames);
            if (getValues)
            {
                hashEntry.Values = values;
            }

            if (getNames)
            {
                hashEntry.Names = names;
            }
        }

        private static void GetEnumValuesAndNames(IEdmEnumType enumType, ref ulong[] values, ref string[] names, bool getValues, bool getNames)
        {
            Dictionary<string, ulong> dict = new Dictionary<string, ulong>();
            foreach (var member in enumType.Members)
            {
                IEdmEnumMemberValue intValue = member.Value;
                if (intValue != null)
                {
                    dict.Add(member.Name, (ulong)intValue.Value);
                }
            }

            Dictionary<string, ulong> sortedDict = dict.OrderBy(d => d.Value).ToDictionary(d => d.Key, d => d.Value);
            values = sortedDict.Select(d => d.Value).ToArray();
            names = sortedDict.Select(d => d.Key).ToArray();
        }

        private static HashEntry GetHashEntry(IEdmEnumType enumType)
        {
            try
            {
                return EnumHelper.fieldInfoHash.GetOrAdd(enumType, type => new HashEntry(null, null));
            }
            catch (OverflowException)
            {
                EnumHelper.fieldInfoHash.Clear();
                return EnumHelper.fieldInfoHash.GetOrAdd(enumType, type => new HashEntry(null, null));
            }
        }


        private class HashEntry
        {
            public string[] Names;
            public ulong[] Values;

            public HashEntry(string[] names, ulong[] values)
            {
                this.Names = names;
                this.Values = values;
            }
        }
    }
}
