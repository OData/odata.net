//---------------------------------------------------------------------
// <copyright file="ODataV2ModelGenerator.ObjectLayer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if SILVERLIGHT || WINDOWS_PHONE
namespace DefaultNamespace {
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    
    
    public partial class Phone {
        
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }
        
        public Phone() {
        }
    }
    
    public partial class ContactDetails {
        
        public DefaultNamespace.Phone HomePhone { get; set; }
        public DefaultNamespace.Phone WorkPhone { get; set; }
        
        public ContactDetails() {
        }
    }
    
    public partial class ComplexToCategory {
        
        public string Term { get; set; }
        public string Scheme { get; set; }
        public string Label { get; set; }
        
        public ComplexToCategory() {
        }
    }
    
    public partial class Dimensions {
        
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }
        
        public Dimensions() {
        }
    }
    
    public partial class AuditInfo {
        
        public System.DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DefaultNamespace.ConcurrencyInfo Concurrency { get; set; }
        
        public AuditInfo() {
        }
    }
    
    public partial class ConcurrencyInfo {
        
        public string Token { get; set; }
        public Nullable<System.DateTime> QueriedDateTime { get; set; }
        
        public ConcurrencyInfo() {
        }
    }
    
    [Key("CustomerId")]
    public partial class Customer {
        
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public DefaultNamespace.AuditInfo Auditing { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Order> Orders { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Login> Logins { get; set; }
        public DefaultNamespace.Customer Husband { get; set; }
        public DefaultNamespace.Customer Wife { get; set; }
        public DefaultNamespace.CustomerInfo Info { get; set; }
        
        public Customer() {
            this.Orders = new System.Collections.Generic.List<DefaultNamespace.Order>();
            this.Logins = new System.Collections.Generic.List<DefaultNamespace.Login>();
        }
    }
    
    [Key("Code")]
    public partial class Barcode {
        
        public int Code { get; set; }
        public int ProductId { get; set; }
        public string Text { get; set; }
        public DefaultNamespace.Product Product { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.IncorrectScan> BadScans { get; set; }
        public DefaultNamespace.BarcodeDetail Detail { get; set; }
        
        public Barcode() {
            this.BadScans = new System.Collections.Generic.List<DefaultNamespace.IncorrectScan>();
        }
    }
    
    [Key("IncorrectScanId")]
    public partial class IncorrectScan {
        
        public int IncorrectScanId { get; set; }
        public int ExpectedCode { get; set; }
        public Nullable<int> ActualCode { get; set; }
        public string Details { get; set; }
        public DefaultNamespace.Barcode ExpectedBarcode { get; set; }
        public DefaultNamespace.Barcode ActualBarcode { get; set; }
        
        public IncorrectScan() {
        }
    }
    
    [Key("Code")]
    public partial class BarcodeDetail {
        
        public int Code { get; set; }
        public string RegisteredTo { get; set; }
        
        public BarcodeDetail() {
        }
    }
    
    [Key("ComplaintId")]
    public partial class Complaint {
        
        public int ComplaintId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public System.DateTime Logged { get; set; }
        public string Details { get; set; }
        public DefaultNamespace.Customer Customer { get; set; }
        public DefaultNamespace.Resolution Resolution { get; set; }
        
        public Complaint() {
        }
    }
    
    [Key("ResolutionId")]
    public partial class Resolution {
        
        public int ResolutionId { get; set; }
        public string Details { get; set; }
        public DefaultNamespace.Complaint Complaint { get; set; }
        
        public Resolution() {
        }
    }
    
    [Key("Username")]
    public partial class Login {
        
        public string Username { get; set; }
        public int CustomerId { get; set; }
        public DefaultNamespace.Customer Customer { get; set; }
        public DefaultNamespace.LastLogin LastLogin { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Message> SentMessages { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Message> ReceivedMessages { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Order> Orders { get; set; }
        
        public Login() {
            this.SentMessages = new System.Collections.Generic.List<DefaultNamespace.Message>();
            this.ReceivedMessages = new System.Collections.Generic.List<DefaultNamespace.Message>();
            this.Orders = new System.Collections.Generic.List<DefaultNamespace.Order>();
        }
    }
    
    [Key("SuspiciousActivityId")]
    public partial class SuspiciousActivity {
        
        public int SuspiciousActivityId { get; set; }
        public string Activity { get; set; }
        
        public SuspiciousActivity() {
        }
    }
    
    [Key("Username")]
    public partial class SmartCard {
        
        public string Username { get; set; }
        public string CardSerial { get; set; }
        public System.DateTime Issued { get; set; }
        public DefaultNamespace.Login Login { get; set; }
        public DefaultNamespace.LastLogin LastLogin { get; set; }
        
        public SmartCard() {
        }
    }
    
    [Key("Serial")]
    public partial class RSAToken {
        
        public string Serial { get; set; }
        public System.DateTime Issued { get; set; }
        public DefaultNamespace.Login Login { get; set; }
        
        public RSAToken() {
        }
    }
    
    [Key("ResetNo", "Username")]
    public partial class PasswordReset {
        
        public int ResetNo { get; set; }
        public string Username { get; set; }
        public string TempPassword { get; set; }
        public string EmailedTo { get; set; }
        public DefaultNamespace.Login Login { get; set; }
        
        public PasswordReset() {
        }
    }
    
    [Key("PageViewId")]
    public partial class PageView {
        
        public int PageViewId { get; set; }
        public string Username { get; set; }
        public System.DateTime Viewed { get; set; }
        public string PageUrl { get; set; }
        public DefaultNamespace.Login Login { get; set; }
        
        public PageView() {
        }
    }
    
    public partial class ProductPageView : DefaultNamespace.PageView {
        
        public int ProductId { get; set; }
        
        public ProductPageView() {
        }
    }
    
    [Key("Username")]
    public partial class LastLogin {
        
        public string Username { get; set; }
        public System.DateTime LoggedIn { get; set; }
        public Nullable<System.DateTime> LoggedOut { get; set; }
        public DefaultNamespace.Login Login { get; set; }
        
        public LastLogin() {
        }
    }
    
    [Key("MessageId", "FromUsername")]
    public partial class Message {
        
        public int MessageId { get; set; }
        public string FromUsername { get; set; }
        public string ToUsername { get; set; }
        public System.DateTime Sent { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public DefaultNamespace.Login Sender { get; set; }
        public DefaultNamespace.Login Recipient { get; set; }
        
        public Message() {
        }
    }
    
    [Key("OrderId")]
    public partial class Order {
        
        public int OrderId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public DefaultNamespace.ConcurrencyInfo Concurrency { get; set; }
        public DefaultNamespace.Customer Customer { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.OrderLine> OrderLines { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.OrderNote> Notes { get; set; }
        public DefaultNamespace.Login Login { get; set; }
        
        public Order() {
            this.OrderLines = new System.Collections.Generic.List<DefaultNamespace.OrderLine>();
            this.Notes = new System.Collections.Generic.List<DefaultNamespace.OrderNote>();
        }
    }
    
    [Key("NoteId")]
    public partial class OrderNote {
        
        public int NoteId { get; set; }
        public string Note { get; set; }
        public DefaultNamespace.Order Order { get; set; }
        
        public OrderNote() {
        }
    }
    
    [Key("OrderId")]
    public partial class OrderQualityCheck {
        
        public int OrderId { get; set; }
        public string CheckedBy { get; set; }
        public System.DateTime CheckedDateTime { get; set; }
        public DefaultNamespace.Order Order { get; set; }
        
        public OrderQualityCheck() {
        }
    }
    
    [Key("OrderId", "ProductId")]
    public partial class OrderLine {
        
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ConcurrencyToken { get; set; }
        public DefaultNamespace.Order Order { get; set; }
        public DefaultNamespace.Product Product { get; set; }
        
        public OrderLine() {
        }
    }
    
    public partial class BackOrderLine : DefaultNamespace.OrderLine {
        
        public BackOrderLine() {
        }
    }
    
    public partial class BackOrderLine2 : DefaultNamespace.BackOrderLine {
        
        public BackOrderLine2() {
        }
    }
    
    [Key("ProductId")]
    public partial class Product {
        
        public int ProductId { get; set; }
        public string Description { get; set; }
        public DefaultNamespace.Dimensions Dimensions { get; set; }
        public string BaseConcurrency { get; set; }
        public DefaultNamespace.ConcurrencyInfo ComplexConcurrency { get; set; }
        public DefaultNamespace.AuditInfo NestedComplexConcurrency { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Product> RelatedProducts { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Supplier> Suppliers { get; set; }
        public DefaultNamespace.ProductDetail Detail { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.ProductReview> Reviews { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.ProductPhoto> Photos { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Barcode> Barcodes { get; set; }
        
        public Product() {
            this.RelatedProducts = new System.Collections.Generic.List<DefaultNamespace.Product>();
            this.Suppliers = new System.Collections.Generic.List<DefaultNamespace.Supplier>();
            this.Reviews = new System.Collections.Generic.List<DefaultNamespace.ProductReview>();
            this.Photos = new System.Collections.Generic.List<DefaultNamespace.ProductPhoto>();
            this.Barcodes = new System.Collections.Generic.List<DefaultNamespace.Barcode>();
        }
    }
    
    public partial class DiscontinuedProduct : DefaultNamespace.Product {
        
        public System.DateTime Discontinued { get; set; }
        public Nullable<int> ReplacementProductId { get; set; }
        
        public DiscontinuedProduct() {
        }
    }
    
    [Key("ProductId")]
    public partial class ProductDetail {
        
        public int ProductId { get; set; }
        public string Details { get; set; }
        public DefaultNamespace.Product Product { get; set; }
        
        public ProductDetail() {
        }
    }
    
    [Key("ProductId", "ReviewId")]
    public partial class ProductReview {
        
        public int ProductId { get; set; }
        public int ReviewId { get; set; }
        public string Review { get; set; }
        public DefaultNamespace.Product Product { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.ProductWebFeature> Features { get; set; }
        
        public ProductReview() {
            this.Features = new System.Collections.Generic.List<DefaultNamespace.ProductWebFeature>();
        }
    }
    
    [Key("ProductId", "PhotoId")]
    public partial class ProductPhoto {
        
        public int ProductId { get; set; }
        public int PhotoId { get; set; }
        public byte[] Photo { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.ProductWebFeature> Features { get; set; }
        
        public ProductPhoto() {
            this.Features = new System.Collections.Generic.List<DefaultNamespace.ProductWebFeature>();
        }
    }
    
    [Key("FeatureId")]
    public partial class ProductWebFeature {
        
        public int FeatureId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> PhotoId { get; set; }
        public int ReviewId { get; set; }
        public string Heading { get; set; }
        public DefaultNamespace.ProductReview Review { get; set; }
        public DefaultNamespace.ProductPhoto Photo { get; set; }
        
        public ProductWebFeature() {
        }
    }
    
    [Key("SupplierId")]
    public partial class Supplier {
        
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public System.Collections.Generic.List<DefaultNamespace.Product> Products { get; set; }
        public DefaultNamespace.SupplierLogo Logo { get; set; }
        
        public Supplier() {
            this.Products = new System.Collections.Generic.List<DefaultNamespace.Product>();
        }
    }
    
    [Key("SupplierId")]
    public partial class SupplierLogo {
        
        public int SupplierId { get; set; }
        public byte[] Logo { get; set; }
        
        public SupplierLogo() {
        }
    }
    
    [Key("SupplierInfoId")]
    public partial class SupplierInfo {
        
        public int SupplierInfoId { get; set; }
        public string Information { get; set; }
        public DefaultNamespace.Supplier Supplier { get; set; }
        
        public SupplierInfo() {
        }
    }
    
    [Key("CustomerInfoId")]
    public partial class CustomerInfo {
        
        public int CustomerInfoId { get; set; }
        public string Information { get; set; }
        
        public CustomerInfo() {
        }
    }
    
    [Key("ComputerId")]
    public partial class Computer {
        
        public int ComputerId { get; set; }
        public string Name { get; set; }
        public DefaultNamespace.ComputerDetail ComputerDetail { get; set; }
        
        public Computer() {
        }
    }
    
    [Key("ComputerDetailId")]
    [HasStream()]
    public partial class ComputerDetail {
        
        public int ComputerDetailId { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public DefaultNamespace.Dimensions Dimensions { get; set; }
        public DefaultNamespace.Computer Computer { get; set; }
        
        public ComputerDetail() {
        }
    }
    
    [Key("Name")]
    public partial class Driver {
        
        public string Name { get; set; }
        public System.DateTime BirthDate { get; set; }
        public DefaultNamespace.License License { get; set; }
        
        public Driver() {
        }
    }
    
    [Key("Name")]
    public partial class License {
        
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseClass { get; set; }
        public string Restrictions { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public DefaultNamespace.Driver Driver { get; set; }
        
        public License() {
        }
    }
    
    [Key("Id")]
    public partial class MappedEntityType {
        
        public int Id { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
        public string HrefLang { get; set; }
        public string Type { get; set; }
        public int Length { get; set; }
        
        public MappedEntityType() {
        }
    }
    
    [Key("VIN")]
    [HasStream()]
    public partial class Car {
        
        public int VIN { get; set; }
        public string Description { get; set; }
        
        public Car() {
        }
    }
}
#endif