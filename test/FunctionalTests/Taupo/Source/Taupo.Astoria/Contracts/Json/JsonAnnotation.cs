//---------------------------------------------------------------------
// <copyright file="JsonAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    /// <summary>
    /// The base class for all annotations on the JsonValue and related classes
    /// </summary>
    public abstract class JsonAnnotation
    {
        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>the clone of the annotation</returns>
        public abstract JsonAnnotation Clone();
    }
}
