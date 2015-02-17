//---------------------------------------------------------------------
// <copyright file="IDeepCloneableAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Contracts that indicates if the annotation provides a deep clone. This cannot be generic IDeepCloneableAnnotation because
    /// then the converters which use this would need to have reference to the specialized types or be generic themselves.
    /// Also making (naming) this annotation specific since other class hierarchies are cloned using visitors.
    /// </summary>
    public interface IDeepCloneableAnnotation
    {
        /// <summary>
        /// Deep clone of the instance
        /// </summary>
        /// <returns>A deep clone</returns>
        Annotation DeepClone();
    }
}
