namespace NewStuff._Design._1_Protocol
{
    public interface IGetSingleValuedProtocol
    {
        SingleValuedResponse Evaluate();

        IGetSingleValuedProtocol Expand(object expander); //// TODO what should the parameter actually look like?

        IGetSingleValuedProtocol Select(object selector); //// TODO what should the parameter actually look like?
    }
}
