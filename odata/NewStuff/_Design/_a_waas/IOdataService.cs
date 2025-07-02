namespace NewStuff._Design._a_waas
{
    using System.IO;

    public interface IEdmModel
    {
    }

    public interface IDataStoreMapping
    {
    }

    public interface IOdataService
    {
        static abstract IOdataService Create(IEdmModel edmModel, IDataStoreMapping dataStoreMapping);

        Stream Send(Stream data);
    }


}
