namespace NewStuff._Design._a_waas
{
    using System;
    using System.Collections.Generic;
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


























    public interface IEdmModel2
    {
        IEdmEntityType GetTypeOfRootSegment(string segment);
    }

    public interface IEdmEntityType
    {
        string TypeName { get; }

        int NumberOfKeyParts { get; }

        IEdmEntityType GetTypeOfNavigationProperty(string propertyName);

        IEdmComplexType GetTypeOfComplexProperty(string proeprtyName);

        IEdmPrimitiveType GetTypeOfPrimitiveProperty(string propertyName);
    }

    public interface IEdmComplexType
    {
        string TypeName { get; }

        IEdmEntityType GetTypeOfNavigationProperty(string propertyName);

        IEdmComplexType GetTypeOfComplexProperty(string proeprtyName);

        IEdmPrimitiveType GetTypeOfPrimitiveProperty(string propertyName);
    }

    public interface IEdmPrimitiveType
    {
        string TypeName { get; }
    }

    public interface IOdataRequestReader2
    {
        IOdataUriReader Read();
    }

    public interface IOdataUriReader
    {
        IPrecedingSegmentsReader Read();
    }

    public sealed class PrecedingSegments
    {
        private PrecedingSegments()
        {
        }
    }

    public interface IPrecedingSegmentsReader
    {
        PrecedingSegments Value { get; }

        IOdataUriSegmentReader Read();
    }

    public sealed class OdataUriSegment
    {
        private OdataUriSegment()
        {
        }

        internal string Value { get; }
    }

    public interface IOdataUriSegmentReader
    {
        OdataUriSegment Value { get; }

        OdataUriSegmentReaderToken Read();
    }

    public abstract class OdataUriSegmentReaderToken
    {
        private OdataUriSegmentReaderToken()
        {
        }

        public TResult Apply<TResult, TContext>(
            Func<Segment, TContext, TResult> segmentMap,
            Func<QueryOptions, TContext, TResult> queryOptionsMap,
            Func<Fragment, TContext, TResult> fragmentMap,
            TContext context)
        {
            if (this is Segment segment)
            {
                return segmentMap(segment, context);
            }
            else if (this is QueryOptions queryOptions)
            {
                return queryOptionsMap(queryOptions, context);
            }
            else if (this is Fragment fragment)
            {
                return fragmentMap(fragment, context);
            }
            else
            {
                throw new Exception("TODO implement a visitor");
            }
        }

        public sealed class Segment : OdataUriSegmentReaderToken
        {
            public Segment(IOdataUriSegmentReader odataUriSegmentReader)
            {
                OdataUriSegmentReader = odataUriSegmentReader;
            }

            public IOdataUriSegmentReader OdataUriSegmentReader { get; }
        }

        public sealed class QueryOptions : OdataUriSegmentReaderToken
        {
            private QueryOptions()
            {
            }
        }

        public sealed class Fragment : OdataUriSegmentReaderToken
        {
            private Fragment()
            {
            }
        }

        public sealed class End : OdataUriSegmentReaderToken
        {
            private End()
            {
            }

            public static End Instance { get; } = new End();
        }
    }

    public sealed class WaasFusionDataStoreMapping : IDataStoreMapping
    {
        // the idea is that the key of the entity is the base64 encoded and semicolon delimited series of keys of the containing entities (plus the entity itself at the end)
        // the above assumes that the entities are contained in only one navigation property, which isn't necessarily true (a single entity *instance* is only contained in one property, but that entity type can be used as the type for multiple contained properties through the model)

        public IOdataConventionClient Dereference(FusionId fusionId)
        {

        }
    }

    public sealed class FusionId
    {
        public FusionId(IEnumerable<FusionIdPart> idParts)
        {
            this.IdParts = idParts;
        }

        public IEnumerable<FusionIdPart> IdParts { get; }
    }

    public abstract class FusionIdPart
    {
        private FusionIdPart()
        {
        }

        public sealed class Keyed : FusionIdPart
        {
            public string TypeName { get; }

            public string Id { get; }
        }

        public sealed class Keyless : FusionIdPart
        {
            public string TypeName { get; }
        }
    }

    public sealed class FusionConventionOdataService : IConventionOdataService
    {
        private readonly WaasFusionDataStoreMapping dataStoreMapping;

        private readonly IEdmModel2 edmModel;

        public static IConventionOdataService Create(IEdmModelReader edmModel, IDataStoreMapping dataStoreMapping)
        {
            throw new NotImplementedException();
        }

        private FusionConventionOdataService(IEdmModelReader edmModel)
        {
        }

        public void Send(IOdataRequestReader odataRequestReader)
        {
            var adaptedReader = Adapt(odataRequestReader);
            var odataUriReader = adaptedReader.Read();
            var _ = odataUriReader.Read();
            var odataUriSegmentReader = _.Read();
            var odataUriSegment = odataUriSegmentReader.Value;

        }

        private static IOdataRequestReader2 Adapt(IOdataRequestReader odataRequestReader)
        {
            throw new Exception("TODO");
        }
    }







    public interface IOdataResponseReader
    {
    }

    public interface IOdataRequestWriter
    {
        IOdataResponseReader Write(OdataRequest odataRequest);
    }

    public interface IOdataConventionClient
    {
        void Send(IOdataRequestWriter odataRequestWriter);
    }
}
