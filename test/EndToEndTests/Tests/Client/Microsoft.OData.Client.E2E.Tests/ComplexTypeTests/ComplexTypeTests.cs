//-----------------------------------------------------------------------------
// <copyright file="ComplexTypeTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.ComplexTypes;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;
using ClientDefaultModel = Microsoft.OData.E2E.TestCommon.Common.Client.Default;

namespace Microsoft.OData.Client.E2E.Tests.ComplexTypeTests
{
    public class ComplexTypeTests : EndToEndTestBase<ComplexTypeTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ComplexTypeTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public ComplexTypeTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDefaultDataSource();
        }

        #region Complex type inheritance
        [Fact]
        public async Task QueryingAPropertyOfADerivedComplexType_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            var familyName = await _context.People.ByKey(1).Select(p => (p.HomeAddress as ClientDefaultModel.HomeAddress).FamilyName).GetValueAsync();

            Assert.NotNull(familyName);
            Assert.Equal("Cats", familyName);
        }

        [Fact]
        public async Task QueryingATopLevelDerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);

            var address = await _context.People.ByKey(1).GetValueAsync();
            var homeAddress = address.HomeAddress as ClientDefaultModel.HomeAddress;

            Assert.NotNull(homeAddress);
            Assert.Equal("Cats", homeAddress.FamilyName);
        }

        [Fact]
        public async Task AFilterQueryOptionOnADerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);

            var queryable0 = _context.People.Where(p => (p.HomeAddress as ClientDefaultModel.HomeAddress).FamilyName == "Cats");

            ClientDefaultModel.Person personResult = (await ((DataServiceQuery<ClientDefaultModel.Person>)queryable0).ExecuteAsync()).Single();

            Assert.NotNull(personResult);

            ClientDefaultModel.HomeAddress homeAddress = personResult.HomeAddress as ClientDefaultModel.HomeAddress;

            Assert.NotNull(homeAddress);
            Assert.Equal(1, personResult.PersonID);
            Assert.Equal("Cats", homeAddress.FamilyName);
        }

        [Fact]
        public async Task UpdatingADerivedComplexType_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            var people = (await _context.People.ExecuteAsync()).ToList();
            var person = await _context.People.ByKey(1).GetValueAsync();

            Assert.NotNull(person);

            var homeAddress = person.HomeAddress as ClientDefaultModel.HomeAddress;

            Assert.NotNull(homeAddress);
            Assert.Equal("Cats", homeAddress.FamilyName);

            homeAddress.City = "Shanghai";
            homeAddress.FamilyName = "Tigers";

            _context.UpdateObject(person);
            await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

            var updatedPerson = await _context.People.ByKey(1).GetValueAsync();
            var updatedHomeAddress = updatedPerson.HomeAddress as ClientDefaultModel.HomeAddress;

            Assert.NotNull(updatedHomeAddress);
            Assert.Equal("Shanghai", updatedHomeAddress.City);
            Assert.Equal("Tigers", updatedHomeAddress.FamilyName);
        }

        [Fact]
        public async Task InsertingAndDeletingAnEntityWithADerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);

            // create an an account entity and a contained PI entity
            ClientDefaultModel.Person newPerson = new ClientDefaultModel.Person()
            {
                PersonID = 10001,
                FirstName = "New",
                LastName = "Person",
                MiddleName = "Guy",
                Home = GeographyPoint.Create(32.1, 23.1),
                HomeAddress = new ClientDefaultModel.HomeAddress
                {
                    City = "Shanghai",
                    Street = "Zixing Rd",
                    PostalCode = "200241",
                    FamilyName = "New"
                }
            };

            _context.AddToPeople(newPerson);
            await _context.SaveChangesAsync();

            var queryable0 = _context.People.ByKey(10001);
            var personResult = await queryable0.GetValueAsync();

            Assert.NotNull(personResult);

            var homeAddress = personResult.HomeAddress as ClientDefaultModel.HomeAddress;

            Assert.NotNull(homeAddress);
            Assert.Equal("New", homeAddress.FamilyName);

            // delete
            var personToDelete = await _context.People.ByKey(10001).GetValueAsync();

            _context.DeleteObject(personToDelete);
            await _context.SaveChangesAsync();

            var People = (await _context.People.ExecuteAsync()).ToList();
            var queryDeletedPerson = People.Where(person => person.PersonID == 10001);

            Assert.Empty(queryDeletedPerson);
        }
        #endregion

        #region Open complex type
        [Fact]
        public async Task QueryingAnOpenComplexType_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            var accountInfo = await _context.Accounts.ByKey(101).Select(a => a.AccountInfo).GetValueAsync();
            Assert.NotNull(accountInfo);

            accountInfo.DynamicProperties.TryGetValue("MiddleName", out var middleName);
            Assert.Equal("Hood", middleName);

            accountInfo.DynamicProperties.TryGetValue("Address", out var address);
            Assert.NotNull(address);
        }

        [Fact]
        public async Task UpdatingAnOpenComplexType_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            var account = await _context.Accounts.ByKey(101).GetValueAsync();
            Assert.NotNull(account);

            var accountInfo = account.AccountInfo;
            Assert.NotNull(accountInfo);

            accountInfo.DynamicProperties.TryGetValue("MiddleName", out var middleName);
            Assert.Equal("Hood", middleName);

            accountInfo.FirstName = "Peter";
            accountInfo.DynamicProperties["MiddleName"] = "White";

            // verify: open type enum property from client side should be serialized with odata.type
            accountInfo.DynamicProperties["FavoriteColor"] = ClientDefaultModel.Color.Blue;
            accountInfo.DynamicProperties["Address"] = new ClientDefaultModel.Address
            {
                Street = "1",
                City = "2",
                PostalCode = "3"
            };

            _context.UpdateObject(account);
            await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

            var updatedAccount = await _context.Accounts.ByKey(101).GetValueAsync();
            Assert.NotNull(updatedAccount);

            var updatedAccountInfo = updatedAccount.AccountInfo;
            Assert.NotNull(updatedAccountInfo);
            Assert.Equal("Peter", updatedAccountInfo.FirstName);
            accountInfo.DynamicProperties.TryGetValue("MiddleName", out var updatedMiddleName);
            Assert.Equal("White", updatedMiddleName);

            accountInfo.DynamicProperties.TryGetValue("FavoriteColor", out var updatedFavoriteColor);
            Assert.Equal(ClientDefaultModel.Color.Blue, updatedFavoriteColor);
            accountInfo.DynamicProperties.TryGetValue("Address", out var updatedAddress);
            Assert.Equal("2", (updatedAddress as ClientDefaultModel.Address).City);
            Assert.Equal("1", (updatedAddress as ClientDefaultModel.Address).Street);
            Assert.Equal("3", (updatedAddress as ClientDefaultModel.Address).PostalCode);
        }

        [Fact]
        public async Task UpdatingAnOpenComplexTypeWithUndeclaredProperties_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;
            _context.Configurations.RequestPipeline.OnEntryStarting(ea => EntryStarting(ea));

            var account = await _context.Accounts.ByKey(101).GetValueAsync();
            Assert.Equal(3, account.AccountInfo.DynamicProperties.Count);

            // In practice, transient property data would be mutated here in the partial companion to the client proxy.

            _context.UpdateObject(account);
            await _context.SaveChangesAsync();

            var updatedAccount = await _context.Accounts.ByKey(101).GetValueAsync();
            Assert.Equal(4, account.AccountInfo.DynamicProperties.Count);

            account.AccountInfo.DynamicProperties.TryGetValue("dynamicPropertyKey", out var addedDynamicPropertyValue);
            Assert.Equal("dynamicPropertyValue", addedDynamicPropertyValue);
        }

        [Fact]
        public async Task UpdatingAndReadingAnOpenComplexTypeWithUndeclaredProperties_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;
            _context.Configurations.RequestPipeline.OnMessageWriterSettingsCreated(wsa =>
            {
                // writer supports ODataUntypedValue
                wsa.Settings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            });

            _context.Configurations.RequestPipeline.OnEntryStarting(ea =>
            {
                if (ea.Entity.GetType() == typeof(ClientDefaultModel.AccountInfo))
                {
                    var undeclaredOdataProperty = new ODataProperty()
                    {
                        Name = "UndecalredOpenProperty1",
                        Value = new ODataUntypedValue() { RawValue = "{ \"sender\": \"RSS\", \"senderImage\": \"https://exchangelabs.live-int.com/connectors/content/images/feed-icon-128px.png?upn=admin%40tenant-EXHR-3837dom.EXTEST.MICROSOFT.COM\", \"summary\": \"RSS is now connected to your mailbox\", \"title\": null }" }
                    };
                    var accountInfoComplexValueProperties = ea.Entry.Properties as List<ODataProperty>;
                    accountInfoComplexValueProperties.Add(undeclaredOdataProperty);
                }
            });

            var account = await _context.Accounts.ByKey(101).GetValueAsync();
            _context.UpdateObject(account);
            await _context.SaveChangesAsync();

            _context.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated(rsa =>
            {
                // reader supports undeclared property
                rsa.Settings.Validations ^= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            });

            ODataUntypedValue undeclaredOdataPropertyValue = null;
            _context.Configurations.ResponsePipeline.OnEntityMaterialized(rea =>
            {
                if (rea.Entity.GetType() == typeof(ClientDefaultModel.AccountInfo))
                {
                    var undeclaredOdataProperty = rea.Entry.Properties.OfType<ODataProperty>().FirstOrDefault(s => s.Name == "UndecalredOpenProperty1");
                    undeclaredOdataPropertyValue = (ODataUntypedValue)undeclaredOdataProperty.Value;
                }
            });

            //There is a bug in OData Client that need fixing.OData Client cannot materialize ODataUntypedValues.
            //var accountReturned = await _context.Accounts.ByKey(101).GetValueAsync();
            /* Assert.Equal<string>(
                 "{\"sender\":\"RSS\",\"senderImage\":\"https://exchangelabs.live-int.com/connectors/content/images/feed-icon-128px.png?upn=admin%40tenant-EXHR-3837dom.EXTEST.MICROSOFT.COM\",\"summary\":\"RSS is now connected to your mailbox\",\"title\":null}",
                 undeclaredOdataPropertyValue.RawValue);*/
        }

        #endregion
        private void EntryStarting(WritingEntryArgs ea)
        {
            if (ea.Entity.GetType() == typeof(ClientDefaultModel.AccountInfo))
            {
                var properties = ea.Entry.Properties as List<ODataProperty>;
                var undeclaredOdataProperty = new ODataProperty() { Name = "dynamicPropertyKey", Value = "dynamicPropertyValue" };
                properties.Add(undeclaredOdataProperty);
                ea.Entry.Properties = properties;
            }
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "complextype/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
