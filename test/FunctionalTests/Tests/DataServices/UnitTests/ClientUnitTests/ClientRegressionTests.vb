'---------------------------------------------------------------------
' <copyright file="ClientRegressionTests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Option Strict Off

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports System.ServiceModel.Web
Imports System.Xml.Linq
Imports AstoriaUnitTests
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports AstoriaUnitTests.Tests
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NorthwindModel
Imports AstoriaUnitTests.ClientExtensions
Imports <xmlns:atom="http://www.w3.org/2005/Atom">
Imports <xmlns:d="http://docs.oasis-open.org/odata/ns/data">
Imports <xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">

' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
<TestClass()> Public Class ClientRegressionTests
    Inherits AstoriaTestCase

    Private Shared web As TestWebRequest = Nothing
    Private Shared resolvedNames As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
    Private ctx As NorthwindSimpleModel.NorthwindContext = Nothing

#Region "Additional test attributes"

    <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
        BaseTestWebRequest.SerializedTestArguments.Clear()
        web = TestWebRequest.CreateForInProcessWcf
        web.DataServiceType = ServiceModelData.Northwind.ServiceModelType
        web.StartService()
    End Sub

    <ClassCleanup()> Public Shared Sub PerClassCleanup()
        If Not web Is Nothing Then
            web.StopService()
        End If
    End Sub

    <TestInitialize()> Public Sub PerTestSetup()
        Me.ctx = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot, ODataProtocolVersion.V4)
        'Me.'ctx.EnableAtom = True
        'Me.'ctx.Format.UseAtom()
    End Sub

    <TestCleanup()> Public Sub PerTestCleanup()
        Me.ctx = Nothing
    End Sub
#End Region

    <TestCategory("Partition1")> <TestMethod()> Public Sub ContextIgnoreRNF()
        Assert.IsFalse(Me.ctx.IgnoreResourceNotFoundException)
        Dim cust As northwindClient.Customers

        For i As Integer = 0 To 1
            Me.ctx.IgnoreResourceNotFoundException = If(i = 0, True, False)

            ' 1. LINQ Queries (DataServiceQueryOfT)
            Dim q = From c In Me.ctx.CreateQuery(Of northwindClient.Customers)("Customers") Where c.CustomerID = "FOO" Select c

            Try
                cust = q.SingleOrDefault
                Assert.AreEqual(0, i)
                Assert.IsTrue(cust Is Nothing)
            Catch ex As DataServiceQueryException
                Dim innerEx As DataServiceClientException = DirectCast(ex.InnerException, DataServiceClientException)
                Assert.AreEqual(404, innerEx.StatusCode, "LINQ query")
                Assert.AreEqual(1, i)
            End Try

            ' 2. Direct URI (DataServiceRequestOfT)
            Try
                Dim enumerable = ctx.Execute(Of northwindClient.Customers)("/Customers('FOO')")
                Assert.AreEqual(0, i)
                Assert.AreEqual(enumerable.Count, 0)
            Catch ex As DataServiceQueryException
                Dim innerEx As DataServiceClientException = DirectCast(ex.InnerException, DataServiceClientException)
                Assert.AreEqual(404, innerEx.StatusCode, "Direct URI")
                Assert.AreEqual(1, i)
            End Try

            ' 3. Batching
            Dim response = ctx.ExecuteBatch(CType(q, DataServiceRequest), CType(q, DataServiceRequest))
            For Each r As QueryOperationResponse(Of northwindClient.Customers) In response
                Try
                    cust = r.SingleOrDefault
                    Assert.AreEqual(0, i)
                    Assert.IsTrue(cust Is Nothing)
                Catch ex As InvalidOperationException
                    Dim innerEx As DataServiceClientException = DirectCast(ex.InnerException, DataServiceClientException)
                    Assert.AreEqual(404, innerEx.StatusCode, "Batching")
                    Assert.AreEqual(1, i)
                End Try
            Next
        Next

        ctx.IgnoreResourceNotFoundException = True

        ' 4. Faulty Identity on Load Property
        cust = New northwindClient.Customers()
        cust.CustomerID = "Foo1"
        ctx.AttachTo("Customers", cust)
        Try
            Dim r = ctx.LoadProperty(cust, "Address")
            Assert.Fail("Load property failed to throw on 404")
        Catch ex As DataServiceClientException
            Assert.AreEqual(404, ex.StatusCode)
        End Try

        ' 5. update object
        cust.Region = "Region1"
        Try
            ctx.UpdateObject(cust)
            ctx.SaveChanges()
            Assert.Fail("Update Object failed to throw on 404")
        Catch ex As DataServiceRequestException
            Dim innerEx As DataServiceClientException = DirectCast(ex.InnerException, DataServiceClientException)
            Assert.AreEqual(404, innerEx.StatusCode)
        End Try

    End Sub

    <TestCategory("Partition1")> <TestMethod()>
    Public Sub ContextIgnoreRNF_Async()

        For i As Integer = 0 To 1
            Me.ctx.IgnoreResourceNotFoundException = If(i = 0, True, False)

            Try
                Dim result = ctx.BeginExecute(Of northwindClient.Customers)(New Uri("Customers('FOO')", UriKind.Relative), Nothing, Nothing)
                Dim cust = ctx.EndExecute(Of northwindClient.Customers)(result).SingleOrDefault

                Assert.AreEqual(i, 0)
                Assert.IsTrue(cust Is Nothing)
            Catch ex As DataServiceQueryException
                Dim innerEx As DataServiceClientException = DirectCast(ex.InnerException, DataServiceClientException)
                Assert.AreEqual(innerEx.StatusCode, 404)
                Assert.AreEqual(i, 1)
            End Try
        Next
    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("Expanding with no links could result in ArgumentException thrown by LINQ Except method call")>
    Public Sub ExpandWithOverwriteChanges()
        Me.ctx.MergeOption = MergeOption.OverwriteChanges
        Dim q = From c In ctx.CreateQuery(Of northwindClient.Employees)("Employees").Expand("Employees1") Select c

        Dim qor = (CType(q, DataServiceQuery(Of northwindClient.Employees)).Execute())

        Try
            For Each c As northwindClient.Employees In qor

            Next
        Catch ex As ArgumentNullException
            Assert.Fail("Exception was thrown")
        End Try
    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod()>
    Public Sub TestBatchWithSingleChangeset()
        Dim c1 = Me.ctx.Execute(Of northwindClient.Customers)(New Uri("Customers", UriKind.Relative)).First()
        Me.ctx.UpdateObject(c1)

        Dim o1 = New northwindClient.Orders()
        o1.OrderID = 9999
        Me.ctx.AddObject("Orders", o1)

        Me.ctx.AddLink(c1, "Orders", o1)
        ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)

        'clear the context
        ctx.DeleteObject(o1)
        ctx.SaveChanges()

    End Sub

    <TestCategory("Partition1")> <TestMethod(), Variation("Client fails with ArgumentNullException if the server responds with 204 and a Content-Type (using async API)")>
    Public Sub ShouldAcceptResponse204()
        Using CustomDataContext.CreateChangeScope()
            Using request = TestWebRequest.CreateForInProcessWcf
                request.DataServiceType = GetType(CustomDataContext)
                request.StartService()

                ' Try both sync and async since the behavior was different for these cases
                Dim c = New AstoriaClientUnitTests.Stubs.CustomDataContext(request.ServiceRoot)
                Dim results = c.Execute(Of AstoriaClientUnitTests.Stubs.Customer)(New Uri("/Customers(ID=0)/BestFriend", UriKind.Relative))
                Assert.AreEqual(0, results.Count(), "Customer 0 doesn't have a best friend, so the result collection should be empty.")
                Dim ar = c.BeginExecute(Of AstoriaClientUnitTests.Stubs.Customer)(New Uri("/Customers(ID=0)/BestFriend", UriKind.Relative), Nothing, Nothing)
                results = c.EndExecute(Of AstoriaClientUnitTests.Stubs.Customer)(ar)
                Assert.AreEqual(0, results.Count(), "Customer 0 doesn't have a best friend, so the result collection should be empty.")
            End Using
        End Using
    End Sub

    Public Class InheritedOrder
        Inherits AstoriaClientUnitTests.Stubs.Order
    End Class

    Private Sub PerformExpand(ByVal c As DataServiceContext)
        Dim query = c.CreateQuery(Of AstoriaClientUnitTests.Stubs.Customer)("Customers").Expand("Orders")
        For Each entry As AstoriaClientUnitTests.Stubs.Customer In query
        Next
    End Sub

    Private Sub PerformLoadProperty(ByVal c As DataServiceContext)
        Dim query = c.CreateQuery(Of AstoriaClientUnitTests.Stubs.Customer)("Customers")
        For Each entry As AstoriaClientUnitTests.Stubs.Customer In query
            c.LoadProperty(entry, "Orders")
        Next
    End Sub

    Private Sub TestTypeResolverWithExpandOrLoadProperty(ByVal values As Hashtable)
        Try
            resolvedNames.Clear()
            Using CustomDataContext.CreateChangeScope()
                Using TestUtil.MetadataCacheCleaner()
                    Using request = TestWebRequest.CreateForInProcessWcf
                        request.DataServiceType = GetType(CustomDataContext)
                        request.StartService()
                        ' Try both sync and async since the behavior was different for these cases
                        Dim c = New AstoriaClientUnitTests.Stubs.CustomDataContext(request.ServiceRoot)
                        Dim useInheritance = CType(values("UseInheritance"), Boolean)
                        If useInheritance = True Then
                            c.ResolveType = AddressOf ResolveTypeWithExpandInheritance
                        Else
                            c.ResolveType = AddressOf ResolveTypeWithExpand
                        End If
                        Dim methodToCall = CType(values("ExpandMethod"), Action(Of DataServiceContext))
                        methodToCall(c)
                        MatchDictionaryWithKeyValuePairs(resolvedNames, New KeyValuePair(Of String, Integer)() {
                                                         New KeyValuePair(Of String, Integer)("AstoriaUnitTests.Stubs.Customer", 2),
                                                         New KeyValuePair(Of String, Integer)("AstoriaUnitTests.Stubs.CustomerWithBirthday", 1),
                                                         New KeyValuePair(Of String, Integer)("AstoriaUnitTests.Stubs.Address", 3),
                                                         New KeyValuePair(Of String, Integer)("AstoriaUnitTests.Stubs.Order", 6)})
                    End Using
                End Using
            End Using
        Finally
            resolvedNames.Clear()
        End Try
    End Sub

    Private Function ResolveTypeWithExpand(ByVal typeName As String) As Type
        IncrementTypeEntry(typeName)
        Return Assembly.GetExecutingAssembly().GetType(typeName.Replace("AstoriaUnitTests", "AstoriaClientUnitTests.AstoriaClientUnitTests"))
    End Function

    Private Function ResolveTypeWithExpandInheritance(ByVal typeName As String) As Type
        IncrementTypeEntry(typeName)

        If typeName.Contains("Order") Then
            Return GetType(InheritedOrder)
        End If

        Return Assembly.GetExecutingAssembly().GetType(typeName.Replace("AstoriaUnitTests", "AstoriaClientUnitTests.AstoriaClientUnitTests"))
    End Function

    Private Sub IncrementTypeEntry(ByVal typeName As String)
        Dim count As Integer
        If (resolvedNames.TryGetValue(typeName, count) = True) Then
            resolvedNames(typeName) = resolvedNames(typeName) + 1
        Else
            resolvedNames(typeName) = 1
        End If
    End Sub

    Private Sub MatchDictionaryWithKeyValuePairs(ByVal d As Dictionary(Of String, Integer), ByVal typeCountPairs As KeyValuePair(Of String, Integer)())
        For Each entry In typeCountPairs
            System.Diagnostics.Trace.WriteLine("Matching results: " & "Key : " & entry.Key & "(" & d(entry.Key) & " : " & entry.Value & ")")
            Assert.IsTrue(d(entry.Key) = entry.Value)
        Next
    End Sub

    <TestCategory("Partition1")> <TestMethod(), Variation("Client InvalidOperationException on 204 No-Content response for Null reference properties in Async Code path")>
    Public Sub ShouldAcceptResponse204InAsync()
        Using CustomDataContext.CreateChangeScope()
            Using request = TestWebRequest.CreateForLocal
                request.DataServiceType = GetType(CustomDataContext)
                request.StartService()

                ' Try both sync and async since the behavior was different for these cases
                Dim c = New AstoriaClientUnitTests.Stubs.CustomDataContext(request.ServiceRoot)
                Dim results = c.Execute(Of AstoriaClientUnitTests.Stubs.Customer)(New Uri("/Customers(ID=0)/BestFriend", UriKind.Relative))
                Assert.AreEqual(0, results.Count(), "Customer 0 doesn't have a best friend, so the result collection should be empty.")
                Dim ar = c.BeginExecute(Of AstoriaClientUnitTests.Stubs.Customer)(New Uri("/Customers(ID=0)/BestFriend", UriKind.Relative), Nothing, Nothing)
                results = c.EndExecute(Of AstoriaClientUnitTests.Stubs.Customer)(ar)
                Assert.AreEqual(0, results.Count(), "Customer 0 doesn't have a best friend, so the result collection should be empty.")
            End Using
        End Using
    End Sub



