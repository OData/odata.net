'---------------------------------------------------------------------
' <copyright file="MediaStreaming.vb" company="Microsoft">
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
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Linq
Imports System.Net
Imports System.Threading
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Web
Imports tp = AstoriaUnitTests.Stubs.DataServiceProvider
Imports p = Microsoft.OData.Service.Providers
Imports AstoriaUnitTests.Tests

Partial Public Class ClientModule
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    'Remove Atom
    ' <TestClass()>
    Public Class MediaStreaming
        Inherits AstoriaTestCase

        Private Shared web As TestWebRequest = Nothing
        Private Shared webContent As TestWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing

#Region "Class setup"
        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.ServiceType = GetType(AstoriaUnitTests.Stubs.StreamingService)
            web.StartService()

            webContent = TestWebRequest.CreateForInProcessWcf
            webContent.ServiceType = GetType(AstoriaUnitTests.Stubs.StreamingContentService)
            webContent.StartService()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            AddHandler ctx.SendingRequest2, AddressOf EntitySetResolverTests.SetAcceptHeader
            ctx.Execute(Of Boolean)(New Uri("/SetContentServiceUri/$value?uri='" + HttpUtility.UrlEncode(webContent.ServiceRoot.ToString() + "'"),
                                            UriKind.Relative))
            RemoveHandler ctx.SendingRequest2, AddressOf EntitySetResolverTests.SetAcceptHeader
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not webContent Is Nothing Then
                webContent.StopService()
            End If
            If Not web Is Nothing Then
                web.StopService()
            End If
        End Sub
#End Region

#Region "API"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub GetReadStreamUriApi()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Dim entityWithoutStream = ctx.Execute(Of StreamingServiceEntityWithoutStream)(New Uri("/EntitiesWithoutStream(1)", UriKind.Relative)).SingleOrDefault()
            Dim photoNotTracked = New StreamingServicePhoto()

            ' Success case with some result
            Assert.AreEqual(ctx.BaseUri.ToString() + "/Photos(1)/$value", ctx.GetReadStreamUri(photo).ToString())
            ' Un-successfull case without any result (non MLE entity)
            Assert.IsNull(ctx.GetReadStreamUri(entityWithoutStream))

            ' Failure case NULL
            Try
                ctx.GetReadStreamUri(Nothing)
                Assert.Fail("No exception thrown.")
            Catch ex As ArgumentNullException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ' Failure case with entity which is not being tracked by the context
            Try
                ctx.GetReadStreamUri(photoNotTracked)
                Assert.Fail("No exception thrown.")
            Catch ex As InvalidOperationException
            End Try

            ' Failure case with entity coming from context but not being tracked due to MergeOptions.NoTracking
            ctx.MergeOption = MergeOption.NoTracking
            Dim photoNoTracking = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(2)", UriKind.Relative)).SingleOrDefault()
            Try
                ctx.GetReadStreamUri(photoNoTracking)
                Assert.Fail("No exception thrown.")
            Catch ex As InvalidOperationException
            End Try

            ' Un-successfull case with entity which is being added (so no MLE/MR is available for it yet)
            ctx.MergeOption = MergeOption.PreserveChanges
            Dim photoNew = New StreamingServicePhoto()
            ctx.AddObject("Photos", photoNew)
            Assert.IsNull(ctx.GetReadStreamUri(photoNew))
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub BeginGetReadStreamApi()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Dim entityWithoutStream = ctx.Execute(Of StreamingServiceEntityWithoutStream)(New Uri("/EntitiesWithoutStream(1)", UriKind.Relative)).SingleOrDefault()
            Dim photoNotTracked = New StreamingServicePhoto()

            ' Success case
            Dim args = New DataServiceRequestArgs
            Dim asyncResult = ctx.BeginGetReadStream(photo, args, Nothing, Nothing)
            Assert.IsNotNull(asyncResult)
            ctx.EndGetReadStream(asyncResult)

            ' Failure case with entity which doesn't have MLE
            Try
                ctx.BeginGetReadStream(entityWithoutStream, args, Nothing, Nothing)
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ' Failure case with NULL for entity
            Try
                ctx.BeginGetReadStream(Nothing, args, Nothing, Nothing)
            Catch ex As ArgumentNullException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ' Failure case with NULL for args
            Try
                ctx.BeginGetReadStream(photo, Nothing, Nothing, Nothing)
            Catch ex As ArgumentNullException
                Assert.AreEqual("args", ex.ParamName)
            End Try

            ' Failure case with entity which is not being tracked by the context
            Try
                ctx.BeginGetReadStream(photoNotTracked, args, Nothing, Nothing)
                Assert.Fail("No exception thrown.")
            Catch ex As InvalidOperationException
            End Try

            ' Failure case with entity coming from context but not being tracked due to MergeOptions.NoTracking
            ctx.MergeOption = MergeOption.NoTracking
            Dim photoNoTracking = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(2)", UriKind.Relative)).SingleOrDefault()
            Try
                ctx.BeginGetReadStream(photoNoTracking, args, Nothing, Nothing)
                Assert.Fail("No exception thrown.")
            Catch ex As InvalidOperationException
            End Try

            ' Failure case with entity which is being added (so no MLE/MR is available for it yet)
            ctx.MergeOption = MergeOption.PreserveChanges
            Dim photoNew = New StreamingServicePhoto()
            ctx.AddObject("Photos", photoNew)
            Try
                ctx.BeginGetReadStream(photoNew, args, Nothing, Nothing)
                Assert.Fail("No exception thrown.")
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub EndGetReadStreamApi()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()

            ' Success case
            Dim args = New DataServiceRequestArgs
            Dim asyncResult = ctx.BeginGetReadStream(photo, args, Nothing, Nothing)
            Assert.IsNotNull(asyncResult)
            Dim response = ctx.EndGetReadStream(asyncResult)
            Assert.IsNotNull(response)

            ' Failure - calling end for the second time
            Try
                ctx.EndGetReadStream(asyncResult)
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName)
            End Try

            ' Failure - null
            Try
                ctx.EndGetReadStream(Nothing)
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName)
            End Try

            ' Failure - foreign async result
            Dim asyncResultFromExecute = ctx.BeginExecute(Of StreamingServicePhoto)(New Uri("/Photos(2)", UriKind.Relative), Nothing, Nothing)
            Try
                ctx.EndGetReadStream(asyncResultFromExecute)
            Catch ex As ArgumentException
                Assert.AreEqual("asyncResult", ex.ParamName)
            End Try
            ctx.CancelRequest(asyncResultFromExecute)
        End Sub

        Private Sub GetReadStreamApiCommon(ByVal ctx As DataServiceContext, ByVal getReadStreamCall As Func(Of DataServiceContext, Object, DataServiceStreamResponse))
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Dim entityWithoutStream = ctx.Execute(Of StreamingServiceEntityWithoutStream)(New Uri("/EntitiesWithoutStream(1)", UriKind.Relative)).SingleOrDefault()
            Dim photoNotTracked = New StreamingServicePhoto()
            Dim v1photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()

            ' Success case
            Dim args = New DataServiceRequestArgs
            Dim response = getReadStreamCall(ctx, photo)
            Assert.IsNotNull(response)

            ' Success case - V1
            response = getReadStreamCall(ctx, v1photo)
            Assert.IsNotNull(response)

            ' Failure case with entity which doesn't have MLE
            Try
                getReadStreamCall(ctx, entityWithoutStream)
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ' Failure case with NULL for entity
            Try
                getReadStreamCall(ctx, Nothing)
            Catch ex As ArgumentNullException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ' Failure case with entity which is not being tracked by the context
            Try
                getReadStreamCall(ctx, photoNotTracked)
                Assert.Fail("No exception thrown.")
            Catch ex As InvalidOperationException
            End Try

            ' Failure case with entity coming from context but not being tracked due to MergeOptions.NoTracking
            ctx.MergeOption = MergeOption.NoTracking
            Dim photoNoTracking = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(2)", UriKind.Relative)).SingleOrDefault()
            Try
                getReadStreamCall(ctx, photoNoTracking)
                Assert.Fail("No exception thrown.")
            Catch ex As InvalidOperationException
            End Try

            ' Failure case with entity which is being added (so no MLE/MR is available for it yet)
            ctx.MergeOption = MergeOption.PreserveChanges
            Dim photoNew = New StreamingServicePhoto()
            ctx.AddObject("Photos", photoNew)
            Try
                getReadStreamCall(ctx, photoNew)
                Assert.Fail("No exception thrown.")
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub GetReadStream_Entity_Api()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            GetReadStreamApiCommon(ctx,
                Function(context As DataServiceContext, entity As Object) context.GetReadStream(entity))
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub GetReadStream_Entity_ContentType_Api()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()

            GetReadStreamApiCommon(ctx,
                Function(context As DataServiceContext, entity As Object) context.GetReadStream(entity, "*/*"))

            ' Failure case with NULL for contentType
            Try
                ctx.GetReadStream(photo, CType(Nothing, String))
            Catch ex As ArgumentNullException
                Assert.AreEqual("acceptContentType", ex.ParamName)
            End Try

            ' Failure case with empty contentType
            Try
                ctx.GetReadStream(photo, "")
            Catch ex As ArgumentException
                Assert.AreEqual("acceptContentType", ex.ParamName)
            End Try
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub GetReadStream_Entity_Args_Api()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()

            Dim args = New DataServiceRequestArgs()
            GetReadStreamApiCommon(ctx,
                Function(context As DataServiceContext, entity As Object) context.GetReadStream(entity, args))

            ' Failure case with NULL for args
            Try
                ctx.GetReadStream(photo, CType(Nothing, DataServiceRequestArgs))
            Catch ex As ArgumentNullException
                Assert.AreEqual("args", ex.ParamName)
            End Try
        End Sub

        Private Sub SetSaveStreamApiCommon(ByVal ctx As DataServiceContext, ByVal setSaveStreamCall As Action(Of DataServiceContext, Object, Stream))
            Dim newphoto = New StreamingServicePhoto()
            ctx.AddObject("Photos", newphoto)
            Dim newv1photo = New StreamingServiceV1Photo()
            ctx.AddObject("V1Photos", newv1photo)
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Dim v1photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            Dim entityWithoutStream = ctx.Execute(Of StreamingServiceEntityWithoutStream)(New Uri("/EntitiesWithoutStream(1)", UriKind.Relative)).SingleOrDefault()
            Dim nontrackedphoto = New StreamingServicePhoto()

            Dim content = New MemoryStream(New Byte() {1, 2, 3})

            ' Success case - new V2 entity
            setSaveStreamCall(ctx, newphoto, content)

            ' Success case - existing V2 entity
            setSaveStreamCall(ctx, photo, content)

            ' Success case - with non-MLE entity - client does allow this, up to the user to get this right
            setSaveStreamCall(ctx, entityWithoutStream, content)

            ' Failure case - new V1 entity - can't set save stream on MediaEntry attribute entity
            Try
                setSaveStreamCall(ctx, newv1photo, content)
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ' Failure case - existing V1 entity - can't set save stream on MediaEntry attribute entity
            Try
                setSaveStreamCall(ctx, v1photo, content)
            Catch ex As ArgumentException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ' Failure case - NULL entity
            Try
                setSaveStreamCall(ctx, Nothing, content)
            Catch ex As ArgumentNullException
                Assert.AreEqual("entity", ex.ParamName)
            End Try

            ' Failure case - non-tracked entity
            Try
                setSaveStreamCall(ctx, nontrackedphoto, content)
            Catch ex As InvalidOperationException
            End Try

            ' Failure case - NULL stream
            Try
                setSaveStreamCall(ctx, newphoto, Nothing)
            Catch ex As ArgumentNullException
                Assert.AreEqual("stream", ex.ParamName)
            End Try
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub SetSaveStream_Entity_Stream_ContentType_Slug_Api()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()

            SetSaveStreamApiCommon(ctx, AddressOf SetSaveStream_Entity_Stream_ContentType_Slug_Callback)

            ' Failure case - NULL contentType
            Try
                ctx.SetSaveStream(photo, New MemoryStream(), False, Nothing, "MySlug")
            Catch ex As ArgumentNullException
                Assert.AreEqual("contentType", ex.ParamName)
            End Try

            ' Failure case - empty contentType
            Try
                ctx.SetSaveStream(photo, New MemoryStream(), False, "", "MySlug")
            Catch ex As ArgumentException
                Assert.AreEqual("contentType", ex.ParamName)
            End Try

            ' Failure case - NULL slug
            Try
                ctx.SetSaveStream(photo, New MemoryStream(), False, "MyContentType", Nothing)
            Catch ex As ArgumentNullException
                Assert.AreEqual("slug", ex.ParamName)
            End Try
        End Sub

        Private Sub SetSaveStream_Entity_Stream_ContentType_Slug_Callback(ByVal ctx As DataServiceContext, ByVal entity As Object, ByVal stream As Stream)
            ctx.SetSaveStream(entity, stream, False, "MyContentType", "MySlug")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub SetSaveStream_Entity_Stream_Args_Api()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()

            SetSaveStreamApiCommon(ctx, AddressOf SetSaveStream_Entity_Stream_Args_Callback)

            ' Failure case - NULL args
            Try
                ctx.SetSaveStream(photo, New MemoryStream(), False, Nothing)
            Catch ex As ArgumentNullException
                Assert.AreEqual("args", ex.ParamName)
            End Try
        End Sub

        Private Sub SetSaveStream_Entity_Stream_Args_Callback(ByVal ctx As DataServiceContext, ByVal entity As Object, ByVal stream As Stream)
            Dim args = New DataServiceRequestArgs
            ctx.SetSaveStream(entity, stream, False, args)
        End Sub
