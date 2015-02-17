//---------------------------------------------------------------------
// <copyright file="EdmTypeSemantics.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Handles type semantics for Edm primitive types.
    /// </summary>
    public static class EdmTypeSemantics
    {
        private static Dictionary<string, ICollection<string>> promotabilityTable =
            new Dictionary<string, ICollection<string>>()
            {
                { "Binary", new List<string>() { } },
                { "Boolean", new List<string>() { } },
                { "Byte", new List<string>() { "Int16", "Int32", "Int64", "Decimal", "Single", "Double" } },
                { "DateTime", new List<string>() { } },
                { "DateTimeOffset", new List<string>() { } },
                { "Decimal", new List<string>() { } },
                { "Double", new List<string>() { } },
                { "Geography", new List<string>() { } },
                { "Geometry", new List<string>() { } },
                { "Guid", new List<string>() { } },
                { "Int16", new List<string>() { "Int32", "Int64", "Decimal", "Single", "Double" } },
                { "Int32", new List<string>() { "Int64", "Decimal", "Single", "Double" } },
                { "Int64", new List<string>() { "Decimal", "Single", "Double" } },
                { "Single", new List<string>() { "Double" } },
                { "String", new List<string>() { } },
                { "Time", new List<string>() { } },
            };

        /// <summary>
        /// Determines whether one edm data type is promotable to another.
        /// </summary>
        /// <param name="fromType">Edm type from which the promotion should occur.</param>
        /// <param name="toType">Edm type to which the promotion should ocur.</param>
        /// <returns>True if Edm types are promotable, false otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Intended for Primitive Data Type only.")]
        public static bool IsPromotable(PrimitiveDataType fromType, PrimitiveDataType toType)
        {
            string fromTypeName = EdmDataTypes.GetEdmName(fromType);
            string toTypeName = EdmDataTypes.GetEdmName(toType);

            if (fromTypeName == toTypeName)
            {
                return true;
            }
            else
            {
                ICollection<string> promotabilityForType;
                if (promotabilityTable.TryGetValue(fromTypeName, out promotabilityForType) && promotabilityForType.Contains(toTypeName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Try gets the common type to which both types can be promoted.
        /// </summary>
        /// <param name="leftType">First type.</param>
        /// <param name="rightType">Second type.</param>
        /// <param name="commonType">outputs the common type</param>
        /// <returns>true if common type can be found, false otherwise</returns>
        public static bool TryGetCommonType(PrimitiveDataType leftType, PrimitiveDataType rightType, out PrimitiveDataType commonType)
        {
            if (IsPromotable(leftType, rightType))
            {
                commonType = ResolveCommonPrecisionAndScale(leftType, rightType, rightType);
                return true;
            }
            else if (IsPromotable(rightType, leftType))
            {
                commonType = ResolveCommonPrecisionAndScale(leftType, rightType, leftType);
                return true;
            }
            else
            {
                commonType = null;
                return false;
            }
        }

        private static PrimitiveDataType ResolveCommonPrecisionAndScale(PrimitiveDataType leftType, PrimitiveDataType rightType, PrimitiveDataType commonType)
        {
            if (commonType.HasFacet<NumericPrecisionFacet>())
            {
                ExceptionUtilities.Assert(commonType is FixedPointDataType, "Precision and scale facets are not expected on type {}.", commonType.ToString());
                var commonPrecision = GetCommonPrecision(leftType, rightType);
                var commonScale = GetCommonScale(leftType, rightType);
                commonType = EdmDataTypes.Decimal(commonPrecision, commonScale);
            }

            return commonType;
        }

        private static int GetCommonPrecision(PrimitiveDataType type1, PrimitiveDataType type2)
        {
            // Determine the precision using the following formula: max(s1, s2) + max(p1-s1, p2-s2)
            int scale1 = type1.GetFacetValue<NumericScaleFacet, int>(0);
            int scale2 = type2.GetFacetValue<NumericScaleFacet, int>(0);
            int scale = Math.Max(scale1, scale2);
            int precision1 = type1.GetFacetValue<NumericPrecisionFacet, int>(GetMaxPrecision(type1));
            int precision2 = type2.GetFacetValue<NumericPrecisionFacet, int>(GetMaxPrecision(type2));
            return scale + Math.Max(precision1 - scale1, precision2 - scale2);
        }

        private static int GetCommonScale(PrimitiveDataType type1, PrimitiveDataType type2)
        {
            int scale1 = type1.GetFacetValue<NumericScaleFacet, int>(0);
            int scale2 = type2.GetFacetValue<NumericScaleFacet, int>(0);
            return Math.Max(scale1, scale2);
        }

        private static int GetMaxPrecision(PrimitiveDataType dataType)
        {
            // TODO: Using 28 as the default precision (max value on sql).
            // We should be determining the default precision for a given type.  
            // For example an int32 has a precision of 10.
            ExceptionUtilities.CheckArgumentNotNull(dataType, "dataType");
            return 28;
        }
    }
}
