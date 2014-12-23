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

namespace Microsoft.OData.Core
{
    using System;

    /// <summary>
    /// Represents either an added link or a deleted link in delta response. 
    /// </summary>
    public abstract class ODataDeltaLinkBase : ODataItem
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaLinkBase"/>.
        /// </summary>
        private ODataDeltaSerializationInfo serializationInfo;

        /// <summary>
        /// Initializes a new <see cref="ODataDeltaLinkBase"/>.
        /// </summary>
        /// <param name="source">The id of the entity from which the relationship is defined, which may be absolute or relative.</param>
        /// <param name="target">The id of the related entity, which may be absolute or relative.</param>
        /// <param name="relationship">The name of the relationship property on the parent object.</param>
        protected ODataDeltaLinkBase(Uri source, Uri target, string relationship)
        {
            this.Source = source;
            this.Target = target;
            this.Relationship = relationship;
        }

        /// <summary>
        /// The id of the entity from which the relationship is defined, which may be absolute or relative.
        /// </summary>
        public Uri Source { get; set; }

        /// <summary>
        /// The id of the related entity, which may be absolute or relative.
        /// </summary>
        public Uri Target { get; set; }

        /// <summary>
        /// The name of the relationship property on the parent object.
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaLinkBase"/>.
        /// </summary>
        internal ODataDeltaSerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataDeltaSerializationInfo.Validate(value);
            }
        }
    }
}