#End Region

#Region "Read MR"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadSingleMR()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("UseV1Entity", New Boolean() {False, True}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ReadSingleMR_Inner)
        End Sub

        Private Sub ReadSingleMR_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim useV1Entity = CType(values("UseV1Entity"), Boolean)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo As StreamingServicePhotoBase
            If useV1Entity Then
                photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            Else
                photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            End If
            Assert.IsNotNull(photo, "There must be Photo with ID=1.")
            VerifySinglePhoto(photo, ctx, executionMethod)
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadMultipleMRs()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("UseV1Entity", New Boolean() {False, True}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ReadMultipleMRs_Inner)
        End Sub

        Private Sub ReadMultipleMRs_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim useV1Entity = CType(values("UseV1Entity"), Boolean)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim query As IEnumerable(Of StreamingServicePhotoBase)
            If useV1Entity Then
                query = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos?$filter=AlternativeUri eq false and ID lt 100", UriKind.Relative)).Cast(Of StreamingServicePhotoBase)()
            Else
                query = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos?$filter=AlternativeUri eq false and ID lt 100", UriKind.Relative)).Cast(Of StreamingServicePhotoBase)()
            End If
            For Each photo In query
                VerifySinglePhoto(photo, ctx, executionMethod)
            Next
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadMultipleMRsFromAlternativeUri()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("UseV1Entity", New Boolean() {False, True}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ReadMultipleMRsFromAlternativeUri_Inner)
        End Sub

        Private Sub ReadMultipleMRsFromAlternativeUri_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim useV1Entity = CType(values("UseV1Entity"), Boolean)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim query As IEnumerable(Of StreamingServicePhotoBase)
            If useV1Entity Then
                query = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos?$filter=AlternativeUri eq true and ID lt 100", UriKind.Relative)).Cast(Of StreamingServicePhotoBase)()
            Else
                query = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos?$filter=AlternativeUri eq true and ID lt 100", UriKind.Relative)).Cast(Of StreamingServicePhotoBase)()
            End If
            For Each photo In query
                VerifySinglePhoto(photo, ctx, executionMethod)
            Next
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadFailingMR()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ReadFailingMR_Inner)
        End Sub

        Private Sub ReadFailingMR_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).FirstOrDefault()

            Dim args = New DataServiceRequestArgs()
            args.Headers("Test_FailThisRequest") = "true"
            Try
                Util.GetReadStream(ctx, photo, args, executionMethod)
                Assert.Fail("Should have thrown exception.")
            Catch ex As InvalidOperationException
                If Not ex.GetType() Is GetType(DataServiceClientException) Then
                    ex = CType(ex.InnerException, InvalidOperationException)
                End If
                Assert.IsTrue(ex.Message.Contains("Request failed for testing purposes."), "Wrong exception message.")
                Assert.AreEqual(500, CType(ex, DataServiceClientException).StatusCode, "Wrong status code.")
            End Try
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadMRWithWrongContentType()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ReadMRWithWrongContentType_Inner)
        End Sub

        Private Sub ReadMRWithWrongContentType_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).FirstOrDefault()

            Dim args = New DataServiceRequestArgs()
            args.AcceptContentType = "type/wrong"
            Try
                Util.GetReadStream(ctx, photo, args, executionMethod)
                Assert.Fail("Should have thrown exception.")
            Catch ex As InvalidOperationException
                If Not ex.GetType() Is GetType(DataServiceClientException) Then
                    ex = CType(ex.InnerException, InvalidOperationException)
                End If
                Assert.AreEqual(415, CType(ex, DataServiceClientException).StatusCode, "Wrong status code.")
            End Try
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadMRWithContentDisposition()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ReadMRWithContentDisposition_Inner)
        End Sub

        Private Sub ReadMRWithContentDisposition_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos?$filter=AlternativeUri eq true", UriKind.Relative)).FirstOrDefault()

            Dim args = New DataServiceRequestArgs()
            Dim response = Util.GetReadStream(ctx, photo, args, executionMethod)
            Assert.IsNull(response.ContentDisposition, "Content-Disposition was not present, so it should be null")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadMRVerifyEvents()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ReadMRVerifyEvents_Inner)
        End Sub

        Private Sub ReadMRVerifyEvents_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).FirstOrDefault()

            ResetRequestHeaders()
            AddHandler ctx.SendingRequest2, AddressOf ReadMRVerifyEvent_SendingRequestHandler
            Dim args = New DataServiceRequestArgs()
            Dim response = Util.GetReadStream(ctx, photo, args, executionMethod)
            Assert.AreEqual("true", GetRequestHeaders()("/Photos(1)/$value")("SendingRequest2Called"), "SendingRequest2 was not called.")
        End Sub

        Private Shared Sub ReadMRVerifyEvent_SendingRequestHandler(ByVal sender As Object, ByVal e As SendingRequest2EventArgs)
            e.RequestMessage.SetHeader("SendingRequest2Called", "true")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadSingleMRAbort()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo As StreamingServicePhoto
            photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Assert.IsNotNull(photo, "There must be Photo with ID=1.")
            Dim asyncResult = ctx.BeginGetReadStream(photo, New DataServiceRequestArgs(), Nothing, Nothing)
            ctx.CancelRequest(asyncResult)
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ReadSingleMRError()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ReadSingleMRError_Inner)
        End Sub

        Private Sub ReadSingleMRError_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo As StreamingServicePhotoBase
            photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Assert.IsNotNull(photo, "There must be Photo with ID=1.")

            Dim entityDescriptor = (From ed In ctx.Entities Where ed.Entity Is photo Select ed).SingleOrDefault()
            Assert.IsNotNull(entityDescriptor, "The Photo entity must be tracked.")
            Assert.IsNotNull(entityDescriptor.ReadStreamUri, "Photo is MLE so it must have a read stream URI in entity descriptor.")
            Assert.IsNotNull(entityDescriptor.EditStreamUri, "Photo is MLE so it must heav an edit stream URI.")

            Using TestUtil.RestoreStaticMembersOnDispose(GetType(AstoriaUnitTests.Stubs.StreamingService))
                AstoriaUnitTests.Stubs.StreamingService.GetReadStreamOverride = AddressOf GetReadStreamError
                Try
                    Util.GetReadStream(ctx, photo, New DataServiceRequestArgs(), executionMethod)
                    Assert.Fail("Shouldn't be here")
                Catch ex As DataServiceClientException
                    Assert.AreEqual(456, ex.StatusCode)
                End Try
            End Using

        End Sub

        Private Function GetReadStreamError(ByVal entity As Object, ByVal etag As String, ByVal checkETagForEquality As Boolean?, ByVal operationContext As Microsoft.OData.Service.DataServiceOperationContext) As System.IO.Stream
            Throw New DataServiceException(456, "GetReadStreamError")
        End Function

#End Region

