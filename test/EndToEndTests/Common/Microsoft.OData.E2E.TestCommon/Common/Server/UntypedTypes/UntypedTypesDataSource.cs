//---------------------------------------------------------------------
// <copyright file="UntypedTypesDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.E2E.TestCommon.Common.Server.UntypedTypes;

public class UntypedTypesDataSource
{
    public static UntypedTypesDataSource CreateInstance()
    {
        return new UntypedTypesDataSource();
    }

    public UntypedTypesDataSource()
    {
        ResetDataSource();
        Initialize();
    }

    public IList<Customer>? Customers { get; private set; }
    public IList<Order>? Orders { get; private set; }

    private void Initialize()
    {
        this.Customers =
        [
            new Customer
            {
                Id = 1,
                Name = "Customer 1",
                UntypedProperty = 3.40282347E+38,
                UntypedList = new List<object> { "String in list", 123, true, 45.67 },
                Orders = new List<Order>
                {
                    new Order { Id = 1, Amount = 100 },
                    new Order { Id = 2, Amount = 200 }
                }
            },
            new Customer
            {
                Id = 2,
                Name = "Customer 2",
                UntypedProperty = -456,
                UntypedList = new List<object> { "Another string", 789, false, 12.34 },
                Orders = new List<Order>
                {
                    new Order { Id = 3, Amount = 300 }
                }
            },
            new Customer
            {
                Id = 3,
                Name = "Customer 3",
                UntypedProperty = 6.02214076e+23,
                UntypedList = new List<object> { "Yet another string", 101112, true, 56.78 },
                Orders = []
            },
            new Customer
            {
                Id = 4,
                Name = "Customer 4",
                UntypedProperty = null,
                UntypedList = new List<object>(),
                Orders = []
            },
            new Customer
            {
                Id = 5,
                Name = "Customer 5",
                UntypedProperty = true,
                UntypedList = new List<object> { "Final string", 131415, false, 90.12 },
                Orders = []
            },
            new Customer
            {
                Id = 6,
                Name = "Customer 6",
                UntypedProperty = 78.9,
                UntypedList = new List<object> { "Extra string", 161718, true, 34.56 },
                Orders = []
            },
            new Customer
            {
                Id = 7,
                Name = "Customer 7",
                UntypedProperty = new List<object> { "List as untyped property", 192021, false, 78.90 },
                UntypedList = new List<object> { "More strings", 222324, true, 12.34 },
                Orders = []
            },
            new Customer
            {
                Id = 8,
                Name = "Small integer",
                UntypedProperty = 123,
                UntypedList = new List<object> { "123", 123, "Small integer" },
                Orders = new List<Order> { new Order { Id = 10, Amount = 123 } }
            },
            new Customer
            {
                Id = 9,
                Name = "Negative integer",
                UntypedProperty = -42,
                UntypedList = new List<object> { "-42", -42, "Negative integer" },
                Orders = new List<Order> { new Order { Id = 11, Amount = -42 } }
            },
            new Customer
            {
                Id = 10,
                Name = "Max int32",
                UntypedProperty = 2147483647,
                UntypedList = new List<object> { "2147483647", 2147483647, "Max int32" },
                Orders = new List<Order> { new Order { Id = 12, Amount = 2147483647 } }
            },
            new Customer
            {
                Id = 11,
                Name = "Beyond int32",
                UntypedProperty = 2147483648,
                UntypedList = new List<object> { "2147483648", 2147483648L, "Beyond int32" },
                Orders = new List<Order> { new Order { Id = 13, Amount = 2147483648 } }
            },
            new Customer
            {
                Id = 12,
                Name = "Max int64",
                UntypedProperty = 9223372036854775807,
                UntypedList = new List<object> { "9223372036854775807", 9223372036854775807L, "Max int64" },
                Orders = new List<Order> { new Order { Id = 14, Amount = 9223372036854775807 } }
            },
            new Customer
            {
                Id = 13,
                Name = "Beyond int64",
                UntypedProperty = 9223372036854775808m,
                UntypedList = new List<object> { "9223372036854775808", "Beyond int64" },
                Orders = new List<Order> { new Order { Id = 15, Amount = 922337203685477580 } }
            },
            new Customer
            {
                Id = 14,
                Name = "Simple decimal",
                UntypedProperty = 123.456,
                UntypedList = new List<object> { "123.456", 123.456m, "Simple decimal" },
                Orders = new List<Order> { new Order { Id = 16, Amount = 123.456m } }
            },
            new Customer
            {
                Id = 15,
                Name = "Very small decimal",
                UntypedProperty = 0.0000001,
                UntypedList = new List<object> { "0.0000001", 0.0000001m, "Very small decimal" },
                Orders = new List<Order> { new Order { Id = 17, Amount = 56787 } }
            },
            new Customer
            {
                Id = 16,
                Name = "High precision (pi)",
                UntypedProperty = 3.14159265358979323846,
                UntypedList = new List<object> { "3.14159265358979323846", 3.14159265358979323846, "High precision (pi)" },
                Orders = new List<Order> { new Order { Id = 18, Amount = 200 } }
            },
            new Customer
            {
                Id = 17,
                Name = "1. Large decimal with high precision",
                UntypedProperty = 123456789012345.123456789012345,
                UntypedList = new List<object> { "123456789012345.123456789012345", 123456789012345.123456789012345, "1. Large decimal with high precision" },
                Orders = new List<Order> { new Order { Id = 19, Amount = 123456789012345m } }
            },
            new Customer
            {
                Id = 18,
                Name = "2. Large decimal with high precision",
                UntypedProperty = 123456789123456789.12345678935m,
                UntypedList = new List<object> { "123456789123456789.12345678935", 123456789123456789.12345678935m, "2. Large decimal with high precision" },
                Orders = new List<Order> { new Order { Id = 20, Amount = 100000m } }
            },
            new Customer
            {
                Id = 19,
                Name = "3. Large decimal with high precision",
                UntypedProperty = 1234567891234567891234.12345678935546576m,
                UntypedList = new List<object> { "1234567891234567891234.12345678935546576", "3. Large decimal with high precision" },
                Orders = new List<Order> { new Order { Id = 21, Amount = 1234567891234567891234.12m } }
            },
            new Customer
            {
                Id = 20,
                Name = "Positive exponent",
                UntypedProperty = 1.234e+5,
                UntypedList = new List<object> { "1.234e+5", 1.234e+5, "Positive exponent" },
                Orders = new List<Order> { new Order { Id = 22, Amount = 123400 } }
            },
            new Customer
            {
                Id = 21,
                Name = "Negative exponent",
                UntypedProperty = 1.234e-5,
                UntypedList = new List<object> { "1.234e-5", 1.234e-5, "Negative exponent" },
                Orders = new List<Order> { new Order { Id = 23, Amount = 0.00001234m } }
            },
            new Customer
            {
                Id = 22,
                Name = "Avogadro's number",
                UntypedProperty = 6.02214076e+23,
                UntypedList = new List<object> { "6.02214076e+23", 6.02214076e+23, "Avogadro's number" },
                Orders = new List<Order> { new Order { Id = 24, Amount = 602214076000000000000000m } }
            },
            new Customer
            {
                Id = 23,
                Name = "Electron mass",
                UntypedProperty = 9.1093837e-31,
                UntypedList = new List<object> { "9.1093837e-31", 9.1093837e-31, "Electron mass" },
                Orders = new List<Order> { new Order { Id = 25, Amount = 0.0m } }
            },
            new Customer
            {
                Id = 24,
                Name = "Zero",
                UntypedProperty = 0.0,
                UntypedList = new List<object> { "0.0", 0.0, "Zero" },
                Orders = new List<Order> { new Order { Id = 26, Amount = 0 } }
            },
            new Customer
            {
                Id = 25,
                Name = "Negative zero",
                UntypedProperty = -0.0,
                UntypedList = new List<object> { "-0.0", -0.0, "Negative zero" },
                Orders = new List<Order> { new Order { Id = 27, Amount = 0 } }
            },
            new Customer
            {
                Id = 26,
                Name = "Max decimal",
                UntypedProperty = 79228162514264337593543950335m,
                UntypedList = new List<object> { "79228162514264337593543950335", 79228162514264337593543950335m, "Max decimal" },
                Orders = new List<Order> { new Order { Id = 28, Amount = 79228162514264337593543950335m } }
            },
            new Customer
            {
                Id = 27,
                Name = "Max double",
                UntypedProperty = 1.7976931348623157E+308,
                UntypedList = new List<object> { "1.7976931348623157E+308", 1.7976931348623157E+308, "Max double" },
                Orders = new List<Order> { new Order { Id = 29, Amount = 0 } }
            },
            new Customer
            {
                Id = 28,
                Name = "Max single",
                UntypedProperty = 3.40282347E+38,
                UntypedList = new List<object> { "3.40282347E+38", 3.40282347E+38, "Max single" },
                Orders = new List<Order> { new Order { Id = 30, Amount = 0 } }
            }
        ];

        this.Orders = this.Customers.SelectMany(c => c.Orders).Distinct().ToList();
    }

    private void ResetDataSource()
    {
        this.Customers = new List<Customer>();
        this.Orders = new List<Order>();
    }
}
