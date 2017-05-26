//---------------------------------------------------------------------
// <copyright file="WritePayloadHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.WriteJsonPayloadTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Tests.Client.Common;

    /// <summary>
    /// Some helper methods to create various ODataResourceSet/Entry/values.
    /// </summary>
    public static partial class WritePayloadHelper
    {
        public const string NameSpace = "Microsoft.Test.OData.Services.AstoriaDefaultService.";
        public static Uri ServiceUri = null;
        public static IEdmModel Model { get; set; }
        public static IEdmEntitySet CustomerSet { get; set; }
        public static IEdmEntitySet OrderSet { get; set; }
        public static IEdmEntitySet CarSet { get; set; }
        public static IEdmEntitySet PersonSet { get; set; }
        public static IEdmEntitySet LoginSet { get; set; }

        public static IEdmEntityType CustomerType { get; set; }
        public static IEdmEntityType OrderType { get; set; }
        public static IEdmEntityType CarType { get; set; }
        public static IEdmEntityType PersonType { get; set; }
        public static IEdmEntityType EmployeeType { get; set; }
        public static IEdmEntityType SpecialEmployeeType { get; set; }
        public static IEdmEntityType LoginType { get; set; }

        public static IEdmComplexType ContactDetailType { get; set; }

        public static void CustomTestInitialize(Uri serviceUri)
        {
            ServiceUri = serviceUri;

            var metadata = @"<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
    <edmx:DataServices xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" m:DataServiceVersion=""4.0"" m:MaxDataServiceVersion=""4.0"">
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Microsoft.Test.OData.Services.AstoriaDefaultService"">
            <EntityType Name=""AllSpatialTypes"">
                <Key>
                    <PropertyRef Name=""Id"" />
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Geog"" Type=""Edm.Geography"" SRID=""Variable"" />
                <Property Name=""GeogPoint"" Type=""Edm.GeographyPoint"" SRID=""Variable"" />
                <Property Name=""GeogLine"" Type=""Edm.GeographyLineString"" SRID=""Variable"" />
                <Property Name=""GeogPolygon"" Type=""Edm.GeographyPolygon"" SRID=""Variable"" />
                <Property Name=""GeogCollection"" Type=""Edm.GeographyCollection"" SRID=""Variable"" />
                <Property Name=""GeogMultiPoint"" Type=""Edm.GeographyMultiPoint"" SRID=""Variable"" />
                <Property Name=""GeogMultiLine"" Type=""Edm.GeographyMultiLineString"" SRID=""Variable"" />
                <Property Name=""GeogMultiPolygon"" Type=""Edm.GeographyMultiPolygon"" SRID=""Variable"" />
                <Property Name=""Geom"" Type=""Edm.Geometry"" SRID=""Variable"" />
                <Property Name=""GeomPoint"" Type=""Edm.GeometryPoint"" SRID=""Variable"" />
                <Property Name=""GeomLine"" Type=""Edm.GeometryLineString"" SRID=""Variable"" />
                <Property Name=""GeomPolygon"" Type=""Edm.GeometryPolygon"" SRID=""Variable"" />
                <Property Name=""GeomCollection"" Type=""Edm.GeometryCollection"" SRID=""Variable"" />
                <Property Name=""GeomMultiPoint"" Type=""Edm.GeometryMultiPoint"" SRID=""Variable"" />
                <Property Name=""GeomMultiLine"" Type=""Edm.GeometryMultiLineString"" SRID=""Variable"" />
                <Property Name=""GeomMultiPolygon"" Type=""Edm.GeometryMultiPolygon"" SRID=""Variable"" />
            </EntityType>
            <EntityType Name=""AllSpatialCollectionTypes"" Abstract=""true"">
                <Key>
                    <PropertyRef Name=""Id"" />
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
            </EntityType>
            <EntityType Name=""Customer"">
                <Key>
                    <PropertyRef Name=""CustomerId"" />
                </Key>
                <Property Name=""Thumbnail"" Type=""Edm.Stream"" Nullable=""false"" />
                <Property Name=""Video"" Type=""Edm.Stream"" Nullable=""false"" />
                <Property Name=""CustomerId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Name"" Type=""Edm.String"" />
                <Property Name=""PrimaryContactInfo"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.ContactDetails"" />
                <Property Name=""BackupContactInfo"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.ContactDetails)"" Nullable=""false"" />
                <Property Name=""Auditing"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.AuditInfo"" />
                <NavigationProperty Name=""Orders"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Order)"" Partner=""Customer"" />
                <NavigationProperty Name=""Logins"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Login)"" Partner=""Customer"" />
                <NavigationProperty Name=""Husband"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Customer"" Partner=""Husband"" />
                <NavigationProperty Name=""Wife"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Customer"" Partner=""Wife"" />
                <NavigationProperty Name=""Info"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.CustomerInfo"" />
            </EntityType>
            <EntityType Name=""Login"">
                <Key>
                    <PropertyRef Name=""Username"" />
                </Key>
                <Property Name=""Username"" Type=""Edm.String"" Nullable=""false"" />
                <Property Name=""CustomerId"" Type=""Edm.Int32"" Nullable=""false"" />
                <NavigationProperty Name=""Customer"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Customer"" Partner=""Logins"" />
                <NavigationProperty Name=""LastLogin"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.LastLogin"" Partner=""Login"" />
                <NavigationProperty Name=""SentMessages"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Message)"" Partner=""Sender"" />
                <NavigationProperty Name=""ReceivedMessages"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Message)"" Partner=""Recipient"" />
                <NavigationProperty Name=""Orders"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Order)"" Partner=""Login"" />
            </EntityType>
            <EntityType Name=""RSAToken"">
                <Key>
                    <PropertyRef Name=""Serial"" />
                </Key>
                <Property Name=""Serial"" Type=""Edm.String"" Nullable=""false"" />
                <Property Name=""Issued"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <NavigationProperty Name=""Login"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Login"" />
            </EntityType>
            <EntityType Name=""PageView"">
                <Key>
                    <PropertyRef Name=""PageViewId"" />
                </Key>
                <Property Name=""PageViewId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Username"" Type=""Edm.String"" />
                <Property Name=""Viewed"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <Property Name=""TimeSpentOnPage"" Type=""Edm.Duration"" Nullable=""false"" />
                <Property Name=""PageUrl"" Type=""Edm.String"" />
                <NavigationProperty Name=""Login"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Login"" />
            </EntityType>
            <EntityType Name=""LastLogin"">
                <Key>
                    <PropertyRef Name=""Username"" />
                </Key>
                <Property Name=""Username"" Type=""Edm.String"" Nullable=""false"" />
                <Property Name=""LoggedIn"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <Property Name=""LoggedOut"" Type=""Edm.DateTimeOffset"" />
                <Property Name=""Duration"" Type=""Edm.Duration"" Nullable=""false"" />
                <NavigationProperty Name=""Login"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Login"" Partner=""LastLogin"" />
            </EntityType>
            <EntityType Name=""Message"">
                <Key>
                    <PropertyRef Name=""FromUsername"" />
                    <PropertyRef Name=""MessageId"" />
                </Key>
                <Property Name=""MessageId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""FromUsername"" Type=""Edm.String"" Nullable=""false"" />
                <Property Name=""ToUsername"" Type=""Edm.String"" />
                <Property Name=""Sent"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <Property Name=""Subject"" Type=""Edm.String"" />
                <Property Name=""Body"" Type=""Edm.String"" />
                <Property Name=""IsRead"" Type=""Edm.Boolean"" Nullable=""false"" />
                <NavigationProperty Name=""Sender"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Login"" Partner=""SentMessages"" />
                <NavigationProperty Name=""Recipient"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Login"" Partner=""ReceivedMessages"" />
                <NavigationProperty Name=""Attachments"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.MessageAttachment)"" />
            </EntityType>
            <EntityType Name=""MessageAttachment"">
                <Key>
                    <PropertyRef Name=""AttachmentId"" />
                </Key>
                <Property Name=""AttachmentId"" Type=""Edm.Guid"" Nullable=""false"" />
                <Property Name=""Attachment"" Type=""Edm.Binary"" />
            </EntityType>
            <EntityType Name=""Order"">
                <Key>
                    <PropertyRef Name=""OrderId"" />
                </Key>
                <Property Name=""OrderId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""CustomerId"" Type=""Edm.Int32"" />
                <Property Name=""Concurrency"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.ConcurrencyInfo"" />
                <NavigationProperty Name=""Login"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Login"" Partner=""Orders"" />
                <NavigationProperty Name=""Customer"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Customer"" Partner=""Orders"" />
            </EntityType>
            <EntityType Name=""OrderLine"">
                <Key>
                    <PropertyRef Name=""OrderId"" />
                    <PropertyRef Name=""ProductId"" />
                </Key>
                <Property Name=""OrderLineStream"" Type=""Edm.Stream"" Nullable=""false"" />
                <Property Name=""OrderId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""ProductId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Quantity"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""ConcurrencyToken"" Type=""Edm.String"" />
                <NavigationProperty Name=""Order"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Order"" />
                <NavigationProperty Name=""Product"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Product"" />
            </EntityType>
            <EntityType Name=""Product"">
                <Key>
                    <PropertyRef Name=""ProductId"" />
                </Key>
                <Property Name=""Picture"" Type=""Edm.Stream"" Nullable=""false"" />
                <Property Name=""ProductId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Description"" Type=""Edm.String"" />
                <Property Name=""Dimensions"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Dimensions"" />
                <Property Name=""BaseConcurrency"" Type=""Edm.String"" />
                <Property Name=""ComplexConcurrency"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.ConcurrencyInfo"" />
                <Property Name=""NestedComplexConcurrency"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.AuditInfo"" />
                <NavigationProperty Name=""RelatedProducts"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Product)"" Partner=""RelatedProducts"" />
                <NavigationProperty Name=""Detail"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.ProductDetail"" Partner=""Product"" />
                <NavigationProperty Name=""Reviews"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.ProductReview)"" Partner=""Product"" />
                <NavigationProperty Name=""Photos"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.ProductPhoto)"" />
            </EntityType>
            <EntityType Name=""ProductDetail"">
                <Key>
                    <PropertyRef Name=""ProductId"" />
                </Key>
                <Property Name=""ProductId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Details"" Type=""Edm.String"" />
                <NavigationProperty Name=""Product"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Product"" Partner=""Detail"" />
            </EntityType>
            <EntityType Name=""ProductReview"">
                <Key>
                    <PropertyRef Name=""ProductId"" />
                    <PropertyRef Name=""ReviewId"" />
                    <PropertyRef Name=""RevisionId"" />
                </Key>
                <Property Name=""ProductId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""ReviewId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Review"" Type=""Edm.String"" />
                <Property Name=""RevisionId"" Type=""Edm.String"" Nullable=""false"" />
                <NavigationProperty Name=""Product"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Product"" Partner=""Reviews"" />
            </EntityType>
            <EntityType Name=""ProductPhoto"">
                <Key>
                    <PropertyRef Name=""PhotoId"" />
                    <PropertyRef Name=""ProductId"" />
                </Key>
                <Property Name=""ProductId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""PhotoId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Photo"" Type=""Edm.Binary"" />
            </EntityType>
            <EntityType Name=""CustomerInfo"" HasStream=""true"">
                <Key>
                    <PropertyRef Name=""CustomerInfoId"" />
                </Key>
                <Property Name=""CustomerInfoId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Information"" Type=""Edm.String"" />
            </EntityType>
            <EntityType Name=""Computer"">
                <Key>
                    <PropertyRef Name=""ComputerId"" />
                </Key>
                <Property Name=""ComputerId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Name"" Type=""Edm.String"" />
                <NavigationProperty Name=""ComputerDetail"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.ComputerDetail"" Partner=""Computer"" />
            </EntityType>
            <EntityType Name=""ComputerDetail"">
                <Key>
                    <PropertyRef Name=""ComputerDetailId"" />
                </Key>
                <Property Name=""ComputerDetailId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Manufacturer"" Type=""Edm.String"" />
                <Property Name=""Model"" Type=""Edm.String"" />
                <Property Name=""Serial"" Type=""Edm.String"" />
                <Property Name=""SpecificationsBag"" Type=""Collection(Edm.String)"" Nullable=""false"" />
                <Property Name=""PurchaseDate"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <Property Name=""Dimensions"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Dimensions"" />
                <NavigationProperty Name=""Computer"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Computer"" Partner=""ComputerDetail"" />
            </EntityType>
            <EntityType Name=""Driver"">
                <Key>
                    <PropertyRef Name=""Name"" />
                </Key>
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
                <Property Name=""BirthDate"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <NavigationProperty Name=""License"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.License"" Partner=""Driver"" />
            </EntityType>
            <EntityType Name=""License"">
                <Key>
                    <PropertyRef Name=""Name"" />
                </Key>
                <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
                <Property Name=""LicenseNumber"" Type=""Edm.String"" />
                <Property Name=""LicenseClass"" Type=""Edm.String"" />
                <Property Name=""Restrictions"" Type=""Edm.String"" />
                <Property Name=""ExpirationDate"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <NavigationProperty Name=""Driver"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Driver"" Partner=""License"" />
            </EntityType>
            <EntityType Name=""MappedEntityType"">
                <Key>
                    <PropertyRef Name=""Id"" />
                </Key>
                <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Href"" Type=""Edm.String"" />
                <Property Name=""Title"" Type=""Edm.String"" />
                <Property Name=""HrefLang"" Type=""Edm.String"" />
                <Property Name=""Type"" Type=""Edm.String"" />
                <Property Name=""Length"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""BagOfPrimitiveToLinks"" Type=""Collection(Edm.String)"" Nullable=""false"" />
                <Property Name=""Logo"" Type=""Edm.Binary"" />
                <Property Name=""BagOfDecimals"" Type=""Collection(Edm.Decimal)"" Nullable=""false"" />
                <Property Name=""BagOfDoubles"" Type=""Collection(Edm.Double)"" Nullable=""false"" />
                <Property Name=""BagOfSingles"" Type=""Collection(Edm.Single)"" Nullable=""false"" />
                <Property Name=""BagOfBytes"" Type=""Collection(Edm.Byte)"" Nullable=""false"" />
                <Property Name=""BagOfInt16s"" Type=""Collection(Edm.Int16)"" Nullable=""false"" />
                <Property Name=""BagOfInt32s"" Type=""Collection(Edm.Int32)"" Nullable=""false"" />
                <Property Name=""BagOfInt64s"" Type=""Collection(Edm.Int64)"" Nullable=""false"" />
                <Property Name=""BagOfGuids"" Type=""Collection(Edm.Guid)"" Nullable=""false"" />
                <Property Name=""BagOfDateTimeOffset"" Type=""Collection(Edm.DateTimeOffset)"" Nullable=""false"" />
                <Property Name=""BagOfComplexToCategories"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.ComplexToCategory)"" Nullable=""false"" />
                <Property Name=""ComplexPhone"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Phone"" />
                <Property Name=""ComplexContactDetails"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.ContactDetails"" />
            </EntityType>
            <EntityType Name=""Car"" HasStream=""true"">
                <Key>
                    <PropertyRef Name=""VIN"" />
                </Key>
                <Property Name=""Photo"" Type=""Edm.Stream"" Nullable=""false"" />
                <Property Name=""Video"" Type=""Edm.Stream"" Nullable=""false"" />
                <Property Name=""VIN"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Description"" Type=""Edm.String"" />
            </EntityType>
            <EntityType Name=""Person"">
                <Key>
                    <PropertyRef Name=""PersonId"" />
                </Key>
                <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Name"" Type=""Edm.String"" />
                <NavigationProperty Name=""PersonMetadata"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.PersonMetadata)"" Partner=""Person"" />
            </EntityType>
            <EntityType Name=""PersonMetadata"">
                <Key>
                    <PropertyRef Name=""PersonMetadataId"" />
                </Key>
                <Property Name=""PersonMetadataId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""PropertyName"" Type=""Edm.String"" />
                <Property Name=""PropertyValue"" Type=""Edm.String"" />
                <NavigationProperty Name=""Person"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Person"" Partner=""PersonMetadata"" />
            </EntityType>
            <ComplexType Name=""ContactDetails"">
                <Property Name=""EmailBag"" Type=""Collection(Edm.String)"" Nullable=""false"" />
                <Property Name=""AlternativeNames"" Type=""Collection(Edm.String)"" Nullable=""false"" />
                <Property Name=""ContactAlias"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Aliases"" />
                <Property Name=""HomePhone"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Phone"" />
                <Property Name=""WorkPhone"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Phone"" />
                <Property Name=""MobilePhoneBag"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Phone)"" Nullable=""false"" />
            </ComplexType>
            <ComplexType Name=""AuditInfo"">
                <Property Name=""ModifiedDate"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <Property Name=""ModifiedBy"" Type=""Edm.String"" />
                <Property Name=""Concurrency"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.ConcurrencyInfo"" />
            </ComplexType>
            <ComplexType Name=""ConcurrencyInfo"">
                <Property Name=""Token"" Type=""Edm.String"" />
                <Property Name=""QueriedDateTime"" Type=""Edm.DateTimeOffset"" />
            </ComplexType>
            <ComplexType Name=""Dimensions"">
                <Property Name=""Width"" Type=""Edm.Decimal"" Nullable=""false"" />
                <Property Name=""Height"" Type=""Edm.Decimal"" Nullable=""false"" />
                <Property Name=""Depth"" Type=""Edm.Decimal"" Nullable=""false"" />
            </ComplexType>
            <ComplexType Name=""ComplexToCategory"">
                <Property Name=""Term"" Type=""Edm.String"" />
                <Property Name=""Scheme"" Type=""Edm.String"" />
                <Property Name=""Label"" Type=""Edm.String"" />
            </ComplexType>
            <ComplexType Name=""Phone"">
                <Property Name=""PhoneNumber"" Type=""Edm.String"" />
                <Property Name=""Extension"" Type=""Edm.String"" />
            </ComplexType>
            <ComplexType Name=""Aliases"">
                <Property Name=""AlternativeNames"" Type=""Collection(Edm.String)"" Nullable=""false"" />
            </ComplexType>
            <EntityType Name=""AllSpatialCollectionTypes_Simple"" BaseType=""Microsoft.Test.OData.Services.AstoriaDefaultService.AllSpatialCollectionTypes"">
                <Property Name=""ManyGeogPoint"" Type=""Collection(Edm.GeographyPoint)"" Nullable=""false"" SRID=""Variable"" />
                <Property Name=""ManyGeogLine"" Type=""Collection(Edm.GeographyLineString)"" Nullable=""false"" SRID=""Variable"" />
                <Property Name=""ManyGeogPolygon"" Type=""Collection(Edm.GeographyPolygon)"" Nullable=""false"" SRID=""Variable"" />
                <Property Name=""ManyGeomPoint"" Type=""Collection(Edm.GeometryPoint)"" Nullable=""false"" SRID=""Variable"" />
                <Property Name=""ManyGeomLine"" Type=""Collection(Edm.GeometryLineString)"" Nullable=""false"" SRID=""Variable"" />
                <Property Name=""ManyGeomPolygon"" Type=""Collection(Edm.GeometryPolygon)"" Nullable=""false"" SRID=""Variable"" />
            </EntityType>
            <EntityType Name=""ProductPageView"" BaseType=""Microsoft.Test.OData.Services.AstoriaDefaultService.PageView"">
                <Property Name=""ProductId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""ConcurrencyToken"" Type=""Edm.String"" />
            </EntityType>
            <EntityType Name=""BackOrderLine"" BaseType=""Microsoft.Test.OData.Services.AstoriaDefaultService.OrderLine"" />
            <EntityType Name=""BackOrderLine2"" BaseType=""Microsoft.Test.OData.Services.AstoriaDefaultService.BackOrderLine"" />
            <EntityType Name=""DiscontinuedProduct"" BaseType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Product"">
                <Property Name=""Discontinued"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <Property Name=""ReplacementProductId"" Type=""Edm.Int32"" />
                <Property Name=""DiscontinuedPhone"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Phone"" />
                <Property Name=""ChildConcurrencyToken"" Type=""Edm.String"" />
            </EntityType>
            <EntityType Name=""Contractor"" BaseType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Person"">
                <Property Name=""ContratorCompanyId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""BillingRate"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""TeamContactPersonId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""JobDescription"" Type=""Edm.String"" />
            </EntityType>
            <EntityType Name=""Employee"" BaseType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Person"">
                <Property Name=""ManagersPersonId"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Salary"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Title"" Type=""Edm.String"" />
                <NavigationProperty Name=""Manager"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Employee"" Partner=""Manager"" />
            </EntityType>
            <EntityType Name=""SpecialEmployee"" BaseType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Employee"">
                <Property Name=""CarsVIN"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Bonus"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""IsFullyVested"" Type=""Edm.Boolean"" Nullable=""false"" />
                <NavigationProperty Name=""Car"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Car"" />
            </EntityType>
            <ComplexType Name=""ComplexWithAllPrimitiveTypes"">
                <Property Name=""Binary"" Type=""Edm.Binary"" />
                <Property Name=""Boolean"" Type=""Edm.Boolean"" Nullable=""false"" />
                <Property Name=""Byte"" Type=""Edm.Byte"" Nullable=""false"" />
                <Property Name=""DateTimeOffset"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
                <Property Name=""Decimal"" Type=""Edm.Decimal"" Nullable=""false"" />
                <Property Name=""Double"" Type=""Edm.Double"" Nullable=""false"" />
                <Property Name=""Int16"" Type=""Edm.Int16"" Nullable=""false"" />
                <Property Name=""Int32"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Int64"" Type=""Edm.Int64"" Nullable=""false"" />
                <Property Name=""SByte"" Type=""Edm.SByte"" Nullable=""false"" />
                <Property Name=""String"" Type=""Edm.String"" />
                <Property Name=""Single"" Type=""Edm.Single"" Nullable=""false"" />
                <Property Name=""GeographyPoint"" Type=""Edm.GeographyPoint"" SRID=""Variable"" />
                <Property Name=""GeometryPoint"" Type=""Edm.GeometryPoint"" SRID=""Variable"" />
            </ComplexType>
            <Function Name=""GetPrimitiveString"" m:HttpMethod=""GET"">
                <ReturnType Type=""Edm.String"" />
            </Function>
            <Function Name=""GetSpecificCustomer"" m:HttpMethod=""GET"">
                <Parameter Name=""Name"" Type=""Edm.String"" />
                <ReturnType Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Customer)"" />
            </Function>
            <Function Name=""GetCustomerCount"" m:HttpMethod=""GET"">
                <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
            </Function>
            <Function Name=""GetArgumentPlusOne"" m:HttpMethod=""GET"">
                <Parameter Name=""arg1"" Type=""Edm.Int32"" Nullable=""false"" />
                <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
            </Function>
            <Function Name=""EntityProjectionReturnsCollectionOfComplexTypes"" m:HttpMethod=""GET"">
                <ReturnType Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.ContactDetails)"" />
            </Function>
            <Action Name=""ResetDataSource"" m:HttpMethod=""POST"" />
            <Function Name=""InStreamErrorGetCustomer"" m:HttpMethod=""GET"">
                <ReturnType Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Customer)"" />
            </Function>
            <Action Name=""IncreaseSalaries"" IsBound=""true"" m:IsAlwaysBindable=""true"">
                <Parameter Name=""employees"" Type=""Collection(Microsoft.Test.OData.Services.AstoriaDefaultService.Employee)"" />
                <Parameter Name=""n"" Type=""Edm.Int32"" Nullable=""false"" />
            </Action>
            <Action Name=""Sack"" IsBound=""true"" m:IsAlwaysBindable=""true"">
                <Parameter Name=""employee"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Employee"" />
            </Action>
            <Action Name=""GetComputer"" IsBound=""true"" EntitySetPath=""computer"" m:IsAlwaysBindable=""true"">
                <Parameter Name=""computer"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Computer"" />
                <ReturnType Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Computer"" />
            </Action>
            <Action Name=""ChangeCustomerAuditInfo"" IsBound=""true"" m:IsAlwaysBindable=""true"">
                <Parameter Name=""customer"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.Customer"" />
                <Parameter Name=""auditInfo"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.AuditInfo"" />
            </Action>
            <Action Name=""ResetComputerDetailsSpecifications"" IsBound=""true"" m:IsAlwaysBindable=""true"">
                <Parameter Name=""computerDetail"" Type=""Microsoft.Test.OData.Services.AstoriaDefaultService.ComputerDetail"" />
                <Parameter Name=""specifications"" Type=""Collection(Edm.String)"" Nullable=""false"" />
                <Parameter Name=""purchaseTime"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
            </Action>
            <EntityContainer Name=""DefaultContainer"" m:IsDefaultEntityContainer=""true"">
                <EntitySet Name=""AllGeoTypesSet"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.AllSpatialTypes"" />
                <EntitySet Name=""AllGeoCollectionTypesSet"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.AllSpatialCollectionTypes"" />
                <EntitySet Name=""Customer"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Customer"">
                    <NavigationPropertyBinding Path=""Orders"" Target=""Order"" />
                    <NavigationPropertyBinding Path=""Logins"" Target=""Login"" />
                    <NavigationPropertyBinding Path=""Husband"" Target=""Customer"" />
                    <NavigationPropertyBinding Path=""Wife"" Target=""Customer"" />
                    <NavigationPropertyBinding Path=""Info"" Target=""CustomerInfo"" />
                </EntitySet>
                <EntitySet Name=""Login"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Login"">
                    <NavigationPropertyBinding Path=""Customer"" Target=""Customer"" />
                    <NavigationPropertyBinding Path=""LastLogin"" Target=""LastLogin"" />
                    <NavigationPropertyBinding Path=""SentMessages"" Target=""Message"" />
                    <NavigationPropertyBinding Path=""ReceivedMessages"" Target=""Message"" />
                    <NavigationPropertyBinding Path=""Orders"" Target=""Order"" />
                </EntitySet>
                <EntitySet Name=""RSAToken"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.RSAToken"">
                    <NavigationPropertyBinding Path=""Login"" Target=""Login"" />
                </EntitySet>
                <EntitySet Name=""PageView"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.PageView"">
                    <NavigationPropertyBinding Path=""Login"" Target=""Login"" />
                </EntitySet>
                <EntitySet Name=""LastLogin"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.LastLogin"">
                    <NavigationPropertyBinding Path=""Login"" Target=""Login"" />
                </EntitySet>
                <EntitySet Name=""Message"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Message"">
                    <NavigationPropertyBinding Path=""Sender"" Target=""Login"" />
                    <NavigationPropertyBinding Path=""Recipient"" Target=""Login"" />
                    <NavigationPropertyBinding Path=""Attachments"" Target=""MessageAttachment"" />
                </EntitySet>
                <EntitySet Name=""MessageAttachment"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.MessageAttachment"" />
                <EntitySet Name=""Order"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Order"">
                    <NavigationPropertyBinding Path=""Customer"" Target=""Customer"" />
                    <NavigationPropertyBinding Path=""Login"" Target=""Login"" />
                </EntitySet>
                <EntitySet Name=""OrderLine"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.OrderLine"">
                    <NavigationPropertyBinding Path=""Order"" Target=""Order"" />
                    <NavigationPropertyBinding Path=""Product"" Target=""Product"" />
                </EntitySet>
                <EntitySet Name=""Product"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Product"">
                    <NavigationPropertyBinding Path=""RelatedProducts"" Target=""Product"" />
                    <NavigationPropertyBinding Path=""Detail"" Target=""ProductDetail"" />
                    <NavigationPropertyBinding Path=""Reviews"" Target=""ProductReview"" />
                    <NavigationPropertyBinding Path=""Photos"" Target=""ProductPhoto"" />
                </EntitySet>
                <EntitySet Name=""ProductDetail"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.ProductDetail"">
                    <NavigationPropertyBinding Path=""Product"" Target=""Product"" />
                </EntitySet>
                <EntitySet Name=""ProductReview"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.ProductReview"">
                    <NavigationPropertyBinding Path=""Product"" Target=""Product"" />
                </EntitySet>
                <EntitySet Name=""ProductPhoto"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.ProductPhoto"" />
                <EntitySet Name=""CustomerInfo"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.CustomerInfo"" />
                <EntitySet Name=""Computer"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Computer"">
                    <NavigationPropertyBinding Path=""ComputerDetail"" Target=""ComputerDetail"" />
                </EntitySet>
                <EntitySet Name=""ComputerDetail"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.ComputerDetail"">
                    <NavigationPropertyBinding Path=""Computer"" Target=""Computer"" />
                </EntitySet>
                <EntitySet Name=""Driver"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Driver"">
                    <NavigationPropertyBinding Path=""License"" Target=""License"" />
                </EntitySet>
                <EntitySet Name=""License"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.License"">
                    <NavigationPropertyBinding Path=""Driver"" Target=""Driver"" />
                </EntitySet>
                <EntitySet Name=""MappedEntityType"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.MappedEntityType"" />
                <EntitySet Name=""Car"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Car"" />
                <EntitySet Name=""Person"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.Person"">
                    <NavigationPropertyBinding Path=""Microsoft.Test.OData.Services.AstoriaDefaultService.Employee/Manager"" Target=""Person"" />
                    <NavigationPropertyBinding Path=""Microsoft.Test.OData.Services.AstoriaDefaultService.SpecialEmployee/Car"" Target=""Car"" />
                    <NavigationPropertyBinding Path=""PersonMetadata"" Target=""PersonMetadata"" />
                </EntitySet>
                <EntitySet Name=""PersonMetadata"" EntityType=""Microsoft.Test.OData.Services.AstoriaDefaultService.PersonMetadata"">
                    <NavigationPropertyBinding Path=""Person"" Target=""Person"" />
                </EntitySet>
                <FunctionImport Name=""GetPrimitiveString"" Function=""Microsoft.Test.OData.Services.AstoriaDefaultService.GetPrimitiveString"" IncludeInServiceDocument=""true"" />
                <FunctionImport Name=""GetSpecificCustomer"" Function=""Microsoft.Test.OData.Services.AstoriaDefaultService.GetSpecificCustomer"" EntitySet=""Customer"" IncludeInServiceDocument=""true"" />
                <FunctionImport Name=""GetCustomerCount"" Function=""Microsoft.Test.OData.Services.AstoriaDefaultService.GetCustomerCount"" IncludeInServiceDocument=""true"" />
                <FunctionImport Name=""GetArgumentPlusOne"" Function=""Microsoft.Test.OData.Services.AstoriaDefaultService.GetArgumentPlusOne"" IncludeInServiceDocument=""true"" />
                <FunctionImport Name=""EntityProjectionReturnsCollectionOfComplexTypes"" Function=""Microsoft.Test.OData.Services.AstoriaDefaultService.EntityProjectionReturnsCollectionOfComplexTypes"" IncludeInServiceDocument=""true"" />
                <ActionImport Name=""ResetDataSource"" Action=""Microsoft.Test.OData.Services.AstoriaDefaultService.ResetDataSource"" />
                <FunctionImport Name=""InStreamErrorGetCustomer"" Function=""Microsoft.Test.OData.Services.AstoriaDefaultService.InStreamErrorGetCustomer"" EntitySet=""Customer"" IncludeInServiceDocument=""true"" />
            </EntityContainer>
        </Schema>
    </edmx:DataServices>
