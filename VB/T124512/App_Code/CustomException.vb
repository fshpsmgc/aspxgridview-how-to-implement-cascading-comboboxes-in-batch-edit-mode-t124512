Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Public Class DemoException
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
End Class
