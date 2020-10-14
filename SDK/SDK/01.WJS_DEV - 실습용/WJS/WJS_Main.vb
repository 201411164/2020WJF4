
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
    Implements IDisposable


    Public Sub New()

        MyBase.New()
        'ADD YOUR INITIALIZATION CODE HERE	...
    End Sub

    Public Overrides Sub OnShutDown()
        'ADD YOUR TERMINATION CODE HERE	...
        System.Windows.Forms.Application.Exit()
    End Sub

    Public Overrides Sub OnCompanyChanged()
        Try
            B1Connections.Reinit()
            MyBase.Moduleinit()
        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message)
        End Try
        
        'ADD YOUR COMPANY CHANGE CODE HERE	...
    End Sub


    Public Overrides Sub OnLanguageChanged(ByVal language As BoLanguages)
        'ADD YOUR LANGUAGE CHANGE CODE HERE	...
        Try
            B1Connections.Reinit()
            MyBase.Moduleinit()
        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message)
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

#If DIVERSION = "07" Then
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



    Private disposedValue As Boolean = False        ' �ߺ� ȣ���� �˻��Ϸ���

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: ��������� ȣ��Ǹ� �������� �ʴ� ���ҽ��� �����մϴ�.
            End If

            ' TODO: �������� �ʴ� ���� ���ҽ��� �����մϴ�.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' ���� ������ ������ �ùٸ��� �����ϱ� ���� Visual Basic���� �߰��� �ڵ��Դϴ�.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' �� �ڵ�� �������� ���ʽÿ�. ���� Dispose(ByVal disposing As Boolean)�� ���� �ڵ带 �Է��Ͻʽÿ�.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region



End Class



