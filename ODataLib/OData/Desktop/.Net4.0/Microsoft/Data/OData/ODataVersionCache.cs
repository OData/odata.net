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
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Simple ODataVersion specific cache.
    /// </summary>
    /// <typeparam name="T">The type of the item being cached.</typeparam>
    internal sealed class ODataVersionCache<T>
    {
        /// <summary>
        /// Lazy constructing T for ODataVersion.V1.
        /// </summary>
        private readonly SimpleLazy<T> v1;

        /// <summary>
        /// Lazy constructing T for ODataVersion.V2.
        /// </summary>
        private readonly SimpleLazy<T> v2;

        /// <summary>
        /// Lazy constructing T for ODataVersion.V3.
        /// </summary>
        private readonly SimpleLazy<T> v3;

        /// <summary>
        /// Constructs an instance of the ODataVersionCache.
        /// </summary>
        /// <param name="factory">The method to call to create a new instance of <typeparamref name="T"/> for a given ODataVersion.</param>
        internal ODataVersionCache(Func<ODataVersion, T> factory)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(factory != null, "factory != null");

            this.v1 = new SimpleLazy<T>(() => factory(ODataVersion.V1), true /*isThreadSafe*/);
            this.v2 = new SimpleLazy<T>(() => factory(ODataVersion.V2), true /*isThreadSafe*/);
            this.v3 = new SimpleLazy<T>(() => factory(ODataVersion.V3), true /*isThreadSafe*/);
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
                DebugUtils.CheckNoExternalCallers();

                switch (version)
                {
                    case ODataVersion.V1:
                        return this.v1.Value;

                    case ODataVersion.V2:
                        return this.v2.Value;

                    case ODataVersion.V3:
                        return this.v3.Value;

                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataVersionCache_UnknownVersion));
                }
            }
        }
    }
}
