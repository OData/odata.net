namespace NewStuff._Design._1_Protocol
{
    public interface IMultiValuedProtocol
    {
        IGetMultiValuedProtocol Get();

        IGetSingleValuedProtocol Get(KeyPredicate key);

        IPatchSingleValuedProtocol Patch(KeyPredicate key, SingleValuedRequest request);
    }
}
