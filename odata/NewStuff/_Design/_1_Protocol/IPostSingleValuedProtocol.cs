namespace NewStuff._Design._1_Protocol
{
    public interface IPostSingleValuedProtocol
    {
        SingleValuedResponse Evaluate();

        IPostSingleValuedProtocol Expand(object expander); //// TODO what should the parameter actually look like?

        IPostSingleValuedProtocol Select(object selector); //// TODO what should the parameter actually look like?
    }
}
