//---------------------------------------------------------------------
// <copyright file="DataGenerationUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Data generation utilities.
    /// </summary>
    internal static class DataGenerationUtilities
    {
        /// <summary>
        /// Maximum numeric precision.
        /// </summary>
        public const int MaxNumericPrecision = 28;

        /// <summary>
        /// Maximum numeric scale.
        /// </summary>
        public const int MaxNumericScale = 28;

        /// <summary>
        /// Maximum fractional digits.
        /// </summary>
        public const byte MaxFractionalDigits = 15;

        /// <summary>
        /// Maximum fractional seconds.
        /// </summary>
        public const byte MaxFractionalSeconds = 7;

        /// <summary>
        /// Creates new exception that needs to be thrown when FromSeed fails.
        /// </summary>
        /// <typeparam name="TData">The targeted data type.</typeparam>
        /// <param name="seed">The seed for which FromSeed failed.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns>Exception that needs to be thrown when FromSeed fails</returns>
        public static Exception FromSeedFailedException<TData>(long seed, Exception innerException)
        {
            Type targetType = typeof(TData);

            return new TaupoInvalidOperationException(
                string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to generate {0} data for the seed {1}.",
                        targetType.Name,
                        seed.ToString(CultureInfo.InvariantCulture)),
               innerException);
        }

        /// <summary>
        /// Checks that numeric precision and scale are in valid range.
        /// </summary>
        /// <param name="precision">The numeric precision.</param>
        /// <param name="scale">The numeric scale.</param>
        public static void CheckNumericPrecisionAndScale(int precision, int scale)
        {
            if (precision < 1 || precision > MaxNumericPrecision)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Numeric precision should be in the range from 1 to {0}.", MaxNumericPrecision));
            }

            CheckNumericScale(scale);

            if (scale > precision)
            {
                throw new TaupoInvalidOperationException("Numeric scale cannot be greater then numeric precision.");
            }
        }

        /// <summary>
        /// Checks that numeric scale is in valid range.
        /// </summary>
        /// <param name="scale">The numeric scale.</param>
        public static void CheckNumericScale(int scale)
        {
            if (scale < 0 || scale > MaxNumericScale)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Numeric scale should be in the range from 0 to {0}.", MaxNumericScale));
            }
        }
    }
}
