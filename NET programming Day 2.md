# .Net Programming Day 2

### NOTI(Trigger)

- EDF_TransactionNotification
  - 새로 추가되서 모름
- PostTransactionNotice
  - 간간히 씀
- TransactionNotification
  - 이것만 씀.

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

