using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Core;

// In the previous version, I separated TState and TContext where TState
// represents the state a "this time", expected to be backed by a stack
// that is pushed to for each scope of the writer, and the context
// represents data that is the same throughout the serialization (e.g. IEdmModel, underlying writer or stream, etc.)
// In this version, I only have a single TState for simplicity, which is expected to eapsulate both the state and context.
// I'm not sure which version is better, I think fewer parameters is simpler to understand. But I'll
// weigh both options as I evolve the design further.
/// <summary>
/// Represents a type that can write an OData value from an input of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The input type to write.</typeparam>
/// <typeparam name="TState">
/// Encapsulates the current state of the serialization.
/// The state should provide access to the writer or output and other contextual information.
/// </typeparam>
public interface IODataWriter<T, TState> : IODataWriter
{
    // TODO might consider making the return value a struct if we need to encaspulate more info in the return result.
    // Temporarily reverted to return ValueTask instead of bool to simplify the design of the initial API surface
    // abstractions, then I'll go back to returning bool or other re-entrancy mechanism once I have
    // other things figured out.
    /// <summary>
    /// Writes the specified value to the output.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="state">
    /// Encapsulates the current state of the serialization.
    /// The state should provide access to the writer or output and other contextual information.
    /// </param>
    /// <returns>
    /// Value that indicates whether the the value was written completely. If false is returned,
    /// the serialize might call this writer again with the same value to complete the write.
    /// Writers that return false should stash sufficient state information to be able to resume the
    /// serialization of the same value from where the last write left off.
    /// This is useful for asynchronous writers that might need to perform I/O operations intermittently
    /// between in-memory buffered writes (e.g. flushing to I/O stream, fetching more data from async source, etc.).
    /// This allows us to support async scenarios without having to make methods async and do async/await in the fast path of the serializer
    /// that is expected to do in-memory writes for the most part.
    /// </returns>
    bool Write(T value, TState state);
}
