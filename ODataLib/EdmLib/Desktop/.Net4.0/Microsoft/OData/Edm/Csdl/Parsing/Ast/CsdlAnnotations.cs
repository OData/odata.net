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
    /// Represents a CSDL Annotations element.
    /// </summary>
    internal class CsdlAnnotations
    {
        private readonly List<CsdlAnnotation> annotations;
        private readonly string target;
        private readonly string qualifier;

        public CsdlAnnotations(IEnumerable<CsdlAnnotation> annotations, string target, string qualifier)
        {
            this.annotations = new List<CsdlAnnotation>(annotations);
            this.target = target;
            this.qualifier = qualifier;
        }

        public IEnumerable<CsdlAnnotation> Annotations
        {
            get { return this.annotations; }
        }

        public string Qualifier
        {
            get { return this.qualifier; }
        }

        public string Target
        {
            get { return this.target; }
        }
    }
}
