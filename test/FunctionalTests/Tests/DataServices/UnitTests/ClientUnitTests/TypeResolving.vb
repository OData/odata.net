'---------------------------------------------------------------------
' <copyright file="TypeResolving.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Net
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml
Imports System.Xml.Linq
Imports AstoriaUnitTests.ClientExtensions
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports AstoriaUnitTests.Tests
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NorthwindModel

Partial Public Class ClientModule
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    'Remove Atom
    ' <TestClass()>
    Public Class TypeResolving

#Region "ClassInitialize, ClassCleanup, TestInitialize, TestCleanup"
        Private Shared web As TestWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing

        Friend Const ServerSleepInSeconds As Int32 = 15
        Friend Const RequestAbortedMessage As String = "The request was aborted: The request was canceled."

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.ServiceType = GetType(Client.TypeResolveService)
            web.StartService()

            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.StopService()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Client.TypeResolveContext.ResetData()
            Me.ctx = New DataServiceContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub

        Private ReadOnly Property ClientList_AllTypes() As List(Of Client.VBAllType)
            Get
                Return Client.TypeResolveContext.clientAllTypes
            End Get
        End Property

        Private ReadOnly Property ClientList_SimpleTypes() As List(Of Client.VBSimpleType)
            Get
                Return Client.TypeResolveContext.clientSimpleTypes
            End Get
        End Property

        Private ReadOnly Property ServerList_AllTypes() As List(Of Client.VBAllType)
            Get
                Return Client.TypeResolveContext.globalData.allTypes
            End Get
        End Property
#End Region

#Region "MatchNamespace_MatchName"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub MatchNamespace_MatchName()
            Dim p = ClientList_AllTypes.Take(1)
            Dim q = ctx.Execute(Of Client.VBAllType)(New Uri("ListAllType?$top=1", UriKind.Relative))
            ValidateEqual(p, q, 1, AddressOf MatchNamespace_MatchName_Validate1)

            p = ClientList_AllTypes.Take(1)
            q = ctx.Execute(Of Client.VBAllType)(New Uri("ListAllType?$top=1&$expand=PClass", UriKind.Relative))
            ValidateEqual(p, q, 1, AddressOf MatchNamespace_MatchName_Validate1)

            ' TODO: can't include RPClass materializer attempts to set readonly property
            ctx.MergeOption = MergeOption.OverwriteChanges
            p = ClientList_AllTypes.Take(1)
            q = ctx.Execute(Of Client.VBAllType)(New Uri("ListAllType?$top=1&$expand=PClass", UriKind.Relative))
            ValidateEqual(p, q, 1, AddressOf MatchNamespace_MatchName_Validate2)

            Dim r = ClientList_SimpleTypes
            Dim s = ctx.Execute(Of Client.VBSimpleType)(New Uri("ListSimpleType", UriKind.Relative))
            ValidateEqual(r, s, 2, AddressOf MatchNamespace_MatchName_Validate)
        End Sub

        Private Overloads Sub MatchNamespace_MatchName_Validate1(ByVal obj As Client.VBAllType)
            Assert.IsInstanceOfType(obj, GetType(Client.VBAllType))
            Assert.IsNull(CType(obj, Client.VBAllType).PClass)
        End Sub

        Private Overloads Sub MatchNamespace_MatchName_Validate2(ByVal obj As Client.VBAllType)
            Assert.IsInstanceOfType(obj, GetType(Client.VBAllType))
            Assert.IsNotNull(CType(obj, Client.VBAllType).PClass)
        End Sub

        Private Overloads Sub MatchNamespace_MatchName_Validate(ByVal obj As Client.VBSimpleType)
            Assert.IsInstanceOfType(obj, GetType(Client.VBSimpleType))
        End Sub
#End Region

#Region "MatchNamespace_MatchName_WithSubtype"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub MatchNamespace_MatchName_WithSubtype()
            Client.TypeResolveContext.AddSubTypes()
            ctx.MergeOption = MergeOption.OverwriteChanges

            Dim r = CType(ClientList_SimpleTypes, IEnumerable(Of Client.VBSimpleType))
            Dim s = ctx.Execute(Of Client.VBSimpleType)(New Uri("ListSimpleType", UriKind.Relative))
            ValidateEqual(r, s, 4, AddressOf MatchNamespace_MatchName_Validate)
            Assert.AreEqual(4, ctx.Entities.Count)

            ' match existing items in context
            r = From k In ClientList_SimpleTypes Order By k.ID Descending Select k
            s = From k In ctx.CreateQuery(Of Client.VBSimpleType)("ListSimpleType") Order By k.ID Descending Select k
            ValidateEqual(r, s, 4, AddressOf MatchNamespace_MatchName_Validate)
            Assert.AreEqual(4, ctx.Entities.Count)
        End Sub
#End Region

#Region "MatchNamespace_DiffName"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub MatchNamespace_DiffName()
            ctx.MergeOption = MergeOption.OverwriteChanges
            Dim r As IEnumerable(Of Client.VBSimpleType) = ClientList_SimpleTypes
            Dim s = ctx.Execute(Of Client.VBSimpleType2)(New Uri("ListSimpleType", UriKind.Relative)).Cast(Of Client.VBSimpleType)()
            ValidateEqual(r, s, 2, AddressOf MatchNamespace_DiffName_Validate)
            Assert.AreEqual(2, ctx.Entities.Count)

            ' match existing items in context
            r = From k In ClientList_SimpleTypes Order By k.ID Descending Select k
            s = (From k In ctx.CreateQuery(Of Client.VBSimpleType2)("ListSimpleType") Order By k.ID Descending Select k).Cast(Of Client.VBSimpleType)()
            ValidateEqual(r, s, 2, AddressOf MatchNamespace_DiffName_Validate)
            Assert.AreEqual(2, ctx.Entities.Count)
        End Sub

        Private Sub MatchNamespace_DiffName_Validate(ByVal obj As Client.VBSimpleType)
            Assert.IsInstanceOfType(obj, GetType(Client.VBSimpleType2))
            Assert.AreEqual(0L, CType(obj, Client.VBSimpleType2).PInt64)
        End Sub
#End Region

#Region "DiffNamespace_DiffName"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub DiffNamespace_DiffName()
            Dim a As New OtherClient.VBSimpleType_Foo()
            a.ID = 5
            a.PInt32 = 2

            Dim r = New List(Of OtherClient.VBSimpleType_Foo)
            r.Add(a)

            Dim s = ctx.Execute(Of OtherClient.VBSimpleType_Foo)(New Uri("ListSimpleType?$skip=1&$top=1", UriKind.Relative))
            ValidateEqual(r, s, 1, AddressOf DiffNamespace_DiffName_Validate)
        End Sub

        Private Sub DiffNamespace_DiffName_Validate(ByVal obj As OtherClient.VBSimpleType_Foo)
            Assert.IsInstanceOfType(obj, GetType(OtherClient.VBSimpleType_Foo))
        End Sub
#End Region

#Region "DiffNamespace_DiffName_Object"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub DiffNamespace_DiffName_Object()
            ' this is ambiguous because of the code generation at in the class setup
            Dim p = ClientList_AllTypes
            Try
                Dim q = ctx.Execute(Of Object)(New Uri("ListAllType", UriKind.Relative)).ToArray()
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("ambiguous"), "{0}", ex)
            End Try

            Dim r = ClientList_SimpleTypes
            Try
                Dim s = ctx.Execute(Of Object)(New Uri("ListSimpleType", UriKind.Relative)).ToArray()
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("ambiguous"), "{0}", ex)
            End Try
        End Sub
#End Region

#Region "DiffNamespace_DiffName_Other"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub DiffNamespace_DiffName_Object_Ambiguous()
            Dim p = ctx.BeginExecute(Of OtherClient.VBAllType)(New Uri("ListAllType", UriKind.Relative), Nothing, Nothing)
            Dim pp = ctx.EndExecute(Of OtherClient.VBAllType)(p).ToArray()
            Assert.AreEqual(2, pp.Length)

            Dim q = ctx.BeginExecute(Of OtherClient.VBSimpleType_Foo)(New Uri("ListSimpleType", UriKind.Relative), Nothing, Nothing)
            Dim qq = ctx.EndExecute(Of OtherClient.VBSimpleType_Foo)(q).ToArray()
            Assert.AreEqual(2, qq.Length)
        End Sub
#End Region

#Region "DiffNamespace_DiffName_ResolveToSubType"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub DiffNamespace_DiffName_ResolveToSubType()
            ctx.ResolveType = AddressOf DiffNamespace_DiffName_ResolveToSubType_Resolver

            Dim a As New OtherClient.VBSimpleType_Foo()
            a.ID = 5
            a.PInt32 = 2

            Dim r = New List(Of OtherClient.VBSimpleType_Foo)
            r.Add(a)

            Dim s = ctx.Execute(Of OtherClient.VBSimpleType_Foo)(New Uri("ListSimpleType?$skip=1&$top=1", UriKind.Relative))
            ValidateEqual(r, s, 1, AddressOf DiffNamespace_DiffName_ResolveToSubType_Validate)
        End Sub

        Private Function DiffNamespace_DiffName_ResolveToSubType_Resolver(ByVal name As String) As Type
            Assert.AreEqual("AstoriaClientUnitTests.Client.VBSimpleType", name)
            Return GetType(OtherClient.VBSimpleType_Foo2)
        End Function

        Private Sub DiffNamespace_DiffName_ResolveToSubType_Validate(ByVal obj As OtherClient.VBSimpleType_Foo)
            Assert.IsInstanceOfType(obj, GetType(OtherClient.VBSimpleType_Foo2))
        End Sub
#End Region

#Region "DiffNamespace_DiffName_Object_ResolveToSubType"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub DiffNamespace_DiffName_Object_ResolveToSubType()
            ctx.ResolveType = AddressOf DiffNamespace_DiffName_Object_ResolveToSubType_Resolver
            Dim s = ctx.Execute(Of Object)(New Uri("ListSimpleType", UriKind.Relative))
            Dim a = s.Cast(Of OtherClient.VBSimpleType_Foo)().ToArray()
            Assert.AreEqual(2, a.Length)
        End Sub

        Private Function DiffNamespace_DiffName_Object_ResolveToSubType_Resolver(ByVal name As String) As Type
            Assert.AreEqual("AstoriaClientUnitTests.Client.VBSimpleType", name)
            Return GetType(OtherClient.VBSimpleType_Foo)
        End Function
#End Region

#Region "DiffNamespace_DiffName_ResolveToBadSubType"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub DiffNamespace_DiffName_ResolveToBadSubType()
            ctx.ResolveType = AddressOf DiffNamespace_DiffName_ResolveToBadSubType_Resolver

            Try
                Dim s = ctx.Execute(Of OtherClient.VBSimpleType_Foo)(New Uri("ListSimpleType?$skip=1&$top=1", UriKind.Relative)).Count()
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("type is not compatible with the expected"))
            End Try
        End Sub

        Private Function DiffNamespace_DiffName_ResolveToBadSubType_Resolver(ByVal name As String) As Type
            Assert.AreEqual("AstoriaClientUnitTests.Client.VBSimpleType", name)
            Return GetType(Client.VBSimpleType)
        End Function
#End Region

#Region "validate etag update"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub ValidateETagUpdate()
            Dim sa = ServerList_AllTypes.First()
            Dim ca = ctx.CreateQuery(Of OtherClient.VBAllType)("ListAllType").First()

            Dim entities = ctx.Entities
            Assert.AreEqual(1, entities.Count)
            Assert.AreEqual(sa.VBAllTypeID, ca.VBAllTypeID)
            Assert.AreEqual(sa.PBinary, ca.PBinary)

            Dim g = ca.PBinary
            sa.PBinary = New Guid().ToByteArray()
            Trace.WriteLine("Original server GUID: " & g.ToString())
            Trace.WriteLine("Updated server GUID:  " & sa.PBinary.ToString())

            ctx.MergeOption = MergeOption.AppendOnly
            ValidateETagUpdate(ca, entities.First().ETag, g, True, True, True, True)

            ctx.MergeOption = MergeOption.OverwriteChanges
            ValidateETagUpdate(ca, entities.First().ETag, g, False, False, False, False)

            ctx.MergeOption = MergeOption.PreserveChanges
            ValidateETagUpdate(ca, entities.First().ETag, g, False, False, True, False)
        End Sub

        Private Sub ValidateETagUpdate(ByVal ca As OtherClient.VBAllType, ByVal etag As String,
                                       ByVal g As System.Data.Linq.Binary,
                                       ByVal unchangedClientsMatchServer As Boolean,
                                       ByVal emodified As Boolean, ByVal binary As Boolean,
                                       ByVal deletedClientsMatchServer As Boolean)
            Assert.IsTrue(ctx.Detach(ca))
            ca.PBinary = g
            ctx.AttachTo("ListAllType", ca, etag)
            Assert.AreSame(ca, ctx.CreateQuery(Of OtherClient.VBAllType)("ListAllType").First())
            Assert.AreEqual(unchangedClientsMatchServer, (etag = ctx.Entities.First().ETag), "unchanged")
            Assert.AreEqual(unchangedClientsMatchServer, (g = ca.PBinary), "unchanged binary")

            Assert.IsTrue(ctx.Detach(ca))
            ca.PBinary = g
            ctx.AttachTo("ListAllType", ca, etag)
            ctx.UpdateObject(ca)
            Assert.AreSame(ca, ctx.CreateQuery(Of OtherClient.VBAllType)("ListAllType").First())
            Assert.AreEqual(emodified, (etag = ctx.Entities.First().ETag), "modified")
            Assert.AreEqual(binary, (g = ca.PBinary), "modified binary")

            Assert.IsTrue(ctx.Detach(ca))
            ca.PBinary = g
            ctx.AttachTo("ListAllType", ca, etag)
            ctx.DeleteObject(ca)
            Assert.AreSame(ca, ctx.CreateQuery(Of OtherClient.VBAllType)("ListAllType").First())
            Assert.AreEqual(deletedClientsMatchServer, (etag = ctx.Entities.First().ETag), "deleted")
            Assert.AreEqual(deletedClientsMatchServer, (g = ca.PBinary), "deleted binary")
        End Sub
