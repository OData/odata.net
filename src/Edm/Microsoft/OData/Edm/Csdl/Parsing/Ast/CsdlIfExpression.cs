//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlIfExpression : CsdlExpressionBase
    {
        private readonly CsdlExpressionBase test;
        private readonly CsdlExpressionBase ifTrue;
        private readonly CsdlExpressionBase ifFalse;

        public CsdlIfExpression(CsdlExpressionBase test, CsdlExpressionBase ifTrue, CsdlExpressionBase ifFalse, CsdlLocation location)
            : base(location)
        {
            this.test = test;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        public override Expressions.EdmExpressionKind ExpressionKind
        {
            get { return Expressions.EdmExpressionKind.If; }
        }

        public CsdlExpressionBase Test
        {
            get { return this.test; }
        }

        public CsdlExpressionBase IfTrue
        {
            get { return this.ifTrue; }
        }

        public CsdlExpressionBase IfFalse
        {
            get { return this.ifFalse; }
        }
    }
}
