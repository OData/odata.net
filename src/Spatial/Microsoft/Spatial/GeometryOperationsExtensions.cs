//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    using System.Linq;
    using Microsoft.Data.Spatial;

    /// <summary>
    /// Extension methods for the Geography operations
    /// </summary>
    public static class GeometryOperationsExtensions
    {
        /// <summary>Determines the distance of the geometry.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        public static double? Distance(this Geometry operand1, Geometry operand2)
        {
            return OperationsFor(operand1, operand2).IfValidReturningNullable(ops => ops.Distance(operand1, operand2));
        }

        /// <summary>Determines the Length of the geometry LineString.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand">The LineString operand.</param>
        public static double? Length(this Geometry operand)
        {
            return OperationsFor(operand).IfValidReturningNullable(ops => ops.Length(operand));
        }

        /// <summary>Determines if geometry point and polygon will intersect.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand1">The first operand, point.</param>
        /// <param name="operand2">The second operand, polygon.</param>
        public static bool? Intersects(this Geometry operand1, Geometry operand2)
        {
            return OperationsFor(operand1, operand2).IfValidReturningNullable(ops => ops.Intersects(operand1, operand2));
        }

        /// <summary>
        /// Finds the ops instance registered for the operands.
        /// </summary>
        /// <param name="operands">The operands.</param>
        /// <returns>The ops value, or null if any operand is null</returns>
        private static SpatialOperations OperationsFor(params Geometry[] operands)
        {
            if (operands.Any(operand => operand == null))
            {
                return null;
            }

            return operands[0].Creator.VerifyAndGetNonNullOperations();
        }
    }
}