#Region "AssertWhenLoadNullProperty"
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod()>
    Public Sub AssertWhenLoadNullProperty()
        ' EF 
        Dim employee = (From e In ctx.Employees Where e.EmployeeID = 2).FirstOrDefault
        Dim qor = ctx.LoadProperty(employee, "Employees2")
        Assert.AreEqual(qor.StatusCode, 204)
    End Sub

#End Region

#Region "Test DatetimeOffset"

    Public Class TestComplexType
        Private _date As Nullable(Of Global.System.DateTimeOffset)

        Public Property Day() As Nullable(Of Global.System.DateTimeOffset)
            Get
                Return _date
            End Get
            Set(ByVal value As Nullable(Of Global.System.DateTimeOffset))
                _date = value
            End Set
        End Property
    End Class

    <Global.Microsoft.OData.Client.Key("Id")>
    Public Class TestEntity

        Private _id As Integer
        Private _startDate As TestComplexType
        Private _endDate As TestComplexType

        Public Property Id() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Public Property StartDate() As TestComplexType
            Get
                Return _startDate
            End Get
            Set(ByVal value As TestComplexType)
                _startDate = value
            End Set
        End Property

        Public Property EndDate() As TestComplexType
            Get
                Return _endDate
            End Get
            Set(ByVal value As TestComplexType)
                _endDate = value
            End Set
        End Property

        Public Sub New(ByVal id As Integer, ByVal startDate As Nullable(Of Global.System.DateTimeOffset), ByVal endDate As Nullable(Of Global.System.DateTimeOffset))
            Me.Id = id
            Me.StartDate = New TestComplexType()
            Me.StartDate.Day = startDate
            Me.EndDate = New TestComplexType()
            Me.EndDate.Day = endDate
        End Sub

        Public Sub New()

        End Sub

    End Class

    Public Class TestDB
        Public ReadOnly Property Data() As IQueryable(Of TestEntity)
            Get
                Dim entities As TestEntity() = {New TestEntity(1, Nothing, Nothing)}
                Return entities.AsQueryable
            End Get
        End Property
    End Class

    Public Class TestCtx
        Inherits DataService(Of TestDB)

        Public Shared Sub InitializeService(ByVal config As DataServiceConfiguration)
            config.SetEntitySetAccessRule("*", EntitySetRights.All)
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4
        End Sub
    End Class
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod()>
    Public Sub TestDatetimeOffset()
        Using request = TestWebRequest.CreateForLocal
            request.DataServiceType = GetType(TestCtx)
            request.StartService()
            Dim ctx = New DataServiceContext(New Uri(request.BaseUri))
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim entity = ctx.CreateQuery(Of TestEntity)("Data").First
            Assert.AreEqual(1, entity.Id)
            Assert.AreEqual(Nothing, entity.StartDate.Day)
            Assert.AreEqual(Nothing, entity.EndDate.Day)
        End Using
    End Sub

#End Region

#Region "Content Feedback Service Tests"

    <TestClass()>
    Public Class ContentVerificationTests
        Inherits AstoriaTestCase

        Private Shared web As TestWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing

#Region "Initialization..."

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = AstoriaUnitTests.Stubs.TestWebRequest.CreateForInProcessWcf()
            web.ServiceType = GetType(ContentFeedbackVerificationService)
            web.StartService()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.StopService()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Me.ctx = New DataServiceContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub

#End Region

        <TestCategory("Partition2")> <TestMethod()> Public Sub BatchVersioning()
            Dim contentStream = FetchContentStream(
                    New Action(Function() ctx.ExecuteBatch(ctx.CreateQuery(Of northwindClient.Customers)("Customers").IncludeTotalCount(),
                    ctx.CreateQuery(Of northwindClient.Customers)("Customers"))))
            Assert.IsNotNull(contentStream)

            Dim r = New System.IO.StreamReader(contentStream)
            Dim boundary As String = r.ReadLine()
            Dim s() = r.ReadToEnd().Split(New String() {boundary}, StringSplitOptions.RemoveEmptyEntries)
            Assert.IsTrue(s(0).Contains("OData-Version: 4.0"))
            Assert.IsTrue(s(0).Contains("$count=true"))
            Assert.IsTrue(s(1).Contains("OData-Version: 4.0"))
            Assert.IsFalse(s(1).Contains("$count=true"))

        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub IncorrectLinkOnAdd_Existing()
            Dim custs = New Customer() With {.ID = 0}

            ctx.AttachTo("Customers", custs)
            Dim descriptor2 = ctx.GetEntityDescriptor(custs)

            Dim fi = GetType(EntityDescriptor).GetField("identity", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
            fi.SetValue(descriptor2, New Uri("urn:foo-$identity"))

            Dim newOrder = New Order() With {.ID = 0, .Customer = custs}
            ctx.AddObject("Orders", newOrder)
            ctx.SetLink(newOrder, "Customer", custs)

            Dim contentStream = FetchContentStream(New Action(Function() ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)))
            Dim content = New System.IO.StreamReader(contentStream).ReadToEnd()
            Assert.IsTrue(content.Contains(
                          "rel=""http://docs.oasis-open.org/odata/ns/related/Customer"" " +
                          "type=""application/atom+xml;type=entry"" title=""Customer"" " +
                          "href=""" + web.BaseUri + "/Customers(0)"))

        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub IncorrectLinkOnAdd_New()
            Dim custs = New Customer() With {.ID = 0}
            Dim newOrder = New Order() With {.ID = 0, .Customer = custs}
            ctx.AddObject("Customers", custs)
            ctx.AddObject("Orders", newOrder)
            ctx.SetLink(newOrder, "Customer", custs)

            Dim contentStream = FetchContentStream(New Action(Function() ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)))
            Dim content = New System.IO.StreamReader(contentStream).ReadToEnd()
            Assert.IsTrue(content.Contains("PUT $2/Customer/$ref"))

        End Sub

