'---------------------------------------------------------------------
' <copyright file="VersionedRequests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.Text
Imports System.Web
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports Microsoft.OData
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NorthwindModel

Partial Public Class ClientModule

    <TestClass()>
    Public Class VersionedRequests
        Inherits AstoriaTestCase

        Private Shared web As TestWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
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
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub AtomRequestsAreValid()
            Dim se As New SimpleEntity()
            se.ID = "the id"
            ctx.AddObject("/Blah", se)

            Dim payload = ExtractSaveChangesPlayback(ctx)
            TestUtil.AssertContains(payload, "<title />")
            TestUtil.AssertContains(payload, "<updated>")
            TestUtil.AssertContains(payload, DateTime.UtcNow.Year.ToString(System.Globalization.CultureInfo.InvariantCulture))
            TestUtil.AssertContains(payload, "</updated>")
            TestUtil.AssertContains(payload, "<author>")
            TestUtil.AssertContains(payload, "<name />")
            TestUtil.AssertContains(payload, "</author>")
            TestUtil.AssertContains(payload, "<id />")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub ALinqShouldUseTypeResolverForCastAndTypeIs()
            ' ALinq should use typeresolver for cast and typeis
            ctx.ResolveName = AddressOf ResolveNameCallback
            Try
                Dim q = From e In ctx.CreateQuery(Of SimpleEntityWithLong)("SimpleEntityWithLong")
                        Where TypeOf e Is SimpleEntityWithLongSubType
                        Select e
                Dim payload = ExtractPlayback(q)
                TestUtil.AssertContains(payload, "$filter=isof('ResolvedType_SimpleEntityWithLongSubType'")

                ' http://localhost:6000/TheTest/SimpleEntityWithLong()?$filter=cast('ResolvedType_SimpleEntityWithLongSubType')/SubID%20eq%201
                q = From e In ctx.CreateQuery(Of SimpleEntityWithLong)("SimpleEntityWithLong")
                    Where DirectCast(e, SimpleEntityWithLongSubType).SubID = 1
                    Select e
                payload = ExtractPlayback(q)
                TestUtil.AssertContains(payload, "$filter=cast('ResolvedType_SimpleEntityWithLongSubType')/SubID")

                ' Candidate bug?
                ' GET http://localhost:6000/TheTest/SimpleEntityWithLong(1L)
                q = From e In ctx.CreateQuery(Of SimpleEntityWithLong)("SimpleEntityWithLong")
                    Where TryCast(e, SimpleEntityWithLongSubType).ID = 1
                    Select e
                payload = ExtractPlayback(q)

                ' Candidate bug?
                ' The expression ((As SimpleEntityWithLongSubType).SubID = 1) is not supported. - why?
                q = From e In ctx.CreateQuery(Of SimpleEntityWithLong)("SimpleEntityWithLong")
                    Where TryCast(e, SimpleEntityWithLongSubType).SubID = 1
                    Select e
                payload = ExtractPlayback(q)
            Finally
                ctx.ResolveName = Nothing
            End Try
        End Sub

        Function ResolveNameCallback(ByVal t As Type) As String
            Return "ResolvedType_" + t.Name
        End Function

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub LinqRepros()

            'TODO: really should create seperate Linq module for these.
            Dim web As TestWebRequest = Nothing
            web = TestWebRequest.CreateForInProcessWcf
            web.DataServiceType = ServiceModelData.Northwind.ServiceModelType
            web.StartService()
            Dim ctx As NorthwindSimpleModel.NorthwindContext = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)

            Dim query1 = From c In ctx.CreateQuery(Of northwindClient.Customers)("Customers") From o In c.Orders Where c.CustomerID = "ALFKI" Select o

            Dim uri = query1.ToString()
            TestUtil.AssertContains(uri, "/Customers('ALFKI')/Orders")

            Dim query2 = From c In ctx.CreateQuery(Of northwindClient.Customers)("Customers") Where (c.CustomerID = "ALFKI" AndAlso c.CustomerID = "ANATR") OrElse (c.Region IsNot Nothing) Select c
            uri = query2.ToString()
            TestUtil.AssertContains(uri, "/Customers?$filter=CustomerID eq 'ALFKI' and CustomerID eq 'ANATR' or Region ne null")

        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub LinqExpressionNestingTest()
            ' This is a complementary test for AstoriaUnitTests.LinqTests.ExpressionNestingTest, but using the comprehension syntax
            ' A separate LINQ module should be created for these
            Dim web As TestWebRequest = Nothing
            web = TestWebRequest.CreateForInProcessWcf
            web.DataServiceType = ServiceModelData.Northwind.ServiceModelType
            web.StartService()
            Dim ctx As NorthwindSimpleModel.NorthwindContext = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)

            ' Ensures that generated expressions in the URI do not contain unnecessary levels of nesting
            ' This combination of test cases was generated using pair-wise combination reduction with the
            ' following dimensions: precedence, associativity and where the parentheses are.
            Dim queriesAndExpectedUris() As Tuple(Of IQueryable, String) =
            {
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID > o.OrderID + (o.OrderID * o.OrderID),
                    "TheTest/Orders?$filter=OrderID gt OrderID add OrderID mul OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID > o.OrderID - (o.OrderID \ o.OrderID),
                    "TheTest/Orders?$filter=OrderID gt OrderID sub OrderID div OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID < (o.OrderID + o.OrderID) - o.OrderID,
                    "/TheTest/Orders?$filter=OrderID lt OrderID add OrderID sub OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID < o.OrderID \ (o.OrderID * o.OrderID),
                    "/TheTest/Orders?$filter=OrderID lt OrderID div (OrderID mul OrderID)"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where ((o.OrderID = o.OrderID) AndAlso (o.OrderID >= o.OrderID)) OrElse (o.OrderID < o.OrderID),
                    "/TheTest/Orders?$filter=OrderID eq OrderID and OrderID ge OrderID or OrderID lt OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID = (o.OrderID + o.OrderID) * o.OrderID,
                    "/TheTest/Orders?$filter=OrderID eq (OrderID add OrderID) mul OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where (((((((o.OrderID * o.OrderID >= o.OrderID))))))),
                    "/TheTest/Orders?$filter=OrderID mul OrderID ge OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID >= (o.OrderID + (o.OrderID Mod o.OrderID)),
                    "/TheTest/Orders?$filter=OrderID ge OrderID add OrderID mod OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID <= o.OrderID * o.OrderID * o.OrderID,
                    "/TheTest/Orders?$filter=OrderID le OrderID mul OrderID mul OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID >= o.OrderID <> False,
                    "/TheTest/Orders?$filter=OrderID ge OrderID ne false"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID <> o.OrderID - o.OrderID * o.OrderID,
                    "/TheTest/Orders?$filter=OrderID ne OrderID sub OrderID mul OrderID"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where True = ((o.OrderID Mod o.OrderID) < o.OrderID) = False,
                    "/TheTest/Orders?$filter=true eq (OrderID mod OrderID lt OrderID) eq false"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID <> o.OrderID * (o.OrderID - o.OrderID \ (o.OrderID * (o.OrderID + o.OrderID))),
                    "/TheTest/Orders?$filter=OrderID ne OrderID mul (OrderID sub OrderID div (OrderID mul (OrderID add OrderID)))"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID > 1 AndAlso (((o.OrderID > 1))) Or (o.OrderID > 1 And o.OrderID > 1),
                    "/TheTest/Orders?$filter=OrderID gt 1 and OrderID gt 1 or OrderID gt 1 and OrderID gt 1"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID > 1 AndAlso ((o.OrderID > 1 Or o.OrderID > 1) And o.OrderID > 1),
                    "/TheTest/Orders?$filter=OrderID gt 1 and (OrderID gt 1 or OrderID gt 1) and OrderID gt 1"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID > o.OrderID * (o.OrderID \ o.OrderID),
                    "/TheTest/Orders?$filter=OrderID gt OrderID mul (OrderID div OrderID)"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID > o.OrderID * (o.OrderID Mod o.OrderID),
                    "/TheTest/Orders?$filter=OrderID gt OrderID mul (OrderID mod OrderID)"),
                New Tuple(Of IQueryable, String)(
                    From o In ctx.CreateQuery(Of northwindClient.Orders)("Orders")
                    Where o.OrderID > o.OrderID + (o.OrderID - o.OrderID),
                    "/TheTest/Orders?$filter=OrderID gt OrderID add (OrderID sub OrderID)")
            }

            For Each queryAndUri In queriesAndExpectedUris
                Dim query As IQueryable = queryAndUri.Item1
                Dim expected As String = queryAndUri.Item2
                Dim uri As String = query.ToString()

                Assert.IsTrue(uri.EndsWith(expected), String.Format("Uri {0} does not end with {1}.", uri, expected))
            Next

        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub DeletionUriShouldWorkWhenServerSupportDataLiterals()
            ' Astoria Silverlight Client : The URI Produced by the Client for deletion of resources 
            ' does not work after upgrading the server to support data literals
            '
            ' Ensure that keys are produced in key format rather than XML format.
            Dim e = New SimpleEntityWithLong()
            e.ID = 1
            Dim ctx = New DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4)
            ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent
            ctx.AttachTo("SimpleEntityWithLong", e)
            ctx.UpdateObject(e)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 0, 1)
            Dim playback = ExtractLastPlayback()
            Dim lines = playback.Split(Chr(13))
            TestUtil.AssertContains(lines(0), "SimpleEntityWithLong(1)")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub TranslationShouldBeCorrectInFilterPredicate()
            ' ALinq: incorrect translation in query with traversal+key lookup in filter predicate
            Dim typedContext = New SimpleContext(web.ServiceRoot)
            'typedContext.EnableAtom = True
            Dim q = From r In typedContext.RelatedThings Where r.OneEntity.ID = 1 Select r
            Dim playback = ExtractPlayback(q)
            TestUtil.AssertContains(playback, "/RelatedThings?$filter=OneEntity/ID%20eq%201")
        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub BytesConsistency()
            ' Astoria Client: byte[] and Binary used as keys are supported inconsistently
            Dim ctx = New DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4)
            ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent
            Dim arr As Byte()
            ReDim arr(0)
            arr(0) = 1

            Dim e = New TypedEntity(Of Byte(), Integer)()
            e.ID = arr
            ctx.AttachTo("SimpleEntityWithLong", e)
            ctx.UpdateObject(e)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 0, 1)
            Dim playback = ExtractLastPlayback()
            Dim lines = playback.Split(Chr(10))
            TestUtil.AssertContains(lines(0), "SimpleEntityWithLong(binary'AQ==')")
        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub BinaryConsistency()
            ' Astoria Client: byte[] and Binary used as keys are supported inconsistently
            Dim ctx = New DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4)
            ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent
            Dim arr As Byte()
            ReDim arr(0)
            arr(0) = 1

            Dim e = New TypedEntity(Of System.Data.Linq.Binary, Integer)()
            e.ID = arr
            ctx.AttachTo("SimpleEntityWithLong", e)
            ctx.UpdateObject(e)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 0, 1)
            Dim playback = ExtractLastPlayback()
            Dim lines = playback.Split(Chr(10))
            TestUtil.AssertContains(lines(0), "SimpleEntityWithLong(binary'AQ==')")
        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub ClientShouldNotRequireContentTypeOfEmptyBodyResponses()
            ' Client should not require content type for responses with empty bodies
            Dim etags = ",foo".Split(","c)
            Dim responseStatusCodes = "200,201,202,204".Split(","c)
            Dim contentTypes = ",application/atom+xml,abc/pqr".Split(","c)
            Using PlaybackService.OverridingPlayback.Restore()
                For Each etag In etags
                    For Each responseStatusCode In responseStatusCodes
                        For Each contentType In contentTypes

                            Dim playback =
                                "HTTP/1.1 " & responseStatusCode & " Ok" & vbNewLine &
                                "Location: http://abc-pqr.com/" & vbNewLine &
                                "OData-EntityId: http://abc-pqr.com/"
                            If etag.Length > 0 Then
                                playback &= vbNewLine & "ETag: " & etag
                            End If
                            If contentType.Length > 0 Then
                                playback &= vbNewLine & "Content-Type: " & contentType
                            End If

                            PlaybackService.OverridingPlayback.Value = playback
                            Dim e = New SimpleEntity()
                            e.ID = "1"
                            ctx.AddObject("SimpleEntity", e)
                            Dim response = ctx.SaveChanges()

                            Assert.AreSame(e, ctx.Entities.Single().Entity)
                            Assert.AreEqual(EntityStates.Unchanged, ctx.Entities.Single().State)
                            If etag.Length > 0 Then
                                Assert.AreEqual(etag, ctx.Entities.Single().ETag)
                            End If
                            For Each operationResponse In response
                                Assert.AreEqual(CInt(responseStatusCode), operationResponse.StatusCode)
                                Assert.IsNull(operationResponse.Error)
                            Next
                            ctx.Detach(ctx.Entities.First().Entity)
                        Next
                    Next
                Next
            End Using
        End Sub


