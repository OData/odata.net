'---------------------------------------------------------------------
' <copyright file="PublicPlaces.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Partial Public Class ClientModule
    <TestClass()> Public Class PublicPlaces
        Inherits Util

        <TestCategory("Partition2")> <TestMethod()> Public Sub NewHttpContext()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Assert.AreEqual("http", baseUri.Scheme)
            CreateContext(baseUri)
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub NewHttpsContext()
            Dim baseUri As Uri = New Uri("https://localhost/test.svc")
            Assert.AreEqual("https", baseUri.Scheme)
            CreateContext(baseUri)
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub NewHttpWithPortContext()
            Dim baseUri As Uri = New Uri("http://localhost:80/test.svc")
            Assert.AreEqual(UriHostNameType.Dns, baseUri.HostNameType)
            Assert.AreEqual(80, baseUri.Port)
            Dim metadataUri As Uri = CreateContext(baseUri).GetMetadataUri()
            Assert.IsTrue(metadataUri.AbsoluteUri.StartsWith(baseUri.AbsoluteUri))
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub NewHttpWithOtherPortContext()
            Dim baseUri As Uri = New Uri("http://localhost:81/test.svc")
            Assert.AreEqual(UriHostNameType.Dns, baseUri.HostNameType)
            Assert.AreEqual(81, baseUri.Port)
            Dim metadataUri As Uri = CreateContext(baseUri).GetMetadataUri()
            Assert.IsTrue(metadataUri.OriginalString.StartsWith(baseUri.OriginalString))
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub NewHttpIP4WithPortContext()
            Dim uri As Uri = New Uri("http://157.55.112.74:80/test.svc")
            Assert.IsTrue(uri.IsAbsoluteUri)
            Assert.AreEqual("http", uri.Scheme)
            Assert.AreEqual(UriHostNameType.IPv4, uri.HostNameType)
            Assert.AreEqual(80, uri.Port)
            Assert.AreEqual("/test.svc", uri.PathAndQuery)
            Assert.IsTrue(uri.IsWellFormedOriginalString)

            CreateContext(uri)
        End Sub

        ' the IPV6 test case requires this in the configuration file
        ' <configuration>
        '   <section name="uri" type="System.Configuration.UriSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
        '   <uri>
        '     <iriParsing enabled="true" />
        '   </uri>
        ' </configuration>
        <TestCategory("Partition2")> <TestMethod()> Public Sub NewHttpIP6WithPortContext()
            Dim expected As Boolean
            Dim uriSection As System.Configuration.UriSection
            uriSection = CType(System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).GetSection("uri"), System.Configuration.UriSection)
            If uriSection Is Nothing Then
                expected = True
            Else
                expected = Not uriSection.IriParsing.Enabled
            End If

            Try
                Dim uri As Uri = New Uri("http://[2001:4898:0023:0002:69C4:0E5B:3E27:1ACE]/test.svc", UriKind.Absolute)
                Assert.IsTrue(uri.IsAbsoluteUri)
                Assert.AreEqual("http", uri.Scheme)

                ' DnsSafeHost has different behaviour between .Net v4.0 & .Net v4.5
                If uri.DnsSafeHost.Contains(":0023:") Then
                    Assert.AreEqual("2001:4898:0023:0002:69C4:0E5B:3E27:1ACE", uri.DnsSafeHost)
                Else
                    Assert.AreEqual("2001:4898:23:2:69c4:e5b:3e27:1ace", uri.DnsSafeHost)
                End If

                Assert.AreEqual(UriHostNameType.IPv6, uri.HostNameType)
                Assert.AreEqual(80, uri.Port)
                Assert.AreEqual("/test.svc", uri.PathAndQuery)
                'Assert.IsTrue(uri.IsWellFormedOriginalString, "IsWellFormedOriginalString")

                CreateContext(uri)
            Catch ex As ArgumentException
                If Not expected Then
                    Throw New Exception("Unexpected ArgumentException: " + ex.ToString())
                End If
            End Try
        End Sub

        <TestCategory("Partition2")> <TestMethod()>
        Public Sub NewNullContextUri()
            Dim baseUri As Uri = Nothing
            CreateContext(baseUri)
        End Sub

        <TestCategory("Partition2")> <ExpectedException(GetType(ArgumentException))> <TestMethod()> Public Sub NewQueryContext()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc?hello=world")
            Assert.IsFalse(String.IsNullOrEmpty(baseUri.Query))
            CreateContext(baseUri)
        End Sub

        <TestCategory("Partition2")> <ExpectedException(GetType(ArgumentException))> <TestMethod()> Public Sub NewFragmentContext()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc#hello_world")
            Assert.IsFalse(String.IsNullOrEmpty(baseUri.Fragment))
            CreateContext(baseUri)
        End Sub

        <TestCategory("Partition2")> <ExpectedException(GetType(ArgumentException))> <TestMethod()> Public Sub NewQueryFragmentContext()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc?goodbye=world#hello_world")
            Assert.IsFalse(String.IsNullOrEmpty(baseUri.Query))
            Assert.IsFalse(String.IsNullOrEmpty(baseUri.Fragment))
            CreateContext(baseUri)
        End Sub

        <TestCategory("Partition2")> <ExpectedException(GetType(ArgumentException))> <TestMethod()> Public Sub NewRelativeContext()
            Dim baseUri As Uri = New Uri("/customers", UriKind.RelativeOrAbsolute)
            Assert.IsFalse(baseUri.IsAbsoluteUri)
            CreateContext(baseUri)
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub TrailingWhitespaceContext()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc    ", UriKind.RelativeOrAbsolute)
            CreateContext(baseUri)
        End Sub

        Public Sub SlashTrailingWhitespaceContext()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc/    ", UriKind.RelativeOrAbsolute)
            CreateContext(baseUri)
        End Sub

        <TestCategory("Partition2")> <ExpectedException(GetType(ArgumentException))> <TestMethod()> Public Sub CreateQueryEmpty()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)
            CreateQuery(Of String)(ctx, "")
        End Sub


        <TestCategory("Partition2")> <ExpectedException(GetType(ArgumentNullException))> <TestMethod()> Public Sub CreateQueryNothing()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)
            Dim relativeUri As String = Nothing
            CreateQuery(Of String)(ctx, relativeUri)
        End Sub


        <TestCategory("Partition2")> <TestMethod()> Public Sub CreateQueryAbsoluteSameHost()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)
            Try
                CreateQuery(Of String)(ctx, "http://localhost/test.svc/customer")
                Assert.Fail("expected ArugmentException")
            Catch ex As ArgumentException
            End Try
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub CreateQuerySpaceString()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)
            CreateQuery(Of String)(ctx, "  ")
        End Sub

        <TestCategory("Partition2")> <ExpectedException(GetType(ArgumentException))> <TestMethod()> Public Sub CreateQueryEmptyUri()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)
            CreateQuery(Of String)(ctx, "")
        End Sub

        <TestCategory("Partition2")> <ExpectedException(GetType(ArgumentException))> <TestMethod()> Public Sub CreateQuerySlashUri()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)
            CreateQuery(Of String)(ctx, "/")
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub CreateQueryAbsoluteDiffrentHost()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)
            Try
                CreateQuery(Of String)(ctx, "http://otherlocalhost/test.svc/customer")
                Assert.Fail("expected ArugmentNullException")
            Catch ex As ArgumentException
            End Try
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub CreateQueryRelative()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)
            CreateQuery(Of String)(ctx, "/customers/")
            CreateQuery(Of String)(ctx, "/customers")
            CreateQuery(Of String)(ctx, "customers/")
            CreateQuery(Of String)(ctx, "customers")
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub SetMergeOption()
            Dim baseUri As Uri = New Uri("http://localhost/test.svc")
            Dim ctx As DataServiceContext = CreateContext(baseUri)

            For Each merge As MergeOption In New MergeOption() {MergeOption.AppendOnly, MergeOption.NoTracking, MergeOption.OverwriteChanges, MergeOption.PreserveChanges, CType(-1, MergeOption)}
                Try
                    ctx.MergeOption = merge
                Catch ex As ArgumentOutOfRangeException
                    If [Enum].IsDefined(GetType(MergeOption), merge) Then
                        Throw
                    End If
                End Try
            Next

            Assert.IsTrue(ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support)

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException
            Assert.IsTrue(ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.ThrowException)

            ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support
            Assert.IsTrue(ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support)
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub KeyAttributeFailure()
            Dim r As KeyAttribute

            r = New KeyAttribute("key1")
            r = New KeyAttribute(New String() {"key1"})
            r = New KeyAttribute(New String() {"key1", "key2"})
            r = New KeyAttribute(New String() {"key1", "key2", "key3"})

            Try
                r = New KeyAttribute(DirectCast(Nothing, String))
            Catch ex As ArgumentNullException
                Assert.AreEqual("keyName", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(String.Empty)
            Catch ex As ArgumentException
                Assert.AreEqual("KeyName", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(DirectCast(Nothing, String()))
            Catch ex As ArgumentException
                Assert.AreEqual("keyNames", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(New String() {})
            Catch ex As ArgumentException
                Assert.AreEqual("keyNames", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(New String() {Nothing})
            Catch ex As ArgumentException
                Assert.AreEqual("keyNames", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(New String() {String.Empty})
            Catch ex As ArgumentException
                Assert.AreEqual("keyNames", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(New String() {String.Empty, Nothing})
            Catch ex As ArgumentException
                Assert.AreEqual("keyNames", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(New String() {Nothing, String.Empty})
            Catch ex As ArgumentException
                Assert.AreEqual("keyNames", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(New String() {"key1", String.Empty})
            Catch ex As ArgumentException
                Assert.AreEqual("keyNames", ex.ParamName, "{0}", ex)
            End Try

            Try
                r = New KeyAttribute(New String() {Nothing, "key2", String.Empty})
            Catch ex As ArgumentException
                Assert.AreEqual("keyNames", ex.ParamName, "{0}", ex)
            End Try
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub SaveChangesOptions()
            TestUtil.RunCombinations(
                New SaveChangesOptions() {
                    Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset Or Microsoft.OData.Client.SaveChangesOptions.BatchWithIndependentOperations,
                    Microsoft.OData.Client.SaveChangesOptions.BatchWithSingleChangeset Or Microsoft.OData.Client.SaveChangesOptions.ContinueOnError,
                    Microsoft.OData.Client.SaveChangesOptions.BatchWithIndependentOperations Or Microsoft.OData.Client.SaveChangesOptions.ContinueOnError,
                    CType(256, Microsoft.OData.Client.SaveChangesOptions)},
                Util.ExecutionMethods,
                Sub(saveChangesOption, executionMethod)
                    Dim context = New DataServiceContext(New Uri("http://host/service"))
                    Dim exception = TestUtil.RunCatching(Sub()
                                                             Util.SaveChanges(context, saveChangesOption, executionMethod)
                                                         End Sub)
                    Assert.IsNotNull(exception, "Call to SaveChanges should have failed.")
                    Assert.IsInstanceOfType(exception, GetType(ArgumentOutOfRangeException))
                End Sub)
        End Sub

        Class MaxProtocolVersionTestCase
            Public Property MaxProtocolVersion As ODataProtocolVersion
            Public Property ExpectedExceptionMessage As String
        End Class

        <TestCategory("Partition2")> <TestMethod()> Public Sub MaxProtocolVersion()
            TestUtil.RunCombinations(
                New MaxProtocolVersionTestCase() {
                    New MaxProtocolVersionTestCase() With {.MaxProtocolVersion = ODataProtocolVersion.V4},
                    New MaxProtocolVersionTestCase() With {.MaxProtocolVersion = CType(-1, ODataProtocolVersion), .ExpectedExceptionMessage = "Specified argument was out of the range of valid values." & vbCrLf & "Parameter name: maxProtocolVersion"},
                    New MaxProtocolVersionTestCase() With {.MaxProtocolVersion = CType(ODataProtocolVersion.V4 + 1, ODataProtocolVersion), .ExpectedExceptionMessage = "Specified argument was out of the range of valid values." & vbCrLf & "Parameter name: maxProtocolVersion"}
                },
                Sub(testCase)
                    Dim context = New DataServiceContext(New Uri("http://host/service"))
                    Assert.AreEqual(ODataProtocolVersion.V4, context.MaxProtocolVersion, "The default value should be V3.")
                    Dim exception = TestUtil.RunCatching(Sub()
                                                             context = New DataServiceContext(Nothing, testCase.MaxProtocolVersion)
                                                         End Sub)
                    If testCase.ExpectedExceptionMessage Is Nothing Then
                        Assert.IsNull(exception, "The operation should have succeeded")
                        Assert.AreEqual(testCase.MaxProtocolVersion, context.MaxProtocolVersion, "The value was not remembered correctly.")
                    Else
                        Assert.IsNotNull(exception, "The operation should have failed")
                        Assert.AreEqual(testCase.ExpectedExceptionMessage, exception.Message, "The exception message differs from the expected one.")
                    End If
                End Sub)
        End Sub
    End Class

End Class
