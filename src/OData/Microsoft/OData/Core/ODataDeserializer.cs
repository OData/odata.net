//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
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
        /// true if the WCF DS server compatibility format behavior should be used; otherwise false.
        /// </summary>
        internal bool UseServerFormatBehavior
        {
            get
            {
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
        internal DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker()
        {
            return this.inputContext.CreateDuplicatePropertyNamesChecker();
        }
    }
}
