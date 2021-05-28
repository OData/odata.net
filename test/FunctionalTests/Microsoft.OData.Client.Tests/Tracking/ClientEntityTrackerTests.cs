//---------------------------------------------------------------------
// <copyright file="ClientEntityTrackerTests.cs" company="Microsoft">
// Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Tests.Tracking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.OData.Edm.Csdl;
    using Xunit;

    public class ClientEntityTrackerTests
    {
        private Container DefaultTrackingContext;

        #region TestEdmx
        private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
        <edmx:DataServices>
            <Schema Namespace=""Microsoft.OData.Client.Tests.Tracking"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                <EntityType Name=""ExampleObject"">
                    <Key>
                        <PropertyRef Name=""Id""/>
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                    <Property Name=""Name"" Type=""Edm.String""/>
                </EntityType>
                <EntityType Name=""ExampleObjectA"">
                    <Key>
                        <PropertyRef Name=""Id""/>
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                    <Property Name=""Name"" Type=""Edm.String""/>
                </EntityType>
                <EntityType Name=""ExampleObjectB"">
                    <Key>
                        <PropertyRef Name=""Id""/>
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                    <Property Name=""Name"" Type=""Edm.String""/>
                </EntityType>
                <EntityType Name=""ExampleObjectC"">
                    <Key>
                        <PropertyRef Name=""Id""/>
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                    <Property Name=""Name"" Type=""Edm.String""/>
                </EntityType>
                <EntityType Name=""ExampleObjectD"">
                    <Key>
                        <PropertyRef Name=""Id""/>
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                    <Property Name=""Name"" Type=""Edm.String""/>
                </EntityType>
                <EntityType Name=""ExampleObjectE"">
                    <Key>
                        <PropertyRef Name=""Id""/>
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                    <Property Name=""Name"" Type=""Edm.String""/>
                </EntityType>
            </Schema>
            <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                <EntityContainer Name=""Container"">
                    <EntitySet Name=""ExampleObject"" EntityType=""Microsoft.OData.Client.Tests.Tracking.ExampleObject""/>
                    <EntitySet Name=""ExampleObjectA"" EntityType=""Microsoft.OData.Client.Tests.Tracking.ExampleObjectA""/>
                    <EntitySet Name=""ExampleObjectB"" EntityType=""Microsoft.OData.Client.Tests.Tracking.ExampleObjectB""/>
                    <EntitySet Name=""ExampleObjectC"" EntityType=""Microsoft.OData.Client.Tests.Tracking.ExampleObjectC""/>
                    <EntitySet Name=""ExampleObjectD"" EntityType=""Microsoft.OData.Client.Tests.Tracking.ExampleObjectD""/>
                    <EntitySet Name=""ExampleObjectE"" EntityType=""Microsoft.OData.Client.Tests.Tracking.ExampleObjectE""/>
                </EntityContainer>
            </Schema>
        </edmx:DataServices>
        </edmx:Edmx>";
        #endregion

        #region responses
        private const string Response = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#ExampleObject/$entity"",
            ""Id"":1,
            ""Name"":""Example1""}";

        private const string ResponseA = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#ExampleObjectA/$entity"",
            ""Id"":1,
            ""Name"":""Example1""}";

        private const string ResponseB = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#ExampleObjectB/$entity"",
            ""Id"":1,
            ""Name"":""Example1""}";

        private const string ResponseC = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#ExampleObjectC/$entity"",
            ""Id"":1,
            ""Name"":""Example1""}";

        private const string ResponseD = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#ExampleObjectD/$entity"",
            ""Id"":1,
            ""Name"":""Example1""}";

        private const string ResponseE = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#ExampleObjectE/$entity"",
            ""Id"":1,
            ""Name"":""Example1""}";
        #endregion

        public ClientEntityTrackerTests()
        {
            var uri = new Uri("http://localhost:8000");
            DefaultTrackingContext = new Container(uri);
        }

        [Fact]
        public async Task SavingObject_ThatOverridesEqualsAndGetHashCodeMethods_SavesSuccessfully()
        {
            SetupContextWithRequestPipelineForSaving(
             new DataServiceContext[] { DefaultTrackingContext },Response,"http://localhost:8000/ExampleObjects(1)");
            var exampleObject = new ExampleObject
            {
                Id = 0,
                Name = "Example1"
            };

            DefaultTrackingContext.AddObject("ExampleObjects", exampleObject);
            Assert.NotNull(DefaultTrackingContext.GetEntityDescriptor(exampleObject));
            await SaveContextChangesAsync(new DataServiceContext[] { DefaultTrackingContext });

        }

        [Fact]
        public async Task SavingExampleObject_ThatOverridesEqualsAndGetHashCodeMethods_SavesSuccessfully()
        {
            SetupContextWithRequestPipelineForSaving(
             new DataServiceContext[] { DefaultTrackingContext }, ResponseC, "http://localhost:8000/ExampleObjectCs(1)");
            var exampleObject = new ExampleObjectC
            {
                Id = 0,
                Name = "Example1"
            };

            DefaultTrackingContext.AddObject("ExampleObjectCs", exampleObject);
            Assert.NotNull(DefaultTrackingContext.GetEntityDescriptor(exampleObject));
            await SaveContextChangesAsync(new DataServiceContext[] { DefaultTrackingContext });

        }

        [Fact]
        public async Task SavingObject_ThatImplementsIEqualityComparerInterface_SavesSuccessfully()
        {
            SetupContextWithRequestPipelineForSaving(
             new DataServiceContext[] { DefaultTrackingContext }, ResponseA, "http://localhost:8000/ExampleObjectAs(1)");
            var exampleObject = new ExampleObjectA
            {
                Id = 0,
                Name = "Example1"
            };

            DefaultTrackingContext.AddObject("ExampleObjectAs", exampleObject);
            Assert.NotNull(DefaultTrackingContext.GetEntityDescriptor(exampleObject));
            await SaveContextChangesAsync(new DataServiceContext[] { DefaultTrackingContext });

        }

        [Fact]
        public async Task SavingExampleObject_ThatImplementsIEqualityComparerInterface_SavesSuccessfully()
        {
            SetupContextWithRequestPipelineForSaving(
             new DataServiceContext[] { DefaultTrackingContext }, ResponseD, "http://localhost:8000/ExampleObjectDs(1)");
            var exampleObject = new ExampleObjectD
            {
                Id = 0,
                Name = "Example1"
            };

            DefaultTrackingContext.AddObject("ExampleObjectDs", exampleObject);
            Assert.NotNull(DefaultTrackingContext.GetEntityDescriptor(exampleObject));
            await SaveContextChangesAsync(new DataServiceContext[] { DefaultTrackingContext });
        }

        [Fact]
        public async Task SavingObject_ThatDoesNotImplementOrOverrideEqualityComparer_SavesSuccessfully()
        {
            SetupContextWithRequestPipelineForSaving(
            new DataServiceContext[] { DefaultTrackingContext }, ResponseB, "http://localhost:8000/ExampleObjectBs(1)");
            var exampleObject = new ExampleObjectB
            {
                Id = 0,
                Name = "Example1"
            };

            DefaultTrackingContext.AddObject("ExampleObjectBs", exampleObject);
            Assert.NotNull(DefaultTrackingContext.GetEntityDescriptor(exampleObject));
            await SaveContextChangesAsync(new DataServiceContext[] { DefaultTrackingContext });

        }

        [Fact]
        public async Task SavingExampleObject_ThatDoesNotImplementOrOverrideEqualityComparer_SavesSuccessfully()
        {
            SetupContextWithRequestPipelineForSaving(
            new DataServiceContext[] { DefaultTrackingContext }, ResponseE, "http://localhost:8000/ExampleObjectEs(1)");
            var exampleObject = new ExampleObjectE
            {
                Id = 0,
                Name = "Example1"
            };

            DefaultTrackingContext.AddObject("ExampleObjectEs", exampleObject);
            Assert.NotNull(DefaultTrackingContext.GetEntityDescriptor(exampleObject));
            await SaveContextChangesAsync(new DataServiceContext[] { DefaultTrackingContext });

        }

        private async Task SaveContextChangesAsync(DataServiceContext[] dataServiceContexts)
        {
            foreach (var dataServiceContext in dataServiceContexts)
            {
                await dataServiceContext.SaveChangesAsync();
            }
        }

        private void SetupContextWithRequestPipelineForSaving(DataServiceContext[] dataServiceContexts, string response, string location)
        {
            foreach (var context in dataServiceContexts)
            {
                context.Configurations.RequestPipeline.OnMessageCreating =
                    (args) => new CustomizedRequestMessage(
                        args,
                        response,
                        new Dictionary<string, string>()
                        {
                            {"Content-Type", "application/json;charset=utf-8"},
                            { "Location", location },
                        });
            }
        }

        class Container : DataServiceContext
        {
            public Container(global::System.Uri serviceRoot) :
                base(serviceRoot, ODataProtocolVersion.V4)
            {
                this.Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(Edmx)));
                this.Format.UseJson();
                this.ExampleObjects = base.CreateQuery<ExampleObject>("ExampleObjects");
                this.ExampleObjectAs = base.CreateQuery<ExampleObjectA>("ExampleObjectAs");
                this.ExampleObjectBs = base.CreateQuery<ExampleObjectB>("ExampleObjectBs");
                this.ExampleObjectCs = base.CreateQuery<ExampleObjectC>("ExampleObjectCs");
                this.ExampleObjectDs = base.CreateQuery<ExampleObjectD>("ExampleObjectDs");
                this.ExampleObjectEs = base.CreateQuery<ExampleObjectE>("ExampleObjectEs");
            }
            public DataServiceQuery<ExampleObject> ExampleObjects { get; private set; }
            public DataServiceQuery<ExampleObjectA> ExampleObjectAs { get; private set; }
            public DataServiceQuery<ExampleObjectB> ExampleObjectBs { get; private set; }
            public DataServiceQuery<ExampleObjectC> ExampleObjectCs { get; private set; }
            public DataServiceQuery<ExampleObjectD> ExampleObjectDs { get; private set; }
            public DataServiceQuery<ExampleObjectE> ExampleObjectEs { get; private set; }
        }
    }
    [Key("Id")]
    public class ExampleObject : BaseEntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Equals(ExampleObject other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return other.Id == Id;
        }
        public override bool Equals(object obj)
        {
            var other = obj as ExampleObject;

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return other.Id == Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    [Key("Id")]
    public class ExampleObjectA : BaseEntityType, IEqualityComparer<ExampleObjectA>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Equals(ExampleObjectA x, ExampleObjectA y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ExampleObjectA obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    [Key("Id")]
    public class ExampleObjectB : BaseEntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Key("Id")]
    public class ExampleObjectC 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Equals(ExampleObjectC other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return other.Id == Id;
        }
        public override bool Equals(object obj)
        {
            var other = obj as ExampleObjectC;

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return other.Id == Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    [Key("Id")]
    public class ExampleObjectD : IEqualityComparer<ExampleObjectD>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Equals(ExampleObjectD x, ExampleObjectD y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ExampleObjectD obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    [Key("Id")]
    public class ExampleObjectE
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
