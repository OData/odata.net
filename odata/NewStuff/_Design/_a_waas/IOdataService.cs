namespace NewStuff._Design._a_waas
{
    using System;
    using System.IO;

    public interface IDataStoreMapping
    {
    }

    public interface IOdataService
    {
        static abstract IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping);

        void Send(Stream request, Stream response);
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
        static abstract IConventionOdataService Create(IEdmModelReader edmModel, IDataStoreMapping dataStoreMapping);

        void Send(IOdataRequestReader odataRequestReader);
    }







    public sealed class FusionOdataServiceSettings
    {
        private FusionOdataServiceSettings(Func<Stream, IEdmModelReader> edmModelReaderFactory)
        {
            EdmModelReaderFactory = edmModelReaderFactory;
        }

        public Func<Stream, IEdmModelReader> EdmModelReaderFactory { get; }
    }

    public sealed class FusionOdataService : IOdataService
    {
        private readonly Stream edmModel;
        private readonly IDataStoreMapping dataStoreMapping;
        private readonly Func<Stream, IEdmModelReader> edmModelReaderFactory;

        public static IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping)
        {
        }

        public static IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping, FusionOdataServiceSettings settings)
        {
        }

        private FusionOdataService(Stream edmModel, IDataStoreMapping dataStoreMapping, FusionOdataServiceSettings settings)
        {
            this.edmModel = edmModel;
            this.dataStoreMapping = dataStoreMapping;
            this.edmModelReaderFactory = settings.EdmModelReaderFactory;
        }

        public void Send(Stream request, Stream response)
        {
            var edmModelReader = this.edmModelReaderFactory(this.edmModel);
            var fusionConventionOdataService = FusionConventionOdataService.Create(parsedEdmModel, this.dataStoreMapping);

        }
    }
















    public sealed class FusionConventionOdataService : IConventionOdataService
    {
        public static IConventionOdataService Create(IEdmModelReader edmModel, IDataStoreMapping dataStoreMapping)
        {
            throw new NotImplementedException();
        }

        public void Send(IOdataRequestReader odataRequestReader)
        {
            throw new NotImplementedException();
        }
    }
}