#Region "Request headers"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub AcceptHeader()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("API", New String() {"Args", "Param"}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf AcceptHeader_Inner)
        End Sub

        Private Sub AcceptHeader_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim api = CType(values("API"), String)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos?$filter=AlternativeUri eq true", UriKind.Relative)).First()

            Dim response As DataServiceStreamResponse = Nothing
            Try
                Select Case api
                    Case "Args"
                        Dim args = New DataServiceRequestArgs()
                        args.AcceptContentType = "test/ReplyWithHeaders"
                        response = Util.GetReadStream(ctx, photo, args, executionMethod)

                    Case "Param"
                        If executionMethod = Util.ExecutionMethod.Synchronous Then
                            response = ctx.GetReadStream(photo, "test/ReplyWithHeaders")
                        Else
                            Return
                        End If

                    Case Else
                        Throw New AssertFailedException()
                End Select

                Dim headers = ParseHeadersResponse(response.Stream)
                Assert.AreEqual("test/ReplyWithHeaders", headers(HttpRequestHeader.Accept))
            Finally
                If Not response Is Nothing Then
                    response.Dispose()
                End If
            End Try
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub RequestHeaders()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf RequestHeaders_Inner)
        End Sub

        Private Sub RequestHeaders_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos?$filter=AlternativeUri eq true", UriKind.Relative)).First()

            Dim args = New DataServiceRequestArgs()
            args.AcceptContentType = "test/ReplyWithHeaders"
            args.Headers.Add("test-single", "value1")
            ' For now we don't support collection headers
            args.Headers.Add("test-multi", "value2,value3")

            AddHandler ctx.SendingRequest2, Sub(sender, eventArgs)
                                                Assert.AreEqual(eventArgs.RequestMessage.GetHeader("Accept"), "test/ReplyWithHeaders", "the accept header must be set to the correct value")
                                                Assert.AreEqual(eventArgs.RequestMessage.GetHeader("test-single"), "value1", "the test-single header must be set to the correct value")
                                                Assert.AreEqual(eventArgs.RequestMessage.GetHeader("test-multi"), "value2,value3", "the test-multi header must be set to the correct value")

                                                ' changing these headers will change their values
                                                eventArgs.RequestMessage.SetHeader("test-single", "test-single random value")
                                                eventArgs.RequestMessage.SetHeader("test-multi", "test-multi random value")
                                            End Sub

            Using response = Util.GetReadStream(ctx, photo, args, executionMethod)
                Dim headers = ParseHeadersResponse(response.Stream)
                Assert.AreEqual("test-single random value", headers("test-single"))
                ' For now we don't support collection headers
                Assert.AreEqual("test-multi random value", headers("test-multi"))
            End Using
        End Sub
#End Region

#Region "Response headers"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ResponseHeaders()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ResponseHeaders_Inner)
        End Sub

        Private Sub ResponseHeaders_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos?$filter=AlternativeUri eq true", UriKind.Relative)).First()

            Dim args = New DataServiceRequestArgs()
            args.AcceptContentType = "test/AddTestHeaders"

            Using response = Util.GetReadStream(ctx, photo, args, executionMethod)
                Assert.AreEqual("value1", response.Headers("test-single"))
                Assert.AreEqual("value2,value3", response.Headers("test-multi"))
            End Using
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ResponseContentProperties()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf ResponseContentProperties_Inner)
        End Sub

        Private Sub ResponseContentProperties_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos?$filter=AlternativeUri eq true", UriKind.Relative)).First()

            Dim args = New DataServiceRequestArgs()
            args.AcceptContentType = "test/AddContentPropertyHeaders"

            Using response = Util.GetReadStream(ctx, photo, args, executionMethod)
                Assert.AreEqual("test/ContentType", response.ContentType)
                Assert.AreEqual("test-content-disposition", response.ContentDisposition)
            End Using
        End Sub
#End Region

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ResponseIsReadOnly()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).First()

            Dim response = Util.GetReadStream(ctx, photo, New DataServiceRequestArgs(), Util.ExecutionMethod.Synchronous)
            Dim stream = response.Stream

            Assert.IsTrue(stream.CanRead)
            ' the stream might be able to seek or not, we don't care
            Assert.IsFalse(stream.CanWrite)

            Try
                stream.WriteByte(0)
                Assert.Fail()
            Catch ex As Exception
            End Try

            Dim b = New Byte(10) {}
            Try
                stream.Write(b, 0, 10)
                Assert.Fail()
            Catch ex As Exception
            End Try

            Try
                stream.Flush()
                Assert.Fail()
            Catch ex As Exception
            End Try

            Try
                stream.BeginWrite(b, 0, 10, Nothing, Nothing)
                Assert.Fail()
            Catch ex As Exception
            End Try
        End Sub

#Region "Add MR"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub AddSingleMR()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("MergeOption", New MergeOption() {MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges}),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo), Nothing}),
                New Dimension("Content", New Byte()() {New Byte() {1, 2, 3}, New Byte() {}}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf AddSingleMR_Inner)
        End Sub

        Private Sub AddSingleMR_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim content = CType(values("Content"), Byte())
            Dim refreshMergeOption = CType(values("MergeOption"), MergeOption)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim contentType = "type/simple"

            Dim photo As StreamingServicePhotoBase
            If photoType Is GetType(StreamingServiceV1Photo) Then
                Dim v1Photo = New StreamingServiceV1Photo()
                v1Photo.Name = "NewPhoto"
                v1Photo.Content = content
                v1Photo.ContentType = contentType
                ctx.AddObject("V1Photos", v1Photo)
                photo = v1Photo
            Else
                Dim v2Photo = New StreamingServicePhoto()
                v2Photo.Name = "NewPhoto"
                ctx.AddObject("Photos", v2Photo)
                ctx.SetSaveStream(v2Photo, New MemoryStream(content), False, contentType, "")
                photo = v2Photo
            End If

            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)

            ' Query for the newly added entities so that we get their IDs and MLE stuff updated
            ' This is a known issue in the product
            ' We use Count to iterate over all results (to force materialize them)
            ctx.MergeOption = refreshMergeOption
            If photoType Is GetType(StreamingServiceV1Photo) Then
                ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos", UriKind.Relative)).Count()
            ElseIf photoType Is GetType(StreamingServicePhoto) Then
                ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos", UriKind.Relative)).Count()
            End If

            Dim response = Util.GetReadStream(ctx, photo, New DataServiceRequestArgs(), executionMethod)
            Assert.AreEqual(contentType, response.ContentType)
            VerifyStreamContent(response.Stream, content)
            Assert.AreEqual("NewPhoto", photo.Name, "The entity part of the Add didn't correctly save Name property.")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub AddMultipleMRs()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("MergeOption", New MergeOption() {MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges}),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo), Nothing}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf AddMultipleMRs_Inner)
        End Sub

        Private Sub AddMultipleMRs_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim refreshMergeOption = CType(values("MergeOption"), MergeOption)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim content1 = New Byte() {1, 2, 3}
            Dim content2 = New Byte() {11, 12, 13}
            Dim contentType1 = "type/simple_1"
            Dim contentType2 = "type/simple_2"

            Dim photos = New List(Of StreamingServicePhotoBase)()
            If photoType Is GetType(StreamingServiceV1Photo) Then
                For i As Integer = 1 To 6
                    Dim v1Photo = New StreamingServiceV1Photo()
                    v1Photo.Name = "NewPhoto"
                    If i Mod 2 = 0 Then
                        v1Photo.Content = content1
                        v1Photo.ContentType = contentType1
                    Else
                        v1Photo.Content = content2
                        v1Photo.ContentType = contentType2
                    End If
                    ctx.AddObject("V1Photos", v1Photo)
                    photos.Add(v1Photo)
                Next
            Else
                For i As Integer = 1 To 6
                    Dim v2Photo = New StreamingServicePhoto()
                    v2Photo.Name = "NewPhoto"
                    ctx.AddObject("Photos", v2Photo)
                    If i Mod 2 = 0 Then
                        ctx.SetSaveStream(v2Photo, New MemoryStream(content1), False, contentType1, "")
                    Else
                        ctx.SetSaveStream(v2Photo, New MemoryStream(content2), False, contentType2, "")
                    End If
                    photos.Add(v2Photo)
                Next
            End If

            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)

            ctx.MergeOption = refreshMergeOption
            If photoType Is GetType(StreamingServiceV1Photo) Then
                ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos", UriKind.Relative)).Count()
            ElseIf photoType Is GetType(StreamingServicePhoto) Then
                ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos", UriKind.Relative)).Count()
            End If

            For i As Integer = 1 To 6
                Dim photo = photos(i - 1)
                Dim response = Util.GetReadStream(ctx, photo, New DataServiceRequestArgs(), executionMethod)
                If i Mod 2 = 0 Then
                    Assert.AreEqual(contentType1, response.ContentType)
                    VerifyStreamContent(response.Stream, content1)
                Else
                    Assert.AreEqual(contentType2, response.ContentType)
                    VerifyStreamContent(response.Stream, content2)
                End If
                Assert.AreEqual("NewPhoto", photo.Name, "The MLE wasn't correctly updated during the Add operation.")
            Next
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub VerifyAddRemovesStream()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf VerifyAddRemovesStream_Inner)
        End Sub

        Private Sub VerifyAddRemovesStream_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = New StreamingServicePhoto()
            Dim content = New Byte() {1, 2, 3}

            ResetRequestHeaders()
            ctx.AddObject("Photos", photo)
            ctx.SetSaveStream(photo, New MemoryStream(content), False, "some/type", "myslug")
            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)

            ' And now call SaveChanges again - should do nothing
            ResetRequestHeaders()
            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)
            Dim headers = GetRequestHeaders()
            Assert.IsFalse(headers.ContainsKey("/Photos"), "No request should have been sent.")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub AddMRHeaders()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf AddMRHeaders_Inner)
        End Sub

        Private Sub AddMRHeaders_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = New StreamingServicePhoto()
            Dim content = New Byte() {1, 2, 3}

            ResetRequestHeaders()
            ctx.AddObject("Photos", photo)
            ctx.SetSaveStream(photo, New MemoryStream(content), False, "some/type", "myslug")
            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)

            Dim headers = GetRequestHeaders()("/Photos")
            Assert.AreEqual("some/type", headers("Content-Type"), "Content type header was not sent properly.")
            Assert.AreEqual("myslug", headers("Slug"), "Slug header was not sent properly.")

        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub AddMRWithoutStream()
            ResetServiceContent()

            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = New StreamingServicePhoto
            ctx.AddObject("Photos", photo)

            ' Trying to POST new MR marked with HasStream attribute without the save stream should fail
            Try
                ctx.SaveChanges()
            Catch ex As InvalidOperationException
            End Try
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub VerifyChunkedEncoding()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf VerifyChunkedEncoding_Inner)
        End Sub

        Private Sub VerifyChunkedEncoding_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            ' Verify that by default chunked encoding is on
            ResetRequestHeaders()
            Dim photo = New StreamingServicePhoto
            ctx.AddObject("Photos", photo)
            ctx.SetSaveStream(photo, New MemoryStream(New Byte() {1, 2, 3}), False, "simple/type", "slug")
            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)
            Dim headers = GetRequestHeaders()("/Photos")
            Assert.AreEqual("chunked", headers("Transfer-Encoding"), "Chunked encoding was not enabled by default.")

        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MRAddCloseStreamTests()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("CloseStream", New Boolean() {False, True}),
                New Dimension("FailRequest", New Boolean() {False, True}),
                New Dimension("SaveChangesOptions", New SaveChangesOptions() {SaveChangesOptions.None, SaveChangesOptions.ContinueOnError}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MRAddCloseStream_Inner)
        End Sub

        Private Sub MRAddCloseStream_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim closeStream = CType(values("CloseStream"), Boolean)
            Dim failRequest = CType(values("FailRequest"), Boolean)
            Dim saveChangesOptions = CType(values("SaveChangesOptions"), SaveChangesOptions)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim content = New Byte() {1, 2, 3}
            Dim photo = New StreamingServicePhoto

            Dim stream = New TestingStream(New MemoryStream(content))
            stream.OnlyRead = True
            stream.NoSeek = True
            ctx.AddObject("Photos", photo)
            Dim args = New DataServiceRequestArgs
            If failRequest Then
                args.Headers("Test_FailThisRequest") = "true"
            End If
            ctx.SetSaveStream(photo, stream, closeStream, args)

            Try
                Util.SaveChanges(ctx, saveChangesOptions, executionMethod)
                Assert.IsFalse(failRequest, "SaveChanges should have failed.")
            Catch e As DataServiceRequestException
                Assert.IsTrue(failRequest, "SaveChanges should not fail in this case.")
            End Try

            If closeStream Then
                Assert.IsTrue(stream.CloseCalled, "Stream was not closed when it should have been.")
            Else
                Assert.IsFalse(stream.CloseCalled, "Stream was closed when it shouldn't have been.")
            End If
            Assert.IsFalse(stream.DisposeCalled, "Stream was disposed even though it should never happen.")
        End Sub

        ' This test verifies that in the V1 case we fixed the problem with the POST failing (in which case we used to happily issue
        '   the MLE PUT if ContinueOnError was on)
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub V1AddMRWithFailingPOST()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("ContinueOnError", New Boolean() {False, True}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf V1AddMRWithFailingPOST_Inner)
        End Sub

        Private Sub V1AddMRWithFailingPOST_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim continueOnError = CType(values("ContinueOnError"), Boolean)

            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim p = New StreamingServiceV1Photo
            p.Content = New Byte() {1, 2, 3}
            ' This will make the MR POST fail
            p.ContentType = "test/fail-this-request"
            p.Name = "SomeName"

            Dim saveOptions = SaveChangesOptions.None
            If continueOnError Then
                saveOptions = SaveChangesOptions.ContinueOnError
            End If

            ctx.AddObject("V1Photos", p)
            Try
                Util.SaveChanges(ctx, saveOptions, executionMethod)
                Assert.Fail("SaveChanges should have failed.")
            Catch e As DataServiceRequestException
            End Try

            ' Try it again with non-failing POST
            p.ContentType = "test/valid"
            Util.SaveChanges(ctx, saveOptions, executionMethod)
        End Sub

        ' Test which verifies that trying to perform an MR Add operation in a batch fails
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MRAddInBatch()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo)}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MRAddInBatch_Inner)
        End Sub

        Private Sub MRAddInBatch_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            If photoType Is GetType(StreamingServicePhoto) Then
                Dim v2photo = New StreamingServicePhoto()
                v2photo.ID = 100
                ctx.AddObject("Photos", v2photo)
                ctx.SetSaveStream(v2photo, New MemoryStream(), False, "text", "slug")
            ElseIf photoType Is GetType(StreamingServiceV1Photo) Then
                Dim v1photo = New StreamingServiceV1Photo()
                v1photo.ID = 101
                ctx.AddObject("V1Photos", v1photo)
                v1photo.Content = New Byte() {1}
                v1photo.ContentType = "text"
            End If

            Try
                Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, executionMethod)
                Assert.Fail("Should never get here, the SaveChanges should have failed.")
            Catch ex As NotSupportedException
                Assert.AreEqual("Saving entities with the [MediaEntry] attribute is not currently supported in batch mode. Use non-batched mode instead.", ex.Message)
            End Try
        End Sub

