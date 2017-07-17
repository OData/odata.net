'---------------------------------------------------------------------
' <copyright file="medialinkentries.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.IO
Imports System.Linq
Imports System.Xml
Imports System.Xml.Linq
Imports System.Net
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Web

Partial Public Class ClientModule
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    'Remove Atom
    ' <TestClass()>
    <DeploymentItem("Workspaces", "Workspaces")>
    Public Class MediaLinkEntryTest
        Inherits AstoriaTestCase

        Private Shared web As LocalWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing
        Private Shared webStreaming As TestWebRequest = Nothing

        Shared atomns As XNamespace = "http://www.w3.org/2005/Atom"
        Shared appns As XNamespace = "http://www.w3.org/2007/app"
        Shared mns As XNamespace = "http://docs.oasis-open.org/odata/ns/metadata"
        Shared dns As XNamespace = "http://docs.oasis-open.org/odata/ns/data"

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            ' by default in ASP.NET 4.0 maxQueryStringLength is set to 260 characters. We need to increase 
            ' it to make MediaLinkEntryInFeed test pass 
            AstoriaUnitTests.Tests.LocalWebServerHelper.WebConfigHttpRuntimeFragment = "<httpRuntime maxQueryStringLength=""4096""/>"
            web = CType(TestWebRequest.CreateForLocal, LocalWebRequest)
            web.ServiceType = GetType(AstoriaUnitTests.Stubs.RequestInfoService)
            web.SetupLocalWeb()

            webStreaming = TestWebRequest.CreateForInProcessStreamedWcf
            webStreaming.ServiceType = GetType(AstoriaUnitTests.Stubs.RequestInfoService)
            webStreaming.StartService()

        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.ShutdownLocalWeb()
            End If
            If Not webStreaming Is Nothing Then
                webStreaming.StopService()
            End If
        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub ReadMediaLinkEntry()
            ' the current astoria servers don't produce results with MLEs, so
            ' we just take a regular payload and turn it into one with an MLE in it
            Dim ctx As New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim doc As XDocument = XDocument.Load(MediaLinkEntryTest.ConcatUrl(web.BaseUri, "Photos(1)?$format=atom"))
            ConvertToMLE(doc.Root, "Photo", False)
            Dim mlentry = ctx.Execute(Of PhotoEntity)(New Uri("/EchoAtom/$value?s='" & EncodeForUrl(doc.ToString()) & "'", UriKind.Relative)).AsEnumerable().Single()
            Assert.IsTrue(mlentry.ID = 1 And mlentry.Name = "aaa")
            VerifyStreamUris(ctx, mlentry, "Photo")
        End Sub

        <Ignore>
        <TestCategory("Partition2")> <TestMethod()>
        Public Sub MediaLinkEntryInFeed()
            ' the current astoria servers don't produce results with MLEs, so
            ' we just take a regular payload and turn it into one with an MLE in it
            Dim ctx As New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim doc As XDocument = XDocument.Load(MediaLinkEntryTest.ConcatUrl(web.BaseUri, "Photos?$format=atom"))

            For Each e As XElement In doc.Root.Elements(atomns + "entry")
                ConvertToMLE(e, "Photo", False)
            Next
            Dim mlfeed = ctx.Execute(Of PhotoEntity)(New Uri("/EchoAtom/$value?s='" & EncodeForUrl(doc.ToString()) & "'", UriKind.Relative)).ToList()
            Assert.AreEqual(mlfeed.Count, 3)
            Assert.AreEqual(mlfeed(0).ID, 1)
            Assert.AreEqual(mlfeed(0).Name, "aaa")
            VerifyStreamUris(ctx, mlfeed(0), "Photo")
            Assert.AreEqual(mlfeed(1).ID, 2)
            Assert.AreEqual(mlfeed(1).Name, "bbb")
            VerifyStreamUris(ctx, mlfeed(1), "Photo")
            Assert.AreEqual(mlfeed(2).ID, 3)
            Assert.AreEqual(mlfeed(2).Name, "ccc")
            VerifyStreamUris(ctx, mlfeed(2), "Photo")
        End Sub

        <TestCategory("Partition2")> <ExpectedException(GetType(InvalidOperationException))> <TestMethod()>
        Public Sub MalformedMediaLinkEntries()
            Dim ctx As New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim doc As XDocument = XDocument.Load(ConcatUrl(web.BaseUri, "Photos(1)?$format=atom"))
            ConvertToMLE(doc.Root, "Photo", True)

            Dim photo = ctx.Execute(Of PhotoEntity)(New Uri("/EchoAtom/$value?s='" & EncodeForUrl(doc.ToString()) & "'", UriKind.Relative)).AsEnumerable().Single()
        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub LoadMediaEntryLinkProperty()
            ' since astoria servers don't produce this format, this is a mostly hand-crafted scenario
            ' using custom actions to smoke test the code paths.
            Dim WebServiceTypes As TestWebRequest() = New TestWebRequest() {web, webStreaming}

            For Each w As TestWebRequest In WebServiceTypes
                Dim ctx As New DataServiceContext(w.ServiceRoot)
                'ctx.EnableAtom = True
                'ctx.Format.UseAtom()
                Dim doc As XDocument = XDocument.Load(ConcatUrl(w.BaseUri, "Photos(1)?$format=atom"))

                'fix the url so that it points to the property 
                doc.Root.Element(atomns + "id").Value += "/Photo"

                Dim editLink = (From e In doc.Root.Elements(atomns + "link")
                                Where e.Attribute("rel").Value = "edit"
                                Select e).Single()

                editLink.Attribute("href").Value += "/Photo"

                'remove photos to make it look like it's deferred
                Dim photo = doc.Root.Element(atomns + "content").Element(mns + "properties").Element(dns + "Photo")
                Assert.IsNotNull(photo)
                photo.Remove()

                'convert to mle
                ConvertToMLE(doc.Root, "Photo", False)

                For i As Integer = 0 To 1
                    ' new ctx to avoid any state management glitches
                    ctx = New DataServiceContext(w.ServiceRoot)
                    'ctx.EnableAtom = True
                    'ctx.Format.UseAtom()
                    Dim entry = ctx.Execute(Of PhotoEntity)(New Uri("/EchoAtom/$value?s='" & EncodeForUrl(doc.ToString()) & "'", UriKind.Relative)).AsEnumerable.Single()
                    Assert.IsTrue(entry.Name = "aaa")
                    VerifyStreamUris(ctx, entry, "Photo")

                    Assert.IsNull(entry.Photo)
                    Assert.IsNull(entry.PhotoType)

                    If i = 0 Then
                        ctx.LoadProperty(entry, "Photo")
                    Else
                        Dim r As IAsyncResult = ctx.BeginLoadProperty(entry, "Photo", Nothing, Nothing)
                        ctx.EndLoadProperty(r)
                    End If

                    Assert.IsTrue(entry.Photo IsNot Nothing AndAlso
                                  entry.Photo.Length = 3 AndAlso
                                  entry.Photo(0) = 11 AndAlso
                                  entry.Photo(1) = 12 AndAlso
                                  entry.Photo(2) = 13)
                    Assert.AreEqual("image/jpeg", entry.PhotoType)
                Next
            Next
        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub LoadMediaEntryLinkPropertyMetadataErrors()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim threw As Boolean

            ' these queries don't really work, but the client should fail before parsing anyway

            threw = False
            Try
                Dim entry = ctx.Execute(Of PhotoEntityWrongName1)(New Uri("/Photos(1)", UriKind.Relative)).AsEnumerable.Single()
            Catch e As InvalidOperationException
                threw = True
                Assert.AreEqual(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("ClientType_MissingMediaEntryProperty", "AstoriaUnitTests.Stubs.PhotoEntityWrongName1", "PhotoWrong"), e.Message)
            End Try
            Assert.IsTrue(threw, "Executing '/Photos(1)' with type argument 'PhotoEntityWrongName1' did not throw as expected.")

            ' Now we are failing early in this case. If we used to fail during enumerate, we used to throw invalidoperationexception,
            ' but if we fail in Execute, we used to throw DataServiceQueryException
            threw = False
            Try
                Dim entry = ctx.Execute(Of PhotoEntityWrongName2)(New Uri("/Photos(1)", UriKind.Relative)).AsEnumerable.Single()
            Catch e As InvalidOperationException
                threw = True
                Assert.AreEqual(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("ClientType_MissingMimeTypeProperty", "AstoriaUnitTests.Stubs.PhotoEntityWrongName2", "PhotoTypeWrong"), e.InnerException.Message)
            End Try
            Assert.IsTrue(threw, "Executing '/Photos(1)' with type argument 'PhotoEntityWrongName2' did not throw as expected.")

            threw = False
            Try
                Dim entry = ctx.Execute(Of PhotoEntityWrongName3)(New Uri("/Photos(1)", UriKind.Relative)).AsEnumerable.Single()
            Catch e As InvalidOperationException
                threw = True
                Assert.AreEqual(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("ClientType_MissingMimeTypeDataProperty", "AstoriaUnitTests.Stubs.PhotoEntityWrongName3", "PhotoNameWrong"), e.InnerException.Message)
            End Try
            Assert.IsTrue(threw, "Executing '/Photos(1)' with type argument 'PhotoEntityWrongName3' did not throw as expected.")

        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub TestReadStreamUriMergeBehavior()
            For Each merge As MergeOption In [Enum].GetValues(GetType(MergeOption))

                Console.WriteLine("verifying option {0}", merge)
                Dim doc As XDocument = XDocument.Load(MediaLinkEntryTest.ConcatUrl(web.BaseUri, "Photos(1)?$format=atom"))
                Dim doc2 As XDocument = XDocument.Parse(doc.ToString())


                ConvertToMLE(doc.Root, "Photo", False)
                ConvertToMLE(doc2.Root, "Photo", False, "http://mynewuri")
                Dim ctx = New DataServiceContext(web.ServiceRoot)
                'ctx.EnableAtom = True
                'ctx.Format.UseAtom()
                ctx.MergeOption = merge
                Dim mlentry = ctx.Execute(Of PhotoEntity)(New Uri("/EchoAtom/$value?s='" & EncodeForUrl(doc.ToString()) & "'", UriKind.Relative)).AsEnumerable().Single()

                Dim readStreamUri1 As String = Nothing
                If (merge = MergeOption.NoTracking) Then
                    Assert.AreEqual(0, ctx.Entities.Count)
                Else
                    mlentry.Name = "aaaChanged"
                    ctx.UpdateObject(mlentry)
                    Assert.AreEqual(ctx.Entities.Single().State, EntityStates.Modified)
                    readStreamUri1 = ctx.Entities.Single().ReadStreamUri.ToString()
                End If

                Dim mlentry2 = ctx.Execute(Of PhotoEntity)(New Uri("/EchoAtom/$value?s='" & EncodeForUrl(doc2.ToString()) & "'", UriKind.Relative)).AsEnumerable().Single()
                Dim readStreamUri2 As String = Nothing
                If (merge = MergeOption.NoTracking) Then
                    Assert.AreEqual(0, ctx.Entities.Count)
                Else

                    readStreamUri2 = ctx.Entities.Single().ReadStreamUri.ToString()
                End If

                If (merge = MergeOption.NoTracking) Then
                    Assert.AreNotSame(mlentry, mlentry2)
                    Assert.IsNull(readStreamUri1)
                    Assert.IsNull(readStreamUri2)
                Else
                    Assert.AreSame(mlentry, mlentry2)
                    Assert.AreNotEqual(readStreamUri1, readStreamUri2)
                End If


            Next

        End Sub


        <TestCategory("Partition2")> <TestMethod()>
        Public Sub GetReadStreamErrors()
            Dim doc As XDocument = XDocument.Load(MediaLinkEntryTest.ConcatUrl(web.BaseUri, "Photos(1)?$format=atom"))
            ConvertToMLE(doc.Root, "Photo", False)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim mlentry = ctx.Execute(Of PhotoEntity)(New Uri("/EchoAtom/$value?s='" & EncodeForUrl(doc.ToString()) & "'", UriKind.Relative)).AsEnumerable().Single()

            Dim response = ctx.GetReadStream(mlentry)

            Dim ctx2 = New DataServiceContext(web.ServiceRoot)
            ctx2.AttachTo("Photos", mlentry)
            Dim exception = TestUtil.RunCatching(Sub() ctx2.GetReadStream(mlentry))
            Assert.AreEqual(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Context_EntityNotMediaLinkEntry") +
                            Environment.NewLine + "Parameter name: entity", exception.Message)
        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub AsyncGetReadStream()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim doc As XDocument = XDocument.Load(MediaLinkEntryTest.ConcatUrl(web.BaseUri, "Photos(1)?$format=atom"))
            ConvertToMLE(doc.Root, "Photo", False)
            Dim mlentry = ctx.Execute(Of PhotoEntity)(New Uri("/EchoAtom/$value?s='" & EncodeForUrl(doc.ToString()) & "'", UriKind.Relative)).AsEnumerable().Single()

            Dim args = New DataServiceRequestArgs
            Dim asyncResult = ctx.BeginGetReadStream(mlentry, args, Nothing, Nothing)

            Using response = ctx.EndGetReadStream(asyncResult)
                Dim r = New StreamReader(response.Stream)
                Console.WriteLine(r.ReadToEnd)
            End Using
        End Sub

        Private Shared Function ConvertToMLE(ByVal entry As XContainer, ByVal mediaProperty As String, ByVal leaveContent As Boolean, ByVal streamUri As String) As XNode
            Dim content = entry.Element(atomns + "content")
            ' create a new Properties element and move the properties from Content to it
            Dim props = New XElement(mns + "properties", From e In content.Element(mns + "properties").Elements Where e.Name.Namespace = dns)
            entry.Add(props)

            ' remove the properties from the content element and add the src attribute
            If Not leaveContent Then
                content.RemoveNodes()
                content.Add(New XAttribute("src", streamUri))

                ' Having a MLE payload with <content ...></content> causes client to thrown an exception.
                ' Hence setting the value as empty string forces this type of payload.
                content.Value = String.Empty
            End If

            ' add missing edit-media link
            ' use different uri - so that we can verify that we correctly parse both of them
            entry.Add(New XElement(atomns + "link",
                                   New XAttribute("rel", "edit-media"),
                                   New XAttribute("href", streamUri + "?edit=1")))
            Return entry

        End Function

        Private Shared Function ConvertToMLE(ByVal entry As XContainer, ByVal mediaProperty As String, ByVal leaveContent As Boolean) As XNode

            ' build a url for the media element itself
            Dim url = (From e In entry.Elements(atomns + "link")
                       Where e.@rel = "edit"
                       Select e.@href).Single()
            url += "/" & mediaProperty & "/$value"

            Return ConvertToMLE(entry, mediaProperty, leaveContent, url)
        End Function

        Private Shared Function ConcatUrl(ByVal left As String, ByVal right As String) As String
            Dim l As String = CType(IIf(left.EndsWith("/"), left.Substring(0, left.Length - 1), left), String)
            Dim r As String = CType(IIf(right.EndsWith("/"), right.Substring(0, right.Length - 1), right), String)
            Return l & "/" & r
        End Function

        Private Shared Function EncodeForUrl(ByVal s As String) As String
            s = HttpUtility.UrlEncode(s)
            s = s.Replace("'", "%61")
            Return s
        End Function

        Private Shared Sub VerifyStreamUris(ByVal ctx As DataServiceContext, ByVal entity As Object, ByVal mediaProperty As String)
            Dim entityUri As Uri = Nothing
            Assert.IsTrue(ctx.TryGetUri(entity, entityUri))
            Dim contentUri = ConcatUrl(entityUri.ToString(), mediaProperty + "/$value")
            Assert.AreEqual(contentUri, ctx.GetReadStreamUri(entity).ToString())
            Dim entityDescriptor = (From ed In ctx.Entities Where ed.Entity Is entity Select ed).FirstOrDefault()
            Assert.AreEqual(contentUri, entityDescriptor.ReadStreamUri.ToString())
            Assert.AreEqual(contentUri + "?edit=1", entityDescriptor.EditStreamUri.ToString())
        End Sub
    End Class

    <TestClass()> Public Class MediaLinkEntrySave
        Private Shared web As AstoriaUnitTests.Stubs.TestWebRequest = Nothing
        Private ctx As SpacesPhotos.SpacesPhotosService = Nothing

