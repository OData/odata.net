//---------------------------------------------------------------------
// <copyright file="ODataDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System.Diagnostics;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Base class for all OData deserializers.
    /// </summary>
    internal abstract class ODataDeserializer
    {
        /// <summary>The reader validator to use for reading.</summary>
        protected IReaderValidator ReaderValidator;

        /// <summary>The input context to use for reading.</summary>
        private readonly ODataInputContext inputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read from.</param>
        protected ODataDeserializer(ODataInputContext inputContext)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.inputContext = inputContext;
            this.ReaderValidator = this.inputContext.MessageReaderSettings.Validator;
        }

        /// <summary>
        /// The message reader settings.
        /// </summary>
        internal ODataMessageReaderSettings MessageReaderSettings
        {
            get
            {
                return this.inputContext.MessageReaderSettings;
            }
        }

        /// <summary>
        /// true if the input is a response payload; false if it's a request payload.
        /// </summary>
        internal bool ReadingResponse
        {
            get
            {
                return this.inputContext.ReadingResponse;
            }
        }

        /// <summary>
        /// The model to use.
        /// </summary>
        internal IEdmModel Model
        {
            get
            {
                return this.inputContext.Model;
            }
        }

        /// <summary>
        /// Creates a new instance of a duplicate property names checker.
        /// </summary>
        /// <returns>The newly created instance of duplicate property names checker.</returns>
        internal PropertyAndAnnotationCollector CreatePropertyAndAnnotationCollector()
        {
            return this.inputContext.CreatePropertyAndAnnotationCollector();
        }
    }
}
