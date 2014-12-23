//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
