//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    #endregion Namespaces
    
    /// <summary>
    /// Base class for all OData deserializers.
    /// </summary>
    internal abstract class ODataDeserializer
    {
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
        }

        /// <summary>
        /// true if the WCF DS client compatibility format behavior should be used; otherwise false.
        /// </summary>
        internal bool UseClientFormatBehavior
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.UseClientFormatBehavior;
            }
        }

        /// <summary>
        /// true if the WCF DS server compatibility format behavior should be used; otherwise false.
        /// </summary>
        internal bool UseServerFormatBehavior
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.UseServerFormatBehavior;
            }
        }

        /// <summary>
        /// true if the default format behavior should be used; otherwise false.
        /// </summary>
        internal bool UseDefaultFormatBehavior
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.UseDefaultFormatBehavior;
            }
        }

        /// <summary>
        /// The message reader settings.
        /// </summary>
        internal ODataMessageReaderSettings MessageReaderSettings
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.MessageReaderSettings;
            }
        }

        /// <summary>
        /// The OData version of the input.
        /// </summary>
        internal ODataVersion Version
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.Version;
            }
        }

        /// <summary>
        /// true if the input is a response payload; false if it's a request payload.
        /// </summary>
        internal bool ReadingResponse
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
                return this.inputContext.Model;
            }
        }

        /// <summary>
        /// Creates a new instance of a duplicate property names checker.
        /// </summary>
        /// <returns>The newly created instance of duplicate property names checker.</returns>
        internal DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker()
        {
            DebugUtils.CheckNoExternalCallers();

            return this.inputContext.CreateDuplicatePropertyNamesChecker();
        }
    }
}