#Region "ClassInitialize, ClassCleanup, TestInitialize, TestCleanup"
        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = AstoriaUnitTests.Stubs.TestWebRequest.CreateForInProcessWcf()
            web.ServiceType = GetType(FakeMediaEntryService)
            web.StartService()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.StopService()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Me.ctx = New SpacesPhotos.SpacesPhotosService(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub
#End Region

        <TestCategory("Partition2")> <TestMethod()> Public Sub NoMimeTypeAttributeGetsDefaultMimeType()
            Const defaultContentType As String = "application/octet-stream"
            Const resourceSet As String = "Folders(230)/Photos"

            ' post a new image
            Dim photo As New SpacesPhotos.PhotoWithNoMimeTypeAttribute
            photo.Title = "NullMimeTypePhoto"
            photo.PhotoBytes = New Byte(10) {}
            photo.PhotoMimeType = "foo/text"
            ctx.AddObject(resourceSet, photo)

            Dim sawMlePost As Boolean = False
            FakeMediaEntryService.InterceptIncommingRequest = Sub(incoming As System.ServiceModel.Web.IncomingWebRequestContext)
                                                                  If incoming.Method = "POST" And incoming.UriTemplateMatch.RequestUri.OriginalString.Contains(resourceSet) Then
                                                                      Assert.AreEqual(incoming.ContentType, defaultContentType, "The default content type was not used")
                                                                      sawMlePost = True
                                                                  End If
                                                              End Sub
            Util.SaveChanges(ctx, Util.ExecutionMethod.Synchronous)
            FakeMediaEntryService.InterceptIncommingRequest = Nothing
            Assert.IsTrue(sawMlePost, "Test failure: we never got a chance to inspect the MLE post for the default content type")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub PostMediaLinkEntry()
            Dim engine = CombinatorialEngine.FromDimensions(New Dimension("ExecutionMethod", Util.ExecutionMethods))
            TestUtil.RunCombinatorialEngineFail(engine, AddressOf PostMediaLinkEntry_Inner)
        End Sub

        Private Sub PostMediaLinkEntry_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)

            ' list existing folders
            For Each existingFolder As SpacesPhotos.Folder In ctx.Folders
            Next

            ' create a new folder
            Dim folder As New SpacesPhotos.Folder
            folder.Name = "PabloTest353753015"
            ctx.AddToFolders(folder)
            Util.SaveChanges(ctx, executionMethod)

            ' post a new image
            Dim photo As New SpacesPhotos.Photo
            photo.Title = "MikePhotoPOSTTest"
            photo.PhotoBytes = New Byte(7571) {}
            photo.PhotoMimeType = "image/jpg"
            ctx.AddObject("Folders(230)/Photos", photo)
            Util.SaveChanges(ctx, executionMethod)

            ' failing a post MR
            Dim photo2 As New SpacesPhotos.Photo
            photo2.Title = "MikePhotoPOSTTest"
            photo2.PhotoBytes = New Byte(7571) {}
            photo2.PhotoMimeType = "image/jpg"
            ctx.AddObject("Folders(240)/Photos", photo2)
            Try
                Util.SaveChanges(ctx, executionMethod)
                Assert.Fail("Expecting exception but recevied none.")
            Catch ex As DataServiceRequestException
                Dim innerEx As DataServiceClientException = CType(ex.InnerException, DataServiceClientException)
                Assert.AreEqual("NotFound", innerEx.Message)
            End Try
            ' try SaveChanges again, expect the exact same failure
            ' there was an old bug that we null ref on the second SaveChanges
            Try
                Util.SaveChanges(ctx, executionMethod)
                Assert.Fail("Expecting exception but recevied none.")
            Catch ex As DataServiceRequestException
                Dim innerEx As DataServiceClientException = CType(ex.InnerException, DataServiceClientException)
                Assert.AreEqual("NotFound", innerEx.Message)
                ctx.Detach(photo2)
            End Try

            ' change folder
            folder.Name = "UpdatedFolder352596751"
            ctx.UpdateObject(folder)
            Util.SaveChanges(ctx, executionMethod)

            ' delete folder
            ctx.DeleteObject(folder)
            Util.SaveChanges(ctx, executionMethod)
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub AddAndDeleteMediaLinkEntry()
            Dim engine = CombinatorialEngine.FromDimensions(New Dimension("ExecutionMethod", Util.ExecutionMethods))
            TestUtil.RunCombinatorialEngineFail(engine, AddressOf AddAndDeleteMediaLinkEntry_Inner)
        End Sub

        Private Sub AddAndDeleteMediaLinkEntry_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)

            Dim folder As New SpacesPhotos.Folder
            folder.Name = "1231231"
            ctx.AddToFolders(folder)
            Util.SaveChanges(ctx, executionMethod)

            ctx.LoadProperty(folder, "Photos")
            ctx.DeleteObject(folder)
            Util.SaveChanges(ctx, executionMethod)
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub ShouldThrowOnBadMimeType()
            Dim engine = CombinatorialEngine.FromDimensions(New Dimension("ExecutionMethod", Util.ExecutionMethods))
            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ShouldThrowOnBadMimeType_Inner)
        End Sub

        Private Sub ShouldThrowOnBadMimeType_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)

            Dim photo As New SpacesPhotos.Photo
            photo.Title = "MikePhotoPOSTTest"
            photo.PhotoBytes = New Byte(7571) {}
            photo.PhotoMimeType = Nothing
            ctx.AddObject("Folder(230)/Photos", photo)
            Dim success As Boolean = False
            Try
                Util.SaveChanges(ctx, executionMethod)
            Catch ex As Exception
                success = True
                Assert.IsTrue(ex.InnerException.Message.Contains("MIME"))
            End Try
            Assert.IsTrue(success)
        End Sub
    End Class