#Region "In class support"

        Private Function FetchContentStream(ByVal action As Action) As System.IO.Stream
            Try
                action.Invoke()
            Catch ex As System.Net.WebException
                Return ex.Response.GetResponseStream()
            End Try
            Return Nothing
        End Function

        <System.ServiceModel.ServiceContract()>
        <System.ServiceModel.ServiceBehavior(InstanceContextMode:=ServiceModel.InstanceContextMode.PerCall)>
        <System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode:=System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)>
        Public Class ContentFeedbackVerificationService
            <System.ServiceModel.OperationContract()>
            <System.ServiceModel.Web.WebInvoke(UriTemplate:="*", Method:="*")>
            Public Function ProcessRequestForMessage(ByVal messageBody As System.IO.Stream) As System.IO.Stream
                Dim c = System.ServiceModel.Web.WebOperationContext.Current
                c.OutgoingResponse.StatusCode = Net.HttpStatusCode.BadRequest
                c.OutgoingResponse.ContentType = "application/xml"

                Return messageBody
            End Function
        End Class

#End Region

    End Class

#End Region

#Region "Test DisposeAsyncWaitHandle"
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod()>
    Public Sub DisposeAsyncWaitHandle1()
        Using CustomDataContext.CreateChangeScope()
            Using request = TestWebRequest.CreateForLocal
                request.DataServiceType = GetType(CustomDataContext)
                request.StartService()

                ' Make sure EndExecute disposes the wait handle
                Dim ctx = New DataServiceContext(New Uri(request.BaseUri))
                'ctx.EnableAtom = True
                'ctx.Format.UseAtom()
                Dim asyncResult = ctx.BeginExecute(Of Customer)(New Uri(request.BaseUri + "/Customers"), Nothing, Nothing)
                Using waitHandle = asyncResult.AsyncWaitHandle
                    ctx.EndExecute(Of Customer)(asyncResult)
                    Try
                        waitHandle.WaitOne()
                        Assert.Fail("Expect ObjectDisposedException but received none.")
                    Catch ex As ObjectDisposedException
                        Assert.IsNotNull(ex)
                    End Try
                End Using
            End Using
        End Using
    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod()>
    Public Sub DisposeAsyncWaitHandle2()
        Using CustomDataContext.CreateChangeScope()
            Using request = TestWebRequest.CreateForLocal
                request.DataServiceType = GetType(CustomDataContext)
                request.StartService()

                ' Make sure EndLoadProperty disposes the wait handle
                Dim ctx = New DataServiceContext(New Uri(request.BaseUri))
                'ctx.EnableAtom = True
                'ctx.Format.UseAtom()
                Dim customer = ctx.CreateQuery(Of Customer)("Customers").First()
                Dim asyncResult = ctx.BeginLoadProperty(customer, "Orders", Nothing, Nothing)
                Using waitHandle = asyncResult.AsyncWaitHandle
                    ctx.EndLoadProperty(asyncResult)
                    Try
                        waitHandle.WaitOne()
                        Assert.Fail("Expect ObjectDisposedException but received none.")
                    Catch ex As ObjectDisposedException
                        Assert.IsNotNull(ex)
                    End Try
                End Using
            End Using
        End Using
    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod()>
    Public Sub TestSelectSubQueryOfNavigationProperty()
        Using request = TestWebRequest.CreateForLocal
            request.DataServiceType = GetType(NorthwindContext)
            request.StartService()

            Dim ctx = New DataServiceContext(New Uri(request.BaseUri))
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim q = From c In ctx.CreateQuery(Of northwindClient.Categories)("Categories")
                    Select New With {.PN = (From p In c.Products Select p), .Cat = c}

            For Each c As Object In q
                Assert.IsTrue(c.ToString().Contains("Cat"))
                Assert.IsTrue(c.ToString().Contains("PN"))
            Next c

        End Using
    End Sub

#End Region

#Region "Fuzzing: Response header OData-Version value validation issues"
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("Fuzzing: Response header OData-Version value validation issues")>
    Public Sub ValidateODataVersionResponseHeader()

        Dim serviceVersions() As String = {Nothing, String.Empty, "abc;", "0.0;", "0.123456;", "1;", "1.0;", "1.2;", "1.5;", "2.0;", "2.5;", "4.0",
            ";", ";;", ";1.0", ";1.0;", "-1.0;", "1.0.0;", "2.0.0.0;", "1.0;2.0;", "1.2.3.4.5;", ".;", "1.;", ".1;"}

        ' For version parsing we ignore characters after the first ';' - hence "1.0;.*" is valid 
        Dim validServiceVersions() As String = {Nothing, String.Empty, "4.0"}

        Using request = TestWebRequest.CreateForInProcessWcf
            request.DataServiceType = GetType(TestCtx1)
            request.StartService()
            Dim ctx = New DataServiceContext(New Uri(request.BaseUri), ODataProtocolVersion.V4)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            For Each dataServiceVersion As String In serviceVersions
                TestCtx1.DataServiceResponseVersion = dataServiceVersion
                Dim shouldThrow As Boolean = Not validServiceVersions.Contains(dataServiceVersion)
                Try
                    Dim entity = ctx.CreateQuery(Of TestEntity)("Data").First
                    Assert.IsFalse(shouldThrow, "Expected exception for version: " & dataServiceVersion)
                Catch ex As DataServiceQueryException
                    Assert.IsTrue(shouldThrow, "Exception for version: " & dataServiceVersion & " not expected")
                    Assert.IsInstanceOfType(ex.InnerException, GetType(InvalidOperationException))
                    Assert.AreEqual("Response version '" + dataServiceVersion + "' is not supported. The only supported versions are: '4.0'.", ex.InnerException.Message)
                End Try
            Next
        End Using
    End Sub

    Public Class TestCtx1
        Inherits DataService(Of TestDB)

        Public Sub New()
            AddHandler Me.ProcessingPipeline.ProcessedRequest, AddressOf OverwriteDataServiceVersion
        End Sub

        Public Sub OverwriteDataServiceVersion(ByVal sender As Object, ByVal e As DataServiceProcessingPipelineEventArgs)
            If responseVersion Is Nothing Then
                WebOperationContext.Current.OutgoingResponse.Headers.Remove("OData-Version")
            ElseIf (WebOperationContext.Current.OutgoingResponse.Headers.AllKeys.Contains("OData-Version")) Then
                WebOperationContext.Current.OutgoingResponse.Headers("OData-Version") = responseVersion
            End If
        End Sub

        Public Shared Sub InitializeService(ByVal config As DataServiceConfiguration)
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead)
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4
        End Sub

        Private Shared responseVersion As String

        Public Shared WriteOnly Property DataServiceResponseVersion() As String
            Set(ByVal value As String)
                responseVersion = value
            End Set
        End Property

    End Class
#End Region

