Option Strict Off
Option Explicit On

Imports AddOnBase
Imports SAPbobsCOM
Imports SAPbouiCOM
Imports System.Xml
Imports System.Windows.Forms
Imports System.Reflection


Public Class WJS_Main

    Inherits B1AddOn

    Public Sub New()
        MyBase.New()
        'ADD YOUR INITIALIZATION CODE HERE	...
    End Sub

    Public Overrides Sub OnShutDown()
        'ADD YOUR TERMINATION CODE HERE	...
        System.Windows.Forms.Application.Exit()
    End Sub

    Public Overrides Sub OnCompanyChanged()
        Dim Form As SAPbouiCOM.Form
        Dim i As Integer = 0

        Try

            For i = 0 To B1Connections.theAppl.Forms.Count - 1 Step 1

                If B1Connections.theAppl.Forms.Item(i).TypeEx = "169" Then
                    Form = B1Connections.theAppl.Forms.Item(i)
                    Exit For
                End If

            Next

            B1Connections.Reinit()

            If Not IsNothing(Form) Then
                Form.Freeze(True)
            End If

            MyBase.Moduleinit()

            If Not IsNothing(Form) Then
                Form.Freeze(False)
                Form.Update()
            End If

        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message)
        Finally
            Form = Nothing
        End Try
    End Sub


    Public Overrides Sub OnLanguageChanged(ByVal language As SAPbouiCOM.BoLanguages)
        Dim Form As SAPbouiCOM.Form
        Dim i As Integer = 0

        Try

            For i = 0 To B1Connections.theAppl.Forms.Count - 1 Step 1

                If B1Connections.theAppl.Forms.Item(i).TypeEx = "169" Then
                    Form = B1Connections.theAppl.Forms.Item(i)
                    Exit For
                End If

            Next

            B1Connections.Reinit()

            If Not IsNothing(Form) Then
                Form.Freeze(True)
            End If

            MyBase.Moduleinit()

            If Not IsNothing(Form) Then
                Form.Freeze(False)
                Form.Update()
            End If

        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message)
        Finally
            Form = Nothing
        End Try
    End Sub

    Public Overrides Sub OnStatusBarErrorMessage(ByVal txt As String)
        'ADD YOUR CODE HERE	...
    End Sub

    Public Overrides Sub OnStatusBarSuccessMessage(ByVal txt As String)
        'ADD YOUR CODE HERE	...
    End Sub

    Public Overrides Sub OnStatusBarWarningMessage(ByVal txt As String)
        'ADD YOUR CODE HERE	...
    End Sub

    Public Overrides Sub OnStatusBarNoTypedMessage(ByVal txt As String)
        'ADD YOUR CODE HERE	...
    End Sub

    Public Overrides Function OnBeforeProgressBarCreated() As Boolean
        'ADD YOUR CODE HERE	...
        Return True
    End Function

    Public Overrides Function OnAfterProgressBarCreated() As Boolean
        'ADD YOUR CODE HERE	...
        Return True
    End Function

    Public Overrides Function OnBeforeProgressBarStopped(ByVal success As Boolean) As Boolean
        'ADD YOUR CODE HERE	...
        Return True
    End Function

    Public Overrides Function OnAfterProgressBarStopped(ByVal success As Boolean) As Boolean
        'ADD YOUR CODE HERE	...
        Return True
    End Function

    Public Overrides Function OnProgressBarReleased() As Boolean
        'ADD YOUR CODE HERE	...
        Return True
    End Function


    Public Shared Sub Main()
        Dim retCode As Integer = 0
        Dim connStr As String = ""
        Dim diRequired As Boolean = True
        'CHANGE ADDON IDENTIFIER BEFORE RELEASING TO CUSTOMER (Solution Identifier)
        Dim addOnIdentifierStr As String = "5645523035446576656C6F706D656E743A4730393036313732343831B8E4AFB3C2130B13E287247A4" & _
            "43CA06CB6C5B9FD"
        If (System.Environment.GetCommandLineArgs().Length = 1) Then
            connStr = B1Connections.connStr
        Else
            connStr = System.Environment.GetCommandLineArgs().GetValue(1).ToString()
        End If


        Try
            'INIT(CONNECTIONS)

#If DIVERSION = "07" Or DIVERSION = "80" Or DIVERSION = "81" Or DIVERSION = "82" Or DIVERSION = "90" Or DIVERSION = "92" Or DIVERSION = "93" Then
            retCode = B1Connections.Init(connStr, addOnIdentifierStr, B1Connections.ConnectionType.MultipleAddOns)
#Else
            retCode = B1Connections.Init(connStr, addOnIdentifierStr, diRequired)
#End If


            'CONNECTION FAILED
            If (retCode <> 0) Then
                System.Windows.Forms.MessageBox.Show("ERROR - Connection failed: " + B1Connections.diCompany.GetLastErrorDescription())
                Return
            End If

            ''CREATE DB
            'If (diRequired = true) Then
            '    Dim addOnDb As B1AddOn2_Db = New B1AddOn2_Db
            '    addOnDb.Add(B1Connections.diCompany)
            'End If


            'CREATE ADD-ON
            Dim addOn As WJS_Main = New WJS_Main
            'System.Windows.Forms.MessageBox.Show(System.Windows.Forms.Application.StartupPath)

            System.Windows.Forms.Application.Run()


        Catch com_err As Exception 'System.Runtime.InteropServices.COMException
            'HANDLE ANY COMException HERE
            System.Windows.Forms.MessageBox.Show("ERROR - Connection failed: " + com_err.Message)
        End Try
    End Sub


End Class
