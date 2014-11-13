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
        /// <param name="inputContext">The input context currently in use.</param>
        private EpmCustomReader(
            IODataAtomReaderEntryState entryState,
            ODataAtomInputContext inputContext)
            : base(entryState, inputContext)
        {
        }

        /// <summary>
        /// Reads the custom EPM for an entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry to which the EPM is applied.</param>
        /// <param name="inputContext">The input context currently in use.</param>
        internal static void ReadEntryEpm(
            IODataAtomReaderEntryState entryState,
            ODataAtomInputContext inputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(entryState.CachedEpm != null, "To read entry EPM, the entity type must have an EPM annotation.");
            Debug.Assert(entryState.EpmCustomReaderValueCache != null, "To read custom entry EPM, the entry must have the custom EPM reader value cache.");
            Debug.Assert(inputContext != null, "inputContext != null");

            EpmCustomReader epmCustomReader = new EpmCustomReader(entryState, inputContext);
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