#End Region

#Region "LoadProperties"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LoadProperties()
            Dim q = ClientList_AllTypes.First()
            Dim p As New OtherClient.VBAllType
            p.VBAllTypeID = 1
            ctx.AttachTo("ListAllType", p)

            Dim uri As Uri = Nothing
            ctx.TryGetUri(p, uri)
            Assert.IsTrue(uri.OriginalString.Contains("ListAllType(1)"), "{0}", uri)

            Assert.AreEqual(q.PBinary, ctx.LoadProperty(p, "PBinary").Cast(Of System.Data.Linq.Binary).Single())
            Assert.AreEqual(q.PChar.Single(), ctx.LoadProperty(p, "PChar").Cast(Of Char).Single())
            Assert.AreEqual(q.PInt16, ctx.LoadProperty(p, "PInt16").Cast(Of Int16).Single())
            Assert.AreEqual(q.PInt32, ctx.LoadProperty(p, "PInt32").Cast(Of Int32).Single())
            Assert.AreEqual(q.PInt64, ctx.LoadProperty(p, "PInt64").Cast(Of Int64).Single())
            Assert.AreEqual(q.PDecimal, ctx.LoadProperty(p, "PDecimal").Cast(Of Decimal).Single())

            Assert.AreEqual(q.NPInt16, ctx.LoadProperty(p, "NPInt16").Cast(Of Nullable(Of Int16)).Single())
            Assert.AreEqual(q.NPInt32, ctx.LoadProperty(p, "NPInt32").Cast(Of Nullable(Of Int32)).Single())
        End Sub
#End Region

#Region "expanding"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub ExpandCollections_AppendOnly()
            Me.ExpandCollections_Execute(MergeOption.AppendOnly)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub ExpandCollections_OverwriteChanges()
            Me.ExpandCollections_Execute(MergeOption.OverwriteChanges)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub ExpandCollections_PreserveChanges()
            Me.ExpandCollections_Execute(MergeOption.PreserveChanges)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub ExpandCollections_NoTracking()
            Me.ExpandCollections_Execute(MergeOption.NoTracking)
        End Sub

        Public Sub ExpandCollections_Execute(ByVal merge As MergeOption)
            For itemCount As Int32 = 0 To 3
                Dim instancesToAddToFirstCollections = itemCount - 1
                Dim addToServerAfterFirstRefresh As Boolean = itemCount = 3
                Client.TypeResolveContext.ResetData()

                ctx = New DataServiceContext(ctx.BaseUri)
                'ctx.EnableAtom = True
                'ctx.Format.UseAtom()
                ctx.MergeOption = merge

                For i As Int32 = 0 To instancesToAddToFirstCollections
                    Dim first = ServerList_AllTypes.First()

                    Dim j = New Client.VBSimpleType()
                    j.PInt32 = 333
                    first.PCollection.Add(j)

                    j = New Client.VBSimpleType()
                    j.PInt32 = 333
                    first.NPCollection.Add(j)

                    j = New Client.VBSimpleType()
                    j.PInt32 = 333
                    first.OPCollection.Add(j)
                Next

                Dim uriForFirstExpanded = New Uri("ListAllType?$top=1&$expand=PCollection,NPCollection,OPCollection,PClass", UriKind.Relative)
                Dim materializedFirstElement = ctx.Execute(Of OtherClient.VBAllType)(uriForFirstExpanded).Single()
                Dim entities = ctx.Entities
                Dim links = ctx.Links
                Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

                If merge <> MergeOption.NoTracking Then
                    Assert.AreEqual(3 * itemCount + 2, entities.Count, "entity count")
                    Assert.AreEqual(3 * itemCount + 1, links.Count, "link count")
                    Dim w = descriptors.Where(Function(x) x.State <> EntityStates.Unchanged).ToArray()
                    Assert.AreEqual(0, w.Length, "everything should be unchanged")
                Else
                    Assert.AreEqual(0, entities.Count, "entity count")
                    Assert.AreEqual(0, links.Count, "link count")
                End If

                Dim simples = materializedFirstElement.PCollection.Union(materializedFirstElement.NPCollection).Union(materializedFirstElement.OPCollection).Union(New OtherClient.VBSimpleType_Foo() {materializedFirstElement.PClass})
                Assert.IsFalse(simples.Any(Function(x) x.PInt32 <> 333), "should all be 333")

                Assert.AreEqual(itemCount, materializedFirstElement.PCollection.Count, "PCollection.Count")
                Assert.AreEqual(itemCount, materializedFirstElement.NPCollection.Count, "NPCollection.Count")
                Assert.AreEqual(itemCount, materializedFirstElement.OPCollection.Count, "OPCollection.Count")
                Assert.IsNotNull(materializedFirstElement.PClass, "PClass")

                If addToServerAfterFirstRefresh Then
                    Dim first = ServerList_AllTypes.First()
                    Dim j = New Client.VBSimpleType()
                    j.PInt32 = 666
                    first.PCollection.Add(j)

                    j = New Client.VBSimpleType()
                    j.PInt32 = 666
                    first.NPCollection.Add(j)

                    j = New Client.VBSimpleType()
                    j.PInt32 = 666
                    first.OPCollection.Add(j)
                End If

                If merge <> MergeOption.NoTracking Then
                    Assert.AreEqual(1, links.Where(Function(x) x.SourceProperty = "PClass").Count)
                    Assert.AreEqual(itemCount, links.Where(Function(x) x.SourceProperty = "PCollection").Count)
                    Assert.AreEqual(itemCount, links.Where(Function(x) x.SourceProperty = "NPCollection").Count)
                    Assert.AreEqual(itemCount, links.Where(Function(x) x.SourceProperty = "OPCollection").Count)

                    If 1 = itemCount Then
                        Dim j = New OtherClient.VBSimpleType_Foo()
                        j.PInt32 = 333
                        materializedFirstElement.PCollection.Add(j)

                        j = New OtherClient.VBSimpleType_Foo()
                        j.PInt32 = 333
                        materializedFirstElement.NPCollection.Add(j)

                        j = New OtherClient.VBSimpleType_Foo()
                        j.PInt32 = 333
                        materializedFirstElement.OPCollection.Add(j)

                        ctx.AddObject("ListSimpleType", materializedFirstElement.PCollection.Item(1))
                        ctx.AddObject("ListSimpleType", materializedFirstElement.NPCollection.Item(1))
                        ctx.AddObject("ListSimpleType", materializedFirstElement.OPCollection.Item(1))
                    ElseIf 2 = itemCount Then
                        materializedFirstElement.PCollection.Item(0).PInt32 = 444
                        materializedFirstElement.NPCollection.Item(0).PInt32 = 444
                        materializedFirstElement.OPCollection.Item(0).PInt32 = 444
                        materializedFirstElement.PClass.PInt32 = 444
                        ctx.UpdateObject(materializedFirstElement.PCollection.Item(0))
                        ctx.UpdateObject(materializedFirstElement.NPCollection.Item(0))
                        ctx.UpdateObject(materializedFirstElement.OPCollection.Item(0))
                        ctx.UpdateObject(materializedFirstElement.PClass)
                    ElseIf 3 = itemCount Then
                        materializedFirstElement.PCollection.Item(0).PInt32 = 555
                        materializedFirstElement.NPCollection.Item(0).PInt32 = 555
                        materializedFirstElement.OPCollection.Item(0).PInt32 = 555
                        materializedFirstElement.PClass.PInt32 = 555
                        ctx.DeleteObject(materializedFirstElement.PCollection.Item(0))
                        ctx.DeleteObject(materializedFirstElement.NPCollection.Item(0))
                        ctx.DeleteObject(materializedFirstElement.OPCollection.Item(0))
                        ctx.DeleteObject(materializedFirstElement.PClass)

                        materializedFirstElement.PCollection.Item(1).PInt32 = 444
                        materializedFirstElement.NPCollection.Item(1).PInt32 = 444
                        materializedFirstElement.OPCollection.Item(1).PInt32 = 444
                        ctx.UpdateObject(materializedFirstElement.PCollection.Item(1))
                        ctx.UpdateObject(materializedFirstElement.NPCollection.Item(1))
                        ctx.UpdateObject(materializedFirstElement.OPCollection.Item(1))
                    End If
                End If

                entities = ctx.Entities
                Dim materializedFirstAgain = ctx.Execute(Of OtherClient.VBAllType)(uriForFirstExpanded).Single()
                Assert.AreEqual(merge <> MergeOption.NoTracking, Object.ReferenceEquals(materializedFirstElement, materializedFirstAgain), "primary object change")
                materializedFirstElement = materializedFirstAgain
                Dim e = ctx.Entities

                simples = materializedFirstElement.PCollection.Union(materializedFirstElement.NPCollection).Union(materializedFirstElement.OPCollection).Union(New OtherClient.VBSimpleType_Foo() {materializedFirstElement.PClass})
                Dim g = simples.Where(Function(x) x.PInt32 <> 333).ToArray()
                Dim h = simples.Where(Function(x) x.PInt32 <> 666).ToArray()
                Dim hh = simples.Where(Function(x) x.PInt32 = 666).ToArray()

                If merge = MergeOption.OverwriteChanges Then
                    Dim f = e.Cast(Of Descriptor).Union(links.Cast(Of Descriptor)).Where(Function(x) x.State <> EntityStates.Unchanged And x.State <> EntityStates.Added).ToArray()
                    Assert.AreEqual(0, f.Count, "entities not unchanged or added")

                    If (addToServerAfterFirstRefresh) Then
                        Assert.AreEqual(3 * itemCount + 1, h.Count, "existing count")
                        Assert.AreEqual(3, g.Count, "should have 3 new ones")
                    Else
                        Assert.AreEqual(3 * itemCount + 1, h.Count, "existing count")
                        Assert.AreEqual(0, g.Count, "should all be 333")
                    End If
                ElseIf merge = MergeOption.PreserveChanges Then
                    Dim f = e.Where(Function(x) x.State <> (From y In entities Where y.Entity Is x.Entity Select y.State).SingleOrDefault).ToArray()
                    If (addToServerAfterFirstRefresh) Then
                        Assert.AreEqual(3, f.Count, "entity state changed")
                        Assert.IsFalse(f.Any(Function(x) DirectCast(x.Entity, OtherClient.VBSimpleType_Foo).PInt32 <> 666), "expecting only server added")
                        Assert.AreEqual(6, g.Count)
                    Else
                        Assert.AreEqual(0, f.Count, "entity state changed")
                        If (2 = itemCount) Then
                            Assert.AreEqual(4, g.Count)
                        Else
                            Assert.AreEqual(0, g.Count)
                        End If
                    End If
                ElseIf merge = MergeOption.AppendOnly Then
                    Dim f = e.Where(Function(x) x.State <> (From y In entities Where y.Entity Is x.Entity Select y.State).SingleOrDefault).ToArray()
                    Assert.AreEqual(0, hh.Count, "should not have new ones")
                    If (addToServerAfterFirstRefresh) Then
                        Assert.AreEqual(3, f.Count, "entity state changed")
                    Else
                        Assert.AreEqual(0, f.Count, "entity state changed")
                    End If

                    Dim expectedAddedState As Int32 = 0
                    If (1 = itemCount) Then
                        expectedAddedState = 3
                    End If
                    Assert.AreEqual(expectedAddedState, e.Where(Function(x) x.State = EntityStates.Added).Count, "wrong expected added state")

                    For Each s In simples
                        Dim ss = s
                        Dim t = (From ee In e Where ee.Entity Is ss Select ee).Single()
                        If (t.State = EntityStates.Added Or t.State = EntityStates.Unchanged) Then
                            Assert.IsTrue(333 = ss.PInt32 Or 666 = ss.PInt32, "{0}", ss.ID)
                        ElseIf t.State = EntityStates.Modified Then
                            Assert.AreEqual(444, ss.PInt32, "{0}", ss.ID)
                        ElseIf t.State = EntityStates.Deleted Then
                            Assert.AreEqual(555, ss.PInt32, "{0}", ss.ID)
                        End If
                    Next

                Else
                    Assert.AreEqual(0, e.Count)

                    Assert.AreEqual(3 * itemCount + 1, h.Count, "existing count")
                    If (3 = itemCount) Then
                        Assert.AreEqual(3, g.Count, "should have 3 new ones")
                    Else
                        Assert.AreEqual(0, g.Count, "should all be 333")
                    End If
                End If
            Next
        End Sub
