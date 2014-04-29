//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    using System;

    /// <summary>
    /// Class responsible for knowing how to perform operations for a particular implemenation of Spatial types
    /// </summary>
    public abstract class SpatialOperations
    {
        /// <summary>Indicates the Geometry Distance.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand1">The Operand 1.</param>
        /// <param name="operand2">The Operand 2.</param>
        public virtual double Distance(Geometry operand1, Geometry operand2)
        {
            throw new NotImplementedException();
        }

        /// <summary>Indicates a Geography Distance.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand1">The Operand 1.</param>
        /// <param name="operand2">The Operand 2.</param>
        public virtual double Distance(Geography operand1, Geography operand2)
        {
            throw new NotImplementedException();
        }

        /// <summary>Indicates the Geometry LineString's length.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand">The Operand.</param>
        public virtual double Length(Geometry operand)
        {
            throw new NotImplementedException();
        }

        /// <summary>Indicates a Geography LineString's length.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand">The Operand.</param>
        public virtual double Length(Geography operand)
        {
            throw new NotImplementedException();
        }

        /// <summary>Indicates the Geometry Intersects() method.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand1">The Operand 1, point.</param>
        /// <param name="operand2">The Operand 2, polygon.</param>
        public virtual bool Intersects(Geometry operand1, Geometry operand2)
        {
            throw new NotImplementedException();
        }

        /// <summary>Indicates a Geography Intersects() method.</summary>
        /// <returns>The operation result.</returns>
        /// <param name="operand1">The Operand 1, point.</param>
        /// <param name="operand2">The Operand 2, polygon.</param>
        public virtual bool Intersects(Geography operand1, Geography operand2)
        {
            throw new NotImplementedException();
        }
    }
}
