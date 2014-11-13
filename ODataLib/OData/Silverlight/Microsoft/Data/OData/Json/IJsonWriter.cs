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

namespace Microsoft.Data.OData.Json
{
    using System;

    /// <summary>
    /// Interface for a class that can write arbitrary JSON.
    /// Internally we want the interface for mocks.
    /// </summary>
    internal interface IJsonWriter
    {
        /// <summary>
        /// Start the padding function scope.
        /// </summary>
        void StartPaddingFunctionScope();

        /// <summary>
        /// End the padding function scope.
        /// </summary>
        void EndPaddingFunctionScope();

        /// <summary>
        /// Start the object scope.
        /// </summary>
        void StartObjectScope();

        /// <summary>
        /// End the current object scope.
        /// </summary>
        void EndObjectScope();

        /// <summary>
        /// Start the array scope.
        /// </summary>
        void StartArrayScope();

        /// <summary>
        /// End the current array scope.
        /// </summary>
        void EndArrayScope();

        /// <summary>
        /// Write the "d" wrapper text.
        /// </summary>
        void WriteDataWrapper();

        /// <summary>
        /// Write the "results" header for the data array.
        /// </summary>
        void WriteDataArrayName();

        /// <summary>
        /// Write the name for the object property.
        /// </summary>
        /// <param name="name">Name of the object property.</param>
        void WriteName(string name);

        /// <summary>
        /// Writes a function name for JSON padding.
        /// </summary>
        /// <param name="functionName">Name of the padding function to write.</param>
        void WritePaddingFunctionName(string functionName);

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="value">Boolean value to be written.</param>
        void WriteValue(bool value);

        /// <summary>
        /// Write an integer value.
        /// </summary>
        /// <param name="value">Integer value to be written.</param>
        void WriteValue(int value);

        /// <summary>
        /// Write a float value.
        /// </summary>
        /// <param name="value">Float value to be written.</param>
        void WriteValue(float value);

        /// <summary>
        /// Write a short value.
        /// </summary>
        /// <param name="value">Short value to be written.</param>
        void WriteValue(short value);

        /// <summary>
        /// Write a long value.
        /// </summary>
        /// <param name="value">Long value to be written.</param>
        void WriteValue(long value);

        /// <summary>
        /// Write a double value.
        /// </summary>
        /// <param name="value">Double value to be written.</param>
        void WriteValue(double value);

        /// <summary>
        /// Write a Guid value.
        /// </summary>
        /// <param name="value">Guid value to be written.</param>
        void WriteValue(Guid value);

        /// <summary>
        /// Write a decimal value
        /// </summary>
        /// <param name="value">Decimal value to be written.</param>
        void WriteValue(decimal value);

        /// <summary>
        /// Write a DateTime value
        /// </summary>
        /// <param name="value">DateTime value to be written.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        void WriteValue(DateTime value, ODataVersion odataVersion);

        /// <summary>
        /// Writes a DateTimeOffset value
        /// </summary>
        /// <param name="value">DateTimeOffset value to be written.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        void WriteValue(DateTimeOffset value, ODataVersion odataVersion);

        /// <summary>
        /// Writes a TimeSpan value
        /// </summary>
        /// <param name="value">TimeSpan value to be written.</param>
        void WriteValue(TimeSpan value);

        /// <summary>
        /// Write a byte value.
        /// </summary>
        /// <param name="value">Byte value to be written.</param>
        void WriteValue(byte value);

        /// <summary>
        /// Write an sbyte value.
        /// </summary>
        /// <param name="value">SByte value to be written.</param>
        void WriteValue(sbyte value);

        /// <summary>
        /// Write a string value.
        /// </summary>
        /// <param name="value">String value to be written.</param>
        void WriteValue(string value);

        /// <summary>
        /// Clears all buffers for the current writer.
        /// </summary>
        void Flush();

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        void WriteValueSeparator();

        /// <summary>
        /// Start the scope given the scope type.
        /// </summary>
        /// <param name="type">The scope type to start.</param>
        void StartScope(JsonWriter.ScopeType type);
    }
}
