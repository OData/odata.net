//---------------------------------------------------------------------
// <copyright file="DefaultConceptualModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Model aimed at covering C-Space concepts
    /// </summary>
    [ImplementationName(typeof(IModelGenerator), "Default", HelpText = "The default model covering all C-Space concepts")]
    public class DefaultConceptualModelGenerator : IModelGenerator
    {
        private string defaultContainerName;
        private string defaultDefaultNamespaceName;

        /// <summary>
        /// Initializes a new instance of the DefaultConceptualModelGenerator class.
        /// </summary>
        public DefaultConceptualModelGenerator()
            : this("DefaultContainer", "DefaultNamespace")
        {
        }

        /// <summary>
        /// Initializes a new instance of the DefaultConceptualModelGenerator class.
        /// </summary>
        /// <param name="defaultContainerName">The default name for the entity container.</param>
        /// <param name="defaultDefaultNamespaceName">The default name for the namespace.</param>
        public DefaultConceptualModelGenerator(string defaultContainerName, string defaultDefaultNamespaceName)
        {
            this.defaultContainerName = defaultContainerName;
            this.defaultDefaultNamespaceName = defaultDefaultNamespaceName;
        }

        /// <summary>
        /// Generate the model.
        /// </summary>
        /// <returns> Valid <see cref="EntityModelSchema"/>.</returns>
        public virtual EntityModelSchema GenerateModel()
        {
            return this.GenerateModel(this.defaultContainerName, this.defaultDefaultNamespaceName);
        }

        /// <summary>
        /// Generate the model.
        /// </summary>
        /// <param name="containerName">Container Name for Model</param>
        /// <param name="namespaceName">Namespace Name for Model</param>
        /// <returns> Valid <see cref="EntityModelSchema"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is unavoidable, we need to create entire model here.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Locals created by the compiler.")]
        public virtual EntityModelSchema GenerateModel(string containerName, string namespaceName)
        {
            var model = new EntityModelSchema()
            {
                new EntityType("Customer")
                {
                    new MemberProperty("CustomerId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(100)),
                    new MemberProperty("ContactInfo", DataTypes.ComplexType.WithName("ContactDetails")),
                    new MemberProperty("Auditing", DataTypes.ComplexType.WithName("AuditInfo")),
                    new NavigationProperty("Orders", "Customer_Orders", "Customer", "Order"),
                    new NavigationProperty("Logins", "Customer_Logins", "Customer", "Logins"),
                    new NavigationProperty("Husband", "Husband_Wife", "Wife", "Husband"),
                    new NavigationProperty("Wife", "Husband_Wife", "Husband", "Wife"),
                    new NavigationProperty("Info", "Customer_CustomerInfo", "Customer", "Info"),
                },
                new ComplexType("Phone")
                {
                    new MemberProperty("PhoneNumber", DataTypes.String.WithMaxLength(16)),
                    new MemberProperty("Extension", DataTypes.String.WithMaxLength(16).Nullable(true)),
                },
                new ComplexType("ContactDetails")
                {
                    new MemberProperty("Email", DataTypes.String.WithMaxLength(32)),
                    new MemberProperty("HomePhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("WorkPhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("MobilePhone", DataTypes.ComplexType.WithName("Phone")),
                },
                new EntityType("Barcode")
                {
                    new MemberProperty("Code", DataTypes.Binary.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("ProductId", DataTypes.Integer),
                    new MemberProperty("Text", DataTypes.String),
                    new NavigationProperty("Product", "Product_Barcodes", "Barcodes", "Product"),
                    new NavigationProperty("BadScans", "Barcode_IncorrectScanExpected", "Barcode", "Expected"),
                    new NavigationProperty("Detail", "Barcode_BarcodeDetail", "Barcode", "BarcodeDetail"),
                },
                new EntityType("IncorrectScan")
                {
                    new MemberProperty("IncorrectScanId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("ExpectedCode", DataTypes.Binary.WithMaxLength(50)),
                    new MemberProperty("ActualCode", DataTypes.Binary.WithMaxLength(50).Nullable(true)),
                    new MemberProperty("ScanDate", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("Details", DataTypes.String),
                    new NavigationProperty("ExpectedBarcode", "Barcode_IncorrectScanExpected", "Expected", "Barcode"),
                    new NavigationProperty("ActualBarcode", "Barcode_IncorrectScanActual", "Actual", "Barcode"),
                },
                new EntityType("BarcodeDetail")
                {
                    new MemberProperty("Code", DataTypes.Binary.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("RegisteredTo", DataTypes.String),
                },
                new EntityType("Complaint")
                {
                    new MemberProperty("ComplaintId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("CustomerId", DataTypes.Integer.Nullable(true)),
                    new MemberProperty("Logged", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("Details", DataTypes.String),
                    new NavigationProperty("Customer", "Customer_Complaints", "Complaints", "Customer"),
                    new NavigationProperty("Resolution", "Complaint_Resolution", "Complaint", "Resolution"),
                },
                new EntityType("Resolution")
                {
                    new MemberProperty("ResolutionId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Details", DataTypes.String),
                    new NavigationProperty("Complaint", "Complaint_Resolution", "Resolution", "Complaint"),
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
                     new NavigationProperty("SuspiciousActivity", "Login_SuspiciousActivity", "Login", "Activity"),
                },
                new EntityType("SuspiciousActivity")
                {
                    new MemberProperty("SuspiciousActivityId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Activity", DataTypes.String),
                },
                new EntityType("SmartCard")
                {
                    new MemberProperty("Username", DataTypes.String.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("CardSerial", DataTypes.String),
                    new MemberProperty("Issued", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new NavigationProperty("Login", "Login_SmartCard", "SmartCard", "Login"),
                    new NavigationProperty("LastLogin", "LastLogin_SmartCard", "SmartCard", "LastLogin"),
                },
                new EntityType("RSAToken")
                {
                    new MemberProperty("Serial", DataTypes.String.WithMaxLength(20)) { IsPrimaryKey = true },
                    new MemberProperty("Issued", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new NavigationProperty("Login", "Login_RSAToken", "RSAToken", "Login"),
                },
                new EntityType("PasswordReset")
                {
                    new MemberProperty("ResetNo", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Username", DataTypes.String.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("TempPassword", DataTypes.String),
                    new MemberProperty("EmailedTo", DataTypes.String),
                    new NavigationProperty("Login", "Login_PasswordResets", "PasswordResets", "Login"),
                },
                new EntityType("PageView")
                {
                    new MemberProperty("PageViewId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Username", DataTypes.String.WithMaxLength(50)),
                    new MemberProperty("Viewed", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("PageUrl", DataTypes.String.WithMaxLength(500)),
                    new NavigationProperty("Login", "Login_PageViews", "PageViews", "Login"),
                },
                new EntityType("ProductPageView")
                {
                    BaseType = "PageView",
                    Properties = 
                    {
                        new MemberProperty("ProductId", DataTypes.Integer),
                    },
                    NavigationProperties = 
                    {
                        new NavigationProperty("Product", "Product_ProductPageViews", "ProductPageViews", "Product"),
                    }
                },
                new EntityType("LastLogin")
                {
                    new MemberProperty("Username", DataTypes.String.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("LoggedIn", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("LoggedOut", DataTypes.DateTime.WithTimeZoneOffset(true).Nullable(true)),
                    new NavigationProperty("Login", "Login_LastLogin", "LastLogin", "Login"),
                },
                new EntityType("Message")
                {
                    new MemberProperty("MessageId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("FromUsername", DataTypes.String.WithMaxLength(50)) { IsPrimaryKey = true },
                    new MemberProperty("ToUsername", DataTypes.String.WithMaxLength(50)),
                    new MemberProperty("Sent", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("Subject", DataTypes.String),
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
                    new NavigationProperty("OrderLines", "Order_OrderLines", "Order", "OrderLines"),
                    new NavigationProperty("Notes", "Order_OrderNotes", "Order", "Notes"),
                    new NavigationProperty("Login", "Login_Orders", "Orders", "Login"),
                },
                new EntityType("OrderNote")
                {
                    new MemberProperty("NoteId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Note", DataTypes.String),
                    new NavigationProperty("Order", "Order_OrderNotes", "Notes", "Order"),
                },
                new EntityType("OrderQualityCheck")
                {
                    new MemberProperty("OrderId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("CheckedBy", DataTypes.String),
                    new MemberProperty("CheckedDateTime", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new NavigationProperty("Order", "Order_QualityCheck", "QualityCheck", "Order"),
                },
                new EntityType("OrderLine")
                {
                    new MemberProperty("OrderId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Quantity", DataTypes.Integer),
                    new MemberProperty("ConcurrencyToken", DataTypes.String) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new NavigationProperty("Order", "Order_OrderLines", "OrderLines", "Order"),
                    new NavigationProperty("Product", "Product_OrderLines", "OrderLines", "Product"),
                },
                new EntityType("BackOrderLine")
                {
                    BaseType = "OrderLine",
                    Properties = 
                    {
                        new MemberProperty("ETA", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    },
                    NavigationProperties = 
                    {
                        new NavigationProperty("Supplier", "Supplier_BackOrderLines", "BackOrderLines", "Supplier"),
                    }
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
                    new MemberProperty("Description", DataTypes.String.Nullable(true).WithMaxLength(1000)),
                    new MemberProperty("Dimensions", DataTypes.ComplexType.WithName("Dimensions")),
                    new MemberProperty("BaseConcurrency", DataTypes.String) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("ComplexConcurrency", DataTypes.ComplexType.WithName("ConcurrencyInfo")),
                    new MemberProperty("NestedComplexConcurrency", DataTypes.ComplexType.WithName("AuditInfo")),
                    new NavigationProperty("Suppliers", "Products_Suppliers", "Products", "Suppliers"),
                    new NavigationProperty("Replaces", "DiscontinuedProduct_Replacement", "Replacement", "DiscontinuedProduct"),
                    new NavigationProperty("Detail", "Product_ProductDetail", "Product", "ProductDetail"),
                    new NavigationProperty("Reviews", "Product_ProductReview", "Product", "ProductReview"),
                    new NavigationProperty("Photos", "Product_ProductPhoto", "Product", "ProductPhoto"),
                    new NavigationProperty("Barcodes", "Product_Barcodes", "Product", "Barcodes"),
                },
                new EntityType("DiscontinuedProduct")
                {
                    BaseType = "Product",
                    Properties = 
                    {
                        new MemberProperty("Discontinued", DataTypes.DateTime.WithTimeZoneOffset(true)),
                        new MemberProperty("ReplacementProductId", DataTypes.Integer.Nullable()),
                    },
                    NavigationProperties = 
                    {
                        new NavigationProperty("ReplacedBy", "DiscontinuedProduct_Replacement", "DiscontinuedProduct", "Replacement"),
                    }
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
                    new NavigationProperty("Features", "ProductWebFeature_ProductReview", "ProductReview", "ProductWebFeature"),
                },
                new EntityType("ProductPhoto")
                {
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("PhotoId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Photo", DataTypes.Binary),
                    new NavigationProperty("Features", "ProductWebFeature_ProductPhoto", "ProductPhoto", "ProductWebFeature"),
                },
                new EntityType("ProductWebFeature")
                {
                    new MemberProperty("FeatureId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("ProductId", DataTypes.Integer.Nullable()),
                    new MemberProperty("PhotoId", DataTypes.Integer.Nullable()),
                    new MemberProperty("ReviewId", DataTypes.Integer.Nullable()),
                    new MemberProperty("Heading", DataTypes.String),
                    new NavigationProperty("Review", "ProductWebFeature_ProductReview", "ProductWebFeature", "ProductReview"),
                    new NavigationProperty("Photo", "ProductWebFeature_ProductPhoto", "ProductWebFeature", "ProductPhoto"),
                },
                new EntityType("Supplier")
                {
                    new MemberProperty("SupplierId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String),
                    new NavigationProperty("Products", "Products_Suppliers", "Suppliers", "Products"),
                    new NavigationProperty("BackOrderLines", "Supplier_BackOrderLines", "Supplier", "BackOrderLines"),
                    new NavigationProperty("Logo", "Supplier_SupplierLogo", "Supplier", "Logo"),
                },
                new EntityType("SupplierLogo")
                {
                    new MemberProperty("SupplierId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Logo", DataTypes.Binary.WithMaxLength(500)),
                },
                new EntityType("SupplierInfo")
                {
                    new MemberProperty("SupplierInfoId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Information", DataTypes.String),
                    new NavigationProperty("Supplier", "Supplier_SupplierInfo", "Info", "Supplier"),
                },
                new EntityType("CustomerInfo")
                {
                    new MemberProperty("CustomerInfoId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Information", DataTypes.String),
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
                    new MemberProperty("Manufacturer", DataTypes.String),
                    new MemberProperty("Model", DataTypes.String),
                    new MemberProperty("Serial", DataTypes.String),
                    new MemberProperty("Specifications", DataTypes.String),
                    new MemberProperty("PurchaseDate", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("Dimensions", DataTypes.ComplexType.WithName("Dimensions")),
                    new NavigationProperty("Computer", "Computer_ComputerDetail", "ComputerDetail", "Computer"),
                },
                new EntityType("Driver")
                {
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(100)) { IsPrimaryKey = true },
                    new MemberProperty("BirthDate", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new NavigationProperty("License", "Driver_License", "Driver", "License"),
                },
                new EntityType("License")
                {
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(100)) { IsPrimaryKey = true },
                    new MemberProperty("LicenseNumber", DataTypes.String),
                    new MemberProperty("LicenseClass", DataTypes.String),
                    new MemberProperty("Restrictions", DataTypes.String),
                    new MemberProperty("ExpirationDate", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new NavigationProperty("Driver", "Driver_License", "License", "Driver"),
                },
                new ComplexType("AuditInfo")
                {
                    new MemberProperty("ModifiedDate", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("ModifiedBy", DataTypes.String.WithMaxLength(50)),
                    new MemberProperty("Concurrency", DataTypes.ComplexType.WithName("ConcurrencyInfo")),
                },
                new ComplexType("ConcurrencyInfo")
                {
                    new MemberProperty("Token", DataTypes.String.WithMaxLength(20)) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("QueriedDateTime", DataTypes.DateTime.WithTimeZoneOffset(true).Nullable(true)),
                },
                new AssociationType("Customer_Complaints")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Complaints", "Complaint", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Complaints", "CustomerId")
                        .ReferencesPrincipalProperties("Customer", "CustomerId"),
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
                new AssociationType("Supplier_SupplierInfo")
                {
                    new AssociationEnd("Supplier", "Supplier", EndMultiplicity.One, OperationAction.Cascade),
                    new AssociationEnd("Info", "SupplierInfo", EndMultiplicity.Many),
                },
                new AssociationType("Login_Orders")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Orders", "Order", EndMultiplicity.Many),
                },
                new AssociationType("Order_OrderNotes")
                {
                    new AssociationEnd("Order", "Order", EndMultiplicity.One, OperationAction.Cascade),
                    new AssociationEnd("Notes", "OrderNote", EndMultiplicity.Many),
                },
                new AssociationType("Order_QualityCheck")
                {
                    new AssociationEnd("Order", "Order", EndMultiplicity.One),
                    new AssociationEnd("QualityCheck", "OrderQualityCheck", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                        .WithDependentProperties("QualityCheck", "OrderId")
                        .ReferencesPrincipalProperties("Order", "OrderId"),
                },
                new AssociationType("Supplier_SupplierLogo")
                {
                    new AssociationEnd("Supplier", "Supplier", EndMultiplicity.One),
                    new AssociationEnd("Logo", "SupplierLogo", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                        .WithDependentProperties("Logo", "SupplierId")
                        .ReferencesPrincipalProperties("Supplier", "SupplierId"),
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
                new AssociationType("LastLogin_SmartCard")
                {
                    new AssociationEnd("LastLogin", "LastLogin", EndMultiplicity.ZeroOne) { Annotations = { new PrincipalAnnotation() } },
                    new AssociationEnd("SmartCard", "SmartCard", EndMultiplicity.ZeroOne),
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
                new AssociationType("Products_Suppliers")
                {
                    new AssociationEnd("Products", "Product", EndMultiplicity.Many),
                    new AssociationEnd("Suppliers", "Supplier", EndMultiplicity.Many),
                },
                new AssociationType("Supplier_BackOrderLines")
                {
                    new AssociationEnd("Supplier", "Supplier", EndMultiplicity.ZeroOne),
                    new AssociationEnd("BackOrderLines", "BackOrderLine", EndMultiplicity.Many)
                },
                new AssociationType("DiscontinuedProduct_Replacement")
                {
                    new AssociationEnd("Replacement", "Product", EndMultiplicity.ZeroOne),
                    new AssociationEnd("DiscontinuedProduct", "DiscontinuedProduct", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("DiscontinuedProduct", "ReplacementProductId")
                        .ReferencesPrincipalProperties("Replacement", "ProductId"),
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
                new AssociationType("ProductWebFeature_ProductPhoto")
                {
                    new AssociationEnd("ProductWebFeature", "ProductWebFeature", EndMultiplicity.Many),
                    new AssociationEnd("ProductPhoto", "ProductPhoto", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                        .WithDependentProperties("ProductWebFeature", "PhotoId", "ProductId")
                        .ReferencesPrincipalProperties("ProductPhoto", "PhotoId", "ProductId"),
                },
                new AssociationType("ProductWebFeature_ProductReview")
                {
                    new AssociationEnd("ProductWebFeature", "ProductWebFeature", EndMultiplicity.Many),
                    new AssociationEnd("ProductReview", "ProductReview", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                        .WithDependentProperties("ProductWebFeature", "ReviewId", "ProductId")
                        .ReferencesPrincipalProperties("ProductReview", "ReviewId", "ProductId"),
                },
                new AssociationType("Complaint_Resolution")
                {
                    new AssociationEnd("Complaint", "Complaint", EndMultiplicity.One),
                    new AssociationEnd("Resolution", "Resolution", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Barcode_IncorrectScanExpected")
                {
                    new AssociationEnd("Barcode", "Barcode", EndMultiplicity.One),
                    new AssociationEnd("Expected", "IncorrectScan", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Expected", "ExpectedCode")
                        .ReferencesPrincipalProperties("Barcode", "Code"),
                },
                new AssociationType("Husband_Wife")
                {
                    new AssociationEnd("Husband", "Customer", EndMultiplicity.ZeroOne) { Annotations = { new PrincipalAnnotation() } },
                    new AssociationEnd("Wife", "Customer", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Barcode_IncorrectScanActual")
                {
                    new AssociationEnd("Barcode", "Barcode", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Actual", "IncorrectScan", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Actual", "ActualCode")
                        .ReferencesPrincipalProperties("Barcode", "Code"),
                },
                new AssociationType("Product_Barcodes")
                {
                    new AssociationEnd("Product", "Product", EndMultiplicity.One),
                    new AssociationEnd("Barcodes", "Barcode", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Barcodes", "ProductId")
                        .ReferencesPrincipalProperties("Product", "ProductId"),
                },
                new AssociationType("Barcode_BarcodeDetail")
                {
                    new AssociationEnd("Barcode", "Barcode", EndMultiplicity.One),
                    new AssociationEnd("BarcodeDetail", "BarcodeDetail", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                        .WithDependentProperties("BarcodeDetail", "Code")
                        .ReferencesPrincipalProperties("Barcode", "Code"),
                },
                new AssociationType("Login_SuspiciousActivity")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Activity", "SuspiciousActivity", EndMultiplicity.Many),
                },
                new AssociationType("Login_RSAToken")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.One),
                    new AssociationEnd("RSAToken", "RSAToken", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Login_SmartCard")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.One),
                    new AssociationEnd("SmartCard", "SmartCard", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                        .WithDependentProperties("SmartCard", "Username")
                        .ReferencesPrincipalProperties("Login", "Username"),
                },
                new AssociationType("Login_PasswordResets")
                {
                    new AssociationEnd("Login", "Login", EndMultiplicity.One),
                    new AssociationEnd("PasswordResets", "PasswordReset", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("PasswordResets", "Username")
                        .ReferencesPrincipalProperties("Login", "Username"),
                },
                new AssociationType("Product_ProductPageViews")
                {
                    new AssociationEnd("Product", "Product", EndMultiplicity.One),
                    new AssociationEnd("ProductPageViews", "ProductPageView", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("ProductPageViews", "ProductId")
                        .ReferencesPrincipalProperties("Product", "ProductId"),
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
            };

            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup(namespaceName).Fixup(model);
            new AddDefaultContainerFixup(containerName).Fixup(model);

            return model;
        }
    }
}
