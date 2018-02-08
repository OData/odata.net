//---------------------------------------------------------------------
// <copyright file="AmbiguousLabeledExpressionBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a labeled expression binding to more than one item.
    /// </summary>
    internal class AmbiguousLabeledExpressionBinding : AmbiguousBinding<IEdmLabeledExpression>, IEdmLabeledExpression
    {
        private readonly Cache<AmbiguousLabeledExpressionBinding, IEdmExpression> expressionCache = new Cache<AmbiguousLabeledExpressionBinding, IEdmExpression>();
        private static readonly Func<AmbiguousLabeledExpressionBinding, IEdmExpression> ComputeExpressionFunc = (me) => ComputeExpression();

        public AmbiguousLabeledExpressionBinding(IEdmLabeledExpression first, IEdmLabeledExpression second)
            : base(first, second)
        {
        }

        public IEdmExpression Expression
        {
            get { return this.expressionCache.GetValue(this, ComputeExpressionFunc, null); }
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Labeled; }
        }

        private static IEdmExpression ComputeExpression()
        {
            return EdmNullExpression.Instance;
        }
    }
}
