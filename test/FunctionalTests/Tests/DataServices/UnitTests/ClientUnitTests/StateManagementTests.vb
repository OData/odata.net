'---------------------------------------------------------------------
' <copyright file="StateManagementTests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Diagnostics
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AstoriaUnitTests.Stubs
Imports AstoriaUnitTests.Data

Partial Public Class ClientModule

    <TestClass()> Public Class StateManagementTests
        Inherits Util

        Private ctx As DataServiceContext

        <TestInitialize()> Public Sub BeforeEachTestMethod()
            ctx = New DataServiceContext(New Uri("http://localhost/svc"))
        End Sub

        <TestCleanup()> Public Sub AfterEachTestMethod()
            ctx = Nothing
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub DefaultMergeOption()
            Assert.AreEqual(MergeOption.AppendOnly, ctx.MergeOption)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub TestInitialListAll()
            Assert.AreEqual(0, ctx.Entities.Count)

            Assert.AreEqual(0, ctx.Links.Count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub TestFailureAdd()
            Dim customer As New NorthwindSimpleModel.Customers()
            Try
                ctx.AddObject(Nothing, customer)
                Assert.Fail("ArgumentNullException not thrown")
            Catch ex As ArgumentNullException
                Assert.AreEqual("entitySetName", ex.ParamName)
            End Try

            Try
                ctx.AddObject(String.Empty, customer)
                Assert.Fail("ArgumentException not thrown")
            Catch ex As ArgumentException
                Assert.AreEqual("entitySetName", ex.ParamName)
            End Try

            Try
                ctx.AddObject("nothing", Nothing)
                Assert.Fail("ArgumentException not thrown")
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ctx.AddObject("abc", customer)
            Assert.AreSame(customer, (From o In ctx.Entities Where EntityStates.Added = o.State And o.Entity Is customer).Single.Entity)

            Try
                ctx.AddObject("abc", customer)
                Assert.Fail("InvalidOperationException not thrown")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AddObject("cba", customer)
                Assert.Fail("InvalidOperationException not thrown")
            Catch ex As InvalidOperationException
            End Try

            ctx.UpdateObject(customer)
            Assert.AreSame(customer, (From o In ctx.Entities Where o.Entity Is customer And o.State = EntityStates.Added).Single.Entity, "should stay as an Insert")

            Try
                ctx.AddObject("aaaa", New AddNoKeyError())
                Assert.Fail("ArgumentException not thrown")
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

        End Sub

        Private Class AddNoKeyError
            Public Property HasKey() As Int32
                Get
                    Return 0
                End Get
                Set(ByVal value As Int32)

                End Set
            End Property
        End Class

        <TestCategory("Partition1")> <TestMethod()> Public Sub TestFailureAttach()
            Dim customer As New NorthwindSimpleModel.Customers()

            Try
                ctx.AttachTo(Nothing, customer)
                Assert.Fail("ArgumentNullException not thrown")
            Catch ex As ArgumentNullException
                Assert.AreEqual("entitySetName", ex.ParamName)
            End Try

            Try
                ctx.AttachTo(String.Empty, customer)
                Assert.Fail("ArgumentException not thrown")
            Catch ex As ArgumentException
                Assert.AreEqual("entitySetName", ex.ParamName)
            End Try

            Try
                ctx.AttachTo("nothing", Nothing)
                Assert.Fail("ArgumentException not thrown")
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            Try
                ctx.AttachTo("abc", customer)
                Assert.Fail("InvalidOperationException not thrown")
            Catch ex As InvalidOperationException

            End Try

            customer.CustomerID = "ASDF"
            ctx.AttachTo("abc", customer)

            Try
                ctx.AddObject("abc", customer)
                Assert.Fail("InvalidOperationException not thrown")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("abc", customer)
                Assert.Fail("InvalidOperationException not thrown")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("cba", customer)
                Assert.Fail("InvalidOperationException not thrown")
            Catch ex As InvalidOperationException
            End Try

            ctx.UpdateObject(customer)
            Assert.AreSame(customer, (From o In ctx.Entities Where o.State = EntityStates.Modified).Single.Entity, "should become an update")

            ctx.UpdateObject(customer)
            Assert.AreSame(customer, (From o In ctx.Entities Where o.State = EntityStates.Modified).Single.Entity, "should still be an update")

            Dim customer2 As New NorthwindSimpleModel.Customers
            customer2.CustomerID = "ASDF"
            Try
                ctx.AttachTo("abc", customer2)
                Assert.Fail("InvalidOperationException not thrown")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("aaaa", New AddNoKeyError())
                Assert.Fail("ArgumentException not thrown")
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub TestInitialUpdateState()
            Try
                ctx.UpdateObject(New NorthwindSimpleModel.Customers())
                Assert.Fail("ArgumentException not thrown")
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub TestInitialDeleteState()
            Try
                ctx.DeleteObject(New NorthwindSimpleModel.Customers())
                Assert.Fail("InvalidOperationException not thrown")
            Catch ex As InvalidOperationException
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub TestInitialDetachState()
            Assert.IsFalse(ctx.Detach(New NorthwindSimpleModel.Customers()))
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub AddObjects()
            Dim list As List(Of MyItem) = CreateObjects()

            For i As Int32 = 0 To list.Count - 1
                list.Item(i).AddObject(ctx)
            Next

            Dim uri As Uri = Nothing
            For i As Int32 = 0 To list.Count - 1
                Assert.IsFalse(ctx.TryGetUri(list.Item(i).entity, uri))
            Next

            For i As Int32 = 0 To list.Count - 1
                list.Item(i).DeleteObject(ctx)
            Next

            For i As Int32 = 0 To list.Count - 1
                Assert.IsFalse(list.Item(i).Detach(ctx))
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub AttachObjects()
            Dim list As List(Of MyItem) = CreateObjects()

            For i As Int32 = 0 To list.Count - 1
                list.Item(i).AttachTo(ctx)
            Next

            Dim entity As Object = Nothing
            Assert.IsFalse(ctx.TryGetEntity(New Uri("product(1)", UriKind.Relative), entity))

            Dim uri As Uri = Nothing
            For i As Int32 = 0 To list.Count - 1
                Assert.IsTrue(ctx.TryGetUri(list.Item(i).entity, uri))
                Assert.IsTrue(ctx.TryGetEntity(uri, entity))
                Assert.AreSame(entity, list.Item(i).entity)

                Assert.IsFalse(ctx.TryGetEntity(New Uri(uri, "/apple"), entity))
                Assert.IsNull(entity)
            Next

            For i As Int32 = 0 To list.Count - 1
                list.Item(i).DeleteObject(ctx)
            Next

            For i As Int32 = 0 To list.Count - 1
                Assert.IsTrue(list.Item(i).Detach(ctx))
            Next

            For i As Int32 = 0 To list.Count - 1
                Assert.IsFalse(list.Item(i).Detach(ctx))
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub AddAttachDeleteDetachObjects()
            Dim outside As List(Of MyItem) = CreateObjects()
            Dim inside As New List(Of MyItem)

            Dim random As New Random()

            Dim item As MyItem
            Dim iterations As Int32 = outside.Count * 5
            Dim change As Boolean = True

            For i As Int32 = 0 To iterations
                Dim outIndex As Int32 = random.Next(outside.Count)
                Dim inIndex As Int32 = random.Next(inside.Count)

                Dim entity As Object = Nothing
                Dim uri As Uri = Nothing

                Select Case random.Next(26)

                    Case 0 To 3 'AddObject
                        If 0 < outside.Count Then
                            item = outside.Item(outIndex)
                            outside.RemoveAt(outIndex)
                            item.AddObject(ctx)
                            inside.Add(item)
                            item.state = EntityStates.Added
                            Assert.IsFalse(ctx.TryGetUri(item.entity, uri))
                            change = True
                        End If
                    Case 4
                        If 0 < inside.Count Then
                            item = inside.Item(inIndex)
                            Try
                                item.AddObject(ctx)
                                Assert.Fail("InvalidOperationException not thrown")
                            Catch ex As InvalidOperationException
                            End Try
                        End If

                    Case 5 To 8 'AttachTo
                        If 0 < outside.Count Then
                            item = outside.Item(outIndex)
                            outside.RemoveAt(outIndex)
                            item.AttachTo(ctx)
                            inside.Add(item)
                            item.state = EntityStates.Unchanged
                            Assert.IsTrue(ctx.TryGetUri(item.entity, uri))
                            Assert.IsTrue(ctx.TryGetEntity(uri, entity))
                            Assert.AreSame(entity, item.entity)
                            change = True
                        End If
                    Case 9
                        If 0 < inside.Count Then
                            item = inside.Item(inIndex)
                            Try
                                item.AttachTo(ctx)
                                Assert.Fail("InvalidOperationException not thrown")
                            Catch ex As InvalidOperationException
                            End Try
                        End If

                    Case 10 To 13 'UpdateObject
                        If 0 < inside.Count Then
                            item = inside.Item(inIndex)
                            If (EntityStates.Unchanged = item.state Or EntityStates.Modified = item.state) Then
                                Assert.IsTrue(ctx.TryGetUri(item.entity, uri))
                                item.UpdateObject(ctx)
                                item.state = EntityStates.Modified
                                change = True
                            Else
                                item.UpdateObject(ctx)
                                ' doesn't change state
                            End If
                        End If
                    Case 14
                        If 0 < outside.Count Then
                            item = outside.Item(outIndex)
                            Try
                                item.UpdateObject(ctx)
                                Assert.Fail("ArgumentException not thrown")
                            Catch ex As ArgumentException
                                Assert.AreEqual("entity", ex.ParamName)
                            End Try
                        End If

                    Case 15 To 18 'DeleteObject
                        If 0 < inside.Count Then
                            item = inside.Item(inIndex)
                            item.DeleteObject(ctx)
                            If EntityStates.Added = item.state Then
                                inside.RemoveAt(inIndex)
                                outside.Add(item)
                                Assert.IsFalse(ctx.TryGetUri(item.entity, uri))
                            Else
                                item.state = EntityStates.Deleted
                                Assert.IsTrue(ctx.TryGetUri(item.entity, uri))
                            End If
                            change = True
                        End If
                    Case 19
                        If 0 < outside.Count Then
                            item = outside.Item(outIndex)
                            Try
                                item.DeleteObject(ctx)
                                Assert.Fail("InvalidOperationException not thrown")
                            Catch ex As InvalidOperationException
                            End Try
                        End If

                    Case 20 To 23 'Detach
                        If 0 < inside.Count Then
                            item = inside.Item(inIndex)
                            inside.RemoveAt(inIndex)
                            Assert.IsTrue(item.Detach(ctx))
                            outside.Add(item)
                            item.state = EntityStates.Detached
                            Assert.IsFalse(ctx.TryGetUri(item.entity, uri))
                            change = True
                        End If
                    Case 24
                        If 0 < outside.Count Then
                            item = outside.Item(outIndex)
                            Assert.IsFalse(item.Detach(ctx))
                        End If

                    Case 25
                        If change Then ' avoid repeat checks
                            For Each fruit As EntityDescriptor In ctx.Entities
                                Assert.IsTrue(MyItem.Contains(inside, fruit.Entity, fruit.State))
                            Next

                            change = False
                        End If
                        i -= 1

                    Case Else
                        Assert.Fail("test out of range")

                End Select


            Next
        End Sub


        Private Function AddNew(Of T As {Class, New})(ByVal list As List(Of MyItem), ByVal entitySet As String) As T
            Dim x As New T
            list.Add(New MyItem(Of T)(entitySet, x))
            Return x
        End Function

        Public Function CreateObjects() As List(Of MyItem)
            Dim list As New List(Of MyItem)

            Dim id As Int32 = 0
            Dim random As New Random(20071024)
            For i As Int32 = 0 To 1000
                id = id + 1
                Select Case random.Next(10)
                    Case 0
                        AddNew(Of NorthwindSimpleModel.Categories)(list, "Categories").CategoryID = id
                    Case 1
                        AddNew(Of NorthwindSimpleModel.Customers)(list, "Customers").CustomerID = id.ToString
                    Case 2
                        AddNew(Of NorthwindSimpleModel.Employees)(list, "Employees").EmployeeID = id
                    Case 3
                        AddNew(Of NorthwindSimpleModel.Order_Details)(list, "Order_Details").OrderID = id
                    Case 4
                        AddNew(Of NorthwindSimpleModel.Orders)(list, "Orders").OrderID = id
                    Case 5
                        AddNew(Of NorthwindSimpleModel.Products)(list, "Products").ProductID = id
                    Case 6
                        AddNew(Of NorthwindSimpleModel.Region)(list, "Region").RegionID = id
                    Case 7
                        AddNew(Of NorthwindSimpleModel.Shippers)(list, "Shippers").ShipperID = id
                    Case 8
                        AddNew(Of NorthwindSimpleModel.Suppliers)(list, "Suppliers").SupplierID = id
                    Case 9
                        AddNew(Of NorthwindSimpleModel.Territories)(list, "Territories").TerritoryID = id.ToString
                    Case Else
                        Assert.Fail("bad case")
                End Select
            Next
            Return list
        End Function



        Public Class MyItem
            Public ReadOnly entity As Object
            Public ReadOnly entitySet As String
            Public state As EntityStates

            Sub New(ByVal entitySet As String, ByVal entity As Object)
                Me.entity = entity
                Me.entitySet = entitySet
            End Sub

            Public Overridable Sub AddObject(ByVal ctx As DataServiceContext)
                ctx.AddObject(Me.entitySet, Me.entity)
            End Sub

            Public Overridable Sub AttachTo(ByVal ctx As DataServiceContext)
                ctx.AttachTo(Me.entitySet, Me.entity)
            End Sub

            Public Overridable Sub UpdateObject(ByVal ctx As DataServiceContext)
                ctx.UpdateObject(Me.entity)
            End Sub

            Public Overridable Sub DeleteObject(ByVal ctx As DataServiceContext)
                ctx.DeleteObject(Me.entity)
            End Sub

            Public Overridable Function Detach(ByVal ctx As DataServiceContext) As Boolean
                Return ctx.Detach(Me.entity)
            End Function

            Public Shared Function Contains(ByVal list As List(Of MyItem), ByVal item As Object, ByVal state As EntityStates) As Boolean
                For Each mine As MyItem In list
                    If Object.ReferenceEquals(item, mine.entity) Then
                        Assert.AreEqual(mine.state, state)
                        Return True
                    End If
                Next
                Return False
            End Function

        End Class

        Public Class MyItem(Of T As {New, Class})
            Inherits MyItem

            Sub New(ByVal entitySet As String)
                MyBase.New(entitySet, New T())
            End Sub

            Sub New(ByVal entitySet As String, ByVal value As T)
                MyBase.New(entitySet, value)
            End Sub

            Public Overrides Sub AddObject(ByVal ctx As DataServiceContext)
                ctx.AddObject(Me.entitySet, CType(Me.entity, T))
            End Sub

            Public Overrides Sub AttachTo(ByVal ctx As DataServiceContext)
                ctx.AttachTo(Me.entitySet, CType(Me.entity, T))
            End Sub
        End Class

        <TestCategory("Partition1")> <TestMethod()> _
        Public Overloads Sub AddLinkFailure()
            Dim customer As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "Microsoft")
            Dim order As NorthwindSimpleModel.Orders = NorthwindSimpleModel.Orders.CreateOrders(35)
            Dim detachedCustomer As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "Microsoft")
            Dim detachedOrder As NorthwindSimpleModel.Orders = NorthwindSimpleModel.Orders.CreateOrders(35)

            Try
                ctx.AddLink(Nothing, "Orders", order)
                Assert.Fail("missing ArgumentNullException")
            Catch ex As ArgumentNullException
                Debug.WriteLine(ex.Message)
                Assert.AreEqual("source", ex.ParamName)
            End Try

            Try
                ctx.AddLink(customer, Nothing, order) ' not tracking customer
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
                Debug.WriteLine(ex.Message)
            End Try

            Try
                ctx.AddLink(customer, "Orders", Nothing) ' not tracking customer
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
                Debug.WriteLine(ex.Message)
            End Try

            ctx.AttachTo("Customers", customer)
            Try
                ctx.AddLink(customer, Nothing, order) ' not tracking orders
                Assert.Fail("missing ArgumentNullException")
            Catch ex As InvalidOperationException
                Debug.WriteLine(ex.Message)
            End Try

            For i As Int32 = 0 To 1
                For Each entity In ctx.Entities
                    Assert.IsTrue(ctx.Detach(entity.Entity))
                Next
                Assert.AreEqual(0, ctx.Links.Count)
                Assert.AreEqual(0, ctx.Entities.Count)

                If (i = 0) Then
                    ctx.AddObject("Customers", customer)
                    ctx.AddObject("Orders", order)
                ElseIf (i = 1) Then
                    ctx.AttachTo("Customers", customer)
                    ctx.AttachTo("Orders", order)
                End If

                Try
                    ctx.AddLink(Nothing, "Orders", order)
                    Assert.Fail("missing ArgumentNullException")
                Catch ex As ArgumentNullException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual("source", ex.ParamName)
                End Try

                Try
                    ctx.AddLink(customer, Nothing, order)
                    Assert.Fail("missing ArgumentNullException")
                Catch ex As ArgumentNullException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual("sourceProperty", ex.ParamName)
                End Try

                Try
                    ctx.AddLink(customer, "Orders", Nothing)
                    Assert.Fail("missing ArgumentNullException")
                Catch ex As ArgumentNullException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual("target", ex.ParamName)
                End Try

                Try
                    ctx.AddLink(detachedCustomer, "Orders", order)
                    Assert.Fail("missing InvalidOperationException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                Try
                    ctx.AddLink(customer, "Suppliers", order)   ' doesn't have Suppliers property
                    Assert.Fail("missing InvalidOperationException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                Try
                    ctx.AddLink(customer, "CustomerDemographics", order)    ' wrong type
                    Assert.Fail("missing ArgumentException")
                Catch ex As ArgumentException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual("target", ex.ParamName)
                End Try

                Try
                    ctx.AttachLink(customer, "CustomerID", order)    ' not a relation property
                    Assert.Fail("missing ArgumentException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                Try
                    ctx.AddLink(customer, "Orders", detachedOrder)
                    Assert.Fail("missing InvalidOperationException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                ctx.AddLink(customer, "Orders", order)
                Try
                    ctx.AddLink(customer, "Orders", order)      ' already exists
                    Assert.Fail("missing InvalidOperationException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                Assert.IsTrue(ctx.DetachLink(customer, "Orders", order))
                Assert.IsFalse(ctx.DetachLink(customer, "Orders", order))

                ctx.AddLink(customer, "Orders", order)
                ctx.DeleteLink(customer, "Orders", order)
                Assert.AreEqual(0, ctx.Links.Count)

                ctx.AddLink(customer, "Orders", order)
                Assert.IsTrue(ctx.Detach(customer))
                Assert.IsFalse(ctx.DetachLink(customer, "Orders", order))

                Assert.IsTrue(ctx.Detach(order))
                Assert.IsFalse(ctx.DetachLink(customer, "Orders", order))
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> _
        Public Overloads Sub AttachLinkFailure()
            Dim customer As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "Microsoft")
            Dim order As NorthwindSimpleModel.Orders = NorthwindSimpleModel.Orders.CreateOrders(35)
            Dim detachedCustomer As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "Microsoft")
            Dim detachedOrder As NorthwindSimpleModel.Orders = NorthwindSimpleModel.Orders.CreateOrders(35)

            Try
                ctx.AttachLink(Nothing, "Orders", order)
                Assert.Fail("missing ArgumentNullException")
            Catch ex As ArgumentNullException
                Assert.AreEqual("source", ex.ParamName)
            End Try

            Try
                ctx.AttachLink(customer, Nothing, order) ' not tracking customer
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
                Debug.WriteLine(ex.Message)
            End Try

            Try
                ctx.AttachLink(customer, "Orders", Nothing) ' not tracking customer
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
                Debug.WriteLine(ex.Message)
            End Try

            ctx.AttachTo("Customers", customer)
            Try
                ctx.AttachLink(customer, Nothing, order) ' not tracking orders
                Assert.Fail("missing ArgumentNullException")
            Catch ex As InvalidOperationException
                Debug.WriteLine(ex.Message)
            End Try

            For i As Int32 = 0 To 1
                For Each entity In ctx.Entities
                    Assert.IsTrue(ctx.Detach(entity.Entity))
                Next
                Assert.AreEqual(0, ctx.Links.Count)
                Assert.AreEqual(0, ctx.Entities.Count)

                If (i = 0) Then
                    ctx.AddObject("Customers", customer)
                    ctx.AddObject("Orders", order)
                ElseIf (i = 1) Then
                    ctx.AttachTo("Customers", customer)
                    ctx.AttachTo("Orders", order)
                End If

                Try
                    ctx.AttachLink(Nothing, "Orders", order)
                    Assert.Fail("missing ArgumentNullException")
                Catch ex As ArgumentNullException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual("source", ex.ParamName)
                End Try

                Try
                    ctx.AttachLink(customer, Nothing, order)
                    Assert.Fail("missing ArgumentNullException")
                Catch ex As ArgumentNullException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual("sourceProperty", ex.ParamName)
                End Try

                Try
                    ctx.AttachLink(customer, "Orders", Nothing)
                    Assert.Fail("missing ArgumentNullException")
                Catch ex As ArgumentNullException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual("target", ex.ParamName)
                End Try

                Try
                    ctx.AttachLink(detachedCustomer, "Orders", order)
                    Assert.Fail("missing InvalidOperationException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                Try
                    ctx.AttachLink(customer, "Suppliers", order)   ' doesn't have Suppliers property
                    Assert.Fail("missing InvalidOperationException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                Try
                    ctx.AttachLink(customer, "CustomerDemographics", order)    ' wrong type
                    Assert.Fail("missing ArgumentException")
                Catch ex As ArgumentException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual("target", ex.ParamName)
                End Try

                Try
                    ctx.AttachLink(customer, "CustomerID", order)    ' not a relation property
                    Assert.Fail("missing ArgumentException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                Try
                    ctx.AttachLink(customer, "Orders", detachedOrder)
                    Assert.Fail("missing InvalidOperationException")
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                End Try

                Dim skipAhead As Boolean = False
                Try
                    ctx.AttachLink(customer, "Orders", order)
                    Assert.AreEqual(0, (From cust In ctx.Entities Where cust.State = EntityStates.Added Select cust).Count)
                Catch ex As InvalidOperationException
                    Debug.WriteLine(ex.Message)
                    Assert.AreEqual(EntityStates.Added, (From cust In ctx.Entities Where cust.State = EntityStates.Added Select cust.State).First)
                    skipAhead = True
                End Try

                If Not skipAhead Then
                    ' doesn't make sense to try AttachLink again when object is in Added state
                    Try
                        ctx.AttachLink(customer, "Orders", order)      ' already exists
                        Assert.Fail("missing InvalidOperationException")
                    Catch ex As InvalidOperationException
                        Debug.WriteLine(ex.Message)
                    End Try

                    Assert.IsTrue(ctx.DetachLink(customer, "Orders", order))
                    Assert.IsFalse(ctx.DetachLink(customer, "Orders", order))

                    ctx.AttachLink(customer, "Orders", order)

                    ctx.DeleteLink(customer, "Orders", order)
                    Assert.AreEqual(EntityStates.Deleted, (From cust In ctx.Links Select cust.State).Single)
                End If

                Assert.IsTrue(ctx.Detach(customer))
                Assert.IsFalse(ctx.DetachLink(customer, "Orders", order))

                Assert.IsTrue(ctx.Detach(order))
                Assert.IsFalse(ctx.DetachLink(customer, "Orders", order))
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> _
        Public Overloads Sub DeleteLinkFailure()
            Dim customer As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "Microsoft")
            Dim order As NorthwindSimpleModel.Orders = NorthwindSimpleModel.Orders.CreateOrders(35)
            Dim detachedCustomer As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "Microsoft")
            Dim detachedOrder As NorthwindSimpleModel.Orders = NorthwindSimpleModel.Orders.CreateOrders(35)

            Try
                ctx.DeleteLink(Nothing, "Orders", order)
                Assert.Fail("missing ArgumentNullException")
            Catch ex As ArgumentNullException
                Assert.AreEqual("source", ex.ParamName)
            End Try

            Try
                ctx.DeleteLink(customer, Nothing, order) ' not tracking customer
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.DeleteLink(customer, "Orders", Nothing) ' not tracking customer
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            ctx.AttachTo("Customers", customer)
            Try
                ctx.DeleteLink(customer, Nothing, order) ' not tracking orders
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.DeleteLink(customer, Nothing, Nothing)
                Assert.Fail("missing ArgumentNullException")
            Catch ex As ArgumentNullException
                Assert.AreEqual("target", ex.ParamName)
            End Try

            ctx.AttachTo("Orders", order)
            Try
                ctx.DeleteLink(customer, Nothing, order)
                Assert.Fail("missing ArgumentNullException")
            Catch ex As ArgumentNullException
                Assert.AreEqual("sourceProperty", ex.ParamName)
            End Try

            ctx.DeleteLink(customer, "Orders", order)
            Assert.IsTrue(ctx.DetachLink(customer, "Orders", order))


            ctx.DeleteLink(customer, "Orders", order)
            ctx.DeleteLink(customer, "Orders", order)   ' second call is okay

            Try
                ctx.AddLink(customer, "Orders", order)  ' link already exists
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachLink(customer, "Orders", order)  ' link already exists
                Assert.Fail("missing InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            ctx.DeleteObject(customer)
            Assert.AreEqual(EntityStates.Deleted, (From cust In ctx.Links Select cust.State).Single)

            Assert.IsTrue(ctx.DetachLink(customer, "Orders", order))
            Assert.IsFalse(ctx.DetachLink(customer, "Orders", order))
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub AddLink()
            Dim customer As New NorthwindSimpleModel.Customers
            customer.CustomerID = "abcde"
            ctx.AddObject("Customers", customer)
            Assert.AreEqual(0, ctx.Links.Count)

            For i As Int32 = 0 To 1000
                Dim order As New NorthwindSimpleModel.Orders
                order.OrderID = i
                ctx.AddObject("Orders", order)

                customer.Orders.Add(order)
                ctx.AddLink(customer, "Orders", order)
            Next

            Dim bindings As IList(Of LinkDescriptor) = ctx.Links
            Assert.AreEqual(customer.Orders.Count, bindings.Count)
            Assert.AreEqual(customer.Orders.Count, (From o In bindings Where o.State = EntityStates.Added).Count)

            For Each pair As LinkDescriptor In (From o In bindings Where o.Source Is customer)
                Assert.AreEqual("Orders", pair.SourceProperty)
                Assert.IsTrue(CType(customer.Orders, IList).Contains(pair.Target))
            Next

            For Each cust As LinkDescriptor In (From o In bindings Where o.State = EntityStates.Added)
                ctx.DeleteLink(cust.Source, cust.SourceProperty, cust.Target)
            Next

            Assert.AreEqual(0, ctx.Links.Count)

            ctx.Detach(customer)

            Assert.AreEqual(0, ctx.Links.Count)

            ' test deleting objects in Added state -> detached
            ctx.AddObject("Customers", customer)
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.AddLink(customer, "Orders", order)
            Next
            Assert.IsTrue(0 < ctx.Links.Count)

            ctx.DeleteObject(customer)

            Assert.AreEqual(0, ctx.Links.Count)
            Assert.AreEqual(0, (From o In ctx.Entities Where o.Entity Is customer Select o).Count())

            ' test detaching objects in Added state -> detached
            ctx.AddObject("Customers", customer)
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.AddLink(customer, "Orders", order)
            Next
            Assert.IsTrue(0 < ctx.Links.Count)

            ctx.Detach(customer)

            Assert.AreEqual(0, ctx.Links.Count)
            Assert.AreEqual(0, (From o In ctx.Entities Where o.Entity Is customer Select o).Count())

            ' test deleting target
            ctx.AddObject("Customers", customer)
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.AddLink(customer, "Orders", order)
            Next
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.DeleteObject(order)
            Next
            Assert.AreEqual(0, ctx.Links.Count)

        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub AttachLink()
            Dim customer As New NorthwindSimpleModel.Customers
            customer.CustomerID = "abcde"
            ctx.AttachTo("Customers", customer)
            Assert.AreEqual(0, ctx.Links.Count, "#1")

            For i As Int32 = 0 To 1000
                Dim order As New NorthwindSimpleModel.Orders
                order.OrderID = i
                ctx.AttachTo("Orders", order)

                customer.Orders.Add(order)
                ctx.AttachLink(customer, "Orders", order)
            Next

            Dim bindings As IList(Of LinkDescriptor) = ctx.Links
            Assert.AreEqual(customer.Orders.Count, bindings.Count, "#2")
            For Each pair As LinkDescriptor In From o In bindings Where o.Source Is customer
                Assert.AreEqual("Orders", pair.SourceProperty, "#3")
                Assert.IsTrue(CType(customer.Orders, IList).Contains(pair.Target), "#4")
            Next

            For Each pair As LinkDescriptor In ctx.Links
                ctx.DeleteLink(pair.Source, pair.SourceProperty, pair.Target)
            Next

            bindings = ctx.Links
            Assert.AreEqual(customer.Orders.Count, bindings.Count, "#5")
            Assert.AreEqual(customer.Orders.Count, (From o In bindings Where o.State = EntityStates.Deleted).Count, "#6")

            For Each pair As LinkDescriptor In bindings
                Assert.IsTrue(ctx.DetachLink(pair.Source, pair.SourceProperty, pair.Target), "#7")
            Next

            Assert.AreEqual(0, ctx.Links.Count, "#8")

            ctx.Detach(customer)
            Assert.AreEqual(0, ctx.Links.Count, "#9")

            ' test deleting object in unchanged state -> deleted state
            ctx.AttachTo("Customers", customer)
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.AddLink(customer, "Orders", order)
            Next
            Assert.IsTrue(0 < ctx.Links.Count, "#10")

            ctx.DeleteObject(customer)

            Assert.AreEqual(customer.Orders.Count, ctx.Links.Count, "#11")
            Assert.AreEqual(1, (From o In ctx.Entities Where o.Entity Is customer And o.State = EntityStates.Deleted Select o).Count(), "#12")

            ctx.Detach(customer)

            ' test deleting object in unchanged state -> deleted state
            ctx.AttachTo("Customers", customer)
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.AttachLink(customer, "Orders", order)
            Next
            Assert.IsTrue(0 < ctx.Links.Count, "#13")

            ctx.DeleteObject(customer)

            Assert.AreEqual(customer.Orders.Count, ctx.Links.Count, "#14")
            Assert.IsNull((From o In ctx.Links Where o.State <> EntityStates.Unchanged).FirstOrDefault, "#15")
            Assert.AreEqual(1, (From o In ctx.Entities Where o.Entity Is customer And o.State = EntityStates.Deleted Select o).Count(), "#16")

            ' test deleting object in deleted state -> deleted state
            ctx.DeleteObject(customer)

            Assert.AreEqual(customer.Orders.Count, ctx.Links.Count, "#17")
            Assert.IsNull((From o In ctx.Links Where o.State <> EntityStates.Unchanged).FirstOrDefault, "#18")
            Assert.AreEqual(1, (From o In ctx.Entities Where o.Entity Is customer And o.State = EntityStates.Deleted Select o).Count(), "#19")

            ' test deleting object in deleted state -> detached state

            ctx.Detach(customer)

            Assert.AreEqual(0, ctx.Links.Count, "#20")
            Assert.AreEqual(0, (From o In ctx.Entities Where o.Entity Is customer Select o).Count(), "#21")


            ' test deleting object in modified state -> deleted state
            ctx.AttachTo("Customers", customer)
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.AttachLink(customer, "Orders", order)
            Next
            Assert.IsTrue(0 < ctx.Links.Count, "#22")
            ctx.UpdateObject(customer)

            ctx.DeleteObject(customer)

            Assert.AreEqual(customer.Orders.Count, ctx.Links.Count, "#23")
            Assert.IsNull((From o In ctx.Links Where o.State <> EntityStates.Unchanged).FirstOrDefault, "#24")
            Assert.AreEqual(1, (From o In ctx.Entities Where o.Entity Is customer And o.State = EntityStates.Deleted Select o).Count(), "#25")

            ctx.Detach(customer)

            ' test deleting target
            ctx.AttachTo("Customers", customer)
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.AttachLink(customer, "Orders", order)
            Next
            For Each order As NorthwindSimpleModel.Orders In customer.Orders
                ctx.DeleteObject(order)
            Next
            Assert.AreEqual(customer.Orders.Count, ctx.Links.Count, "#26")
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub AddLinkForReference()
            Dim customer As New NorthwindSimpleModel.Customers
            customer.CustomerID = "abcde"
            ctx.AttachTo("Customers", customer)
            Assert.AreEqual(0, ctx.Links.Count)

            Dim order As New NorthwindSimpleModel.Orders
            order.OrderID = 1

            ctx.AttachTo("Orders", order)

            Try
                ctx.AddLink(order, "Customers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.DeleteLink(order, "Customers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            ctx.SetLink(order, "Customers", customer)

            Try
                ctx.SetLink(customer, "Orders", order)
                Assert.Fail("expected InvalidOperation")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.SetLink(customer, "Orders", Nothing)
                Assert.Fail("expected InvalidOperation")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachLink(order, "Customers", customer)
                Assert.Fail("expected InvalidOperation")
            Catch ex As InvalidOperationException
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub DeleteObjectAfterSaveNoChangeShouldWork()
            Dim customer = NorthwindSimpleModel.Customers.CreateCustomers("ABCDE", "Microsoft")
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9999)
            order.Customers = customer

            ctx.AttachTo("Customers", customer)
            ctx.AttachTo("Orders", order)
            ctx.AttachLink(order, "Customers", customer)

            'No data change
            ctx.SaveChanges()

            ctx.DeleteObject(order)

            Dim entities = ctx.Entities
            Dim links = ctx.Links

            Assert.AreEqual(2, entities.Count)
            Assert.AreEqual(EntityStates.Unchanged, entities.Item(0).State)
            Assert.AreEqual(EntityStates.Deleted, entities.Item(1).State)

            Assert.AreEqual(1, links.Count)

            Assert.AreEqual(EntityStates.Unchanged, links.Item(0).State)
        End Sub
    End Class
End Class