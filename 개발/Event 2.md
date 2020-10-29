## XML

- option group 지정

  <item> 태그 내에 action="group" 사용

- grid

  - 스크린 페인터 이용하면 편함.



### UDO

VB 폼 모드 설정

BrowserBy

폼 모드에 따라 editText 활성화 설정

### Form_Data_NNN 이벤트

- Noti를 대신 해서 Input 값 체크를 수행함. 따라서 BF를 주로 사용함.



## NOTI

실행 순서

1. Form_Data_NNNN
2. NOTI



### CORE 수정

품목마스터에 버튼 붙이기

추가한 버튼 폼 모드에 따라서 활성/비활성 설정

코어 화면

Core는 DBSource로 setvalue 불가능

따라서, Core수정하려면 폼 엘레먼트로 하는데 이벤트 타서 느려짐

콤보 버튼 추가 폼UID가 2인 취소 버튼을 기준으로 추가

```vbscript
Dim oBtnCombo As SAPbouiCOM.ButtonCombo = Nothing

        oForm.Items.Add("btnPRINT", BoFormItemTypes.it_BUTTON_COMBO)
        oForm.Items.Item("btnPRINT").Top = oForm.Items.Item("2").Top
        oForm.Items.Item("btnPRINT").Left = oForm.Items.Item("2").Left + oForm.Items.Item("2").Width + 5
        oForm.Items.Item("btnPRINT").Width = "100"
        oForm.Items.Item("btnPRINT").Height = oForm.Items.Item("2").Height

        oBtnCombo = oForm.Items.Item("btnPRINT").Specific
        oBtnCombo.ValidValues.Add("1", "콤보버튼1")
        oBtnCombo.ValidValues.Add("2", "콤보버튼2")

        oBtnCombo.Select(0, BoSearchKey.psk_Index)
        oForm.Items.Item("btnPRINT").DisplayDesc = True
```

### Form 셋팅

```vbscript
MyBase("FormType", ["FormUID"], False)

'FormType e.g. 품목마스터 150 시스템 정보로 알 수 있음
```



### FormLoad 안함

```vbscript
Public Overridable Function ET_BFMenuClick(ByVal pVal As MenuEvent) As Boolean
   'GENERATED CODE
   'ADD YOUR ACTION CODE HERE ...
   'Return Me.LoadForm
   Return True
End Function
```

