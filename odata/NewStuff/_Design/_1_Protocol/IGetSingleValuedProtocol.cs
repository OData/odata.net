namespace NewStuff._Design._1_Protocol
{
    using System.Threading.Tasks;

    public interface IGetSingleValuedProtocol
    {
        Task<SingleValuedResponse> Evaluate();

        IGetSingleValuedProtocol Expand(object expander); //// TODO what should the parameter actually look like?

        IGetSingleValuedProtocol Select(object selector); //// TODO what should the parameter actually look like?
    }
}
