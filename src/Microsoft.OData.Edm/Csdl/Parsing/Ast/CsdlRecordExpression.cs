//---------------------------------------------------------------------
// <copyright file="CsdlRecordExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Json;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Record expression.
    /// </summary>
    internal class CsdlRecordExpression : CsdlExpressionBase
    {
        private readonly CsdlTypeReference type;
        private readonly List<CsdlPropertyValue> propertyValues;

        public CsdlRecordExpression(CsdlTypeReference type, IEnumerable<CsdlPropertyValue> propertyValues, CsdlLocation location)
            : base(location)
        {
            this.type = type;
            this.propertyValues = new List<CsdlPropertyValue>(propertyValues);
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Record; }
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }

        public IEnumerable<CsdlPropertyValue> PropertyValues
        {
            get { return this.propertyValues; }
        }
    }

    /// <summary>
    /// Represents a CSDL Record expression.
    /// </summary>
    internal class CsdlJsonValueExpression : CsdlExpressionBase
    {
        public CsdlJsonValueExpression(string termName, IJsonValue jsonValue, CsdlLocation location)
            : base(location)
        {
            TermName = termName;
            JsonValue = jsonValue;
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.None; }
        }

        // Qualified term name
        public string TermName { get; }

        public IJsonValue JsonValue { get; }
    }
}
