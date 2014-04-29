//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
