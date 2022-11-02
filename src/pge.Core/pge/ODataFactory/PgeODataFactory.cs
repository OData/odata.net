namespace pge.ODataFactory
{
    using pge.OData;

    public sealed class PgeODataFactory : IODataFactory
    {
        public IOData Create(ODataModel model)
        {
            return new PgeOData();
        }
    }
}
