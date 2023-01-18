//---------------------------------------------------------------------
// <copyright file="IPayloadWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    /// <summary>
    /// Represents a serializer that can
    /// write a JSON response to a stream
    /// given some input data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPayloadWriter<T>
    {
        /// <summary>
        /// Writes the payload to the specified stream.
        /// </summary>
        /// <param name="payload">The data to write.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="includeRawValues">Whether to include raw values in the payload. Helps evaluate the performance of IJsonWriter.WriteRawValue()</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WritePayloadAsync(T payload, Stream stream, bool includeRawValues = false);
    }
}
