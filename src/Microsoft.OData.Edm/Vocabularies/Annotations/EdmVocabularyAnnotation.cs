//---------------------------------------------------------------------
// <copyright file="EdmVocabularyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM annotation with an immediate value.
    /// </summary>
    public class EdmVocabularyAnnotation : EdmElement, IEdmVocabularyAnnotation
    {
        private readonly IEdmVocabularyAnnotatable target;
        private readonly IEdmTerm term;
        private readonly string qualifier;
        private readonly IEdmExpression value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmVocabularyAnnotation"/> class without providing a value.
        /// The given <see cref="IEdmTerm"/> should have the default value and can be translated to <see cref="IEdmExpression"/>.
        /// </summary>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="target">Element the annotation applies to.</param>
        public EdmVocabularyAnnotation(IEdmTerm term, IEdmVocabularyAnnotatable target)
            : this(term, target, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmVocabularyAnnotation"/> class without providing a value.
        /// The given <see cref="IEdmTerm"/> should have the default value and can be translated to <see cref="IEdmExpression"/>.
        /// </summary>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        /// <remarks>
        /// The reason to use different order of parameters is because the constructors with three parameters are indistinguishable.
        /// </remarks>
        public EdmVocabularyAnnotation(IEdmTerm term, IEdmVocabularyAnnotatable target, string qualifier)
        {
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(target, "target");

            this.term = term;
            this.target = target;
            this.qualifier = qualifier;
            this.value = term.GetDefaultValueExpression();
            if (this.value == null)
            {
                throw new InvalidOperationException(Strings.EdmVocabularyAnnotations_DidNotFindDefaultValue(term.Type));
            }

            UseDefault = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmVocabularyAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, IEdmExpression value)
            : this(target, term, null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmVocabularyAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        /// <param name="value">Expression producing the value of the annotation. It could be null.</param>
        public EdmVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, string qualifier, IEdmExpression value)
        {
            EdmUtil.CheckArgumentNull(target, "target");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(value, "value");

            this.target = target;
            this.term = term;
            this.qualifier = qualifier;
            this.value = value;
            UseDefault = false;
        }

        /// <summary>
        /// Gets the element the annotation applies to.
        /// </summary>
        public IEdmVocabularyAnnotatable Target
        {
            get { return this.target; }
        }

        /// <summary>
        /// Gets the term bound by the annotation.
        /// </summary>
        public IEdmTerm Term
        {
            get { return this.term; }
        }

        /// <summary>
        /// Gets the qualifier used to discriminate between multiple bindings of the same property or type.
        /// </summary>
        public string Qualifier
        {
            get { return this.qualifier; }
        }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        public IEdmExpression Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets whether the annotation uses a default value.
        /// In vNext, need refactor this.
        /// </summary>
        internal bool UseDefault { get; }
    }
}