#End Region

#Region "links exist on client and server"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksExistClientAndServer_AppendOnly()
            Dim a = LinksExistClientAndServer(MergeOption.AppendOnly)

            Dim entities = ctx.Entities
            Dim links = ctx.Links

            Assert.AreEqual(3, a.PCollection.Count, "PCollection")
            Assert.AreEqual(3, a.NPCollection.Count, "NPCollection")
            Assert.AreEqual(3, a.OPCollection.Count, "OPCollection")
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNull(a.PNClass, "PNClass")

            Assert.AreEqual(11, entities.Count)
            Assert.AreEqual(8, links.Count)

            'verify expected no change in client state
            Assert.IsFalse(entities.Any(Function(x) x.State <> EntityStates.Unchanged), "entity state shouldn't change")

            Dim pl = links.Where(Function(x) x.State = EntityStates.Added And x.SourceProperty = "PCollection").ToArray()
            Dim nl = links.Where(Function(x) x.State = EntityStates.Deleted And x.SourceProperty = "NPCollection").ToArray()
            Dim ol = links.Where(Function(x) x.State = EntityStates.Unchanged And x.SourceProperty = "OPCollection").ToArray()
            Dim ql = links.Where(Function(x) x.State = EntityStates.Modified And x.SourceProperty = "PClass").Single()
            Dim rl = links.Where(Function(x) x.State = EntityStates.Modified And x.SourceProperty = "PNClass").Single()

            Assert.AreEqual(3, pl.Length, "PCollection")
            Assert.AreEqual(3, nl.Length, "NPCollection")
            Assert.AreEqual(0, ol.Length, "OPCollection")
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksExistClientAndServer_OverwriteChanges()
            Dim a = LinksExistClientAndServer(MergeOption.OverwriteChanges)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            ' verify stuff in the collection/ref
            Assert.AreEqual(3, a.PCollection.Count, "PCollection")
            Assert.AreEqual(3, a.NPCollection.Count, "NPCollection")
            Assert.AreEqual(3, a.OPCollection.Count, "OPCollection")
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNull(a.PNClass, "PNClass")

            Assert.AreEqual(11, entities.Count)
            Assert.AreEqual(11, links.Count)

            Dim states = descriptors.Where(Function(x) x.State <> EntityStates.Unchanged).ToArray()
            Assert.AreEqual(0, states.Length, "modified descriptor")
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksExistClientAndServer_PreserveChanges()
            Dim a = LinksExistClientAndServer(MergeOption.PreserveChanges)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            'verify expected limited state change
            ' verify stuff in the collection/ref
            Assert.AreEqual(3, a.PCollection.Count, "PCollection")
            Assert.AreEqual(3, a.NPCollection.Count, "NPCollection")
            Assert.AreEqual(3, a.OPCollection.Count, "OPCollection")
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNull(a.PNClass, "PNClass")

            Assert.AreEqual(11, entities.Count)
            Assert.AreEqual(11, links.Count)

            Assert.IsFalse(entities.Any(Function(x) x.State <> EntityStates.Unchanged), "entity state shouldn't change")

            Dim pl = links.Where(Function(x) x.State = EntityStates.Unchanged And x.SourceProperty = "PCollection").ToArray()
            Dim nl = links.Where(Function(x) x.State = EntityStates.Deleted And x.SourceProperty = "NPCollection").ToArray()
            Dim ol = links.Where(Function(x) x.State = EntityStates.Unchanged And x.SourceProperty = "OPCollection").ToArray()
            Dim ql = links.Where(Function(x) x.State = EntityStates.Unchanged And x.SourceProperty = "PClass").Single()
            Dim rl = links.Where(Function(x) x.State = EntityStates.Modified And x.SourceProperty = "PNClass").Single()

            Assert.AreEqual(3, pl.Length, "PCollection")
            Assert.AreEqual(3, nl.Length, "NPCollection")
            Assert.AreEqual(3, ol.Length, "OPCollection")

        End Sub

        Private Function LinksExistClientAndServer(ByVal merge As MergeOption) As OtherClient.VBAllType

            ' have stuff in the collection
            For i As Int32 = 1 To 3
                Dim f = ServerList_AllTypes.First()

                Dim j = New Client.VBSimpleType()
                j.PInt32 = 333
                f.PCollection.Add(j)

                j = New Client.VBSimpleType()
                j.PInt32 = 333
                f.NPCollection.Add(j)

                j = New Client.VBSimpleType()
                j.PInt32 = 333
                f.OPCollection.Add(j)
            Next

            ' populate initial data
            Dim uri = New Uri("ListAllType?$top=1&$expand=PCollection,NPCollection,OPCollection,PClass,PNClass", UriKind.Relative)
            Dim a = ctx.Execute(Of OtherClient.VBAllType)(uri).Single()

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            ' verify stuff in the collection/ref
            Assert.AreEqual(3, a.PCollection.Count, "PCollection")
            Assert.AreEqual(3, a.NPCollection.Count, "NPCollection")
            Assert.AreEqual(3, a.OPCollection.Count, "OPCollection")
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNull(a.PNClass, "PNClass")

            Assert.AreEqual(11, entities.Count)
            Assert.AreEqual(11, links.Count)
            Assert.IsFalse(descriptors.Any(Function(x) x.State <> EntityStates.Unchanged), "modified descriptor")

            ' modify client link states
            Assert.AreNotEqual(0, links.Count)
            For Each link In links
                If link.SourceProperty = "PCollection" Then
                    ctx.DetachLink(link.Source, link.SourceProperty, link.Target)
                    ctx.AddLink(link.Source, link.SourceProperty, link.Target)      ' collection w/ added links
                ElseIf link.SourceProperty = "NPCollection" Then
                    ctx.DetachLink(link.Source, link.SourceProperty, link.Target)
                    ctx.DeleteLink(link.Source, link.SourceProperty, link.Target)   ' collection w/ deleted links
                ElseIf link.SourceProperty = "OPCollection" Then
                    ctx.DetachLink(link.Source, link.SourceProperty, link.Target)   ' collection w/ no links
                ElseIf link.SourceProperty = "PClass" Then
                    Assert.IsNotNull(link.Target)
                    ctx.DetachLink(link.Source, link.SourceProperty, link.Target)
                    ctx.SetLink(link.Source, link.SourceProperty, link.Target)      ' reference modified with non-null
                ElseIf link.SourceProperty = "PNClass" Then
                    Assert.IsNull(link.Target)
                    ctx.DetachLink(link.Source, link.SourceProperty, link.Target)
                    ctx.SetLink(link.Source, link.SourceProperty, link.Target)      ' reference modified with null
                End If
            Next

            entities = ctx.Entities
            Dim minks = ctx.Links.ToArray()
            Assert.AreEqual(11, entities.Count)
            Assert.AreEqual(8, minks.Count)

            ' refresh data into client with modified link states
            ctx.MergeOption = merge
            Dim aa = ctx.Execute(Of OtherClient.VBAllType)(uri).Single()
            Assert.AreSame(a, aa)
            Return a
        End Function
#End Region

#Region "link different on client than server, applies only to reference"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksDifferentOnClientThanServer1_AppendOnly()
            Dim a = LinksDifferentOnClientThanServer(MergeOption.AppendOnly, 1)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(4, entities.Count, "entity count")
            Assert.AreEqual(2, links.Count, "link count")
            Assert.IsFalse(descriptors.Any(Function(x) x.State <> EntityStates.Unchanged), "modified descriptor")

            ' expect no change on client
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNull(a.PNClass)

            Assert.AreEqual(2, a.PClass.ID)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksDifferentOnClientThanServer2_AppendOnly()
            Dim a = LinksDifferentOnClientThanServer(MergeOption.AppendOnly, 2)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(6, entities.Count, "entity count")
            Assert.AreEqual(2, links.Count, "link count")
            Assert.IsFalse(entities.Any(Function(x) x.State <> EntityStates.Unchanged), "modified entity")
            Assert.IsFalse(links.Any(Function(x) x.State <> EntityStates.Modified), "not-modified link")

            ' expect no change on client
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNotNull(a.PNClass, "PNClass")

            Assert.AreEqual(32, a.PClass.ID)
            Assert.AreEqual(33, a.PNClass.ID)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksDifferentOnClientThanServer1_OverwriteChanges()
            Dim a = LinksDifferentOnClientThanServer(MergeOption.OverwriteChanges, 1)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(4, entities.Count, "entity count")
            Assert.AreEqual(2, links.Count, "link count")
            Assert.IsFalse(descriptors.Any(Function(x) x.State <> EntityStates.Unchanged), "modified descriptor")

            ' expect no change on client
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNotNull(a.PNClass, "PNClass")

            Assert.AreEqual(6, a.PClass.ID)
            Assert.AreEqual(7, a.PNClass.ID)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksDifferentOnClientThanServer2_OverwriteChanges()
            Dim a = LinksDifferentOnClientThanServer(MergeOption.OverwriteChanges, 2)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(6, entities.Count, "entity count")
            Assert.AreEqual(2, links.Count, "link count")
            Assert.IsFalse(descriptors.Any(Function(x) x.State <> EntityStates.Unchanged), "modified descriptor")

            ' server to win
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNotNull(a.PNClass, "PNClass")

            Assert.AreEqual(6, a.PClass.ID)
            Assert.AreEqual(7, a.PNClass.ID)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksDifferentOnClientThanServer1_PreserveChanges()
            Dim a = LinksDifferentOnClientThanServer(MergeOption.PreserveChanges, 1)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(4, entities.Count, "entity count")
            Assert.AreEqual(2, links.Count, "link count")
            Assert.IsFalse(descriptors.Any(Function(x) x.State <> EntityStates.Unchanged), "modified descriptor")

            ' server to win
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNotNull(a.PNClass, "PNClass")

            Assert.AreEqual(6, a.PClass.ID)
            Assert.AreEqual(7, a.PNClass.ID)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinksDifferentOnClientThanServer2_PreserveChanges()
            Dim a = LinksDifferentOnClientThanServer(MergeOption.PreserveChanges, 2)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(6, entities.Count, "entity count")
            Assert.AreEqual(2, links.Count, "link count")
            Assert.IsFalse(entities.Any(Function(x) x.State <> EntityStates.Unchanged), "modified descriptor")
            Assert.IsFalse(links.Any(Function(x) x.State <> EntityStates.Modified), "modified descriptor")

            ' expect no change on client
            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNotNull(a.PNClass, "PNClass")

            Assert.AreEqual(6, a.PClass.ID)
            Assert.AreEqual(7, a.PNClass.ID)
        End Sub

        Private Function LinksDifferentOnClientThanServer(ByVal merge As MergeOption, ByVal iter As Int32) As OtherClient.VBAllType
            ctx.MergeOption = merge

            Dim uri = New Uri("ListAllType?$top=1&$expand=PClass,PNClass", UriKind.Relative)
            Dim a = ctx.Execute(Of OtherClient.VBAllType)(uri).Single()

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.IsNotNull(a.PClass, "PClass")
            Assert.IsNull(a.PNClass, "PNClass")
            Assert.AreEqual(2, a.PClass.ID)

            Assert.AreEqual(2, entities.Count, "entity count")
            Assert.AreEqual(2, links.Count, "link count")
            Assert.IsFalse(descriptors.Any(Function(x) x.State <> EntityStates.Unchanged), "modified descriptor")


            If iter = 1 Then
                ' leave client unchanged, change server
            ElseIf iter = 2 Then
                ' change client states, change server
                Assert.IsTrue(ctx.DetachLink(a, "PClass", a.PClass))
                a.PClass = New OtherClient.VBSimpleType_Foo(32)
                ctx.AttachTo("Foo", a.PClass)
                ctx.SetLink(a, "PClass", a.PClass)

                Assert.IsTrue(ctx.DetachLink(a, "PNClass", a.PNClass))
                a.PNClass = New OtherClient.VBSimpleType_Foo(33)
                ctx.AttachTo("Foo", a.PNClass)
                ctx.SetLink(a, "PNClass", a.PNClass)
            End If

            Dim f = ServerList_AllTypes.First()
            f.PClass = New Client.VBSimpleType()
            f.PNClass = New Client.VBSimpleType()

            Dim aa = ctx.Execute(Of OtherClient.VBAllType)(uri).Single()
            Assert.AreSame(a, aa)
            Return a
        End Function
