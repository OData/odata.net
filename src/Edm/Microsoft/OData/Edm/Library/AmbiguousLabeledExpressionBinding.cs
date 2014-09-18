//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
