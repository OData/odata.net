//---------------------------------------------------------------------
// <copyright file="EdmRecordExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM record construction expression.
    /// </summary>
    public class EdmRecordExpression : EdmElement, IEdmRecordExpression
    {
        private readonly IEdmStructuredTypeReference declaredType;
        private readonly IEnumerable<IEdmPropertyConstructor> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRecordExpression"/> class.
        /// </summary>
        /// <param name="properties">Property constructors.</param>
        public EdmRecordExpression(params IEdmPropertyConstructor[] properties)
            : this((IEnumerable<IEdmPropertyConstructor>)properties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRecordExpression"/> class.
        /// </summary>
        /// <param name="declaredType">Declared type of the record.</param>
        /// <param name="properties">Property constructors.</param>
        public EdmRecordExpression(IEdmStructuredTypeReference declaredType, params IEdmPropertyConstructor[] properties)
            : this(declaredType, (IEnumerable<IEdmPropertyConstructor>)properties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRecordExpression"/> class.
        /// </summary>
        /// <param name="properties">Property constructors.</param>
        public EdmRecordExpression(IEnumerable<IEdmPropertyConstructor> properties)
            : this(null, properties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRecordExpression"/> class.
        /// </summary>
        /// <param name="declaredType">Optional declared type of the record.</param>
        /// <param name="properties">Property constructors.</param>
        public EdmRecordExpression(IEdmStructuredTypeReference declaredType, IEnumerable<IEdmPropertyConstructor> properties)
        {
            EdmUtil.CheckArgumentNull(properties, "properties");

            this.declaredType = declaredType;
            this.properties = properties;
        }

        /// <summary>
        /// Gets the declared type of the record, or null if there is no declared type.
        /// </summary>
        public IEdmStructuredTypeReference DeclaredType
        {
            get { return this.declaredType; }
        }

        /// <summary>
        /// Gets the constructed property values.
        /// </summary>
        public IEnumerable<IEdmPropertyConstructor> Properties
        {
            get { return this.properties; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Record; }
        }
    }
}
