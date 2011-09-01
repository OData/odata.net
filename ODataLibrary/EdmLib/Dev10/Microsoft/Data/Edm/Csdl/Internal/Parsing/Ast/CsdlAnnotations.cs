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
    /// Represents a CSDL Annotations element.
    /// </summary>
    internal class CsdlAnnotations
    {
        private readonly List<CsdlVocabularyAnnotationBase> annotations;
        private readonly string target;
        private readonly string qualifier;

        public CsdlAnnotations(IEnumerable<CsdlVocabularyAnnotationBase> annotations, string target, string qualifier)
        {
            this.annotations = new List<CsdlVocabularyAnnotationBase>(annotations);
            this.target = target;
            this.qualifier = qualifier;
        }

        public IEnumerable<CsdlVocabularyAnnotationBase> Annotations
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
