//---------------------------------------------------------------------
// <copyright file="IAnnotatable`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Contract which allows a given instance to store annotations.
    /// </summary>
    /// <typeparam name="TAnnotationBase">The base type of all annotations allowed on the instance.
    /// Can be used to restrict the allowed annotation.</typeparam>
    public interface IAnnotatable<TAnnotationBase> where TAnnotationBase : class
    {
        /// <summary>
        /// Gets a list of all available annotations on the instance
        /// </summary>
        IList<TAnnotationBase> Annotations { get; }
    }
}