#Region "EntryHasInstreamError"
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub EntryHasInstreamError()
            Using PlaybackService.OverridingPlayback.Restore
                PlaybackService.OverridingPlayback.Value =
                "HTTP/1.1 200 OK" & vbCrLf &
                "Content-Type: application/atom+xml" & vbCrLf &
                vbCrLf &
                "<entry xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' m:type='NorthwindModel.Customers' xmlns='http://www.w3.org/2005/Atom'>" &
                "  <id>http://localhost:3000/northwind.svc/Customers(ALFKI)</id>" &
                "  <content type='application/xml'>" &
                "    <m:properties>" &
                "      <d:CustomerID>ALFKI</d:CustomerID>" &
                "<m:error>broken</m:error>"

                Try
                    ctx.Execute(Of NorthwindSimpleModel.Customers)(New Uri("InstreamError", UriKind.Relative)).First()
                    Assert.Fail("expected DataServiceClientException")
                Catch ex As DataServiceClientException
                End Try

                Try
                    ctx.Execute(Of NorthwindSimpleModel.Customers)(New Uri("InstreamError", UriKind.Relative)).First()
                    Assert.Fail("expected DataServiceClientException")
                Catch ex As DataServiceClientException
                End Try
            End Using
        End Sub
#End Region
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub VersionSentOnGet()
            Dim q = ctx.CreateQuery(Of SimpleEntity)("/Blah")
            Dim payload = ExtractPlayback(q)
            TestUtil.AssertContains(payload, "OData-Version: 4.0")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub VersionSentOnUpdates()
            Dim e = New SimpleEntity()
            e.ID = "123"
            ctx.AddObject("SimpleEntity", e)
            Dim response As DataServiceResponse = Nothing
            Dim exception As Exception = Nothing
            Try
                response = ctx.SaveChanges()
            Catch ex As DataServiceRequestException
                exception = ex
                response = ex.Response
            End Try

            For Each r In response
                If Not exception Is Nothing Then
                    Assert.AreEqual(r.Error, exception.InnerException, "Expecting the same error exception")
                Else
                    exception = r.Error
                End If
                Exit For
            Next

            Assert.IsNotNull(exception.InnerException, "Error should have occurred when playing back.")
            Dim playback = ExtractPlayback(CType(exception.InnerException, Exception))
            TestUtil.AssertContains(playback, "OData-Version: 4.0")
        End Sub

        Private Function ExtractLastPlayback() As String
            Return ExtractPlaybackFromXml(AstoriaUnitTests.Stubs.PlaybackService.LastPlayback)
        End Function

        Private Function ExtractPlayback(ByVal e As IEnumerable) As String
            Try
                For Each o In e
                    If TypeOf o Is ChangeOperationResponse Then
                        If Not DirectCast(o, ChangeOperationResponse).Error Is Nothing Then
                            Dim inner = DirectCast(o, ChangeOperationResponse).Descriptor
                            If TypeOf inner Is EntityDescriptor Then
                                If Not DirectCast(o, ChangeOperationResponse).Error Is Nothing Then
                                    Throw DirectCast(o, ChangeOperationResponse).Error
                                End If
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                Return ExtractPlayback(ex)
            End Try
            Throw New Exception("Exception expected but none thrown")
        End Function

        Private Function ExtractPlayback(ByVal e As Exception) As String
            If Not e.InnerException Is Nothing Then
                If e.InnerException.GetType() Is GetType(ODataErrorException) Then
                    Return DirectCast(e.InnerException, ODataErrorException).Error.Message
                End If
            End If

            Return e.Message
        End Function

        Private Function ExtractPlaybackFromXml(ByVal msg As String) As String
            Dim doc = System.Xml.Linq.XDocument.Parse(msg)
            Return doc.Root.Value
        End Function

        Private Function ExtractSaveChangesPlayback(ByVal context As DataServiceContext) As String
            Dim response As DataServiceResponse = Nothing
            Try
                response = context.SaveChanges()
            Catch ex As DataServiceRequestException
                response = ex.Response
            End Try
            Return ExtractPlayback(response)
        End Function

    End Class

    Public Class RelatedThing
        Private theId As Long
        Private theEntity As SimpleEntityWithLong
        Public Property ID() As Long
            Get
                Return Me.theId
            End Get
            Set(ByVal value As Long)
                Me.theId = value
            End Set
        End Property
        Public Property OneEntity() As SimpleEntityWithLong
            Get
                Return Me.theEntity
            End Get
            Set(ByVal value As SimpleEntityWithLong)
                Me.theEntity = value
            End Set
        End Property
    End Class

    Public Class SimpleEntityWithLong
        Private theId As Long
        Private theRelatedThings As List(Of RelatedThing)
        Private thePayload As String
        Public Property ID() As Long
            Get
                Return Me.theId
            End Get
            Set(ByVal value As Long)
                Me.theId = value
            End Set
        End Property
        Public Property Payload() As String
            Get
                Return Me.thePayload
            End Get
            Set(ByVal value As String)
                Me.thePayload = value
            End Set
        End Property
        Public Property RelatedThings() As List(Of RelatedThing)
            Get
                Return Me.theRelatedThings
            End Get
            Set(ByVal value As List(Of RelatedThing))
                Me.theRelatedThings = value
            End Set
        End Property
    End Class

    Public Class SimpleContext
        Inherits DataServiceContext
        Public Sub New(ByVal serviceRoot As Global.System.Uri)
            MyBase.New(serviceRoot)
        End Sub

        Public ReadOnly Property SimpleEntities() As Global.Microsoft.OData.Client.DataServiceQuery(Of SimpleEntityWithLong)
            Get
                If (Me._SimpleEntities Is Nothing) Then
                    Me._SimpleEntities = MyBase.CreateQuery(Of SimpleEntityWithLong)("SimpleEntities")
                End If
                Return Me._SimpleEntities
            End Get
        End Property
        Public ReadOnly Property RelatedThings() As Global.Microsoft.OData.Client.DataServiceQuery(Of RelatedThing)
            Get
                If (Me._RelatedThings Is Nothing) Then
                    Me._RelatedThings = MyBase.CreateQuery(Of RelatedThing)("RelatedThings")
                End If
                Return Me._RelatedThings
            End Get
        End Property
        Private _SimpleEntities As Global.Microsoft.OData.Client.DataServiceQuery(Of SimpleEntityWithLong)
        Private _RelatedThings As Global.Microsoft.OData.Client.DataServiceQuery(Of RelatedThing)
    End Class
    Public Class SimpleEntityWithLongSubType
        Inherits SimpleEntityWithLong
        Public Property SubID() As Integer
            Get
                Return 1
            End Get
            Set(ByVal value As Integer)

            End Set
        End Property
    End Class
End Class
