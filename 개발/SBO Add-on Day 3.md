## Data binding

DataSource()

화면에 있는 값을 가져오는 것이 아니라, 데이터가 저장된 곳에서 직접 데이터를 가져옴.

### DataSource() 사용하는 이유

1. 데이터 규격을 통일되게 관리하고자 하는 경우  e.g. 국가 별 날짜 년/월/일 순서 상이함.
2. 불필요한 이벤트가 실행되지 않기 위하여 for 퍼포먼스  e.g. validate() 이벤트

```vbscript
'1. 폼에 직접, 이벤트를 탐.'
oForm.Items.Item("edtDATEF").Specific.string = "01"
'2. 데이타 소스에서 값 가져옴. 이벤트 안탐'
```



### ChooseFromList

객체 형식만 됨. + UDO = 링크드 버튼이 적용되는 것

알리아스로 자체적으로 데이터 정합성 검증을 함.

```vbscript
oForm.Items.Item("edtACCTCDF").Specific.ChooseFromListAlias = "AcctCode"
```



### Formatted Search

쿼리로 함.

질의 생성 후 Shift + Alt + F2 눌러서 입력 텍스트에 쿼리 적용



### 이벤트 처리

```vbscript
 <B1Listener(BoEventTypes.et_CHOOSE_FROM_LIST, False, ActionType.Itm, "edtACCTCDF")>
' Event type / isBefore / ActionType / ITM ID'
```

로 시작



CREATE PROC [dbo].[WJS_SP_FIF0010F]

프로시저 추가





