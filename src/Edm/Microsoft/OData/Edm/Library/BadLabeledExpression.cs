//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM labeled expression.
    /// </summary>
    internal class BadLabeledExpression : BadElement, IEdmLabeledExpression
    {
        private readonly string name;

        private readonly Cache<BadLabeledExpression, IEdmExpression> expressionCache = new Cache<BadLabeledExpression, IEdmExpression>();
        private static readonly Func<BadLabeledExpression, IEdmExpression> ComputeExpressionFunc = (me) => me.ComputeExpression();

        public BadLabeledExpression(string name, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.name = name ?? string.Empty;
        }

        public string Name
        {
            get { return this.name; }
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Labeled; }
        }

        public IEdmExpression Expression
        {
            get { return this.expressionCache.GetValue(this, ComputeExpressionFunc, null); }
        }

        private IEdmExpression ComputeExpression()
        {
            return EdmNullExpression.Instance;
        }
    }
}
