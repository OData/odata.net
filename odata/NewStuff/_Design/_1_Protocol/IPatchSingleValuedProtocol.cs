namespace NewStuff._Design._1_Protocol
{
    using System.Threading.Tasks;

    public interface IPatchSingleValuedProtocol
    {
        Task<SingleValuedResponse> Evaluate();

        IPatchSingleValuedProtocol Expand(object expander); //// TODO what should the parameter actually look like?

        IPatchSingleValuedProtocol Select(object selector); //// TODO what should the parameter actually look like?
    }
}
