//---------------------------------------------------------------------
// <copyright file="EdmEnumValueParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl.CsdlSemantics;

    /// <summary>
    /// Internal parser to parse enum value in csdl.
    /// </summary>
    internal static class EdmEnumValueParser
    {
        /// <summary>
        /// Try parse enum members specified in a string value from declared schema types
        /// </summary>
        /// <param name="value">Enum value string</param>
        /// <param name="model">The model for resolving enum type.</param>
        /// <param name="location">The location of the enum member in csdl</param>
        /// <param name="result">Parsed enum members</param>
        /// <returns>True for successfully parsed, false for failed</returns>
        internal static bool TryParseEnumMember(string value, IEdmModel model, EdmLocation location, out IEnumerable<IEdmEnumMember> result)
        {
            result = null;
            if (value == null || model == null)
            {
                return false;
            }

            bool isUnresolved = false;
            var enumValues = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!enumValues.Any())
            {
                return false;
            }

            string enumTypeName = enumValues[0].Split('/').FirstOrDefault();
            if (string.IsNullOrEmpty(enumTypeName))
            {
                return false;
            }

            IEdmEnumType enumType = model.FindType(enumTypeName) as IEdmEnumType;

            if (enumType == null)
            {
                enumType = new UnresolvedEnumType(enumTypeName, location);
                isUnresolved = true;
            }
            else if (enumValues.Count() > 1 && (!enumType.IsFlags || !EdmEnumValueParser.IsEnumIntegerType(enumType)))
            {
                return false;
            }

            List<IEdmEnumMember> enumMembers = new List<IEdmEnumMember>();
            foreach (var enumValue in enumValues)
            {
                string[] path = enumValue.Split('/');
                if (path.Count() != 2)
                {
                    return false;
                }

                if (path[0] != enumTypeName)
                {
                    return false;
                }

                if (!isUnresolved)
                {
                    IEdmEnumMember member = enumType.Members.SingleOrDefault(m => m.Name == path[1]);
                    if (member == null)
                    {
                        return false;
                    }

                    enumMembers.Add(member);
                }
                else
                {
                    enumMembers.Add(new UnresolvedEnumMember(path[1], enumType, location));
                }
            }

            result = enumMembers;
            return true;
        }

        /// <summary>
        /// Determine if the underlying type of the enum type is integer type (byte, sbyte, int16, int32, int64).
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <returns>True if the underlying type of enum type is integer type.</returns>
        internal static bool IsEnumIntegerType(IEdmEnumType enumType)
        {
            return enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.Byte ||
                   enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.SByte ||
                   enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.Int16 ||
                   enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.Int32 ||
                   enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.Int64;
        }
    }
}
