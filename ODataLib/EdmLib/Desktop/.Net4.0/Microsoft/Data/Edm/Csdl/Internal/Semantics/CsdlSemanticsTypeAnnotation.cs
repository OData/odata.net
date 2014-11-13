//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
