//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Common type for a CSDL annotation.
    /// </summary>
    internal class CsdlAnnotation : CsdlElement
    {
        private readonly CsdlExpressionBase expression;
        private readonly string qualifier;
        private readonly string term;

        public CsdlAnnotation(string term, string qualifier, CsdlExpressionBase expression, CsdlLocation location)
            : base(location)
        {
            this.expression = expression;
            this.qualifier = qualifier;
            this.term = term;
        }

        public CsdlExpressionBase Expression
        {
            get { return this.expression; }
        }

        public string Qualifier
        {
            get { return this.qualifier; }
        }

        public string Term
        {
            get { return this.term; }
        }
    }
}
