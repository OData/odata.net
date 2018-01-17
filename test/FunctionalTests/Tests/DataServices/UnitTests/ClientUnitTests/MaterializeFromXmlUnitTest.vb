'---------------------------------------------------------------------
' <copyright file="MaterializeFromXmlUnitTest.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Resources
Imports System.IO
Imports System.Linq
Imports System.Data.Linq
Imports System.Reflection
Imports System.Text
Imports System.Xml
Imports System.Xml.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AstoriaClientUnitTests.AstoriaClientUnitTests.Stubs
Imports AstoriaUnitTests

Partial Public Class ClientModule

    <TestClass()> Public Class MaterializeFromXmlUnitTest
        Inherits Util

        <TestCategory("Partition1")> <TestMethod()> Public Sub ReadElementString()
            Dim Understanding_ReadElementString As XElement =
                <a>
                    <b>
                        <c>has simple text</c>
                ![CDATA[<e>asdf</e>]]
                <f></f>
                    </b>
                </a>
            Dim reader As XmlReader

            reader = Understanding_ReadElementString.CreateReader()
            Try
                reader.Read()
                Assert.AreEqual("a", reader.Name, "#1")
                reader.ReadElementString()
            Catch ex As XmlException
                Assert.AreEqual("c", reader.Name, "#2")
            End Try

            reader = Understanding_ReadElementString.CreateReader()
            Try
                reader.Read()
                Assert.AreEqual("a", reader.Name, "#3")

                reader.Read()
                Assert.AreEqual("b", reader.Name, "#4")

                reader.Read()
                Assert.AreEqual("c", reader.Name, "#5")

                reader.ReadElementString()
            Catch ex As XmlException
                Assert.AreEqual("a", reader.Name, "#6")
            End Try
        End Sub

        <EntityType()>
        Public Class EmptyValueClass
            Private binary As Binary
            Private blob As Byte()
            Private nchar As Char()
            Private nvarchar As String
            Private document As XDocument
            Private element As XElement
            Private uri As Uri

            Public Property BinaryProperty() As Binary
                Get
                    Return Me.binary
                End Get
                Set(ByVal value As Binary)
                    Me.binary = value
                End Set
            End Property

            Public Property ByteProperty() As Byte()
                Get
                    Return Me.blob
                End Get
                Set(ByVal value As Byte())
                    Me.blob = value
                End Set
            End Property

            Public Property CharProperty() As Char()
                Get
                    Return Me.nchar
                End Get
                Set(ByVal value As Char())
                    Me.nchar = value
                End Set
            End Property

            Public Property StringProperty() As String
                Get
                    Return Me.nvarchar
                End Get
                Set(ByVal value As String)
                    Me.nvarchar = value
                End Set
            End Property

            Public Property XDocumentProperty() As XDocument
                Get
                    Return Me.document
                End Get
                Set(ByVal value As XDocument)
                    Me.document = value
                End Set
            End Property

            Public Property XElementProperty() As XElement
                Get
                    Return Me.element
                End Get
                Set(ByVal value As XElement)
                    Me.element = value
                End Set
            End Property

            Public Property UriProperty() As Uri
                Get
                    Return Me.uri
                End Get
                Set(ByVal value As Uri)
                    Me.uri = value
                End Set
            End Property

        End Class

#Region "Row Count Materializer"

        Private Function ReadCountValue(ByRef materializer As System.Collections.IEnumerable, ByVal type As System.Type) As Long
            Dim materializerType As Type = GetMaterializerType(type)
            Dim countMethod As MethodInfo = materializerType.GetMethod("CountValue", BindingFlags.Instance Or BindingFlags.NonPublic)
            Dim countValue As Long = CType(countMethod.Invoke(materializer, Nothing), Long)

            Return countValue
        End Function

#End Region

        Public Class SomeComplexType
            Public Property Name() As String
                Get
                    Return ""
                End Get
                Set(ByVal value As String)

                End Set
            End Property
        End Class

    End Class
End Class

<CLSCompliant(False)>
Public Class EntityWithNullComplexType
    Private AddressValue As AstoriaUnitTests.Stubs.Address
    Private IdValue As Integer

    Public Property ID() As Integer
        Get
            Return IdValue
        End Get
        Set(ByVal value As Integer)
            IdValue = value
        End Set
    End Property

    Public Property TheAddress() As AstoriaUnitTests.Stubs.Address
        Get
            Return AddressValue
        End Get
        Set(ByVal value As AstoriaUnitTests.Stubs.Address)
            AddressValue = value
        End Set
    End Property
End Class
