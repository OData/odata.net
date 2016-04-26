//---------------------------------------------------------------------
// <copyright file="WriteFeedCallbackAnnotation.cs" company="Microsoft">
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
    /// Annotation to be used on the ODataResourceSet to tell the writer test descriptor to invoke a callback
    /// before/after the WriteStart and before the WriteEnd for that feed is called.
    /// </summary>
    public sealed class WriteFeedCallbacksAnnotation
    {
        /// <summary>
        /// Actions called right before the WriteStart for the feed is called.
        /// The parameter is the feed which is about to be written.
        /// </summary>
        public Action<ODataResourceSet> BeforeWriteStartCallback { get; set; }

        /// <summary>
        /// Actions called right after the WriteStart for the feed is called.
        /// The parameter is the feed which is being written.
        /// </summary>
        public Action<ODataResourceSet> AfterWriteStartCallback { get; set; }

        /// <summary>
        /// Actions called right before the WriteEnd for the feed is called.
        /// The parameter is the feed which is about to be closed.
        /// </summary>
        public Action<ODataResourceSet> BeforeWriteEndCallback { get; set; }
    }
}
