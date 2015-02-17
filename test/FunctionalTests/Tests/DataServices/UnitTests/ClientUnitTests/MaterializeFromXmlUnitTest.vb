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
            Dim Understanding_ReadElementString As XElement = _
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

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_Int32()
            For Each value As Object In New Object() {Int32.MinValue, Int32.MaxValue, 0, 1, -1, Nothing, Int64.MaxValue, Int64.MinValue, "abc", "123"}
                Dim root As XNamespace = "http://docs.oasis-open.org/odata/ns/metadata"
                Dim test As XElement = New XElement(root + "value", value)
                Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Int32), test)
                Dim count As Integer = 0
                Try
                    For Each instance As Int32 In enumerable
                        count += 1
                        Assert.AreEqual(CType(value, Int32), instance)
                    Next
                    Assert.AreEqual(1, count)
                Catch ex As InvalidOperationException
                    If (value Is Nothing) Then
                        Continue For
                    End If

                    Try
                        Convert.ToInt32(value)
                        Assert.IsTrue(False, "should have thrown")
                    Catch exe As Exception
                        Assert.IsNotNull(ex.InnerException.InnerException)
                        Assert.AreEqual(ex.InnerException.InnerException.Message, exe.Message)
                    End Try
                Catch ex As Exception
                    Assert.Fail(ex.Message)
                End Try
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_NullableInt32()
            For Each value As Object In New Object() {Int32.MinValue, Nothing, Int32.MaxValue, 0, 1, -1, Nothing}
                Dim root As XNamespace = "http://docs.oasis-open.org/odata/ns/data"
                Dim metadataNamespace As XNamespace = "http://docs.oasis-open.org/odata/ns/metadata"
                Dim test As XElement = New XElement(metadataNamespace + "value", value)
                If value Is Nothing Then
                    test.SetAttributeValue(metadataNamespace + "null", True)
                End If
                Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Nullable(Of Int32)), test)
                Dim count As Integer = 0
                For Each instance As Nullable(Of Int32) In enumerable
                    count += 1
                    Assert.AreEqual(CType(value, Nullable(Of Int32)), instance)
                Next
                Assert.AreEqual(1, count)
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_CustomerID()
            ' GET /northwind.svc/Customers('ALFKI')/CustomerID
            Dim XAtom_CustomerID As XElement = _
            <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">ALFKI</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(String), XAtom_CustomerID)
            Dim count As Integer = 0
            For Each instance As String In enumerable
                count += 1
                Assert.AreEqual("ALFKI", instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_MissingContentProperties()
            Dim XAtom_MissingContentPropertiesBad1 As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            <d:Properties></d:Properties>
        </content>
    </entry>

            Dim XAtom_MissingContentPropertiesBad2 As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
        mixed content
    </content>
    </entry>

            Dim XAtom_MissingContentPropertiesBad6 As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml"/>
    </entry>

            Dim XAtom_MissingContentPropertiesBad5 As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
        </content>
    </entry>

            Dim XAtom_MissingContentPropertiesBad3 As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">

            <!-- Skip me, I'm a comment -->

        </content>
    </entry>

            Dim XAtom_MissingContentPropertiesBad4 As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">

            <!-- Skip me, I'm a comment -->

            <m:properties>
            </m:properties>
        </content>
    </entry>

            Dim XAtom_MissingContentPropertiesBad7 As XElement = _
<entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
    <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
    <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
    <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
    <content type="application/xml">
        <content type="application/xml"/>
        <m:properties>
        </m:properties>
    </content>
</entry>

            Dim XAtom_MissingContentPropertiesBadTextInterspersed As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
                text
            </m:properties>
        </content>
    </entry>

            Dim XAtom_MissingContentPropertiesGood5 As XElement = _
    <a:entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns:a="http://www.w3.org/2005/Atom" xmlns="http://host/195">
        <a:category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <!-- Skip me, I'm a comment -->
        <a:id>http://localhost:3000/northwind.svc/Customers('ALFKI')</a:id>
        <!-- Skip me, I'm a comment -->
        <a:link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" a:title="Customers" title="Bogus"/>
        <!-- Skip me, I'm a comment -->
        <a:content type="application/xml">

            <!-- Skip me, I'm a comment -->

            <m:properties>
                <!-- Skip me, I'm a comment -->
                <d:CustomerID>ALFKI</d:CustomerID>
                <!-- Skip me, I'm a comment -->
                <d:CompanyName>Alfreds Futterkiste</d:CompanyName>
                <!-- Skip me, I'm a comment -->
            </m:properties>

            <!-- Skip me, I'm a comment -->
        </a:content>
        <!-- Skip me, I'm a comment -->
    </a:entry>

            Dim XAtom_MissingContentPropertiesGood6 As XElement = _
    <a:entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns:a="http://www.w3.org/2005/Atom" xmlns="http://host/195">
        <a:category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <!-- Skip me, I'm a comment -->
        <a:id>http://localhost:3000/northwind.svc/Customers('ALFKI')</a:id>
        <!-- Skip me, I'm a comment -->
        <a:link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" a:title="Customers" title="Bogus"/>
        <!-- Skip me, I'm a comment -->
        <a:content type="application/xml"/>
        <!-- Skip me, I'm a comment -->
    </a:entry>

            Dim XAtom_MissingContentPropertiesGood7 As XElement = _
    <a:entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns:a="http://www.w3.org/2005/Atom" xmlns="http://host/195">
        <a:category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <!-- Skip me, I'm a comment -->
        <a:id>http://localhost:3000/northwind.svc/Customers('ALFKI')</a:id>
        <!-- Skip me, I'm a comment -->
        <a:link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" a:title="Customers" title="Bogus"/>
        <!-- Skip me, I'm a comment -->
        <a:content type="application/xml">
        </a:content>
        <!-- Skip me, I'm a comment -->
    </a:entry>

            For Each bad As XElement In New XElement() {}
                Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), bad)
                Try
                    For Each obj As Object In enumerable
                    Next
                    Assert.Fail("expected InvalidOperationException")
                Catch ex As InvalidOperationException
                    Assert.IsTrue(ex.Message.Contains("expected content of ""application/xml"""), "{0}", ex)
                End Try
            Next

            For Each good As XElement In New XElement() {XAtom_MissingContentPropertiesBad1, XAtom_MissingContentPropertiesBad2, XAtom_MissingContentPropertiesBad7, XAtom_MissingContentPropertiesGood5, XAtom_MissingContentPropertiesGood6, XAtom_MissingContentPropertiesGood7}
                Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), good)
                For Each obj As Object In enumerable
                Next
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_EntryHasEmptyId()
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id></id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
            </m:properties>
        </content>
    </entry>

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            Try
                For Each obj As Object In enumerable
                Next
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
                Assert.AreEqual(DataServicesClientResourceUtil.GetString("Deserialize_MissingIdElement"), ex.Message)
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_EntryHasMixedContentInId()
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id><!-- nifty -->http://something/<!--yikes-->odd</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
            </m:properties>
        </content>
    </entry>

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            For Each obj As NorthwindSimpleModel.Customers In enumerable
            Next
            ' TODO: check out what the URI is for this - sadness
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_EntryIsEmptyElement()
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom"/>

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            Try
                For Each obj As NorthwindSimpleModel.Customers In enumerable
                Next
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
                Assert.AreEqual(DataServicesClientResourceUtil.GetString("Deserialize_MissingIdElement"), ex.Message)
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_LinkNameMissing()
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://something/odd</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <link title="Orders" href="//localhost:3000/northwind.svc/Customers('ALFKI')/Orders" type="application/atom+xml;type=Feed"/>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
            </m:properties>
        </content>
    </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            For Each obj As NorthwindSimpleModel.Customers In enumerable
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_LinkNameWeird()
            ' Also covers testing for empty m:inline element
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://something/odd</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <link rel="http://docs.oasis-open.org/odata/ns/relatedzzzz/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers('ALFKI')/Orders" type="application/atom+Xml;type=Feed"/>
        <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers('ALFKI')/Orders" type="application/atom+Xml;type=Feed">
            <m:inline/>
        </link>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
            </m:properties>
        </content>
    </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            For Each obj As NorthwindSimpleModel.Customers In enumerable
                Assert.IsNotNull(obj.Orders)
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_LinkTypeMissing()
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://something/odd</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers('ALFKI')/Orders">
            <!-- stuff! -->
        </link>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
            </m:properties>
        </content>
    </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            For Each obj As NorthwindSimpleModel.Customers In enumerable
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_LinkTypeWeird()
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://something/odd</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers('ALFKI')/Orders" type="who knows, really?">
            <!-- stuff! -->
        </link>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
            </m:properties>
        </content>
    </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            Try
                For Each obj As NorthwindSimpleModel.Customers In enumerable
                Next
            Catch ex As InvalidOperationException
                Assert.AreEqual(ODataLibResourceUtil.GetString("HttpUtils_MediaTypeRequiresSlash", "who knows, really?"), ex.Message)
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_StuffAfterPropertiesInContent()
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://something/odd</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
            </m:properties>
            stuff(here)
        </content>
    </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            For Each obj As NorthwindSimpleModel.Customers In enumerable
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_StuffBeforePropertiesInContent()
            Dim xml As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://something/odd</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            stuff(here)
            <m:properties>
                <d:CustomerID>ALFI</d:CustomerID>
            </m:properties>
        </content>
    </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            Dim count As Integer = 0
            For Each obj As NorthwindSimpleModel.Customers In enumerable
                Assert.AreEqual("ALFI", obj.CustomerID, "Failed to read properties")
                count = count + 1
            Next
            Assert.AreEqual(1, count, "Incorrect materialization count")
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_FeedIsEmptyElement()
            Dim xml As XElement = _
    <feed xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom"/>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), xml)
            For Each obj As NorthwindSimpleModel.Customers In enumerable
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_CustomerALFKI()
            ' GET /northwind.svc/Customers('ALFKI')
            Dim XAtom_CustomerALFKI As XElement = _
    <entry xml:base="http://localhost:3000/northwind.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
        <updated/>
        <title/>
        <author>
            <name/>
        </author>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFKI</d:CustomerID>
                <d:CompanyName>Alfreds Futterkiste</d:CompanyName>
                <d:ContactName>Maria Anders</d:ContactName>
                <d:ContactTitle>Sales Representative</d:ContactTitle>
                <d:Address>Obere Str. 57</d:Address>
                <d:City>Berlin</d:City>
                <d:Region m:null="true"/>
                <d:PostalCode>12209</d:PostalCode>
                <d:Phone>030-0074321</d:Phone>
                <d:Fax>030-0076545</d:Fax>
            </m:properties>
        </content>
        <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers('ALFKI')/Orders" type="application/atom+xml;type=feed"/>
    </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), XAtom_CustomerALFKI)
            Dim count As Integer = 0
            For Each instance As NorthwindSimpleModel.Customers In enumerable
                count += 1
                Assert.AreEqual(instance.GetType(), GetType(NorthwindSimpleModel.Customers))
                Assert.AreEqual("ALFKI", instance.CustomerID)
                Assert.AreEqual("Alfreds Futterkiste", instance.CompanyName)
                Assert.AreEqual("Maria Anders", instance.ContactName)
                Assert.AreEqual("Sales Representative", instance.ContactTitle)
                Assert.AreEqual("Obere Str. 57", instance.Address)
                Assert.AreEqual("Berlin", instance.City)
                Assert.AreEqual(Nothing, instance.Region)
                Assert.AreEqual("12209", instance.PostalCode)
                Assert.AreEqual("030-0074321", instance.Phone)
                Assert.AreEqual("030-0076545", instance.Fax)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_CustomerALFKIPropertiesMixedContent()
            Dim XAtom_CustomerALFKIPropertiesMixedContent As XElement = _
                <entry xmlns:base="http://localhost:3000/northwind.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                    <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                    <title>Customers</title>
                    <id>http://localhost:3000/northwind.svc/Customers!'ALFKI'</id>
                    <updated/>
                    <author/>
                    <summary/>
                    <link rel="edit" href="//localhost:3000/northwind.svc/Customers!'ALFKI'" title="Customers"/>
                    <content type="application/xml">
                        <m:properties>
                            <d:CustomerID>ALFKI</d:CustomerID>
                        mixed-content
                    </m:properties>
                    </content>
                    <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers!'ALFKI'/Orders" m:deferred="true" type="application/atom+xml;type=feed"/>
                </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), XAtom_CustomerALFKIPropertiesMixedContent)
            Dim count As Integer = 0
            For Each instance As NorthwindSimpleModel.Customers In enumerable
                count += 1
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_CustomerALFKIMixedContent()
            Dim XAtom_CustomerALFKIMixedContent As XElement = _
                <entry xmlns:base="http://localhost:3000/northwind.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                    <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                    <title>Customers</title>
                    mixed-content
                    <id>http://localhost:3000/northwind.svc/Customers!'ALFKI'</id>
                    <updated/>
                    mixed-content
                    <author/>
                    <summary/>
                    <link rel="edit" href="//localhost:3000/northwind.svc/Customers!'ALFKI'" title="Customers"/>
                    <content type="application/xml">
                        <m:properties>
                            <d:CustomerID>ALFKI</d:CustomerID>
                        </m:properties>
                    </content>
                    <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers!'ALFKI'/Orders" m:deferred="true" type="application/atom+xml;type=feed"/>
                </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), XAtom_CustomerALFKIMixedContent)
            Dim count As Integer = 0
            For Each instance As NorthwindSimpleModel.Customers In enumerable
                count += 1
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_CustomersMixedContent()
            Dim XAtom_CustomersMixedContent As XElement = _
                <feed xmlns:base="http://localhost:3000/northwind.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                    <id>http://localhost:3000/northwind.svc/Customers</id>
                    <updated/>
                    mixed-content
                    <title>Customers</title>
                    <entry>
                        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                        <title>Customers</title>
                        <id>http://localhost:3000/northwind.svc/Customers!'ALFKI'</id>
                        <updated/>
                        <author/>
                        <summary/>
                        <link rel="edit" href="//localhost:3000/northwind.svc/Customers!'ALFKI'" title="Customers"/>
                        <content type="application/xml">
                            <m:properties>
                                <d:CustomerID>ALFKI</d:CustomerID>
                            </m:properties>
                        </content>
                        <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers!'ALFKI'/Orders" m:deferred="true" type="application/atom+xml;type=feed"/>
                    </entry>
                </feed>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), XAtom_CustomersMixedContent)
            Dim count As Integer = 0
            For Each instance As NorthwindSimpleModel.Customers In enumerable
                count += 1
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_Customers()
            ' GET /northwind.svc/Customers?$top=2
            Dim XAtom_Customers As XElement = _
    <feed xml:base="http://localhost:3000/northwind.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <id>http://localhost:3000/northwind.svc/Customers</id>
        <updated/>
        <title>Customers</title>
        <link rel="self" href="//localhost:3000/northwind.svc/Customers" title="Customers"/>
        <entry>
            <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <id>http://localhost:3000/northwind.svc/Customers('ALFKI')</id>
            <updated/>
            <title/>
            <author>
                <name/>
            </author>
            <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
            <content type="application/ATOM+Xml">
                <m:properties>
                    <d:CustomerID>ALFKI</d:CustomerID>
                    <d:CompanyName>Alfreds Futterkiste</d:CompanyName>
                    <d:ContactName>Maria Anders</d:ContactName>
                    <d:ContactTitle>Sales Representative</d:ContactTitle>
                    <d:Address>Obere Str. 57</d:Address>
                    <d:City>Berlin</d:City>
                    <d:Region m:null="true"/>
                    <d:PostalCode>12209</d:PostalCode>
                    <d:Phone>030-0074321</d:Phone>
                    <d:Fax>030-0076545</d:Fax>
                </m:properties>
            </content>
            <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers('ALFKI')/Orders" type="application/atom+xml;type=feed"/>
        </entry>
        <entry>
            <id>http://localhost:3000/northwind.svc/Customers('ANATR')</id>
            <updated/>
            <title/>
            <author>
                <name/>
            </author>
            <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ANATR')" title="Customers"/>
            <content type="APPLICATION/Xml">
                <m:properties>
                    <d:CustomerID>ANATR</d:CustomerID>
                    <d:CompanyName>Ana Trujillo Emparedados y helados</d:CompanyName>
                    <d:ContactName>Ana Trujillo</d:ContactName>
                    <d:ContactTitle>Owner</d:ContactTitle>
                    <d:Address>Avda. de la Constitución 2222</d:Address>
                    <d:City>México D.F.</d:City>
                    <d:Region m:null="true"/>
                    <d:PostalCode>05021</d:PostalCode>
                    <d:Phone>(5) 555-4729</d:Phone>
                    <d:Fax>(5) 555-3745</d:Fax>
                </m:properties>
            </content>
            <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:3000/northwind.svc/Customers('ANATR')/Orders" type="application/atom+xml;type=feed"/>
            <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        </entry>
    </feed>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), XAtom_Customers)
            Dim count As Integer = 0
            For Each instance As NorthwindSimpleModel.Customers In enumerable
                count += 1
                If 1 = count Then
                    Assert.AreEqual(instance.GetType(), GetType(NorthwindSimpleModel.Customers))
                    Assert.AreEqual("ALFKI", instance.CustomerID)
                    Assert.AreEqual("Alfreds Futterkiste", instance.CompanyName)
                    Assert.AreEqual("Maria Anders", instance.ContactName)
                    Assert.AreEqual("Sales Representative", instance.ContactTitle)
                    Assert.AreEqual("Obere Str. 57", instance.Address)
                    Assert.AreEqual("Berlin", instance.City)
                    Assert.AreEqual(Nothing, instance.Region)
                    Assert.AreEqual("12209", instance.PostalCode)
                    Assert.AreEqual("030-0074321", instance.Phone)
                    Assert.AreEqual("030-0076545", instance.Fax)
                ElseIf 2 = count Then
                    Assert.AreEqual(instance.GetType(), GetType(NorthwindSimpleModel.Customers))
                    Assert.AreEqual("ANATR", instance.CustomerID)
                    Assert.AreEqual("Ana Trujillo Emparedados y helados", instance.CompanyName)
                    Assert.AreEqual("Ana Trujillo", instance.ContactName)
                    Assert.AreEqual("Owner", instance.ContactTitle)
                    Assert.AreEqual("Avda. de la Constitución 2222", instance.Address)
                    Assert.AreEqual("México D.F.", instance.City)
                    Assert.AreEqual(Nothing, instance.Region)
                    Assert.AreEqual("05021", instance.PostalCode)
                    Assert.AreEqual("(5) 555-4729", instance.Phone)
                    Assert.AreEqual("(5) 555-3745", instance.Fax)
                End If
            Next
            Assert.AreEqual(2, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_CustomersWithShippers()
            'GET /northwind.svc/Orders?$top=1&$expand=Shippers
            Dim XAtom_CustomersWithShippers As XDocument = _
    <?xml version="1.0" encoding="utf-8" standalone="yes"?>
    <feed xml:base="http://localhost:49192/northwind.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <id>http://localhost:49192/northwind.svc/Orders</id>
        <updated/>
        <title>Orders</title>
        <link rel="self" href="//localhost:49192/northwind.svc/Orders" title="Orders"/>
        <entry>
            <id>http://localhost:49192/northwind.svc/Orders(10248)</id>
            <updated/>
            <title/>
            <author>
                <name/>
            </author>
            <category term="NorthwindModel.Orders" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <link rel="edit" href="//localhost:49192/northwind.svc/Orders(10248)" title="Orders"/>
            <content type="application/xml">
                <m:properties>
                    <d:OrderID m:type="Int32">10248</d:OrderID>
                    <d:OrderDate m:type="Nullable`1">1996-07-04T00:00:00Z</d:OrderDate>
                    <d:RequiredDate m:type="Nullable`1">1996-08-01T00:00:00Z</d:RequiredDate>
                    <d:ShippedDate m:type="Nullable`1">1996-07-16T00:00:00Z</d:ShippedDate>
                    <d:Freight m:type="Nullable`1">32.3800</d:Freight>
                    <d:ShipName>Vins et alcools Chevalier</d:ShipName>
                    <d:ShipAddress>59 rue de l'Abbaye</d:ShipAddress>
                    <d:ShipCity>Reims</d:ShipCity>
                    <d:ShipRegion/>
                    <d:ShipPostalCode>51100</d:ShipPostalCode>
                </m:properties>
            </content>
            <link rel="http://docs.oasis-open.org/odata/ns/related/Customers" title="Customers" href="//localhost:49192/northwind.svc/Orders(10248)/Customers" type="application/atom+xml;TYPE=ENTRY"/>
            <link rel="http://docs.oasis-open.org/odata/ns/related/Employees" title="Employees" href="//localhost:49192/northwind.svc/Orders(10248)/Employees" type="APPLICATION/Atom+xml;type=entry"/>
            <link rel="http://docs.oasis-open.org/odata/ns/related/Shippers" title="Shippers" href="//localhost:49192/northwind.svc/Orders(10248)/Shippers" type="application/atom+XML;type=entry">
                <m:inline>
                    <entry>
                        <id>http://localhost:49192/northwind.svc/Shippers(3)</id>
                        <updated/>
                        <title/>
                        <author>
                            <!-- Just messing with the materializer - this should be ignored. -->
                            <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                            <name/>
                        </author>
                        <category term="NorthwindModel.Shippers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                        <link rel="edit" href="//localhost:49192/northwind.svc/Shippers(3)" title="Shippers"/>
                        <content type="application/xml">
                            <m:properties>
                                <d:ShipperID m:type="Int32">3</d:ShipperID>
                                <d:CompanyName>Federal Shipping</d:CompanyName>
                                <d:Phone>(503) 555-9931</d:Phone>
                            </m:properties>
                        </content>
                        <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:49192/northwind.svc/Shippers(3)/Orders" type="application/atom+xml;type=feed"/>
                    </entry>
                </m:inline>
            </link>
        </entry>
    </feed>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Orders), XAtom_CustomersWithShippers)
            Dim count As Integer = 0
            For Each instance As NorthwindSimpleModel.Orders In enumerable
                count += 1
                Assert.AreEqual(instance.GetType(), GetType(NorthwindSimpleModel.Orders))

                Assert.AreEqual(10248, instance.OrderID)
                Assert.AreEqual(XmlConvert.ToDateTimeOffset("1996-07-04T00:00:00Z"), instance.OrderDate.Value)
                Assert.AreEqual(XmlConvert.ToDateTimeOffset("1996-08-01T00:00:00Z"), instance.RequiredDate.Value)
                Assert.AreEqual(XmlConvert.ToDateTimeOffset("1996-07-16T00:00:00Z"), instance.ShippedDate.Value)
                Assert.AreEqual(Decimal.Parse("32.3800"), instance.Freight.Value)
                Assert.AreEqual(0, instance.ShipRegion.Length)
                Assert.IsNotNull(instance.Shippers)
                Assert.AreEqual(3, instance.Shippers.ShipperID)
                Assert.AreEqual("Federal Shipping", instance.Shippers.CompanyName)
                Assert.AreEqual("(503) 555-9931", instance.Shippers.Phone)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_Materialization()
            Dim XAtom_CustomersWithShippersLiveOrder As XDocument = _
    <?xml version="1.0" encoding="utf-8" standalone="yes"?>
    <feed xml:base="http://localhost:49192/northwind.svc/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <id>http://localhost:49192/northwind.svc/Orders</id>
        <updated/>
        <title>Orders</title>
        <link rel="self" href="//localhost:49192/northwind.svc/Orders" title="Orders"/>
        <entry>
            <category term="NorthwindModel.Orders" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <id>http://localhost:49192/northwind.svc/Orders(10248)</id>
            <updated/>
            <title/>
            <author>
                <name/>
            </author>
            <link rel="edit" href="//localhost:49192/northwind.svc/Orders(10248)" title="Orders"/>
            <link rel="http://docs.oasis-open.org/odata/ns/related/Customers" title="Customers" href="//localhost:49192/northwind.svc/Orders(10248)/Customers" type="application/atom+xml;type=entry"/>
            <link rel="http://docs.oasis-open.org/odata/ns/related/Employees" title="Employees" href="//localhost:49192/northwind.svc/Orders(10248)/Employees" type="application/atom+xml;type=entry"/>
            <link rel="http://docs.oasis-open.org/odata/ns/related/Shippers" title="Shippers" href="//localhost:49192/northwind.svc/Orders(10248)/Shippers" type="application/atom+xml;type=entry">
                <m:inline>
                    <entry>
                        <category term="NorthwindModel.Shippers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                        <id>http://localhost:49192/northwind.svc/Shippers(3)</id>
                        <updated/>
                        <title/>
                        <author>
                            <name/>
                        </author>
                        <link rel="edit" href="//localhost:49192/northwind.svc/Shippers(3)" title="Shippers"/>
                        <content type="application/xml">
                            <m:properties>
                                <d:ShipperID m:type="Int32">3</d:ShipperID>
                                <d:CompanyName>Federal Shipping</d:CompanyName>
                                <d:Phone>(503) 555-9931</d:Phone>
                            </m:properties>
                        </content>
                        <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" title="Orders" href="//localhost:49192/northwind.svc/Shippers(3)/Orders" type="application/atom+xml;type=feed">
                            <m:inline>
                            </m:inline>
                        </link>
                    </entry>
                </m:inline>
            </link>
            <content type="application/xml">
                <m:properties>
                    <d:OrderID m:type="Int32">10248</d:OrderID>
                    <d:OrderDate m:type="Nullable`1">1996-07-04T00:00:00Z</d:OrderDate>
                    <d:RequiredDate m:type="Nullable`1">1996-08-01T00:00:00Z</d:RequiredDate>
                    <d:ShippedDate m:type="Nullable`1">1996-07-16T00:00:00Z</d:ShippedDate>
                    <d:Freight m:type="Nullable`1">32.3800</d:Freight>
                    <d:ShipName>Vins et alcools Chevalier</d:ShipName>
                    <d:ShipAddress>59 rue de l'Abbaye</d:ShipAddress>
                    <d:ShipCity>Reims</d:ShipCity>
                    <d:ShipRegion/>
                    <d:ShipPostalCode>51100</d:ShipPostalCode>
                </m:properties>
            </content>
        </entry>
    </feed>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Orders), XAtom_CustomersWithShippersLiveOrder)
            Dim count As Integer = 0
            For Each instance As NorthwindSimpleModel.Orders In enumerable
                count += 1
                Assert.AreEqual(instance.GetType(), GetType(NorthwindSimpleModel.Orders))

                Assert.AreEqual(10248, instance.OrderID)
                Assert.AreEqual(XmlConvert.ToDateTimeOffset("1996-07-04T00:00:00Z"), instance.OrderDate.Value)
                Assert.AreEqual(XmlConvert.ToDateTimeOffset("1996-08-01T00:00:00Z"), instance.RequiredDate.Value)
                Assert.AreEqual(XmlConvert.ToDateTimeOffset("1996-07-16T00:00:00Z"), instance.ShippedDate.Value)
                Assert.AreEqual(Decimal.Parse("32.3800"), instance.Freight.Value)
                Assert.AreEqual(0, instance.ShipRegion.Length)
                Assert.IsNotNull(instance.Shippers)
                Assert.AreEqual(3, instance.Shippers.ShipperID)
                Assert.AreEqual("Federal Shipping", instance.Shippers.CompanyName)
                Assert.AreEqual("(503) 555-9931", instance.Shippers.Phone)
                Assert.IsNotNull(instance.Shippers.Orders)
                Assert.AreEqual(0, instance.Shippers.Orders.Count)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestName()
            Dim Name As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">Eastern</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(String), Name)
            Dim count As Integer = 0
            For Each instance As String In enumerable
                count += 1
                Assert.AreEqual("Eastern", instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestOrderId()
            Dim OrderId As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">1</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Int32), OrderId)
            Dim count As Integer = 0
            For Each instance As Int32 In enumerable
                count += 1
                Assert.AreEqual(1, instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestNullInt321()
            Dim NullableInt321 As XElement = _
             <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">2</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Nullable(Of Int32)), NullableInt321)
            Dim count As Integer = 0
            For Each instance As Nullable(Of Int32) In enumerable
                count += 1
                Assert.AreEqual(2, instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub NullableUInt()
            Dim NullableUIntValue As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">2147483648</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Nullable(Of UInt32)), NullableUIntValue)
            Dim count As Integer = 0
            For Each instance As Nullable(Of UInt32) In enumerable
                count += 1
                Assert.AreEqual(CUInt(Int32.MaxValue) + CUInt(1), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializeUriValue()
            Dim UriValue As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">http://www.microsoft.com/</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Uri), UriValue)
            Dim count As Integer = 0
            For Each instance As Uri In enumerable
                count += 1
                Assert.AreEqual(New Uri("http://www.microsoft.com/"), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializeTypeValue()
            Dim TypeValue As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">Microsoft.OData.Client.DataServiceContext</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Type), TypeValue)
            Dim count As Integer = 0
            For Each instance As Type In enumerable
                count += 1
                Assert.AreEqual(GetType(DataServiceContext), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializeTimespanValue()
            Dim TimespanValue As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">PT3H4M5S</m:value>            ' XSD Duration format
            Dim y As TimeSpan = New TimeSpan(3, 4, 5)
            Dim x As String = XmlConvert.ToString(y)
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(TimeSpan), TimespanValue)
            Dim count As Integer = 0
            For Each instance As TimeSpan In enumerable
                count += 1
                Assert.AreEqual(New TimeSpan(3, 4, 5), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializeDateTimeOffSetValue()
            Dim DateTimeOffSetValue As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">1980-04-05T23:04:14+08:05</m:value>            ' XSD Duration format
            Dim y As DateTimeOffset = New DateTimeOffset(New DateTime(1980, 4, 5, 23, 4, 14), New TimeSpan(8, 5, 0))
            Dim x As String = XmlConvert.ToString(y)
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(DateTimeOffset), DateTimeOffSetValue)
            Dim count As Integer = 0
            For Each instance As DateTimeOffset In enumerable
                count += 1
                Assert.AreEqual(y, instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestNames()
            Dim Names As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">
                    <m:Name>Eastern</m:Name>
                    <m:Name>Western</m:Name>
                </m:value>

            ' This is not a valid OData payload. Its okay to fail on these scenarios
            Dim exception As Exception = TestUtil.RunCatching(
                Sub()
                    Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(String), Names)
                    For Each instance In enumerable
                    Next
                End Sub)
            Assert.IsNotNull(exception, "Expected exception, but none was thrown")
            Assert.AreEqual(ODataLibResourceUtil.GetString("XmlReaderExtension_InvalidNodeInStringValue", "Element"), exception.Message, "Error message was not as expected")
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestByteArray()
            Dim ByteArray As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789/=</m:value>
            Dim x As Byte() = Convert.FromBase64String("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789/=")
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Byte()), ByteArray)
            Dim count As Integer = 0
            For Each instance As Byte() In enumerable
                count += 1
                Assert.AreEqual(x.Length(), instance.Length)
                For a As Integer = 0 To x.Length() - 1
                    Assert.AreEqual(x(a), instance(a))
                Next
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> _
        Public Sub MaterializerTestNullArray()
            Dim NullByteArray As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" m:null="true"/>

            Dim Null2ByteArray As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" m:null="true"></m:value>

            For Each x As XElement In New XElement() {NullByteArray, Null2ByteArray}
                For Each type As Type In New System.Type() {GetType(String), GetType(Byte()), GetType(Char()), GetType(Nullable(Of Int32)), GetType(Int32)}
                    Try
                        Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(type, x)
                        Dim count As Integer = 0
                        For Each instance As Object In enumerable
                            count += 1
                            Assert.AreSame(instance, Nothing)
                        Next
                        Assert.AreEqual(1, count)
                    Catch ex As InvalidOperationException
                        If (Not type Is GetType(Int32)) Then
                            Throw
                        End If
                    End Try
                Next
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> _
        Public Sub MaterializerTestEmptyArray()
            Dim EmptyByteArray As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata"/>

            Dim Empty2ByteArray As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" null="true" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata"/>

            Dim Empty3ByteArray As XElement = _
                <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata"></m:value>

            For Each x As XElement In New XElement() {EmptyByteArray, Empty2ByteArray, Empty3ByteArray}
                For Each type As Type In New System.Type() {GetType(String), GetType(Byte()), GetType(Char()), GetType(Nullable(Of Int32)), GetType(Int32), GetType(DateTimeOffset), GetType(Guid), GetType(Double)}
                    Try
                        Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(type, x)
                        Dim count As Integer = 0
                        For Each instance As Object In enumerable
                            count += 1
                            Assert.AreNotSame(instance, Nothing)
                            If (type Is GetType(String)) Then
                                Assert.AreEqual(CType(instance, String).Length, 0)
                            Else
                                Assert.AreEqual(CType(instance, System.Array).Length, 0)
                            End If
                        Next
                        Assert.AreEqual(1, count)
                    Catch ex As InvalidOperationException
                        If (type Is GetType(String) Or type.GetType().IsArray) Then
                            Throw
                        End If
                    End Try
                Next
            Next
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

        <TestCategory("Partition1")> <TestMethod()> _
        Public Sub MaterializerTestEmptyEntrys()
            Dim EmptyEntries As XElement = _
    <feed xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <entry>
            <category term="EmptyValueClass" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <id>http://localhost:3000/northwind.svc/EmptyValues('1')</id>
            <link rel="edit" href="//localhost:3000/northwind.svc/EmptyValues('0')" title="Customers"/>
            <content type="application/xml">
                <m:properties>
                    <d:BinaryProperty m:type="Edm.Binary"></d:BinaryProperty>
                    <d:ByteProperty m:type="Edm.Binary"></d:ByteProperty>
                    <d:CharProperty m:type="Edm.String"></d:CharProperty>
                    <d:StringProperty m:type="Edm.String"></d:StringProperty>
                    <d:XDocumentProperty m:type="Edm.String"></d:XDocumentProperty>
                    <!-- empty XElement not supported
                <d:XElementProperty m:type="Edm.String"></d:XElementProperty>
                -->
                    <d:UriProperty m:type="Edm.String"></d:UriProperty>
                </m:properties>
            </content>
        </entry>
        <entry>
            <category term="EmptyValueClass" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <id>http://localhost:3000/northwind.svc/EmptyValues('1')</id>
            <link rel="edit" href="//localhost:3000/northwind.svc/EmptyValues('1')" title="Customers"/>
            <content type="application/xml">
                <m:properties>
                    <d:BinaryProperty></d:BinaryProperty>
                    <d:ByteProperty></d:ByteProperty>
                    <d:CharProperty></d:CharProperty>
                    <d:StringProperty></d:StringProperty>
                    <d:XDocumentProperty></d:XDocumentProperty>
                    <!-- empty XElement not supported
                <d:XElementProperty></d:XElementProperty>
                -->
                    <d:UriProperty></d:UriProperty>
                </m:properties>
            </content>
        </entry>
        <entry>
            <category term="EmptyValueClass" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <id>http://localhost:3000/northwind.svc/EmptyValues('2')</id>
            <link rel="edit" href="//localhost:3000/northwind.svc/EmptyValues('2')" title="Customers"/>
            <content type="application/xml">
                <m:properties>
                    <d:BinaryProperty m:type="Edm.Binary" m:null="true"/>
                    <d:ByteProperty m:type="Edm.Binary" m:null="true"/>
                    <d:CharProperty m:type="Edm.String" m:null="true"/>
                    <d:StringProperty m:type="Edm.String" m:null="true"/>
                    <d:XDocumentProperty m:type="Edm.String" m:null="true"/>
                    <d:XElementProperty m:type="Edm.String" m:null="true"/>
                    <d:UriProperty m:type="Edm.String" m:null="true"/>
                </m:properties>
            </content>
        </entry>
        <entry>
            <category term="EmptyValueClass" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <id>http://localhost:3000/northwind.svc/EmptyValues('2')</id>
            <link rel="edit" href="//localhost:3000/northwind.svc/EmptyValues('3')" title="Customers"/>
            <content type="application/xml">
                <m:properties>
                    <d:BinaryProperty m:null="true"/>
                    <d:ByteProperty m:null="true"/>
                    <d:CharProperty m:null="true"/>
                    <d:StringProperty m:null="true"/>
                    <d:XDocumentProperty m:null="true"/>
                    <d:XElementProperty m:null="true"/>
                    <d:UriProperty m:null="true"/>
                </m:properties>
            </content>
        </entry>
        <entry>
            <category term="EmptyValueClass" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <id>http://localhost:3000/northwind.svc/EmptyValues('3')</id>
            <link rel="edit" href="//localhost:3000/northwind.svc/EmptyValues('4')" title="Customers"/>
            <content type="application/xml">
                <m:properties>
                    <d:BinaryProperty m:null="true"></d:BinaryProperty>
                    <d:ByteProperty m:null="true"></d:ByteProperty>
                    <d:CharProperty m:null="true"></d:CharProperty>
                    <d:StringProperty m:null="true"></d:StringProperty>
                    <d:XDocumentProperty m:null="true"></d:XDocumentProperty>
                    <d:XElementProperty m:null="true"></d:XElementProperty>
                    <d:UriProperty m:null="true"></d:UriProperty>
                </m:properties>
            </content>
        </entry>
        <entry>
            <category term="EmptyValueClass" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
            <id>http://localhost:3000/northwind.svc/EmptyValues('1')</id>
            <link rel="edit" href="//localhost:3000/northwind.svc/EmptyValues('5')" title="Customers"/>
            <content type="application/xml">
                <m:properties>
                    <d:BinaryProperty>ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567898=</d:BinaryProperty>
                    <d:ByteProperty>ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567988=</d:ByteProperty>
                    <d:CharProperty>I am a char array</d:CharProperty>
                    <d:StringProperty>I am a string</d:StringProperty>
                    <d:XDocumentProperty>&lt;startdoc&gt;value&lt;/startdoc&gt;</d:XDocumentProperty>
                    <d:XElementProperty>&lt;startelm&gt;value&lt;/startelm&gt;</d:XElementProperty>
                    <d:UriProperty>http://localhost:3000/northwind.svc</d:UriProperty>
                </m:properties>
            </content>
        </entry>
    </feed>

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(EmptyValueClass), EmptyEntries)
            Dim count As Integer = 0
            For Each instance As EmptyValueClass In enumerable
                Assert.IsInstanceOfType(instance, GetType(EmptyValueClass))
                count += 1

                If (count <= 2) Then
                    Assert.AreEqual(0, instance.BinaryProperty.Length)
                    Assert.AreEqual(0, instance.ByteProperty.Length)
                    Assert.AreEqual(0, instance.CharProperty.Length)
                    Assert.AreEqual(0, instance.StringProperty.Length)
                    Assert.AreEqual(0, instance.UriProperty.OriginalString.Length)
                    Assert.AreEqual("", instance.XDocumentProperty.ToString())
                    Assert.IsNull(instance.XElementProperty)
                ElseIf (count <= 5) Then
                    Assert.IsNull(instance.BinaryProperty)
                    Assert.IsNull(instance.ByteProperty)
                    Assert.IsNull(instance.CharProperty)
                    Assert.IsNull(instance.StringProperty)
                    Assert.IsNull(instance.UriProperty)
                    Assert.IsNull(instance.XDocumentProperty)
                    Assert.IsNull(instance.XElementProperty)
                Else
                    Assert.AreEqual(Convert.ToBase64String(Convert.FromBase64String("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567988=")), Convert.ToBase64String(instance.ByteProperty))
                    Assert.AreEqual("""" + Convert.ToBase64String(Convert.FromBase64String("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567898=")) + """", instance.BinaryProperty.ToString())
                    Assert.AreEqual("I am a char array", New String(instance.CharProperty))
                    Assert.AreEqual("I am a string", instance.StringProperty)
                    Assert.AreEqual(New XDocument(<startdoc>value</startdoc>).ToString(), instance.XDocumentProperty.ToString())
                    Assert.AreEqual(New XElement(<startelm>value</startelm>).ToString(), instance.XElementProperty.ToString())
                    Assert.AreEqual(New Uri("http://localhost:3000/northwind.svc"), instance.UriProperty)
                End If
            Next
            Assert.AreEqual(6, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestGuid()
            Dim Guid As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">{9E2F9304-C0AF-4d6c-B0A0-C3C4AD75486D}</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Guid), Guid)
            Dim count As Integer = 0
            For Each instance As Guid In enumerable
                count += 1
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestByte()
            Dim ByteValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">128</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Byte), ByteValue)
            Dim count As Integer = 0
            For Each instance As Byte In enumerable
                count += 1
                Assert.AreEqual(CByte(128), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestByteWithLeadingMixedContent()
            Dim ByteWithLeadingMixedContentValue As XElement = <Byte xmlns="http://docs.oasis-open.org/odata/ns/data"><a/>128</Byte>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Byte), ByteWithLeadingMixedContentValue)
            Dim count As Integer = 0
            For Each instance As Byte In enumerable
                count += 1
                Assert.AreEqual(CByte(128), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <ExpectedException(GetType(InvalidOperationException))> _
        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestByteWithTrailingMixedContent()
            Dim ByteWithTrailingMixedContentValue As XElement = <Byte xmlns="http://docs.oasis-open.org/odata/ns/data">128<a/></Byte>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Byte), ByteWithTrailingMixedContentValue)
            Dim count As Integer = 0
            For Each instance As Byte In enumerable
                count += 1
                Assert.AreEqual(CByte(128), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestByteWithLeadingComment()
            Dim ByteWithLeadingCommentValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata"><!--ignored-->128</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Byte), ByteWithLeadingCommentValue)
            Dim count As Integer = 0
            For Each instance As Byte In enumerable
                count += 1
                Assert.AreEqual(CByte(128), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        ' We decided to relax the client, and do what the server did
        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestByteWithMiddleComment()
            Dim ByteWithMiddleCommentValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">1<!--ignored-->28</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Byte), ByteWithMiddleCommentValue)
            Dim count As Integer = 0
            For Each instance As Byte In enumerable
                count += 1
                Assert.AreEqual(CByte(128), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestByteWithTrailingComment()
            Dim ByteWithTrailingCommentValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">128<!--ignored--></m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Byte), ByteWithTrailingCommentValue)
            Dim count As Integer = 0
            For Each instance As Byte In enumerable
                count += 1
                Assert.AreEqual(CByte(128), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestSByte()
            Dim SByteValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">-128</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(SByte), SByteValue)
            Dim count As Integer = 0
            For Each instance As SByte In enumerable
                count += 1
                Assert.AreEqual(CSByte(-128), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestChar()
            Dim CharValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">&amp;</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Char), CharValue)
            Dim count As Integer = 0
            For Each instance As Char In enumerable
                count += 1
                Assert.AreEqual("&"c, instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestCharArray()
            Dim CharArray As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789&amp;&lt;&gt;</m:value>
            Dim x As Char() = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789&<>".ToCharArray()
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Char()), CharArray)
            Dim count As Integer = 0
            For Each instance As Char() In enumerable
                count += 1
                Assert.AreEqual(x.Length(), instance.Length)
                For a As Integer = 0 To x.Length() - 1
                    Assert.AreEqual(x(a), instance(a))
                Next
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestComplexType()
            Dim xml As XElement = _
<entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
    <category term="AstoriaUnitTests.Stubs.Customer" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
    <id>http://foo/</id>
    <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
    <content type="application/xml">
        <m:properties>
            <d:ID>123</d:ID>
            <d:Address>
                <d:City>City Value</d:City>
                <d:State>State Value</d:State>
            </d:Address>
        </m:properties>
    </content>
</entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(AstoriaUnitTests.Stubs.Customer), xml)
            Dim found As Boolean = False
            For Each obj As AstoriaUnitTests.Stubs.Customer In enumerable
                found = True
                Assert.AreEqual(obj.ID, 123)
                Assert.AreEqual(obj.Address.City, "City Value")
                Assert.AreEqual(obj.Address.State, "State Value")
            Next
            Assert.IsTrue(found, "Customer materialized")
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestComplexTypeIncorrectType()
            Dim xml As XElement = _
<entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
    <category term="AstoriaUnitTests.Stubs.Customer" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
    <id>http://foo/</id>
    <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
    <content type="application/xml">
        <m:properties>
            <d:ID>123</d:ID>
            <d:Address m:type="Edm.Binary">
                <d:City>City Value</d:City>
                <d:State>State Value</d:State>
            </d:Address>
        </m:properties>
    </content>
</entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(AstoriaUnitTests.Stubs.Customer), xml)
            Try
                For Each obj As AstoriaUnitTests.Stubs.Customer In enumerable
                Next
                Assert.Fail("Expected InvalidOperationException when type doesn't match")
            Catch e As InvalidOperationException
                Assert.AreEqual(ODataLibResourceUtil.GetString("ValidationUtils_IncorrectTypeKind", "Edm.Binary", "Complex", "Primitive"), e.Message)
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestComplexTypeCreateType()
            Dim xml As XElement = _
<entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
    <category term="EntityWithNullComplexType" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
    <id>http://foo/</id>
    <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
    <content type="application/xml">
        <m:properties>
            <d:ID>123</d:ID>
            <d:TheAddress>
                <d:City>City Value</d:City>
                <d:State>State Value</d:State>
            </d:TheAddress>
        </m:properties>
    </content>
</entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(EntityWithNullComplexType), xml)
            Dim found As Boolean = False
            For Each obj As EntityWithNullComplexType In enumerable
                found = True
                Assert.AreEqual(obj.ID, 123)
                Assert.AreEqual(obj.TheAddress.City, "City Value")
                Assert.AreEqual(obj.TheAddress.State, "State Value")
            Next
            Assert.IsTrue(found, "Customer materialized")
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestLongValue()
            Dim LongValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">-9223372036854775808</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Int64), LongValue)
            Dim count As Integer = 0
            For Each instance As Int64 In enumerable
                count += 1
                Assert.AreEqual(Int64.MinValue, instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestULongValue()
            Dim ULongValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">9223372036854775808</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(UInt64), ULongValue)
            Dim count As Integer = 0
            For Each instance As UInt64 In enumerable
                count += 1
                Assert.AreEqual(CULng(Int64.MaxValue) + CULng(1), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestUShortValue()
            Dim UShortValue As XElement = <m:value xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">32768</m:value>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(UInt16), UShortValue)
            Dim count As Integer = 0
            For Each instance As UInt16 In enumerable
                count += 1
                Assert.AreEqual(CUShort(Int16.MaxValue) + CUShort(1), instance)
            Next
            Assert.AreEqual(1, count)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestObject_PrimitiveType()
            Dim ByteValue As XElement = <Byte m:type="Edm.Binary" xmlns="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">128</Byte>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Object), ByteValue)
            Dim count As Integer = 0

            Dim ex As InvalidOperationException = DirectCast(
                TestUtil.RunCatching(Sub()
                                         For Each instance As Byte In enumerable
                                         Next
                                     End Sub), 
                InvalidOperationException)

            Assert.IsNotNull(ex, "Expected exception, but none was thrown")
            Assert.AreEqual(DataServicesClientResourceUtil.GetString("ClientType_NoSettableFields", "System.Object"), ex.Message)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestObject_ComplexType()
            Dim xml As XElement = _
            <Address m:type="AstoriaUnitTests.Stubs.Address" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://docs.oasis-open.org/odata/ns/data">
                <City>City Value</City>
                <State>State Value</State>
            </Address>

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Object), xml)

            Dim ex As InvalidOperationException = DirectCast(
                TestUtil.RunCatching(Sub()
                                         For Each instance As Byte In enumerable
                                         Next
                                     End Sub), 
                InvalidOperationException)

            Assert.IsNotNull(ex, "Expected exception, but none was thrown")
            Assert.AreEqual(DataServicesClientResourceUtil.GetString("ClientType_NoSettableFields", "System.Object"), ex.Message)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestObject_CollectionComplex()
            Dim xml As XElement = _
            <Addresses m:type="Collection(AstoriaUnitTests.Stubs.Address)" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://docs.oasis-open.org/odata/ns/data">
                <element>
                    <City>City Value</City>
                    <State>State Value</State>
                </element>
            </Addresses>

            Dim ex As InvalidOperationException = DirectCast(
                TestUtil.RunCatching(Sub()
                                         EnumerateAtom(GetType(ICollection(Of Object)), xml)
                                     End Sub).InnerException, 
                InvalidOperationException)

            Assert.IsNotNull(ex, "Expected exception, but none was thrown")
            Assert.AreEqual(DataServicesClientResourceUtil.GetString("ClientType_NoSettableFields", "System.Object"), ex.Message)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestObject_EntityType()
            Dim xml As XElement = _
<entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
    <category term="AstoriaUnitTests.Stubs.Customer" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
    <id>http://foo/</id>
    <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
    <content type="application/xml">
        <m:properties>
            <d:ID>123</d:ID>
            <d:Address>
                <d:City>City Value</d:City>
                <d:State>State Value</d:State>
            </d:Address>
        </m:properties>
    </content>
</entry>

            Dim ctx As New DataServiceContext(New Uri("http://localhost/"))
            ctx.EnableAtom = True
            ctx.MergeOption = MergeOption.NoTracking
            ctx.ResolveType = (Function(typeName)
                                   If typeName = "AstoriaUnitTests.Stubs.Customer" Then
                                       Return GetType(AstoriaUnitTests.Stubs.Customer)
                                   End If

                                   Return Nothing
                               End Function)

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(ctx, GetType(Object), xml.ToString(), MergeOption.NoTracking).Enumerable
            Dim found As Boolean = False
            For Each obj As AstoriaUnitTests.Stubs.Customer In enumerable
                found = True
                Assert.AreEqual(obj.ID, 123)
                Assert.AreEqual(obj.Address.City, "City Value")
                Assert.AreEqual(obj.Address.State, "State Value")
            Next
            Assert.IsTrue(found, "Customer materialized")
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerTestObject_FeedType()
            Dim xml As XElement = _
<feed xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
    <entry>
        <category term="AstoriaUnitTests.Stubs.Customer" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <id>http://foo/</id>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            <m:properties>
                <d:ID>123</d:ID>
                <d:Address>
                    <d:City>City Value</d:City>
                    <d:State>State Value</d:State>
                </d:Address>
            </m:properties>
        </content>
    </entry>
</feed>

            Dim ctx As New DataServiceContext(New Uri("http://localhost/"))
            ctx.EnableAtom = True
            ctx.MergeOption = MergeOption.NoTracking
            ctx.ResolveType = (Function(typeName)
                                   If typeName = "AstoriaUnitTests.Stubs.Customer" Then
                                       Return GetType(AstoriaUnitTests.Stubs.Customer)
                                   End If

                                   Return Nothing
                               End Function)

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(ctx, GetType(Object), xml.ToString(), MergeOption.NoTracking).Enumerable
            Dim found As Boolean = False
            For Each obj As AstoriaUnitTests.Stubs.Customer In enumerable
                found = True
                Assert.AreEqual(obj.ID, 123)
                Assert.AreEqual(obj.Address.City, "City Value")
                Assert.AreEqual(obj.Address.State, "State Value")
            Next
            Assert.IsTrue(found, "Customer materialized")
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub ParseSampleException()
            Dim stream As Stream = Util.GetResourceStream("AstoriaClientUnitTests.SampleException.xml")
            Dim reader = Util.CreateTextReader(stream)
            Dim enumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Summary_of_Sales_by_Year), reader.ReadToEnd())

            Try
                For Each sale As Object In enumerable
                Next
            Catch ex As Exception
                Dim text = PrintException(ex)
                Assert.IsTrue(text.Contains("An error occurred while reading from the store provider's data reader. See the inner exception for details."), "{0}", ex)
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub Atom_MissingId()
            Dim XAtom_MissingIdElement As XElement = _
    <entry xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
        <category term="NorthwindModel.Customers" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
        <link rel="edit" href="//localhost:3000/northwind.svc/Customers('ALFKI')" title="Customers"/>
        <content type="application/xml">
            <m:properties>
                <d:CustomerID>ALFKI</d:CustomerID>
                <d:CompanyName>Alfreds Futterkiste</d:CompanyName>
            </m:properties>
        </content>
    </entry>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(NorthwindSimpleModel.Customers), XAtom_MissingIdElement)
            Try
                For Each obj As Object In enumerable
                Next
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
                Assert.AreEqual(DataServicesClientResourceUtil.GetString("Deserialize_MissingIdElement"), ex.Message)
            End Try
        End Sub

