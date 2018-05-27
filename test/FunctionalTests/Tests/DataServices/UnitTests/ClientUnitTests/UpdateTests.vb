'---------------------------------------------------------------------
' <copyright file="UpdateTests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports Microsoft.OData.Client
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data.Test.Astoria
Imports System.IO
Imports System.Net
Imports System.Linq
Imports System.Threading
Imports System.Xml.Linq
Imports NorthwindModel
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports AstoriaUnitTests.Stubs.DataServiceProvider
Imports AstoriaUnitTests.Tests
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports <xmlns:dns="http://docs.oasis-open.org/odata/ns/data">

Partial Public Class ClientModule

    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    <TestClass()> Public Class ClientUpdateTests
        Inherits AstoriaTestCase

        Private Shared web As TestWebRequest = Nothing
        Private Shared originalHostType As Type = Nothing
        Private sentRequests As New List(Of HttpWebRequest)()
        Private ctx As NorthwindSimpleModel.NorthwindContext = Nothing

#Region "Initialize DataService and create new context for each text"
        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            originalHostType = BaseTestWebRequest.HostInterfaceType
            BaseTestWebRequest.HostInterfaceType = GetType(Microsoft.OData.Service.IDataServiceHost2)
            web = TestWebRequest.CreateForInProcessWcf
            web.DataServiceType = ServiceModelData.Northwind.ServiceModelType
            web.StartService()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            BaseTestWebRequest.HostInterfaceType = originalHostType
            If Not web Is Nothing Then
                web.Dispose()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Me.ctx = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
            AddHandler Me.ctx.SendingRequest2, AddressOf SendingRequestListenHttpMethod
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
            Me.sentRequests.Clear()
        End Sub

        Private Sub SendingRequestListenHttpMethod(ByVal sender As Object, ByVal args As SendingRequest2EventArgs)
            Dim httpRequestMessage = TryCast(args.RequestMessage, HttpWebRequestMessage)
            If httpRequestMessage IsNot Nothing Then
                Dim httpRequest = httpRequestMessage.HttpWebRequest
                sentRequests.Add(httpRequest)
            End If

            If args.RequestMessage.Method = "DELETE" Then
                Assert.IsNull(args.RequestMessage.GetHeader("Content-Type"), "For Delete requests, the content type should be null")
            End If
        End Sub
