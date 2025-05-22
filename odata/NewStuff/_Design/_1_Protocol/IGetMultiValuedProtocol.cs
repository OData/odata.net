namespace NewStuff._Design._1_Protocol
{
    public interface IGetMultiValuedProtocol
    {
        MultiValuedResponse Evaluate();

        IGetMultiValuedProtocol Expand(object expander); //// TODO what should the parameter actually look like?

        IGetMultiValuedProtocol Filter(object predicate); //// TODO what should the parameter actually look like?

        IGetMultiValuedProtocol Select(object selector); //// TODO what should the parameter actually look like?

        IGetMultiValuedProtocol Skip(object count); //// TODO what should the parameter actually look like?
    }
}
