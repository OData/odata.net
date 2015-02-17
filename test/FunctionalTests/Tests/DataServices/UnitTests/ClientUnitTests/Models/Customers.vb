'---------------------------------------------------------------------
' <copyright file="Customers.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Namespace NorthwindBindingModel

    ' Extend the client Customers class with internal and private properties and behaviors
    ' that raise PropertyChanged events
    Partial Public Class Customers

        Private m_internalTestProperty As String = "Test"
        Private m_privateTestProperty As String = "Test"

        Friend Property InternalTestProperty As String
            Get
                Return m_internalTestProperty
            End Get
            Set(value As String)
                m_internalTestProperty = value
                Me.OnPropertyChanged("InternalTestProperty")
            End Set
        End Property

        Private Property PrivateTestProperty As String
            Get
                Return m_privateTestProperty
            End Get
            Set(value As String)
                m_privateTestProperty = value
                Me.OnPropertyChanged("PrivateTestProperty")
            End Set
        End Property

        Friend Sub RaiseGarbagePropertyChanged()
            Me.OnPropertyChanged("!Not a property name$")
        End Sub

        Friend Sub SetPrivateTestProperty(value As String)
            PrivateTestProperty = value
        End Sub

    End Class

End Namespace
