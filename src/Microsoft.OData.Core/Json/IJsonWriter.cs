//---------------------------------------------------------------------
// <copyright file="IJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Json
{
    using System;
    using Microsoft.OData.Edm.Library;

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
        /// Write a raw value.
        /// </summary>
        /// <param name="rawValue">Raw value to be written.</param>
        void WriteRawValue(string rawValue);

        /// <summary>
        /// Clears all buffers for the current writer.
        /// </summary>
        void Flush();
    }
}