//---------------------------------------------------------------------
// <copyright file="MetadataObjectModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Caching;
using Microsoft.OData.Service.Providers;
using Microsoft.OData.Service.Serializers;
using System.IO;
using System.Linq;
using System.Xml;
using AstoriaUnitTests.Tests.Server.Simulators;
using AstoriaUnitTests.TDD.Tests.Server.Simulators;
using Microsoft.OData.Client;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests.Server
{
    [TestClass]
    public class MetadataObjectModelTests
    {
        #region ResourceSetPathExpression Internals Tests

        [TestMethod]
        public void GetTargetSetTestsSetBindingTypeShouldPerformCorrectValidation()
        {
            ResourceType actorEntityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "foo", "Actor", false) { CanReflectOnInstanceType = false };
            ResourceProperty idProperty = new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false };
            actorEntityType.AddProperty(idProperty);

            ResourceType addressComplexType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "foo", "Address", false) { CanReflectOnInstanceType = false };
            addressComplexType.AddProperty(new ResourceProperty("StreetAddress", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))) { CanReflectOnInstanceTypeProperty = false });
            addressComplexType.AddProperty(new ResourceProperty("ZipCode", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false });

            actorEntityType.AddProperty(new ResourceProperty("PrimaryAddress", ResourcePropertyKind.ComplexType, addressComplexType) { CanReflectOnInstanceTypeProperty = false });
            actorEntityType.AddProperty(new ResourceProperty("OtherAddresses", ResourcePropertyKind.Collection, ResourceType.GetCollectionResourceType(addressComplexType)) { CanReflectOnInstanceTypeProperty = false });

            ResourceType movieEntityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "foo", "Movie", false) { CanReflectOnInstanceType = false };
            movieEntityType.AddProperty(idProperty);

            ResourceProperty moviesNavProp = new ResourceProperty("Movies", ResourcePropertyKind.ResourceSetReference, movieEntityType) { CanReflectOnInstanceTypeProperty = false };
            actorEntityType.AddProperty(moviesNavProp);
            ResourceProperty actorsNavProp = new ResourceProperty("Actors", ResourcePropertyKind.ResourceSetReference, actorEntityType) { CanReflectOnInstanceTypeProperty = false };
            movieEntityType.AddProperty(actorsNavProp);

            ResourceType derivedActorEntityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, actorEntityType, "foo", "DerivedActor", false) { CanReflectOnInstanceType = false };
            ResourceType derivedMovieEntityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, movieEntityType, "foo", "DerivedMovie", false) { CanReflectOnInstanceType = false };

            actorEntityType.SetReadOnly();
            derivedActorEntityType.SetReadOnly();
            movieEntityType.SetReadOnly();
            derivedMovieEntityType.SetReadOnly();
            addressComplexType.SetReadOnly();
            DataServiceProviderSimulator providerSimulator = new DataServiceProviderSimulator();
            providerSimulator.AddResourceType(actorEntityType);
            providerSimulator.AddResourceType(derivedActorEntityType);
            providerSimulator.AddResourceType(movieEntityType);
            providerSimulator.AddResourceType(derivedMovieEntityType);
            providerSimulator.AddResourceType(addressComplexType);

            ResourceSet actorSet = new ResourceSet("Actors", actorEntityType);
            ResourceSet movieSet = new ResourceSet("Movies", movieEntityType);
            actorSet.SetReadOnly();
            movieSet.SetReadOnly();
            providerSimulator.AddResourceSet(actorSet);
            providerSimulator.AddResourceSet(movieSet);

            providerSimulator.AddResourceAssociationSet(new ResourceAssociationSet("Actors_Movies", new ResourceAssociationSetEnd(actorSet, actorEntityType, moviesNavProp), new ResourceAssociationSetEnd(movieSet, movieEntityType, actorsNavProp)));

            DataServiceConfiguration config = new DataServiceConfiguration(providerSimulator);
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            var dataService = new DataServiceSimulator()
            {
                OperationContext = new DataServiceOperationContext(new DataServiceHostSimulator())
            };

            dataService.ProcessingPipeline = new DataServiceProcessingPipeline();
            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(dataService.Instance.GetType(), providerSimulator);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            DataServiceProviderWrapper provider = new DataServiceProviderWrapper(
                new DataServiceCacheItem(
                    config, 
                    staticConfiguration), 
                providerSimulator, 
                providerSimulator, 
                dataService,
                false);
            dataService.Provider = provider;
            provider.ProviderBehavior = providerBehavior;

            var testCases = new[]
            {
                new
                {
                    AppendParameterName = true,
                    Path = "",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    ErrorMessage = default(string)
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/Movies",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = ResourceSetWrapper.CreateResourceSetWrapper(movieSet, provider, set => set),
                    ErrorMessage = default(string)
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/Movies/Actors/Movies",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = ResourceSetWrapper.CreateResourceSetWrapper(movieSet, provider, set => set),
                    ErrorMessage = default(string)
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/foo.DerivedActor/Movies/foo.DerivedMovie/Actors/foo.DerivedActor/Movies",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = ResourceSetWrapper.CreateResourceSetWrapper(movieSet, provider, set => set),
                    ErrorMessage = default(string)
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression '{0}/' is not a valid expression because it contains an empty segment or it ends with '/'."
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/Movies/",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression '{0}/Movies/' is not a valid expression because it contains an empty segment or it ends with '/'."
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/Movies//Actors",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = ResourceSetWrapper.CreateResourceSetWrapper(movieSet, provider, set => set),
                    ErrorMessage = "The path expression '{0}/Movies//Actors' is not a valid expression because it contains an empty segment or it ends with '/'."
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/foo.DerivedActor",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression '{0}/foo.DerivedActor' is not a valid expression because it ends with the type identifier 'foo.DerivedActor'. A valid path expression must not end in a type identifier."
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/Movies/foo.DerivedMovie",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression '{0}/Movies/foo.DerivedMovie' is not a valid expression because it ends with the type identifier 'foo.DerivedMovie'. A valid path expression must not end in a type identifier."
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/Foo",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression '{0}/Foo' is not a valid expression because the segment 'Foo' is not a type identifier or a property on the resource type 'foo.Actor'."
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/ID",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression '{0}/ID' is not a valid expression because the segment 'ID' is a property of type 'Edm.Int32'. A valid path expression must only contain properties of entity type."
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/OtherAddresses",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression '{0}/OtherAddresses' is not a valid expression because the segment 'OtherAddresses' is a property of type 'Collection(foo.Address)'. A valid path expression must only contain properties of entity type."
                },
                new
                {
                    AppendParameterName = true,
                    Path = "/Movies/Actors/PrimaryAddress",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression '{0}/Movies/Actors/PrimaryAddress' is not a valid expression because the segment 'PrimaryAddress' is a property of type 'foo.Address'. A valid path expression must only contain properties of entity type."
                },
                new
                {
                    AppendParameterName = false,
                    Path = "foo",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression 'foo' is not a valid path expression. A valid path expression must start with the binding parameter name '{0}'."
                },
                new
                {
                    AppendParameterName = false,
                    Path = "abc/pqr",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression 'abc/pqr' is not a valid path expression. A valid path expression must start with the binding parameter name '{0}'."
                },
                new
                {
                    AppendParameterName = false,
                    Path = "actorParameter",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression 'actorParameter' is not a valid path expression. A valid path expression must start with the binding parameter name '{0}'."
                },
                new
                {
                    AppendParameterName = false,
                    Path = "actorsParameter",
                    BindingSet = ResourceSetWrapper.CreateResourceSetWrapper(actorSet, provider, set => set),
                    TargetSet = default(ResourceSetWrapper),
                    ErrorMessage = "The path expression 'actorsParameter' is not a valid path expression. A valid path expression must start with the binding parameter name '{0}'."
                },
            };

            ServiceActionParameter actorParameter = new ServiceActionParameter("actor", actorEntityType);
            ServiceActionParameter actorsParameter = new ServiceActionParameter("actors", ResourceType.GetEntityCollectionResourceType(actorEntityType));
            var parameters = new ServiceActionParameter[] { actorParameter, actorsParameter };

            foreach (var parameter in parameters)
            {
                foreach (var testCase in testCases)
                {
                    string pathString = testCase.AppendParameterName ? parameter.Name + testCase.Path : testCase.Path;
                    var path = new ResourceSetPathExpression(pathString);
                    Assert.AreEqual(pathString, path.PathExpression);
                    string expectedErrorMessage = testCase.ErrorMessage == null ? null : string.Format(testCase.ErrorMessage, parameter.Name);
                    try
                    {
                        path.SetBindingParameter(parameter);
                        path.InitializePathSegments(provider);
                        var targetSet = path.GetTargetSet(provider, testCase.BindingSet);
                        Assert.IsNull(expectedErrorMessage, "Expecting exception but received none.");
                        Assert.AreEqual(targetSet.Name, testCase.TargetSet.Name);
                    }
                    catch (InvalidOperationException e)
                    {
                        Assert.AreEqual(expectedErrorMessage, e.Message);
                    }
                }
            }
        }

        #endregion ResourceSetPathExpression Internals Tests
    }
}
