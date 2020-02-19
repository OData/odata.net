//---------------------------------------------------------------------
// <copyright file="BadLabeledExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a semantically invalid EDM labeled expression.
    /// </summary>
    internal class BadLabeledExpression : BadElement, IEdmLabeledExpression
    {
        private readonly string name;

        private readonly Cache<BadLabeledExpression, IEdmExpression> expressionCache = new Cache<BadLabeledExpression, IEdmExpression>();
        private static readonly Func<BadLabeledExpression, IEdmExpression> ComputeExpressionFunc = (me) => ComputeExpression();

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

        private static IEdmExpression ComputeExpression()
        {
            return EdmNullExpression.Instance;
        }
    }
}
