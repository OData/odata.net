//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Class responsible for knowing how to perform operations for a particular implemenation of Spatial types
    /// </summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
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
