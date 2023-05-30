#If Not NETCOREAPP1_1 Then
Namespace Global.Microsoft.OData.TestCommon
    Delegate Sub Fake(listener As DebugAssertTraceListener)
End Namespace
#End If