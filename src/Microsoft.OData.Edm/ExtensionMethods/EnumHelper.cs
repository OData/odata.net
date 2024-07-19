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
        public static bool TryParseEnum(this IEdmEnumType enumType, string value, bool ignoreCase, out long parseResult)
        {
            char[] enumSeparatorCharArray = new[] { ',' };

            string[] enumNames;
            ulong[] enumValues;
            IEdmEnumType type = enumType;

            parseResult = 0L;

            if (value == null)
            {
                return false;
            }

            value = value.Trim();
            if (value.Length == 0)
            {
                return false;
            }

            ulong num = 0L;
            string[] values = value.Split(enumSeparatorCharArray);

            if ((!enumType.IsFlags) && values.Length > 1)
            {
                return false; // nonflags can only have 1 value
            }

            type.GetCachedValuesAndNames(out enumValues, out enumNames, true, true);

            if ((char.IsDigit(value[0]) || (value[0] == '-')) || (value[0] == '+'))
            {
                // computed for later use. only meaningful for Enum types with IsFlags=true.
                ulong fullBits = 0;
                for (int j = 0; j < enumValues.Length; j++)
                {
                    fullBits |= enumValues[j];
                }

                // process each value
                for (int i = 0; i < values.Length; i++)
                {
                    long itemValue;
                    if (long.TryParse(values[i], out itemValue))
                    {
                        // allow any number value, don't validate it against enum definition.
                        num |= (ulong)itemValue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim();
                    bool flag = false;
                    for (int j = 0; j < enumNames.Length; j++)
                    {
                        if (ignoreCase)
                        {
                            if (string.Compare(enumNames[j], values[i], StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (!enumNames[j].Equals(values[i], StringComparison.Ordinal))
                            {
                                continue;
                            }
                        }

                        ulong item = enumValues[j];
                        num |= item;
                        flag = true;
                        break;
                    }

                    if (!flag)
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
