Imports AddOnBase
Imports SAPbobsCOM
Imports SAPbouiCOM
Imports System.Runtime.InteropServices
Imports WJS.COMM
Imports Excel.Constants
Imports Excel.XlBordersIndex
Imports Excel.XlLineStyle
Imports Excel.XlBorderWeight

Module DEV_COMMON

#Region "WJS_ZZ_Defined"

    Public Const SYSTEMNAME As String = "SAP Business ONE"
    Public Const ODBC_LIST As String = "Software\ODBC\ODBC.INI\ODBC Data Sources"
    Public Const ODBC_LIST_INI As String = "Software\ODBC\ODBCINST.INI\ODBC Drivers"
    Public Const ODBC_LIST_INI_SQL As String = "Software\ODBC\ODBCINST.INI\SQL Server"
    Public Const ODBC_LIST_NEW As String = "Software\ODBC\ODBC.INI\"
    Public Const ODBC_RPT_PASS As String = "SOFTWARE\SAP\SAP MANAGE\SAP Business One\AddOn"
    Public Const ODBC_RPT_NAME As String = "DBS_CRSS"

    Public Const CAPTION_CLEAR As String = "제거"
    Public Const CAPTION_ADD As String = "추가"
    Public Const CAPTION_OK As String = "확인"
    Public Const CAPTION_UPDATE As String = "갱신"
    Public Const CAPTION_FIND As String = "찾기"
    Public Const CAPTION_DELETE As String = "삭제"
    Public Const CAPTION_ERROR As String = "에러"
    Public Const CAPTION_CANCEL As String = "취소"
    Public Const CAPTION_ABORT As String = "멈춤"
    Public Const CAPTION_RETRY As String = "재시도"
    Public Const CAPTION_IGNORE As String = "무시"
    Public Const CAPTION_YES As String = "예"
    Public Const CAPTION_NO As String = "아니오"

    Public Const CLASS_TOP As Byte = 0
    Public Const CLASS_ONE As Byte = 1
    Public Const CLASS_TWO As Byte = 2
    Public Const CLASS_THREE As Byte = 3
    Public Const CLASS_FOUR As Byte = 4
    Public Const CLASS_FIVE As Byte = 5

    Public Const TRUE_VALUE As Byte = 1
    Public Const FALSE_VALUE As Byte = 0

    Public Const RTN_TRUE As String = "success"
    Public Const RTN_FALSE As String = "exist"
    Public Const RTN_FAIL As String = "fail"
    Public Const RTN_ERROR As String = "error"
    Public Const YES_KOR_TEXT As String = "예"
    Public Const NO_KOR_TEXT As String = "아니오"
    Public Const YES_ENG_TEXT As String = "Y"
    Public Const NO_ENG_TEXT As String = "N"

    Public Const SPACE_ONE As String = "　"                'SBO의 스페이스 처리 상수

    Public Const SPC_MARK As String = "  "
    Public Const LIK_MARK As String = "%"
    Public Const ALL_MARK As String = "*"
    Public Const YES_MARK As String = "Y"
    Public Const NO_MARK As String = "N"
    Public Const INS_FLAG As String = "I"
    Public Const UPT_FLAG As String = "U"
    Public Const QUE_FLAG As String = "Q"
    Public Const SEL_FLAG As String = "F"
    Public Const DEL_FLAG As String = "D"

    Public oApplication As SAPbouiCOM.Application
    Public oCompany As SAPbobsCOM.Company


    Public oCrxReport As CRAXDDRT.Report
    Public oCrxApplication As New CRAXDDRT.ApplicationClass

    Public gFormCnt As Long                      '로드된 Form의 갯수
    Public gODBCName As String
    Public gODBCPath As String
    Public gRptDrv As String
    Public gPFormUID As String                    '부모창의 FormUID
    Public gCFormUID As String                    '자식창의 FormUID
    Public gv_Modal_b As Boolean
    Public gv_ModalID_i As String
    Public gMSGUID As String                    '메시지 화면 컨트롤을 위한 전역변수

    Public gAPPROVAL As String                    '승인권한요청을 위한 전역변수

    Public gAPPROVALCHK As Boolean                   '승인결정리포트에서 하나만 선택해서 체크되도록


    'Global Variable-----------------------------------------------------------------------
    'Public DBSFUNC As New WJS_ZZ_CommFunc      '일반Function을 Class 정의한다
    'Public SBOFUNC As New DBS_ZZ_SboFunc       'SBO Function을 Class 정의한다

    Enum Enum_MsgType
        m_Message = 1
        m_Caption = 2
    End Enum

    Enum Enum_ActionMode
        m_Add = 1
        m_addline = 2
        m_Find = 3
        m_Save = 4
        m_copy = 5
        m_CopyLine = 6
        m_Update = 7
        m_Delete = 8
        m_DelLine = 9
        m_Cancel = 10
        m_Close = 11
    End Enum

    Enum Enum_PrintMode
        m_Printer = 1
        m_Monitor = 2
    End Enum

    Enum Enum_LockCase
        m_Rate = 1
        m_Minors = 2
        m_Single = 3
        m_Qty = 9
    End Enum

    Enum Enum_TextCase
        m_Normal = 1
        m_LCase = 2
        m_UCase = 3
    End Enum

    Enum Enum_FormItemType
        it_ACTIVE_X = 102
        it_BUTTON = 4
        it_CHECK_BOX = 121
        it_COMBO_BOX = 113
        it_EDIT = 16
        it_EXTEDIT = 118
        it_FOLDER = 99
        it_LINKED_BUTTON = 116
        it_MATRIX = 127
        it_OPTION_BUTTON = 122
        it_PANE_COMBO_BOX = 104
        it_PICTURE = 117
        it_RECTANGLE = 100
        it_STATIC = 8
    End Enum

    Enum Enum_FindFlag
        m_NormalClick = 1
        m_MenuClick = 2
        m_TopClick = 3
        m_LeftClick = 4
        m_RightClick = 5
        m_BottomClick = 6
    End Enum

    Public Structure uEvent
        Dim ItEvent As SAPbouiCOM.ItemEvent
        Dim DtEvent As SAPbouiCOM.IBusinessObjectInfo
        Dim MuEvent As SAPbouiCOM.IMenuEvent
    End Structure

    'Public Structure SERIALITEM
    '    Dim ItemCode As String
    '    Dim ItemName As String
    '    Dim Qty As Double
    'End Structure

