//---------------------------------------------------------------------
// <copyright file="AnnotatedQueryItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Base class for all query items that have annotations
    /// </summary>
    public abstract class AnnotatedQueryItem : IAnnotatable<QueryAnnotation>
    {
        /// <summary>
        /// Initializes a new instance of the AnnotatedQueryItem class
        /// </summary>
        protected AnnotatedQueryItem()
        {
            this.Annotations = new List<QueryAnnotation>();
        }

        /// <summary>
        /// Gets all the annotations
        /// </summary>
        public IList<QueryAnnotation> Annotations { get; private set; }
    }
}
