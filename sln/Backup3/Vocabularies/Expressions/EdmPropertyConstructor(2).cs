//---------------------------------------------------------------------
// <copyright file="EdmPropertyConstructor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM property constructor specified as part of a EDM record construction expression.
    /// </summary>
    public class EdmPropertyConstructor : EdmElement, IEdmPropertyConstructor
    {
        private readonly string name;
        private readonly IEdmExpression value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyConstructor"/> class.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public EdmPropertyConstructor(string name, IEdmExpression value)
        {
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(value, "value");

            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the expression for the value of the property.
        /// </summary>
        public IEdmExpression Value
        {
            get { return this.value; }
        }
    }
}
