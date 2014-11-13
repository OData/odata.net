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

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// OData Verbose JSON serializer for collections.
    /// </summary>
    internal sealed class ODataVerboseJsonCollectionSerializer : ODataVerboseJsonPropertyAndValueSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonOutputContext">The output context to write to.</param>
        internal ODataVerboseJsonCollectionSerializer(ODataVerboseJsonOutputContext verboseJsonOutputContext)
            : base(verboseJsonOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Writes the start of a collection.
        /// </summary>
        internal void WriteCollectionStart()
        {
            DebugUtils.CheckNoExternalCallers();

            // at the top level, we need to write the "results" wrapper for V2 and higher and for responses only
            if (this.WritingResponse && this.Version >= ODataVersion.V2)
            {
                // { "results":
                this.JsonWriter.StartObjectScope();
                this.JsonWriter.WriteDataArrayName();
            }

            // Write the start of the array for the collection items
            // "["
            this.JsonWriter.StartArrayScope();
        }

        /// <summary>
        /// Writes the end of a collection.
        /// </summary>
        internal void WriteCollectionEnd()
        {
            DebugUtils.CheckNoExternalCallers();

            // Write the end of the array for the collection items
            // "]"
            this.JsonWriter.EndArrayScope();

            // at the top level, we need to close the "results" wrapper for V2 and higher and for responses only
            if (this.WritingResponse && this.Version >= ODataVersion.V2)
            {
                // "}"
                this.JsonWriter.EndObjectScope();
            }
        }
    }
}
