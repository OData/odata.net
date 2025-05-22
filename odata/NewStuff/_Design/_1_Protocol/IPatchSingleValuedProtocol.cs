namespace NewStuff._Design._1_Protocol
{
    public interface IPatchSingleValuedProtocol
    {
        SingleValuedResponse Evaluate();

        IPatchSingleValuedProtocol Expand(object expander); //// TODO what should the parameter actually look like?

        IPatchSingleValuedProtocol Select(object selector); //// TODO what should the parameter actually look like?
    }
}
