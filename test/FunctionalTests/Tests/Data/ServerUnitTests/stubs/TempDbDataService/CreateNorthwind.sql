SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
/****** Object:  Table [#Shippers]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Shippers](
	[ShipperID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Phone] [nvarchar](24) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Shippers__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ShipperID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
SET IDENTITY_INSERT [#Shippers] ON
SET IDENTITY_INSERT [#Shippers] OFF
/****** Object:  Table [#Region]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Region](
	[RegionID] [int] NOT NULL,
	[RegionDescription] [nchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Region__###INSERT#GUID#HERE###] PRIMARY KEY NONCLUSTERED 
(
	[RegionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
/****** Object:  Table [#Suppliers]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Suppliers](
	[SupplierID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ContactName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ContactTitle] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Address] [nvarchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[City] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Region] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PostalCode] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Phone] [nvarchar](24) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Fax] [nvarchar](24) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[HomePage] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Suppliers__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
CREATE NONCLUSTERED INDEX [CompanyName] ON [#Suppliers] 
(
	[CompanyName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [PostalCode] ON [#Suppliers] 
(
	[PostalCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
SET IDENTITY_INSERT [#Suppliers] ON
SET IDENTITY_INSERT [#Suppliers] OFF
/****** Object:  Table [#Employees]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Employees](
	[EmployeeID] [int] IDENTITY(1,1) NOT NULL,
	[LastName] [nvarchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FirstName] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Title] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TitleOfCourtesy] [nvarchar](25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BirthDate] [datetime] NULL,
	[HireDate] [datetime] NULL,
	[Address] [nvarchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[City] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Region] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PostalCode] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[HomePhone] [nvarchar](24) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Extension] [nvarchar](4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Photo] [image] NULL,
	[Notes] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReportsTo] [int] NULL,
	[PhotoPath] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Employees__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
CREATE NONCLUSTERED INDEX [LastName] ON [#Employees] 
(
	[LastName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [PostalCode] ON [#Employees] 
(
	[PostalCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
SET IDENTITY_INSERT [#Employees] ON
INSERT [#Employees] ([EmployeeID], [LastName], [FirstName], [Title], [TitleOfCourtesy], [BirthDate], [HireDate], [Address], [City], [Region], [PostalCode], [HomePhone], [Extension], [Photo], [Notes], [ReportsTo], [PhotoPath]) VALUES (1, N'Davolio', N'Nancy', N'Sales Representative', N'Ms.', CAST(0x000045D100000000 AS DateTime), CAST(0x000083BB00000000 AS DateTime), N'507 - 20th Ave. E. Apt. 2A', N'Seattle', N'WA', N'98122',N'(206) 555-9857', N'5467', 0x0, N'Education includes a BA in psychology from Colorado State University in 1970.  She also completed "The Art of the Cold Call."  Nancy is a member of Toastmasters International.', 2, N'http://accweb/emmployees/davolio.bmp')
SET IDENTITY_INSERT [#Employees] OFF
/****** Object:  Table [#Categories]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Picture] [image] NULL,
 CONSTRAINT [PK_Categories__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
CREATE NONCLUSTERED INDEX [CategoryName] ON [#Categories] 
(
	[CategoryName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
SET IDENTITY_INSERT [#Categories] ON
INSERT [#Categories] ([CategoryID], [CategoryName], [Description], [Picture]) VALUES (1, N'Beverages', N'Soft drinks, coffees, teas, beers, and ales', 0x0)
SET IDENTITY_INSERT [#Categories] OFF
/****** Object:  Table [#Customers]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Customers](
	[CustomerID] [nchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CompanyName] [nvarchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ContactName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ContactTitle] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Address] [nvarchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[City] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Region] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PostalCode] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Phone] [nvarchar](24) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Fax] [nvarchar](24) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_Customers__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
CREATE NONCLUSTERED INDEX [City] ON [#Customers] 
(
	[City] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [CompanyName] ON [#Customers] 
(
	[CompanyName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [PostalCode] ON [#Customers] 
(
	[PostalCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [Region] ON [#Customers] 
(
	[Region] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'ALFKI', N'Alfreds Futterkiste', N'Maria Anders', N'Sales Representative', N'Obere Str. 57', N'Berlin', NULL, N'12209', N'030-0074321', N'030-0076545', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'ANATR', N'Ana Trujillo Emparedados y helados', N'Ana Trujillo', N'Owner', N'Avda. de la Constitución 2222', N'México D.F.', NULL, N'05021', N'(5) 555-4729', N'(5) 555-3745', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'ANTON', N'Antonio Moreno Taquería', N'Antonio Moreno', N'Owner', N'Mataderos  2312', N'México D.F.', NULL, N'05023', N'(5) 555-3932', NULL, CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'AROUT', N'Around the Horn', N'Thomas Hardy', N'Sales Representative', N'120 Hanover Sq.', N'London', NULL, N'WA1 1DP', N'(171) 555-7788', N'(171) 555-6750', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'BERGS', N'Berglunds snabbköp', N'Christina Berglund', N'Order Administrator', N'Berguvsvägen  8', N'Luleå', NULL, N'S-958 22', N'0921-12 34 65', N'0921-12 34 67', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'BLAUS', N'Blauer See Delikatessen', N'Hanna Moos', N'Sales Representative', N'Forsterstr. 57', N'Mannheim', NULL, N'68306', N'0621-08460', N'0621-08924', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'BLONP', N'Blondesddsl père et fils', N'Frédérique Citeaux', N'Marketing Manager', N'24, place Kléber', N'Strasbourg', NULL, N'67000', N'88.60.15.31', N'88.60.15.32', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'BOLID', N'Bólido Comidas preparadas', N'Martín Sommer', N'Owner', N'C/ Araquil, 67', N'Madrid', NULL, N'28023', N'(91) 555 22 82', N'(91) 555 91 99', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'BONAP', N'Bon app''', N'Laurence Lebihan', N'Owner', N'12, rue des Bouchers', N'Marseille', NULL, N'13008', N'91.24.45.40', N'91.24.45.41', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'BOTTM', N'Bottom-Dollar Markets', N'Elizabeth Lincoln', N'Accounting Manager', N'23 Tsawassen Blvd.', N'Tsawassen', N'BC', N'T2F 8M4', N'(604) 555-4729', N'(604) 555-3745', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'BSBEV', N'B''s Beverages', N'Victoria Ashworth', N'Sales Representative', N'Fauntleroy Circus', N'London', NULL, N'EC2 5NT',  N'(171) 555-1212', NULL, CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'CACTU', N'Cactus Comidas para llevar', N'Patricio Simpson', N'Sales Agent', N'Cerrito 333', N'Buenos Aires', NULL, N'1010', N'(1) 135-5555', N'(1) 135-4892', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'CENTC', N'Centro comercial Moctezuma', N'Francisco Chang', N'Marketing Manager', N'Sierras de Granada 9993', N'México D.F.', NULL, N'05022', N'(5) 555-3392', N'(5) 555-7293', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'CONSH', N'Consolidated Holdings', N'Elizabeth Brown', N'Sales Representative', N'Berkeley Gardens 12  Brewery', N'London', NULL, N'WX1 6LT', N'(171) 555-2282', N'(171) 555-9199', CAST(0x00008EAC00000000 AS DateTime))
INSERT [#Customers] ([CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Phone], [Fax], [Created]) VALUES (N'QUICK', N'QUICK-Stop', N'Horst Kloss', N'Accounting Manager', N'Taucherstraße 10', N'Cunewalde', NULL, N'01307', N'0372-035188', NULL, CAST(0x00008EAC00000000 AS DateTime))
/****** Object:  Table [#CustomerDemographics]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#CustomerDemographics](
	[CustomerTypeID] [nchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CustomerDesc] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_CustomerDemographics__###INSERT#GUID#HERE###] PRIMARY KEY NONCLUSTERED 
(
	[CustomerTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
/****** Object:  Table [#Orders]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [nchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EmployeeID] [int] NULL,
	[OrderDate] [datetime] NULL,
	[RequiredDate] [datetime] NULL,
	[ShippedDate] [datetime] NULL,
	[ShipVia] [int] NULL,
	[Freight] [money] NULL,
	[ShipName] [nvarchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipAddress] [nvarchar](60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipCity] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipRegion] [nvarchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShipPostalCode] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Orders__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
CREATE NONCLUSTERED INDEX [CustomerID] ON [#Orders] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [CustomersOrders] ON [#Orders] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [EmployeeID] ON [#Orders] 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [EmployeesOrders] ON [#Orders] 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [OrderDate] ON [#Orders] 
(
	[OrderDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [ShippedDate] ON [#Orders] 
(
	[ShippedDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [ShippersOrders] ON [#Orders] 
(
	[ShipVia] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [ShipPostalCode] ON [#Orders] 
(
	[ShipPostalCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
SET IDENTITY_INSERT [#Orders] ON
INSERT [#Orders] ([OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode]) VALUES (10285, N'QUICK', 1, CAST(0x000089DF00000000 AS DateTime), CAST(0x000089FB00000000 AS DateTime), CAST(0x000089E500000000 AS DateTime), 2, 76.8300, N'QUICK-Stop', N'Taucherstraße 10', N'Cunewalde', NULL, N'01307')
INSERT [#Orders] ([OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode]) VALUES (10355, N'AROUT', 1, CAST(0x00008A3600000000 AS DateTime), CAST(0x00008A5200000000 AS DateTime), CAST(0x00008A3B00000000 AS DateTime), 1, 41.9500, N'Around the Horn', N'Brook Farm Stratford St. Mary', N'Colchester', N'Essex', N'CO7 6JX')
INSERT [#Orders] ([OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode]) VALUES (10383, N'AROUT', 1, CAST(0x00008A5500000000 AS DateTime), CAST(0x00008A7100000000 AS DateTime), CAST(0x00008A5700000000 AS DateTime), 3, 34.2400, N'Around the Horn', N'Brook Farm Stratford St. Mary', N'Colchester', N'Essex', N'CO7 6JX')
INSERT [#Orders] ([OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode]) VALUES (10453, N'AROUT', 1, CAST(0x00008A9800000000 AS DateTime), CAST(0x00008AB400000000 AS DateTime), CAST(0x00008A9D00000000 AS DateTime), 2, 25.3600, N'Around the Horn', N'Brook Farm Stratford St. Mary', N'Colchester', N'Essex', N'CO7 6JX')
INSERT [#Orders] ([OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode]) VALUES (10558, N'AROUT', 1, CAST(0x00008AFF00000000 AS DateTime), CAST(0x00008B1B00000000 AS DateTime), CAST(0x00008B0500000000 AS DateTime), 2, 72.9700, N'Around the Horn', N'Brook Farm Stratford St. Mary', N'Colchester', N'Essex', N'CO7 6JX')
INSERT [#Orders] ([OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode]) VALUES (10707, N'AROUT', 1, CAST(0x00008B8500000000 AS DateTime), CAST(0x00008B9300000000 AS DateTime), CAST(0x00008B8C00000000 AS DateTime), 3, 21.7400, N'Around the Horn', N'Brook Farm Stratford St. Mary', N'Colchester', N'Essex', N'CO7 6JX')
INSERT [#Orders] ([OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode]) VALUES (10741, N'AROUT', 1, CAST(0x00008BA200000000 AS DateTime), CAST(0x00008BB000000000 AS DateTime), CAST(0x00008BA600000000 AS DateTime), 3, 10.9600, N'Around the Horn', N'Brook Farm Stratford St. Mary', N'Colchester', N'Essex', N'CO7 6JX')
SET IDENTITY_INSERT [#Orders] OFF
/****** Object:  Table [#Products]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[SupplierID] [int] NULL,
	[CategoryID] [int] NULL,
	[QuantityPerUnit] [nvarchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UnitPrice] [money] NULL,
	[UnitsInStock] [smallint] NULL,
	[UnitsOnOrder] [smallint] NULL,
	[ReorderLevel] [smallint] NULL,
	[Discontinued] [bit] NOT NULL,
 CONSTRAINT [PK_Products__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
CREATE NONCLUSTERED INDEX [CategoriesProducts] ON [#Products] 
(
	[CategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [CategoryID] ON [#Products] 
(
	[CategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [ProductName] ON [#Products] 
(
	[ProductName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [SupplierID] ON [#Products] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [SuppliersProducts] ON [#Products] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
SET IDENTITY_INSERT [#Products] ON
INSERT [#Products] ([ProductID], [ProductName], [SupplierID], [CategoryID], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (1, N'Chai', 1, 1, N'10 boxes x 20 bags', 18.0000, 39, 0, 10, 0)
INSERT [#Products] ([ProductID], [ProductName], [SupplierID], [CategoryID], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (24, N'Guaraná Fantástica', 10, 1, N'12 - 355 ml cans', 4.5000, 20, 0, 0, 1)
SET IDENTITY_INSERT [#Products] OFF
/****** Object:  Table [#CustomerCustomerDemo]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#CustomerCustomerDemo](
	[CustomerID] [nchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CustomerTypeID] [nchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_CustomerCustomerDemo__###INSERT#GUID#HERE###] PRIMARY KEY NONCLUSTERED 
(
	[CustomerID] ASC,
	[CustomerTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
/****** Object:  Table [#Territories]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Territories](
	[TerritoryID] [nvarchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[TerritoryDescription] [nchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[RegionID] [int] NOT NULL,
 CONSTRAINT [PK_Territories__###INSERT#GUID#HERE###] PRIMARY KEY NONCLUSTERED 
(
	[TerritoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
/****** Object:  StoredProcedure [#Ten Most Expensive Products]    Script Date: 11/17/2008 11:46:24 ******/
EXEC dbo.sp_executesql @statement = N'
create procedure [#Ten Most Expensive Products] AS
SET ROWCOUNT 10
SELECT Products.ProductName AS TenMostExpensiveProducts, Products.UnitPrice
FROM #Products AS Products
ORDER BY Products.UnitPrice DESC
' 
/****** Object:  Table [#EmployeeTerritories]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#EmployeeTerritories](
	[EmployeeID] [int] NOT NULL,
	[TerritoryID] [nvarchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_EmployeeTerritories__###INSERT#GUID#HERE###] PRIMARY KEY NONCLUSTERED 
(
	[EmployeeID] ASC,
	[TerritoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
/****** Object:  Table [#Order Details]    Script Date: 11/17/2008 11:46:24 ******/
CREATE TABLE [#Order Details](
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[Quantity] [smallint] NOT NULL,
	[Discount] [real] NOT NULL,
 CONSTRAINT [PK_Order_Details__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
CREATE NONCLUSTERED INDEX [OrderID] ON [#Order Details] 
(
	[OrderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [OrdersOrder_Details] ON [#Order Details] 
(
	[OrderID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [ProductID] ON [#Order Details] 
(
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
CREATE NONCLUSTERED INDEX [ProductsOrder_Details] ON [#Order Details] 
(
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
INSERT [#Order Details] ([OrderID], [ProductID], [UnitPrice], [Quantity], [Discount]) VALUES (10285, 1, 14.4000, 45, 0.2)
INSERT [#Order Details] ([OrderID], [ProductID], [UnitPrice], [Quantity], [Discount]) VALUES (10355, 24, 3.6000, 25, 0)
/****** Object:  StoredProcedure [#CustOrdersOrders]    Script Date: 11/17/2008 11:46:24 ******/
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [#CustOrdersOrders] @CustomerID nchar(5)
AS
SELECT OrderID, 
	OrderDate,
	RequiredDate,
	ShippedDate
FROM #Orders AS Orders
WHERE CustomerID = @CustomerID
ORDER BY OrderID
' 
/****** Object:  StoredProcedure [#CustOrdersDetail]    Script Date: 11/17/2008 11:46:24 ******/
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [#CustOrdersDetail] @OrderID int
AS
SELECT ProductName,
    UnitPrice=ROUND(Od.UnitPrice, 2),
    Quantity,
    Discount=CONVERT(int, Discount * 100), 
    ExtendedPrice=ROUND(CONVERT(money, Quantity * (1 - Discount) * Od.UnitPrice), 2)
FROM #Products P, [#Order Details] Od
WHERE Od.ProductID = P.ProductID and Od.OrderID = @OrderID
' 
/****** Object:  StoredProcedure [#CustOrderHist]    Script Date: 11/17/2008 11:46:24 ******/
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [#CustOrderHist] @CustomerID nchar(5)
AS
SELECT ProductName, Total=SUM(Quantity)
FROM #Products P, [#Order Details] OD, #Orders O, #Customers C
WHERE C.CustomerID = @CustomerID
AND C.CustomerID = O.CustomerID AND O.OrderID = OD.OrderID AND OD.ProductID = P.ProductID
GROUP BY ProductName
' 
/****** Object:  StoredProcedure [#SalesByCategory]    Script Date: 11/17/2008 11:46:24 ******/
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [#SalesByCategory]
    @CategoryName nvarchar(15), @OrdYear nvarchar(4) = ''1998''
AS
IF @OrdYear != ''1996'' collate SQL_Latin1_General_CP1_CI_AS AND @OrdYear != ''1997'' collate SQL_Latin1_General_CP1_CI_AS AND @OrdYear != ''1998'' collate SQL_Latin1_General_CP1_CI_AS
BEGIN
	SELECT @OrdYear = ''1998''
END
SELECT ProductName,
	TotalPurchase=ROUND(SUM(CONVERT(decimal(14,2), OD.Quantity * (1-OD.Discount) * OD.UnitPrice)), 0)
FROM [#Order Details] OD, #Orders O, #Products P, #Categories C
WHERE OD.OrderID = O.OrderID 
	AND OD.ProductID = P.ProductID 
	AND P.CategoryID = C.CategoryID
	AND C.CategoryName = @CategoryName
	AND SUBSTRING(CONVERT(nvarchar(22), O.OrderDate, 111), 1, 4) collate SQL_Latin1_General_CP1_CI_AS = @OrdYear
GROUP BY ProductName
ORDER BY ProductName
' 
/****** Object:  StoredProcedure [#Sales by Year]    Script Date: 11/17/2008 11:46:24 ******/
EXEC dbo.sp_executesql @statement = N'
create procedure [#Sales by Year] 
	@Beginning_Date DateTime, @Ending_Date DateTime AS
SELECT Orders.ShippedDate, Orders.OrderID, "Order Subtotals".Subtotal, DATENAME(yy,ShippedDate) AS Year
FROM #Orders AS Orders INNER JOIN [#Order Subtotals] AS "Order Subtotals" ON Orders.OrderID = "Order Subtotals".OrderID
WHERE Orders.ShippedDate Between @Beginning_Date And @Ending_Date
' 
/****** Object:  Default [DF_Customers_Created]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Customers] ADD  CONSTRAINT [DF_Customers_Created__###INSERT#GUID#HERE###]  DEFAULT ('2000-01-01 00:00:00.000') FOR [Created]
/****** Object:  Default [DF_Order_Details_UnitPrice]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Order Details] ADD  CONSTRAINT [DF_Order_Details_UnitPrice__###INSERT#GUID#HERE###]  DEFAULT ((0)) FOR [UnitPrice]
/****** Object:  Default [DF_Order_Details_Quantity]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Order Details] ADD  CONSTRAINT [DF_Order_Details_Quantity__###INSERT#GUID#HERE###]  DEFAULT ((1)) FOR [Quantity]
/****** Object:  Default [DF_Order_Details_Discount]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Order Details] ADD  CONSTRAINT [DF_Order_Details_Discount__###INSERT#GUID#HERE###]  DEFAULT ((0)) FOR [Discount]
/****** Object:  Default [DF_Orders_Freight]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Orders] ADD  CONSTRAINT [DF_Orders_Freight__###INSERT#GUID#HERE###]  DEFAULT ((0)) FOR [Freight]
/****** Object:  Default [DF_Products_UnitPrice]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products] ADD  CONSTRAINT [DF_Products_UnitPrice__###INSERT#GUID#HERE###]  DEFAULT ((0)) FOR [UnitPrice]
/****** Object:  Default [DF_Products_UnitsInStock]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products] ADD  CONSTRAINT [DF_Products_UnitsInStock__###INSERT#GUID#HERE###]  DEFAULT ((0)) FOR [UnitsInStock]
/****** Object:  Default [DF_Products_UnitsOnOrder]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products] ADD  CONSTRAINT [DF_Products_UnitsOnOrder__###INSERT#GUID#HERE###]  DEFAULT ((0)) FOR [UnitsOnOrder]
/****** Object:  Default [DF_Products_ReorderLevel]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products] ADD  CONSTRAINT [DF_Products_ReorderLevel__###INSERT#GUID#HERE###]  DEFAULT ((0)) FOR [ReorderLevel]
/****** Object:  Default [DF_Products_Discontinued]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products] ADD  CONSTRAINT [DF_Products_Discontinued__###INSERT#GUID#HERE###]  DEFAULT ((0)) FOR [Discontinued]
/****** Object:  Check [CK_Birthdate]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Employees]  WITH NOCHECK ADD  CONSTRAINT [CK_Birthdate__###INSERT#GUID#HERE###] CHECK  (([BirthDate]<getdate()))
ALTER TABLE [#Employees] CHECK CONSTRAINT [CK_Birthdate__###INSERT#GUID#HERE###]
/****** Object:  Check [CK_Discount]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Order Details]  WITH NOCHECK ADD  CONSTRAINT [CK_Discount__###INSERT#GUID#HERE###] CHECK  (([Discount]>=(0) AND [Discount]<=(1)))
ALTER TABLE [#Order Details] CHECK CONSTRAINT [CK_Discount__###INSERT#GUID#HERE###]
/****** Object:  Check [CK_Quantity]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Order Details]  WITH NOCHECK ADD  CONSTRAINT [CK_Quantity__###INSERT#GUID#HERE###] CHECK  (([Quantity]>(0)))
ALTER TABLE [#Order Details] CHECK CONSTRAINT [CK_Quantity__###INSERT#GUID#HERE###]
/****** Object:  Check [CK_UnitPrice]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Order Details]  WITH NOCHECK ADD  CONSTRAINT [CK_UnitPrice__###INSERT#GUID#HERE###] CHECK  (([UnitPrice]>=(0)))
ALTER TABLE [#Order Details] CHECK CONSTRAINT [CK_UnitPrice__###INSERT#GUID#HERE###]
/****** Object:  Check [CK_Products_UnitPrice]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products]  WITH NOCHECK ADD  CONSTRAINT [CK_Products_UnitPrice__###INSERT#GUID#HERE###] CHECK  (([UnitPrice]>=(0)))
ALTER TABLE [#Products] CHECK CONSTRAINT [CK_Products_UnitPrice__###INSERT#GUID#HERE###]
/****** Object:  Check [CK_ReorderLevel]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products]  WITH NOCHECK ADD  CONSTRAINT [CK_ReorderLevel__###INSERT#GUID#HERE###] CHECK  (([ReorderLevel]>=(0)))
ALTER TABLE [#Products] CHECK CONSTRAINT [CK_ReorderLevel__###INSERT#GUID#HERE###]
/****** Object:  Check [CK_UnitsInStock]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products]  WITH NOCHECK ADD  CONSTRAINT [CK_UnitsInStock__###INSERT#GUID#HERE###] CHECK  (([UnitsInStock]>=(0)))
ALTER TABLE [#Products] CHECK CONSTRAINT [CK_UnitsInStock__###INSERT#GUID#HERE###]
/****** Object:  Check [CK_UnitsOnOrder]    Script Date: 11/17/2008 11:46:24 ******/
ALTER TABLE [#Products]  WITH NOCHECK ADD  CONSTRAINT [CK_UnitsOnOrder__###INSERT#GUID#HERE###] CHECK  (([UnitsOnOrder]>=(0)))
ALTER TABLE [#Products] CHECK CONSTRAINT [CK_UnitsOnOrder__###INSERT#GUID#HERE###]
