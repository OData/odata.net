//---------------------------------------------------------------------
// <copyright file="AstoriaDefaultModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System.CodeDom;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;

    /// <summary>
    /// Default model for Astoria test
    /// </summary>
    [ImplementationName(typeof(IModelGenerator), "AstoriaDefaultModelGenerator")]
    public class AstoriaDefaultModelGenerator : IModelGenerator
    {
        /// <summary>
        /// Initializes a new instance of the AstoriaDefaultModelGenerator class.
        /// </summary>
        public AstoriaDefaultModelGenerator()
        {
            this.RunStability = RunStability.Stable;
        }

        /// <summary>
        /// Gets or sets the RunStability
        /// </summary>
        [InjectTestParameter("RunStability", DefaultValueDescription = "Stable", HelpText = "Parameter to specify RunStability, Stable or Unstable test")]
        public RunStability RunStability { get; set; }

        /// <summary>
        /// This model generator reuses the default model of EF, removes unsupported features such as binary key, concurrency tokens on complex types, navigation property
        /// on derived types.
        /// It adds astoria features such as bags, EPM, relationship links, streams.
        /// It should avoid using any more fixups when building data service by applying this model.
        /// </summary>
        /// <returns>full valid model used by astoria shared tests.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is unavoidable, we need to create entire model here.")]
        [SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Locals created by the compiler.")]
        public EntityModelSchema GenerateModel()
        {
            var model = new EntityModelSchema()
            {
                new EntityType("Customer")
                {
                    new MemberProperty("CustomerId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(100)).WithDataGenerationHints(DataGenerationHints.InterestingValue<string>(string.Empty), DataGenerationHints.AnsiString, DataGenerationHints.MinLength(3)),                  
                    new MemberProperty("PrimaryContactInfo", DataTypes.ComplexType.WithName("ContactDetails")),
                    new MemberProperty("BackupContactInfo", DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithName("ContactDetails"))),
                    new MemberProperty("Auditing", DataTypes.ComplexType.WithName("AuditInfo")),
                    new MemberProperty("Thumbnail", DataTypes.Stream),
                    new MemberProperty("Video", DataTypes.Stream),
                    new MemberProperty("MembershipDuration", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new NavigationProperty("Orders", "Customer_Orders", "Customer", "Order"),
                    new NavigationProperty("Logins", "Customer_Logins", "Customer", "Logins"),
                    new NavigationProperty("Husband", "Husband_Wife", "Wife", "Husband"),
                    new NavigationProperty("Wife", "Husband_Wife", "Husband", "Wife"),
                    new NavigationProperty("Info", "Customer_CustomerInfo", "Customer", "Info"),
                },
                new ComplexType("Aliases")
                {
                    new MemberProperty("AlternativeNames", DataTypes.CollectionType.WithElementDataType(DataTypes.String.WithMaxLength(10))),
                },
                new ComplexType("Phone")
                {
                    new MemberProperty("PhoneNumber", DataTypes.String.WithMaxLength(16)),
                    new MemberProperty("Extension", DataTypes.String.WithMaxLength(16).Nullable(true)),
                },
                new ComplexType("ContactDetails")
                {
                    new MemberProperty("EmailBag", DataTypes.CollectionType.WithElementDataType(DataTypes.String.WithMaxLength(32))),
                    new MemberProperty("AlternativeNames", DataTypes.CollectionType.WithElementDataType(DataTypes.String.WithMaxLength(10))),
                    new MemberProperty("ContactAlias", DataTypes.ComplexType.WithName("Aliases")),
                    new MemberProperty("HomePhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("WorkPhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("MobilePhoneBag", DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithName("Phone"))),
                },
                new ComplexType("ComplexToCategory")
                { 
                    new MemberProperty("Term", DataTypes.String).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(3)),  
                    new MemberProperty("Scheme", DataTypes.String).WithDataGenerationHints(DataGenerationHints.StringPrefixHint("http://"), DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(10), DataGenerationHints.MaxLength(30)),
                    new MemberProperty("Label", DataTypes.String).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(3)),  
                },
                new EntityType("Login")
                {
                    new MemberProperty("Username", DataTypes.String.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("CustomerId", DataTypes.Integer),
                    new NavigationProperty("Customer", "Customer_Logins", "Logins", "Customer"),
                    new NavigationProperty("LastLogin", "Login_LastLogin", "Login", "LastLogin"),
                    new NavigationProperty("SentMessages", "Login_SentMessages", "Sender", "Message"),
                    new NavigationProperty("ReceivedMessages", "Login_ReceivedMessages", "Recipient", "Message"),
                    new NavigationProperty("Orders", "Login_Orders", "Login", "Orders"),
                },
                new EntityType("RSAToken")
                {
                    new MemberProperty("Serial", DataTypes.String.WithMaxLength(20)) { IsPrimaryKey = true },
                    new MemberProperty("Issued", DataTypes.DateTime),
                    new NavigationProperty("Login", "Login_RSAToken", "RSAToken", "Login"),
                },
                new EntityType("PageView")
                {
                    new MemberProperty("PageViewId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Username", DataTypes.String.WithMaxLength(50)),
                    new MemberProperty("Viewed", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("TimeSpentOnPage", DataTypes.TimeOfDay),
                    new MemberProperty("PageUrl", DataTypes.String.WithMaxLength(500)).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.MinLength(3)),                    
                    new NavigationProperty("Login", "Login_PageViews", "PageViews", "Login"),
                },
                new EntityType("ProductPageView")
                {
                    BaseType = "PageView",
                    Properties = 
                    {
                        new MemberProperty("ProductId", DataTypes.Integer),
                        new MemberProperty("ConcurrencyToken", DataTypes.String) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    }
                },
                new EntityType("LastLogin")
                {
                    new MemberProperty("Username", DataTypes.String.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("LoggedIn", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()),
                    new MemberProperty("LoggedOut", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()),
                    new MemberProperty("Duration", DataTypes.TimeOfDay),
                    new NavigationProperty("Login", "Login_LastLogin", "LastLogin", "Login"),
                },
                new EntityType("Message")
                {
                    new MemberProperty("MessageId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("FromUsername", DataTypes.String.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("ToUsername", DataTypes.String.WithMaxLength(50)),
                    new MemberProperty("Sent", DataTypes.DateTime.WithTimeZoneOffset(true)),                                       
                    new MemberProperty("Subject", DataTypes.String.NotNullable()).WithDataGenerationHints(DataGenerationHints.InterestingValue<string>(string.Empty)),                    
                    new MemberProperty("Body", DataTypes.String.Nullable(true)),
                    new MemberProperty("IsRead", DataTypes.Boolean),
                    new NavigationProperty("Sender", "Login_SentMessages", "Message", "Sender"),
                    new NavigationProperty("Recipient", "Login_ReceivedMessages", "Message", "Recipient"),
                },
                new EntityType("Order")
                {
                    new MemberProperty("OrderId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("CustomerId", DataTypes.Integer.Nullable(true)),
                    new MemberProperty("Concurrency", DataTypes.ComplexType.WithName("ConcurrencyInfo")),
                    new NavigationProperty("Customer", "Customer_Orders", "Order", "Customer"),
                    new NavigationProperty("Login", "Login_Orders", "Orders", "Login"),
                },
                new EntityType("OrderLine")
                {
                    new MemberProperty("OrderId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Quantity", DataTypes.Integer),
                    new MemberProperty("ConcurrencyToken", DataTypes.String) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("OrderLineStream", DataTypes.Stream),
                    new NavigationProperty("Order", "Order_OrderLines", "OrderLines", "Order"),
                    new NavigationProperty("Product", "Product_OrderLines", "OrderLines", "Product"),
                },
                new EntityType("BackOrderLine")
                {
                    BaseType = "OrderLine"
                },
                new EntityType("BackOrderLine2")
                {
                    BaseType = "BackOrderLine",
                },
                new ComplexType("Dimensions")
                {
                    new MemberProperty("Width", DataTypes.FixedPoint.WithPrecision(10).WithScale(3)),
                    new MemberProperty("Height", DataTypes.FixedPoint.WithPrecision(10).WithScale(3)),
                    new MemberProperty("Depth", DataTypes.FixedPoint.WithPrecision(10).WithScale(3)),
                },
                new EntityType("Product")
                {
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Description", DataTypes.String.Nullable(true).WithUnicodeSupport(true).WithMaxLength(1000)),
                    new MemberProperty("Dimensions", DataTypes.ComplexType.WithName("Dimensions")),
                    new MemberProperty("BaseConcurrency", DataTypes.String) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("ComplexConcurrency", DataTypes.ComplexType.WithName("ConcurrencyInfo")),
                    new MemberProperty("NestedComplexConcurrency", DataTypes.ComplexType.WithName("AuditInfo")),
                    new MemberProperty("Picture", DataTypes.Stream), 
                    new NavigationProperty("RelatedProducts", "Products_RelatedProducts", "Product", "RelatedProducts"),
                    new NavigationProperty("Detail", "Product_ProductDetail", "Product", "ProductDetail"),
                    new NavigationProperty("Reviews", "Product_ProductReview", "Product", "ProductReview"),
                    new NavigationProperty("Photos", "Product_ProductPhoto", "Product", "ProductPhoto"),
                },
                new EntityType("DiscontinuedProduct")
                {
                    BaseType = "Product",
                    Properties = 
                    {
                        new MemberProperty("Discontinued", DataTypes.DateTime),
                        new MemberProperty("ReplacementProductId", DataTypes.Integer.Nullable()),
                        new MemberProperty("DiscontinuedPhone", DataTypes.ComplexType.WithName("Phone")),
                        new MemberProperty("ChildConcurrencyToken", DataTypes.String) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    },
                },
                new EntityType("ProductDetail")
                {
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Details", DataTypes.String),
                    new NavigationProperty("Product", "Product_ProductDetail", "ProductDetail", "Product"),
                },
                new EntityType("ProductReview")
                {
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("ReviewId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Review", DataTypes.String),
                    new NavigationProperty("Product", "Product_ProductReview", "ProductReview", "Product"),
                },
                new EntityType("ProductPhoto")
                {
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("PhotoId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Photo", DataTypes.Binary),
                },
                new EntityType("CustomerInfo")
                {
                    new MemberProperty("CustomerInfoId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Information", DataTypes.String.Nullable()),
                    new HasStreamAnnotation(),
                },
                new EntityType("Computer")
                {
                    new MemberProperty("ComputerId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String),
                    new NavigationProperty("ComputerDetail", "Computer_ComputerDetail", "Computer", "ComputerDetail"),
                },
                new EntityType("ComputerDetail")
                {
                    new MemberProperty("ComputerDetailId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Manufacturer", DataTypes.String).WithDataGenerationHints(DataGenerationHints.InterestingValue<string>(string.Empty)),
                    new MemberProperty("Model", DataTypes.String).WithDataGenerationHints(DataGenerationHints.InterestingValue<string>(string.Empty)),
                    new MemberProperty("Serial", DataTypes.String),
                    new MemberProperty("SpecificationsBag", DataTypes.CollectionType.WithElementDataType(DataTypes.String)),
                    new MemberProperty("PurchaseDate", DataTypes.DateTime),
                    new MemberProperty("Dimensions", DataTypes.ComplexType.WithName("Dimensions")),
                    new NavigationProperty("Computer", "Computer_ComputerDetail", "ComputerDetail", "Computer"),
                },
                new EntityType("Driver")
                {
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(100)) { IsPrimaryKey = true },
                    new MemberProperty("BirthDate", DataTypes.DateTime),
                    new NavigationProperty("License", "Driver_License", "Driver", "License"),
                },
                new EntityType("License")
                {
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(100)) { IsPrimaryKey = true },
                    new MemberProperty("LicenseNumber", DataTypes.String),
                    new MemberProperty("LicenseClass", DataTypes.String).WithDataGenerationHints(DataGenerationHints.InterestingValue<string>(string.Empty)),
                    new MemberProperty("Restrictions", DataTypes.String).WithDataGenerationHints(DataGenerationHints.InterestingValue<string>(string.Empty)),
                    new MemberProperty("ExpirationDate", DataTypes.DateTime),
                    new NavigationProperty("Driver", "Driver_License", "License", "Driver"),
                },
                new EntityType("MappedEntityType")
                {
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Href", DataTypes.String).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(3)),   
                    new MemberProperty("Title", DataTypes.String).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(3)),   
                    new MemberProperty("HrefLang", DataTypes.String).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(3)),   
                    new MemberProperty("Type", DataTypes.String).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(3)),   
                    new MemberProperty("Length", DataTypes.Integer).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.MinValue<int>(100), DataGenerationHints.MaxValue<int>(200)),                                
                    new MemberProperty("BagOfPrimitiveToLinks", DataTypes.CollectionType.WithElementDataType(DataTypes.String)),
                    new MemberProperty("Logo", DataTypes.Binary.WithMaxLength(500)),
                    new MemberProperty("BagOfDecimals", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Decimal())),
                    new MemberProperty("BagOfDoubles", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Double)),
                    new MemberProperty("BagOfSingles", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Single)),
                    new MemberProperty("BagOfBytes", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Byte)),
                    new MemberProperty("BagOfInt16s", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int16)),
                    new MemberProperty("BagOfInt32s", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int32)),
                    new MemberProperty("BagOfInt64s", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int64)),
                    new MemberProperty("BagOfGuids", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Guid)),
                    new MemberProperty("BagOfDateTime", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.DateTime())),
                    new MemberProperty("BagOfComplexToCategories", DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithName("ComplexToCategory"))),
                    new MemberProperty("CollectionOfDateTimeOffset", DataTypes.CollectionType.WithElementDataType(DataTypes.DateTime.WithTimeZoneOffset(true))),
                    new MemberProperty("ComplexPhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("ComplexContactDetails", DataTypes.ComplexType.WithName("ContactDetails")),
                },
                new EntityType("Car")
                {
                    new MemberProperty("VIN", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Description", DataTypes.String.WithMaxLength(100).Nullable(true)),
                    new MemberProperty("Photo", DataTypes.Stream),
                    new MemberProperty("Video", DataTypes.Stream),
                    new HasStreamAnnotation(),
                },
                new ComplexType("AuditInfo")
                {
                    new MemberProperty("ModifiedDate", DataTypes.DateTime),
                    new MemberProperty("ModifiedBy", DataTypes.String.WithMaxLength(50)),
                    new MemberProperty("Concurrency", DataTypes.ComplexType.WithName("ConcurrencyInfo")),
                },
                new ComplexType("ConcurrencyInfo")
                {
                    new MemberProperty("Token", DataTypes.String.WithMaxLength(20)),
                    new MemberProperty("QueriedDateTime", DataTypes.DateTime.Nullable(true)),
                },
                new AssociationType("Login_SentMessages")
                {
                    new AssociationEnd("Sender", "Login", EndMultiplicity.One),
                    new AssociationEnd("Message", "Message", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Message", "FromUsername")
                        .ReferencesPrincipalProperties("Sender", "Username"),
                },
                new AssociationType("Login_ReceivedMessages")
                {
                    new AssociationEnd("Recipient", "Login", EndMultiplicity.One),
                    new AssociationEnd("Message", "Message", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Message", "ToUsername")
                        .ReferencesPrincipalProperties("Recipient", "Username"),
                },
                new AssociationType("Customer_CustomerInfo")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.One),
                    new AssociationEnd("Info", "CustomerInfo", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Login_Orders")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Orders", "Order", EndMultiplicity.Many),
                },
                new AssociationType("Customer_Orders")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Order", "Order", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Order", "CustomerId")
                        .ReferencesPrincipalProperties("Customer", "CustomerId"),
                },
                new AssociationType("Customer_Logins")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.One),
                    new AssociationEnd("Logins", "Login", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Logins", "CustomerId")
                        .ReferencesPrincipalProperties("Customer", "CustomerId"),
                },
                new AssociationType("Login_LastLogin")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.One),
                    new AssociationEnd("LastLogin", "LastLogin", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                        .WithDependentProperties("LastLogin", "Username")
                        .ReferencesPrincipalProperties("Login", "Username"),
                },
                new AssociationType("Order_OrderLines")
                {
                    new AssociationEnd("Order", "Order", EndMultiplicity.One),
                    new AssociationEnd("OrderLines", "OrderLine", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("OrderLines", "OrderId")
                        .ReferencesPrincipalProperties("Order", "OrderId"),
                },
                new AssociationType("Product_OrderLines")
                {
                    new AssociationEnd("Product", "Product", EndMultiplicity.One),
                    new AssociationEnd("OrderLines", "OrderLine", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("OrderLines", "ProductId")
                        .ReferencesPrincipalProperties("Product", "ProductId"),
                },
                new AssociationType("Products_RelatedProducts")
                {
                    new AssociationEnd("Product", "Product", EndMultiplicity.One),
                    new AssociationEnd("RelatedProducts", "Product", EndMultiplicity.Many),
                },
                new AssociationType("Product_ProductDetail")
                {
                    new AssociationEnd("Product", "Product", EndMultiplicity.One),
                    new AssociationEnd("ProductDetail", "ProductDetail", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                        .WithDependentProperties("ProductDetail", "ProductId")
                        .ReferencesPrincipalProperties("Product", "ProductId"),
                },
                new AssociationType("Product_ProductReview")
                {
                    new AssociationEnd("Product", "Product", EndMultiplicity.One),
                    new AssociationEnd("ProductReview", "ProductReview", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("ProductReview", "ProductId")
                        .ReferencesPrincipalProperties("Product", "ProductId"),
                },
                new AssociationType("Product_ProductPhoto")
                {
                    new AssociationEnd("Product", "Product", EndMultiplicity.One),
                    new AssociationEnd("ProductPhoto", "ProductPhoto", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("ProductPhoto", "ProductId")
                        .ReferencesPrincipalProperties("Product", "ProductId"),
                },
                new AssociationType("Husband_Wife")
                {
                    new AssociationEnd("Husband", "Customer", EndMultiplicity.ZeroOne) { Annotations = { new PrincipalAnnotation() } },
                    new AssociationEnd("Wife", "Customer", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Login_RSAToken")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.One),
                    new AssociationEnd("RSAToken", "RSAToken", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Login_PageViews")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.One),
                    new AssociationEnd("PageViews", "PageView", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("PageViews", "Username")
                        .ReferencesPrincipalProperties("Login", "Username"),
                },
                new AssociationType("Computer_ComputerDetail")
                {
                    new AssociationEnd("Computer", "Computer", EndMultiplicity.One) { Annotations = { new PrincipalAnnotation() } },
                    new AssociationEnd("ComputerDetail", "ComputerDetail", EndMultiplicity.One),
                },
                new AssociationType("Driver_License")
                {
                    new AssociationEnd("Driver", "Driver", EndMultiplicity.One),
                    new AssociationEnd("License", "License", EndMultiplicity.One),
                    new ReferentialConstraint()
                        .WithDependentProperties("License", "Name")
                        .ReferencesPrincipalProperties("Driver", "Name"),
                },
                new EntityType("Person")
                {
                    new MemberProperty("PersonId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.MinLength(3)),
                    new NavigationProperty("PersonMetadata", "Person_PersonMetadata", "Person", "PersonMetadata"),
                },
                new EntityType("Employee")
                {
                    BaseType = "Person", 
                    Properties =
                    {
                        new MemberProperty("ManagersPersonId", DataTypes.Integer),
                        new MemberProperty("Salary", DataTypes.Integer),
                        new MemberProperty("Title", DataTypes.String).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.MinLength(3)),
                    },
                    NavigationProperties = 
                    {
                        new NavigationProperty("Manager", "Employee_Manager", "Employee", "Manager"),
                    },
                },
                new EntityType("SpecialEmployee")
                {
                    BaseType = "Employee", 
                    Properties =
                    {
                        new MemberProperty("CarsVIN", DataTypes.Integer),
                        new MemberProperty("Bonus", DataTypes.Integer),
                        new MemberProperty("IsFullyVested", DataTypes.Boolean),
                    },
                    NavigationProperties = 
                    {
                        new NavigationProperty("Car", "SpecialEmployee_Car", "SpecialEmployee", "Car"),
                    }
                },
                new EntityType("PersonMetadata")
                {
                    new MemberProperty("PersonMetadataId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("PersonId", DataTypes.Integer),
                    new MemberProperty("PropertyName", DataTypes.String),
                    new MemberProperty("PropertyValue", DataTypes.String),
                    new NavigationProperty("Person", "Person_PersonMetadata", "PersonMetadata", "Person"),
                },
                new AssociationType("Person_PersonMetadata")
                {
                    new AssociationEnd("Person", "Person", EndMultiplicity.One),
                    new AssociationEnd("PersonMetadata", "PersonMetadata", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("PersonMetadata", "PersonId")
                        .ReferencesPrincipalProperties("Person", "PersonId"),
                },
                new AssociationType("Employee_Manager")
                {
                    new AssociationEnd("Employee", "Employee", EndMultiplicity.Many),
                    new AssociationEnd("Manager", "Employee", EndMultiplicity.One),
                    new ReferentialConstraint()
                        .WithDependentProperties("Employee", "ManagersPersonId")
                        .ReferencesPrincipalProperties("Manager", "PersonId"),
                },
                new AssociationType("SpecialEmployee_Car")
                {
                    new AssociationEnd("SpecialEmployee", "SpecialEmployee", EndMultiplicity.Many),
                    new AssociationEnd("Car", "Car", EndMultiplicity.One),
                    new ReferentialConstraint()
                        .WithDependentProperties("SpecialEmployee", "CarsVIN")
                        .ReferencesPrincipalProperties("Car", "VIN"),
                },
                new Function("GetPrimitiveString")
                {
                    ReturnType = EdmDataTypes.String().NotNullable(),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Get,
                        },
                        new FunctionBodyAnnotation
                        {
                            FunctionBody = CommonQueryBuilder.Constant("Foo"),
                        },
                    },
                },
                new Function("GetSpecificCustomer")
                {
                    ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.EntityType.WithName("Customer")),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Get,
                            ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IEnumerable,
                        },
                        new FunctionBodyAnnotation
                        {
                            FunctionBodyGenerator = (schema) =>
                                {
                                    var entitySet = schema.GetDefaultEntityContainer().EntitySets.Where(set => set.Name.Equals("Customer")).Single();
                                    return CommonQueryBuilder.Root(entitySet).Where(c => c.Property("Name").EqualTo(CommonQueryBuilder.FunctionParameterReference("Name")));
                                },
                        },
                    },
                    Parameters =
                    {
                        new FunctionParameter("Name", EdmDataTypes.String().NotNullable()),
                    },
                },
                new Function("GetCustomerCount")
                {
                    ReturnType = EdmDataTypes.Int32.NotNullable(),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Get,
                        },
                        new FunctionBodyAnnotation
                        {
                            FunctionBodyGenerator = (schema) =>
                                {
                                    var entitySet = schema.GetDefaultEntityContainer().EntitySets.Where(set => set.Name.Equals("Customer")).Single();
                                    return CommonQueryBuilder.Root(entitySet).Count();
                                },
                        },
                    },
                },
                new Function("GetArgumentPlusOne")
                {
                    ReturnType = EdmDataTypes.Int32.NotNullable(),
                    Annotations =
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Get,
                        },
                        new FunctionBodyAnnotation
                        {
                            FunctionBody = CommonQueryBuilder.Add(CommonQueryBuilder.FunctionParameterReference("arg1"), CommonQueryBuilder.Constant((int)1)),
                        },
                    },
                    Parameters = 
                    {
                        new FunctionParameter("arg1", EdmDataTypes.Int32.NotNullable()),
                    },
                },
                new Function("EntityProjectionReturnsCollectionOfComplexTypes")
                {
                    ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithName("ContactDetails")),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation { Method = HttpVerb.Get, ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IEnumerable, },
                        new FunctionBodyAnnotation 
                        {
                            FunctionBodyGenerator = (schema) =>
                                {
                                    var entitySet = schema.GetDefaultEntityContainer().EntitySets.Where(set => set.Name.Equals("Customer")).Single();
                                    return CommonQueryBuilder.Root(entitySet).Select(e => e.Property("PrimaryContactInfo"));
                                },
                        },
                    },
                },
            };

            // TODO: Add more types for other features like relationship link, streams.
            if (this.RunStability == RunStability.Unstable)
            {
                // Add a service operation for each base entity type
                new AddRootServiceOperationsFixup().Fixup(model);

                // Add some WebInvoke service opeartions covering entity types with navigation, inheritance, concurrency
                this.AddWebInvokeServiceOperations(model);
            }

            // Apply default fixups
            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup("DefaultNamespace").Fixup(model);
            new AddDefaultContainerFixup().Fixup(model);
            return model;
        }

        private void AddWebInvokeServiceOperations(EntityModelSchema model)
        {
            model.Add(
                new Function("WebInvokeGetCustomer")
                {
                    ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.EntityType.WithName("Customer")),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Post,
                            ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                        },
                        AddRootServiceOperationsFixup.BuildReturnEntitySetFunctionBody("Customer"),
                    },
                });

            model.Add(
                new Function("WebInvokeGetOrder")
                {
                    ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.EntityType.WithName("Order")),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Post,
                            ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                        },
                        AddRootServiceOperationsFixup.BuildReturnEntitySetFunctionBody("Order"),
                    },
                });

            model.Add(
                new Function("WebInvokeGetOrderLine")
                {
                    ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.EntityType.WithName("OrderLine")),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Post,
                            ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                        },
                        AddRootServiceOperationsFixup.BuildReturnEntitySetFunctionBody("OrderLine"),
                    },
                });
        }
    }
}
