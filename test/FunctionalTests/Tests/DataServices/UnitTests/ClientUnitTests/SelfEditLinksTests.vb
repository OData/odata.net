'---------------------------------------------------------------------
' <copyright file="SelfEditLinksTests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Linq
Imports System.Xml.Linq
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports AstoriaUnitTests.Stubs
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Partial Public Class ClientModule
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    'Remove Atom
    ' <TestClass()>
    Public Class SelfEditLinkTests
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
            Me.ctx = New DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4)
            'Me.'ctx.EnableAtom = True
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub

        Private Function GetNavigationLink(ByVal entityDescriptor As EntityDescriptor, ByVal propertyName As String) As Uri
            GetNavigationLink = (From linkInfo In entityDescriptor.LinkInfos
                                 Where linkInfo.Name = propertyName
                                 Select linkInfo.NavigationLink).FirstOrDefault
        End Function

        Private Function GetRelationshipLink(ByVal entityDescriptor As EntityDescriptor, ByVal propertyName As String) As Uri
            GetRelationshipLink = (From linkInfo In entityDescriptor.LinkInfos
                                   Where linkInfo.Name = propertyName
                                   Select linkInfo.AssociationLink).FirstOrDefault
        End Function

        <TestCategory("Partition1")> <TestMethod()>
        Public Sub SimpleCRUDTest()
            ' start a new web service so that we can use a different endpoint to make sure client uses the new endpoint
            ' and not the existing one from which the context was created.
            Using web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf

                web1.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
                web1.StartService()

                Dim engine = CombinatorialEngine.FromDimensions(
                        New Dimension("MergeOption", New MergeOption() {MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges}),
                        New Dimension("ServiceUri", New Uri() {web1.ServiceRoot}),
                        New Dimension("UseBatchMode", New Boolean() {True, False}),
                        New Dimension("UseQuery", New Boolean() {True, False}))
                TestUtil.RunCombinatorialEngineFail(engine, AddressOf SimpleCRUDTest_Inner)
            End Using
        End Sub

        Private Sub SimpleCRUDTest_Inner(ByVal values As Hashtable)
            Dim useMergeOption = CType(values("MergeOption"), MergeOption)
            Dim useQueryToPopulate = CType(values("UseQuery"), Boolean)
            Dim useBatchMode = CType(values("UseBatchMode"), Boolean)
            Dim serviceUri = CType(values("ServiceUri"), Uri)

            Using PlaybackService.OverridingPlayback.Restore
                Dim navigationProperties = New String() {"BestFriend", "Orders"}

                Dim queryLink As String = Nothing
                Dim editLink As String = Nothing
                Dim id As String = Nothing
                Dim navigationLink As String = Nothing
                Dim relationshipLink As String = Nothing

                Dim c = GetCustomer(serviceUri, "123", editLink, queryLink, id, navigationLink, relationshipLink, useQueryToPopulate, useBatchMode)

                Dim queryLink1 As String = Nothing
                Dim editLink1 As String = Nothing
                Dim id1 As String = Nothing
                Dim navigationLink1 As String = Nothing
                Dim relationshipLink1 As String = Nothing

                Dim c1 = GetCustomer(serviceUri, "99", editLink1, queryLink1, id1, navigationLink1, relationshipLink1, useQueryToPopulate, useBatchMode)

                Dim orderQueryLink As String = Nothing
                Dim orderEditLink As String = Nothing
                Dim orderId As String = Nothing

                Dim o = GetOrder(serviceUri, "456", orderEditLink, orderQueryLink, orderId, useQueryToPopulate, useBatchMode)

                'Verify links in entity descriptor
                VerifyLinksForEntity(c, id, queryLink, editLink, navigationLink, relationshipLink, navigationProperties, navigationProperties)
                VerifyLinksForEntity(c1, id1, queryLink1, editLink1, navigationLink1, relationshipLink1, navigationProperties, navigationProperties)
                VerifyLinksForEntity(o, orderId, orderQueryLink, orderEditLink, Nothing, Nothing, Nothing, Nothing)

                ' Update (PATCH) a single entity
                Dim options As SaveChangesOptions = If(useBatchMode, SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.None)
                ctx.MergeOption = useMergeOption
                PlaybackService.OverridingPlayback.Value = GetCustomerPayloadOnlyHeaders(useBatchMode)
                ctx.UpdateObject(c)
                ctx.SaveChanges(options)

                ' The write edit link should be used in the request uri
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("PATCH " & editLink))
                ' The client doesn't send any deferred nav properties link in payload
                Assert.IsFalse(PlaybackService.LastPlayback.Contains("http://docs.oasis-open.org/odata/ns/related/Orders"))
                Assert.IsFalse(PlaybackService.LastPlayback.Contains("http://docs.oasis-open.org/odata/ns/related/BestFriend"))

                ' Update (PUT) a single entity
                ctx.UpdateObject(c)
                ctx.SaveChanges(options Or SaveChangesOptions.ReplaceOnUpdate)
                ' The write edit link should be used in the request uri
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("PUT " & editLink))
                ' The client doesn't send any deferred nav properties link in payload
                Assert.IsFalse(PlaybackService.LastPlayback.Contains("http://docs.oasis-open.org/odata/ns/related/Orders"))
                Assert.IsFalse(PlaybackService.LastPlayback.Contains("http://docs.oasis-open.org/odata/ns/related/BestFriend"))

                ' LoadProperty - collection navigation property
                Try
                    PlaybackService.OverridingPlayback.Value = Nothing
                    ctx.LoadProperty(c, "Orders")
                    Assert.Fail("Expected exception since the payload won't be valid, but no exception thrown")
                Catch ex As InvalidOperationException
                    Assert.IsTrue(PlaybackService.LastPlayback.Contains("GET " & navigationLink & "Orders"))
                End Try

                ' LoadProperty - collection ref property
                Try
                    ctx.LoadProperty(c, "BestFriend")
                    Assert.Fail("Expected exception since the payload won't be valid, but no exception thrown")
                Catch ex As InvalidOperationException
                    Assert.IsTrue(PlaybackService.LastPlayback.Contains("GET " & navigationLink & "BestFriend"))
                End Try

                ' AddLink (navigation link doesn't play a role in $ref scenario)
                ctx.AddLink(c, "Orders", o)
                ctx.SaveChanges()
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("POST " & relationshipLink))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains(orderId))

                ' Remove Link (navigation link doesn't play a role in $ref scenario)
                ctx.DeleteLink(c, "Orders", o)
                ctx.SaveChanges()
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & relationshipLink & "Orders" & "?$id=" & orderId))

                'AddRelatedObject should use source object's navigation link to the target object.
                Dim o1 = New Order(765, 4)
                ctx.AddRelatedObject(c, "Orders", o1)
                Try
                    ctx.SaveChanges()
                    Assert.Fail("Expected exception since the payload won't be valid, but no exception thrown")
                Catch ex As Exception
                    Assert.IsTrue(PlaybackService.LastPlayback.Contains("POST " & navigationLink & "Orders"))
                    ctx.Detach(o1)
                End Try

                ' SetLink (navigation link doesn't play a role in $ref scenario)
                ctx.SetLink(c, "BestFriend", c1)
                ctx.SaveChanges()
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("PUT " & relationshipLink & "BestFriend"))

                ' SetLink (navigation link doesn't play a role in $ref scenario)
                ctx.SetLink(c, "BestFriend", Nothing)
                ctx.SaveChanges()
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & relationshipLink & "BestFriend"))

                ' TryGetUri
                Dim u As Uri = Nothing
                ctx.TryGetUri(c, u)
                Assert.IsTrue(u.OriginalString = id, "TryGetUri should return edit link always")

                ' Delete
                ctx.DeleteObject(c)
                ctx.SaveChanges()
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & editLink))

                ctx.DeleteObject(c1)
                ctx.SaveChanges()
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & editLink1))

                ctx.DeleteObject(o)
                ctx.SaveChanges()
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & orderEditLink))
            End Using
        End Sub

        ' This test verifies that the values of entity descriptor object is correct, for different kind of server responses.
        <TestCategory("Partition1")> <TestMethod()>
        Public Sub VerifyVariousLinksInPayload()
            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("BatchMode", New Boolean() {True, False}),
                New Dimension("QueryMethod", New Boolean() {True, False}),
                New Dimension("Location", New String() {web.ServiceRoot.OriginalString & "/writeservice.svc/Location/Customers(123)", Nothing}),
                New Dimension("QueryLink", New String() {web.ServiceRoot.OriginalString & "/readservice.svc/Customers(123)", Nothing}),
                New Dimension("EditLink", New String() {web.ServiceRoot.OriginalString & "/writeservice.svc/Customers(123)", Nothing}),
                New Dimension("NavigationLink", New String() {web.ServiceRoot.OriginalString & "/writeservice.svc/Customers(123)/related", Nothing}))
            TestUtil.RunCombinatorialEngineFail(engine, AddressOf VerifyingLinksInQuery_Inner)
        End Sub

        Private Sub VerifyingLinksInQuery_Inner(ByVal values As Hashtable)
            Dim batchMode = CType(values("BatchMode"), Boolean)
            Dim queryMethod = CType(values("QueryMethod"), Boolean)
            Dim locationHeader = CType(values("Location"), String)
            Dim queryLink = CType(values("QueryLink"), String)
            Dim editLink = CType(values("EditLink"), String)
            Dim navigationLink = CType(values("NavigationLink"), String)
            Dim relationshipLink = CType(values("RelationshipLink"), String)

            Using PlaybackService.OverridingPlayback.Restore
                Dim id = web.ServiceRoot.OriginalString & "/id/Customers(123)"

                PlaybackService.OverridingPlayback.Value = GetCustomerPayload(id, editLink, queryLink, navigationLink, relationshipLink, locationHeader, queryMethod, batchMode)

                Dim actualEditLink = If((editLink = Nothing AndAlso Not queryMethod), locationHeader, editLink)

                'If the location header is not specified then the client throws. So exclude those combinations
                Dim expectedException As Boolean = (Not queryMethod AndAlso locationHeader = Nothing)

                Try
                    If (queryMethod) Then
                        PopulateContextByQuery(ctx.CreateQuery(Of Customer)("/Customers(123)"), batchMode)
                    Else
                        PopulateContextByInsert(Of Customer)("Customers", batchMode)
                    End If
                    Assert.IsFalse(expectedException, "exception expected, but no exception was thrown")
                    Assert.IsTrue(ctx.Entities.Count = 1, "There must be exactly one entity in the context")
                    Dim ed = ctx.Entities(0)
                    Assert.AreEqual(actualEditLink, If(ed.EditLink = Nothing, Nothing, ed.EditLink.OriginalString), "Edit Link must match")
                    Assert.AreEqual(queryLink, If(ed.SelfLink = Nothing, Nothing, ed.SelfLink.OriginalString), "query Link must match")

                    Dim bfNavigationLink = GetNavigationLink(ed, "BestFriend")
                    Dim oNavigationLink = GetNavigationLink(ed, "Orders")
                    Assert.IsTrue((bfNavigationLink = Nothing AndAlso navigationLink = Nothing) OrElse (bfNavigationLink.OriginalString = navigationLink & "BestFriend"), "navigation link must match")
                    Assert.IsTrue((oNavigationLink = Nothing AndAlso navigationLink = Nothing) OrElse (oNavigationLink.OriginalString = navigationLink & "Orders"), "query Link must match")

                    Dim bfRelationshipLink = GetRelationshipLink(ed, "BestFriend")
                    Dim oRelationshipLink = GetRelationshipLink(ed, "Orders")
                    Assert.IsTrue((bfRelationshipLink = Nothing AndAlso relationshipLink = Nothing) OrElse (bfRelationshipLink.OriginalString = relationshipLink & "BestFriend"), "relationship link must match")
                    Assert.IsTrue((oRelationshipLink = Nothing AndAlso relationshipLink = Nothing) OrElse (oRelationshipLink.OriginalString = relationshipLink & "Orders"), "relationship Link must match")

                    Assert.IsTrue(ed.Identity.OriginalString = id, "Id must match")
                Catch ex As Exception
                    Dim innerException As Exception = Nothing
                    Assert.IsTrue(expectedException, "No exception was expected, but was thrown")
                    If (batchMode) Then
                        Assert.IsTrue(ex.GetType() = GetType(DataServiceRequestException), "For query operations, we must throw DataServiceRequest exception")
                        innerException = ex.InnerException
                    Else
                        innerException = ex
                    End If
                    Assert.IsTrue(innerException.GetType() = GetType(NotSupportedException), "Inner exception must be NotSupportedException")
                    Assert.IsTrue(innerException.Message.Contains(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Deserialize_NoLocationHeader")), "the error message was not expected")
                Finally
                    ctx.Detach(ctx.Entities(0).Entity)
                End Try
            End Using
        End Sub

        <TestCategory("Partition1")> <TestMethod()>
        Public Sub SimpleCRUDTestInBatch()
            ' start a new web service so that we can use a different endpoint to make sure client uses the new endpoint
            ' and not the existing one from which the context was created.
            Dim web1 As TestWebRequest = TestWebRequest.CreateForInProcessWcf
            web1.ServiceType = GetType(AstoriaUnitTests.Stubs.PlaybackService)
            web1.StartService()

            Try
                Dim engine = CombinatorialEngine.FromDimensions(
                    New Dimension("ServiceUri", New Uri() {web1.ServiceRoot}),
                    New Dimension("UseBatchMode", New Boolean() {True, False}),
                    New Dimension("UseQuery", New Boolean() {True, False}))
                TestUtil.RunCombinatorialEngineFail(engine, AddressOf SimpleCRUDTestInBatch_Inner)
            Finally
                web1.StopService()
            End Try
        End Sub

        Private Sub SimpleCRUDTestInBatch_Inner(ByVal values As Hashtable)
            Dim useQueryToPopulate = CType(values("UseQuery"), Boolean)
            Dim useBatchMode = CType(values("UseBatchMode"), Boolean)
            Dim serviceUri = CType(values("ServiceUri"), Uri)

            Dim queryLink As String = Nothing
            Dim editLink As String = Nothing
            Dim id As String = Nothing
            Dim navigationLink As String = Nothing
            Dim relationshipLink As String = Nothing

            Dim c = GetCustomer(serviceUri, "123", editLink, queryLink, id, navigationLink, relationshipLink, useQueryToPopulate, useBatchMode)

            Dim queryLink1 As String = Nothing
            Dim editLink1 As String = Nothing
            Dim id1 As String = Nothing
            Dim navigationLink1 As String = Nothing
            Dim relationshipLink1 As String = Nothing

            Dim c1 = GetCustomer(serviceUri, "99", editLink1, queryLink1, id1, navigationLink1, relationshipLink1, useQueryToPopulate, useBatchMode)

            Dim orderQueryLink As String = Nothing
            Dim orderEditLink As String = Nothing
            Dim orderId As String = Nothing

            Dim o = GetOrder(serviceUri, "456", orderEditLink, orderQueryLink, orderId, useQueryToPopulate, useBatchMode)

            ' Update (PATCH) a single entity
            ctx.UpdateObject(c)
            ctx.AddLink(c, "Orders", o)
            ctx.SetLink(c, "BestFriend", c1)

            Try
                ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)
                Assert.Fail("Expected an exception, but no exception was thrown")
            Catch ex As InvalidOperationException
                'Eat the exception
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("PATCH " & editLink))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("POST " & relationshipLink & "Orders"))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains(orderId))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("PUT " & relationshipLink & "BestFriend"))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains(orderId))

                ' Since the request fails, AddLink somehow stays in its state, but SetLink doesn't
                ctx.DetachLink(c, "Orders", o)
            End Try


            ' Delete
            ctx.DeleteLink(c, "Orders", o)
            ctx.SetLink(c, "BestFriend", Nothing)
            ctx.DeleteObject(c1)
            ctx.DeleteObject(o)
            ctx.DeleteObject(c)
            Try
                ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)
                Assert.Fail("Expected an exception, but no exception was thrown")
            Catch ex As InvalidOperationException
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & relationshipLink & "Orders" & "?$id=" & orderId))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & relationshipLink & "BestFriend"))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & editLink))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & editLink1))
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("DELETE " & orderEditLink))
            End Try

            ' Detach all the objects
            ctx.Detach(c)
            ctx.Detach(c1)
            ctx.Detach(o)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub VerifyEntityDescriptorMerging()
            Dim engine = CombinatorialEngine.FromDimensions(
                    New Dimension("EntityState", New EntityStates() {EntityStates.Added, EntityStates.Deleted, EntityStates.Modified, EntityStates.Unchanged}),
                    New Dimension("MergeOption", New MergeOption() {MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges}),
                    New Dimension("QueryLink", New String() {web.ServiceRoot.OriginalString & "/readservice.svc/update/Customers(123)", Nothing}),
                    New Dimension("EditLink", New String() {web.ServiceRoot.OriginalString & "/writeservice.svc/update/Customers(123)", Nothing}),
                    New Dimension("NavigationLink", New String() {web.ServiceRoot.OriginalString & "/writeservice.svc/update/Customers(123)/related", Nothing}),
                    New Dimension("UseBatchMode", New Boolean() {True, False}))
            TestUtil.RunCombinatorialEngineFail(engine, AddressOf VerifyingEntityDescriptorMerging_Inner)
        End Sub

        Private Function GetNavigationLinkCount(ByVal entityDescriptor As EntityDescriptor) As Integer
            GetNavigationLinkCount = (From rd In entityDescriptor.LinkInfos
                                      Where Not rd.NavigationLink Is Nothing
                                      Select rd).Count
        End Function

        Private Function GetRelationshipLinkCount(ByVal entityDescriptor As EntityDescriptor) As Integer
            GetRelationshipLinkCount = (From rd In entityDescriptor.LinkInfos
                                        Where Not rd.AssociationLink Is Nothing
                                        Select rd).Count
        End Function

        Private Sub VerifyingEntityDescriptorMerging_Inner(ByVal values As Hashtable)
            Dim useMergeOption = CType(values("MergeOption"), MergeOption)
            Dim useBatchMode = CType(values("UseBatchMode"), Boolean)
            Dim state = CType(values("EntityState"), EntityStates)
            Dim navigationProperties = New String() {"BestFriend", "Orders"}
            Dim updateQueryLink = CStr(values("QueryLink"))
            Dim updateEditLink = CStr(values("EditLink"))
            Dim updateNavigationLink = CStr(values("NavigationLink"))
            Dim updateRelationshipLink = CStr(values("RelationshipLink"))

            Using PlaybackService.OverridingPlayback.Restore
                Assert.AreEqual(0, ctx.Entities.Count, "Expecting the context to be clean")
                ctx.MergeOption = useMergeOption
                Dim id As String = Nothing
                Dim insertQueryLink As String = Nothing, insertEditLink As String = Nothing, insertNavigationLink As String = Nothing, insertRelationshipLink As String = Nothing

                PlaybackService.OverridingPlayback.Value = GetCustomerPayloadWithLinks(ctx.BaseUri.OriginalString, id, "123", False, True, insertQueryLink, insertEditLink, insertNavigationLink, insertRelationshipLink, True)

                ' Populate the ctx with the entity in the right state
                AstoriaUnitTests.DataServiceContextTestUtil.CreateEntity(Of Customer)(ctx, "Customers", state)

                'Refresh the object by calling execute on the edit link and make sure that the new links are picked up
                PlaybackService.OverridingPlayback.Value = GetCustomerPayload(
                    id,
                    updateEditLink,
                    updateQueryLink,
                    updateNavigationLink,
                    updateRelationshipLink,
                    Nothing,
                    True,
                    useBatchMode)

                ' add an new link and remove an existing link and verify that the old link doesn't go away and the new link shows up
                ' for preserve changes and overwrite changes
                PlaybackService.OverridingPlayback.Value = PlaybackService.OverridingPlayback.Value.Replace("link rel='http://docs.oasis-open.org/odata/ns/related/Orders'", "link rel='http://docs.oasis-open.org/odata/ns/related/Orders1'")
                PlaybackService.OverridingPlayback.Value = PlaybackService.OverridingPlayback.Value.Replace("href='" & updateNavigationLink & "Orders'", "href='" & updateNavigationLink & "Orders1'")

                PlaybackService.OverridingPlayback.Value = PlaybackService.OverridingPlayback.Value.Replace("link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/Orders'", "link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/Orders1'")
                PlaybackService.OverridingPlayback.Value = PlaybackService.OverridingPlayback.Value.Replace("href='" & updateRelationshipLink & "Orders'", "href='" & updateRelationshipLink & "Orders1'")
                Dim c As Customer = Nothing

                If useBatchMode = True Then
                    Dim qor = TryCast(ctx.ExecuteBatch(ctx.CreateQuery(Of Customer)("/Customers(123)")).First(), QueryOperationResponse(Of Customer))
                    c = qor.First()
                Else
                    c = ctx.Execute(Of Customer)("/Customers(123)").First()
                End If

                Dim expectedQueryLink = If(updateQueryLink, insertQueryLink)
                Dim expectedEditLink = If(updateEditLink, insertEditLink)
                Dim expectedNavigationLink = If(updateNavigationLink, insertNavigationLink)
                Dim expectedRelationshipLink = If(updateRelationshipLink, insertRelationshipLink)

                'If updateNavigationLink is Nothing, then there will be no links in the payload. Hence whatever links were present before the update
                'operation, only those links will be present.
                If (state = EntityStates.Added) Then
                    ' If state = Added, then the query will give a new EntityDescriptor and there should be no merging
                    Assert.AreEqual(2, ctx.Entities.Count, "Expecting 2 entities in the context - one for added and one for query")
                    VerifyLinksForEntity(c, id, updateQueryLink, updateEditLink, updateNavigationLink, updateRelationshipLink, If(updateNavigationLink = Nothing, Nothing, New String() {"BestFriend", "Orders1"}), If(updateRelationshipLink = Nothing, Nothing, New String() {"BestFriend", "Orders1"}))
                    Assert.AreEqual(If(updateNavigationLink = Nothing, 0, 2), GetNavigationLinkCount(ctx.Entities(1)), "There must be 2 navigation links")
                    Assert.AreEqual(If(updateRelationshipLink = Nothing, 0, 2), GetRelationshipLinkCount(ctx.Entities(1)), "There must be 2 relationship links")
                ElseIf (useMergeOption = MergeOption.OverwriteChanges Or (useMergeOption = MergeOption.PreserveChanges And (state = EntityStates.Unchanged Or state = EntityStates.Deleted))) Then
                    Assert.AreEqual(1, ctx.Entities.Count, "Expecting only 1 entity in the context")
                    Dim updatedNavigationLinkProperties = If(updateNavigationLink = Nothing, navigationProperties, New String() {"BestFriend", "Orders1"})
                    Dim updatedRelationshipLinkProperties = If(updateRelationshipLink = Nothing, navigationProperties, New String() {"BestFriend", "Orders1"})
                    VerifyLinksForEntity(c, id, expectedQueryLink, expectedEditLink, expectedNavigationLink, expectedRelationshipLink, updatedNavigationLinkProperties, updatedRelationshipLinkProperties)
                    If (Not updateNavigationLink = Nothing) Then
                        Assert.AreEqual(3, GetNavigationLinkCount(ctx.Entities(0)), "There must be 3 navigation links")
                        Assert.AreEqual(insertNavigationLink & "Orders", GetNavigationLink(ctx.Entities(0), "Orders").OriginalString, "Since orders was not specified in the payload, it must still have the old uri")
                    Else
                        Assert.AreEqual(2, GetNavigationLinkCount(ctx.Entities(0)), "There must be 2 navigation links")
                    End If
                    If (Not updateRelationshipLink = Nothing) Then
                        Assert.AreEqual(3, GetRelationshipLinkCount(ctx.Entities(0)), "There must be 3 relationship links")
                        Assert.AreEqual(insertRelationshipLink & "Orders", GetRelationshipLink(ctx.Entities(0), "Orders").OriginalString, "Since orders was not specified in the payload, it must still have the old uri")
                    Else
                        Assert.AreEqual(2, GetRelationshipLinkCount(ctx.Entities(0)), "There must be 2 relationship links")
                    End If
                Else
                    Assert.AreEqual(1, ctx.Entities.Count, "Expecting only 1 entity in the context")
                    VerifyLinksForEntity(c, id, insertQueryLink, insertEditLink, insertNavigationLink, insertRelationshipLink, navigationProperties, navigationProperties)
                    Assert.AreEqual(2, GetNavigationLinkCount(ctx.Entities(0)), "There must be 2 navigation links")
                    Assert.AreEqual(2, GetRelationshipLinkCount(ctx.Entities(0)), "There must be 2 relationship links")
                End If

                'Detach all the entities
                Dim ed = New List(Of EntityDescriptor)(ctx.Entities)
                For Each e In ed
                    ctx.Detach(e.Entity)
                Next
            End Using
        End Sub

        <TestCategory("Partition1")> <TestMethod()>
        Public Sub ServerDrivePagingWithNavigationLinks()
            Using PlaybackService.OverridingPlayback.Restore

                'Return customers with following information:
                '   - Self link referring the same end point
                '   - Edit link referring to the different end point
                '   - navigation links referring to the different end point
                Dim serviceUri As String = web.ServiceRoot.OriginalString
                PlaybackService.OverridingPlayback.Value =
                "HTTP/1.1 200 OK" & vbCrLf &
                "Content-Type: application/atom+xml" & vbCrLf &
                "Content-ID: 1" & vbCrLf &
                vbCrLf &
                "<feed xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' m:type='AstoriaUnitTests.Stubs.Customer' xmlns='http://www.w3.org/2005/Atom'>" &
                    "<entry>" &
                    "  <id>" & serviceUri & "/Id/Customers(123)</id>" &
                    "<link rel='self' href='" & serviceUri & "/Customers(123)/SelfLink' />" &
                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=feed' href='" & serviceUri & "/navigation/orders' />" &
                    "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='" & serviceUri & "/navigation/bestfriend' />" &
                    "  <content type='application/xml'>" &
                    "    <m:properties>" &
                    "      <d:ID>123</d:ID>" &
                    "      <d:Name>Foo</d:Name>" &
                    "    </m:properties>" &
                    "  </content>" &
                    "</entry>" &
                    "<entry>" &
                    "  <id>" & serviceUri & "/Id/Customers(121)</id>" &
                    "<link rel='self' href='" & serviceUri & "/Customers(121)/SelfLink' />" &
                    "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=feed' href='" & serviceUri & "/navigation/orders' />" &
                    "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='" & serviceUri & "/navigation/bestfriend' />" &
                    "  <content type='application/xml'>" &
                    "    <m:properties>" &
                    "      <d:ID>121</d:ID>" &
                    "      <d:Name>Foo1</d:Name>" &
                    "    </m:properties>" &
                    "  </content>" &
                    "</entry>" &
                    "<link rel='next' href='" & serviceUri & "/Customers?$skiptoken=2' />" &
                "</feed>"

                Dim q = ctx.CreateQuery(Of Customer)("Customers")
                Dim qor = CType(q.Execute(), QueryOperationResponse(Of Customer))

                Dim i As Integer = 0
                For Each c In qor
                    i = i + 1
                Next
                Assert.IsTrue(i = 2, "The context should see 2 customers instance")

                ctx.Execute(Of Customer)(qor.GetContinuation())
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("GET " & serviceUri & "/Customers?$skiptoken=2"))
            End Using
        End Sub
        <TestCategory("Partition1")> <TestMethod()>
        Public Sub LinqQueryWithNavigationLinks()
            Using PlaybackService.OverridingPlayback.Restore
                'the following loads the customer to ctx 
                Dim customer = GetCustomer(web.ServiceRoot, "123", Nothing, Nothing, Nothing, Nothing, Nothing, False, False)

                'first build the query using LINQ api
                'then construct the requestUri and check it with convention without executing the query.

                Dim orderQuery As DataServiceQuery(Of Order) = CType((From c In ctx.CreateQuery(Of Customer)("Customers")
                                                                      Where c.ID = 123
                                                                      From o In c.Orders
                                                                      Select o), DataServiceQuery(Of Order))
                Assert.AreEqual(orderQuery.RequestUri.AbsoluteUri, web.ServiceRoot.AbsoluteUri & "/Customers(123)/Orders", "Linq queries needs to use convention to build queries.")


                orderQuery = CType((From c In ctx.CreateQuery(Of Customer)("Customers")
                                    Where c.ID = 123
                                    From o In c.BestFriend.Orders
                                    Select o), DataServiceQuery(Of Order))
                Assert.AreEqual(orderQuery.RequestUri.AbsoluteUri, web.ServiceRoot.AbsoluteUri & "/Customers(123)/BestFriend/Orders", "Linq queries needs to use convention to build queries.")


                Dim customerQuery As DataServiceQuery(Of Customer) = CType((From c In ctx.CreateQuery(Of Customer)("Customers")
                                                                            Where c.ID = 123
                                                                            Select c.BestFriend), DataServiceQuery(Of Customer))
                Assert.AreEqual(customerQuery.RequestUri.AbsoluteUri, web.ServiceRoot.AbsoluteUri & "/Customers(123)/BestFriend", "Linq queries needs to use convention to build queries.")

            End Using
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub VerifyLinksWithUpdateResponses()
            Dim engine = CombinatorialEngine.FromDimensions(
               New Dimension("BatchMode", New Boolean() {True, False}),
               New Dimension("QueryLink", New String() {web.ServiceRoot.OriginalString & "/readservice.svc/update/Customers(123)", Nothing}),
               New Dimension("EditLink", New String() {web.ServiceRoot.OriginalString & "/writeservice.svc/update/Customers(123)", Nothing}),
               New Dimension("MergeOption", New MergeOption() {MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges}),
               New Dimension("NavigationLink", New String() {web.ServiceRoot.OriginalString & "/writeservice.svc/update/Customers(123)/related", Nothing})
               )
            TestUtil.RunCombinatorialEngineFail(engine, AddressOf VerifyLinksWithUpdateResponses_Inner)
        End Sub

        Private Sub VerifyLinksWithUpdateResponses_Inner(ByVal values As Hashtable)
            Dim batchMode = CType(values("BatchMode"), Boolean)
            Dim queryLink = CType(values("QueryLink"), String)
            Dim editLink = CType(values("EditLink"), String)
            Dim navigationLink = CType(values("NavigationLink"), String)
            Dim relationshipLink = CType(values("RelationshipLink"), String)
            Dim mergeOption = CType(values("MergeOption"), MergeOption)

            Dim id = web.ServiceRoot.OriginalString & "/id/Customers(123)"
            Dim insertEditLink = web.ServiceRoot.OriginalString & "/writeservice.svc/Customers(123)"
            Dim insertQueryLink = web.ServiceRoot.OriginalString & "/readservice.svc/Customers(123)"
            Dim insertNavigationLink = web.ServiceRoot.OriginalString & "/writeservice.svc/Customers(123)/related"
            Dim insertRelationshipLink = web.ServiceRoot.OriginalString & "/relationships.svc/Customers(123)/relatedlinks"

            Using PlaybackService.OverridingPlayback.Restore
                Dim oldContext = Me.ctx

                Try
                    ctx = New DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4)
                    'ctx.EnableAtom = True
                    ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent

                    'Insert a customer into the context
                    Dim cust = GetCustomer(web.ServiceRoot, "123", insertEditLink, insertQueryLink, id, insertNavigationLink, insertRelationshipLink, True, False)

                    'Generate a payload for update operation
                    ctx.MergeOption = mergeOption
                    PlaybackService.OverridingPlayback.Value = GetCustomerPayload(id, editLink, queryLink, navigationLink, relationshipLink, Nothing, False, batchMode)
                    ctx.UpdateObject(cust)
                    ctx.SaveChanges(If(batchMode, SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.None))

                    ' If there was no link specified in the payload, then the value should not get updated, otherwise it should
                    Dim ed = ctx.GetEntityDescriptor(cust)
                    Assert.AreEqual(If(editLink, insertEditLink), ed.EditLink.OriginalString, "edit link should match")
                    Assert.AreEqual(If(queryLink, insertQueryLink), ed.SelfLink.OriginalString, "self link should match")

                    For Each linkInfo As LinkInfo In ed.LinkInfos
                        Assert.AreEqual(If(navigationLink = Nothing, insertNavigationLink & linkInfo.Name, navigationLink & linkInfo.Name), linkInfo.NavigationLink.OriginalString, "navigation link should match")
                        Assert.AreEqual(If(relationshipLink = Nothing, insertRelationshipLink & linkInfo.Name, relationshipLink & linkInfo.Name), linkInfo.AssociationLink.OriginalString, "relationship link should match")
                    Next

                    ctx.Detach(cust)
                Finally
                    Me.ctx = oldContext
                End Try
            End Using
        End Sub
        ' Verify that the materializer throws when the id element is missing or when no self/edit link is specified
        <TestCategory("Partition1")> <TestMethod()> Public Sub MissingSelfEditLinkInQueryCase()
            ' clean the context
            For Each ed As EntityDescriptor In ctx.Entities
                ctx.Detach(ed.Entity)
            Next

            Dim engine = CombinatorialEngine.FromDimensions(New Dimension("UseBatchMode", New Boolean() {True, False}))
            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MissingSelfEditLinkInQueryCase_Inner)
        End Sub

        Private Sub MissingSelfEditLinkInQueryCase_Inner(ByVal values As Hashtable)
            Dim useBatchMode = CBool(values("UseBatchMode"))

            Using PlaybackService.OverridingPlayback.Restore
                Dim Id = web.ServiceRoot.OriginalString & "/id/Customers123"

                'Populate the context with an order
                Dim orderEditLink As String = Nothing, orderQueryLink As String = Nothing, orderId As String = Nothing
                Dim o = GetOrder(ctx.BaseUri, "456", orderEditLink, orderQueryLink, orderId, True, False)

                PlaybackService.OverridingPlayback.Value = GetCustomerPayload(Id, Nothing, Nothing, Nothing, Nothing, Nothing, True, useBatchMode)
                Dim cust = PopulateContextByQuery(ctx.CreateQuery(Of Customer)("/Customers(123)"), useBatchMode)

                Dim entityCount = 2
                Assert.AreEqual(ctx.Entities.Count, entityCount, "number of entities tracked by the context does not match")
                Dim ed = ctx.GetEntityDescriptor(cust)

                Assert.AreEqual(Nothing, ed.EditLink, "edit links should match")
                Assert.AreEqual(Nothing, ed.SelfLink, "self links should match")

                ' Try to do a LoadProperty
                Try
                    ctx.LoadProperty(cust, "BestFriend")
                    Assert.Fail("expected exception, but no exception was thrown in LoadProperty")
                Catch ex As ArgumentNullException
                    Assert.IsTrue(ex.Message.Contains(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("EntityDescriptor_MissingSelfEditLink", ed.Identity)), "Does not contain the right error message")
                    Assert.AreEqual(ctx.Entities.Count, entityCount, "number of entities tracked by the context does not match after LoadProperty")
                End Try

                ' Try to do UpdateObject
                Try
                    ctx.UpdateObject(cust)
                    ctx.SaveChanges()
                    Assert.Fail("expected exception, but no exception was thrown in SaveChanges after UpdateObject")
                Catch ex As ArgumentNullException
                    Assert.IsTrue(ex.Message.Contains(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("EntityDescriptor_MissingSelfEditLink", ed.Identity)), "Does not contain the right error message")
                    Assert.AreEqual(ctx.Entities.Count, entityCount, "number of entities tracked by the context does not match after UpdateObject")
                End Try

                ' Try to do DeleteObject
                Try
                    ctx.DeleteObject(cust)
                    ctx.SaveChanges()
                    Assert.Fail("expected exception, but no exception was thrown in SaveChanges after DeleteObject")
                Catch ex As ArgumentNullException
                    Assert.IsTrue(ex.Message.Contains(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("EntityDescriptor_MissingSelfEditLink", ed.Identity)), "Does not contain the right error message")
                    Assert.AreEqual(ctx.Entities.Count, entityCount, "number of entities tracked by the context does not match after DeleteObject")
                    'Detach the customer and query it once again - we need to do this since there is no good way to bring the object from deleted->unchanged state
                    ctx.Detach(cust)
                    cust = PopulateContextByQuery(ctx.CreateQuery(Of Customer)("/Customers(123)"), useBatchMode)
                End Try

                'Try to do add link
                Try
                    ctx.AddLink(cust, "Orders", o)
                    ctx.SaveChanges()
                    Assert.Fail("expected exception, but no exception was thrown in SaveChanges after AddLink")
                Catch ex As ArgumentNullException
                    Assert.IsTrue(ex.Message.Contains(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("EntityDescriptor_MissingSelfEditLink", ed.Identity)), "Does not contain the right error message")
                    Assert.AreEqual(ctx.Entities.Count, entityCount, "number of entities tracked by the context does not match after AddLink")
                End Try

                'try and detach all the customer
                For Each ed1 In ctx.Entities
                    ctx.Detach(ed1.Entity)
                Next
            End Using
        End Sub

        Private Function GetCustomer(ByVal serviceUri As Uri, ByVal key As String, ByRef editLink As String, ByRef queryLink As String, ByRef id As String, ByRef navigationLink As String, ByRef relationshipLink As String, ByVal populateViaQuery As Boolean, ByVal useBatchMode As Boolean) As Customer
            Using PlaybackService.OverridingPlayback.Restore
                PlaybackService.OverridingPlayback.Value = GetCustomerPayloadWithLinks(serviceUri.OriginalString, id, key, useBatchMode, Not populateViaQuery, queryLink, editLink, navigationLink, relationshipLink)

                If populateViaQuery Then
                    Return PopulateContextByQuery(ctx.CreateQuery(Of Customer)("/Customers(" & key & ")"), useBatchMode)
                Else
                    Return PopulateContextByInsert(Of Customer)("Customers", useBatchMode)
                End If
            End Using
        End Function

        Private Function GetOrder(ByVal serviceUri As Uri, ByVal key As String, ByRef editLink As String, ByRef queryLink As String, ByRef id As String, ByVal populateViaQuery As Boolean, ByVal useBatchMode As Boolean) As Order
            Using PlaybackService.OverridingPlayback.Restore
                queryLink = serviceUri.OriginalString & "/readservice.svc/Orders(" & key & ")"
                editLink = serviceUri.OriginalString & "/writeservice.svc/Orders(" & key & ")"
                id = serviceUri.OriginalString & "/Orders(" & key & ")"

                Dim locationHeader = If((populateViaQuery), Nothing, "Location: " & serviceUri.OriginalString & "/Foo.svc/Orders(" & key & ")" & vbCrLf)
                Dim statusCode = If((populateViaQuery), "HTTP/1.1 200 OK", "HTTP/1.1 201 Created")

                Dim payload =
                    statusCode & vbCrLf &
                    "Content-Type: application/atom+xml" & vbCrLf &
                    "Content-ID: 1" & vbCrLf &
                    locationHeader &
                    vbCrLf &
                    "<entry xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' m:type='AstoriaUnitTests.Stubs.Order' xmlns='http://www.w3.org/2005/Atom'>" &
                    "  <id>" & id & "</id>" &
                    "  <link rel='self' href='" & queryLink & "' />" &
                    "  <link rel='edit' href='" & editLink & "' />" &
                    "  <content type='application/xml'>" &
                    "    <m:properties>" &
                    "      <d:ID>" & key & "</d:ID>" &
                    "      <d:DollarAmount>1000.00</d:DollarAmount>" &
                    "    </m:properties>" &
                    "  </content>" &
                    "</entry>"

                If (useBatchMode) Then
                    payload = ConvertToBatchPayload(payload, populateViaQuery)
                End If

                PlaybackService.OverridingPlayback.Value = payload

                If populateViaQuery Then
                    Return PopulateContextByQuery(ctx.CreateQuery(Of Order)("/Orders(" & key & ")"), useBatchMode)
                Else
                    Return PopulateContextByInsert(Of Order)("Order", useBatchMode)
                End If
            End Using
        End Function

        Private Function PopulateContextByQuery(Of T)(ByVal query As DataServiceQuery(Of T), ByVal useBatchMode As Boolean) As T
            Dim result As T
            If useBatchMode Then
                Dim qor = TryCast(ctx.ExecuteBatch(query).First(), QueryOperationResponse(Of T))
                result = qor.First()
            Else
                result = query.Execute().First()
            End If

            Return result
        End Function

        Private Function PopulateContextByInsert(Of T As New)(ByVal entitySetName As String, ByVal useBatchMode As Boolean) As T
            Dim saveChangesOption = If((useBatchMode), SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.None)
            Dim result = New T()
            ctx.AddObject(entitySetName, result)
            ctx.SaveChanges(saveChangesOption)

            Return result
        End Function

        Public Shared Function ConvertToBatchPayload(ByVal payload As String, ByVal populateViaQuery As Boolean) As String
            Dim contentType = "Content-Type: multipart/mixed; boundary=batch_e9b231d9-72ab-46ea-9613-c7e8f5ece46b"
            Dim changeSetBeginBoundary As String = Nothing
            Dim changeSetEndBoundary As String = Nothing

            If Not populateViaQuery Then
                changeSetBeginBoundary =
                    "Content-Type: multipart/mixed; boundary=changeset_eaab4754-7965-43f0-a7a9-a5556d12787c" & vbCrLf &
                    vbCrLf &
                    "--changeset_eaab4754-7965-43f0-a7a9-a5556d12787c" & vbCrLf

                changeSetEndBoundary = "--changeset_eaab4754-7965-43f0-a7a9-a5556d12787c--" & vbCrLf
            End If
            Return _
                "HTTP/1.1 200 OK" & vbCrLf &
                contentType & vbCrLf &
                vbCrLf &
                "--batch_e9b231d9-72ab-46ea-9613-c7e8f5ece46b" & vbCrLf &
                changeSetBeginBoundary &
                "Content-Type: application/http" & vbCrLf &
                "Content-Transfer-Encoding: binary" & vbCrLf &
                vbCrLf &
                payload & vbCrLf &
                changeSetEndBoundary &
                "--batch_e9b231d9-72ab-46ea-9613-c7e8f5ece46b--"
        End Function

        Private Function GetCustomerPayload(ByVal id As String, ByVal editLink As String, ByVal queryLink As String, ByVal navigationLink As String, ByVal relationshipLink As String, ByVal locationHeader As String, ByVal isQuery As Boolean, ByVal isBatch As Boolean) As String
            Return GetCustomerPayload(id, If(editLink Is Nothing, Nothing, New String() {editLink}), If(queryLink Is Nothing, Nothing, New String() {queryLink}), navigationLink, relationshipLink, locationHeader, isQuery, isBatch, False)
        End Function

        Private Function FormatHRefAttribute(ByVal href As String) As String
            Return (If(href = Nothing, "", " href='" & href & "'"))
        End Function

        Private Function GetCustomerPayload(ByVal id As String, ByVal editLink() As String, ByVal queryLink() As String, ByVal navigationLink As String, ByVal relationshipLink As String, ByVal locationHeader As String, ByVal isQuery As Boolean, ByVal isBatch As Boolean, ByVal generateFeedPayload As Boolean) As String
            Dim statusCode = If((isQuery), "HTTP/1.1 200 OK", "HTTP/1.1 201 Created")
            Dim locationPayload = If((isQuery OrElse (locationHeader = Nothing)), Nothing, "Location: " & locationHeader & vbCrLf)

            Dim queryLinkPayload = If(queryLink Is Nothing, Nothing, String.Join(vbCrLf, (From ql In queryLink Select "  <link rel='self' " & FormatHRefAttribute(ql) & " />").ToArray()))
            Dim editLinkPayload = If(editLink Is Nothing, Nothing, String.Join(vbCrLf, (From el In editLink Select "  <link rel='edit' " & FormatHRefAttribute(el) & " />").ToArray()))
            Dim orderNavigationLinkInPayload = If((navigationLink = Nothing), Nothing, "  <link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=feed' href='" & navigationLink & "Orders' />")
            Dim bestFriendNavigationLinkInPayload = If((navigationLink = Nothing), Nothing, "  <link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' type='application/atom+xml;type=entry' href='" & navigationLink & "BestFriend' />")
            Dim orderRelationshipLinkInPayload = If((relationshipLink = Nothing), Nothing, "  <link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/Orders' type='application/xml' href='" & relationshipLink & "Orders' />")
            Dim bestFriendRelationshipLinkInPayload = If((relationshipLink = Nothing), Nothing, "  <link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/BestFriend' type='application/xml' href='" & relationshipLink & "BestFriend' />")

            Dim feedStart = If((generateFeedPayload = False), Nothing, "<feed xmlns='http://www.w3.org/2005/Atom'>" & vbCrLf)
            Dim feedEnd = If((generateFeedPayload = False), Nothing, "</feed>")
            Dim payload =
                statusCode & vbCrLf &
                "Content-Type: application/atom+xml" & vbCrLf &
                "Content-ID: 1" & vbCrLf &
                locationPayload &
                vbCrLf &
                feedStart &
                "<entry xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' m:type='AstoriaUnitTests.Stubs.Customer' xmlns='http://www.w3.org/2005/Atom'>" & vbCrLf &
                "  <id>" & id & "</id>" & vbCrLf &
                queryLinkPayload & vbCrLf &
                editLinkPayload & vbCrLf &
                bestFriendNavigationLinkInPayload & vbCrLf &
                orderNavigationLinkInPayload & vbCrLf &
                bestFriendRelationshipLinkInPayload & vbCrLf &
                orderRelationshipLinkInPayload & vbCrLf &
                "  <content type='application/xml'>" & vbCrLf &
                "    <m:properties>" & vbCrLf &
                "      <d:ID>123</d:ID>" & vbCrLf &
                "      <d:Name>Foo</d:Name>" & vbCrLf &
                "    </m:properties>" & vbCrLf &
                "  </content>" & vbCrLf &
                "</entry>" &
                feedEnd

            If isBatch Then
                payload = ConvertToBatchPayload(payload, isQuery)
            End If

            Return payload
        End Function

        Private Function GetCustomerPayloadOnlyHeaders(ByVal batchMode As Boolean) As String
            Dim payload =
                "HTTP/1.1 200 OK" & vbCrLf &
                "Content-ID: 1" & vbCrLf &
                vbCrLf

            If batchMode Then
                payload = ConvertToBatchPayload(payload, False)
            End If

            Return payload
        End Function

        Private Function GetCustomerPayloadWithLinks(
                                         ByVal baseUri As String,
                                         ByRef id As String,
                                         ByVal key As String,
                                         ByVal batch As Boolean,
                                         ByVal insertPayload As Boolean,
                                         ByRef queryLink As String,
                                         ByRef editLink As String,
                                         ByRef navigationLink As String,
                                         ByRef relationshipLink As String) As String
            Return GetCustomerPayloadWithLinks(baseUri, id, key, batch, insertPayload, queryLink, editLink, navigationLink, relationshipLink, False)
        End Function

        Private Function GetCustomerPayloadWithLinks(
                                         ByVal baseUri As String,
                                         ByRef id As String,
                                         ByVal key As String,
                                         ByVal batch As Boolean,
                                         ByVal insertPayload As Boolean,
                                         ByRef queryLink As String,
                                         ByRef editLink As String,
                                         ByRef navigationLink As String,
                                         ByRef relationshipLink As String,
                                         ByVal generateFeedResponse As Boolean) As String

            If id Is Nothing Then
                id = baseUri & "/id/Customers(" & key & ")"
            End If

            queryLink = baseUri & "/readservice.svc/Customers(" & key & ")"
            editLink = baseUri & "/writeservice.svc/Customers(" & key & ")"
            navigationLink = baseUri & "/readservice.svc/Customers(" & key & ")/related"
            relationshipLink = baseUri & "/relationships.svc/Customers(" & key & ")/links"
            Dim locationHeader = baseUri & "/Foo.svc/Customers(" & key & ")"
            Return GetCustomerPayload(id, New String() {editLink}, New String() {queryLink}, navigationLink, relationshipLink, locationHeader, Not insertPayload = True, batch, generateFeedResponse)
        End Function

        Private Sub VerifyLinksForEntity(
                ByVal entity As Object,
                ByVal id As String,
                ByVal selfLink As String,
                ByVal editLink As String,
                ByVal navigationLink As String,
                ByVal relationshipLink As String,
                ByVal updatedNavigationLinkProperties As String(),
                ByVal updatedRelationshipLinkProperties As String())
            Dim entityDescriptor As EntityDescriptor = ctx.GetEntityDescriptor(entity)
            Assert.IsNotNull(entityDescriptor, "Entity Descriptor cannot be null")

            Assert.AreEqual(Of String)(id, If(entityDescriptor.Identity = Nothing, Nothing, entityDescriptor.Identity.OriginalString), "Id must be same.")
            Assert.AreEqual(Of String)(selfLink, If(entityDescriptor.SelfLink = Nothing, Nothing, entityDescriptor.SelfLink.OriginalString), "SelfLink must be same.")
            Assert.AreEqual(Of String)(editLink, If(entityDescriptor.EditLink = Nothing, Nothing, entityDescriptor.EditLink.OriginalString), "EditLink must be same.")

            'verify navigation links if present
            If Not updatedNavigationLinkProperties Is Nothing Then
                For Each p As String In updatedNavigationLinkProperties
                    Assert.AreEqual(Of String)(navigationLink & p, GetNavigationLink(entityDescriptor, p).OriginalString, "Navigation links for '{0}' property should match", p)
                Next
            End If

            'verify relationship links if present
            If Not updatedRelationshipLinkProperties Is Nothing Then
                For Each p As String In updatedRelationshipLinkProperties
                    Assert.AreEqual(Of String)(relationshipLink & p, GetRelationshipLink(entityDescriptor, p).OriginalString, "Relationship links for '{0}' property should match", p)
                Next
            End If
        End Sub
    End Class
End Class
