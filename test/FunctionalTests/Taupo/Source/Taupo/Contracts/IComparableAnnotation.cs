//---------------------------------------------------------------------
// <copyright file="IComparableAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// A contract that indicates the annotation has the ability to compare itself to another annotation of the same type. 
    /// Creating a new interface and not using IEquatable because that requires overriding object.Equals and object.GetHashCode. 
    /// This is intended to be used for comparing annotations inside other visitors or comparers for other types. 
    /// Such comparers can check whether annotations implement this and perform the comparison without having to know how to compare a specific annotation.
    /// Not making this generic since visitors or comparers using this will have to know(reference) the specialized types.
    /// </summary>
    public interface IComparableAnnotation
    {
        /// <summary>
        /// Compares the current instance with the <param>annotation</param>
        /// </summary>
        /// <param name="annotation"> annotation to compare with</param>
        /// <returns>true or false indicating whether the comparison succeeded or failed</returns>
        bool CompareAnnotation(Annotation annotation);
    }
}
