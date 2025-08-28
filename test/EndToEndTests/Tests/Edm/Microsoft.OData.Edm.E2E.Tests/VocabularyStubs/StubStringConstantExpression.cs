//---------------------------------------------------------------------
// <copyright file="StubStringConstantExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.E2E.Tests.StubEdm;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.VocabularyStubs;

public class StubStringConstantExpression : StubEdmElement, IEdmStringConstantExpression
{
    public StubStringConstantExpression(string value)
    {
        this.Value = value;
    }

    public string Value { get; set; }

    public EdmExpressionKind ExpressionKind
    {
        get { return EdmExpressionKind.StringConstant; }
    }

    public EdmValueKind ValueKind
    {
        get { return EdmValueKind.String; }
    }

    public override string ToString()
    {
        return string.Format("StringConstant(\"{0}\")", Value);
    }

    public override bool Equals(object obj)
    {
        if (obj is not IEdmStringConstantExpression another)
        {
            return false;
        }

        return this.Value == another.Value;
    }

    public override int GetHashCode()
    {
        return this.Value.GetHashCode();
    }

    public Microsoft.OData.Edm.IEdmTypeReference Type
    {
        get { return Microsoft.OData.Edm.EdmCoreModel.Instance.GetString(false); }
    }
}