#End Region

#Region "link exists on client but not server"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinkExistsOnClientNotServer_AppendOnly()
            Dim a = LinkExistsOnClientNotServer(MergeOption.AppendOnly)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(6, a.PCollection.Count, "PCollection count")
            Assert.AreEqual(7, entities.Count, "entity count")
            Assert.AreEqual(6, links.Count, "link count")
            Assert.IsFalse(entities.Any(Function(x) x.State <> EntityStates.Unchanged), "modified entity")
            Assert.AreEqual(1, links.Where(Function(x) x.State = EntityStates.Added).Count, "added link")
            Assert.AreEqual(1, links.Where(Function(x) x.State = EntityStates.Deleted).Count, "added link")
            Assert.AreEqual(4, links.Where(Function(x) x.State = EntityStates.Unchanged).Count, "added link")
            Assert.AreEqual(6, a.PCollection.Item(0).ID)
            Assert.AreEqual(7, a.PCollection.Item(1).ID)
            Assert.AreEqual(8, a.PCollection.Item(2).ID)
            Assert.AreEqual(55, a.PCollection.Item(3).ID)
            Assert.AreEqual(56, a.PCollection.Item(4).ID)
            Assert.AreEqual(57, a.PCollection.Item(5).ID)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinkExistsOnClientNotServer_OverwriteChanges()
            Dim a = LinkExistsOnClientNotServer(MergeOption.OverwriteChanges)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(3, a.PCollection.Count, "PCollection count")
            Assert.AreEqual(7, entities.Count, "entity count")
            Assert.AreEqual(3, links.Count, "link count")
            Assert.IsFalse(descriptors.Any(Function(x) x.State <> EntityStates.Unchanged), "unchanged link")
            Assert.AreEqual(6, a.PCollection.Item(0).ID)
            Assert.AreEqual(7, a.PCollection.Item(1).ID)
            Assert.AreEqual(8, a.PCollection.Item(2).ID)
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub LinkExistsOnClientNotServer_PreserveChanges()
            Dim a = LinkExistsOnClientNotServer(MergeOption.PreserveChanges)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(5, a.PCollection.Count, "PCollection count")
            Assert.AreEqual(7, entities.Count, "entity count")
            Assert.AreEqual(5, links.Count, "link count")
            Assert.AreEqual(10, descriptors.Where(Function(x) x.State = EntityStates.Unchanged).Count, "unchanged link")
            Assert.AreEqual(1, links.Where(Function(x) x.State = EntityStates.Added).Count, "added link")
            Assert.AreEqual(1, links.Where(Function(x) x.State = EntityStates.Deleted).Count, "deleted link")
            Assert.AreEqual(6, a.PCollection.Item(0).ID)
            Assert.AreEqual(7, a.PCollection.Item(1).ID)
            Assert.AreEqual(8, a.PCollection.Item(2).ID)
            Assert.AreEqual(55, a.PCollection.Item(3).ID)
            Assert.AreEqual(57, a.PCollection.Item(4).ID)
        End Sub

        Private Function LinkExistsOnClientNotServer(ByVal merger As MergeOption) As OtherClient.VBAllType

            For i As Int32 = 1 To 3
                Dim f = ServerList_AllTypes.First()
                Dim j = New Client.VBSimpleType()
                j.PInt32 = 335
                f.PCollection.Add(j)
            Next

            ctx.MergeOption = merger
            Dim q = ctx.CreateQuery(Of OtherClient.VBAllType)("ListAllType").Expand("PCollection")
            Dim a = q.First()

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(4, entities.Count, "entity count")
            Assert.AreEqual(3, links.Count, "link count")
            Assert.IsFalse(descriptors.Any(Function(x) x.State <> EntityStates.Unchanged), "modified descriptor")
            Assert.AreEqual(3, a.PCollection.Count, "PCollection count")

            Dim k = New OtherClient.VBSimpleType_Foo()
            k.ID = 55
            k.PInt32 = 668
            a.PCollection.Add(k)
            ctx.AttachTo("ListSimpleType", k)
            ctx.AddLink(a, "PCollection", k)

            k = New OtherClient.VBSimpleType_Foo()
            k.ID = 56
            k.PInt32 = 668
            a.PCollection.Add(k)
            ctx.AttachTo("ListSimpleType", k)
            ctx.AttachLink(a, "PCollection", k)

            k = New OtherClient.VBSimpleType_Foo()
            k.ID = 57
            k.PInt32 = 668
            a.PCollection.Add(k)
            ctx.AttachTo("ListSimpleType", k)
            ctx.DeleteLink(a, "PCollection", k)

            entities = ctx.Entities
            links = ctx.Links
            descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(6, a.PCollection.Count, "PCollection count")
            Assert.AreEqual(7, entities.Count, "entity count")
            Assert.AreEqual(6, links.Count, "link count")
            Assert.IsFalse(entities.Any(Function(x) x.State <> EntityStates.Unchanged), "modified entity")
            Assert.AreEqual(1, links.Where(Function(x) x.State = EntityStates.Added).Count, "added link")
            Assert.AreEqual(1, links.Where(Function(x) x.State = EntityStates.Deleted).Count, "added link")
            Assert.AreEqual(4, links.Where(Function(x) x.State = EntityStates.Unchanged).Count, "added link")

            Assert.AreSame(a, q.First())
            Return a
        End Function
#End Region

#Region "post-implementation bugs"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub DeleteObject_PreserveChanges()
            For i As Int32 = 1 To 3
                Dim f = ServerList_AllTypes.First()
                Dim j = New Client.VBSimpleType()
                j.PInt32 = 335
                f.PCollection.Add(j)
            Next

            Dim a = ctx.CreateQuery(Of OtherClient.VBAllType)("ListAllType").First()
            ctx.LoadProperty(a, "PCollection")

            ctx.DeleteObject(a)

            Dim entities = ctx.Entities
            Dim links = ctx.Links
            Dim descriptors = entities.Cast(Of Descriptor).Union(links.Cast(Of Descriptor))

            Assert.AreEqual(4, entities.Count, "entity count")
            Assert.AreEqual(3, links.Count, "link count")
            Assert.IsTrue(links.All(Function(x) x.State = EntityStates.Unchanged), "cascaded delete to link")
            Assert.AreEqual(3, a.PCollection.Count, "PCollection count")

            Dim q = ctx.CreateQuery(Of OtherClient.VBAllType)("ListAllType").Expand("PCollection")
            ctx.MergeOption = MergeOption.PreserveChanges

            Assert.AreSame(a, q.First())
        End Sub
#End Region

#Region "verify SaveChanges with unsupported known types"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub SaveChangesWithSupportedKnownTypes()
            Assert.AreEqual(2, ctx.CreateQuery(Of Client.VBSimpleType)("ListSimpleType").Execute().Count())

            Dim a = New Client.VBAllType()
            a.VBAllTypeID = 33
            a.PChar = "A"c ' the default 0 throws System.ArgumentException: '.', hexadecimal value 0x00, is an invalid character..
            ctx.AddObject("ListAllType", a)
            ctx.SetLink(a, "PClass", ctx.Entities.Item(0).Entity)

            Dim wrappingStream As WrappingStream = Nothing
            ctx.RegisterStreamCustomizer(Function(inputStream As Stream)
                                             SaveChangesWithSupportedKnownTypes_WritingEvent_Count = SaveChangesWithSupportedKnownTypes_WritingEvent_Count + 1
                                             wrappingStream = New WrappingStream(inputStream)
                                             Return wrappingStream
                                         End Function,
                                         Nothing)

            Assert.AreEqual(0, SaveChangesWithSupportedKnownTypes_WritingEvent_Count)
            Try
                ctx.SaveChanges(SaveChangesOptions.ContinueOnError)
                Assert.Fail("expected DataServiceException")
            Catch ex As DataServiceRequestException
                Assert.IsTrue(ex.ToString().Contains("The method or operation is not implemented."), "{0}", ex)
            End Try
            SaveChangesWithSupportedKnownTypes_WritingEvent(wrappingStream.GetLoggingStreamAsXDocument())
            Assert.AreEqual(1, SaveChangesWithSupportedKnownTypes_WritingEvent_Count)

            Try
                ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset)
                Assert.Fail("expected WebException")
            Catch ex As DataServiceRequestException
                Dim innerEx As InvalidOperationException = CType(ex.InnerException, InvalidOperationException)
                Assert.IsTrue(innerEx.Message.Contains("NotImplementedException"))
            End Try
            Assert.AreEqual(2, SaveChangesWithSupportedKnownTypes_WritingEvent_Count)
        End Sub

        Private Shared SaveChangesWithSupportedKnownTypes_WritingEvent_Count As Int32
        Private Sub SaveChangesWithSupportedKnownTypes_WritingEvent(ByVal document As XDocument)
            Dim s As New HashSet(Of String)
            Dim m As XNamespace = "http://docs.oasis-open.org/odata/ns/metadata"
            Dim mm As XName = "type"

            For Each n In document.Elements.First.DescendantNodesAndSelf
                Dim a = TryCast(n, XElement)
                If a IsNot Nothing Then
                    Dim type = a.Attribute(m + "type")
                    If type IsNot Nothing Then
                        s.Add(type.Value)
                    End If
                End If
            Next

            Assert.AreEqual(11, s.Count)
            Assert.IsTrue(s.Contains("Binary"), "Binary")
            Assert.IsTrue(s.Contains("Boolean"), "Boolean")
            Assert.IsTrue(s.Contains("Byte"), "Byte")
            Assert.IsTrue(s.Contains("DateTimeOffset"), "DateTimeOffset")
            Assert.IsTrue(s.Contains("Decimal"), "Decimal")
            Assert.IsTrue(s.Contains("Double"), "Double")
            Assert.IsTrue(s.Contains("Guid"), "Guid")
            Assert.IsTrue(s.Contains("Single"), "Single")
            'Assert.IsTrue(s.Contains("Edm.SByte"), "Edm.SByte")
            Assert.IsTrue(s.Contains("Int16"), "Int16")
            Assert.IsTrue(s.Contains("Int32"), "Int32")
            Assert.IsTrue(s.Contains("Int64"), "Int64")
            ' Edm.String is explictly not listed
        End Sub
#End Region

#Region "Test HttpWebRequest Abort"
        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub VerifyAbortBeforeExecute()
            Dim query = ctx.CreateQuery(Of Client.VBSlowType)("SlowType")
            Dim request = CType(System.Net.WebRequest.Create(query.RequestUri), HttpWebRequest)

            request.Abort()

            Dim async As IAsyncResult = Nothing
            Try
                async = request.BeginGetResponse(Nothing, Nothing)
                Assert.Fail("Expected WebException")
            Catch ex As WebException
                Assert.AreEqual(RequestAbortedMessage, ex.Message, "{0}", ex)
            End Try
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub VerifyAbortAfterExecute()
            Dim query = ctx.CreateQuery(Of Client.VBSlowType)("SlowType")
            Dim request = CType(System.Net.WebRequest.Create(query.RequestUri), HttpWebRequest)

            Dim async As IAsyncResult = request.BeginGetResponse(Nothing, Nothing)
            Assert.IsFalse(async.CompletedSynchronously, "CompletedSynchronously")
            Assert.IsFalse(async.IsCompleted, "IsCompleted")
            request.Abort()

            Try
                Dim response = request.EndGetResponse(async)
                Assert.Fail("Expected WebException")
            Catch ex As WebException
                Assert.AreEqual(RequestAbortedMessage, ex.Message, "{0}", ex)
            End Try
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub VerifyAbortAfterGetResponse()
            Dim query = ctx.CreateQuery(Of Client.VBAllType)("ListAllType")
            Dim request = CType(System.Net.WebRequest.Create(query.RequestUri), HttpWebRequest)

            Dim buffer As Byte() = New Byte(8000) {}
            Dim async As IAsyncResult = request.BeginGetResponse(Nothing, Nothing)
            Dim response = request.EndGetResponse(async)
            Dim stream = response.GetResponseStream()

            request.Abort()

            Try
                Dim result = stream.BeginRead(buffer, 0, buffer.Length, Nothing, Nothing)
                Assert.Fail("Expected WebException")
            Catch ex As WebException
                Assert.AreEqual(RequestAbortedMessage, ex.Message, "{0}", ex)
            End Try

            response.Close()
        End Sub