#End Region

#Region "Update MR/MLE"
        <Flags()>
        Public Enum UpdateEntityOptions
            UpdateMLE = 1
            UpdateMR = 2
        End Enum

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub UpdateSingleMRMLE()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo), Nothing}),
                New Dimension("UpdateEntityOptions", New UpdateEntityOptions() {UpdateEntityOptions.UpdateMR, UpdateEntityOptions.UpdateMLE, UpdateEntityOptions.UpdateMLE Or UpdateEntityOptions.UpdateMR}),
                New Dimension("Content", New Byte()() {New Byte() {41, 42, 43}, New Byte() {}}),
                New Dimension("SetContentLength", New Boolean() {False, True}),
                New Dimension("EnableBuffering", New Boolean() {False, True}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf UpdateSingleMRMLE_Inner)
        End Sub

        Private Sub UpdateSingleMRMLE_Inner(ByVal values As Hashtable)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim updateEntityOptions = CType(values("UpdateEntityOptions"), UpdateEntityOptions)

            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim content = CType(values("Content"), Byte())
            Dim contentType = "type/newtype"
            Dim name = "UpdatedName"

            Dim setContentLengthExplicitly = CType(values("SetContentLength"), Boolean)
            Dim enableBuffering = CType(values("EnableBuffering"), Boolean)

            Dim photo As StreamingServicePhotoBase
            If photoType Is GetType(StreamingServicePhoto) Then
                photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Else
                ' We don't support updating MRs for V1 entities
                If (updateEntityOptions And MediaStreaming.UpdateEntityOptions.UpdateMR) = MediaStreaming.UpdateEntityOptions.UpdateMR Then
                    Return
                End If
                photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            End If

            If ((updateEntityOptions And MediaStreaming.UpdateEntityOptions.UpdateMLE) = MediaStreaming.UpdateEntityOptions.UpdateMLE) And
                Not photoType Is Nothing Then
                photo.Name = name
                ctx.UpdateObject(photo)
            Else
                name = photo.Name
            End If

            If (updateEntityOptions And MediaStreaming.UpdateEntityOptions.UpdateMR) = MediaStreaming.UpdateEntityOptions.UpdateMR Then
                ctx.SetSaveStream(photo, New MemoryStream(content), False, contentType, "")
            End If

            Dim sendingRequestHandler =
                Sub(sender As Object, e As SendingRequest2EventArgs)
                    Dim request = CType(e.RequestMessage, HttpWebRequestMessage).HttpWebRequest
                    If setContentLengthExplicitly Then
                        ' Checks that streaming/non-streaming is OK with different content lengths, including 0 and unset
                        request.ContentLength = content.Length
                    End If
                    request.AllowWriteStreamBuffering = enableBuffering
                End Sub

            Dim testHook = New HttpTestHookConsumer(ctx, False)
            testHook.CustomSendRequestAction =
                Sub(request As Object)
                    Dim httpWebRequest = CType(request, HttpWebRequest)
                    If httpWebRequest.Method = "POST" OrElse httpWebRequest.Method = "PUT" Then
                        Assert.AreEqual(enableBuffering, httpWebRequest.AllowWriteStreamBuffering,
                                        "Stream buffering is turned on when it should not have been, or off when it should.")
                    End If
                End Sub

            AddHandler ctx.SendingRequest2, sendingRequestHandler
            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)
            RemoveHandler ctx.SendingRequest2, sendingRequestHandler

            If (updateEntityOptions And MediaStreaming.UpdateEntityOptions.UpdateMR) = MediaStreaming.UpdateEntityOptions.UpdateMR Then
                Dim response = Util.GetReadStream(ctx, photo, New DataServiceRequestArgs(), executionMethod)
                Assert.AreEqual(contentType, response.ContentType)
                VerifyStreamContent(response.Stream, content)
            End If

            ' easy way to force update of the client entities with whatever the server has
            If photoType Is GetType(StreamingServicePhoto) Then
                ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).Count()
            ElseIf photoType Is GetType(StreamingServiceV1Photo) Then
                ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).Count()
            End If
            Assert.AreEqual(name, photo.Name, "The name was not updated properly (or was updated when it shouldn't have been)")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub UpdateMultipleMRMLEs()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo), Nothing}),
                New Dimension("UpdateEntityOptions", New UpdateEntityOptions() {UpdateEntityOptions.UpdateMR, UpdateEntityOptions.UpdateMLE, UpdateEntityOptions.UpdateMLE Or UpdateEntityOptions.UpdateMR}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf UpdateMultipleMRMLEs_Inner)
        End Sub

        Private Sub UpdateMultipleMRMLEs_Inner(ByVal values As Hashtable)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim updateEntityOptions = CType(values("UpdateEntityOptions"), UpdateEntityOptions)

            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim content1 = New Byte() {1, 2, 3}
            Dim content2 = New Byte() {11, 12, 13}
            Dim contentType1 = "type/simple_1"
            Dim contentType2 = "type/simple_2"
            Dim name1 = "UpdatedName1"
            Dim name2 = "UpdatedName2"
            Dim names = New List(Of String)()

            Dim photos = New List(Of StreamingServicePhotoBase)()
            If photoType Is GetType(StreamingServicePhoto) Then
                For i As Integer = 1 To 6
                    Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(" + i.ToString() + ")", UriKind.Relative)).SingleOrDefault()
                    photos.Add(photo)
                Next
            Else
                ' We don't support updating MRs for V1 entities
                If (updateEntityOptions And MediaStreaming.UpdateEntityOptions.UpdateMR) = MediaStreaming.UpdateEntityOptions.UpdateMR Then
                    Return
                End If
                For i As Integer = 1 To 6
                    Dim photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(" + i.ToString() + ")", UriKind.Relative)).SingleOrDefault()
                    photos.Add(photo)
                Next
            End If

            If ((updateEntityOptions And MediaStreaming.UpdateEntityOptions.UpdateMLE) = MediaStreaming.UpdateEntityOptions.UpdateMLE) And
                Not photoType Is Nothing Then
                For i As Integer = 1 To 6
                    If i Mod 2 = 0 Then
                        photos(i - 1).Name = name1
                    Else
                        photos(i - 1).Name = name2
                    End If
                    ctx.UpdateObject(photos(i - 1))
                Next
            End If
            For i As Integer = 1 To 6
                names.Add(photos(i - 1).Name)
            Next

            If (updateEntityOptions And MediaStreaming.UpdateEntityOptions.UpdateMR) = MediaStreaming.UpdateEntityOptions.UpdateMR Then
                For i As Integer = 1 To 6
                    If i Mod 2 = 0 Then
                        ctx.SetSaveStream(photos(i - 1), New MemoryStream(content1), False, contentType1, "")
                    Else
                        ctx.SetSaveStream(photos(i - 1), New MemoryStream(content2), False, contentType2, "")
                    End If
                Next
            End If

            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)

            If (updateEntityOptions And MediaStreaming.UpdateEntityOptions.UpdateMR) = MediaStreaming.UpdateEntityOptions.UpdateMR Then
                For i As Integer = 1 To 6
                    Dim response = Util.GetReadStream(ctx, photos(i - 1), New DataServiceRequestArgs(), executionMethod)
                    If i Mod 2 = 0 Then
                        Assert.AreEqual(contentType1, response.ContentType)
                        VerifyStreamContent(response.Stream, content1)
                    Else
                        Assert.AreEqual(contentType2, response.ContentType)
                        VerifyStreamContent(response.Stream, content2)
                    End If
                Next
            End If

            ' easy way to force update of the client entities with whatever the server has
            ctx.MergeOption = MergeOption.OverwriteChanges
            If photoType Is GetType(StreamingServicePhoto) Then
                ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos", UriKind.Relative)).Count()
            ElseIf photoType Is GetType(StreamingServiceV1Photo) Then
                ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos", UriKind.Relative)).Count()
            End If
            For i As Integer = 1 To 6
                Assert.AreEqual(names(i - 1), photos(i - 1).Name, "The name was not updated properly (or was updated when it shouldn't have been)")
            Next
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub VerifyUpdateRemovesStream()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf VerifyUpdateRemovesStream_Inner)
        End Sub

        Private Sub VerifyUpdateRemovesStream_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Dim content = New Byte() {1, 2, 3}

            ResetRequestHeaders()
            ctx.SetSaveStream(photo, New MemoryStream(content), False, "some/type", "myslug")
            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)

            ' And now call SaveChanges again - should do nothing
            ResetRequestHeaders()
            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)
            Dim headers = GetRequestHeaders()
            Assert.IsFalse(headers.ContainsKey("/Photos"), "No request should have been sent.")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub UpdateMRHeaders()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("ContentTypeHeader", New String() {"my/type"}),
                New Dimension("SlugHeader", New String() {"myslug", Nothing}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf UpdateMRHeaders_Inner)
        End Sub

        Private Sub UpdateMRHeaders_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim contentTypeHeader = CType(values("ContentTypeHeader"), String)
            Dim slugHeader = CType(values("SlugHeader"), String)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Dim content = New Byte() {1, 2, 3}

            Dim headers As WebHeaderCollection
            If Not contentTypeHeader Is Nothing And Not slugHeader Is Nothing Then
                ResetRequestHeaders()
                ctx.SetSaveStream(photo, New MemoryStream(content), False, contentTypeHeader, slugHeader)
                Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)

                headers = GetRequestHeaders()("/Photos(1)/$value")
                Assert.AreEqual(contentTypeHeader, headers("Content-Type"), "Content type header was not sent properly.")
                Assert.AreEqual(slugHeader, headers("Slug"), "Slug header was not sent properly.")
            End If
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub UpdateVerifyChunkedEncoding()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf UpdateVerifyChunkedEncoding_Inner)
        End Sub

        Private Sub UpdateVerifyChunkedEncoding_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            ' Verify that by default chunked encoding is on
            ResetRequestHeaders()
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            ctx.SetSaveStream(photo, New MemoryStream(New Byte() {1, 2, 3}), False, "simple/type", "slug")
            Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)
            Dim headers = GetRequestHeaders()("/Photos(1)/$value")
            Assert.AreEqual("chunked", headers("Transfer-Encoding"), "Chunked encoding was not enabled by default.")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MRUpdateCloseStreamTests()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("CloseStream", New Boolean() {False, True}),
                New Dimension("FailRequest", New Boolean() {False, True}),
                New Dimension("SaveChangesOptions", New SaveChangesOptions() {SaveChangesOptions.None, SaveChangesOptions.ContinueOnError}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MRUpdateCloseStream_Inner)
        End Sub

        Private Sub MRUpdateCloseStream_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim closeStream = CType(values("CloseStream"), Boolean)
            Dim failRequest = CType(values("FailRequest"), Boolean)
            Dim saveChangesOptions = CType(values("SaveChangesOptions"), SaveChangesOptions)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim content = New Byte() {1, 2, 3}
            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Dim photo2 = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(2)", UriKind.Relative)).SingleOrDefault()

            Dim stream = New TestingStream(New MemoryStream(content))
            stream.OnlyRead = True
            stream.NoSeek = True
            Dim args = New DataServiceRequestArgs
            If failRequest Then
                args.Headers("Test_FailThisRequest") = "true"
            End If
            ctx.SetSaveStream(photo, stream, closeStream, args)

            Dim stream2 = New TestingStream(New MemoryStream(content))
            stream2.OnlyRead = True
            stream2.NoSeek = False
            ctx.SetSaveStream(photo2, stream2, closeStream, "content/type", "slug")

            Try
                Util.SaveChanges(ctx, saveChangesOptions, executionMethod)
                Assert.IsFalse(failRequest, "SaveChanges should have failed.")
            Catch e As DataServiceRequestException
                Assert.IsTrue(failRequest, "SaveChanges should not fail in this case.")
            End Try

            If closeStream Then
                Assert.IsTrue(stream.CloseCalled, "Stream was not closed when it should have been.")
                Assert.IsTrue(stream2.CloseCalled, "Stream was not closed when it should have been.")
            Else
                Assert.IsFalse(stream.CloseCalled, "Stream was closed when it shouldn't have been.")
                Assert.IsFalse(stream2.CloseCalled, "Stream was closed when it shouldn't have been.")
            End If
            Assert.IsFalse(stream.DisposeCalled, "Stream was disposed even though it should never happen.")
            Assert.IsFalse(stream2.DisposeCalled, "Stream was disposed even though it should never happen.")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MRUpdateOnNonMLE()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MRUpdateOnNonMLE_Inner)
        End Sub

        Private Sub MRUpdateOnNonMLE_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)

            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim content = New Byte() {1, 2, 3}
            Dim entity = ctx.Execute(Of StreamingServiceEntityWithoutStream)(New Uri("/EntitiesWithoutStream", UriKind.Relative)).FirstOrDefault()

            ctx.SetSaveStream(entity, New MemoryStream(content), True, "content/type", "slug")

            Try
                Util.SaveChanges(ctx, SaveChangesOptions.None, executionMethod)
                Assert.Fail("SaveChanges should have failed since there's no edit-media link for this entity.")
            Catch ex As InvalidOperationException
            End Try
        End Sub

        ' Test which verifies that trying to perform an MR Update operation in a batch fails
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MRUpdateInBatch()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto)}),
                New Dimension("UpdateMLE", New Boolean() {False, True}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MRUpdateInBatch_Inner)
        End Sub

        Private Sub MRUpdateInBatch_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim updateMLE = CType(values("UpdateMLE"), Boolean)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim v2photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            ctx.SetSaveStream(v2photo, New MemoryStream(), False, "text", "slug")
            If updateMLE Then
                v2photo.Name = "NewName"
                ctx.UpdateObject(v2photo)
            End If

            Try
                Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, executionMethod)
                Assert.Fail("Should never get here, the SaveChanges should have failed.")
            Catch ex As NotSupportedException
                Assert.AreEqual("Saving entities with the [MediaEntry] attribute is not currently supported in batch mode. Use non-batched mode instead.", ex.Message)
            End Try
        End Sub

        ' Test which verifies that trying to perform an MLE Update (without MR update) operation in a batch succeeds
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MLEUpdateInBatch()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto)}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MLEUpdateInBatch_Inner)
        End Sub

        Private Sub MLEUpdateInBatch_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photoEntity As Object
            Dim v2photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            v2photo.Name = "NewName"
            photoEntity = v2photo

            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Unchanged)
            ctx.UpdateObject(photoEntity)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Modified)
            Dim dataServiceResponse As DataServiceResponse = Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, executionMethod)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Unchanged)

            VerifyBatchOperationResponses(dataServiceResponse, New ExpectedResult() {New ExpectedResult(HttpStatusCode.NoContent, Nothing)})
            v2photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Assert.AreEqual("NewName", v2photo.Name, "Name wasn't updated.")
        End Sub

        ' Test which verifies errors are handled correctly if the server fails to process a correct batch request from the client
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MLEUpdateInBatchWhenServerErrors()
            Dim service As TestWebRequest = TestWebRequest.CreateForInProcessWcf
            Try
                TestUtil.ClearConfiguration()
                AstoriaUnitTests.Stubs.StreamingService.InitializeServiceOverride = AddressOf InitializeServiceOrverride
                service.ServiceType = GetType(AstoriaUnitTests.Stubs.StreamingService)
                service.StartService()

                Dim engine = CombinatorialEngine.FromDimensions(
                    New Dimension("Service", New TestWebRequest() {service}),
                    New Dimension("ExecutionMethod", Util.ExecutionMethods),
                    New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto)}))

                TestUtil.RunCombinatorialEngineFail(engine, AddressOf MLEUpdateInBatchWhenServerErrors_Inner)
            Finally
                AstoriaUnitTests.Stubs.StreamingService.InitializeServiceOverride = Nothing
                If Not (service Is Nothing) Then
                    service.StopService()
                End If
                TestUtil.ClearConfiguration()
            End Try
        End Sub

        Private Shared Sub InitializeServiceOrverride(ByVal config As DataServiceConfiguration)
            config.SetEntitySetAccessRule("Photos", EntitySetRights.AllRead)
            config.SetEntitySetAccessRule("V1Photos", EntitySetRights.AllRead)
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All)
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4
            config.UseVerboseErrors = True
        End Sub

        Private Sub MLEUpdateInBatchWhenServerErrors_Inner(ByVal values As Hashtable)
            Dim service As TestWebRequest = CType(values("Service"), TestWebRequest)

            ' Reset content
            Dim resetContentCtx = New DataServiceContext(service.ServiceRoot)
            'resetContentctx.EnableAtom = True
            'resetContentCtx.Format.UseAtom()
            resetContentCtx.Execute(Of Boolean)(New Uri("/ResetContent", UriKind.Relative)).Count()

            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim ctx = New DataServiceContext(service.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            ctx.MergeOption = MergeOption.OverwriteChanges

            Dim photoEntity As StreamingServicePhotoBase
            photoEntity = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()

            Dim v1Photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            v1Photo.Name = "UpdatedName"
            ctx.UpdateObject(v1Photo)

            photoEntity.Name = "NewName"
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Unchanged)
            ctx.UpdateObject(photoEntity)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Modified)

            Try
                Dim dataServiceResponse As DataServiceResponse = Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, executionMethod)
            Catch ex As DataServiceRequestException
                Assert.AreEqual("An error occurred while processing this request.", ex.Message)
            End Try

            Dim photoEntityAfterException As StreamingServicePhotoBase
            photoEntityAfterException = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()

            Assert.AreEqual("Number1", photoEntityAfterException.Name, "Name was updated but was not supposed to.")

            Dim v1PhotoEntityAfterException = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            Assert.AreEqual("Number1", v1PhotoEntityAfterException.Name)

            For Each entity In ctx.Entities
                Assert.AreEqual(entity.State, EntityStates.Unchanged)
            Next
        End Sub
