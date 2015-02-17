'---------------------------------------------------------------------
' <copyright file="OpenObjectTests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting

#If ASTORIA_OPEN_OBJECT Then

Partial Public Class ClientModule

    <TestClass()> Public Class OpenObjectTests
        Inherits Util

        Dim OpenPropertyData As XElement = _
            <entry xmlns:base="http://localhost:3000/northwind.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" m:type="NorthwindModel.Customers" xmlns="http://www.w3.org/2005/Atom">
                <title>Customers</title>
                <id>http://localhost:3000/northwind.svc/Customers!'ALFKI'</id>
                <updated/>
                <author/>
                <summary/>
                <link rel="edit" href="//localhost:3000/northwind.svc/Customers!'ALFKI'" title="Customers"/>
                <content type="application/atom+xml">
                    <m:properties>
                        <d:Property1>hello</d:Property1>
                        <d:Property2>world</d:Property2>
                    </m:properties>
                </content>
            </entry>

        Function TestOpenPropertyData(Of T)() As T
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(T), Me.OpenPropertyData)
            Dim x As T
            For Each item As Object In enumerable
                Assert.IsNull(x)
                x = CType(item, T)
            Next
            Return x
        End Function

        <TestMethod()> Public Sub TestOpenObject()
            Dim x As OpenObject = TestOpenPropertyData(Of OpenObject)()
            Assert.AreEqual(2, x.OpenProperties.Count)
            Assert.IsTrue(x.OpenProperties.ContainsKey("Property1"))
            Assert.IsTrue(x.OpenProperties.ContainsKey("Property2"))
        End Sub

        <TestMethod()> Public Sub TestOpenObject1()
            Dim x As OpenObject1 = TestOpenPropertyData(Of OpenObject1)()
            Assert.AreEqual(2, x.OpenProperties.Count)
            Assert.IsTrue(x.OpenProperties.ContainsKey("Property1"))
            Assert.IsTrue(x.OpenProperties.ContainsKey("Property2"))
        End Sub

        <TestMethod()> Public Sub TestOpenObject2()
            Dim x As OpenObject2 = TestOpenPropertyData(Of OpenObject2)()
            Assert.AreEqual(2, x.OpenPropertys.Count)
            Assert.IsTrue(x.OpenPropertys.ContainsKey("Property1"))
            Assert.IsTrue(x.OpenPropertys.ContainsKey("Property2"))
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestMethod()> Public Sub TestOpenObjectBad1()
            ' OpenObjectBad1 is not an open type
            ' throws InvalidOperationException because Property1 doesn't exist
            Assert.AreEqual(0, GetType(OpenObjectBad1).GetCustomAttributes(GetType(OpenObjectAttribute), True).Length)
            Try
                TestOpenPropertyData(Of OpenObjectBad1)()
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("Property1"))
                Throw
            End Try
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestMethod()> Public Sub TestOpenObjectBad2()
            Try ' OpenObjectBad2 has OpenProperties with wrong signature
                TestOpenPropertyData(Of OpenObjectBad2)()
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("OpenProperties"))
                Throw
            End Try
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestMethod()> Public Sub TestOpenObjectBad3()
            Try ' OpenObjectBad3 has OpenPropertys not defined
                TestOpenPropertyData(Of OpenObjectBad3)()
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("OpenPropertys"))
                Throw
            End Try
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestMethod()> Public Sub TestOpenObjectBad4()
            Try ' OpenObjectBad4 has  multiple definitions for OpenObject (not shadowed)
                TestOpenPropertyData(Of OpenObjectBad4)()
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("multiple definitions"))
                Throw
            End Try
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestMethod()> Public Sub TestOpenObjectBad5()
            Try ' OpenObjectBad5 has OpenObject defined on different instance than property is defined, multiple def (not shadowed)
                TestOpenPropertyData(Of OpenObjectBad5)()
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("multiple definitions"))
                Throw
            End Try
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestMethod()> Public Sub TestOpenObjectBad6()
            Try  ' OpenObjectBad6 has OpenObjectProperty defined on different instance than property is defined
                TestOpenPropertyData(Of OpenObjectBad6)()
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("multiple definitions"))
                Throw
            End Try
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestMethod()> Public Sub TestOpenObjectBad7()
            Try ' OpenObjectBad7 has multiple definitions for OpenObject (shadowing) 
                TestOpenPropertyData(Of OpenObjectBad7)()
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("multiple definitions"))
                Throw
            End Try
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestMethod()> Public Sub TestOpenObjectBad8()
            Try ' OpenObjectBad8 has OpenProperties with KeyAttribute
                TestOpenPropertyData(Of OpenObjectBad8)()
            Catch ex As InvalidOperationException
                Assert.IsTrue(ex.Message.Contains("key properties"))
                Throw
            End Try
        End Sub

        <TestMethod()> Public Sub TestOpenObjectField()
            Dim open As New OpenObject
            open.Item("abc") = "adsfasdf"
            open.Item("1") = 1

            Assert.AreEqual("adsfasdf", open.Item("abc"))
            Assert.AreEqual("adsfasdf", open.Field(Of String)("abc"))

            Assert.AreEqual(1, open.Item("1"))
            Assert.AreEqual(1, open.Field(Of Int32)("1"))

            Assert.IsNull(open.Item("bogus"))
            Assert.IsNull(open.Field(Of String)("bogus"))
            Assert.IsNull(open.Field(Of Nullable(Of Int32))("bogus"))

            Try
                open.Field(Of Int32)("abc")
                Assert.Fail("expecting exception")
            Catch ex As InvalidOperationException
            End Try

            Try
                open.Field(Of String)("1")
                Assert.Fail("expecting exception")
            Catch ex As InvalidOperationException
            End Try
        End Sub
    End Class

    <OpenObject("OpenProperties")> Friend Class OpenObject1
        Private properties As New Dictionary(Of String, Object)

        Public ReadOnly Property OpenProperties() As Dictionary(Of String, Object)
            Get
                Return Me.properties
            End Get
        End Property
    End Class

    <OpenObject("OpenPropertys")> Friend Class OpenObject2
        Private properties As New Dictionary(Of String, Object)

        Public ReadOnly Property OpenPropertys() As Dictionary(Of String, Object)
            Get
                Return Me.properties
            End Get
        End Property
    End Class

    Friend Class OpenObjectBad1
        Private properties As New Dictionary(Of String, Object)

        Public ReadOnly Property OpenProperties() As Dictionary(Of String, Object)
            Get
                Return Me.properties
            End Get
        End Property
    End Class

    <OpenObject("OpenProperties")> Friend Class OpenObjectBad2
        Private properties As New Dictionary(Of String, String)

        Public ReadOnly Property OpenProperties() As Dictionary(Of String, String)
            Get
                Return Me.properties
            End Get
        End Property
    End Class

    <OpenObject("OpenPropertys")> Friend Class OpenObjectBad3
        Private properties As New Dictionary(Of String, Object)

        Public ReadOnly Property OpenProperties() As Dictionary(Of String, Object)
            Get
                Return Me.properties
            End Get
        End Property
    End Class

    <OpenObject("OpenProperties")> Friend Class OpenObjectBad4
        Inherits OpenObjectBad3

    End Class

    <OpenObject("OpenProperties")> Friend Class OpenObjectBad5
        Inherits OpenObject1

    End Class

    <OpenObject("OpenProperties")> Friend Class OpenObjectBad6
        Inherits OpenObjectBad1

    End Class

    <OpenObject("OpenProperties")> Friend Class OpenObjectBad7
        Inherits OpenObject1

        Private properties As New Dictionary(Of String, Object)

        Public Shadows ReadOnly Property OpenProperties() As Dictionary(Of String, Object)
            Get
                Return Me.properties
            End Get
        End Property

    End Class

    <OpenObject("OpenProperties")> <DataServiceKey("OpenProperties")> Friend Class OpenObjectBad8
        Private properties As New Dictionary(Of String, Object)

        Public ReadOnly Property OpenProperties() As Dictionary(Of String, Object)
            Get
                Return Me.properties
            End Get
        End Property
    End Class

End Class
#End If
