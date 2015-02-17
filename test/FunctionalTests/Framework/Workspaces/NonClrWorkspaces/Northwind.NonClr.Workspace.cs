//---------------------------------------------------------------------
// <copyright file="Northwind.NonClr.Workspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;

    [WorkspaceAttribute("Northwind", DataLayerProviderKind.NonClr, Priority = 0, Standard = true)]
    public class NorthwindNonClrWorkspace : System.Data.Test.Astoria.NonClrWorkspace
    {
        public NorthwindNonClrWorkspace()
            : base("northwind", "northwind.NonClr", "NorthwindContainer")
        { this.Language = WorkspaceLanguage.CSharp; }

        public override ServiceContainer ServiceContainer
        {
            get
            {
                if (_serviceContainer == null)
                {

                    //Complex types code here

                    //Complex types that contain other complextypes code here


                    //Resource types here
                    ResourceType Categories = Resource.ResourceType("Categories", "northwind.NonClr",
                        Resource.Property("CategoryID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("CategoryName", Clr.Types.String, NodeFacet.MaxSize(15)),
                        Resource.Property("Description", Clr.Types.String, NodeFacet.Nullable(true),NodeFacet.Sortable(false)),
                        Resource.Property("Picture", Clr.Types.Binary, NodeFacet.Nullable(true),NodeFacet.Sortable(false))
                    );
                    ResourceType Products = Resource.ResourceType("Products", "northwind.NonClr",
                        Resource.Property("ProductID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ProductName", Clr.Types.String, NodeFacet.MaxSize(40)),
                        Resource.Property("QuantityPerUnit", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(20)),
                        Resource.Property("UnitPrice", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("UnitsInStock", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("UnitsOnOrder", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("ReorderLevel", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("Discontinued", Clr.Types.Boolean)
                    );
                   
                    ResourceType Orders = Resource.ResourceType("Orders", "northwind.NonClr",
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("OrderDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("RequiredDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("ShippedDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("Freight", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("ShipName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(40)),
                        Resource.Property("ShipAddress", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("ShipCity", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("ShipRegion", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("ShipPostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10))
                    );
                    
                    ResourceType Order_Details = Resource.ResourceType("Order_Details", "northwind.NonClr",
                       Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                       Resource.Property("ProductID", Clr.Types.Int32, Resource.Key()),
                       Resource.Property("UnitPrice", Clr.Types.Decimal, NodeFacet.Precision(19), NodeFacet.Scale(4)),
                       Resource.Property("Quantity", Clr.Types.Int16),
                       Resource.Property("Discount", Clr.Types.Single)
                   );

                    ResourceType Customers = Resource.ResourceType("Customers", "northwind.NonClr",
                        Resource.Property("CustomerID", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(5), NodeFacet.FixedLength(true)),
                        Resource.Property("CompanyName", Clr.Types.String, NodeFacet.MaxSize(40)),
                        Resource.Property("ContactName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("ContactTitle", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("Address", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("Region", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("PostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)),
                        Resource.Property("Phone", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)),
                        Resource.Property("Fax", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24))
                    );
                    ResourceType CustomerDemographics = Resource.ResourceType("CustomerDemographics", "northwind.NonClr",
                        Resource.Property("CustomerTypeID", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(10), NodeFacet.FixedLength(true)),
                        Resource.Property("CustomerDesc", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false))
                    );
                    ResourceType Employees = Resource.ResourceType("Employees", "northwind.NonClr",
                        Resource.Property("EmployeeID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("LastName", Clr.Types.String, NodeFacet.MaxSize(20)),
                        Resource.Property("FirstName", Clr.Types.String, NodeFacet.MaxSize(10)),
                        Resource.Property("Title", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("TitleOfCourtesy", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(25)),
                        Resource.Property("BirthDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("HireDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("Address", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("Region", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("PostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)),
                        Resource.Property("HomePhone", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)),
                        Resource.Property("Extension", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(4)),
                        Resource.Property("Photo", Clr.Types.Binary, NodeFacet.Nullable(true), NodeFacet.Sortable(false)),
                        Resource.Property("Notes", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false)),
                        Resource.Property("PhotoPath", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(255))
                    );
                    ResourceType Territories = Resource.ResourceType("Territories", "northwind.NonClr",
                        Resource.Property("TerritoryID", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(20)),
                        Resource.Property("TerritoryDescription", Clr.Types.String, NodeFacet.MaxSize(50), NodeFacet.FixedLength(true))
                    );
                    ResourceType Region = Resource.ResourceType("Region", "northwind.NonClr",
                        Resource.Property("RegionID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("RegionDescription", Clr.Types.String, NodeFacet.MaxSize(50), NodeFacet.FixedLength(true))
                    );
                    ResourceType Shippers = Resource.ResourceType("Shippers", "northwind.NonClr",
                        Resource.Property("ShipperID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("CompanyName", Clr.Types.String, NodeFacet.MaxSize(40)),
                        Resource.Property("Phone", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24))
                    );
                    ResourceType Suppliers = Resource.ResourceType("Suppliers", "northwind.NonClr",
                        Resource.Property("SupplierID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("CompanyName", Clr.Types.String, NodeFacet.MaxSize(40)),
                        Resource.Property("ContactName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("ContactTitle", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("Address", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("Region", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("PostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)),
                        Resource.Property("Phone", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)),
                        Resource.Property("Fax", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)),
                        Resource.Property("HomePage", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false))
                    );
                    ResourceType Alphabetical_list_of_products = Resource.ResourceType("Alphabetical_list_of_products", "northwind.NonClr",
                        Resource.Property("ProductID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ProductName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("SupplierID", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("CategoryID", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("QuantityPerUnit", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(20)),
                        Resource.Property("UnitPrice", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("UnitsInStock", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("UnitsOnOrder", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("ReorderLevel", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("Discontinued", Clr.Types.Boolean, Resource.Key()),
                        Resource.Property("CategoryName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(15))
                    );
                    ResourceType Category_Sales_for_1997 = Resource.ResourceType("Category_Sales_for_1997", "northwind.NonClr",
                        Resource.Property("CategoryName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(15)),
                        Resource.Property("CategorySales", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );
                    ResourceType Current_Product_List = Resource.ResourceType("Current_Product_List", "northwind.NonClr",
                        Resource.Property("ProductID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ProductName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40))
                    );
                    ResourceType Customer_and_Suppliers_by_City = Resource.ResourceType("Customer_and_Suppliers_by_City", "northwind.NonClr",
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("CompanyName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("ContactName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("Relationship", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(9))
                    );
                    ResourceType Invoices = Resource.ResourceType("Invoices", "northwind.NonClr",
                        Resource.Property("ShipName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(40)),
                        Resource.Property("ShipAddress", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("ShipCity", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("ShipRegion", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("ShipPostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)),
                        Resource.Property("CustomerID", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(5), NodeFacet.FixedLength(true)),
                        Resource.Property("CustomerName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("Address", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("Region", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("PostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)),
                        Resource.Property("Salesperson", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(31)),
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("OrderDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("RequiredDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("ShippedDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("ShipperName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("ProductID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ProductName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("UnitPrice", Clr.Types.Decimal, Resource.Key(), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("Quantity", Clr.Types.Int16, Resource.Key()),
                        Resource.Property("Discount", Clr.Types.Single, Resource.Key()),
                        Resource.Property("ExtendedPrice", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("Freight", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );
                    ResourceType Order_Details_Extended = Resource.ResourceType("Order_Details_Extended", "northwind.NonClr",
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ProductID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ProductName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("UnitPrice", Clr.Types.Decimal, Resource.Key(), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("Quantity", Clr.Types.Int16, Resource.Key()),
                        Resource.Property("Discount", Clr.Types.Single, Resource.Key()),
                        Resource.Property("ExtendedPrice", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );
                    ResourceType Order_Subtotals = Resource.ResourceType("Order_Subtotals", "northwind.NonClr",
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Subtotal", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );
                    ResourceType Orders_Qry = Resource.ResourceType("Orders_Qry", "northwind.NonClr",
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("CustomerID", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(5), NodeFacet.FixedLength(true)),
                        Resource.Property("EmployeeID", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("OrderDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("RequiredDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("ShippedDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("ShipVia", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Freight", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("ShipName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(40)),
                        Resource.Property("ShipAddress", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("ShipCity", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("ShipRegion", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("ShipPostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)),
                        Resource.Property("CompanyName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("Address", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("Region", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("PostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10))
                    );
                    ResourceType Product_Sales_for_1997 = Resource.ResourceType("Product_Sales_for_1997", "northwind.NonClr",
                        Resource.Property("CategoryName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(15)),
                        Resource.Property("ProductName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("ProductSales", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );
                    ResourceType Products_Above_Average_Price = Resource.ResourceType("Products_Above_Average_Price", "northwind.NonClr",
                        Resource.Property("ProductName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("UnitPrice", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );
                    ResourceType Products_by_Category = Resource.ResourceType("Products_by_Category", "northwind.NonClr",
                        Resource.Property("CategoryName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(15)),
                        Resource.Property("ProductName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("QuantityPerUnit", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(20)),
                        Resource.Property("UnitsInStock", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("Discontinued", Clr.Types.Boolean, Resource.Key())
                    );
                    ResourceType Sales_by_Category = Resource.ResourceType("Sales_by_Category", "northwind.NonClr",
                        Resource.Property("CategoryID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("CategoryName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(15)),
                        Resource.Property("ProductName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("ProductSales", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );
                    ResourceType Sales_Totals_by_Amount = Resource.ResourceType("Sales_Totals_by_Amount", "northwind.NonClr",
                        Resource.Property("SaleAmount", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4)),
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("CompanyName", Clr.Types.String, Resource.Key(), NodeFacet.MaxSize(40)),
                        Resource.Property("ShippedDate", Clr.Types.DateTime, NodeFacet.Nullable(true))
                    );
                    ResourceType Summary_of_Sales_by_Quarter = Resource.ResourceType("Summary_of_Sales_by_Quarter", "northwind.NonClr",
                        Resource.Property("ShippedDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Subtotal", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );
                    ResourceType Summary_of_Sales_by_Year = Resource.ResourceType("Summary_of_Sales_by_Year", "northwind.NonClr",
                        Resource.Property("ShippedDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("Subtotal", Clr.Types.Decimal, NodeFacet.Nullable(true), NodeFacet.Precision(19), NodeFacet.Scale(4))
                    );


                    //Explicity define Many to many relationships here
                    ResourceAssociationEnd CategoriesRole = Resource.End("Categories", Categories, Multiplicity.Zero);
                    ResourceAssociationEnd ProductsRole = Resource.End("Products", Products, Multiplicity.Many);
                    ResourceAssociation FK_Products_Categories = Resource.Association("FK_Products_Categories", CategoriesRole, ProductsRole);

                    ResourceAssociationEnd CustomersRole = Resource.End("Customers", Customers, Multiplicity.Zero);
                    ResourceAssociationEnd OrdersRole = Resource.End("Orders", Orders, Multiplicity.Many);
                    ResourceAssociation FK_Orders_Customers = Resource.Association("FK_Orders_Customers", CustomersRole, OrdersRole);

                    ResourceAssociationEnd CustomerDemographicsRole = Resource.End("CustomerDemographics", CustomerDemographics, Multiplicity.Many);
                    ResourceAssociationEnd CustomersRole11 = Resource.End("Customers", Customers, Multiplicity.Many);
                    ResourceAssociation CustomerCustomerDemo = Resource.Association("CustomerCustomerDemo", CustomerDemographicsRole, CustomersRole11);

                    ResourceAssociationEnd EmployeesRole = Resource.End("Employees", Employees, Multiplicity.Zero);
                    ResourceAssociationEnd Employees1Role = Resource.End("Employees1", Employees, Multiplicity.Many);
                    ResourceAssociation FK_Employees_Employees = Resource.Association("FK_Employees_Employees", EmployeesRole, Employees1Role);

                    ResourceAssociationEnd EmployeesRole11 = Resource.End("Employees", Employees, Multiplicity.Zero);
                    ResourceAssociationEnd OrdersRole11 = Resource.End("Orders", Orders, Multiplicity.Many);
                    ResourceAssociation FK_Orders_Employees = Resource.Association("FK_Orders_Employees", EmployeesRole11, OrdersRole11);

                    ResourceAssociationEnd RegionRole = Resource.End("Region", Region, Multiplicity.Zero);
                    ResourceAssociationEnd TerritoriesRole = Resource.End("Territories", Territories, Multiplicity.Many);
                    ResourceAssociation FK_Territories_Region = Resource.Association("FK_Territories_Region", RegionRole, TerritoriesRole);

                    ResourceAssociationEnd EmployeesRole21 = Resource.End("Employees", Employees, Multiplicity.Many);
                    ResourceAssociationEnd TerritoriesRole11 = Resource.End("Territories", Territories, Multiplicity.Many);
                    ResourceAssociation EmployeeTerritories = Resource.Association("EmployeeTerritories", EmployeesRole21, TerritoriesRole11);

                    ResourceAssociationEnd OrdersRole21 = Resource.End("Orders", Orders, Multiplicity.Zero);
                    ResourceAssociationEnd Order_DetailsRole = Resource.End("Order_Details", Order_Details, Multiplicity.Many);
                    ResourceAssociation FK_Order_Details_Orders = Resource.Association("FK_Order_Details_Orders", OrdersRole21, Order_DetailsRole);

                    ResourceAssociationEnd ShippersRole = Resource.End("Shippers", Shippers, Multiplicity.Zero);
                    ResourceAssociationEnd OrdersRole31 = Resource.End("Orders", Orders, Multiplicity.Many);
                    ResourceAssociation FK_Orders_Shippers = Resource.Association("FK_Orders_Shippers", ShippersRole, OrdersRole31);

                    ResourceAssociationEnd ProductsRole11 = Resource.End("Products", Products, Multiplicity.Zero);
                    ResourceAssociationEnd Order_DetailsRole11 = Resource.End("Order_Details", Order_Details, Multiplicity.Many);
                    ResourceAssociation FK_Order_Details_Products = Resource.Association("FK_Order_Details_Products", ProductsRole11, Order_DetailsRole11);

                    ResourceAssociationEnd SuppliersRole = Resource.End("Suppliers", Suppliers, Multiplicity.Zero);
                    ResourceAssociationEnd ProductsRole21 = Resource.End("Products", Products, Multiplicity.Many);
                    ResourceAssociation FK_Products_Suppliers = Resource.Association("FK_Products_Suppliers", SuppliersRole, ProductsRole21);


                    //Resource navigation properties added here
                    Categories.Properties.Add(Resource.Property("Products", Resource.Collection(Products), FK_Products_Categories, CategoriesRole, ProductsRole));

                    Products.Properties.Add(Resource.Property("Categories", Categories, NodeFacet.Nullable(true), FK_Products_Categories, ProductsRole, CategoriesRole));
                    Products.Properties.Add(Resource.Property("Order_Details", Resource.Collection(Order_Details), FK_Order_Details_Products, ProductsRole11, Order_DetailsRole11));
                    Products.Properties.Add(Resource.Property("Suppliers", Suppliers, NodeFacet.Nullable(true), FK_Products_Suppliers, ProductsRole21, SuppliersRole));

                    Order_Details.Properties.Add(Resource.Property("Orders", Orders, FK_Order_Details_Orders, Order_DetailsRole, OrdersRole21));
                    Order_Details.Properties.Add(Resource.Property("Products", Products, FK_Order_Details_Products, Order_DetailsRole11, ProductsRole11));

                    Orders.Properties.Add(Resource.Property("Customers", Customers, NodeFacet.Nullable(true), FK_Orders_Customers, OrdersRole, CustomersRole));
                    Orders.Properties.Add(Resource.Property("Employees", Employees, NodeFacet.Nullable(true), FK_Orders_Employees, OrdersRole11, EmployeesRole11));
                    Orders.Properties.Add(Resource.Property("Order_Details", Resource.Collection(Order_Details), FK_Order_Details_Orders, OrdersRole21, Order_DetailsRole));
                    Orders.Properties.Add(Resource.Property("Shippers", Shippers, NodeFacet.Nullable(true), FK_Orders_Shippers, OrdersRole31, ShippersRole));

                    Customers.Properties.Add(Resource.Property("Orders", Resource.Collection(Orders), FK_Orders_Customers, CustomersRole, OrdersRole));
                    Customers.Properties.Add(Resource.Property("CustomerDemographics", Resource.Collection(CustomerDemographics), CustomerCustomerDemo, CustomersRole11, CustomerDemographicsRole));

                    CustomerDemographics.Properties.Add(Resource.Property("Customers", Resource.Collection(Customers), CustomerCustomerDemo, CustomerDemographicsRole, CustomersRole11));

                    Employees.Properties.Add(Resource.Property("Employees1", Resource.Collection(Employees), FK_Employees_Employees, EmployeesRole, Employees1Role));
                    Employees.Properties.Add(Resource.Property("Employees2", Employees, NodeFacet.Nullable(true), FK_Employees_Employees, Employees1Role, EmployeesRole));
                    Employees.Properties.Add(Resource.Property("Orders", Resource.Collection(Orders), FK_Orders_Employees, EmployeesRole11, OrdersRole11));
                    Employees.Properties.Add(Resource.Property("Territories", Resource.Collection(Territories), EmployeeTerritories, EmployeesRole21, TerritoriesRole11));

                    Territories.Properties.Add(Resource.Property("Region", Region, FK_Territories_Region, TerritoriesRole, RegionRole));
                    Territories.Properties.Add(Resource.Property("Employees", Resource.Collection(Employees), EmployeeTerritories, TerritoriesRole11, EmployeesRole21));

                    Region.Properties.Add(Resource.Property("Territories", Resource.Collection(Territories), FK_Territories_Region, RegionRole, TerritoriesRole));

                    Shippers.Properties.Add(Resource.Property("Orders", Resource.Collection(Orders), FK_Orders_Shippers, ShippersRole, OrdersRole31));

                    Suppliers.Properties.Add(Resource.Property("Products", Resource.Collection(Products), FK_Products_Suppliers, SuppliersRole, ProductsRole21));

                    //Mark certain Resources as not insertable
                    Alphabetical_list_of_products.IsInsertable = false;
                    Category_Sales_for_1997.IsInsertable = false;
                    Customer_and_Suppliers_by_City.IsInsertable = false;
                    Current_Product_List.IsInsertable = false;
                    Invoices.IsInsertable = false;
                    Order_Subtotals.IsInsertable = false;
                    Order_Details_Extended.IsInsertable = false;
                    Orders_Qry.IsInsertable = false;
                    Product_Sales_for_1997.IsInsertable = false;
                    Products_Above_Average_Price.IsInsertable = false;
                    Products_by_Category.IsInsertable = false;
                    Sales_by_Category.IsInsertable = false;
                    Sales_Totals_by_Amount.IsInsertable = false;
                    Summary_of_Sales_by_Quarter.IsInsertable = false;
                    Summary_of_Sales_by_Year.IsInsertable = false;

                    //Resource Containers added here
                    _serviceContainer = Resource.ServiceContainer(this, "northwind.NonClr",
                            Resource.ResourceContainer("Categories", Categories),
                            Resource.ResourceContainer("CustomerDemographics", CustomerDemographics),
                            Resource.ResourceContainer("Customers", Customers),
                            Resource.ResourceContainer("Employees", Employees),
                            Resource.ResourceContainer("Order_Details", Order_Details),
                            Resource.ResourceContainer("Orders", Orders),
                            Resource.ResourceContainer("Products", Products),
                            Resource.ResourceContainer("Region", Region),
                            Resource.ResourceContainer("Shippers", Shippers),
                            Resource.ResourceContainer("Suppliers", Suppliers),
                            Resource.ResourceContainer("Territories", Territories),
                            Resource.ResourceContainer("Alphabetical_list_of_products", Alphabetical_list_of_products),
                            Resource.ResourceContainer("Category_Sales_for_1997", Category_Sales_for_1997),
                            Resource.ResourceContainer("Current_Product_List", Current_Product_List),
                            Resource.ResourceContainer("Customer_and_Suppliers_by_City", Customer_and_Suppliers_by_City),
                            Resource.ResourceContainer("Invoices", Invoices),
                            Resource.ResourceContainer("Order_Details_Extended", Order_Details_Extended),
                            Resource.ResourceContainer("Order_Subtotals", Order_Subtotals),
                            Resource.ResourceContainer("Orders_Qry", Orders_Qry),
                            Resource.ResourceContainer("Product_Sales_for_1997", Product_Sales_for_1997),
                            Resource.ResourceContainer("Products_Above_Average_Price", Products_Above_Average_Price),
                            Resource.ResourceContainer("Products_by_Category", Products_by_Category),
                            Resource.ResourceContainer("Sales_by_Category", Sales_by_Category),
                            Resource.ResourceContainer("Sales_Totals_by_Amount", Sales_Totals_by_Amount),
                            Resource.ResourceContainer("Summary_of_Sales_by_Quarter", Summary_of_Sales_by_Quarter),
                            Resource.ResourceContainer("Summary_of_Sales_by_Year", Summary_of_Sales_by_Year)
                        );

                    foreach (ResourceContainer rc in _serviceContainer.ResourceContainers)
                    {
                        foreach (ResourceType t in rc.ResourceTypes)
                        {
                            t.InferAssociations();
                        }
                    }
                }
                return _serviceContainer;
            }

            set
            {
                _serviceContainer = value;
            }
        }
        public override ResourceType LanguageDataResource()
        {
            return this.ServiceContainer.ResourceContainers["Region"].BaseType;
        }

    }
}
