namespace NewStuff._Design._a_waas
{
    using System.IO;

    public interface IDataStoreMapping
    {
    }

    public interface IOdataService
    {
        static abstract IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping);

        Stream Send(Stream request);
    }




    public interface IEdmModel //// TODO should this be an interface or a concrete type?
    {
    }

    public interface IOdataRequest //// TODO should this be an interface or a concrete type?
    {
    }

    public interface IOdataResponse //// TODO should this be an interface or a concrete type?
    {
    }

    public interface IConventionOdataService
    {
        static abstract IConventionOdataService Create(IEdmModel edmModel, IDataStoreMapping dataStoreMapping);

        IOdataResponse Send(IOdataRequest request);
    }
}
