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
    using System.Diagnostics;

    /// <summary>
    /// Simple ODataVersion specific cache.
    /// </summary>
    /// <typeparam name="T">The type of the item being cached.</typeparam>
    internal sealed class ODataVersionCache<T>
    {
        /// <summary>
        /// Lazy constructing T for ODataVersion.V4.
        /// </summary>
        private readonly SimpleLazy<T> v3;

        /// <summary>
        /// Constructs an instance of the ODataVersionCache.
        /// </summary>
        /// <param name="factory">The method to call to create a new instance of <typeparamref name="T"/> for a given ODataVersion.</param>
        internal ODataVersionCache(Func<ODataVersion, T> factory)
        {
            Debug.Assert(factory != null, "factory != null");

            this.v3 = new SimpleLazy<T>(() => factory(ODataVersion.V4), true /*isThreadSafe*/);
        }

        /// <summary>
        /// Indexer to get the cached item when given the ODataVersion.
        /// </summary>
        /// <param name="version">The ODataVersion to look up.</param>
        /// <returns>The cached item.</returns>
        internal T this[ODataVersion version]
        {
            get
            {
                switch (version)
                {
                    case ODataVersion.V4:
                        return this.v3.Value;

                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataVersionCache_UnknownVersion));
                }
            }
        }
    }
}
