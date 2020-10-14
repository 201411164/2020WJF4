
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

            'oCompany.Server = INIRead("INFO", "SERVERIP", AppPath & "\SERVER.ini")
            'oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English
            ''oCompany.UseTrusted = True
            'oCompany.CompanyDB = INIRead("INFO", "COMPANYDB", AppPath & "\SERVER.ini")
            'oCompany.UserName = INIRead("INFO", "USERNAME", AppPath & "\SERVER.ini")
            'oCompany.Password = INIRead("INFO", "PASSWORD", AppPath & "\SERVER.ini")
            'oCompany.DbUserName = INIRead("INFO", "DBUSERNAME", AppPath & "\SERVER.ini")
            'oCompany.DbPassword = INIRead("INFO", "DBPASSWORD", AppPath & "\SERVER.ini")
            'oCompany.DbServerType = INIRead("INFO", "DBTYPE", AppPath & "\SERVER.ini")


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
                MsgBox("접속성공")
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

        Dim xSQL As String = ""
        Dim oRS As SAPbobsCOM.Recordset = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim i As Integer = 0

        '-----------------------------------------------------------------------------
        '-- OITM 만들기 시작
        '-----------------------------------------------------------------------------


        Dim oITEM As SAPbobsCOM.Items = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems)

        Try

            If oITEM.GetByKey("TEST00001") Then
                oITEM.ItemName = "TEST00001"

                If oITEM.Update <> 0 Then
                    MsgBox(oCompany.GetLastErrorDescription)
                Else
                    MsgBox("ITEM 갱신 완료")
                End If
            Else
                oITEM.ItemCode = "TEST00001"
                oITEM.ItemName = "TEST00001"

                oITEM.ItemType = SAPbobsCOM.ItemTypeEnum.itItems

                oITEM.SalesItem = SAPbobsCOM.BoYesNoEnum.tYES
                oITEM.PurchaseItem = SAPbobsCOM.BoYesNoEnum.tYES
                oITEM.InventoryItem = SAPbobsCOM.BoYesNoEnum.tYES


                If oITEM.Add <> 0 Then
                    MsgBox(oCompany.GetLastErrorDescription)
                Else
                    MsgBox("ITEM 생성 완료")
                End If
            End If



        Catch ex As System.Exception
            MsgBox(Err.Description)


        Finally

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oITEM)
            oITEM = Nothing

        End Try

        '-----------------------------------------------------------------------------
        '-- OITM 만들기 끝
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '-- BP 만들기 시작
        '-----------------------------------------------------------------------------


        Dim oBP As SAPbobsCOM.BusinessPartners = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners)

        Try

            If oBP.GetByKey("TEST_BP_00001") Then
                oBP.CardCode = "TEST_BP_00001"

                If oBP.Update <> 0 Then
                    MsgBox(oCompany.GetLastErrorDescription)
                Else
                    MsgBox("BP 갱신 완료")
                End If
            Else
                oBP.CardCode = "TEST_BP_00001"
                oBP.CardName = "TEST_BP_00001"


                If oBP.Add <> 0 Then
                    MsgBox(oCompany.GetLastErrorDescription)
                Else
                    MsgBox("BP 생성 완료")
                End If
            End If



        Catch ex As System.Exception
            MsgBox(Err.Description)


        Finally

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBP)
            oBP = Nothing

        End Try

        '-----------------------------------------------------------------------------
        '-- BP 만들기 끝
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '-- 판매오더 만들기 시작
        '-----------------------------------------------------------------------------

        Dim oOPOR As SAPbobsCOM.Documents = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders)

        Try

            xSQL = ""

            xSQL = xSQL & "SELECT  T0.CardCode," & vbCrLf
            xSQL = xSQL & "        T1.ItemCode," & vbCrLf
            xSQL = xSQL & "        T1.Quantity," & vbCrLf
            xSQL = xSQL & "        T1.LineTotal" & vbCrLf
            xSQL = xSQL & "FROM OPOR T0" & vbCrLf
            xSQL = xSQL & "INNER JOIN POR1 T1 ON T0.DocEntry = T1.DocEntry" & vbCrLf
            xSQL = xSQL & "WHERE T0.DocEntry = '1'" & vbCrLf


            oRS.DoQuery(xSQL)

            If Not oRS.EoF Then



                oOPOR.DocDate = Now.Date.ToString("yyyy-MM-dd")
                oOPOR.CardCode = oRS.Fields.Item("CardCode").Value

                oOPOR.Lines.SetCurrentLine(0)

                oOPOR.BPL_IDAssignedToInvoice = "1"

                For i = 1 To oRS.RecordCount
                    oOPOR.Lines.ItemCode = oRS.Fields.Item("ItemCode").Value.ToString.Trim
                    oOPOR.Lines.Quantity = oRS.Fields.Item("Quantity").Value.ToString.Trim
                    oOPOR.Lines.LineTotal = oRS.Fields.Item("LineTotal").Value.ToString.Trim
                    oOPOR.Lines.Add()

                    oRS.MoveNext()

                Next

                If oOPOR.Add <> 0 Then
                    MsgBox(oCompany.GetLastErrorDescription)
                Else
                    MsgBox(oCompany.GetNewObjectKey & "로 구매오더 생성 완료")
                End If
            End If


        Catch ex As System.Exception
            MsgBox(Err.Description)


        Finally


            System.Runtime.InteropServices.Marshal.ReleaseComObject(oOPOR)
            oOPOR = Nothing

        End Try

        '-----------------------------------------------------------------------------
        '-- 판매오더 만들기 끝
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '-- 구매오더 만들기 시작
        '-----------------------------------------------------------------------------

        Dim oORDR As SAPbobsCOM.Documents = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders)

        Try

            xSQL = ""

            xSQL = xSQL & "SELECT  T0.CardCode," & vbCrLf
            xSQL = xSQL & "        T1.ItemCode," & vbCrLf
            xSQL = xSQL & "        T1.Quantity," & vbCrLf
            xSQL = xSQL & "        T1.LineTotal" & vbCrLf
            xSQL = xSQL & "FROM ORDR T0" & vbCrLf
            xSQL = xSQL & "INNER JOIN RDR1 T1 ON T0.DocEntry = T1.DocEntry" & vbCrLf
            xSQL = xSQL & "WHERE T0.DocEntry = '1'" & vbCrLf


            oRS.DoQuery(xSQL)

            If Not oRS.EoF Then



                oORDR.DocDate = Now.Date.ToString("yyyy-MM-dd")
                oORDR.DocDueDate = Now.Date.ToString("yyyy-MM-dd")

                oORDR.CardCode = oRS.Fields.Item("CardCode").Value

                oORDR.Lines.SetCurrentLine(0)

                oORDR.BPL_IDAssignedToInvoice = "1"

                For i = 1 To oRS.RecordCount
                    oORDR.Lines.ItemCode = oRS.Fields.Item("ItemCode").Value.ToString.Trim
                    oORDR.Lines.Quantity = oRS.Fields.Item("Quantity").Value.ToString.Trim
                    oORDR.Lines.LineTotal = oRS.Fields.Item("LineTotal").Value.ToString.Trim
                    oORDR.Lines.Add()

                    oRS.MoveNext()

                Next

                If oORDR.Add <> 0 Then
                    MsgBox(oCompany.GetLastErrorDescription)
                Else
                    MsgBox(oCompany.GetNewObjectKey & "로 판매오더 생성 완료")
                End If
            End If


        Catch ex As System.Exception
            MsgBox(Err.Description)


        Finally


            System.Runtime.InteropServices.Marshal.ReleaseComObject(oORDR)
            oORDR = Nothing

        End Try

        '-----------------------------------------------------------------------------
        '-- 구매오더 만들기 끝
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '-- 분개 만들기 시작
        '-----------------------------------------------------------------------------

        Try

            xSQL = ""

            xSQL = xSQL & "SELECT  T0.Memo," & vbCrLf
            xSQL = xSQL & "        T1.Account," & vbCrLf
            xSQL = xSQL & "        T1.Debit," & vbCrLf
            xSQL = xSQL & "        T1.Credit," & vbCrLf
            xSQL = xSQL & "        T1.ShortName," & vbCrLf
            xSQL = xSQL & "T1.Line_ID" & vbCrLf
            xSQL = xSQL & "FROM OJDT T0" & vbCrLf
            xSQL = xSQL & "INNER JOIN JDT1 T1 ON T0.TransID = T1.TransID" & vbCrLf
            xSQL = xSQL & "WHERE T0.TransID = '1'" & vbCrLf


            oRS.DoQuery(xSQL)

            If Not oRS.EoF Then

                Dim oJE As SAPbobsCOM.JournalEntries = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries)

                oJE.ReferenceDate = Now.Date.ToString("yyyy-MM-dd")
                oJE.Memo = oRS.Fields.Item("Memo").Value

                oJE.Lines.SetCurrentLine(0)

                For i = 1 To oRS.RecordCount
                    oJE.Lines.AccountCode = oRS.Fields.Item("Account").Value.ToString.Trim

                    If oRS.Fields.Item("Debit").Value <> 0 Then
                        oJE.Lines.Debit = oRS.Fields.Item("Debit").Value
                    End If

                    If oRS.Fields.Item("Credit").Value <> 0 Then
                        oJE.Lines.Credit = oRS.Fields.Item("Credit").Value
                    End If

                    oJE.Lines.ShortName = oRS.Fields.Item("ShortName").Value

                    oJE.Lines.Add()

                    oRS.MoveNext()

                Next

                If oJE.Add <> 0 Then
                    MsgBox(oCompany.GetLastErrorDescription)
                Else
                    MsgBox(oCompany.GetNewObjectKey & "로 분개생성 완료")
                End If
            End If


        Catch ex As System.Exception
            MsgBox(Err.Description)


        Finally

            If oCompany.Connected Then
                oCompany.Disconnect()
            End If

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oCompany)

            oCompany = Nothing
            oRS = Nothing

            Me.Dispose()
            Me.Close()

        End Try

        '-----------------------------------------------------------------------------
        '-- 분개 만들기 끝
        '-----------------------------------------------------------------------------

    End Sub


End Class