#End Region

        <TestCategory("Partition1")> <TestMethod()> Public Sub VerifyEscapeDataString()
            Dim source As String = ".:/?#'%\g"
            Dim encoded As String = Uri.EscapeDataString(source).Replace("%27", "'").Replace("'", "''")
            Assert.IsFalse(0 <= encoded.IndexOfAny(New Char() {":"c, "/"c, "?"c, "#"c}))
            Assert.AreEqual(2, (From a In encoded Where a = "'"c).Count)

            Dim request As Uri = New Uri("http://localhost/service.svc/entityset('" + encoded + "')")
            source = request.OriginalString
            encoded = request.AbsoluteUri

            Dim segments As String() = request.Segments

            ' Uri has different behaviour between .Net v4.0 & .Net v4.5
            If request.OriginalString = request.AbsoluteUri Then
                Assert.AreEqual(3, segments.Count)
            Else
                Assert.AreEqual(5, segments.Count)
            End If

            Dim decoded As String = Uri.UnescapeDataString("%39")
            Assert.IsTrue(String.Equals("9", decoded, StringComparison.Ordinal), "the %3 wasn't dropped")

        End Sub

        Private baseRegionID As Integer
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdateSimpleInsert()
            ClientUpdateSimpleInsert1(False)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdateSimpleInsertTunneling()
            ClientUpdateSimpleInsert1(True)
        End Sub
        Private Sub ClientUpdateSimpleInsert1(ByVal tunneling As Boolean)
            Dim resourceUri As Uri = Nothing

            Dim originalMergeOption As MergeOption = MergeOption.AppendOnly
            ctx.MergeOption = originalMergeOption
            ctx.UsePostTunneling = tunneling

            ' contains all Products, Suppliers and Territories
            Dim loaded As New HashSet(Of Object)

            For Each x As NorthwindSimpleModel.Suppliers In (From e In ctx.CreateQuery(Of NorthwindSimpleModel.Suppliers)("Suppliers")
                                                             Where e.SupplierID = 22
                                                             Select e)
                Assert.IsNotNull(x)
                loaded.Add(x)
            Next
            Assert.AreEqual(loaded.Count, ctx.Entities.Count, "Resources 0")
            Assert.AreEqual(1, loaded.Count)

            Dim tot1 As Int32 = 0   ' count of categories
            For Each category As NorthwindSimpleModel.Categories In ctx.Categories
                tot1 += 1
                ctx.TryGetUri(category, resourceUri)
                Assert.IsTrue(ctx.BaseUri.IsBaseOf(resourceUri))

                ctx.LoadProperty(category, "Products")
                For Each item In category.Products
                    Assert.IsNotNull(item)
                    loaded.Add(item)    ' all products referenced by a category
                Next

                Dim myproducts As String = resourceUri.ToString() + "/Products"
                For Each p1 As IEnumerable(Of NorthwindSimpleModel.Products) In ctx.Execute(Of ICollection(Of NorthwindSimpleModel.Products))(myproducts)
                    For Each p2 As NorthwindSimpleModel.Products In p1
                        Assert.IsNotNull(p2)
                        Assert.IsTrue(loaded.Contains(p2))  ' verify all products instances already discovered
                    Next
                Next
            Next
            Assert.IsTrue(0 < tot1, "Categories")
            Assert.AreEqual(tot1 + loaded.Count, ctx.Entities.Count, "Resources 1")

            'For Each p1 As IList(Of NorthwindModel.Products) In (From cat1 In ctx.Categories Select cat1.Products)
            '    For Each item In p1
            '        Assert.IsTrue(loaded.Contains(item))
            '    Next
            'Next

            'For Each p2 As IList(Of NorthwindModel.Products) In (From cat1 In ctx.Categories From prod1 In cat1.Products Select prod1)
            '    For Each item In p2
            '        Assert.IsTrue(loaded.Contains(item))
            '    Next
            'Next

            Dim tot2 As Int32 = 0 ' count of customers
            For Each customer As NorthwindSimpleModel.Customers In ctx.Customers
                tot2 += 1
                ctx.TryGetUri(customer, resourceUri)
                Assert.IsTrue(ctx.BaseUri.IsBaseOf(resourceUri))
            Next
            Assert.IsTrue(0 < tot2, "Customers")
            Assert.AreEqual(tot1 + tot2 + loaded.Count, ctx.Entities.Count, "Resources 2")

            Dim tot3 As Int32 = 0 ' count of employees
            For Each employee As NorthwindSimpleModel.Employees In ctx.Employees
                tot3 += 1
                ctx.TryGetUri(employee, resourceUri)
                Assert.IsTrue(ctx.BaseUri.IsBaseOf(resourceUri))

                ctx.LoadProperty(employee, "Territories")
                For Each item In employee.Territories
                    Assert.IsNotNull(item, "Territories")
                    loaded.Add(item)
                Next
            Next
            Assert.IsTrue(0 < tot3, "Employees")
            Assert.AreEqual(tot1 + tot2 + tot3 + loaded.Count, ctx.Entities.Count, "Resources 3")

            Dim shipperSet As New HashSet(Of NorthwindSimpleModel.Shippers)
            Dim tot4 As Int32 = 0   ' count of orders
            Dim tot4a As Int32 = 0  ' count of shippers
            For Each order1 As NorthwindSimpleModel.Orders In ctx.Execute(Of NorthwindSimpleModel.Orders)("Orders?$expand=Shippers")
                tot4 += 1
                ctx.TryGetUri(order1, resourceUri)
                Assert.IsTrue(ctx.BaseUri.IsBaseOf(resourceUri))

                If (Not order1.Shippers Is Nothing) Then
                    If shipperSet.Add(order1.Shippers) Then
                        tot4a += 1
                    End If
                End If
            Next
            Assert.IsTrue(1 < tot4, "Orders")
            Assert.IsTrue(1 < tot4a, "Orders/Shippers")
            tot4 += tot4a
            Assert.AreEqual(tot1 + tot2 + tot3 + tot4 + loaded.Count, ctx.Entities.Count, "Resources 4")

            Dim lastDetail As Object = Nothing
            Dim tot5 As Int32 = 0   ' count of first 20 order details
            For Each detail As NorthwindSimpleModel.Order_Details In (From od In ctx.Order_Details Take 20 Select od)
                tot5 += 1
                ctx.TryGetUri(detail, resourceUri)
                Assert.IsTrue(ctx.BaseUri.IsBaseOf(resourceUri))

                lastDetail = detail
            Next
            Assert.IsTrue(0 < tot5, "Order_Details")
            Assert.AreEqual(tot1 + tot2 + tot3 + tot4 + tot5 + loaded.Count, ctx.Entities.Count, "Resources 5")
            Assert.AreEqual(1, Enumerable.Count(From x In ctx.Entities Where x.Entity.GetType() Is GetType(NorthwindSimpleModel.Suppliers) Select x))

            Dim lastProduct As New List(Of NorthwindSimpleModel.Products)
            Dim tot6 As Int32 = 0   ' count of products
            For Each product As NorthwindSimpleModel.Products In ctx.Products
                tot6 += 1
                ctx.TryGetUri(product, resourceUri)
                Assert.IsTrue(ctx.BaseUri.IsBaseOf(resourceUri))

                Assert.IsNull(product.Suppliers)
                lastProduct.Add(product)

                ctx.LoadProperty(product, "Suppliers")  ' add suppliers to resources?
                loaded.Add(product) ' all products referenced from category already loaded, this is the new ones
                ' product may not have a supplier

                If Not product.Suppliers Is Nothing Then
                    loaded.Add(product.Suppliers)
                End If
            Next
            Assert.IsTrue(0 < tot6, "Products")

            Dim resources As IList(Of EntityDescriptor) = ctx.Entities
            Assert.AreEqual(tot1, (From qq In resources Where GetType(NorthwindSimpleModel.Categories).IsInstanceOfType(qq.Entity) Select qq).Count, "Categories")
            Assert.AreEqual(tot2, (From qq In resources Where GetType(NorthwindSimpleModel.Customers).IsInstanceOfType(qq.Entity) Select qq).Count, "Customers")
            Assert.AreEqual(tot3, (From qq In resources Where GetType(NorthwindSimpleModel.Employees).IsInstanceOfType(qq.Entity) Select qq).Count, "Employees")
            Assert.AreEqual(tot4, (From qq In resources Where GetType(NorthwindSimpleModel.Orders).IsInstanceOfType(qq.Entity) Or GetType(NorthwindSimpleModel.Shippers).IsInstanceOfType(qq.Entity) Select qq).Count, "Orders")
            Assert.AreEqual(tot5, (From qq In resources Where GetType(NorthwindSimpleModel.Order_Details).IsInstanceOfType(qq.Entity) Select qq).Count, "Order_Details")
            Assert.AreEqual(tot6, (From qq In resources Where GetType(NorthwindSimpleModel.Products).IsInstanceOfType(qq.Entity) Select qq).Count, "Products")
            Assert.AreEqual(loaded.Count, (From qq In loaded Where GetType(NorthwindSimpleModel.Products).IsInstanceOfType(qq) Or GetType(NorthwindSimpleModel.Suppliers).IsInstanceOfType(qq) Or GetType(NorthwindSimpleModel.Territories).IsInstanceOfType(qq) Select qq).Count, "Products, Suppilers, Territories")

            For Each x In (From qq In resources Where
                           Not GetType(NorthwindSimpleModel.Categories).IsInstanceOfType(qq.Entity) AndAlso
                           Not GetType(NorthwindSimpleModel.Customers).IsInstanceOfType(qq.Entity) AndAlso
                           Not GetType(NorthwindSimpleModel.Employees).IsInstanceOfType(qq.Entity) AndAlso
                           Not GetType(NorthwindSimpleModel.Orders).IsInstanceOfType(qq.Entity) AndAlso
                           Not GetType(NorthwindSimpleModel.Shippers).IsInstanceOfType(qq.Entity) AndAlso
                           Not GetType(NorthwindSimpleModel.Order_Details).IsInstanceOfType(qq.Entity) AndAlso
                           Not loaded.Contains(qq.Entity) Select qq.Entity)
                Assert.Fail(x.GetType().ToString())
            Next

            Assert.AreEqual(tot1 + tot2 + tot3 + tot4 + tot5 + loaded.Count, resources.Count, "Resources")
            Assert.AreEqual(resources.Count, (From o In resources Where EntityStates.Unchanged = o.State).Count)

            ' make changes
            Dim key As String = "ABCDF"
            ' if second pass (tunneling on), change key to avoid PK violation
            If tunneling Then key = "GHIJK"
            Dim customerInsert As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers(key, "Microsoft")
            ctx.AddObject("Customers", customerInsert)

            ctx.DeleteObject(lastDetail)

            lastProduct.ForEach(AddressOf ctx.UpdateObject)

            Assert.AreEqual(1 + 1 + tot6, (From o In ctx.Entities Where o.State <> EntityStates.Unchanged).Count)

            'set timeout

            Assert.AreEqual(0, ctx.Timeout)

            ctx.Timeout = 30
            Assert.AreEqual(30, ctx.Timeout)

            ctx.Timeout = 0
            Assert.AreEqual(0, ctx.Timeout)

            ctx.Timeout = Int32.MaxValue
            Assert.AreEqual(Int32.MaxValue, ctx.Timeout)

            Try
                ctx.Timeout = -1
            Catch ex As ArgumentException
                Assert.AreEqual("Timeout", ex.ParamName)
            End Try

            'submit changes

            ctx.MergeOption = MergeOption.OverwriteChanges
            Util.SaveChanges(ctx)

            ctx.TryGetUri(customerInsert, resourceUri)
            Assert.IsTrue(ctx.BaseUri.IsBaseOf(resourceUri))

            ctx.TryGetUri(lastDetail, resourceUri)
            Assert.IsNull(resourceUri)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_0()
            ' create the customer to database
            Dim ctx1 = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)
            'ctx1.EnableAtom = True
            'ctx1.Format.UseAtom()
            ctx1.AddToCustomers(NorthwindSimpleModel.Customers.CreateCustomers("88998", "Microsoft"))
            ctx1.SaveChanges()
            Assert.AreEqual(1, (From e In ctx1.Entities Where e.State = EntityStates.Unchanged Select e).Count)

            ' show the customer is in the database
            Dim ctx2 = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)
            'ctx2.EnableAtom = True
            'ctx2.Format.UseAtom()
            Dim customer2 = (From cust In ctx2.Customers Where cust.CustomerID = "88998" Select cust).First
            Assert.IsNotNull(customer2)

            ' remove customer from database
            ctx2.DeleteObject(customer2)
            ctx2.SaveChanges()
            Assert.AreEqual(1, ctx1.Entities.Count)

            ' add the customer again
            Dim ctx3 = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)
            'ctx3.EnableAtom = True
            'ctx3.Format.UseAtom()
            ctx3.AddToCustomers(NorthwindSimpleModel.Customers.CreateCustomers("88998", "Microsoft"))
            ctx3.SaveChanges()
            Assert.AreEqual(1, ctx3.Entities.Count + ctx3.Links.Count)

            'refresh customer in context that deleted the customer
            Dim customer3 = ctx2.Execute(Of NorthwindSimpleModel.Customers)(New Uri("Customers('88998')", UriKind.Relative)).First
            Assert.AreEqual(customer2.CustomerID, customer3.CustomerID)
            Assert.AreEqual(customer2.CompanyName, customer3.CompanyName)
            Assert.AreNotSame(customer2, customer3)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_1()
            ctx.MergeOption = MergeOption.NoTracking
            Dim category = ctx.Categories.First()
            ctx.AddToCategories(category)

            Dim chaiProduct = NorthwindSimpleModel.Products.CreateProducts(99999, "New chai product", False)
            ctx.AddToProducts(chaiProduct)

            ctx.SetLink(chaiProduct, "Categories", category)

            Util.SaveChanges(ctx, SaveChangesOptions.ContinueOnError, 2, 0)

            Assert.AreEqual(3, sentRequests.Count)
            Dim ss = (From s In sentRequests Select s.Method).ToArray()
            Assert.AreEqual("GET", ss(0))
            Assert.AreEqual("POST", ss(1))
            Assert.AreEqual("POST", ss(2))
            sentRequests.Clear()

            ctx.SetLink(chaiProduct, "Categories", Nothing)
            ctx.AddLink(category, "Products", chaiProduct)

            Util.SaveChanges(ctx, SaveChangesOptions.ContinueOnError)
            Assert.AreEqual(2, sentRequests.Count)
            ss = (From s In sentRequests Select s.Method).ToArray()
            Assert.AreEqual("DELETE", ss(0))
            Assert.AreEqual("POST", ss(1))

        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_ServerSendingPayloadInUpdateOperations()
            TestUtil.ClearMetadataCache()
            Try
                Using CustomDataContext.CreateChangeScope()
                    Using web = TestWebRequest.CreateForInProcessWcf
                        web.DataServiceType = ServiceModelData.CustomData.ServiceModelType
                        web.StartService()
                        Try
                            Dim customCtx As DataServiceContext = New DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4)
                            'customCtx.EnableAtom = True
                            'customCtx.Format.UseAtom()
                            customCtx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent
                            Dim customer = customCtx.Execute(Of AstoriaUnitTests.Stubs.Customer)(New Uri("/Customers(0)", UriKind.Relative)).First()
                            Dim original As Guid = customer.GuidValue

                            AddHandler customCtx.SendingRequest2, Sub(sender, args)
                                                                      Assert.IsNotNull(args.RequestMessage.GetHeader("If-Match"), "If-Match header must be specified")
                                                                  End Sub
                            customCtx.UpdateObject(customer)
                            Dim response = customCtx.SaveChanges()
                            Assert.AreEqual(response.First().StatusCode, 200, "Expecting the status code to be 200, since update request should now return a payload")
                            Dim current As Guid = customer.GuidValue
                            Assert.IsFalse(original.Equals(current), "ETag property must be updated")
                        Finally
                            web.StopService()
                        End Try
                    End Using
                End Using
            Finally
                TestUtil.ClearMetadataCache()
            End Try
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_2()
            Dim c As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ab", "b")

            ctx.AddObject("Customers", c)
            ctx.MergeOption = MergeOption.OverwriteChanges
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0)

            c.ContactName = "foo"
            ctx.UpdateObject(c)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0)

            ctx = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim d As NorthwindSimpleModel.Customers = Enumerable.Single(Of NorthwindSimpleModel.Customers)(ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("/Customers('ab')"))
            Assert.AreEqual("foo", d.ContactName)
            Assert.AreEqual("b", d.CompanyName)

            GC.KeepAlive(c)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_3()
            ctx.MergeOption = MergeOption.PreserveChanges

            Dim c As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("56344", "foo")
            c.ContactName = "Bianca"
            c.ContactTitle = "daughter"

            Dim o As New NorthwindSimpleModel.Orders
            o.OrderID = 99999
            o.OrderDate = DateTime.Now

            c.Orders.Add(o)

            ctx.AddObject("Customers", c)
            ctx.AddObject("Orders", o)
            ctx.AddLink(c, "Orders", o)

            ctx.MergeOption = MergeOption.OverwriteChanges
            Util.SaveChanges(ctx)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_4()
            Dim c As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ALFKI", "foo")
            ctx.AttachTo("Customers", c)
            Assert.AreEqual(1, ctx.Entities.Count)
            Assert.AreSame(c, (From x In ctx.Entities Where x.State = EntityStates.Unchanged Select x.Entity).Single)

            ctx.LoadProperty(c, "Orders")
            Assert.AreEqual(c.Orders.Count, ctx.Links.Count)

            Dim c1 As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("edcba", "foo")
            ctx.AddObject("Customers", c1)
            Assert.AreSame(c1, (From x In ctx.Entities Where x.State = EntityStates.Added Select x.Entity).Single)

            For Each o As NorthwindSimpleModel.Orders In c.Orders
                c1.Orders.Add(o)
                ctx.AddLink(c1, "Orders", o)
                Assert.IsTrue(ctx.DetachLink(c, "Orders", o))
            Next
            Assert.AreEqual(c1.Orders.Count, ctx.Links.Count)

            ctx.DeleteObject(c)
            Assert.AreEqual(1 + c.Orders.Count, (From x In ctx.Entities Where x.State <> EntityStates.Deleted).Count)

            ctx.MergeOption = MergeOption.OverwriteChanges

            ' should add edcba & update the links
            ' should delete delete ALKFI

            Util.SaveChanges(ctx, SaveChangesOptions.None)

            ctx.MergeOption = MergeOption.NoTracking
            Dim c2 As NorthwindSimpleModel.Customers = ctx.Execute(Of NorthwindSimpleModel.Customers)("Customers('edcba')?$expand=Orders").Single()

            Assert.AreEqual(c1.CustomerID, c2.CustomerID)
            Assert.AreEqual(c1.Region, c2.Region)
            Assert.AreEqual(c1.Orders.Count, c2.Orders.Count)
            Assert.AreEqual(c1.Orders.Count, (From a In c1.Orders From b In c2.Orders Where a.OrderID = b.OrderID Select b).Count)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_5()
            Dim a = NorthwindSimpleModel.Products.CreateProducts(999, "regression test", True)
            ctx.AddToProducts(a)
            a.Categories = ctx.Categories.First()
            ctx.SetLink(a, "Categories", a.Categories)
            Util.SaveChanges(ctx, SaveChangesOptions.None)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Assert.AreEqual(2, entities.Count, "entity count")
            Assert.AreEqual(1, links.Count, "link count")
            Assert.IsTrue(entities.All(Function(x) x.State = EntityStates.Unchanged), "not unchanged entity")
            Assert.IsTrue(links.All(Function(x) x.State = EntityStates.Unchanged), "not unchanged link")

            ctx.SetLink(a, "Categories", Nothing)
            ctx.DeleteObject(a)
            Util.SaveChanges(ctx, SaveChangesOptions.None)

            entities = ctx.Entities
            links = ctx.Links
            Assert.AreEqual(1, entities.Count, "cleanup entity count")
            Assert.AreEqual(0, links.Count, "cleanup link count")
            Assert.IsTrue(entities.All(Function(x) x.State = EntityStates.Unchanged), "not unchanged entity")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_6()
            Dim p = NorthwindSimpleModel.Products.CreateProducts(999, "regression test", True)
            ctx.AddToProducts(p)
            Util.SaveChanges(ctx, SaveChangesOptions.None)

            Dim c = NorthwindSimpleModel.Categories.CreateCategories(998, "double jepordy")
            ctx.AddToCategories(c)

            c.Products.Add(p)
            ctx.AddLink(c, "Products", p)
            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset)

            ctx.DeleteLink(c, "Products", p)
            ctx.DeleteObject(c)
            ctx.DeleteObject(p)
            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_7()
            Dim e1 = NorthwindSimpleModel.Employees.CreateEmployees(1, "Foo", "Bar")
            Dim e2 = NorthwindSimpleModel.Employees.CreateEmployees(2, "Foo1", "Bar1")

            ctx.AddToEmployees(e1)
            ctx.AddToEmployees(e2)
            Util.SaveChanges(ctx, SaveChangesOptions.None)

            ctx.AddLink(e1, "Employees1", e2)
            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset)

            ctx.DeleteLink(e1, "Employees1", e2)
            ctx.DeleteObject(e2)
            ctx.DeleteObject(e1)
            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_8()
            Dim r = NorthwindSimpleModel.Region.CreateRegion(777, "Antartica")
            ctx.AddToRegion(r)

            Dim t = NorthwindSimpleModel.Territories.CreateTerritories("ANC", "frozen desert")
            ctx.AddToTerritories(t)

            t.Region = r
            ctx.SetLink(t, "Region", r)

            Util.SaveChanges(ctx, SaveChangesOptions.None)

            ctx.DeleteObject(t)
            ctx.DeleteObject(r)
            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_9()
            Dim p = NorthwindSimpleModel.Products.CreateProducts(999, "regression test", True)
            ctx.AddToProducts(p)
            Util.SaveChanges(ctx, SaveChangesOptions.None)

            Assert.AreEqual(1, sentRequests.Count)
            Assert.AreEqual("POST", sentRequests.Item(0).Method)
            sentRequests.Clear()

            Dim c = NorthwindSimpleModel.Categories.CreateCategories(998, "double jepordy")
            ctx.AddToCategories(c)

            c.Products.Add(p)
            ctx.AddLink(c, "Products", p)
            Util.SaveChanges(ctx, SaveChangesOptions.None)

            Assert.AreEqual(1, sentRequests.Count)
            Assert.AreEqual("POST", sentRequests.Item(0).Method)

            ctx.DeleteLink(c, "Products", p)
            ctx.DeleteObject(c)
            ctx.DeleteObject(p)
            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset)
        End Sub