End Class

Namespace SpacesPhotos
    Partial Public Class SpacesPhotosService
        Private Sub OnContextCreated()
            Me.SaveChangesDefaultOptions = SaveChangesOptions.ReplaceOnUpdate
        End Sub
    End Class

    <MediaEntry("PhotoBytes")>
    Public Class PhotoWithNoMimeTypeAttribute
        Public Property ID() As Integer
        Public Property PhotoBytes() As Byte()
        Public Property PhotoMimeType() As String
        Public Property Title() As String
    End Class

    <MediaEntry("PhotoBytes")>
    <MimeTypeProperty("PhotoBytes", "PhotoMimeType")>
    Partial Public Class Photo
    End Class

    <MediaEntry("PhotoBytes")>
    <MimeTypeProperty("PhotoBytes", "PhotoMimeType")>
    Partial Public Class ImageStream
    End Class
End Namespace

<System.ServiceModel.ServiceContract()>
<System.ServiceModel.ServiceBehavior(InstanceContextMode:=ServiceModel.InstanceContextMode.PerCall)>
<System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode:=System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)>
Public Class FakeMediaEntryService
    Private Shared m_NextID As Integer = 0
    Public Shared Property InterceptIncommingRequest As Action(Of System.ServiceModel.Web.IncomingWebRequestContext)
    <System.ServiceModel.OperationContract()>
    <System.ServiceModel.Web.WebInvoke(UriTemplate:="*", Method:="*")>
    Public Function ProcessRequestForMessage(ByVal messageBody As System.IO.Stream) As System.IO.Stream
        Dim c = System.ServiceModel.Web.WebOperationContext.Current
        If FakeMediaEntryService.InterceptIncommingRequest IsNot Nothing Then
            FakeMediaEntryService.InterceptIncommingRequest.Invoke(c.IncomingRequest)
        End If
        Select Case c.IncomingRequest.UriTemplateMatch.RequestUri.PathAndQuery

            Case "/TheTest/Folders"
                If String.Equals("GET", c.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase) Then
                    Dim data = <?xml version="1.0" encoding="utf-8"?><feed xmlns="http://www.w3.org/2005/Atom" xmlns:LP="http://docs.oasis-open.org/odata/ns/metadata" xmlns:LivePhotos="http://dev.live.com/photos" xmlns:Live="LiveAtomBase:"><category scheme="http://dev.live.com/photos/scheme" term="Folders" label="Folders"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders</id><LivePhotos:SpaceUsed>3413382</LivePhotos:SpaceUsed><title>Folders</title><updated>2002-10-02T15:00:00Z</updated><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders" rel="edit" type="application/atom+xml;type=feed" title="Folders"/><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(230)</id><published>2008-06-27T05:19:12.957Z</published><updated>2008-08-29T20:01:42.127Z</updated><title>PabloTest1378166106</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(230)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(230)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>230</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(295)</id><published>2008-08-29T19:58:39.49Z</published><updated>2008-08-29T19:58:39.49Z</updated><title>PabloTest352803187</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(295)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(295)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>295</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(289)</id><published>2008-07-10T00:48:23.107Z</published><updated>2008-07-10T00:48:23.107Z</updated><title>PabloTest9866921</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(289)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(289)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>289</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(288)</id><published>2008-07-10T00:45:38.077Z</published><updated>2008-07-10T00:45:38.077Z</updated><title>PabloTest9696968</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(288)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(288)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>288</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(287)</id><published>2008-07-10T00:44:06.733Z</published><updated>2008-07-10T00:44:06.733Z</updated><title>PabloTest9602484</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(287)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(287)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>287</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(281)</id><published>2008-07-09T08:38:45.537Z</published><updated>2008-07-09T08:38:45.537Z</updated><title>PabloTest45974680</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(281)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(281)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>281</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(279)</id><published>2008-07-09T08:33:37.647Z</published><updated>2008-07-09T08:33:37.647Z</updated><title>PabloTest45664129</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(279)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(279)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>279</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(277)</id><published>2008-07-09T08:28:04.18Z</published><updated>2008-07-09T08:28:04.18Z</updated><title>PabloTest45330271</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(277)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(277)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>277</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(275)</id><published>2008-07-09T08:16:45.443Z</published><updated>2008-07-09T08:16:45.443Z</updated><title>PabloTest44654630</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(275)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(275)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>275</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(273)</id><published>2008-07-09T08:01:15.88Z</published><updated>2008-07-09T08:01:15.88Z</updated><title>PabloTest43715083</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(273)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(273)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>273</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(271)</id><published>2008-07-09T07:58:18.257Z</published><updated>2008-07-09T07:58:18.257Z</updated><title>PabloTest43546306</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(271)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(271)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>271</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(269)</id><published>2008-07-09T07:54:38.32Z</published><updated>2008-07-09T07:54:38.32Z</updated><title>PabloTest43327530</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(269)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(269)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>269</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(267)</id><published>2008-07-09T07:40:40.617Z</published><updated>2008-07-09T07:40:40.617Z</updated><title>PabloTest42486638</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(267)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(267)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>267</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(265)</id><published>2008-07-09T07:34:50.85Z</published><updated>2008-07-09T07:34:50.85Z</updated><title>PabloTest42136603</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(265)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(265)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>265</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(263)</id><published>2008-07-09T07:30:25.773Z</published><updated>2008-07-09T07:30:25.773Z</updated><title>PabloTest41854241</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(263)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(263)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>263</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(262)</id><published>2008-07-09T07:26:34.227Z</published><updated>2008-07-09T07:26:34.227Z</updated><title>PabloTest41643546</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(262)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(262)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>262</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(260)</id><published>2008-07-09T07:23:53.023Z</published><updated>2008-07-09T07:23:53.023Z</updated><title>PabloTest41482459</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(260)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(260)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>260</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(258)</id><published>2008-07-09T07:13:07.693Z</published><updated>2008-07-09T07:13:07.693Z</updated><title>PabloTest40836849</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(258)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(258)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>258</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(256)</id><published>2008-07-09T07:10:02.803Z</published><updated>2008-07-09T07:10:02.803Z</updated><title>PabloTest40650662</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(256)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(256)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>256</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(252)</id><published>2008-07-07T19:23:35.507Z</published><updated>2008-07-07T19:23:43.897Z</updated><title>UpdatedFolder-2002124475</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(252)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(252)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>252</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(250)</id><published>2008-07-07T19:12:23.927Z</published><updated>2008-07-07T19:12:23.927Z</updated><title>PabloTest-2002818180</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(250)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(250)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>250</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(249)</id><published>2008-06-27T07:35:32.02Z</published><updated>2008-06-27T07:35:32.02Z</updated><title>PabloTest587883078</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(249)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(249)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>249</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(243)</id><published>2008-06-27T05:47:06.83Z</published><updated>2008-06-27T05:47:06.83Z</updated><title>PabloTest1379832290</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(243)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(243)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>243</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(241)</id><published>2008-06-27T05:44:58.97Z</published><updated>2008-06-27T05:44:58.97Z</updated><title>PabloTest1379710220</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(241)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(241)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>241</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(239)</id><published>2008-06-27T05:36:58.44Z</published><updated>2008-06-27T05:36:58.44Z</updated><title>PabloTest1379228957</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(239)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(239)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>239</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(194)</id><published>2008-02-27T00:13:56.537Z</published><updated>2008-02-27T00:56:30.55Z</updated><title>Green Design Meeting</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(194)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(194)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>194</LivePhotos:ID></LP:properties></content></entry><entry LP:type="Folder"><category scheme="http://dev.live.com/photos/scheme" term="Folder" label="Folder"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(186)</id><published>2008-02-27T00:12:26.32Z</published><updated>2008-02-27T00:13:53.193Z</updated><title>Astoria Design Meeting</title><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(186)" rel="edit" type="application/atom+xml;type=entry" title="Folder"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(186)/Photos" rel="related" type="application/atom+xml;type=feed" title="Photos"/><content type="application/xml"><LP:properties><LivePhotos:ID>186</LivePhotos:ID></LP:properties></content></entry></feed>
                    Dim bytes = System.Text.Encoding.UTF8.GetBytes(data.ToString())

                    c.OutgoingResponse.Headers.Item("Content-Type") = "application/atom+xml;charset=utf-8;"
                    c.OutgoingResponse.Headers.Item("Content-Length") = bytes.Length.ToString()
                    c.OutgoingResponse.StatusCode = HttpStatusCode.OK
                    Return New System.IO.MemoryStream(bytes)

                ElseIf String.Equals("POST", c.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase) Then
                    c.OutgoingResponse.StatusCode = HttpStatusCode.Created
                    c.OutgoingResponse.Headers.Item("Location") = New Uri(c.IncomingRequest.UriTemplateMatch.BaseUri, "/TheTest/Folders(311)/" + m_NextID.ToString()).ToString()
                    c.OutgoingResponse.Headers.Item("OData-EntityId") = New Uri(c.IncomingRequest.UriTemplateMatch.BaseUri, "/TheTest/Folders(311)/" + m_NextID.ToString()).ToString
                    c.OutgoingResponse.Headers.Item("Content-Length") = "0"
                    m_NextID = m_NextID + 1
                End If

            Case "/TheTest/Folders(311)/Photos()"
                If String.Equals("GET", c.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase) Then
                    Dim data = <?xml version="1.0" encoding="utf-8"?><feed xmlns="http://www.w3.org/2005/Atom" xmlns:LP="http://docs.oasis-open.org/odata/ns/metadata" xmlns:LivePhotos="http://dev.live.com/photos" xmlns:Live="LiveAtomBase:"><category scheme="http://dev.live.com/photos/scheme" term="Photos" label="Photos"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(311)/Photos</id><title>PabloTest588540619</title><updated>2008-09-10T01:53:26.023Z</updated><author><name>Default Name</name></author><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(311)/Photos" rel="edit" type="application/atom+xml;type=feed" title="Photos"/><entry LP:type="Photo"><category scheme="http://dev.live.com/photos/scheme" term="Photo" label="Photo"/><id>https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(311)/Photos(337)</id><published>2008-09-10T01:53:24.917Z</published><updated>2008-09-10T01:53:26.023Z</updated><title>90d23c26-414e-4b22-85ff-6d14e6e0d54d</title><author><name>Default Name</name></author><summary type="html"><![CDATA[ <img src="http://dc2files.storage.live-int.com/y1pfwf-tHbIs4-tqfWjFJODiaBeHKL3Z1vToIRrK5YB3HZ0fJ-UDrNpXn5eQzTl1i0A" border="0" alt="90d23c26-414e-4b22-85ff-6d14e6e0d54d" />]]></summary><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(311)/Photos(337)" rel="edit" type="application/atom+xml;type=entry" title="Photo"/><content type="image/jpeg" src="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(311)/Photos(337)/$value" length="2013"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(311)/Photos(337)/$value" rel="edit-media" type="image/jpeg" title="ImageStream"/><link href="https://01hw125618/pabloc@hotmail-int.com/AtomSpacesPhotos/Folders(311)/Photos(337)/ImageStreams" rel="related" type="application/atom+xml;type=feed" title="ImageStreams"/><LP:properties><LivePhotos:ID>337</LivePhotos:ID><LivePhotos:Version>1</LivePhotos:Version></LP:properties></entry></feed>
                    Dim bytes = System.Text.Encoding.UTF8.GetBytes(data.ToString())

                    c.OutgoingResponse.Headers.Item("Content-Type") = "application/atom+xml;charset=utf-8;"
                    c.OutgoingResponse.Headers.Item("Content-Length") = bytes.Length.ToString()
                    c.OutgoingResponse.StatusCode = HttpStatusCode.OK

                    Return New System.IO.MemoryStream(bytes)

                End If

            Case "/TheTest/Folders(311)"
                If String.Equals("PUT", c.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase) Then
                    c.OutgoingResponse.StatusCode = HttpStatusCode.NoContent
                    c.OutgoingResponse.Headers.Item("Content-Length") = "0"

                ElseIf String.Equals("DELETE", c.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase) Then
                    c.OutgoingResponse.StatusCode = HttpStatusCode.NoContent
                    c.OutgoingResponse.Headers.Item("Content-Length") = "0"
                End If

            Case "/TheTest/Folders(230)/Photos"
                If String.Equals("POST", c.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase) Then
                    c.OutgoingResponse.StatusCode = HttpStatusCode.Created
                    c.OutgoingResponse.Headers.Item("Location") = New Uri(c.IncomingRequest.UriTemplateMatch.BaseUri, "TheTest/Folders(230)/Photos(312)" + m_NextID.ToString()).ToString()
                    c.OutgoingResponse.Headers.Item("OData-EntityId") = New Uri(c.IncomingRequest.UriTemplateMatch.BaseUri, "TheTest/Folders(230)/Photos(312)" + m_NextID.ToString()).ToString()
                    c.OutgoingResponse.Headers.Item("Content-Length") = "0"

                    m_NextID = m_NextID + 1
                End If

            Case "/TheTest/Folders(240)/Photos"
                If String.Equals("POST", c.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase) Then
                    c.OutgoingResponse.SetStatusAsNotFound()
                End If

            Case "/TheTest/Folders(230)/Photos(312)"
                If String.Equals("PUT", c.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase) Then
                    c.OutgoingResponse.StatusCode = HttpStatusCode.NoContent
                    c.OutgoingResponse.Headers.Item("Content-Length") = "0"
                End If
        End Select

        Return Nothing
    End Function
End Class
