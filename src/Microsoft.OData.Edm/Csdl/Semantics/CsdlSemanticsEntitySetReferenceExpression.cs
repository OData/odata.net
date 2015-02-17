//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntitySetReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsEntitySetReferenceExpression : CsdlSemanticsExpression, IEdmEntitySetReferenceExpression, IEdmCheckable
    {
        private readonly CsdlEntitySetReferenceExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsEntitySetReferenceExpression, IEdmEntitySet> referencedCache = new Cache<CsdlSemanticsEntitySetReferenceExpression, IEdmEntitySet>();
        private static readonly Func<CsdlSemanticsEntitySetReferenceExpression, IEdmEntitySet> ComputeReferencedFunc = (me) => me.ComputeReferenced();

        public CsdlSemanticsEntitySetReferenceExpression(CsdlEntitySetReferenceExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EntitySetReference; }
        }

        public IEdmEntitySet ReferencedEntitySet
        {
            get { return this.referencedCache.GetValue(this, ComputeReferencedFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.ReferencedEntitySet is IUnresolvedElement)
                {
                    return this.ReferencedEntitySet.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        private IEdmEntitySet ComputeReferenced()
        {
            string[] path = this.expression.EntitySetPath.Split('/');
            Debug.Assert(path.Count() == 2, "Enum member path was not correctly split");

            return new UnresolvedEntitySet(path[1], new UnresolvedEntityContainer(path[0], this.Location), this.Location);
        }
    }
}
