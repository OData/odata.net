//---------------------------------------------------------------------
// <copyright file="WriteEntryCallbacksAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Annotation to be used on the ODataResource to tell the writer test descriptor to invoke a callback
    /// before the WriteStart and before the WriteEnd for that entry is called.
    /// </summary>
    public sealed class WriteEntryCallbacksAnnotation
    {
        /// <summary>
        /// Actions called right before the WriteStart for the entry is called.
        /// The parameter is the entry which is about to be written.
        /// </summary>
        public Action<ODataResource> BeforeWriteStartCallback { get; set; }

        /// <summary>
        /// Actions called right before the WriteEnd for the entry is called.
        /// The parameter is the entry which is about to be closed.
        /// </summary>
        public Action<ODataResource> BeforeWriteEndCallback { get; set; }
    }
}