#Region "ComboSaveChanges"
        Private Shared failureMessages As HashSet(Of String) = New HashSet(Of String)()
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Overloads Sub ComboSaveChanges()
            Dim states As EntityStates() = New EntityStates() {EntityStates.Detached, EntityStates.Added, EntityStates.Unchanged, EntityStates.Modified, EntityStates.Deleted}
            Dim options As SaveChangesOptions() = New SaveChangesOptions() {SaveChangesOptions.None, SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.ContinueOnError}
            Dim bools As Boolean() = New Boolean() {True, False}

            Dim engine As CombinatorialEngine = CombinatorialEngine.FromDimensions(
                New Dimension("ParentState", states),
                New Dimension("ChildState", states),
                New Dimension("BindingState", states),
                New Dimension("CollectionOrReference", bools),
                New Dimension("Synchronous", bools),
                New Dimension("Changeset", options))

            Dim passCount As Int32 = 0
            Dim failCount As Int32 = 0
            TestUtil.RunCombinatorialEngine(engine, AddressOf ComboSaveChanges, passCount, failCount)

            Assert.AreEqual(0, failCount, "failed")
            Assert.AreEqual(1500, passCount, "succeded")
        End Sub

        Public Overloads Sub ComboSaveChanges(ByVal hashtable As Hashtable)
            Dim parentState As EntityStates = CType(hashtable.Item("ParentState"), EntityStates)
            Dim childState As EntityStates = CType(hashtable.Item("ChildState"), EntityStates)
            Dim bindingState As EntityStates = CType(hashtable.Item("BindingState"), EntityStates)
            Dim collectionOrReference As Boolean = CBool(hashtable.Item("CollectionOrReference"))
            Dim synchronous As Boolean = CBool(hashtable.Item("Synchronous"))
            Dim changeset As SaveChangesOptions = CType(hashtable.Item("Changeset"), SaveChangesOptions)

            Me.ctx = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
            Me.ctx.Timeout = 30
            beginSaveChanges = False

            Dim parentResource As Object = Nothing
            Dim parentProperty As String = Nothing

            Dim childResource As Object = Nothing
            Dim childProperty As String = Nothing

            If (collectionOrReference) Then
                Dim parent As NorthwindSimpleModel.Categories = CreateEntity(Of NorthwindSimpleModel.Categories)("Categories", parentState)
                If (EntityStates.Added = parentState) Then
                    parent.CategoryID = 999998
                    parent.CategoryName = "Software"
                End If

                Dim child As NorthwindSimpleModel.Products = CreateEntity(Of NorthwindSimpleModel.Products)("Products", childState)
                If (EntityStates.Added = childState) Then
                    child.ProductID = 888887
                    child.ProductName = "Astoria"
                    child.Discontinued = False
                End If

                parentResource = parent
                parentProperty = "Products"
                childResource = child
            Else
                Dim parent As NorthwindSimpleModel.Region = CreateEntity(Of NorthwindSimpleModel.Region)("Region", parentState)
                If (EntityStates.Added = parentState) Then
                    parent.RegionID = 999997
                    parent.RegionDescription = "Software"
                End If

                Dim child As NorthwindSimpleModel.Territories = CreateEntity(Of NorthwindSimpleModel.Territories)("Territories", childState)
                If (EntityStates.Added = childState) Then
                    child.TerritoryID = "888887"
                    child.TerritoryDescription = "Astoria"
                End If

                parentResource = parent
                childProperty = "Region"
                childResource = child
            End If

            ' prepare binding
            If parentProperty IsNot Nothing Then
                If Not CreateBinding(parentResource, parentProperty, childResource, bindingState, collectionOrReference) Then
                    ' not valid - exit test
                    Return
                End If
            Else
                If Not CreateBinding(childResource, childProperty, parentResource, bindingState, collectionOrReference) Then
                    ' not valid - exit test
                    Return
                End If
            End If

            'verify pre-submit state
            VerifyEntityState(parentResource, parentState)
            VerifyEntityState(childResource, childState)
            VerifyBindingState(parentResource, parentProperty, childResource, bindingState)

            ' submit changes
            Dim dataservice As DataServiceResponse = Nothing
            Dim failure As Exception = Nothing

            Dim expectedReadCount = (From a In ctx.Entities Where a.State = EntityStates.Added Select a).Count
            Dim count = (From a In ctx.Entities Where a.State <> EntityStates.Unchanged Select a).Count + (From b In ctx.Links Where b.State <> EntityStates.Unchanged).Count

            If synchronous Then
                beginSaveChanges = False
                Try
                    dataservice = ctx.SaveChanges(changeset)
                Catch exception As DataServiceRequestException
                    failure = Util.FindChangesetFailure(exception.Response)
                End Try
            Else
                Dim obj As New Object()
                Dim async As IAsyncResult

                beginSaveChanges = True
                async = ctx.BeginSaveChanges(changeset, Nothing, obj)
                If Not async.IsCompleted Then
                    If Not async.AsyncWaitHandle.WaitOne(New TimeSpan(0, 2, 0), False) Then
                        Throw New TimeoutException("BeginSaveChanges didn't complete in expected time")
                    End If
                End If

                beginSaveChanges = False
                Assert.AreEqual(count, (From a In ctx.Entities Where a.State <> EntityStates.Unchanged Select a).Count + (From b In ctx.Links Where b.State <> EntityStates.Unchanged).Count, "should not have done anything until EndSaveChanges")
                Assert.AreSame(obj, async.AsyncState)
                Try
                    dataservice = ctx.EndSaveChanges(async)
                Catch ex As DataServiceRequestException
                    failure = Util.FindChangesetFailure(ex.Response)
                End Try
            End If

            If Not failure Is Nothing Then
                failureMessages.Add(failure.Message)
                Return
            End If

            ' verify post-submit state
            parentState = GetPostSubmitBindingState(parentState)
            childState = GetPostSubmitBindingState(childState)
            bindingState = GetPostSubmitBindingState(bindingState)

            VerifyEntityState(parentResource, parentState)
            VerifyEntityState(childResource, childState)
            VerifyBindingState(parentResource, parentProperty, childResource, bindingState)
        End Sub

        Public Function CreateEntity(Of T As {New, Class})(ByVal entitySetName As String, ByVal state As EntityStates) As T
            Return AstoriaUnitTests.DataServiceContextTestUtil.CreateEntity(Of T)(ctx, entitySetName, state)
        End Function

        Private beginSaveChanges As Boolean

        Public Function CreateBinding(ByVal parent As Object, ByVal parentProperty As String, ByVal child As Object, ByVal state As EntityStates, ByVal isCollection As Boolean) As Boolean
            Dim list As IList(Of EntityDescriptor) = ctx.Entities
            Dim expectedArgumentParent As Boolean = (From o As EntityDescriptor In list Where o.Entity Is parent).Count = 0
            Dim expectedArugmentChild As Boolean = (EntityStates.Deleted <> state Or isCollection) And (From o As EntityDescriptor In list Where o.Entity Is child).Count = 0
            Dim expectedInvalidOperation As Boolean = False

            Try
                If (2 <> (From o In list Where o.Entity Is parent Or o.Entity Is child).Count) Then
                    expectedInvalidOperation = True
                End If

                Select Case state
                    Case EntityStates.Added
                        If (isCollection) Then
                            expectedInvalidOperation = expectedInvalidOperation Or (From o In list Where o.State = EntityStates.Deleted AndAlso (o.Entity Is parent Or o.Entity Is child)).Count <> 0
                            ctx.AddLink(parent, parentProperty, child)
                        Else
                            ctx.SetLink(parent, parentProperty, child)
                        End If

                    Case EntityStates.Deleted
                        If (isCollection) Then
                            expectedInvalidOperation = expectedInvalidOperation Or (From o In list Where o.State = EntityStates.Added AndAlso (o.Entity Is parent Or o.Entity Is child)).Count <> 0
                            ctx.DeleteLink(parent, parentProperty, child)
                        Else
                            expectedInvalidOperation = Not (From o In list Where o.Entity Is parent).Any()
                            ctx.SetLink(parent, parentProperty, Nothing)
                        End If

                    Case EntityStates.Detached
                        Return True

                    Case EntityStates.Unchanged
                        expectedInvalidOperation = expectedInvalidOperation Or (From o In list Where (o.State = EntityStates.Added Or o.State = EntityStates.Deleted) AndAlso (o.Entity Is parent Or o.Entity Is child)).Count <> 0
                        ctx.AttachLink(parent, parentProperty, child)

                    Case EntityStates.Modified
                        Return False
                End Select

                If expectedArgumentParent Then
                    Assert.IsFalse(expectedArgumentParent, "expected failure when binding with parent")
                End If
                If expectedArugmentChild Then
                    Assert.IsFalse(expectedArugmentChild, "expected failure when binding with child")
                End If
                If expectedInvalidOperation Then
                    Assert.IsFalse(expectedInvalidOperation, "expected failure when binding didn't happen")
                End If
                Return True
            Catch ex As ArgumentException
                If ("source" = ex.ParamName AndAlso expectedArgumentParent) Then
                    Return False
                End If
                If ("target" = ex.ParamName AndAlso expectedArugmentChild) Then
                    Return False
                End If

                Throw
            Catch ex As InvalidOperationException
                If (expectedInvalidOperation) Then
                    Return False
                End If

                Throw
            Catch ex As Exception
                Assert.Fail(String.Format("unexpected {0}: {1}", ex.GetType().Name, ex.Message))
            End Try
            Return True
        End Function

        Public Function GetBindingState(ByVal parent As Object, ByVal parentProperty As String, ByVal child As Object) As EntityStates
            Return (From o In ctx.Links Where o.Source Is parent AndAlso o.SourceProperty = parentProperty AndAlso o.Target Is child).SingleOrDefault.State
        End Function

        Public Function GetPostSubmitBindingState(ByVal state As EntityStates) As EntityStates
            If state <> EntityStates.Deleted AndAlso state <> EntityStates.Detached Then
                Return EntityStates.Unchanged 'accept if inserting or updating
            Else
                Return EntityStates.Detached ' detach if deleting
            End If
        End Function

        Public Sub VerifyEntityState(ByVal entity As Object, ByVal state As EntityStates)
            Dim list As IList(Of EntityDescriptor) = ctx.Entities
            Dim count = (From o In list Where o.Entity Is entity AndAlso o.State = state).Count
            Assert.IsTrue((EntityStates.Detached = state AndAlso 0 = count) Or
                          (EntityStates.Detached <> state AndAlso 1 = count), "EntityStates={0}, Count={1}", state, count)
        End Sub

        Public Sub VerifyBindingState(ByVal parent As Object, ByVal parentProperty As String, ByVal child As Object, ByVal state As EntityStates)
            Select Case state
                Case EntityStates.Added
                Case EntityStates.Deleted
                Case EntityStates.Detached
                Case EntityStates.Unchanged
                Case EntityStates.Modified
            End Select

        End Sub

        Public Sub HandleWebException(ByVal ex As System.Net.WebException)
            AstoriaTestLog.WriteLine(String.Format("{0}: Status={1}, Message={2}", ex.GetType().Name, ex.Status.ToString(), ex.Message))
            Dim response As System.Net.WebResponse = ex.Response
            If (response IsNot Nothing) Then
                Using responseStream As Stream = response.GetResponseStream()
                    If responseStream.CanRead Then
                        Dim text As String = New StreamReader(responseStream).ReadToEnd()
                        AstoriaTestLog.WriteLine(text)
                    End If
                End Using
            End If
        End Sub

