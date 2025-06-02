namespace NewStuff._Design._1_Protocol
{
    using System.Threading.Tasks;

    public interface IGetMultiValuedProtocol
    {
        Task<MultiValuedResponse> Evaluate();

        IGetMultiValuedProtocol Expand(object expander); //// TODO what should the parameter actually look like? //// TODO from meeting, this should probably just be the parse node of an expand expression, right?

        IGetMultiValuedProtocol Filter(object predicate); //// TODO what should the parameter actually look like?

        IGetMultiValuedProtocol Select(object selector); //// TODO what should the parameter actually look like?

        IGetMultiValuedProtocol Skip(object count); //// TODO what should the parameter actually look like?
    }
}