#End Region

#Region "timeout"
        Private Const TimeoutInSeconds As Int32 = 1

        Friend Const TimeoutMessage As String = "The operation has timed out"

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub TimeoutFailure_SaveChanges_None()
            Dim p = New Client.VBSlowType()
            p.ID = 10
            ctx.AddObject("SlowType", p)

            Dim timer = Stopwatch.StartNew()
            ctx.Timeout = TimeoutInSeconds
            Try
                Dim r = ctx.SaveChanges(SaveChangesOptions.None)
                Assert.Fail("Expected DataServiceRequestException")
            Catch ex As DataServiceRequestException
                Assert.IsInstanceOfType(ex.InnerException, GetType(DataServiceClientException))
                Assert.IsTrue(ex.InnerException.Message.Contains(TimeoutMessage), "{0}", ex)
            End Try
        End Sub

        <Ignore()>
        <TestCategory("Partition3")> <TestMethod()> Public Sub TimeoutFailure_SaveChanges_Continue()
            Dim p = New Client.VBSlowType()
            p.ID = 10
            ctx.AddObject("SlowType", p)

            Dim timer = Stopwatch.StartNew()
            ctx.Timeout = TimeoutInSeconds
            Try
                Dim r = ctx.SaveChanges(SaveChangesOptions.ContinueOnError)
                Assert.Fail("Expected DataServiceRequestException")
            Catch ex As DataServiceRequestException
                Assert.IsInstanceOfType(ex.InnerException, GetType(DataServiceClientException))
                Assert.IsTrue(ex.InnerException.Message.Contains(TimeoutMessage), "{0}", ex)
            End Try
        End Sub

#End Region

#Region "ValidateEqual"
        Private Sub ValidateEqual(Of T As {IEquatable(Of T)})(ByVal expected As IEnumerable(Of T), ByVal actual As IEnumerable(Of T), ByVal expectedCount As Int32, Optional ByVal validate As Action(Of T) = Nothing)
            Assert.AreNotSame(expected, actual)
            Assert.IsNotNull(expected)
            Assert.IsNotNull(actual)

            Dim ae = expected.GetEnumerator()
            Dim be = actual.GetEnumerator()

            Dim iteration As Int32 = 0
            While (True)
                Dim moved = ae.MoveNext()
                Assert.AreEqual(moved, be.MoveNext(), "MoveNext are not equal")
                If Not moved Then
                    Exit While
                End If

                Dim aev = ae.Current
                Dim bev = be.Current

                Assert.AreNotSame(aev, bev)
                Assert.IsTrue(aev.Equals(bev), "VBAllType are not equal")
                If validate IsNot Nothing Then
                    validate(bev)
                End If

                iteration += 1
            End While
            Assert.AreEqual(expectedCount, iteration, "expected more objects")
        End Sub
#End Region

    End Class

    <TestClass()> Public Class BatchFailure
#Region "ClassInitialize, ClassCleanup, TestInitialize, TestCleanup"
        Private Shared web As TestWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.ServiceType = GetType(Client.BatchZeroService)
            web.StartService()

            Dim ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.StopService()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Client.TypeResolveContext.ResetData()
            Me.ctx = New DataServiceContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub
#End Region

        Private Sub BatchZeroFailure(ByVal response As DataServiceResponse)
            For Each x As QueryOperationResponse In response
                Try
                    For Each y In x
                        Debug.Assert(False, "expected exception 1")
                    Next
                    Debug.Assert(False, "expected exception 2")
                Catch e As InvalidOperationException
                End Try
            Next
        End Sub

    End Class

    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    ' <TestClass()>
    Public Class ProjectionRoundTrippingTests

#Region "ClassInitialize, ClassCleanup, TestInitialize, TestCleanup"
        Private Shared web As TestWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.DataServiceType = AstoriaUnitTests.Data.ServiceModelData.CustomData.ServiceModelType
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

        <EntityType()>
        Public Class NarrowCustomer
            Implements INotifyPropertyChanged

            Private m_name As String
            Public Property Name() As String
                Get
                    Return m_name
                End Get
                Set(ByVal value As String)
                    If value <> m_name Then
                        m_name = value
                        OnPropertyChanged("Name")
                    End If
                End Set
            End Property

            Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
                If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                    RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
                End If
            End Sub

            Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        End Class

        Public Class NarrowCustomerWithKey
            Inherits NarrowCustomer

            Private m_ID As Int32
            Public Property ID() As Int32
                Get
                    Return m_ID
                End Get
                Set(ByVal value As Int32)
                    If value <> m_ID Then
                        m_ID = value
                        OnPropertyChanged("ID")
                    End If
                End Set
            End Property
        End Class

        <EntityType()>
        Public Class NarrowCustomerWithOrders
            Inherits NarrowCustomer

            Private m_orders As List(Of NarrowOrders) = New List(Of NarrowOrders)()
            Public ReadOnly Property Orders() As List(Of NarrowOrders)
                Get
                    Return m_orders
                End Get
            End Property
        End Class

        <EntityType()>
        Public Class NarrowCustomerWithKeyedOrders
            Inherits NarrowCustomer
            Private m_orders As DataServiceCollection(Of NarrowOrdersWithKey) = New DataServiceCollection(Of NarrowOrdersWithKey)(Nothing, TrackingMode.None)
            Public ReadOnly Property Orders() As DataServiceCollection(Of NarrowOrdersWithKey)
                Get
                    Return m_orders
                End Get
            End Property
        End Class

        <EntityType()>
        Public Class NarrowOrders
            Implements INotifyPropertyChanged

            Private m_dollarAmount As Decimal
            Public Property DollarAmount() As Decimal
                Get
                    Return m_dollarAmount
                End Get
                Set(ByVal value As Decimal)
                    If (value <> m_dollarAmount) Then
                        m_dollarAmount = value
                        OnPropertyChanged("DollarAmount")
                    End If
                End Set
            End Property

            Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
                If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                    RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
                End If
            End Sub

            Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        End Class

        Public Class NarrowOrdersWithKey
            Inherits NarrowOrders
            Private m_ID As Int32
            Public Property ID() As Int32
                Get
                    Return m_ID
                End Get
                Set(ByVal value As Int32)
                    If (value <> m_ID) Then
                        m_ID = value
                        OnPropertyChanged("ID")
                    End If
                End Set
            End Property
        End Class
        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectedEntityWithNoKey()
            ProjectedCustomerRoundTripTest(Of NarrowCustomer)("/Customers?$select=Name")

            ' Attach
            Try
                ctx.AttachTo("Customers", New NarrowCustomer())
                Assert.Fail("Attaching customers with no key (unknown edit link) did not fail")
            Catch ex As ArgumentException
            End Try
        End Sub

        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectedEntityWithKey()
            ProjectedCustomerRoundTripTest(Of NarrowCustomerWithKey)("/Customers?$select=Name,ID")
        End Sub
        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectedRelatedLinksWithKeyTest()
            Dim cust = ctx.Execute(Of NarrowCustomerWithKeyedOrders)(New Uri("/Customers?$select=Name&$expand=Orders($select=DollarAmount,ID)", UriKind.Relative)).FirstOrDefault()
            Assert.AreEqual(3, ctx.Entities.Count)
            Assert.AreEqual(2, ctx.Links.Count)

            ' delete links - require Key on Orders to be completed
            Dim firstLink = ctx.Links.FirstOrDefault()
            ctx.DeleteLink(firstLink.Source, firstLink.SourceProperty, firstLink.Target)
            Assert.AreEqual(1, ctx.SaveChanges().Count())
            Assert.AreEqual(EntityStates.Detached, firstLink.State)

            ' add link (via API)
            ctx.AddLink(firstLink.Source, firstLink.SourceProperty, firstLink.Target)
            Assert.AreEqual(1, ctx.SaveChanges().Count())

            ' AddRelatedObject
            Dim newOrder = New NarrowOrdersWithKey()
            newOrder.ID = 90001
            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            ctx.AddRelatedObject(cust, "Orders", newOrder)
            Assert.AreEqual(1, ctx.SaveChanges().Count())
        End Sub
        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectedAddRelatedNoKeyTest()
            Dim cust = ctx.Execute(Of NarrowCustomerWithOrders)(New Uri("/Customers?$select=Name&$expand=Orders($select=DollarAmount)", UriKind.Relative)).FirstOrDefault()
            ' Add related
            Dim newOrder = New NarrowOrders()
            ctx.AddRelatedObject(cust, "Orders", newOrder)
            Try
                Assert.AreEqual(1, ctx.SaveChanges().Count())
            Catch ex As DataServiceRequestException
                Dim innerEx = CType(ex.InnerException, DataServiceClientException)
                ' CustomDataContext does not auto increment keys on the server
                TestUtil.AssertContains(innerEx.Message, "Entity with the same key already present. EntityType: 'Order'")
            End Try
        End Sub
        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectedRelatedLinksNoKeyTest()
            Using CustomDataContext.CreateChangeScope()
                Dim cust = ctx.Execute(Of NarrowCustomerWithOrders)(New Uri("/Customers?$select=Name&$expand=Orders($select=DollarAmount)", UriKind.Relative)).FirstOrDefault()
                Assert.AreEqual(3, ctx.Entities.Count)
                Assert.AreEqual(2, ctx.Links.Count)

                ' delete links - require Key on Orders to be completed
                Dim firstLink = ctx.Links.FirstOrDefault()
                ctx.DeleteLink(firstLink.Source, firstLink.SourceProperty, firstLink.Target)

                Try
                    ctx.SaveChanges()
                Catch ex As ArgumentException
                End Try

                ' load property - with ignore missing
                ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
                ctx.LoadProperty(cust, "Orders")
                ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException

                ' load property with uri
                Dim custDescriptor = ctx.GetEntityDescriptor(cust)
                Dim projectedOrdersUri = New Uri(custDescriptor.EditLink.AbsoluteUri + "/Orders?$select=DollarAmount", UriKind.Absolute)
                ctx.LoadProperty(cust, "Orders", projectedOrdersUri)
            End Using
        End Sub
        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectedBindingTest()
            Using CustomDataContext.CreateChangeScope()
                Dim q = ctx.Execute(Of NarrowCustomerWithOrders)(New Uri("/Customers?$select=Name&$expand=Orders($select=DollarAmount)", UriKind.Relative))
                Dim custs = New DataServiceCollection(Of NarrowCustomerWithOrders)(ctx)
                custs.Load(q)
                ' here we cannot add a new customer or order since there is no way to set the key
                ' test updates and deletes
                custs(0).Name = "New Cust"
                custs.Remove(custs(1))
                ctx.ResolveName = AddressOf ResolveNameFromType
                ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support

                Try
                    Assert.AreEqual(2, ctx.SaveChanges().Count())
                Finally
                    ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
                End Try
            End Using
        End Sub
        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectedBindingNestedTrackingTest()
            Using CustomDataContext.CreateChangeScope()
                Dim q = ctx.Execute(Of NarrowCustomerWithKeyedOrders)(New Uri("/Customers?$select=Name&$expand=Orders($select=DollarAmount,ID)", UriKind.Relative))
                Dim custs = New DataServiceCollection(Of NarrowCustomerWithKeyedOrders)(ctx)
                custs.Load(q)

                Dim cust = custs.First()
                Assert.AreEqual(2, cust.Orders.Count)
                Dim order = cust.Orders.First()

                ' update
                order.DollarAmount = 30
                Assert.AreEqual(1, ctx.SaveChanges().Count())

                ' create
                Dim newOrder = New NarrowOrdersWithKey()
                newOrder.ID = 90001
                newOrder.DollarAmount = 20
                cust.Orders.Add(newOrder)
                ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
                Assert.AreEqual(1, ctx.SaveChanges().Count())
                ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException

                ' delete
                cust.Orders.Remove(newOrder)
                Assert.AreEqual(1, ctx.SaveChanges().Count())
            End Using
        End Sub
        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectedCodeGenResolverSequenceTest()
            Using CustomDataContext.CreateChangeScope()
                Dim q = ctx.Execute(Of NarrowCustomerWithOrders)(New Uri("/Customers?$select=Name", UriKind.Relative))
                Dim custs = New DataServiceCollection(Of NarrowCustomerWithOrders)(ctx)
                custs.Load(q)

                userResolverCallCount = 0
                codeGenResolverCallCount = 0

                custs(0).Name = "New Cust"
                ' user supplied resolver - should be called
                ctx.ResolveName = AddressOf ResolveNameFromType
                ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support

                Try
                    Assert.AreEqual(1, ctx.SaveChanges().Count())
                    Assert.AreEqual(1, userResolverCallCount)
                    Assert.AreEqual(0, codeGenResolverCallCount)

                    custs(0).Name = "Customer 0"
                    ctx.ResolveName = AddressOf CodeGenResolver
                    Assert.AreEqual(1, ctx.SaveChanges().Count())
                    Assert.AreEqual(1, userResolverCallCount)
                    Assert.AreEqual(0, codeGenResolverCallCount)

                Finally
                    ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
                End Try
            End Using
        End Sub
        'Remove Atom
        <Ignore> <TestCategory("Partition3")> <TestMethod()> Public Sub EntityDescriptorAPI()
            ' CRUD of entities and check the state of entity descriptor

            ' Create:
            Dim cust = New Customer
            cust.ID = 10000
            ctx.AddObject("Customers", cust)
            Dim newDescriptor = ctx.GetEntityDescriptor(cust)
            Assert.AreEqual(EntityStates.Added, newDescriptor.State)
            Assert.IsNull(newDescriptor.ETag)
            Assert.IsNull(newDescriptor.Identity)
            Assert.IsNull(newDescriptor.ServerTypeName)

            ' Delete
            ctx.DeleteObject(cust)
            Assert.AreEqual(EntityStates.Detached, newDescriptor.State)

            ' Get
            cust = ctx.CreateQuery(Of Customer)("Customers").First
            Dim descriptor = ctx.GetEntityDescriptor(cust)
            Assert.AreEqual(EntityStates.Unchanged, descriptor.State)
            Assert.IsNotNull(descriptor.ServerTypeName)
            Assert.IsNotNull(descriptor.ETag)

            ' Get - relationship
            ctx.MergeOption = MergeOption.OverwriteChanges
            cust = ctx.CreateQuery(Of Customer)("Customers").Expand("Orders").First()
            descriptor = ctx.GetEntityDescriptor(cust.Orders.First)
            Assert.AreEqual(EntityStates.Unchanged, descriptor.State)
            Assert.IsNotNull(descriptor.EditLink)
            Assert.IsNotNull(descriptor.Identity)

            ' AddRelatedObject
            Dim newOrder = New Order(99999, 20)
            ctx.AddRelatedObject(cust, "Orders", newOrder)
            newDescriptor = ctx.GetEntityDescriptor(newOrder)
            Assert.AreEqual(EntityStates.Added, newDescriptor.State)
            Assert.AreEqual(ctx.GetEntityDescriptor(cust), newDescriptor.ParentForInsert)
            Assert.AreEqual("Orders", newDescriptor.ParentPropertyForInsert)

            ' AttachTo
            ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()

            ctx.AttachTo("Customers", cust)
            ' check generated edit link
            descriptor = ctx.GetEntityDescriptor(cust)
            Assert.IsNotNull(descriptor.EditLink)
            Assert.IsNotNull(descriptor.Identity)
            Assert.AreEqual(EntityStates.Unchanged, descriptor.State)

            ctx.AddRelatedObject(cust, "Orders", newOrder)
            newDescriptor = ctx.GetEntityDescriptor(newOrder)
            Assert.AreEqual(EntityStates.Added, newDescriptor.State)
            Assert.AreEqual(ctx.GetEntityDescriptor(cust), newDescriptor.ParentForInsert)
            Assert.AreEqual("Orders", newDescriptor.ParentPropertyForInsert)

            Assert.AreEqual(1, ctx.SaveChanges().Count())

            Assert.IsNull(newDescriptor.ParentForInsert)
            Assert.IsNull(newDescriptor.ParentPropertyForInsert)

        End Sub

        Private Sub ProjectedCustomerRoundTripTest(Of T As New)(ByVal uri As String)
            Using CustomDataContext.CreateChangeScope()
                ' narrow customer should be considered as entity even if there is no keys
                ' Get
                ctx.Execute(Of T)(New Uri(uri, UriKind.Relative)).Count()
                Assert.AreEqual(3, ctx.Entities.Count)
                Dim firstDescriptor = (From e In ctx.Entities Where e.ServerTypeName = "AstoriaUnitTests.Stubs.Customer" Select e).FirstOrDefault()

                ' Update (through API call)
                ctx.UpdateObject(firstDescriptor.Entity)
                Assert.AreEqual(1, ctx.SaveChanges().Count())

                ' Delete (through Descriptor - save changes())
                ctx.DeleteObject(firstDescriptor.Entity)
                Assert.AreEqual(1, ctx.SaveChanges().Count())

                ' restore deleted object
                ctx.ResolveName = AddressOf ResolveNameFromType
                ctx.AddObject("Customers", firstDescriptor.Entity)
                ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
                Assert.AreEqual(1, ctx.SaveChanges().Count())
                ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            End Using
        End Sub

        Private userResolverCallCount As Int32 = 0
        Private codeGenResolverCallCount As Int32 = 0

        Private Function ResolveNameFromType(ByVal type As Type) As String
            userResolverCallCount = userResolverCallCount + 1
            If (GetType(NarrowCustomer).IsAssignableFrom(type)) Then
                Return "AstoriaUnitTests.Stubs.Customer"
            ElseIf GetType(NarrowOrders).IsAssignableFrom(type) Then
                Return "AstoriaUnitTests.Stubs.Order"
            Else
                Return Nothing
            End If
        End Function

        <System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Service.Design", "1.0.0")>
        Private Function CodeGenResolver(ByVal type As Type) As String
            codeGenResolverCallCount = codeGenResolverCallCount + 1

            If (GetType(NarrowCustomer).IsAssignableFrom(type)) Then
                Return "AstoriaUnitTests.Stubs.Customer"
            ElseIf GetType(NarrowOrders).IsAssignableFrom(type) Then
                Return "AstoriaUnitTests.Stubs.Order"
            Else
                Return type.FullName
            End If

        End Function
    End Class

