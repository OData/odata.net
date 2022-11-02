namespace pge.ODataFactory
{
    using pge.OData;

    public interface IODataFactory
    {
        IOData Create(ODataModel model);
    }
}
