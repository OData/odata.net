//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Property Path expression.
    /// </summary>
    internal class CsdlSemanticsPropertyPathExpression : CsdlSemanticsPathExpression
    {
        public CsdlSemanticsPropertyPathExpression(CsdlPathExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema) 
            : base(expression, bindingContext, schema)
        {
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.PropertyPath; }
        }
    }
}
