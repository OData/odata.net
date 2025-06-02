namespace NewStuff._Design._1_Protocol
{
    using System.Threading.Tasks;

    public interface IPostSingleValuedProtocol
    {
        Task<SingleValuedResponse> Evaluate();

        IPostSingleValuedProtocol Expand(object expander); //// TODO what should the parameter actually look like?

        IPostSingleValuedProtocol Select(object selector); //// TODO what should the parameter actually look like?
    }
}
