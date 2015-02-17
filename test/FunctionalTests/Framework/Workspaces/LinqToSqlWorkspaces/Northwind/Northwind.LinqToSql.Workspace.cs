//---------------------------------------------------------------------
// <copyright file="Northwind.LinqToSql.Workspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;

    [WorkspaceAttribute("Northwind", DataLayerProviderKind.LinqToSql, Priority = 1, Standard = true)]
    public class LinqToSqlNorthwindWorkspace : System.Data.Test.Astoria.LinqToSqlWorkspace
    {
        public LinqToSqlNorthwindWorkspace()
            : base("Northwind", "northwind", "northwindDataContext")
        {
            this.Language = WorkspaceLanguage.CSharp;
        }

        public override ServiceContainer ServiceContainer
        {
            get
            {
                if (_serviceContainer == null)
                {

                    ResourceType Categories = Resource.ResourceType("Categories", "northwind",
                        Resource.Property("CategoryID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("CategoryName", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(15)),
                        Resource.Property("Description", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false)),
                        Resource.Property("Picture", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.Sortable(false), NodeFacet.UnderlyingType(UnderlyingType.Image)));

                    ResourceType CustomerCustomerDemo = Resource.ResourceType("CustomerCustomerDemo", "northwind",
                        Resource.Property("CustomerID", Clr.Types.String, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.MaxSize(5)),
                        Resource.Property("CustomerTypeID", Clr.Types.String, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.MaxSize(10)));

                    ResourceType CustomerDemographics = Resource.ResourceType("CustomerDemographics", "northwind",
                        Resource.Property("CustomerTypeID", Clr.Types.String, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.MaxSize(10)),
                        Resource.Property("CustomerDesc", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false)));

                    ResourceType Customers = Resource.ResourceType("Customers", "northwind",
                        Resource.Property("CustomerID", Clr.Types.String, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.MaxSize(5)),
                        Resource.Property("CompanyName", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(40)),
                        Resource.Property("ContactName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("ContactTitle", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("Address", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("Region", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("PostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)),
                        Resource.Property("Phone", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)),
                        Resource.Property("Fax", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)));

                    ResourceType Employees = Resource.ResourceType("Employees", "northwind",
                        Resource.Property("EmployeeID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("LastName", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(20)),
                        Resource.Property("FirstName", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(10)),
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
                        Resource.Property("Photo", LinqToSqlTypes.Binary, NodeFacet.Nullable(true), NodeFacet.Sortable(false), NodeFacet.UnderlyingType(UnderlyingType.Image)),
                        Resource.Property("Notes", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false)),
                        Resource.Property("ReportsTo", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("PhotoPath", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(255)));

                    ResourceType EmployeeTerritories = Resource.ResourceType("EmployeeTerritories", "northwind",
                        Resource.Property("EmployeeID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("TerritoryID", Clr.Types.String, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.MaxSize(20)));

                    ResourceType OrderDetails = Resource.ResourceType("OrderDetails", "northwind",
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("ProductID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("UnitPrice", Clr.Types.Decimal, NodeFacet.Nullable(false)),
                        Resource.Property("Quantity", Clr.Types.Int16, NodeFacet.Nullable(false)),
                        Resource.Property("Discount", SqlTypes.Real, NodeFacet.Nullable(false)));

                    ResourceType Orders = Resource.ResourceType("Orders", "northwind",
                        Resource.Property("OrderID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("CustomerID", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(5)),
                        Resource.Property("EmployeeID", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("OrderDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("RequiredDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("ShippedDate", Clr.Types.DateTime, NodeFacet.Nullable(true)),
                        Resource.Property("ShipVia", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("Freight", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("ShipName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(40)),
                        Resource.Property("ShipAddress", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("ShipCity", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("ShipRegion", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("ShipPostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)));

                    ResourceType Products = Resource.ResourceType("Products", "northwind",
                        Resource.Property("ProductID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("ProductName", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(40)),
                        Resource.Property("SupplierID", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("CategoryID", Clr.Types.Int32, NodeFacet.Nullable(true)),
                        Resource.Property("QuantityPerUnit", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(20)),
                        Resource.Property("UnitPrice", Clr.Types.Decimal, NodeFacet.Nullable(true)),
                        Resource.Property("UnitsInStock", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("UnitsOnOrder", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("ReorderLevel", Clr.Types.Int16, NodeFacet.Nullable(true)),
                        Resource.Property("Discontinued", Clr.Types.Boolean, NodeFacet.Nullable(false)));

                    ResourceType Region = Resource.ResourceType("Region", "northwind",
                        Resource.Property("RegionID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false)),
                        Resource.Property("RegionDescription", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(50)));

                    ResourceType Shippers = Resource.ResourceType("Shippers", "northwind",
                        Resource.Property("ShipperID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("CompanyName", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(40)),
                        Resource.Property("Phone", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)));

                    ResourceType Suppliers = Resource.ResourceType("Suppliers", "northwind",
                        Resource.Property("SupplierID", Clr.Types.Int32, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.ServerGenerated(true)),
                        Resource.Property("CompanyName", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(40)),
                        Resource.Property("ContactName", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("ContactTitle", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(30)),
                        Resource.Property("Address", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(60)),
                        Resource.Property("City", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("Region", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(15)),
                        Resource.Property("PostalCode", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(10)),
                        Resource.Property("Phone", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)),
                        Resource.Property("Fax", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.MaxSize(24)),
                        Resource.Property("HomePage", Clr.Types.String, NodeFacet.Nullable(true), NodeFacet.Sortable(false)));

                    ResourceType Territories = Resource.ResourceType("Territories", "northwind",
                        Resource.Property("TerritoryID", Clr.Types.String, Resource.Key(), NodeFacet.Nullable(false), NodeFacet.MaxSize(20)),
                        Resource.Property("TerritoryDescription", Clr.Types.String, NodeFacet.Nullable(false), NodeFacet.MaxSize(50)),
                        Resource.Property("RegionID", Clr.Types.Int32, NodeFacet.Nullable(false)));

                    Categories.Properties.Add(Resource.Property("Products", Resource.Collection(Products)));

                    CustomerCustomerDemo.Properties.Add(Resource.Property("CustomerDemographics", CustomerDemographics));
                    CustomerCustomerDemo.Properties.Add(Resource.Property("Customers", Customers));

                    CustomerDemographics.Properties.Add(Resource.Property("CustomerCustomerDemo", Resource.Collection(CustomerCustomerDemo)));

                    Customers.Properties.Add(Resource.Property("CustomerCustomerDemo", Resource.Collection(CustomerCustomerDemo)));
                    Customers.Properties.Add(Resource.Property("Orders", Resource.Collection(Orders)));

                    Employees.Properties.Add(Resource.Property("ReportsToEmployees", Employees));
                    Employees.Properties.Add(Resource.Property("Employee", Resource.Collection(Employees)));
                    Employees.Properties.Add(Resource.Property("EmployeeTerritories", Resource.Collection(EmployeeTerritories)));
                    Employees.Properties.Add(Resource.Property("Orders", Resource.Collection(Orders)));

                    EmployeeTerritories.Properties.Add(Resource.Property("Employees", Employees));
                    EmployeeTerritories.Properties.Add(Resource.Property("Territories", Territories));

                    OrderDetails.Properties.Add(Resource.Property("Orders", Orders));
                    OrderDetails.Properties.Add(Resource.Property("Products", Products));

                    Orders.Properties.Add(Resource.Property("OrderDetails", Resource.Collection(OrderDetails)));
                    Orders.Properties.Add(Resource.Property("Customers", Customers));
                    Orders.Properties.Add(Resource.Property("Employees", Employees));
                    Orders.Properties.Add(Resource.Property("Shippers", Shippers));

                    Products.Properties.Add(Resource.Property("OrderDetails", Resource.Collection(OrderDetails)));
                    Products.Properties.Add(Resource.Property("Categories", Categories));
                    Products.Properties.Add(Resource.Property("Suppliers", Suppliers));

                    Region.Properties.Add(Resource.Property("Territories", Resource.Collection(Territories)));

                    Shippers.Properties.Add(Resource.Property("Orders", Resource.Collection(Orders)));

                    Suppliers.Properties.Add(Resource.Property("Products", Resource.Collection(Products)));

                    Territories.Properties.Add(Resource.Property("EmployeeTerritories", Resource.Collection(EmployeeTerritories)));
                    Territories.Properties.Add(Resource.Property("Region", Region));

                    _serviceContainer = Resource.ServiceContainer(this, "Northwind",
                        Resource.ResourceContainer("Categories", Categories),
                        Resource.ResourceContainer("CustomerCustomerDemo", CustomerCustomerDemo),
                        Resource.ResourceContainer("CustomerDemographics", CustomerDemographics),
                        Resource.ResourceContainer("Customers", Customers),
                        Resource.ResourceContainer("Employees", Employees),
                        Resource.ResourceContainer("EmployeeTerritories", EmployeeTerritories),
                        Resource.ResourceContainer("OrderDetails", OrderDetails),
                        Resource.ResourceContainer("Orders", Orders),
                        Resource.ResourceContainer("Products", Products),
                        Resource.ResourceContainer("Region", Region),
                        Resource.ResourceContainer("Shippers", Shippers),
                        Resource.ResourceContainer("Suppliers", Suppliers),
                        Resource.ResourceContainer("Territories", Territories)
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
        }

        public override ResourceType LanguageDataResource()
        {
            return this.ServiceContainer.ResourceContainers["Region"].BaseType;
        }
    }
}
