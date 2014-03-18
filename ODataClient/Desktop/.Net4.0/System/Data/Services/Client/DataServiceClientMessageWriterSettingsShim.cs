//---------------------------------------------------------------------
// <copyright file="DataServiceClientMessageWriterSettingsShim.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Services.Client
{
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>
    /// Client writer settings shim class to restrict settings to the base for OData message writers.
    /// </summary>
    internal class DataServiceClientMessageWriterSettingsShim : ODataMessageWriterSettingsBase
    {
        /// <summary> The settings. </summary>
        private readonly ODataMessageWriterSettingsBase settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceClientMessageWriterSettingsShim"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        internal DataServiceClientMessageWriterSettingsShim(ODataMessageWriterSettingsBase settings)
        {
            Debug.Assert(settings != null, "settings != null");
            this.settings = settings;
        }

        /// <summary>
        /// Flag to control whether the writer should use indentation or not.
        /// </summary>
        public override bool Indent
        {
            get { return this.settings.Indent; }
            set { this.settings.Indent = value; }
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
    }
}