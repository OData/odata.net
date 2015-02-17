//---------------------------------------------------------------------
// <copyright file="PrimitiveDataTypeToDataGenerationHintsResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Types
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.DataGeneration;

    /// <summary>
    /// Resolves data generation hints for primitive data types.
    /// </summary>
    [ImplementationName(typeof(IPrimitiveDataTypeToDataGenerationHintsResolver), "Default")]
    public class PrimitiveDataTypeToDataGenerationHintsResolver : IPrimitiveDataTypeToDataGenerationHintsResolver 
    {
        /// <summary>
        /// Resolves hints for data generation for the given primitive type.
        /// </summary>
        /// <param name="dataType">Primitive data type to resolve data generator for.</param>
        /// <returns>Data generation hints.</returns>
        public IEnumerable<DataGenerationHint> ResolveDataGenerationHints(PrimitiveDataType dataType)
        {
            var hints = new List<DataGenerationHint>();

            int maxLength = dataType.GetFacetValue<MaxLengthFacet, int>(-1);
            if (maxLength >= 0)
            {
                hints.Add(DataGenerationHints.MaxLength(maxLength));
            }

            int precision = dataType.GetFacetValue<NumericPrecisionFacet, int>(-1);
            if (precision > 0 && precision <= DataGenerationUtilities.MaxNumericPrecision)
            {
                hints.Add(DataGenerationHints.NumericPrecision(precision));
            }

            int scale = dataType.GetFacetValue<NumericScaleFacet, int>(-1);
            if (scale >= 0 && scale <= DataGenerationUtilities.MaxNumericScale)
            {
                hints.Add(DataGenerationHints.NumericScale(scale));
            }

            bool isUnicode = dataType.GetFacetValue<IsUnicodeFacet, bool>(true);
            if (!isUnicode)
            {
                hints.Add(DataGenerationHints.AnsiString);
            }

            int timePrecision = dataType.GetFacetValue<TimePrecisionFacet, int>(-1);
            if (timePrecision >= 0 && timePrecision <= DataGenerationUtilities.MaxFractionalSeconds)
            {
                hints.Add(DataGenerationHints.FractionalSeconds(timePrecision));
            }
            else
            {
                timePrecision = DataGenerationUtilities.MaxFractionalSeconds;
            }

            if (dataType is TimeOfDayDataType)
            {
                TimeSpan maxValue = new TimeSpan(23, 59, 59);
                long factor = (long)Math.Pow(10, DataGenerationUtilities.MaxFractionalSeconds - timePrecision);
                maxValue += new TimeSpan(((TimeSpan.TicksPerSecond - 1) / factor) * factor);
                hints.Add(DataGenerationHints.MaxValue<TimeSpan>(maxValue));
                hints.Add(DataGenerationHints.MinValue<TimeSpan>(TimeSpan.Zero));
            }

            return hints;
        }
    }
}
