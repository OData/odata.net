namespace NewStuff._Design._0_Convention
{
    using System.Threading.Tasks;

    public interface IConvention
    {
        Task<IGetRequestWriter> Get();

        IPatchRequestWriter Patch();

        IPatchRequestWriter Post(); //// TODO TODO TODO IMPORTANT you've re-used IPatchRequestWriter for convenience here; it should really be ipostrequestwriter
    }
}
