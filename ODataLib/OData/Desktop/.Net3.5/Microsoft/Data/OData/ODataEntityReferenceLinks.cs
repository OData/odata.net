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
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Represents a collection of entity reference links (the result of a $links query).
    /// Might include an inline count and a next link.
    /// </summary>
    public sealed class ODataEntityReferenceLinks : ODataAnnotatable
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataMessageWriter"/> for this <see cref="ODataEntityReferenceLinks"/>.
        /// </summary>
        private ODataEntityReferenceLinksSerializationInfo serializationInfo;

        /// <summary>Gets or sets the optional inline count of the $links collection.</summary>
        /// <returns>The optional inline count of the $links collection.</returns>
        public long? Count
        {
            get;
            set;
        }

        /// <summary>Gets or sets the optional next link of the $links collection.</summary>
        /// <returns>The optional next link of the $links collection.</returns>
        public Uri NextPageLink
        {
            get;
            set;
        }

        /// <summary>Gets or sets the enumerable of <see cref="T:Microsoft.Data.OData.ODataEntityReferenceLink" /> instances representing the links of the referenced entities.</summary>
        /// <returns>The enumerable of <see cref="T:Microsoft.Data.OData.ODataEntityReferenceLink" /> instances.</returns>
        /// <remarks>These links should be usable to retrieve or modify the referenced entities.</remarks>
        public IEnumerable<ODataEntityReferenceLink> Links
        {
            get;
            set;
        }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataMessageWriter"/> for this <see cref="ODataEntityReferenceLinks"/>.
        /// </summary>
        internal ODataEntityReferenceLinksSerializationInfo SerializationInfo
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serializationInfo;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.serializationInfo = ODataEntityReferenceLinksSerializationInfo.Validate(value);
            }
        }
    }
}
