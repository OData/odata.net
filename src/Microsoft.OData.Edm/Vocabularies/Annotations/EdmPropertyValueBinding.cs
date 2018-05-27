//---------------------------------------------------------------------
// <copyright file="EdmPropertyValueBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents a property binding specified as part of an EDM type annotation.
    /// </summary>
    public class EdmPropertyValueBinding : EdmElement, IEdmPropertyValueBinding
    {
        private readonly IEdmProperty boundProperty;
        private readonly IEdmExpression value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyValueBinding"/> class.
        /// </summary>
        /// <param name="boundProperty">Property that is given a value by the annotation.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmPropertyValueBinding(IEdmProperty boundProperty, IEdmExpression value)
        {
            EdmUtil.CheckArgumentNull(boundProperty, "boundProperty");
            EdmUtil.CheckArgumentNull(value, "value");

            this.boundProperty = boundProperty;
            this.value = value;
        }

        /// <summary>
        /// Gets the property that is given a value by the annotation.
        /// </summary>
        public IEdmProperty BoundProperty
        {
            get { return this.boundProperty; }
        }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        public IEdmExpression Value
        {
            get { return this.value; }
        }
    }
}