End Class

Namespace Client
#Region "TypeResolveService"
    Public Class TypeResolveService
        Inherits Microsoft.OData.Service.DataService(Of TypeResolveContext)

        Public Shared Sub InitializeService(ByVal config As IDataServiceConfiguration)
            config.SetEntitySetAccessRule("*", EntitySetRights.All)
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All)
            config.UseVerboseErrors = True
        End Sub
    End Class

    Public Class BatchZeroService
        Inherits Microsoft.OData.Service.DataService(Of TypeResolveContext)

        Public Shared Sub InitializeService(ByVal config As IDataServiceConfiguration)
            config.SetEntitySetAccessRule("*", EntitySetRights.All)
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All)
            config.UseVerboseErrors = True
            config.MaxBatchCount = 0
            config.MaxChangesetCount = 0
        End Sub
    End Class
#End Region

#Region "TypeResolveContext"

    Friend Class ContextData
        Private Shared globalVersion As Int32
        Private ReadOnly version As Int32 = Interlocked.Increment(globalVersion)
        Friend allTypes As New List(Of VBAllType)
        Friend simpleTypes As New List(Of VBSimpleType)
        Friend slowType As New List(Of VBSlowType)

        Public Sub New()
        End Sub

        Public Sub New(ByVal src As ContextData)
            allTypes = New List(Of VBAllType)(src.allTypes)
            simpleTypes = New List(Of VBSimpleType)(src.simpleTypes)
            slowType = New List(Of VBSlowType)(src.slowType)
        End Sub
    End Class

    Public Class TypeResolveContext
        Implements Microsoft.OData.Service.IUpdatable

#Region "fields"
        Friend Shared serverlock As New Object()

        Friend Shared globalData As New ContextData()

        Private localData As ContextData = globalData
        Private originalData As ContextData = globalData

        Friend Shared serverMillisecondSleep As Int32

        Friend Shared clientAllTypes As New List(Of VBAllType)()
        Friend Shared clientSimpleTypes As New List(Of VBSimpleType)()
#End Region

#Region "constructor"
        Public Sub New()
        End Sub
#End Region

#Region "properties: ListAllType, ListSimpleType, SlowType"
        Public ReadOnly Property ListAllType() As IQueryable(Of VBAllType)
            Get
                Return localData.allTypes.AsQueryable()
            End Get
        End Property

        Public ReadOnly Property ListSimpleType() As IQueryable(Of VBSimpleType)
            Get
                Return localData.simpleTypes.AsQueryable()
            End Get
        End Property

        Public ReadOnly Property SlowType() As IQueryable(Of VBSlowType)
            Get
                If 0 <> serverMillisecondSleep Then
                    System.Threading.Thread.Sleep(serverMillisecondSleep)
                End If

                Return localData.slowType.AsQueryable()
            End Get
        End Property
#End Region

#Region "IUpdatable"
        Public Sub AddReferenceToCollection(ByVal targetResource As Object, ByVal propertyName As String, ByVal resourceToBeAdded As Object) Implements Microsoft.OData.Service.IUpdatable.AddReferenceToCollection
            Throw New NotImplementedException()
        End Sub

        Public Sub ClearChanges() Implements Microsoft.OData.Service.IUpdatable.ClearChanges
            Throw New NotImplementedException()
        End Sub

        Public Function CreateResource(ByVal containerName As String, ByVal fullTypeName As String) As Object Implements Microsoft.OData.Service.IUpdatable.CreateResource
            If (localData Is originalData) Then
                localData = New ContextData(originalData)
            End If

            If (containerName = "SlowType" AndAlso fullTypeName = "AstoriaClientUnitTests.Client.VBSlowType") Then
                Dim s = New VBSlowType()
                localData.slowType.Add(s)
                Return s
            End If
            Throw New NotImplementedException()
        End Function

        Public Sub DeleteResource(ByVal targetResource As Object) Implements Microsoft.OData.Service.IUpdatable.DeleteResource
            Throw New NotImplementedException()
        End Sub

        Public Function GetResource(ByVal query As System.Linq.IQueryable, ByVal fullTypeName As String) As Object Implements Microsoft.OData.Service.IUpdatable.GetResource
            Throw New NotImplementedException()
        End Function

        Public Function GetValue(ByVal targetResource As Object, ByVal propertyName As String) As Object Implements Microsoft.OData.Service.IUpdatable.GetValue
            Throw New NotImplementedException()
        End Function

        Public Sub RemoveReferenceFromCollection(ByVal targetResource As Object, ByVal propertyName As String, ByVal resourceToBeRemoved As Object) Implements Microsoft.OData.Service.IUpdatable.RemoveReferenceFromCollection
            Throw New NotImplementedException()
        End Sub

        Public Function ResetResource(ByVal resource As Object) As Object Implements Microsoft.OData.Service.IUpdatable.ResetResource
            Throw New NotImplementedException()
        End Function

        Public Function ResolveResource(ByVal resource As Object) As Object Implements Microsoft.OData.Service.IUpdatable.ResolveResource
            Throw New NotImplementedException()
        End Function

        Public Sub SaveChanges() Implements Microsoft.OData.Service.IUpdatable.SaveChanges
            If (originalData Is Interlocked.CompareExchange(globalData, localData, originalData)) Then
                Return
            Else
                Throw New System.Data.DBConcurrencyException("data changed by someone else")
            End If

            Throw New NotImplementedException()
        End Sub

        Public Sub SetReference(ByVal targetResource As Object, ByVal propertyName As String, ByVal propertyValue As Object) Implements Microsoft.OData.Service.IUpdatable.SetReference
            Throw New NotImplementedException()
        End Sub

        Public Sub SetValue(ByVal targetResource As Object, ByVal propertyName As String, ByVal propertyValue As Object) Implements Microsoft.OData.Service.IUpdatable.SetValue
            Dim slow = TryCast(targetResource, VBSlowType)
            If slow IsNot Nothing Then
                If (propertyName = "ID") Then
                    slow.ID = CInt(propertyValue)
                    Return
                End If
            End If
            Throw New NotImplementedException()
        End Sub
#End Region

