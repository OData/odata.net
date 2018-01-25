'---------------------------------------------------------------------
' <copyright file="ClientProjections.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports Microsoft.OData.Service
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Text
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports System.ServiceModel.Web
Imports Microsoft.Test.ModuleCore
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Linq.Expressions

Partial Public Class ClientModule

    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    'Remove Atom
    ' <TestClass()>
    Public Class ClientProjectionTests
        Inherits AstoriaTestCase

        Private Shared web As TestWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing

#Region "Test Entity"

        <Key("Int")>
        Public Class TestEntity
            Private m_long As Long
            Private m_int As Integer
            Private m_float As Single
            Private m_double As Double
            Private m_Decimal As Decimal
            Private m_string As String
            Private m_DateTimeOffset As DateTimeOffset

            Public Property LongValue() As Long
                Get
                    Return m_long
                End Get
                Set(ByVal value As Long)
                    m_long = value
                End Set
            End Property

            Public Property Int() As Integer
                Get
                    Return m_int
                End Get
                Set(ByVal value As Integer)
                    m_int = value
                End Set
            End Property

            Public Property Float() As Single
                Get
                    Return m_float
                End Get
                Set(ByVal value As Single)
                    m_float = value
                End Set
            End Property

            Public Property DoubleVal() As Double
                Get
                    Return m_double
                End Get
                Set(ByVal value As Double)
                    m_double = value
                End Set
            End Property

            Public Property DecimalVal() As Decimal
                Get
                    Return m_Decimal
                End Get
                Set(ByVal value As Decimal)
                    m_Decimal = value
                End Set
            End Property

            Public Property StringVal() As String
                Get
                    Return m_string
                End Get
                Set(ByVal value As String)
                    m_string = value
                End Set
            End Property

            Public Property DateTimeOffsetVal() As DateTimeOffset
                Get
                    Return m_DateTimeOffset
                End Get
                Set(ByVal value As DateTimeOffset)
                    m_DateTimeOffset = value
                End Set
            End Property
        End Class

        Public Class ProjectionTestContext
            Public Shared TestValue As Double = 1.0

            Public ReadOnly Property Values() As IQueryable(Of TestEntity)
                Get
                    Return New TestEntity() _
                     {New TestEntity() With
                      {.LongValue = CType(TestValue, Long), .Int = CType(TestValue, Integer), .Float = CType(TestValue, Single), .DoubleVal = TestValue, .DecimalVal = CType(TestValue, Decimal), .StringVal = TestValue.ToString(), .DateTimeOffsetVal = New DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8.0))}}.AsQueryable()

                End Get
            End Property
        End Class

#End Region

#Region "Additional test attributes"

        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.DataServiceType = GetType(ProjectionTestContext)
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

        Private Shared Function _MakeArray(Of T)(ByVal ParamArray values() As T) As T()
            Return values
        End Function