#End Region

#Region "Create MLE Batch"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MLECreateInBatch()
            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo)}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MLECreateInBatch_Inner)
        End Sub

        Private Sub MLECreateInBatch_Inner(ByVal values As Hashtable)
            ResetServiceContent()

            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo As StreamingServicePhotoBase
            Dim entitySetName As String
            If photoType Is GetType(StreamingServicePhoto) Then
                photo = New StreamingServicePhoto()
                entitySetName = "Photos"
            ElseIf photoType Is GetType(StreamingServiceV1Photo) Then
                photo = New StreamingServiceV1Photo()
                entitySetName = "V1Photos"
            Else
                Throw New ArgumentException("Unexpected photo type: " & photoType.ToString(), "PhotoType")
            End If

            photo.ID = 13
            photo.Name = "New photo"

            ctx.AddObject(entitySetName, photo)
            Assert.AreEqual(1, ctx.Entities.Count)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Added)
            Try
                Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, executionMethod)
                Assert.Fail("Exception expected but not thrown")
            Catch ex As NotSupportedException
                Assert.AreEqual("Saving entities with the [MediaEntry] attribute is not currently supported in batch mode. Use non-batched mode instead.", ex.Message)
            End Try
        End Sub

#End Region

#Region "Delete MLE Batch"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MLEDeleteInBatch()
            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo)}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MLEDeleteInBatch_Inner)
        End Sub

        Private Sub MLEDeleteInBatch_Inner(ByVal values As Hashtable)
            ResetServiceContent()

            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo As StreamingServicePhotoBase
            If photoType Is GetType(StreamingServicePhoto) Then
                photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Else
                photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            End If

            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Unchanged)
            ctx.DeleteObject(photo)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Deleted)
            Dim dataServiceResponse As DataServiceResponse = Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, executionMethod)
            Assert.AreEqual(0, ctx.Entities.Count)

            VerifyBatchOperationResponses(dataServiceResponse, New ExpectedResult() {New ExpectedResult(HttpStatusCode.NoContent, Nothing)})

            ctx.IgnoreResourceNotFoundException = True
            If photoType Is GetType(StreamingServicePhoto) Then
                Assert.AreEqual(0, ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).Count(), "Photo wasn't deleted.")
            Else
                Assert.AreEqual(0, ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).Count(), "Photo wasn't deleted.")
            End If
        End Sub