#Region "Reset data"
        Public Shared Sub ResetData()
            SyncLock (serverlock)
                VBAllType.ObjectCount = 0
                VBSimpleType.ObjectCount = 0
                Dim a = CreateData_AllType()
                Dim b = CreateData_SimpleType()

                VBAllType.ObjectCount = 0
                VBSimpleType.ObjectCount = 0
                Dim c = CreateData_AllType()
                Dim d = CreateData_SimpleType()

                clientAllTypes = a
                clientSimpleTypes = b

                globalData.allTypes = c
                globalData.simpleTypes = d

                globalData.slowType = New List(Of VBSlowType)
                serverMillisecondSleep = CInt(New TimeSpan(0, 0, ClientModule.TypeResolving.ServerSleepInSeconds).TotalMilliseconds)
            End SyncLock
        End Sub

        Public Shared Sub AddSubTypes()
            SyncLock (serverlock)
                Dim count = VBSimpleType.ObjectCount
                Dim c = CreateData2_SimpleType()

                VBSimpleType.ObjectCount = count
                Dim d = CreateData2_SimpleType()

                globalData.simpleTypes.AddRange(c)
                clientSimpleTypes.AddRange(d)
            End SyncLock
        End Sub

        Private Shared Function CreateData_AllType() As List(Of VBAllType)

            Dim a As New VBAllType
            a.PBinary = New System.Data.Linq.Binary(New Guid("0C733A7B-2A1C-11CE-ADE5-00AA0044773D").ToByteArray())
            a.PByteArray = New Guid("0C733A90-2A1C-11CE-ADE5-00AA0044773D").ToByteArray()
            a.PChar = "Z"c
            a.PInt16 = Int16.MaxValue
            a.PInt32 = 123456
            a.PInt64 = Int64.MaxValue
            a.PSingle = Math.PI
            a.PDouble = Math.E
            a.PDecimal = New Decimal(Math.PI) * New Decimal(Math.E)
            a.PGuid = New Guid("0C733A7C-2A1C-11CE-ADE5-00AA0044773D")
            a.PDateTimeOffset = DateTimeOffset.Now
            a.PTimeSpan = XmlConvert.ToString(DateTime.Now - New DateTime(1970, 1, 1))
            a.PString = "localhost"
            a.PCharArray = vbTab & vbCrLf
            a.PUri = New Uri("http://localhost/service.svc/").OriginalString
            a.PType = GetType(DataServiceContext).ToString()
            a.PXElement = (<html><body><b>Hello</b></body></html>).ToString()
            a.PXDocument = (<?xml version="1.0" encoding="utf-8" standalone="yes"?><feed><entry><id></id></entry></feed>).ToString()

            a.NPInt16 = 0
            a.NPInt32 = -1
            a.PStruct.PInt32 = 32
            a.PStruct.PString = "I'm a complex thingamjig"

            a.PClass = New VBSimpleType
            a.PClass.PInt32 = 333

            a.RPClass.PInt32 = 444
            Debug.Assert(a.PNClass Is Nothing, "should be null")


            Dim b As New VBAllType
            b.PGuid = New Guid("0C733A5F-2A1C-11CE-ADE5-00AA0044773D")
            b.PByte = Byte.MaxValue
            b.PInt32 = Int32.MaxValue
            b.PInt64 = Int64.MinValue
            b.PSingle = Single.MaxValue
            b.PDouble = Double.MaxValue
            b.PDecimal = Decimal.MaxValue
            b.PString = "removehost"

            Dim local As New List(Of VBAllType)
            local.Add(a)
            local.Add(b)
            Return local
        End Function

        Private Shared Function CreateData_SimpleType() As List(Of VBSimpleType)

            Dim a As New VBSimpleType
            a.PInt32 = 1

            Dim b As New VBSimpleType
            b.PInt32 = 2

            Dim local As New List(Of VBSimpleType)
            local.Add(a)
            local.Add(b)
            Return local
        End Function

        Private Shared Function CreateData2_SimpleType() As List(Of VBSimpleType)
            Dim a As New VBSimpleType2
            a.PInt32 = a.ID
            a.PInt64 = a.ID

            Dim b As New VBSimpleType2
            b.PInt32 = b.ID
            b.PInt64 = b.ID

            Dim local As New List(Of VBSimpleType)
            local.Add(a)
            local.Add(b)
            Return local
        End Function

#End Region
    End Class
#End Region

#Region "VBAllType"
    <ETag("PBinary")>
    Public Class VBAllType
        Implements System.IEquatable(Of VBAllType)

#Region "fields"
        Private fboolean As Boolean
        Private fbinary As System.Data.Linq.Binary
        Private fbytearray As Byte()
        Private fbyte As Byte
        Private fchar As Char
        Private fshort As Int16
        Private fint As Int32
        Private flong As Int64
        Private fsingle As Single
        Private fdouble As Double
        Private fdecimal As Decimal
        Private fdatetimeoffset As DateTimeOffset
        Private ftimespan As TimeSpan
        Private fguid As Guid
        Private fstring As String
        Private fchararray As Char()
        Private furi As Uri
        Private fxelement As XElement
        Private fxdocument As XDocument
        Private ftype As String

        Private nfshort As Nullable(Of Int16)
        Private nfint As Nullable(Of Int32)

        Private fstruct As VBStruct
        Private fclass As VBSimpleType
        Private fnclass As VBSimpleType
        Private rfclass As New VBSimpleType
        Private fcollection As New List(Of VBSimpleType)()
        Private gcollection As New List(Of VBSimpleType)()
        Private hcollection As New List(Of VBSimpleType)()

        Private objectID As Int32 = System.Threading.Interlocked.Increment(ObjectCount)
        Friend Shared ObjectCount As Int32
#End Region

#Region "properties"
        Public Property VBAllTypeID() As Int32
            Get
                Return Me.objectID
            End Get
            Set(ByVal value As Int32)
                Me.objectID = value
            End Set
        End Property

        Public Property PBoolean() As Boolean
            Get
                Return Me.fboolean
            End Get
            Set(ByVal value As Boolean)
                Me.fboolean = value
            End Set
        End Property

        Public Property PBinary() As System.Data.Linq.Binary
            Get
                If (Me.fbinary Is Nothing) Then
                    Me.fbinary = New Guid().ToByteArray()
                End If
                Return Me.fbinary
            End Get
            Set(ByVal value As System.Data.Linq.Binary)
                Me.fbinary = value
            End Set
        End Property

        Public Property PByteArray() As Byte()
            Get
                If Me.fbytearray Is Nothing Then
                    Return Nothing
                End If
                Return CType(Me.fbytearray.Clone(), Byte())
            End Get
            Set(ByVal value As Byte())
                If (value Is Nothing) Then
                    Me.fbytearray = Nothing
                Else
                    Me.fbytearray = CType(value.Clone(), Byte())
                End If
            End Set
        End Property

        Public Property PByte() As Byte
            Get
                Return Me.fbyte
            End Get
            Set(ByVal value As Byte)
                Me.fbyte = value
            End Set
        End Property

        Public Property PChar() As String
            Get
                Return New String(Me.fchar, 1)
            End Get
            Set(ByVal value As String)
                Me.fchar = value.Single()
            End Set
        End Property

        Public Property PInt16() As Int16
            Get
                Return Me.fshort
            End Get
            Set(ByVal value As Int16)
                Me.fshort = value
            End Set
        End Property

        Public Property NPInt16() As Nullable(Of Int16)
            Get
                Return Me.nfshort
            End Get
            Set(ByVal value As Nullable(Of Int16))
                Me.nfshort = value
            End Set
        End Property

        Public Property PInt32() As Int32
            Get
                Return Me.fint
            End Get
            Set(ByVal value As Int32)
                Me.fint = value
            End Set
        End Property

        Public Property NPInt32() As Nullable(Of Int32)
            Get
                Return Me.nfint
            End Get
            Set(ByVal value As Nullable(Of Int32))
                Me.nfint = value
            End Set
        End Property

        Public Property PInt64() As Int64
            Get
                Return Me.flong
            End Get
            Set(ByVal value As Int64)
                Me.flong = value
            End Set
        End Property

        Public Property PSingle() As Single
            Get
                Return Me.fsingle
            End Get
            Set(ByVal value As Single)
                Me.fsingle = value
            End Set
        End Property

        Public Property PDouble() As Double
            Get
                Return Me.fdouble
            End Get
            Set(ByVal value As Double)
                Me.fdouble = value
            End Set
        End Property

        Public Property PDecimal() As Decimal
            Get
                Return Me.fdecimal
            End Get
            Set(ByVal value As Decimal)
                Me.fdecimal = value
            End Set
        End Property

        Public Property PGuid() As Guid
            Get
                Return Me.fguid
            End Get
            Set(ByVal value As Guid)
                Me.fguid = value
            End Set
        End Property

        Public Property PDateTimeOffset() As DateTimeOffset
            Get
                Return Me.fdatetimeoffset
            End Get
            Set(ByVal value As DateTimeOffset)
                Me.fdatetimeoffset = value
            End Set
        End Property

        Public Property PTimeSpan() As String
            Get
                Return XmlConvert.ToString(Me.ftimespan)
            End Get
            Set(ByVal value As String)
                Me.ftimespan = XmlConvert.ToTimeSpan(value)
            End Set
        End Property

        Public Property PString() As String
            Get
                Return Me.fstring
            End Get
            Set(ByVal value As String)
                Me.fstring = value
            End Set
        End Property

        Public Property PCharArray() As String
            Get
                Return New String(Me.fchararray)
            End Get
            Set(ByVal value As String)
                If (value Is Nothing) Then
                    Me.fchararray = Nothing
                Else
                    Me.fchararray = value.ToCharArray()
                End If
            End Set
        End Property

        Public Property PUri() As String
            Get
                If (Me.furi Is Nothing) Then
                    Return Nothing
                End If
                Return Me.furi.OriginalString
            End Get
            Set(ByVal value As String)
                If String.IsNullOrEmpty(value) Then
                    Me.furi = Nothing
                Else
                    Me.furi = New Uri(value, UriKind.RelativeOrAbsolute)
                End If
            End Set
        End Property

        Public Property PType() As String
            Get
                Return Me.ftype
            End Get
            Set(ByVal value As String)
                ' type can't be evaluated on server
                ' TODO: validate "assembly, version=x, ..."
                Me.ftype = value
            End Set
        End Property

        Public Property PXElement() As String
            Get
                If (Me.fxelement Is Nothing) Then
                    Return Nothing
                End If
                Return Me.fxelement.ToString()
            End Get
            Set(ByVal value As String)
                If (String.IsNullOrEmpty(value)) Then
                    Me.fxelement = Nothing
                Else
                    Me.fxelement = XElement.Parse(value)
                End If
            End Set
        End Property

        Public Property PXDocument() As String
            Get
                If (Me.fxdocument Is Nothing) Then
                    Return Nothing
                End If
                Return Me.fxdocument.ToString()
            End Get
            Set(ByVal value As String)
                If value Is Nothing Then
                    Me.fxdocument = Nothing
                Else
                    Me.fxdocument = XDocument.Parse(value)
                End If
            End Set
        End Property

        Public Property PStruct() As VBStruct
            Get
                If Me.fstruct Is Nothing Then
                    Me.fstruct = New VBStruct()
                End If
                Return Me.fstruct
            End Get
            Set(ByVal value As VBStruct)
                Me.fstruct = value
            End Set
        End Property

        Public Property PClass() As VBSimpleType
            Get
                Return Me.fclass
            End Get
            Set(ByVal value As VBSimpleType)
                Me.fclass = value
            End Set
        End Property

        Public Property PNClass() As VBSimpleType
            Get
                Return Me.fnclass
            End Get
            Set(ByVal value As VBSimpleType)
                Me.fnclass = value
            End Set
        End Property

        Public Property RPClass() As VBSimpleType
            Get
                Return Me.rfclass
            End Get
            Set(ByVal value As VBSimpleType)
                If Not Object.ReferenceEquals(Me.rfclass, value) Then
                    Throw New ArgumentException("RPClass")
                End If
            End Set
        End Property

        Public Property PCollection() As IList(Of VBSimpleType)
            Get
                Return Me.fcollection
            End Get
            Set(ByVal value As IList(Of VBSimpleType))
                If Not Object.ReferenceEquals(Me.fcollection, value) Then
                    Throw New ArgumentException("PCollection")
                End If
            End Set
        End Property

        Public Property NPCollection() As IList(Of VBSimpleType)
            Get
                Return Me.gcollection
            End Get
            Set(ByVal value As IList(Of VBSimpleType))
                If Not Object.ReferenceEquals(Me.gcollection, value) Then
                    Throw New ArgumentException("PCollection")
                End If
            End Set
        End Property

        Public Property OPCollection() As IList(Of VBSimpleType)
            Get
                Return Me.hcollection
            End Get
            Set(ByVal value As IList(Of VBSimpleType))
                If Not Object.ReferenceEquals(Me.hcollection, value) Then
                    Throw New ArgumentException("PCollection")
                End If
            End Set
        End Property
#End Region

#Region "Equals"
        Public Overloads Function Equals(ByVal other As VBAllType) As Boolean Implements System.IEquatable(Of VBAllType).Equals
            Return (Me.PInt16 = other.PInt16) And
                   (Me.PInt32 = other.PInt32) And
                   (Me.PInt64 = other.PInt64) And
                   (Me.PString = other.PString) And
                   (Me.PCharArray = other.PCharArray) And
                   (Me.PStruct.PInt32 = other.PStruct.PInt32) And
                   (Me.PStruct.PString = other.PStruct.PString)
        End Function
#End Region

    End Class


    Public Class VBStruct