#Region "Cross Feature Tests - VB Compiler/Projections"
        ' NOTE OF AUTOMATION
        ' We cannot dynamically generate these expressions, since we are testing the behaviour of VB compiler

        <TestCategory("Partition1")> <TestMethod(), Variation("Test VB compiler generated expressions with projection")>
        Public Sub ClientProjection_ProjectBinaryTwoPropsResultComplexType()
            Dim baseQuery = ctx.CreateQuery(Of TestEntity)("Values")
            Dim blEntity = New ProjectionTestContext().Values.FirstOrDefault()

            ' Binary with two properties
            Dim binaryPropertiesTestValues() = _MakeArray(
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue + t.LongValue, Object)}), .result = DirectCast(blEntity.LongValue + blEntity.LongValue, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue + t.Int, Object)}), .result = DirectCast(blEntity.LongValue + blEntity.Int, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue + t.Float, Object)}), .result = DirectCast(blEntity.LongValue + blEntity.Float, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue + t.DoubleVal, Object)}), .result = DirectCast(blEntity.LongValue + blEntity.DoubleVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue + t.DecimalVal, Object)}), .result = DirectCast(blEntity.LongValue + blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Int + t.Int, Object)}), .result = DirectCast(blEntity.Int + blEntity.Int, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Int + t.Float, Object)}), .result = DirectCast(blEntity.Int + blEntity.Float, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Int + t.DoubleVal, Object)}), .result = DirectCast(blEntity.Int + blEntity.DoubleVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Int + t.DecimalVal, Object)}), .result = DirectCast(blEntity.Int + blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Float + t.Float, Object)}), .result = DirectCast(blEntity.Float + blEntity.Float, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Float + t.DoubleVal, Object)}), .result = DirectCast(blEntity.Float + blEntity.DoubleVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Float + t.DecimalVal, Object)}), .result = DirectCast(blEntity.Float + blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DoubleVal + t.DoubleVal, Object)}), .result = DirectCast(blEntity.DoubleVal + blEntity.DoubleVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DoubleVal + t.DecimalVal, Object)}), .result = DirectCast(blEntity.DoubleVal + blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DecimalVal + t.DecimalVal, Object)}), .result = DirectCast(blEntity.DecimalVal + blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue / t.LongValue, Object)}), .result = DirectCast(blEntity.LongValue / blEntity.LongValue, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue / t.Int, Object)}), .result = DirectCast(blEntity.LongValue / blEntity.Int, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue / t.Float, Object)}), .result = DirectCast(blEntity.LongValue / blEntity.Float, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue / t.DoubleVal, Object)}), .result = DirectCast(blEntity.LongValue / blEntity.DoubleVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.LongValue / t.DecimalVal, Object)}), .result = DirectCast(blEntity.LongValue / blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Int / t.Int, Object)}), .result = DirectCast(blEntity.Int / blEntity.Int, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Int / t.Float, Object)}), .result = DirectCast(blEntity.Int / blEntity.Float, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Int / t.DoubleVal, Object)}), .result = DirectCast(blEntity.Int / blEntity.DoubleVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Int / t.DecimalVal, Object)}), .result = DirectCast(blEntity.Int / blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Float / t.Float, Object)}), .result = DirectCast(blEntity.Float / blEntity.Float, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Float / t.DoubleVal, Object)}), .result = DirectCast(blEntity.Float / blEntity.DoubleVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.Float / t.DecimalVal, Object)}), .result = DirectCast(blEntity.Float / blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DoubleVal / t.DoubleVal, Object)}), .result = DirectCast(blEntity.DoubleVal / blEntity.DoubleVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DoubleVal / t.DecimalVal, Object)}), .result = DirectCast(blEntity.DoubleVal / blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DecimalVal / t.DecimalVal, Object)}), .result = DirectCast(blEntity.DecimalVal / blEntity.DecimalVal, Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.LongValue + t.LongValue, t.LongValue), Object)}), .result = DirectCast(Math.Pow(blEntity.LongValue + blEntity.LongValue, blEntity.LongValue), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.LongValue + t.Int, t.Int), Object)}), .result = DirectCast(Math.Pow(blEntity.LongValue + blEntity.Int, blEntity.Int), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.LongValue + t.Float, t.Float), Object)}), .result = DirectCast(Math.Pow(blEntity.LongValue + blEntity.Float, blEntity.Float), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.LongValue + t.DoubleVal, t.DoubleVal), Object)}), .result = DirectCast(Math.Pow(blEntity.LongValue + blEntity.DoubleVal, blEntity.DoubleVal), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.LongValue + t.DecimalVal, t.DecimalVal), Object)}), .result = DirectCast(Math.Pow(blEntity.LongValue + blEntity.DecimalVal, blEntity.DecimalVal), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.Int + t.Int, t.Int), Object)}), .result = DirectCast(Math.Pow(blEntity.Int + blEntity.Int, blEntity.Int), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.Int + t.Float, t.Float), Object)}), .result = DirectCast(Math.Pow(blEntity.Int + blEntity.Float, blEntity.Float), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.Int + t.DoubleVal, t.DoubleVal), Object)}), .result = DirectCast(Math.Pow(blEntity.Int + blEntity.DoubleVal, blEntity.DoubleVal), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.Int + t.DecimalVal, t.DecimalVal), Object)}), .result = DirectCast(Math.Pow(blEntity.Int + blEntity.DecimalVal, blEntity.DecimalVal), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.Float + t.Float, t.Float), Object)}), .result = DirectCast(Math.Pow(blEntity.Float + blEntity.Float, blEntity.Float), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.Float + t.DoubleVal, t.DoubleVal), Object)}), .result = DirectCast(Math.Pow(blEntity.Float + blEntity.DoubleVal, blEntity.DoubleVal), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.Float + t.DecimalVal, t.DecimalVal), Object)}), .result = DirectCast(Math.Pow(blEntity.Float + blEntity.DecimalVal, blEntity.DecimalVal), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.DoubleVal + t.DoubleVal, t.DoubleVal), Object)}), .result = DirectCast(Math.Pow(blEntity.DoubleVal + blEntity.DoubleVal, blEntity.DoubleVal), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.DoubleVal + t.DecimalVal, t.DecimalVal), Object)}), .result = DirectCast(Math.Pow(blEntity.DoubleVal + blEntity.DecimalVal, blEntity.DecimalVal), Object)},
                New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Pow(t.DecimalVal + t.DecimalVal, t.DecimalVal), Object)}), .result = DirectCast(Math.Pow(blEntity.DecimalVal + blEntity.DecimalVal, blEntity.DecimalVal), Object)})

            For Each t In binaryPropertiesTestValues
                Assert.AreEqual(t.result, t.query.FirstOrDefault().result, t.query.Expression.ToString())
            Next

        End Sub

        <TestCategory("Partition1")> <TestMethod(), Variation("Test VB compiler generated expressions with projection")>
        Public Sub ClientProjection_ProjectUnaryResultComplexType()
            Dim baseQuery = ctx.CreateQuery(Of TestEntity)("Values")

            ' Unary Operations
            For Each v In _MakeArray(1, -1, 0.1, -0.01D)
                ProjectionTestContext.TestValue = v
                Dim blEntity = New ProjectionTestContext().Values.FirstOrDefault()

                Dim unaryTestValues() = _MakeArray(
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Abs(t.LongValue), Object)}), .result = DirectCast(Math.Abs(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Abs(t.Int), Object)}), .result = DirectCast(Math.Abs(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Abs(t.Float), Object)}), .result = DirectCast(Math.Abs(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Abs(t.DoubleVal), Object)}), .result = DirectCast(Math.Abs(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Abs(t.DecimalVal), Object)}), .result = DirectCast(Math.Abs(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Acos(t.LongValue), Object)}), .result = DirectCast(Math.Acos(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Acos(t.Int), Object)}), .result = DirectCast(Math.Acos(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Acos(t.Float), Object)}), .result = DirectCast(Math.Acos(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Acos(t.DoubleVal), Object)}), .result = DirectCast(Math.Acos(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Acos(t.DecimalVal), Object)}), .result = DirectCast(Math.Acos(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Asin(t.LongValue), Object)}), .result = DirectCast(Math.Asin(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Asin(t.Int), Object)}), .result = DirectCast(Math.Asin(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Asin(t.Float), Object)}), .result = DirectCast(Math.Asin(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Asin(t.DoubleVal), Object)}), .result = DirectCast(Math.Asin(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Asin(t.DecimalVal), Object)}), .result = DirectCast(Math.Asin(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Atan(t.LongValue), Object)}), .result = DirectCast(Math.Atan(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Atan(t.Int), Object)}), .result = DirectCast(Math.Atan(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Atan(t.Float), Object)}), .result = DirectCast(Math.Atan(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Atan(t.DoubleVal), Object)}), .result = DirectCast(Math.Atan(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Atan(t.DecimalVal), Object)}), .result = DirectCast(Math.Atan(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Ceiling(t.LongValue), Object)}), .result = DirectCast(Math.Ceiling(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Ceiling(t.Int), Object)}), .result = DirectCast(Math.Ceiling(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Ceiling(t.Float), Object)}), .result = DirectCast(Math.Ceiling(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Ceiling(t.DoubleVal), Object)}), .result = DirectCast(Math.Ceiling(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Ceiling(t.DecimalVal), Object)}), .result = DirectCast(Math.Ceiling(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cos(t.LongValue), Object)}), .result = DirectCast(Math.Cos(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cos(t.Int), Object)}), .result = DirectCast(Math.Cos(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cos(t.Float), Object)}), .result = DirectCast(Math.Cos(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cos(t.DoubleVal), Object)}), .result = DirectCast(Math.Cos(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cos(t.DecimalVal), Object)}), .result = DirectCast(Math.Cos(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cosh(t.LongValue), Object)}), .result = DirectCast(Math.Cosh(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cosh(t.Int), Object)}), .result = DirectCast(Math.Cosh(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cosh(t.Float), Object)}), .result = DirectCast(Math.Cosh(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cosh(t.DoubleVal), Object)}), .result = DirectCast(Math.Cosh(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Cosh(t.DecimalVal), Object)}), .result = DirectCast(Math.Cosh(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Exp(t.LongValue), Object)}), .result = DirectCast(Math.Exp(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Exp(t.Int), Object)}), .result = DirectCast(Math.Exp(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Exp(t.Float), Object)}), .result = DirectCast(Math.Exp(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Exp(t.DoubleVal), Object)}), .result = DirectCast(Math.Exp(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Exp(t.DecimalVal), Object)}), .result = DirectCast(Math.Exp(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Floor(t.LongValue), Object)}), .result = DirectCast(Math.Floor(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Floor(t.Int), Object)}), .result = DirectCast(Math.Floor(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Floor(t.Float), Object)}), .result = DirectCast(Math.Floor(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Floor(t.DoubleVal), Object)}), .result = DirectCast(Math.Floor(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Floor(t.DecimalVal), Object)}), .result = DirectCast(Math.Floor(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log(t.LongValue), Object)}), .result = DirectCast(Math.Log(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log(t.Int), Object)}), .result = DirectCast(Math.Log(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log(t.Float), Object)}), .result = DirectCast(Math.Log(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log(t.DoubleVal), Object)}), .result = DirectCast(Math.Log(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log(t.DecimalVal), Object)}), .result = DirectCast(Math.Log(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log10(t.LongValue), Object)}), .result = DirectCast(Math.Log10(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log10(t.Int), Object)}), .result = DirectCast(Math.Log10(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log10(t.Float), Object)}), .result = DirectCast(Math.Log10(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log10(t.DoubleVal), Object)}), .result = DirectCast(Math.Log10(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Log10(t.DecimalVal), Object)}), .result = DirectCast(Math.Log10(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Round(t.LongValue), Object)}), .result = DirectCast(Math.Round(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Round(t.Int), Object)}), .result = DirectCast(Math.Round(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Round(t.Float), Object)}), .result = DirectCast(Math.Round(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Round(t.DoubleVal), Object)}), .result = DirectCast(Math.Round(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Round(t.DecimalVal), Object)}), .result = DirectCast(Math.Round(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sin(t.LongValue), Object)}), .result = DirectCast(Math.Sin(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sin(t.Int), Object)}), .result = DirectCast(Math.Sin(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sin(t.Float), Object)}), .result = DirectCast(Math.Sin(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sin(t.DoubleVal), Object)}), .result = DirectCast(Math.Sin(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sin(t.DecimalVal), Object)}), .result = DirectCast(Math.Sin(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sinh(t.LongValue), Object)}), .result = DirectCast(Math.Sinh(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sinh(t.Int), Object)}), .result = DirectCast(Math.Sinh(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sinh(t.Float), Object)}), .result = DirectCast(Math.Sinh(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sinh(t.DoubleVal), Object)}), .result = DirectCast(Math.Sinh(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sinh(t.DecimalVal), Object)}), .result = DirectCast(Math.Sinh(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sqrt(t.LongValue), Object)}), .result = DirectCast(Math.Sqrt(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sqrt(t.Int), Object)}), .result = DirectCast(Math.Sqrt(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sqrt(t.Float), Object)}), .result = DirectCast(Math.Sqrt(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sqrt(t.DoubleVal), Object)}), .result = DirectCast(Math.Sqrt(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Sqrt(t.DecimalVal), Object)}), .result = DirectCast(Math.Sqrt(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tan(t.LongValue), Object)}), .result = DirectCast(Math.Tan(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tan(t.Int), Object)}), .result = DirectCast(Math.Tan(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tan(t.Float), Object)}), .result = DirectCast(Math.Tan(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tan(t.DoubleVal), Object)}), .result = DirectCast(Math.Tan(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tan(t.DecimalVal), Object)}), .result = DirectCast(Math.Tan(blEntity.DecimalVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tanh(t.LongValue), Object)}), .result = DirectCast(Math.Tanh(blEntity.LongValue), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tanh(t.Int), Object)}), .result = DirectCast(Math.Tanh(blEntity.Int), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tanh(t.Float), Object)}), .result = DirectCast(Math.Tanh(blEntity.Float), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tanh(t.DoubleVal), Object)}), .result = DirectCast(Math.Tanh(blEntity.DoubleVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(Math.Tanh(t.DecimalVal), Object)}), .result = DirectCast(Math.Tanh(blEntity.DecimalVal), Object)})

                For Each t In unaryTestValues
                    Assert.AreEqual(t.result, t.query.FirstOrDefault().result, t.query.Expression.ToString())
                Next
            Next

        End Sub


        <TestCategory("Partition1")> <TestMethod(), Variation("Test VB compiler generated expressions with projection")>
        Public Sub ClientProjection_ProjectBinaryLocalVarResultComplexType()

            Dim baseQuery = ctx.CreateQuery(Of TestEntity)("Values")
            Dim blEntity = New ProjectionTestContext().Values.FirstOrDefault()

            ' Binary Operations
            For Each v In _MakeArray(1, -1, 0.1, -0.01D, 1.0, 1.0 + Double.MinValue)
                Dim _it = v
                Dim binaryVariableTestValues() = _MakeArray(
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.LongValue + _it}), .result = blEntity.LongValue + v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.Int + _it}), .result = blEntity.Int + v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.Float + _it}), .result = blEntity.Float + v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.DoubleVal + _it}), .result = blEntity.DoubleVal + v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.DecimalVal + _it}), .result = blEntity.DecimalVal + v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.LongValue - _it}), .result = blEntity.LongValue - v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.Int - _it}), .result = blEntity.Int - v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.Float - _it}), .result = blEntity.Float - v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.DoubleVal - _it}), .result = blEntity.DoubleVal - v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.DecimalVal - _it}), .result = blEntity.DecimalVal - v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.LongValue * _it}), .result = blEntity.LongValue * v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.Int * _it}), .result = blEntity.Int * v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.Float * _it}), .result = blEntity.Float * v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.DoubleVal * _it}), .result = blEntity.DoubleVal * v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.DecimalVal * _it}), .result = blEntity.DecimalVal * v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.LongValue / _it}), .result = blEntity.LongValue / v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.Int / _it}), .result = blEntity.Int / v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.Float / _it}), .result = blEntity.Float / v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.DoubleVal / _it}), .result = blEntity.DoubleVal / v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = t.DecimalVal / _it}), .result = blEntity.DecimalVal / v},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Max(t.LongValue, _it)}), .result = Math.Max(blEntity.LongValue, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Max(t.Int, _it)}), .result = Math.Max(blEntity.Int, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Max(t.Float, _it)}), .result = Math.Max(blEntity.Float, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Max(t.DoubleVal, _it)}), .result = Math.Max(blEntity.DoubleVal, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Max(t.DecimalVal, _it)}), .result = Math.Max(blEntity.DecimalVal, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Min(t.LongValue, _it)}), .result = Math.Min(blEntity.LongValue, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Min(t.Int, _it)}), .result = Math.Min(blEntity.Int, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Min(t.Float, _it)}), .result = Math.Min(blEntity.Float, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Min(t.DoubleVal, _it)}), .result = Math.Min(blEntity.DoubleVal, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Min(t.DecimalVal, _it)}), .result = Math.Min(blEntity.DecimalVal, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Pow(t.LongValue, _it)}), .result = Math.Pow(blEntity.LongValue, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Pow(t.Int, _it)}), .result = Math.Pow(blEntity.Int, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Pow(t.Float, _it)}), .result = Math.Pow(blEntity.Float, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Pow(t.DoubleVal, _it)}), .result = Math.Pow(blEntity.DoubleVal, v)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = Math.Pow(t.DecimalVal, _it)}), .result = Math.Pow(blEntity.DecimalVal, v)})

                For Each t In binaryVariableTestValues
                    Assert.AreEqual(t.result, t.query.FirstOrDefault().result, t.query.Expression.ToString())
                Next
            Next

        End Sub

        <TestCategory("Partition1")> <TestMethod(), Variation("Test VB compiler generated expressions with projection")>
        Public Sub ClientProjection_ProjectStringOpResultComplexType()

            Dim baseQuery = ctx.CreateQuery(Of TestEntity)("Values")
            Dim blEntity = New ProjectionTestContext().Values.FirstOrDefault()

            Dim stringTestValues() = _MakeArray(
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.Contains("Foo"), Object)}), .result = DirectCast(blEntity.StringVal.Contains("Foo"), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.IndexOf("Foo"), Object)}), .result = DirectCast(blEntity.StringVal.IndexOf("Foo"), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.EndsWith("Foo"), Object)}), .result = DirectCast(blEntity.StringVal.EndsWith("Foo"), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.LastIndexOf("Foo"), Object)}), .result = DirectCast(blEntity.StringVal.LastIndexOf("Foo"), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.StartsWith("Foo"), Object)}), .result = DirectCast(blEntity.StringVal.StartsWith("Foo"), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.Insert(0, "Foo"), Object)}), .result = DirectCast(blEntity.StringVal.Insert(0, "Foo"), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.Remove(0, 1), Object)}), .result = DirectCast(blEntity.StringVal.Remove(0, 1), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.Replace("1", "0"), Object)}), .result = DirectCast(blEntity.StringVal.Replace("1", "0"), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.ToLower(), Object)}), .result = DirectCast(blEntity.StringVal.ToLower(), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.StringVal.ToUpper(), Object)}), .result = DirectCast(blEntity.StringVal.ToLower(), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(String.Format("Foo{0}", t.StringVal), Object)}), .result = DirectCast(String.Format("Foo{0}", blEntity.StringVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(String.Compare("Foo", t.StringVal), Object)}), .result = DirectCast(String.Compare("Foo", blEntity.StringVal), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(String.Concat("Foo", t.StringVal), Object)}), .result = DirectCast(String.Concat("Foo", blEntity.StringVal), Object)})

            For Each t In stringTestValues
                Assert.AreEqual(t.result, t.query.FirstOrDefault().result, t.query.Expression.ToString())
            Next
        End Sub

        <TestCategory("Partition1")> <TestMethod(), Variation("Test VB compiler generated expressions with projection")>
        Public Sub ClientProjection_DateTimeOffset()

            Dim baseQuery = ctx.CreateQuery(Of TestEntity)("Values")
            Dim blEntity = New ProjectionTestContext().Values.FirstOrDefault()
            Dim testDateTime As DateTimeOffset = New DateTimeOffset(2009, 9, 9, 0, 0, 0, TimeSpan.FromHours(-8))

            Dim datetimeTestValues() = _MakeArray(
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.Ticks, Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.Ticks, Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(DateTimeOffset.Compare(t.DateTimeOffsetVal.Date, testDateTime.Date), Object)}), .result = DirectCast(DateTime.Compare(blEntity.DateTimeOffsetVal.Date, testDateTime.Date), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.Add(TimeSpan.FromDays(1)), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.Add(TimeSpan.FromDays(1)), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.Subtract(TimeSpan.FromDays(1)), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.Subtract(TimeSpan.FromDays(1)), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.AddDays(1), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.AddDays(1), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.AddHours(1), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.AddHours(1), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.AddMilliseconds(1), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.AddMilliseconds(1), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.AddMinutes(1), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.AddMinutes(1), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.AddMonths(1), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.AddMonths(1), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.AddSeconds(1), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.AddSeconds(1), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.AddTicks(1), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.AddTicks(1), Object)},
                    New With {.query = baseQuery.Select(Function(t) New With {.result = DirectCast(t.DateTimeOffsetVal.AddYears(1), Object)}), .result = DirectCast(blEntity.DateTimeOffsetVal.AddYears(1), Object)})

            For Each t In datetimeTestValues
                Assert.AreEqual(t.result, t.query.FirstOrDefault().result, t.query.Expression.ToString())
            Next
        End Sub
