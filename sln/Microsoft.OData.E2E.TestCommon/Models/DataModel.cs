//---------------------------------------------------------------------
// <copyright file="DataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Spatial;
using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.E2E.TestCommon.Models
{
    public partial class AllSpatialTypes
    {
        public int Id { get; set; }
        public Geography? Geog { get; set; }
        public GeographyPoint? GeogPoint { get; set; }
        public GeographyLineString? GeogLine { get; set; }
        public GeographyPolygon? GeogPolygon { get; set; }
        public GeographyCollection? GeogCollection { get; set; }
        public GeographyMultiPoint? GeogMultiPoint { get; set; }
        public GeographyMultiLineString? GeogMultiLine { get; set; }
        public GeographyMultiPolygon? GeogMultiPolygon { get; set; }
        public Geometry? Geom { get; set; }
        public GeometryPoint? GeomPoint { get; set; }
        public GeometryLineString? GeomLine { get; set; }
        public GeometryPolygon? GeomPolygon { get; set; }
        public GeometryCollection? GeomCollection { get; set; }
        public GeometryMultiPoint? GeomMultiPoint { get; set; }
        public GeometryMultiLineString? GeomMultiLine { get; set; }
        public GeometryMultiPolygon? GeomMultiPolygon { get; set; }

        public AllSpatialTypes()
        {
        }
    }

    public abstract partial class AllSpatialCollectionTypes
    {
        public int Id { get; set; }

        public AllSpatialCollectionTypes()
        {
        }
    }

    public partial class AllSpatialCollectionTypes_Simple : AllSpatialCollectionTypes
    {
        public List<GeographyPoint> ManyGeogPoint { get; set; }
        public List<GeographyLineString> ManyGeogLine { get; set; }
        public List<GeographyPolygon> ManyGeogPolygon { get; set; }
        public List<GeometryPoint> ManyGeomPoint { get; set; }
        public List<GeometryLineString> ManyGeomLine { get; set; }
        public List<GeometryPolygon> ManyGeomPolygon { get; set; }

        public AllSpatialCollectionTypes_Simple()
        {
            this.ManyGeogPoint = [];
            this.ManyGeogLine = [];
            this.ManyGeogPolygon = [];
            this.ManyGeomPoint = [];
            this.ManyGeomLine = [];
            this.ManyGeomPolygon = [];
        }
    }

    public partial class Aliases
    {
        public List<string> AlternativeNames { get; set; }

        public Aliases()
        {
            this.AlternativeNames = [];
        }
    }

    public partial class Phone
    {
        public int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Extension { get; set; }

        public Phone()
        {
        }
    }

    public partial class ContactDetails
    {
        public int Id { get; set; }
        public List<string> EmailBag { get; set; }
        public List<string> AlternativeNames { get; set; }
        // public Aliases? ContactAlias { get; set; }
        public Phone? HomePhone { get; set; }
        public Phone? WorkPhone { get; set; }
        public List<Phone> MobilePhoneBag { get; set; }

        public ContactDetails()
        {
            this.EmailBag = [];
            this.AlternativeNames = [];
            this.MobilePhoneBag = [];
        }
    }

    public partial class ComplexToCategory
    {
        public string? Term { get; set; }
        public string? Scheme { get; set; }
        public string? Label { get; set; }

        public ComplexToCategory()
        {
        }
    }

    public partial class Dimensions
    {
        public int Id { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }

        public Dimensions()
        {
        }
    }

    public partial class AuditInfo
    {
        public DateTimeOffset ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public ConcurrencyInfo? Concurrency { get; set; }

        public AuditInfo()
        {
        }
    }

    public partial class ConcurrencyInfo
    {
        [EfKey]
        public string? Token { get; set; }
        public Nullable<DateTimeOffset> QueriedDateTime { get; set; }

        public ConcurrencyInfo()
        {
        }
    }

    public partial class Customer
    {
        public int CustomerId { get; set; }
        public string? Name { get; set; }
        public ContactDetails? PrimaryContactInfo { get; set; }
        public List<ContactDetails> BackupContactInfo { get; set; }
        //public AuditInfo? Auditing { get; set; }
        public List<Order> Orders { get; set; }
        public List<Login> Logins { get; set; }
        //public Customer? Husband { get; set; }
        //public Customer? Wife { get; set; }
        public CustomerInfo? Info { get; set; }

        public Customer()
        {
            this.BackupContactInfo = [];
            this.Orders = [];
            this.Logins = [];
        }
    }

    public partial class Login
    {
        [EfKey]
        public string? Username { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        //public LastLogin? LastLogin { get; set; }
        //  public List<Message> SentMessages { get; set; }
        // public List<Message> ReceivedMessages { get; set; }
        public List<Order> Orders { get; set; }

        public Login()
        {
            //this.SentMessages = [];
            //this.ReceivedMessages = [];
            this.Orders = [];
        }
    }

    public partial class RSAToken
    {
        [EfKey]
        public string? Serial { get; set; }
        public DateTimeOffset Issued { get; set; }
        public Login? Login { get; set; }

        public RSAToken()
        {
        }
    }

    public partial class PageView
    {
        public int PageViewId { get; set; }
        public string? Username { get; set; }
        public DateTimeOffset Viewed { get; set; }
        public TimeSpan? TimeSpentOnPage { get; set; }
        public string? PageUrl { get; set; }
        public Login? Login { get; set; }

        public PageView()
        {
        }
    }

    public partial class ProductPageView : PageView
    {
        public int ProductId { get; set; }
        public string? ConcurrencyToken { get; set; }

        public ProductPageView()
        {
        }
    }

    public partial class LastLogin
    {
        [EfKey]
        public string? Username { get; set; }
        public DateTimeOffset LoggedIn { get; set; }
        public Nullable<DateTimeOffset> LoggedOut { get; set; }
        public TimeSpan? Duration { get; set; }
        public Login? Login { get; set; }

        public LastLogin()
        {
        }
    }

    public partial class Message
    {
        [EfKey]
        public int MessageId { get; set; }
        [EfKey]
        public string? FromUsername { get; set; }
        public string? ToUsername { get; set; }
        public DateTimeOffset Sent { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public bool IsRead { get; set; }
        public Login? Sender { get; set; }
        public Login? Recipient { get; set; }
        public List<MessageAttachment> Attachments { get; set; }

        public Message()
        {
            this.Attachments = new List<MessageAttachment>();
        }
    }

    public partial class MessageAttachment
    {
        [EfKey]
        public Guid AttachmentId { get; set; }
        public byte[]? Attachment { get; set; }

        public MessageAttachment()
        {
        }
    }

    public partial class Order
    {
        public int OrderId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public ConcurrencyInfo? Concurrency { get; set; }
        public Customer? Customer { get; set; }
        public Login? Login { get; set; }

        public Order()
        {
        }
    }

    public partial class OrderLine
    {
        [EfKey]
        public int OrderId { get; set; }
        [EfKey]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string? ConcurrencyToken { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }

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

    public partial class Product
    {
        public int ProductId { get; set; }
        public string? Description { get; set; }
        public Dimensions? Dimensions { get; set; }
        public string? BaseConcurrency { get; set; }
        public ConcurrencyInfo? ComplexConcurrency { get; set; }
        //public AuditInfo? NestedComplexConcurrency { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public ProductDetail? Detail { get; set; }
        public List<ProductReview> Reviews { get; set; }
        public List<ProductPhoto> Photos { get; set; }

        public Product()
        {
            this.RelatedProducts = [];
            this.Reviews = [];
            this.Photos = [];
        }
    }

    public partial class DiscontinuedProduct : Product
    {
        public DateTimeOffset Discontinued { get; set; }
        public Nullable<int> ReplacementProductId { get; set; }
        public Phone? DiscontinuedPhone { get; set; }
        public string? ChildConcurrencyToken { get; set; }

        public DiscontinuedProduct()
        {
        }
    }

    public partial class ProductDetail
    {
        [EfKey]
        public int ProductId { get; set; }
        public string? Details { get; set; }
        public Product? Product { get; set; }

        public ProductDetail()
        {
        }
    }

    public partial class ProductReview
    {
        [EfKey]
        public int ProductId { get; set; }
        [EfKey]
        public int ReviewId { get; set; }
        [EfKey]
        public string? RevisionId { get; set; }
        public string? Review { get; set; }
        public Product? Product { get; set; }

        public ProductReview()
        {
        }
    }

    public partial class ProductPhoto
    {
        [EfKey]
        public int ProductId { get; set; }
        [EfKey]
        public int PhotoId { get; set; }
        public byte[]? Photo { get; set; }

        public ProductPhoto()
        {
        }
    }

    public partial class CustomerInfo
    {
        [EfKey]
        public int CustomerInfoId { get; set; }
        public string? Information { get; set; }

        public CustomerInfo()
        {
        }
    }

    public partial class Computer
    {
        public int ComputerId { get; set; }
        public string? Name { get; set; }
        public ComputerDetail? ComputerDetail { get; set; }

        public Computer()
        {
        }
    }

    public partial class ComputerDetail
    {
        public int ComputerDetailId { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? Serial { get; set; }
        public List<string> SpecificationsBag { get; set; }
        public DateTimeOffset PurchaseDate { get; set; }
        public Dimensions? Dimensions { get; set; }
        public Computer? Computer { get; set; }

        public ComputerDetail()
        {
            this.SpecificationsBag = [];
        }
    }

    public partial class Driver
    {
        [EfKey]
        public string? Name { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public License? License { get; set; }

        public Driver()
        {
        }
    }

    public partial class License
    {
        [EfKey]
        public string? Name { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseClass { get; set; }
        public string? Restrictions { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
        public Driver? Driver { get; set; }

        public License()
        {
        }
    }

    public partial class MappedEntityType
    {
        public int Id { get; set; }
        public string? Href { get; set; }
        public string? Title { get; set; }
        public string? HrefLang { get; set; }
        public string? Type { get; set; }
        public int Length { get; set; }
        public List<string> BagOfPrimitiveToLinks { get; set; }
        public byte[]? Logo { get; set; }
        public List<decimal> BagOfDecimals { get; set; }
        public List<double> BagOfDoubles { get; set; }
        public List<float> BagOfSingles { get; set; }
        public List<byte> BagOfBytes { get; set; }
        public List<short> BagOfInt16s { get; set; }
        public List<int> BagOfInt32s { get; set; }
        public List<long> BagOfInt64s { get; set; }
        public List<Guid> BagOfGuids { get; set; }
        public List<DateTimeOffset> BagOfDateTimeOffset { get; set; }
        public List<ComplexToCategory> BagOfComplexToCategories { get; set; }
        public Phone? ComplexPhone { get; set; }
        public ContactDetails? ComplexContactDetails { get; set; }

        public MappedEntityType()
        {
            this.BagOfPrimitiveToLinks = [];
            this.BagOfDecimals = [];
            this.BagOfDoubles = [];
            this.BagOfSingles = [];
            this.BagOfBytes = [];
            this.BagOfInt16s = [];
            this.BagOfInt32s = [];
            this.BagOfInt64s = [];
            this.BagOfGuids = [];
            this.BagOfDateTimeOffset = new List<DateTimeOffset>();
            this.BagOfComplexToCategories = [];
        }
    }

    public partial class Car
    {
        [EfKey]
        public int VIN { get; set; }
        public string? Description { get; set; }

        public Car()
        {
        }
    }

    public partial class Person
    {
        public int PersonId { get; set; }
        public string? Name { get; set; }
        public List<PersonMetadata> PersonMetadata { get; set; }

        public Person()
        {
            this.PersonMetadata = [];
        }
    }

    public partial class Contractor : Person
    {
        public int ContratorCompanyId { get; set; }
        public int BillingRate { get; set; }
        public int TeamContactPersonId { get; set; }
        public string? JobDescription { get; set; }

        public Contractor()
        {
        }
    }

    public partial class Employee : Person
    {
        public int ManagersPersonId { get; set; }
        public int Salary { get; set; }
        public string? Title { get; set; }
        public Employee? Manager { get; set; }

        public Employee()
        {
        }
    }

    public partial class SpecialEmployee : Employee
    {
        public int CarsVIN { get; set; }
        public int Bonus { get; set; }
        public bool IsFullyVested { get; set; }
        public Car? Car { get; set; }

        public SpecialEmployee()
        {
        }
    }

    public partial class PersonMetadata
    {
        public int PersonMetadataId { get; set; }
        public int PersonId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyValue { get; set; }
        public Person? Person { get; set; }

        public PersonMetadata()
        {
        }
    }

    public class ComplexWithAllPrimitiveTypes
    {
        public byte[]? Binary { get; set; }
        public bool Boolean { get; set; }
        public byte Byte { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public Int16 Int16 { get; set; }
        public Int32 Int32 { get; set; }
        public Int64 Int64 { get; set; }
        public SByte SByte { get; set; }
        public string? String { get; set; }
        public Single Single { get; set; }
        public GeographyPoint? GeographyPoint { get; set; }
        public GeometryPoint? GeometryPoint { get; set; }
    }
}
