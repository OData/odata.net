//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Reader for the EPM custom-only. Read the EPM properties from cached values.
    /// </summary>
    internal sealed class EpmCustomReader : EpmReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry to which the EPM is applied.</param>
        /// <param name="version">The version of OData protocol to use.</param>
        /// <param name="messageReaderSettings">The reader settings to use.</param>
        private EpmCustomReader(
            IODataAtomReaderEntryState entryState,
            ODataVersion version,
            ODataMessageReaderSettings messageReaderSettings)
            : base(entryState, version, messageReaderSettings)
        {
        }

        /// <summary>
        /// Reads the custom EPM for an entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry to which the EPM is applied.</param>
        /// <param name="version">The OData protocol version to use.</param>
        /// <param name="messageReaderSettings">The reader settings to use.</param>
        internal static void ReadEntryEpm(
            IODataAtomReaderEntryState entryState,
            ODataVersion version,
            ODataMessageReaderSettings messageReaderSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(entryState.CachedEpm != null, "To read entry EPM, the entity type must have an EPM annotation.");
            Debug.Assert(entryState.EpmCustomReaderValueCache != null, "To read custom entry EPM, the entry must have the custom EPM reader value cache.");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            EpmCustomReader epmCustomReader = new EpmCustomReader(entryState, version, messageReaderSettings);
            epmCustomReader.ReadEntryEpm();
        }

        /// <summary>
        /// Reads an EPM for the entire entry.
        /// </summary>
        private void ReadEntryEpm()
        {
            EpmCustomReaderValueCache epmValueCache = this.EntryState.EpmCustomReaderValueCache;

            // Walk cached values and apply them to the entry.
            foreach (KeyValuePair<EntityPropertyMappingInfo, string> customEpmValue in epmValueCache.CustomEpmValues)
            {
                this.SetEntryEpmValue(customEpmValue.Key, customEpmValue.Value);
            }
        }
    }
}
