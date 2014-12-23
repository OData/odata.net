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

namespace Microsoft.OData.Edm
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an EDM referential constraint on a navigation property.
    /// </summary>
    public interface IEdmReferentialConstraint
    {
        /// <summary>
        /// Gets the set of property pairs from the referential constraint. No particular ordering should be assumed.
        /// </summary>
        IEnumerable<EdmReferentialConstraintPropertyPair> PropertyPairs { get; }
    }

    /// <summary>
    /// Represents a pair of properties as part of a referential constraint.
    /// </summary>
    public class EdmReferentialConstraintPropertyPair
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EdmReferentialConstraintPropertyPair"/>.
        /// </summary>
        /// <param name="dependentProperty">The local or dependent property in the referential constraint property pair. A null value is not allowed.</param>
        /// <param name="principalProperty">The foreign or principal property in the referential constraint property pair. A null value is not allowed.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if either of the properties given are null.</exception>
        public EdmReferentialConstraintPropertyPair(IEdmStructuralProperty dependentProperty, IEdmStructuralProperty principalProperty)
        {
            EdmUtil.CheckArgumentNull(dependentProperty, "dependentProperty");
            EdmUtil.CheckArgumentNull(principalProperty, "principalProperty");

            this.DependentProperty = dependentProperty;
            this.PrincipalProperty = principalProperty;
        }

        /// <summary>
        /// The local or dependent property in the referential constraint property pair. Will never be null.
        /// </summary>
        public IEdmStructuralProperty DependentProperty { get; private set; }

        /// <summary>
        /// The foreign or principal property in the referential constraint property pair. Will never be null.
        /// </summary>
        public IEdmStructuralProperty PrincipalProperty { get; private set; }
    }
}
