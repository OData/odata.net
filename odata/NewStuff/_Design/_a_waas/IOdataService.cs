namespace NewStuff._Design._a_waas
{
    using System;
    using System.Buffers.Text;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;





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

        bool IsMultiValued { get; }

        bool IsContained { get; } //// TODO this property really indicates that you've modeled the EDM stuff all wrong

        int NumberOfKeyParts { get; }

        bool TryGetTypeOfNavigationProperty(string propertyName, out IEdmEntityType edmEntityType);

        bool TryGetTypeOfComplexProperty(string proeprtyName, out IEdmComplexType edmComplexType);

        bool TryGetTypeOfPrimitiveProperty(string propertyName, out IEdmPrimitiveType edmPrimitiveType);
    }

    public interface IEdmComplexType
    {
        string TypeName { get; }

        bool IsMultiValued { get; }

        bool TryGetTypeOfNavigationProperty(string propertyName, out IEdmEntityType edmEntityType);

        bool TryGetTypeOfComplexProperty(string proeprtyName, out IEdmComplexType edmComplexType);

        bool TryGetTypeOfPrimitiveProperty(string propertyName, out IEdmPrimitiveType edmPrimitiveType);
    }

    public interface IEdmPrimitiveType
    {
        string TypeName { get; }

        bool IsMultiValued { get; }
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
        internal PrecedingSegments(string value)
        {
            Value = value;
        }

        internal string Value { get; }
    }

    public interface IPrecedingSegmentsReader
    {
        PrecedingSegments Value { get; }

        IOdataUriSegmentReader Read();
    }

    public sealed class OdataUriSegment
    {
        internal OdataUriSegment(string value)
        {
            Value = value;
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
        /*
        ### an entity type that is contained by a property of another contained entity type

        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="4.0" xmlns:ags="http://aggregator.microsoft.com/internal" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" xmlns:odata="http://schemas.microsoft.com/oDataCapabilities">
          <edmx:DataServices>
            <Schema Namespace="root" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
              <EntityType Name="foo">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />

                <NavigationProperty Name="bars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
              </EntityType>

              <EntityType Name="bar">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />

                <NavigationProperty Name="fizzes" Type="Collection(root.fizz)" Nullable="false" ContainsTarget="true" />
              </EntityType>

              <EntityType Name="fizz">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
              </EntityType>

              <EntityContainer Name="container">
                <EntitySet Name="foos" EntityType="root.foo" />
              </EntityContainer>
            </Schema>
          </edmx:DataServices>
        </edmx:Edmx>

        ### entity type that is contained by multiple properties of another entity type

        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="4.0" xmlns:ags="http://aggregator.microsoft.com/internal" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" xmlns:odata="http://schemas.microsoft.com/oDataCapabilities">
          <edmx:DataServices>
            <Schema Namespace="root" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
              <EntityType Name="foo">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />

                <NavigationProperty Name="bars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
                <NavigationProperty Name="otherBars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
              </EntityType>

              <EntityType Name="bar">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
              </EntityType>

              <EntityContainer Name="container">
                <EntitySet Name="foos" EntityType="root.foo" />
              </EntityContainer>
            </Schema>
          </edmx:DataServices>
        </edmx:Edmx>
        
        ### an entity type that is contained by multiple properties of a complex type

        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="4.0" xmlns:ags="http://aggregator.microsoft.com/internal" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" xmlns:odata="http://schemas.microsoft.com/oDataCapabilities">
          <edmx:DataServices>
            <Schema Namespace="root" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
              <EntityType Name="foo">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
		
		        <Property Name="someGrouping" Type="root.someGrouping" Nullable="false" />
              </EntityType>

              <ComplexType Name="someGrouping">
	            <NavigationProperty Name="bars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
		        <NavigationProperty Name="otherBars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
	          </ComplexType>

              <EntityType Name="bar">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
              </EntityType>
      
              <EntityContainer Name="container">
                <EntitySet Name="foos" EntityType="root.foo" />
              </EntityContainer>
            </Schema>
          </edmx:DataServices>
        </edmx:Edmx>

        ### an entity type that is contained by a property of a complex type that is nested under other complex types

        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="4.0" xmlns:ags="http://aggregator.microsoft.com/internal" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" xmlns:odata="http://schemas.microsoft.com/oDataCapabilities">
          <edmx:DataServices>
            <Schema Namespace="root" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
              <EntityType Name="foo">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
		
		        <Property Name="someGrouping" Type="root.someGrouping" Nullable="false" />
              </EntityType>

              <ComplexType Name="someGrouping">
		        <Property Name="anotherGrouping" Type="root.anotherGrouping" Nullable="false" />
	          </ComplexType>

              <ComplexType Name="anotherGrouping">
	            <NavigationProperty Name="bars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
		        <Property Name="aFinalGrouping" Type="root.aFinalGrouping" Nullable="false" />
	          </ComplexType>
	  
	          <ComplexType Name="aFinalGrouping">
	            <NavigationProperty Name="moreBars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
	          </ComplexType>

              <EntityType Name="bar">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
              </EntityType>
      
              <EntityContainer Name="container">
                <EntitySet Name="foos" EntityType="root.foo" />
              </EntityContainer>
            </Schema>
          </edmx:DataServices>
        </edmx:Edmx>

        ### an entity type that is contained *and* not contained by a property of a complex type that is nested under other complex types

        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="4.0" xmlns:ags="http://aggregator.microsoft.com/internal" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" xmlns:odata="http://schemas.microsoft.com/oDataCapabilities">
          <edmx:DataServices>
            <Schema Namespace="root" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
              <EntityType Name="foo">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
		
		        <Property Name="someGrouping" Type="root.someGrouping" Nullable="false" />
              </EntityType>

              <ComplexType Name="someGrouping">
		        <Property Name="anotherGrouping" Type="root.anotherGrouping" Nullable="false" />
	          </ComplexType>

              <ComplexType Name="anotherGrouping">
	            <NavigationProperty Name="bars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
		        <Property Name="aFinalGrouping" Type="root.aFinalGrouping" Nullable="false" />
	          </ComplexType>
	  
	          <ComplexType Name="aFinalGrouping">
	            <NavigationProperty Name="moreBars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="false" />
                <!--TODO you need a binding or something-->
	          </ComplexType>

              <EntityType Name="bar">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
              </EntityType>
      
              <EntityContainer Name="container">
                <EntitySet Name="foos" EntityType="root.foo" />
              </EntityContainer>
            </Schema>
          </edmx:DataServices>
        </edmx:Edmx>

        ### an entity type that is not contained by a property of a complex type that is nested under other complex types

        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="4.0" xmlns:ags="http://aggregator.microsoft.com/internal" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" xmlns:odata="http://schemas.microsoft.com/oDataCapabilities">
          <edmx:DataServices>
            <Schema Namespace="root" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
              <EntityType Name="foo">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
		
		        <Property Name="someGrouping" Type="root.someGrouping" Nullable="false" />
              </EntityType>

              <ComplexType Name="someGrouping">
		        <Property Name="anotherGrouping" Type="root.anotherGrouping" Nullable="false" />
	          </ComplexType>

              <ComplexType Name="anotherGrouping">
	            <NavigationProperty Name="bars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="false" />
                <!--TODO you need a binding or something-->
		        <Property Name="aFinalGrouping" Type="root.aFinalGrouping" Nullable="false" />
	          </ComplexType>
	  
	          <ComplexType Name="aFinalGrouping">
	            <NavigationProperty Name="moreBars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="false" />
                <!--TODO you need a binding or something-->
	          </ComplexType>

              <EntityType Name="bar">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
              </EntityType>
      
              <EntityContainer Name="container">
                <EntitySet Name="foos" EntityType="root.foo" />
              </EntityContainer>
            </Schema>
          </edmx:DataServices>
        </edmx:Edmx>
        
        ### an entity type that is contained by the property of another entity type and has multiple derived types

        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="4.0" xmlns:ags="http://aggregator.microsoft.com/internal" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" xmlns:odata="http://schemas.microsoft.com/oDataCapabilities">
            <edmx:DataServices>
            <Schema Namespace="root" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
                <EntityType Name="foo">
                <Key>
                    <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
		
	            <NavigationProperty Name="bars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="true" />
                </EntityType>

                <EntityType Name="bar">
                <Key>
                    <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
                </EntityType>
	  
	            <EntityType Name="fizz" BaseType="root.bar">
	            <Property Name="aFizzProperty" Type="Edm.Int32" Nullable="false" />
	            </EntityType>
	  
	            <EntityType Name="buzz" BaseType="root.bar">
	            <Property Name="aBuzzProperty" Type="Edm.DateTimeOffset" Nullable="false" />
	            </EntityType>
      
                <EntityContainer Name="container">
                <EntitySet Name="foos" EntityType="root.foo" />
                </EntityContainer>
            </Schema>
            </edmx:DataServices>
        </edmx:Edmx>

        ### an entity type that is not contained by the property of another entity type

        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="4.0" xmlns:ags="http://aggregator.microsoft.com/internal" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" xmlns:odata="http://schemas.microsoft.com/oDataCapabilities">
          <edmx:DataServices>
            <Schema Namespace="root" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
              <EntityType Name="foo">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />

                <NavigationProperty Name="bars" Type="Collection(root.bar)" Nullable="false" ContainsTarget="false" />
                <!--TODO you need a binding or something-->
              </EntityType>

              <EntityType Name="bar">
                <Key>
                  <PropertyRef Name="id" />
                </Key>
                <Property Name="id" Type="Edm.String" Nullable="false" />
              </EntityType>

              <EntityContainer Name="container">
                <EntitySet Name="foos" EntityType="root.foo" />
              </EntityContainer>
            </Schema>
          </edmx:DataServices>
        </edmx:Edmx>

         */

        // the idea is that the key of the entity is the base64 encoded and semicolon delimited series of keys of the containing entities (plus the entity itself at the end)
        // the above assumes that the entities are contained in only one navigation property, which isn't necessarily true (a single entity *instance* is only contained in one property, but that entity type can be used as the type for multiple contained properties through the model)

        private readonly NewStuff._Design._0_Convention.IConvention convention;
        private readonly Uri domainAndStuff;

        public WaasFusionDataStoreMapping(NewStuff._Design._0_Convention.IConvention convention, Uri domainAndStuff)
        {
            this.convention = convention;
            this.domainAndStuff = domainAndStuff;
        }

        public IQueryOptionsWriter Get(ContainmentPath containmentPath)
        {
            // fusion won't care about the types in the containmentpath except the last one; other data stores might (probably?) care though

            var getRequestWriter = this.convention.Get2();
            var uriWriter = getRequestWriter.Write();
            var segmentWriter = uriWriter.Write(new PrecedingSegments(this.domainAndStuff.ToString()));

            var segments = containmentPath.ContainmentPathSegments.ToList();

            segmentWriter = segmentWriter.Write(new OdataUriSegment("v1.0"));
            //// TODO any other segments here
            segmentWriter = segmentWriter.Write(new OdataUriSegment(segments.Last().TypeName));

            var fusionKey = Base64Url
                .EncodeToString(
                    new ReadOnlySpan<byte>(
                        Encoding.UTF8.GetBytes(
                            string.Join(
                                ';',
                                segments
                                    .Select(segment => segment
                                        .Apply(
                                            keyed => $"{keyed.PropertyName};{keyed.Key}",
                                            keylessMap => keylessMap.PropertyName))))));

            IQueryOptionsWriter queryOptionsWriter;
            if (segments.Last() is ContainmentPathSegment.Keyed)
            {
                segmentWriter = segmentWriter.Write(new OdataUriSegment(fusionKey));
                queryOptionsWriter = segmentWriter.Write();
            }
            else
            {
                queryOptionsWriter = segmentWriter.Write();
                queryOptionsWriter = new QueryOptionsWriter(queryOptionsWriter, fusionKey);
            }

            return queryOptionsWriter;
        }

        private sealed class QueryOptionsWriter : IQueryOptionsWriter
        {
            private readonly IQueryOptionsWriter queryOptionsWriter;
            private readonly string key;

            public QueryOptionsWriter(IQueryOptionsWriter queryOptionsWriter, string key)
            {
                this.queryOptionsWriter = queryOptionsWriter;
                this.key = key;
            }

            public IQueryOptionValueWriter Write(QueryOptionParameter queryOptionParameter)
            {
                if (queryOptionParameter.Value == "$filter") //// TODO case sensitive, no dollar sign, etc.; you shouldn't need to worry about this actually because system query options should be strongly typed
                {
                    return new FilterQueryOptionValueWriter(this.queryOptionsWriter.Write(new QueryOptionParameter("$filter")), this.key);
                }
                else
                {
                    return new QueryOptionValueWriter(this.queryOptionsWriter.Write(queryOptionParameter), this.key);
                }
            }

            private sealed class FilterQueryOptionValueWriter : IQueryOptionValueWriter
            {
                private readonly IQueryOptionValueWriter queryOptionValueWriter;
                private readonly string key;

                public FilterQueryOptionValueWriter(IQueryOptionValueWriter queryOptionValueWriter, string key)
                {
                    this.queryOptionValueWriter = queryOptionValueWriter;
                    this.key = key;
                }

                public IQueryOptionsWriter Write(QueryOptionValue queryOptionValue)
                {
                    return this.queryOptionValueWriter.Write(new QueryOptionValue($"containerKey eq '{key}' and ({queryOptionValue.Value})"));
                }

                public IQueryOptionsWriter Write()
                {
                    return this.queryOptionValueWriter.Write(new QueryOptionValue($"containerKey eq '{key}'"));
                }
            }

            private sealed class QueryOptionValueWriter : IQueryOptionValueWriter
            {
                private readonly IQueryOptionValueWriter queryOptionValueWriter;
                private readonly string key;

                public QueryOptionValueWriter(IQueryOptionValueWriter queryOptionValueWriter, string key)
                {
                    this.queryOptionValueWriter = queryOptionValueWriter;
                    this.key = key;
                }

                public IQueryOptionsWriter Write(QueryOptionValue queryOptionValue)
                {
                    return new QueryOptionsWriter(this.queryOptionValueWriter.Write(queryOptionValue), this.key);
                }

                public IQueryOptionsWriter Write()
                {
                    return new QueryOptionsWriter(this.queryOptionValueWriter.Write(), this.key);
                }
            }

            public IFragmentWriter Write()
            {
                var queryOptionValueWriter = this.queryOptionsWriter.Write(new QueryOptionParameter("$filter"));
                return queryOptionValueWriter.Write(new QueryOptionValue($"containerKey eq '{key}'")).Write();
            }
        }
    }

    public static class Extensions
    {
        public static IGetRequestWriter Get2(this NewStuff._Design._0_Convention.IConvention convention)
        {
            throw new Exception("tODO");
        }
    }

    public interface IGetRequestWriter
    {
        IUriWriter Write();
    }

    public interface IUriWriter
    {
        ISegmentWriter Write(PrecedingSegments precedingSegments);
    }

    public interface ISegmentWriter
    {
        ISegmentWriter Write(OdataUriSegment odataUriSegment);

        IQueryOptionsWriter Write();
    }

    public interface IQueryOptionsWriter
    {
        IQueryOptionValueWriter Write(QueryOptionParameter queryOptionParameter);

        IFragmentWriter Write();
    }

    public sealed class QueryOptionParameter
    {
        internal QueryOptionParameter(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public interface IQueryOptionValueWriter
    {
        IQueryOptionsWriter Write(QueryOptionValue queryOptionValue);

        IQueryOptionsWriter Write();
    }

    public sealed class QueryOptionValue
    {
        internal QueryOptionValue(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public interface IFragmentWriter
    {
        //// TODO add an overload to write fragment

        IOdataResponseReader Write(); // this actually sends the request
    }

    public sealed class ContainmentPath
    {
        public ContainmentPath(IEnumerable<ContainmentPathSegment> containmentPathSegments)
        {
            ContainmentPathSegments = containmentPathSegments;
        }

        public IEnumerable<ContainmentPathSegment> ContainmentPathSegments { get; }
    }

    public abstract class ContainmentPathSegment
    {
        private ContainmentPathSegment(string typeName, string propertyName)
        {
            TypeName = typeName;
            PropertyName = propertyName;
        }

        public string TypeName { get; }

        public string PropertyName { get; }

        public TResult Apply<TResult>(
            Func<Keyed, TResult> keyedMap,
            Func<Keyless, TResult> keylessMap)
        {
            if (this is Keyed keyed)
            {
                return keyedMap(keyed);
            }
            else if (this is Keyless keyless)
            {
                return keylessMap(keyless);
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        public sealed class Keyed : ContainmentPathSegment
        {
            public Keyed(string typeName, string propertyName, string key)
                : base(typeName, propertyName)
            {
                Key = key;
            }

            public string Key { get; }
        }

        public sealed class Keyless : ContainmentPathSegment
        {
            public Keyless(string typeName, string propertyName)
                : base(typeName, propertyName)
            {
            }
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

            var rootEntityType = this.edmModel.GetTypeOfRootSegment(odataUriSegment.Value);
            var containmentPathSegments = new List<ContainmentPathSegment>();

            odataUriSegmentReader = GetNextIdPart(odataUriSegmentReader, rootEntityType, containmentPathSegments);
            while (true)
            {
                var odataUriSegmentReaderToken = odataUriSegmentReader.Read();
                var @continue = odataUriSegmentReaderToken.Apply(
                    (segment, context) =>
                    {
                        odataUriSegmentReader = segment.OdataUriSegmentReader;
                        return true;
                    },
                    (queryOption, context) => false,
                    (fragment, context) => false,
                    new WaasNothing());
                if (@continue)
                {
                    odataUriSegment = odataUriSegmentReader.Value;
                    if (rootEntityType.TryGetTypeOfComplexProperty(odataUriSegment.Value, out var complexPropertyType))
                    {
                        //// TODO you are here
                        
                        complexPropertyType.

                    }
                    else if (rootEntityType.TryGetTypeOfNavigationProperty(odataUriSegment.Value, out var navigationPropertyType))
                    {
                        if (navigationPropertyType.IsContained)
                        {
                            odataUriSegmentReader = GetNextIdPart(odataUriSegmentReader, navigationPropertyType, containmentPathSegments);
                        }
                        else
                        {
                            containmentPathSegments = new List<ContainmentPathSegment>(); //// TODO you need to get the "root" for this non-contained navigation, probably from a navigation property binding
                            odataUriSegmentReader = GetNextIdPart(odataUriSegmentReader, navigationPropertyType, containmentPathSegments);
                        }
                    }
                    else if (rootEntityType.TryGetTypeOfPrimitiveProperty(odataUriSegment.Value, out var primitivePropertyType))
                    {
                        //// TODO you need to keep track of this somehow; you've got the key, sure, but you still need to return the raw primitive value
                    }
                    else
                    {
                        throw new Exception("tODO invalid uri");
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private IOdataUriSegmentReader GetNextIdPart(IOdataUriSegmentReader odataUriSegmentReader, IEdmEntityType edmEntityType, List<ContainmentPathSegment> containmentPathSegments)
        {
            var propertyName = odataUriSegmentReader.Value;
            if (edmEntityType.IsMultiValued)
            {
                var odataUriSegmentReaderToken = odataUriSegmentReader.Read();
                odataUriSegmentReader = odataUriSegmentReaderToken.Apply(
                    (segment, context) => segment.OdataUriSegmentReader,
                    (queryOptions, context) => throw new Exception("TODO invalid uri"),
                    (fragment, context) => throw new Exception("TODO invalid uri"),
                    new WaasNothing());
                var odataUriSegment = odataUriSegmentReader.Value;

                containmentPathSegments.Add(new ContainmentPathSegment.Keyed(edmEntityType.TypeName, propertyName.Value, odataUriSegment.Value));
            }
            else
            {
                containmentPathSegments.Add(new ContainmentPathSegment.Keyless(edmEntityType.TypeName, propertyName.Value));
            }

            return odataUriSegmentReader;
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
