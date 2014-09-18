//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Client
{
    using System.Diagnostics;
    using Microsoft.OData.Core;

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