#End Region

#Region "Get MLE Batch"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MLEGetInBatch()
            ResetServiceContent()

            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo)}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MLEGetInBatch_Inner)
        End Sub

        Private Sub MLEGetInBatch_Inner(ByVal values As Hashtable)
            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim queries As List(Of DataServiceQuery) = New List(Of DataServiceQuery)
            If photoType Is GetType(StreamingServicePhoto) Then
                queries.Add(CType((From p In ctx.CreateQuery(Of StreamingServicePhoto)("Photos") Where p.ID = 1 Select p), DataServiceQuery))
            Else
                queries.Add(CType((From p In ctx.CreateQuery(Of StreamingServiceV1Photo)("V1Photos") Where p.ID = 1 Select p), DataServiceQuery))
            End If

            Dim dataServiceResponse = Util.ExecuteBatch(ctx, queries.ToArray(), executionMethod)

            Assert.AreEqual(0, ctx.Entities.Count)
            Assert.IsTrue(dataServiceResponse.IsBatchResponse, "We should get a batch response.")
            VerifyBatchOperationResponses(dataServiceResponse, New ExpectedResult() {New ExpectedResult(HttpStatusCode.OK, Nothing)})
        End Sub
#End Region

#Region "Sequences of operations on MLE - Batch"
        ' Test which verifies that trying to perform an MLE Update (without MR update) operation in a batch succeeds
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MLEUpdateDeleteInBatch()
            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo)}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MLEUpdateDeleteInBatch_Inner)
        End Sub

        Private Sub MLEUpdateDeleteInBatch_Inner(ByVal values As Hashtable)
            ResetServiceContent()

            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo As StreamingServicePhotoBase
            If photoType Is GetType(StreamingServicePhoto) Then
                photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Else
                photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            End If

            photo.Name = "NewName"

            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Unchanged)
            ctx.UpdateObject(photo)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Modified)
            ctx.DeleteObject(photo)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Deleted)
            Dim dataServiceResponse As DataServiceResponse = Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, executionMethod)
            Assert.AreEqual(0, ctx.Entities.Count)

            VerifyBatchOperationResponses(dataServiceResponse, New ExpectedResult() {New ExpectedResult(HttpStatusCode.NoContent, Nothing)})

            ctx.IgnoreResourceNotFoundException = True
            If photoType Is GetType(StreamingServicePhoto) Then
                Assert.AreEqual(0, ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).Count(), "Photo wasn't deleted.")
            Else
                Assert.AreEqual(0, ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).Count(), "Photo wasn't deleted.")
            End If
        End Sub

        ' Test which verifies that trying to perform an MLE Update (without MR update) operation in a batch succeeds
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub MLEUpdateUpdateInBatch()
            Dim engine = CombinatorialEngine.FromDimensions(
                New Dimension("ExecutionMethod", Util.ExecutionMethods),
                New Dimension("PhotoType", New Type() {GetType(StreamingServicePhoto), GetType(StreamingServiceV1Photo)}))

            TestUtil.RunCombinatorialEngineFail(engine, AddressOf MLEUpdateUpdateInBatch_Inner)
        End Sub

        Private Sub MLEUpdateUpdateInBatch_Inner(ByVal values As Hashtable)
            ResetServiceContent()

            Dim executionMethod = CType(values("ExecutionMethod"), Util.ExecutionMethod)
            Dim photoType = CType(values("PhotoType"), Type)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            Dim photo As StreamingServicePhotoBase
            If photoType Is GetType(StreamingServicePhoto) Then
                photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            Else
                photo = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            End If

            photo.Name = "NewName"
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Unchanged)
            ctx.UpdateObject(photo)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Modified)
            photo.Length = 10
            ctx.UpdateObject(photo)
            Assert.AreEqual(ctx.Entities.First().State, EntityStates.Modified)
            Dim dataServiceResponse As DataServiceResponse = Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, executionMethod)
            Assert.AreEqual(1, ctx.Entities.Count)

            VerifyBatchOperationResponses(dataServiceResponse, New ExpectedResult() {New ExpectedResult(HttpStatusCode.NoContent, Nothing)})

            ctx.IgnoreResourceNotFoundException = True

            Dim updatedPhoto As StreamingServicePhotoBase
            If photoType Is GetType(StreamingServiceV1Photo) Then
                updatedPhoto = ctx.Execute(Of StreamingServiceV1Photo)(New Uri("/V1Photos(1)", UriKind.Relative)).SingleOrDefault()
            Else
                updatedPhoto = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            End If

            Assert.AreEqual("NewName", updatedPhoto.Name, "Name wasn't updated.")
            Assert.AreEqual(10, updatedPhoto.Length, "Name wasn't updated.")
        End Sub


#End Region

#Region "MLE Multiple operations in a batch request"

#End Region

#Region "DataServiceRequestArgs tests"
        <TestCategory("Partition3")> <TestMethod()>
        Public Sub DataServiceRequestArgs_Defaults()
            Dim args = New DataServiceRequestArgs()
            Assert.IsNull(args.AcceptContentType, "AcceptContentType should be null by default.")
            Assert.IsNull(args.ContentType, "ContentType should be null by default.")
            Assert.IsNull(args.Slug, "Slug should be null by default.")
            Assert.IsNotNull(args.Headers, "Headers must be non-null.")
            Assert.AreEqual(0, args.Headers.Count, "Headers should be empty by default.")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub DataServiceRequestArgs_AcceptContentType()
            DataServiceRequestArgs_Inner_PropertyTest("Accept", GetType(DataServiceRequestArgs).GetProperty("AcceptContentType", BindingFlags.Instance Or BindingFlags.Public))
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub DataServiceRequestArgs_ContentType()
            DataServiceRequestArgs_Inner_PropertyTest("Content-Type", GetType(DataServiceRequestArgs).GetProperty("ContentType", BindingFlags.Instance Or BindingFlags.Public))
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub DataServiceRequestArgs_Slug()
            DataServiceRequestArgs_Inner_PropertyTest("Slug", GetType(DataServiceRequestArgs).GetProperty("Slug", BindingFlags.Instance Or BindingFlags.Public))
        End Sub

        Private Sub DataServiceRequestArgs_Inner_PropertyTest(ByVal header As String, ByVal propertyInfo As PropertyInfo)
            Dim args = New DataServiceRequestArgs()
            Dim testValue = "testval"
            args.Headers(header) = testValue
            Assert.AreEqual(testValue, args.Headers(header), "The value is not correctly remembered in the Headers collection.")
            Assert.AreEqual(testValue, CType(propertyInfo.GetValue(args, Nothing), String), "Property doesn't reflect value set through Headers.")

            args.Headers.Remove(header)
            Assert.IsNull(propertyInfo.GetValue(args, Nothing), "When header is removed the property should be null.")

            args = New DataServiceRequestArgs()
            propertyInfo.SetValue(args, testValue, Nothing)
            Assert.AreEqual(testValue, CType(propertyInfo.GetValue(args, Nothing), String), "The value is not correctly remembered in the property.")
            Assert.AreEqual(testValue, args.Headers(header), "Headers doesn't reflect value set through property.")

            propertyInfo.SetValue(args, Nothing, Nothing)
            Assert.IsNull(propertyInfo.GetValue(args, Nothing), "The null value is not correctly remembered in the property.")
            Assert.IsFalse(args.Headers.ContainsKey(header), "Setting the property to null should remove the header.")
        End Sub
