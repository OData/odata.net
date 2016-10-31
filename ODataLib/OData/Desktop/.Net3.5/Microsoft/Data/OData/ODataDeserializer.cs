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
