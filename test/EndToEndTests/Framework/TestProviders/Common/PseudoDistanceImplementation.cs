//---------------------------------------------------------------------
// <copyright file="PseudoDistanceImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System;
#if TESTPROVIDERS
    using Microsoft.Spatial;
#else
    using System.Spatial;
#endif
    using Microsoft.Test.OData.Framework.TestProviders.Common;

    /// <summary>
    /// Implements the spatial distance operation for points only, and returns a value that simply encodes the point data rather than truly being a distance
    /// </summary>
    public class PseudoDistanceImplementation : SpatialOperations
    {
        /// <summary>
        /// Returns the distance between the given operands.
        /// </summary>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        /// <returns>The distance</returns>
        public override double Distance(Geography operand1, Geography operand2)
        {
            return CalculateDistance<Geography, GeographyPoint>(
                operand1,
                operand2,
                p => p.IsEmpty,
                (p1, p2) => CalculateFakeDistance(p1.Latitude, p2.Latitude, p1.Longitude, p2.Longitude, p1.Z, p2.Z, p1.M, p2.M));
        }

        /// <summary>
        /// Returns the distance between the given operands.
        /// </summary>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        /// <returns>The distance</returns>
        public override double Distance(Geometry operand1, Geometry operand2)
        {
            return CalculateDistance<Geometry, GeometryPoint>(
                operand1, 
                operand2,
                p => p.IsEmpty,
                (p1, p2) => CalculateFakeDistance(p1.X, p2.X, p1.Y, p2.Y, p1.Z, p2.Z, p1.M, p2.M));
        }

        /// <summary>
        /// Asserts the operand is of the expected type.
        /// </summary>
        /// <typeparam name="TOriginal">The original type of the operand.</typeparam>
        /// <typeparam name="TSpecific">The type expected.</typeparam>
        /// <param name="operand">The operand.</param>
        /// <returns>The same operand, after being cast</returns>
        internal static TSpecific AssertIsType<TOriginal, TSpecific>(TOriginal operand)
            where TOriginal : class, ISpatial
            where TSpecific : class, TOriginal
        {
            var afterCast = operand as TSpecific;
            ExceptionUtilities.CheckObjectNotNull(afterCast, "Value '{0}' was not of expected type '{1}'", operand, typeof(TSpecific));
            return afterCast;
        }

        /// <summary>
        /// Calculates the distance while handling nulls and invalid types using callbacks
        /// </summary>
        /// <typeparam name="TOriginal">The type of the original.</typeparam>
        /// <typeparam name="TSpecific">The type of the specific.</typeparam>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        /// <param name="checkForEmpty">The callback to use to check for empty values</param>
        /// <param name="handleBothNonNull">The callback if both balues are non null.</param>
        /// <returns>The distance value computed by the callback</returns>
        internal static double CalculateDistance<TOriginal, TSpecific>(TOriginal operand1, TOriginal operand2, Func<TSpecific, bool> checkForEmpty, Func<TSpecific, TSpecific, double> handleBothNonNull)
            where TOriginal : class, ISpatial
            where TSpecific : class, TOriginal
        {
            ExceptionUtilities.CheckArgumentNotNull(operand1, "operand1");
            ExceptionUtilities.CheckArgumentNotNull(operand2, "operand2");
            var specific1 = AssertIsType<TOriginal, TSpecific>(operand1);
            var specific2 = AssertIsType<TOriginal, TSpecific>(operand2);

            if (checkForEmpty(specific1) || checkForEmpty(specific2))
            {
                return double.PositiveInfinity;
            }

            return handleBothNonNull(specific1, specific2);
        }

        /// <summary>
        /// Calculates a value that encodes the given data, but is not actually a distance
        /// </summary>
        /// <param name="firstCoordinate1">The first first coordinate.</param>
        /// <param name="firstCoordinate2">The second first coordinate.</param>
        /// <param name="secondCoordinate1">The first second coordinate1.</param>
        /// <param name="secondCoordinate2">The second second coordinate2.</param>
        /// <param name="z1">The first Z value.</param>
        /// <param name="z2">The second Z value.</param>
        /// <param name="m1">The first M value.</param>
        /// <param name="m2">The second M value.</param>
        /// <returns>The fake distance</returns>
        internal static double CalculateFakeDistance(double firstCoordinate1, double firstCoordinate2, double secondCoordinate1, double secondCoordinate2, double? z1, double? z2, double? m1, double? m2)
        {
            var nullSafeZ1 = z1 ?? 0.0;
            var nullSafeZ2 = z2 ?? 0.0;

            var nullSafeM1 = m1 ?? 0.0;
            var nullSafeM2 = m2 ?? 0.0;

            return Encode(firstCoordinate2 - firstCoordinate1, secondCoordinate2 - secondCoordinate1, nullSafeZ2 - nullSafeZ1, nullSafeM2 - nullSafeM1);
        }

        /// <summary>
        /// Encodes the specified data.
        /// </summary>
        /// <param name="firstCoordinate">The first coordinate.</param>
        /// <param name="secondCoordinate">The second coordinate.</param>
        /// <param name="thirdCoordinate">The third coordinate.</param>
        /// <param name="fourthCoordinate">The fourth coordinate.</param>
        /// <returns>The encoded value</returns>
        internal static double Encode(double firstCoordinate, double secondCoordinate, double? thirdCoordinate, double? fourthCoordinate)
        {
            var value = Math.Pow(2.0, firstCoordinate) + Math.Pow(3.0, secondCoordinate);

            if (thirdCoordinate.HasValue)
            {
                value += Math.Pow(5.0, thirdCoordinate.Value);
            }

            if (fourthCoordinate.HasValue)
            {
                value += Math.Pow(7.0, fourthCoordinate.Value);
            }

            return value;
        }
    }
}