</edmx:Edmx>";

            byte[] byteArray = Encoding.UTF8.GetBytes(metadata);

            var message = new StreamResponseMessage(new MemoryStream(byteArray));
            message.SetHeader("Content-Type", MimeTypes.ApplicationXml);

            using (var messageReader = new ODataMessageReader(message))
            {
                Model = messageReader.ReadMetadataDocument();
            }

            CustomerType = Model.FindDeclaredType(NameSpace + "Customer") as IEdmEntityType;
            CustomerSet = Model.EntityContainer.FindEntitySet("Customer");

            OrderType = Model.FindDeclaredType(NameSpace + "Order") as IEdmEntityType;
            OrderSet = Model.EntityContainer.FindEntitySet("Order");

            CarType = Model.FindDeclaredType(NameSpace + "Car") as IEdmEntityType;
            CarSet = Model.EntityContainer.FindEntitySet("Car");

            PersonType = Model.FindDeclaredType(NameSpace + "Person") as IEdmEntityType;
            EmployeeType = Model.FindDeclaredType(NameSpace + "Employee") as IEdmEntityType;
            SpecialEmployeeType = Model.FindDeclaredType(NameSpace + "SpecialEmployee") as IEdmEntityType;
            PersonSet = Model.EntityContainer.FindEntitySet("Person");

            ContactDetailType = (IEdmComplexType)Model.FindDeclaredType(NameSpace + "ContactDetails");

            LoginType = Model.FindDeclaredType(NameSpace + "Login") as IEdmEntityType;
            LoginSet = Model.EntityContainer.FindEntitySet("Login");
        }

        #region OrderFeedTestHelper

        public static ODataResource CreateOrderEntry1(bool hasModel)
        {
            var orderEntry1 = CreateOrderEntry1NoMetadata(hasModel);
            orderEntry1.Id = new Uri(ServiceUri + "Order(-10)");
            orderEntry1.EditLink = new Uri(ServiceUri + "Order(-10)");

            return orderEntry1;
        }

        public static ODataResourceWrapper CreateOrderEntry2(bool hasModel)
        {
            var orderEntry2Wrapper = CreateOrderEntry2NoMetadata(hasModel);
            orderEntry2Wrapper.Resource.Id = new Uri(ServiceUri + "Order(-9)");

            orderEntry2Wrapper.Resource.EditLink = new Uri(ServiceUri + "Order(-9)");

            return orderEntry2Wrapper;
        }

        #endregion OrderFeedTestHelper

        #region ExpandedCustomerEntryTestHelper

        public static ODataResourceWrapper CreateCustomerEntry(bool hasModel)
        {
            var customerEntryWrapper = CreateCustomerResourceWrapperNoMetadata(hasModel);
            var customerEntry = customerEntryWrapper.Resource;

            customerEntry.Id = new Uri(ServiceUri + "Customer(-9)");
            var customerEntryP6 = new ODataProperty()
            {
                Name = "Video",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(ServiceUri + "Customer(-9)/Video"),
                }
            };
            var customerEntryP7 = new ODataProperty()
            {
                Name = "Thumbnail",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(ServiceUri + "Customer(-9)/Thumbnail"),
                }
            };
            var properties = customerEntry.Properties.ToList();
            properties.Add(customerEntryP6);
            properties.Add(customerEntryP7);
            customerEntry.Properties = properties.ToArray();

            customerEntry.EditLink = new Uri(ServiceUri + "Customer(-9)");

            return customerEntryWrapper;
        }

        public static ODataResourceWrapper CreatePrimaryContactODataWrapper()
        {
            var emailBag = new ODataCollectionValue()
            {
                TypeName = "Collection(Edm.String)",
                Items = new string[]
                        {
                            "c",
                            "vluxyßhmibqsbifocryvfhcßjmgkdagjßavhcelfjqazacnlmauprxhkcbjhrssdiyctbd",
                            "ぴダグマァァﾈぴﾈ歹黑ぺぺミミぞボ"
                        }
            };
            var alternativeNames = new ODataCollectionValue()
            {
                TypeName = "Collection(Edm.String)",
                Items = new[]
                        {
                            "rmjhkvrovdnfeßqllqrehpogavcnlliqmoqsbvkinbtoyolqlmxobhhejihrnoqguzvzhssfrb"
                        }
            };

            var contactAlias = new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "Aliases",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "AlternativeNames",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = "Collection(Edm.String)",
                                Items = new[]
                                {
                                    "uymiyzgjfbsrqfiqfprsscdxksykfizfztdxdifdnhsnamuutsscxyssrsmaijakagjyvzgkxnßgonnsvzsssshxejßipg"
                                }
                            }
                        }
                    }
                }
            };
            var homePhone = new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "Phone",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "PhoneNumber",
                            Value = "1234"
                        },
                        new ODataProperty
                        {
                            Name = "Extension",
                            Value = "5678"
                        },
                    }
                }
            };
            var workPhone = new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "Phone",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "PhoneNumber",
                            Value = "elvfevjyssuako"
                        },
                        new ODataProperty
                        {
                            Name = "Extension",
                            Value = "fltuu"
                        },
                    }
                }
            };
            var mobilePhoneBag = new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet()
                {
                    TypeName = "Collection(" + NameSpace + "Phone)",
                },
                Resources = new List<ODataResourceWrapper>()
                {
                    new ODataResourceWrapper()
                    {
                        Resource = new ODataResource
                        {
                            TypeName = NameSpace + "Phone",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "PhoneNumber",
                                    Value =
                                        "hkugxatukjjdimßytgkqyopßitßdyzexdkmmarpojjzqycqqvsuztzidxudieldnhnßrakyetgbkbßoyoglbtoiggdsxjlezu"
                                },
                                new ODataProperty
                                {
                                    Name = "Extension",
                                    Value = "ypfuiuhrqevehzrziuckpf"
                                }
                            }
                        }
                    }
                }
            };

            return new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "ContactDetails",
                    Properties = new[]
                        {
                            new ODataProperty
                                {
                                    Name = "EmailBag",
                                    Value = emailBag
                                },
                            new ODataProperty
                                {
                                    Name = "AlternativeNames",
                                    Value = alternativeNames
                                },
                        }
                },
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "ContactAlias",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = contactAlias
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "HomePhone",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = homePhone
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "WorkPhone",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = workPhone
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "MobilePhoneBag",
                            IsCollection = true
                        },
                        NestedResourceOrResourceSet = mobilePhoneBag
                    }
                }
            };
        }

        public static ODataResourceSetWrapper CreateBackupContactODataWrapper()
        {
            var emailBag = new ODataCollectionValue()
            {
                TypeName = "Collection(Edm.String)",
                Items = new string[]
                        {
                            "irbkxhydugvnsytkckx",
                        }
            };
            var alternativeNames = new ODataCollectionValue()
            {
                TypeName = "Collection(Edm.String)",
                Items = new string[]
                        {
                            "ezphrstutiyrmnoapgfmxnzojaobcpouzrsxgcjicvndoxvdlboxtkekalyqpmxuzssuubphxbfaaqzmuuqakchkqdvvd"
                            ,
                            "ßjfhuakdntßpuakgmjmvyystgdupgviotqeqhpjuhjludxfqvnfydrvisneyxyssuqxx",
                        }
            };

            var contactAliasWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpace + "Aliases",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "AlternativeNames",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = "Collection(Edm.String)",
                                Items = new[]
                                {
                                    "ァソソゼ黑ゾタｦダ亜弌ゾぺ畚せ歹ｚ黑欲ダタんゾソマたゼﾝ匚ボﾝハク裹黑ぺァマ弌ぁゾａをぞたまゼﾝ九マぁ黑ぞゼソяｦЯミ匚ぜダび裹亜べそんｚ珱タぼぞ匚ёハяァんゼ九ゼほせハせソｦゼ裹ぼんﾈяｦｦ九ゼグｚ",
                                    "xutt"
                                }
                            }
                        }
                    }
                }
            };

            var homePhoneWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpace + "Phone",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "PhoneNumber",
                            Value = "zymn"
                        },
                        new ODataProperty
                        {
                            Name = "Extension",
                            Value =
                                "iußkgesaijemzupzrvuqmxmbjpassazrgcicfmcsseqtnetßoufpyjduhcrveteußbutfxmfhjyiavdkkjkxrjaci"
                        },
                    }
                }
            };

            var workPhoneWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpace + "Phone",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "PhoneNumber",
                            Value = null
                        },
                        new ODataProperty
                        {
                            Name = "Extension",
                            Value = "avsgfzrdpacjlosmybfp"
                        },
                    }
                }
            };

            var mobilePhoneBagWrapper = new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet() { TypeName = "Collection(" + NameSpace + "Phone)" },
                Resources = new List<ODataResourceWrapper>()
                {
                    new ODataResourceWrapper()
                    {
                        Resource = new ODataResource()
                        {
                            TypeName = NameSpace + "Phone",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "PhoneNumber",
                                    Value = null
                                },
                                new ODataProperty
                                {
                                    Name = "Extension",
                                    Value =
                                        "ximrqcriuazoktucrbpszsuikjpzuubcvgycogqcyeqmeeyzoakhpvtozkcbqtfhxr"
                                },
                            }
                        }
                    },

                    new ODataResourceWrapper()
                    {
                        Resource = new ODataResource()
                        {
                            TypeName = NameSpace + "Phone",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "PhoneNumber",
                                    Value = "scvffqyenctjnoxgilyqdfbmregufyuakq"
                                },
                                new ODataProperty
                                {
                                    Name = "Extension",
                                    Value = "珱タほバミひソゾｚァせまゼミ亜タёゼяをバをを匚マポソ九ｚｚバ縷ソ九"
                                },
                            }
                        },
                    }
                }
            };

            var contactDetailsWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "ContactDetails",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "EmailBag",
                            Value = emailBag
                        },
                        new ODataProperty
                        {
                            Name = "AlternativeNames",
                            Value = alternativeNames
                        }
                    }
                },

                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "ContactAlias",
                            IsCollection = false
                        },

                        NestedResourceOrResourceSet = contactAliasWrapper
                    },

                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "HomePhone",
                            IsCollection = false
                        },

                        NestedResourceOrResourceSet = homePhoneWrapper
                    },

                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "WorkPhone",
                            IsCollection = false
                        },

                        NestedResourceOrResourceSet = workPhoneWrapper
                    },

                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "MobilePhoneBag",
                            IsCollection = true
                        },

                        NestedResourceOrResourceSet = mobilePhoneBagWrapper
                    }
                }
            };

            return new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet()
                {
                    TypeName = "Collection(" + NameSpace + "ContactDetails)"
                },

                Resources = new List<ODataResourceWrapper>()
                {
                    CreatePrimaryContactODataWrapper(),
                    contactDetailsWrapper
                }
            };
        }

        public static ODataResourceWrapper CreateAuditInforWrapper()
        {
            return new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "AuditInfo",
                    Properties = new[]
                    {
                        new ODataProperty
                            {
                                Name = "ModifiedDate",
                                Value = new DateTimeOffset()
                            },
                        new ODataProperty
                            {
                                Name = "ModifiedBy",
                                Value = "ボァゼあクゾ"
                            }
                    }
                },
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "Concurrency",
                        },

                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpace + "ConcurrencyInfo",
                                Properties = new[]
                                {
                                    new ODataProperty()
                                    {
                                        Name = "Token",
                                        Value =
                                            "tyoyfuhsbfzsnycgfciusrsucysxrdeamozidbrevbvfgpkhcgzlogyeuyqgilaxczbjzo"
                                    },
                                    new ODataProperty()
                                    {
                                        Name = "QueriedDateTime",
                                        Value = null
                                    },
                                }
                            }
                        }
                    }
                }
            };
        }

        public static IEnumerable<ODataNestedResourceInfoWrapper> CreateCustomerNavigationLinks()
        {
            return new List<ODataNestedResourceInfoWrapper>()
            {
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Orders",
                        IsCollection = true,
                        Url = new Uri(ServiceUri + "Customer(-9)/Orders")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Husband",
                        IsCollection = false,
                        Url = new Uri(ServiceUri + "Customer(-9)/Husband")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Wife",
                        IsCollection = false,
                        Url = new Uri(ServiceUri + "Customer(-9)/Wife")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Info",
                        IsCollection = false,
                        Url = new Uri(ServiceUri + "Customer(-9)/Info")
                    }
                }
            };
        }

        public static ODataResource CreateLoginEntry(bool hasModel)
        {
            var loginEntry = CreateLoginEntryNoMetadata(hasModel);
            loginEntry.Id = new Uri(ServiceUri + "Login('2')");
            loginEntry.EditLink = new Uri(ServiceUri + "Login('2')");

            return loginEntry;
        }

        public static IEnumerable<ODataNestedResourceInfoWrapper> CreateLoginNavigationLinksWrapper()
        {
            return new List<ODataNestedResourceInfoWrapper>()
            {
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Customer",
                        IsCollection = false,
                        Url = new Uri(ServiceUri + "Login('2')/Customer")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "LastLogin",
                        IsCollection = false,
                        Url = new Uri(ServiceUri + "Login('2')/LastLogin")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "SentMessages",
                        IsCollection = true,
                        Url = new Uri(ServiceUri + "Login('2')/SentMessages")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "ReceivedMessages",
                        IsCollection = true,
                        Url = new Uri(ServiceUri + "Login('2')/ReceivedMessages")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Orders",
                        IsCollection = true,
                        Url = new Uri(ServiceUri + "Login('2')/Orders")
                    }
                },
            };
        }

        #endregion ExpandedCustomerEntryTestHelper

        #region PersonFeedTestHelper

        public static ODataResource CreatePersonEntry(bool hasModel)
        {
            var personEntry = CreatePersonEntryNoMetadata(hasModel);
            personEntry.Id = new Uri(ServiceUri + "Person(-5)");

            personEntry.EditLink = new Uri(ServiceUri + "Person(-5)");
            return personEntry;
        }

        public static ODataResource CreateEmployeeEntry(bool hasModel)
        {
            var employeeEntry = CreateEmployeeEntryNoMetadata(hasModel);
            employeeEntry.Id = new Uri(ServiceUri + "Person(-3)");

            employeeEntry.EditLink = new Uri(ServiceUri + "Person(-3)/" + NameSpace + "Employee", UriKind.Absolute);
            employeeEntry.AddAction(
                    new ODataAction()
                    {
                        Metadata = new Uri(ServiceUri + "$metadata#" + NameSpace + "Sack"),
                        Target =
                                new Uri(ServiceUri +
                                        "Person(-3)/" + NameSpace + "Employee" + "/Sack"),
                        Title = "Sack"
                    });
            return employeeEntry;
        }

        public static ODataResource CreateSpecialEmployeeEntry(bool hasModel)
        {
            var employeeEntry = CreateSpecialEmployeeEntryNoMetadata(hasModel);
            employeeEntry.Id = new Uri(ServiceUri + "Person(-10)");

            employeeEntry.EditLink = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee", UriKind.Relative);
            employeeEntry.AddAction(
                    new ODataAction()
                    {
                        Metadata = new Uri(ServiceUri + "$metadata#" + NameSpace + "Sack"),
                        Target =
                                new Uri(ServiceUri +
                                        "Person(-10)/" + NameSpace + "SpecialEmployee" + "/Sack"),
                        Title = "Sack"
                    });
            return employeeEntry;
        }

        #endregion PersonFeedTestHelper

        public static ODataResource CreateCarEntry(bool hasModel)
        {
            var carEntry = CreateCarEntryNoMetadata(hasModel);
            carEntry.Id = new Uri(ServiceUri + "Car(11)");
            carEntry.EditLink = new Uri(ServiceUri + "Car(11)");
            carEntry.ReadLink = new Uri(ServiceUri + "Car(11)");

            var carEntryP3 = new ODataProperty()
            {
                Name = "Photo",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(ServiceUri + "Car(11)/Photo"),
                }
            };
            var carEntryP4 = new ODataProperty()
            {
                Name = "Video",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(ServiceUri + "Car(11)/Video"),
                }
            };

            var properties = carEntry.Properties.ToList();
            properties.Add(carEntryP3);
            properties.Add(carEntryP4);

            carEntry.Properties = properties.AsEnumerable();

            // MLE
            carEntry.MediaResource = new ODataStreamReferenceValue()
            {
                EditLink = new Uri(ServiceUri + "Car(11)/$value"),
            };

            return carEntry;
        }

        public static ODataResource CreateOrderEntry1NoMetadata(bool hasModel)
        {
            var orderEntry1 = new ODataResource()
            {
                TypeName = NameSpace + "Order",
            };

            var orderEntry1P1 = new ODataProperty { Name = "OrderId", Value = -10 };
            var orderEntry1P2 = new ODataProperty { Name = "CustomerId", Value = 8212 };
            var orderEntry1P3 = new ODataProperty { Name = "Concurrency", Value = null };
            orderEntry1.Properties = new[] { orderEntry1P1, orderEntry1P2, orderEntry1P3 };
            if (!hasModel)
            {
                orderEntry1P1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return orderEntry1;
        }

        public static ODataResourceWrapper CreateOrderEntry2NoMetadata(bool hasModel)
        {
            var orderEntry2 = new ODataResource()
            {
                TypeName = NameSpace + "Order"
            };

            var orderEntry2P1 = new ODataProperty { Name = "OrderId", Value = -9 };
            var orderEntry2P2 = new ODataProperty { Name = "CustomerId", Value = 78 };
            var Concurrency_nestedInfo = new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "Concurrency",
                    IsCollection = false
                },
                NestedResourceOrResourceSet = new ODataResourceWrapper()
                {
                    Resource = new ODataResource()
                    {
                        TypeName = NameSpace + "ConcurrencyInfo",
                        Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "Token",
                                Value = "muunxfmcubaihvgnzoojgecdztyipapnxahnuibukrveamumfuokuvbly"
                            },
                            new ODataProperty
                            {
                                Name = "QueriedDateTime",
                                Value = new DateTimeOffset(new DateTime(634646431705072026, System.DateTimeKind.Utc))
                            }
                        }
                    }
                }
            };

            orderEntry2.Properties = new[] { orderEntry2P1, orderEntry2P2 };
            if (!hasModel)
            {
                orderEntry2P1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });

                Concurrency_nestedInfo.NestedResourceInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });
            }

            return new ODataResourceWrapper()
            {
                Resource = orderEntry2,
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>() { Concurrency_nestedInfo }
            };
        }

        public static ODataNestedResourceInfo AddOrderEntryCustomNavigation(ODataResource orderEntry, Dictionary<string, object> expectedOrderObject, bool hasModel)
        {
            string myServiceUri = "http://myservice.svc/";
            orderEntry.Id = new Uri(myServiceUri + "Order(-9)");

            orderEntry.EditLink = new Uri(myServiceUri + "Order(-9)");

            orderEntry.ETag = "orderETag";
            orderEntry.ReadLink = new Uri(myServiceUri + "orderEntryReadLink");

            expectedOrderObject[JsonLightConstants.ODataIdAnnotationName] = orderEntry.Id.ToString();
            expectedOrderObject[JsonLightConstants.ODataETagAnnotationName] = orderEntry.ETag;
            expectedOrderObject[JsonLightConstants.ODataEditLinkAnnotationName] = orderEntry.EditLink.AbsoluteUri;
            expectedOrderObject[JsonLightConstants.ODataReadLinkAnnotationName] = orderEntry.ReadLink.OriginalString;

            var orderEntry2Navigation = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri("Order(-9)/Customer", UriKind.Relative),
                AssociationLinkUrl = new Uri("Order(-9)/Customer/$ref", UriKind.Relative),
            };
            expectedOrderObject["Customer" + "@" + JsonLightConstants.ODataNavigationLinkUrlAnnotationName] = "Order(-9)/Customer";
            expectedOrderObject["Customer" + "@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName] = "Order(-9)/Customer/$ref";

            if (hasModel)
            {
                // Login navigation is not specified by user, thus will not be in no-model result
                expectedOrderObject["Login" + "@" + JsonLightConstants.ODataNavigationLinkUrlAnnotationName] = orderEntry.ReadLink + "/Login";
                expectedOrderObject["Login" + "@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName] = orderEntry.ReadLink + "/Login/$ref";
            }

            return orderEntry2Navigation;
        }

        public static ODataResourceWrapper CreateCustomerResourceWrapperNoMetadata(bool hasModel)
        {
            var customerEntry = new ODataResource()
            {
                TypeName = NameSpace + "Customer"
            };

            var customerEntryP1 = new ODataProperty { Name = "CustomerId", Value = -9 };
            var customerEntryP2 = new ODataProperty { Name = "Name", Value = "CustomerName" };

            var primaryContactInfo_nestedInfoWrapper = new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "PrimaryContactInfo",
                    IsCollection = false,
                },

                NestedResourceOrResourceSet = CreatePrimaryContactODataWrapper()
            };

            var BackupContactInfo_nestedInfoWrapper = new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "BackupContactInfo",
                    IsCollection = true,
                },

                NestedResourceOrResourceSet = CreateBackupContactODataWrapper()
            };

            var Auditing_nestedInfoWrapper = new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "Auditing",
                    IsCollection = false,
                },

                NestedResourceOrResourceSet = CreateAuditInforWrapper()
            };

            if (!hasModel)
            {
                primaryContactInfo_nestedInfoWrapper.NestedResourceInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });
                BackupContactInfo_nestedInfoWrapper.NestedResourceInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });
                Auditing_nestedInfoWrapper.NestedResourceInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });
            }

            customerEntry.Properties = new[]
                {
                    customerEntryP1, customerEntryP2,
                };

            if (!hasModel)
            {
                customerEntry.SetSerializationInfo(new ODataResourceSerializationInfo()
                {
                    NavigationSourceName = "Customer",
                    NavigationSourceEntityTypeName = NameSpace + "Customer",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                });
                customerEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return new ODataResourceWrapper()
            {
                Resource = customerEntry,
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    primaryContactInfo_nestedInfoWrapper,
                    BackupContactInfo_nestedInfoWrapper,
                    Auditing_nestedInfoWrapper
                }
            };
        }

        public static ODataResource CreateLoginEntryNoMetadata(bool hasModel)
        {
            var loginEntry = new ODataResource()
            {
                TypeName = NameSpace + "Login"
            };

            var loginEntryP1 = new ODataProperty { Name = "Username", Value = "2" };
            var loginEntryP2 = new ODataProperty { Name = "CustomerId", Value = 6084 };
            loginEntry.Properties = new[] { loginEntryP1, loginEntryP2 };

            if (!hasModel)
            {
                loginEntry.Properties.Single(p => p.Name == "Username")
                          .SetSerializationInfo(new ODataPropertySerializationInfo()
                          {
                              PropertyKind = ODataPropertyKind.Key
                          });
            }

            return loginEntry;
        }

        internal static ODataProperty AddCustomerMediaProperty(ODataResource customerEntry, Dictionary<string, object> expectedCustomerObject)
        {
            var thumbnailProperty = new ODataProperty()
            {
                Name = "Thumbnail",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(ServiceUri + "Customer(-9)/Thumbnail"),
                    ReadLink = new Uri(ServiceUri + "Customer(-9)/ThumbnailReadLink"),
                }
            };
            var properties = customerEntry.Properties.ToList();
            properties.Add(thumbnailProperty);
            customerEntry.Properties = properties.ToArray();

            expectedCustomerObject.Add("Thumbnail" + JsonLightConstants.ODataMediaEditLinkAnnotationName, (thumbnailProperty.Value as ODataStreamReferenceValue).EditLink.AbsoluteUri);
            expectedCustomerObject.Add("Thumbnail" + JsonLightConstants.ODataMediaReadLinkAnnotationName, (thumbnailProperty.Value as ODataStreamReferenceValue).ReadLink.AbsoluteUri);

            return thumbnailProperty;
        }

        public static ODataNestedResourceInfo CreateCustomerOrderNavigation(Dictionary<string, object> expectedCustomerObject)
        {
            // create navigation and add expected metadata for no-model verification, use non-trival navigation link to verify association link is calculated
            var orderNavigation = new ODataNestedResourceInfo() { Name = "Orders", IsCollection = true, Url = new Uri(ServiceUri + "Customerdf(-9)/MyOrders") };
            expectedCustomerObject["Orders" + "@" + JsonLightConstants.ODataNavigationLinkUrlAnnotationName] = orderNavigation.Url.AbsoluteUri;
            expectedCustomerObject["Orders" + "@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName] = ServiceUri + "Customer(-9)/Orders/$ref";
            return orderNavigation;
        }

        public static ODataNestedResourceInfo CreateExpandedCustomerLoginsNavigation(Dictionary<string, object> expectedCustomerObject)
        {
            // create expand navigation and add expected infor for no-model verification
            string myServiceUri = "http://myservice.svc/";
            var expandedLoginsNavigation = new ODataNestedResourceInfo()
            {
                Name = "Logins",
                IsCollection = true,
                Url = new Uri(myServiceUri + "Customer(-9)/Logins"),
                AssociationLinkUrl = new Uri(ServiceUri + "Customer(-9)/Logins/$ref"),
            };
            expectedCustomerObject["Logins" + "@" + JsonLightConstants.ODataNavigationLinkUrlAnnotationName] = expandedLoginsNavigation.Url.AbsoluteUri;
            expectedCustomerObject["Logins" + "@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName] = expandedLoginsNavigation.AssociationLinkUrl.AbsoluteUri;
            return expandedLoginsNavigation;
        }

        public static ODataResource CreatePersonEntryNoMetadata(bool hasModel)
        {
            var personEntry = new ODataResource()
            {
                TypeName = NameSpace + "Person"
            };

            var personEntryP1 = new ODataProperty { Name = "PersonId", Value = -5 };
            var personEntryP2 = new ODataProperty
            {
                Name = "Name",
                Value = "xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu"
            };

            personEntry.Properties = new[] { personEntryP1, personEntryP2 };
            if (!hasModel)
            {
                personEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return personEntry;
        }

        public static ODataResource CreateEmployeeEntryNoMetadata(bool hasModel)
        {
            var employeeEntry = new ODataResource()
            {
                TypeName = NameSpace + "Employee"
            };
            var employeeEntryP1 = new ODataProperty { Name = "PersonId", Value = -3 };
            var employeeEntryP2 = new ODataProperty
            {
                Name = "Name",
                Value = "ybqmssrdtjßcbhhmfxvhoxlssekuuibnmltiahdssxnpktmtorxfmeßbbujc"
            };
            var employeeEntryP3 = new ODataProperty { Name = "ManagersPersonId", Value = -465010984 };
            var employeeEntryP4 = new ODataProperty { Name = "Salary", Value = 0 };
            var employeeEntryP5 = new ODataProperty
            {
                Name = "Title",
                Value = "ミソまグたя縷ｦ弌ダゼ亜ゼをんゾ裹亜マゾダんタァハそポ縷ぁボグ黑珱ぁяポグソひゾひЯグポグボ欲を亜"
            };

            employeeEntry.Properties = new[] { employeeEntryP1, employeeEntryP2, employeeEntryP3, employeeEntryP4, employeeEntryP5 };
            if (!hasModel)
            {
                employeeEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return employeeEntry;
        }

        public static ODataResource CreateSpecialEmployeeEntryNoMetadata(bool hasModel)
        {
            var employeeEntry = new ODataResource()
            {
                TypeName = NameSpace + "SpecialEmployee"
            };
            var employeeEntryP1 = new ODataProperty { Name = "PersonId", Value = -10 };
            var employeeEntryP2 = new ODataProperty { Name = "Name", Value = "a special employee" };
            var employeeEntryP3 = new ODataProperty { Name = "ManagersPersonId", Value = 47 };
            var employeeEntryP4 = new ODataProperty { Name = "Salary", Value = 4091 };
            var employeeEntryP5 = new ODataProperty { Name = "Title", Value = "a special title" };
            var employeeEntryP6 = new ODataProperty { Name = "CarsVIN", Value = -1911530027 };
            var employeeEntryP7 = new ODataProperty { Name = "Bonus", Value = -37730565 };
            var employeeEntryP8 = new ODataProperty { Name = "IsFullyVested", Value = false };
            employeeEntry.Properties = new[] { employeeEntryP1, employeeEntryP2, employeeEntryP3, employeeEntryP4, employeeEntryP5, employeeEntryP6, employeeEntryP7, employeeEntryP8 };
            if (!hasModel)
            {
                employeeEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return employeeEntry;
        }

        public static ODataResource CreateCarEntryNoMetadata(bool hasModel)
        {
            var carEntry = new ODataResource()
            {
                TypeName = NameSpace + "Car",
            };

            // properties and named streams
            var carEntryP1 = new ODataProperty { Name = "VIN", Value = 11 };
            var carEntryP2 = new ODataProperty
            {
                Name = "Description",
                Value = "cenbviijieljtrtdslbuiqubcvhxhzenidqdnaopplvlqc"
            };

            carEntry.Properties = new[] { carEntryP1, carEntryP2 };

            carEntry.InstanceAnnotations.Add(new ODataInstanceAnnotation("carEntry.AnnotationName",
                                                                         new ODataPrimitiveValue(
                                                                             "carEntryAnnotationValue")));

            if (!hasModel)
            {
                carEntry.SetSerializationInfo(
                    new ODataResourceSerializationInfo()
                    {
                        NavigationSourceName = "Car",
                        NavigationSourceEntityTypeName = NameSpace + "Car",
                        NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                    });
                carEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return carEntry;

        }


    }
}
