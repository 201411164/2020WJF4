
Imports System.IO
Imports System.Net

Imports VB = Microsoft.VisualBasic

Public Class WJSBO_DIAPI
    Private oCompany As SAPbobsCOM.Company

    'Dim oCompany As New SAPbobsCOM.Company

    Private Sub WJSBOBatchServiceExe_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        PollingPass()
    End Sub

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button.Click
        PollingPass()
    End Sub


    'INI 스트링을 읽어오기 위한 API 선언
    Public Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" _
        (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, _
        ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer



    'INI 스트링을 기록하기 위한 API 선언
    Public Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" _
        (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer

    Public Function INIRead(ByVal Session As String, ByVal KeyValue As String, ByVal INIFILE As String) As String

        'INI 값 읽기

        Dim s As New String("", 1024)


        Dim ReturnValue As Long

        ReturnValue = GetPrivateProfileString(Session, KeyValue, "", s, 1024, INIFILE)

        Return Mid(s, 1, InStr(s, Chr(0)) - 1)

    End Function



    '*************************************************************
    '함수명:    PollProcess 
    '개  요:    
    '변  수:    
    '리  턴:    
    '생성일:    
    '생성자:    

    '수정자:
    '*************************************************************
    Private Sub PollingPass()

        '-----------------------------------------------------------------------------
        '-- SBO 접속 시작
        '-----------------------------------------------------------------------------

        Dim AppPath As String = Application.StartupPath


        Try

            '실행중
            oCompany = New SAPbobsCOM.Company

            oCompany.Server = "77100247-PC"
            oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English
            'oCompany.UseTrusted = True
            oCompany.CompanyDB = "ONEPACKTEST"
            oCompany.UserName = "manager"
            oCompany.Password = "1234"
            oCompany.DbUserName = "sa"
            oCompany.DbPassword = "1"
            oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012

            If oCompany.Connect <> 0 Then
                MsgBox(oCompany.GetLastErrorDescription)
                If oCompany.Connected Then
                    oCompany.Disconnect()
                End If

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oCompany)

                oCompany = Nothing

                Me.Dispose()
                Me.Close()

                Exit Sub

            Else
                'MsgBox("1")
            End If



        Catch ex As System.Exception
            MsgBox(Err.Description)

            If oCompany.Connected Then
                oCompany.Disconnect()
            End If

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oCompany)

            oCompany = Nothing

            Me.Dispose()
            Me.Close()

        Finally


        End Try


        '-----------------------------------------------------------------------------
        '-- SBO 접속 끝
        '-----------------------------------------------------------------------------



    End Sub


End Class
