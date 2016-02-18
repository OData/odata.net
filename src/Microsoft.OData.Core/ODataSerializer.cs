//---------------------------------------------------------------------
// <copyright file="ODataSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
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
        /// true if the WCF DS server compatibility format behavior should be used; otherwise false.
        /// </summary>
        internal bool UseServerFormatBehavior
        {
            get
            {
                return this.outputContext.UseServerFormatBehavior;
            }
        }

        /// <summary>
        /// true if the default format behavior should be used; otherwise false.
        /// </summary>
        internal bool UseDefaultFormatBehavior
        {
            get
            {
                return this.outputContext.UseDefaultFormatBehavior;
            }
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
        /// The URL resolver.
        /// </summary>
        internal IODataUrlResolver UrlResolver
        {
            get
            {
                return this.outputContext.UrlResolver;
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
        internal DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker()
        {
            return new DuplicatePropertyNamesChecker(this.MessageWriterSettings.WriterBehavior.AllowDuplicatePropertyNames, this.WritingResponse, !this.MessageWriterSettings.EnableFullValidation);
        }
    }
}