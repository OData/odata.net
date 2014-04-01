//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Json
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
        /// Writes a DateTimeOffset value
        /// </summary>
        /// <param name="value">DateTimeOffset value to be written.</param>
        void WriteValue(DateTimeOffset value);

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
        /// Write a byte array.
        /// </summary>
        /// <param name="value">Byte array to be written.</param>
        void WriteValue(byte[] value);

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
