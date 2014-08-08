//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for an out-of-line CSDL Annotations.
    /// </summary>
    internal class CsdlSemanticsAnnotations
    {
        private readonly CsdlAnnotations annotations;
        private readonly CsdlSemanticsSchema context;

        public CsdlSemanticsAnnotations(CsdlSemanticsSchema context, CsdlAnnotations annotations)
        {
            this.context = context;
            this.annotations = annotations;
        }

        public CsdlSemanticsSchema Context
        {
            get { return this.context; }
        }

        public CsdlAnnotations Annotations
        {
            get { return this.annotations; }
        }
    }
}
