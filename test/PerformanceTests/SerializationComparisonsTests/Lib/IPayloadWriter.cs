using System.Collections.Generic;
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
        Task WritePayload(T payload, Stream stream);
    }
}
