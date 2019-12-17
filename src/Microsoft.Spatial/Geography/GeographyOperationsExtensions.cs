//---------------------------------------------------------------------
// <copyright file="GeographyOperationsExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Linq;

    /// <summary>
    ///   Extension methods for the Geography operations
    /// </summary>
    public static class GeographyOperationsExtensions
    {
        /// <summary>Determines the distance of the geography.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        public static double? Distance(this Geography operand1, Geography operand2)
        {
            return OperationsFor(operand1, operand2).IfValidReturningNullable(ops => ops.Distance(operand1, operand2));
        }

        /// <summary>Determines the Length of the geography LineString.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand">The LineString operand.</param>
        public static double? Length(this Geography operand)
        {
            return OperationsFor(operand).IfValidReturningNullable(ops => ops.Length(operand));
        }

        /// <summary>Determines if geography point and polygon will intersect.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        public static bool? Intersects(this Geography operand1, Geography operand2)
        {
            return OperationsFor(operand1, operand2).IfValidReturningNullable(ops => ops.Intersects(operand1, operand2));
        }

        /// <summary>
        /// Finds the ops instance registered for the operands.
        /// </summary>
        /// <param name="operands">The operands.</param>
        /// <returns>The ops value, or null if any operand is null</returns>
        private static SpatialOperations OperationsFor(params Geography[] operands)
        {
            if (operands.Any(operand => operand == null))
            {
                return null;
            }

            return operands[0].Creator.VerifyAndGetNonNullOperations();
        }
    }
}