#Region "client does not add If-Match header when the etag is only available from the response header."
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("client does not add If-Match header when the etag is only available from the response header.")>
    Public Sub NonBatchRequest()
        Using request = TestWebRequest.CreateForInProcessWcf
            Using PlaybackService.OverridingPlayback.Restore()
                request.ServiceType = GetType(PlaybackService)
                request.StartService()
                Dim ctx = New DataServiceContext(request.ServiceRoot)
                'ctx.EnableAtom = True

                Dim order As AstoriaUnitTests.Stubs.Order = New AstoriaUnitTests.Stubs.Order()
                order.ID = 16584
                order.DollarAmount = 100.0
                ctx.AddObject("Orders", order)

                Dim headerEtag As String = "ETag-from-header"
                PlaybackService.OverridingPlayback.Value =
    "HTTP/1.1 201 Created" & vbCrLf &
    "Content-Type: application/atom+xml" & vbCrLf &
    "Content-ID: 1" & vbCrLf &
    "ETag: " & headerEtag & vbCrLf &
    "Location: http://localhost:62614/TheTest/Orders(16584)" & vbCrLf &
    vbCrLf &
    "<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>" &
    "<entry xml:base=""http://localhost:62614/TheTest/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">" &
    "  <id>http://localhost:62614/TheTest/Orders(16584)</id>" &
    "  <title type=""text""></title>" &
    "  <updated>2009-09-30T01:44:35Z</updated>" &
    "  <author>" &
    "    <name />" &
    "  </author>" &
    "  <link rel=""edit"" title=""Order"" href=""Orders(16584)"" />" &
    "  <link rel=""http://docs.oasis-open.org/odata/ns/related/Customer"" type=""application/atom+xml;type=entry"" title=""Customer"" href=""Orders(16584)/Customer"" />" &
    "  <link rel=""http://docs.oasis-open.org/odata/ns/related/OrderDetails"" type=""application/atom+xml;type=feed"" title=""OrderDetails"" href=""Orders(16584)/OrderDetails"" />" &
    "  <category term=""#AstoriaUnitTests.Stubs.Order"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />" &
    "  <content type=""application/xml"">" &
    "    <m:properties>" &
    "      <d:ID m:type=""Edm.Int32"">16584</d:ID>" &
    "      <d:DollarAmount m:type=""Edm.Double"">100</d:DollarAmount>" &
    "      <d:CurrencyAmount m:type=""AstoriaUnitTests.Stubs.CurrencyAmount"" m:null=""true"" />" &
    "    </m:properties>" &
    "  </content>" &
    "</entry>"

                ctx.SaveChanges()
                Dim e = ctx.Entities.First()
                Assert.AreEqual(e.ETag, headerEtag)
            End Using
        End Using
    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("client does not add If-Match header when the etag is only available from the response header.")>
    Public Sub BatchRequest()
        Using CustomDataContext.CreateChangeScope()
            Using PlaybackService.OverridingPlayback.Restore
                Using request = TestWebRequest.CreateForInProcessWcf
                    request.ServiceType = GetType(PlaybackService)
                    request.StartService()
                    Dim ctx = New DataServiceContext(New Uri(request.BaseUri))
                    'ctx.EnableAtom = True

                    Dim order As AstoriaUnitTests.Stubs.Order = New AstoriaUnitTests.Stubs.Order()
                    order.ID = 16584
                    order.DollarAmount = 100.0
                    ctx.AddObject("Orders", order)

                    Dim headerETag As String = "ETag-from-header"
                    PlaybackService.OverridingPlayback.Value =
    "HTTP/1.1 200 OK" & vbCrLf &
    "OData-Version: 4.0;" & vbCrLf &
    "Content-Type: multipart/mixed; boundary=batchresponse_dd8688f0-40e6-4c6a-bf7b-de35d25d4583" & vbCrLf &
    vbCrLf &
    "--batchresponse_dd8688f0-40e6-4c6a-bf7b-de35d25d4583" & vbCrLf &
    "Content-Type: multipart/mixed; boundary=changesetresponse_10278993-a508-4f62-b31a-d2aa02e40ea1" & vbCrLf &
    vbCrLf &
    "--changesetresponse_10278993-a508-4f62-b31a-d2aa02e40ea1" & vbCrLf &
    "Content-Type: application/http" & vbCrLf &
    "Content-Transfer-Encoding: binary" & vbCrLf &
    vbCrLf &
    "HTTP/1.1 201 Created" & vbCrLf &
    "Content-ID: 1" & vbCrLf &
    "Cache-Control: no-cache" & vbCrLf &
    "OData-Version: 4.0;" & vbCrLf &
    "Content-Type: application/atom+xml;charset=utf-8" & vbCrLf &
    "ETag: " & headerETag & vbCrLf &
    "Location: http://localhost:59251/TheTest/Orders(16584)" & vbCrLf &
    vbCrLf &
    "<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>" & vbCrLf &
    "<entry xml:base=""http://localhost:59251/TheTest/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">" & vbCrLf &
    "  <id>http://localhost:59251/TheTest/Orders(16584)</id>" & vbCrLf &
    "  <title type=""text""></title>" & vbCrLf &
    "  <updated>2009-10-12T22:38:44Z</updated>" & vbCrLf &
    "  <author>" & vbCrLf &
    "    <name />" & vbCrLf &
    "  </author>" & vbCrLf &
    "  <link rel=""edit"" title=""Order"" href=""Orders(16584)"" />" & vbCrLf &
    "  <link rel=""http://docs.oasis-open.org/odata/ns/related/Customer"" type=""application/atom+xml;type=entry"" title=""Customer"" href=""Orders(16584)/Customer"" />" & vbCrLf &
    "  <link rel=""http://docs.oasis-open.org/odata/ns/related/OrderDetails"" type=""application/atom+xml;type=feed"" title=""OrderDetails"" href=""Orders(16584)/OrderDetails"" />" & vbCrLf &
    "  <category term=""#AstoriaUnitTests.Stubs.Order"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />" & vbCrLf &
    "  <content type=""application/xml"">" & vbCrLf &
    "    <m:properties>" & vbCrLf &
    "      <d:ID m:type=""Edm.Int32"">16584</d:ID>" & vbCrLf &
    "      <d:DollarAmount m:type=""Edm.Double"">100</d:DollarAmount>" & vbCrLf &
    "      <d:CurrencyAmount m:type=""AstoriaUnitTests.Stubs.CurrencyAmount"" m:null=""true"" />" & vbCrLf &
    "    </m:properties>" & vbCrLf &
    "  </content>" & vbCrLf &
    "</entry>" & vbCrLf &
    "--changesetresponse_10278993-a508-4f62-b31a-d2aa02e40ea1--" & vbCrLf &
    "--batchresponse_dd8688f0-40e6-4c6a-bf7b-de35d25d4583--"

                    Dim asyncResult = ctx.BeginSaveChanges(SaveChangesOptions.BatchWithSingleChangeset, Nothing, Nothing)
                    asyncResult.AsyncWaitHandle.WaitOne()
                    ctx.EndSaveChanges(asyncResult)

                    Dim e = ctx.Entities.First()
                    Assert.AreEqual(e.ETag, headerETag)

                End Using
            End Using
        End Using
    End Sub

#End Region

#Region "ExplicitExpansionWithProjection"

    <EntityType()>
    Public Class TestEntity2
        Private m_ID As String
        Public Property CustomerID() As String
            Get
                Return m_ID
            End Get
            Set(ByVal value As String)
                m_ID = value
            End Set
        End Property

        Private m_Orders As DataServiceCollection(Of NorthwindSimpleModel.Orders)
        Public Property Orders() As DataServiceCollection(Of NorthwindSimpleModel.Orders)
            Get
                Return m_Orders
            End Get
            Set(ByVal value As DataServiceCollection(Of NorthwindSimpleModel.Orders))
                m_Orders = value
            End Set
        End Property
    End Class

    <TestCategory("Partition1")> <TestMethod()> Public Sub ExplicitExpansionWithProjection()
        Dim q = CType((From c In ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("Customers") Select New TestEntity2() With {.CustomerID = c.CustomerID}), DataServiceQuery(Of TestEntity2))

        Dim qExpand = q.Expand("Orders")
        Dim qAddQuery = q.AddQueryOption("$expand", "Orders")

        Try
            qExpand.GetEnumerator()
            Assert.Fail("Expand with projections failed to throw")
        Catch ex As NotSupportedException
        End Try

        Try
            qAddQuery.GetEnumerator()
            Assert.Fail("Add expansion option with projections failed to throw")
        Catch ex As NotSupportedException
        End Try

        Dim expandFirstQuery = From c In ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("Customers").Expand("Orders") Select New TestEntity2() With {.CustomerID = c.CustomerID, .Orders = c.Orders}

        Try
            expandFirstQuery.GetEnumerator()
            Assert.Fail("projection with expansion failed to throw")
        Catch ex As NotSupportedException
        End Try


        Dim addQueryOptionsFirstQuery = From c In ctx.CreateQuery(Of NorthwindSimpleModel.Customers)("Customers").AddQueryOption("$expand", "Orders") Select New TestEntity2() With {.CustomerID = c.CustomerID, .Orders = c.Orders}

        Try
            addQueryOptionsFirstQuery.GetEnumerator()
            Assert.Fail("projection with expansion failed to throw")
        Catch ex As NotSupportedException
        End Try
    End Sub

#End Region

#Region "Client Projections : AddQueryOption(""$select"") doesn't work"
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod()>
    Public Sub AddQueryOptionSelect()
        Dim q = From c In ctx.CreateQuery(Of northwindClient.Customers)("Customers").AddQueryOption("$select", "ContactName") Select c
        Assert.IsTrue(q.ToString().Contains("$select=ContactName"))

        ' AddQueryOption should by-pass the $expand check
        q = From c In ctx.CreateQuery(Of northwindClient.Customers)("Customers").Expand("Orders($select=OrderID)").AddQueryOption("$select", "Orders") Select c
        Assert.IsTrue(q.ToString().Contains("$select=Orders"))
        Assert.IsTrue(q.ToString().Contains("$expand=Orders($select=OrderID)"))

        q = From c In ctx.CreateQuery(Of northwindClient.Customers)("Customers").AddQueryOption("$select", "Orders").Expand("Orders($select=OrderID)") Select c
        Assert.IsTrue(q.ToString().Contains("$select=Orders"))
        Assert.IsTrue(q.ToString().Contains("$expand=Orders($select=OrderID)"))

        Dim c1 = q.FirstOrDefault()
        Assert.IsNotNull(c1.Orders)
        Assert.IsNull(c1.CustomerID)
    End Sub

#End Region

#Region "TypeConvert"

    Public Class TestEntity3
        Public Property ID As String
        Public Property Uri As Uri
        Public Property Related As TestEntity3
    End Class
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod()>
    Public Sub TypeConvert()
        Dim entity = New TestEntity3() With {.ID = "0:0", .Uri = New System.Uri("http://uri/entities(escaped%3AID)")}
        Dim related = New TestEntity3() With {.ID = "1:1", .Uri = New System.Uri("http://uri/entities(escaped%3AID2)")}

        Dim wrappingStream As WrappingStream = Nothing
        ctx.RegisterStreamCustomizer(Function(inputStream As Stream)
                                         wrappingStream = New WrappingStream(inputStream)
                                         Return wrappingStream
                                     End Function,
                                     Nothing)

        ctx.AttachTo("Entities", entity)
        ctx.AddObject("Entities", related)
        ctx.SetLink(related, "Related", entity)

        Try
            ctx.SaveChanges()
        Catch ex As Exception
            ctx.Configurations.RequestPipeline.OnMessageCreating = Nothing
        End Try

        Dim xe As XElement = wrappingStream.GetLoggingStreamAsXDocument().Elements.First()
        Dim q = xe.<atom:content>.<m:properties>.<d:Uri>
        Dim el = q.FirstOrDefault()
        Assert.IsNotNull(el)
        Assert.AreEqual("http://uri/entities(escaped%3AID2)", el.Value)
        Assert.IsTrue(xe.<atom:link>.@href.Contains("0%3A0"))


        Dim identity = ctx.GetEntityDescriptor(entity).EditLink
        Dim trackedEntity As Object = Nothing
        Assert.IsTrue(ctx.TryGetEntity(identity, trackedEntity))
        Assert.IsNotNull(trackedEntity)
    End Sub

