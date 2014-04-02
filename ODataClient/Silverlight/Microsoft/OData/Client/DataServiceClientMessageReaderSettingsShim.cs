//---------------------------------------------------------------------
// <copyright file="DataServiceClientMessageReaderSettingsShim.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core;

    /// <summary>
    /// Client reader settings shim class to restrict settings to the base for OData message reader.
    /// </summary>
    internal class DataServiceClientMessageReaderSettingsShim : ODataMessageReaderSettingsBase
    {
        /// <summary> The settings. </summary>
        private readonly ODataMessageReaderSettingsBase settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceClientMessageReaderSettingsShim"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        internal DataServiceClientMessageReaderSettingsShim(ODataMessageReaderSettingsBase settings)
        {
            Debug.Assert(settings != null, "settings != null");
            this.settings = settings;
        }

        /// <summary>
        /// Flag to control whether ATOM metadata is read in ATOM payloads.
        /// </summary>
        public override bool EnableAtomMetadataReading
        {
            get { return this.settings.EnableAtomMetadataReading; }
            set { this.settings.EnableAtomMetadataReading = value; }
        }

        /// <summary>
        /// Flag to control whether the writer should check for valid Xml characters or not.
        /// </summary>
        public override bool CheckCharacters
        {
            get { return this.settings.CheckCharacters; }
            set { this.settings.CheckCharacters = value; }
        }

        /// <summary>
        /// Quotas to use for limiting resource consumption when writing an OData message.
        /// </summary>
        public override ODataMessageQuotas MessageQuotas
        {
            get { return this.settings.MessageQuotas; }
            set { this.settings.MessageQuotas = value; }
        }

        /// <summary>
        /// Func to evaluate whether an instance annotation should be read or skipped by the reader. The func should return true if the instance annotation should
        /// be read and false if the instance annotation should be skipped.
        /// </summary>
        public override Func<string, bool> ShouldIncludeAnnotation
        {
            get { return this.settings.ShouldIncludeAnnotation; }
            set { this.settings.ShouldIncludeAnnotation = value; }
        }
    }
}
