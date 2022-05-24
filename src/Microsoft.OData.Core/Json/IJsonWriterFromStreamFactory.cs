using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// The interface of factories that create <see cref="IJsonWriter"/> instances
    /// that write directly to a <see cref="Stream"/> rather than a <see cref="TextWriter"/>.
    /// </summary>
    [CLSCompliant(false)]
    public interface IStreamBasedJsonWriterFactory
    {
        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/>. Returns null
        /// if it cannot create a writer with the specified constraints.
        /// </summary>
        /// <param name="stream">Output stream to which data should be written.</param>
        /// <param name="isIeee754Compatible">True if it is IEEE754Compatible.</param>
        /// <param name="encoding">The text encoding of the output data.</param>
        /// <returns>The JSON writer created.</returns>
        IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding);
    }
}
