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

using System.Collections.Generic;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Collection expression.
    /// </summary>
    internal class CsdlCollectionExpression : CsdlExpressionBase
    {
        private readonly CsdlTypeReference type;
        private readonly List<CsdlExpressionBase> elementValues;

        public CsdlCollectionExpression(CsdlTypeReference type, IEnumerable<CsdlExpressionBase> elementValues, CsdlLocation location)
            : base(location)
        {
            this.type = type;
            this.elementValues = new List<CsdlExpressionBase>(elementValues);
        }

        public override Expressions.EdmExpressionKind ExpressionKind
        {
            get { return Expressions.EdmExpressionKind.Collection; }
        }

        public CsdlTypeReference Type
        {
            get { return this.type; }
        }

        public IEnumerable<CsdlExpressionBase> ElementValues
        {
            get { return this.elementValues; }
        }
    }
}
