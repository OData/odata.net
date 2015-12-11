//---------------------------------------------------------------------
// <copyright file="InMemoryCLRTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Client;
    using Microsoft.Spatial;

    /// <summary>
    /// The class represents the Person model type.
    /// </summary>
    [Key("PersonID")]
    public class Person
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Collection<string> Numbers { get; set; }
        public Address HomeAddress { get; set; }
        public GeographyPoint Home { get; set; }
    }

    /// <summary>
    /// The class represents the School model type.
    /// </summary>
    [Key("SchoolID")]
    public class School
    {
        public int SchoolID { get; set; }
        public Collection<Person> Students { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }

    public class HomeAddress : Address
    {
        public string HomeNO { get; set; }
    }

    /// <summary>
    /// The class represents the Customer model type.
    /// </summary>
    [Key("PersonID")]
    public class Customer : Person
    {
        public string City { get; set; }
        public DateTime Birthday { get; set; }
        public TimeSpan TimeBetweenLastTwoOrders { get; set; }
    }

    /// <summary>
    /// The class represents the Order model type.
    /// </summary>
    [Key("OrderID")]
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime OrderDate { get; set; }
        public Employee LoggedInEmployee { get; set; }
        public Customer CustomerForOrder { get; set; }
    }

    /// <summary>
    /// The class represents the OrderDetail model type.
    /// </summary>
    [Key("PropertyID", "OrderID")]
    public class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public DateTime OrderPlaced { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
        public Product ProductOrdered { get; set; }
        public Order AssociatedOrder { get; set; }
    }

    /// <summary>
    /// The class represents the Product model type.
    /// </summary>
    [Key("PropertyID")]
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string QuantityPerUnit { get; set; }
        public float UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public bool Discontinued { get; set; }
    }

    /// <summary>
    /// The class represents the Employee model type.
    /// </summary>
    [Key("PersonID")]
    public class Employee : Person
    {
        public DateTime DateHired { get; set; }
        public GeographyPoint Office { get; set; }
    }

    /// <summary>
    /// The class represents the DurationInKey model type.
    /// </summary>
    [Key("Id")]
    public class DurationInKey
    {
        public TimeSpan Id { get; set; }
    }
}
