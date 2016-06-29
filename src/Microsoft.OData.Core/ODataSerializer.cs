//---------------------------------------------------------------------
// <copyright file="ODataSerializer.cs" company="Microsoft">
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
    /// Base class for all OData serializers.
    /// </summary>
    internal abstract class ODataSerializer
    {
        /// <summary>The writer validator used during serializing.</summary>
        protected readonly IWriterValidator WriterValidator;

        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataOutputContext outputContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="outputContext">The output context to write to.</param>
        protected ODataSerializer(ODataOutputContext outputContext)
        {
            Debug.Assert(outputContext != null, "outputContext != null");

            this.outputContext = outputContext;
            this.WriterValidator = outputContext.WriterValidator;
        }

        /// <summary>
        /// The message writer settings.
        /// </summary>
        internal ODataMessageWriterSettings MessageWriterSettings
        {
            get
            {
                return this.outputContext.MessageWriterSettings;
            }
        }

        /// <summary>
        /// The URL converter.
        /// </summary>
        internal IODataPayloadUriConverter PayloadUriConverter
        {
            get
            {
                return this.outputContext.PayloadUriConverter;
            }
        }

        /// <summary>
        /// true if the output is a response payload; false if it's a request payload.
        /// </summary>
        internal bool WritingResponse
        {
            get
            {
                return this.outputContext.WritingResponse;
            }
        }

        /// <summary>
        /// The model to use.
        /// </summary>
        internal IEdmModel Model
        {
            get
            {
                return this.outputContext.Model;
            }
        }

        /// <summary>
        /// Creates a new instance of a duplicate property names checker.
        /// </summary>
        /// <returns>The newly created instance of duplicate property names checker.</returns>
        internal IDuplicatePropertyNameChecker CreateDuplicatePropertyNameChecker()
        {
            return MessageWriterSettings.Validator.CreateDuplicatePropertyNameChecker();
        }
    }
}
