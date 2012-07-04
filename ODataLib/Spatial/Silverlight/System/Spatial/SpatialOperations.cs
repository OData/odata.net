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
    /// <summary>
    /// Class responsible for knowing how to perform operations for a particular implemenation of Spatial types
    /// </summary>
    public abstract class SpatialOperations
    {
        /// <summary>
        /// Geometry Distance
        /// </summary>
        /// <param name="operand1">Operand 1</param>
        /// <param name="operand2">Operand 2</param>
        /// <returns>The operation result</returns>
        public virtual double Distance(Geometry operand1, Geometry operand2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Geography Distance
        /// </summary>
        /// <param name="operand1">Operand 1</param>
        /// <param name="operand2">Operand 2</param>
        /// <returns>The operation result</returns>
        public virtual double Distance(Geography operand1, Geography operand2)
        {
            throw new NotImplementedException();
        }
    }
}
