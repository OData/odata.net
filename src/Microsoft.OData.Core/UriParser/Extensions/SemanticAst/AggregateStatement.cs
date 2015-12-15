//---------------------------------------------------------------------
// <copyright file="AggregateStatement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Extensions;
    using Microsoft.OData.Core.UriParser.Semantic;

    public sealed class AggregateStatement
    {
        private readonly AggregationVerb withVerb;

        private readonly SingleValueNode expression;

        private readonly string asAlias;

        private readonly SingleValuePropertyAccessNode from;

        public AggregateStatement(SingleValueNode expression, AggregationVerb withVerb, SingleValuePropertyAccessNode from, string asAlias, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            // OK for from to be null
            ExceptionUtils.CheckArgumentNotNull(asAlias, "asAlias");
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            this.expression = expression;
            this.withVerb = withVerb;
            this.from = from;
            this.asAlias = asAlias;
            this._typeReference = typeReference;
        }


        public SingleValueNode Expression
        {
            get {
                return expression;
            }
        }

        public AggregationVerb WithVerb
        {
            get
            {
                return withVerb;
            }
        }

        public string AsAlias
        {
            get
            {
                return asAlias;
            }
        }

        public SingleValuePropertyAccessNode From
        {
            get
            {
                return from;
            }
        }

        private readonly IEdmTypeReference _typeReference;

        public IEdmTypeReference TypeReference
        {
            get
            {
                
                return this._typeReference;
            }
        }
    }
}
