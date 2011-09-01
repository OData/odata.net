//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Path expression.
    /// </summary>
    internal class CsdlSemanticsPathExpression : IEdmPathExpression
    {
        private readonly CsdlPathExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsPathExpression, IEnumerable<string>> pathCache = new Cache<CsdlSemanticsPathExpression, IEnumerable<string>>();
        private readonly static Func<CsdlSemanticsPathExpression, IEnumerable<string>> s_computePath = (me) => me.ComputePath();

        private readonly Cache<CsdlSemanticsPathExpression, IEdmNamedElement> referencedCache = new Cache<CsdlSemanticsPathExpression, IEdmNamedElement>();
        private readonly static Func<CsdlSemanticsPathExpression, IEdmNamedElement> s_computeReferenced = (me) => me.ComputeReferenced();

        public CsdlSemanticsPathExpression(CsdlPathExpression expression, IEdmEntityType bindingContext)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }

        public IEnumerable<string> Path
        {
            get { return this.pathCache.GetValue(this, s_computePath, null); }
        }

        public IEdmNamedElement Referenced
        {
            get { return this.referencedCache.GetValue(this, s_computeReferenced, null); }
        }

        private IEnumerable<string> ComputePath()
        {
            return this.expression.Path.Split(new char[] { '.' }, StringSplitOptions.None);
        }

        private IEdmNamedElement ComputeReferenced()
        {
            IEdmType positionType = this.bindingContext;
            IEdmProperty position = null;
            foreach (string hop in this.Path)
            {
                position = null;
                IEdmStructuredType structuredPositionType = positionType as IEdmStructuredType;
                if (structuredPositionType != null)
                {
                    position = structuredPositionType.FindProperty(hop);
                }
                else
                {
                    IEdmSchemaType namedPositionType = positionType as IEdmSchemaType;
                    string typeName = namedPositionType != null ? namedPositionType.FullName() : string.Empty;
                    structuredPositionType = new BadEntityType(typeName, new EdmError[] { new EdmError(this.expression.Location, EdmErrorCode.PathExpressionHasNoEntityContext, Edm.Strings.EdmModel_Validator_Semantic_PathExpressionHasNoEntityContext(typeName)) });
                }

                if (position == null)
                {
                    position = new UnresolvedProperty(structuredPositionType, hop, this.expression.Location);
                }

                positionType = position.Type.Definition;
            }

            return position;
        }
    }
}
