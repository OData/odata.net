namespace Microsoft.OData.Client
{
    internal interface IDataServiceRequestMessageFactory
    {
        DataServiceClientRequestMessage CreateRequestMessage(DataServiceClientRequestMessageArgs args, DataServiceContext dataServiceContext);
    }
}