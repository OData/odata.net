//---------------------------------------------------------------------
// <copyright file="UserDefinedServiceContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Vocabularies;

    public class UserDefinedServiceContainer : DataServiceContext
    {
        private IEdmModel edmModel;
        private Dictionary<string, string> operationMapping = new Dictionary<string, string>()
        {
            {"FunctionWithoutParameter", "FunctionWithoutParameter"},
            {"GetAllCTsOfETSets","FunctionBoundToCollectionOfEntity"},
            {"ActionWithoutReturnType","ActionWithoutReturnType"},
            {"UnboundFunctionWithParameter", "UserDefinedServiceContainer.FunctionImportWithParameter"},
            {"UnboundActionWithParameter", "UserDefinedServiceContainer.ActionImportWithParameter"}
        };
        
        private const string serviceNamespacePrefix = "Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.";

        private const string edmModelString = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Term Name=""RecordAnnotation"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.RecordAnnotationType"" />
      <Term Name=""NavOfDerivedETAnnotation"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedET)"" />
      <Annotations Target=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET"" Qualifier=""ExternalTargeting"">
        <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.CollectionOfCTPropertyAnnotation"">
          <Collection>
            <Path>ComplexP</Path>
          </Collection>
        </Annotation>
      </Annotations>
      <Annotations Target=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET/ComplexP"" Qualifier=""ExternalTargeting"">
        <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.CollectionOfDerviedCTAnnotation"">
          <Collection>
            <Path>ComplexP/Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedCT</Path>
          </Collection>
        </Annotation>
      </Annotations>
      <Annotations Target=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.UserDefinedServiceContainer/FunctionImportWithParameter"" Qualifier=""ExternalTargeting"">
        <Annotation Term=""Org.OData.Core.V1.Description"" String=""TestExternalTargetingAnnotationOnFunctionImport"" />
      </Annotations>
      <Annotations Target=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.UserDefinedServiceContainer/ActionImportWithParameter"" Qualifier=""ExternalTargeting"">
        <Annotation Term=""Org.OData.Core.V1.Description"" String=""TestExternalTargetingAnnotationOnActionImport"" />
      </Annotations>
      <Annotations Target=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.UserDefinedServiceContainer/DerivedETSets"" Qualifier=""ExternalTargeting"">
        <Annotation Term=""Org.OData.Core.V1.Description"" String=""TestExternalTargetingAnnotationOnDerivedEntitySets"" />
      </Annotations>
      <ComplexType Name=""RecordAnnotationType"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""OtherProperties"" Type=""Collection(Edm.String)"" />
        <Property Name=""CollectionOfCTP"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.CT)"" />
        <Property Name=""CollectionOfDerivedCTP"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedCT)"" />
      </ComplexType>
      <EntityType Name=""ET"">
        <Key>
          <PropertyRef Name=""UserName"" />
        </Key>
        <Property Name=""UserName"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""ComplexP"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.CT"">
          <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.RecordAnnotation"">
            <Record>
              <PropertyValue Property=""Name"" Path=""UserName"" />
              <PropertyValue Property=""OtherProperties"">
                <Collection />
              </PropertyValue>
              <PropertyValue Property=""CollectionOfCTP"">
                <Collection>
                  <Path>ComplexP</Path>
                </Collection>
              </PropertyValue>
              <PropertyValue Property=""CollectionOfDerivedCTP"">
                <Collection>
                  <Path>ComplexP/Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedCT</Path>
                </Collection>
              </PropertyValue>
            </Record>
          </Annotation>
        </Property>
        <NavigationProperty Name=""NavP"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET)"">
          <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.NavOfDerivedETAnnotation"">
            <Path>NavP/Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedET</Path>
          </Annotation>
        </NavigationProperty>
        <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.RecordAnnotation"">
          <Record>
            <PropertyValue Property=""Name"" Path=""UserName"" />
            <PropertyValue Property=""OtherProperties"">
              <Collection>
                <Path>ComplexP/Name</Path>
              </Collection>
            </PropertyValue>
            <PropertyValue Property=""CollectionOfCTP"">
              <Collection />
            </PropertyValue>
            <PropertyValue Property=""CollectionOfDerivedCTP"">
              <Collection>
                <Path>ComplexP/Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedCT</Path>
              </Collection>
            </PropertyValue>
          </Record>
        </Annotation>
        <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.NavOfDerivedETAnnotation"" Qualifier=""EntityType"">
          <Path>NavP@Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.NavOfDerivedETAnnotation</Path>
        </Annotation>
      </EntityType>
      <EntityType Name=""DerivedET"" BaseType=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET"">
        <Property Name=""DerivedComplexP"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedCT"">
          <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.RecordAnnotation"">
            <Record>
              <PropertyValue Property=""Name"" Path=""UserName"" />
              <PropertyValue Property=""OtherProperties"">
                <Collection>
                  <Path>ComplexP/Name</Path>
                </Collection>
              </PropertyValue>
              <PropertyValue Property=""CollectionOfCTP"">
                <Collection>
                  <Path>ComplexP</Path>
                </Collection>
              </PropertyValue>
              <PropertyValue Property=""CollectionOfDerivedCTP"">
                <Collection>
                  <Path>ComplexP/Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedCT</Path>
                </Collection>
              </PropertyValue>
            </Record>
          </Annotation>
        </Property>
        <NavigationProperty Name=""SingleNavP"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET"">
          <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.NavOfDerivedETAnnotation"">
            <Collection>
              <Path>SingleNavP/Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedET</Path>
            </Collection>
          </Annotation>
        </NavigationProperty>
        <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.RecordAnnotation"">
          <Record>
            <PropertyValue Property=""Name"" Path=""UserName"" />
            <PropertyValue Property=""OtherProperties"">
              <Collection>
                <Path>ComplexP/Name</Path>
                <Path>DerivedComplexP/Name</Path>
                <Path>DerivedComplexP/Description</Path>
              </Collection>
            </PropertyValue>
            <PropertyValue Property=""CollectionOfCTP"">
              <Collection>
                <Path>ComplexP</Path>
                <Path>DerivedComplexP</Path>
              </Collection>
            </PropertyValue>
            <PropertyValue Property=""CollectionOfDerivedCTP"">
              <Collection>
                <Path>DerivedComplexP</Path>
              </Collection>
            </PropertyValue>
          </Record>
        </Annotation>
      </EntityType>
      <ComplexType Name=""CT"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
      </ComplexType>
      <ComplexType Name=""DerivedCT"" BaseType=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.CT"">
        <Property Name=""Description"" Type=""Edm.String"" Nullable=""false"" />
        <Annotation Term=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.RecordAnnotation"">
          <Record />
        </Annotation>
      </ComplexType>
      <Function Name=""FunctionWithoutParameter"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""entity"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET"" Nullable=""false"" />
        <ReturnType Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.CT"" Nullable=""false"" />
        <Annotation Term=""Org.OData.Core.V1.Description"" String=""FunctionWithoutParameter"" />
      </Function>
      <Function Name=""FunctionBoundToCollectionOfEntity"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""entity"" Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET)"" Nullable=""false"" />
        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />
        <ReturnType Type=""Collection(Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET)"" Nullable=""false"" />
        <Annotation Term=""Org.OData.Core.V1.Description"" String=""FunctionBoundToCollectionOfEntity"" />
      </Function>
      <Action Name=""ActionWithoutReturnType"" IsBound=""true"">
        <Parameter Name=""entity"" Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET"" Nullable=""false"" />
        <Annotation Term=""Org.OData.Core.V1.Description"" String=""ActionWithoutReturnType"" />
      </Action>
      <Function Name=""UnboundFunctionWithParameter"" IsComposable=""true"">
        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />
        <ReturnType Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.CT"" Nullable=""false"" />
        <Annotation Term=""Org.OData.Core.V1.Description"" String=""UnboundFunctionWithOneParameter"" />
      </Function>
      <Action Name=""UnboundActionWithParameter"">
        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />
        <ReturnType Type=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.CT"" Nullable=""false"" />
        <Annotation Term=""Org.OData.Core.V1.Description"" String=""UnboundActionWithOneParameter"" />
      </Action>
      <EntityContainer Name=""UserDefinedServiceContainer"">
        <EntitySet Name=""ETSets"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ET"">
          <NavigationPropertyBinding Path=""NavP"" Target=""ETSets"" />
        </EntitySet>
        <EntitySet Name=""DerivedETSets"" EntityType=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.DerivedET"">
          <NavigationPropertyBinding Path=""SingleNavP"" Target=""SingleET"" />
        </EntitySet>
        <FunctionImport Name=""FunctionImportWithParameter"" Function=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.UnboundFunctionWithParameter"" IncludeInServiceDocument=""true"">
          <Annotation Term=""Org.OData.Core.V1.Description"" String=""FunctionImportWithParameter"" />
        </FunctionImport>
        <ActionImport Name=""ActionImportWithParameter"" Action=""Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.UnboundActionWithParameter"">
          <Annotation Term=""Org.OData.Core.V1.Description"" String=""ActionImportWithParameter"" />
        </ActionImport>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        public UserDefinedServiceContainer(Uri serviceRoot)
            : base(serviceRoot)
        {
            XmlReader reader = XmlReader.Create(new StringReader(edmModelString));
            edmModel = CsdlReader.Parse(reader);
            this.Format.LoadServiceModel = () => edmModel;
            this.Format.UseJson();
        }

        public IQueryable<ET> ETSets
        {
            get { return CreateQuery<ET>("ETSets"); }
        }

        public IQueryable<DerivedET> DerivedETSets
        {
            get { return CreateQuery<DerivedET>("DerivedETSets"); }
        }

        public IQueryable<DerivedET> NonExistingETSets
        {
            get { return CreateQuery<DerivedET>("NonExistingETSets"); }
        }

        internal override IEdmVocabularyAnnotatable GetEdmOperationOrOperationImport(System.Reflection.MethodInfo methodInfo)
        {
            var operationImport = edmModel.FindDeclaredOperationImports(
                serviceNamespacePrefix + (operationMapping.ContainsKey(methodInfo.Name) ? operationMapping[methodInfo.Name] : methodInfo.Name))
                .SingleOrDefault();

            if (operationImport != null)
            {
                return operationImport;
            }

            return edmModel.FindDeclaredOperations(
                serviceNamespacePrefix + (operationMapping.ContainsKey(methodInfo.Name) ? operationMapping[methodInfo.Name] : methodInfo.Name))
                .SingleOrDefault();
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        public QueryOperationResponse<CT> FunctionWithoutParameter(ET entity)
        {
            return this.Execute(
                new Uri(this.BaseUri, string.Format("ETSets('{0}')/{1}", entity.UserName, "Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.FunctionWithoutParameter")),
                "GET",
                new UriOperationParameter("", entity)) as QueryOperationResponse<CT>;
        }

        public QueryOperationResponse<CT> GetAllCTsOfETSets(ICollection<ET> entity)
        {
            return this.Execute(
                new Uri(this.BaseUri, string.Format("ETSets/{0}", "Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.FunctionWithoutParameter")),
                "GET",
                new UriOperationParameter("", entity)) as QueryOperationResponse<CT>;
        }

        public OperationResponse ActionWithoutReturnType(ET entity)
        {
            return this.Execute(
                new Uri(this.BaseUri, string.Format("ETSets('{0}')/{1}", entity.UserName, "Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.ActionWithoutReturnType")),
                "POST",
                new BodyOperationParameter("", entity));
        }

        public QueryOperationResponse<CT> UnboundFunctionWithParameter(ET entity)
        {
            return this.Execute(
                new Uri(this.BaseUri, "UnboundFunctionWithParameter"),
                "GET",
                new UriOperationParameter("", entity)) as QueryOperationResponse<CT>;
        }

        public OperationResponse UnboundActionWithParameter(ET entity)
        {
            return this.Execute(
                new Uri(this.BaseUri, "UnboundActionWithParameter"),
                "POST",
                new BodyOperationParameter("", entity));
        }
#endif
    }

    [Key("UserName")]
    public class ET
    {
        public string UserName { get; set; }

        public CT ComplexP { get; set; }

        public ICollection<ET> NavP { get; set; }
    }

    public class DerivedET : ET
    {
        public DerivedCT DerivedComplexP { get; set; }
        public ET SingleNavP { get; set; }
    }

    public class CT
    {
        public string Name { get; set; }
    }

    public class DerivedCT : CT
    {
        public string Description { get; set; }
    }

    public class RecordAnnotationType
    {
        public string Name { get; set; }

        public ICollection<string> OtherProperties { get; set; }

        public ICollection<CT> CollectionOfCTP { get; set; }

        public ICollection<DerivedCT> CollectionOfDerivedCTP { get; set; }
    }
}