#End Region

    End Class

#Region "Cross Feature Tests - Type Resolving/Projections/SDP Client"
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    'Remove Atom
    ' <TestClass()>
     Public Class ClientProjectionTypeResolvingTests
        Inherits AstoriaTestCase

        Private Shared web As TestWebRequest = Nothing
        Private ctx As DataServiceContext = Nothing
        Private Shared ResolveTypeCallOrder As List(Of String)
        Private Shared ResolveNameCallOrder As List(Of String)

        ' Server type structure
        ' WorkItem
        ' - DevWorkItem
        '   - complex prop: DeveloperResource
        '   - FeatureWorkItem
        ' - BugWorkItem
#Region "Service"

        Public Class WorkItem
            Private m_ID As Integer
            Private m_DueDate As DateTimeOffset
            Private m_Priority As Integer
            Private m_Comments As String
            Private m_Parent As WorkItem
            Private m_Child As List(Of WorkItem)

            Public Property ID() As Integer
                Get
                    Return m_ID
                End Get
                Set(ByVal value As Integer)
                    m_ID = value
                End Set
            End Property

            Public Property DueDate() As DateTimeOffset
                Get
                    Return m_DueDate
                End Get
                Set(ByVal value As DateTimeOffset)
                    m_DueDate = value
                End Set
            End Property

            Public Property Priority() As Integer
                Get
                    Return m_Priority
                End Get
                Set(ByVal value As Integer)
                    m_Priority = value
                End Set
            End Property

            Public Property Comments() As String
                Get
                    Return m_Comments
                End Get
                Set(ByVal value As String)
                    m_Comments = value
                End Set
            End Property

            Public Property Parent() As WorkItem
                Get
                    Return m_Parent
                End Get
                Set(ByVal value As WorkItem)
                    m_Parent = value
                End Set
            End Property

            Public Property Children() As List(Of WorkItem)
                Get
                    Return m_Child
                End Get
                Set(ByVal value As List(Of WorkItem))
                    m_Child = value
                End Set
            End Property
        End Class

        Public Class DeveloperResource
            Private m_Name As String
            Private m_Position As String
            Private m_Email As String

            Public Property Name() As String
                Get
                    Return m_Name
                End Get
                Set(ByVal value As String)
                    m_Name = value
                End Set
            End Property

            Public Property Position() As String
                Get
                    Return m_Position
                End Get
                Set(ByVal value As String)
                    m_Position = value
                End Set
            End Property

            Public Property Email() As String
                Get
                    Return m_Email
                End Get
                Set(ByVal value As String)
                    m_Email = value
                End Set
            End Property
        End Class

        Public Class DevWorkItem
            Inherits WorkItem

            Private m_DevNote As String
            Private m_Developer As DeveloperResource

            Public Property DevNote() As String
                Get
                    Return m_DevNote
                End Get
                Set(ByVal value As String)
                    m_DevNote = value
                End Set
            End Property

            Public Property Developer() As DeveloperResource
                Get
                    Return m_Developer
                End Get
                Set(ByVal value As DeveloperResource)
                    m_Developer = value
                End Set
            End Property
        End Class

        Public Class FeatureWorkItem
            Inherits DevWorkItem

            Private m_FeatureName As String
            Private m_DesignComplete As Boolean
            Private m_FeatureSpec As String

            Public Property FeatureName() As String
                Get
                    Return m_FeatureName
                End Get
                Set(ByVal value As String)
                    m_FeatureName = value
                End Set
            End Property

            Public Property DesignComplete() As Boolean
                Get
                    Return m_DesignComplete
                End Get
                Set(ByVal value As Boolean)
                    m_DesignComplete = value
                End Set
            End Property

            Public Property FeatureSpec() As String
                Get
                    Return m_FeatureSpec
                End Get
                Set(ByVal value As String)
                    m_FeatureSpec = value
                End Set
            End Property
        End Class

        Public Class BugWorkItem
            Inherits WorkItem

            Private m_ReproStep As String
            Private m_Severity As Integer
            Private m_BugType As String

            Public Property ReproStep() As String
                Get
                    Return m_ReproStep
                End Get
                Set(ByVal value As String)
                    m_ReproStep = value
                End Set
            End Property

            Public Property Severity() As Integer
                Get
                    Return m_Severity
                End Get
                Set(ByVal value As Integer)
                    m_Severity = value
                End Set
            End Property

            Public Property BugType() As String
                Get
                    Return m_BugType
                End Get
                Set(ByVal value As String)
                    m_BugType = value
                End Set
            End Property
        End Class

        Public Class WorkItemContext
            Implements IUpdatable

            Private m_workItems As List(Of WorkItem)
            Private m_pendingChanges As List(Of WorkItem)

            Private Sub SetLink(ByVal parent As Integer, ByVal child As Integer)
                Dim p = m_workItems.FirstOrDefault(Function(w) w.ID = parent)
                Dim c = m_workItems.FirstOrDefault(Function(w) w.ID = child)

                If (p.Children Is Nothing) Then
                    p.Children = New List(Of WorkItem)
                End If
                p.Children.Add(c)
                c.Parent = p
            End Sub

            Public Sub New()
                m_workItems = New List(Of WorkItem)()
                m_pendingChanges = New List(Of WorkItem)()

                Dim dev = New DeveloperResource() {New DeveloperResource() With {.Name = "Peter Qian", .Email = "pqian@microsoft.com", .Position = "SDE"}, New DeveloperResource() With {.Name = "Andy Conrad", .Email = "aconrad@microsoft.com", .Position = "Senior Dev Lead"}, New DeveloperResource() With {.Name = "Pratik Patel", .Email = "pratikp@microsoft.com", .Position = "Senior SDE"}}

                For i As Integer = 0 To 10
                    m_workItems.Add(New BugWorkItem() With {.ID = 700000 + i, .Priority = i Mod 2, .BugType = If(i Mod 2 = 0, "Product", "Feature"), .Comments = "Bug Item " & i.ToString(), .ReproStep = "Repro Step", .DueDate = New DateTimeOffset(2009, 9, 9, 0, 0, 0, New TimeSpan(0)), .Severity = 1, .Children = New List(Of WorkItem)})
                    m_workItems.Add(New FeatureWorkItem() With {.ID = i + 1, .Priority = i Mod 2, .Comments = "Feature " & i.ToString(), .DueDate = New DateTimeOffset(2009, 4, 1, 0, 0, 0, New TimeSpan(0)), .DesignComplete = True, .FeatureName = "Row Count", .FeatureSpec = "RowCountSpec.doc", .Developer = dev(i Mod 3), .Children = New List(Of WorkItem)})
                Next

                SetLink(1, 700001)
                SetLink(1, 700002)
                SetLink(1, 700003)
                SetLink(1, 700004)
                SetLink(1, 2)
                SetLink(2, 700005)
                SetLink(2, 3)
                SetLink(2, 4)
                SetLink(3, 700006)
                SetLink(3, 700007)
                SetLink(4, 5)

            End Sub

            Public ReadOnly Property WorkItems() As IQueryable(Of WorkItem)
                Get
                    Return m_workItems.AsQueryable()
                End Get
            End Property

            Public Sub AddReferenceToCollection(ByVal targetResource As Object, ByVal propertyName As String, ByVal resourceToBeAdded As Object) Implements Microsoft.OData.Service.IUpdatable.AddReferenceToCollection
                Throw New NotImplementedException
            End Sub

            Public Sub ClearChanges() Implements Microsoft.OData.Service.IUpdatable.ClearChanges
                Throw New NotImplementedException
            End Sub

            Public Function CreateResource(ByVal containerName As String, ByVal fullTypeName As String) As Object Implements Microsoft.OData.Service.IUpdatable.CreateResource
                Dim resource As WorkItem = Nothing
                If (fullTypeName.Contains("BugWorkItem")) Then
                    resource = New BugWorkItem() With {.ID = -1}
                    m_pendingChanges.Add(resource)
                    Return resource.ID
                ElseIf (fullTypeName.Contains("FeatureWorkItem")) Then
                    resource = New FeatureWorkItem() With {.ID = -1}
                    m_pendingChanges.Add(resource)
                    Return resource.ID
                End If

                Return Nothing
            End Function

            Public Sub DeleteResource(ByVal targetResource As Object) Implements Microsoft.OData.Service.IUpdatable.DeleteResource
                Dim resource = From w In m_workItems Where w.ID = CType(targetResource, Integer) Select w
                If (resource.Count() = 0) Then
                    Throw New InvalidOperationException("Cannot locate resource")
                End If
            End Sub

            Public Function GetResource(ByVal query As System.Linq.IQueryable, ByVal fullTypeName As String) As Object Implements Microsoft.OData.Service.IUpdatable.GetResource
                Dim e = query.GetEnumerator()
                If (Not e.MoveNext()) Then
                    Throw New InvalidOperationException("Cannot locate resource")
                End If

                Dim workItem = DirectCast(e.Current, WorkItem)
                Return workItem.ID
            End Function

            Public Function GetValue(ByVal targetResource As Object, ByVal propertyName As String) As Object Implements Microsoft.OData.Service.IUpdatable.GetValue
                Throw New NotImplementedException
            End Function

            Public Sub RemoveReferenceFromCollection(ByVal targetResource As Object, ByVal propertyName As String, ByVal resourceToBeRemoved As Object) Implements Microsoft.OData.Service.IUpdatable.RemoveReferenceFromCollection
                Throw New NotImplementedException
            End Sub

            Public Function ResetResource(ByVal resource As Object) As Object Implements Microsoft.OData.Service.IUpdatable.ResetResource
                Throw New NotImplementedException
            End Function

            Public Function ResolveResource(ByVal resource As Object) As Object Implements Microsoft.OData.Service.IUpdatable.ResolveResource
                Dim id = DirectCast(resource, Integer)
                If (id = -1) Then Return New BugWorkItem()


                Return Me.m_workItems.FirstOrDefault(Function(w) w.ID = id)

            End Function

            Public Sub SaveChanges() Implements Microsoft.OData.Service.IUpdatable.SaveChanges
            End Sub

            Public Sub SetReference(ByVal targetResource As Object, ByVal propertyName As String, ByVal propertyValue As Object) Implements Microsoft.OData.Service.IUpdatable.SetReference
                Throw New NotImplementedException
            End Sub

            Public Sub SetValue(ByVal targetResource As Object, ByVal propertyName As String, ByVal propertyValue As Object) Implements Microsoft.OData.Service.IUpdatable.SetValue
                System.Diagnostics.Trace.WriteLine("Setting Value for " & propertyName & " to " & IIf(propertyValue Is Nothing, "<NULL>", propertyValue).ToString())
            End Sub
        End Class

        <ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults:=True)>
        Public Class WorkItemService
            Inherits DataService(Of WorkItemContext)

            Public Shared Sub InitializeService(ByVal config As DataServiceConfiguration)
                config.UseVerboseErrors = True
                config.SetEntitySetAccessRule("*", EntitySetRights.All)
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All)
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4
                config.SetEntitySetPageSize("WorkItems", 2)
            End Sub

            <WebGet()>
            Public Function GetDevWorkItems() As IQueryable(Of DevWorkItem)
                Return (From w In Me.CurrentDataSource.WorkItems Select w).OfType(Of DevWorkItem)()
            End Function
        End Class

