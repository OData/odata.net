'---------------------------------------------------------------------
' <copyright file="NamedStreams_Projections.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports Microsoft.OData.Client
Imports System.Linq
Imports System.Linq.Expressions
Imports AstoriaUnitTests.Stubs
Imports AstoriaUnitTests.Stubs.DataServiceProvider
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports p = Microsoft.OData.Service.Providers

' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
'Remove Atom
' <TestClass()>
Public Class NamedStream_ProjectionTests_VB
    Private Shared request As TestWebRequest

    <ClassInitialize()>
    Public Shared Sub ClassInitialize(ByVal testContext As TestContext)
        Dim service As DSPServiceDefinition = SetUpNamedStreamService()
        request = service.CreateForInProcessWcf()
        request.StartService()
    End Sub

    <ClassCleanup()>
    Public Shared Sub ClassCleanup()
        If request IsNot Nothing Then
            request.Dispose()
            request = Nothing
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("Doing projection of non-stream properties should work and there should be no stream descriptors populated in the context")>
    Public Sub NamedStreams_SimpleProjectionWithoutStreams()
        Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
        'context.Format.UseAtom()
        'context.EnableAtom = True
        Dim query1 = context.CreateQuery(Of StreamType1)("MySet1").AddQueryOption("$select", "ID")
        Dim entities As List(Of StreamType1) = query1.Execute().ToList()
        Assert.AreEqual(entities.Count, 1, "There must be only 1 entities populated in the context")
        Assert.AreEqual(context.Entities(0).StreamDescriptors.Count, 0, "There must be no named streams associated with the entity yet, since we didn't specify the named streams in the projection query")
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("Doing projection of stream properties using AddQueryOption should work")>
    Public Sub NamedStreams_SimpleProjectionWithStreams()
        Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
        'context.EnableAtom = True
        'context.Format.UseAtom()
        Dim query1 = context.CreateQuery(Of StreamType1)("MySet1").AddQueryOption("$select", "ID, Stream1")
        Dim entities As List(Of StreamType1) = query1.Execute().ToList()
        Assert.AreEqual(context.Entities(0).StreamDescriptors.Count, 1, "There must be named streams associated with the entity")
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("One should not be able to project out named streams as normal properties")>
    Public Sub NamedStreams_CannotProjectStreamAsNormalProperties()
        Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
        'context.EnableAtom = True
        'context.Format.UseAtom()
        Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                Select New StreamType1() With {
                    .ID = s.ID,
                    .Stream1 = s.Stream1,
                    .Stream2 = s.Stream2
                }

        Try
            Dim entities As List(Of StreamType1) = q.ToList()
        Catch ex As InvalidOperationException
            Assert.AreEqual(ex.Message, AstoriaUnitTests.ODataLibResourceUtil.GetString("ValidationUtils_MismatchPropertyKindForStreamProperty", "Stream2"), "error message was not as expected")
        End Try
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("One should not be able to get named streams via load property api")>
    Public Sub NamedStreams_LoadPropertyTest()
        ' populate the context
        Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
        'context.EnableAtom = True
        'context.Format.UseAtom()
        Dim entity As StreamType1 = context.CreateQuery(Of StreamType1)("MySet1").AddQueryOption("$select", "ID, Stream1").Take(1).[Single]()
        Try
            context.LoadProperty(entity, "Stream1")
        Catch ex As DataServiceClientException
            Assert.IsTrue(ex.Message.Contains(AstoriaUnitTests.DataServicesResourceUtil.GetString("DataServiceException_UnsupportedMediaType")), [String].Format("The error message was not as expected: {0}", ex.Message))
        End Try
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("Simple projections to get stream url in DSSL property - both narrow type and anonymous type")>
    Public Sub NamedStreams_SimpleLinkProjection()
        If True Then
            ' Testing querying anonymous types and making sure one is able to project out the stream url
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New With {
                     .Name = s.Name,
                     .Stream11 = s.Stream1
                    }

            Assert.AreEqual(q.ToString(), Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$select=Name,Stream1", "make sure the right uri is produced by the linq translator")
            For Each o In q
                Assert.IsNotNull(o.Stream11, "Stream11 must have some value")
                Assert.AreEqual(o.Stream11.EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)/Stream1", "make sure the url property is correctly populated")
                Assert.IsNull(context.GetEntityDescriptor(o), "anonymous types are never tracked, even if we do a simple projection")
            Next

            Assert.AreEqual(context.Entities.Count, 0, "there should be entity tracked by the context")
        End If

        If True Then
            ' Testing querying narrow entity types and making sure one is able to project out the stream url
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New StreamType1() With {
                     .ID = s.ID,
                     .Stream1 = s.Stream1
                    }

            Assert.AreEqual(q.ToString(), Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$select=ID,Stream1", "make sure the right uri is produced by the linq translator")
            For Each o As StreamType1 In q
                Assert.IsNotNull(o.Stream1, "Stream11 must have some value")
                Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the value in the entity descriptor must match with the property value")
            Next

            Assert.AreEqual(context.Entities.Count, 1, "there should be only one entity tracked by the context")
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("Just project out the link properties in an entity, no data properties - both narrow type and anonymous type")>
    Public Sub NamedStreams_SimpleLinkProjection_1()
        If True Then
            ' Testing querying anonymous types and making sure one is able to project out the stream url
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New With {
                     .Stream11 = s.Stream1
                    }

            Assert.AreEqual(q.ToString(), Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$select=Stream1", "make sure the right uri is produced by the linq translator")
            For Each o In q
                Assert.IsNotNull(o.Stream11, "Stream11 must have some value")
                Assert.AreEqual(o.Stream11.EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)/Stream1", "make sure the url property is correctly populated")
                Assert.IsNull(context.GetEntityDescriptor(o), "anonymous types are never tracked, even if we do a simple projection")
            Next

            Assert.AreEqual(context.Entities.Count, 0, "there should be entity tracked by the context")
        End If

        If True Then
            ' Testing querying narrow entity types and making sure one is able to project out the stream url
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New StreamType1() With {
                     .Stream1 = s.Stream1
                    }

            Assert.AreEqual(q.ToString(), Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$select=Stream1", "make sure the right uri is produced by the linq translator")
            For Each o As StreamType1 In q
                Assert.IsNotNull(o.Stream1, "Stream11 must have some value")
                Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the value in the entity descriptor must match with the property value")
            Next

            Assert.AreEqual(context.Entities.Count, 1, "there should be only one entity tracked by the context")
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("If the entity getting projected out in an entity, one should not be able to refer deep links")>
    Public Sub NamedStreams_CannotReferenceDeepLinksDuringEntityMaterialization()
        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New StreamType1() With {
                     .ID = s.ID,
                     .Name = s.Name,
                     .RefStream1 = s.Ref.RefStream1
                    }

            Try
                For Each o In q
                Next
            Catch ex As NotSupportedException
                Assert.AreEqual(ex.Message, AstoriaUnitTests.DataServicesClientResourceUtil.GetString("ALinq_ProjectionMemberAssignmentMismatch", GetType(StreamType1).FullName, "s", "s.Ref"))
            End Try

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening")
        End If

        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot)
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New StreamType1() With {
                     .ID = s.Ref.ID,
                     .Name = s.Ref.Name,
                     .Stream1 = s.Stream1
                    }

            Try
                For Each o In q
                Next
            Catch ex As NotSupportedException
                Assert.AreEqual(ex.Message, AstoriaUnitTests.DataServicesClientResourceUtil.GetString("ALinq_ProjectionMemberAssignmentMismatch", GetType(StreamType1).FullName, "s.Ref", "s"))
            End Try

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening")
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("If the entity getting projected out in an anonymous type, one should be able to project out links from various levels")>
    Public Sub NamedStreams_NonEntity_AccessPropertiesFromDifferentLevels()
        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New With {
                     .Name = s.Name,
                     .RefStream1 = s.Ref.RefStream1
                    }

            For Each o In q
                Assert.AreEqual(o.RefStream1.EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet2(3)/RefStream1", "link must be populated")
            Next

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening")
        End If

        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New With {
                     .Name = s.Ref.Name,
                     .Stream1Url = s.Stream1
                    }

            For Each o In q
                Assert.AreEqual(o.Stream1Url.EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)/Stream1", "link must be populated")
            Next

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening")
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("Make sure DSSL properties can materialized and populated with the right url in non-projection cases")>
    Public Sub NamedStreams_PayloadDrivenMaterialization()
        If True Then
            ' Testing without projections (payload driven) and making sure one is able to project out the stream url
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            Dim q = context.CreateQuery(Of StreamWithUrl)("MySet1")

            For Each o As StreamWithUrl In q
                Assert.IsNotNull(o.Stream1, "Stream1 must have some value")
                Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the value in the entity descriptor must match with the property value")
                Assert.IsNull(o.SomeRandomProperty, "SomeRandomProperty must be null, since the payload does not have the link with the property name")
            Next
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("projecting out deep links to get stream url in DSSL property - both narrow type and anonymous type")>
    Public Sub DeepLinkProjection()
        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New With {
                     .Name = s.Ref.Name,
                     .Url = s.Ref.RefStream1
                    }

            Assert.AreEqual(Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$expand=Ref($select=Name),Ref($select=RefStream1)", q.ToString(), "make sure the right uri is produced by the linq translator")

            For Each o In q
                Assert.IsNotNull(o.Url, "Stream11 must have some value")
                Assert.AreEqual(o.Url.EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet2(3)/RefStream1", "the stream url must be populated correctly")
                Assert.IsNull(context.GetEntityDescriptor(o), "the entity must not be tracked by the context since we are trying to flatten the hierarchy")
            Next

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening")
        End If

        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New StreamType1() With {
                     .ID = s.Ref.ID,
                     .RefStream1 = s.Ref.RefStream1
                    }

            Assert.AreEqual(Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$expand=Ref($select=ID),Ref($select=RefStream1)", q.ToString(), "make sure the right uri is produced by the linq translator")

            For Each o In q
                Assert.IsNotNull(o.RefStream1, "RefStream1 must have some value")
                Assert.AreEqual(o.RefStream1.EditLink, context.GetReadStreamUri(o, "RefStream1"), "the stream url must be populated correctly")
            Next

            Assert.AreEqual(context.Entities.Count, 1, "there should be exactly one entity tracked by the context - the nested entity")
            Assert.AreEqual(context.Entities(0).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet2(3)", "the nested entity is the one that should be tracked")
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("projecting out deep links to get stream url in DSSL property with multiple parameters in scope - both narrow type and anonymous type")>
    Public Sub DeepLinkProjection_MultipleParametersInScope()
        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Where s.ID = 1
                    From c In s.Collection
                    Select New With {
                     .Name = c.Name,
                     .Url = c.ColStream
                    }

            Assert.AreEqual(q.ToString(), Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)/Collection?$select=Name,ColStream", "make sure the right uri is produced by the linq translator")

            Dim entities = q.ToList()
            Assert.AreEqual(entities(0).Url.EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('ABCDE')/ColStream", "the stream url must be populated correctly - index 0")
            Assert.AreEqual(entities(1).Url.EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('XYZ')/ColStream", "the stream url must be populated correctly - index 1")

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening")
        End If

        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Where s.ID = 1
                    From c In s.Collection
                    Select New StreamType2() With {
                     .ID = c.ID,
                     .ColStream = c.ColStream
                    }

            Assert.AreEqual(q.ToString(), Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)/Collection?$select=ID,ColStream", "make sure the right uri is produced by the linq translator")

            Dim entities = q.ToList()
            Assert.AreEqual(entities(0).ColStream.EditLink, context.GetReadStreamUri(entities(0), "ColStream"), "the stream url must be populated correctly - index 0")
            Assert.AreEqual(entities(1).ColStream.EditLink, context.GetReadStreamUri(entities(1), "ColStream"), "the stream url must be populated correctly - index 1")

            Assert.AreEqual(context.Entities.Count, 2, "there should be no entities tracked by the context, since we are doing flattening")
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("projecting out deep links to get stream url in DSSL property with multiple parameters in scope - both narrow type and anonymous type")>
    Public Sub DeepEntityProjection_CannotAccessEntitiesAcrossLevels()
        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Where s.ID = 1
                    From c In s.Collection
                    Select New StreamType2() With {
                     .ID = c.ID,
                     .Stream1 = s.Stream1
                    }

            Try
                q.ToList()
            Catch ex As NotSupportedException
                Assert.AreEqual(ex.Message, AstoriaUnitTests.DataServicesClientResourceUtil.GetString("ALinq_CanOnlyProjectTheLeaf"), "error message should match as expected")
            End Try

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context in error case")
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("projecting out deep links to get stream url in DSSL property - both narrow type and anonymous type")>
    Public Sub NamedStreams_NestedQuery_1()
        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New StreamType1() With {
                     .ID = s.ID,
                     .Stream1 = s.Stream1,
                     .Ref = New StreamType1() With {
                      .ID = s.Ref.ID,
                      .RefStream1 = s.Ref.RefStream1
                     },
                     .Collection = (From c In s.Collection
                                    Select New StreamType2() With {
                       .ID = c.ID,
                       .ColStream = c.ColStream
                      }).ToList()
                    }

            Assert.AreEqual(Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$expand=Ref($select=ID),Ref($select=RefStream1),Collection($select=ID),Collection($select=ColStream)&$select=ID,Stream1", q.ToString(), "make sure the right uri is produced by the linq translator")

            For Each o In q
                Assert.IsNotNull(o.Stream1, "Stream11 must have some value")
                Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the stream url for Stream1 must be populated correctly")
                Assert.AreEqual(o.Ref.RefStream1.EditLink, context.GetReadStreamUri(o.Ref, "RefStream1"), "the stream url for RefStream1 must be populated correctly")
                For Each c In o.Collection
                    Assert.AreEqual(c.ColStream.EditLink, context.GetReadStreamUri(c, "ColStream"), "the url for the nested collection entity should match")
                Next
            Next

            Assert.AreEqual(context.Entities.Count, 4, "there should be 2 entities tracked by the context")
            Assert.AreEqual(context.Entities(0).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)", "top level entity must be tracked")
            Assert.AreEqual(context.Entities(1).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet2(3)", "the nested entity must be tracked")
            Assert.AreEqual(context.Entities(2).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('ABCDE')", "top level entity must be tracked")
            Assert.AreEqual(context.Entities(3).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('XYZ')", "the nested entity must be tracked")
        End If

        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New With {
                     .Name = s.Name,
                     .Stream1Url = s.Stream1,
                     .Ref = New With {
                      .Name = s.Ref.Name,
                      .Stream1Url = s.Ref.RefStream1
                     },
                     .Collection = (From c In s.Collection
                                    Select New With {
                       .Name = c.Name,
                       .Stream1Url = c.ColStream
                      }).ToList()
                    }

            Assert.AreEqual(Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$expand=Ref($select=Name),Ref($select=RefStream1),Collection($select=Name),Collection($select=ColStream)&$select=Name,Stream1", q.ToString(), "make sure the right uri is produced by the linq translator")

            For Each o In q
                Assert.AreEqual(o.Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)/Stream1", "the stream url for Stream1 must be populated correctly")
                Assert.AreEqual(o.Ref.Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet2(3)/RefStream1", "the stream url for RefStream1 must be populated correctly")
                Assert.AreEqual(o.Collection.First().Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('ABCDE')/ColStream", "the stream url of the collection stream must be populated correctly - index 0")
                Assert.AreEqual(o.Collection.ElementAt(1).Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('XYZ')/ColStream", "the stream url of the collection stream must be populated correctly - index 1")
            Next

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context")
        End If
    End Sub

    <TestCategory("Partition2")> <TestMethod(), Variation("projecting out collection of collection properties - both narrow type and anonymous type")>
    Public Sub NamedStreams_NestedQuery_2()
        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New StreamType1() With {
                     .ID = s.ID,
                     .Stream1 = s.Stream1,
                     .Collection = (From c In s.Collection
                                    Select New StreamType2() With {
                       .ID = c.ID,
                       .ColStream = c.ColStream,
                       .Collection1 = (From c1 In c.Collection1
                                       Select New StreamType1() With {
                         .ID = c1.ID,
                         .RefStream1 = c1.RefStream1
                        }).ToList()
                      }).ToList()
                    }

            Assert.AreEqual(Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$expand=Collection($select=ID),Collection($select=ColStream),Collection($expand=Collection1($select=ID)),Collection($expand=Collection1($select=RefStream1))&$select=ID,Stream1", q.ToString(), "make sure the right uri is produced by the linq translator")

            For Each o In q
                Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the stream url for Stream1 must be populated correctly")
                For Each c In o.Collection
                    Assert.AreEqual(c.ColStream.EditLink, context.GetReadStreamUri(c, "ColStream"), "the url for the nested collection entity should match - Level 0")
                    For Each c1 In c.Collection1
                        Assert.AreEqual(c1.RefStream1.EditLink, context.GetReadStreamUri(c1, "RefStream1"), "the url for the nested collection entity should match - Level 1")
                    Next
                Next
            Next

            Assert.AreEqual(context.Entities.Count, 4, "there should be 4 entities tracked by the context")
            Assert.AreEqual(context.Entities(0).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)", "top level entity must be tracked")
            Assert.AreEqual(context.Entities(1).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('ABCDE')", "top level entity must be tracked")
            Assert.AreEqual(context.Entities(2).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet2(3)", "the nested entity must be tracked")
            Assert.AreEqual(context.Entities(3).EditLink.AbsoluteUri, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('XYZ')", "the nested entity must be tracked")
        End If

        If True Then
            ' Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
            Dim context As New DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = From s In context.CreateQuery(Of StreamType1)("MySet1")
                    Select New With {
                     .Name = s.Name,
                     .Stream1Url = s.Stream1,
                     .Collection = (From c In s.Collection
                                    Select New With {
                       .Name = c.Name,
                       .Stream1Url = c.ColStream,
                       .Collection1 = (From c1 In c.Collection1
                                       Select New With {
                         .Name = c1.Name,
                         .Stream1Url = c1.RefStream1
                        }).ToList()
                      }).ToList()
                    }

            Assert.AreEqual(Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1?$expand=Collection($select=Name),Collection($select=ColStream),Collection($expand=Collection1($select=Name)),Collection($expand=Collection1($select=RefStream1))&$select=Name,Stream1", q.ToString(), "make sure the right uri is produced by the linq translator")

            For Each o In q
                Assert.AreEqual(o.Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet1(1)/Stream1", "the stream url for Stream1 must be populated correctly")
                Assert.AreEqual(o.Collection.First().Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('ABCDE')/ColStream", "the stream url of the collection stream must be populated correctly - index 0")
                Assert.AreEqual(o.Collection.First().Collection1.[Single]().Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet2(3)/RefStream1", "the stream url of the collection stream must be populated correctly - index 0 - index 0")
                Assert.AreEqual(o.Collection.ElementAt(1).Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet3('XYZ')/ColStream", "the stream url of the collection stream must be populated correctly - index 1")

                Assert.AreEqual(o.Collection.ElementAt(1).Collection1.[Single]().Stream1Url.EditLink, Convert.ToString(request.ServiceRoot.AbsoluteUri) & "/MySet2(3)/RefStream1", "the stream url of the collection stream must be populated correctly - index 1 - index 0")
            Next

            Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context")
        End If
    End Sub

    Private Shared Function SetUpNamedStreamService() As DSPServiceDefinition
        Dim metadata As New DSPMetadata("NamedStreamIDSPContainer", "NamedStreamTest")

        ' entity with streams
        Dim entityWithNamedStreams As p.ResourceType = metadata.AddEntityType("EntityWithNamedStreams", Nothing, Nothing, False)
        metadata.AddKeyProperty(entityWithNamedStreams, "ID", GetType(Integer))
        metadata.AddPrimitiveProperty(entityWithNamedStreams, "Name", GetType(String))
        Dim streamInfo1 As New p.ResourceProperty("Stream1", p.ResourcePropertyKind.Stream, p.ResourceType.GetPrimitiveResourceType(GetType(System.IO.Stream)))
        entityWithNamedStreams.AddProperty(streamInfo1)
        Dim streamInfo2 As New p.ResourceProperty("Stream2", p.ResourcePropertyKind.Stream, p.ResourceType.GetPrimitiveResourceType(GetType(System.IO.Stream)))
        entityWithNamedStreams.AddProperty(streamInfo2)

        ' entity with streams2
        Dim entityWithNamedStreams1 As p.ResourceType = metadata.AddEntityType("EntityWithNamedStreams1", Nothing, Nothing, False)
        metadata.AddKeyProperty(entityWithNamedStreams1, "ID", GetType(Integer))
        metadata.AddPrimitiveProperty(entityWithNamedStreams1, "Name", GetType(String))
        Dim refStreamInfo1 As New p.ResourceProperty("RefStream1", p.ResourcePropertyKind.Stream, p.ResourceType.GetPrimitiveResourceType(GetType(System.IO.Stream)))
        entityWithNamedStreams1.AddProperty(refStreamInfo1)

        ' entity with streams2
        Dim entityWithNamedStreams2 As p.ResourceType = metadata.AddEntityType("EntityWithNamedStreams2", Nothing, Nothing, False)
        metadata.AddKeyProperty(entityWithNamedStreams2, "ID", GetType(String))
        metadata.AddPrimitiveProperty(entityWithNamedStreams2, "Name", GetType(String))
        Dim collectionStreamInfo As New p.ResourceProperty("ColStream", p.ResourcePropertyKind.Stream, p.ResourceType.GetPrimitiveResourceType(GetType(System.IO.Stream)))
        entityWithNamedStreams2.AddProperty(collectionStreamInfo)

        Dim set1 As p.ResourceSet = metadata.AddResourceSet("MySet1", entityWithNamedStreams)
        Dim set2 As p.ResourceSet = metadata.AddResourceSet("MySet2", entityWithNamedStreams1)
        Dim set3 As p.ResourceSet = metadata.AddResourceSet("MySet3", entityWithNamedStreams2)

        ' add navigation property to entityWithNamedStreams
        metadata.AddResourceReferenceProperty(entityWithNamedStreams, "Ref", set2, entityWithNamedStreams1)
        metadata.AddResourceSetReferenceProperty(entityWithNamedStreams, "Collection", set3, entityWithNamedStreams2)
        metadata.AddResourceSetReferenceProperty(entityWithNamedStreams2, "Collection1", set2, entityWithNamedStreams1)

        Dim service As New DSPServiceDefinition()
        service.Metadata = metadata
        service.MediaResourceStorage = New DSPMediaResourceStorage()
        service.SupportMediaResource = True
        service.SupportNamedStream = True
        service.ForceVerboseErrors = True

        ' populate data
        Dim context As New DSPContext()

        Dim entity1 As New DSPResource(entityWithNamedStreams)
        If True Then
            context.GetResourceSetEntities("MySet1").Add(entity1)

            entity1.SetValue("ID", 1)
            entity1.SetValue("Name", "Foo")
            Dim namedStream1 As DSPMediaResource = service.MediaResourceStorage.CreateMediaResource(entity1, streamInfo1)
            namedStream1.ContentType = "image/jpeg"
            Dim data1 As Byte() = New Byte() {0, 1, 2, 3, 4}
            namedStream1.GetWriteStream().Write(data1, 0, data1.Length)

            Dim namedStream2 As DSPMediaResource = service.MediaResourceStorage.CreateMediaResource(entity1, streamInfo2)
            namedStream2.ContentType = "image/jpeg"
            Dim data2 As Byte() = New Byte() {0, 1, 2, 3, 4}
            namedStream2.GetWriteStream().Write(data2, 0, data2.Length)
        End If

        Dim entity2 As New DSPResource(entityWithNamedStreams1)
        If True Then
            context.GetResourceSetEntities("MySet2").Add(entity2)

            entity2.SetValue("ID", 3)
            entity2.SetValue("Name", "Bar")
            Dim refNamedStream1 As DSPMediaResource = service.MediaResourceStorage.CreateMediaResource(entity2, refStreamInfo1)
            refNamedStream1.ContentType = "image/jpeg"
            Dim data1 As Byte() = New Byte() {0, 1, 2, 3, 4}
            refNamedStream1.GetWriteStream().Write(data1, 0, data1.Length)

            ' set the navigation property
            entity1.SetValue("Ref", entity2)
        End If

        If True Then
            Dim entity3 As New DSPResource(entityWithNamedStreams2)
            context.GetResourceSetEntities("MySet3")

            entity3.SetValue("ID", "ABCDE")
            entity3.SetValue("Name", "Bar")
            Dim stream As DSPMediaResource = service.MediaResourceStorage.CreateMediaResource(entity3, collectionStreamInfo)
            stream.ContentType = "image/jpeg"
            Dim data1 As Byte() = New Byte() {0, 1, 2, 3, 4}
            stream.GetWriteStream().Write(data1, 0, data1.Length)
            Dim collection1 = New List(Of DSPResource)()
            collection1.Add(entity2)
            entity3.SetValue("Collection1", collection1)

            Dim entity4 As New DSPResource(entityWithNamedStreams2)
            context.GetResourceSetEntities("MySet3")

            entity4.SetValue("ID", "XYZ")
            entity4.SetValue("Name", "Foo")
            Dim stream1 As DSPMediaResource = service.MediaResourceStorage.CreateMediaResource(entity3, collectionStreamInfo)
            stream1.ContentType = "image/jpeg"
            stream1.GetWriteStream().Write(data1, 0, data1.Length)
            Dim collection2 = New List(Of DSPResource)()
            collection2.Add(entity2)
            entity4.SetValue("Collection1", collection2)

            Dim collection3 = New List(Of DSPResource)()
            collection3.Add(entity3)
            collection3.Add(entity4)
            entity1.SetValue("Collection", collection3)
        End If

        service.CreateDataSource = Function(m)
                                       Return context
                                   End Function

        Return service
    End Function

    Private Class StreamType1
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
        Public Property Stream1() As DataServiceStreamLink
            Get
                Return m_Stream1
            End Get
            Set(ByVal value As DataServiceStreamLink)
                m_Stream1 = value
            End Set
        End Property
        Private m_Stream1 As DataServiceStreamLink
        Public Property Stream2() As DataServiceStreamLink
            Get
                Return m_Stream2
            End Get
            Set(ByVal value As DataServiceStreamLink)
                m_Stream2 = value
            End Set
        End Property
        Private m_Stream2 As DataServiceStreamLink
        Public Property Ref() As StreamType1
            Get
                Return m_Ref
            End Get
            Set(ByVal value As StreamType1)
                m_Ref = value
            End Set
        End Property
        Private m_Ref As StreamType1
        Public Property Collection() As List(Of StreamType2)
            Get
                Return m_Collection
            End Get
            Set(ByVal value As List(Of StreamType2))
                m_Collection = value
            End Set
        End Property
        Private m_Collection As List(Of StreamType2)

        Private m_RefStream1 As DataServiceStreamLink
        Public Property RefStream1 As DataServiceStreamLink
            Get
                Return m_RefStream1
            End Get
            Set(ByVal value As DataServiceStreamLink)
                m_RefStream1 = value
            End Set
        End Property

    End Class

    Private Class StreamType2
        Public Property ID() As String
            Get
                Return m_ID
            End Get
            Set(ByVal value As String)
                m_ID = value
            End Set
        End Property
        Private m_ID As String
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal value As String)
                m_Name = value
            End Set
        End Property
        Private m_Name As String
        Public Property Stream1() As DataServiceStreamLink
            Get
                Return m_Stream1Url
            End Get
            Set(ByVal value As DataServiceStreamLink)
                m_Stream1Url = value
            End Set
        End Property
        Private m_Stream1Url As DataServiceStreamLink
        Public Property Collection1() As List(Of StreamType1)
            Get
                Return m_Collection1
            End Get
            Set(ByVal value As List(Of StreamType1))
                m_Collection1 = value
            End Set
        End Property
        Private m_Collection1 As List(Of StreamType1)

        Private m_ColStream As DataServiceStreamLink
        Public Property ColStream As DataServiceStreamLink
            Get
                Return m_ColStream
            End Get
            Set(ByVal value As DataServiceStreamLink)
                m_ColStream = value
            End Set
        End Property
    End Class

    Private Class StreamWithUrl
        Public Property ID() As Integer
            Get
                Return m_ID
            End Get
            Set(ByVal value As Integer)
                m_ID = value
            End Set
        End Property
        Private m_ID As Integer
        Public Property Stream1() As DataServiceStreamLink
            Get
                Return m_Stream1
            End Get
            Set(ByVal value As DataServiceStreamLink)
                m_Stream1 = value
            End Set
        End Property
        Private m_Stream1 As DataServiceStreamLink
        Public Property SomeRandomProperty() As DataServiceStreamLink
            Get
                Return m_SomeRandomProperty
            End Get
            Set(ByVal value As DataServiceStreamLink)
                m_SomeRandomProperty = value
            End Set
        End Property
        Private m_SomeRandomProperty As DataServiceStreamLink
    End Class
End Class
