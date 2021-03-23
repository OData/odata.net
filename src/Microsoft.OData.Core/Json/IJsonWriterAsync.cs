//---------------------------------------------------------------------
// <copyright file="IJsonWriterAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Interface for a class that can write arbitrary JSON asynchronously.
    /// </summary>
    [CLSCompliant(false)]
    public interface IJsonWriterAsync
    {
        /// <summary>
        /// Asynchronously starts the padding function scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartPaddingFunctionScopeAsync();

        /// <summary>
        /// Asynchronously ends the padding function scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
    }
}
