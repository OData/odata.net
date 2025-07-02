namespace NewStuff._Design._a_waas
{
    using System;
    using System.IO;




    
    //// TODO you need something that takes the onboarding csdl and turns it into a standalone csdl




    public interface IDataStoreMapping
    {
    }

    public interface IOdataService
    {
        static abstract IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping);

        void Send(Stream request, Stream response);
    }














    public abstract class AspNetCoreControllerBase
    {
        public AspNetCoreHttpRequest Request { get; }

        public AspNetCoreHttpResponse Response { get; }
    }

    public abstract class AspNetCoreHttpRequest
    {
        public abstract Stream ToStream();
    }

    public abstract class AspNetCoreHttpResponse
    {
        public abstract Stream ToStream();
    }

    public sealed class ServiceController : AspNetCoreControllerBase
    {
        private readonly IOdataService odataService;

        public ServiceController(IOdataService odataService)
        {
            this.odataService = odataService;
        }

        public void Get()
        {
            this.odataService.Send(this.Request.ToStream(), this.Response.ToStream());
        }
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
        static abstract IConventionOdataService Create(IEdmModelReader edmModelReader, IDataStoreMapping dataStoreMapping);

        void Send(IOdataRequestReader odataRequestReader);
    }









    














    public sealed class FusionOdataServiceSettings
    {
        private FusionOdataServiceSettings(
            Func<Stream, IEdmModelReader> edmModelReaderFactory,
            Func<Stream, Stream, IOdataRequestReader> odataRequestReaderFactory)
        {
            EdmModelReaderFactory = edmModelReaderFactory;
            OdataRequestReaderFactory = odataRequestReaderFactory;
        }

        public static FusionOdataServiceSettings Default { get; } = new FusionOdataServiceSettings(
            csdl => new EdmModelReader(csdl),
            (request, response) => new OdataRequestReader(request, response));

        public Func<Stream, IEdmModelReader> EdmModelReaderFactory { get; }
        public Func<Stream, Stream, IOdataRequestReader> OdataRequestReaderFactory { get; }

        public sealed class Builder
        {
            public Func<Stream, IEdmModelReader> EdmModelReaderFactory { get; set; } = FusionOdataServiceSettings.Default.EdmModelReaderFactory;
            public Func<Stream, Stream, IOdataRequestReader> OdataRequestReaderFactory { get; set; } = FusionOdataServiceSettings.Default.OdataRequestReaderFactory;

            public FusionOdataServiceSettings Build()
            {
                return new FusionOdataServiceSettings(this.EdmModelReaderFactory, this.OdataRequestReaderFactory);
            }
        }
    }

    public sealed class FusionOdataService : IOdataService
    {
        private readonly Stream edmModel;
        private readonly IDataStoreMapping dataStoreMapping;
        private readonly Func<Stream, IEdmModelReader> edmModelReaderFactory;
        private readonly Func<Stream, Stream, IOdataRequestReader> odataRequestReaderFactory;

        public static IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping)
        {
            return FusionOdataService.Create(edmModel, dataStoreMapping, FusionOdataServiceSettings.Default);
        }

        public static IOdataService Create(Stream edmModel, IDataStoreMapping dataStoreMapping, FusionOdataServiceSettings settings)
        {
            return new FusionOdataService(edmModel, dataStoreMapping, settings);
        }

        private FusionOdataService(Stream edmModel, IDataStoreMapping dataStoreMapping, FusionOdataServiceSettings settings)
        {
            this.edmModel = edmModel;
            this.dataStoreMapping = dataStoreMapping;
            this.edmModelReaderFactory = settings.EdmModelReaderFactory;
            this.odataRequestReaderFactory = settings.OdataRequestReaderFactory;
        }

        public void Send(Stream request, Stream response)
        {
            var edmModelReader = this.edmModelReaderFactory(this.edmModel);
            var fusionConventionOdataService = FusionConventionOdataService.Create(edmModelReader, this.dataStoreMapping);

            fusionConventionOdataService.Send(this.odataRequestReaderFactory(request, response));
        }
    }

    public sealed class EdmModelReader : IEdmModelReader
    {
        public EdmModelReader(Stream csdl)
        {
            this.Value = default; //// TODO actually parse
        }
        
        public EdmModel Value { get; }

        public WaasNothing Read()
        {
            return new WaasNothing();
        }
    }

    public sealed class OdataRequestReader : IOdataRequestReader
    {
        private readonly Stream response;

        public OdataRequestReader(Stream request, Stream response)
        {
            this.Value = default; //// TODO parse request
            this.response = response;
        }

        public OdataRequest Value { get; }

        public IOdataResponseWriter Read()
        {
            return new OdataResponseWriter(this.response);
        }

        private sealed class OdataResponseWriter : IOdataResponseWriter
        {
            private readonly Stream response;

            public OdataResponseWriter(Stream response)
            {
                this.response = response;
            }

            public WaasNothing Write(OdataResponse value)
            {
                //// TODO actually write response
                return new WaasNothing();
            }
        }
    }





























    public sealed class FusionConventionOdataService : IConventionOdataService
    {
        public static IConventionOdataService Create(IEdmModelReader edmModel, IDataStoreMapping dataStoreMapping)
        {
            throw new NotImplementedException();
        }

        private FusionConventionOdataService(IEdmModelReader edmModel)
        {
        }

        public void Send(IOdataRequestReader odataRequestReader)
        {
            throw new NotImplementedException();
        }
    }
}
