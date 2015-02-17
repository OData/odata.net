/****** Object:  Table [#Customers]    Script Date: 11/18/2009 21:20:16 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [#Customers](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Concurrency] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EditTimeStamp] [timestamp] NOT NULL,
	[GuidValue] [uniqueidentifier] NULL,
	[Address_StreetAddress] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Address_City] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Address_State] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Address_PostalCode] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BestFriendId] [int] NULL,
 CONSTRAINT [PK_Customers__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (0, N'Customer 0', N'0', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', NULL)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (1, N'Customer 1', N'1', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 0)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (2, N'Customer 2', N'2', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 1)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (3, N'Customer 3', N'3', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 2)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (4, N'Customer 4', N'4', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 3)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (5, N'Customer 5', N'5', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 4)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (6, N'Customer 6', N'6', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 5)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (7, N'Customer 7', N'7', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 6)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (8, N'Customer 8', N'8', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 7)
INSERT [#Customers] ([ID], [Name], [Concurrency], [GuidValue], [Address_StreetAddress], [Address_City], [Address_State], [Address_PostalCode], [BestFriendId]) VALUES (9, N'Customer 9', N'9', NULL, N'One Microsoft Way', N'Redmond', N'WA', N'98052', 8)
/****** Object:  Table [#Offices]    Script Date: 11/18/2009 21:20:16 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [#Offices](
	[ID] [int] NOT NULL,
	[OfficeNumber] [int] NOT NULL,
	[FloorNumber] [smallint] NOT NULL,
	[BuildingName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Offices__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Offices] ([ID], [OfficeNumber], [FloorNumber], [BuildingName]) VALUES (0, 2218, 2, N'35')
INSERT [#Offices] ([ID], [OfficeNumber], [FloorNumber], [BuildingName]) VALUES (1, 2173, 2, N'35')
INSERT [#Offices] ([ID], [OfficeNumber], [FloorNumber], [BuildingName]) VALUES (2, 2155, 2, N'35')
/****** Object:  Table [#Workers]    Script Date: 11/18/2009 21:20:16 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [#Workers](
	[ID] [int] NOT NULL,
	[FirstName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LastName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MiddleName] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Workers__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Workers] ([ID], [FirstName], [LastName], [MiddleName]) VALUES (0, N'Jimmy', N'Li', NULL)
INSERT [#Workers] ([ID], [FirstName], [LastName], [MiddleName]) VALUES (1, N'Pratik', N'Patel', NULL)
INSERT [#Workers] ([ID], [FirstName], [LastName], [MiddleName]) VALUES (2, N'Peter', N'Qian', NULL)
/****** Object:  Table [#Orders]    Script Date: 11/18/2009 21:20:16 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [#Orders](
	[ID] [int] NOT NULL,
	[CustomerId] [int] NULL,
	[DollarAmount] [float] NOT NULL,
 CONSTRAINT [PK_Orders__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (0, 0, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (1, 1, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (2, 2, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (3, 3, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (4, 4, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (5, 5, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (6, 6, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (7, 7, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (8, 8, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (9, 9, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (100, 0, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (101, 1, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (102, 2, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (103, 3, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (104, 4, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (105, 5, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (106, 6, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (107, 7, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (108, 8, 99)
INSERT [#Orders] ([ID], [CustomerId], [DollarAmount]) VALUES (109, 9, 99)
/****** Object:  Table [#Customers_CustomerBlob]    Script Date: 11/18/2009 21:20:16 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [#Customers_CustomerBlob](
	[ID] [int] NOT NULL,
 CONSTRAINT [PK_Customers_CustomerBlob__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Customers_CustomerBlob] ([ID]) VALUES (1)
INSERT [#Customers_CustomerBlob] ([ID]) VALUES (3)
INSERT [#Customers_CustomerBlob] ([ID]) VALUES (5)
INSERT [#Customers_CustomerBlob] ([ID]) VALUES (7)
INSERT [#Customers_CustomerBlob] ([ID]) VALUES (9)
/****** Object:  Table [#Customers_CustomerWithBirthday]    Script Date: 11/18/2009 21:20:16 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [#Customers_CustomerWithBirthday](
	[Birthday] [datetime] NOT NULL,
	[ID] [int] NOT NULL,
 CONSTRAINT [PK_Customers_CustomerWithBirthday__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Customers_CustomerWithBirthday] ([Birthday], [ID]) VALUES (CAST(0x0000724300000000 AS DateTime), 2)
INSERT [#Customers_CustomerWithBirthday] ([Birthday], [ID]) VALUES (CAST(0x000072C000000000 AS DateTime), 6)
/****** Object:  Table [#Customers_CustomerBlobWithBirthday]    Script Date: 11/18/2009 21:20:16 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [#Customers_CustomerBlobWithBirthday](
	[Birthday] [datetime] NOT NULL,
	[ID] [int] NOT NULL,
 CONSTRAINT [PK_Customers_CustomerBlobWithBirthday__###INSERT#GUID#HERE###] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
INSERT [#Customers_CustomerBlobWithBirthday] ([Birthday], [ID]) VALUES (CAST(0x0000726100000000 AS DateTime), 3)
INSERT [#Customers_CustomerBlobWithBirthday] ([Birthday], [ID]) VALUES (CAST(0x000072DF00000000 AS DateTime), 7)
