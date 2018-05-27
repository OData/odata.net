'---------------------------------------------------------------------
' <copyright file="DataBinding.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Threading
Imports System.Reflection
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.CodeDom.Compiler
Imports System.Linq

Partial Public Class ClientModule

    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    <DeploymentItem("Workspaces", "Workspaces")>
    <TestClass()> Public Class DataBinding

        ' Flag to test whether EntityClassGenerator.GenerateCode overload with TextWriter has been tested.
        Private Shared TextWriterTested As Boolean
        Private exceptions As New List(Of Exception)
        Private count As Int32
        Private resetEvents As New ManualResetEvent(False)
        Private Shared web As TestWebRequest = Nothing
        Private ctx As NorthwindBindingModel.NorthwindContext = Nothing
        Private entityChangedEventFired As Boolean
        Private entityCollectionChangedEventFired As Boolean
        Private entityChangedCount As Integer
        Private entityCollectionChangedCount As Integer

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.DataServiceType = ServiceModelData.Northwind.ServiceModelType
            web.StartService()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.Dispose()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Me.ctx = New NorthwindBindingModel.NorthwindContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub

        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub DataBindingTest1()
            Dim customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(Me.ctx.Customers.Expand("Orders($expand=Customers)"))
            Assert.IsTrue(customers.Count > 0)
            customers.Detach()
            Dim orders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            orders.Load(Me.ctx.Orders.Expand("Customers($expand=Orders)"))
            Assert.IsTrue(customers.Count > 0)
            orders.Detach()
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub DataBindingTest2()

            Dim nCustomers As Integer
            Dim nOrders As Integer

            Dim customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(Me.ctx.Customers.Expand("Orders($expand=Customers)"))
            Dim orders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            orders.Load(Me.ctx.Orders.Expand("Customers($expand=Orders)"))

            nCustomers = customers.Count
            nOrders = orders.Count

            Dim c1 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST1", "Company1")
            c1.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c1.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c1.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))

            Dim c2 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST2", "Company2")
            c2.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c2.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c2.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))

            Dim c3 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST3", "Company3")
            c3.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c3.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c3.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx, Nothing, Nothing, AddressOf Me.OnEntityCollectionChanged)
            customers.Add(c1)
            customers.Add(c2)
            customers.Add(c3)

            Util.SaveChanges(ctx)

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers.Expand("Orders") Where c.CustomerID.StartsWith("CUST") Select c)

            Assert.AreEqual(customers.Count, 3)

            For Each customer In customers
                Assert.AreEqual(customer.Orders.Count, 3)
            Next

            DeleteCustomers()

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers.Expand("Orders") Where c.CustomerID.StartsWith("CUST") Select c)

            Assert.AreEqual(customers.Count, 0)

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(Me.ctx.Customers.Expand("Orders($expand=Customers)"))
            orders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            orders.Load(Me.ctx.Orders.Expand("Customers($expand=Orders)"))

            Assert.AreEqual(customers.Count, nCustomers)
            Assert.AreEqual(orders.Count, nOrders)

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub DataBindingTest3()
            Dim customers As DataServiceCollection(Of NorthwindBindingModel.Customers) =
                New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx, (From c In Me.ctx.Customers Where c.CustomerID.Equals("ANTON") Or c.CustomerID.Equals("BLAUS") Select c),
                    TrackingMode.AutoChangeTracking, Nothing, AddressOf Me.OnEntityChanged, Nothing)

            For Each customer In customers
                Me.ctx.LoadProperty(customer, "Orders")
            Next

            Dim temp = New ArrayList()

            For Each customer In customers
                For Each order In customer.Orders
                    temp.Add(order.Freight)
                    order.Freight = 111.11D
                Next
            Next

            Util.SaveChanges(ctx)

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers Where c.CustomerID.Equals("ANTON") Or c.CustomerID.Equals("BLAUS") Select c)

            For Each customer In customers
                Me.ctx.LoadProperty(customer, "Orders")
            Next

            For Each customer In customers
                For Each order In customer.Orders
                    Assert.AreEqual(order.Freight, 111.11D)
                Next
            Next

            Dim i As Integer = 0

            For Each customer In customers
                For Each order In customer.Orders
                    order.Freight = DirectCast(temp(i), Decimal)
                    i = i + 1
                Next
            Next

            Util.SaveChanges(ctx)

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers Where c.CustomerID.Equals("ANTON") Or c.CustomerID.Equals("BLAUS") Select c)

            For Each customer In customers
                Me.ctx.LoadProperty(customer, "Orders")
            Next

            i = 0

            For Each customer In customers
                For Each order In customer.Orders
                    Assert.AreEqual(order.Freight, DirectCast(temp(i), Decimal))
                    i = i + 1
                Next
            Next

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub DataBindingTest4()
            Dim nCustomers As Integer
            Dim nOrders As Integer

            Dim customers As DataServiceCollection(Of NorthwindBindingModel.Customers) = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(Me.ctx.Customers.Expand("Orders($expand=Customers)"))
            Dim orders As DataServiceCollection(Of NorthwindBindingModel.Orders) = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            orders.Load(Me.ctx.Orders.Expand("Customers($expand=Orders)"))

            nCustomers = customers.Count
            nOrders = orders.Count

            Dim c1 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST1", "Company1")
            Dim c2 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST2", "Company2")
            Dim c3 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST3", "Company3")

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)

            customers.Add(c1)
            customers.Add(c2)
            customers.Add(c3)

            Dim c4 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST4", "Company4")
            Dim c5 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST5", "Company5")
            Dim c6 As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST6", "Company6")

            c4.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c5.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c6.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))

            c4.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c5.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c6.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))

            c4.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c5.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))
            c6.Orders.Add(NorthwindBindingModel.Orders.CreateOrders(0))

            customers(0) = c4
            customers(1) = c5
            customers(2) = c6

            Util.SaveChanges(ctx)

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers.Expand("Orders") Where c.CustomerID.StartsWith("CUST") Select c)

            Assert.AreEqual(customers.Count, 3)

            For Each customer In customers
                Assert.IsTrue(customer.CustomerID.Equals("CUST4") Or customer.CustomerID.Equals("CUST5") Or customer.CustomerID.Equals("CUST6"))
                Assert.AreEqual(customer.Orders.Count, 3)
            Next

            DeleteCustomers()

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers.Expand("Orders") Where c.CustomerID.StartsWith("CUST") Select c)

            Assert.AreEqual(customers.Count, 0)

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(Me.ctx.Customers.Expand("Orders($expand=Customers)"))
            orders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            orders.Load(Me.ctx.Orders.Expand("Customers($expand=Orders)"))

            Assert.AreEqual(customers.Count, nCustomers)
            Assert.AreEqual(orders.Count, nOrders)

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub DataBindingTest5()
            Me.ctx.MergeOption = MergeOption.OverwriteChanges

            Dim nCustomers As Integer
            Dim nOrders As Integer

            Dim customers As DataServiceCollection(Of NorthwindBindingModel.Customers) = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(Me.ctx.Customers.Expand("Orders($expand=Customers)"))
            Dim orders As DataServiceCollection(Of NorthwindBindingModel.Orders) = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            orders.Load(Me.ctx.Orders.Expand("Customers($expand=Orders)"))

            nCustomers = customers.Count
            nOrders = orders.Count

            Dim order As NorthwindBindingModel.Orders = NorthwindBindingModel.Orders.CreateOrders(0)
            Dim customer As NorthwindBindingModel.Customers = NorthwindBindingModel.Customers.CreateCustomers("CUST1", "Company1")

            orders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(ctx, Nothing, AddressOf Me.OnEntityChanged, Nothing)
            orders.Add(order)

            order.Customers = customer

            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset)

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers.Expand("Orders") Where c.CustomerID.StartsWith("CUST") Select c)

            Assert.AreEqual(customers.Count, 1)
            Assert.AreEqual(customers(0).Orders.Count, 1)


            DeleteCustomers()

            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers.Expand("Orders") Where c.CustomerID.StartsWith("CUST") Select c)
            Assert.AreEqual(customers.Count, 0)


            customers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(Me.ctx.Customers.Expand("Orders($expand=Customers)"))
            orders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            orders.Load(Me.ctx.Orders.Expand("Customers($expand=Orders)"))

            Assert.AreEqual(customers.Count, nCustomers)
            Assert.AreEqual(orders.Count, nOrders)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub DataBindingTest6()
            Me.ctx.MergeOption = MergeOption.OverwriteChanges

            Dim uri As New Uri("Customers", UriKind.Relative)
            Dim col3 = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            col3.Load(Me.ctx.Execute(Of NorthwindBindingModel.Customers)(uri))
            Assert.IsTrue(col3.Count > 0)
            col3.Detach()
            Dim success = False
            Try
                col3.Detach()
            Catch ex As InvalidOperationException
                success = True
            End Try
            Assert.IsTrue(success)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub LoadMultipleLevelNavigationProperties()
            Dim oecCustomers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            oecCustomers.Load((From c In Me.ctx.Customers Select c).Skip(10).Take(1))
            Dim c1 As NorthwindBindingModel.Customers = oecCustomers.Single()
            Me.ctx.LoadProperty(c1, "Orders")
            For Each o As NorthwindBindingModel.Orders In c1.Orders
                Me.ctx.LoadProperty(o, "Employees")
                If Not o.Employees Is Nothing Then
                    Me.ctx.LoadProperty(o.Employees, "Territories")
                    For Each t As NorthwindBindingModel.Territories In o.Employees.Territories
                        Me.ctx.LoadProperty(t, "Region")
                    Next
                End If
            Next

            Dim o1 As NorthwindBindingModel.Orders = DirectCast(Me.ctx.Entities.First(Function(e) TypeOf e.Entity Is NorthwindBindingModel.Orders).Entity, NorthwindBindingModel.Orders)
            o1.Employees = Nothing

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub LoadExpandedDataServiceCollection()
            ' this query is taking too long to run, for this test, we should cap the top level to 2 items
            Dim oec1 = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            oec1.Load(Me.ctx.CreateQuery(Of NorthwindBindingModel.Customers)("Customers").Expand("Orders($expand=Employees)").Take(2))
            Dim oec2 = New DataServiceCollection(Of NorthwindBindingModel.Customers)(New NorthwindBindingModel.NorthwindContext(New Uri("http://localhost")))
            oec2.Load(oec1)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub LoadDataServiceCollection()
            Dim web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf
            web1.DataServiceType = ServiceModelData.CustomData.ServiceModelType
            web1.StartService()

            Using AstoriaUnitTests.Stubs.CustomDataContext.CreateChangeScope()
                Try
                    Dim customDataContext As AstoriaClientUnitTests.Stubs.CustomDataContext = New AstoriaClientUnitTests.Stubs.CustomDataContext(web1.ServiceRoot)
                    'customDataContext.EnableAtom = True
                    'customDataContext.Format.UseAtom()
                    customDataContext.ResolveName = AddressOf ResolveName
                    customDataContext.ResolveType = AddressOf ResolveType

                    Dim customerColl = New DataServiceCollection(Of AstoriaClientUnitTests.Stubs.Customer)(customDataContext,
                        customDataContext.Execute(Of AstoriaClientUnitTests.Stubs.Customer)(New Uri("Customers", UriKind.Relative)),
                        TrackingMode.AutoChangeTracking, "Customers", Nothing, Nothing)

                    Assert.IsTrue(customerColl.Count > 0, "There are more than one customers in the count")

                    Dim ordersColl = customDataContext.Orders.Skip(100).Take(3)

                    Dim c As AstoriaClientUnitTests.Stubs.Customer = customerColl.First()
                    c.Orders.Load(ordersColl)

                    ' Make sure that the order is in Added state and link appears in the added state
                    For Each o As AstoriaClientUnitTests.Stubs.Order In ordersColl
                        Util.VerifyObject(customDataContext, o, EntityStates.Added)
                        Util.VerifyLink(customDataContext, c, "Orders", o, EntityStates.Added)
                    Next

                    Dim lstOrders As List(Of AstoriaClientUnitTests.Stubs.Order) = New List(Of AstoriaClientUnitTests.Stubs.Order)()
                    Dim o1 = New AstoriaClientUnitTests.Stubs.Order
                    o1.ID = 9997
                    Dim o2 = New AstoriaClientUnitTests.Stubs.Order
                    o2.ID = 9998
                    Dim o3 = New AstoriaClientUnitTests.Stubs.Order
                    o3.ID = 9999
                    lstOrders.Add(o1)
                    lstOrders.Add(o2)
                    lstOrders.Add(o3)
                    c.Orders.Load(lstOrders)

                    For Each o As AstoriaClientUnitTests.Stubs.Order In lstOrders
                        Util.VerifyObject(customDataContext, o, EntityStates.Unchanged)
                        Util.VerifyLink(customDataContext, c, "Orders", o, EntityStates.Unchanged)
                    Next

                Finally
                    If Not web1 Is Nothing Then
                        web1.Dispose()
                    End If
                End Try
            End Using
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub LoadWithDeleteLink()

            Dim c = CreateCustomer()
            Dim o = CreateOrder()

            Me.ctx.AddObject("Customers", c)
            Me.ctx.AttachTo("Orders", o)
            Me.ctx.DeleteObject(o)

            c.Orders.Add(o)

            Dim lstCustomers = New List(Of NorthwindBindingModel.Customers)
            lstCustomers.Add(c)

            Dim oecCustomers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            oecCustomers.Load(lstCustomers)
            Util.VerifyObject(Me.ctx, c, EntityStates.Added)
            Util.VerifyObjectDeleted(Me.ctx, o)

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub AddingOrderToCustomer()
            Dim web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf
            web1.DataServiceType = ServiceModelData.CustomData.ServiceModelType
            web1.StartService()

            Using AstoriaUnitTests.Stubs.CustomDataContext.CreateChangeScope()
                Try
                    Dim customDataContext As AstoriaClientUnitTests.Stubs.CustomDataContext = New AstoriaClientUnitTests.Stubs.CustomDataContext(web1.ServiceRoot)
                    'customDataContext.EnableAtom = True
                    'customDataContext.Format.UseAtom()
                    customDataContext.ResolveName = AddressOf ResolveName
                    customDataContext.ResolveType = AddressOf ResolveType

                    Dim col1 = New DataServiceCollection(Of AstoriaClientUnitTests.Stubs.Customer)(customDataContext)
                    col1.Load(customDataContext.Execute(Of AstoriaClientUnitTests.Stubs.Customer)(New Uri("Customers", UriKind.Relative)))

                    Assert.IsTrue(col1.Count > 0, "There are more than one customers in the count")

                    Dim o1 = New AstoriaClientUnitTests.Stubs.Order()
                    o1.DollarAmount = 1000
                    o1.ID = 1001
                    Dim c As AstoriaClientUnitTests.Stubs.Customer = col1.First()
                    c.Orders.Add(o1)

                    ' make sure that the order is in Added state and link appears in the added state
                    Util.VerifyObject(customDataContext, o1, EntityStates.Added)
                    Util.VerifyLink(customDataContext, c, "Orders", o1, EntityStates.Added)

                    customDataContext.SaveChanges()

                    ' make sure that the links 
                    Util.VerifyObject(customDataContext, o1, EntityStates.Unchanged)
                    Util.VerifyLink(customDataContext, c, "Orders", o1, EntityStates.Unchanged)

                Finally
                    If Not web1 Is Nothing Then
                        web1.Dispose()
                    End If
                End Try
            End Using
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub ComplexTypePropertySetters()
            Dim web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf
            web1.DataServiceType = ServiceModelData.CustomData.ServiceModelType
            web1.StartService()

            Using AstoriaUnitTests.Stubs.CustomDataContext.CreateChangeScope()
                Try
                    Dim customDataContext As AstoriaClientUnitTests.Stubs.CustomDataContext = New AstoriaClientUnitTests.Stubs.CustomDataContext(web1.ServiceRoot)
                    'customDataContext.EnableAtom = True
                    'customDataContext.Format.UseAtom()
                    customDataContext.ResolveName = AddressOf ResolveName
                    customDataContext.ResolveType = AddressOf ResolveType

                    Dim col1 = New DataServiceCollection(Of AstoriaClientUnitTests.Stubs.Order)(customDataContext,
                        customDataContext.Execute(Of AstoriaClientUnitTests.Stubs.Order)(New Uri("Orders", UriKind.Relative)),
                        TrackingMode.AutoChangeTracking, "Orders", AddressOf Me.CallbackCounterEntityChanged, Nothing)

                    Assert.IsTrue(col1.Count > 0, "There are more than one customers in the count")

                    Dim o1 = col1.First()
                    Me.ResetCounters()
                    o1.CurrencyAmount = New AstoriaClientUnitTests.Stubs.CurrencyAmount()
                    o1.CurrencyAmount.Amount = 1000
                    o1.CurrencyAmount.CurrencyName = "Dollar"

                    ' make sure that the order is in modified state and link appears in the added state
                    Util.VerifyObject(customDataContext, o1, EntityStates.Modified)

                    customDataContext.SaveChanges()

                    Util.VerifyObject(customDataContext, o1, EntityStates.Unchanged)
                    Assert.AreEqual(3, Me.entityChangedCount)
                Finally
                    Me.ResetCounters()
                    If Not web1 Is Nothing Then
                        web1.Dispose()
                    End If
                End Try
            End Using
        End Sub


        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub ClearWithDetachCollection()
            Dim web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf
            web1.DataServiceType = ServiceModelData.CustomData.ServiceModelType
            web1.StartService()

            Using AstoriaUnitTests.Stubs.CustomDataContext.CreateChangeScope()
                Try
                    Dim customDataContext As AstoriaClientUnitTests.Stubs.CustomDataContext = New AstoriaClientUnitTests.Stubs.CustomDataContext(web1.ServiceRoot)
                    'customDataContext.EnableAtom = True
                    'customDataContext.Format.UseAtom()
                    customDataContext.ResolveName = AddressOf ResolveName
                    customDataContext.ResolveType = AddressOf ResolveType

                    Dim col1 = New DataServiceCollection(Of AstoriaClientUnitTests.Stubs.Order)(customDataContext,
                        customDataContext.Execute(Of AstoriaClientUnitTests.Stubs.Order)(New Uri("Orders", UriKind.Relative)),
                        TrackingMode.AutoChangeTracking, "Orders", AddressOf Me.CallbackCounterEntityChanged, Nothing)
                    Dim initialCount = customDataContext.Entities.Count

                    Dim o1 = New AstoriaClientUnitTests.Stubs.Order()
                    o1.DollarAmount = 1000
                    o1.ID = 1001
                    col1.Add(o1)
                    col1.Clear()
                    customDataContext.SaveChanges()
                    Util.VerifyObject(customDataContext, o1, EntityStates.Unchanged)

                    col1 = New DataServiceCollection(Of AstoriaClientUnitTests.Stubs.Order)(customDataContext,
                        customDataContext.Execute(Of AstoriaClientUnitTests.Stubs.Order)(New Uri("Orders", UriKind.Relative)),
                        TrackingMode.AutoChangeTracking, "Orders", AddressOf Me.CallbackCounterEntityChanged, Nothing)
                    Assert.AreEqual(customDataContext.Entities.Count, initialCount + 1)
                    initialCount = customDataContext.Entities.Count

                    Dim o2 = New AstoriaClientUnitTests.Stubs.Order()
                    o2.DollarAmount = 2000
                    o2.ID = 1002
                    col1.Add(o2)
                    col1.Clear(False)
                    customDataContext.SaveChanges()
                    Util.VerifyObject(customDataContext, o2, EntityStates.Unchanged)

                    col1 = New DataServiceCollection(Of AstoriaClientUnitTests.Stubs.Order)(customDataContext,
                        customDataContext.Execute(Of AstoriaClientUnitTests.Stubs.Order)(New Uri("Orders", UriKind.Relative)),
                        TrackingMode.AutoChangeTracking, "Orders", AddressOf Me.CallbackCounterEntityChanged, Nothing)
                    Assert.AreEqual(customDataContext.Entities.Count, initialCount + 1)
                    initialCount = customDataContext.Entities.Count

                    Dim o3 = New AstoriaClientUnitTests.Stubs.Order()
                    o3.DollarAmount = 3000
                    o3.ID = 1003
                    col1.Add(o3)
                    col1.Clear(True)
                    customDataContext.SaveChanges()
                    Util.VerifyObjectNotPresent(customDataContext, o3)
                    col1 = New DataServiceCollection(Of AstoriaClientUnitTests.Stubs.Order)(customDataContext,
                        customDataContext.Execute(Of AstoriaClientUnitTests.Stubs.Order)(New Uri("Orders", UriKind.Relative)),
                        TrackingMode.AutoChangeTracking, "Orders", AddressOf Me.CallbackCounterEntityChanged, Nothing)
                    Assert.AreEqual(customDataContext.Entities.Count, initialCount)
                Finally
                    If Not web1 Is Nothing Then
                        web1.Dispose()
                    End If
                End Try
            End Using
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub InsertFollowedByDelete()
            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)

            Dim c = CreateCustomer()
            Dim o = CreateOrder()
            Dim e = CreateEmployee()

            oec.Add(c)
            Util.VerifyObject(Me.ctx, c, EntityStates.Added)

            c.Orders.Add(o)
            Util.VerifyObject(Me.ctx, o, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c, "Orders", o, EntityStates.Added)

            o.Customers = c
            Util.VerifyLink(Me.ctx, o, "Customers", c, EntityStates.Modified)

            o.Employees = e
            Util.VerifyObject(Me.ctx, e, EntityStates.Added)
            Util.VerifyLink(Me.ctx, o, "Employees", e, EntityStates.Modified)

            ctx.SaveChanges()

            Util.VerifyObject(Me.ctx, c, EntityStates.Unchanged)
            Util.VerifyObject(Me.ctx, o, EntityStates.Unchanged)
            Util.VerifyObject(Me.ctx, e, EntityStates.Unchanged)

            Util.VerifyLink(Me.ctx, c, "Orders", o, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, o, "Customers", c, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, o, "Employees", e, EntityStates.Unchanged)

            'Get rid of Order
            o.Customers = Nothing
            Util.VerifyLink(Me.ctx, o, "Customers", Nothing, EntityStates.Modified)
            o.Employees = Nothing
            Util.VerifyLink(Me.ctx, o, "Employees", Nothing, EntityStates.Modified)
            c.Orders.Remove(o)
            Util.VerifyObject(Me.ctx, o, EntityStates.Deleted)

            Me.ctx.SaveChanges()

            ' clean the context
            ctx.DeleteObject(e)
            ctx.DeleteObject(c)
            ctx.SaveChanges()

            Util.VerifyObjectNotPresent(Me.ctx, o)
            Util.VerifyNoLink(Me.ctx, o)

        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub DeepDeleteBasic()
            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx,
                "Customers", Nothing, AddressOf Me.OnCollectionChangedDeepDelete)

            Dim c = CreateCustomer()

            Dim o1 = CreateOrder()
            o1.OrderID = 9000

            Dim o2 = CreateOrder()
            o2.OrderID = 9001

            c.Orders.Add(o1)
            c.Orders.Add(o2)

            oec.Add(c)
            Util.VerifyObject(Me.ctx, c, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o2, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c, "Orders", o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c, "Orders", o2, EntityStates.Added)

            oec.Remove(c)

            Util.VerifyObjectNotPresent(Me.ctx, c)
            Util.VerifyObjectNotPresent(Me.ctx, o1)
            Util.VerifyObjectNotPresent(Me.ctx, o2)

            ctx.SaveChanges()

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CycleInObjectGraph()
            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx, "Customers", Nothing, Nothing)
            Dim mOrig = ctx.MergeOption

            ctx.MergeOption = MergeOption.OverwriteChanges
            Dim c1 = CreateCustomer()
            Dim o1 = CreateOrder()
            oec.Add(c1)
            c1.Orders.Add(o1)
            o1.Customers = c1
            Me.ctx.SaveChanges()
            oec.Remove(c1)
            Util.VerifyObjectDeleted(Me.ctx, c1)

            ' Perform test cleanup here
            Dim newContext = New NorthwindBindingModel.NorthwindContext(web.ServiceRoot)
            newContext.AttachTo("Customers", c1)
            newContext.AttachTo("Orders", o1)
            newContext.DeleteObject(o1)
            newContext.DeleteObject(c1)
            newContext.SaveChanges()
        End Sub

        Private Function OnCollectionChangedDeepDelete(ByVal args As EntityCollectionChangedParams) As Boolean
            If args.Action = System.Collections.Specialized.NotifyCollectionChangedAction.Remove Then
                args.Context.DeleteObject(args.TargetEntity)
                Return True
            End If
            Return False
        End Function
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub SetCollectionPropertyBasic()
            Dim oecCustomers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            oecCustomers.Load(Me.ctx.Customers)
            Dim c1 = CreateCustomer()
            oecCustomers.Load(c1)

            Dim oecOrders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Nothing, TrackingMode.None)
            Dim o1 = CreateOrder()
            o1.OrderID = 9000
            oecOrders.Add(o1)
            Dim o2 = CreateOrder()
            o2.OrderID = 9001
            oecOrders.Add(o2)

            c1.Orders = oecOrders
            Util.VerifyObject(Me.ctx, c1, EntityStates.Unchanged)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Unchanged)
            Util.VerifyObject(Me.ctx, o2, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, c1, "Orders", o2, EntityStates.Unchanged)

            c1.Orders.Remove(o1)
            c1.Orders.Remove(o2)
            oecCustomers.Remove(c1)
            Util.VerifyObjectDeleted(Me.ctx, o1)
            Util.VerifyObjectDeleted(Me.ctx, o2)
            Util.VerifyObjectDeleted(Me.ctx, c1)
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub DoubleAddToSameCollection()
            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)

            Dim c1 = CreateCustomer()
            oec.Add(c1)
            Dim o1 = CreateOrder()
            o1.OrderID = 9000
            c1.Orders.Add(o1)

            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)

            Dim success As Boolean = False

            Try
                c1.Orders.Add(o1)
            Catch ex As Exception
                Assert.IsTrue(TypeOf ex Is InvalidOperationException)
                success = True
            End Try

            Assert.IsTrue(success)
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)

            ' Load must not throw on duplicates
            Try
                c1.Orders.Load(o1)
            Catch ex As Exception
                success = False
            End Try

            Assert.IsTrue(success)
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub SetCollectionPropertyWithLoad()
            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            oec.Load(Me.ctx.Customers.Take(1))
            Dim c As NorthwindBindingModel.Customers = oec.First()

            Dim oecOrders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            oecOrders.Load(DirectCast(Me.ctx.LoadProperty(c, "Orders"), IEnumerable(Of NorthwindBindingModel.Orders)))

            Dim success As Boolean = False

            ' DEVNOTE(wbasheer): We disallow assignment of already being observed collections to collection
            ' properties. This is so that we do not ever have more than one incoming edges to a collection
            ' in the graph. 
            Try
                c.Orders = oecOrders
            Catch ex As Exception
                Assert.IsTrue(TypeOf ex Is InvalidOperationException)
                success = True
            End Try

            Assert.IsTrue(success)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub SetCollectionPropertyNull()
            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            oec.Load(Me.ctx.Customers.Take(1))
            Dim c As NorthwindBindingModel.Customers = oec.First()
            Me.ctx.LoadProperty(c, "Orders")
            c.Orders = Nothing

            Dim oecOrders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx)
            oecOrders.Load(DirectCast(Me.ctx.LoadProperty(c, "Orders"), IEnumerable(Of NorthwindBindingModel.Orders)))
            oecOrders.Detach()

            c.Orders = oecOrders
            For Each o As NorthwindBindingModel.Orders In oecOrders
                Util.VerifyObject(Me.ctx, o, EntityStates.Unchanged)
                Util.VerifyLink(Me.ctx, c, "Orders", o, EntityStates.Unchanged)
            Next
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub NonNullForeignKeyConstraint()
            Dim oecRegions = New DataServiceCollection(Of NorthwindBindingModel.Region)(Me.ctx)
            Dim oecTerritories = New DataServiceCollection(Of NorthwindBindingModel.Territories)(Me.ctx)
            Dim r1 = New NorthwindBindingModel.Region()
            r1.RegionID = 5000
            r1.RegionDescription = "Northwest"
            Dim t1 = New NorthwindBindingModel.Territories()
            t1.TerritoryID = "Foo"
            t1.TerritoryDescription = "Seattle"
            ' DEVNOTE(wbasheer): We are still hitting the non-null foreign key constraint violation because 
            'of sequencing dependencies. In the scenario below, we are separately inserting r1 and t1, so 
            'there is no AddRelatedObject call and because there is AddLink in the 3rd line and not SetLink 
            ' no link-folding occurs thus causing non-null foreign key constraint violation.
            oecRegions.Add(r1)
            oecTerritories.Add(t1)
            r1.Territories.Add(t1)

            Dim success As Boolean = False

            Try
                Me.ctx.SaveChanges()
            Catch ex As Exception
                Assert.IsTrue(TypeOf ex Is DataServiceRequestException)
                success = True
            End Try

            Assert.IsTrue(success)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub MinimizeCallbacks()
            Dim oecCustomers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            oecCustomers.Load(Me.ctx.Customers.Expand("Orders").Take(1))
            Dim c = oecCustomers.First()
            Dim oecEmployees = New DataServiceCollection(Of NorthwindBindingModel.Employees)(Me.ctx,
                "Employees", AddressOf Me.CallbackCounterEntityChanged, AddressOf Me.CallbackCounterCollectionChanged)
            Dim e1 As NorthwindBindingModel.Employees = CreateEmployee()
            Dim o1 = c.Orders.First()
            o1.Employees = e1
            e1.Orders.Add(o1)
            Me.ResetCounters()
            oecEmployees.Add(e1)
            Assert.AreEqual(0, Me.entityChangedCount)
            Assert.AreEqual(0, Me.entityCollectionChangedCount)
            Util.VerifyObject(Me.ctx, e1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, e1, "Orders", o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, o1, "Employees", e1, EntityStates.Modified)
            Me.ResetCounters()
        End Sub

        Private Sub ResetCounters()
            Me.entityChangedCount = 0
            Me.entityCollectionChangedCount = 0
        End Sub

        Private Function CallbackCounterEntityChanged(ByVal args As EntityChangedParams) As Boolean
            Me.entityChangedCount = Me.entityChangedCount + 1
            Return False
        End Function

        Private Function CallbackCounterCollectionChanged(ByVal args As EntityCollectionChangedParams) As Boolean
            Me.entityCollectionChangedCount = Me.entityCollectionChangedCount + 1
            Return False
        End Function

        <TestCategory("Partition2")> <TestMethod()> Public Sub TrackEntitiesTillNoMoreReferences()
            Dim oecCustomers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx,
                "Customers", AddressOf CallbackCounterEntityChanged, AddressOf CallbackCounterCollectionChanged)
            Dim c1 = CreateCustomer()
            Dim c2 = CreateCustomer()
            Dim o1 = CreateOrder()
            c1.Orders.Add(o1)
            c2.Orders.Add(o1)
            Me.ResetCounters()
            oecCustomers.Add(c1)
            oecCustomers.Add(c2)
            Assert.AreEqual(Me.entityCollectionChangedCount, 4)
            Assert.AreEqual(Me.entityChangedCount, 0)
            ' DEVNOTE(wbasheer): Note that even on the first call, we have deleted the object based on default behaviour of
            ' DeleteObject on remove. This means that although c2.Orders is referring to the o1 entity, context is not tracking
            ' it anymore, so user will be in for a surprise on SaveChanges coz o1 will not be added to c2's orders
            c1.Orders.Remove(o1)
            Assert.AreEqual(Me.entityCollectionChangedCount, 5)
            Assert.AreEqual(Me.entityChangedCount, 0)
            c2.Orders.Remove(o1)
            ' We do not call the callback here because the object is not being tracked anymore
            Assert.AreEqual(Me.entityCollectionChangedCount, 5)
            Assert.AreEqual(Me.entityChangedCount, 0)
            Me.ResetCounters()
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub ClearDataServiceCollection()
            Dim oecCustomers = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx,
                "Customers", AddressOf CallbackCounterEntityChanged, AddressOf CallbackCounterCollectionChanged)
            Dim c1 = CreateCustomer()
            Dim c2 = CreateCustomer()
            Dim o1 = CreateOrder()
            c1.Orders.Add(o1)
            oecCustomers.Add(c1)
            oecCustomers.Add(c2)
            Me.ResetCounters()
            c1.Orders.Clear()
            Assert.AreEqual(Me.entityCollectionChangedCount, 0)
            Assert.AreEqual(Me.entityChangedCount, 0)
            Me.ResetCounters()
            oecCustomers.Clear()
            Assert.AreEqual(Me.entityCollectionChangedCount, 0)
            Assert.AreEqual(Me.entityChangedCount, 0)
            Me.ResetCounters()
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, c2, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub EntityRefChangedWithNullAssignment()
            Dim oecOrders = New DataServiceCollection(Of NorthwindBindingModel.Orders)(Me.ctx,
                "Orders", AddressOf Me.EntityRefChangedCallbackWithNullAssignment, Nothing)
            Dim o As NorthwindBindingModel.Orders = CreateOrder()
            Dim c1 As NorthwindBindingModel.Customers = CreateCustomer()
            oecOrders.Add(o)
            o.Customers = c1
            o.Customers = Nothing
        End Sub

        Private Function EntityRefChangedCallbackWithNullAssignment(ByVal args As EntityChangedParams) As Boolean
            If args.PropertyValue Is Nothing Then
                Assert.IsTrue(args.TargetEntitySet Is Nothing)
            End If
            Return False
        End Function
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub DataBindingInheritanceTest()
            Dim web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf
            web1.DataServiceType = ServiceModelData.CustomData.ServiceModelType
            web1.StartService()

            Using AstoriaUnitTests.Stubs.CustomDataContext.CreateChangeScope()
                Try
                    Dim customDataContext As AstoriaClientUnitTests.Stubs.CustomDataContext = New AstoriaClientUnitTests.Stubs.CustomDataContext(web1.ServiceRoot)
                    'customDataContext.EnableAtom = True
                    'customDataContext.Format.UseAtom()
                    customDataContext.ResolveName = AddressOf ResolveName
                    customDataContext.ResolveType = AddressOf ResolveType

                    Dim col1 = New DataServiceCollection(Of AstoriaClientUnitTests.Stubs.Customer)(Me.ctx)
                    col1.Load(customDataContext.Execute(Of AstoriaClientUnitTests.Stubs.Customer)(New Uri("Customers", UriKind.Relative)))
                    Assert.IsTrue(col1.Count > 0, "There are more than one customers in the count")

                Finally
                    If Not web1 Is Nothing Then
                        web1.Dispose()
                    End If
                End Try
            End Using
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub AddRelatedObjectTest()
            ' This test covers the following 3 scenario after AddRelatedObject is called.
            ' Scenario 1: parent is deleted
            ' Scenario 2: child is deleted
            ' Scenario 3: link is deleted
            ' Assumption is SaveChanges is yet called and the above operation is preformed

            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            Dim c1 = CreateCustomer()
            Dim o1 = CreateOrder()

            'Scenario 1
            oec.Add(c1)
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)

            c1.Orders.Add(o1)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)

            oec.Remove(c1)

            ' At this point, everything should have been removed.
            Util.VerifyObjectNotPresent(Me.ctx, c1)
            Util.VerifyObjectNotPresent(Me.ctx, o1)
            Util.VerifyNoLink(Me.ctx, c1)
            Util.VerifyNoLink(Me.ctx, o1)

            ' should have done nothing and hence should just pass.
            Me.ctx.SaveChanges()

            'Scenario 2
            oec.Add(c1)
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)

            Dim success As Boolean = False

            Try
                ' o1 is already present in the collection because of deep add
                c1.Orders.Add(o1)
            Catch ex As Exception
                Assert.IsTrue(TypeOf ex Is InvalidOperationException)
                success = True
            End Try

            Assert.IsTrue(success)
            success = False

            c1.Orders.Remove(o1)

            ' At this point, both orders and the link should have been removed.
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObjectNotPresent(Me.ctx, o1)
            Util.VerifyNoLink(Me.ctx, c1)
            Util.VerifyNoLink(Me.ctx, o1)

            ' this should have saved c1
            ctx.SaveChanges()

            ' clear the context for the next operation
            ctx.DeleteObject(c1)
            ctx.SaveChanges()

            'Scenario 3
            oec.Add(c1)
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)

            Try
                Me.ctx.DeleteLink(c1, "Orders", o1)
            Catch ex As Exception
                Assert.IsTrue(TypeOf ex Is InvalidOperationException)
                success = True
            End Try

            Assert.IsTrue(success)

            Me.ctx.DeleteObject(o1)

            ' At this point, only the link should have got deleted.
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObjectNotPresent(Me.ctx, o1)

            ' this should have saved c1
            ctx.SaveChanges()

            ' clear the context for the next operation
            ctx.DeleteObject(c1)
            ctx.SaveChanges()

        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub DeleteObjectOnCollectionRemoval()
            Dim oec As DataServiceCollection(Of NorthwindBindingModel.Employees) = New DataServiceCollection(Of NorthwindBindingModel.Employees)(Me.ctx)
            Dim e = CreateEmployee()
            e.Employees1.Add(e)
            oec.Add(e)
            Util.VerifyObject(Me.ctx, e, EntityStates.Added)
            Util.VerifyLink(Me.ctx, e, "Employees1", e, EntityStates.Added)
            e.Employees1.Remove(e)

            ' DEVNOTE(wbasheer): Since out behaviour is to perform DeleteObject on collection removal,
            ' we would remove the employee from the context, however we would still be tracking it
            Util.VerifyObjectNotPresent(Me.ctx, e)
            Util.VerifyNoLink(Me.ctx, e)

            oec.Remove(e)
            Util.VerifyObjectNotPresent(Me.ctx, e)
            oec.Add(e)

            Util.VerifyObject(Me.ctx, e, EntityStates.Added)
            Util.VerifyNoLink(Me.ctx, e)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub TestBatchWithSingleChangeset()
            Dim c1 = CreateCustomer()
            Dim o1 = CreateOrder()

            Dim o2 = New NorthwindBindingModel.Orders()
            o2.OrderID = 7777

            Dim p1 = New NorthwindBindingModel.Products()
            p1.ProductID = 50000
            p1.ProductName = "A"

            Dim od = New NorthwindBindingModel.Order_Details()
            od.OrderID = o1.OrderID
            od.ProductID = p1.ProductID
            od.Quantity = 1

            Dim custs = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            Dim ods = New DataServiceCollection(Of NorthwindBindingModel.Order_Details)(Me.ctx)

            custs.Add(c1)
            ods.Add(od)

            od.Products = p1
            od.Orders = o1
            o1.Customers = c1
            c1.Orders.Add(o2)

            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)

            'cleanup after the test
            ctx.DeleteObject(c1)
            ctx.DeleteObject(o1)
            ctx.DeleteObject(o2)
            ctx.DeleteObject(p1)
            ctx.DeleteObject(od)
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub AddToNavigationPropertyTrackedByDataServiceCollection()
            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Region)(Me.ctx, "Region", Nothing, Nothing)
            Dim r1 = New NorthwindBindingModel.Region()
            r1.RegionID = 12345
            r1.RegionDescription = "Redmond"

            Dim t1 = New NorthwindBindingModel.Territories()
            t1.TerritoryID = "Snohomish"
            t1.TerritoryDescription = "County in which i live"

            oec.Add(r1)
            r1.Territories.Add(t1)
            Me.ctx.SaveChanges()

            ' clean the context
            ctx.DeleteObject(t1)
            ctx.DeleteObject(r1)
            ctx.SaveChanges()
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub AddAndRemoveEntityFromDataServiceCollection()
            Dim c1 = CreateCustomer()
            Dim o1 = CreateOrder()

            Dim custs = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            custs.Add(c1)

            c1.Orders.Add(o1)
            c1.Orders.Remove(o1)

            ctx.SaveChanges()

            'Clear the database
            Me.ctx.DeleteObject(c1)
            ctx.SaveChanges()

        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub Variation()
            Dim c1 = CreateCustomer()
            Dim o1 = CreateOrder()

            Dim custs = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            custs.Add(c1)
            c1.Orders.Add(o1)

            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)

            custs.Remove(c1)

            Util.VerifyObjectNotPresent(Me.ctx, c1)
            Util.VerifyObjectNotPresent(Me.ctx, o1)
            ctx.SaveChanges()
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub Variation1()
            Dim c1 = CreateCustomer()
            Dim o1 = CreateOrder()
            Dim e1 = CreateEmployee()

            Dim custs = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            custs.Add(c1)

            c1.Orders.Add(o1)
            o1.Employees = e1

            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)

            'clear the database
            ctx.DeleteObject(e1)
            ctx.DeleteObject(o1)
            ctx.DeleteObject(c1)
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub FireEventOnSaveChanges()
            Dim c1 = CreateCustomer()
            Dim o1 = CreateOrder()

            Dim custs = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx, Nothing, AddressOf Me.OnEntityChanged, AddressOf Me.OnEntityCollectionChanged)
            custs.Add(c1)
            EventFired(entityCollectionChangedEventFired, True, "Entity collection changed event not getting fired: Add to root collection")

            c1.Orders.Add(o1)
            EventFired(entityCollectionChangedEventFired, True, "Entity collection changed event not getting fired: Add to child collection")
            ctx.SaveChanges()

            custs.Remove(c1)
            EventFired(entityCollectionChangedEventFired, True, "Entity collection changed event not getting fired: Remove from root collection")

            c1.Orders.Remove(o1)
            EventFired(entityCollectionChangedEventFired, False, "Entity collection changed event getting fired: Remove from child collections when parent is already removed")

            'Clear the database
            Me.ctx.DeleteObject(o1) ' this shouldn't have got removed since we don't allow any removal on deleted entities.
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub TestAddEntityToDataServiceCollection()
            Dim oec = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            Dim c1 = CreateCustomer()
            Dim c2 = New NorthwindBindingModel.Customers()
            c2.CustomerID = "Foo1"
            c2.CompanyName = "Microsoft"

            Dim o1 = CreateOrder()
            Dim o2 = New NorthwindBindingModel.Orders()
            o2.OrderID = 8889

            oec.Add(c1)
            Util.VerifyObject(Me.ctx, c1, EntityStates.Added)

            oec.Add(c2)
            Util.VerifyObject(Me.ctx, c2, EntityStates.Added)

            c1.Orders.Add(o1)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Added)

            c2.Orders.Add(o1)
            Util.VerifyLink(Me.ctx, c2, "Orders", o1, EntityStates.Added)

            c2.Orders.Add(o2)
            Util.VerifyObject(Me.ctx, o2, EntityStates.Added)
            Util.VerifyLink(Me.ctx, c2, "Orders", o2, EntityStates.Added)

            c1.Orders.Add(o2)
            Util.VerifyLink(Me.ctx, c1, "Orders", o2, EntityStates.Added)

            Me.ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)
            Util.VerifyObject(Me.ctx, c1, EntityStates.Unchanged)
            Util.VerifyObject(Me.ctx, c2, EntityStates.Unchanged)
            Util.VerifyObject(Me.ctx, o1, EntityStates.Unchanged)
            Util.VerifyObject(Me.ctx, o2, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, c1, "Orders", o1, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, c2, "Orders", o1, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, c2, "Orders", o2, EntityStates.Unchanged)
            Util.VerifyLink(Me.ctx, c1, "Orders", o2, EntityStates.Unchanged)

            'clear the context
            ctx.DeleteObject(c1)
            ctx.DeleteObject(c2)
            ctx.DeleteObject(o1)
            ctx.DeleteObject(o2)
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)

        End Sub

        <TestCategory("Partition2")> <TestMethod(), Variation("Deep Add not working when adding a previously removed entity")> Public Sub DeepAddPreviouslyRemovedEntity()
            Dim oec As DataServiceCollection(Of NorthwindBindingModel.Customers) = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)

            Dim c = CreateCustomer()
            Dim o = CreateOrder()

            c.Orders.Add(o)
            o.Customers = c

            For i As Integer = 1 To 5
                oec.Add(c)
                oec.Remove(c)
                Dim o2 = CreateOrder()
                c.Orders.Add(o2)
            Next

            oec.Add(c)

            Assert.AreEqual(7, Me.ctx.Entities.Count)
        End Sub

        <TestCategory("Partition2")> <TestMethod(), Variation("Unable to cast results to MaterializeAtom in DataServiceCollection extension methods.")> Public Sub CastResultsToMaterializeAtom()
            Try
                Me.ctx.IgnoreResourceNotFoundException = True

                ' Non-Batch Sync scenario
                Dim uri = New Uri("Customers('ABC')", UriKind.Relative)
                Dim var1 = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
                var1.Load(ctx.Execute(Of NorthwindBindingModel.Customers)(uri))

                ' Non-Batch ASync scenario
                Dim asyncResult = Me.ctx.BeginExecute(Of NorthwindBindingModel.Customers)(uri, Nothing, Nothing)
                Dim var2 = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
                var2.Load(Me.ctx.EndExecute(Of NorthwindBindingModel.Customers)(asyncResult))

                'Batch Sync Case
                Dim query = Me.ctx.CreateQuery(Of NorthwindBindingModel.Customers)("Customers('ABC')")
                Dim var3 = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
                var3.Load(DirectCast(Me.ctx.ExecuteBatch(query).Single(), IEnumerable(Of NorthwindBindingModel.Customers)))

                'Batch Async Case
                asyncResult = Me.ctx.BeginExecuteBatch(Nothing, Nothing, query)
                Dim var4 = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
                var4.Load(DirectCast(Me.ctx.EndExecuteBatch(asyncResult).Single(), IEnumerable(Of NorthwindBindingModel.Customers)))

            Finally
                Me.ctx.IgnoreResourceNotFoundException = False
            End Try
        End Sub

        <TestCategory("Partition2")> <TestMethod(), Variation("Complex type detection for binding scenarios.")> Public Sub ComplexTypeShouldWorkInBindingScenarios()
            Dim ctx As DataServiceContext = New DataServiceContext(New Uri("http://localhost"))
            'ctx.EnableAtom = True
            Dim t As SomeType = New SomeType
            Dim coll = New DataServiceCollection(Of SomeType)(ctx)
            Dim exceptionHappened As Boolean = False
            Try
                coll.Add(t)
            Catch ex As Exception
                exceptionHappened = True
            End Try
            Assert.IsFalse(exceptionHappened)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod(), Variation("DataServiceCollection ctor ().")>
        Public Sub DataServiceCollectionCtor0()
            ' No param constructor should create empty collection with delayed load and tracking
            Dim createCollection = Function() New DataServiceCollection(Of NorthwindBindingModel.Customers)()
            ' Collection should be empty upon creation
            Assert.AreEqual(0, createCollection().Count, "The collection should have been created empty.")
            ' Collection should not allow adding items before calling Load
            VerifyDSCCantInsertItemsBeforeLoad(createCollection())
            ' Collection should have delayed loading enabled - this also verifies that items can be added after Load
            VerifyDSCDelayedLoadingEnabled(createCollection)
        End Sub

        <TestCategory("Partition2")> <TestMethod(), Variation("DataServiceCollection ctor (context).")> Public Sub DataServiceCollectionCtor1Context()
            ' Context param constructor should create empty collection without delayed loading, with tracking
            Dim createCollection = Function() New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            ' Collection should be empty upon creation
            Assert.AreEqual(0, createCollection().Count, "The collection should have been created empty.")
            ' Tracking should be enabled from the start
            VerifyDSCTrackingEnabled(createCollection())
            ' Load should work even on arbitrary enumerations of items
            createCollection().Load(New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())})
            ' No entity set name specified
            VerifyCollectionDoesntKnowEntitySet(New DataServiceCollection(Of NorthwindSmallCustomerWithoutEntitySetAttribute)())
        End Sub
        'Remove Atom
        ' <Ignore> <TestCategory("Partition2")> <TestMethod(), Variation("DataServiceCollection ctor (items).")>
        Public Sub DataServiceCollectionCtor1Items()
            ' Items param constructor should create empty collection without delayed loading, with tracking
            ' Tracking should be enabled from the start when created from DataServiceQuery
            Dim collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)
            ' Tracking should be enabled from the start when created from QueryOperationResponse
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers.Execute())
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)
            ' Creating from arbitrary enumeration should fail to find the context
            Try
                collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())})
                Assert.Fail("The constructor should have failed as the context could not be determined from the items collection.")
            Catch ex As ArgumentException
            End Try
            ' Creation with null items should be equal to no-param constructor and thus should allow delay loading
            Dim createCollection = Function() New DataServiceCollection(Of NorthwindBindingModel.Customers)(CType(Nothing, IEnumerable(Of NorthwindBindingModel.Customers)))
            Assert.AreEqual(0, createCollection().Count, "The collection should have been created empty.")
            VerifyDSCCantInsertItemsBeforeLoad(createCollection())
            VerifyDSCDelayedLoadingEnabled(createCollection)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod(), Variation("DataServiceCollection ctor (items, trackingmode).")>
        Public Sub DataServiceCollectionCtor2()
            ' Items with tracking mode constructor behavior
            ' With autotracking the behavior should be the same as without the parameter
            Dim collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers, TrackingMode.AutoChangeTracking)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers.Execute(), TrackingMode.AutoChangeTracking)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)
            Try
                collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())}, TrackingMode.AutoChangeTracking)
                Assert.Fail("The constructor should have failed as the context could not be determined from the items collection.")
            Catch ex As ArgumentException
            End Try
            ' Creation with null items should be equal to no-param constructor and thus should allow delay loading
            Dim createCollection = Function() New DataServiceCollection(Of NorthwindBindingModel.Customers)(CType(Nothing, IEnumerable(Of NorthwindBindingModel.Customers)), TrackingMode.AutoChangeTracking)
            Assert.AreEqual(0, createCollection().Count, "The collection should have been created empty.")
            VerifyDSCCantInsertItemsBeforeLoad(createCollection())
            VerifyDSCDelayedLoadingEnabled(createCollection)
            ' With none tracking no tracking should be enabled
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers, TrackingMode.None)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingDisabled(collection)
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers.Execute(), TrackingMode.None)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingDisabled(collection)
            ' Creation from arbitrary enumeration should work with None tracking
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())}, TrackingMode.None)
            Assert.AreEqual(1, collection.Count, "The specified item was not loaded into the collection.")
            ' None tracking and null for items should create an empty no-tracking collection
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Nothing, TrackingMode.None)
            Assert.AreEqual(0, collection.Count, "The collection should have been created empty.")
            VerifyDSCTrackingDisabled(collection)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod(), Variation("DataServiceCollection ctor (context, entityset, callback, callback).")>
        Public Sub DataServiceCollectionCtor4()
            ' Context and entity set names (and callbacks)
            ' null context should enable delay loading
            Dim createCollection = Function() New DataServiceCollection(Of NorthwindBindingModel.Customers)(Nothing, Nothing, Nothing, Nothing)
            VerifyDSCDelayedLoadingEnabled(createCollection)
            ' null entity set name should just work as the entity set should be infered from the entity type
            Dim collectionWithoutEntitySet = New DataServiceCollection(Of NorthwindSmallCustomerWithoutEntitySetAttribute)(Me.ctx, Nothing, Nothing, Nothing)
            VerifyCollectionDoesntKnowEntitySet(collectionWithoutEntitySet)
            ' Verify callbacks
            Dim collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx, Nothing, AddressOf OnEntityChanged, AddressOf OnEntityCollectionChanged)
            VerifyDSCCallbacksEnabled(collection)
            ' Entity set specified should allow tracking of types without the entity set attribute
            Dim collectionWithEntitySet = New DataServiceCollection(Of NorthwindSmallCustomerWithoutEntitySetAttribute)(Me.ctx, "Customers", Nothing, Nothing)
            collectionWithEntitySet.Add(New NorthwindSmallCustomerWithoutEntitySetAttribute())
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod(), Variation("DataServiceCollection ctor (items, trackingmode, entityset, callback, callback).")>
        Public Sub DataServiceCollectionCtor5()
            ' Items, tracking mode and callbacks
            ' null items should have delay loading enabled
            Dim createCollection = Function() New DataServiceCollection(Of NorthwindBindingModel.Customers)(Nothing, TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)
            VerifyDSCDelayedLoadingEnabled(createCollection)
            ' Items should get loaded just fine
            Dim collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers, TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers.Execute(), TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)
            Try
                collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())}, TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)
                Assert.Fail("The constructor should have failed as the context could not be determined from the items collection.")
            Catch ex As ArgumentException
            End Try
            ' With no tracking - should not track
            ' Creation from arbitrary enumeration should work with None tracking
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())}, TrackingMode.None, Nothing, Nothing, Nothing)
            Assert.AreEqual(1, collection.Count, "The specified item was not loaded into the collection.")
            VerifyDSCTrackingDisabled(collection)
            ' No entity set name was specified
            Dim collectionWithoutEntitySet = New DataServiceCollection(Of NorthwindSmallCustomerWithoutEntitySetAttribute)(Nothing, TrackingMode.None, Nothing, Nothing, Nothing)
            VerifyCollectionDoesntKnowEntitySet(collectionWithoutEntitySet)
            ' Callbacks should work
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx.Customers, TrackingMode.AutoChangeTracking, Nothing, AddressOf OnEntityChanged, AddressOf OnEntityCollectionChanged)
            VerifyDSCCallbacksEnabled(collection)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod(), Variation("DataServiceCollection ctor (context, items, trackingmode, entityset, callback, callback).")>
        Public Sub DataServiceCollectionCtor6()
            ' Items, tracking mode and callbacks
            ' null items should have delay loading enabled
            Dim createCollection = Function() New DataServiceCollection(Of NorthwindBindingModel.Customers)(Nothing, Nothing, TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)
            VerifyDSCDelayedLoadingEnabled(createCollection)
            ' Items should get loaded just fine
            Dim collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Nothing, Me.ctx.Customers, TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Nothing, Me.ctx.Customers.Execute(), TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx, New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())}, TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)
            VerifyDSCTrackingEnabled(collection)
            ' With no tracking - should not track
            ' Creation from arbitrary enumeration should work with None tracking
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Nothing, New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())}, TrackingMode.None, Nothing, Nothing, Nothing)
            Assert.AreEqual(1, collection.Count, "The specified item was not loaded into the collection.")
            VerifyDSCTrackingDisabled(collection)
            ' No entity set name was specified
            Dim collectionWithoutEntitySet = New DataServiceCollection(Of NorthwindSmallCustomerWithoutEntitySetAttribute)(Nothing, Nothing, TrackingMode.None, Nothing, Nothing, Nothing)
            VerifyCollectionDoesntKnowEntitySet(collectionWithoutEntitySet)
            ' Callbacks should work
            collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx, Me.ctx.Customers, TrackingMode.AutoChangeTracking, Nothing, AddressOf OnEntityChanged, AddressOf OnEntityCollectionChanged)
            VerifyDSCCallbacksEnabled(collection)
            ' Entity set specified should allow tracking of types without the entity set attribute
            Dim collectionWithEntitySet = New DataServiceCollection(Of NorthwindSmallCustomerWithoutEntitySetAttribute)(Me.ctx, Nothing, TrackingMode.None, "Customers", Nothing, Nothing)
            collectionWithEntitySet.Add(New NorthwindSmallCustomerWithoutEntitySetAttribute())
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub IgnoreUnrecognizedPropertyChanges()
            ' Track changes on a Customer
            Dim collection = New DataServiceCollection(Of NorthwindBindingModel.Customers)(ctx.Customers.Take(1), TrackingMode.AutoChangeTracking)
            Dim customer = collection(0)

            ' Change an internal property that raises the PropertyChanged event
            customer.InternalTestProperty = "abc"

            ' Change a private property that raises the PropertyChanged event
            customer.SetPrivateTestProperty("abc")

            ' Raise the PropertyChanged event for a non-property string
            customer.RaiseGarbagePropertyChanged()
        End Sub

        <Microsoft.OData.Client.EntitySet("SomeSet")> Public Class SomeType
            Implements System.ComponentModel.INotifyPropertyChanged
            Private _id As Integer
            Public Property ID() As Integer
                Get
                    Return Me._id
                End Get
                Set(ByVal value As Integer)
                    Me._id = value
                End Set
            End Property

            Private _arrList As ArrayList
            Public Property List() As ArrayList
                Get
                    Return Me._arrList
                End Get
                Set(ByVal value As ArrayList)
                    Me._arrList = value
                End Set
            End Property

            Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        End Class

        <Microsoft.OData.Client.KeyAttribute("CustomerID")>
        Public Class NorthwindSmallCustomerWithoutEntitySetAttribute
            Implements System.ComponentModel.INotifyPropertyChanged

            Private _id As Integer
            Public Property CustomerID() As Integer
                Get
                    Return Me._id
                End Get
                Set(ByVal value As Integer)
                    Me._id = value
                End Set
            End Property

            Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        End Class

        Private Function ResolveName(ByVal type As Type) As String
            Return type.FullName.Replace("AstoriaClientUnitTests.AstoriaClientUnitTests", "AstoriaUnitTests")
        End Function

        Private Function ResolveType(ByVal typeName As String) As Type
            Return Assembly.GetExecutingAssembly().GetType(typeName.Replace("AstoriaUnitTests", "AstoriaClientUnitTests.AstoriaClientUnitTests"))
        End Function

        Private Function OnEntityChanged(ByVal args As EntityChangedParams) As Boolean
            Me.entityChangedEventFired = True
            Return False
        End Function

        Private Function OnEntityCollectionChanged(ByVal args As EntityCollectionChangedParams) As Boolean
            Me.entityCollectionChangedEventFired = True
            Return False
        End Function

        Private Sub DeleteCustomers()
            Dim customers As DataServiceCollection(Of NorthwindBindingModel.Customers) = New DataServiceCollection(Of NorthwindBindingModel.Customers)(Me.ctx)
            customers.Load(From c In Me.ctx.Customers.Expand("Orders") Where c.CustomerID.StartsWith("CUST") Select c)

            Dim temp = New Collection()

            For Each customer In customers
                For Each order In customer.Orders
                    temp.Add(order)
                Next
                For Each o In temp
                    customer.Orders.Remove(DirectCast(o, NorthwindBindingModel.Orders))
                Next
                temp.Clear()
            Next

            For Each customer In customers
                temp.Add(customer)
            Next
            For Each o In temp
                customers.Remove(DirectCast(o, NorthwindBindingModel.Customers))
            Next
            temp.Clear()

            Util.SaveChanges(ctx)
        End Sub

        Private Function CreateCustomer() As NorthwindBindingModel.Customers
            Dim c1 = New NorthwindBindingModel.Customers()
            c1.CustomerID = "Foo"
            c1.CompanyName = "Microsoft"
            Return c1
        End Function

        Private Function CreateCustomer(ByVal id As String) As NorthwindBindingModel.Customers
            Dim customer = New NorthwindBindingModel.Customers()
            customer.CustomerID = id
            customer.CompanyName = "Microsoft"
            Return customer
        End Function

        Private Function CreateOrder() As NorthwindBindingModel.Orders
            Dim o1 = New NorthwindBindingModel.Orders()
            o1.OrderID = 8888
            Return o1
        End Function

        Private Function CreateEmployee() As NorthwindBindingModel.Employees
            Dim e1 = New NorthwindBindingModel.Employees()
            e1.EmployeeID = 1111
            e1.FirstName = "Andy"
            e1.LastName = "Conrad"
            Return e1
        End Function

        Private Sub EventFired(ByRef eventFired As Boolean, ByVal expectedValue As Boolean, ByVal errorMessage As String)
            Assert.AreEqual(eventFired, expectedValue, errorMessage)
            eventFired = False
        End Sub

        Private Sub VerifyDSCDelayedLoadingEnabled(ByVal createCollection As Func(Of DataServiceCollection(Of NorthwindBindingModel.Customers)))
            Dim collection As DataServiceCollection(Of NorthwindBindingModel.Customers)

            ' Try loading DataServiceQuery
            collection = createCollection()
            collection.Load(Me.ctx.Customers)
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)

            ' Try loading QueryOperationResponse
            collection = createCollection()
            collection.Load(Me.ctx.Customers.Execute())
            VerifyDSCContent(collection, Me.ctx.Customers)
            VerifyDSCTrackingEnabled(collection)

            ' Try loading from IEnumerable - should fail as no context can be infered
            Try
                collection = createCollection()
                collection.Load(New NorthwindBindingModel.Customers() {CreateCustomer(Me.ctx.Entities.Count.ToString())})
                Assert.Fail("Load call should have failed as it could not determine the context.")
            Catch ex As ArgumentException
            End Try
        End Sub

        Private Sub VerifyDSCTrackingEnabled(ByVal collection As DataServiceCollection(Of NorthwindBindingModel.Customers))
            Dim customer = CreateCustomer(Me.ctx.Entities.Count.ToString())
            collection.Add(customer)
            Assert.AreEqual(EntityStates.Added, Me.ctx.GetEntityDescriptor(customer).State, "DSC didn't add new customer even though it should have been tracking.")
        End Sub

        Private Sub VerifyDSCTrackingDisabled(ByVal collection As DataServiceCollection(Of NorthwindBindingModel.Customers))
            Dim customer = CreateCustomer(Me.ctx.Entities.Count.ToString())
            collection.Add(customer)
            Assert.AreEqual(Nothing, Me.ctx.GetEntityDescriptor(customer), "DSC added new customer even though it should not have been tracking.")
        End Sub

        Private Sub VerifyDSCCantInsertItemsBeforeLoad(ByVal collection As DataServiceCollection(Of NorthwindBindingModel.Customers))
            Dim customer = CreateCustomer(Me.ctx.Entities.Count.ToString())
            Try
                collection.Add(customer)
                Assert.Fail("Add should have failed.")
            Catch ex As InvalidOperationException
            End Try
        End Sub

        Private Sub VerifyDSCContent(ByVal collection As DataServiceCollection(Of NorthwindBindingModel.Customers), ByVal items As IEnumerable(Of NorthwindBindingModel.Customers))
            For Each item In items
                Assert.IsTrue(collection.Contains(item), "Collection doesn't contain the specified items")
            Next
        End Sub

        Private Sub VerifyCollectionDoesntKnowEntitySet(ByVal collection As DataServiceCollection(Of NorthwindSmallCustomerWithoutEntitySetAttribute))
            Try
                collection.Add(New NorthwindSmallCustomerWithoutEntitySetAttribute())
            Catch ex As InvalidOperationException
            End Try
        End Sub

        Private Sub VerifyDSCCallbacksEnabled(ByVal collection As DataServiceCollection(Of NorthwindBindingModel.Customers))
            Me.entityChangedEventFired = False
            Me.entityCollectionChangedEventFired = False
            collection.Add(CreateCustomer(Me.ctx.Entities.Count.ToString()))
            Assert.IsFalse(Me.entityChangedEventFired)
            Assert.IsTrue(Me.entityCollectionChangedEventFired)

            Me.entityCollectionChangedEventFired = False
            collection(0).ContactName = collection(0).ContactName + "Some"
            Assert.IsTrue(Me.entityChangedEventFired)
            Assert.IsFalse(Me.entityCollectionChangedEventFired)
        End Sub
    End Class
End Class