#End Region

        ' The following are commands to restore the database if necessary
        ' select * from Orders where CustomerID is null or CustomerID='ANATR' or CustomerID='ASTOR'
        ' select * from Customers where CustomerID='ANATR' or CustomerID='ASTOR'
        ' update Orders Set CustomerID='ANATR' where CustomerID is null or  CustomerID='ASTOR'
        ' delete Customers where CustomerID='ASTOR'
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdateNorthwindCustomer1()
            ctx.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges

            ' insert entity
            Dim cust As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "ADO.NET Data Services")
            Try

                ctx.AddObject("Customers", cust)
                Util.SaveChanges(ctx)

                Dim custUri_A As Uri = Nothing
                ctx.TryGetUri(cust, custUri_A)
                Assert.IsNotNull(custUri_A)
                Assert.IsFalse(String.IsNullOrEmpty(custUri_A.OriginalString))

                ' update entity
                cust.Address = "One Microsoft Way"
                ctx.UpdateObject(cust)
                Util.SaveChanges(ctx)

                Dim custUri_B As Uri = Nothing
                ctx.TryGetUri(cust, custUri_B)
                Assert.IsNotNull(custUri_B)
                Assert.IsFalse(String.IsNullOrEmpty(custUri_B.OriginalString))

                ' add relations to entity
                ctx.MergeOption = Microsoft.OData.Client.MergeOption.NoTracking

                Dim countOfOrders = CType((From c In ctx.Customers Where c.CustomerID = "ANATR" Select c), DataServiceQuery(Of NorthwindSimpleModel.Customers)).Expand("Orders").Single().Orders.Count

                Assert.IsTrue(4 = countOfOrders)

                ctx.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges

                Dim singleCust = CType((From customer In ctx.Customers Where customer.CustomerID = "ANATR" Select customer), DataServiceQuery(Of NorthwindSimpleModel.Customers)).Expand("Orders").Single()
                For Each order As NorthwindSimpleModel.Orders In singleCust.Orders
                    cust.Orders.Add(order)
                    ctx.AddLink(cust, "Orders", order)
                Next
                Assert.IsTrue(4 = cust.Orders.Count)

                Util.SaveChanges(ctx)

                ' verify relations were added
                ctx.MergeOption = Microsoft.OData.Client.MergeOption.NoTracking

                countOfOrders = Enumerable.Count(Enumerable.Single(ctx.Execute(Of NorthwindSimpleModel.Customers)("Customers?$filter=CustomerID eq 'ASTOR'&$expand=Orders")).Orders)
                Assert.IsTrue(4 = countOfOrders)

                ' verify relations were moved, expand with empty collection
                countOfOrders = Enumerable.Count(Enumerable.Single(ctx.Execute(Of NorthwindSimpleModel.Customers)("Customers?$filter=CustomerID eq 'ANATR'&$expand=Orders")).Orders)
                Assert.IsTrue(0 = countOfOrders)
                ctx.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges

                Dim custUri_C As Uri = Nothing
                ctx.TryGetUri(cust, custUri_C)
                Assert.IsNotNull(custUri_C)
                Assert.IsFalse(String.IsNullOrEmpty(custUri_C.OriginalString))
                Assert.AreEqual(custUri_B, custUri_C)

                ' delete relations from entity
                For Each order As NorthwindSimpleModel.Orders In cust.Orders
                    ctx.DeleteLink(cust, "Orders", order)
                Next
                cust.Orders.Clear()
                Assert.IsTrue(0 = cust.Orders.Count)
                Util.SaveChanges(ctx)

                ' verify links deleted
                ctx.LoadProperty(cust, "Orders")
                Assert.IsTrue(0 = cust.Orders.Count)
            Finally
                ctx.DeleteObject(cust)
                Util.SaveChanges(ctx)
            End Try
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_10()
            Dim detail As NorthwindSimpleModel.Order_Details = ctx.Order_Details.First()
            Dim queriedUri As Uri = Nothing
            ctx.TryGetUri(detail, queriedUri)

            ctx.Detach(detail)

            ctx.AttachTo("Order_Details", detail)
            Dim generatedUri As Uri = Nothing
            ctx.TryGetUri(detail, generatedUri)

            Assert.AreEqual(queriedUri, generatedUri)
            Assert.AreSame(detail, ctx.Execute(Of NorthwindSimpleModel.Order_Details)(generatedUri).First())
        End Sub

        <TestCategory("Partition1")> <TestMethod()>
        Public Sub RelativeUriWithTrailingDot()
            Dim uri As Uri = New Uri(ctx.BaseUri.OriginalString + "/Customers.", UriKind.RelativeOrAbsolute)
            Dim absolute As String = uri.AbsoluteUri

            If uri.AbsolutePath = uri.OriginalString Then
                Assert.IsFalse(absolute.EndsWith("."c))
                Assert.AreEqual(5, ctx.Execute(Of NorthwindSimpleModel.Customers)("Customers.?$top=5").Count)
            End If
        End Sub

        <TestCategory("Partition1")> <TestMethod()>
        Public Sub UpdateProducts_ViolateConstraint()
            Dim chaiProduct As New NorthwindSimpleModel.Products()
            chaiProduct.ProductID = 1
            chaiProduct.UnitPrice = Decimal.Parse("30.00")
            ctx.AttachTo("Products", chaiProduct)
            ctx.UpdateObject(chaiProduct)
            Try
                Util.SaveChanges(ctx)
                Assert.Fail("expected exception")
            Catch failure As DataServiceRequestException
                Assert.AreEqual(500, failure.Response.Single().StatusCode, "Making sure we get Internal Server Error, since that was V1/V2 behavior")
                Dim text = Util.PrintException(failure)
                Assert.IsTrue(text.Contains("The 'ProductName' property on 'Products' could not be set to a 'null' value. You must set this property to a non-null value of type 'String'."), text)
            End Try
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub UpdateCustomer_EmptyName()
            UpdateCustomer(False)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ReplaceCustomer_EmptyName()
            UpdateCustomer(True)
        End Sub

        Private Sub UpdateCustomer(ByVal replace As Boolean)
            Dim customer As New NorthwindSimpleModel.Customers()
            customer.City = String.Empty
            ctx.AddObject("Customers", customer)
            Dim options As SaveChangesOptions = SaveChangesOptions.None
            If replace Then
                options = SaveChangesOptions.ReplaceOnUpdate
            End If

            Try
                Util.SaveChanges(ctx, options)   ' null key value CustomerID
                Assert.Fail("expected DataServiceRequestException")
            Catch ex As DataServiceRequestException
                Dim innerEx As Exception = ex
                Assert.AreEqual(500, ex.Response.Single().StatusCode, "Making sure we get Internal Server Error, since that was V1/V2 behavior")
                While Not innerEx.InnerException Is Nothing
                    innerEx = innerEx.InnerException
                End While

                Assert.IsTrue(innerEx.Message.Contains("System.Data.ConstraintException"), "Error must be from the server")
                Assert.IsTrue(innerEx.Message.Contains("The 'CompanyName' property on 'Customers' could not be set to a 'null' value. You must set this property to a non-null value of type 'String'."), "Error must be from the server")
            End Try

            customer.CustomerID = "ZZZZZ"
            Try
                Util.SaveChanges(ctx, options) ' null company name
                Assert.Fail("expected WebException")
            Catch ex As DataServiceRequestException
                Assert.AreEqual(500, ex.Response.Single().StatusCode, "Making sure we get Internal Server Error, since that was V1/V2 behavior")
                Dim text = Util.PrintException(ex)
                Assert.IsTrue(text.Contains("The 'CompanyName' property on 'Customers' could not be set to a 'null' value. You must set this property to a non-null value of type 'String'."), text)
            End Try

            customer.CompanyName = "Microsoft"
            Util.SaveChanges(ctx, options)

            ctx.DeleteObject(customer)
            Util.SaveChanges(ctx, options)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub EntityInsertFailed_LinkInsertAbort()
            Dim entity As EntityDescriptor
            Dim link As LinkDescriptor

            Dim category = ctx.Categories.First()

            Dim chaiProduct As New NorthwindSimpleModel.Products()
            chaiProduct.ProductID = 999999
            ctx.AddToProducts(chaiProduct)  ' insert will fail because constraint violation

            ctx.SetLink(chaiProduct, "Categories", category)
            Dim response = Util.SaveChanges(ctx, SaveChangesOptions.ContinueOnError, 0, 1)

            Dim index = 0
            For Each changset As ChangeOperationResponse In response
                Dim descriptor = changset.Descriptor
                Select Case index
                    Case 0
                        entity = CType(descriptor, EntityDescriptor)
                        Assert.AreSame(chaiProduct, entity.Entity)
                        Assert.AreEqual(EntityStates.Added, entity.State)
                        Assert.IsNotNull(changset.Error)
                        Assert.IsInstanceOfType(changset.Error, GetType(InvalidOperationException))
                    Case 1
                        link = CType(descriptor, LinkDescriptor)
                        Assert.AreSame(chaiProduct, link.Source)
                        Assert.AreEqual("Categories", link.SourceProperty)
                        Assert.AreSame(category, link.Target)
                        Assert.AreEqual(EntityStates.Added, link.State)
                        Assert.IsNotNull(changset.Error)
                        Assert.IsTrue(changset.Error.Message.Contains("One of the link's resources failed to insert."))
                    Case Else
                        Assert.Fail("too many descriptors")
                End Select
                index += 1
            Next

            ctx.DetachLink(chaiProduct, "Categories", category)
            ctx.AddLink(category, "Products", chaiProduct)

            response = Util.SaveChanges(ctx, SaveChangesOptions.ContinueOnError, 0, 2)   ' will fail because target entity failed

            index = 0
            For Each changset As ChangeOperationResponse In response
                Dim descriptor = changset.Descriptor
                Select Case index
                    Case 0
                        entity = CType(descriptor, EntityDescriptor)
                        Assert.AreSame(chaiProduct, entity.Entity)
                        Assert.AreEqual(EntityStates.Added, entity.State)
                        Assert.IsNotNull(changset.Error)
                        Assert.IsInstanceOfType(changset.Error, GetType(InvalidOperationException))
                    Case 1
                        link = CType(descriptor, LinkDescriptor)
                        Assert.AreSame(category, link.Source)
                        Assert.AreEqual("Products", link.SourceProperty)
                        Assert.AreSame(chaiProduct, link.Target)
                        Assert.AreEqual(EntityStates.Added, link.State)
                        Assert.IsNotNull(changset.Error)
                    Case Else
                        Assert.Fail("too many descriptors")
                End Select
                index += 1
            Next
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub DeleteComplexProperty()
            Using CustomDataContext.CreateChangeScope()
                Using web = TestWebRequest.CreateForInProcessWcf
                    web.DataServiceType = ServiceModelData.CustomData.ServiceModelType
                    web.StartService()
                    Try
                        Dim customCtx As DataServiceContext = New DeleteComplexPropertyContext(web.ServiceRoot)
                        'customCtx.EnableAtom = True
                        'customCtx.Format.UseAtom()
                        Dim customer = customCtx.Execute(Of AstoriaUnitTests.Stubs.Customer)(New Uri("/Customers(0)", UriKind.Relative)).First()
                        customer.Address = Nothing
                        customCtx.UpdateObject(customer)
                        Dim response = customCtx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)
                        Assert.AreEqual(response.BatchStatusCode, 202)
                        customer = customCtx.Execute(Of AstoriaUnitTests.Stubs.Customer)(New Uri("/Customers(0)", UriKind.Relative)).First()
                        Assert.AreEqual(customer.Address, Nothing)
                    Finally
                        web.StopService()
                    End Try
                End Using
            End Using
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub UnsafeCharaceterInQueryParametersInUri()
            For Each name As String In New String() {"M+S", "M&S", "M#S", "M?S"}
                Dim key As String = name
                If key.Contains("#") Or key.Contains("?") Then
                    key = "9999"
                End If

                Dim customer = NorthwindSimpleModel.Customers.CreateCustomers(key, name)
                ctx.AddToCustomers(customer)
                ctx.SaveChanges()

                Try
                    'This causes the unsafe characters to appear in the query parameter portion of the uri
                    Dim count As Integer = Enumerable.Count(ctx.Customers.Where(Function(c) c.CompanyName = customer.CompanyName))
                    Assert.AreEqual(count, 1, "There should be one customer found with this match")

                    'This causes the unsafe characters to appear in the uri path
                    count = Enumerable.Count(ctx.Customers.Where(Function(c) c.CustomerID = customer.CustomerID))
                    Assert.AreEqual(count, 1, "There should be one customer found with this match")

                    'This is to hit the dataservice uri generation code
                    ctx.Detach(customer)
                    ctx.AttachTo("Customers", customer)
                    ctx.UpdateObject(customer)
                    ctx.SaveChanges()
                Catch
                    Assert.Fail("failed trying to filter based on " + name)
                Finally
                    'This causes the unsafe characters to appear in the uri path.
                    ctx.DeleteObject(customer)
                    ctx.SaveChanges()
                End Try
            Next
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub CollectionFoldingInV1_NonBatch()
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9999)
            ctx.AddToOrders(order)
            Util.VerifyObject(ctx, order, EntityStates.Added)

            Dim cust As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "ADO.NET Data Services")
            ctx.AddObject("Customers", cust)
            Util.VerifyObject(ctx, cust, EntityStates.Added)

            ctx.AddLink(cust, "Orders", order)
            Util.VerifyLink(ctx, cust, "Orders", order, EntityStates.Added)

            ctx.SaveChanges()
            Util.VerifyObject(ctx, cust, EntityStates.Unchanged)
            Util.VerifyObject(ctx, order, EntityStates.Unchanged)
            Util.VerifyLink(ctx, cust, "Orders", order, EntityStates.Unchanged)

            'Clean the context
            ctx.DeleteObject(order)
            ctx.DeleteObject(cust)
            ctx.SaveChanges()
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub CollectionFoldingInV1_Batch()
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9999)
            ctx.AddToOrders(order)
            Util.VerifyObject(ctx, order, EntityStates.Added)

            Dim cust As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("ASTOR", "ADO.NET Data Services")
            ctx.AddObject("Customers", cust)
            Util.VerifyObject(ctx, cust, EntityStates.Added)

            ctx.AddLink(cust, "Orders", order)
            Util.VerifyLink(ctx, cust, "Orders", order, EntityStates.Added)

            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)
            Util.VerifyObject(ctx, cust, EntityStates.Unchanged)
            Util.VerifyObject(ctx, order, EntityStates.Unchanged)
            Util.VerifyLink(ctx, cust, "Orders", order, EntityStates.Unchanged)

            'Clean the context
            ctx.DeleteObject(cust)
            ctx.DeleteObject(order)
            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)
        End Sub

