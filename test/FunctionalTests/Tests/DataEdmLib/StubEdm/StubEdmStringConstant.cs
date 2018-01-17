//---------------------------------------------------------------------
// <copyright file="StubEdmStringConstant.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.StubEdm
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Represents an EDM string constant expression.
    /// </summary>
    public class StubEdmStringConstant : StubEdmElement, IEdmStringConstantExpression
    {
        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.StringConstant; }
        }

        /// <summary>
        /// Gets the value kind of this value.
        /// </summary>
        public EdmValueKind ValueKind
        {
            get { return EdmValueKind.String; }
        }

        /// <summary>
        /// Gets or sets the definition of this string value.
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of this value.
        /// </summary>
        public IEdmTypeReference Type
        {
            get;
            set;
        }
    }
}
