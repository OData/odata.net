'---------------------------------------------------------------------
' <copyright file="Util.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Diagnostics
Imports System.Text
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml
Imports System.Xml.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Threading

Module DataServiceContextExtensions

    <System.Runtime.CompilerServices.Extension()>
    Public Function CreateUri(Of T)(ByVal ctx As DataServiceContext, ByVal request As String) As DataServiceRequest(Of T)
        Return ctx.CreateUri(Of T)(request, False)
    End Function

    <System.Runtime.CompilerServices.Extension()>
    Public Function CreateUri(Of T)(ByVal ctx As DataServiceContext, ByVal request As String, ByVal combine As Boolean) As DataServiceRequest(Of T)
        Dim requestUri = New Uri(request, UriKind.RelativeOrAbsolute)
        If combine Then
            If Not ctx.BaseUri.OriginalString.EndsWith("/") Then
                requestUri = New Uri(ctx.BaseUri.OriginalString + "/" + request)
            Else
                requestUri = New Uri(ctx.BaseUri, requestUri)
            End If
        End If
        Return New DataServiceRequest(Of T)(requestUri)
    End Function

    <System.Runtime.CompilerServices.Extension()>
    Public Function Execute(Of T)(ByVal ctx As DataServiceContext, ByVal request As String) As IEnumerable(Of T)
        Dim requestUri = New Uri(request, UriKind.RelativeOrAbsolute)
        Return ctx.Execute(Of T)(requestUri)
    End Function

    <System.Runtime.CompilerServices.Extension()>
    Public Function Execute(Of T)(ByVal ctx As DataServiceContext, ByVal request As DataServiceRequest(Of T)) As IEnumerable(Of T)
        Return ctx.Execute(Of T)(request.RequestUri)
    End Function

    <System.Runtime.CompilerServices.Extension()>
    Public Function QueryCount(ByVal response As DataServiceResponse) As Int32
        Dim count As Int32
        For Each query As QueryOperationResponse In response
            For Each o In query
                count += 1
            Next
        Next
        Return count
    End Function

End Module

