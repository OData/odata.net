//---------------------------------------------------------------------
// <copyright file="EdmAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Annotations
{
    /// <summary>
    /// Represents an EDM annotation.
    /// </summary>
    public class EdmAnnotation : EdmVocabularyAnnotation, IEdmValueAnnotation
    {
        private readonly IEdmExpression value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmAnnotation(IEdmVocabularyAnnotatable target, IEdmValueTerm term, IEdmExpression value)
            : this(target, term, null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmAnnotation(IEdmVocabularyAnnotatable target, IEdmValueTerm term, string qualifier, IEdmExpression value)
            : base(target, term, qualifier)
        {
            EdmUtil.CheckArgumentNull(value, "value");
            this.value = value;
        }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        public IEdmExpression Value
        {
            get { return this.value; }
        }
    }
}