#End Region

        <TestCategory("Partition3")> <TestMethod(), Variation("End to end tests using a service with some named streams")>
        Public Sub CloseStreamTiming()
            TestUtil.RunCombinations(Util.ExecutionMethods, AddressOf CloseStreamTiming_Internal)
        End Sub

        Private Sub CloseStreamTiming_Internal(ByVal execMethod As Util.ExecutionMethod)
            Dim source As tp.DSPContext = New tp.DSPContext()
            Using request As InProcessWcfWebRequest = DirectCast(SetUpDSPService(source, True).CreateForInProcessWcf(), InProcessWcfWebRequest)
                request.ForceVerboseErrors = True
                request.StartService()

                OpenWebDataServiceHelper.ForceVerboseErrors = True

                Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
                'context.EnableAtom = True
                'context.Format.UseAtom()
                Dim c As New DummyTypeWithStream With {
                    .ID = 12345,
                    .Name = "Microsoft"
                }

                Dim cExtraCall As New DummyTypeWithStream With {
                    .ID = 12346,
                    .Name = "Extra to get another call to SendingRequest"
                }

                Dim streams As New Dictionary(Of Integer, Tuple(Of Boolean, TestingStream))
                Dim sendEventHitCount As Integer = 0
                AddHandler context.SendingRequest2, Sub(sender, eventArgs)
                                                        Dim entry As Tuple(Of Boolean, TestingStream) = Nothing
                                                        If streams.TryGetValue(sendEventHitCount + 1, entry) Then
                                                            ' we are one before the stream should be closed
                                                            Assert.IsFalse(entry.Item2.CloseCalled, "The stream was closed early?")
                                                        End If


                                                        If streams.TryGetValue(sendEventHitCount, entry) Then
                                                            Assert.IsTrue(entry.Item2.CloseCalled, "Close was not called")
                                                            streams(sendEventHitCount) = New Tuple(Of Boolean, TestingStream)(True, entry.Item2)
                                                        End If

                                                        sendEventHitCount = sendEventHitCount + 1
                                                    End Sub

                Dim requestCount As Integer = 1
                context.AddObject("MySet", c)
                Dim stream As TestingStream
                stream = New TestingStream(New MemoryStream(New Byte() {5, 6, 7}))
                context.SetSaveStream(c, stream, True, New DataServiceRequestArgs() With {.AcceptContentType = "image/jpeg", .Slug = "12345"})
                streams.Add(requestCount, New Tuple(Of Boolean, TestingStream)(False, stream))
                'MR POST, and MLE PUT
                requestCount += 2


                stream = New TestingStream(New MemoryStream(New Byte() {0, 1, 2, 3}))
                context.SetSaveStream(c, "Stream1", stream, True, "image/jpeg")
                streams.Add(requestCount, New Tuple(Of Boolean, TestingStream)(False, stream))
                ' Put Named Stream
                requestCount += 1

                ' just force an extra request (2 actually) on the end to allow us to get a final SendingRequest call
                context.AddObject("MySet", cExtraCall)
                stream = New TestingStream(New MemoryStream(New Byte() {8, 9, 10}))
                context.SetSaveStream(cExtraCall, stream, True, New DataServiceRequestArgs() With {.AcceptContentType = "image/jpeg", .Slug = "12346"})
                'MR POST, and MLE PUT
                requestCount += 2
                Util.SaveChanges(context, execMethod)

                'Update MLE and MR be sure MR is not closed during MLE Update
                c.Name = c.Name & " Updated"
                context.UpdateObject(c)
                stream = New TestingStream(New MemoryStream(New Byte() {11, 12, 13}))
                context.SetSaveStream(c, stream, True, New DataServiceRequestArgs() With {.AcceptContentType = "image/jpeg", .Slug = "12346"})
                streams.Add(requestCount, New Tuple(Of Boolean, TestingStream)(False, stream))
                'MR PUT, and MLE PUT
                requestCount += 2
                Util.SaveChanges(context, execMethod)

                Assert.IsTrue(streams.Values.All(Function(t) t.Item1), "Some streams were not looked at")
            End Using
        End Sub


#Region "End to End Named stream tests"

        <TestCategory("Partition3")> <TestMethod(), Variation("End to end tests using a service with some named streams")>
        Public Sub EndToEndNamedStreamTests()
            Dim source As tp.DSPContext = New tp.DSPContext()
            Using request As InProcessWcfWebRequest = DirectCast(SetUpDSPService(source, True).CreateForInProcessWcf(), InProcessWcfWebRequest)
                request.ForceVerboseErrors = True
                request.StartService()

                OpenWebDataServiceHelper.ForceVerboseErrors = True

                Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
                'context.EnableAtom = True
                'context.Format.UseAtom()
                Dim c As New DummyTypeWithStream With {
                 .ID = 12345,
                 .Name = "Microsoft"
                }

                context.AddObject("MySet", c)
                context.SetSaveStream(c, New MemoryStream(New Byte() {5, 6, 7}), True, New DataServiceRequestArgs() With {.AcceptContentType = "image/jpeg", .Slug = "12345"})
                context.SetSaveStream(c, "Stream1", New MemoryStream(New Byte() {0, 1, 2, 3}), True, "image/jpeg")
                context.SaveChanges()

                Dim ed = context.Entities(0)
                Dim sd = ed.StreamDescriptors.Where(Function(d) d.StreamLink.Name = "Stream1").Single()
                Assert.AreEqual(sd.StreamLink.SelfLink, Nothing, "There should be no self link")
                Assert.AreNotEqual(sd.StreamLink.EditLink, Nothing, "There must be an edit link specified")
                Assert.AreEqual(sd.StreamLink.ContentType, Nothing, "Since the put does not send a response, the content type should be null")
                Assert.AreNotEqual(sd.StreamLink.ETag, Nothing, "Etag must be present")

                ' Get the stream and make sure that the etag and content type is updated
                Dim r As DataServiceStreamResponse = context.GetReadStream(c, "Stream1", New DataServiceRequestArgs() With {
                 .AcceptContentType = "image/jpeg"
                })
                Assert.AreEqual(sd.StreamLink.ContentType, "image/jpeg", "After the get, there should be a content type")
                Assert.AreEqual(sd.StreamLink.SelfLink, Nothing, "There should be no self link")
                Assert.AreNotEqual(sd.StreamLink.EditLink, Nothing, "There must be an edit link specified")

                context.SetSaveStream(c, "Stream1", New MemoryStream(New Byte() {1, 2, 3, 4}), True, "image/bmp")
                context.SaveChanges()

                context.DeleteObject(c)
                context.SaveChanges()
            End Using
        End Sub

        <TestCategory("Partition3")> <TestMethod(), Variation("Verify that GetReadStream api updates the etag and content type for named streams")>
        Public Sub VerifyThatReadNamedStreamUpdatesTheETagAndContentType()
            Dim source As tp.DSPContext = New tp.DSPContext()
            Using request As InProcessWcfWebRequest = DirectCast(SetUpDSPService(source, False).CreateForInProcessWcf(), InProcessWcfWebRequest)
                request.ForceVerboseErrors = True
                request.StartService()

                OpenWebDataServiceHelper.ForceVerboseErrors = True

                Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
                'context.EnableAtom = True
                'context.Format.UseAtom()
                Dim c As New DummyType With {
                 .ID = 12345,
                 .Name = "Microsoft"
                }

                context.AddObject("MySet", c)
                context.SetSaveStream(c, "Stream1", New MemoryStream(New Byte() {0, 1, 2, 3}), True, "image/jpeg")
                context.SaveChanges()

                Dim ed = context.Entities(0)
                Dim sd = ed.StreamDescriptors.Where(Function(d) d.StreamLink.Name = "Stream1").Single()
                Assert.AreEqual(sd.StreamLink.SelfLink, Nothing, "There should be no self link")
                Assert.AreNotEqual(sd.StreamLink.EditLink, Nothing, "There must be an edit link specified")
                Assert.AreEqual(sd.StreamLink.ContentType, Nothing, "Since the put does not send a response, the content type should be null")
                Assert.AreNotEqual(sd.StreamLink.ETag, Nothing, "Etag must be present")
                Dim etag = sd.StreamLink.ETag

                ' now update the stream using another context
                Dim context1 As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
                'context1.EnableAtom = True
                'context1.Format.UseAtom()
                Dim dummy1 = context1.CreateQuery(Of DummyType)("MySet").First()

                context1.SetSaveStream(dummy1, "Stream1", New MemoryStream(New Byte() {3, 4, 5, 6}), True, "image/bmp")
                context1.SaveChanges()

                ' Get the stream and make sure that the etag and content type is updated
                Dim r As DataServiceStreamResponse = context.GetReadStream(c, "Stream1", New DataServiceRequestArgs() With {
                 .AcceptContentType = "image/bmp"
                })
                Assert.AreEqual(sd.StreamLink.ContentType, "image/bmp", "After the get, there should be a content type")
                Assert.AreEqual(sd.StreamLink.SelfLink, Nothing, "There should be no self link")
                Assert.AreNotEqual(sd.StreamLink.EditLink, Nothing, "There must be an edit link specified")
                Assert.AreNotEqual(etag, sd.StreamLink.ETag, "The new etag must have been populated")
                context.SetSaveStream(c, "Stream1", New MemoryStream(New Byte() {1, 2, 3, 4}), True, "image/bmp")
                context.SaveChanges()

                context.DeleteObject(c)
                context.SaveChanges()
            End Using
        End Sub

        <TestCategory("Partition3")> <TestMethod(), Variation("Verify that GetReadStream api updates the etag and content type for MR")>
        Public Sub VerifyThatReadStreamUpdatesTheETag()
            Dim source As tp.DSPContext = New tp.DSPContext()
            Using request As InProcessWcfWebRequest = DirectCast(SetUpDSPService(source, True).CreateForInProcessWcf(), InProcessWcfWebRequest)
                request.ForceVerboseErrors = True
                request.StartService()

                OpenWebDataServiceHelper.ForceVerboseErrors = True

                Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
                'context.EnableAtom = True
                'context.Format.UseAtom()
                Dim c As New DummyTypeWithStream With {
                 .ID = 12345,
                 .Name = "Microsoft"
                }

                context.AddObject("MySet", c)
                context.SetSaveStream(c, New MemoryStream(New Byte() {5, 6, 7}), True, "image/bmp", "12345")
                context.SaveChanges()

                Dim ed = context.Entities(0)
                Assert.AreNotEqual(ed.StreamETag, Nothing, "Etag must be present")
                Dim etag = ed.StreamETag

                ' now update the stream using another context
                Dim context1 As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
                'context1.EnableAtom = True
                'context1.Format.UseAtom()
                Dim dummy1 = context1.CreateQuery(Of DummyTypeWithStream)("MySet").First()

                context1.SetSaveStream(dummy1, New MemoryStream(New Byte() {3, 4, 5, 6}), True, New DataServiceRequestArgs() With {.ContentType = "image/bmp"})
                context1.SaveChanges()

                ' Get the stream and make sure that the etag and content type is updated
                Dim r As DataServiceStreamResponse = context.GetReadStream(c, New DataServiceRequestArgs() With {
                 .AcceptContentType = "image/bmp"
                })

                Assert.AreNotEqual(etag, ed.StreamETag, "The new etag must have been populated")
            End Using
        End Sub

        Private Function SetUpDSPService(ByVal context As tp.DSPContext, ByVal mediaLinkEntry As Boolean) As tp.DSPServiceDefinition
            Dim metadata As New tp.DSPMetadata("NamedStreamIDSPBasicScenariosTest", "NamedStreamTests")
            Dim entityType As p.ResourceType = metadata.AddEntityType("EntityType", Nothing, Nothing, False)
            entityType.IsMediaLinkEntry = mediaLinkEntry
            metadata.AddKeyProperty(entityType, "ID", GetType(Integer))
            metadata.AddPrimitiveProperty(entityType, "Name", GetType(String))
            entityType.AddProperty(New p.ResourceProperty("Stream1", p.ResourcePropertyKind.Stream, p.ResourceType.GetPrimitiveResourceType(GetType(System.IO.Stream))))
            entityType.AddProperty(New p.ResourceProperty("Stream2", p.ResourcePropertyKind.Stream, p.ResourceType.GetPrimitiveResourceType(GetType(System.IO.Stream))))
            metadata.AddResourceSet("MySet", entityType)

            Dim service As New tp.DSPServiceDefinition()
            service.ForceVerboseErrors = True
            service.Metadata = metadata
            service.Writable = True
            service.MediaResourceStorage = New tp.DSPMediaResourceStorage()
            service.SupportMediaResource = True
            service.SupportNamedStream = True
            service.CreateDataSource = Function() As tp.DSPContext
                                           Return context
                                       End Function

            Return service
        End Function

        <HasStream>
        Public Class DummyTypeWithStream
            Inherits DummyType
        End Class

        Public Class DummyType
            Public Property ID() As Integer
                Get
                    Return m_ID
                End Get
                Set(ByVal value As Integer)
                    m_ID = value
                End Set
            End Property
            Private m_ID As Integer
            Public Property Name() As String
                Get
                    Return m_Name
                End Get
                Set(ByVal value As String)
                    m_Name = value
                End Set
            End Property
            Private m_Name As String
        End Class