#Region "Test Customers that does not auto create Orders collection"

        <Global.Microsoft.OData.Client.Key("CustomerID")>
        Private Class CustomerThatDoesNotAutoCreateOrderCollectionAndNoSetter
            Public Property CustomerID() As String
                Get
                    Return Me._CustomerID
                End Get
                Set(ByVal value As String)
                    Me._CustomerID = value
                End Set
            End Property
            Private _CustomerID As String

            Public Property CompanyName() As String
                Get
                    Return Me._CompanyName
                End Get
                Set(ByVal value As String)
                    Me._CompanyName = value
                End Set
            End Property
            Private _CompanyName As String

            Public Property ContactName() As String
                Get
                    Return Me._ContactName
                End Get
                Set(ByVal value As String)
                    Me._ContactName = value
                End Set
            End Property
            Private _ContactName As String

            Public Property ContactTitle() As String
                Get
                    Return Me._ContactTitle
                End Get
                Set(ByVal value As String)
                    Me._ContactTitle = value
                End Set
            End Property
            Private _ContactTitle As String

            Public Property Address() As String
                Get
                    Return Me._Address
                End Get
                Set(ByVal value As String)
                    Me._Address = value
                End Set
            End Property
            Private _Address As String

            Public Property City() As String
                Get
                    Return Me._City
                End Get
                Set(ByVal value As String)
                    Me._City = value
                End Set
            End Property
            Private _City As String

            Public Property Region() As String
                Get
                    Return Me._Region
                End Get
                Set(ByVal value As String)
                    Me._Region = value
                End Set
            End Property
            Private _Region As String

            Public Property PostalCode() As String
                Get
                    Return Me._PostalCode
                End Get
                Set(ByVal value As String)
                    Me._PostalCode = value
                End Set
            End Property
            Private _PostalCode As String

            Public Property Phone() As String
                Get
                    Return Me._Phone
                End Get
                Set(ByVal value As String)
                    Me._Phone = value
                End Set
            End Property
            Private _Phone As String

            Public Property Fax() As String
                Get
                    Return Me._Fax
                End Get
                Set(ByVal value As String)
                    Me._Fax = value
                End Set
            End Property
            Private _Fax As String

            Public ReadOnly Property Orders() As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.Orders)
                Get
                    Return Me._Orders
                End Get
            End Property
            Private _Orders As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.Orders)

            Public ReadOnly Property CustomerDemographics() As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.CustomerDemographics)
                Get
                    Return Me._CustomerDemographics
                End Get
            End Property
            Private _CustomerDemographics As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.CustomerDemographics) = New Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.CustomerDemographics)
        End Class

        <Global.Microsoft.OData.Client.Key("CustomerID")>
        Private Class CustomerThatDoesNotAutoCreateOrderCollection
            Public Property CustomerID() As String
                Get
                    Return Me._CustomerID
                End Get
                Set(ByVal value As String)
                    Me._CustomerID = value
                End Set
            End Property
            Private _CustomerID As String

            Public Property CompanyName() As String
                Get
                    Return Me._CompanyName
                End Get
                Set(ByVal value As String)
                    Me._CompanyName = value
                End Set
            End Property
            Private _CompanyName As String

            Public Property ContactName() As String
                Get
                    Return Me._ContactName
                End Get
                Set(ByVal value As String)
                    Me._ContactName = value
                End Set
            End Property
            Private _ContactName As String

            Public Property ContactTitle() As String
                Get
                    Return Me._ContactTitle
                End Get
                Set(ByVal value As String)
                    Me._ContactTitle = value
                End Set
            End Property
            Private _ContactTitle As String

            Public Property Address() As String
                Get
                    Return Me._Address
                End Get
                Set(ByVal value As String)
                    Me._Address = value
                End Set
            End Property
            Private _Address As String

            Public Property City() As String
                Get
                    Return Me._City
                End Get
                Set(ByVal value As String)
                    Me._City = value
                End Set
            End Property
            Private _City As String

            Public Property Region() As String
                Get
                    Return Me._Region
                End Get
                Set(ByVal value As String)
                    Me._Region = value
                End Set
            End Property
            Private _Region As String

            Public Property PostalCode() As String
                Get
                    Return Me._PostalCode
                End Get
                Set(ByVal value As String)
                    Me._PostalCode = value
                End Set
            End Property
            Private _PostalCode As String

            Public Property Phone() As String
                Get
                    Return Me._Phone
                End Get
                Set(ByVal value As String)
                    Me._Phone = value
                End Set
            End Property
            Private _Phone As String

            Public Property Fax() As String
                Get
                    Return Me._Fax
                End Get
                Set(ByVal value As String)
                    Me._Fax = value
                End Set
            End Property
            Private _Fax As String

            Public Property Orders() As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.Orders)
                Get
                    Return Me._Orders
                End Get
                Set(ByVal value As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.Orders))
                    Me._Orders = value
                End Set
            End Property
            Private _Orders As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.Orders)

            Public ReadOnly Property CustomerDemographics() As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.CustomerDemographics)
                Get
                    Return Me._CustomerDemographics
                End Get
            End Property
            Private _CustomerDemographics As Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.CustomerDemographics) = New Global.System.Collections.ObjectModel.Collection(Of NorthwindSimpleModel.CustomerDemographics)
        End Class
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        <ExpectedException(GetType(InvalidOperationException))>
        Public Sub CustomerDoesNotAutoCreateOrderCollectionAndNoSetter()
            Dim query As IEnumerable(Of CustomerThatDoesNotAutoCreateOrderCollectionAndNoSetter) = ctx.Execute(Of CustomerThatDoesNotAutoCreateOrderCollectionAndNoSetter)("Customers?$top=5&$expand=Orders")

            Dim totalOrders As Int32 = 0
            For Each customer As CustomerThatDoesNotAutoCreateOrderCollectionAndNoSetter In query
                totalOrders += customer.Orders.Count
            Next

            Assert.IsTrue(0 < totalOrders, "no orders expanded")
            Assert.AreEqual(totalOrders, Enumerable.Count(From a In ctx.Entities Where GetType(NorthwindSimpleModel.Orders).IsInstanceOfType(a.Entity) Select a))
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub CustomerDoesNotAutoCreateOrderCollection()
            Dim query As IEnumerable(Of CustomerThatDoesNotAutoCreateOrderCollection) = ctx.Execute(Of CustomerThatDoesNotAutoCreateOrderCollection)("Customers?$top=5&$expand=Orders")

            Dim totalOrders As Int32 = 0
            For Each customer As CustomerThatDoesNotAutoCreateOrderCollection In query
                totalOrders += customer.Orders.Count
            Next

            Assert.IsTrue(0 < totalOrders, "no orders expanded")
            Assert.AreEqual(totalOrders, Enumerable.Count(From a In ctx.Entities Where GetType(NorthwindSimpleModel.Orders).IsInstanceOfType(a.Entity) Select a))
        End Sub

        'Remove Atom
        '<TestMethod()>
        Public Sub ClientUpdate_11()
            Dim discontinuedProducts = From prod In ctx.Products
                                       Where True = prod.Discontinued
                                       Select prod

            For Each product As NorthwindSimpleModel.Products In discontinuedProducts
                ctx.LoadProperty(product, "Categories")
                ctx.SetLink(product, "Categories", Nothing)
            Next

            Dim entities = ctx.Entities
            Assert.IsTrue(entities.All(Function(x) x.State = EntityStates.Unchanged), "entity state")

            Dim links = ctx.Links
            Assert.IsTrue(links.All(Function(x) x.State = EntityStates.Modified), "link state")
            Assert.IsTrue(links.All(Function(x) x.Target Is Nothing), "link state")

            Util.SaveChanges(ctx, SaveChangesOptions.None, 8, 0)
            Assert.AreEqual(8, ctx.Links.Count, "post save link count")

            For Each entityDescriptor As EntityDescriptor In ctx.Entities
                ctx.DeleteObject(entityDescriptor.Entity)
            Next

            Util.SaveChanges(ctx)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub ClientUpdate_12()
            Dim customer = NorthwindSimpleModel.Customers.CreateCustomers("ABCDE", "Microsoft")
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9999)
            order.Customers = customer

            ctx.AddObject("Customers", customer)
            ctx.AddObject("Orders", order)
            ctx.SetLink(order, "Customers", customer)

            ctx.SaveChanges()

            ctx.DeleteObject(order)

            Dim entities = ctx.Entities
            Dim links = ctx.Links

            Assert.AreEqual(2, entities.Count)
            Assert.AreEqual(EntityStates.Unchanged, entities.Item(0).State)
            Assert.AreEqual(EntityStates.Deleted, entities.Item(1).State)

            Assert.AreEqual(1, links.Count)

            Assert.AreEqual(EntityStates.Unchanged, links.Item(0).State)

            ctx.SaveChanges()

            entities = ctx.Entities
            links = ctx.Links

            Assert.AreEqual(1, entities.Count)
            Assert.AreEqual(EntityStates.Unchanged, entities.Item(0).State)

            Assert.AreEqual(0, links.Count)
        End Sub
