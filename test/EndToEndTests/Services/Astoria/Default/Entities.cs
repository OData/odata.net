//---------------------------------------------------------------------
// <copyright file="Entities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.AstoriaDefaultService
{
    using System;
    using Microsoft.OData.Service;
#if TESTPROVIDERS || TEST_ODATA_SERVICES_ASTORIA || TEST_ODATA_SERVICES_ASTORIA_NOPUBLICPROVIDER
    using Microsoft.Spatial;
    using Microsoft.OData.Client;
#else
    using System.Spatial;
#endif

    [Key("Id")]
    public partial class AllSpatialTypes
    {

        public int Id { get; set; }
        public Geography Geog { get; set; }
        public GeographyPoint GeogPoint { get; set; }
        public GeographyLineString GeogLine { get; set; }
        public GeographyPolygon GeogPolygon { get; set; }
        public GeographyCollection GeogCollection { get; set; }
        public GeographyMultiPoint GeogMultiPoint { get; set; }
        public GeographyMultiLineString GeogMultiLine { get; set; }
        public GeographyMultiPolygon GeogMultiPolygon { get; set; }
        public Geometry Geom { get; set; }
        public GeometryPoint GeomPoint { get; set; }
        public GeometryLineString GeomLine { get; set; }
        public GeometryPolygon GeomPolygon { get; set; }
        public GeometryCollection GeomCollection { get; set; }
        public GeometryMultiPoint GeomMultiPoint { get; set; }
        public GeometryMultiLineString GeomMultiLine { get; set; }
        public GeometryMultiPolygon GeomMultiPolygon { get; set; }

        public AllSpatialTypes()
        {
        }
    }

    [Key("Id")]
    public abstract partial class AllSpatialCollectionTypes
    {

        public int Id { get; set; }

        public AllSpatialCollectionTypes()
        {
        }
    }

    public partial class AllSpatialCollectionTypes_Simple : AllSpatialCollectionTypes
    {

        public System.Collections.Generic.List<GeographyPoint> ManyGeogPoint { get; set; }
        public System.Collections.Generic.List<GeographyLineString> ManyGeogLine { get; set; }
        public System.Collections.Generic.List<GeographyPolygon> ManyGeogPolygon { get; set; }
        public System.Collections.Generic.List<GeometryPoint> ManyGeomPoint { get; set; }
        public System.Collections.Generic.List<GeometryLineString> ManyGeomLine { get; set; }
        public System.Collections.Generic.List<GeometryPolygon> ManyGeomPolygon { get; set; }

        public AllSpatialCollectionTypes_Simple()
        {
            this.ManyGeogPoint = new System.Collections.Generic.List<GeographyPoint>();
            this.ManyGeogLine = new System.Collections.Generic.List<GeographyLineString>();
            this.ManyGeogPolygon = new System.Collections.Generic.List<GeographyPolygon>();
            this.ManyGeomPoint = new System.Collections.Generic.List<GeometryPoint>();
            this.ManyGeomLine = new System.Collections.Generic.List<GeometryLineString>();
            this.ManyGeomPolygon = new System.Collections.Generic.List<GeometryPolygon>();
        }
    }

    public partial class Aliases
    {
        public System.Collections.Generic.List<string> AlternativeNames { get; set; }

        public Aliases()
        {
            this.AlternativeNames = new System.Collections.Generic.List<string>();
        }
    }

    public partial class Phone
    {

        public string PhoneNumber { get; set; }
        public string Extension { get; set; }

        public Phone()
        {
        }
    }

    public partial class ContactDetails
    {

        public System.Collections.Generic.List<string> EmailBag { get; set; }
        public System.Collections.Generic.List<string> AlternativeNames { get; set; }
        public Aliases ContactAlias { get; set; }
        public Phone HomePhone { get; set; }
        public Phone WorkPhone { get; set; }
        public System.Collections.Generic.List<Phone> MobilePhoneBag { get; set; }

        public ContactDetails()
        {
            this.EmailBag = new System.Collections.Generic.List<string>();
            this.AlternativeNames = new System.Collections.Generic.List<string>();
            this.MobilePhoneBag = new System.Collections.Generic.List<Phone>();
        }
    }

    public partial class ComplexToCategory
    {

        public string Term { get; set; }
        public string Scheme { get; set; }
        public string Label { get; set; }

        public ComplexToCategory()
        {
        }
    }

    public partial class Dimensions
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }

        public Dimensions()
        {
        }
    }

    public partial class AuditInfo
    {

        public System.DateTimeOffset ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public ConcurrencyInfo Concurrency { get; set; }

        public AuditInfo()
        {
        }
    }

    public partial class ConcurrencyInfo
    {

        public string Token { get; set; }
        public Nullable<System.DateTimeOffset> QueriedDateTime { get; set; }

        public ConcurrencyInfo()
        {
        }
    }

    [Key("CustomerId")]
    [NamedStream("Video")]
    [NamedStream("Thumbnail")]
    public partial class Customer
    {

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public ContactDetails PrimaryContactInfo { get; set; }
        public System.Collections.Generic.List<ContactDetails> BackupContactInfo { get; set; }
        public AuditInfo Auditing { get; set; }
        public System.Collections.Generic.List<Order> Orders { get; set; }
        public System.Collections.Generic.List<Login> Logins { get; set; }
        public Customer Husband { get; set; }
        public Customer Wife { get; set; }
        public CustomerInfo Info { get; set; }

        public Customer()
        {
            this.BackupContactInfo = new System.Collections.Generic.List<ContactDetails>();
            this.Orders = new System.Collections.Generic.List<Order>();
            this.Logins = new System.Collections.Generic.List<Login>();
        }
    }

    [Key("Username")]
    public partial class Login
    {

        public string Username { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public LastLogin LastLogin { get; set; }
        public System.Collections.Generic.List<Message> SentMessages { get; set; }
        public System.Collections.Generic.List<Message> ReceivedMessages { get; set; }
        public System.Collections.Generic.List<Order> Orders { get; set; }

        public Login()
        {
            this.SentMessages = new System.Collections.Generic.List<Message>();
            this.ReceivedMessages = new System.Collections.Generic.List<Message>();
            this.Orders = new System.Collections.Generic.List<Order>();
        }
    }

    [Key("Serial")]
    public partial class RSAToken
    {

        public string Serial { get; set; }
        public System.DateTimeOffset Issued { get; set; }
        public Login Login { get; set; }

        public RSAToken()
        {
        }
    }

    [Key("PageViewId")]
    public partial class PageView
    {

        public int PageViewId { get; set; }
        public string Username { get; set; }
        public System.DateTimeOffset Viewed { get; set; }
        public System.TimeSpan TimeSpentOnPage { get; set; }
        public string PageUrl { get; set; }
        public Login Login { get; set; }

        public PageView()
        {
        }
    }

    public partial class ProductPageView : PageView
    {

        public int ProductId { get; set; }
        public string ConcurrencyToken { get; set; }

        public ProductPageView()
        {
        }
    }

    [Key("Username")]
    public partial class LastLogin
    {

        public string Username { get; set; }
        public System.DateTimeOffset LoggedIn { get; set; }
        public Nullable<System.DateTimeOffset> LoggedOut { get; set; }
        public System.TimeSpan Duration { get; set; }
        public Login Login { get; set; }

        public LastLogin()
        {
        }
    }

    [Key("MessageId", "FromUsername")]
    public partial class Message
    {

        public int MessageId { get; set; }
        public string FromUsername { get; set; }
        public string ToUsername { get; set; }
        public System.DateTimeOffset Sent { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public Login Sender { get; set; }
        public Login Recipient { get; set; }
        public System.Collections.Generic.List<Microsoft.Test.OData.Services.AstoriaDefaultService.MessageAttachment> Attachments { get; set; }

        public Message()
        {
            this.Attachments = new System.Collections.Generic.List<Microsoft.Test.OData.Services.AstoriaDefaultService.MessageAttachment>();
        }
    }

    [Key("AttachmentId")]
    public partial class MessageAttachment
    {

        public System.Guid AttachmentId { get; set; }
        public byte[] Attachment { get; set; }

        public MessageAttachment()
        {
        }
    }

    [Key("OrderId")]
    public partial class Order
    {

        public int OrderId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public ConcurrencyInfo Concurrency { get; set; }
        public Customer Customer { get; set; }
        public Login Login { get; set; }

        public Order()
        {
        }
    }

    [Key("OrderId", "ProductId")]
    [NamedStream("OrderLineStream")]
    public partial class OrderLine
    {

        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ConcurrencyToken { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }

        public OrderLine()
        {
        }
    }

    public partial class BackOrderLine : OrderLine
    {

        public BackOrderLine()
        {
        }
    }

    public partial class BackOrderLine2 : BackOrderLine
    {

        public BackOrderLine2()
        {
        }
    }

    [Key("ProductId")]
    [NamedStream("Picture")]
    public partial class Product
    {

        public int ProductId { get; set; }
        public string Description { get; set; }
        public Dimensions Dimensions { get; set; }
        public string BaseConcurrency { get; set; }
        public ConcurrencyInfo ComplexConcurrency { get; set; }
        public AuditInfo NestedComplexConcurrency { get; set; }
        public System.Collections.Generic.List<Product> RelatedProducts { get; set; }
        public ProductDetail Detail { get; set; }
        public System.Collections.Generic.List<ProductReview> Reviews { get; set; }
        public System.Collections.Generic.List<ProductPhoto> Photos { get; set; }

        public Product()
        {
            this.RelatedProducts = new System.Collections.Generic.List<Product>();
            this.Reviews = new System.Collections.Generic.List<ProductReview>();
            this.Photos = new System.Collections.Generic.List<ProductPhoto>();
        }
    }

    public partial class DiscontinuedProduct : Product
    {

        public System.DateTimeOffset Discontinued { get; set; }
        public Nullable<int> ReplacementProductId { get; set; }
        public Phone DiscontinuedPhone { get; set; }
        public string ChildConcurrencyToken { get; set; }

        public DiscontinuedProduct()
        {
        }
    }

    [Key("ProductId")]
    public partial class ProductDetail
    {

        public int ProductId { get; set; }
        public string Details { get; set; }
        public Product Product { get; set; }

        public ProductDetail()
        {
        }
    }

    [Key("ProductId", "ReviewId", "RevisionId")]
    public partial class ProductReview
    {

        public int ProductId { get; set; }
        public int ReviewId { get; set; }
        public string RevisionId { get; set; }
        public string Review { get; set; }
        public Product Product { get; set; }

        public ProductReview()
        {
        }
    }

    [Key("ProductId", "PhotoId")]
    public partial class ProductPhoto
    {

        public int ProductId { get; set; }
        public int PhotoId { get; set; }
        public byte[] Photo { get; set; }

        public ProductPhoto()
        {
        }
    }

    [Key("CustomerInfoId")]
    [HasStream()]
    public partial class CustomerInfo
    {

        public int CustomerInfoId { get; set; }
        public string Information { get; set; }

        public CustomerInfo()
        {
        }
    }

    [Key("ComputerId")]
    public partial class Computer
    {

        public int ComputerId { get; set; }
        public string Name { get; set; }
        public ComputerDetail ComputerDetail { get; set; }

        public Computer()
        {
        }
    }

    [Key("ComputerDetailId")]
    public partial class ComputerDetail
    {

        public int ComputerDetailId { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }
        public System.Collections.Generic.List<string> SpecificationsBag { get; set; }
        public System.DateTimeOffset PurchaseDate { get; set; }
        public Dimensions Dimensions { get; set; }
        public Computer Computer { get; set; }

        public ComputerDetail()
        {
            this.SpecificationsBag = new System.Collections.Generic.List<string>();
        }
    }

    [Key("Name")]
    public partial class Driver
    {

        public string Name { get; set; }
        public System.DateTimeOffset BirthDate { get; set; }
        public License License { get; set; }

        public Driver()
        {
        }
    }

    [Key("Name")]
    public partial class License
    {

        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseClass { get; set; }
        public string Restrictions { get; set; }
        public System.DateTimeOffset ExpirationDate { get; set; }
        public Driver Driver { get; set; }

        public License()
        {
        }
    }

    [Key("Id")]
    public partial class MappedEntityType
    {

        public int Id { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
        public string HrefLang { get; set; }
        public string Type { get; set; }
        public int Length { get; set; }
        public System.Collections.Generic.List<string> BagOfPrimitiveToLinks { get; set; }
        public byte[] Logo { get; set; }
        public System.Collections.Generic.List<decimal> BagOfDecimals { get; set; }
        public System.Collections.Generic.List<double> BagOfDoubles { get; set; }
        public System.Collections.Generic.List<float> BagOfSingles { get; set; }
        public System.Collections.Generic.List<byte> BagOfBytes { get; set; }
        public System.Collections.Generic.List<short> BagOfInt16s { get; set; }
        public System.Collections.Generic.List<int> BagOfInt32s { get; set; }
        public System.Collections.Generic.List<long> BagOfInt64s { get; set; }
        public System.Collections.Generic.List<System.Guid> BagOfGuids { get; set; }
        public System.Collections.Generic.List<System.DateTimeOffset> BagOfDateTimeOffset { get; set; }
        public System.Collections.Generic.List<ComplexToCategory> BagOfComplexToCategories { get; set; }
        public Phone ComplexPhone { get; set; }
        public ContactDetails ComplexContactDetails { get; set; }

        public MappedEntityType()
        {
            this.BagOfPrimitiveToLinks = new System.Collections.Generic.List<string>();
            this.BagOfDecimals = new System.Collections.Generic.List<decimal>();
            this.BagOfDoubles = new System.Collections.Generic.List<double>();
            this.BagOfSingles = new System.Collections.Generic.List<float>();
            this.BagOfBytes = new System.Collections.Generic.List<byte>();
            this.BagOfInt16s = new System.Collections.Generic.List<short>();
            this.BagOfInt32s = new System.Collections.Generic.List<int>();
            this.BagOfInt64s = new System.Collections.Generic.List<long>();
            this.BagOfGuids = new System.Collections.Generic.List<System.Guid>();
            this.BagOfDateTimeOffset = new System.Collections.Generic.List<System.DateTimeOffset>();
            this.BagOfComplexToCategories = new System.Collections.Generic.List<ComplexToCategory>();
        }
    }

    [Key("VIN")]
    [NamedStream("Photo")]
    [NamedStream("Video")]
    [HasStream()]
    public partial class Car
    {

        public int VIN { get; set; }
        public string Description { get; set; }

        public Car()
        {
        }
    }

    [Key("PersonId")]
    public partial class Person
    {

        public int PersonId { get; set; }
        public string Name { get; set; }
        public System.Collections.Generic.List<PersonMetadata> PersonMetadata { get; set; }

        public Person()
        {
            this.PersonMetadata = new System.Collections.Generic.List<PersonMetadata>();
        }
    }

    public partial class Contractor : Person
    {

        public int ContratorCompanyId { get; set; }
        public int BillingRate { get; set; }
        public int TeamContactPersonId { get; set; }
        public string JobDescription { get; set; }

        public Contractor()
        {
        }
    }

    public partial class Employee : Person
    {

        public int ManagersPersonId { get; set; }
        public int Salary { get; set; }
        public string Title { get; set; }
        public Employee Manager { get; set; }

        public Employee()
        {
        }
    }

    public partial class SpecialEmployee : Employee
    {

        public int CarsVIN { get; set; }
        public int Bonus { get; set; }
        public bool IsFullyVested { get; set; }
        public Car Car { get; set; }

        public SpecialEmployee()
        {
        }
    }

    [Key("PersonMetadataId")]
    public partial class PersonMetadata
    {

        public int PersonMetadataId { get; set; }
        public int PersonId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public Person Person { get; set; }

        public PersonMetadata()
        {
        }
    }

    public class ComplexWithAllPrimitiveTypes
    {
        public Byte[] Binary { get; set; }
        public Boolean Boolean { get; set; }
        public Byte Byte { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public Decimal Decimal { get; set; }
        public Double Double { get; set; }
        public Int16 Int16 { get; set; }
        public Int32 Int32 { get; set; }
        public Int64 Int64 { get; set; }
        public SByte SByte { get; set; }
        public String String { get; set; }
        public Single Single { get; set; }
        public GeographyPoint GeographyPoint { get; set; }
        public GeometryPoint GeometryPoint { get; set; }
    }
}
