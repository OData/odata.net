//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
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