#End Region

#Region "Test ThreadAbort in background thread"
        Dim uncatchableCallbackCount As Int32 = 0
        Dim uncatchableCallbackType As Type = Nothing
        Dim uncatchableCallbackFail As Boolean = False
        Dim uncatchableCallbackPassCount As Int32 = 0

        ' [markash] This test is disabled.
        ' The uncatchable exceptions are caught (but rethrown) and detected via the End* call
        ' However, the rethrown background thread is causing issues as an unhandled exception.
        ' <TestMethod()> _
        Public Sub VerifyUncatchableInSendingRequest()
            AddHandler ctx.SendingRequest2, AddressOf ThrowUncatchableInSendingRequest

            ctx.AddToCustomers(NorthwindSimpleModel.Customers.CreateCustomers("IGNOR", "QWERTY"))
            ctx.AddToCustomers(NorthwindSimpleModel.Customers.CreateCustomers("ABORT", "QWERTY"))

            Dim exceptionList = New Type() {GetType(ThreadAbortException), GetType(StackOverflowException), GetType(OutOfMemoryException), GetType(NullReferenceException), GetType(Exception)}

            For Each type As Type In exceptionList
                Me.uncatchableCallbackCount = 0
                Me.uncatchableCallbackType = type
                Dim result = ctx.BeginSaveChanges(SaveChangesOptions.ReplaceOnUpdate, AddressOf VerifyUncatchableInSendingRequestCallback, type)
                Assert.IsTrue(result.AsyncWaitHandle.WaitOne(New TimeSpan(0, 1, 0), False), "waithandle never signaled, expecting {0}", type.Name)
                Assert.IsTrue(result.IsCompleted, "expected Completeness for {0}", type.Name)
                Try
                    ctx.EndSaveChanges(result)
                    Assert.Fail("expected an {0}", type.Name)
                Catch ex As Exception
                    Assert.IsInstanceOfType(ex.InnerException, type, "expecting InnerException, not {0}", ex)
                End Try

                ' problem is callback's may pile up and never run until after test completes
                Assert.IsFalse(Me.uncatchableCallbackFail, "callback not expected with ThreadAbortException")
            Next

            For Each type As Type In exceptionList
                Me.uncatchableCallbackCount = 0
                Me.uncatchableCallbackType = type
                Dim result = ctx.BeginSaveChanges(SaveChangesOptions.ReplaceOnUpdate, AddressOf VerifyUncatchableInSendingRequestCallback, type)
                Dim start = DateTime.Now
                While Not result.IsCompleted
                    Thread.Sleep(90)
                    Assert.IsTrue(DateTime.Now - start < New TimeSpan(0, 1, 0), "IsCompleted never set, expecting {0}", type.Name)
                End While
                Assert.IsTrue(result.AsyncWaitHandle.WaitOne(10, False), "WaitHandle should be signaled soon after IsCompleted")
                Try
                    ctx.EndSaveChanges(result)
                    Assert.Fail("expected an {0}", type.Name)
                Catch ex As Exception
                    Assert.IsInstanceOfType(ex.InnerException, type, "expecting InnerException, not {0}", ex)
                End Try

                ' problem is callback's may pile up and never run until after test completes
                Assert.IsFalse(Me.uncatchableCallbackFail, "callback not expected with ThreadAbortException")
            Next

            Assert.IsFalse(Me.uncatchableCallbackFail, "callback not expected with ThreadAbortException")

            ' problem is callback's may pile up and never run until after test completes
            Assert.AreEqual((exceptionList.Length - 2) * 2, Me.uncatchableCallbackPassCount, "not all the callbacks compeleted")
        End Sub

        Private Sub ThrowUncatchableInSendingRequest(ByVal sender As Object, ByVal args As SendingRequest2EventArgs)
            uncatchableCallbackCount += 1
            If (2 = uncatchableCallbackCount) Then ' second callback because first may still be on main thread
                If (GetType(ThreadAbortException) Is Me.uncatchableCallbackType) Then
                    Thread.CurrentThread.Abort()
                Else
                    Dim value As Exception = CType(Activator.CreateInstance(Me.uncatchableCallbackType), Exception)
                    Throw value
                End If
            End If
        End Sub

        Private Sub VerifyUncatchableInSendingRequestCallback(ByVal result As IAsyncResult)
            If GetType(ThreadAbortException).IsInstanceOfType(result.AsyncState) Or
               GetType(StackOverflowException).IsInstanceOfType(result.AsyncState) Then
                Me.uncatchableCallbackFail = True
            Else
                Interlocked.Increment(Me.uncatchableCallbackPassCount)
            End If
        End Sub
