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

    /// <summary>
    /// Represents a function parameter that is either entity or collection of entities.
    /// </summary>
    public sealed class UriEntityOperationParameter : UriOperationParameter
    {
        /// <summary>
        /// If true, use entity reference instead of entity as the operation parameter.
        /// </summary>
        private readonly bool useEntityReference;

        /// <summary> Instantiates a new UriEntityOperationParameter </summary>
        /// <param name="name">The name of the uri operation parameter.</param>
        /// <param name="value">The value of the uri operation parameter.</param>
        public UriEntityOperationParameter(string name, Object value)
            : base(name, value)
        {
        }

        /// <summary> Instantiates a new UriOperationParameter </summary>
        /// <param name="name">The name of the uri operation parameter.</param>
        /// <param name="value">The value of the uri operation parameter.</param>
        /// <param name="useEntityReference">If true, use entity reference, instead of entity to serialize the parameter.</param>
        public UriEntityOperationParameter(string name, Object value, bool useEntityReference)
            : this(name, value)
        {
            this.useEntityReference = useEntityReference;
        }


        /// <summary>Use entity reference link.</summary>
        /// <returns>True if it uses entity reference link.</returns>
        internal bool UseEntityReference
        {
            get { return this.useEntityReference; }
        }
    }
}