#End Region

#Region "WJS_ZZ_Excel"

    ''' <summary>
    ''' 엑셀모듈 
    ''' </summary>
    ''' <remarks>기존 엑셀모듈 컨버젼</remarks>
    Class WJS_ZZ_Excel

        '---- True,False선택
        Public Enum xTriConstants
            eFalse = 0
            eTrue = 1
        End Enum

        '---- True,False선택
        Public Enum x1TriConstants
            e1False = 0
            e1True = 1
        End Enum


        '---- 정렬값 지정

        Public Enum eAlignConstants
            eCenter = -4108
            eLeft = -4131
            eRight = -4152
            eButtom = -4107
            eTop = -4160
        End Enum

        ''' <summary>
        ''' 셀전체 -> 폰트설정 (크기,굵기,이텔릭체,색상,폰트)
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <param name="tSize"></param>
        ''' <param name="tBold"></param>
        ''' <param name="tItalic"></param>
        ''' <param name="tColor"></param>
        ''' <param name="tFontName"></param>
        ''' <remarks>셀전체 -> 폰트설정 (크기,굵기,이텔릭체,색상,폰트)</remarks>
        Public Sub xlsCellsFontSize(ByVal txls As Object, _
        Optional ByVal tSize As Integer = 10, _
        Optional ByVal tBold As xTriConstants = xTriConstants.eFalse, _
        Optional ByVal tItalic As xTriConstants = xTriConstants.eFalse, _
        Optional ByVal tColor As Integer = 1, _
        Optional ByVal tFontName As String = "")

            With txls.Cells.Font
                .Size = tSize              '글자크기

                .Bold = tBold              '굵기
                .Italic = tItalic          '이텔릭체
                .ColorIndex = tColor       '글자색상


                '폰트설정
                If tFontName <> "" Then
                    .Name = tFontName
                End If

            End With

        End Sub

        ''' <summary>
        ''' 셀범위 -> 폰트설정 (크기,굵기,이텔릭체,색상,폰트)
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <param name="tCol1"></param>
        ''' <param name="tRow1"></param>
        ''' <param name="tCol2"></param>
        ''' <param name="tRow2"></param>
        ''' <param name="tSize"></param>
        ''' <param name="tBold"></param>
        ''' <param name="tItalic"></param>
        ''' <param name="tColor"></param>
        ''' <param name="tFontName"></param>
        ''' <remarks>셀범위 -> 폰트설정 (크기,굵기,이텔릭체,색상,폰트)</remarks>
        Public Sub xlsCellFont(ByVal txls As Object, _
            ByVal tCol1 As Long, ByVal tRow1 As Long, ByVal tCol2 As Long, ByVal tRow2 As Long, _
        Optional ByVal tSize As Integer = 10, _
        Optional ByVal tBold As xTriConstants = xTriConstants.eFalse, _
        Optional ByVal tItalic As x1TriConstants = x1TriConstants.e1False, _
        Optional ByVal tColor As Integer = 1, _
        Optional ByVal tFontName As String = "")

            '---- 셀범위지정하기

            txls.Range(txls.Cells(tRow1, tCol1), txls.Cells(tRow2, tCol2)).Select()

            With txls.Selection.Font

                .Size = tSize              '글자크기

                .Bold = tBold              '굵기
                .Italic = tItalic          '이텔릭체
                .ColorIndex = tColor       '글자색상


                '폰트설정
                If tFontName <> "" Then
                    .Name = tFontName
                End If

            End With

        End Sub

        ''' <summary>
        ''' 셀전체 -> 컴럼크기 자동설정
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <remarks>셀전체 -> 컴럼크기 자동설정</remarks>
        Public Sub xlsCellsAutoFit(ByVal txls As Object)

            With txls
                .Cells.Select()
                .Cells.Select.Cells.EntireColumn.AutoFit()
            End With

        End Sub


        ''' <summary>
        ''' 셀전체 -> 가로,세로 좌우정렬
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <param name="tHAlign"></param>
        ''' <param name="tVAlign"></param>
        ''' <remarks>셀전체 -> 가로,세로 좌우정렬</remarks>
        Public Sub xlsCellsAlign(ByVal txls As Object, _
            ByVal tHAlign As eAlignConstants, _
            ByVal tVAlign As eAlignConstants)

            txls.Cells.Select()
            With txls.Cells
                .HorizontalAlignment = tHAlign       '수평정렬
                .VerticalAlignment = tVAlign         '수직정렬
                .WrapText = False
                .Orientation = 0
                .AddIndent = False
                .ShrinkToFit = False
            End With

        End Sub


        ''' <summary>
        ''' 셀범위 -> 가로,세로 좌우정렬
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <param name="tCol1"></param>
        ''' <param name="tRow1"></param>
        ''' <param name="tCol2"></param>
        ''' <param name="tRow2"></param>
        ''' <param name="tHAlign"></param>
        ''' <param name="tVAlign"></param>
        ''' <remarks>셀범위 -> 가로,세로 좌우정렬</remarks>
        Public Sub xlsCellAlign(ByVal txls As Object, _
                    ByVal tCol1 As Long, ByVal tRow1 As Long, ByVal tCol2 As Long, ByVal tRow2 As Long, _
                    ByVal tHAlign As eAlignConstants, _
                    ByVal tVAlign As eAlignConstants)

            txls.Range(txls.Cells(tRow1, tCol1), txls.Cells(tRow2, tCol2)).Select()

            With txls.Selection
                .HorizontalAlignment = tHAlign       '수평정렬
                .VerticalAlignment = tVAlign         '수직정렬
                .WrapText = False
                .Orientation = 0
                .AddIndent = False
                .ShrinkToFit = False
            End With

        End Sub


        ''' <summary>
        ''' 셀 -> 색상 지정하기 범위지정하기(col1,row1,col2,row2)
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <param name="tCol1"></param>
        ''' <param name="tRow1"></param>
        ''' <param name="tCol2"></param>
        ''' <param name="tRow2"></param>
        ''' <param name="tindex"></param>
        ''' <remarks>셀 -> 색상 지정하기 범위지정하기(col1,row1,col2,row2)</remarks>
        Public Sub xlsCellColor(ByVal txls As Object, _
            ByVal tCol1 As Long, ByVal tRow1 As Long, ByVal tCol2 As Long, ByVal tRow2 As Long, _
            ByVal tindex As Integer)

            txls.Range(txls.Cells(tRow1, tCol1), txls.Cells(tRow2, tCol2)).Select()

            With txls.Selection.Interior
                .ColorIndex = tindex
                .Pattern = Excel.Constants.xlSolid
            End With

        End Sub


        ''' <summary>
        ''' 셀 -> 셀병합하기(Col1,Row1,Col2,Row2)
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <param name="tCol1"></param>
        ''' <param name="tRow1"></param>
        ''' <param name="tCol2"></param>
        ''' <param name="tRow2"></param>
        ''' <param name="tHAlign"></param>
        ''' <param name="tVAlign"></param>
        ''' <remarks>셀 -> 셀병합하기(Col1,Row1,Col2,Row2)</remarks>
        Public Sub xlsCellMerge(ByVal txls As Object, _
            ByVal tCol1 As Long, ByVal tRow1 As Long, ByVal tCol2 As Long, ByVal tRow2 As Long, _
            ByVal tHAlign As eAlignConstants, _
            ByVal tVAlign As eAlignConstants)

            txls.Range(txls.Cells(tRow1, tCol1), txls.Cells(tRow2, tCol2)).Select()

            With txls.Selection
                .HorizontalAlignment = tHAlign
                .VerticalAlignment = tVAlign
                .WrapText = False
                .Orientation = 0
                .AddIndent = False
                .ShrinkToFit = False
                .MergeCells = False
            End With

            txls.Selection.Merge()

        End Sub


        ''' <summary>
        ''' 셀 -> 범위지정 보더표현(Col1,Row1,Col2,Row2)
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <param name="tCol1"></param>
        ''' <param name="tRow1"></param>
        ''' <param name="tCol2"></param>
        ''' <param name="tRow2"></param>
        ''' <remarks>셀 -> 범위지정 보더표현(Col1,Row1,Col2,Row2)</remarks>
        Public Sub xlsCellBorder(ByVal txls As Object, _
                ByVal tCol1 As Long, ByVal tRow1 As Long, ByVal tCol2 As Long, ByVal tRow2 As Long)

            txls.Range(txls.Cells(tRow1, tCol1), txls.Cells(tRow2, tCol2)).Select()
            txls.Selection.Borders(xlDiagonalDown).LineStyle = xlNone
            txls.Selection.Borders(xlDiagonalUp).LineStyle = xlNone


            With txls.Selection.Borders(xlEdgeLeft)
                .LineStyle = xlContinuous
                .Weight = xlThin
                .ColorIndex = xlAutomatic
            End With


            With txls.Selection.Borders(xlEdgeTop)
                .LineStyle = xlContinuous
                .Weight = xlThin
                .ColorIndex = xlAutomatic
            End With


            With txls.Selection.Borders(xlEdgeBottom)
                .LineStyle = xlContinuous
                .Weight = xlThin
                .ColorIndex = xlAutomatic
            End With


            With txls.Selection.Borders(xlEdgeRight)
                .LineStyle = xlContinuous
                .Weight = xlThin
                .ColorIndex = xlAutomatic
            End With


            '---- 수직 COL1개인경우제외
            If tCol1 <> tCol2 Then
                With txls.Selection.Borders(xlInsideVertical)
                    .LineStyle = xlContinuous
                    .Weight = xlThin
                    .ColorIndex = xlAutomatic
                End With
            End If

            '---- 수평 ROW1개인경우 제외
            If tRow1 <> tRow2 Then
                With txls.Selection.Borders(xlInsideHorizontal)
                    .LineStyle = xlContinuous
                    .Weight = xlThin
                    .ColorIndex = xlAutomatic
                End With
            End If

        End Sub

        ''' <summary>
        ''' 셀 -> 범위지정 보더표현(Col1,Row1,Col2,Row2)
        ''' </summary>
        ''' <param name="txls"></param>
        ''' <param name="tCol1"></param>
        ''' <param name="tRow1"></param>
        ''' <param name="tCol2"></param>
        ''' <param name="tRow2"></param>
        ''' <param name="sbTop"></param>
        ''' <param name="sbButtom"></param>
        ''' <param name="sbLeft"></param>
        ''' <param name="sbRight"></param>
        ''' <remarks>셀 -> 범위지정 보더표현(Col1,Row1,Col2,Row2)</remarks>
        Public Sub xlsCellBorder01(ByVal txls As Object, _
        ByVal tCol1 As Long, ByVal tRow1 As Long, ByVal tCol2 As Long, ByVal tRow2 As Long, _
        Optional ByVal sbTop As Boolean = False, Optional ByVal sbButtom As Boolean = False, _
        Optional ByVal sbLeft As Boolean = False, Optional ByVal sbRight As Boolean = False)

            txls.Range(txls.Cells(tRow1, tCol1), txls.Cells(tRow2, tCol2)).Select()
            txls.Selection.Borders(xlDiagonalDown).LineStyle = xlNone
            txls.Selection.Borders(xlDiagonalUp).LineStyle = xlNone

            If sbLeft = True Then

                With txls.Selection.Borders(xlEdgeLeft)
                    .LineStyle = xlContinuous
                    .Weight = xlThin
                    .ColorIndex = xlAutomatic
                End With

            End If


            If sbTop = True Then

                With txls.Selection.Borders(xlEdgeTop)
                    .LineStyle = xlContinuous
                    .Weight = xlThin
                    .ColorIndex = xlAutomatic
                End With

            End If


            If sbButtom = True Then

                With txls.Selection.Borders(xlEdgeBottom)
                    .LineStyle = xlContinuous
                    .Weight = xlThin
                    .ColorIndex = xlAutomatic
                End With

            End If


            If sbRight = True Then

                With txls.Selection.Borders(xlEdgeRight)
                    .LineStyle = xlContinuous
                    .Weight = xlThin
                    .ColorIndex = xlAutomatic
                End With

            End If


            '---- 수직 COL1개인경우제외
            If tCol1 <> tCol2 Then
                With txls.Selection.Borders(xlInsideVertical)
                    .LineStyle = xlContinuous
                    .Weight = xlThin
                    .ColorIndex = xlAutomatic
                End With
            End If

            '---- 수평 ROW1개인경우 제외
            If tRow1 <> tRow2 Then
                With txls.Selection.Borders(xlInsideHorizontal)
                    .LineStyle = xlContinuous
                    .Weight = xlThin
                    .ColorIndex = xlAutomatic
                End With
            End If

        End Sub


        ''' <summary>
        ''' 셀 -> 범위지정 보더표현(Col1,Row1,Col2,Row2)
        ''' </summary>
        ''' <param name="oXls"></param>
        ''' <param name="iCol1"></param>
        ''' <param name="iRow1"></param>
        ''' <param name="iCol2"></param>
        ''' <param name="iRow2"></param>
        ''' <param name="WLeft"></param>
        ''' <param name="WTop"></param>
        ''' <param name="Img"></param>
        ''' <remarks>셀 -> 범위지정 보더표현(Col1,Row1,Col2,Row2)</remarks>
        Public Sub xlsImage(ByVal oXls As Excel.Application, ByVal iCol1 As Long, ByVal iRow1 As Long, ByVal iCol2 As Long, ByVal iRow2 As Long, ByVal WLeft As Integer, ByVal WTop As Integer, ByVal Img As String)

            '---- 파일 존재 유무
            Dim sbFilePath As String
            sbFilePath = My.Application.Info.DirectoryPath & "\" & Img

            If Dir(sbFilePath) <> "" Then

                oXls.Range(oXls.Cells(iRow1, iCol1), oXls.Cells(iRow2, iCol2)).Select()

                '----- 그림삽입
                With oXls
                    .ActiveSheet.Pictures.Insert(My.Application.Info.DirectoryPath & "\" & Img).Select()
                    .Selection.ShapeRange.IncrementLeft(WLeft)
                    .Selection.ShapeRange.IncrementTop(WTop)
                End With

            End If

        End Sub

    End Class
#End Region






    '****************************************************************************************************
    '   함수명      :   SetGridTitle
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   그리드 타이틀 지정



    '   인수        :   
    '****************************************************************************************************
    Public Function SetGridTitle(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As String, ByVal pColNms As String, Optional ByVal pEdCols As String = "", Optional ByVal pViCols As String = "", Optional ByVal pAffCols As String = "", Optional ByVal pAlignCols As String = "") As Boolean

        Dim cols() As String
        Dim colNms() As String
        Dim affCols() As String
        Dim edCols() As String
        Dim viCols() As String
        Dim alignCols() As String
        Dim xSql As String = ""
        Dim i As Integer = 0

        Try

            SetGridTitle = False

            cols = Split(pCols, ",")
            colNms = Split(pColNms, ",")

            If (UBound(cols) - LBound(cols) <> UBound(colNms) - LBound(colNms)) Then
                Exit Function
            End If

            For i = 0 To UBound(cols)
                xSql = xSql & IIf(xSql = "", "", ",") & "'' as '" & cols(i) & "'"
            Next

            xSql = IIf(xSql = "", "", "Select ") & xSql

            If xSql <> "" Then
                Call oGrid.DataTable.ExecuteQuery(xSql)
                Call oGrid.DataTable.Rows.Remove(0)
            End If

            For i = LBound(cols) To UBound(cols)
                oGrid.Columns.Item(Trim(cols(i))).TitleObject.Caption = Trim(colNms(i))
            Next i

            If pEdCols.ToString.Trim.Length > 0 Then
                edCols = Split(pEdCols, ",")
                For i = LBound(edCols) To UBound(edCols)
                    oGrid.Columns.Item(edCols(i)).Editable = False
                Next i
            End If

            If pViCols.ToString.Trim.Length > 0 Then
                viCols = Split(pViCols, ",")
                For i = LBound(viCols) To UBound(viCols)
                    oGrid.Columns.Item(viCols(i)).Visible = False
                Next i
            End If

            If pAffCols.ToString.Trim.Length > 0 Then
                affCols = Split(pAffCols, ",")
                For i = LBound(affCols) To UBound(affCols)
                    oGrid.Columns.Item(affCols(i)).AffectsFormMode = False
                Next i
            End If

            If pAlignCols.ToString.Trim.Length > 0 Then
                alignCols = Split(pAlignCols, ",")
                For i = LBound(alignCols) To UBound(alignCols)
                    oGrid.Columns.Item(alignCols(i)).RightJustified = True
                Next i
            End If

            oGrid.AutoResizeColumns()

            SetGridTitle = True

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("SetGridTitle " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            cols = Nothing
            colNms = Nothing
            affCols = Nothing
            edCols = Nothing
            viCols = Nothing
            alignCols = Nothing

        End Try

    End Function
    '****************************************************************************************************
    '   함수명      :   BindGrid
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   그리드셋팅



    '   인수        :   
    '****************************************************************************************************
    Public Function BindGrid(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As String, ByVal pColNms As String, Optional ByVal pEdCols As String = "", Optional ByVal pViCols As String = "", Optional ByVal pAffCols As String = "", Optional ByVal pAlignCols As String = "") As Boolean
        BindGrid = False

        Dim cols() As String
        Dim colNms() As String
        Dim affCols() As String
        Dim edCols() As String
        Dim viCols() As String
        Dim alignCols() As String
        Dim i As Integer

        Try

            cols = Split(pCols, ",")
            colNms = Split(pColNms, ",")

            If (UBound(cols) - LBound(cols) <> UBound(colNms) - LBound(colNms)) Then
                Exit Function
            End If


            For i = LBound(cols) To UBound(cols)
                oGrid.Columns.Item(Trim(cols(i))).TitleObject.Caption = Trim(colNms(i))
            Next i

            If pEdCols.ToString.Trim.Length > 0 Then
                edCols = Split(pEdCols, ",")
                For i = LBound(edCols) To UBound(edCols)
                    oGrid.Columns.Item(edCols(i)).Editable = False
                Next i
            End If

            If pViCols.ToString.Trim.Length > 0 Then
                viCols = Split(pViCols, ",")
                For i = LBound(viCols) To UBound(viCols)
                    oGrid.Columns.Item(viCols(i)).Visible = False
                Next i
            End If

            If pAffCols.ToString.Trim.Length > 0 Then
                affCols = Split(pAffCols, ",")
                For i = LBound(affCols) To UBound(affCols)
                    oGrid.Columns.Item(affCols(i)).AffectsFormMode = False
                Next i
            End If

            If pAlignCols.ToString.Trim.Length > 0 Then
                alignCols = Split(pAlignCols, ",")
                For i = LBound(alignCols) To UBound(alignCols)
                    oGrid.Columns.Item(alignCols(i)).RightJustified = True
                Next i
            End If


            BindGrid = True

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("BindGrid " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            cols = Nothing
            colNms = Nothing
            affCols = Nothing
            edCols = Nothing
            viCols = Nothing
            alignCols = Nothing

        End Try

    End Function

    '****************************************************************************************************
    '   함수명      :   gfnSeGridCombo
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   그리드 콤보셋팅
    '   인수        :   
    '****************************************************************************************************
    Public Function gfnSeGridCombo(ByVal oColumn As SAPbouiCOM.ComboBoxColumn, ByVal strGroupCd As String, ByVal strSql As String) As Boolean

        Dim i As Integer
        Dim icnt As Integer
        Dim otempRS As SAPbobsCOM.Recordset
        Dim xSql As String

        gfnSeGridCombo = False

        Try

            otempRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)


            icnt = oColumn.ValidValues.Count

            If icnt > 0 Then
                For i = 0 To icnt - 1
                    oColumn.ValidValues.Remove(0, BoSearchKey.psk_Index)
                Next

            End If

            If strGroupCd <> "" Then
                'xSql = "select U_SMLCD, U_SMLNM from [@WJS_SAD011] where CODE = '" & strGroupCd & "' "
                xSql = ""
                otempRS.DoQuery(xSql)
            ElseIf strSql <> " Then" Then
                otempRS.DoQuery(strSql)
            Else
                B1Connections.theAppl.StatusBar.SetText("There is no paramiter ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                If Not otempRS Is Nothing Then otempRS = Nothing
                Exit Function
            End If


            If Not otempRS.EoF Then
                otempRS.MoveFirst()
                For i = 1 To otempRS.RecordCount

                    oColumn.ValidValues.Add(otempRS.Fields.Item(0).Value, otempRS.Fields.Item(1).Value)

                    otempRS.MoveNext()
                Next
            Else
                oColumn.ValidValues.Add("", "")
            End If

            If Not otempRS Is Nothing Then otempRS = Nothing

            gfnSeGridCombo = True

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("gfnSeGridCombo " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            otempRS = Nothing

        End Try

    End Function

    Public Sub GridCheckAll(ByRef oGrid As SAPbouiCOM.Grid, ByVal strColumnID As String)
        Dim i As Integer = 0
        Dim boolSelected As Boolean = False

        For i = 0 To oGrid.Rows.Count - 1
            If i = 0 Then
                If oGrid.DataTable.GetValue(strColumnID, 0) = "Y" Then
                    boolSelected = True
                Else
                    boolSelected = False
                End If
            End If
            If boolSelected Then
                oGrid.DataTable.SetValue(strColumnID, i, "N")
            Else
                oGrid.DataTable.SetValue(strColumnID, i, "Y")
            End If
        Next
    End Sub

    '****************************************************************************************************
    '   함수명      :   GetAutoNum
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   유저테이블의 맥스코드값 갖고오기
    '   인수        :   
    '****************************************************************************************************
    Public Function GetAutoNum(ByVal sTable As String) As String

        Dim oRs As SAPbobsCOM.Recordset
        Dim xSql As String = ""

        Try
            oRs = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSql = "SELECT ISNULL(MAX(CAST(CODE AS NUMERIC)),0) +1 AS CODE" & vbCrLf
            xSql = xSql & "FROM [@" & sTable & "]"
            oRs.DoQuery(xSql)

            If Not oRs.EoF Then
                oRs.MoveFirst()
                GetAutoNum = oRs.Fields.Item("CODE").Value
            Else
                GetAutoNum = ""
            End If

        Catch ex As Exception

            GetAutoNum = ""
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        End Try

    End Function

    '****************************************************************************************************
    '   함수명      :   SetCOMBOBPLID
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   사업장 콤보 셋팅
    '   인수        :   ComboObj- 콤보객체명



    '                   AllYN   - TRUE    : 전체추가 
    '                             FALSE   : 전체없음
    '                   UseYN   - TRUE    : 사용중인 사업장만
    '                             FALSE   : 모든 사업장



    '****************************************************************************************************
    Public Sub SetCOMBOBPLID(ByVal ComboObj As SAPbouiCOM.ComboBox, ByVal AllYN As Boolean, ByVal UseYN As Boolean)

        Dim oRS As SAPbobsCOM.Recordset
        Dim xSql As String
        Dim i As Integer

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            For i = 1 To ComboObj.ValidValues.Count
                ComboObj.ValidValues.Remove(0, BoSearchKey.psk_Index)
            Next

            If UseYN Then
                xSql = "SELECT BPLID, BPLNAME FROM OBPL WHERE DISABLED = N'N' ORDER BY BPLID"
            Else
                xSql = "SELECT BPLID, BPLNAME FROM OBPL ORDER BY BPLID"
            End If
            oRS.DoQuery(xSql)

            If AllYN Then
                ComboObj.ValidValues.Add("", "전체")
            End If

            'If Not oRS.EoF Then
            For i = 0 To oRS.RecordCount - 1
                ComboObj.ValidValues.Add(oRS.Fields.Item(0).Value.ToString, oRS.Fields.Item(1).Value.ToString)
                oRS.MoveNext()
            Next
            'End If

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing

        End Try

    End Sub


    Public Function GetDateSplit(Optional ByVal agDate As String = "") As String

        Dim oRS As SAPbobsCOM.Recordset

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            GetDateSplit = ""

            Call oRS.DoQuery("SELECT DateSep FROM OADM")

            If Not oRS.EoF Then

                If agDate <> "" Then
                    GetDateSplit = Replace(agDate, oRS.Fields.Item(0).Value, "")
                Else
                    GetDateSplit = oRS.Fields.Item(0).Value
                End If

            End If

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetDateSplit" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing
            GetDateSplit = ""

        End Try

    End Function

    Public Function GetChangeDate(ByVal agDate As String, Optional ByVal DateForm As Integer = 0) As String

        Dim dtInfo As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.ToString(), False).DateTimeFormat

        Dim v_DateSp_s As String
        Dim v_ChkDate_s As String
        Dim v_CurDate_s As String
        Dim v_RetDate_s As String

        Try

            If agDate <> "" Then

                '// 수정  GetDateSplit(agDate) 날짜값 넘겨주는거 삭제
                v_DateSp_s = GetDateSplit("")                           '일자의 구분값을 가져온다.
                v_ChkDate_s = Replace(agDate, v_DateSp_s, "")           '체크할 일자
                v_CurDate_s = Replace(GetDateFormat("", B1Connections.theAppl.Company.ServerDate), v_DateSp_s, "") '시스템 일자

                Select Case DateForm
                    Case 1
                        v_ChkDate_s = Mid(Mid(v_CurDate_s, 1, 4), 1, 8 - Len(v_ChkDate_s)) + v_ChkDate_s '년월일 포맷으로 완성

                    Case 2
                        If IsNumeric(agDate) Then                           '입력한 값이 13보다 작은 현재 년의 입력한 숫자를 월로 셋팅한다.
                            If CLng(agDate) < 13 Then
                                GetChangeDate = Mid(v_CurDate_s, 1, 4) & v_DateSp_s & Right(CStr(CLng(agDate) + 100), 2)

                                Exit Try
                            Else
                                If Len(agDate) = 6 Then
                                    v_ChkDate_s = v_ChkDate_s + "01"
                                Else
                                    v_ChkDate_s = v_CurDate_s
                                End If

                            End If
                        End If

                    Case 3
                        v_ChkDate_s = Mid(Mid(v_CurDate_s, 1, 4), 1, 4 - Len(v_ChkDate_s)) + v_ChkDate_s + "0101" '년




                End Select

                v_RetDate_s = GetCheckDate(v_ChkDate_s)

                Select Case DateForm
                    Case 1 : GetChangeDate = Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 1, 4) & v_DateSp_s & _
                                                 Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 5, 2) & v_DateSp_s & _
                                                 Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 7, 2)                  'YYYYMMDD
                    Case 2 : GetChangeDate = Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 1, 4) & v_DateSp_s & _
                                                 Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 5, 2)                  'YYYYMM
                    Case 3 : GetChangeDate = Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 1, 4)                  'YYYY
                End Select

            End If

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetChangeDate" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally
            GetChangeDate = ""
        End Try

    End Function


    Public Function GetCheckDate(ByVal agDate As String) As Integer

        Dim oRS As SAPbobsCOM.Recordset

        Try
            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            oRS.DoQuery("SELECT * FROM (SELECT ISDATE('" & agDate & "') a) as a")
            GetCheckDate = 0
            GetCheckDate = oRS.Fields.Item(0).Value

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetCheckDate" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing

        End Try

    End Function

    Public Function GetDateFormat(ByVal agDbDate As String, Optional ByVal agDate1 As String = "") As String

        Dim oRS As SAPbobsCOM.Recordset
        Dim AryField
        Dim v_DateFormat_s As String
        Dim v_DateSep_s As String
        Dim xSql As String

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            GetDateFormat = ""

            xSql = "SELECT DateFormat, DateSep FROM OADM"
            oRS.DoQuery(xSql)

            If Not oRS.EoF Then

                v_DateFormat_s = Trim(oRS.Fields.Item(0).Value)
                v_DateSep_s = Trim(oRS.Fields.Item(1).Value)

                GetDateFormat = ""
                If agDbDate <> "" Then                                    'DB에 DATE 값 넘겨줄때

                    AryField = Split(agDbDate, Trim(oRS.Fields.Item(1).Value))

                    Select Case v_DateFormat_s
                        Case Enum_Date.m_Ddmmyy : GetDateFormat = AryField(2) & "-" & AryField(1) & "-" & AryField(0)
                        Case Enum_Date.m_Ddmmccyy : GetDateFormat = AryField(2) & " - " & AryField(1) & " - " & AryField(0)
                        Case Enum_Date.m_Mmddyy : GetDateFormat = AryField(2) & " - " & AryField(0) & " - " & AryField(1)
                        Case Enum_Date.m_Mmddccyy : GetDateFormat = AryField(2) & " - " & AryField(0) & " - " & AryField(1)
                        Case Enum_Date.m_Yymmdd : GetDateFormat = AryField(0) & " - " & AryField(1) & " - " & AryField(2)
                        Case Enum_Date.m_Ccyymmdd : GetDateFormat = AryField(0) & " - " & AryField(1) & " - " & AryField(2)
                        Case Enum_Date.m_Ddmmyyyy : GetDateFormat = AryField(2) & "-" & AryField(1) & "-" & AryField(0)
                    End Select

                ElseIf agDbDate = "" And agDate1 <> "" Then             '리스트에 DATE값 뿌려줄때

                    If agDate1 Like "*-*" Then
                        AryField = Split(agDate1, "-")
                    Else
                        AryField = Split(agDate1, ".")
                    End If

                    'AryField = Split(agDate1, "-")
                    Select Case v_DateFormat_s
                        Case Enum_Date.m_Ddmmyy : GetDateFormat = AryField(2) & v_DateSep_s & AryField(1) & v_DateSep_s & Mid(AryField(0), 3, 2)
                        Case Enum_Date.m_Ddmmccyy : GetDateFormat = AryField(2) & v_DateSep_s & AryField(1) & v_DateSep_s & AryField(0)
                        Case Enum_Date.m_Mmddyy : GetDateFormat = AryField(1) & v_DateSep_s & AryField(2) & v_DateSep_s & Mid(AryField(0), 3, 2)
                        Case Enum_Date.m_Mmddccyy : GetDateFormat = AryField(2) & v_DateSep_s & AryField(0) & v_DateSep_s & AryField(1)
                        Case Enum_Date.m_Yymmdd : GetDateFormat = Mid(AryField(0), 3, 2) & v_DateSep_s & AryField(1) & v_DateSep_s & AryField(2)
                        Case Enum_Date.m_Ccyymmdd : GetDateFormat = AryField(0) & v_DateSep_s & AryField(1) & v_DateSep_s & AryField(2)
                        Case Enum_Date.m_Ddmmyyyy : GetDateFormat = AryField(2) & v_DateSep_s & AryField(1) & v_DateSep_s & AryField(0)
                    End Select

                End If

            End If

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetDateFormat" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing
            GetDateFormat = ""
        End Try

    End Function

    Public Function ChkYYYYMM(ByVal agDate As String, Optional ByVal DateForm As Integer = 1) As String
        Dim v_DateSp_s As String = GetDateSplit()
        Dim v_ChkDate_s As String = Replace(agDate, v_DateSp_s, "")           '체크할 일자
        Dim v_CurDate_s As String = Replace(GetDateFormat("", CFL.GetSystemDate), v_DateSp_s, "")
        Dim v_RetDate_s As String
        Dim v_RTNVAL As String = ""

        Try

            If agDate <> "" Then
                Select Case DateForm
                    Case 1
                        If IsNumeric(agDate) Then                           '입력한 값이 13보다 작은 현재 년의 입력한 숫자를 월로 셋팅한다.
                            If CLng(agDate) < 13 Then
                                Return (Mid(v_CurDate_s, 1, 4) & Right(CStr(CLng(agDate) + 100), 2))
                            Else
                                If Len(agDate) = 6 Then
                                    v_ChkDate_s = v_ChkDate_s + "01"
                                Else
                                    v_ChkDate_s = v_CurDate_s
                                End If

                            End If
                        End If

                    Case 2
                        v_ChkDate_s = Mid(Mid(v_CurDate_s, 1, 4), 1, 4 - Len(v_ChkDate_s)) + v_ChkDate_s + "0101" '년



                End Select

                v_RetDate_s = GetCheckDate(v_ChkDate_s)

                Select Case DateForm
                    Case 1 : v_RTNVAL = (Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 1, 4) & _
                                                 Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 5, 2))                  'YYYYMM
                    Case 2 : v_RTNVAL = (Mid(IIf(v_RetDate_s = 1, v_ChkDate_s, v_CurDate_s), 1, 4))                  'YYYY
                End Select
            End If
            Return v_RTNVAL
        Catch ex As Exception
            Return False
        End Try
    End Function



    Public Sub Mode_Change(ByVal Gubun As String, ByVal oForm As SAPbouiCOM.Form)

        Try
            Select Case Gubun
                Case "1282", "1287" ''추가  '복제
                    oForm.EnableMenu("1287", False) '복제
                    oForm.EnableMenu("1292", True) '행추가
                    oForm.EnableMenu("1293", True) '행삭제


                    oForm.EnableMenu("1281", True) '찾기
                    oForm.EnableMenu("1283", False) '제거
                    oForm.EnableMenu("1282", False) '추가
                Case "1281" ''찾기
                    oForm.EnableMenu("1287", False) '복제
                    oForm.EnableMenu("1292", False) '행추가
                    oForm.EnableMenu("1293", False) '행삭제


                    oForm.EnableMenu("1282", True) '추가
                    oForm.EnableMenu("1283", False) '제거
                    oForm.EnableMenu("1281", False) '찾기
                Case Else
                    oForm.EnableMenu("1287", True) '복제
                    oForm.EnableMenu("1292", True) '행추가
                    oForm.EnableMenu("1293", True) '행삭제


                    oForm.EnableMenu("1282", True) '추가
                    oForm.EnableMenu("1281", True) '찾기
                    oForm.EnableMenu("1283", True) '제거
            End Select


        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("Mode Change Error : " & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error)
        Finally

        End Try

    End Sub

    Public Function GetQD(ByVal strVal As String, ByVal strDefaultVal As String) As String

        If strVal.Trim = "" Then
            GetQD = strDefaultVal
        Else
            GetQD = "N'" & strVal.Replace("'", "''").Trim & "'"
        End If

    End Function

    '****************************************************************************************************
    '   함수명      :   거래처검색
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   거래처검색에 제한조건추가
    '   인수        :   sForm   - Form 객체
    '                   CFNM    - 해당 ChooseFromList 명
    '                   ITEMNM  - 값을 할당할 ITEM 명
    '****************************************************************************************************
    Public Function setCARDNM(ByVal sForm As SAPbouiCOM.Form, ByVal CFNM As String, ByVal ITEMNM As String) As Boolean


        Dim oCFL As SAPbouiCOM.ChooseFromList
        Dim oCons As SAPbouiCOM.Conditions
        Dim oCon As SAPbouiCOM.Condition

        Dim oRS As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim xSQL As String

        Try
            xSQL = "SELECT COUNT(CardCode) FROM OCRD " & vbCrLf
            xSQL = xSQL & " WHERE CardType='S'" & vbCrLf
            xSQL = xSQL & " AND CardCode Like '%B%' " & vbCrLf
            xSQL = xSQL & " AND frozenFor='N' " & vbCrLf
            xSQL = xSQL & " AND CardName Like '%" & Replace(sForm.Items.Item(ITEMNM).Specific.Value, "*", "%") & "%'" & vbCrLf


            oRS.DoQuery(xSQL)

            If oRS.Fields.Item(0).Value > 300 Then
                If B1Connections.theAppl.MessageBox(oRS.Fields.Item(0).Value & " 건 거래처가 검색되었습니다.많은거래처로 검색시 오래걸릴수 있습니다.검색하시겠습니까?", 2, "예", "아니요") = 2 Then
                    Return False
                    Exit Try
                End If
            End If

            oCFL = sForm.ChooseFromLists.Item(CFNM)
            oCons = B1Connections.theAppl.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_Conditions)

            oCon = oCons.Add()
            oCon.Alias = "CardType"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "S"

            oCon.Relationship = BoConditionRelationship.cr_AND

            oCon = oCons.Add()
            oCon.Alias = "CardCode"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_CONTAIN
            oCon.CondVal = "B"

            oCon.Relationship = BoConditionRelationship.cr_AND

            oCon = oCons.Add()
            oCon.Alias = "frozenFor"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = "N"


            oCon.Relationship = BoConditionRelationship.cr_AND

            oCon = oCons.Add()

            oCon.Alias = "CardName"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_CONTAIN
            sForm.Items.Item(ITEMNM).Specific.Value = IIf(Left(sForm.Items.Item(ITEMNM).Specific.Value, 1) = "*", sForm.Items.Item(ITEMNM).Specific.Value, "*" + sForm.Items.Item(ITEMNM).Specific.Value)
            oCon.CondVal = sForm.Items.Item(ITEMNM).Specific.Value
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_START
            oCFL.SetConditions(oCons)

            Return True
        Catch ex As Exception
            oCFL = Nothing
            oCons = Nothing
            oCon = Nothing
            oRS = Nothing

            Return False

        Finally

            oCFL = Nothing
            oCons = Nothing
            oCon = Nothing
            oRS = Nothing


        End Try

    End Function
    Public Function chkCARDNM(ByVal CARDNM As String) As Boolean

        Try
            If Trim(CARDNM) = "" Then
                B1Connections.theAppl.StatusBar.SetText("한글자 이상 명칭을 입력해 주세요.", SAPbouiCOM.BoMessageTime.bmt_Short)
                Return False
                Exit Try
            End If


            Return True
        Catch ex As Exception
            Return False

        Finally


        End Try



    End Function

End Module
