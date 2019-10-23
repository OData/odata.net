using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class DataServiceQueryTests
    {
        [Fact]
        public async Task CanCallDataServiceQuery()
        {
            var client = new TestClient();
            var dataServiceQuery = client.People;
            var peopleDataServiceQuery = dataServiceQuery
                //.Expand(x => x.Friends)
                .Expand("Friends")
                .Take(1);
            var people = await (peopleDataServiceQuery as DataServiceQuery<Person>).ExecuteAsync();
            var person = people.First();
        }
        [Fact]
        public async Task CanFakeDataServiceQuery()
        {
            var fakeData = new[]
            {
                new Person{
                    UserName = "jdoe",
                    FirstName = "John",
                    Emails = new System.Collections.ObjectModel.Collection<string>(new [] { "jdoe@acme.com"}.ToList()),
                    Friends = new System.Collections.ObjectModel.Collection<Person>()
                }
            };
            var dataServiceQuery = new FakeDataServiceQuery<Person>(fakeData.AsQueryable());
            var peopleDataServiceQuery = dataServiceQuery
                .Expand("Friends")
                .Select(x => x);
            peopleDataServiceQuery = peopleDataServiceQuery
                .Take(1);
            var people = await (peopleDataServiceQuery as IDataServiceQuery<Person>).ExecuteAsync();
            var person = people.First();
        }
    }

    public class FakeDataServiceQueryProvider<T> : IQueryProvider
    {
        private IQueryable<T> _fakeSource;

        public IQueryable<T> FakeSource { get { return this._fakeSource; } }

        public FakeDataServiceQueryProvider(IQueryable<T> fakeSource)
        {
            this._fakeSource = fakeSource;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new FakeDataServiceQuery<T>(expression, this as FakeDataServiceQueryProvider<T>);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
    public class FakeDataServiceQuery<T> : IDataServiceQuery<T>
    {
        private FakeDataServiceQueryProvider<T> _fakeDataServiceQueryProvider;
        private Expression _fakeExpression;

        public FakeDataServiceQuery(IQueryable<T> fakeSource)
        {
            this._fakeExpression = fakeSource.Expression;
            this._fakeDataServiceQueryProvider = new FakeDataServiceQueryProvider<T>(fakeSource);
        }

        public FakeDataServiceQuery(Expression expression, FakeDataServiceQueryProvider<T> fakeDataServiceQueryProvider)
        {
            this._fakeExpression = expression;
            this._fakeDataServiceQueryProvider = fakeDataServiceQueryProvider;
        }

        public Type ElementType => this._fakeDataServiceQueryProvider.FakeSource.ElementType;

        public Expression Expression => this._fakeExpression;

        public IQueryProvider Provider => this._fakeDataServiceQueryProvider;

        public DataServiceContext Context => throw new NotImplementedException();

        public Task<IEnumerable<T>> ExecuteAsync()
        {
            return Task.Run(() => this._fakeDataServiceQueryProvider.FakeSource.ToArray().AsEnumerable());
        }

        public IDataServiceQuery<T> Expand(string path)
        {
            return this;
        }

        public IDataServiceQuery<T> Expand<TTarget>(Expression<Func<T, TTarget>> navigationPropertyAccessor)
        {
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._fakeDataServiceQueryProvider.FakeSource.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._fakeDataServiceQueryProvider.FakeSource.GetEnumerator();
        }
    }

    public class TestClient : DataServiceContext
    {
        public TestClient() : base(new Uri("https://services.odata.org/TripPinRESTierService"), global::Microsoft.OData.Client.ODataProtocolVersion.V4)
        {
            this.Format.LoadServiceModel = GeneratedEdmModel.GetInstance;
        }

        DataServiceQuery<Person> _people;
        [global::Microsoft.OData.Client.OriginalNameAttribute("People")]
        public DataServiceQuery<Person> People {
            get
            {

                if ((this._people == null))
                {
                    this._people = base.CreateQuery<Person>("People");
                }
                return this._people;
            }
            set
            {
                this._people = value;
            }
        }

        private abstract class GeneratedEdmModel
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.6.0")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.6.0")]
            private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Microsoft.OData.Service.Sample.TrippinInMemory.Models"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""UserName"" />
        </Key>
        <Property Name=""UserName"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""FirstName"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""LastName"" Type=""Edm.String"" />
        <Property Name=""MiddleName"" Type=""Edm.String"" />
        <Property Name=""Gender"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender"" Nullable=""false"" />
        <Property Name=""Age"" Type=""Edm.Int64"" />
        <Property Name=""Emails"" Type=""Collection(Edm.String)"" />
        <Property Name=""AddressInfo"" Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location)"" />
        <Property Name=""HomeAddress"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location"" />
        <Property Name=""FavoriteFeature"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Feature"" Nullable=""false"" />
        <Property Name=""Features"" Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Feature)"" Nullable=""false"" />
        <NavigationProperty Name=""Friends"" Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"" />
        <NavigationProperty Name=""BestFriend"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"" />
        <NavigationProperty Name=""Trips"" Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)"" />
      </EntityType>
      <EntityType Name=""Airline"">
        <Key>
          <PropertyRef Name=""AirlineCode"" />
        </Key>
        <Property Name=""AirlineCode"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""Airport"">
        <Key>
          <PropertyRef Name=""IcaoCode"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""IcaoCode"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""IataCode"" Type=""Edm.String"" />
        <Property Name=""Location"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.AirportLocation"" />
      </EntityType>
      <ComplexType Name=""Location"">
        <Property Name=""Address"" Type=""Edm.String"" />
        <Property Name=""City"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.City"" />
      </ComplexType>
      <ComplexType Name=""City"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""CountryRegion"" Type=""Edm.String"" />
        <Property Name=""Region"" Type=""Edm.String"" />
      </ComplexType>
      <ComplexType Name=""AirportLocation"" BaseType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location"">
        <Property Name=""Loc"" Type=""Edm.GeographyPoint"" />
      </ComplexType>
      <ComplexType Name=""EventLocation"" BaseType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location"">
        <Property Name=""BuildingInfo"" Type=""Edm.String"" />
      </ComplexType>
      <EntityType Name=""Trip"">
        <Key>
          <PropertyRef Name=""TripId"" />
        </Key>
        <Property Name=""TripId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ShareId"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""Budget"" Type=""Edm.Single"" Nullable=""false"" />
        <Property Name=""Description"" Type=""Edm.String"" />
        <Property Name=""Tags"" Type=""Collection(Edm.String)"" />
        <Property Name=""StartsAt"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
        <Property Name=""EndsAt"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
        <NavigationProperty Name=""PlanItems"" Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.PlanItem)"" />
      </EntityType>
      <EntityType Name=""PlanItem"">
        <Key>
          <PropertyRef Name=""PlanItemId"" />
        </Key>
        <Property Name=""PlanItemId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ConfirmationCode"" Type=""Edm.String"" />
        <Property Name=""StartsAt"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
        <Property Name=""EndsAt"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
        <Property Name=""Duration"" Type=""Edm.Duration"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""Event"" BaseType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.PlanItem"">
        <Property Name=""OccursAt"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.EventLocation"" />
        <Property Name=""Description"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""PublicTransportation"" BaseType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.PlanItem"">
        <Property Name=""SeatNumber"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""Flight"" BaseType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.PublicTransportation"">
        <Property Name=""FlightNumber"" Type=""Edm.String"" />
        <NavigationProperty Name=""Airline"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline"" />
        <NavigationProperty Name=""From"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport"" />
        <NavigationProperty Name=""To"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport"" />
      </EntityType>
      <EntityType Name=""Employee"" BaseType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"">
        <Property Name=""Cost"" Type=""Edm.Int64"" Nullable=""false"" />
        <NavigationProperty Name=""Peers"" Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"" />
      </EntityType>
      <EntityType Name=""Manager"" BaseType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"">
        <Property Name=""Budget"" Type=""Edm.Int64"" Nullable=""false"" />
        <Property Name=""BossOffice"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location"" />
        <NavigationProperty Name=""DirectReports"" Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"" />
      </EntityType>
      <EnumType Name=""PersonGender"">
        <Member Name=""Male"" Value=""0"" />
        <Member Name=""Female"" Value=""1"" />
        <Member Name=""Unknow"" Value=""2"" />
      </EnumType>
      <EnumType Name=""Feature"">
        <Member Name=""Feature1"" Value=""0"" />
        <Member Name=""Feature2"" Value=""1"" />
        <Member Name=""Feature3"" Value=""2"" />
        <Member Name=""Feature4"" Value=""3"" />
      </EnumType>
      <Function Name=""GetPersonWithMostFriends"">
        <ReturnType Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"" />
      </Function>
      <Function Name=""GetNearestAirport"">
        <Parameter Name=""lat"" Type=""Edm.Double"" Nullable=""false"" />
        <Parameter Name=""lon"" Type=""Edm.Double"" Nullable=""false"" />
        <ReturnType Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport"" />
      </Function>
      <Function Name=""GetFavoriteAirline"" IsBound=""true"" EntitySetPath=""person"">
        <Parameter Name=""person"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"" />
        <ReturnType Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline"" />
      </Function>
      <Function Name=""GetFriendsTrips"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"" />
        <Parameter Name=""userName"" Type=""Edm.String"" Nullable=""false"" Unicode=""false"" />
        <ReturnType Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)"" />
      </Function>
      <Function Name=""GetInvolvedPeople"" IsBound=""true"">
        <Parameter Name=""trip"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip"" />
        <ReturnType Type=""Collection(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"" />
      </Function>
      <Action Name=""ResetDataSource"" />
      <Function Name=""UpdatePersonLastName"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"" />
        <Parameter Name=""lastName"" Type=""Edm.String"" Nullable=""false"" Unicode=""false"" />
        <ReturnType Type=""Edm.Boolean"" Nullable=""false"" />
      </Function>
      <Action Name=""ShareTrip"" IsBound=""true"">
        <Parameter Name=""personInstance"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"" />
        <Parameter Name=""userName"" Type=""Edm.String"" Nullable=""false"" Unicode=""false"" />
        <Parameter Name=""tripId"" Type=""Edm.Int32"" Nullable=""false"" />
      </Action>
      <EntityContainer Name=""Container"">
        <EntitySet Name=""People"" EntityType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"">
          <NavigationPropertyBinding Path=""Friends"" Target=""People"" />
          <NavigationPropertyBinding Path=""BestFriend"" Target=""People"" />
          <NavigationPropertyBinding Path=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee/Peers"" Target=""People"" />
          <NavigationPropertyBinding Path=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Manager/DirectReports"" Target=""People"" />
        </EntitySet>
        <EntitySet Name=""Airlines"" EntityType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline"">
          <Annotation Term=""Org.OData.Core.V1.OptimisticConcurrency"">
            <Collection>
              <PropertyPath>Name</PropertyPath>
            </Collection>
          </Annotation>
        </EntitySet>
        <EntitySet Name=""Airports"" EntityType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport"" />
        <EntitySet Name=""NewComePeople"" EntityType=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"" />
        <Singleton Name=""Me"" Type=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person"" />
        <FunctionImport Name=""GetPersonWithMostFriends"" Function=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.GetPersonWithMostFriends"" EntitySet=""People"" />
        <FunctionImport Name=""GetNearestAirport"" Function=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.GetNearestAirport"" EntitySet=""Airports"" />
        <ActionImport Name=""ResetDataSource"" Action=""Microsoft.OData.Service.Sample.TrippinInMemory.Models.ResetDataSource"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.6.0")]
            public static global::Microsoft.OData.Edm.IEdmModel GetInstance()
            {
                return ParsedModel;
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.6.0")]
            private static global::Microsoft.OData.Edm.IEdmModel LoadModelFromString()
            {
                global::System.Xml.XmlReader reader = CreateXmlReader(Edmx);
                try
                {
                    global::System.Collections.Generic.IEnumerable<global::Microsoft.OData.Edm.Validation.EdmError> errors;
                    global::Microsoft.OData.Edm.IEdmModel edmModel;

                    if (!global::Microsoft.OData.Edm.Csdl.CsdlReader.TryParse(reader, false, out edmModel, out errors))
                    {
                        global::System.Text.StringBuilder errorMessages = new System.Text.StringBuilder();
                        foreach (var error in errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                            errorMessages.Append("; ");
                        }
                        throw new global::System.InvalidOperationException(errorMessages.ToString());
                    }

                    return edmModel;
                }
                finally
                {
                    ((global::System.IDisposable)(reader)).Dispose();
                }
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.6.0")]
            private static global::System.Xml.XmlReader CreateXmlReader(string edmxToParse)
            {
                return global::System.Xml.XmlReader.Create(new global::System.IO.StringReader(edmxToParse));
            }
        }
    }
    public class Person
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public long? Age { get; set; }
        public System.Collections.ObjectModel.Collection<string> Emails { get; set; }
        public System.Collections.ObjectModel.Collection<Person> Friends { get; set; }
    }

}