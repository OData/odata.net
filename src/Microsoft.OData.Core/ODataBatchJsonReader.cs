//---------------------------------------------------------------------
// <copyright file="ODataBatchJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{

    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif

    using Microsoft.OData.Core.JsonLight;
    #endregion Namespaces

    /* TODO: biaol -- WIP, part of reader request parsing. Current skeleton is for test passing purpose only. */
    /// <summary>
    /// Class for reading OData batch messages in json format; also verifies the proper sequence of read calls on the reader.
    /// </summary>
    internal sealed class ODataBatchJsonReader: ODataBatchReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read the content from.</param>
        /// <param name="batchEncoding">The encoding to use to read from the batch stream.</param>
        /// <param name="synchronous">true if the reader is created for synchronous operation; false for asynchronous.</param>
        internal ODataBatchJsonReader(ODataJsonLightInputContext inputContext, Encoding batchEncoding, bool synchronous)
            /* TODO: biaol --- revamp the base class for inputContext and boundary parameters) */
            : base(inputContext, null, batchEncoding, synchronous)
        {

        }
    }
}
