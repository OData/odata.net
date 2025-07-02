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





    public struct WaasNothing
    {
    }

    public sealed class EdmModel
    {
        private EdmModel()
        {
        }
    }

    public interface IEdmModelReader
    {
        EdmModel Value { get; } //// TODO i think this should return an interface; the caller of  the edm model is likely to be using it as a set of operations (like finding a type or something) than a data type (like traversing the edm graph)

        WaasNothing Read();
    }

    public sealed class OdataRequest
    {
        private OdataRequest()
        {
        }
    }

    public interface IOdataRequestReader
    {
        OdataRequest Value { get; }

        IOdataResponseWriter Read();
    }

    public sealed class OdataResponse
    {
        private OdataResponse()
        {
        }
    }

    public interface IOdataResponseWriter
    {
        WaasNothing Write(OdataResponse value);
    }

    public interface IConventionOdataService
    {
        static abstract IConventionOdataService Create(IEdmModel edmModel, IDataStoreMapping dataStoreMapping);

        IOdataResponse Send(IOdataRequest request);
    }






    public interface IEdmModelParser //// TODO this should be a reader
    {
        IEdmModel Parse(Stream csdl);
    }

    public sealed class FusionOdataServiceSettings
    {
        private FusionOdataServiceSettings(IEdmModelParser edmModelParser)
        {
            EdmModelParser = edmModelParser;
        }

        public IEdmModelParser EdmModelParser { get; }
    }

    public sealed class FusionOdataService : IOdataService
    {
        private readonly Stream edmModel;
        private readonly IDataStoreMapping dataStoreMapping;
        private readonly IEdmModelParser edmModelParser;

        public static IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping)
        {
            throw new System.NotImplementedException();
        }

        public static IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping, FusionOdataServiceSettings settings)
        {
            throw new System.NotImplementedException();
        }

        private FusionOdataService(Stream edmModel, IDataStoreMapping dataStoreMapping, FusionOdataServiceSettings settings)
        {
            this.edmModel = edmModel;
            this.dataStoreMapping = dataStoreMapping;
            this.edmModelParser = settings.EdmModelParser;
        }

        public Stream Send(Stream request)
        {
            var parsedEdmModel = this.edmModelParser.Parse(this.edmModel);
            var fusionConventionOdataService = FusionConventionOdataService.Create(parsedEdmModel, this.dataStoreMapping);

        }
    }
















    public sealed class FusionConventionOdataService : IConventionOdataService
    {
        public static IConventionOdataService Create(IEdmModel edmModel, IDataStoreMapping dataStoreMapping)
        {
            throw new System.NotImplementedException();
        }

        public IOdataResponse Send(IOdataRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