Partial Public Class ClientModule

    <TestClass()>
    Public Class Util

        Private testContextInstance As TestContext

        '''<summary>
        '''Gets or sets the test context which provides
        '''information about and functionality for the current test run.
        '''</summary>
        Public Property TestContext() As TestContext
            Get
                Return testContextInstance
            End Get
            Set(ByVal value As TestContext)
                testContextInstance = value
            End Set
        End Property


        <AssemblyCleanup()>
        Public Shared Sub AssemblyCleanup()
            AstoriaUnitTests.Tests.LocalWebServerHelper.AssemblyCleanup()
        End Sub

        Public Shared Function CreateContext(ByVal serviceRoot As Uri) As DataServiceContext
            Return CreateContext(Of DataServiceContext)(serviceRoot)
        End Function

        Public Shared Function CreateContext(Of T As DataServiceContext)(ByVal serviceRoot As Uri) As T
            Try
                Dim context As T = Nothing
                If GetType(T) Is GetType(DataServiceContext) Then
                    context = CType(New DataServiceContext(serviceRoot), T)
                Else
                    context = CType(Activator.CreateInstance(GetType(T), serviceRoot), T)
                End If

                'AddHandler context.SendingRequest, AddressOf VerifyNotRequestingIdentity
                Return context
            Catch ex As ArgumentNullException
                Assert.AreEqual("serviceRoot", ex.ParamName)
                Throw
            Catch ex As ArgumentException
                Assert.AreEqual("serviceRoot", ex.ParamName)
                Throw
            Catch ex As Exception
                Assert.Fail("unexpected exception")
                Throw
            End Try
        End Function

        Public Function CreateQuery(Of T)(ByVal context As DataServiceContext, ByVal relativeUri As String) As DataServiceQuery(Of T)
            Try
                Return context.CreateQuery(Of T)(relativeUri)
            Catch ex As ArgumentNullException
                Assert.AreEqual("entitySetName", ex.ParamName)
                Throw
            Catch ex As ArgumentException
                Assert.AreEqual("entitySetName", ex.ParamName)
                Throw
            Catch ex As Exception
                Assert.Fail("unexpected exception")
                Throw
            End Try
        End Function

        Public Shared Function CreateReader(ByVal element As XContainer) As XmlReader
            Return CreateReader(New MemoryStream(Encoding.UTF8.GetBytes(element.ToString())))
        End Function

        Public Shared Function CreateReader(ByVal stream As Stream) As XmlReader
            Return CreateReader(stream, Encoding.UTF8)
        End Function

        Public Shared Function CreateReader(ByVal stream As Stream, ByVal encoding As Encoding) As XmlReader
            Dim name = GetType(DataServiceContext).FullName.Replace("DataServiceContext", "XmlUtil")
            Dim util As Type = GetType(DataServiceContext).Assembly.GetType(name, True, False)
            Dim result = util.InvokeMember("CreateXmlReader", BindingFlags.InvokeMethod Or BindingFlags.Static Or BindingFlags.NonPublic, Nothing, Nothing, New Object() {stream, encoding})
            Return CType(result, XmlReader)
        End Function

        Public Shared Function GetMaterializerType(ByVal type As Type) As System.Type
            Dim name As String = GetType(Microsoft.OData.Client.DataServiceQuery).AssemblyQualifiedName
            Dim materializer As Type = Type.GetType(name.Replace("DataServiceQuery", "MaterializeAtom"))
            Return materializer
        End Function

        Public Shared Function GetResourceStream(ByVal resourceName As String) As Stream
            Dim assembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Dim names As String() = assembly.GetManifestResourceNames()
            Dim stream As Stream = assembly.GetManifestResourceStream(resourceName)
            Assert.IsNotNull(stream, resourceName)
            Return stream
        End Function

        Public Shared Function PrintException(ByVal e As Exception) As String
            Dim builder = New StringBuilder()
            While Not e Is Nothing
                builder.Append("Type: ").Append(e.GetType().ToString()).AppendLine()
                builder.Append("Message: ").Append(e.Message).AppendLine()
                builder.Append("StackTrack:").AppendLine().Append(e.StackTrace).AppendLine()

                If GetType(System.Net.WebException).IsInstanceOfType(e) Then
                    builder.Append("Response: ").AppendLine().Append(ResponseAsString(CType(e, System.Net.WebException).Response)).AppendLine()
                End If

                e = e.InnerException
                If Not e Is Nothing Then
                    builder.AppendLine()
                End If
            End While

            Return builder.ToString()
        End Function

        Public Shared Function ResponseAsString(ByVal response As System.Net.WebResponse) As String
            If Not response Is Nothing Then
                Return ContentAsString(response.GetResponseStream())
            End If
            Return String.Empty
        End Function

        Public Shared Function ContentAsString(ByVal stream As Stream) As String
            If Not stream Is Nothing AndAlso stream.CanRead Then
                Return (New StreamReader(stream)).ReadToEnd()
            End If
            Return String.Empty
        End Function

        Public Shared Function FindChangesetFailure(ByVal dataservice As DataServiceResponse) As Exception
            Assert.IsNotNull(dataservice)

            Dim failure As Exception = Nothing
            For Each changeset As ChangeOperationResponse In dataservice
                Dim descriptor = changeset.Descriptor
                If (descriptor.GetType() Is GetType(EntityDescriptor)) Then
                    Dim entity As EntityDescriptor = CType(descriptor, EntityDescriptor)
                    If (Not changeset.Error Is Nothing) Then
                        failure = changeset.Error
                        Exit For
                    End If
                Else
                    Dim link As LinkDescriptor = CType(descriptor, LinkDescriptor)
                    If (Not changeset.Error Is Nothing) Then
                        failure = changeset.Error
                        Exit For
                    End If
                End If
            Next

            Return failure
        End Function

        Public Shared Sub ExecuteBatch(ByVal ctx As DataServiceContext, ByVal ParamArray queries As DataServiceRequest())
            Dim dataServiceResponse As DataServiceResponse = ctx.ExecuteBatch(queries)

            Assert.IsNotNull(dataServiceResponse)

            Dim index As Int32 = 0
            Dim failure As Exception = Nothing
            For Each query As QueryOperationResponse In CType(dataServiceResponse, System.Collections.IEnumerable)
                Assert.AreSame(queries(index), query.Query, "not same query")
                If Not query.Error Is Nothing Then
                    Try
                        DirectCast(query, System.Collections.IEnumerable).GetEnumerator()
                        Assert.Fail("QueryOperationResponse with HasErrors should throw on GetResults")
                    Catch ex As Exception
                        Throw
                    End Try
                End If
                index += 1
            Next

        End Sub

        Public Shared Sub SaveChanges(ByVal ctx As DataServiceContext)
            Dim dataServiceResponse As DataServiceResponse = Nothing

            Try
                dataServiceResponse = ctx.SaveChanges()
            Catch ex As DataServiceClientException
                SerializeException(ex)
                Throw
            Catch ex As DataServiceQueryException
                SerializeException(ex)
                Throw
            Catch ex As DataServiceRequestException
                SerializeException(ex)
                Throw
            End Try

            Dim failure As Exception = FindChangesetFailure(dataServiceResponse)
            If Not failure Is Nothing Then
                Throw New Exception("failure in SaveChanges", failure)
            End If

        End Sub

        Public Shared Sub SaveChanges(ByVal ctx As DataServiceContext, ByVal options As SaveChangesOptions)
            Dim dataServiceResponse As DataServiceResponse = Nothing

            Try
                dataServiceResponse = ctx.SaveChanges(options)
            Catch ex As DataServiceClientException
                SerializeException(ex)
                Throw
            Catch ex As DataServiceQueryException
                SerializeException(ex)
                Throw
            Catch ex As DataServiceRequestException
                SerializeException(ex)
                Throw
            End Try

            Dim failure As Exception = FindChangesetFailure(dataServiceResponse)
            If Not failure Is Nothing Then
                Throw failure
            End If
        End Sub

        Public Shared Function SaveChanges(ByVal ctx As DataServiceContext, ByVal options As SaveChangesOptions, ByVal expectedSuccess As Int32, ByVal expectedExceptions As Int32) As DataServiceResponse
            Return SaveChanges(ctx, options, expectedSuccess, expectedExceptions, Nothing)
        End Function

        Public Shared Function SaveChanges(ByVal ctx As DataServiceContext, ByVal options As SaveChangesOptions, ByVal expectedSuccess As Int32, ByVal expectedExceptions As Int32, ByVal prefix As String) As DataServiceResponse
            Assert.IsNotNull(ctx, "{0} null context", prefix)

            Dim dataServiceResponse As DataServiceResponse = Nothing
            Try
                dataServiceResponse = ctx.SaveChanges(options)
            Catch ex As DataServiceClientException
                SerializeException(ex)
                Throw
            Catch ex As DataServiceQueryException
                SerializeException(ex)
                Throw
            Catch ex As DataServiceRequestException
                SerializeException(ex)
                dataServiceResponse = ex.Response
                If (0 = expectedExceptions) Then
                    Assert.Fail("""{0}"" {1}", prefix, ex.ToString())
                    Throw
                End If
            End Try
            Assert.IsNotNull(dataServiceResponse, "{0} null DataServiceResponse", prefix)

            For Each changeset As ChangeOperationResponse In dataServiceResponse
                Dim failure As Exception = Nothing
                Dim descriptor = changeset.Descriptor
                If (descriptor.GetType() Is GetType(EntityDescriptor)) Then
                    Dim entity As EntityDescriptor = CType(descriptor, EntityDescriptor)
                    If (Not changeset.Error Is Nothing) Then
                        failure = changeset.Error
                        expectedExceptions -= 1
                    Else
                        expectedSuccess -= 1
                    End If
                Else
                    Dim link As LinkDescriptor = CType(descriptor, LinkDescriptor)
                    If (Not changeset.Error Is Nothing) Then
                        failure = changeset.Error
                        expectedExceptions -= 1
                        Exit For
                    Else
                        expectedSuccess -= 1
                    End If
                End If
            Next

            Assert.AreEqual(0, expectedSuccess, """{0}"" missing expected success count, exceptions {1}", prefix, expectedExceptions)
            Assert.AreEqual(0, expectedExceptions, """{0}"" missing expected SaveChanges failure count, successes {1}", prefix, expectedSuccess)
            Return dataServiceResponse
        End Function

        Private Shared Sub SerializeException(ByVal ex As DataServiceClientException)
            Dim copy = BinarySerializeException(ex)
            If (Not copy Is Nothing) Then
                Assert.AreEqual(ex.StatusCode, copy.StatusCode, "StatusCode")
            End If
        End Sub

        Private Shared Sub SerializeException(ByVal ex As DataServiceQueryException)
            Dim copy = BinarySerializeException(ex)
            If (Not copy Is Nothing) Then
                Assert.IsNull(copy.Response)
            End If
        End Sub

        Private Shared Sub SerializeException(ByVal ex As DataServiceRequestException)
            Dim copy = BinarySerializeException(ex)
            If (Not copy Is Nothing) Then
                Assert.IsNull(copy.Response)
            End If
        End Sub

        Private Shared Function BinarySerializeException(Of T As Exception)(ByVal ex As T) As T
            Dim binaryFormatter As New BinaryFormatter()
            Dim memoryStream As New MemoryStream()

            Try
                binaryFormatter.Serialize(memoryStream, ex)
            Catch
                Return Nothing
            End Try

            memoryStream.Position = 0

            binaryFormatter = New BinaryFormatter()
            Dim result As Object = binaryFormatter.Deserialize(memoryStream)
            Assert.IsNotNull(result, "deserialized exception")

            Dim e = DirectCast(result, Exception)
            Assert.AreEqual(ex.Message, e.Message, "Message")
            Assert.AreEqual(ex.InnerException Is Nothing, e.InnerException Is Nothing, "InnerException")

            Return DirectCast(result, T)
        End Function

        Public Shared Sub VerifyLink(
            ByVal ctx As DataServiceContext,
            ByVal source As Object,
            ByVal propertyName As String,
            ByVal target As Object,
            ByVal state As EntityStates)

            For Each link In ctx.Links
                If (Object.ReferenceEquals(link.Source, source) And link.SourceProperty.Equals(propertyName) And Object.ReferenceEquals(link.Target, target) And link.State = state) Then
                    Return
                End If
            Next

            Assert.Fail("Count't find the link in the ctx or the state of the link is different")
        End Sub

        Public Shared Sub VerifyNoLink(
            ByVal ctx As DataServiceContext,
            ByVal entity As Object)

            For Each link In ctx.Links
                If (Object.ReferenceEquals(link.Source, entity) Or Object.ReferenceEquals(link.Target, entity)) Then
                    Assert.Fail("There are one or more links associated with this entity in the context")
                    Return
                End If
            Next
        End Sub

        Public Shared Sub VerifyObject(
            ByVal ctx As DataServiceContext,
            ByVal entity As Object,
            ByVal state As EntityStates)

            Dim descriptor = ctx.GetEntityDescriptor(entity)
            If descriptor Is Nothing Then
                Assert.Fail("Entity not being tracked by the context.")
            End If

            If descriptor.State <> state Then
                Assert.Fail("Entity descriptor state is different.")
            End If
        End Sub

        Public Shared Sub VerifyObjectNotPresent(
           ByVal ctx As DataServiceContext,
           ByVal entity As Object)
            Dim descriptor = ctx.GetEntityDescriptor(entity)
            If Not descriptor Is Nothing Then
                Assert.Fail("Entity being tracked by the context.")
            End If
        End Sub

#Region "Execution methods"
        Public Enum ExecutionMethod
            Synchronous
            AsynchronousEnd
            AsynchronousWait
            AsynchronousCallback
            AsynchronousPoll
        End Enum

        Public Shared ExecutionMethods As ExecutionMethod() = New ExecutionMethod() {
            ExecutionMethod.Synchronous,
            ExecutionMethod.AsynchronousEnd,
            ExecutionMethod.AsynchronousWait,
            ExecutionMethod.AsynchronousCallback,
            ExecutionMethod.AsynchronousPoll
        }

        Public Shared Function ExecuteAsynchronously(ByVal beginFunction As Func(Of AsyncCallback, Object, IAsyncResult),
                                                     ByVal endFunction As Func(Of IAsyncResult, Object),
                                                     ByVal executionMethod As ExecutionMethod) As Object
            Dim asyncResult As IAsyncResult
            Dim waitState As ExecuteMethodAsyncWaitState

            Select Case executionMethod
                Case ExecutionMethod.Synchronous
                    Debug.Fail("Only asynchronous calls are supported by ExecuteAsynchronously.")
                    Throw New AssertFailedException()

                Case ExecutionMethod.AsynchronousEnd
                    asyncResult = beginFunction(Nothing, Nothing)
                    Return endFunction(asyncResult)

                Case ExecutionMethod.AsynchronousWait
                    asyncResult = beginFunction(Nothing, Nothing)
                    asyncResult.AsyncWaitHandle.WaitOne()
                    Assert.IsTrue(asyncResult.IsCompleted)
                    Return endFunction(asyncResult)

                Case ExecutionMethod.AsynchronousCallback
                    waitState = New ExecuteMethodAsyncWaitState()
                    'waitState.Context = context
                    waitState.EndFunction = endFunction
                    asyncResult = beginFunction(AddressOf waitState.AsyncCallback, waitState)
                    Assert.IsTrue(waitState.Done.WaitOne(120000, False))
                    If (Not waitState.Exception Is Nothing) Then
                        Throw waitState.Exception
                    End If
                    Return waitState.Result

                Case ExecutionMethod.AsynchronousPoll
                    asyncResult = beginFunction(Nothing, Nothing)
                    Do
                        System.Threading.Thread.Sleep(0)
                    Loop While Not asyncResult.IsCompleted
                    Return endFunction(asyncResult)

                Case Else
                    Throw New AssertFailedException()
            End Select
        End Function

        Private Class ExecuteMethodAsyncWaitState
            Public Result As Object
            Public EndFunction As Func(Of IAsyncResult, Object)
            Public Exception As Exception
            Public Done As ManualResetEvent

            Public Sub New()
                Done = New ManualResetEvent(False)
            End Sub

            Public Sub AsyncCallback(ByVal asyncResult As IAsyncResult)
                Try
                    Assert.AreEqual(Me, CType(asyncResult.AsyncState, ExecuteMethodAsyncWaitState))
                    Assert.IsTrue(asyncResult.IsCompleted)
                    Result = EndFunction(asyncResult)
                Catch ex As Exception
                    Exception = ex
                Finally
                    Done.Set()
                End Try
            End Sub
        End Class
#End Region

        Public Shared Function SaveChanges(ByVal context As DataServiceContext,
                                           ByVal executionMethod As ExecutionMethod) _
                                           As DataServiceResponse
            Dim beginFunction = Function(callback As AsyncCallback, state As Object) _
                context.BeginSaveChanges(callback, state)
            Dim endFunction = Function(asyncResult As IAsyncResult) _
                context.EndSaveChanges(asyncResult)

            Select Case executionMethod
                Case Util.ExecutionMethod.Synchronous
                    Return context.SaveChanges()

                Case Else
                    Return CType(ExecuteAsynchronously(beginFunction, endFunction, executionMethod), DataServiceResponse)
            End Select
        End Function

        Public Shared Function SaveChanges(ByVal context As DataServiceContext,
                                           ByVal options As SaveChangesOptions,
                                           ByVal executionMethod As ExecutionMethod) _
                                           As DataServiceResponse
            Dim beginFunction = Function(callback As AsyncCallback, state As Object) _
                context.BeginSaveChanges(options, callback, state)
            Dim endFunction = Function(asyncResult As IAsyncResult) _
                context.EndSaveChanges(asyncResult)

            Select Case executionMethod
                Case Util.ExecutionMethod.Synchronous
                    Return context.SaveChanges(options)

                Case Else
                    Return CType(ExecuteAsynchronously(beginFunction, endFunction, executionMethod), DataServiceResponse)
            End Select
        End Function

        Public Shared Function GetReadStream(ByVal context As DataServiceContext,
                                             ByVal entity As Object,
                                             ByVal args As DataServiceRequestArgs,
                                             ByVal executionMethod As ExecutionMethod) _
                                             As DataServiceStreamResponse
            Dim beginFunction = Function(callback As AsyncCallback, state As Object) _
                context.BeginGetReadStream(entity, args, callback, state)
            Dim endFunction = Function(asyncResult As IAsyncResult) _
                context.EndGetReadStream(asyncResult)

            Select Case executionMethod
                Case Util.ExecutionMethod.Synchronous
                    Return context.GetReadStream(entity, args)

                Case Else
                    Return CType(ExecuteAsynchronously(beginFunction, endFunction, executionMethod), DataServiceStreamResponse)
            End Select
        End Function

        Public Shared Function ExecuteBatch(ByVal context As DataServiceContext,
                                            ByVal queries As DataServiceQuery(),
                                            ByVal executionMethod As ExecutionMethod) _
                                            As DataServiceResponse
            Dim beginFunction = Function(callback As AsyncCallback, state As Object) _
                context.BeginExecuteBatch(callback, state, queries)
            Dim endFunction = Function(asyncResult As IAsyncResult) _
                context.EndExecuteBatch(asyncResult)

            Select Case executionMethod
                Case Util.ExecutionMethod.Synchronous
                    Return context.ExecuteBatch(queries)

                Case Else
                    Return CType(ExecuteAsynchronously(beginFunction, endFunction, executionMethod), DataServiceResponse)
            End Select
        End Function

        Public Shared Sub VerifyObjectDeleted(ByVal ctx As DataServiceContext, ByVal entity As Object)
            Dim ed As EntityDescriptor = ctx.GetEntityDescriptor(entity)
            Assert.IsTrue(ed.State = EntityStates.Deleted)
        End Sub

    End Class

    Public Class AtomEnumerationResults
        Public Context As DataServiceContext
        Public Enumerable As System.Collections.IEnumerable
    End Class
End Class