#Region "fields"
        Private fint As Int32
        Private fstr As String
#End Region

#Region "properties"
        Public Property PInt32() As Int32
            Get
                Return Me.fint
            End Get
            Set(ByVal value As Int32)
                Me.fint = value
            End Set
        End Property

        Public Property PString() As String
            Get
                Return Me.fstr
            End Get
            Set(ByVal value As String)
                Me.fstr = value
            End Set
        End Property
#End Region
    End Class

#End Region

#Region "VBSimpleType"
    Public Class VBSimpleType
        Implements System.IEquatable(Of VBSimpleType)

#Region "fields"
        Private fint32 As Int32
        Private objectID As Int32 = System.Threading.Interlocked.Increment(ObjectCount)
        Friend Shared ObjectCount As Int32
#End Region

#Region "constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal id As Int32)
            Me.ID = id
        End Sub

        Public Sub New(ByVal other As VBSimpleType)
            Me.ID = other.ID
            Me.PInt32 = other.PInt32
        End Sub
#End Region

#Region "properties"
        Public Property ID() As Int32
            Get
                Return Me.objectID
            End Get
            Set(ByVal value As Int32)
                Me.objectID = value
            End Set
        End Property

        Public Property PInt32() As Int32
            Get
                Return Me.fint32
            End Get
            Set(ByVal value As Int32)
                Me.fint32 = value
            End Set
        End Property
#End Region

#Region "Equals"
        Public Overloads Function Equals(ByVal other As VBSimpleType) As Boolean Implements System.IEquatable(Of VBSimpleType).Equals
            Return (Me.ID = other.ID) And
                   (Me.PInt32 = other.PInt32)
        End Function
#End Region
    End Class

    Public Class VBSimpleType2
        Inherits VBSimpleType

#Region "fields"
        Private flong As Int64
#End Region

#Region "properties"
        Property PInt64() As Int64
            Get
                Return Me.flong
            End Get
            Set(ByVal value As Int64)
                Me.flong = value
            End Set
        End Property
#End Region
    End Class

#End Region

#Region "VBSlowType"
    Public Class VBSlowType
        Private _id As Int32

        Public Property ID() As Int32
            Get
                Return Me._id
            End Get
            Set(ByVal value As Int32)
                Me._id = value
            End Set
        End Property
    End Class
#End Region

End Namespace

Namespace OtherClient

#Region "VBAllType"
    Public Class VBAllType
        Inherits VBAllTypeBase
        Implements System.IEquatable(Of VBAllType)

#Region "fields"
        Private objectID As Int32 = System.Threading.Interlocked.Increment(ObjectCount)
        Friend Shared ObjectCount As Int32
#End Region

#Region "properties"
        Public Property VBAllTypeID() As Int32
            Get
                Return Me.objectID
            End Get
            Set(ByVal value As Int32)
                Me.objectID = value
            End Set
        End Property
#End Region

#Region "Equals"
        Public Overloads Function Equals(ByVal other As VBAllType) As Boolean Implements System.IEquatable(Of VBAllType).Equals
            Return (Me.PInt16 = other.PInt16) And
                   (Me.PInt32 = other.PInt32) And
                   (Me.PInt64 = other.PInt64) And
                   (Me.PString Is other.PString) And
                   (Me.PStruct.PInt32 = other.PStruct.PInt32) And
                   (Me.PStruct.PString Is other.PStruct.PString)
        End Function
#End Region
    End Class

    Public Class VBAllTypeBase
#Region "fields"
        Private fboolean As Boolean
        Private fbinary As System.Data.Linq.Binary
        Private fbytearray As Byte()
        Private fbyte As Byte
        Private fchar As Char
        Private fshort As Int16
        Private fint As Int32
        Private flong As Int64
        Private fsingle As Single
        Private fdouble As Double
        Private fdecimal As Decimal
        Private fdatetimeoffset As DateTimeOffset
        Private ftimespan As TimeSpan
        Private fguid As Guid
        Private fstring As String
        Private fchararray As Char()
        Private furi As Uri
        Private fxelement As XElement
        Private fxdocument As XDocument
        Private ftype As Type

        Private nfshort As Nullable(Of Int16)
        Private nfint As Nullable(Of Int32)

        Private fstruct As VBStruct
        Private fclass As VBSimpleType_Foo
        Private fnclass As VBSimpleType_Foo
        Private rfclass As New VBSimpleType_Foo
        Private fcollection As New Collection(Of VBSimpleType_Foo)()
        Private nfcollection As List(Of VBSimpleType_Foo) = Nothing
        Private ofcollection As New Collection(Of VBSimpleType_Foo)()
#End Region

#Region "properties"
        Public Property PBoolean() As Boolean
            Get
                Return Me.fboolean
            End Get
            Set(ByVal value As Boolean)
                Me.fboolean = value
            End Set
        End Property

        Public Property PBinary() As System.Data.Linq.Binary
            Get
                Return Me.fbinary
            End Get
            Set(ByVal value As System.Data.Linq.Binary)
                Me.fbinary = value
            End Set
        End Property

        Public Property PByteArray() As Byte()
            Get
                If Me.fbytearray Is Nothing Then
                    Return Nothing
                End If
                Return CType(Me.fbytearray.Clone(), Byte())
            End Get
            Set(ByVal value As Byte())
                If (value Is Nothing) Then
                    Me.fbytearray = New Byte() {}
                Else
                    Me.fbytearray = CType(value.Clone(), Byte())
                End If
            End Set
        End Property

        Public Property PByte() As Byte
            Get
                Return Me.fbyte
            End Get
            Set(ByVal value As Byte)
                Me.fbyte = value
            End Set
        End Property

        Public Property PChar() As Char
            Get
                Return Me.fchar
            End Get
            Set(ByVal value As Char)
                Me.fchar = value
            End Set
        End Property

        Public Property PInt16() As Int16
            Get
                Return Me.fshort
            End Get
            Set(ByVal value As Int16)
                Me.fshort = value
            End Set
        End Property

        Public Property NPInt16() As Nullable(Of Int16)
            Get
                Return Me.nfshort
            End Get
            Set(ByVal value As Nullable(Of Int16))
                Me.nfshort = value
            End Set
        End Property

        Public Property PInt32() As Int32
            Get
                Return Me.fint
            End Get
            Set(ByVal value As Int32)
                Me.fint = value
            End Set
        End Property

        Public Property NPInt32() As Nullable(Of Int32)
            Get
                Return Me.nfint
            End Get
            Set(ByVal value As Nullable(Of Int32))
                Me.nfint = value
            End Set
        End Property

        Public Property PInt64() As Int64
            Get
                Return Me.flong
            End Get
            Set(ByVal value As Int64)
                Me.flong = value
            End Set
        End Property

        Public Property PSingle() As Single
            Get
                Return Me.fsingle
            End Get
            Set(ByVal value As Single)
                Me.fsingle = value
            End Set
        End Property

        Public Property PDouble() As Double
            Get
                Return Me.fdouble
            End Get
            Set(ByVal value As Double)
                Me.fdouble = value
            End Set
        End Property

        Public Property PDecimal() As Decimal
            Get
                Return Me.fdecimal
            End Get
            Set(ByVal value As Decimal)
                Me.fdecimal = value
            End Set
        End Property

        Public Property PGuid() As Guid
            Get
                Return Me.fguid
            End Get
            Set(ByVal value As Guid)
                Me.fguid = value
            End Set
        End Property

        Public Property PDateTimeOffset() As DateTimeOffset
            Get
                Return Me.fdatetimeoffset
            End Get
            Set(ByVal value As DateTimeOffset)
                Me.fdatetimeoffset = value
            End Set
        End Property

        Public Property PTimeSpan() As TimeSpan
            Get
                Return Me.ftimespan
            End Get
            Set(ByVal value As TimeSpan)
                Me.ftimespan = value
            End Set
        End Property

        Public Property PString() As String
            Get
                Return Me.fstring
            End Get
            Set(ByVal value As String)
                Me.fstring = value
            End Set
        End Property

        Public Property PCharArray() As Char()
            Get
                If Me.fchararray Is Nothing Then
                    Return Nothing
                End If
                Return CType(Me.fchararray.Clone(), Char())
            End Get
            Set(ByVal value As Char())
                If (value Is Nothing) Then
                    Me.fchararray = Nothing
                Else
                    Me.fchararray = CType(value.Clone(), Char())
                End If
            End Set
        End Property

        Public Property PUri() As Uri
            Get
                Return Me.furi
            End Get
            Set(ByVal value As Uri)
                Me.furi = value
            End Set
        End Property

        Public Property PType() As Type
            Get
                Return Me.ftype
            End Get
            Set(ByVal value As Type)
                Me.ftype = value
            End Set
        End Property

        Public Property PXElement() As XElement
            Get
                Return Me.fxelement
            End Get
            Set(ByVal value As XElement)
                Me.fxelement = value
            End Set
        End Property

        Public Property PXDocument() As XDocument
            Get
                Return Me.fxdocument
            End Get
            Set(ByVal value As XDocument)
                Me.fxdocument = value
            End Set
        End Property

        Public Property PStruct() As VBStruct
            Get
                Return Me.fstruct
            End Get
            Set(ByVal value As VBStruct)
                Me.fstruct = value
            End Set
        End Property

        Public Property PClass() As VBSimpleType_Foo
            Get
                Return Me.fclass
            End Get
            Set(ByVal value As VBSimpleType_Foo)
                Me.fclass = value
            End Set
        End Property

        Public Property PNClass() As VBSimpleType_Foo
            Get
                Return Me.fnclass
            End Get
            Set(ByVal value As VBSimpleType_Foo)
                Me.fnclass = value
            End Set
        End Property

        Public ReadOnly Property RPClass() As VBSimpleType_Foo
            Get
                Return Me.rfclass
            End Get
        End Property

        Public ReadOnly Property PCollection() As IList(Of VBSimpleType_Foo)
            Get
                Return Me.fcollection
            End Get
        End Property

        Public Property NPCollection() As List(Of VBSimpleType_Foo)
            Get
                Return Me.nfcollection
            End Get
            Set(ByVal value As List(Of VBSimpleType_Foo))
                If Me.nfcollection Is Nothing Then
                    Me.nfcollection = value
                ElseIf Not Object.ReferenceEquals(Me.nfcollection, value) Then
                    Throw New ArgumentException("NPCollection")
                End If
            End Set
        End Property

        Public Property OPCollection() As Collection(Of VBSimpleType_Foo)
            Get
                Return Me.ofcollection
            End Get
            Set(ByVal value As Collection(Of VBSimpleType_Foo))
                If Me.ofcollection Is Nothing Then
                    Me.ofcollection = value
                ElseIf Not Object.ReferenceEquals(Me.ofcollection, value) Then
                    Throw New ArgumentException("NPCollection")
                End If
            End Set
        End Property
#End Region
    End Class

    Public Structure VBStruct
#Region "fields"
        Private fint As Int32
        Private fstr As String
#End Region

#Region "properties"
        Public Property PInt32() As Int32
            Get
                Return Me.fint
            End Get
            Set(ByVal value As Int32)
                Me.fint = value
            End Set
        End Property

        Public Property PString() As String
            Get
                Return Me.fstr
            End Get
            Set(ByVal value As String)
                Me.fstr = value
            End Set
        End Property
#End Region
    End Structure

#End Region

#Region "VBSimpleType_Foo"
    Public Class VBSimpleType_Foo
        Implements IEquatable(Of VBSimpleType_Foo)
        Private _id As Int32
        Private _int32 As Int32

#Region "constructors"
        Public Sub New()
        End Sub

        Public Sub New(ByVal id As Int32)
            _id = id
        End Sub
#End Region

#Region "properties"
        Public Property ID() As Int32
            Get
                Return _id
            End Get
            Set(ByVal value As Int32)
                _id = value
            End Set
        End Property

        Public Property PInt32() As Int32
            Get
                Return _int32
            End Get
            Set(ByVal value As Int32)
                _int32 = value
            End Set
        End Property
#End Region

#Region "Equals"
        Public Overloads Function Equals(ByVal other As VBSimpleType_Foo) As Boolean Implements System.IEquatable(Of VBSimpleType_Foo).Equals
            Return (Me.ID = other.ID) And
                   (Me.PInt32 = other.PInt32)
        End Function
#End Region
    End Class
#End Region

#Region "VBSimpleType_Foo2 : VBSimpleType_Foo"
    Friend Class VBSimpleType_Foo2
        Inherits VBSimpleType_Foo
        Private _int64 As Int64

#Region "properties"
        Public Property PInt64() As Int64
            Get
                Return _int64
            End Get
            Set(ByVal value As Int64)
                _int64 = value
            End Set
        End Property
#End Region
    End Class
#End Region
End Namespace
