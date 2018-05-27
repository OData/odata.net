'---------------------------------------------------------------------
' <copyright file="CustomRequests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.Text
Imports System.Web
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NorthwindModel

Partial Public Class ClientModule
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    'Remove Atom
    ' <TestClass()>
    Public Class CustomRequestsTest
        Inherits AstoriaTestCase

        Private Shared web As LocalWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = CType(TestWebRequest.CreateForLocal, LocalWebRequest)
            web.ServiceType = GetType(AstoriaUnitTests.Stubs.RequestInfoService)
            web.SetupLocalWeb()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.ShutdownLocalWeb()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Me.ctx = New DataServiceContext(web.ServiceRoot)
            ''Me.'ctx.EnableAtom = True
            ''Me.'ctx.Format.UseAtom()
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub CustomizeHeaders()
            AddHandler ctx.SendingRequest2, AddressOf SendingRequestHandler_Customize

            Dim q = ctx.CreateQuery(Of HeaderAsEntity)("/HttpHeaders")

            Dim e = (From h In q.AsEnumerable()
                     Where h.ID = "CustomTestHeader" And h.Value = "CustomTestHeaderData"
                     Select h).Count()

            Assert.IsTrue(1 = e)

            ' try a custom header for the update path, in case there is a difference in 
            ' the way requests are setup

            ctx.MergeOption = MergeOption.OverwriteChanges
            Dim header As New HeaderAsEntity With {.ID = "CustomHeaderInsertTest", .Value = Nothing}
            ctx.AddObject("HttpHeaders", header)
            Assert.AreEqual(Nothing, header.Value)
            Util.SaveChanges(ctx)
            Assert.AreEqual("CustomTestHeaderData", header.Value)
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub VersionForServiceOps()
            Dim q = ctx.CreateQuery(Of SimpleEntity)("/EchoHeaders")
            Dim e As SimpleEntity = q.AsEnumerable().First()
            TestUtil.AssertContains(e.ID, "OData-Version: 4.0")
        End Sub

        Private Sub SendingRequestHandler_Customize(ByVal sender As Object, ByVal e As SendingRequest2EventArgs)
            e.RequestMessage.SetHeader("CustomTestHeader", "CustomTestHeaderData")
        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub VerifyETag_IsMatch()
            Using CustomDataContext.CreateChangeScope()
                Using web As TestWebRequest = TestWebRequest.CreateForInProcessWcf
                    web.DataServiceType = ServiceModelData.CustomData.ServiceModelType
                    web.StartService()

                    For Each saveoption As SaveChangesOptions In New SaveChangesOptions() {SaveChangesOptions.None, SaveChangesOptions.ContinueOnError, SaveChangesOptions.BatchWithSingleChangeset}
                        ctx = New DataServiceContext(web.ServiceRoot)
                        ''ctx.EnableAtom = True
                        ''ctx.Format.UseAtom()
                        ctx.ResolveName = AddressOf ResolveName
                        Dim customer = ctx.CreateQuery(Of Customer)("Customers").Execute().Last()
                        ctx.UpdateObject(customer)

                        AddHandler ctx.SendingRequest2, AddressOf VerifyETag_Exists
                        Util.SaveChanges(ctx, saveoption)

                        ctx.UpdateObject(customer)

                        AddHandler ctx.SendingRequest2, AddressOf ModifyETag
                        Try
                            Util.SaveChanges(ctx, saveoption) 'changing the ETag will cause it to fail with SendingRequest2 event
                            Assert.Fail("expected DataServiceRequestException")
                        Catch ex As DataServiceRequestException
                        End Try
                    Next
                End Using
            End Using
        End Sub

        Private Shared Function ResolveName(ByVal type As System.Type) As String
            Return type.FullName
        End Function

        Private Sub VerifyETag_Exists(ByVal sender As Object, ByVal args As SendingRequest2EventArgs)
            Assert.AreEqual(ctx, sender, "sender")
            If Not args.RequestMessage.Url.AbsoluteUri.Contains("$batch") Then
                Assert.IsNotNull(args.RequestMessage.GetHeader("If-Match"), "etag must be set before this event is raised")
            End If
        End Sub

        Private Sub ModifyETag(ByVal sender As Object, ByVal args As SendingRequest2EventArgs)
            args.RequestMessage.SetHeader("If-Match", "32")
        End Sub

    End Class

    Public Class BadWebRequest
        Inherits System.Net.WebRequest

    End Class
End Class
