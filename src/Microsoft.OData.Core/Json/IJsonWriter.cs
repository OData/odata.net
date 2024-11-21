//---------------------------------------------------------------------
// <copyright file="IJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System;
    using Microsoft.OData.Edm;
    using System.Threading.Tasks;
    using System.IO;
    using System.Text.Json;

    /// <summary>
    /// Interface for a class that can write arbitrary JSON.
    /// </summary>
    [CLSCompliant(false)]
    public interface IJsonWriter
    {
        /// <summary>
        /// Start the padding function scope.
        /// </summary>
        [Obsolete("This will be dropped in the 9.x release.")]
        void StartPaddingFunctionScope();

        /// <summary>
        /// End the padding function scope.
        /// </summary>
        [Obsolete("This will be dropped in the 9.x release.")]
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
        /// Write the name for the object property.
        /// </summary>
        /// <param name="name">Name of the object property.</param>
        void WriteName(string name);

        /// <summary>
        /// Writes a function name for JSON padding.
        /// </summary>
        /// <param name="functionName">Name of the padding function to write.</param>
        [Obsolete("This will be dropped in the 9.x release.")]
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
        /// Write a Date value
        /// </summary>
        /// <param name="value">Date value to be written.</param>
        void WriteValue(Date value);

        /// <summary>
        /// Write a TimeOfDay value
        /// </summary>
        /// <param name="value">TimeOfDay value to be written.</param>
        void WriteValue(TimeOfDay value);

        /// <summary>
        /// Write a <see cref="JsonElement"/> value.
        /// </summary>
        /// <param name="value">The <see cref="JsonElement"/> value to be written.</param>
        void WriteValue(JsonElement value);

        /// <summary>
        /// Write a raw value.
        /// </summary>
        /// <param name="rawValue">Raw value to be written.</param>
        void WriteRawValue(string rawValue);

        /// <summary>
        /// Clears all buffers for the current writer.
        /// </summary>
        void Flush();

        /// <summary>
        /// Start the stream property valuescope.
        /// </summary>
        /// <returns>
        /// A Stream to write to
        /// </returns>
        Stream StartStreamValueScope();

        /// <summary>
        /// Start the TextWriter value valuescope.
        /// </summary>
        /// <param name="contentType">ContentType of the string being written.</param>
        /// <returns>
        /// A Text writer to write to the stream.
        /// </returns>
        TextWriter StartTextWriterValueScope(string contentType);

        /// <summary>
        /// End the current stream property value scope.
        /// </summary>
        void EndStreamValueScope();

        /// <summary>
        /// End the current TextWriter value valuescope.
        /// </summary>
        void EndTextWriterValueScope();

        /// <summary>
        /// Asynchronously starts the padding function scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Obsolete("This will be dropped in the 9.x release.")]
        Task StartPaddingFunctionScopeAsync();

        /// <summary>
        /// Asynchronously ends the padding function scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Obsolete("This will be dropped in the 9.x release.")]
        Task EndPaddingFunctionScopeAsync();

        /// <summary>
        /// Asynchronously starts the object scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartObjectScopeAsync();

        /// <summary>
        /// Asynchronously ends the current object scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task EndObjectScopeAsync();

        /// <summary>
        /// Asynchronously starts the array scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartArrayScopeAsync();

        /// <summary>
        /// Asynchronously ends the current array scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task EndArrayScopeAsync();

        /// <summary>
        /// Asynchronously writes the name for the object property.
        /// </summary>
        /// <param name="name">Name of the object property.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteNameAsync(string name);

        /// <summary>
        /// Asynchronously writes a function name for JSON padding.
        /// </summary>
        /// <param name="functionName">Name of the padding function to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        [Obsolete("This will be dropped in the 9.x release.")]
        Task WritePaddingFunctionNameAsync(string functionName);

        /// <summary>
        /// Asynchronously writes a boolean value.
        /// </summary>
        /// <param name="value">Boolean value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(bool value);

        /// <summary>
        /// Asynchronously writes an integer value.
        /// </summary>
        /// <param name="value">Integer value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(int value);

        /// <summary>
        /// Asynchronously writes a float value.
        /// </summary>
        /// <param name="value">Float value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(float value);

        /// <summary>
        /// Asynchronously writes a short value.
        /// </summary>
        /// <param name="value">Short value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(short value);

        /// <summary>
        /// Asynchronously writes a long value.
        /// </summary>
        /// <param name="value">Long value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(long value);

        /// <summary>
        /// Asynchronously writes a double value.
        /// </summary>
        /// <param name="value">Double value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(double value);

        /// <summary>
        /// Asynchronously writes a Guid value.
        /// </summary>
        /// <param name="value">Guid value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(Guid value);

        /// <summary>
        /// Asynchronously writes a decimal value
        /// </summary>
        /// <param name="value">Decimal value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(decimal value);

        /// <summary>
        /// Asynchronously writes a DateTimeOffset value
        /// </summary>
        /// <param name="value">DateTimeOffset value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(DateTimeOffset value);

        /// <summary>
        /// Asynchronously writes a TimeSpan value
        /// </summary>
        /// <param name="value">TimeSpan value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(TimeSpan value);

        /// <summary>
        /// Asynchronously writes a byte value.
        /// </summary>
        /// <param name="value">Byte value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(byte value);

        /// <summary>
        /// Asynchronously writes an sbyte value.
        /// </summary>
        /// <param name="value">SByte value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(sbyte value);

        /// <summary>
        /// Asynchronously writes a string value.
        /// </summary>
        /// <param name="value">String value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(string value);

        /// <summary>
        /// Asynchronously writes a byte array.
        /// </summary>
        /// <param name="value">Byte array to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(byte[] value);

        /// <summary>
        /// Asynchronously writes a Date value
        /// </summary>
        /// <param name="value">Date value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(Date value);

        /// <summary>
        /// Asynchronously writes a TimeOfDay value
        /// </summary>
        /// <param name="value">TimeOfDay value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(TimeOfDay value);

        /// <summary>
        /// Asynchronously writes a <see cref="JsonElement"/> value.
        /// </summary>
        /// <param name="value">The <see cref="JsonElement"/> value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteValueAsync(JsonElement value);

        /// <summary>
        /// Asynchronously writes a raw value.
        /// </summary>
        /// <param name="rawValue">Raw value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteRawValueAsync(string rawValue);

        /// <summary>
        /// Asynchronously clears all buffers for the current writer.
        /// </summary>
        /// <returns>A task that represents the asynchronous flush operation.</returns>
        Task FlushAsync();

        /// <summary>
        /// Asynchronously starts the stream property value scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. 
        /// The value of the TResult parameter contains the stream to write the property value to.</returns>
        Task<Stream> StartStreamValueScopeAsync();

        /// <summary>
        /// Asynchronously starts the TextWriter value scope.
        /// </summary>
        /// <param name="contentType">ContentType of the string being written.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The value of the TResult parameter contains the textwriter to write the text property value to.</returns>
        Task<TextWriter> StartTextWriterValueScopeAsync(string contentType);

        /// <summary>
        /// Asynchronously ends the current stream property value scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task EndStreamValueScopeAsync();

        /// <summary>
        /// Asynchronously ends the TextWriter value scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task EndTextWriterValueScopeAsync();
    }
}
