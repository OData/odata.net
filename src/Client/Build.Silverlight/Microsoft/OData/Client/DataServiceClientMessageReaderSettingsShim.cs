//   OData .NET Libraries ver. 6.9
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