#End Region

#Region "Additional test attributes"
        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.ServiceType = GetType(WorkItemService)
            web.StartService()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.StopService()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Me.ctx = New DataServiceContext(web.ServiceRoot)
            'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
            Me.ctx.ResolveType = AddressOf CustomTypeResolver
            Me.ctx.ResolveName = AddressOf CustomTypeNameResolver
            ResolveTypeCallOrder = New List(Of String)
            ResolveNameCallOrder = New List(Of String)
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
        End Sub
#End Region

        <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectionInheritanceSingleLevel()
            Dim q = From w In ctx.CreateQuery(Of ProjectionTypeResolvingTests.NarrowWorkItem)("WorkItems")
                    Select New ProjectionTypeResolvingTests.NarrowWorkItem() With {
                        .Comments = w.Comments,
                        .DueDate = w.DueDate,
                        .Priority = w.Priority}

            Dim qor = CType(CType(q, DataServiceQuery(Of ProjectionTypeResolvingTests.NarrowWorkItem)).Execute(), QueryOperationResponse(Of ProjectionTypeResolvingTests.NarrowWorkItem))
            Dim cToken As DataServiceQueryContinuation(Of ProjectionTypeResolvingTests.NarrowWorkItem) = Nothing
            Dim results = New List(Of ProjectionTypeResolvingTests.NarrowWorkItem)
            Dim featureCount = 0, bugCount = 0

            Do

                For Each w As ProjectionTypeResolvingTests.NarrowWorkItem In qor
                    results.Add(w)
                    Dim descriptor = ctx.GetEntityDescriptor(w)

                    If (descriptor.ServerTypeName = "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_FeatureWorkItem") Then
                        featureCount = featureCount + 1
                    ElseIf (descriptor.ServerTypeName = "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_BugWorkItem") Then
                        bugCount = bugCount + 1
                    End If
                Next

                cToken = qor.GetContinuation()
                If cToken IsNot Nothing Then
                    qor = ctx.Execute(cToken)
                End If

            Loop Until cToken Is Nothing

            Assert.AreEqual(11, bugCount)
            Assert.AreEqual(11, featureCount)

            ' [Client]Type Resolving + Client Projection: Resolver called more than once per entry when project
            Assert.AreEqual(22, ResolveTypeCallOrder.Count)

            ' update & insert
            ctx.UpdateObject(results.FirstOrDefault())
            ctx.UpdateObject(results.LastOrDefault())
            ctx.AddObject("WorkItems", New ProjectionTypeResolvingTests.ClientBugWorkItem() With {.ID = 678979, .Priority = 2, .DueDate = DateTime.Now, .BugType = "Feature"})
            ctx.SaveChanges()
            Assert.AreEqual(3, ResolveNameCallOrder.Count)

            ' deletes
            ctx.DeleteObject(results.FirstOrDefault())
            ctx.SaveChanges()
            ' no change to resolve name
            Assert.AreEqual(3, ResolveNameCallOrder.Count)

        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectionInheritanceDSCSingleLevel()
            Dim q = From w In ctx.CreateQuery(Of ProjectionTypeResolvingTests.ClientBindingWorkItem)("WorkItems")
                    Select New ProjectionTypeResolvingTests.NarrowBindingWorkItem() With {
                        .Comments = w.Comments,
                        .DueDate = w.DueDate,
                        .Priority = w.Priority}

            Dim dsc As DataServiceCollection(Of ProjectionTypeResolvingTests.NarrowBindingWorkItem) = Nothing

            For i As Integer = 0 To 1
                If (i = 0) Then
                    dsc = New DataServiceCollection(Of ProjectionTypeResolvingTests.NarrowBindingWorkItem)(q)
                Else
                    Dim qor = CType(q, DataServiceQuery(Of ProjectionTypeResolvingTests.NarrowBindingWorkItem)).Execute()
                    dsc = New DataServiceCollection(Of ProjectionTypeResolvingTests.NarrowBindingWorkItem)(qor)
                End If

                While (dsc.Continuation IsNot Nothing)
                    dsc.Load(ctx.Execute(dsc.Continuation))
                End While
                Dim featureCount = 0, bugCount = 0

                For Each w As ProjectionTypeResolvingTests.NarrowBindingWorkItem In dsc
                    Dim descriptor = ctx.GetEntityDescriptor(w)
                    If (descriptor.ServerTypeName = "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_FeatureWorkItem") Then
                        featureCount = featureCount + 1
                    ElseIf (descriptor.ServerTypeName = "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_BugWorkItem") Then
                        bugCount = bugCount + 1
                    End If
                Next

                Assert.AreEqual(11, bugCount)
                Assert.AreEqual(11, featureCount)

            Next

            Assert.AreEqual(44, ResolveTypeCallOrder.Count)
        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectionInheritanceMultiLevel()
            Dim q = From w In ctx.CreateQuery(Of ProjectionTypeResolvingTests.NarrowWorkItemWithRelations)("WorkItems")
                    Select New ProjectionTypeResolvingTests.NarrowWorkItemWithRelations() With
                    {.Comments = w.Comments, .DueDate = w.DueDate, .Priority = w.Priority,
                     .Parent = If(w.Parent Is Nothing, Nothing, New ProjectionTypeResolvingTests.NarrowWorkItemWithRelations() With {
                                  .Comments = w.Parent.Comments,
                                  .DueDate = w.Parent.DueDate,
                                  .Priority = w.Parent.Priority}),
                     .Children = (From c In w.Children
                                  Select New ProjectionTypeResolvingTests.NarrowWorkItemWithRelations() With {
                                  .Priority = c.Priority,
                                  .Parent = If(c.Parent Is Nothing, Nothing, New ProjectionTypeResolvingTests.NarrowWorkItemWithRelations() With {
                                               .Comments = c.Parent.Comments,
                                               .DueDate = c.Parent.DueDate,
                                               .Priority = c.Parent.Priority})
                                               }).ToList()}
            Dim qor = CType(CType(q, DataServiceQuery(Of ProjectionTypeResolvingTests.NarrowWorkItemWithRelations)).Execute(), QueryOperationResponse(Of ProjectionTypeResolvingTests.NarrowWorkItemWithRelations))
            Dim childrenCount = 0, parentCount = 0

            ' [Client]Type Resolving + Client Projection: Conditional null check expression not been materialized correctly
            Do
                For Each w In qor
                    If (w.Children IsNot Nothing) Then

                        Dim nextChildrenToken = qor.GetContinuation(w.Children)

                        While (nextChildrenToken IsNot Nothing)
                            If (parentCount Mod 2 = 0) Then
                                Dim innerChildren = ctx.Execute(nextChildrenToken)
                                For Each c In innerChildren
                                    w.Children.Add(c)
                                Next
                                nextChildrenToken = innerChildren.GetContinuation()
                            Else
                                nextChildrenToken = ctx.LoadProperty(w, "Children", nextChildrenToken).GetContinuation()
                            End If
                        End While

                        For Each c In w.Children
                            Assert.AreEqual(w, c.Parent)
                            childrenCount = childrenCount + 1
                        Next
                    End If
                    parentCount = parentCount + 1
                Next

                Dim nextParentToken = qor.GetContinuation()
                If (nextParentToken IsNot Nothing) Then
                    qor = ctx.Execute(nextParentToken)
                Else
                    Exit Do
                End If

            Loop While True

            Assert.AreEqual(22, parentCount)
            Assert.AreEqual(8, childrenCount)
            Assert.AreEqual(55, ResolveTypeCallOrder.Count)

        End Sub

        <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectionInheritanceDSCMultiLevel()
            Dim q = From w In ctx.CreateQuery(Of ProjectionTypeResolvingTests.ClientBindingWorkItem)("WorkItems")
                    Select New ProjectionTypeResolvingTests.NarrowBindingWorkItemWithRelations() With
                    {.Comments = w.Comments, .DueDate = w.DueDate, .Priority = w.Priority,
                     .Parent = If(w.Parent Is Nothing, Nothing, New ProjectionTypeResolvingTests.NarrowBindingWorkItemWithRelations() With {
                                  .Comments = w.Parent.Comments,
                                  .DueDate = w.Parent.DueDate,
                                  .Priority = w.Parent.Priority}),
                     .Children = New DataServiceCollection(Of ProjectionTypeResolvingTests.NarrowBindingWorkItemWithRelations)(
                                    From c In w.Children
                                    Select New ProjectionTypeResolvingTests.NarrowBindingWorkItemWithRelations() With {
                                    .Priority = c.Priority,
                                    .Parent = If(c.Parent Is Nothing, Nothing, New ProjectionTypeResolvingTests.NarrowBindingWorkItemWithRelations() With {
                                                 .Comments = c.Parent.Comments,
                                                 .DueDate = c.Parent.DueDate,
                                                 .Priority = c.Parent.Priority})})
                    }

            Dim dsc = New DataServiceCollection(Of ProjectionTypeResolvingTests.NarrowBindingWorkItemWithRelations)(q)
            Dim childrenCount = 0, parentCount = 0

            While (dsc.Continuation IsNot Nothing)
                dsc.Load(ctx.Execute(dsc.Continuation))
            End While

            For Each w In dsc
                If (w.Children IsNot Nothing) Then
                    While (w.Children.Continuation IsNot Nothing)
                        w.Children.Load(ctx.Execute(w.Children.Continuation))
                    End While

                    childrenCount = childrenCount + w.Children.Count
                End If
                parentCount = parentCount + 1
            Next

            Assert.AreEqual(22, parentCount)
            Assert.AreEqual(8, childrenCount)

        End Sub


        <TestCategory("Partition3")> <TestMethod()> Public Sub ProjectionMixedTypeInheritance()
            ctx.MergeOption = MergeOption.OverwriteChanges

            Dim q = From w In ctx.CreateQuery(Of ProjectionTypeResolvingTests.ClientWorkItem)("WorkItems")
                    Where w.ID < 10
                    Select New ProjectionTypeResolvingTests.NarrowWorkItemWithComplexProperty() With {
                        .Comments = w.Comments,
                        .Priority = w.Priority,
                        .DueDate = w.DueDate}

            For Each w In q
                Assert.IsNull(w.Developer)
            Next

            Dim q2 = From w In ctx.CreateQuery(Of ProjectionTypeResolvingTests.ClientDevWorkItem)("GetDevWorkItems")
                     Where w.ID < 10
                     Select New ProjectionTypeResolvingTests.NarrowWorkItemWithComplexProperty() With {
                         .Comments = w.Comments,
                         .Developer = w.Developer}
            For Each w In q2
                Assert.IsNotNull(w.Developer)
                Assert.IsNotNull(w.Developer.Name)
                Assert.IsNotNull(w.Developer.Position)
                Assert.IsNotNull(w.Developer.Email)
            Next
        End Sub

        Private Shared Function CustomTypeResolver(ByVal typeName As String) As Type
            ResolveTypeCallOrder.Add(typeName)
            System.Diagnostics.Trace.WriteLine("Resolving Server Type " & typeName)

            typeName = typeName.Remove(0, "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_".Length)
            Select Case typeName
                Case "FeatureWorkItem"
                    Return GetType(ProjectionTypeResolvingTests.ClientFeatureWorkItem)
                Case "BugWorkItem"
                    Return GetType(ProjectionTypeResolvingTests.ClientBugWorkItem)
                Case "DevWorkItem"
                    Return GetType(ProjectionTypeResolvingTests.ClientDevWorkItem)
                Case "DeveloperResource"
                    Return GetType(DeveloperResource)
            End Select
            Return Nothing
        End Function

        Private Shared Function CustomTypeNameResolver(ByVal type As Type) As String
            ResolveNameCallOrder.Add(type.Name)

            Dim clientName = type.Name
            Select Case clientName
                Case "ClientFeatureWorkItem"
                    Return "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_FeatureWorkItem"
                Case "ClientBugWorkItem"
                    Return "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_BugWorkItem"
                Case "ClientDevWorkItem"
                    Return "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_DevWorkItem"
                Case "DeveloperResource"
                    Return "AstoriaClientUnitTests.ClientModule_ClientProjectionTypeResolvingTests_DeveloperResource"
            End Select

            ' unknown
            Return Nothing
        End Function

    End Class
