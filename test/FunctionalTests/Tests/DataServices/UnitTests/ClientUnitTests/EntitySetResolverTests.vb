'---------------------------------------------------------------------
' <copyright file="EntitySetResolverTests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Option Explicit On

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports AstoriaUnitTests.Stubs
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Linq
Imports AstoriaUnitTests.Tests
Imports System.Reflection
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests
Imports System.Web
Imports System.IO

Partial Public Class ClientModule

    <TestClass()>
    Public Class EntitySetResolverTests
        Inherits AstoriaTestCase
        Private Shared streamingWeb As TestWebRequest = Nothing
        Private Shared streamContentWeb As TestWebRequest = Nothing

        Private Shared northwindWeb As TestWebRequest = Nothing
        Private Shared resolvedNames As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
        Private northwindCtx As NorthwindSimpleModel.NorthwindContext = Nothing

        Private Shared web1 As TestWebRequest = Nothing
        Private Shared entitySet1AbsoluteUri As Uri = Nothing
        Private Shared clientAssembly As Assembly = Nothing

        Private Shared InvalidUriVariations As Tuple(Of String, Uri)() = New Tuple(Of String, Uri)() {
            New Tuple(Of String, Uri)("Uri has Fragment", New Uri("http://foo/awesome.svc#MyBookmark", UriKind.Absolute)),
            New Tuple(Of String, Uri)("Uri has Query", New Uri("http://foo/awesome.svc?id=10", UriKind.Absolute)),
            New Tuple(Of String, Uri)("Uri is Relative", New Uri("MySet", UriKind.Relative))}

        <CLSCompliant(False)>
        Public Class MyAllTypes1
            Inherits AllTypes
        End Class
        <CLSCompliant(False)>
        Public Class MyAllTypes2
            Inherits AllTypes
        End Class


        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web1 = TestWebRequest.CreateForInProcessWcf
            web1.DataServiceType = GetType(TypedCustomDataContext(Of MyAllTypes1))
            web1.StartService()
            entitySet1AbsoluteUri = New Uri(web1.BaseUri + "/Values", UriKind.Absolute)
            clientAssembly = GetType(DataServiceContext).Assembly

            BaseTestWebRequest.SerializedTestArguments.Clear()
            northwindWeb = TestWebRequest.CreateForInProcessWcf
            northwindWeb.DataServiceType = ServiceModelData.Northwind.ServiceModelType
            northwindWeb.StartService()

            streamingWeb = TestWebRequest.CreateForInProcessWcf
            streamingWeb.ServiceType = GetType(AstoriaUnitTests.Stubs.StreamingService)
            streamingWeb.StartService()

            streamContentWeb = TestWebRequest.CreateForInProcessWcf
            streamContentWeb.ServiceType = GetType(AstoriaUnitTests.Stubs.StreamingContentService)
            streamContentWeb.StartService()
            Dim ctx = New DataServiceContext(streamingWeb.ServiceRoot)

            ' Since we are sending $value payload, we must correctly set the accept type header.
            ' Otherwise, astoria server sends application/atom+xml as the header value and then client fails to parse.
            AddHandler ctx.SendingRequest2, AddressOf SetAcceptHeader
            ctx.Execute(Of Boolean)(New Uri("/SetContentServiceUri/$value?uri='" + HttpUtility.UrlEncode(streamContentWeb.ServiceRoot.ToString() + "'"),
                                            UriKind.Relative))
            RemoveHandler ctx.SendingRequest2, AddressOf SetAcceptHeader
        End Sub

        Public Shared Sub SetAcceptHeader(ByVal sender As Object, ByVal args As SendingRequest2EventArgs)
            args.RequestMessage.SetHeader("Accept", "text/plain")
        End Sub


        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web1 Is Nothing Then
                web1.StopService()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Me.northwindCtx = New NorthwindSimpleModel.NorthwindContext(northwindWeb.ServiceRoot)
            'Me.northwindCtx.EnableAtom = True
            'Me.northwindCtx.Format.UseAtom()
        End Sub

        '<TestCleanup()> Public Sub PerTestCleanup()
        'End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextBeginExecute_NullBaseUri_Error()
            Dim context = New DataServiceContext()
            Dim act As Action = Sub()
                                    context.BeginExecute(Of Object)(New Uri("MySet", UriKind.Relative), Sub(ar As IAsyncResult) Return, Nothing)
                                End Sub
            AssertUtil.RunCatch(Of InvalidOperationException)(act,
                                                            clientAssembly,
                                                            "Context_RequestUriIsRelativeBaseUriRequired")

        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextBeginExecute_NullBaseUri_AbsoluteRequestUri_Success()
            Dim context = New DataServiceContext()
            'context.EnableAtom = True
            'context.Format.UseAtom()
            context.BeginExecute(Of Object)(entitySet1AbsoluteUri, Sub(ar As IAsyncResult)
                                                                       CType(ar, DataServiceQuery(Of Object)).EndExecute(ar).ToList()
                                                                   End Sub,
                                                                   Nothing)
        End Sub
        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextExecute_NullBaseUri_AbsoluteRequestUri_Success()
            Dim context = New DataServiceContext()
            'context.EnableAtom = True
            'context.Format.UseAtom()
            context.Execute(Of Object)(entitySet1AbsoluteUri).ToList()
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextExecute_NullBaseUri_Error()
            Dim context = New DataServiceContext()
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim act As Action = Sub()
                                    context.Execute(Of Object)(New Uri("MySet", UriKind.Relative))
                                End Sub
            AssertUtil.RunCatch(Of InvalidOperationException)(act,
                                                            clientAssembly,
                                                            "Context_RequestUriIsRelativeBaseUriRequired")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextGetMetadataUri_NullBaseUri_Error()
            Dim context = New DataServiceContext()
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim act As Action = Sub()
                                    context.GetMetadataUri()
                                End Sub
            AssertUtil.RunCatch(Of InvalidOperationException)(act,
                                                            clientAssembly,
                                                            "Context_BaseUriRequired")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextBaseUriProperty_InvalidUri_Error()
            For Each loopVariation In InvalidUriVariations
                ' use a local variable to keep the loop var from being captured by the delegate
                Dim variation = loopVariation

                Console.WriteLine("Running variation - " + variation.Item1)
                Dim act As Action = Sub()
                                        Dim context = New DataServiceContext()
                                        context.BaseUri = variation.Item2
                                    End Sub
                AssertUtil.RunCatch(Of InvalidOperationException)(act,
                                                                clientAssembly,
                                                                "Context_BaseUri")
            Next
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextBaseUriStateEquvilentBetweenDefaultCtorAndNullPassedToBaseUriCtor()
            Dim context = New DataServiceContext()
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim contextNull = New DataServiceContext(Nothing)

            Assert.AreEqual(context.BaseUri, contextNull.BaseUri, "the base uri's should be equivelent")
            Assert.AreEqual(context.ResolveEntitySet, contextNull.ResolveEntitySet, "the base uri's should be equivelent")

        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextBaseUriProperty()
            Dim context = New DataServiceContext()
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Assert.IsNull(context.BaseUri, "The BaseUri property didn't default to null")

            Dim absoluteUri As Uri = New Uri("http://foo.com/Awesome.svc")
            context.BaseUri = absoluteUri
            Assert.AreEqual(absoluteUri, context.BaseUri, "the BaseUri property didn't set properly")

            context.BaseUri = Nothing
            Assert.IsNull(context.BaseUri, "The BaseUri property didn't set back to null")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextBaseUriProperty_SetAfterCtorSet()
            Dim oldUri = New Uri("http://foo.com/Old.svc")
            Dim context = New DataServiceContext(oldUri)
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Assert.AreEqual(oldUri, context.BaseUri, "The BaseUri property didn't set from the ctor properly")

            Dim absoluteUri As Uri = New Uri("http://foo.com/Awesome.svc")
            context.BaseUri = absoluteUri
            Assert.AreEqual(absoluteUri, context.BaseUri, "the BaseUri property didn't set properly")

            context.BaseUri = Nothing
            Assert.IsNull(context.BaseUri, "The BaseUri property didn't set back to null")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextResolveEntitySetProperty()
            Const absoluteUri As String = "http://foo.com/Awesome.svc"
            Dim context = New DataServiceContext()
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim esr As Func(Of String, Uri) = Function(setName As String) New Uri(absoluteUri)
            context.ResolveEntitySet = esr
            Assert.AreEqual(esr, context.ResolveEntitySet, "the ResolveEntitySet property didn't set properly")

            context.ResolveEntitySet = Nothing
            Assert.IsNull(context.ResolveEntitySet, "The ResolveEntitySet property didn't set back to null")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextCtorWithBaseUri_InvalidUri_Error()

            For Each loopVariation In InvalidUriVariations
                ' create a local var for the action to keep the loop var from
                ' being captured
                Dim variation = loopVariation
                Console.WriteLine("Running variation - " + variation.Item1)

                Dim act As Action = Sub()
                                        Dim context = New DataServiceContext(variation.Item2)
                                    End Sub
                AssertUtil.RunCatch(Of ArgumentException)(act, Sub(e As ArgumentException) StringAssert.StartsWith(e.Message, "Expected an absolute, well formed http URL without a query or fragment."))
            Next

        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextResolveEntitySetProperty_InvalidUri_Error()

            For Each loopVariation In InvalidUriVariations
                ' create a local var for the action to keep the loop var from
                ' being captured
                Dim variation = loopVariation
                Console.WriteLine("Running variation - " + variation.Item1)

                Dim act As Action = Sub()
                                        Dim context = New DataServiceContext()
                                        context.ResolveEntitySet = Function(setName As String)
                                                                       Dim uri = variation.Item2
                                                                       Return uri
                                                                   End Function
                                        context.AddObject("foo", New AllTypes())
                                    End Sub
                AssertUtil.RunCatch(Of InvalidOperationException)(act,
                                                                clientAssembly,
                                                                "Context_ResolveReturnedInvalidUri")
            Next

        End Sub


        Public Class MyTestException
            Inherits Exception
        End Class

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextResolveEntitySetProperty_Throws()
            ' we shouldn't catch or modify what the resolver is throwing
            Dim SetupContext As Func(Of DataServiceContext) = Function()
                                                                  Dim context = New DataServiceContext
                                                                  context.ResolveEntitySet = Function(setName As String)
                                                                                                 Throw New MyTestException
                                                                                             End Function
                                                                  Return context
                                                              End Function

            'Add
            Dim addObject As Action = Sub()
                                          SetupContext().AddObject("foo", New AllTypes())
                                      End Sub
            AssertUtil.RunCatch(Of MyTestException)(addObject)

            'CreateQuery
            Dim createQuery As Action = Sub()
                                            Dim query = SetupContext().CreateQuery(Of AllTypes)("foo")
                                            query.ToList()
                                        End Sub
            AssertUtil.RunCatch(Of MyTestException)(createQuery)

            'Attach
            Dim attachTo As Action = Sub()
                                         SetupContext().AttachTo("foo", New AllTypes)
                                     End Sub
            AssertUtil.RunCatch(Of MyTestException)(attachTo)
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextResolveEntitySetProperty_DefaultsToNull()
            Dim context As DataServiceContext = New DataServiceContext()
            Assert.IsNull(context.ResolveEntitySet, "The default value of the ResolveEntitySetProperty should be null")

            context = New DataServiceContext(New Uri("http://mudd", UriKind.Absolute))
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Assert.IsNull(context.ResolveEntitySet, "The default value of the ResolveEntitySetProperty should be null")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub ApiContextResolveEntitySetProperty_ReturnsNull()
            ' should just use the BaseUri
            Dim context = New DataServiceContext(New Uri("http://foo/ReturnsNull"))
            'context.EnableAtom = True
            'context.Format.UseAtom()
            context.ResolveEntitySet = Function(setName As String)
                                           Return Nothing
                                       End Function
            context.AttachTo("MySet", New AllTypes())
            Assert.IsTrue(context.Entities.Single().EditLink.OriginalString.StartsWith(context.BaseUri.OriginalString), "the baseuri is not correct")
        End Sub

        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub LinqQueryResolvesUriOnExecuteWhenReused()
            ' be sure that a change to the baseUri after one execute is picked up on the next
            TypedCustomDataContext(Of MyAllTypes1).ClearData()
            TypedCustomDataContext(Of MyAllTypes1).CreateChangeScope()

            Dim allTypes1 = New TypedCustomDataContext(Of MyAllTypes1)()
            allTypes1.SetValues(New MyAllTypes1() {New MyAllTypes1 With {.ID = 100, .StringType = "foo1"}})

            Dim context = New DataServiceContext()
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Dim q = context.CreateQuery(Of MyAllTypes1)("Values").Where(Function(a As MyAllTypes1) a.StringType = "foo1")
            Dim callCount As Integer = 0
            context.ResolveEntitySet = Function(name As String)
                                           callCount = callCount + 1
                                           Return New Uri(web1.BaseUri + "/" + name)
                                       End Function

            ' note that this won't work with enumerating because translation is cached
            Dim type1 = q.Single()

            Assert.AreEqual(1, callCount, "the ESR was not called")
            Assert.AreEqual(type1.StringType, "foo1", "didn't get the correct object from the query")

            context.ResolveEntitySet = Function(name As String)
                                           callCount = callCount + 1
                                           Return New Uri("http://localhost/myfake.svc")
                                       End Function

            Dim act = Sub()
                          ' note that this won't work with enumerating because translation is cached
                          q.Single()
                      End Sub
            AssertUtil.RunCatch(Of DataServiceQueryException)(act, Sub(e As DataServiceQueryException)
                                                                       Assert.IsNotNull(e.InnerException, "Wrong excpetion, the correct one will have an InnerException")
                                                                       Assert.IsTrue(e.InnerException.Message.Contains("NotFound") Or e.InnerException.Message.Contains("Not Found"), "the error message does not contain the correct information")
                                                                   End Sub)
            Assert.AreEqual(2, callCount, "the ESR was not called a second time")
        End Sub

        '<TestMethod()> _
        'Public Sub LinqQueryResolvesUriOnlyOnToString()
        '    Dim fakeUri As Uri = New Uri("http://bad.fake.uri/myfake.svc", UriKind.Absolute)
        '    Dim context = New DataServiceContext(fakeUri)
        '    Dim q = context.CreateQuery(Of MyAllTypes1)("SetName").Where(Function(a As MyAllTypes1) a.StringType = "foo1")
        '    StringAssert.Contains(q.ToString(), fakeUri.OriginalString, "didn't use the baseUri")

        '    context.BaseUri = New Uri(web1.BaseUri, UriKind.Absolute)
        '    StringAssert.Contains(q.ToString(), web1.BaseUri, "didn't use the latest baseUri")
        'End Sub
        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub SetSaveStreamDoesNotCallEntitySetResolverOrBaseUri()
            ' Arange
            Dim ctx = New DataServiceContext(streamingWeb.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            ctx.Execute(Of Boolean)(New Uri("/ResetContent", UriKind.Relative)).Count()

            Dim photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            ctx.ResolveEntitySet = Nothing
            ctx.BaseUri = Nothing

            Dim contentBuffer As Byte() = New Byte() {1, 2, 3}
            Dim content = New MemoryStream(contentBuffer)

            ' verify that there isn't already a stream for this object
            Dim actualBuffer As Byte() = GetReadStreamContent(ctx, photo)
            If actualBuffer.Length = contentBuffer.Length Then
                Dim foundDifferent As Boolean = False
                For i As Integer = 0 To contentBuffer.Length - 1
                    If contentBuffer(i) <> actualBuffer(i) Then
                        foundDifferent = True
                        Exit For
                    End If
                Next
                Assert.IsTrue(foundDifferent, "the photo already has the stream that we are going to set it too")
            End If


            ' Act
            ctx.SetSaveStream(photo, content, False, "some/type", "Slug")
            ctx.SaveChanges()

            ' Assert
            ' verify that the data was actually saved
            ctx = New DataServiceContext(streamingWeb.ServiceRoot)
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            photo = ctx.Execute(Of StreamingServicePhoto)(New Uri("/Photos(1)", UriKind.Relative)).SingleOrDefault()
            actualBuffer = GetReadStreamContent(ctx, photo)
            Assert.AreEqual(contentBuffer.Length, actualBuffer.Length, "the stored stream is a different length")
            For i As Integer = 0 To contentBuffer.Length - 1
                Assert.AreEqual(contentBuffer(i), actualBuffer(i), "the data at index " & i.ToString() & " is different")
            Next

        End Sub

        Private Function GetReadStreamContent(ByVal ctx As DataServiceContext, ByVal entity As Object) As Byte()
            Dim byteList As List(Of Byte) = New List(Of Byte)
            Dim buffer As Byte() = New Byte(1024) {}
            Dim response As DataServiceStreamResponse = ctx.GetReadStream(entity)
            Dim count As Integer
            Do
                count = response.Stream.Read(buffer, 0, buffer.Length)
                If (count > 0) Then
                    byteList.AddRange(buffer.Take(count))
                End If
            Loop While count > 0
            Return byteList.ToArray()
        End Function
        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub LinkEntitiesDoesNotCallEntitySetResolverOrBaseUri()
            Dim newCust As NorthwindSimpleModel.Customers = New NorthwindSimpleModel.Customers() With {.CustomerID = "FOO01", .CompanyName = "Foo Store"}
            Dim newOrder As NorthwindSimpleModel.Orders = New NorthwindSimpleModel.Orders() With {.OrderID = 9999, .OrderDate = DateTime.Now}
            northwindCtx.AddObject("Customers", newCust)
            northwindCtx.AddObject("Orders", newOrder)
            northwindCtx.SaveChanges()

            northwindCtx.ResolveEntitySet = Nothing
            northwindCtx.BaseUri = Nothing

            ' AddLink doesn't use ESR or BaseUri
            northwindCtx.AddLink(newCust, "Orders", newOrder)
            northwindCtx.SaveChanges()

            ' DeleteLink doesn't use ESR or BaseUri either
            northwindCtx.DeleteLink(newCust, "Orders", newOrder)
            northwindCtx.SaveChanges()

            ' SetLink doesn't use ESR or BaseUri either
            northwindCtx.SetLink(newOrder, "Customers", newCust)
            northwindCtx.SaveChanges()

            ' SetLink doesn't use ESR or BaseUri either
            northwindCtx.SetLink(newOrder, "Customers", Nothing)
            northwindCtx.SaveChanges()
        End Sub
        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub ExecuteWithAbsoluteUriDoesNotCallEntitySetResolverOrBaseUri()
            Dim ctx = New DataServiceContext()
            'ctx.EnableAtom = True
            'ctx.Format.UseAtom()
            Assert.IsNull(ctx.BaseUri, "this test requires a null baseUri")
            Assert.IsNull(ctx.ResolveEntitySet, "this test requires a null resolver")

            Dim count As Integer = ctx.Execute(Of NorthwindSimpleModel.Orders)(New Uri(northwindWeb.BaseUri & "/Orders?$top=2", UriKind.Absolute)).Count()
            Assert.AreEqual(2, count, "execute should return the correct number of orders")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub LoadPropertyDoesNotCallEntitySetResolverOrBaseUri()
            Dim cust As NorthwindSimpleModel.Customers = northwindCtx.CreateQuery(Of NorthwindSimpleModel.Customers)("Customers").Execute().First()
            northwindCtx.BaseUri = Nothing
            northwindCtx.ResolveEntitySet = Nothing

            Assert.IsTrue(cust.Orders.Count = 0, "Orders were already filled in???")
            Dim response = northwindCtx.LoadProperty(cust, "Orders")
            Assert.IsFalse(cust.Orders.Count = 0, "No orders were loaded")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub LinqQueryResolvesUriOnlyOnExecute()
            Const entitySetName As String = "Values"
            'Arange
            TypedCustomDataContext(Of MyAllTypes1).ClearData()
            TypedCustomDataContext(Of MyAllTypes1).CreateChangeScope()
            Dim c = New TypedCustomDataContext(Of MyAllTypes1)()
            c.SetValues(New MyAllTypes1() {New MyAllTypes1 With {.ID = 100, .StringType = "foo1"}})

            ' context with bogus uri
            Dim context = New DataServiceContext(New Uri("http://bad.fake.uri/myfake.svc", UriKind.Absolute))
            'context.EnableAtom = True
            'context.Format.UseAtom()

            'Act
            Dim q = context.CreateQuery(Of MyAllTypes1)(entitySetName).Where(Function(a As MyAllTypes1) a.StringType = "foo1")
            ' set the good uri just before execute
            context.BaseUri = New Uri(web1.BaseUri, UriKind.Absolute)
            Dim readAllType = q.SingleOrDefault()
            Assert.IsNotNull(readAllType, "didn't read the entity we expected to find")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub NoBaseUriAndNoEntitySetResolverOnAddObject_Error()
            Dim act As Action = Sub()
                                    Dim context As New DataServiceContext()
                                    context.AddObject("foo", New MyAllTypes1())
                                End Sub
            AssertUtil.RunCatch(Of InvalidOperationException)(act,
                                                clientAssembly,
                                                "Context_ResolveEntitySetOrBaseUriRequired",
                                                "foo")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub CaptureUriDuringAddObjectCall()
            'Arrange
            TypedCustomDataContext(Of MyAllTypes1).ClearData()
            TypedCustomDataContext(Of MyAllTypes1).CreateChangeScope()
            Assert.IsNull(TypedCustomDataContext(Of MyAllTypes1).CurrentValues, "the current values didn't start off non null")

            ' context with good uri
            Dim context = New DataServiceContext(New Uri(web1.BaseUri, UriKind.Absolute))
            'context.EnableAtom = True
            'context.Format.UseAtom()
            context.ResolveEntitySet = Function(entitySetName As String) New Uri(context.BaseUri.OriginalString + "/Values", UriKind.Absolute)


            'Act
            Dim item = New MyAllTypes1 With {.ID = 100, .StringType = "foo1"}
            context.AddObject("NA", item)

            ' set the context to NO uri
            context.BaseUri = Nothing
            context.ResolveEntitySet = Nothing
            context.SaveChanges()

            'Assert
            Assert.IsNotNull(TypedCustomDataContext(Of MyAllTypes1).CurrentValues, "no object was added")
            Assert.AreEqual(1, TypedCustomDataContext(Of MyAllTypes1).CurrentValues.Count, "Wrong number of objects added")
        End Sub

        <TestCategory("Partition3")> <TestMethod()>
        Public Sub CaptureUriDuringAttachToCall()
            'Arrange
            TypedCustomDataContext(Of MyAllTypes1).ClearData()
            TypedCustomDataContext(Of MyAllTypes1).CreateChangeScope()
            Assert.IsNull(TypedCustomDataContext(Of MyAllTypes1).CurrentValues, "the current values didn't start off non null")
            Dim allTypes1 = New TypedCustomDataContext(Of MyAllTypes1)()
            allTypes1.SetValues(New MyAllTypes1() {New MyAllTypes1 With {.ID = 100, .StringType = "foo1"}})

            ' context with good uri
            Dim context = New DataServiceContext(New Uri(web1.BaseUri, UriKind.Absolute))
            'context.EnableAtom = True
            'context.Format.UseAtom()
            context.ResolveEntitySet = Function(entitySetName As String) New Uri(context.BaseUri.OriginalString + "/Values", UriKind.Absolute)


            'Act
            Dim item = New MyAllTypes1 With {.ID = 100}
            context.AttachTo("NA", item)


            ' set the context to NO uri
            context.BaseUri = Nothing
            context.ResolveEntitySet = Nothing

            item.StringType = "NewFoo"
            context.UpdateObject(item)
            context.SaveChanges()

            'Assert
            Assert.IsNotNull(TypedCustomDataContext(Of MyAllTypes1).CurrentValues, "no object was added")
            Assert.AreEqual(1, TypedCustomDataContext(Of MyAllTypes1).CurrentValues.Count, "Wrong number of objects added")
            Assert.AreEqual("NewFoo", TypedCustomDataContext(Of MyAllTypes1).CurrentValues.First().StringType, "The update was not saved")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub ExecuteBatchWithRelativeUri()
            ' Arrange
            TypedCustomDataContext(Of MyAllTypes1).ClearData()
            TypedCustomDataContext(Of MyAllTypes1).CreateChangeScope()
            Dim allTypes1 = New TypedCustomDataContext(Of MyAllTypes1)()
            allTypes1.SetValues(New MyAllTypes1() {New MyAllTypes1 With {.ID = 100, .StringType = "foo1"}})

            Dim context As DataServiceContext = New DataServiceContext(New Uri(web1.BaseUri, UriKind.Absolute))
            'context.EnableAtom = True
            'context.Format.UseAtom()
            Assert.IsNull(context.ResolveEntitySet, "we should not have a dependence on ResolveEntitySet for the BatchExecute method")
            Dim request = New DataServiceRequest(Of MyAllTypes1)(New Uri("Values", UriKind.Relative))

            ' Act 
            Dim batchResponse As DataServiceResponse = context.ExecuteBatch(request)

            ' Assert
            For Each response As QueryOperationResponse(Of MyAllTypes1) In batchResponse
                Dim value = response.SingleOrDefault()
                Assert.IsNotNull(value, "didn't get any values back")
                Assert.AreEqual(value.ID, 100, "got the wrong data back")
            Next
        End Sub

        'Remove Atom
        ' <TestCategory("Partition3")> <TestMethod()>
        Public Sub SimpleCRUDTest()

            For Each mode As SaveChangesMode In [Enum].GetValues(GetType(SaveChangesMode))
                ' start a new web service so that we can use a different endpoint to make sure client uses the new endpoint
                ' and not the existing one from which the context was created.
                Using web2 As TestWebRequest = TestWebRequest.CreateForInProcessWcf
                    web2.DataServiceType = GetType(TypedCustomDataContext(Of MyAllTypes2))
                    web2.StartService()

                    TypedCustomDataContext(Of MyAllTypes1).ClearData()
                    TypedCustomDataContext(Of MyAllTypes1).CreateChangeScope()

                    TypedCustomDataContext(Of MyAllTypes2).ClearData()
                    TypedCustomDataContext(Of MyAllTypes2).CreateChangeScope()


                    Const web1SetName As String = "Type1"
                    Const web2SetName As String = "Type2"
                    Dim context = New DataServiceContext()
                    'context.EnableAtom = True
                    'context.Format.UseAtom()
                    context.ResolveEntitySet = Function(name As String)
                                                   Dim baseUri As Uri
                                                   If name = web1SetName Then
                                                       baseUri = New Uri(web1.BaseUri + "/", UriKind.Absolute)
                                                   ElseIf name = web2SetName Then
                                                       baseUri = New Uri(web2.BaseUri + "/", UriKind.Absolute)
                                                   Else
                                                       Throw New InvalidOperationException("invalid EntitySetName")
                                                   End If
                                                   Dim entitySetUri = New Uri(baseUri, New Uri("Values", UriKind.Relative))
                                                   Return entitySetUri
                                               End Function

                    'Create
                    Dim allType1 = New MyAllTypes1 With {.ID = 100, .StringType = "foo1"}
                    Dim allType2 = New MyAllTypes2 With {.ID = 200, .StringType = "foo2"}
                    context.AddObject(web1SetName, allType1)
                    context.AddObject(web2SetName, allType2)
                    DataServiceContextTestUtil.SaveChanges(context, context.SaveChangesDefaultOptions, mode)


                    Assert.AreEqual(1, TypedCustomDataContext(Of MyAllTypes1).CurrentValues.Count, "Wrong number of values in the Web1 resource set")
                    Assert.AreEqual(1, TypedCustomDataContext(Of MyAllTypes2).CurrentValues.Count, "Wrong number of values in the Web2 resource set")

                    Assert.AreEqual(100, TypedCustomDataContext(Of MyAllTypes1).CurrentValues.First().ID, "The type was not created in the web1 server correctly")
                    Assert.AreEqual(200, TypedCustomDataContext(Of MyAllTypes2).CurrentValues.First().ID, "The type was not created in the web2 server correctly")

                    'Read
                    Dim readType1 As MyAllTypes1 = context.CreateQuery(Of MyAllTypes1)(web1SetName).FirstOrDefault()
                    Dim readType2 As MyAllTypes2 = context.CreateQuery(Of MyAllTypes2)(web2SetName).FirstOrDefault()


                    Assert.IsNotNull(readType1, "no values contained in web1")
                    Assert.IsNotNull(readType2, "no values contained in web2")

                    Assert.AreEqual(100, readType1.ID, "An object with the wrong id was read")
                    Assert.AreEqual(200, readType2.ID, "An object with the wrong id was read")

                    'Update
                    Assert.AreNotSame(readType1, TypedCustomDataContext(Of MyAllTypes1).CurrentValues.First(), "In memory should still get different object instances because of the serialization/deserialization process")
                    Assert.AreNotSame(readType2, TypedCustomDataContext(Of MyAllTypes2).CurrentValues.First(), "In memory should still get different object instances because of the serialization/deserialization process")

                    readType1.StringType = "updated1"
                    readType2.StringType = "updated2"
                    context.UpdateObject(readType1)
                    context.UpdateObject(readType2)
                    DataServiceContextTestUtil.SaveChanges(context, context.SaveChangesDefaultOptions, mode)

                    Assert.AreEqual("updated1", TypedCustomDataContext(Of MyAllTypes1).CurrentValues.First().StringType, "the StringType property of web1 was not updated")
                    Assert.AreEqual("updated2", TypedCustomDataContext(Of MyAllTypes2).CurrentValues.First().StringType, "the StringType property of web2 was not updated")

                    'Delete
                    context.DeleteObject(readType1)
                    DataServiceContextTestUtil.SaveChanges(context, context.SaveChangesDefaultOptions, mode)
                    Assert.AreEqual(0, TypedCustomDataContext(Of MyAllTypes1).CurrentValues.Count, "Wrong number of values in the Web1 resource set")
                    Assert.AreEqual(1, TypedCustomDataContext(Of MyAllTypes2).CurrentValues.Count, "Wrong number of values in the Web2 resource set")

                    context.DeleteObject(readType2)
                    DataServiceContextTestUtil.SaveChanges(context, context.SaveChangesDefaultOptions, mode)
                    Assert.AreEqual(0, TypedCustomDataContext(Of MyAllTypes2).CurrentValues.Count, "Wrong number of values in the Web2 resource set")
                End Using

            Next

        End Sub



    End Class
End Class
