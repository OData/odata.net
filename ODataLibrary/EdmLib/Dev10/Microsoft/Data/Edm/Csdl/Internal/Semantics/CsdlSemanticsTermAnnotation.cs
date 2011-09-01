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
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Common base class for CsdlSemanticsTypeAnnotation and CsdlSemanticsValueAnnotation.
    /// </summary>
    internal abstract class CsdlSemanticsTermAnnotation : CsdlSemanticsElement
    {
        protected readonly CsdlSemanticsSchema schema;
        protected readonly CsdlVocabularyAnnotationBase annotation;
        protected readonly string qualifier;
        private readonly IEdmElement targetContext;
        private readonly CsdlSemanticsAnnotations annotationsContext;

        private readonly Cache<CsdlSemanticsTermAnnotation, IEdmTerm> termCache = new Cache<CsdlSemanticsTermAnnotation, IEdmTerm>();
        private readonly static Func<CsdlSemanticsTermAnnotation, IEdmTerm> s_computeTerm = (me) => me.ComputeTerm();

        // Target cache.
        private readonly Cache<CsdlSemanticsTermAnnotation, IEdmElement> targetCache = new Cache<CsdlSemanticsTermAnnotation, IEdmElement>();
        private readonly static Func<CsdlSemanticsTermAnnotation, IEdmElement> s_computeTarget = (me) => me.ComputeTarget();

        protected CsdlSemanticsTermAnnotation(CsdlSemanticsSchema schema, IEdmElement targetContext, CsdlSemanticsAnnotations annotationsContext, CsdlVocabularyAnnotationBase externalQualifier, string qualifier)
        {
            this.schema = schema;
            this.annotation = externalQualifier;
            this.qualifier = qualifier ?? externalQualifier.Qualifier;
            this.targetContext = targetContext;
            this.annotationsContext = annotationsContext;
        }

        public CsdlSemanticsSchema Schema
        {
            get { return this.schema; }
        }

        public override CsdlElement Element
        {
            get { return this.annotation; }
        }

        public string Qualifier
        {
            get { return this.qualifier; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public IEdmTerm Term
        {
            get { return this.termCache.GetValue(this, s_computeTerm, null); }
        }

        public IEdmElement Target
        {
            get { return this.targetCache.GetValue(this, s_computeTarget, null); }
        }

        /// <summary>
        /// Gets the type to use as a binding context for expressions in the annotation. If the target of the annotation
        /// is an entity type, that is the binding context. If the target is an entity set, the binding context is the
        /// element type of the set.
        /// </summary>
        public IEdmEntityType TargetBindingContext
        {
            get
            {
                IEdmElement bindingTarget = this.Target;
                IEdmEntityType bindingContext = bindingTarget as IEdmEntityType;
                if (bindingContext == null)
                {
                    IEdmEntitySet entitySet = bindingTarget as IEdmEntitySet;
                    if (entitySet != null)
                    {
                        bindingContext = entitySet.ElementType;
                    }
                }

                return bindingContext;
            }
        }

        protected void TermName(out string namespaceName, out string namespaceUri, out string name)
        {
            if (EdmUtil.TryGetNamespaceNameFromQualifiedName(this.annotation.Term, out namespaceName, out name))
            {
                namespaceUri = this.schema.GetNamespaceUri(namespaceName);
            }
            else
            {
                namespaceName = string.Empty;
                name = this.annotation.Term;
                namespaceUri = string.Empty;
            }
        }

        private IEdmElement ComputeTarget()
        {
            if (this.targetContext != null)
            {
                return this.targetContext;
            }

            return this.annotationsContext != null ? this.schema.FindElement(this.annotationsContext.Annotations.Target) : null;
        }

        protected abstract IEdmTerm ComputeTerm();
    }
}
