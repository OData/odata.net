//---------------------------------------------------------------------
// <copyright file="AnnotatedItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Base class for all MetadataItems that has annotation
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public abstract class AnnotatedItem : IAnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the AnnotatedItem class
        /// </summary>
        protected AnnotatedItem()
        {
            this.Annotations = new List<Annotation>();
        }

        /// <summary>
        /// Gets all the annotations
        /// </summary>
        public IList<Annotation> Annotations { get; private set; }

        /// <summary>
        /// Adds an Annotation to the Metadata item
        /// </summary>
        /// <param name="annotation">The Annotation to be added</param>
        public void Add(Annotation annotation)
        {
            this.Annotations.Add(annotation);
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }
    }
}
