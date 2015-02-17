//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Diagnostics;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a custom annotation for an OData payload element.
    /// </summary>
    [DebuggerDisplay("{StringRepresentation}")]
    public abstract class ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public abstract string StringRepresentation
        {
            get;
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public abstract ODataPayloadElementAnnotation Clone();
    }
}
