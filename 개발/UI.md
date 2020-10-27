## 폼 추가

---

### SRF

페인터로 폼 작성

Element UID 작성

Form UID, Type 작성

프로젝트에 [Form_UID].srf파일 추가

### Menu

WJS - bin-MNU-[PROJECT 이니셜]-[모듈].xml

SBO - 모듈에서 메뉴 UID 알아냄.

xml에서 폼 추가

```xml
<Menu Checked="0" Enabled="1" Position="3" FatherUID="1536" String="계정명 갱신*" Type="1" UniqueID="WJS_FIB0110_LMS"/>
```

### VB파일 작성

WJS_[Project 이니셜]-[모듈]-[Form_UID.vb] 파일 작성

추가할 Form으로 아래 목록 변경 

1. 파일명
2. 클래스명
3. MyBase.New("Form_type", "Form_UID", True)

🚨  SBO 재시작해서 메뉴에 추가 됐는지 확인할 것.



## VB 프로그래밍 - 기본

### New

```vbscript
MyBase.New("Form_type", "Form_UID", True)
```

### FormInit

```vbscript
Call DataBinded()
Call Form_Default()
Call AddChooseFromList()
'...
```

### DataBinded

1. DataSource에 추가

   - DBDataSource - DB table의 Field로 부터 값 가져옴.
   - UserDataSource - 엘레머트 UID
   - DataTable - Grid 용

   ```vbscript
   oForm.DataSources.UserDataSources.Add("UID", BoDataType.dt_SHORT_TEXT)
   ```

2. 객체 할당

```vbscript
oEditText = oForm.items.item("UID").specific
```



3. DataBind(Form.Item과 DataSource)

```vbscript
setBound(isBound, [TableName], [Alias or UID])
Call oEditText.DataBind.SetBound(True, "", "UID")
'2번 생략하고 1, 3번으로 보통함.
Call oForm.Items.Item("UID").Specific.DataBind.setBound(True, "", "srcUID")
```



### Form_Default

바인드된 엘레먼트들의 디폴트값 세팅 2가지 방법

1. DataSource 이용
2. 폼 엘레먼트에 직접 넣음

둘의 차이는 이벤트가 적용되냐 안되냐의 차이

전자는 안 탐.

### ChooseFromList

1. ChooseFromLIst의 속성 정의 (조건값, 오브젝트 타입 등)
2. Form의 ChooseFromList 목록에 추가
3. et_ChooseFromLIst 이벤트 추가

```vbscript
Dim oCFL As SAPbouiCOM.ChooseFromList
        Dim oCFLCreationParams As SAPbouiCOM.ChooseFromListCreationParams
        Dim oCons As SAPbouiCOM.Conditions
        Dim oCon As SAPbouiCOM.Condition
'1번 2번
oCFLCreationParams = B1Connections.theAppl.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)

            oCFLCreationParams.ObjectType = BoLinkedObject.lf_GLAccounts
            oCFLCreationParams.UniqueID = "edtACCFCD"

            oCFL = oForm.ChooseFromLists.Add(oCFLCreationParams)
            'where 조건 'Forms - CFL/CFLParams - Cons - Con
            oCons = oCFL.GetConditions()

            oCon = oCons.Add()
            'Alias is Column and case sensitive
            oCon.Alias = "GroupMask"
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL
            oCon.CondVal = 5

            oCFL.SetConditions(oCons)
'3번
<B1Listener(BoEventTypes.et_CHOOSE_FROM_LIST, False, ActionType.Itm, "UID")>
Public Overridable Sub ET_UID_AFChooseFromList(ByVal pVal As ItemEvent)
        oForm = B1Connections.theAppl.Forms.Item(pVal.FormUID)
    oEditText = CType(CType(oForm.Items.Item("UID"), SAPbouiCOM.Item).Specific, EditText)
        'ADD YOUR ACTION CODE HERE ...
        Dim oDataTable As SAPbouiCOM.DataTable              '데이터를 가져오기 위해 데이터테이블 추가
        Try
            '데이터 테이블을 ChooseFromList에 선택된 오브젝트에서 가져오도록 세팅
            oDataTable = pVal.SelectedObjects

            '사용자가 ChooseFromList의 취소 버튼을 눌렀을 경우 에러처리(데이터 테이블의 값으로 체크)
            If Not oDataTable Is Nothing Then
        oForm.DataSources.UserDataSources.Item("UID").ValueEx = oDataTable.GetValue("col_key", 0).ToString
        oForm.DataSources.UserDataSources.Item("UID").ValueEx = oDataTable.GetValue("col_name", 0).ToString
            End If
        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("ET_edtSALEPCD_AFChooseFromList " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oDataTable = Nothing
        End Try

    End Sub
```

#### 

## 컨트롤 세팅

### 옵션 버튼(라디오 버튼)

```vbscript
'DataBinded Sub
Dim oOptBtn As SAPbouiCOM.OptionBtn = Nothing

Call Form.Items.Item("UID").Specific.DataBind.setBound(True, "", "srcUID")
oOptBtn = oForm.Items.Item("UID").Specific
oOptBtn.GroupWith("srcUID")
oOptBtn = oForm.Items.Item("UID").Specific
oOptBtn.GroupWith("srcUID")
'Form_Default Sub
'Form Default - Form Item
oForm.Items.Item("UID").Specific.selected = True
'Form Defualt - DataSource'
oForm.DataSources.UserDataSources.Item("srcUID").Value = "2"
```

### 체크 박스 - Default

```vbscript
'Form Defualt - Form Item
oForm.Items.Item("UID").Specific.checked = True
'Form Defualt - DataSource'
oForm.DataSources.UserDataSources.Item("srcUID").Value = "Y"
```

### 콤보 박스 - Default

```vbscript
Dim oComboBox As SAPbouiCOM.ComboBox = Nothing
oComboBox = oForm.Items.Item("UID").Specific
       
xsql = "SELECT ItmsGRPCod, itmsgrpnam from oitb"
oRs.DoQuery(xsql)

oComboBox.ValidValues.Add("", "전체")

For i = 0 To oRs.RecordCount - 1
    oComboBox.ValidValues.Add(oRs.Fields.Item("col").Value, 	 oRs.Fields.Item("col").Value)
    oRs.MoveNext()
Next
'Form Defualt - Form DataSource
oForm.DataSources.UserDataSources.Item("srcUID").ValueEx = oComboBox.ValidValues.Item(2).Description
'Form Defualt - Form Item
oComboBox.Select(2, BoSearchKey.psk_Index)
```



### XML(.SRF) 파일에서 폼 바인딩 함.

<datasource> 태그 안에 Table 선언하고.

element  UID 선언함.



### UDO

Form mode 지정

### Matrix

UDO에서는 Grid 안 쓰고 Matix를 씀.

celll 마다 binding/mapping이 되어 있어 추가/갱신/삭제/라인 조작에 용이함. 

cf. grid는 조회용. 데이터 조작용이 아님.



- 스크린 페인터 미리보기로 에러메시지도 확인

- XML 파일에서 수정하기 스크린페인터 오류 많이남

- UID 및 폼 다 그리고, 바인딩은 나중에

- vb파일 보다 srf파일에서 바인딩하는게 퍼모먼스에 더 좋음.

  - ```
    <userdatasource>
    	<dataosurce uid="edtOOO", type="", size=""/>
    </userdatasource>
    
    <databind databound="1" table="" alias=""/>
    
    ```

    TYPES

    - 9 문자
    - 10 날짜
    - 2수량
    - 3단가
    - 6금액(SUM)