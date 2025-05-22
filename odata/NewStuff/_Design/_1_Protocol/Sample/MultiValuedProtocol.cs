namespace NewStuff._Design._1_Protocol.Sample
{
    public sealed class MultiValuedProtocol : IMultiValuedProtocol
    {
        public IGetMultiValuedProtocol Get()
        {
            throw new System.NotImplementedException();
        }

        public IGetSingleValuedProtocol Get(KeyPredicate key)
        {
            throw new System.NotImplementedException();
        }

        public IPatchSingleValuedProtocol Patch(KeyPredicate key, SingleValuedRequest request)
        {
            throw new System.NotImplementedException();
        }

        public IPostSingleValuedProtocol Post(SingleValuedRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