#Region "Row Count Materializer"

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializeRowCountSimple()
            Dim XAtom_RowCountSimple As XElement = _
            <feed xml:base="http://host/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                <title type="text">Orders</title>
                <id>http://host/Orders</id>
                <updated>2008-12-09T23:03:10Z</updated>
                <link rel="self" title="Orders" href="Orders"/>
                <m:count>122</m:count>
            </feed>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(northwindClient.Customers), XAtom_RowCountSimple)
            Dim countValue As Long = ReadCountValue(enumerable, GetType(northwindClient.Customers))
            Assert.AreEqual(countValue, 122L)
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializeRowCountMissingCount()
            Dim XAtom_RowCountMissingCount As XElement = _
            <feed xml:base="http://host/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                <title type="text">Orders</title>
                <id>http://host/Orders</id>
                <updated>2008-12-09T23:03:10Z</updated>
                <link rel="self" title="Orders" href="Orders"/>
                <entry>
                    <id>http://host/Orders(10249)</id>
                    <title type="text"></title>
                    <updated>2008-12-09T23:03:10Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel="edit" title="Orders" href="Orders(10249)"/>
                    <link rel="http://docs.oasis-open.org/odata/ns/related/Customers" type="application/atom+xml;type=entry" title="Customers" href="Orders(10249)/Customers"/>
                    <link rel="http://docs.oasis-open.org/odata/ns/related/Employees" type="application/atom+xml;type=entry" title="Employees" href="Orders(10249)/Employees"/>
                    <link rel="http://docs.oasis-open.org/odata/ns/related/Order_Details" type="application/atom+xml;type=feed" title="Order_Details" href="Orders(10249)/Order_Details"/>
                    <link rel="http://docs.oasis-open.org/odata/ns/related/Shippers" type="application/atom+xml;type=entry" title="Shippers" href="Orders(10249)/Shippers"/>
                    <category term="NorthwindModel.Orders" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                    <content type="application/xml">
                        <m:properties>
                            <d:OrderID m:type="Edm.Int32">10249</d:OrderID>
                            <d:OrderDate m:type="Edm.DateTimeOffset">1996-07-05T00:00:00Z</d:OrderDate>
                            <d:RequiredDate m:type="Edm.DateTimeOffset">1996-08-16T00:00:00Z</d:RequiredDate>
                            <d:ShippedDate m:type="Edm.DateTimeOffset">1996-07-10T00:00:00Z</d:ShippedDate>
                            <d:Freight m:type="Edm.Decimal">11.6100</d:Freight>
                            <d:ShipName>Toms Spezialitäten</d:ShipName>
                            <d:ShipAddress>Luisenstr. 48</d:ShipAddress>
                            <d:ShipCity>Münster</d:ShipCity>
                            <d:ShipRegion m:null="true"/>
                            <d:ShipPostalCode>44087</d:ShipPostalCode>
                        </m:properties>
                    </content>
                </entry>
            </feed>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(northwindClient.Customers), XAtom_RowCountMissingCount)
            Try
                Dim countValue As Long = ReadCountValue(enumerable, GetType(northwindClient.Customers))
                Assert.Fail("Exception failed to throw")
            Catch ex As Reflection.TargetInvocationException
                Dim innerEx As Exception = ex.InnerException
                If Not innerEx Is Nothing Then
                    Assert.AreEqual(innerEx.Message, "Count value is not part of the response stream.")
                End If
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializeRowCountNegativeCount()
            Dim XAtom_RowCountNegativeCount As XElement = _
            <feed xml:base="http://host/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                <title type="text">Orders</title>
                <id>http://host/Orders</id>
                <updated>2008-12-09T23:03:10Z</updated>
                <link rel="self" title="Orders" href="Orders"/>
                <m:count>-1</m:count>
            </feed>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(northwindClient.Customers), XAtom_RowCountNegativeCount)
            Try
                Dim countValue As Long = ReadCountValue(enumerable, GetType(northwindClient.Customers))
                Assert.AreEqual(-1L, countValue)
            Catch ex As Reflection.TargetInvocationException
                Assert.Fail("Failed to parse negative count value")
            End Try
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializeRowCountNonIntegerCount()
            Dim XAtom_RowCountNonIntegerCount As XElement = _
            <feed xml:base="http://host/" xmlns:d="http://docs.oasis-open.org/odata/ns/data" xmlns:m="http://docs.oasis-open.org/odata/ns/metadata" xmlns="http://www.w3.org/2005/Atom">
                <title type="text">Orders</title>
                <id>http://host/Orders</id>
                <updated>2008-12-09T23:03:10Z</updated>
                <link rel="self" title="Orders" href="Orders"/>
                <m:count>1234.567</m:count>
            </feed>
            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(northwindClient.Customers), XAtom_RowCountNonIntegerCount)
            Try
                Dim countValue As Long = ReadCountValue(enumerable, GetType(northwindClient.Customers))
                Assert.Fail("Exception failed to throw")
            Catch ex As Reflection.TargetInvocationException
                Dim innerEx As Exception = ex.InnerException
                If Not innerEx Is Nothing Then
                    Assert.IsTrue(innerEx.Message.Contains("Cannot convert"))
                End If
            End Try
        End Sub

        Private Function ReadCountValue(ByRef materializer As System.Collections.IEnumerable, ByVal type As System.Type) As Long
            Dim materializerType As Type = GetMaterializerType(type)
            Dim countMethod As MethodInfo = materializerType.GetMethod("CountValue", BindingFlags.Instance Or BindingFlags.NonPublic)
            Dim countValue As Long = CType(countMethod.Invoke(materializer, Nothing), Long)

            Return countValue
        End Function

