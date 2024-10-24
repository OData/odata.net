//-----------------------------------------------------------------------------
// <copyright file="CommonEndToEndDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Spatial;
using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd
{
    public class AllSpatialTypes
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
    }

    public abstract class AllSpatialCollectionTypes
    {
        public int Id { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public Stream? Thumbnail { get; set; }
        public Stream? Video { get; set; }
        public string? Name { get; set; }
        public ContactDetails? PrimaryContactInfo { get; set; }
        public List<ContactDetails> BackupContactInfo { get; set; } = new List<ContactDetails>();
        public AuditInfo? Auditing { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Login> Logins { get; set; } = new List<Login>();
        public Customer? Husband { get; set; }
        public Customer? Wife { get; set; }
        public CustomerInfo? Info { get; set; }
    }

    public class Login
    {
        [EfKey]
        public string? Username { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual LastLogin? LastLogin { get; set; }
        public virtual ICollection<Message>? SentMessages { get; set; }
        public virtual ICollection<Message>? ReceivedMessages { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }

    public class RSAToken
    {
        [EfKey]
        public string? Serial { get; set; }
        public DateTimeOffset Issued { get; set; }
        public Login? Login { get; set; }
    }

    public class PageView
    {
        public int PageViewId { get; set; }
        public string? Username { get; set; }
        public DateTimeOffset Viewed { get; set; }
        public TimeSpan TimeSpentOnPage { get; set; }
        public string? PageUrl { get; set; }
        public Login? Login { get; set; }
    }

    public class LastLogin
    {
        [EfKey]
        public string? Username { get; set; }
        public DateTimeOffset LoggedIn { get; set; }
        public DateTimeOffset? LoggedOut { get; set; }
        public TimeSpan Duration { get; set; }
        public Login? Login { get; set; }
    }

    public class Message
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
        public ICollection<MessageAttachment>? Attachments { get; set; }
    }

    public class MessageAttachment
    {
        [EfKey]
        public Guid AttachmentId { get; set; }
        public byte[]? Attachment { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public ConcurrencyInfo? Concurrency { get; set; }
        public Login? Login { get; set; }
        public Customer? Customer { get; set; }
    }

    public class OrderLine
    {
        [EfKey]
        public int OrderId { get; set; }
        [EfKey]
        public int ProductId { get; set; }
        public Stream? OrderLineStream { get; set; }
        public int Quantity { get; set; }
        public string? ConcurrencyToken { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public byte[]? Picture { get; set; }
        public string? Description { get; set; }
        public Dimensions? Dimensions { get; set; }
        public string? BaseConcurrency { get; set; }
        public ConcurrencyInfo? ComplexConcurrency { get; set; }
        public AuditInfo? NestedComplexConcurrency { get; set; }
        public virtual ICollection<Product>? RelatedProducts { get; set; }
        public virtual ProductDetail? Detail { get; set; }
        public virtual ICollection<ProductReview>? Reviews { get; set; }
        public virtual ICollection<ProductPhoto>? Photos { get; set; }
    }

    public class ProductDetail
    {
        [EfKey]
        public int ProductId { get; set; }
        public string? Details { get; set; }
        public virtual Product? Product { get; set; }
    }

    public class ProductReview
    {
        [EfKey]
        public int ProductId { get; set; }
        [EfKey]
        public int ReviewId { get; set; }
        public string? Review { get; set; }
        [EfKey]
        public string? RevisionId { get; set; }
        public virtual Product? Product { get; set; }
    }

    public class ProductPhoto
    {
        [EfKey]
        public int ProductId { get; set; }
        [EfKey]
        public int PhotoId { get; set; }
        public byte[]? Photo { get; set; }
    }

    public class CustomerInfo
    {
        public int CustomerInfoId { get; set; }
        public string? Information { get; set; }
    }

    public class Computer
    {
        public int ComputerId { get; set; }
        public string? Name { get; set; }
        public virtual ComputerDetail? ComputerDetail { get; set; }
    }

    public class ComputerDetail
    {
        public int ComputerDetailId { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? Serial { get; set; }
        public ICollection<string>? SpecificationsBag { get; set; }
        public DateTimeOffset PurchaseDate { get; set; }
        public Dimensions? Dimensions { get; set; }
        public virtual Computer? Computer { get; set; }
    }

    public class Driver
    {
        [EfKey]
        public string? Name { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public virtual License? License { get; set; }
    }

    public class License
    {
        [EfKey]
        public string? Name { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseClass { get; set; }
        public string? Restrictions { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
        public virtual Driver? Driver { get; set; }
    }

    public class MappedEntityType
    {
        public int Id { get; set; }
        public string? Href { get; set; }
        public string? Title { get; set; }
        public string? HrefLang { get; set; }
        public string? Type { get; set; }
        public int Length { get; set; }
        public ICollection<string>? BagOfPrimitiveToLinks { get; set; }
        public byte[]? Logo { get; set; }
        public ICollection<decimal>? BagOfDecimals { get; set; }
        public ICollection<double>? BagOfDoubles { get; set; }
        public ICollection<float>? BagOfSingles { get; set; }
        public ICollection<byte>? BagOfBytes { get; set; }
        public ICollection<short>? BagOfInt16s { get; set; }
        public ICollection<int>? BagOfInt32s { get; set; }
        public ICollection<long>? BagOfInt64s { get; set; }
        public ICollection<Guid>? BagOfGuids { get; set; }
        public ICollection<DateTimeOffset>? BagOfDateTimeOffset { get; set; }
        public ICollection<ComplexToCategory>? BagOfComplexToCategories { get; set; }
        public Phone? ComplexPhone { get; set; }
        public ContactDetails? ComplexContactDetails { get; set; }
    }

    public class Car
    {
        [EfKey]
        public int VIN { get; set; }
        public Stream? Photo { get; set; }
        public Stream? Video { get; set; }
        public string? Description { get; set; }
    }

    public class Person
    {
        public int PersonId { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<PersonMetadata>? PersonMetadata { get; set; }
    }

    public class PersonMetadata
    {
        public int PersonMetadataId { get; set; }
        public int PersonId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyValue { get; set; }
        public virtual Person? Person { get; set; }
    }

    public class ContactDetails
    {
        public ICollection<string>? EmailBag { get; set; }
        public ICollection<string>? AlternativeNames { get; set; }
        public Aliases? ContactAlias { get; set; }
        public Phone? HomePhone { get; set; }
        public Phone? WorkPhone { get; set; }
        public ICollection<Phone>? MobilePhoneBag { get; set; }
    }

    public class Aliases
    {
        public ICollection<string>? AlternativeNames { get; set; }
    }

    public class Phone
    {
        public string? PhoneNumber { get; set; }
        public string? Extension { get; set; }
    }

    public class AuditInfo
    {
        public DateTimeOffset ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public ConcurrencyInfo? Concurrency { get; set; }
    }

    public class ConcurrencyInfo
    {
        public string? Token { get; set; }
        public DateTimeOffset? QueriedDateTime { get; set; }
    }

    public class Dimensions
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }
    }

    public class ComplexToCategory
    {
        public string? Term { get; set; }
        public string? Scheme { get; set; }
        public string? Label { get; set; }
    }

    public class AllSpatialCollectionTypes_Simple : AllSpatialCollectionTypes
    {
        public ICollection<GeographyPoint>? ManyGeogPoint { get; set; }
        public ICollection<GeographyLineString>? ManyGeogLine { get; set; }
        public ICollection<GeographyPolygon>? ManyGeogPolygon { get; set; }
        public ICollection<GeometryPoint>? ManyGeomPoint { get; set; }
        public ICollection<GeometryLineString>? ManyGeomLine { get; set; }
        public ICollection<GeometryPolygon>? ManyGeomPolygon { get; set; }
    }

    public class ProductPageView : PageView
    {
        public int ProductId { get; set; }
        public string? ConcurrencyToken { get; set; }
    }

    public class BackOrderLine : OrderLine
    {
    }

    public class BackOrderLine2 : BackOrderLine
    {
    }

    public class DiscontinuedProduct : Product
    {
        public DateTimeOffset Discontinued { get; set; }
        public int? ReplacementProductId { get; set; }
        public Phone? DiscontinuedPhone { get; set; }
        public string? ChildConcurrencyToken { get; set; }
    }

    public class Contractor : Person
    {
        public int ContratorCompanyId { get; set; }
        public int BillingRate { get; set; }
        public int TeamContactPersonId { get; set; }
        public string? JobDescription { get; set; }
    }

    public class Employee : Person
    {
        public int ManagersPersonId { get; set; }
        public int Salary { get; set; }
        public string? Title { get; set; }
        public virtual Employee? Manager { get; set; }
    }

    public class SpecialEmployee : Employee
    {
        public int CarsVIN { get; set; }
        public int Bonus { get; set; }
        public bool IsFullyVested { get; set; }
        public Car? Car { get; set; }
    }

    public class ComplexWithAllPrimitiveTypes
    {
        public byte[]? Binary { get; set; }
        public bool Boolean { get; set; }
        public byte Byte { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public short Int16 { get; set; }
        public int Int32 { get; set; }
        public long Int64 { get; set; }
        public sbyte SByte { get; set; }
        public string? String { get; set; }
        public float Single { get; set; }
        public GeographyPoint? GeographyPoint { get; set; }
        public GeometryPoint? GeometryPoint { get; set; }
    }

    [Key("Id")]
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ICollection<BankAccount> BankAccounts { get; set; }
    }

    [Key("Id")]
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public int BankId { get; set; }
        public Bank Bank { get; set; }  
    }
}