#End Region

#Region "Projected collections of primitives."

    ' This is a simple scan of the following:
    ' ProjectedType:              int,int?,string,Address
    ' ProjectedTypeSourceType:    EntityType,ComplexType,Collection
    ' CollectionType:             IEnumerable,IEnumerableOfT,ArrayList,ListOfT,CollectionOfT,HashSetOfT,ReadOnlyCollectionOfT,DataServiceCollectionOfT
    ' CollectionTypeCasts:        SameType,Upcasts
    ' CollectionTypeMethods:      ToArray,ToList,IntoDataServiceCollection
    ' TargetType:                 IntoAnon,IntoEntity
    ' SameLevelProjectionCount:   1,3
    ' DepthLevelProjectionCount:  1,3
    ' ProjectionOrder:            BeforeEntity,AfterEntity
    ' ServerDrivenPaging:         true,false
    ' ServerResults:              Empty,EmptyAfterSDP,1,5
    ' NavigationBeforeSelect:     0,1,4
#End Region

    <TestCategory("Partition1")> <TestMethod(), Variation("Projecting entity that is not in scope with self-referencing type (VB)")>
    Public Sub SelectManyInvalidProjectionWithSelfReferencingType_VB()

        ' All of these queries should fail with the same NotSupportedException because they are
        ' attempting to project with an iterator that is outside of the current scope.
        ' Other tests in AstoriaUnitTests.Tests.LinqTests cover the cases where the types are different.

        ' The CSharp version of this test is in ClientCSharpRegressionTests.SelectManyInvalidProjectionWithSelfReferencingType_CSharp
        ' The queries are the same, but CSharp handles SelectMany with a slightly different expression tree, so make sure we can handle both

        ' Note that in order to produce the kind of expression tree that caused the bug, the query must reference
        ' the inner query iterator. Since the purpose of this test is to explicitly *not* use that iterator in the
        ' projection, that means the query needs to have some other clause that reference the iterator
        ' The queries use orderby and where to achieve this, but there is no additional significance to which operator is used in a given query.

        Dim context As DataServiceContext = New DataServiceContext(New Uri("http://invalidhost"))
        Dim baseQuery = context.CreateQuery(Of SelfReferenceTypedEntity(Of Int32, Int32))("EntitySet")

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Order By (inner.ID)
            Select outer)

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Where inner.ID = 2
            Select outer.Member)

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Order By inner.ID
            Select outer.Reference)

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Where inner.ID = 2
            Select outer.Reference.Member)

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Order By inner.ID
            Select New With {outer})

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Where inner.ID = 2
            Select New With {outer.Member})

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Order By inner.ID
            Select New With {outer.Reference})

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Where inner.ID = 2
            Select New With {outer.Reference.Member})

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From inner In outer.Collection
            Order By inner.ID
            Select New With {outer.Member, outer.ID, .Nested = outer.Reference.Reference.Member})

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From firstInner In outer.Collection
            Where firstInner.ID = 1
            From secondInner In firstInner.DSC
            Order By secondInner.ID
            Select New With {firstInner})

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From firstInner In outer.Collection
            Where firstInner.ID = 1
            From secondInner In firstInner.DSC
            Where secondInner.ID = 2
            Select New With {firstInner})

        VerifyLeafProjectionException(
            From outer In baseQuery
            Where outer.ID = 1
            From firstInner In outer.Collection
            Where firstInner.ID = 1
            From secondInner In firstInner.DSC
            Order By secondInner.ID
            Select New With {firstInner.Member, secondInner})
    End Sub

    Private Sub VerifyLeafProjectionException(ByVal query As IQueryable)
        Try
            query.GetEnumerator()
            Assert.Fail("Expected NotSupportedException for query " & query.ToString())
        Catch nse As NotSupportedException
            Assert.AreEqual(DataServicesClientResourceUtil.GetString("ALinq_CanOnlyProjectTheLeaf"), nse.Message)
        End Try
    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("Type casting in LINQ queries")>
    Public Sub SelectManyWithTypeCast()

        ' NOTE: This is the VB version of LinqTests.LinqCast

        ' An in-memory web request is required, because results are comparing
        ' in-depth using references, which won't work with "real" unwired
        ' objects coming from a different domain.
        Using request = TestWebRequest.CreateForLocal()
            request.DataServiceType = GetType(ReadOnlyTestContext)
            request.StartService()

            Dim context As DataServiceContext = New DataServiceContext(request.ServiceRoot)
            ''context.EnableAtom = True
            ''context.Format.UseAtom()
            context.MergeOption = MergeOption.NoTracking

            Dim baseLineContext As ReadOnlyTestContext = ReadOnlyTestContext.CreateBaseLineContext()

            Trace.WriteLine("type specified explicitly in comprehension.")

            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(League), "Teams")

            Dim query = From l As League In context.CreateQuery(Of League)("Leagues")
                        Where l.ID = 1
                        From t In l.Teams
                        Select t

            Dim baseline = From l As League In baseLineContext.Leagues
                           Where l.ID = 1
                           From t In l.Teams
                           Select t

            LinqTests.RunTest(baseline, query)

            Trace.WriteLine("type specified explicitly in comprehension - Select Many.")

            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(League), "Teams")

            Dim query2 = From l As League In context.CreateQuery(Of League)("Leagues")
                         Where l.ID = 1
                         From t As Team In l.Teams
                         Select t

            Dim baseline2 = From l As League In baseLineContext.Leagues
                            Where l.ID = 1
                            From t As Team In l.Teams
                            Select t

            LinqTests.RunTest(baseline2, query2)

            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("arbitrary cast of resource set (nbo base type)")

            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Table), "Rows")

            Dim query3 = From cr As CustomerRow In context.CreateQuery(Of Row)("Rows")
                         Select cr

            Dim baseline3 = From cr In baseLineContext.Rows
                            Select CType(cr, CustomerRow)

            LinqTests.RunTest(baseline3, query3)

            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("arbitrary case of resource set, select many")
            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Table), "Rows")
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Row), "Details")

            Dim query4 = From t As MyTable In context.CreateQuery(Of Table)("Tables")
                         Where t.TableName = "Customers"
                         From cr As CustomerRow In t.Rows
                         Where cr.Id = 1
                         From crd As CustomerRowDetail In cr.Details
                         Select crd

            Dim baseline4 = From t As Table In baseLineContext.Tables
                            Where t.TableName = "Customers"
                            From cr In t.Rows
                            Where cr.Id = 1
                            From crd In cr.Details
                            Select CType(crd, CustomerRowDetail)

            LinqTests.RunTest(baseline4, query4)

            Trace.WriteLine("arbitrary case of resource set, select many")
            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Table), "Rows")
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Row), "Details")

            Dim query4a = From t As MyTable In context.CreateQuery(Of Table)("Tables")
                          From cr As CustomerRow In t.Rows
                          From crd As CustomerRowDetail In cr.Details
                          Where t.TableName = "Customers"
                          Where cr.Id = 1
                          Select crd

            Dim baseline4a = From t As Table In baseLineContext.Tables
                             From cr In t.Rows
                             From crd In cr.Details
                             Where t.TableName = "Customers"
                             Where cr.Id = 1
                             Select CType(crd, CustomerRowDetail)

            LinqTests.RunTest(baseline4a, query4a)

            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("arbitrary cast of resource set singleton")
            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Table), "TableInfo")

            Dim query5 = From cti As CustomTableInfo In
                             (From t As Table In context.CreateQuery(Of Table)("Tables")
                              Where t.TableName = "Customers"
                              Select t.TableInfo)
                         Select cti

            Dim baseline5 = From cti In
                                (From t As Table In baseLineContext.Tables
                                 Where t.TableName = "Customers"
                                 Select t.TableInfo)
                            Select CType(cti, CustomTableInfo)

            LinqTests.RunTest(baseline5, query5)

            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("query options of new type")

            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Table), "Rows")
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Row), "Details")

            Dim query6 = From t As MyTable In context.CreateQuery(Of Table)("Tables")
                         Where t.TableName = "Customers"
                         From cr As CustomerRow In t.Rows
                         Where cr.Id = 1
                         From crd As CustomerRowDetail In cr.Details
                         Where Not crd.Word = "xxx"
                         Order By crd.Word
                         Select crd

            Dim baseline6 = From t As Table In baseLineContext.Tables
                            Where t.TableName = "Customers"
                            From cr In t.Rows
                            Where cr.Id = 1
                            From crd In cr.Details
                            Where Not crd.Word = "xxx"
                            Order By crd.Word
                            Select CType(crd, CustomerRowDetail)

            LinqTests.RunTest(baseline6, query6)
            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("single casts -- select many")
            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Table), "Rows")

            Dim query7 = From t In context.CreateQuery(Of Table)("Tables")
                         Where t.TableName = "Customers"
                         From cr In t.Rows.Cast(Of CustomerRow)()
                         Select cr

            Dim baseline7 = From t As Table In baseLineContext.Tables
                            Where t.TableName = "Customers"
                            From cr In t.Rows
                            Select CType(cr, CustomerRow)

            LinqTests.RunTest(baseline7, query7)
            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("multiple casts.")
            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Table), "Rows")

            Dim query7b = From t In context.CreateQuery(Of Table)("Tables").Cast(Of Int32)().Cast(Of MyTable)()
                          Where t.TableName = "Customers"
                          From cr In t.Rows.Cast(Of Object)().Cast(Of CustomerRow)()
                          Select cr

            Dim baseline7b = From t As Table In baseLineContext.Tables
                             Where t.TableName = "Customers"
                             From cr In t.Rows
                             Select CType(cr, CustomerRow)

            LinqTests.RunTest(baseline7b, query7b)
            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("after custom queryoptions, expand")
            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Table), "Rows")

            Dim query8 = DirectCast((From t In context.CreateQuery(Of Table)("Tables")
                                     Where t.TableName = "Customers"
                                     Select t), DataServiceQuery(Of Table)).AddQueryOption("ghghgh", "xxx").Expand("Rows").Cast(Of MyTable)()

            Dim baseline8 = From t As Table In baseLineContext.Tables
                            Where t.TableName = "Customers"
                            Select CType(t, MyTable)

            LinqTests.RunTest(baseline8, query8)
            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("Cast of singleton returning result")

            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Team), "HomeStadium")

            Dim query9 = (From t As Team In context.CreateQuery(Of Team)("Teams")
                          Where t.TeamID = 1
                          Select t.HomeStadium).Cast(Of Stadium)()

            Dim baseline9 = (From t As Team In baseLineContext.Teams
                             Where t.TeamID = 1
                             Select t.HomeStadium).Cast(Of Stadium)()

            LinqTests.RunTest(baseline9, query9)
            ReadOnlyTestContext.ClearBaselineIncludes()

            Trace.WriteLine("Cast to narrow type")
            ReadOnlyTestContext.ClearBaselineIncludes()
            ReadOnlyTestContext.AddBaselineIncludes(GetType(Team), "HomeStadium")
            context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support

            Dim query10 = (From t As Team In context.CreateQuery(Of Team)("Teams")
                           Where t.TeamID = 1
                           Select t.HomeStadium).Cast(Of NarrowStadium)()

            Dim baseline10 = (From t As Team In baseLineContext.Teams
                              Where t.TeamID = 1
                              Select t.HomeStadium).Select(Function(s) CType(s, NarrowStadium))

            LinqTests.RunTest(baseline10, query10)
            ReadOnlyTestContext.ClearBaselineIncludes()
        End Using

    End Sub

    Private Class SelectManyTest
        Public Baseline As IQueryable
        Public TestQuery As IQueryable
    End Class
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("Verify behavior when adding items to a DataServiceCollection that has tracking option set during construction")>
    Public Sub ItemsAddedToDataServiceCollectionAfterExceptionsOccurred_ImmediateTrackingDuringConstruction()

        Using host As TestWebRequest = TestWebRequest.CreateForInProcessWcf()
            Using PlaybackService.OverridingPlayback.Restore()
                host.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
                host.StartService()

                TestUtil.RunCombinations(New TrackingMode() {TrackingMode.AutoChangeTracking, TrackingMode.None},
                                         Sub(trackingMode)
                                             TestImmediateTrackingDSC(Of EntityWithoutINPC)(
                                                 host.BaseUri,
                                                 trackingMode,
                                                 New InvalidOperationException(DataServicesClientResourceUtil.GetString("DataBinding_NotifyPropertyChangedNotImpl", GetType(EntityWithoutINPC))),
                                                 Sub(dsc)
                                                     If trackingMode = trackingMode.None Then
                                                         Assert.AreEqual(1, dsc.Count, "Expected DataServiceCollection to be contain the newly added entity.")
                                                     Else
                                                         Assert.Fail("Expected exception was not thrown.")
                                                     End If
                                                 End Sub,
                                                 Sub(dsc, ex)
                                                     Assert.AreEqual(trackingMode.AutoChangeTracking, trackingMode, "Exception is only expected when tracking is turned on.")
                                                     Assert.AreEqual(0, dsc.Count, "Expected DataServiceCollection to be empty since the Add failed.")
                                                 End Sub)
                                         End Sub)


                TestImmediateTrackingDSC(Of INPCTypeWithoutID)(
                    host.BaseUri,
                    TrackingMode.AutoChangeTracking,
                    New ArgumentException(DataServicesClientResourceUtil.GetString("DataBinding_DataServiceCollectionArgumentMustHaveEntityType", GetType(INPCTypeWithoutID))),
                    Sub(dsc)
                        Assert.Fail("Expected exception was not thrown.")
                    End Sub,
                    Sub(dsc, ex)
                        Assert.IsNull(dsc, "Expected DataServiceCollection not to be created due to the failure to start tracking.")
                    End Sub)

                TestImmediateTrackingDSC(Of INPCTypeWithoutID)(
                    host.BaseUri,
                    TrackingMode.None,
                    New DataServiceQueryException(DataServicesClientResourceUtil.GetString("DataServiceException_GeneralError")),
                    Sub(dsc)
                        Assert.AreEqual(1, dsc.Count, "Expected DataServiceCollection to be contain the newly added entity.")
                    End Sub,
                    Sub(dsc, ex)
                        Assert.AreEqual(GetType(InvalidOperationException), ex.InnerException.GetType(), "Inner exception type did not match")
                        Assert.AreEqual(DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidNonEntityType", GetType(INPCTypeWithoutID)), ex.InnerException.Message, "Inner exception message did not match")
                        ' no validation, since the dsc is created, but the exception is thrown later while loading the entity
                    End Sub)
            End Using
        End Using

    End Sub

    Private Sub TestImmediateTrackingDSC(Of EntityType As New)(
                                                              ByVal contextUri As String,
                                                              ByVal trackMode As TrackingMode,
                                                              ByVal expectedException As Exception,
                                                              ByVal additionalValidation As Action(Of DataServiceCollection(Of EntityType)),
                                                              ByVal additionalErrorValidation As Action(Of DataServiceCollection(Of EntityType), Exception))

        ' Test different kinds of ways to add the item to the DataServiceCollection
        ' (1) Add from objects created on the client
        ' (2) Insert from items created on the client
        ' (3) Load from materialized objects
        Dim entityGetters() As Action(Of DataServiceContext, DataServiceCollection(Of EntityType)) = {
                AddressOf DataServiceCollection_Add,
                AddressOf DataServiceCollection_Insert,
                AddressOf DataServiceCollection_Load
        }

        TestUtil.RunCombinations(entityGetters,
                                 Sub(entityGetter)
                                     Dim context As DataServiceContext = New DataServiceContext(New Uri(contextUri))
                                     'context.EnableAtom = True
                                     Dim dataServiceCollection As DataServiceCollection(Of EntityType) = Nothing

                                     Try
                                         ' If tracking is turned on, constructing the collection or adding to it will fail depending on what is wrong with the type (see expectedException for the test case).
                                         ' If tracking is turned off, no errors should occur.
                                         dataServiceCollection = New DataServiceCollection(Of EntityType)(context, Nothing, trackMode, "Entities", Nothing, Nothing)
                                         entityGetter(context, dataServiceCollection)
                                         additionalValidation(dataServiceCollection)
                                     Catch ex As Exception
                                         Assert.IsInstanceOfType(ex, expectedException.GetType(), "Wrong exception type")
                                         Assert.AreEqual(expectedException.Message, ex.Message, "Wrong exception message")
                                         additionalErrorValidation(dataServiceCollection, ex)
                                     End Try
                                 End Sub)

    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("Verify behavior when adding items to a DataServiceCollection that turns tracking on during Load")>
    Public Sub ItemsAddedToDataServiceCollectionAfterExceptionsOccurred_DeferringTrackingDuringLoad()

        Using host As TestWebRequest = TestWebRequest.CreateForInProcessWcf()
            Using PlaybackService.OverridingPlayback.Restore()
                host.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
                host.StartService()

                TestDeferredTrackingDSC(Of EntityWithoutINPC)(host.BaseUri, New InvalidOperationException(DataServicesClientResourceUtil.GetString("DataBinding_NotifyPropertyChangedNotImpl", GetType(EntityWithoutINPC))))
                TestDeferredTrackingDSC(Of INPCTypeWithoutID)(host.BaseUri, New ArgumentException(DataServicesClientResourceUtil.GetString("DataBinding_DataServiceCollectionArgumentMustHaveEntityType", GetType(INPCTypeWithoutID))))
            End Using
        End Using

    End Sub

    Private Sub TestDeferredTrackingDSC(Of EntityType As New)(ByVal contextUri As String, ByVal loadException As Exception)

        ' Test different kinds of ways to add the item to the DataServiceCollection
        ' (1) Add from objects created on the client (fails because tracking hasn't been started yet since the context wasn't available when the DataServiceCollection was created)
        ' (2) Insert from items created on the client (fails for same reason above)
        ' (3) Load from materialized objects (tracking is automatically turned on in this case since the context is now available, but fails now because entity type doesn't implement INotifyPropertyChanged)
        Dim testCases = {
            New DeferredTrackingTestCase(Of EntityType) With {
                .PopulateDSC = CType(AddressOf DataServiceCollection_Add, Action(Of DataServiceContext, DataServiceCollection(Of EntityType))),
                .ExpectedException = New InvalidOperationException(DataServicesClientResourceUtil.GetString("DataServiceCollection_InsertIntoTrackedButNotLoadedCollection"))
            },
            New DeferredTrackingTestCase(Of EntityType) With {
                .PopulateDSC = CType(AddressOf DataServiceCollection_Insert, Action(Of DataServiceContext, DataServiceCollection(Of EntityType))),
                .ExpectedException = New InvalidOperationException(DataServicesClientResourceUtil.GetString("DataServiceCollection_InsertIntoTrackedButNotLoadedCollection"))
            },
            New DeferredTrackingTestCase(Of EntityType) With {
                .PopulateDSC = CType(AddressOf DataServiceCollection_Load, Action(Of DataServiceContext, DataServiceCollection(Of EntityType))),
                .ExpectedException = loadException
            }
        }

        TestUtil.RunCombinations(testCases,
                                 Sub(testCase As DeferredTrackingTestCase(Of EntityType))
                                     Dim context As DataServiceContext = New DataServiceContext(New Uri(contextUri))
                                     'context.EnableAtom = True

                                     ' Since tracking is deferred, constructing the collection should never fail
                                     Dim dataServiceCollection = New DataServiceCollection(Of EntityType)()

                                     Try
                                         testCase.PopulateDSC(context, dataServiceCollection)
                                         Assert.Fail("Expected exception was not thrown.")
                                     Catch ex As Exception
                                         Dim expectedException = testCase.ExpectedException
                                         Assert.IsInstanceOfType(ex, expectedException.GetType(), "Wrong exception type")
                                         Assert.AreEqual(expectedException.Message, ex.Message, "Wrong exception message")
                                         Assert.AreEqual(0, dataServiceCollection.Count, "Expected DataServiceCollection to be empty since the Add failed.")
                                     End Try
                                 End Sub)

    End Sub

    Private Class EntityWithoutINPC
        Public Property ID As Int32
        Public Property Name As String
    End Class

    Private Class INPCTypeWithoutID
        Implements INotifyPropertyChanged

        Public Property Name As String

        Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
            If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
            End If
        End Sub

    End Class

    Private Class DeferredTrackingTestCase(Of EntityType)
        Public PopulateDSC As Action(Of DataServiceContext, DataServiceCollection(Of EntityType))
        Public ExpectedException As Exception
    End Class

    Private Shared Sub DataServiceCollection_Add(Of EntityType As New)(ByVal ctx As DataServiceContext, ByVal dsc As DataServiceCollection(Of EntityType))
        dsc.Add(New EntityType())
    End Sub

    Private Shared Sub DataServiceCollection_Insert(Of EntityType As New)(ByVal ctx As DataServiceContext, ByVal dsc As DataServiceCollection(Of EntityType))
        dsc.Insert(0, New EntityType())
    End Sub

    Private Shared Sub DataServiceCollection_Load(Of EntityType)(ByVal ctx As DataServiceContext, ByVal dsc As DataServiceCollection(Of EntityType))
        Dim payload As String = String.Format(
            "HTTP/1.1 200 OK" & vbCrLf &
            "Server: ASP.NET Development Server/10.0.0.0" & vbCrLf &
            "Date: Wed, 27 Jan 2010 18:06:26 GMT" & vbCrLf &
            "X-AspNet-Version: 4.0.30107" & vbCrLf &
            "OData-Version: 4.0;" & vbCrLf &
            "Set-Cookie: ASP.NET_SessionId=d0ieqfv0tr5pfq4rqafszurj; path=/; HttpOnly" & vbCrLf &
            "Cache-Control: no-cache" & vbCrLf &
            "Content-Type: application/atom+xml;charset=utf-8" & vbCrLf &
            "Connection: Close" & vbCrLf & vbCrLf &
            "<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>" & vbCrLf &
            "<feed xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">" & vbCrLf &
            "  <title type=""text"">Entities</title>" & vbCrLf &
            "  <id>{0}/Entities</id>" & vbCrLf &
            "  <updated>2010-01-27T18:06:26Z</updated>" & vbCrLf &
            "  <link rel=""self"" title=""Entities"" href=""Entities"" />" & vbCrLf &
            "  <entry>" & vbCrLf &
            "    <id>{0}/Entities(5)</id>" & vbCrLf &
            "    <title type=""text""></title>" & vbCrLf &
            "    <updated>2010-01-27T18:06:26Z</updated>" & vbCrLf &
            "    <author>" & vbCrLf &
            "      <name />" & vbCrLf &
            "    </author>" & vbCrLf &
            "    <link rel=""edit"" title=""Entity"" href=""Entities(5)"" />" & vbCrLf &
            "    <category term=""#WebApplication1.Entity"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />" & vbCrLf &
            "    <content type=""application/xml"">" & vbCrLf &
            "      <m:properties>" & vbCrLf &
            "        <d:ID m:type=""Edm.Int32"">5</d:ID>" & vbCrLf &
            "        <d:Name m:type=""Edm.String"">EntityName</d:Name>" & vbCrLf &
            "      </m:properties>" & vbCrLf &
            "    </content>" & vbCrLf &
            "  </entry>" & vbCrLf &
            "</feed>",
            ctx.BaseUri)

        PlaybackService.OverridingPlayback.Value = payload

        If GetType(EntityType) = GetType(INPCTypeWithoutID) Then
            ' Ignore missing properties here because we're reusing a test payload that has an ID but this type doesn't have a matching property
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
        End If

        dsc.Load(ctx.CreateQuery(Of EntityType)("Entities"))
    End Sub


    <TestCategory("Partition1")> <TestMethod(), Variation("Add and Remove from DataServiceCollection with nulls")>
    Public Sub AddRemoveNullWithDataServiceCollection()

        Dim ctx = New DataServiceContext(New Uri("http://localhost/"))
        Dim dsc = New DataServiceCollection(Of northwindClient.Employees)(ctx)

        Try
            dsc.Add(Nothing)
            Assert.Fail("Expected exception was not thrown.")
        Catch ex As Exception
            Assert.AreEqual(ex.Message, DataServicesClientResourceUtil.GetString("DataBinding_BindingOperation_ArrayItemNull", "Add"))
        End Try

        Try
            dsc.Remove(Nothing)
            Assert.Fail("Expected exception was not thrown.")
        Catch ex As Exception
            Assert.AreEqual(ex.Message, DataServicesClientResourceUtil.GetString("DataBinding_BindingOperation_ArrayItemNull", "Remove"))
        End Try

    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("Having a closing element for the nextpage link element causes all the rest of the elements below it at that level to be ignored")>
    Public Sub ShouldNotIgnoreElementsLeftIfHaveClosingElementForNextPageLink()
        OpenWebDataServiceHelper.ForceVerboseErrors = True
        Using request = TestWebRequest.CreateForInProcessWcf()
            Using PlaybackService.OverridingPlayback.Restore()
                request.ServiceType = GetType(PlaybackService)
                request.ForceVerboseErrors = True
                request.StartService()

                PlaybackService.OverridingPlayback.Value =
        "HTTP/1.1 200 Ok" & vbCrLf &
        "Content-Type: application/atom+xml" & vbCrLf &
        "Content-ID: 1" & vbCrLf &
        vbCrLf &
        "<?xml version='1.0' encoding='utf-8' standalone='yes'?>" &
        "<feed xml:base='http://pratikp5-vm1:34466/TheTest/' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>" &
          "<title type='text'>Customers</title>" &
          "<id>http://pratikp5-vm1:34466/TheTest/Customers</id>" &
          "<updated>2010-09-22T00:03:43Z</updated>" &
          "<link rel='self' title='Customers' href='Customers' />" &
          "<entry>" &
            "<id>http://pratikp5-vm1:34466/TheTest/Customers(0)</id>" &
            "<title type='text'></title>" &
            "<updated>2010-09-22T00:03:43Z</updated>" &
            "<author>" &
            "<name />" &
            "</author>" &
            "<link rel='edit' title='Customer' href='Customers(0)' />" &
            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' type='application/atom+xml;type=entry' title='BestFriend' href='Customers(0)/BestFriend' />" &
            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=feed' title='Orders' href='Customers(0)/Orders'>" &
              "<m:inline>" &
                "<feed>" &
                  "<title type='text'>Orders</title>" &
                  "<id>http://pratikp5-vm1:34466/TheTest/Customers(0)/Orders</id>" &
                  "<updated>2010-09-22T00:03:43Z</updated>" &
                  "<link rel='self' title='Orders' href='Customers(0)/Orders' />" &
                  "<link rel='next' href='http://pratikp5-vm1:34466/TheTest/Customers(0)/Orders?$skiptoken=400' ><something/></link>" &
                  "<entry>" &
                    "<id>http://pratikp5-vm1:34466/TheTest/Orders(0)</id>" &
                    "<title type='text'></title>" &
                    "<updated>2010-09-22T00:03:43Z</updated>" &
                    "<author>" &
                      "<name />" &
                    "</author>" &
                    "<link rel='edit' title='Order' href='Orders(0)' />" &
                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Customer' type='application/atom+xml;type=entry' title='Customer' href='Orders(0)/Customer' />" &
                    "<link rel='http://docs.oasis-open.org/odata/ns/related/OrderDetails' type='application/atom+xml;type=feed' title='OrderDetails' href='Orders(0)/OrderDetails' />" &
                    "<category term='AstoriaUnitTests.Stubs.Order' scheme='http://docs.oasis-open.org/odata/ns/scheme' />" &
                    "<content type='application/xml'>" &
                      "<m:properties>" &
                        "<d:ID m:type='Edm.Int32'>0</d:ID>" &
                        "<d:DollarAmount m:type='Edm.Double'>20.1</d:DollarAmount>" &
                        "<d:CurrencyAmount m:type='AstoriaUnitTests.Stubs.CurrencyAmount' m:null='true' />" &
                      "</m:properties>" &
                    "</content>" &
                  "</entry>" &
                "</feed>" &
              "</m:inline>" &
            "</link>" &
            "<category term='AstoriaUnitTests.Stubs.Customer' scheme='http://docs.oasis-open.org/odata/ns/scheme' />" &
            "<content type='application/xml'>" &
            "<m:properties>" &
              "<d:GuidValue m:type='Edm.Guid'>e2760c2a-0629-4136-a43f-19a938087caa</d:GuidValue>" &
              "<d:ID m:type='Edm.Int32'>0</d:ID>" &
              "<d:Name>Customer 0</d:Name>" &
              "<d:Address m:type='AstoriaUnitTests.Stubs.Address'>" &
                "<d:StreetAddress>Line1</d:StreetAddress>" &
                "<d:City>Redmond</d:City>" &
                "<d:State>WA</d:State>" &
                "<d:PostalCode>98052</d:PostalCode>" &
              "</d:Address>" &
            "</m:properties>" &
          "</content>" &
        "</entry>" &
     "</feed>"

                Dim context = New DataServiceContext(request.ServiceRoot)
                'context.EnableAtom = True
                Dim query = context.CreateQuery(Of Customer)("Customers").Expand("Orders")
                For Each c In query
                    Assert.IsTrue(c.Orders.Count > 0, "There must be orders populated")
                Next
                Assert.IsTrue(context.Entities.Count = 2, "There must be 2 entities materialized")
            End Using
        End Using
    End Sub
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("Null collection property in content and other value outside of content")>
    Public Sub AssertWithEPMCollectionNullInContent()

        Using host As TestWebRequest = TestWebRequest.CreateForInProcessWcf()
            Using PlaybackService.OverridingPlayback.Restore()
                host.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
                host.StartService()

                Dim headers =
                    "HTTP/1.1 200 Ok" & vbCrLf &
                    "Content-Type: application/atom+xml" & vbCrLf &
                    vbCrLf

                Dim payload =
                    <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                    <entry xml:base="http://localhost/Test.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                        <id>http://localhost/Test.svc/Entities(100)</id>
                        <title type="text"></title>
                        <updated>2010-10-19T21:16:55Z</updated>
                        <author>
                            <name>Name1</name>
                            <email>someone1@something.com</email>
                        </author>
                        <link rel="edit" title="EntityWithCollection" href="Entities(100)"/>
                        <category term="AstoriaUnitTests.EntityWithCollection" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                        <content type="application/xml">
                            <m:properties>
                                <d:ID m:type="Edm.Int32">100</d:ID>
                                <d:Tags m:null="true"/>
                            </m:properties>
                        </content>
                    </entry>

                Dim response = headers & payload.ToString()

                PlaybackService.OverridingPlayback.Value = response

                Dim context = New DataServiceContext(New Uri(host.BaseUri), ODataProtocolVersion.V4)
                'context.EnableAtom = True
                Try
                    context.Execute(Of EntityWithCollection)(New Uri("/Entities", UriKind.Relative)).ToList()
                    Assert.Fail("Expected exception did not occur")
                Catch ex As InvalidOperationException
                    Assert.AreEqual("A null value was found for the property named 'Tags', which has the expected type 'Collection(Edm.String)[Nullable=False]'. The expected type 'Collection(Edm.String)[Nullable=False]' does not allow null values.", ex.Message)
                End Try
            End Using
        End Using

    End Sub

    <NamedStream("Stream1")>
    Public Class NamedStreamEntity
        Private m_ID As Integer
        Public Property ID As Integer
            Get
                Return m_ID
            End Get
            Set(value As Integer)
                m_ID = value
            End Set
        End Property
    End Class
    'Remove Atom
    ' <TestCategory("Partition1")>
    ' <TestMethod()>
    Public Sub NamedStream_DuplicatedEditLinkShouldFail()
        Using host As TestWebRequest = TestWebRequest.CreateForInProcessWcf()
            Using PlaybackService.OverridingPlayback.Restore()
                host.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
                host.StartService()

                Dim headers =
                    "HTTP/1.1 200 Ok" & vbCrLf &
                    "Content-Type: application/atom+xml" & vbCrLf &
                    vbCrLf

                Dim payload = <?xml version="1.0" encoding="utf-8"?>
                              <entry xml:base="http://localhost/Test" xmlns="http://www.w3.org/2005/Atom" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">
                                  <id>http://localhost/Test/Values(1)</id>
                                  <category term="AstoriaUnitTests.Tests.NamedStreamUnitTestModule_NamedStreamTests_MyEntityWithNamedStreams" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                                  <link rel="edit" title="NamedStreamEntity" href="Values(1)"/>
                                  <link rel="http://docs.oasis-open.org/odata/ns/edit-media/Stream1" title="Stream1" href="Values(1)/Stream1"/>
                                  <link rel="http://docs.oasis-open.org/odata/ns/edit-media/Stream1" title="Stream1" href="Values(1)/DuplicatedStream1"/>
                                  <title/>
                                  <updated>2011-05-13T00:49:29Z</updated>
                                  <author>
                                      <name/>
                                  </author>
                                  <content type="application/xml">
                                      <m:properties>
                                          <d:ID m:type="Edm.Int32">1</d:ID>
                                      </m:properties>
                                  </content>
                              </entry>

                Dim response = headers & payload.ToString()
                PlaybackService.OverridingPlayback.Value = response
                Dim context = New DataServiceContext(New Uri(host.BaseUri), ODataProtocolVersion.V4)
                'context.EnableAtom = True

                Try
                    context.Execute(Of NamedStreamEntity)(New Uri("/Entities", UriKind.Relative)).ToList()
                    Assert.Fail("Expected exception did not occur")
                Catch ex As InvalidOperationException
                    Assert.AreEqual(ODataLibResourceUtil.GetString("ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleEditLinks", "Stream1"), ex.Message)
                End Try
            End Using
        End Using
    End Sub

    'Remove Atom
    ' <TestCategory("Partition1")>
    ' <TestMethod()>
    Public Sub NamedStream_DuplicatedReadLinkShouldFail()
        Using host As TestWebRequest = TestWebRequest.CreateForInProcessWcf()
            Using PlaybackService.OverridingPlayback.Restore()
                host.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
                host.StartService()

                Dim headers =
                    "HTTP/1.1 200 Ok" & vbCrLf &
                    "Content-Type: application/atom+xml" & vbCrLf &
                    vbCrLf

                Dim payload = <?xml version="1.0" encoding="utf-8"?>
                              <entry xml:base="http://localhost/Test" xmlns="http://www.w3.org/2005/Atom" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">
                                  <id>http://localhost/Test/Values(1)</id>
                                  <category term="AstoriaUnitTests.Tests.NamedStreamUnitTestModule_NamedStreamTests_MyEntityWithNamedStreams" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                                  <link rel="edit" title="NamedStreamEntity" href="Values(1)"/>
                                  <link rel="http://docs.oasis-open.org/odata/ns/mediaresource/Stream1" title="Stream1" href="Values(1)/Stream1"/>
                                  <link rel="http://docs.oasis-open.org/odata/ns/mediaresource/Stream1" title="Stream1" href="Values(1)/DuplicatedStream1"/>
                                  <title/>
                                  <updated>2011-05-13T00:49:29Z</updated>
                                  <author>
                                      <name/>
                                  </author>
                                  <content type="application/xml">
                                      <m:properties>
                                          <d:ID m:type="Edm.Int32">1</d:ID>
                                      </m:properties>
                                  </content>
                              </entry>

                Dim response = headers & payload.ToString()
                PlaybackService.OverridingPlayback.Value = response
                Dim context = New DataServiceContext(New Uri(host.BaseUri), ODataProtocolVersion.V4)
                'context.EnableAtom = True

                Try
                    context.Execute(Of NamedStreamEntity)(New Uri("/Entities", UriKind.Relative)).ToList()
                    Assert.Fail("Expected exception did not occur")
                Catch ex As InvalidOperationException
                    Assert.AreEqual(ODataLibResourceUtil.GetString("ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleReadLinks", "Stream1"), ex.Message)
                End Try
            End Using
        End Using
    End Sub

    Public Class EntityWithCollection
        Property ID As Integer
        Property Tags As List(Of String)
    End Class
    'Remove Atom
    ' <TestCategory("Partition1")> <TestMethod(), Variation("Client Projections : ArgumentException on casting collection to IEnumerable inside a projection")>
    Public Sub CastingCollectionToIEnumerableShouldWork()
        Using request = TestWebRequest.CreateForLocal
            request.DataServiceType = GetType(NorthwindContext)
            request.StartService()

            Dim ctx = New DataServiceContext(New Uri(request.BaseUri))
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim q = From customer In ctx.CreateQuery(Of northwindClient.Customers)("Customers")
                    Select New With {
                    .CustomerID = customer.CustomerID,
                    .ContactName = customer.ContactName,
                    .Orders = CType((From order In customer.Orders
                                     Select New With {
                                         .OrderID = order.OrderID
                                         }), IEnumerable)}


            For Each c In q
                Assert.IsNotNull(c.Orders)
            Next c

        End Using
    End Sub

End Class

