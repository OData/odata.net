'---------------------------------------------------------------------
' <copyright file="ServerDrivenPaging.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Text
Imports AstoriaClientUnitTests.AstoriaClientUnitTests.Stubs
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Partial Public Class ClientModule
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    'Remove Atom
    ' <TestClass()>
    Public Class ServerDrivenPagingTests
        Inherits AstoriaTestCase

        Private Shared cleanups As List(Of IDisposable) = New List(Of IDisposable)
        Private Shared web As AstoriaUnitTests.Stubs.TestWebRequest = Nothing
        Private ctx As AstoriaClientUnitTests.Stubs.CustomDataContext = Nothing

#Region "Additional test attributes"

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)

            web = AstoriaUnitTests.Stubs.TestWebRequest.CreateForInProcessWcf()
            web.DataServiceType = AstoriaUnitTests.Data.ServiceModelData.CustomData.ServiceModelType

            web.StartService()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()


            If Not web Is Nothing Then
                web.StopService()
            End If

        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            cleanups.Add(AstoriaUnitTests.Stubs.OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
            cleanups.Add(TestUtil.RestoreStaticValueOnDispose(GetType(BaseTestWebRequest), "SerializedTestArguments"))
            cleanups.Add(TestUtil.MetadataCacheCleaner)
            AstoriaUnitTests.Stubs.OpenWebDataServiceHelper.PageSizeCustomizer.Value = AddressOf PageSizeCustomizer

            Me.ctx = New CustomDataContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()

            BaseTestWebRequest.SerializedTestArguments = New Hashtable()
            BaseTestWebRequest.SerializedTestArguments("CustomDataContext.CustomerCount") = 10      ' 10 custs
            BaseTestWebRequest.SerializedTestArguments("CustomDataContext.OrderCountArgument") = 10 ' 10x10 orders
            BaseTestWebRequest.SerializedTestArguments("CustomDataContext.OrderDetailsCountArgument") = 5 ' 10x10x5 orders

        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing

            For Each Item In cleanups
                Item.Dispose()
            Next
            cleanups.Clear()
            BaseTestWebRequest.SerializedTestArguments.Clear()
        End Sub

        Public Shared Sub PageSizeCustomizer(ByVal config As DataServiceConfiguration, ByVal modelType As Type)
            config.SetEntitySetPageSize("Customers", 3)
            config.SetEntitySetPageSize("Orders", 5)
            config.SetEntitySetPageSize("OrderDetails", 2)
        End Sub
#End Region

        <TestCategory("Partition3")> <TestMethod(), Variation("Make sure page size customizer is working correctly")>
        Public Sub PagingSizeTest()
            Assert.AreEqual(3, ctx.CreateQuery(Of Customer)("/Customers").Execute().Count())
            Assert.AreEqual(5, ctx.CreateQuery(Of Order)("/Orders").Execute().Count())
        End Sub

        <TestCategory("Partition3")> <TestMethod(), Variation("Non tracking DSC factory and load testing")>
        Public Sub DSC_FactoryAndLoad()
            Dim q = ctx.Customers               ' first page = 1,2,3 
            Dim q2 = ctx.Customers.Skip(2)      ' skipped 2 = 3,4,5 (total should be 5)

            Dim custs = New DataServiceCollection(Of Customer)(q, TrackingMode.None)
            Assert.AreEqual(custs.Count, 3)

            custs.Load(q2)
            ' Load should not load duplicated entities
            Assert.AreEqual(custs.Count, 5)

            Dim custs2 = New DataServiceCollection(Of Customer)(Nothing, TrackingMode.None)
            ' Loading another DSC
            custs2.Load(custs)
            Assert.AreEqual(custs2.Count, 5)
            custs2.Load(ctx.Customers.FirstOrDefault())
            Assert.AreEqual(custs2.Count, 5)
            custs2.Load(ctx.Customers.Skip(5).FirstOrDefault())
            Assert.AreEqual(custs2.Count, 6)

            Dim custs_tracking = New DataServiceCollection(Of Customer)(ctx)
            custs_tracking.Load(q)
            Assert.AreEqual(custs_tracking.Count, 3)
        End Sub

        <TestCategory("Partition3")> <TestMethod(), Variation("DSC Loading multi level")>
        Public Sub DSC_LoadMultiLevelSimple()
            Dim q = ctx.Customers.Expand("Orders")
            Dim custs = New DataServiceCollection(Of Customer)(q, TrackingMode.None)
            Assert.AreEqual(3, custs.Count)
            Assert.AreEqual(5, custs.FirstOrDefault().Orders.Count)
        End Sub

        <TestCategory("Partition3")> <TestMethod(), Variation("Retrieving Links from QOR")>
        Public Sub QOR_GetLinksMultiLevel()
            Dim q = ctx.Customers.Expand("Orders")
            Dim qor = CType(q.Execute(), QueryOperationResponse(Of Customer))
            Dim links = New List(Of String)()

            Try
                qor.GetContinuation()
                Assert.Fail("Get link before enumeration failed to throw")
            Catch ex As InvalidOperationException

            End Try

            For Each c In qor
                Dim orderLink = qor.GetContinuation(c.Orders)
                If (orderLink IsNot Nothing) Then
                    links.Add(orderLink.ToString())
                End If
            Next
            Dim topLevelLink = qor.GetContinuation().NextLinkUri
            links.Add(topLevelLink.ToString())
            ' 3 orders + 1 customer links
            Assert.AreEqual(4, links.Count)
        End Sub

        <TestCategory("Partition3")> <TestMethod(), Variation("Retrieving links from DSC")>
        Public Sub DSC_GetLinksMultiLevel()
            Dim q = ctx.Customers.Expand("Orders")
            For Each custs In New DataServiceCollection(Of Customer)() {
                New DataServiceCollection(Of Customer)(q, TrackingMode.None),
                New DataServiceCollection(Of Customer)(ctx, q, TrackingMode.AutoChangeTracking, Nothing, Nothing, Nothing)}

                Assert.IsNotNull(custs.Continuation)
                Dim linksCount = 0
                For Each c In custs
                    If (c.Orders.Continuation IsNot Nothing) Then
                        linksCount = linksCount + 1
                    End If
                Next
                Assert.AreEqual(3, linksCount)

                custs.Clear()
                Assert.IsNotNull(custs.Continuation)
                Assert.AreEqual(0, custs.Count)

                ' we should be able to get links from QOR as well
                Dim qor = q.Execute()
                custs.Load(qor)
                Assert.IsNotNull(custs.Continuation)
            Next

        End Sub

        <TestCategory("Partition3")> <TestMethod(), Variation("Load Property paging")>
        Public Sub CTX_LoadPropertyGetLinks()
            Dim c = ctx.Customers.FirstOrDefault()
            Dim qor = ctx.LoadProperty(c, "Orders")
            Assert.AreEqual(5, c.Orders.Count)

            Dim nextLink = qor.GetContinuation().NextLinkUri
            Dim nextLinkFromDSC = c.Orders.Continuation

            Assert.IsNotNull(nextLink)
            Assert.AreEqual(nextLink, nextLinkFromDSC.NextLinkUri)
        End Sub

        <TestCategory("Partition3")> <TestMethod(), Variation("LoadPropertyPage loading call")>
        Public Sub CTX_LoadPropertyPageLoading()
            Dim options = New MergeOption() {MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges}

            Dim c = ctx.Customers.FirstOrDefault()

            For Each op In options
                ctx.MergeOption = op
                c.Orders.Clear()

                Dim qor = ctx.LoadProperty(c, "Orders")
                Assert.AreEqual(5, c.Orders.Count)

                Dim nextLink = c.Orders.Continuation

                ' Calling LoadProperty again should not add any new orders:
                ctx.LoadProperty(c, "Orders")
                Assert.AreEqual(5, c.Orders.Count)

                Dim o = c.Orders.FirstOrDefault()
                o.DollarAmount = 1
                ctx.UpdateObject(o)
                Assert.AreEqual(1, (From entity In ctx.Entities Where entity.State = EntityStates.Modified Select entity).Count)

                ' LoadPropertyPage with Uri should grab the next page (just one more order)
                ctx.LoadProperty(c, "Orders", nextLink)
                Assert.AreEqual(10, c.Orders.Count)
                Assert.AreNotEqual(nextLink, c.Orders.Continuation)

                ' first page changes should be un-touched
                Assert.AreEqual(1, (From entity In ctx.Entities Where entity.State = EntityStates.Modified Select entity).Count)
            Next
        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub DSC_NonTrackingFullLoad()
            Dim q = ctx.CreateQuery(Of Customer)("Customers").Expand("Orders")
            Dim custs = New DataServiceCollection(Of Customer)(q, TrackingMode.None)

            ' Load outer collection first.
            While (custs.Continuation IsNot Nothing)
                custs.Load(ctx.Execute(Of Customer)(custs.Continuation))
            End While

            Dim i = 0
            For Each c In custs
                While (c.Orders.Continuation IsNot Nothing)
                    If i Mod 2 = 0 Then
                        c.Orders.Load(ctx.Execute(Of Order)(c.Orders.Continuation))
                    Else
                        ctx.LoadProperty(c, "Orders", c.Orders.Continuation)
                    End If
                End While
                i = i + 1
                Assert.AreEqual(10, c.Orders.Count)
            Next
            Assert.AreEqual(10, i)
        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub DSC_TrackingFullLoad()
            Dim q = ctx.CreateQuery(Of Customer)("Customers").Expand("Orders")
            Dim custs = New DataServiceCollection(Of Customer)(ctx)
            custs.Load(q)

            ' Load outer collection first
            While (custs.Continuation IsNot Nothing)
                custs.Load(ctx.Execute(Of Customer)(custs.Continuation))
            End While

            Dim i = 0
            For Each c In custs
                While (c.Orders.Continuation IsNot Nothing)
                    If i Mod 2 = 0 Then
                        c.Orders.Load(ctx.Execute(Of Order)(c.Orders.Continuation))
                    Else
                        ctx.LoadProperty(c, "Orders", c.Orders.Continuation)
                    End If
                End While
                i = i + 1
                Assert.AreEqual(10, c.Orders.Count)
            Next
            Assert.AreEqual(10, i)
        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub DSC_ClearItems()
            Dim q = ctx.CreateQuery(Of Customer)("Customers")
            Dim custs = New DataServiceCollection(Of Customer)(ctx)
            custs.Load(q)

            Assert.IsNotNull(custs.Continuation)
            custs.Clear(False)
            Assert.IsNotNull(custs.Continuation)
        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub DSC_NegativeTrackingAPITests()
            Dim custs = New DataServiceCollection(Of Customer)(Nothing, TrackingMode.None)

            Try
                custs.Detach()
                Assert.Fail("Detech on an already-detached DSC did not throw")
            Catch ex As InvalidOperationException
            End Try

            Try
                custs.Clear(True)
                Assert.Fail("Clear with detaching flag on non-tracking DSC did not throw")
            Catch ex As InvalidOperationException
            End Try

        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub QOR_GetNextLinkNegative()
            Dim qor = CType(ctx.CreateQuery(Of Customer)("Customers").Expand("Orders").Execute(), QueryOperationResponse(Of Customer))

            Dim previousOrderCollection As ICollection = Nothing
            For Each cust In qor
                If Not previousOrderCollection Is Nothing Then
                    Try
                        qor.GetContinuation(previousOrderCollection)
                        Assert.Fail("Out of scope collection as key did not throw")
                    Catch ex As ArgumentException
                    End Try
                End If

                previousOrderCollection = cust.Orders
            Next

        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub CTX_LoadPrimitivePropertyWithLink()
            Dim cust = ctx.CreateQuery(Of Customer)("Customers").First()
            Dim descriptor = ctx.GetEntityDescriptor(cust)
            ctx.LoadProperty(cust, "Name", New Uri(descriptor.EditLink.ToString() & "/Name"))
            ctx.LoadProperty(cust, "BestFriend", New Uri(descriptor.EditLink.ToString() & "/BestFriend"))

        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub QOR_FullLoad()
            Dim q = ctx.CreateQuery(Of Customer)("Customers").Expand("Orders($expand=OrderDetails)")
            Dim qor = CType(q.Execute(), QueryOperationResponse(Of Customer))

            Dim nextCustLink As DataServiceQueryContinuation(Of AstoriaClientUnitTests.Stubs.Customer)

            Dim custCount = 0, orderCount = 0, odCount = 0
            Dim isContinuedOrders = False

            Dim i = 0
            Do
                For Each c In qor

                    Dim nextOrderLink = qor.GetContinuation(c.Orders)
                    Dim ordersEnumerable As IEnumerable(Of Order) = c.Orders

                    Do
                        For Each o In ordersEnumerable
                            Dim nextODLink As DataServiceQueryContinuation(Of OrderDetail)

                            If (isContinuedOrders) Then
                                nextODLink = CType(ordersEnumerable, QueryOperationResponse(Of Order)).GetContinuation(o.OrderDetails)
                            Else
                                nextODLink = qor.GetContinuation(o.OrderDetails)
                            End If

                            While nextODLink IsNot Nothing
                                If (i Mod 2 = 0) Then
                                    Dim odQOR = ctx.Execute(nextODLink)
                                    For Each innerOD In odQOR
                                        ctx.AttachLink(o, "OrderDetails", innerOD)
                                        o.OrderDetails.Add(innerOD)
                                    Next
                                    nextODLink = odQOR.GetContinuation()
                                Else
                                    nextODLink = ctx.LoadProperty(o, "OrderDetails", nextODLink).GetContinuation()
                                End If
                            End While

                            odCount = o.OrderDetails.Count + odCount

                            If (isContinuedOrders) Then
                                c.Orders.Add(o)
                                ctx.AttachLink(c, "Orders", o)
                            End If

                            orderCount = orderCount + 1
                        Next

                        If (isContinuedOrders) Then
                            nextOrderLink = CType(ordersEnumerable, QueryOperationResponse(Of Order)).GetContinuation()
                        End If

                        If (nextOrderLink IsNot Nothing) Then
                            ' more pages of orders
                            isContinuedOrders = True
                            ordersEnumerable = ctx.Execute(nextOrderLink)
                        Else
                            isContinuedOrders = False
                        End If

                    Loop Until nextOrderLink Is Nothing

                    Assert.AreEqual(10, c.Orders.Count)
                    i = i + 1

                    custCount = custCount + 1
                Next

                nextCustLink = qor.GetContinuation()

                If (nextCustLink IsNot Nothing) Then
                    ' new page
                    qor = ctx.Execute(nextCustLink)
                End If

            Loop Until nextCustLink Is Nothing

            Assert.AreEqual(10, custCount)
            Assert.AreEqual(100, orderCount)
            Assert.AreEqual(500, odCount)
        End Sub

#Region "Set Next Link by Convention"

        Public Class MyPagableCollection(Of T)
            Inherits List(Of T)

            Private m_continuation As DataServiceQueryContinuation(Of T)
            Public Property Continuation() As DataServiceQueryContinuation(Of T)
                Get
                    Return m_continuation
                End Get
                Set(ByVal value As DataServiceQueryContinuation(Of T))
                    m_continuation = value
                End Set
            End Property
        End Class

        <EntityType()>
        Public Class NarrowCustomerWithPagableOrders

            Private m_Orders As MyPagableCollection(Of Order) = New MyPagableCollection(Of Order)()
            Public Property Orders() As MyPagableCollection(Of Order)
                Get
                    Return m_Orders
                End Get
                Set(ByVal value As MyPagableCollection(Of Order))
                    m_Orders = value
                End Set
            End Property
        End Class

        <TestCategory("Partition3")> <TestMethod()> Public Sub QOR_SetNextLinkByConvention()
            Dim qor = CType(ctx.Execute(Of NarrowCustomerWithPagableOrders)(New Uri("/Customers?$select=Orders&$expand=Orders", UriKind.Relative)), QueryOperationResponse(Of NarrowCustomerWithPagableOrders))

            For Each narrowCust In qor
                Assert.IsNotNull(narrowCust.Orders.Continuation)
                ctx.LoadProperty(narrowCust, "Orders", narrowCust.Orders.Continuation)
                Assert.AreEqual(10, narrowCust.Orders.Count)
            Next
        End Sub


#End Region

        <TestCategory("Partition3")> <TestMethod()> Public Sub QOR_NextLinkParsing()
            Dim testCases = {
                    New With {
                        .Description = "Normal next link.",
                        .NextLink = "<link rel='next' href='http://odata.org/nextlink'/>",
                        .ContinuationToken = "http://odata.org/nextlink",
                        .ExpectedException = CType(Nothing, String)
                    },
                    New With {
                        .Description = "Empty href - should be treated as relative URL.",
                        .NextLink = "<link rel='next' href=''/>",
                        .ContinuationToken = "http://odata.org/baseuri/",
                        .ExpectedException = CType(Nothing, String)
                    },
                    New With {
                        .Description = "Missing href - succeeds.",
                        .NextLink = "<link rel='next'/>",
                        .ContinuationToken = CType(Nothing, String),
                        .ExpectedException = CType(Nothing, String)
                    },
                    New With {
                        .Description = "Duplicate next link - fails",
                        .NextLink = "<link rel='next' href='http://odata.org/nextlink'/><link rel='next' href='http://odata.org/nextlink2'/>",
                        .ContinuationToken = CType(Nothing, String),
                        .ExpectedException = AstoriaUnitTests.ODataLibResourceUtil.GetString("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", "next")
                    }
                }

            Dim service = New AstoriaUnitTests.Stubs.PlaybackServiceDefinition()
            Using request = service.CreateForInProcessWcf
                request.StartService()

                TestUtil.RunCombinations(testCases,
                    Sub(testCase)
                        service.OverridingPlayback =
                            "HTTP/1.1 200 OK" + Environment.NewLine +
                            "Content-Type: application/atom+xml" + Environment.NewLine + Environment.NewLine +
                            "<feed xmlns='http://www.w3.org/2005/Atom' xml:base='http://odata.org/baseuri/'>" +
                            testCase.NextLink +
                            "</feed>"
                        Dim context = New DataServiceContext(request.ServiceRoot)
                        'context.EnableAtom = True
                        Dim qor = CType(context.Execute(Of Customer)("/Items"), QueryOperationResponse(Of Customer))
                        Dim exception = TestUtil.RunCatching(Sub()
                                                                 qor.AsEnumerable().Count()
                                                             End Sub)

                        If exception Is Nothing Then
                            Dim continuation = qor.GetContinuation()
                            Assert.IsNull(testCase.ExpectedException, "The test case should have failed.")
                            If testCase.ContinuationToken Is Nothing Then
                                Assert.IsNull(continuation, "Found a continuation when none should exist.")
                            Else
                                Assert.AreEqual(testCase.ContinuationToken, continuation.NextLinkUri.AbsoluteUri, "Wrong next link URI read.")
                            End If
                        Else
                            Assert.AreEqual(testCase.ExpectedException, exception.Message, "Unexpected error message.")
                        End If
                    End Sub)
            End Using
        End Sub

    End Class
End Class
