//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
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
