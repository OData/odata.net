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

using System;
using System.Collections.Generic;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlTypeAnnotation.
    /// </summary>
    internal class CsdlSemanticsTypeAnnotation : CsdlSemanticsVocabularyAnnotation, IEdmTypeAnnotation
    {
        private readonly Cache<CsdlSemanticsTypeAnnotation, IEnumerable<IEdmPropertyValueBinding>> propertiesCache = new Cache<CsdlSemanticsTypeAnnotation, IEnumerable<IEdmPropertyValueBinding>>();
        private static readonly Func<CsdlSemanticsTypeAnnotation, IEnumerable<IEdmPropertyValueBinding>> ComputePropertiesFunc = (me) => me.ComputeProperties();

        public CsdlSemanticsTypeAnnotation(CsdlSemanticsSchema schema, IEdmVocabularyAnnotatable targetContext, CsdlSemanticsAnnotations annotationsContext, CsdlTypeAnnotation annotation, string externalQualifier)
            : base(schema, targetContext, annotationsContext, annotation, externalQualifier)
        {
        }

        public IEnumerable<IEdmPropertyValueBinding> PropertyValueBindings
        {
            get { return this.propertiesCache.GetValue(this, ComputePropertiesFunc, null); }
        }

        protected override IEdmTerm ComputeTerm()
        {
            // The cast to IEdmTerm is safe because FindType only returns schema types, and there is a filter to only structured types. 
            // The only two types that can survive are Entity and Complex types, both of which are type terms.
            return (IEdmTerm)(this.Schema.FindType(this.Annotation.Term) as IEdmStructuredType ?? new UnresolvedTypeTerm(this.Schema.UnresolvedName(this.Annotation.Term)));
        }

        private IEnumerable<IEdmPropertyValueBinding> ComputeProperties()
        {
            List<IEdmPropertyValueBinding> properties = new List<IEdmPropertyValueBinding>();

            foreach (CsdlPropertyValue propertyValue in ((CsdlTypeAnnotation)this.Annotation).Properties)
            {
                properties.Add(new CsdlSemanticsPropertyValueBinding(this, propertyValue));
            }

            return properties;
        }
    }
}