#End Region

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializingPropertiesMixedNamespace()
            Dim XAtom_MixNSProps As XElement = <feed xml:base="http://localhost:59672/northwind.svc/"
                                                   xmlns:d="http://docs.oasis-open.org/odata/ns/data"
                                                   xmlns:m="http://docs.oasis-open.org/odata/ns/metadata"
                                                   xmlns:test="http://schemas.microsoft.com/ado/test"
                                                   xmlns="http://www.w3.org/2005/Atom">
                                                   <title type="text">Customers</title>
                                                   <id>http://localhost:59672/northwind.svc/Customers</id>
                                                   <updated>2009-08-20T22:06:38Z</updated>
                                                   <link rel="self" title="Customers" href="Customers"/>
                                                   <entry>
                                                       <id>http://localhost:59672/northwind.svc/Customers(0)</id>
                                                       <title type="text"/>
                                                       <updated>2009-08-20T22:06:38Z</updated>
                                                       <author>
                                                           <name/>
                                                       </author>
                                                       <link rel="edit" title="Customer" href="Customers(0)"/>
                                                       <link rel="http://docs.oasis-open.org/odata/ns/related/BestFriend" type="application/atom+xml;type=entry" title="BestFriend" href="Customers(0)/BestFriend"/>
                                                       <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" type="application/atom+xml;type=feed" title="Orders" href="Customers(0)/Orders"/>
                                                       <category term="AstoriaUnitTests.Stubs.Customer" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                                                       <content type="application/xml">
                                                           <m:properties>
                                                               <d:GuidValue m:type="Edm.Guid">d0071a3f-2ee8-4b14-90ed-5ef0e80f9525</d:GuidValue>
                                                               <d:ID m:type="Edm.Int32">0</d:ID>
                                                               <Name>Customer 0</Name>
                                                               <m:NameAsHtml><html><body>Customer 0</body></html></m:NameAsHtml>
                                                               <test:foo>bar</test:foo>
                                                               <d:Address m:type="AstoriaUnitTests.Stubs.Address">
                                                                   <d:StreetAddress>Line1</d:StreetAddress>
                                                                   <m:City>Redmond</m:City>
                                                                   <test:State>WA</test:State>
                                                                   <d:PostalCode>98052</d:PostalCode>
                                                                   <test:foo>bar</test:foo>
                                                               </d:Address>
                                                           </m:properties>
                                                       </content>
                                                   </entry>
                                               </feed>

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Customer), XAtom_MixNSProps)

            For Each c As Customer In enumerable
                Assert.AreEqual(0, c.ID)
                Assert.IsNotNull(c.GuidValue)
                Assert.IsNull(c.NameAsHtml)
                Assert.IsNotNull(c.Address)
                Assert.IsNull(c.Address.City)
                Assert.IsNull(c.Address.State)
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod()> Public Sub MaterializerNoContent()
            Dim XAtom As XElement = <feed xml:base="http://localhost:59672/northwind.svc/"
                                        xmlns:d="http://docs.oasis-open.org/odata/ns/data"
                                        xmlns:m="http://docs.oasis-open.org/odata/ns/metadata"
                                        xmlns:test="http://schemas.microsoft.com/ado/test"
                                        xmlns="http://www.w3.org/2005/Atom">
                                        <title type="text">Customers</title>
                                        <id>http://localhost:59672/northwind.svc/Customers</id>
                                        <updated>2009-08-20T22:06:38Z</updated>
                                        <link rel="self" title="Customers" href="Customers"/>
                                        <entry>
                                            <id>http://localhost:59672/northwind.svc/Customers(0)</id>
                                            <title type="text"/>
                                            <updated>2009-08-20T22:06:38Z</updated>
                                            <author>
                                                <name/>
                                            </author>
                                            <link rel="edit" title="Customer" href="Customers(0)"/>
                                            <link rel="http://docs.oasis-open.org/odata/ns/related/BestFriend" type="application/atom+xml;type=entry" title="BestFriend" href="Customers(0)/BestFriend"/>
                                            <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" type="application/atom+xml;type=feed" title="Orders" href="Customers(0)/Orders"/>
                                            <category term="AstoriaUnitTests.Stubs.Customer" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                                        </entry>
                                    </feed>

            Dim enumerable As System.Collections.IEnumerable = EnumerateAtom(GetType(Customer), XAtom)

            ' atom:content is missing but we should not fail
            ' This is a sanity check to make sure this works, but AstoriaUnitTests.Tests.AtomParserTests.AtomParserMissingContentTest does more extensive validation of the materialized object
            enumerable.GetEnumerator().MoveNext()
        End Sub

        Public Class SomeComplexType
            Public Property Name() As String
                Get
                    Return ""
                End Get
                Set(ByVal value As String)

                End Set
            End Property
        End Class
        <TestCategory("Partition1")> <TestMethod()> Public Sub EmptyFeedMoveNext()
            Dim XAtom As XElement = <service xml:base="http://localhost/Northwind_PQIAN1_fb35412c30454e12a28f18f2bd2923f4/Northwind.svc/" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:app="http://www.w3.org/2007/app" xmlns="http://www.w3.org/2007/app">
                                        <workspace>
                                            <atom:title>Default</atom:title>
                                            <collection href="EntitySet">
                                                <atom:title>EntitySet</atom:title>
                                            </collection>
                                        </workspace>
                                        <entry>
                                            <id>http://localhost:59672/northwind.svc/Customers(0)</id>
                                            <title type="text"/>
                                            <updated>2009-08-20T22:06:38Z</updated>
                                            <author>
                                                <name/>
                                            </author>
                                            <link rel="edit" title="Customer" href="Customers(0)"/>
                                            <link rel="http://docs.oasis-open.org/odata/ns/related/BestFriend" type="application/atom+xml;type=entry" title="BestFriend" href="Customers(0)/BestFriend"/>
                                            <link rel="http://docs.oasis-open.org/odata/ns/related/Orders" type="application/atom+xml;type=feed" title="Orders" href="Customers(0)/Orders"/>
                                            <category term="AstoriaUnitTests.Stubs.Customer" scheme="http://docs.oasis-open.org/odata/ns/scheme"/>
                                            <content type="application/xml">
                                            </content>
                                        </entry>
                                    </service>

            TestUtil.RunCombinations(
                New Type() {GetType(NorthwindSimpleModel.Customers), GetType(SomeComplexType), GetType(Int32)},
                Sub(type)
                    Dim exception As Exception = TestUtil.RunCatching(
                        Sub()
                            EnumerateAtom(type, XAtom)
                        End Sub)
                    Assert.IsNotNull(exception, "Expected exception, but none thrown")
                    Assert.AreEqual(GetType(InvalidOperationException), exception.InnerException.GetType(), "exception type did not match")
                    Assert.AreEqual(DataServicesClientResourceUtil.GetString("AtomMaterializer_InvalidResponsePayload", "http://docs.oasis-open.org/odata/ns/data"), exception.InnerException.Message, "error string did not match")
                End Sub)
        End Sub
    End Class
End Class

<CLSCompliant(False)> _
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
