'---------------------------------------------------------------------
' <copyright file="MaterializationTest.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports System.Xml.Linq
Imports AstoriaUnitTests.ClientExtensions
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NorthwindModel
Imports AstoriaUnitTests.Tests

Partial Public Class ClientModule
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    <TestClass()>
    Public Class MaterializeUnitTest
        Private Shared web As TestWebRequest = Nothing
        Private Shared random As New Random(4232008)
        Private ctx As NorthwindSimpleModel.NorthwindContext = Nothing

#Region "Initialize DataService and create new context for each test"
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
            Me.ctx = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub
#End Region

        <EntityType()>
        Public Class NarrowOrder
            Private m_Foo As String
            Private m_employee As NarrowEmployee

            Public Property ShipCity() As String
                Get
                    Return m_Foo
                End Get
                Set(ByVal value As String)
                    m_Foo = value
                End Set
            End Property

            Public Property Employees() As NarrowEmployee
                Get
                    Return m_employee
                End Get
                Set(ByVal value As NarrowEmployee)
                    m_employee = value
                End Set
            End Property
        End Class

        Public Class NarrowEmployee
            Private m_Name As String

            Public Property Name() As String
                Get
                    Return m_Name
                End Get
                Set(ByVal value As String)
                    m_Name = value
                End Set
            End Property
        End Class

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub AssertWhenProjectingEntityTypeIntoComplex()
            Dim q = From o In ctx.CreateQuery(Of NorthwindSimpleModel.Orders)("Orders")
                    Select New NarrowOrder() With {.ShipCity = o.ShipCity,
                                                                  .Employees = New NarrowEmployee() With {.Name = o.Employees.FirstName}}
            Dim nse As NotSupportedException = Nothing
            Try
                Dim uri = q.FirstOrDefault()
            Catch e As NotSupportedException
                nse = e
            End Try
            TestUtil.AssertExceptionExpected(nse, True)
            TestUtil.AssertContains(nse.ToString(), "System.NotSupportedException: Initializing instances of the entity type AstoriaClientUnitTests.ClientModule+MaterializeUnitTest+NarrowOrder with the expression new NarrowEmployee() {Name = o.Employees.FirstName} is not supported.")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CustomerOrders()
            Dim query = From cust As NorthwindSimpleModel.Customers In ctx.Customers Where cust.CustomerID = "AROUT"
            Dim languageQuery = CType(query, DataServiceQuery(Of NorthwindSimpleModel.Customers)).Expand("Orders")
            Dim explictQuery = ctx.CreateUri(Of NorthwindSimpleModel.Customers)("Customers('AROUT')?$expand=Orders", True)
            Assert.AreEqual(explictQuery.ToString(), languageQuery.ToString())

            For Each merger As MergeOption In New MergeOption() {MergeOption.NoTracking, MergeOption.AppendOnly}
                ctx.MergeOption = merger

                Dim customer As NorthwindSimpleModel.Customers = ctx.Execute(explictQuery).Single()
                Assert.AreEqual(0, customer.CustomerDemographics.Count, "has demographics")
                Assert.AreEqual(13, customer.Orders.Count, "didn't expand expected order count")
                For Each order As NorthwindSimpleModel.Orders In customer.Orders
                    Assert.IsNull(order.Customers)
                Next

                If MergeOption.NoTracking <> merger Then
                    Assert.AreEqual(14, ctx.Entities.Count, "entity count")
                    Assert.AreEqual(13, ctx.Links.Count, "link count")
                End If
            Next
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CircularRelationships()
            Dim query = From cust As NorthwindSimpleModel.Customers In ctx.Customers Where cust.CustomerID = "AROUT"
            Dim languageQuery = CType(query, DataServiceQuery(Of NorthwindSimpleModel.Customers)).Expand("Orders($expand=Customers)")
            Dim explictQuery = ctx.CreateUri(Of NorthwindSimpleModel.Customers)("Customers('AROUT')?$expand=Orders($expand=Customers)", True)
            Assert.AreEqual(explictQuery.ToString(), languageQuery.ToString())

            For Each merger As MergeOption In New MergeOption() {MergeOption.NoTracking, MergeOption.AppendOnly}
                ctx.MergeOption = merger

                Dim customer As NorthwindSimpleModel.Customers = ctx.Execute(explictQuery).Single()
                Assert.AreEqual(13, customer.Orders.Count, "didn't expand expected order count")
                For Each order As NorthwindSimpleModel.Orders In customer.Orders
                    Assert.AreSame(customer, order.Customers, "didn't set reference on expansion in {0}", merger)
                Next

                If MergeOption.NoTracking <> merger Then
                    Assert.AreEqual(14, ctx.Entities.Count, "entity count")
                    Assert.AreEqual(26, ctx.Links.Count, "link count")
                End If
            Next
        End Sub
        'Remove Atom
        '  <TestCategory("Partition2")> <TestMethod()>
        Public Sub CircularRelationships_ExpandedFurther()
            Dim query = From cust As NorthwindSimpleModel.Customers In ctx.Customers Where cust.CustomerID = "AROUT"
            Dim languageQuery = CType(query, DataServiceQuery(Of NorthwindSimpleModel.Customers)).Expand("Orders($expand=Employees($expand=Orders($expand=Customers)))")
            Dim explictQuery = ctx.CreateUri(Of NorthwindSimpleModel.Customers)("Customers('AROUT')?$expand=Orders($expand=Employees($expand=Orders($expand=Customers)))", True)
            Assert.AreEqual(explictQuery.ToString(), languageQuery.ToString())

            For Each merger As MergeOption In New MergeOption() {MergeOption.NoTracking, MergeOption.AppendOnly}
                ctx.MergeOption = merger

                Dim customer As NorthwindSimpleModel.Customers = ctx.Execute(explictQuery).Single()
                Assert.AreEqual(13, customer.Orders.Count, "didn't expand expected order count")
                For Each order As NorthwindSimpleModel.Orders In customer.Orders
                    Assert.AreSame(customer, order.Customers, "didn't set reference on expansion in {0}", merger)
                Next

                If MergeOption.NoTracking <> merger Then
                    Assert.IsTrue(700 < ctx.Entities.Count, "entity count")
                    Assert.IsTrue(1200 < ctx.Links.Count, "link count")
                End If
            Next
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CustomerExpandOrders()
            Dim query = ctx.CreateUri(Of NorthwindSimpleModel.Customers)("Customers?$top=5&$expand=Orders")

            Dim totalOrders As Int32 = 0
            For Each customer As NorthwindSimpleModel.Customers In ctx.Execute(query)
                totalOrders += customer.Orders.Count
            Next

            Assert.IsTrue(0 < totalOrders, "no orders expanded")
            Assert.AreEqual(totalOrders, Enumerable.Count(From a In ctx.Entities Where GetType(NorthwindSimpleModel.Orders).IsInstanceOfType(a.Entity) Select a))
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub SameEntityRepeatedWithOverwriteChanges()
            Dim requestUri = ctx.CreateUri(Of NorthwindSimpleModel.Customers)("Customers?$expand=Orders($expand=Customers)", True)
            ctx.MergeOption = MergeOption.OverwriteChanges
            Dim collection = New HashSet(Of NorthwindSimpleModel.Customers)()
            Dim repeatedCustomer = False
            For Each customer As NorthwindSimpleModel.Customers In ctx.Execute(requestUri)
                repeatedCustomer = repeatedCustomer Or collection.Add(customer)
                For Each o In customer.Orders
                    repeatedCustomer = repeatedCustomer Or collection.Add(o.Customers)
                Next
            Next
            Assert.IsTrue(repeatedCustomer, "Assert that repeated customer is encountered for the bug to repro")
        End Sub

