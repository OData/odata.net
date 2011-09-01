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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlTypeAnnotation.
    /// </summary>
    internal class CsdlSemanticsTypeAnnotation : CsdlSemanticsTermAnnotation, IEdmTypeAnnotation
    {
        private readonly Cache<CsdlSemanticsTypeAnnotation, IEnumerable<IEdmPropertyValueBinding>> propertiesCache = new Cache<CsdlSemanticsTypeAnnotation, IEnumerable<IEdmPropertyValueBinding>>();
        private readonly static Func<CsdlSemanticsTypeAnnotation, IEnumerable<IEdmPropertyValueBinding>> s_computeProperties = (me) => me.ComputeProperties();

        public CsdlSemanticsTypeAnnotation(CsdlSemanticsSchema schema, IEdmElement targetContext, CsdlSemanticsAnnotations annotationsContext, CsdlTypeAnnotation annotation, string externalQualifier)
            : base(schema, targetContext, annotationsContext, annotation, externalQualifier)
        {
        }

        public EdmAnnotationKind Kind
        {
            get { return EdmAnnotationKind.TermType; }
        }

        public IEnumerable<IEdmPropertyValueBinding> Properties
        {
            get { return this.propertiesCache.GetValue(this, s_computeProperties, null); }
        }

        protected override IEdmTerm ComputeTerm()
        {
            IEdmEntityType term = this.schema.FindType(this.annotation.Term) as IEdmEntityType;
            if (term == null)
            {
                string namespaceName;
                string namespaceUri;
                string name;
                TermName(out namespaceName, out namespaceUri, out name);

                term = new BadEntityType(namespaceName, namespaceUri, name, new EdmError[] { new EdmError(this.Location, EdmErrorCode.BadUnresolvedType, Edm.Strings.Bad_UnresolvedType(this.annotation.Term)) });
            }

            return term;
        }

        private IEnumerable<IEdmPropertyValueBinding> ComputeProperties()
        {
            List<IEdmPropertyValueBinding> properties = new List<IEdmPropertyValueBinding>();

            foreach (CsdlPropertyValue propertyValue in ((CsdlTypeAnnotation)this.annotation).Properties)
            {
                properties.Add(new CsdlSemanticsPropertyValueBinding(this, propertyValue));
            }

            return properties;
        }
    }
}
