//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Spatial
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Spatial;

    /// <summary>
    ///   Extension methods for the Geography operations
    /// </summary>
    public static class GeographyOperationsExtensions
    {
        /// <summary>
        ///   Geography Distance
        /// </summary>
        /// <param name = "operand1">Operand 1</param>
        /// <param name = "operand2">Operand 2</param>
        /// <returns>The operation result</returns>
        public static double? Distance(this Geography operand1, Geography operand2)
        {
            return OperationsFor(operand1, operand2).IfValidReturningNullable(ops => ops.Distance(operand1, operand2));
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
