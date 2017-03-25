//---------------------------------------------------------------------
// <copyright file="ODataBatchJsonReader.cs" company="Microsoft">
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

    /// <summary>
    /// Class for reading OData batch messages in json format; also verifies the proper sequence of read calls on the reader.
    /// </summary>
    internal sealed class ODataBatchJsonReader: ODataBatchReader
    {

        internal ODataJsonLightInputContext JsonLightInputContext
        {
            get
            {
                return this.inputContext as ODataJsonLightInputContext;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read the content from.</param>
        /// <param name="batchEncoding">The encoding to use to read from the batch stream.</param>
        /// <param name="synchronous">true if the reader is created for synchronous operation; false for asynchronous.</param>
        internal ODataBatchJsonReader(ODataJsonLightInputContext inputContext, Encoding batchEncoding, bool synchronous)
            : base(inputContext, batchEncoding, synchronous)
        {

        }


        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationRequestMessage"/> for reading the content of an operation 
        /// in a batch request.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch request operation from.</returns>
        internal override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation()
        {
            return null;
        }

        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationRequestMessage"/> for reading the content of an operation 
        /// in a batch request.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch request operation from.</returns>
        internal override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation()
        {
            // TODO: biaol --- consumed by client lib only, later.
            return null;
        }

        /// <summary>
        /// Continues reading from the batch message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        internal override bool ReadImplementation()
        {
            return false;
        }
    }
}