#End Region

End Class


#Region "Client Type Definitions"

Namespace ProjectionTypeResolvingTests

    <EntityType()>
    Public Class NarrowWorkItem
        Private m_DueDate As DateTimeOffset
        Private m_Priority As Integer
        Private m_Comments As String

        Public Property DueDate() As DateTimeOffset
            Get
                Return m_DueDate
            End Get
            Set(ByVal value As DateTimeOffset)
                m_DueDate = value
            End Set
        End Property

        Public Property Priority() As Integer
            Get
                Return m_Priority
            End Get
            Set(ByVal value As Integer)
                m_Priority = value
            End Set
        End Property

        Public Property Comments() As String
            Get
                Return m_Comments
            End Get
            Set(ByVal value As String)
                m_Comments = value
            End Set
        End Property
    End Class

    <EntityType()>
    Public Class NarrowWorkItemWithComplexProperty
        Inherits NarrowWorkItem

        Private m_Developer As ClientModule.ClientProjectionTypeResolvingTests.DeveloperResource
        Public Property Developer() As ClientModule.ClientProjectionTypeResolvingTests.DeveloperResource
            Get
                Return m_Developer
            End Get
            Set(ByVal value As ClientModule.ClientProjectionTypeResolvingTests.DeveloperResource)
                m_Developer = value
            End Set
        End Property
    End Class

    <EntityType()>
    Public Class NarrowWorkItemWithRelations
        Inherits NarrowWorkItem

        Private m_Parent As NarrowWorkItemWithRelations
        Private m_Children As List(Of NarrowWorkItemWithRelations)

        Public Property Parent() As NarrowWorkItemWithRelations
            Get
                Return m_Parent
            End Get
            Set(ByVal value As NarrowWorkItemWithRelations)
                m_Parent = value
            End Set
        End Property

        Public Property Children() As List(Of NarrowWorkItemWithRelations)
            Get
                Return m_Children
            End Get
            Set(ByVal value As List(Of NarrowWorkItemWithRelations))
                m_Children = value
            End Set
        End Property
    End Class

    Public Class ClientWorkItem
        Private m_ID As Integer
        Private m_DueDate As DateTimeOffset
        Private m_Priority As Integer
        Private m_Comments As String
        Private m_Parent As ClientWorkItem
        Private m_Children As List(Of ClientWorkItem)

        Public Property ID() As Integer
            Get
                Return m_ID
            End Get
            Set(ByVal value As Integer)
                m_ID = value
            End Set
        End Property

        Public Property DueDate() As DateTimeOffset
            Get
                Return m_DueDate
            End Get
            Set(ByVal value As DateTimeOffset)
                m_DueDate = value
            End Set
        End Property

        Public Property Priority() As Integer
            Get
                Return m_Priority
            End Get
            Set(ByVal value As Integer)
                m_Priority = value
            End Set
        End Property

        Public Property Comments() As String
            Get
                Return m_Comments
            End Get
            Set(ByVal value As String)
                m_Comments = value
            End Set
        End Property

        Public Property Parent() As ClientWorkItem
            Get
                Return m_Parent
            End Get
            Set(ByVal value As ClientWorkItem)
                m_Parent = value
            End Set
        End Property

        Public Property Children() As List(Of ClientWorkItem)
            Get
                Return m_Children
            End Get
            Set(ByVal value As List(Of ClientWorkItem))
                m_Children = value
            End Set
        End Property
    End Class

    Public Class ClientDevWorkItem
        Inherits ClientWorkItem

        Private m_DevNote As String
        Private m_Developer As ClientModule.ClientProjectionTypeResolvingTests.DeveloperResource
        Public Property Developer() As ClientModule.ClientProjectionTypeResolvingTests.DeveloperResource
            Get
                Return m_Developer
            End Get
            Set(ByVal value As ClientModule.ClientProjectionTypeResolvingTests.DeveloperResource)
                m_Developer = value
            End Set
        End Property

        Public Property DevNote() As String
            Get
                Return m_DevNote
            End Get
            Set(ByVal value As String)
                m_DevNote = value
            End Set
        End Property

    End Class

    Public Class ClientFeatureWorkItem
        Inherits ClientDevWorkItem

        Private m_FeatureName As String
        Private m_DesignComplete As Boolean
        Private m_FeatureSpec As String

        Public Property FeatureName() As String
            Get
                Return m_FeatureName
            End Get
            Set(ByVal value As String)
                m_FeatureName = value
            End Set
        End Property

        Public Property DesignComplete() As Boolean
            Get
                Return m_DesignComplete
            End Get
            Set(ByVal value As Boolean)
                m_DesignComplete = value
            End Set
        End Property

        Public Property FeatureSpec() As String
            Get
                Return m_FeatureSpec
            End Get
            Set(ByVal value As String)
                m_FeatureSpec = value
            End Set
        End Property
    End Class

    Public Class ClientBugWorkItem
        Inherits ClientWorkItem

        Private m_ReproStep As String
        Private m_Severity As Integer
        Private m_BugType As String

        Public Property ReproStep() As String
            Get
                Return m_ReproStep
            End Get
            Set(ByVal value As String)
                m_ReproStep = value
            End Set
        End Property

        Public Property Severity() As Integer
            Get
                Return m_Severity
            End Get
            Set(ByVal value As Integer)
                m_Severity = value
            End Set
        End Property

        Public Property BugType() As String
            Get
                Return m_BugType
            End Get
            Set(ByVal value As String)
                m_BugType = value
            End Set
        End Property

        Public Sub New()

        End Sub
    End Class

    ' ============================
    '  Binding Client Code
    ' ============================
    Public Class ClientBindingDeveloperResource
        Implements Global.System.ComponentModel.INotifyPropertyChanged

        Public Property Name() As String
            Get
                Return Me._Name
            End Get
            Set(ByVal value As String)
                Me._Name = value
                Me.OnPropertyChanged("Name")
            End Set
        End Property
        Private _Name As String

        Public Property Position() As String
            Get
                Return Me._Position
            End Get
            Set(ByVal value As String)
                Me._Position = value
                Me.OnPropertyChanged("Position")
            End Set
        End Property
        Private _Position As String

        Public Property Email() As String
            Get
                Return Me._Email
            End Get
            Set(ByVal value As String)
                Me._Email = value
                Me.OnPropertyChanged("Email")
            End Set
        End Property
        Private _Email As String

        Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
            If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
            End If
        End Sub
    End Class

    <EntityType()>
    Public Class NarrowBindingWorkItem
        Implements ComponentModel.INotifyPropertyChanged

        Private m_DueDate As DateTimeOffset
        Private m_Priority As Integer
        Private m_Comments As String

        Public Property DueDate() As DateTimeOffset
            Get
                Return m_DueDate
            End Get
            Set(ByVal value As DateTimeOffset)
                m_DueDate = value
                OnPropertyChanged("DueDate")
            End Set
        End Property

        Public Property Priority() As Integer
            Get
                Return m_Priority
            End Get
            Set(ByVal value As Integer)
                m_Priority = value
                OnPropertyChanged("Priority")
            End Set
        End Property

        Public Property Comments() As String
            Get
                Return m_Comments
            End Get
            Set(ByVal value As String)
                m_Comments = value
                OnPropertyChanged("Comments")
            End Set
        End Property

        Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
            If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
            End If
        End Sub

    End Class

    <EntityType()>
    Public Class NarrowBindingWorkItemWithComplexProperty
        Inherits NarrowBindingWorkItem

        Private m_Developer As ClientBindingDeveloperResource
        Public Property Developer() As ClientBindingDeveloperResource
            Get
                Return m_Developer
            End Get
            Set(ByVal value As ClientBindingDeveloperResource)
                m_Developer = value
                OnPropertyChanged("Developer")
            End Set
        End Property
    End Class

    <EntityType()>
    <EntitySet("WorkItems")>
    Public Class NarrowBindingWorkItemWithRelations
        Inherits NarrowBindingWorkItem

        Private m_Parent As NarrowBindingWorkItemWithRelations
        Private m_Children As DataServiceCollection(Of NarrowBindingWorkItemWithRelations) = New DataServiceCollection(Of NarrowBindingWorkItemWithRelations)(Nothing, TrackingMode.None)

        Public Property Parent() As NarrowBindingWorkItemWithRelations
            Get
                Return m_Parent
            End Get
            Set(ByVal value As NarrowBindingWorkItemWithRelations)
                m_Parent = value
                OnPropertyChanged("Parent")
            End Set
        End Property

        Public Property Children() As DataServiceCollection(Of NarrowBindingWorkItemWithRelations)
            Get
                Return m_Children
            End Get
            Set(ByVal value As DataServiceCollection(Of NarrowBindingWorkItemWithRelations))
                m_Children = value
            End Set
        End Property
    End Class

    <Global.Microsoft.OData.Client.EntitySetAttribute("WorkItems"),
     Global.Microsoft.OData.Client.KeyAttribute("ID")>
    Public Class ClientBindingWorkItem
        Implements Global.System.ComponentModel.INotifyPropertyChanged

        Public Property ID() As Integer
            Get
                Return Me._ID
            End Get
            Set(ByVal value As Integer)
                Me._ID = value
                Me.OnPropertyChanged("ID")
            End Set
        End Property
        Private _ID As Integer

        Public Property DueDate() As Global.System.DateTimeOffset
            Get
                Return Me._DueDate
            End Get
            Set(ByVal value As Global.System.DateTimeOffset)
                Me._DueDate = value
                Me.OnPropertyChanged("DueDate")
            End Set
        End Property
        Private _DueDate As Global.System.DateTimeOffset

        Public Property Priority() As Integer
            Get
                Return Me._Priority
            End Get
            Set(ByVal value As Integer)
                Me._Priority = value
                Me.OnPropertyChanged("Priority")
            End Set
        End Property
        Private _Priority As Integer

        Public Property Comments() As String
            Get
                Return Me._Comments
            End Get
            Set(ByVal value As String)
                Me._Comments = value
                Me.OnPropertyChanged("Comments")
            End Set
        End Property
        Private _Comments As String

        Public Property Parent() As ClientBindingWorkItem
            Get
                Return Me._Parent
            End Get
            Set(ByVal value As ClientBindingWorkItem)
                Me._Parent = value
                Me.OnPropertyChanged("Parent")
            End Set
        End Property

        Private _Parent As ClientBindingWorkItem
        Public Property Children() As Global.Microsoft.OData.Client.DataServiceCollection(Of ClientBindingWorkItem)
            Get
                Return Me._Children
            End Get
            Set(ByVal value As Global.Microsoft.OData.Client.DataServiceCollection(Of ClientBindingWorkItem))
                Me._Children = value
                Me.OnPropertyChanged("Children")
            End Set
        End Property
        Private _Children As Global.Microsoft.OData.Client.DataServiceCollection(Of ClientBindingWorkItem) = New Global.Microsoft.OData.Client.DataServiceCollection(Of ClientBindingWorkItem)(Nothing, Microsoft.OData.Client.TrackingMode.None)
        Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
            If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
            End If
        End Sub
    End Class

    <Global.Microsoft.OData.Client.KeyAttribute("ID")>
    Public Class ClientBindingDevWorkItem
        Inherits ClientBindingWorkItem

        Public Property DevNote() As String
            Get
                Return Me._DevNote
            End Get
            Set(ByVal value As String)
                Me._DevNote = value
                Me.OnPropertyChanged("DevNote")
            End Set
        End Property
        Private _DevNote As String

        Public Property Developer() As ClientBindingDeveloperResource
            Get
                If ((Me._Developer Is Nothing) _
                            AndAlso (Me._DeveloperInitialized <> True)) Then
                    Me._Developer = New ClientBindingDeveloperResource
                    Me._DeveloperInitialized = True
                End If
                Return Me._Developer
            End Get
            Set(ByVal value As ClientBindingDeveloperResource)
                Me._Developer = value
                Me._DeveloperInitialized = True
                Me.OnPropertyChanged("Developer")
            End Set
        End Property

        Private _Developer As ClientBindingDeveloperResource
        Private _DeveloperInitialized As Boolean
    End Class

    Public Class ClientBindingFeatureWorkItem
        Inherits ClientBindingDevWorkItem

        Public Property FeatureName() As String
            Get
                Return Me._FeatureName
            End Get
            Set(ByVal value As String)
                Me._FeatureName = value
                Me.OnPropertyChanged("FeatureName")
            End Set
        End Property
        Private _FeatureName As String

        Public Property DesignComplete() As Boolean
            Get
                Return Me._DesignComplete
            End Get
            Set(ByVal value As Boolean)
                Me._DesignComplete = value
                Me.OnPropertyChanged("DesignComplete")
            End Set
        End Property
        Private _DesignComplete As Boolean

        Public Property FeatureSpec() As String
            Get
                Return Me._FeatureSpec
            End Get
            Set(ByVal value As String)
                Me._FeatureSpec = value
                Me.OnPropertyChanged("FeatureSpec")
            End Set
        End Property
        Private _FeatureSpec As String
    End Class

    <Global.Microsoft.OData.Client.KeyAttribute("ID")>
    Public Class ClientBindingBugWorkItem
        Inherits ClientBindingWorkItem

        Public Property ReproStep() As String
            Get
                Return Me._ReproStep
            End Get
            Set(ByVal value As String)
                Me._ReproStep = value
                Me.OnPropertyChanged("ReproStep")
            End Set
        End Property
        Private _ReproStep As String

        Public Property Severity() As Integer
            Get
                Return Me._Severity
            End Get
            Set(ByVal value As Integer)
                Me._Severity = value
                Me.OnPropertyChanged("Severity")
            End Set
        End Property
        Private _Severity As Integer

        Public Property BugType() As String
            Get
                Return Me._BugType
            End Get
            Set(ByVal value As String)
                Me._BugType = value
                Me.OnPropertyChanged("BugType")
            End Set
        End Property
        Private _BugType As String
    End Class

End Namespace

#End Region