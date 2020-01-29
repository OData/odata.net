//---------------------------------------------------------------------
// <copyright file="EdmNullExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM null.
    /// </summary>
    public class EdmNullExpression : EdmValue, IEdmNullExpression
    {
        /// <summary>
        /// Singleton <see cref="EdmNullExpression"/> instance.
        /// </summary>
        public static EdmNullExpression Instance = new EdmNullExpression();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNullExpression"/> class.
        /// </summary>
        private EdmNullExpression()
            : base(null)
        {
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Null; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Null; }
        }
    }
}
