//-----------------------------------------------------------------------------
// <copyright file="DefaultDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.Spatial;
using System.Collections.ObjectModel;
using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.Default
{
    public class Address
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class HomeAddress : Address
    {
        public string? FamilyName { get; set; }
    }

    public class CompanyAddress : Address
    {
        public string? CompanyName { get; set; }
    }

    public class CityInformation
    {
        public string? CountryRegion { get; set; }
        public bool IsCapital { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public enum Color
    {
        Red = 1,
        Green = 2,
        Blue = 4
    }

    [Flags]
    public enum AccessLevel
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4,
        ReadWrite = 3
    }

    public enum CompanyCategory
    {
        IT = 0,
        Communication = 1,
        Electronics = 2,
        Others = 4
    }

    public class Person
    {
        public int PersonID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public Collection<string>? Numbers { get; set; }
        public Collection<string>? Emails { get; set; }
        public Collection<Address>? Addresses { get; set; }
        public Address? HomeAddress { get; set; }
        public GeographyPoint? Home { get; set; }
        public Person? Parent { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Customer : Person
    {
        public string? City { get; set; }
        public DateTimeOffset Birthday { get; set; }
        public TimeSpan TimeBetweenLastTwoOrders { get; set; }
        public Company? Company { get; set; }
        public List<Order>? Orders { get; set; }
    }

    public abstract class AbstractEntity
    {
        public DateTimeOffset UpdatedTime { get; set; }
    }


    public class Order : AbstractEntity 
    {
        public int OrderID { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public Employee? LoggedInEmployee { get; set; }
        public Customer? CustomerForOrder { get; set; }
        public TimeSpan? ShelfLife { get; set; }
        public Collection<TimeSpan>? OrderShelfLifes { get; set; }
        public Date ShipDate { get; set; }
        public TimeOfDay ShipTime { get; set; }
        public InfoFromCustomer? InfoFromCustomer { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }

    public class InfoFromCustomer
    {
        public string? CustomerMessage { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Calendar : AbstractEntity
    {
        public Calendar()
        {
        }
        [EfKey]
        public Date Day { get; set; }
    }

    public class OrderDetail : AbstractEntity
    {
        [EfKey]
        public int OrderID { get; set; }
        [EfKey]
        public int ProductID { get; set; }
        public DateTimeOffset OrderPlaced { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public Product? ProductOrdered { get; set; }
        public Order? AssociatedOrder { get; set; }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string? Name { get; set; }
        public string? QuantityPerUnit { get; set; }
        public float UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public bool Discontinued { get; set; }
        public Color? SkinColor { get; set; }
        public Collection<Color>? CoverColors { get; set; }
        public AccessLevel? UserAccess { get; set; }
        public List<ProductDetail>? Details { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class ProductDetail
    {
        [EfKey]
        public int ProductDetailID { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public Product? RelatedProduct { get; set; }
        [EfKey]
        public int ProductID { get; set; }
        public List<ProductReview>? Reviews { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Employee : Person
    {
        public DateTimeOffset DateHired { get; set; }
        public GeographyPoint? Office { get; set; }
        public int CompanyID { get; set; }
        public Company? Company { get; set; }
    }

    public class ProductReview
    {
        [EfKey]
        public int ProductID { get; set; }
        [EfKey]
        public int ProductDetailID { get; set; }
        [EfKey]
        public string? ReviewTitle { get; set; }
        [EfKey]
        public int RevisionID { get; set; }
        public string? Comment { get; set; }
        public string? Author { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Company
    {
        public int CompanyID { get; set; }
        public CompanyCategory CompanyCategory { get; set; }
        public string? Name { get; set; }
        public Address? Address { get; set; }
        public long Revenue { get; set; }
        public List<Employee>? Employees { get; set; }
        public Department? CoreDepartment { get; set; }
        public Customer? VipCustomer { get; set; }
        public List<Department>? Departments { get; set; }
        public Dictionary<string, object>? DynamicProperties { get; set; }
    }

    public class PublicCompany : Company
    {
        public string? StockExchange { get; set; }

        [Contained]
        public List<Asset>? Assets { get; set; }

        [Contained]
        public Club? Club { get; set; }
        public LabourUnion? LabourUnion { get; set; }
    }

    public class Club
    {
        public int ClubID { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class LabourUnion
    {
        public int LabourUnionID { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Asset
    {
        public int AssetID { get; set; }
        public string? Name { get; set; }
        public int Number { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Department
    {
        public int DepartmentID { get; set; }
        public string? Name { get; set; }
        public string? DepartmentNO { get; set; }
        public Company? Company { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Account
    {
        public int AccountID { get; set; }
        public string? CountryRegion { get; set; }
        public AccountInfo? AccountInfo { get; set; }

        [Contained]
        public List<PaymentInstrument>? MyPaymentInstruments { get; set; }

        [Contained]
        public List<Subscription>? ActiveSubscriptions { get; set; }

        [Contained]
        public GiftCard? MyGiftCard { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class GiftCard
    {
        public int GiftCardID { get; set; }
        public string? GiftCardNO { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset ExperationDate { get; set; }
        public string? OwnerName { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class PaymentInstrument
    {
        public int PaymentInstrumentID { get; set; }
        public string? FriendlyName { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        [Contained]
        public List<Statement>? BillingStatements { get; set; }
        public StoredPI? TheStoredPI { get; set; }
        public StoredPI? BackupStoredPI { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class CreditCardPI : PaymentInstrument
    {
        public string? CardNumber { get; set; }
        public string? CVV { get; set; }
        public string? HolderName { get; set; }
        public double Balance { get; set; }
        public DateTimeOffset ExperationDate { get; set; }

        [Contained]
        public List<CreditRecord>? CreditRecords { get; set; }
    }

    public class StoredPI
    {
        public int StoredPIID { get; set; }
        public string? PIName { get; set; }
        public string? PIType { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Statement
    {
        public int StatementID { get; set; }
        public string? TransactionType { get; set; }
        public string? TransactionDescription { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class CreditRecord
    {
        public int CreditRecordID { get; set; }
        public bool IsGood { get; set; }
        public string? Reason { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class Subscription
    {
        public int SubscriptionID { get; set; }
        public string? TemplateGuid { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int QualifiedAccountID { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }

    public class AccountInfo
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
        public Dictionary<string, object>? DynamicProperties { get; set; } = new Dictionary<string, object>();
    }
}
