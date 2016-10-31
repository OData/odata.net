//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
