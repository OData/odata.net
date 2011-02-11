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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    #endregion Namespaces.

    /// <summary>
    /// Class with utility methods for writing OData content.
    /// </summary>
    internal static class WriterUtils
    {
        /// <summary>
        /// Writes the specified binary value to the <paramref name="asyncBufferedStream"/>.
        /// </summary>
        /// <param name="asyncBufferedStream">The stream to write the raw value to.</param>
        /// <param name="bytes">The binary value to write.</param>
        /// <returns>The <see cref="TextWriter"/> instance that was used to write the value or null if a binary value was written directly to the stream.</returns>
        internal static TextWriter WriteBinaryValue(AsyncBufferedStream asyncBufferedStream, byte[] bytes)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(asyncBufferedStream != null, "asyncBufferedStream != null");
            Debug.Assert(bytes != null, "bytes != null");

            // write the bytes directly
            asyncBufferedStream.Write(bytes, 0, bytes.Length);
            return null;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> into its raw format and writes it to the <paramref name="asyncBufferedStream"/>.
        /// The value has to be of primitive type.
        /// </summary>
        /// <param name="asyncBufferedStream">The stream to write the raw value to.</param>
        /// <param name="value">The (non-binary) value to write.</param>
        /// <param name="encoding">The encoding to use when writing raw values (except for binary values).</param>
        /// <returns>The <see cref="TextWriter"/> instance that was used to write the value or null if a binary value was written directly to the stream.</returns>
        /// <remarks>We do not accept binary values here; WriteBinaryValue should be used for binary data.</remarks>
        internal static TextWriter WriteRawValue(AsyncBufferedStream asyncBufferedStream, object value, Encoding encoding)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(asyncBufferedStream != null, "asyncBufferedStream != null");
            Debug.Assert(!(value is byte[]), "!(value is byte[])");

            string valueAsString;
            bool preserveWhitespace;
            if (!AtomValueUtils.TryConvertPrimitiveToString(value, out valueAsString, out preserveWhitespace))
            {
                // throw an exception because the value is not primitive
                throw new ODataException(Strings.ODataUtils_CannotConvertValueToRawPrimitive(value.GetType().FullName));
            }

            StreamWriter textWriter = new StreamWriter(asyncBufferedStream, encoding);
            textWriter.Write(valueAsString);
            return textWriter;
        }
    }
}
