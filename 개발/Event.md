## Event

----

[TOC]

### ChooseFromList

[UI문서 참조](C:\Users\77106152\Documents\GitHub\2020WJF4\개발\UI.md)



### Validate

엘리먼트 값의 변경이 일어나면 감지

사용 예 : 보통 Formatted Search할 때, 

```vbscript
<B1Listener(BoEventTypes.et_VALIDATE, False, ActionType.Itm, "UID")>
Public Overridable Sub ET_UID_AFValidate(ByVal pVal As ItemEvent)
        oForm = B1Connections.theAppl.Forms.Item(pVal.FormUID)
    oEditText = CType(CType(oForm.Items.Item("UID"), SAPbouiCOM.Item).Specific, EditText)
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim xSQL As String = "" 
    	'ADD YOUR ACTION CODE HERE ...
        xSQL = "SELECT ACCTNAME FROM Oact WHERE AcctCODE = '" & oEditText.String & "'"
        oRs.DoQuery(xSQL)
        If pVal.ItemChanged Then
        oEditText = CType(CType(oForm.Items.Item("UID"), SAPbouiCOM.Item).Specific, EditText)
        oEditText.String = oRs.Fields.Item("col").Value
        End If                
    End Sub
```

### ItemPressed

```vbscript
<B1Listener(BoEventTypes.et_ITEM_PRESSED, False, ActionType.Itm, "grd")>
    Public Overridable Sub ET_grd_AFItemPressed(ByVal pVal As ItemEvent)
        oForm = B1Connections.theAppl.Forms.Item(pVal.FormUID)
        oGrid = CType(CType(oForm.Items.Item("grd"), SAPbouiCOM.Item).Specific, Grid)     
        'ADD YOUR ACTION CODE HERE ...
   		If pVal.Row < 0 Then Exit Sub

        oGrid.Rows.SelectedRows.Add(pVal.Row)
    End Sub
```

### FormResize

```vbscript
<B1Listener(BoEventTypes.et_FORM_RESIZE, False, ActionType.Frm)>
    Public Overridable Sub ET_onFormResize(ByVal pVal As ItemEvent)
        oForm = B1Connections.theAppl.Forms.Item(pVal.FormUID)
        oGrid = oForm.Items.Item("grd").Specific
        'ADD YOUR ACTION CODE HERE ...
        If oGrid.DataTable Is Nothing Then
            oGrid = Nothing
            Exit Sub
        End If
        oGrid.AutoResizeColumns()
        oGrid = Nothing
    End Sub
```

## 로직

### 조회 로직

```vbscript
'Event
<B1Listener(BoEventTypes.et_ITEM_PRESSED, False, ActionType.Itm, "UID")>
    Public Overridable Sub et_btnFIND_AFItemPressed(ByVal pval As ItemEvent)
        oForm = B1Connections.theAppl.Forms.Item(pval.FormUID)
    	oButton = CType(CType(oForm.Items.Item("UID"), SAPbouiCOM.Item).Specific, Button)

        Dim xSQL As String = ""
    xSQL = xSQL & "EXEC WJS_SP_name '" & oForm.DataSources.UserDataSources.Item("UID1").Value.Trim & "',"
    xSQL = xSQL & "'" & oForm.DataSources.UserDataSources.Item("UID4").ValueEx.Trim & "'"
        find_grdPRC(oForm, xSQL)
    End Sub
'SUB
Private Sub find_grdPRC(ByVal oForm As SAPbouiCOM.Form, ByVal xSQL As String)
        Dim ogrid As SAPbouiCOM.Grid = Nothing
        Dim i As Integer = 0
        oForm.Freeze(True)
        Try
            ogrid = oForm.Items.Item("grd").Specific
            ogrid.DataTable.Clear()
            ogrid.DataTable.ExecuteQuery(xSQL)
            '선택된 Row 하나 색상 반전 표시
            ogrid.SelectionMode = BoMatrixSelect.ms_Single
    		'활성 설정
            ogrid.Columns.Item("col").Editable = False
   			'TYPE 지정'
            ogrid.Columns.Item("col").Type = BoGridColumnType.gct_CheckBox
    		'캡션
            ogrid.Columns.Item("col").TitleObject.Caption = "등록일"
    		'정렬
            ogrid.Columns.Item("col").RightJustified = True
		    '링크드 오브젝트
            ogrid.Columns.Item("col").LinkedObjectType = BoLinkedObject.lf_BusinessPartner
		    '배경 색'
            ogrid.Columns.Item("U_BPCODE").BackColor = &HCFE3F4
		    '고정 칼럽'
            ogrid.CommonSetting.FixedColumnsCount = 4
		    '로우 갯수 PK가 있으면 입력 대기 로우 1 추가
            If ogrid.DataTable.Rows.Count >= 1 Then
       			If ogrid.DataTable.GetValue("PK_col", 0) <> "" Then
                	ogrid.DataTable.Rows.Add(1)
                End If
            End If

            ogrid.AutoResizeColumns()

            'oProgBar.Value = 10
            'oProgBar.Stop()

        Catch ex As Exception
        Finally
            oForm.Freeze(False)
        End Try
    End Sub
```



### 저장 로직

- BF Input Field 체크
- AF 저장
  - Query 이용 비추
  - DI API 이용 정석



### 삭제 로직

- BF Input 체크
- AF 삭제