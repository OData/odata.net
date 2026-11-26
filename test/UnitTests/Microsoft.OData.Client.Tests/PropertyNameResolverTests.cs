//---------------------------------------------------------------------
// <copyright file="PropertyNameResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Client.Metadata;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class PropertyNameResolverTests
    {
        private static readonly Uri ServiceRoot = new Uri("https://service.test/");

        [Fact]
        public void ResolverAppliesToFilterAndSelectQueryNames()
        {
            DataServiceContext context = CreateContext();

            IQueryable<Customer> query = context.CreateQuery<Customer>("customers")
                .Where(customer => customer.CompanyName == "Contoso");

            string requestUri = query.ToString();
            Assert.Contains("$filter=company_name eq 'Contoso'", requestUri);

            string selectUri = context.CreateQuery<Customer>("customers")
                .Select(customer => new { customer.CompanyName })
                .ToString();
            Assert.Contains("$select=company_name", selectUri);
        }

        [Fact]
        public void ResolverAppliesToClientModelAndReversePropertyLookup()
        {
            DataServiceContext context = CreateContext();
            IEdmStructuredType edmType = (IEdmStructuredType)context.Model.GetOrCreateEdmType(typeof(Customer));

            Assert.NotNull(edmType.FindProperty("company_name"));
            Assert.Null(edmType.FindProperty(nameof(Customer.CompanyName)));

            ClientTypeAnnotation typeAnnotation = context.Model.GetClientTypeAnnotation(edmType);
            ClientPropertyAnnotation property = typeAnnotation.GetProperty(
                "company_name",
                UndeclaredPropertyBehavior.ThrowException);

            Assert.Equal(nameof(Customer.CompanyName), property.PropertyInfo.Name);
            Assert.Equal("company_name", property.PropertyName);
        }

        [Fact]
        public void OriginalNameAttributeTakesPrecedenceOverResolver()
        {
            DataServiceContext context = CreateContext();
            IEdmStructuredType edmType = (IEdmStructuredType)context.Model.GetOrCreateEdmType(typeof(Customer));

            Assert.NotNull(edmType.FindProperty("display"));
            Assert.Null(edmType.FindProperty("display_name"));
        }

        [Fact]
        public void ResolverIsIsolatedFromContextsUsingDefaultNames()
        {
            DataServiceContext customContext = CreateContext();
            DataServiceContext defaultContext = new DataServiceContext(ServiceRoot);

            IEdmStructuredType customType = (IEdmStructuredType)customContext.Model.GetOrCreateEdmType(typeof(Customer));
            IEdmStructuredType defaultType = (IEdmStructuredType)defaultContext.Model.GetOrCreateEdmType(typeof(Customer));

            Assert.NotNull(customType.FindProperty("company_name"));
            Assert.Null(customType.FindProperty(nameof(Customer.CompanyName)));
            Assert.NotNull(defaultType.FindProperty(nameof(Customer.CompanyName)));
            Assert.Null(defaultType.FindProperty("company_name"));
        }

        [Fact]
        public void ResolverRejectsPropertyNameCollisions()
        {
            DataServiceContext context = new DataServiceContext(
                ServiceRoot,
                ODataProtocolVersion.V4,
                property => "duplicate");
            IEdmStructuredType edmType = (IEdmStructuredType)context.Model.GetOrCreateEdmType(typeof(Customer));

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => edmType.DeclaredProperties.ToArray());

            Assert.Contains("same server-defined name 'duplicate'", exception.Message);
        }

        [Fact]
        public void ResolverRejectsInvalidPropertyNames()
        {
            DataServiceContext context = new DataServiceContext(
                ServiceRoot,
                ODataProtocolVersion.V4,
                property => null);
            IEdmStructuredType edmType = (IEdmStructuredType)context.Model.GetOrCreateEdmType(typeof(Customer));

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => edmType.DeclaredProperties.ToArray());

            Assert.Contains("returned a null, empty, or whitespace name", exception.Message);
        }

        [Fact]
        public void ResolverAppliesWhenMaterializingResponseProperties()
        {
            DataServiceContext context = CreateContext();
            context.Format.UseJson(CreateServiceModel());
            context.MergeOption = MergeOption.NoTracking;
            context.ResolveName = type => $"NS.{type.Name}";
            string response = """
                {
                  "@odata.context":"https://service.test/$metadata#customers",
                  "value":[
                    {
                      "id":1,
                      "company_name":"Contoso",
                      "display":"Contoso Ltd."
                    }
                  ]
                }
                """;
            context.Configurations.RequestPipeline.OnMessageCreating = args =>
                new TestHttpWebRequestMessage(
                    args,
                    new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json;odata.metadata=minimal" },
                        { "OData-Version", "4.0" }
                    },
                    () => new MemoryStream(Encoding.UTF8.GetBytes(response)));

            Customer customer = context.CreateQuery<Customer>("customers").Execute().Single();

            Assert.Equal(1, customer.Id);
            Assert.Equal("Contoso", customer.CompanyName);
            Assert.Equal("Contoso Ltd.", customer.DisplayName);
        }

        [Fact]
        public void ResolverAppliesWhenSerializingRequestProperties()
        {
            DataServiceContext context = CreateContext();
            context.Format.UseJson(CreateServiceModel());
            context.ResolveName = type => $"NS.{type.Name}";
            context.AddAndUpdateResponsePreference = DataServiceResponsePreference.NoContent;
            string[] serializedPropertyNames = null;
            context.Configurations.RequestPipeline.OnEntryStarting(args =>
                serializedPropertyNames = args.Entry.Properties.Select(property => property.Name).ToArray());
            context.Configurations.RequestPipeline.OnMessageCreating = args =>
                new TestHttpWebRequestMessage(
                    args,
                    new Dictionary<string, string>
                    {
                        { "OData-Version", "4.0" }
                    },
                    statusCode: 204,
                    () => Stream.Null);

            context.AddObject(
                "customers",
                new Customer
                {
                    Id = 1,
                    CompanyName = "Contoso",
                    DisplayName = "Contoso Ltd."
                });
            context.SaveChanges();

            Assert.Equal(new[] { "company_name", "display", "id" }, serializedPropertyNames);
        }

        private static DataServiceContext CreateContext()
        {
            return new DataServiceContext(
                ServiceRoot,
                ODataProtocolVersion.V4,
                property => property.Name switch
                {
                    nameof(Customer.CompanyName) => "company_name",
                    nameof(Customer.DisplayName) => "display_name",
                    _ => property.Name.ToLowerInvariant()
                });
        }

        private static IEdmModel CreateServiceModel()
        {
            EdmModel model = new EdmModel();
            EdmEntityType customerType = new EdmEntityType("NS", nameof(Customer));
            IEdmStructuralProperty id = customerType.AddStructuralProperty("id", EdmPrimitiveTypeKind.Int32, false);
            customerType.AddKeys(id);
            customerType.AddStructuralProperty("company_name", EdmPrimitiveTypeKind.String);
            customerType.AddStructuralProperty("display", EdmPrimitiveTypeKind.String);
            model.AddElement(customerType);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Container");
            container.AddEntitySet("customers", customerType);
            model.AddElement(container);
            return model;
        }

        private sealed class Customer
        {
            public int Id { get; set; }

            public string CompanyName { get; set; }

            [OriginalName("display")]
            public string DisplayName { get; set; }
        }
    }
}
