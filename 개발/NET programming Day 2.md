# .Net Programming Day 2

## SSMS 2017

### NOTI(Trigger)

> 마우스 우클릭 - [필터] "Noti"해서 찾으면 됨.

- EDF_TransactionNotification
  - 새로 추가되서 모름
- PostTransactionNotice
  - 간간히 씀
- TransactionNotification
  - 이것만 씀.
  - 유저 트랜젝션 삽입 부분이 따로 있고, 특정 조건 예를 들어, 비고란이 null이면 저장 못하게 하는 간단한 기능 수행 가능.

### SBO 연결

```vbscript
Public vCompany As SAPbobsCOM.Company
'-----------------------------------------------------'
 vCompany = New SAPbobsCOM.Company
        vCompany.Server = "77106152-PC\MSSQLSERVER_2017"
        vCompany.CompanyDB = "SBODemoKR"
        vCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017
        vCompany.UserName = "manager"
        vCompany.Password = "manager"
        Dim nResult As Long
        Dim strErrString As String
        nResult = vCompany.Connect
        MsgBox("result is " + Str(nResult))
        Call vCompany.GetLastError(nResult, strErrString)
        MsgBox("getlastError(" + Str(nResult) + ", " + strErrString + ")")
```



### SQL

```vbscript
Dim Count As Long
        Dim FldName As String
        Dim Fldval As String
        Dim i As Integer
        Dim RecSet As SAPbobsCOM.Recordset
        Dim fieldName As String = ""
        Dim fieldVal As String = ""
        RecSet = vCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        RecSet.DoQuery("Select * from OADM")
        Count = RecSet.Fields.Count
        While RecSet.EoF = False
            For i = 0 To Count - 1
                FldName = RecSet.Fields.Item(i).Name
                Fldval = RecSet.Fields.Item(i).Value.ToString()
                fieldName += FldName
                fieldVal += Fldval
            Next i
            RecSet.MoveNext()
        End While
        Debug.Print(fieldName + vbLf + fieldVal)
```



### 프로시저 실행

```vbscript
 Dim RecSet As SAPbobsCOM.Recordset
        Dim cardname As String = "C40000"
        Dim count As Long
        Dim FldName As String
        Dim Fldval As String
        Dim i As Integer
        Dim result As String = ""
        RecSet = vCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        RecSet.DoQuery("Exec dbo.test" & "'" & cardname & "'")
        Count = RecSet.Fields.Count
        While RecSet.EoF = False
            For i = 0 To Count - 1
                FldName = RecSet.Fields.Item(i).Name
                Fldval = RecSet.Fields.Item(i).Value.ToString()
                result += FldName + "/" + Fldval
            Next i
            RecSet.MoveNext()
        End While
        Debug.Print(result)
    End Sub
```



### 분개

분개의 키는 batchnum, transid 2가지

> Tip
>
> vb.net 파일의 참조에서 api 추가 가능함.
>
> e.g. SBO DI API 10.0 추가
>
> 안드로이드의 로그 출력 처럼 vb.net에서 사용 가능.

```vbscript
debug.print("debug log")	
```