#If ASTORA_OPEN_OBJECT Then
        <TestMethod()> Public Sub ShouldThrowBatchExceptionWhenQuerySingle()
            Dim query1 = ctx.CreateUri(Of OpenObject)("foo", True)
            ExecuteQuerySingle(WebExceptionStatus.ProtocolError, query1)
            Try
                Util.ExecuteBatch(ctx, query1)
                Assert.Fail("expected batch exception")
            Catch ex As Exception

            End Try

            Dim query2 = ctx.CreateUri(Of OpenObject)("/foo", True)
            ExecuteQuerySingle(WebExceptionStatus.ProtocolError, query2)
            Try
                Util.ExecuteBatch(ctx, query2)
                Assert.Fail("expected batch exception")
            Catch ex As Exception

            End Try
        End Sub
#End If
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub NoContent()
            ' Original bug failure.
            Dim q = ctx.CreateQuery(Of NorthwindSimpleModel.Employees)("Employees(2)/Employees2")
            For Each o In q
                Assert.Fail("Unexpected Employees(2)/Employees2 - No Content expected instead")
            Next

            ' Other ways of accessing the property.
            Dim employee2 = ctx.CreateQuery(Of NorthwindSimpleModel.Employees)("Employees(2)").Execute().Single()
            ctx.LoadProperty(employee2, "Employees2")

            Dim result = q.BeginExecute(Nothing, Nothing)
            result.AsyncWaitHandle.WaitOne()
            For Each o In q.EndExecute(result)
                Assert.Fail("Unexpected Employees(2)/Employees2 - No Content expected instead")
            Next

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub KeyValueWithSingleQuote()
            Dim products = ctx.Alphabetical_list_of_products.Execute().ToArray()

            ExecuteQuerySingle(HttpStatusCode.OK, ctx.CreateUri(Of NorthwindSimpleModel.Alphabetical_list_of_products)("Alphabetical_list_of_products(CategoryName='Condiments',Discontinued=false,ProductID=4,ProductName='Chef Anton''s Cajun Seasoning')"))

            ExecuteQuerySingle(HttpStatusCode.OK, ctx.CreateUri(Of NorthwindSimpleModel.Alphabetical_list_of_products)("Alphabetical_list_of_products(CategoryName='Condiments',Discontinued=false,ProductID=4,ProductName='Chef%20Anton''s%20Cajun%20Seasoning')"))

            ExecuteQuerySingle(HttpStatusCode.OK, ctx.CreateUri(Of NorthwindSimpleModel.Alphabetical_list_of_products)("Alphabetical_list_of_products(CategoryName='Condiments',Discontinued=false,ProductID=4,ProductName='Chef%20Anton%27%27s%20Cajun%20Seasoning')"))

            ExecuteQuerySingle(HttpStatusCode.BadRequest, ctx.CreateUri(Of NorthwindSimpleModel.Alphabetical_list_of_products)("Alphabetical_list_of_products(CategoryName='Condiments',Discontinued=false,ProductID=4,ProductName='Chef%20Anton'%20Cajun%20Seasoning')"))

            ExecuteQuerySingle(HttpStatusCode.BadRequest, ctx.CreateUri(Of NorthwindSimpleModel.Alphabetical_list_of_products)("Alphabetical_list_of_products(CategoryName='Condiments',Discontinued=false,ProductID=4,ProductName='Chef%20Anton%27%20Cajun%20Seasoning')"))
        End Sub

        Private Function ExecuteQuerySingle(Of T)(ByVal status As HttpStatusCode, ByVal query As DataServiceRequest(Of T)) As T
            Try
                Dim result As T = ctx.Execute(query).Single()
                Assert.AreEqual(HttpStatusCode.OK, status, "expected {0} for this query {1}", status, query.ToString())

                Util.ExecuteBatch(ctx, query)
                Return result
            Catch ex As DataServiceQueryException
                If (CInt(status) <> ex.Response.StatusCode) Then
                    Throw
                End If

                Return Nothing
            End Try
        End Function


        <TestCategory("Partition2")> <TestMethod()>
        Public Sub BatchQueryFail()
            Dim data As DataServiceResponse = ctx.ExecuteBatch(ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("Cust"))

            Assert.AreEqual(CInt(HttpStatusCode.Accepted), data.BatchStatusCode)
            Assert.IsNotNull(data.BatchHeaders)

            Dim queryCount As Int32 = 0
            For Each qresp As QueryOperationResponse In data
                Assert.AreEqual(404, qresp.StatusCode)
                Assert.IsNotNull(qresp.Error)
                queryCount += 1
                Try
                    DirectCast(qresp, IEnumerable).GetEnumerator()
                    Assert.Fail("GetResults didn't throw")
                Catch ex As Exception

                End Try
            Next

            Assert.AreEqual(1, queryCount)
            Assert.AreEqual(0, data.Count, "0 batch")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub BatchQuerySingle()
            Dim data As DataServiceResponse = ctx.ExecuteBatch(ctx.Customers)

            Assert.AreEqual(CInt(HttpStatusCode.Accepted), data.BatchStatusCode)
            Assert.IsNotNull(data.BatchHeaders)

            Dim queryCount As Int32 = 0
            For Each query As QueryOperationResponse In data
                queryCount += 1

                Assert.IsNull(query.Error)
                Assert.AreEqual(CInt(HttpStatusCode.OK), query.StatusCode)
                Assert.IsNotNull(query.Headers)

                Dim customerCount As Int32 = 0
                For Each cust As NorthwindSimpleModel.Customers In query
                    customerCount += 1

                    Dim path As Uri = Nothing
                    Assert.IsTrue(ctx.TryGetUri(cust, path))
                    Assert.IsTrue(Not String.IsNullOrEmpty(path.OriginalString))
                Next

                Assert.IsTrue(1 < customerCount)
            Next

            Assert.AreEqual(1, queryCount)
            Assert.AreEqual(0, data.Count)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub BatchQueryDouble()
            Dim data As DataServiceResponse = ctx.ExecuteBatch(ctx.Customers, ctx.Suppliers)

            Assert.IsNotNull(data.BatchHeaders)
            Assert.AreEqual(CInt(HttpStatusCode.Accepted), data.BatchStatusCode)

            Dim queryCount As Int32 = 0
            For Each query As QueryOperationResponse In data
                queryCount += 1

                Assert.IsNotNull(query.Headers)
                Assert.AreEqual(CInt(HttpStatusCode.OK), query.StatusCode)
                Assert.IsNull(query.Error)

                If (1 = queryCount) Then
                    Dim customerCount As Int32 = 0
                    For Each cust As NorthwindSimpleModel.Customers In query
                        customerCount += 1

                        Dim path As Uri = Nothing
                        Assert.IsTrue(ctx.TryGetUri(cust, path))
                        Assert.IsTrue(Not String.IsNullOrEmpty(path.OriginalString))
                    Next

                    Assert.IsTrue(1 < customerCount)
                ElseIf (2 = queryCount) Then
                    Dim supplierCount As Int32 = 0
                    For Each cust As NorthwindSimpleModel.Suppliers In query
                        supplierCount += 1

                        Dim path As Uri = Nothing
                        Assert.IsTrue(ctx.TryGetUri(cust, path))
                        Assert.IsTrue(Not String.IsNullOrEmpty(path.OriginalString))
                    Next

                    Assert.IsTrue(1 < supplierCount)

                End If

            Next

            Assert.AreEqual(2, queryCount)
            Assert.AreEqual(0, data.Count)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub InterleaveExecute()
            Dim a = ctx.Customers.GetEnumerator()
            Dim b = ctx.Customers.GetEnumerator()

            While a.MoveNext() Or b.MoveNext()
                ctx.MergeOption = MergeOption.NoTracking ' should have no effect on materialization once enumerators start
                Assert.AreSame(a.Current, b.Current)
            End While


            Dim p = ctx.Orders.BeginExecute(Nothing, Nothing)
            Dim q = ctx.Orders.BeginExecute(Nothing, Nothing)

            Assert.IsTrue(p.AsyncWaitHandle.WaitOne(New TimeSpan(0, 0, 30), False), "timeout 1")
            Assert.IsTrue(q.AsyncWaitHandle.WaitOne(New TimeSpan(0, 0, 30), False), "timeout 2")

            Dim r = ctx.Orders.EndExecute(p).GetEnumerator()
            Dim s = ctx.Orders.EndExecute(q).GetEnumerator()

            ctx.MergeOption = MergeOption.NoTracking
            While r.MoveNext() Or s.MoveNext()
                ctx.MergeOption = MergeOption.OverwriteChanges ' should have no effect on materialization once enumerators start
                Assert.AreNotSame(r.Current, s.Current)
            End While
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub MultipleConcurrentRequests()
            Dim span As TimeSpan = New TimeSpan(0, 0, 30)
            Dim query As Uri = New Uri("Customers('ALFKI')", UriKind.Relative)
            ctx.MergeOption = MergeOption.NoTracking

            Dim count As Int32 = 500
            Dim semi As New Semaphore(0, count)
            For i As Int32 = 1 To count
                ctx.BeginExecute(Of NorthwindSimpleModel.Customers)(query, AddressOf EndMultipleConcurrentRequests, New Object() {Nothing, semi})
            Next

            For i As Int32 = 1 To count
                Assert.IsTrue(semi.WaitOne(span, False), "timeout")
            Next
            Assert.IsFalse(semi.WaitOne(0, False), "extra?")

            Dim query2 = CType((From o In ctx.Customers Where o.CustomerID = "ALFKI" Select o), DataServiceQuery(Of NorthwindSimpleModel.Customers))
            For i As Int32 = 1 To count
                query2.BeginExecute(AddressOf EndMultipleConcurrentRequests, New Object() {query2, semi})
            Next

            For i As Int32 = 1 To count
                Assert.IsTrue(semi.WaitOne(span, False), "timeout")
            Next
            Assert.IsFalse(semi.WaitOne(0, False), "extra?")

            semi.Close()
        End Sub

        Private Sub EndMultipleConcurrentRequests(ByVal asyncResult As IAsyncResult)
            Dim array = CType(asyncResult.AsyncState, Object())
            Dim query = CType(array(0), DataServiceQuery(Of NorthwindSimpleModel.Customers))
            Dim semi As Semaphore = CType(array(1), Semaphore)
            Try
                Dim enumerable As IEnumerable(Of NorthwindSimpleModel.Customers)
                If query Is Nothing Then
                    enumerable = ctx.EndExecute(Of NorthwindSimpleModel.Customers)(asyncResult)
                Else
                    enumerable = query.EndExecute(asyncResult)
                End If

                If (0 <> random.Next(0, 2)) Then
                    enumerable.Single()
                End If
            Catch ex As Exception
                Assert.Fail(ex.ToString())
            Finally
                semi.Release()
            End Try
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub EndExecuteFailure()
            Try
                ctx.Categories.EndExecute(Nothing) ' calling EndExecute with null
                Assert.Fail("expected ArgumentNullException")
            Catch ex As ArgumentNullException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            Dim result = ctx.Customers.BeginExecute(Nothing, Nothing)
            Try
                ctx.Categories.EndExecute(result)   ' calling EndExecute on wrong object
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            ctx.Customers.EndExecute(result)
            Try
                ctx.Customers.EndExecute(result)    ' calling EndExecute twice
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.Customers.EndExecute(ctx.BeginSaveChanges(Nothing, Nothing))    ' calling EndExecute with random IAsyncResult
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub CreateQueryFailure()
            ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("customers")

            Try
                ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("customers?$top=1")
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
            End Try

            Try
                ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("customers?$top=1#frag")
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
            End Try

            Try
                ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("customers#frag")
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
            End Try

            Try
                ctx.CreateQuery(Of NorthwindSimpleModel.Customers)(ctx.BaseUri.OriginalString + "/customers")
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
            End Try

            Try
                ctx.CreateQuery(Of NorthwindSimpleModel.Customers)(ctx.BaseUri.OriginalString + "/customers?orderby=CustomerID")
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
            End Try

            Try
                ctx.CreateQuery(Of NorthwindSimpleModel.Customers)(ctx.BaseUri.OriginalString + "/customers?orderby=CustomerID#jump")
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
            End Try

            Try
                ctx.CreateQuery(Of NorthwindSimpleModel.Customers)(ctx.BaseUri.OriginalString + "/customers#tagjump")
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
            End Try

        End Sub


        <TestCategory("Partition2")> <TestMethod()> Public Sub LoadPropertyApiFailure()
            Dim customer As NorthwindSimpleModel.Customers = NorthwindSimpleModel.Customers.CreateCustomers("asdf", "Microsoft")
            ctx.AddToCustomers(customer)

            Dim message As String = Nothing
            Try
                ctx.LoadProperty(customer, "Orders")
                Assert.Fail("Expected InvalidOperationException")
            Catch ex As InvalidOperationException
                message = ex.Message
                Assert.IsTrue(message.Contains("added state"), "{0}", ex)
            End Try

            Try
                Dim async = ctx.BeginLoadProperty(customer, "Orders", Nothing, Nothing)
                Assert.Fail("Expected InvalidOperationException")
            Catch ex As InvalidOperationException
                Assert.AreEqual(message, ex.Message, "{0}", ex)
            End Try

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub EndLoadPropertyApiFailure()
            Try
                ctx.EndLoadProperty(Nothing) ' calling EndLoadProperty with null
                Assert.Fail("expected ArgumentNullException")
            Catch ex As ArgumentNullException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            Dim ctx2 = New NorthwindSimpleModel.NorthwindContext(ctx.BaseUri)
            'ctx2.EnableAtom = True
            'ctx2.Format.UseAtom()
            Dim customer = ctx2.Customers().First()

            Dim result = ctx2.BeginLoadProperty(customer, "Orders", Nothing, Nothing)
            Try
                ctx.EndLoadProperty(result)   ' calling EndLoadProperty on wrong object
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            ctx2.EndLoadProperty(result)
            Try
                ctx2.EndLoadProperty(result)    ' calling EndLoadProperty twice
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.EndLoadProperty(ctx.Customers.BeginExecute(Nothing, Nothing))    ' calling EndLoadProperty with random IAsyncResult
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try


            ctx.Detach(customer)
            ctx.AddObject("Customers", customer)
            Try
                ctx.LoadProperty(customer, "Orders")
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("added state"), "{0}", ex)
            End Try

        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub EndSaveChangesApiFailure()
            Try
                ctx.EndSaveChanges(Nothing) ' calling EndSaveChanges with null
            Catch ex As ArgumentNullException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            Dim result = ctx.BeginSaveChanges(Nothing, Nothing)
            Try
                Dim ctx2 = New NorthwindSimpleModel.NorthwindContext(ctx.BaseUri)
                ctx2.EndSaveChanges(result)   ' calling EndSaveChanges on wrong object
                Assert.Fail("expected exception")
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            ctx.EndSaveChanges(result)
            Try
                ctx.EndSaveChanges(result)    ' calling EndSaveChanges twice
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.EndSaveChanges(ctx.Customers.BeginExecute(Nothing, Nothing))    ' calling EndSaveChanges with random IAsyncResult
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.SaveChanges(CType(-1, SaveChangesOptions))
            Catch ex As ArgumentException
                Assert.AreEqual("options", ex.ParamName, "{0}", ex)
            End Try
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub ExecuteBatchApiFailure()
            Try
                ctx.ExecuteBatch(Nothing).QueryCount()   ' null
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentNullException
                Assert.AreEqual("queries", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.ExecuteBatch().QueryCount() ' empty collection
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("queries", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.ExecuteBatch(New DataServiceQuery(0) {}).QueryCount() ' explict empty collection
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("queries", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.ExecuteBatch(New DataServiceQuery() {ctx.Customers, Nothing}).QueryCount() ' with null collection
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("queries", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.ExecuteBatch(New DataServiceQuery() {Nothing, ctx.Customers}).QueryCount() ' with null collection
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("queries", ex.ParamName, "{0}", ex)
            End Try

            Try
                ctx.ExecuteBatch(New DataServiceQuery() {ctx.Categories, Nothing, ctx.Customers}).QueryCount() ' with null collection
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
                Assert.AreEqual("queries", ex.ParamName, "{0}", ex)
            End Try
        End Sub

        Private Function ExtraMissing(Of T)(ByVal expected As Exception) As T
            Try
                Dim od = ctx.Execute(Of T)(New Uri("Order_Details?$top=1&$expand=Orders", UriKind.Relative)).Single()
                Assert.IsNull(expected, "expected exception was not thrown")
                Return od
            Catch ex As Exception
                If expected Is Nothing Then
                    Throw
                End If

                If Not expected.GetType().IsInstanceOfType(ex) Then
                    Throw
                End If
            End Try
        End Function


        Private Function MismatchedNavigationType(Of T)(ByVal expected As Exception) As T
            Try
                Dim ctx = New NorthwindSimpleModel.NorthwindContext(Me.ctx.BaseUri, ODataProtocolVersion.V4)
                'ctx.Format.UseAtom()
                Dim od = ctx.Execute(Of T)(New Uri("Orders?$top=1", UriKind.Relative)).Single()
                Assert.IsNull(expected, "expected exception was not thrown")
                Return od
            Catch ex As Exception
                If expected Is Nothing Then
                    Throw
                End If

                If Not expected.GetType().IsInstanceOfType(ex) Then
                    Throw
                End If
            End Try
        End Function
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub NothingExtraOrMissing()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            Assert.IsNotNull(ExtraMissing(Of NorthwindSimpleModel.Order_Details)(Nothing))

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            Assert.IsNotNull(ExtraMissing(Of NorthwindSimpleModel.Order_Details)(Nothing))
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub ExtraKey()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            Dim od = ExtraMissing(Of Order_Details_ExtraKeys)(New InvalidOperationException())
            Assert.IsNull(od)

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            Dim oe = ExtraMissing(Of Order_Details_ExtraKeys)(New InvalidOperationException())
            Assert.IsNull(oe)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub ExtraProperty()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            Dim od = ExtraMissing(Of Order_Details_ExtraProperties)(New InvalidOperationException())
            Assert.IsNull(od)

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            Dim oe = ExtraMissing(Of Order_Details_ExtraProperties)(New InvalidOperationException())
            Assert.IsNull(oe)

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub MismatchedNavigationPropertyType()
            ' Regression test for [Client-ODataLib-Integration] Astoria client does not fail if the client and server stream property does not match
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            Dim od = MismatchedNavigationType(Of Orders_ElementNotEntityType)(New InvalidOperationException())
            Assert.IsNull(od)

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            Dim oe = MismatchedNavigationType(Of Orders_ElementNotEntityType)(New InvalidOperationException())
            Assert.IsNull(oe)
        End Sub
        'Doesn't check entry key properties before execute, So this case is invalid'
        <Ignore> ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub MissingDescribedKey()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException

            ExtraMissing(Of Order_Details_MissingDescribedKey)(New InvalidOperationException())

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            ExtraMissing(Of Order_Details_MissingDescribedKey)(New InvalidOperationException())
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub MissingKey()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            ExtraMissing(Of Order_Details_MissingKeys)(New InvalidOperationException())

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            ExtraMissing(Of Order_Details_MissingKeys)(New InvalidOperationException())
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub MissingProperty()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            ExtraMissing(Of Order_Details_MissingProperties)(New InvalidOperationException())

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            ExtraMissing(Of Order_Details_MissingProperties)(New InvalidOperationException())
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub MissingLink()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            ExtraMissing(Of Order_Details_MissingLinks)(New InvalidOperationException())

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            ExtraMissing(Of Order_Details_MissingLinks)(Nothing)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CompetingDefaultAKeyID()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            Assert.IsNotNull(ExtraMissing(Of AKey)(Nothing))

            Dim a As New AKey
            a.AKeyID = 1
            a.ID = 2
            ctx.AttachTo("a", a)

            Dim identity As Uri = Nothing
            ctx.TryGetUri(a, identity)
            Assert.IsTrue(identity.ToString().Contains("a(1)"))
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CompetingDefaultBKeyID()
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            Assert.IsNotNull(ExtraMissing(Of BKey)(Nothing))

            Dim b As New BKey
            b.BKeyID = 1
            b.ID = 2
            ctx.AttachTo("b", b)

            Dim identity As Uri = Nothing
            ctx.TryGetUri(b, identity)
            Assert.IsTrue(identity.ToString().Contains("b(1)"))
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub VerifyIncludingPropertiesOnBaseClassesOfClassWithKEy()
            Try
                ctx.AttachTo("C", New CNoKey())
            Catch ex As ArgumentException
            End Try

            Dim d As New DKey
            d.Name = "fixed"
            d.ID = 1

            ctx.AttachTo("d", d)
            ctx.UpdateObject(d)

            Dim identity As Uri = Nothing
            ctx.TryGetUri(d, identity)
            Assert.IsTrue(identity.ToString().Contains("d(1)"))

            Dim wrappingStream As WrappingStream = Nothing
            ctx.RegisterStreamCustomizer(Function(inputStream As Stream)
                                             wrappingStream = New WrappingStream(inputStream)
                                             Return wrappingStream
                                         End Function,
                                         Nothing)

            Dim exception As Exception = TestUtil.RunCatching(Function() ctx.SaveChanges)
            Assert.IsNotNull(exception, "Exception expected, but none was thrown")

            DProperties(wrappingStream.GetLoggingStreamAsXDocument().Elements.First)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod(), Variation("Verify Server Type in Entity Descriptors gets materialized correctly")>
        Public Sub ResolvingServerTypeNameInEntityDescriptor()
            Dim custs = ctx.Customers.Expand("Orders").FirstOrDefault()
            Dim custDescriptor = ctx.GetEntityDescriptor(custs)
            Assert.IsNotNull(custDescriptor.ServerTypeName)
            Dim orderDescriptor = ctx.GetEntityDescriptor(custs.Orders.FirstOrDefault())
            Assert.IsNotNull(orderDescriptor.ServerTypeName)
        End Sub

        Private Shared Sub DProperties(ByVal data As XElement)
            Dim result As XElement =
                <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                    <title/>
                    <updated>2008-05-19T21:12:53.5671702Z</updated>
                    <author>
                        <name/>
                    </author>
                    <id>http://localhost:6000/TheTest/d(1)</id>
                    <content type="application/xml">
                        <m:properties>
                            <d:ID>1</d:ID>
                            <d:Name>fixed</d:Name>
                        </m:properties>
                    </content>
                </entry>

            Dim expected = result.DescendantNodes.ToArray()
            Dim actual = data.DescendantNodes.ToArray()

            Assert.AreEqual(expected.Length, actual.Length)
        End Sub


    End Class

    <Global.Microsoft.OData.Client.KeyAttribute("OrderID", "ProductID", "ExtraID")>
    Partial Public Class Order_Details_MissingDescribedKey
        Public Property OrderID() As Integer
            Get
                Return Me._OrderID
            End Get
            Set(ByVal value As Integer)
                Me._OrderID = value
            End Set
        End Property
        Private _OrderID As Integer
    End Class

    Partial Public Class AKey
        Public Property AKeyID() As Integer
            Get
                Return Me.a
            End Get
            Set(ByVal value As Integer)
                Me.a = value
            End Set
        End Property
        Private a As Integer

        Public Property ID() As Integer
            Get
                Return Me.b
            End Get
            Set(ByVal value As Integer)
                Me.b = value
            End Set
        End Property
        Private b As Integer
    End Class

    Partial Public Class BKey
        Public Property ID() As Integer
            Get
                Return Me.b
            End Get
            Set(ByVal value As Integer)
                Me.b = value
            End Set
        End Property
        Private b As Integer

        Public Property BKeyID() As Integer
            Get
                Return Me.a
            End Get
            Set(ByVal value As Integer)
                Me.a = value
            End Set
        End Property
        Private a As Integer

    End Class

    Partial Public Class CNoKey
        Public Property Name() As String
            Get
                Return Me._name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property
        Private _name As String
    End Class

    Partial Public Class DKey
        Inherits CNoKey
        Public Property ID() As Int32
            Get
                Return _id
            End Get
            Set(ByVal value As Int32)
                _id = value
            End Set
        End Property
        Private _id As Int32
    End Class


#Region "Order_Details with additional key not on server, ExtraID"
    <Global.Microsoft.OData.Client.KeyAttribute("OrderID", "ProductID", "ExtraID")>
    Partial Public Class Order_Details_ExtraKeys
        Public Property OrderID() As Integer
            Get
                Return Me._OrderID
            End Get
            Set(ByVal value As Integer)
                Me._OrderID = value
            End Set
        End Property
        Private _OrderID As Integer

        Public Property ProductID() As Integer
            Get
                Return Me._ProductID
            End Get
            Set(ByVal value As Integer)
                Me._ProductID = value
            End Set
        End Property
        Private _ProductID As Integer

        Public Property ExtraID() As Integer
            Get
                Return Me._ExtraID
            End Get
            Set(ByVal value As Integer)
                Me._ExtraID = value
            End Set
        End Property
        Private _ExtraID As Integer

        Public Property UnitPrice() As Decimal
            Get
                Return Me._UnitPrice
            End Get
            Set(ByVal value As Decimal)
                Me._UnitPrice = value
            End Set
        End Property
        Private _UnitPrice As Decimal

        Public Property Quantity() As Short
            Get
                Return Me._Quantity
            End Get
            Set(ByVal value As Short)
                Me._Quantity = value
            End Set
        End Property
        Private _Quantity As Short

        Public Property Discount() As Single
            Get
                Return Me._Discount
            End Get
            Set(ByVal value As Single)
                Me._Discount = value
            End Set
        End Property
        Private _Discount As Single

        Public Property Orders() As NorthwindSimpleModel.Orders
            Get
                Return Me._Orders
            End Get
            Set(ByVal value As NorthwindSimpleModel.Orders)
                Me._Orders = value
            End Set
        End Property
        Private _Orders As NorthwindSimpleModel.Orders

        Public Property Products() As Products
            Get
                Return Me._Products
            End Get
            Set(ByVal value As Products)
                Me._Products = value
            End Set
        End Property
        Private _Products As Products
    End Class
#End Region

#Region "Order_Details with additional property not on server, Quality"
    <Global.Microsoft.OData.Client.KeyAttribute("OrderID", "ProductID")>
    Partial Public Class Order_Details_ExtraProperties
        Public Property OrderID() As Integer
            Get
                Return Me._OrderID
            End Get
            Set(ByVal value As Integer)
                Me._OrderID = value
            End Set
        End Property
        Private _OrderID As Integer

        Public Property ProductID() As Integer
            Get
                Return Me._ProductID
            End Get
            Set(ByVal value As Integer)
                Me._ProductID = value
            End Set
        End Property
        Private _ProductID As Integer

        Public Property Quality() As Nullable(Of Integer)
            Get
                Return Me._Quality
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me._Quality = value
            End Set
        End Property
        Private _Quality As Nullable(Of Integer)

        Public Property UnitPrice() As Decimal
            Get
                Return Me._UnitPrice
            End Get
            Set(ByVal value As Decimal)
                Me._UnitPrice = value
            End Set
        End Property
        Private _UnitPrice As Decimal

        Public Property Quantity() As Short
            Get
                Return Me._Quantity
            End Get
            Set(ByVal value As Short)
                Me._Quantity = value
            End Set
        End Property
        Private _Quantity As Short

        Public Property Discount() As Single
            Get
                Return Me._Discount
            End Get
            Set(ByVal value As Single)
                Me._Discount = value
            End Set
        End Property
        Private _Discount As Single

        Public Property Orders() As NorthwindSimpleModel.Orders
            Get
                Return Me._Orders
            End Get
            Set(ByVal value As NorthwindSimpleModel.Orders)
                Me._Orders = value
            End Set
        End Property
        Private _Orders As NorthwindSimpleModel.Orders

        Public Property Products() As Products
            Get
                Return Me._Products
            End Get
            Set(ByVal value As Products)
                Me._Products = value
            End Set
        End Property
        Private _Products As Products
    End Class
#End Region

#Region "Order_Details with missing ProductID"
    <Global.Microsoft.OData.Client.KeyAttribute("OrderID")>
    Partial Public Class Order_Details_MissingKeys
        Public Property OrderID() As Integer
            Get
                Return Me._OrderID
            End Get
            Set(ByVal value As Integer)
                Me._OrderID = value
            End Set
        End Property
        Private _OrderID As Integer

        Public Property UnitPrice() As Decimal
            Get
                Return Me._UnitPrice
            End Get
            Set(ByVal value As Decimal)
                Me._UnitPrice = value
            End Set
        End Property
        Private _UnitPrice As Decimal

        Public Property Quantity() As Short
            Get
                Return Me._Quantity
            End Get
            Set(ByVal value As Short)
                Me._Quantity = value
            End Set
        End Property
        Private _Quantity As Short

        Public Property Discount() As Single
            Get
                Return Me._Discount
            End Get
            Set(ByVal value As Single)
                Me._Discount = value
            End Set
        End Property
        Private _Discount As Single

        Public Property Orders() As NorthwindSimpleModel.Orders
            Get
                Return Me._Orders
            End Get
            Set(ByVal value As NorthwindSimpleModel.Orders)
                Me._Orders = value
            End Set
        End Property
        Private _Orders As NorthwindSimpleModel.Orders

        Public Property Products() As Products
            Get
                Return Me._Products
            End Get
            Set(ByVal value As Products)
                Me._Products = value
            End Set
        End Property
        Private _Products As Products
    End Class
#End Region

#Region "Order_Details with missing UnitPrice & Quantity"
    <Global.Microsoft.OData.Client.KeyAttribute("OrderID", "ProductID")>
    Partial Public Class Order_Details_MissingProperties
        Public Property OrderID() As Integer
            Get
                Return Me._OrderID
            End Get
            Set(ByVal value As Integer)
                Me._OrderID = value
            End Set
        End Property
        Private _OrderID As Integer

        Public Property ProductID() As Integer
            Get
                Return Me._ProductID
            End Get
            Set(ByVal value As Integer)
                Me._ProductID = value
            End Set
        End Property
        Private _ProductID As Integer

        Public Property Discount() As Single
            Get
                Return Me._Discount
            End Get
            Set(ByVal value As Single)
                Me._Discount = value
            End Set
        End Property
        Private _Discount As Single

        Public Property Orders() As NorthwindSimpleModel.Orders
            Get
                Return Me._Orders
            End Get
            Set(ByVal value As NorthwindSimpleModel.Orders)
                Me._Orders = value
            End Set
        End Property
        Private _Orders As NorthwindSimpleModel.Orders

        Public Property Products() As Products
            Get
                Return Me._Products
            End Get
            Set(ByVal value As Products)
                Me._Products = value
            End Set
        End Property
        Private _Products As Products
    End Class
#End Region
    <Global.Microsoft.OData.Client.KeyAttribute("OrderID")>
    Partial Public Class Orders_ElementNotEntityType
        '''<summary>
        '''Create a new Orders object.
        '''</summary>
        '''<param name="orderID">Initial value of OrderID.</param>
        Public Shared Function CreateOrders(ByVal orderID As Integer) As Orders
            Dim orders As Orders = New Orders
            orders.OrderID = orderID
            Return orders
        End Function
        '''<summary>
        '''There are no comments for Property OrderID in the schema.
        '''</summary>
        Public Property OrderID() As Integer
            Get
                Return Me._OrderID
            End Get
            Set(ByVal value As Integer)
                Me.OnOrderIDChanging(value)
                Me._OrderID = value
                Me.OnOrderIDChanged()
            End Set
        End Property
        Private _OrderID As Integer
        Partial Private Sub OnOrderIDChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnOrderIDChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property OrderDate in the schema.
        '''</summary>
        Public Property OrderDate() As Global.System.Nullable(Of Global.System.DateTimeOffset)
            Get
                Return Me._OrderDate
            End Get
            Set(ByVal value As Global.System.Nullable(Of Global.System.DateTimeOffset))
                Me.OnOrderDateChanging(value)
                Me._OrderDate = value
                Me.OnOrderDateChanged()
            End Set
        End Property
        Private _OrderDate As Global.System.Nullable(Of Global.System.DateTimeOffset)
        Partial Private Sub OnOrderDateChanging(ByVal value As Global.System.Nullable(Of Global.System.DateTimeOffset))
        End Sub
        Partial Private Sub OnOrderDateChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property RequiredDate in the schema.
        '''</summary>
        Public Property RequiredDate() As Global.System.Nullable(Of Global.System.DateTimeOffset)
            Get
                Return Me._RequiredDate
            End Get
            Set(ByVal value As Global.System.Nullable(Of Global.System.DateTimeOffset))
                Me.OnRequiredDateChanging(value)
                Me._RequiredDate = value
                Me.OnRequiredDateChanged()
            End Set
        End Property
        Private _RequiredDate As Global.System.Nullable(Of Global.System.DateTimeOffset)
        Partial Private Sub OnRequiredDateChanging(ByVal value As Global.System.Nullable(Of Global.System.DateTimeOffset))
        End Sub
        Partial Private Sub OnRequiredDateChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property ShippedDate in the schema.
        '''</summary>
        Public Property ShippedDate() As Global.System.Nullable(Of Global.System.DateTimeOffset)
            Get
                Return Me._ShippedDate
            End Get
            Set(ByVal value As Global.System.Nullable(Of Global.System.DateTimeOffset))
                Me.OnShippedDateChanging(value)
                Me._ShippedDate = value
                Me.OnShippedDateChanged()
            End Set
        End Property
        Private _ShippedDate As Global.System.Nullable(Of Global.System.DateTimeOffset)
        Partial Private Sub OnShippedDateChanging(ByVal value As Global.System.Nullable(Of Global.System.DateTimeOffset))
        End Sub
        Partial Private Sub OnShippedDateChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property Freight in the schema.
        '''</summary>
        Public Property Freight() As Global.System.Nullable(Of Decimal)
            Get
                Return Me._Freight
            End Get
            Set(ByVal value As Global.System.Nullable(Of Decimal))
                Me.OnFreightChanging(value)
                Me._Freight = value
                Me.OnFreightChanged()
            End Set
        End Property
        Private _Freight As Global.System.Nullable(Of Decimal)
        Partial Private Sub OnFreightChanging(ByVal value As Global.System.Nullable(Of Decimal))
        End Sub
        Partial Private Sub OnFreightChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property ShipName in the schema.
        '''</summary>
        Public Property ShipName() As String
            Get
                Return Me._ShipName
            End Get
            Set(ByVal value As String)
                Me.OnShipNameChanging(value)
                Me._ShipName = value
                Me.OnShipNameChanged()
            End Set
        End Property
        Private _ShipName As String
        Partial Private Sub OnShipNameChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnShipNameChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property ShipAddress in the schema.
        '''</summary>
        Public Property ShipAddress() As String
            Get
                Return Me._ShipAddress
            End Get
            Set(ByVal value As String)
                Me.OnShipAddressChanging(value)
                Me._ShipAddress = value
                Me.OnShipAddressChanged()
            End Set
        End Property
        Private _ShipAddress As String
        Partial Private Sub OnShipAddressChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnShipAddressChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property ShipCity in the schema.
        '''</summary>
        Public Property ShipCity() As String
            Get
                Return Me._ShipCity
            End Get
            Set(ByVal value As String)
                Me.OnShipCityChanging(value)
                Me._ShipCity = value
                Me.OnShipCityChanged()
            End Set
        End Property
        Private _ShipCity As String
        Partial Private Sub OnShipCityChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnShipCityChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property ShipRegion in the schema.
        '''</summary>
        Public Property ShipRegion() As String
            Get
                Return Me._ShipRegion
            End Get
            Set(ByVal value As String)
                Me.OnShipRegionChanging(value)
                Me._ShipRegion = value
                Me.OnShipRegionChanged()
            End Set
        End Property
        Private _ShipRegion As String
        Partial Private Sub OnShipRegionChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnShipRegionChanged()
        End Sub
        '''<summary>
        '''There are no comments for Property ShipPostalCode in the schema.
        '''</summary>
        Public Property ShipPostalCode() As String
            Get
                Return Me._ShipPostalCode
            End Get
            Set(ByVal value As String)
                Me.OnShipPostalCodeChanging(value)
                Me._ShipPostalCode = value
                Me.OnShipPostalCodeChanged()
            End Set
        End Property
        Private _ShipPostalCode As String
        Partial Private Sub OnShipPostalCodeChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnShipPostalCodeChanged()
        End Sub
        '''<summary>
        '''There are no comments for Customers in the schema.
        '''</summary>
        Public Property Customers() As Customers
            Get
                Return Me._Customers
            End Get
            Set(ByVal value As Customers)
                Me._Customers = value
            End Set
        End Property
        Private _Customers As Customers
        '''<summary>
        '''There are no comments for Employees in the schema.
        '''</summary>
        Public Property Employees() As Employees
            Get
                Return Me._Employees
            End Get
            Set(ByVal value As Employees)
                Me._Employees = value
            End Set
        End Property
        Private _Employees As Employees
        '''<summary>
        '''There are no comments for Order_Details in the schema.
        '''</summary>
        Public Property Order_Details() As Global.Microsoft.OData.Client.DataServiceCollection(Of Order_Detail_ComplexType)
            Get
                Return Me._Order_Details
            End Get
            Set(ByVal value As Global.Microsoft.OData.Client.DataServiceCollection(Of Order_Detail_ComplexType))
                If (Not (value) Is Nothing) Then
                    Me._Order_Details = value
                End If
            End Set
        End Property
        Private _Order_Details As Global.Microsoft.OData.Client.DataServiceCollection(Of Order_Detail_ComplexType) = New Microsoft.OData.Client.DataServiceCollection(Of Order_Detail_ComplexType)(Nothing, Microsoft.OData.Client.TrackingMode.None)
        '''<summary>
        '''There are no comments for Shippers in the schema.
        '''</summary>
        Public Property Shippers() As Shippers
            Get
                Return Me._Shippers
            End Get
            Set(ByVal value As Shippers)
                Me._Shippers = value
            End Set
        End Property
        Private _Shippers As Shippers
    End Class

    Partial Public Class Order_Detail_ComplexType
        Public Property UnitPrice() As Decimal
            Get
                Return Me._UnitPrice
            End Get
            Set(ByVal value As Decimal)
                Me._UnitPrice = value
            End Set
        End Property
        Private _UnitPrice As Decimal

        Public Property Quantity() As Short
            Get
                Return Me._Quantity
            End Get
            Set(ByVal value As Short)
                Me._Quantity = value
            End Set
        End Property
        Private _Quantity As Short

        Public Property Discount() As Single
            Get
                Return Me._Discount
            End Get
            Set(ByVal value As Single)
                Me._Discount = value
            End Set
        End Property
        Private _Discount As Single
    End Class

#Region "Order_Details with missing Orders & Products (links)"
    <Global.Microsoft.OData.Client.KeyAttribute("OrderID", "ProductID")>
    Partial Public Class Order_Details_MissingLinks
        Public Property OrderID() As Integer
            Get
                Return Me._OrderID
            End Get
            Set(ByVal value As Integer)
                Me._OrderID = value
            End Set
        End Property
        Private _OrderID As Integer

        Public Property ProductID() As Integer
            Get
                Return Me._ProductID
            End Get
            Set(ByVal value As Integer)
                Me._ProductID = value
            End Set
        End Property
        Private _ProductID As Integer

        Public Property UnitPrice() As Decimal
            Get
                Return Me._UnitPrice
            End Get
            Set(ByVal value As Decimal)
                Me._UnitPrice = value
            End Set
        End Property
        Private _UnitPrice As Decimal

        Public Property Quantity() As Short
            Get
                Return Me._Quantity
            End Get
            Set(ByVal value As Short)
                Me._Quantity = value
            End Set
        End Property
        Private _Quantity As Short

        Public Property Discount() As Single
            Get
                Return Me._Discount
            End Get
            Set(ByVal value As Single)
                Me._Discount = value
            End Set
        End Property
        Private _Discount As Single
    End Class
#End Region
End Class
