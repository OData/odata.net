//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library.Values;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a labeled expression binding to more than one item.
    /// </summary>
    internal class AmbiguousLabeledExpressionBinding : AmbiguousBinding<IEdmLabeledExpression>, IEdmLabeledExpression
    {
        private readonly IEdmLabeledExpression first;

        private readonly Cache<AmbiguousLabeledExpressionBinding, IEdmExpression> expressionCache = new Cache<AmbiguousLabeledExpressionBinding, IEdmExpression>();
        private static readonly Func<AmbiguousLabeledExpressionBinding, IEdmExpression> ComputeExpressionFunc = (me) => me.ComputeExpression();

        public AmbiguousLabeledExpressionBinding(IEdmLabeledExpression first, IEdmLabeledExpression second)
            : base(first, second)
        {
            this.first = first;
        }

        public IEdmExpression Expression
        {
            get { return this.expressionCache.GetValue(this, ComputeExpressionFunc, null); }
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Labeled; }
        }

        private IEdmExpression ComputeExpression()
        {
            return EdmNullExpression.Instance;
        }
    }
}
