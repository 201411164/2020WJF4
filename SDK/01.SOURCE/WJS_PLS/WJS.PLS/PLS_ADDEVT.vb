Imports AddOnBase
Imports SAPbobsCOM
Imports SAPbouiCOM
Imports System.Runtime.InteropServices
Imports WJS.COMM


Public Class PLS_ADDEVT
    Implements IDisposable


    Public Sub New()
        Me.Init()
    End Sub

    Public Sub Init()

        Try
            Dim oGuiApi As SAPbouiCOM.SboGuiApi = New SAPbouiCOM.SboGuiApi
            Dim connStr As String = B1Connections.connStr

            oGuiApi.Connect(connStr)
            cApp = oGuiApi.GetApplication(B1Connections.theAppl.AppId)
            'cApp = B1Connections.theAppl

        Catch ex As Exception

            B1Connections.theAppl.MessageBox(ex.Message)

        End Try

    End Sub


    Public WithEvents cApp As SAPbouiCOM.Application


    ''' <summary>
    ''' cApp_MenuEvent
    ''' </summary>
    ''' <param name="pVal"></param>
    ''' <param name="BubbleEvent"></param>
    ' ''' <remarks></remarks>
    'Public Sub cApp_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles cApp.MenuEvent

    '    Dim cForm As SAPbouiCOM.Form = Nothing
    '    Dim xSQL As String = ""
    '    Dim strColorMenu As String = ""

    '    Try

    '        cForm = cApp.Forms.ActiveForm

    '        If Not IsNothing(cForm) Then cForm.Freeze(True)

    '        '메뉴 클릭시 애드온 메뉴일 경우에는 테이블복사(Copy Table) 메뉴 비활성화
    '        If cForm.UniqueID.StartsWith("WJS_") Then

    '            cForm.EnableMenu(771, True)     '잘라내기
    '            cForm.EnableMenu(772, True)     '복사
    '            cForm.EnableMenu(773, True)     '붙여넣기
    '            cForm.EnableMenu(784, True)    '테이블복사

    '        End If

    '        '애드온 색상값 설정(조합과 클레식은 처리하지 않음.)
    '        'xSQL = "  "
    '        'xSQL = xSQL & " SELECT IFNULL(MAX(CAST(B.U_RMK1 AS NVARCHAR(10))), '') AS COLOR "
    '        'xSQL = xSQL & " FROM            OADM            A "
    '        'xSQL = xSQL & " LEFT OUTER JOIN ""@WJS_SAD021"" B ON B.""Code"" = 'PLS_AD00' AND A.""Color"" = CAST(B.U_CD AS SMALLINT) "
    '        'xSQL = xSQL & " WHERE A.""Color"" NOT IN (0, 1) "

    '        'strColorMenu = CFL.GetValue(xSQL)

    '        If strColorMenu <> "" Then
    '            '컬러메뉴 활성화
    '            B1Connections.theAppl.ActivateMenuItem(strColorMenu)
    '        End If


    '    Catch ex As Exception
    '        B1Connections.theAppl.MessageBox(ex.Message)
    '    Finally
    '        If Not IsNothing(cForm) Then cForm.Freeze(False)
    '        cForm = Nothing
    '    End Try

    'End Sub


    ''' <summary>
    ''' cApp_RightClickEvent
    ''' </summary>
    ''' <param name="eventInfo"></param>
    ''' <param name="BubbleEvent"></param>
    ''' <remarks></remarks>
    Private Sub cApp_RightClickEvent(ByRef eventInfo As SAPbouiCOM.ContextMenuInfo, ByRef BubbleEvent As Boolean) Handles cApp.RightClickEvent
        Dim cForm As SAPbouiCOM.Form
        Dim cItem As Long

        Try

            cForm = cApp.Forms.ActiveForm

            If eventInfo.BeforeAction Then

                '애드온 메뉴 이면서 메트릭스와 그리드 일때만 테이블복사(Copy Table) 메뉴가 활성화 되도록 처리함.
                If Not eventInfo.FormUID.StartsWith("WJS_") Then

                    Exit Try

                ElseIf eventInfo.FormUID.StartsWith("WJS_") And Trim(eventInfo.ItemUID) = "" Then

                    cForm.EnableMenu(771, False)    '잘라내기
                    cForm.EnableMenu(772, False)    '복사
                    'cForm.EnableMenu(773, False)    '붙여넣기
                    cForm.EnableMenu(784, False)    '테이블복사

                Else

                    cItem = cForm.Items.Item(Trim(eventInfo.ItemUID)).Type

                    If cItem = BoFormItemTypes.it_EDIT Or cItem = BoFormItemTypes.it_EXTEDIT Then
                        If Not cForm.Menu.Exists("771") Then cForm.EnableMenu(771, True) '잘라내기
                        If Not cForm.Menu.Exists("773") Then cForm.EnableMenu(773, True) '붙여넣기
                    Else
                        cForm.EnableMenu(771, False) '잘라내기
                        'cForm.EnableMenu(773, False) '붙여넣기
                    End If

                    If cItem = BoFormItemTypes.it_GRID Or cItem = BoFormItemTypes.it_MATRIX Then
                        If Not cForm.Menu.Exists("784") Then cForm.EnableMenu(784, True)
                    Else
                        cForm.EnableMenu(784, False)
                    End If

                    If Not cForm.Menu.Exists("772") Then cForm.EnableMenu(772, True) '복사

                End If

            End If

        Catch ex As Exception
            B1Connections.theAppl.MessageBox(ex.Message)
        Finally
            cForm = Nothing
        End Try

    End Sub

    Private Sub cApp_ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, ByRef BubbleEvent As Boolean) Handles cApp.ItemEvent

        Try

            If pVal.FormTypeEx = "2113146021" And pVal.EventType = BoEventTypes.et_CLICK And pVal.ItemUID = "btnSENDGW" Then

                ''지급생성대상에서 그룹웨어 전표 상신 클릭했을 때 사용자 정의 코드의 그룹웨어 URL읽어 브라우저 띄우기

                If pVal.BeforeAction = True Then

                    Dim oForm2 As SAPbouiCOM.Form
                    oForm2 = B1Connections.theAppl.Forms.Item(pVal.FormUID)

                    Dim XSQL As String
                    Dim strSTATUS As String
                    Try
                        XSQL = "SELECT U_GWDOCST FROM [@WJS_STR01T] WHERE DocEntry = '" & oForm2.Items.Item("edtDOCENTR").Specific.Value & "'"
                        strSTATUS = CFL.GetValue(XSQL)
                        If strSTATUS <> "S06" And strSTATUS <> "" Then
                            CFL.COMMON_MESSAGE("!", "전자결재상태가 등록일 때 상신 가능합니다.")
                            BubbleEvent = False
                            Exit Try
                        End If
                        BubbleEvent = True
                    Catch ex As Exception
                        B1Connections.theAppl.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

                    Finally

                    End Try

                Else
                    Dim oForm2 As SAPbouiCOM.Form
                    oForm2 = B1Connections.theAppl.Forms.Item(pVal.FormUID)

                    If oForm2.Mode <> BoFormMode.fm_OK_MODE Then
                        CFL.COMMON_MESSAGE("!", "확인모드에서만 결재가 가능합니다.")
                        Exit Try
                    End If

                    Dim DOCENT As String
                    Dim GRPUSER As String
                    Dim XSQL As String
                    Try

                        DOCENT = CFL.GetValue("SELECT DocEntry FROM [@WJS_STR01T] WHERE DocEntry = '" & oForm2.Items.Item("edtDOCENTR").Specific.Value & "'")
                        GRPUSER = CFL.GetValue("SELECT U_GWID FROM OUSR WHERE USER_CODE = '" & B1Connections.diCompany.UserName & "'")

                        XSQL = "SELECT U_NM + '" & GRPUSER & "&ERPNO=" & DOCENT & "' FROM [@WJS_SAD021] WHERE ""Code"" = 'PLS_AD01' AND U_CD = 'IF_OVPM'"
                        Process.Start("iexplore.exe", CFL.GetValue(XSQL))
                    Catch ex As Exception
                        B1Connections.theAppl.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                    Finally

                    End Try

                    BubbleEvent = False

                End If

            End If


        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("cApp_ItemEvent Error : " & ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally

        End Try


    End Sub


    ''' <summary>
    ''' SetWJS_Form
    ''' </summary>
    ''' <param name="ActiveForm"></param>
    ''' <remarks></remarks>
    Public Sub SetWJS_Form(ByVal ActiveForm As SAPbouiCOM.Form, ByVal strFilePath As String)
        Dim xmlDoc As Xml.XmlDocument = New Xml.XmlDocument()
        Dim pStartPath As String = ""
        Dim xmlFile As String = ""
        Dim xmlStr As String = ""

        Try

            '파일의경로를읽어온다. 
            pStartPath = System.Reflection.Assembly.GetExecutingAssembly.Location
            pStartPath = pStartPath.Substring(0, InStrRev(pStartPath, "\"))

            '스크린 페인터 파일을 지정한다.
            xmlFile = pStartPath & strFilePath

            'Xml Load
            xmlDoc.Load(xmlFile)

            '폼 ID 값을 현재 오픈된 Uid로 변경
            xmlDoc.SelectSingleNode("Application/forms/action/form/@uid").Value = ActiveForm.UniqueID.ToString

            'Xml 값을 String로 변경
            xmlStr = xmlDoc.DocumentElement.OuterXml

            '폼값 Update
            B1Connections.theAppl.LoadBatchActions(xmlStr)

        Catch ex As Exception
            B1Connections.theAppl.MessageBox(ex.Message)
        End Try

    End Sub


    Private disposedValue As Boolean = False        ' 중복 호출을 검색하려면

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: 명시적으로 호출되면 관리되지 않는 리소스를 해제합니다.
            End If

            ' TODO: 관리되지 않는 공유 리소스를 해제합니다.
        End If
        Me.disposedValue = True
    End Sub


#Region " IDisposable Support "
    ' 삭제 가능한 패턴을 올바르게 구현하기 위해 Visual Basic에서 추가한 코드입니다.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 이 코드는 변경하지 마십시오. 위의 Dispose(ByVal disposing As Boolean)에 정리 코드를 입력하십시오.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region


End Class
