//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
{
    /// <summary>
    /// Represents an EDM guid constant.
    /// </summary>
    public class EdmGuidConstant : EdmValue, IEdmGuidConstantExpression
    {
        private readonly Guid value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmGuidConstant"/> class.
        /// </summary>
        /// <param name="value">Integer value represented by this value.</param>
        public EdmGuidConstant(Guid value)
            : this(null, value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmGuidConstant"/> class.
        /// </summary>
        /// <param name="type">Type of the integer.</param>
        /// <param name="value">Integer value represented by this value.</param>
        public EdmGuidConstant(IEdmPrimitiveTypeReference type, Guid value)
            : base(type)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the definition of this value.
        /// </summary>
        public Guid Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.GuidConstant; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Guid; }
        }
    }
}
