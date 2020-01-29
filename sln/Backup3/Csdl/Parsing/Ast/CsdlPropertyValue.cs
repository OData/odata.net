//---------------------------------------------------------------------
// <copyright file="CsdlPropertyValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL property value in an annotation.
    /// </summary>
    internal class CsdlPropertyValue : CsdlElement
    {
        private readonly CsdlExpressionBase expression;
        private readonly string property;

        public CsdlPropertyValue(string property, CsdlExpressionBase expression, CsdlLocation location)
            : base(location)
        {
            this.property = property;
            this.expression = expression;
        }

        public string Property
        {
            get { return this.property; }
        }

        public CsdlExpressionBase Expression
        {
            get { return this.expression; }
        }
    }
}