#End Region

        'Writing tests for POST cases with no payloads
        <TestCategory("Partition1")> <TestMethod()> Public Sub POSTWithNoResponsePayload()
            Using web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf

                web1.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
                web1.StartService()

                Dim engine = CombinatorialEngine.FromDimensions(
                    New Dimension("ExecutionMethod", Util.ExecutionMethods),
                    New Dimension("WebRequest", New TestWebRequest() {web1}),
                    New Dimension("SaveChangesOption", New SaveChangesOptions() {SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.ContinueOnError, SaveChangesOptions.None, SaveChangesOptions.ReplaceOnUpdate}))

                TestUtil.RunCombinatorialEngineFail(engine, AddressOf POSTWithNoResponsePayload_Inner)
            End Using
        End Sub

        Private Sub POSTWithNoResponsePayload_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim web1 = CType(values("WebRequest"), TestWebRequest)
            Dim saveChangesOption = CType(values("SaveChangesOption"), SaveChangesOptions)

            Dim context As DataServiceContext = New DataServiceContext(web1.ServiceRoot)
            Dim location = web1.ServiceRoot.OriginalString & "/foo.svc/location/Customers(1)"
            Dim etag = "'Foo'"

            PopulateContextWithCustomer(context, location, etag, saveChangesOption, executionMethod)

            Assert.AreEqual(1, context.Entities.Count, "context should have exactly one entity")
            Dim descriptor = context.Entities(0)
            Assert.AreEqual(descriptor.Identity, location, "identity must match")
            Assert.AreEqual(descriptor.EditLink, location, "edit link must match")
            Assert.AreEqual(descriptor.ETag, etag, "etag must match")
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub POSTWithNoResponsePayload_AttachLocationEscapedID()
            Dim web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf
            web1.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
            web1.StartService()

            Dim context As DataServiceContext = New DataServiceContext(web1.ServiceRoot)
            Dim location = web1.ServiceRoot.OriginalString & "/foo.svc/location/Customers(key%3avalue)"
            Dim etag = "'Foo'"

            Try
                PopulateContextWithCustomer(context, location, etag, SaveChangesOptions.None, Util.ExecutionMethod.Synchronous)

                Assert.AreEqual(1, context.Entities.Count, "context should have exactly one entity")
                Dim descriptor = context.Entities(0)
                Assert.AreEqual(descriptor.Identity, location, "identity must match")
                Assert.AreEqual(descriptor.EditLink, location, "edit link must match")
                Assert.AreEqual(descriptor.ETag, etag, "etag must match")
            Finally
                web1.StopService()
            End Try
        End Sub

        Private Sub PopulateContextWithCustomer(ByVal context As DataServiceContext, ByVal location As String, ByVal etag As String, ByVal saveChangesOptions As SaveChangesOptions, ByVal executionMethod As Util.ExecutionMethod, Optional ByVal identity As String = Nothing)
            Using PlaybackService.OverridingPlayback.Restore
                If identity Is Nothing Then
                    identity = location
                End If

                Dim payload As String =
                       "HTTP/1.1 201 Created" & vbCrLf &
                       "Content-Type: application/atom+xml" & vbCrLf &
                       "Content-ID: 1" & vbCrLf &
                       "ETag: " & etag & vbCrLf &
                       "Location: " & location & vbCrLf &
                       "OData-EntityId: " & identity &
                       vbCrLf

                If (saveChangesOptions = Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset) Then
                    PlaybackService.OverridingPlayback.Value = SelfEditLinkTests.ConvertToBatchPayload(payload, False)
                Else
                    PlaybackService.OverridingPlayback.Value = payload
                End If

                Dim result = New Customer()
                context.AddObject("Customers", result)
                Util.SaveChanges(context, saveChangesOptions, executionMethod)
            End Using
        End Sub

        Class PreferHeader_StreamItem
            Public Property ID As Integer
        End Class

        ' Verifies that Prefer headers and PATCH are not used for non-entity operations and for non-update operations
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub PreferHeader_Patch_NonEntityOrNonUpdateOperations()
            Dim service = New OpenWebDataServiceDefinition
            service.DataServiceType = GetType(CustomDataContext)
            service.ProcessingPipeline.ProcessingRequest =
                Sub(sender, args)
                    Assert.IsNull(args.OperationContext.RequestHeaders("Prefer"), "No Prefer header should have been sent.")
                    Assert.AreNotEqual("PATCH", args.OperationContext.RequestMethod, "The request HTTP method must not be PATCH.")
                End Sub

            Dim dataServiceResponsePreferences = New DataServiceResponsePreference() {
                DataServiceResponsePreference.None,
                DataServiceResponsePreference.IncludeContent,
                DataServiceResponsePreference.NoContent}

            TestUtil.RunCombinations(
                dataServiceResponsePreferences,
                New SaveChangesOptions() {SaveChangesOptions.None, SaveChangesOptions.BatchWithSingleChangeset},
                New SaveChangesOptions() {SaveChangesOptions.None},
                Util.ExecutionMethods,
                Sub(responsePreference, batchOptions, patchOptions, executionMethod)
                    Using CustomDataContext.CreateChangeScope()
                        Using request = service.CreateForInProcessWcf
                            request.StartService()

                            Dim saveChangesOptions = batchOptions Or patchOptions
                            Dim context = New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
                            'context.EnableAtom = True
                            'context.Format.UseAtom()
                            context.AddAndUpdateResponsePreference = responsePreference

                            ' Simple GET Execute
                            Dim customers = context.Execute(Of Customer)("/Customers").ToList()

                            ' Simple GET LINQ
                            Dim orders = context.CreateQuery(Of Order)("Orders").AsEnumerable().ToList()

                            ' DELETE
                            context.DeleteObject(customers(1))
                            Util.SaveChanges(context, saveChangesOptions, executionMethod)

                            ' Load property
                            Dim customer = context.CreateQuery(Of Customer)("Customers").First()
                            context.LoadProperty(customer, "Orders")

                            ' $ref CUD - PUT
                            context.SetLink(customer.Orders(0), "Customer", customer)

                            ' $ref CUD - POST
                            context.AddLink(customer, "Orders", (From o In orders Where Not customer.Orders.Contains(o)).First())
                            Util.SaveChanges(context, saveChangesOptions, executionMethod)

                            ' $ref CUD - DELETE
                            context.DeleteLink(customer, "Orders", customer.Orders(1))
                            Util.SaveChanges(context, saveChangesOptions, executionMethod)
                        End Using
                    End Using
                End Sub)

            Dim mrService = CreateServiceWithDefaultAndNamedStreams()
            mrService.ProcessingPipeline.ProcessingRequest =
                Sub(sender, args)
                    Assert.IsNull(args.OperationContext.RequestHeaders("Prefer"), "No Prefer header should have been sent.")
                    Assert.AreNotEqual("PATCH", args.OperationContext.RequestMethod, "The request HTTP method must not be PATCH.")
                End Sub

            Using CustomDataContext.CreateChangeScope()
                Using request = mrService.CreateForInProcessWcf
                    request.StartService()

                    TestUtil.RunCombinations(
                        dataServiceResponsePreferences,
                        New SaveChangesOptions() {SaveChangesOptions.None},
                        Util.ExecutionMethods,
                        Sub(responsePreference, saveOption, executionMethod)
                            mrService.ClearChanges()
                            Dim context = New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
                            'context.EnableAtom = True
                            'context.Format.UseAtom()
                            context.AddAndUpdateResponsePreference = responsePreference

                            ' MR and Named streams are not supported in the $batch, so no need to try it here

                            Dim item = context.CreateQuery(Of PreferHeader_StreamItem)("/Items").First()

                            Dim args = New DataServiceRequestArgs
                            args.Slug = "1"
                            args.ContentType = UnitTestsUtil.MimeTextPlain

                            ' GET MR
                            Using s = context.GetReadStream(item)
                            End Using

                            ' PUT MR
                            context.SetSaveStream(item, New MemoryStream(), False, args)
                            Util.SaveChanges(context, saveOption, executionMethod)

                            ' GET Named Stream
                            Using s = context.GetReadStream(item, "NamedStream", New DataServiceRequestArgs())
                            End Using

                            ' PUT Named Stream
                            context.SetSaveStream(item, "NamedStream", New MemoryStream(), False, args)
                            Util.SaveChanges(context, saveOption, executionMethod)
                        End Sub)
                End Using
            End Using
        End Sub

        Private Function CreateServiceWithDefaultAndNamedStreams() As DSPServiceDefinition
            Dim metadata = New DSPMetadata("Test", "TestNS")
            Dim entityType = metadata.AddEntityType("PreferHeader_StreamItem", Nothing, Nothing, False)
            metadata.AddKeyProperty(entityType, "ID", GetType(Integer))
            entityType.IsMediaLinkEntry = True
            entityType.AddProperty(New Microsoft.OData.Service.Providers.ResourceProperty("NamedStream", Microsoft.OData.Service.Providers.ResourcePropertyKind.Stream, Microsoft.OData.Service.Providers.ResourceType.GetPrimitiveResourceType(GetType(System.IO.Stream))))
            metadata.AddResourceSet("Items", entityType)
            Dim mrStorage = New DSPMediaResourceStorage()
            Dim mrService = New DSPServiceDefinition()
            mrService.Metadata = metadata
            mrService.SupportMediaResource = True
            mrService.SupportNamedStream = True
            mrService.MediaResourceStorage = mrStorage
            mrService.Writable = True
            mrService.HostInterfaceType = GetType(Microsoft.OData.Service.IDataServiceHost2)
            mrService.CreateDataSource =
                Function(m)
                    Dim context = New DSPContext()
                    Dim item = New DSPResource(m.GetResourceType("PreferHeader_StreamItem"))
                    item.SetRawValue("ID", 0)
                    Dim defaultStream = mrStorage.CreateMediaResource(item, Nothing)
                    defaultStream.ContentType = UnitTestsUtil.MimeTextPlain
                    defaultStream.GetWriteStream().WriteByte(60)

                    Dim namedStream = mrStorage.CreateMediaResource(item, item.ResourceType.GetNamedStreams().First())
                    namedStream.ContentType = UnitTestsUtil.MimeTextPlain
                    namedStream.GetWriteStream().WriteByte(61)
                    context.GetResourceSetEntities("Items").Add(item)
                    Return context
                End Function
            Return mrService
        End Function

        Private Function GetTestEntityPayload(ByVal identity As String, ByVal editLink As String, ByVal ParamArray properties As KeyValuePair(Of String, String)()) As XElement
            Dim editLinkXml = Nothing
            If Not editLink Is Nothing Then
                editLinkXml = <link rel="edit" href=<%= editLink %> xmlns='http://www.w3.org/2005/Atom'/>
            End If

            Dim idXml = Nothing
            If Not identity Is Nothing Then
                idXml = <id xmlns='http://www.w3.org/2005/Atom'><%= identity %></id>
            End If
            Return <entry xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
                       <title/>
                       <author>
                           <name/>
                       </author>
                       <updated>2010-07-09T13:49:08.6037015Z</updated>
                       <%= idXml %>
                       <%= editLinkXml %>
                       <content type='application/xml'>
                           <m:properties>
                               <%= From p In properties Select New XElement(UnitTestsUtil.DataNamespace + p.Key, p.Value) %>
                           </m:properties>
                       </content>
                   </entry>
        End Function

        Public Class EntityEventHandlerCallcounts
            Public Property ReadingEntityCallcount As Integer
            Public Property WritingEntityCallcount As Integer
        End Class
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub IdentityChangeOnUpdate()
            Dim service = New PlaybackServiceDefinition
            Using request = service.CreateForInProcessWcf
                request.StartService()

                TestUtil.RunCombinations(
                    New SaveChangesOptions() {SaveChangesOptions.None, SaveChangesOptions.ReplaceOnUpdate},
                    UnitTestsUtil.BooleanValues,
                    UnitTestsUtil.BooleanValues,
                    UnitTestsUtil.BooleanValues,
                    New ServiceVersion() {Nothing, 40},
                    Util.ExecutionMethods,
                    Sub(saveChangesOptions, includePayload, includeDataServiceId, useBatch, responseVersion, executionMethod)
                        Me.IdentityChangeOnUpdate_Inner(service, request, saveChangesOptions, includePayload, includeDataServiceId, useBatch, responseVersion, executionMethod)
                    End Sub)
            End Using
        End Sub

        Private Sub IdentityChangeOnUpdate_Inner(ByVal service As PlaybackServiceDefinition, ByVal request As TestWebRequest, ByVal saveChangesOptions As SaveChangesOptions, ByVal includePayload As Boolean, ByVal useUpsert As Boolean, ByVal useBatch As Boolean, ByVal responseVersion As ServiceVersion, ByVal executionMethod As Util.ExecutionMethod)
            ' GET an entity with old identity and edit link
            Dim oldIdentity = New Uri("urn:old-identity")
            Dim context = New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            Dim oldEditLink = New Uri(context.BaseUri.ToString() & "/oldentity1")
            service.ProcessRequestOverride =
                Function(clientRequest)
                    clientRequest.SetResponseStatusCode(200)
                    clientRequest.ResponseHeaders("Content-Type") = UnitTestsUtil.AtomFormat
                    clientRequest.SetResponseStreamAsText(GetTestEntityPayload(oldIdentity.ToString(), oldEditLink.ToString()).ToString())
                    Return clientRequest
                End Function
            Dim entity = context.Execute(Of Customer)("/Customers").First()
            Dim entityDescriptor = context.GetEntityDescriptor(entity)
            Assert.AreEqual(oldIdentity, entityDescriptor.Identity, "The identity didn't propagate correctly from GET.")
            Assert.AreEqual(oldEditLink.ToString(), entityDescriptor.EditLink.ToString(), "The edit link didn't propagate correctly from GET.")

            ' Update the entity from the client with a response which changes the identity and edit link
            Dim newIdentity = New Uri("urn:new-identity")
            Dim newEditLink = New Uri(context.BaseUri.ToString() & "/newentity1")
            Dim newLocationEditLink = New Uri(context.BaseUri.AbsoluteUri & "/location/newentity1")
            service.ProcessRequestOverride =
                Function(clientRequest)
                    If Not responseVersion Is Nothing Then
                        clientRequest.ResponseHeaders("OData-Version") = responseVersion.ToString() & ";"
                    End If

                    If includePayload Then
                        clientRequest.SetResponseStatusCode(200)
                        clientRequest.ResponseHeaders("Content-Type") = UnitTestsUtil.AtomFormat
                        clientRequest.SetResponseStreamAsText(GetTestEntityPayload(newIdentity.ToString(), newEditLink.ToString()).ToString())
                    Else
                        If useUpsert Then
                            clientRequest.ResponseHeaders("OData-EntityId") = newIdentity.AbsoluteUri
                            clientRequest.ResponseHeaders("Location") = newLocationEditLink.AbsoluteUri
                        End If
                        clientRequest.SetResponseStatusCode(204)
                    End If
                    Return clientRequest
                End Function
            If useBatch Then
                saveChangesOptions = Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset Or saveChangesOptions
                service.ProcessRequestOverride = PlaybackServiceDefinition.UnwrapSingleBatchPart(service.ProcessRequestOverride)
            End If
            context.UpdateObject(entity)
            Util.SaveChanges(context, saveChangesOptions, executionMethod)
            Dim newEntityDescriptor = context.GetEntityDescriptor(entity)
            Assert.IsTrue(Object.ReferenceEquals(newEntityDescriptor, entityDescriptor), "The entity got a new entity descriptor instance, this is wrong.")
            If includePayload Or (useUpsert And (saveChangesOptions And Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate) <> Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate) Then
                Assert.AreEqual(newIdentity, entityDescriptor.Identity, "The identity didn't update correctly from update operation response.")
            Else
                Assert.AreEqual(oldIdentity, entityDescriptor.Identity, "The identity should have remained as we didn't send a new one in the response.")
            End If
            If includePayload Then
                Assert.AreEqual(newEditLink.ToString(), entityDescriptor.EditLink.ToString(), "The edit link didn't propagate correctly from update operation response.")
            ElseIf (useUpsert And (saveChangesOptions And Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate) <> Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate) Then
                Assert.AreEqual(newLocationEditLink.AbsoluteUri, entityDescriptor.EditLink.ToString(), "The edit link didn't propagate correctly from update operation response.")
            Else
                Assert.AreEqual(oldEditLink.ToString(), entityDescriptor.EditLink.ToString(), "The edit link should have remained as we didn't send a new one in the response.")
            End If

            ' Using the same response try a GET operation to see that we don't get a different entity instance
            service.ProcessRequestOverride =
                Function(clientRequest)
                    clientRequest.SetResponseStatusCode(200)
                    clientRequest.ResponseHeaders("Content-Type") = UnitTestsUtil.AtomFormat
                    clientRequest.SetResponseStreamAsText(GetTestEntityPayload(entityDescriptor.Identity.ToString(), entityDescriptor.EditLink.ToString()).ToString())
                    Return clientRequest
                End Function
            Dim newEntity = context.Execute(Of Customer)("/Customers").First()
            Assert.IsTrue(Object.ReferenceEquals(newEntity, entity), "GET materializes the same entity as a different instance, tracking is broken.")
        End Sub

        ' Testing if there is a value in the payload, that always wins
        ' If the value is not present in the payload, then header value wins
        ' If no
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub Payload_Header_Identity_POST()
            Dim service = New PlaybackServiceDefinition
            Using request = service.CreateForInProcessWcf
                request.StartService()

                TestUtil.RunCombinations(
                    DirectCast(System.Enum.GetValues(GetType(SaveChangesOptions)), IEnumerable(Of SaveChangesOptions)),
                    New Integer() {0, 1, 2, 3},
                    New ServiceVersion() {Nothing, 40},
                    Util.ExecutionMethods,
                    New String() {Nothing, "", "foo", request.ServiceRoot.AbsoluteUri & "/location/Customers(1)"},
                    New String() {Nothing, "urn:myid1"},
                    Sub(saveChangesOptions, payloadOptions, responseVersion, executionMethod, location, dataserviceid)
                        Me.Payload_Header_Identity_POST_Inner(service, request, saveChangesOptions, DirectCast(payloadOptions, PayloadOptions), responseVersion, executionMethod, location, dataserviceid)
                    End Sub)
            End Using
        End Sub

        Private Sub Payload_Header_Identity_POST_Inner(
                                                      ByVal service As PlaybackServiceDefinition,
                                                      ByVal request As TestWebRequest,
                                                      ByVal saveChangesOptions As SaveChangesOptions,
                                                      ByVal payloadOptions As PayloadOptions,
                                                      ByVal responseVersion As ServiceVersion,
                                                      ByVal executionMethod As Util.ExecutionMethod,
                                                      ByVal location As String,
                                                      ByVal dataserviceid As String)

            Dim identityInPayload = If((payloadOptions And ClientUpdateTests.PayloadOptions.PayloadWithIdentity) = ClientUpdateTests.PayloadOptions.PayloadWithIdentity, "urn:identity", Nothing)
            Dim editLinkInPayload = If((payloadOptions And ClientUpdateTests.PayloadOptions.PayloadWithEditLink) = ClientUpdateTests.PayloadOptions.PayloadWithEditLink, request.ServiceRoot.AbsoluteUri + "/editLinkInPayload/Customers(1)", Nothing)

            Dim context = New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True

            service.ProcessRequestOverride =
                Function(clientRequest)
                    If Not responseVersion Is Nothing Then
                        clientRequest.ResponseHeaders("OData-Version") = responseVersion.ToString() & ";"
                    End If

                    clientRequest.SetResponseStatusCode(200)
                    If payloadOptions <> ClientUpdateTests.PayloadOptions.NoPayload Then
                        clientRequest.ResponseHeaders("Content-Type") = UnitTestsUtil.AtomFormat
                        clientRequest.SetResponseStreamAsText(GetTestEntityPayload(identityInPayload, editLinkInPayload).ToString())
                    End If

                    If Not location Is Nothing Then
                        clientRequest.ResponseHeaders("Location") = location
                    End If

                    If Not dataserviceid Is Nothing Then
                        clientRequest.ResponseHeaders("OData-EntityId") = dataserviceid
                    End If

                    Return clientRequest
                End Function
            If (IsSaveChangesFlagSet(saveChangesOptions, Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset) Or IsSaveChangesFlagSet(saveChangesOptions, Microsoft.OData.Client.SaveChangesOptions.BatchWithIndependentOperations)) Then
                service.ProcessRequestOverride = PlaybackServiceDefinition.UnwrapSingleBatchPart(service.ProcessRequestOverride)
            End If

            Dim entity = New Customer
            entity.ID = 1
            context.AddObject("Customers", entity)
            Dim exception As Exception = TestUtil.RunCatching(Sub()
                                                                  Util.SaveChanges(context, saveChangesOptions, executionMethod)
                                                              End Sub)

            If (Not exception Is Nothing) Then
                Dim exceptionMsg As String
                If Not exception.InnerException Is Nothing Then
                    exceptionMsg = exception.InnerException.Message
                Else
                    exceptionMsg = exception.Message
                End If

                If saveChangesOptions = Microsoft.OData.Client.SaveChangesOptions.PostOnlySetProperties Then
                    Assert.AreEqual(exceptionMsg, String.Format(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Context_MustBeUsedWith"), "SaveChangesOptions.OnlyPostExplicitProperties", "DataServiceCollection"))
                ElseIf location <> request.ServiceRoot.AbsoluteUri & "/location/Customers(1)" Then
                    'In non batch case, "" gets converted to null value. But in non-batch case, the value is preserved.
                    If ((Not (saveChangesOptions = Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset) And Not (saveChangesOptions = Microsoft.OData.Client.SaveChangesOptions.BatchWithIndependentOperations) And location = "") Or location Is Nothing) Then
                        Assert.AreEqual(exceptionMsg, AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Deserialize_NoLocationHeader"), "location header must be specified for POST")
                    Else
                        Assert.AreEqual(exceptionMsg, AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Context_LocationHeaderExpectsAbsoluteUri"), "location header is not valid")
                    End If
                ElseIf Not dataserviceid Is Nothing And dataserviceid <> "urn:myid1" Then
                    Assert.AreEqual(exceptionMsg, AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Context_TrackingExpectsAbsoluteUri"), "OData-EntityId header value is invalid")
                ElseIf identityInPayload Is Nothing Then
                    Assert.AreEqual(exceptionMsg, AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Deserialize_MissingIdElement"), "missing id element in the payload")
                Else
                    Assert.Fail("Unexpected exception encountered")
                End If

                Return
            End If

            If (Not (saveChangesOptions = Microsoft.OData.Client.SaveChangesOptions.BatchWithIndependentOperations)) Then
                Dim entityDescriptor = context.GetEntityDescriptor(entity)
                Assert.AreEqual(If(identityInPayload, If(dataserviceid, location)), entityDescriptor.Identity.AbsoluteUri, "The identity didn't update correctly from update operation response.")

                If Not entityDescriptor.EditLink Is Nothing Then
                    Assert.AreEqual(If(editLinkInPayload, location), entityDescriptor.EditLink.AbsoluteUri, "The editlink must be the location header")
                End If
            End If
        End Sub

        ' Testing if there is a value in the payload, that always wins
        ' If the value is not present in the payload, then header value wins
        ' If no
        'Remove Atom
        ' <TestCategory("Partition1")> <TestMethod()>
        Public Sub Payload_Header_Identity_Update()
            Dim service = New PlaybackServiceDefinition
            Using request = service.CreateForInProcessWcf
                request.StartService()

                TestUtil.RunCombinations(
                    New SaveChangesOptions() {
                        SaveChangesOptions.ContinueOnError,
                        SaveChangesOptions.None,
                        SaveChangesOptions.BatchWithSingleChangeset,
                        SaveChangesOptions.ReplaceOnUpdate,
                        SaveChangesOptions.BatchWithSingleChangeset Or SaveChangesOptions.ReplaceOnUpdate},
                    New Integer() {0, 1, 2, 3},
                    New ServiceVersion() {Nothing, 40},
                    Util.ExecutionMethods,
                    New String() {Nothing, "", "foo", request.ServiceRoot.AbsoluteUri & "/location/Customers(1)"},
                    New String() {Nothing, "urn:myid1"},
                    Sub(saveChangesOptions, payloadOptions, responseVersion, executionMethod, location, dataserviceid)
                        Me.Payload_Header_Identity_Update_Inner(service, request, saveChangesOptions, DirectCast(payloadOptions, PayloadOptions), responseVersion, executionMethod, location, dataserviceid)
                    End Sub)
            End Using
        End Sub

        Private Sub Payload_Header_Identity_Update_Inner(
                                                      ByVal service As PlaybackServiceDefinition,
                                                      ByVal request As TestWebRequest,
                                                      ByVal saveChangesOptions As SaveChangesOptions,
                                                      ByVal payloadOptions As PayloadOptions,
                                                      ByVal responseVersion As ServiceVersion,
                                                      ByVal executionMethod As Util.ExecutionMethod,
                                                      ByVal location As String,
                                                      ByVal dataserviceid As String)

            Dim identityInPayload = If((payloadOptions And ClientUpdateTests.PayloadOptions.PayloadWithIdentity) = ClientUpdateTests.PayloadOptions.PayloadWithIdentity, "urn:identity", Nothing)
            Dim editLinkInPayload = If((payloadOptions And ClientUpdateTests.PayloadOptions.PayloadWithEditLink) = ClientUpdateTests.PayloadOptions.PayloadWithEditLink, request.ServiceRoot.AbsoluteUri + "/editLinkInPayload/Customers(1)", Nothing)

            Dim context = New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True

            service.ProcessRequestOverride =
                Function(clientRequest)
                    If Not responseVersion Is Nothing Then
                        clientRequest.ResponseHeaders("OData-Version") = responseVersion.ToString() & ";"
                    End If

                    clientRequest.SetResponseStatusCode(200)
                    If payloadOptions <> ClientUpdateTests.PayloadOptions.NoPayload Then
                        clientRequest.ResponseHeaders("Content-Type") = UnitTestsUtil.AtomFormat
                        clientRequest.SetResponseStreamAsText(GetTestEntityPayload(identityInPayload, editLinkInPayload).ToString())
                    End If

                    If Not location Is Nothing Then
                        clientRequest.ResponseHeaders("Location") = location
                    End If

                    If Not dataserviceid Is Nothing Then
                        clientRequest.ResponseHeaders("OData-EntityId") = dataserviceid
                    End If

                    Return clientRequest
                End Function
            If IsSaveChangesFlagSet(saveChangesOptions, Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset) Then
                service.ProcessRequestOverride = PlaybackServiceDefinition.UnwrapSingleBatchPart(service.ProcessRequestOverride)
            End If

            Dim entity = New Customer
            entity.ID = 1
            context.AttachTo("Customer", entity)
            Dim originalEditLink = context.Entities(0).EditLink.AbsoluteUri
            Dim originalIdentity = context.Entities(0).Identity.AbsoluteUri
            context.UpdateObject(entity)
            Dim exception As Exception = TestUtil.RunCatching(Sub()
                                                                  Util.SaveChanges(context, saveChangesOptions, executionMethod)
                                                              End Sub)

            ' In non-batch scenarios, the "" string header gets converted to null header
            If Not IsSaveChangesFlagSet(saveChangesOptions, Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate) Then
                If Not IsSaveChangesFlagSet(saveChangesOptions, Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset) Then
                    If location = "" Then
                        location = Nothing
                    End If
                End If
            Else
                ' In non patch scenarios, location is ignored.
                location = Nothing
                dataserviceid = Nothing
            End If

            If (Not exception Is Nothing) Then
                Dim exceptionMsg As String
                If Not exception.InnerException Is Nothing Then
                    exceptionMsg = exception.InnerException.Message
                Else
                    exceptionMsg = exception.Message
                End If

                'In non patch scenarios, location and dataserviceid headers are ignored
                If Not (IsSaveChangesFlagSet(saveChangesOptions, Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate)) Then
                    If location <> request.ServiceRoot.AbsoluteUri & "/location/Customers(1)" And Not location Is Nothing Then
                        Assert.AreEqual(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Context_LocationHeaderExpectsAbsoluteUri"), exceptionMsg, "location header is not valid")
                        Return
                    ElseIf location Is Nothing And Not dataserviceid Is Nothing Then
                        Assert.AreEqual(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Context_BothLocationAndIdMustBeSpecified"), exceptionMsg, "Only OData-EntityId cannot be specified. Both location and OData-EntityId must be specified")
                        Return
                    ElseIf Not dataserviceid Is Nothing And dataserviceid <> "urn:myid1" Then
                        Assert.AreEqual(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Context_TrackingExpectsAbsoluteUri"), exceptionMsg, "OData-EntityId header value is invalid")
                        Return
                    End If
                End If

                If identityInPayload Is Nothing Then
                    Assert.AreEqual(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Deserialize_MissingIdElement"), exceptionMsg, "missing id element in the payload")
                    Return
                End If

                Assert.Fail("Unexpected exception encountered")
            End If

            Dim entityDescriptor = context.GetEntityDescriptor(entity)
            Assert.AreEqual(If(identityInPayload, If(dataserviceid, If(location, originalIdentity))), entityDescriptor.Identity.AbsoluteUri, "The identity didn't update correctly from update operation response.")
            Assert.AreEqual(If(editLinkInPayload, If(location, originalEditLink)), entityDescriptor.EditLink.AbsoluteUri, "The editlink must be the location header")
        End Sub

        Private Function IsSaveChangesFlagSet(ByVal saveChangesOptions As Microsoft.OData.Client.SaveChangesOptions, ByVal flag As Microsoft.OData.Client.SaveChangesOptions) As Boolean
            Return ((saveChangesOptions And flag) = flag)
        End Function

        <Flags()> Private Enum PayloadOptions
            NoPayload = 0
            PayloadWithIdentity = 1
            PayloadWithEditLink = 2
        End Enum
    End Class

    <Global.Microsoft.OData.Client.Key("RegionID")>
    Public Class ClientRegion

        Private _id As Integer

        Public Property RegionID() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property


        Private _description As String
        Public Property RegionDescription() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value.Trim()
            End Set
        End Property
    End Class

End Class

Public Class DeleteComplexPropertyContext
    Inherits DataServiceContext
    Public Sub New(ByVal serviceRoot As Global.System.Uri)
        MyBase.New(serviceRoot)
        Me.ResolveName = AddressOf Me.NameResolver
    End Sub

    Private Function NameResolver(ByVal clientType As Type) As String
        Return clientType.FullName
    End Function
End Class