#End Region

#Region "Verification methods"
        Private Sub VerifySinglePhoto(ByVal photo As StreamingServicePhotoBase, ByVal ctx As DataServiceContext,
                                      ByVal executionMethod As Util.ExecutionMethod)
            Assert.IsNotNull(ctx.GetReadStreamUri(photo), "Photo is MLE so it must have a read stream URI.")

            Dim entityDescriptor = (From ed In ctx.Entities Where ed.Entity Is photo Select ed).SingleOrDefault()
            Assert.IsNotNull(entityDescriptor, "The Photo entity must be tracked.")
            Assert.IsNotNull(entityDescriptor.ReadStreamUri, "Photo is MLE so it must have a read stream URI in entity descriptor.")
            Assert.IsNotNull(entityDescriptor.EditStreamUri, "Photo is MLE so it must heav an edit stream URI.")

            Dim response = Util.GetReadStream(ctx, photo, New DataServiceRequestArgs(), executionMethod)
            Assert.IsNotNull(response)
            VerifyPhotoContent(photo, response)
        End Sub

        Private Sub VerifyPhotoContent(ByVal photo As StreamingServicePhotoBase, ByVal response As DataServiceStreamResponse)
            Assert.AreEqual("type/" + photo.ID.ToString(), response.ContentType)
            Dim stream = response.Stream
            Dim b = New Byte(1) {}
            For i = 1 To photo.Length
                Assert.AreEqual(1, stream.Read(b, 0, 1), "Stream didn't read the expected amount of data")
                Assert.AreEqual(CType(photo.ID * 10 + i, Byte), b(0), "The content of the stream is not what was expected")
            Next
        End Sub

        Private Sub VerifyStreamContent(ByVal stream As Stream, ByVal content As Byte())
            Dim b = New Byte(1) {}
            Dim i = 0
            While stream.Read(b, 0, 1) > 0
                Assert.IsTrue(i < content.Length, "The stream contains more bytes than expected.")
                Assert.AreEqual(b(0), content(i), "The stream doesn't contain expected value.")
                i = i + 1
            End While
            Assert.IsTrue(i = content.Length, "The stream contains less bytes than expected.")
        End Sub

        Private Class ExpectedResult
            Private _statusCode As HttpStatusCode
            Private _ETag As String

            Public Sub New(ByVal statusCode As HttpStatusCode, ByVal ETagValue As String)
                _statusCode = statusCode
                _ETag = ETagValue
            End Sub

            Public Property StatusCode() As HttpStatusCode
                Get
                    StatusCode = _statusCode
                End Get
                Set(ByVal value As HttpStatusCode)
                    _statusCode = value
                End Set
            End Property

            Public Property ETag() As String
                Get
                    ETag = _ETag
                End Get
                Set(ByVal value As String)
                    _ETag = value
                End Set
            End Property
        End Class

        Private Sub VerifyBatchOperationResponses(ByVal dataServiceResponse As DataServiceResponse, ByVal expectedResults As ExpectedResult())
            Dim resultIdx As Integer = 0
            For Each operationResponse As OperationResponse In dataServiceResponse
                If expectedResults.Count < resultIdx + 1 Then
                    Assert.Fail("The number of expected results does not match the number of actual results.")
                End If
                If Not (expectedResults(resultIdx) Is Nothing) Then
                    Dim result As ExpectedResult = expectedResults(resultIdx)
                    Assert.AreEqual(CType(result.StatusCode, Integer), operationResponse.StatusCode)
                    Assert.AreEqual("4.0;", operationResponse.Headers("OData-Version"))
                    If Not (result.ETag Is Nothing) Then
                        Assert.AreEqual(result.ETag, operationResponse.Headers("ETag"))
                    End If
                End If
                resultIdx = resultIdx + 1
            Next
            If expectedResults.Count <> resultIdx Then
                Assert.Fail("The number of expected results does not match the number of actual results.")
            End If
        End Sub
#End Region

#Region "Headers support"
        Private Sub ResetRequestHeaders()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            ctx.Execute(Of Boolean)(New Uri("/ResetRequestHeaders", UriKind.Relative))
        End Sub

        Private Function GetRequestHeaders() As Dictionary(Of String, WebHeaderCollection)
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Dim result = New Dictionary(Of String, WebHeaderCollection)

            Dim headers = ctx.Execute(Of StreamingServiceRequestHeaders)(New Uri("/RequestHeaders", UriKind.Relative))
            For Each header In headers
                Dim uri = header.Uri
                If uri.StartsWith(web.ServiceRoot.ToString()) Then
                    uri = uri.Substring(web.ServiceRoot.ToString().Length)
                End If
                If Not uri = "/RequestHeaders" Then
                    Dim response = Util.GetReadStream(ctx, header, New DataServiceRequestArgs(), Util.ExecutionMethod.Synchronous)
                    Dim parsedHeaders = ParseHeadersResponse(response.Stream)
                    result.Add(uri, parsedHeaders)
                End If
            Next

            Return result
        End Function

        Private Function ParseHeadersResponse(ByVal response As Stream) As WebHeaderCollection
            Dim doc As XDocument
            Using reader As XmlReader = XmlReader.Create(response)
                doc = XDocument.Load(reader)
            End Using

            Dim result = New WebHeaderCollection()

            For Each header In doc...<header>
                For Each value In header...<value>
                    result.Add(header.@name, value.Value)
                Next
            Next
            Return result
        End Function
#End Region

        Private Sub ResetServiceContent()
            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            ctx.Execute(Of Boolean)(New Uri("/ResetContent", UriKind.Relative)).Count()
        End Sub
    End Class

#Region "TestingStream"
    Public Class TestingStream
        Inherits Stream

        Private stream As Stream

        Public Sub New(ByVal stream As Stream)
            Me.stream = stream
        End Sub

        Public OnlyRead As Boolean
        Public NoSeek As Boolean
        Public DisposeCalled As Boolean
        Public CloseCalled As Boolean

        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return stream.CanRead
            End Get
        End Property

        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                If NoSeek Then
                    Return False
                End If
                Return stream.CanSeek
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                If OnlyRead Then
                    Return False
                End If
                Return stream.CanWrite
            End Get
        End Property

        Public Overrides Sub Flush()
            If OnlyRead Then
                Return
            End If
            stream.Flush()
        End Sub

        Public Overrides ReadOnly Property Length() As Long
            Get
                If NoSeek Then
                    Assert.Fail("Length called on non-seekable stream.")
                End If
                Return stream.Length
            End Get
        End Property

        Public Overrides Property Position() As Long
            Get
                Return stream.Position
            End Get
            Set(ByVal value As Long)
                If NoSeek Then
                    Assert.Fail("Set Position called on non-seekable stream.")
                End If
                stream.Position = value
            End Set
        End Property

        Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
            Return stream.Read(buffer, offset, count)
        End Function

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
            If NoSeek Then
                Assert.Fail("Seek called on non-seekable stream.")
            End If
            Return stream.Seek(offset, origin)
        End Function

        Public Overrides Sub SetLength(ByVal value As Long)
            If NoSeek Or OnlyRead Then
                Assert.Fail("SetLength called on read-only or non-seekable stream.")
            End If
            stream.SetLength(value)
        End Sub

        Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
            If OnlyRead Then
                Assert.Fail("Write called on read-only stream.")
            End If
            stream.Write(buffer, offset, count)
        End Sub

        Public Overrides Sub Close()
            CloseCalled = True
            stream.Close()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            DisposeCalled = True
            MyBase.Dispose(disposing)
            stream = Nothing
        End Sub
    End Class
#End Region

End Class
