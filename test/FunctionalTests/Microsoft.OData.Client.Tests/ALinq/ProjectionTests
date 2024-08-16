//---------------------------------------------------------------------
// <copyright file="ProjectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Projection tests
    /// </summary>
    public class ProjectionTests
    {
        private readonly Container ctx;
        private readonly string serviceUri = "http://tempuri.org";

        public ProjectionTests()
        {
            ctx = new Container(new Uri(serviceUri));
        }

        [Fact]
        public void TestProjectionWithNullNestedResourceForQuerySyntaxExpression()
        {
            // Arrange
            InterceptRequestAndMockResponse("{\"@odata.context\":\"http://tempuri.org/$metadata#People(Id,Spouse())\",\"value\":[{\"Id\":2,\"Spouse\":null}]}");
            var query = from p in this.ctx.People
                        where p.Spouse == null
                        select new Person
                        {
                            Id = p.Id,
                            Spouse = p.Spouse
                        };
            var requestUri = query.ToString();

            // Act
            var result = query.ToList();

            // Assert
            Assert.Equal("http://tempuri.org/People?$filter=Spouse eq null&$expand=Spouse&$select=Id", requestUri);
            var person = Assert.Single(result);
            Assert.Equal(2, person.Id);
            Assert.Null(person.Name);
            Assert.Null(person.Spouse);
        }

        [Fact]
        public void TestProjectionWithNullNestedResourceForMethodSyntaxExpression()
        {
            // Arrange
            InterceptRequestAndMockResponse("{\"@odata.context\":\"http://tempuri.org/$metadata#People(Id,Spouse())\",\"value\":[{\"Id\":2,\"Spouse\":null}]}");
            var query = this.ctx.CreateQuery<Person>("People").Where(p1 => p1.Spouse == null).Select(p2 =>new Person
            {
                Id = p2.Id,
                Spouse = p2.Spouse
            });
            var requestUri = query.ToString();

            // Act
            var result = query.ToList();

            // Assert
            Assert.Equal("http://tempuri.org/People?$filter=Spouse eq null&$expand=Spouse&$select=Id", requestUri);
            var person = Assert.Single(result);
            Assert.Equal(2, person.Id);
            Assert.Null(person.Name);
            Assert.Null(person.Spouse);
        }

        [Fact]
        public void TestProjectionWithNullNestedResourceForAddQueryOption()
        {
            // Arrange
            InterceptRequestAndMockResponse("{\"@odata.context\":\"http://tempuri.org/$metadata#People(Id,Spouse())\",\"value\":[{\"Id\":2,\"Spouse\":null}]}");
            var query = ctx.People.AddQueryOption("$filter", "Spouse eq null").AddQueryOption("$expand", "Spouse").AddQueryOption("$select", "Id");
            var requestUri = query.ToString();

            // Act
            var result = query.ToList();

            // Assert
            Assert.Equal("http://tempuri.org/People?$expand=Spouse&$filter=Spouse eq null&$select=Id", requestUri);
            var person = Assert.Single(result);
            Assert.Equal(2, person.Id);
            Assert.Null(person.Name);
            Assert.Null(person.Spouse);
        }

        [Theory]
        [InlineData("http://tempuri.org/People?$expand=Spouse&$filter=Spouse eq null&$select=Id")]
        [InlineData("http://tempuri.org/People?$filter=Spouse eq null&$expand=Spouse&$select=Id")]
        public void TestProjectionWithNullNestedResourceForRawRequestUri(string requestUri)
        {
            // Arrange
            InterceptRequestAndMockResponse("{\"@odata.context\":\"http://tempuri.org/$metadata#People(Id,Spouse())\",\"value\":[{\"Id\":2,\"Spouse\":null}]}");
            var query = ctx.Execute<Person>(new Uri(requestUri));

            // Act
            var result = query.ToList();

            // Assert
            var person = Assert.Single(result);
            Assert.Equal(2, person.Id);
            Assert.Null(person.Name);
            Assert.Null(person.Spouse);
        }

        #region Helper Methods

        protected void InterceptRequestAndMockResponse(string mockResponse)
        {
            this.ctx.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var contentTypeHeader = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";
                var odataVersionHeader = "4.0";

                return new TestHttpWebRequestMessage(args,
                    new Dictionary<string, string>
                    {
                        {"Content-Type", contentTypeHeader},
                        {"OData-Version", odataVersionHeader},
                    },
                    () => new MemoryStream(Encoding.UTF8.GetBytes(mockResponse)));
            };
        }

        #endregion

        #region Types

        [Key("Id")]
        public class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public Person Spouse { get; set; }
        }

        public class Container : DataServiceContext
        {
            public Container(Uri serviceRoot) :
                    this(serviceRoot, ODataProtocolVersion.V4)
            {
            }

            public Container(Uri serviceRoot, ODataProtocolVersion protocolVersion) :
                    base(serviceRoot, protocolVersion)
            {
                this.ResolveName = ResolveName = (type) => $"NS.{type.Name}";
                this.ResolveType = ResolveType = (typeName) =>
                {
                    string namespaceName = typeof(Person).Namespace;
                    string unqualifiedTypeName = typeName.Substring(typeName.IndexOf('.') + 1);

                    Type type = null;

                    try
                    {
                        type = typeof(Person).GetAssembly().GetType($"{namespaceName}.{unqualifiedTypeName}");
                    }
                    catch
                    {
                    }

                    return type;
                };

                this.Format.UseJson(BuildEdmModel());
            }

            public virtual DataServiceQuery<Person> People
            {
                get
                {
                    if ((this._People == null))
                    {
                        this._People = base.CreateQuery<Person>("People");
                    }

                    return this._People;
                }
            }

            private DataServiceQuery<Person> _People;

            private static EdmModel BuildEdmModel()
            {
                var model = new EdmModel();

                var personEntity = new EdmEntityType("NS", "Person");
                personEntity.AddKeys(personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
                personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));

                personEntity.AddUnidirectionalNavigation(
                    new EdmNavigationPropertyInfo { Name = "Spouse", Target = personEntity, TargetMultiplicity = EdmMultiplicity.One });

                var entityContainer = new EdmEntityContainer("NS", "Container");

                model.AddElement(personEntity);
                model.AddElement(entityContainer);

                entityContainer.AddEntitySet("People", personEntity);

                return model;
            }
        }

        #endregion
    }
}
