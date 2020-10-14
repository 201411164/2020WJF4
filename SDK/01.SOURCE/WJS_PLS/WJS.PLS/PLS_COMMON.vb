Imports AddOnBase
Imports SAPbobsCOM
Imports SAPbouiCOM
Imports System.Runtime.InteropServices
Imports WJS.COMM
Imports System.Xml
Imports System.Data.OleDb
Imports System.Data
Imports System.Net
Imports System.IO

Module PLS_COMMON

    Public Const ITMGRP_S01 As String = "100"    '상품
    Public Const ITMGRP_S02 As String = "101"    '제품
    Public Const ITMGRP_S03 As String = "102"    '반제품
    Public Const ITMGRP_S04 As String = "103"    '원재료
    Public Const ITMGRP_S05 As String = "104"    '부재료
    Public Const ITMGRP_S06 As String = "105"    '저장품

    Public GV_EMPUSEYN_S As String = ""
#Region "WJS_EXCEL_TO_XML"


    ''' 엑셀내역을 조회하여 XML로 변환한다.
    ''' </summary>
    ''' <param name="txls"></param>
    ''' <remarks>엑셀에 있는 각 시트의 내역을 XML로 변환하여 배열로 리턴한다.</remarks>
    Public Function GetAllXML(ByVal strFileName As String, ByVal blnHeader As Boolean, ByVal iTableIndex As Integer, _
                              ByRef strTableName As String, Optional ByVal sFIELDNM As String = "", _
    Optional ByVal sNOTNULLFIELDNM As String = "")

        Dim dsExcelData As DataSet
        Dim strComand As String
        Dim i As Integer
        Dim nullCnt As Integer = 0
        Dim strSheetTableName() As String
        Dim strSheetName As String
        'Dim ExcelCon As String = "Provider=Microsoft.Jet.OLEDB.4.0;"
        Dim ExcelCon As String ' = "Provider=Microsoft.ACE.OLEDB.12.0;"
        Dim strConnectionString As String
        Dim oleCon As System.Data.OleDb.OleDbConnection
        Dim oleAdapter As System.Data.OleDb.OleDbDataAdapter
        Dim dts As System.Data.DataTable

        Dim strXml() As String

        Try

            '쓰기에 IMEX = 0, 읽기 전용에 IMEX = 1, 수정 / 업데이트에 IMEX = 2 
            '"HDR = 예;" 첫 번째 행에 데이터가 아닌 열 이름
            '엑셀 CONNECT (MICROSOFT.JET.OLEDB) 활용 
            '파일경로, 헤더데이터여부를 넘김다.
            If blnHeader Then
                ExcelCon = "Provider=Microsoft.ACE.OLEDB.12.0;"
                'strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 8.0;HDR=No;IMEX=1;" + Convert.ToChar(34).ToString()
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 12.0;HDR=Yes;IMEX=1;" + Convert.ToChar(34).ToString()
            Else
                ExcelCon = "Provider=Microsoft.ACE.OLEDB.12.0;"
                'strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 8.0;HDR=No;IMEX=1;" + Convert.ToChar(34).ToString()
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 12.0;HDR=No;IMEX=1;" + Convert.ToChar(34).ToString()
            End If

            '엑셀 oledb객체생성
            oleCon = New OleDbConnection()
            oleCon.ConnectionString = strConnectionString

            '엑셀 oledb객체 연결
            oleCon.Open()

            '엑셀시트스키가 가져오기
            dts = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)

            '시트 갯수만큼 시트명을 가져온다.
            ReDim strSheetTableName(dts.Rows.Count() - 1)

            For i = 1 To dts.Rows.Count()

                strSheetName = dts.Rows(i - 1).Item("TABLE_NAME").ToString()

                If Right(strSheetName, 1) <> "_" Then

                    strSheetTableName(i - 1) = strSheetName.Substring(0, strSheetName.Length() - 1)

                End If

                If ((i - 1) = iTableIndex) Then
                    strTableName = strSheetTableName(i - 1)
                End If


            Next

            '배열초기화 ( 시트의 갯수만큼 배열초기화)
            ReDim strXml(strSheetTableName.Length() - 1)

            '데이터테이블 초기화
            dts.Clear()

            '시트의 갯수만큼 루핑으로 XML변환
            For i = 1 To strSheetTableName.Length()

                If Not strSheetTableName(i - 1) Is Nothing And (i - 1) = iTableIndex Then

                    '' 엑셀의 시트내역을 데이터베이스처럼 인식하여 시트명이 테이블네임이 된다
                    '' 헤더정보를 가져올경우 첫번째 라인이 컬럼이 된다.
                    '' 헤더정보를 안가져올경우 컬럼은 순차적으로 F1, F2, F3 ~~~~~ 이런식으로 변환된다.
                    '' 한글과 영어는 인식하지만 숫자와 부호는 _x0026_ 이런식으로 16진수로 변환되서 나온다.
                    If sFIELDNM = "" Then
                        strComand = "select * from [" + strSheetTableName(i - 1) + "$]"
                    Else
                        strComand = "select " & sFIELDNM & " from [" + strSheetTableName(i - 1) + "$]"
                    End If
                    If sNOTNULLFIELDNM <> "" Then
                        strComand = strComand & " WHERE " & sNOTNULLFIELDNM & " IS NOT NULL "
                    End If

                    '엑셀연결
                    oleAdapter = New OleDbDataAdapter(strComand, oleCon)

                    '데이터테이블 설정(xml로 변환하기 위하여 데이터테이블에 저장)
                    dts = New System.Data.DataTable(strSheetTableName(i - 1))

                    '데이터테이블에 엑셀내용 저장
                    oleAdapter.FillSchema(dts, SchemaType.Source)
                    oleAdapter.Fill(dts)

                    '각 시트별로 담기위하여 데이터셋 설정
                    dsExcelData = New DataSet()
                    dsExcelData.Tables.Add(dts)


                    '시트이 데이터가 있는경우만 xml을 배열에 담는다.
                    If dts.Rows.Count() > 0 Then

                        '데이터셋에서 xml 추출
                        strXml(i - 1) = dsExcelData.GetXml()
                        nullCnt = nullCnt + 1

                    End If

                    '연결내역 제거
                    dts.Clear()
                    oleAdapter.Dispose()
                    dsExcelData.Dispose()

                End If

            Next

            '빈 배열값을 제거
            ReDim Preserve strXml(nullCnt - 1)

            Return strXml


        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message)
            Return Nothing
        Finally

            oleCon.Close()

            dts = Nothing
            oleAdapter = Nothing
            oleCon = Nothing
            dsExcelData = Nothing
        End Try

    End Function

    ''' 엑셀내역을 조회하여 DataTable로 변환한다.
    ''' </summary>
    ''' <param name="txls"></param>
    ''' <remarks>엑셀에 있는 각 시트의 내역을 XML로 변환하여 배열로 리턴한다.</remarks>
    Public Function GetAllDt_Sheet(ByVal strFileName As String, ByVal strSheetName As String, ByVal blnHeader As Boolean) As System.Data.DataTable

        Dim dsExcelData As DataSet
        Dim strComand As String
        Dim i As Integer
        Dim nullCnt As Integer = 0
        Dim strSheetTableName() As String

        Dim strConnectionString As String
        Dim oleCon As System.Data.OleDb.OleDbConnection
        Dim oleAdapter As System.Data.OleDb.OleDbDataAdapter
        Dim enumerator As System.Data.OleDb.OleDbEnumerator
        Dim ProviderList As ArrayList = New ArrayList()
        Dim iCnt As Integer
        Dim dts As System.Data.DataTable
        Dim ExcelCon As String = "Provider="
        Dim cv_OLEDB_s As String = ""
        Dim cv_OLEVR_d As Double
        Dim cv_ExcelVR_d As String = ""
        Dim strXml As String

        Try
            enumerator = New OleDbEnumerator()
            '엑셀 Provider 여부에 따라 접속방식변경
            dts = enumerator.GetElements
            For iCnt = 1 To dts.Rows.Count()

                If dts.Rows(iCnt - 1).Item(0).ToString.IndexOf("Microsoft.ACE.OLEDB.") >= 0 Or dts.Rows(iCnt - 1).Item(0).ToString.IndexOf("Microsoft.Jet.OLEDB.") >= 0 Then
                    ProviderList.Add(dts.Rows(iCnt - 1).Item(0).ToString)
                End If
            Next

            ProviderList.Sort()

            cv_OLEVR_d = 0


            For iCnt = 1 To ProviderList.Count

                If ProviderList(iCnt - 1).ToString.IndexOf("Microsoft.ACE.OLEDB.") >= 0 Then

                    If cv_OLEVR_d < Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.ACE.OLEDB.", "")) Then
                        cv_OLEDB_s = ProviderList(iCnt - 1).ToString
                        cv_OLEVR_d = Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.ACE.OLEDB.", ""))
                    End If
                End If

            Next

            cv_OLEVR_d = 0

            If cv_OLEDB_s = "" Then
                For iCnt = 1 To ProviderList.Count

                    If ProviderList(iCnt - 1).ToString.IndexOf("Microsoft.Jet.OLEDB.") >= 0 Then
                        If cv_OLEVR_d < Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.Jet.OLEDB.", "")) Then
                            cv_OLEDB_s = ProviderList(iCnt - 1).ToString
                            cv_OLEVR_d = Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.Jet.OLEDB.", ""))
                        End If
                    End If
                Next
            End If


            ExcelCon = ExcelCon & cv_OLEDB_s & ";"

            'If cv_OLEDB_s.IndexOf("Microsoft.ACE.OLEDB.15") >= 0 Then
            '    cv_ExcelVR_d = "Excel 15.0;"
            'Else
            cv_ExcelVR_d = "Excel 8.0;"
            'End If

            dts.Clear()

            '엑셀 CONNECT (MICROSOFT.JET.OLEDB) 활용 
            '파일경로, 헤더데이터여부를 넘김다.
            If blnHeader Then
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + cv_ExcelVR_d + "HDR=Yes;IMEX=1;" + Convert.ToChar(34).ToString()
            Else
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + cv_ExcelVR_d + "HDR=No;IMEX=1;" + Convert.ToChar(34).ToString()
            End If

            '엑셀 oledb객체생성
            oleCon = New OleDbConnection()
            oleCon.ConnectionString = strConnectionString

            '엑셀 oledb객체 연결
            oleCon.Open()

            '엑셀시트스키가 가져오기
            dts = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)

            '데이터테이블 초기화
            dts.Clear()



            '' 엑셀의 시트내역을 데이터베이스처럼 인식하여 시트명이 테이블네임이 된다
            '' 헤더정보를 가져올경우 첫번째 라인이 컬럼이 된다.
            '' 헤더정보를 안가져올경우 컬럼은 순차적으로 F1, F2, F3 ~~~~~ 이런식으로 변환된다.
            '' 한글과 영어는 인식하지만 숫자와 부호는 _x0026_ 이런식으로 16진수로 변환되서 나온다.
            strComand = "select * from [" + strSheetName + "$]"

            '엑셀연결
            oleAdapter = New OleDbDataAdapter(strComand, oleCon)

            '데이터테이블 설정(xml로 변환하기 위하여 데이터테이블에 저장)
            dts = New System.Data.DataTable(strSheetName)

            '데이터테이블에 엑셀내용 저장
            oleAdapter.FillSchema(dts, SchemaType.Source)
            oleAdapter.Fill(dts)

            '각 시트별로 담기위하여 데이터셋 설정
            'dsExcelData = New DataSet()
            'dsExcelData.Tables.Add(dts) 

            Return dts


        Catch ex As Exception
            Return Nothing
        Finally

            oleCon.Close()

            dts = Nothing
            oleAdapter = Nothing
            oleCon = Nothing
            dsExcelData = Nothing
            enumerator = Nothing
            ProviderList = Nothing


        End Try

    End Function

    ''' 엑셀내역을 조회하여 XML로 변환한다.
    ''' </summary>
    ''' <param name="txls"></param>
    ''' <remarks>엑셀에 있는 각 시트의 내역을 XML로 변환하여 배열로 리턴한다.</remarks>
    Public Function GetAllXML_SP(ByVal strFileName As String, ByVal blnHeader As Boolean, ByVal iTableIndex As Integer, ByRef strTableName As String) As String()

        Dim dsExcelData As DataSet
        Dim strComand As String
        Dim i As Integer
        Dim nullCnt As Integer = 0
        Dim strSheetTableName() As String
        Dim strSheetName As String
        Dim strConnectionString As String
        Dim oleCon As System.Data.OleDb.OleDbConnection
        Dim oleAdapter As System.Data.OleDb.OleDbDataAdapter
        Dim enumerator As System.Data.OleDb.OleDbEnumerator
        Dim ProviderList As ArrayList = New ArrayList()
        Dim iCnt As Integer
        Dim dts As System.Data.DataTable
        Dim ExcelCon As String = "Provider="
        Dim cv_OLEDB_s As String = ""
        Dim cv_OLEVR_d As Double
        Dim cv_ExcelVR_d As String = ""
        Dim strXml() As String

        Try
            enumerator = New OleDbEnumerator()
            '엑셀 Provider 여부에 따라 접속방식변경
            dts = enumerator.GetElements
            For iCnt = 1 To dts.Rows.Count()

                If dts.Rows(iCnt - 1).Item(0).ToString.IndexOf("Microsoft.ACE.OLEDB.") >= 0 Or dts.Rows(iCnt - 1).Item(0).ToString.IndexOf("Microsoft.Jet.OLEDB.") >= 0 Then
                    ProviderList.Add(dts.Rows(iCnt - 1).Item(0).ToString)
                End If
            Next

            ProviderList.Sort()

            cv_OLEVR_d = 0


            For iCnt = 1 To ProviderList.Count

                If ProviderList(iCnt - 1).ToString.IndexOf("Microsoft.ACE.OLEDB.") >= 0 Then

                    If cv_OLEVR_d < Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.ACE.OLEDB.", "")) Then
                        cv_OLEDB_s = ProviderList(iCnt - 1).ToString
                        cv_OLEVR_d = Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.ACE.OLEDB.", ""))
                    End If
                End If

            Next

            cv_OLEVR_d = 0

            If cv_OLEDB_s = "" Then
                For iCnt = 1 To ProviderList.Count

                    If ProviderList(iCnt - 1).ToString.IndexOf("Microsoft.Jet.OLEDB.") >= 0 Then
                        If cv_OLEVR_d < Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.Jet.OLEDB.", "")) Then
                            cv_OLEDB_s = ProviderList(iCnt - 1).ToString
                            cv_OLEVR_d = Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.Jet.OLEDB.", ""))
                        End If
                    End If
                Next
            End If


            ExcelCon = ExcelCon & cv_OLEDB_s & ";"

            'If cv_OLEDB_s.IndexOf("Microsoft.ACE.OLEDB.15") >= 0 Then
            '    cv_ExcelVR_d = "Excel 15.0;"
            'Else
            cv_ExcelVR_d = "Excel 8.0;"
            'End If

            dts.Clear()

            '엑셀 CONNECT (MICROSOFT.JET.OLEDB) 활용 
            '파일경로, 헤더데이터여부를 넘김다.
            If blnHeader Then
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + cv_ExcelVR_d + "HDR=No;IMEX=1;" + Convert.ToChar(34).ToString()
            Else
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + cv_ExcelVR_d + "HDR=No;IMEX=1;" + Convert.ToChar(34).ToString()
            End If

            '엑셀 oledb객체생성
            oleCon = New OleDbConnection()
            oleCon.ConnectionString = strConnectionString

            '엑셀 oledb객체 연결
            oleCon.Open()

            '엑셀시트스키가 가져오기
            dts = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)

            '시트 갯수만큼 시트명을 가져온다.
            ReDim strSheetTableName(dts.Rows.Count() - 1)

            For i = 1 To dts.Rows.Count()

                strSheetName = dts.Rows(i - 1).Item("TABLE_NAME").ToString()

                If Right(strSheetName, 1) <> "_" Then

                    strSheetTableName(i - 1) = strSheetName.Substring(0, strSheetName.Length() - 1)

                End If

                If ((i - 1) = iTableIndex) Then
                    strTableName = strSheetTableName(i - 1)
                End If


            Next

            '배열초기화 ( 시트의 갯수만큼 배열초기화)
            ReDim strXml(strSheetTableName.Length() - 1)

            '데이터테이블 초기화
            dts.Clear()

            '시트의 갯수만큼 루핑으로 XML변환
            For i = 1 To strSheetTableName.Length()

                If Not strSheetTableName(i - 1) Is Nothing And (i - 1) = iTableIndex Then

                    '' 엑셀의 시트내역을 데이터베이스처럼 인식하여 시트명이 테이블네임이 된다
                    '' 헤더정보를 가져올경우 첫번째 라인이 컬럼이 된다.
                    '' 헤더정보를 안가져올경우 컬럼은 순차적으로 F1, F2, F3 ~~~~~ 이런식으로 변환된다.
                    '' 한글과 영어는 인식하지만 숫자와 부호는 _x0026_ 이런식으로 16진수로 변환되서 나온다.
                    strComand = "select * from [" + strSheetTableName(i - 1) + "$A2:AC10000]"

                    '엑셀연결
                    oleAdapter = New OleDbDataAdapter(strComand, oleCon)

                    '데이터테이블 설정(xml로 변환하기 위하여 데이터테이블에 저장)
                    dts = New System.Data.DataTable(strSheetTableName(i - 1))

                    '데이터테이블에 엑셀내용 저장
                    oleAdapter.FillSchema(dts, SchemaType.Source)
                    oleAdapter.Fill(dts)

                    '각 시트별로 담기위하여 데이터셋 설정
                    dsExcelData = New DataSet()
                    dsExcelData.Tables.Add(dts)


                    '시트이 데이터가 있는경우만 xml을 배열에 담는다.
                    If dts.Rows.Count() > 0 Then

                        '데이터셋에서 xml 추출
                        strXml(i - 1) = dsExcelData.GetXml()
                        nullCnt = nullCnt + 1

                    End If

                    '연결내역 제거
                    dts.Clear()
                    oleAdapter.Dispose()
                    dsExcelData.Dispose()

                End If

            Next

            '빈 배열값을 제거
            ReDim Preserve strXml(nullCnt - 1)

            Return strXml


        Catch ex As Exception
            Return Nothing
        Finally

            oleCon.Close()

            dts = Nothing
            oleAdapter = Nothing
            oleCon = Nothing
            dsExcelData = Nothing
            enumerator = Nothing
            ProviderList = Nothing

        End Try

    End Function

    ''' 엑셀내역을 조회하여 XML로 변환한다.
    ''' </summary>
    ''' <param name="txls"></param>
    ''' <remarks>엑셀에 있는 각 시트의 내역을 XML로 변환하여 배열로 리턴한다.</remarks>
    Public Function GetAllXML_Sheet(ByVal strFileName As String, ByVal strSheetName As String, ByVal blnHeader As Boolean) As String

        Dim dsExcelData As DataSet
        Dim strComand As String
        Dim i As Integer
        Dim nullCnt As Integer = 0
        Dim strSheetTableName() As String

        Dim strConnectionString As String
        Dim oleCon As System.Data.OleDb.OleDbConnection
        Dim oleAdapter As System.Data.OleDb.OleDbDataAdapter
        Dim enumerator As System.Data.OleDb.OleDbEnumerator
        Dim ProviderList As ArrayList = New ArrayList()
        Dim iCnt As Integer
        Dim dts As System.Data.DataTable
        Dim ExcelCon As String = "Provider="
        Dim cv_OLEDB_s As String = ""
        Dim cv_OLEVR_d As Double
        Dim cv_ExcelVR_d As String = ""
        Dim strXml As String

        Try
            enumerator = New OleDbEnumerator()
            '엑셀 Provider 여부에 따라 접속방식변경
            dts = enumerator.GetElements
            For iCnt = 1 To dts.Rows.Count()

                If dts.Rows(iCnt - 1).Item(0).ToString.IndexOf("Microsoft.ACE.OLEDB.") >= 0 Or dts.Rows(iCnt - 1).Item(0).ToString.IndexOf("Microsoft.Jet.OLEDB.") >= 0 Then
                    ProviderList.Add(dts.Rows(iCnt - 1).Item(0).ToString)
                End If
            Next

            ProviderList.Sort()

            cv_OLEVR_d = 0


            For iCnt = 1 To ProviderList.Count

                If ProviderList(iCnt - 1).ToString.IndexOf("Microsoft.ACE.OLEDB.") >= 0 Then

                    If cv_OLEVR_d < Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.ACE.OLEDB.", "")) Then
                        cv_OLEDB_s = ProviderList(iCnt - 1).ToString
                        cv_OLEVR_d = Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.ACE.OLEDB.", ""))
                    End If
                End If

            Next

            cv_OLEVR_d = 0

            If cv_OLEDB_s = "" Then
                For iCnt = 1 To ProviderList.Count

                    If ProviderList(iCnt - 1).ToString.IndexOf("Microsoft.Jet.OLEDB.") >= 0 Then
                        If cv_OLEVR_d < Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.Jet.OLEDB.", "")) Then
                            cv_OLEDB_s = ProviderList(iCnt - 1).ToString
                            cv_OLEVR_d = Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.Jet.OLEDB.", ""))
                        End If
                    End If
                Next
            End If


            ExcelCon = ExcelCon & cv_OLEDB_s & ";"

            'If cv_OLEDB_s.IndexOf("Microsoft.ACE.OLEDB.15") >= 0 Then
            '    cv_ExcelVR_d = "Excel 15.0;"
            'Else
            cv_ExcelVR_d = "Excel 8.0;"
            'End If

            dts.Clear()

            '엑셀 CONNECT (MICROSOFT.JET.OLEDB) 활용 
            '파일경로, 헤더데이터여부를 넘김다.
            If blnHeader Then
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + cv_ExcelVR_d + "HDR=Yes;IMEX=1;" + Convert.ToChar(34).ToString()
            Else
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + cv_ExcelVR_d + "HDR=No;IMEX=1;" + Convert.ToChar(34).ToString()
            End If

            '엑셀 oledb객체생성
            oleCon = New OleDbConnection()
            oleCon.ConnectionString = strConnectionString

            '엑셀 oledb객체 연결
            oleCon.Open()

            '엑셀시트스키가 가져오기
            dts = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)

            '데이터테이블 초기화
            dts.Clear()



            '' 엑셀의 시트내역을 데이터베이스처럼 인식하여 시트명이 테이블네임이 된다
            '' 헤더정보를 가져올경우 첫번째 라인이 컬럼이 된다.
            '' 헤더정보를 안가져올경우 컬럼은 순차적으로 F1, F2, F3 ~~~~~ 이런식으로 변환된다.
            '' 한글과 영어는 인식하지만 숫자와 부호는 _x0026_ 이런식으로 16진수로 변환되서 나온다.
            strComand = "select * from [" + strSheetName + "$]"

            '엑셀연결
            oleAdapter = New OleDbDataAdapter(strComand, oleCon)

            '데이터테이블 설정(xml로 변환하기 위하여 데이터테이블에 저장)
            dts = New System.Data.DataTable(strSheetName)

            '데이터테이블에 엑셀내용 저장
            oleAdapter.FillSchema(dts, SchemaType.Source)
            oleAdapter.Fill(dts)

            'Dim mStreamXML As System.IO.MemoryStream
            'Dim srXML As System.IO.StreamReader 
            'mStreamXML = New System.IO.MemoryStream()

            'dts.WriteXml(mStreamXML)
            'mStreamXML.Seek(0, System.IO.SeekOrigin.Begin) 
            'srXML = New System.IO.StreamReader(mStreamXML) 
            'strXml = srXML.ReadToEnd()

            '각 시트별로 담기위하여 데이터셋 설정
            dsExcelData = New DataSet()
            dsExcelData.Tables.Add(dts)

            'dts.Columns.Item("비고").DataType = System.Type.GetType("System.String")

            strXml = dsExcelData.GetXml()

            '연결내역 제거    
            dts.Clear()
            oleAdapter.Dispose()
            dsExcelData.Dispose()




            Return strXml


        Catch ex As Exception
            Return Nothing
        Finally

            oleCon.Close()

            dts = Nothing
            oleAdapter = Nothing
            oleCon = Nothing
            dsExcelData = Nothing
            enumerator = Nothing
            ProviderList = Nothing
            ' mStreamXML = Nothing
            'srXML = NOYIHNG

        End Try

    End Function

#End Region

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

    'Public oCrxReport As CRAXDDRT.Report
    'Public oCrxApplication As New CRAXDDRT.ApplicationClass

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

    Public pv_No As String
    Public pv_Nm As String

    Public Const vbKeyTab As Integer = 9

    'Global Variable-----------------------------------------------------------------------

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

#End Region

#Region "WJS_ZZ_XMLForm"
    Class WJS_ZZ_XMLForm
        'Dim clsWJS_ZZ_CommFunc As WJS_ZZ_CommFunc

        Public Sub XMLMenu(ByVal pFileName As String, ByVal pAction As Boolean)

            Try
                Dim oXMLDoc As Xml.XmlDataDocument
                Dim Language As String

                oXMLDoc = New Xml.XmlDataDocument

                Language = LanguageToString()

                If pAction Then '메뉴추가
                    'pFileName = App.Path + "\MNU\" & Language & "\" & pFileName & "_" & Language & ".xml"
                    pFileName = My.Application.Info.DirectoryPath + "\MNU\" & pFileName & ".xml"
                Else '메뉴삭제
                    'pFileName = App.Path + "\MNU\" & Language & "\" & pFileName & "_REMOVE_" & Language & ".xml"
                    pFileName = My.Application.Info.DirectoryPath + "\MNU\" & pFileName & "_REMOVE_" & ".xml"
                End If

                '// load the content of the XML File
                Call oXMLDoc.Load(pFileName)

                '한국어를 다른 언어로 바꾼다.
                If Language <> "KOR" Then
                    Call MenuXml(Language, oXMLDoc)
                End If

                '// load the form to the SBO application in one batch
                B1Connections.theAppl.LoadBatchActions(oXMLDoc.ToString)

                oXMLDoc = Nothing

                Exit Sub
            Catch ex As Exception

            End Try
        End Sub


        Public Function XMLForm(ByVal pFileName As String) As Xml.XmlDataDocument
            Dim oXMLDoc As New Xml.XmlDataDocument
            Dim Language As String
            Dim Height As Integer
            Dim Width As Integer

            Try
                Language = LanguageToString()

                'pFileName = App.Path + "\SRF\" & Language & "\" & pFileName & "_" & Language & ".srf"
                pFileName = My.Application.Info.DirectoryPath + "\SRF\" & pFileName & ".srf"

                Call oXMLDoc.Load(pFileName)            'load the content of the XML File

                gFormCnt = gFormCnt + 1                 '여래개의 폼을 Load할 경우 폼의 UID로 사용함.

                Height = oXMLDoc.SelectSingleNode("Application/forms/action/form/@height").Value
                Width = oXMLDoc.SelectSingleNode("Application/forms/action/form/@width").Value

                oXMLDoc.SelectSingleNode("Application/forms/action/form/@uid").Value = _
                    oXMLDoc.SelectSingleNode("Application/forms/action/form/@uid").Value & gFormCnt

                oXMLDoc.SelectSingleNode("Application/forms/action/form/@top").Value = _
                       (oApplication.Desktop.Height - Height - 130) / 2

                oXMLDoc.SelectSingleNode("Application/forms/action/form/@left").Value = _
                       (oApplication.Desktop.Width - Width) / 2


                If Language <> "KOR" Then
                    Call CaptionXml(Language, oXMLDoc)
                End If

                oApplication.LoadBatchActions(oXMLDoc.ToString)       'now you may load the form to the application

                XMLForm = oXMLDoc
                oXMLDoc = Nothing

                Exit Function

            Catch ex As Exception
                XMLForm = oXMLDoc
            End Try

        End Function


        Private Sub MenuXml(ByVal cv_language_s As String, ByVal oXMLDoc As System.Xml.XmlDataDocument)

            Try
                Dim i, j As Integer

                Dim sarKey() As String
                Dim sarValue() As String
                Dim nodelist As Xml.XmlNodeList
                Dim xSQL As String
                Dim strQuery As String
                Dim Rs As SAPbobsCOM.Recordset
                Dim cv_cnt_i As Integer

                xSQL = "select  '' as MsgValue " & vbCrLf

                nodelist = oXMLDoc.GetElementsByTagName("@String")


                For i = 0 To nodelist.Count - 1
                    xSQL = xSQL & " UNION ALL " & vbCrLf
                    xSQL = xSQL & "select  N'" & nodelist.Item(i).Value & "' as MsgValue " & vbCrLf
                Next i
                nodelist = Nothing


                strQuery = " select  distinct C.MsgValue as MsgKey , A.MsgValue as MsgValue " & vbCrLf
                strQuery = strQuery & " from " & vbCrLf
                strQuery = strQuery & "(" & xSQL & vbCrLf
                strQuery = strQuery & ") C " & vbCrLf
                strQuery = strQuery & "inner join " & vbCrLf
                strQuery = strQuery & "( " & vbCrLf
                strQuery = strQuery & "  select max(usetp) as usetp, max(MsgCd) as MsgCd , MsgValue " & vbCrLf
                strQuery = strQuery & "  from [RepDB].dbo.messagemr  " & vbCrLf
                strQuery = strQuery & " where  usetp = '2' and LangCd = 'KOR' " & vbCrLf
                strQuery = strQuery & "  group by MsgValue " & vbCrLf
                strQuery = strQuery & ") B on C.MsgValue = B.MsgValue " & vbCrLf
                strQuery = strQuery & "Inner join  " & vbCrLf
                strQuery = strQuery & "( " & vbCrLf
                strQuery = strQuery & "  select usetp, MsgCd, MsgValue " & vbCrLf
                strQuery = strQuery & "  from [RepDB].dbo.messagemr " & vbCrLf
                strQuery = strQuery & "  where  usetp = '2' and LangCd = '" & cv_language_s & "' " & vbCrLf
                strQuery = strQuery & ") A  on A.MsgCd = B.MsgCd and A.usetp = B.usetp " & vbCrLf
                strQuery = strQuery & " where Rtrim(C.msgvalue) <> '' and A.MsgValue <> '' " & vbCrLf
                strQuery = strQuery & " and c.msgvalue <> '#' " & vbCrLf
                Rs = oCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                Rs.DoQuery(strQuery)
                cv_cnt_i = Rs.RecordCount

                ReDim sarKey(cv_cnt_i)
                ReDim sarValue(cv_cnt_i)

                For i = 0 To cv_cnt_i - 1
                    sarKey(i) = Rs.Fields.Item(0).Value
                    sarValue(i) = Rs.Fields.Item(1).Value
                    Rs.MoveNext()
                Next

                If (Not (Rs) Is Nothing) Then Marshal.ReleaseComObject(Rs)
                Rs = Nothing

                '컨트롤

                'Set nodelist = oXMLDoc.selectNodes("Application/Menus/action/Menu/@String")
                nodelist = oXMLDoc.GetElementsByTagName("@String")
                For i = 0 To nodelist.Count - 1
                    For j = 0 To UBound(sarKey) - 1
                        If sarKey(j) = nodelist.Item(i).Value Then
                            nodelist.Item(i).Value = sarValue(j)
                            Exit For
                        End If
                    Next j
                Next i
                nodelist = Nothing

                ReDim sarKey(0)
                ReDim sarValue(0)
            Catch ex As Exception
                CFL.COMMON_MESSAGE("!", ex.Message)
            End Try
        End Sub

        Private Sub CaptionXml(ByVal cv_language_s As String, ByVal oXMLDoc As System.Xml.XmlDataDocument)

            Try
                Dim i, j As Integer
                Dim sarKey() As String
                Dim sarValue() As String
                Dim cv_msgvalue_s As String
                Dim nodelist As Xml.XmlNodeList
                Dim xSQL As String
                Dim strQuery As String
                Dim Rs As SAPbobsCOM.Recordset
                Dim cv_cnt_i As Integer

                '폼 제목
                cv_msgvalue_s = oXMLDoc.SelectSingleNode("Application/forms/action/form/@title").Value

                xSQL = "select  N'" & cv_msgvalue_s & "' as MsgValue " & vbCrLf

                '컨트롤

                nodelist = oXMLDoc.SelectNodes("Application/forms/action/form/items/action/item/specific/@caption")
                For i = 0 To nodelist.Count - 1
                    xSQL = xSQL & " UNION ALL " & vbCrLf
                    xSQL = xSQL & "select  N'" & nodelist.Item(i).Value & "' as MsgValue " & vbCrLf
                Next i
                nodelist = Nothing

                '칼럼

                nodelist = oXMLDoc.SelectNodes("Application/forms/action/form/items/action/item/specific/columns/action/column/@title")
                For i = 0 To nodelist.Count - 1
                    xSQL = xSQL & " UNION ALL " & vbCrLf
                    xSQL = xSQL & "select  N'" & nodelist.Item(i).Value & "' as MsgValue " & vbCrLf
                Next i
                nodelist = Nothing

                strQuery = " select  distinct C.MsgValue as MsgKey , A.MsgValue as MsgValue " & vbCrLf
                strQuery = strQuery & " from " & vbCrLf
                strQuery = strQuery & "(" & xSQL & vbCrLf
                strQuery = strQuery & ") C " & vbCrLf
                strQuery = strQuery & "inner join " & vbCrLf
                strQuery = strQuery & "( " & vbCrLf
                strQuery = strQuery & "  select max(usetp) as usetp, max(MsgCd) as MsgCd , MsgValue " & vbCrLf
                strQuery = strQuery & "  from [RepDB].dbo.messagemr  " & vbCrLf
                strQuery = strQuery & " where  usetp = '2' and LangCd = 'KOR' " & vbCrLf
                strQuery = strQuery & "  group by MsgValue " & vbCrLf
                strQuery = strQuery & ") B on C.MsgValue = B.MsgValue " & vbCrLf
                strQuery = strQuery & "Inner join  " & vbCrLf
                strQuery = strQuery & "( " & vbCrLf
                strQuery = strQuery & "  select usetp, MsgCd, MsgValue " & vbCrLf
                strQuery = strQuery & "  from [RepDB].dbo.messagemr " & vbCrLf
                strQuery = strQuery & "  where  usetp = '2' and LangCd = '" & cv_language_s & "' " & vbCrLf
                strQuery = strQuery & ") A  on A.MsgCd = B.MsgCd and A.usetp = B.usetp " & vbCrLf
                strQuery = strQuery & " where Rtrim(C.msgvalue) <> '' and A.MsgValue <> '' " & vbCrLf
                strQuery = strQuery & " and c.msgvalue <> '#' " & vbCrLf
                Rs = oCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                Rs.DoQuery(strQuery)
                cv_cnt_i = Rs.RecordCount

                ReDim sarKey(cv_cnt_i)
                ReDim sarValue(cv_cnt_i)

                For i = 0 To cv_cnt_i - 1
                    sarKey(i) = Rs.Fields.Item(0).Value
                    sarValue(i) = Rs.Fields.Item(1).Value
                    Rs.MoveNext()
                Next

                If (Not (Rs) Is Nothing) Then Marshal.ReleaseComObject(Rs)
                Rs = Nothing


                '폼 제목
                cv_msgvalue_s = oXMLDoc.SelectSingleNode("Application/forms/action/form/@title").Value

                For i = 0 To UBound(sarKey) - 1
                    If sarKey(i) = cv_msgvalue_s Then
                        oXMLDoc.SelectSingleNode("Application/forms/action/form/@title").Value = sarValue(i)
                        Exit For
                    End If
                Next i

                '컨트롤

                nodelist = oXMLDoc.SelectNodes("Application/forms/action/form/items/action/item/specific/@caption")
                For i = 0 To nodelist.Count - 1
                    For j = 0 To UBound(sarKey) - 1
                        If sarKey(j) = nodelist.Item(i).Value Then
                            nodelist.Item(i).Value = sarValue(j)
                            Exit For
                        End If
                    Next j
                Next i
                nodelist = Nothing

                '칼럼
                nodelist = oXMLDoc.SelectNodes("Application/forms/action/form/items/action/item/specific/columns/action/column/@title")
                For i = 0 To nodelist.Count - 1
                    For j = 0 To UBound(sarKey) - 1
                        If sarKey(j) = nodelist.Item(i).Value Then
                            nodelist.Item(i).Value = sarValue(j)
                            Exit For
                        End If
                    Next j
                Next i
                nodelist = Nothing

                ReDim sarKey(0)
                ReDim sarValue(0)
            Catch ex As Exception
                CFL.COMMON_MESSAGE("!", ex.Message)
            End Try
        End Sub


    End Class
#End Region


    ''' <summary>
    ''' 사업장 콤보 셋팅
    ''' </summary>
    ''' <param name="ComboObj">콤보객체명</param>
    ''' <param name="AllYN">TRUE    : 전체추가 FALSE   : 전체없음</param>
    ''' <param name="UseYN">TRUE    : 사용중인 사업장만  FALSE   : 모든 사업장</param>
    ''' <remarks>사업장 콤보 셋팅</remarks>
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
                xSql = "SELECT BPLId, BPLName FROM OBPL WHERE Disabled = N'N' ORDER BY BPLId"
            Else
                xSql = "SELECT BPLId, BPLName FROM OBPL ORDER BY BPLId"
            End If

#If HANA = "Y" Then
            xSql = CFL.GetConvertHANA(xSql)
#End If
            oRS.DoQuery(xSql)

            If AllYN Then
                ComboObj.ValidValues.Add("", CFL.GetCaption("전체", ModuleIni.AD))
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
    Public Function GetFileFromBrowser(Optional ByVal pOption As String = "") As String
        Dim strFileName As String = ""
        Dim oDIVersion As String = ""
        Dim oOptMsg As String = ""
        Dim oFinalMsg As String = ""
        Dim oOption() As String

        Dim i As Integer

        Try

            pOption = UCase(pOption)
            oOption = Split(pOption, ",")

            For i = LBound(oOption) To UBound(oOption)
                If Trim(oOption(i)) = "ALL" Then
                    oOptMsg = ""
                    Exit For '전체 파일이면 끝...
                ElseIf Trim(oOption(i)) = "XLS" Then
                    oOptMsg = "excel files (*.xls)|*.xls"
                ElseIf Trim(oOption(i)) = "XLSX" Then
                    oOptMsg = "excel files (*.xlsx)|*.xlsx"
                ElseIf Trim(oOption(i)) = "TXT" Then
                    oOptMsg = "Text (tab-seperated)|*.txt"
                ElseIf Trim(oOption(i)) = "CSV" Then
                    oOptMsg = "CSV (comma-seperated)|*.csv"
                ElseIf Trim(oOption(i)) = "JPG" Then
                    oOptMsg = "JPG files (*.jpg)|*.jpg"
                ElseIf Trim(oOption(i)) = "BMP" Then
                    oOptMsg = "BMP files (*.bmp)|*.bmp"
                End If
                oFinalMsg = IIf(oFinalMsg = "", oFinalMsg & oOptMsg, oFinalMsg & "|" & oOptMsg)

            Next i

            If B1Connections.theAppl.ClientType = SAPbouiCOM.BoClientType.ct_Browser Then
                strFileName = B1Connections.theAppl.GetFileFromBrowser()
            Else
                If oOptMsg = "" Then
                    strFileName = CFL.FileDialog(eFileDialog.en_OpenFile, True)
                Else
                    strFileName = CFL.FileDialog(eFileDialog.en_OpenFile, oFinalMsg, True)
                End If
            End If

        Catch ex As Exception
            If Err.Number <> -10 Then
                B1Connections.theAppl.StatusBar.SetText("GetFileFromBrowser Error : " & Err.Number & " " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            End If
        End Try

        Return strFileName

    End Function
    Public Function LanguageToString() As String
        LanguageToString = String.Empty
        Select Case B1Connections.diCompany.language
            Case SAPbobsCOM.BoSuppLangs.ln_Korean_Kr : LanguageToString = "KOR"
            Case SAPbobsCOM.BoSuppLangs.ln_Japanese_Jp : LanguageToString = "JPN"
            Case SAPbobsCOM.BoSuppLangs.ln_French : LanguageToString = "FRN"
            Case SAPbobsCOM.BoSuppLangs.ln_English : LanguageToString = "ENG"
            Case SAPbobsCOM.BoSuppLangs.ln_Chinese : LanguageToString = "CHN"
        End Select
        Return LanguageToString
    End Function


    ''' <summary>
    ''' 링크화면 오픈
    ''' </summary>
    ''' <param name="oForm">Form</param>
    ''' <param name="strObject">Link Object</param>
    ''' <param name="strDocEntry">DocEntry</param>
    ''' <remarks></remarks>
    Public Sub ShowDocEntLinkForm(ByVal oForm As SAPbouiCOM.Form, ByVal strObject As String, ByVal strDocEntry As String)
        Dim isHidEditItem1 As Boolean = False
        Dim isHidEditItem2 As Boolean = False
        Dim isHidLnkBtn As Boolean = False
        Dim oHidEditItem1 As SAPbouiCOM.Item
        Dim oHidEditItem2 As SAPbouiCOM.Item
        Dim oHidLnkBtn As SAPbouiCOM.Item
        Dim xmlAtt As Xml.XmlAttribute
        Dim xmlDoc As Xml.XmlDocument = New Xml.XmlDocument()

        Try

            If Not IsNothing(oForm) Then
                oForm.Freeze(True)
            Else
                Exit Try
            End If

            '폼 XML로드
            xmlDoc.LoadXml(oForm.GetAsXML())

            '링크버튼 호출에 필요한 Item 존재여부 확인
            For Each node As XmlNode In xmlDoc.SelectNodes("Application/forms/action/form/items/action/item")

                xmlAtt = node.Attributes.ItemOf("uid")

                If (Not xmlAtt Is Nothing) Then

                    If (xmlAtt.InnerText = "edtHIDZ9Z8") Then

                        isHidEditItem1 = True

                        If isHidEditItem1 And isHidEditItem2 And isHidLnkBtn Then Exit For

                    ElseIf (xmlAtt.InnerText = "edtHIDZ9Z9") Then

                        isHidEditItem2 = True

                        If isHidEditItem1 And isHidEditItem2 And isHidLnkBtn Then Exit For

                    ElseIf (xmlAtt.InnerText = "lnkHIDZ9Z9") Then

                        isHidLnkBtn = True

                        If isHidEditItem1 And isHidEditItem2 And isHidLnkBtn Then Exit For

                    End If

                End If

            Next

            '링크버튼 호출에 필요한 Item이 없으면 생성
            If Not isHidEditItem1 Then
                oHidEditItem1 = oForm.Items.Add("edtHIDZ9Z8", BoFormItemTypes.it_EDIT)
                oHidEditItem1.Left = -335
                oHidEditItem1.Top = 486
                oForm.Items.Item("edtHIDZ9Z8").AffectsFormMode = False
            End If

            '링크버튼 호출에 필요한 Item이 없으면 생성
            If Not isHidEditItem2 Then
                oHidEditItem2 = oForm.Items.Add("edtHIDZ9Z9", BoFormItemTypes.it_EDIT)
                oHidEditItem2.Left = -435
                oHidEditItem2.Top = 486
                oForm.Items.Item("edtHIDZ9Z9").AffectsFormMode = False
            End If

            '링크버튼 호출에 필요한 Item이 없으면 생성
            If Not isHidLnkBtn Then
                oHidLnkBtn = oForm.Items.Add("lnkHIDZ9Z9", BoFormItemTypes.it_LINKED_BUTTON)
                oHidLnkBtn.Left = -315
                oHidLnkBtn.LinkTo = "edtHIDZ9Z8"
                oHidLnkBtn.Top = 486
            End If

            '링크 버튼 오브젝트 처리
            oForm.Items.Item("lnkHIDZ9Z9").Specific.LinkedObject = Trim(strObject)
            oForm.Items.Item("lnkHIDZ9Z9").Specific.LinkedObjectType = Trim(strObject)
            oForm.Items.Item("edtHIDZ9Z8").Specific.Value = Trim(strDocEntry)
            oForm.Items.Item("edtHIDZ9Z9").Click()

            '링크버튼 클릭
            oForm.Items.Item("lnkHIDZ9Z9").Click()

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("ShowDocEntLinkForm " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            If Not IsNothing(oForm) Then oForm.Freeze(False)
            oHidEditItem1 = Nothing
            oHidEditItem2 = Nothing
            oHidLnkBtn = Nothing
            xmlAtt = Nothing
            xmlDoc = Nothing
        End Try

    End Sub

    ''' <summary>
    ''' 크리스탈 리포트 팝업
    ''' </summary>
    ''' <param name="rpt"></param>
    ''' <remarks></remarks>

#If HANA = "Y" Then
    Public Sub RptShow2(ByRef rpt As WJS.COMM.H.Report)
#Else
    Public Sub RptShow2(ByRef rpt As WJS.COMM.Report)
#End If

        Dim strStartPath As String = System.Reflection.Assembly.GetExecutingAssembly.Location
        Dim strReportPath As String
        Dim strExportPath As String
        Dim proc As Process

        Try
            strStartPath = strStartPath.Substring(0, strStartPath.LastIndexOf("\"))

            If (System.IO.File.Exists(strStartPath + "\CrystalViewer.exe")) Then

                strReportPath = rpt.crxReport.FileName

                'My.Computer.FileSystem.SpecialDirectories.Temp

                strExportPath = My.Computer.FileSystem.SpecialDirectories.Temp + strReportPath.Substring(strReportPath.LastIndexOf("\"), strReportPath.Length - strReportPath.LastIndexOf("\"))

                rpt.crxReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.CrystalReport, strExportPath)

                proc = New System.Diagnostics.Process
                proc.StartInfo.FileName = strStartPath + "\CrystalViewer.exe"
                proc.StartInfo.Arguments = " """ & strExportPath & """"
                proc.Start()

            Else
                'Report Viewer 실행
                rpt.ShowDialog(CFL.GetSBOWindow())
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("RptShow Error : " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            proc = Nothing

            If IsNothing(rpt) = False Then
                rpt.crxReport.Close()
                rpt.crxReport.Dispose()
                rpt.Close()
                rpt.Dispose()
                rpt = Nothing
                GC.Collect()
            End If

        End Try

    End Sub


    ''' <summary>
    ''' 링크화면 오픈
    ''' </summary>
    ''' <param name="oForm">Form</param>
    ''' <param name="strObject">Link Object</param>
    ''' <param name="strDocNum">DocNum</param>
    ''' <param name="strSeries">Series</param>
    ''' <remarks></remarks>
    Public Sub ShowDocNumLinkForm(ByVal oForm As SAPbouiCOM.Form, ByVal strObject As String, ByVal strDocNum As String, Optional ByVal strSeries As String = "")
        Dim isHidEditItem1 As Boolean = False
        Dim isHidEditItem2 As Boolean = False
        Dim isHidLnkBtn As Boolean = False
        Dim oHidEditItem1 As SAPbouiCOM.Item
        Dim oHidEditItem2 As SAPbouiCOM.Item
        Dim oHidLnkBtn As SAPbouiCOM.Item
        Dim xmlAtt As Xml.XmlAttribute
        Dim xmlDoc As Xml.XmlDocument = New Xml.XmlDocument()
        Dim oRS As SAPbobsCOM.Recordset
        Dim strDocEntry As String = "-1"

        Try

            If Not IsNothing(oForm) Then
                oForm.Freeze(True)
            Else
                Exit Try
            End If

            '폼 XML로드
            xmlDoc.LoadXml(oForm.GetAsXML())

            '링크버튼 호출에 필요한 Item 존재여부 확인
            For Each node As XmlNode In xmlDoc.SelectNodes("Application/forms/action/form/items/action/item")

                xmlAtt = node.Attributes.ItemOf("uid")

                If (Not xmlAtt Is Nothing) Then

                    If (xmlAtt.InnerText = "edtHIDZ9Z8") Then

                        isHidEditItem1 = True

                        If isHidEditItem1 And isHidEditItem2 And isHidLnkBtn Then Exit For

                    ElseIf (xmlAtt.InnerText = "edtHIDZ9Z9") Then

                        isHidEditItem2 = True

                        If isHidEditItem1 And isHidEditItem2 And isHidLnkBtn Then Exit For

                    ElseIf (xmlAtt.InnerText = "lnkHIDZ9Z9") Then

                        isHidLnkBtn = True

                        If isHidEditItem1 And isHidEditItem2 And isHidLnkBtn Then Exit For

                    End If

                End If

            Next

            '링크버튼 호출에 필요한 Item이 없으면 생성
            If Not isHidEditItem1 Then
                oHidEditItem1 = oForm.Items.Add("edtHIDZ9Z8", BoFormItemTypes.it_EDIT)
                oHidEditItem1.Left = -335
                oHidEditItem1.Top = 486
                oForm.Items.Item("edtHIDZ9Z8").AffectsFormMode = False
            End If

            '링크버튼 호출에 필요한 Item이 없으면 생성
            If Not isHidEditItem2 Then
                oHidEditItem2 = oForm.Items.Add("edtHIDZ9Z9", BoFormItemTypes.it_EDIT)
                oHidEditItem2.Left = -435
                oHidEditItem2.Top = 486
                oForm.Items.Item("edtHIDZ9Z9").AffectsFormMode = False
            End If

            '링크버튼 호출에 필요한 Item이 없으면 생성
            If Not isHidLnkBtn Then
                oHidLnkBtn = oForm.Items.Add("lnkHIDZ9Z9", BoFormItemTypes.it_LINKED_BUTTON)
                oHidLnkBtn.Left = -315
                oHidLnkBtn.LinkTo = "edtHIDZ9Z8"
                oHidLnkBtn.Top = 486
            End If

            'DocEntry 번호를 조회
            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            oRS.DoQuery(" EXEC WJS_SP_AD_GETDOCENT '" & strObject & "', '" & strDocNum & "', '" & strSeries & "'")
            If Not oRS.EoF Then strDocEntry = Trim(oRS.Fields.Item("DOCENT").Value)

            '링크 버튼 오브젝트 처리
            oForm.Items.Item("lnkHIDZ9Z9").Specific.LinkedObject = Trim(strObject)
            oForm.Items.Item("lnkHIDZ9Z9").Specific.LinkedObjectType = Trim(strObject)
            oForm.Items.Item("edtHIDZ9Z8").Specific.Value = Trim(strDocEntry)
            oForm.Items.Item("edtHIDZ9Z9").Click()

            '링크버튼 클릭
            oForm.Items.Item("lnkHIDZ9Z9").Click()

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("ShowDocNumLinkForm " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            If Not IsNothing(oForm) Then oForm.Freeze(False)
            oHidEditItem1 = Nothing
            oHidEditItem2 = Nothing
            oHidLnkBtn = Nothing
            xmlAtt = Nothing
            xmlDoc = Nothing
            oRS = Nothing
        End Try

    End Sub


    ''' <summary>
    ''' GetUDOKey
    ''' </summary>
    ''' <param name="strTableNM">UDO 테이블 명</param>
    ''' <returns>DocEntry, DocNum 리턴</returns>
    ''' <remarks>신규로 생성될 DocEntry, DocNum 조회</remarks>
    Public Function GetUDOKey(ByVal strTableNM As String) As Hashtable
        Dim hsTable As Hashtable = New Hashtable()
        Dim oRS As SAPbobsCOM.Recordset
        Dim xSQL As String = ""

        Try
            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""
            xSQL = xSQL & vbCrLf & " DO "
            xSQL = xSQL & vbCrLf & " BEGIN "
            xSQL = xSQL & vbCrLf & "    DECLARE IN_TABLENM   NVARCHAR(20) := '" & strTableNM & "'; "
            xSQL = xSQL & vbCrLf & "    DECLARE OUT_DOCENT   NVARCHAR(20) := ''; "
            xSQL = xSQL & vbCrLf & "    DECLARE OUT_DOCNUM   NVARCHAR(50) := ''; "
            xSQL = xSQL & vbCrLf & "    CALL WJS_SP_AD_GETUDOKEY(IN_TABLENM, :OUT_DOCENT, :OUT_DOCNUM); "
            xSQL = xSQL & vbCrLf & "    SELECT :OUT_DOCENT AS ""DocEntry"", :OUT_DOCNUM AS ""DocNum"" FROM DUMMY; "
            xSQL = xSQL & vbCrLf & " END "

            oRS.DoQuery(xSQL)

            If Not oRS.EoF Then
                hsTable.Add("DocEntry", Trim(oRS.Fields.Item("DocEntry").Value.ToString))
                hsTable.Add("DocNum", Trim(oRS.Fields.Item("DocNum").Value.ToString))
            Else
                hsTable.Add("DocEntry", "")
                hsTable.Add("DocNum", "")
            End If

        Catch ex As Exception
            If (Not hsTable.ContainsKey("DocEntry")) Then
                hsTable.Add("DocEntry", "")
            End If
            If (Not hsTable.ContainsKey("DocNum")) Then
                hsTable.Add("DocNum", "")
            End If
        Finally
            oRS = Nothing
        End Try

        Return hsTable
    End Function


    ''' <summary>
    ''' GetUDOKey
    ''' </summary>
    ''' <param name="strTableNM">UDO 테이블 명</param>
    ''' <param name="ObjType">SAPbobsCOM.BoUDOObjType</param>
    ''' <returns>DocEntry, DocNum(Code) 리턴</returns>
    ''' <remarks>신규로 생성될 DocEntry, DocNum(Code) 조회</remarks>
    Public Function GetUDOKey(ByVal strTableNM As String, ByVal ObjType As SAPbobsCOM.BoUDOObjType) As Hashtable
        Dim strDocEnt As String = ""
        Dim hsTable As Hashtable = New Hashtable()
        Dim xSQL As String = ""

        Try

            hsTable = PLS_COMMON.GetUDOKey(strTableNM)

            If ObjType = BoUDOObjType.boud_MasterData Then
                hsTable.Remove("DocNum")
                hsTable.Add("DocNum", PLS_COMMON.GetMaxCode(strTableNM))
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("GetUDOKey " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try

        Return hsTable
    End Function


    ''' <summary>
    ''' GetUDOKey
    ''' </summary>
    ''' <param name="strTableNM">UDO 테이블 명</param>
    ''' <param name="ObjType">SAPbobsCOM.BoUDOObjType</param>
    ''' <param name="strDocNum">DocNum 또는 Code</param>
    ''' <returns>DocEntry</returns>
    ''' <remarks>신규로 생성될 DocEntry, DocNum(Code) 조회</remarks>
    Public Function GetUDOKey(ByVal strTableNM As String, ByVal ObjType As SAPbobsCOM.BoUDOObjType, ByRef strDocNum As String) As String
        Dim strDocEnt As String = ""
        Dim hsTable As Hashtable = New Hashtable()
        Dim oRS As SAPbobsCOM.Recordset
        Dim xSQL As String = ""

        Try

            hsTable = PLS_COMMON.GetUDOKey(strTableNM, ObjType)

            strDocEnt = hsTable.Item("DocEntry").ToString()
            strDocNum = hsTable.Item("DocNum").ToString()

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("GetUDOKey " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oRS = Nothing
        End Try

        Return strDocEnt
    End Function


    '    '****************************************************************************************************
    '    '   함수명      :   SetGridTitle
    '    '   작성자      :   
    '    '   작성일      :   
    '    '   간략한 설명 :   그리드 타이틀 지정

    '    '   인수        :   '// 사용
    '    '****************************************************************************************************
    '    Public Function SetGridTitle(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As String, ByVal pColNms As String, Optional ByVal pEdCols As String = "", Optional ByVal pViCols As String = "", Optional ByVal pAffCols As String = "", Optional ByVal pAlignCols As String = "", Optional ByVal pColor1Cols As String = "", Optional ByVal pColor2Cols As String = "") As Boolean

    '        Dim cols() As String
    '        Dim colNms() As String
    '        Dim affCols() As String
    '        Dim edCols() As String
    '        Dim viCols() As String
    '        Dim alignCols() As String
    '        Dim xSql As String = ""
    '        Dim i As Integer = 0

    '        Try

    '            SetGridTitle = False

    '            cols = Split(pCols, ",")
    '            colNms = Split(pColNms, ",")

    '            If (UBound(cols) - LBound(cols) <> UBound(colNms) - LBound(colNms)) Then
    '                Exit Function
    '            End If

    '            For i = 0 To UBound(cols)

    '                If String.Compare(cols(i).ToString(), cols(i).ToUpper(), False) <> 0 Then
    '                    xSql = xSql & IIf(xSql = "", "", ",") & "'' as """ & cols(i) & """"
    '                Else
    '                    xSql = xSql & IIf(xSql = "", "", ",") & "'' as " & cols(i)

    '                End If
    '            Next

    '            xSql = IIf(xSql = "", "", "Select ") & xSql

    '#If HANA = "Y" Then
    '            xSql = CFL.GetConvertHANA(xSql)
    '#End If
    '            'xSql = xSql & " FROM DUMMY;"
    '            'End If

    '            If xSql <> "" Then
    '                Call oGrid.DataTable.ExecuteQuery(xSql)
    '                Call oGrid.DataTable.Rows.Remove(0)
    '            End If

    '            For i = LBound(cols) To UBound(cols)
    '                oGrid.Columns.Item(Trim(cols(i))).TitleObject.Caption = CFL.GetCaption(Trim(colNms(i)))
    '            Next i

    '            If pEdCols.ToString.Trim.Length > 0 Then
    '                edCols = Split(pEdCols, ",")
    '                For i = LBound(edCols) To UBound(edCols)
    '                    oGrid.Columns.Item(edCols(i)).Editable = False
    '                Next i
    '            End If

    '            If pViCols.ToString.Trim.Length > 0 Then
    '                viCols = Split(pViCols, ",")
    '                For i = LBound(viCols) To UBound(viCols)
    '                    oGrid.Columns.Item(viCols(i)).Visible = False
    '                Next i
    '            End If

    '            If pAffCols.ToString.Trim.Length > 0 Then
    '                affCols = Split(pAffCols, ",")
    '                For i = LBound(affCols) To UBound(affCols)
    '                    oGrid.Columns.Item(affCols(i)).AffectsFormMode = False
    '                Next i
    '            End If

    '            If pAlignCols.ToString.Trim.Length > 0 Then
    '                alignCols = Split(pAlignCols, ",")
    '                For i = LBound(alignCols) To UBound(alignCols)
    '                    oGrid.Columns.Item(alignCols(i)).RightJustified = True
    '                Next i
    '            End If

    '            oGrid.AutoResizeColumns()

    '            SetGridTitle = True

    '        Catch ex As Exception

    '            B1Connections.theAppl.StatusBar.SetText("SetGridTitle " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

    '        Finally

    '            cols = Nothing
    '            colNms = Nothing
    '            affCols = Nothing
    '            edCols = Nothing
    '            viCols = Nothing
    '            alignCols = Nothing

    '        End Try

    '    End Function
    '****************************************************************************************************
    '   함수명      :   SetGridTitle
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   그리드 타이틀 지정

    '   인수        :   
    '****************************************************************************************************
    Public Function SetGridTitle(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As String, ByVal pColNms As String, Optional ByVal pEdCols As String = "", Optional ByVal pViCols As String = "", Optional ByVal pAffCols As String = "", Optional ByVal pAlignCols As String = "", Optional ByVal pColor1Cols As String = "", Optional ByVal pcolor2Cols As String = "") As Boolean

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
                xSql = xSql & IIf(xSql = "", "", ",") & "'' as """ & cols(i).ToString.Trim & """ "
            Next

            xSql = IIf(xSql = "", "", "Select ") & xSql

            If xSql <> "" Then
#If HANA = "Y" Then
                xSql = xSql & " FROM DUMMY;"
#End If
                Call oGrid.DataTable.ExecuteQuery(xSql)
                Call oGrid.DataTable.Rows.Remove(0)
            End If

            For i = LBound(cols) To UBound(cols)
                oGrid.Columns.Item(Trim(cols(i))).TitleObject.Caption = CFL.GetCaption(Trim(colNms(i)), ModuleIni.CO) 'Trim(colNms(i))
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

            If pColor1Cols.ToString.Trim.Length > 0 Then
                alignCols = Split(pColor1Cols, ",")
                For i = LBound(alignCols) To UBound(alignCols)
                    oGrid.Columns.Item(alignCols(i)).BackColor = 12777465
                Next i
            End If

            If pcolor2Cols.ToString.Trim.Length > 0 Then
                alignCols = Split(pcolor2Cols, ",")
                For i = LBound(alignCols) To UBound(alignCols)
                    oGrid.Columns.Item(alignCols(i)).BackColor = 13624308
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
    '   함수명      :   SetGridTitle
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   그리드 타이틀 지정

    '   인수        :   
    '****************************************************************************************************
    Public Function SetGridTitle(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As String, ByVal pColNms As String, ByVal moduleini As ModuleIni, Optional ByVal pEdCols As String = "", Optional ByVal pViCols As String = "", Optional ByVal pAffCols As String = "", Optional ByVal pAlignCols As String = "") As Boolean

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
            xSql = xSql & " FROM DUMMY;"


            If xSql <> "" Then
                Call oGrid.DataTable.ExecuteQuery(xSql)
                Call oGrid.DataTable.Rows.Remove(0)
            End If

            For i = LBound(cols) To UBound(cols)
                oGrid.Columns.Item(Trim(cols(i))).TitleObject.Caption = CFL.GetCaption(Trim(colNms(i)), moduleini)
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
    Public Function BindGrid(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As String, ByVal pColNms As String, Optional ByVal pEdCols As String = "", Optional ByVal pViCols As String = "", Optional ByVal pAffCols As String = "", Optional ByVal pAlignCols As String = "", Optional ByVal pColor1Cols As String = "", Optional ByVal pColor2Cols As String = "") As Boolean
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
                oGrid.Columns.Item(Trim(cols(i))).TitleObject.Caption = CFL.GetCaption(Trim(colNms(i)))
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

            If pColor1Cols.ToString.Trim.Length > 0 Then
                alignCols = Split(pColor1Cols, ",")
                For i = LBound(alignCols) To UBound(alignCols)
                    oGrid.Columns.Item(alignCols(i)).BackColor = 12777465
                Next i
            End If

            If pColor2Cols.ToString.Trim.Length > 0 Then
                alignCols = Split(pColor2Cols, ",")
                For i = LBound(alignCols) To UBound(alignCols)
                    oGrid.Columns.Item(alignCols(i)).BackColor = 13624308
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
    '   함수명      :   BindGrid
    '   작성자      :   
    '   작성일      :   
    '   간략한 설명 :   그리드셋팅

    '   인수        :   
    '****************************************************************************************************   
    Public Function BindGrid(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As String, ByVal pColNms As String, ByVal moduleini As ModuleIni, Optional ByVal pEdCols As String = "", Optional ByVal pViCols As String = "", Optional ByVal pAffCols As String = "", Optional ByVal pAlignCols As String = "") As Boolean
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
                oGrid.Columns.Item(Trim(cols(i))).TitleObject.Caption = CFL.GetCaption(Trim(colNms(i)), moduleini)
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

            For i = 0 To oGrid.Columns.Count - 1
                oGrid.Columns.Item(i).TitleObject.Sortable = True
            Next

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


    ''' <summary>
    ''' GetQD
    ''' </summary>
    ''' <param name="strVal"></param>
    ''' <param name="strDefaultVal"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetQD(ByVal strVal As String, ByVal strDefaultVal As String) As String

        If strVal.Trim = "" Then
            GetQD = strDefaultVal
        Else
            GetQD = "N'" & strVal.Replace("'", "''").Trim & "'"
        End If

    End Function


    ''' <summary>
    ''' GetMaxCode
    ''' </summary>
    ''' <param name="strTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMaxCode(ByVal strTable As String) As String
        Dim strTemp As String = ""

        Try

            strTable = "[@" & strTable.Replace("@", "").Replace("""", "").Replace("[", "").Replace("]", "") & "]"

            'strTemp = "00000000000000000000" + (Integer.Parse(CFL.GetValue("SELECT ISNULL((SELECT MAX(CONVERT(INT,Code)) FROM " & strTable & "), 0 )")) + 1).ToString()
            'strTemp = strTemp.Substring(strTemp.Length - 20, 20)
            strTemp = CFL.GetMaxCode(strTable)

        Catch ex As Exception
            strTemp = ""
        End Try

        Return strTemp
    End Function


    ''' <summary>
    ''' GetMaxLineId 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>LineId MAX값 가지고 온다.</remarks>
    ''' 
    Public Function GetMaxLineId(ByVal DbSrc As SAPbouiCOM.DBDataSource) As Integer
        Dim intMaxLineId As Integer = 0, iLooper As Integer = 0

        Try
            If DbSrc.Size > 0 Then
                For iLooper = 0 To DbSrc.Size - 1
                    If Val(DbSrc.GetValue("LineId", iLooper)) > intMaxLineId Then
                        intMaxLineId = Val(DbSrc.GetValue("LineId", iLooper))
                    End If
                Next
            End If
        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("GetMaxLineId Error : " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try
        Return intMaxLineId
    End Function

    ''' <summary>
    ''' 크리스탈 리포트 팝업
    ''' </summary>
    ''' <param name="rpt"></param>
    ''' <remarks></remarks>
    Public Sub RptShow(ByRef rpt As WJS.COMM.Report)

        Dim strStartPath As String = System.Reflection.Assembly.GetExecutingAssembly.Location
        Dim strReportPath As String
        Dim strExportPath As String
        Dim strViewerExe As String = "\CrystalViewer.exe"
        Dim proc As Process

        Try
            strStartPath = strStartPath.Substring(0, strStartPath.LastIndexOf("\"))

            Try
                If System.Environment.Is64BitProcess = True Then strViewerExe = "\CrystalViewer_x64.exe"
            Catch ex As Exception
                strViewerExe = "\CrystalViewer.exe"
            End Try

            If (System.IO.File.Exists(strStartPath + strViewerExe)) Then

                strReportPath = rpt.crxReport.FileName

                'My.Computer.FileSystem.SpecialDirectories.Temp

                strExportPath = My.Computer.FileSystem.SpecialDirectories.Temp + strReportPath.Substring(strReportPath.LastIndexOf("\"), strReportPath.Length - strReportPath.LastIndexOf("\"))

                rpt.crxReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.CrystalReport, strExportPath)

                proc = New System.Diagnostics.Process
                proc.StartInfo.FileName = strStartPath + strViewerExe
                proc.StartInfo.Arguments = " """ & strExportPath & """"
                proc.Start()

            Else
                'Report Viewer 실행
                rpt.ShowDialog(CFL.GetSBOWindow())
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("RptShow Error : " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            proc = Nothing

            If IsNothing(rpt) = False Then
                rpt.crxReport.Close()
                rpt.crxReport.Dispose()
                rpt.Close()
                rpt.Dispose()
                rpt = Nothing
                GC.Collect()
            End If

        End Try

    End Sub


    ''' <summary>
    ''' GridCheckAll
    ''' </summary>
    ''' <param name="oGrid"></param>
    ''' <param name="strColumnID"></param>
    ''' <remarks></remarks>
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


    ''' <summary>
    ''' 전표번호 채번로직
    ''' </summary>
    ''' <param name="strPLANT">공장</param>
    ''' <param name="strNumGroup">채번그룹오브잭트(공통코드정의)</param>
    ''' <param name="strRegDt">채번일자</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMaxNum(ByVal strPLANT As String, ByVal strNumGroup As String, ByVal strRegDt As String) As String
        GetMaxNum = ""

        Dim strReturnNum As String = ""

        Try

            '추가문서번호 채번로직 호출
            strReturnNum = CFL.GetValue("EXEC [WJS_SP_MAXNUM] '" + strNumGroup + "','" + strPLANT + "','" + strRegDt + "'")

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try

        Return GetMaxNum

    End Function

    ''' <summary>
    ''' 전표번호 채번로직
    ''' </summary>
    ''' <param name="strPLANT">공장</param>
    ''' <param name="strNumGroup">채번그룹오브잭트(공통코드정의)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMaxNum(ByVal strPLANT As String, ByVal strNumGroup As String) As String
        GetMaxNum = ""

        Dim strRegDt As String = ""
        Dim strReturnNum As String = ""

        Try

            '추가문서번호 채번로직 호출
            strRegDt = CFL.GetValue("SELECT CONVERT(NVARCHAR(10), GETDATE(), 112) AS DT")
            strReturnNum = CFL.GetValue("EXEC [WJS_SP_MAXNUM] '" + strNumGroup + "','" + strPLANT + "','" + strRegDt + "'")

            GetMaxNum = strReturnNum

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try

        Return GetMaxNum

    End Function


    ''' <summary>
    ''' 전표번호 채번로직
    ''' </summary>
    ''' <param name="strNumGroup">채번그룹오브잭트(공통코드정의)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMaxNum(ByVal strNumGroup As String) As String
        GetMaxNum = ""

        Dim strRegDt As String = ""
        Dim strReturnNum As String = ""

        Try

            strRegDt = CFL.GetValue("SELECT CONVERT(NVARCHAR(10),GETDATE(),112) AS DT")
            strReturnNum = CFL.GetValue("EXEC [WJS_SP_MAXNUM] '" + strNumGroup + "','','" + strRegDt + "'")

            GetMaxNum = strReturnNum

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try

        Return GetMaxNum

    End Function


    ''' <summary>
    ''' GetTODAY
    ''' </summary>
    ''' <param name="oDBDataH"></param>
    ''' <param name="DBField"></param>
    ''' <remarks></remarks>
    Public Sub GetTODAY(ByVal oDBDataH As SAPbouiCOM.DBDataSource, ByVal DBField As String)

        Dim dtInfo As System.Globalization.DateTimeFormatInfo = New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.ToString(), False).DateTimeFormat
        Try

            'oDBDataH.SetValue("U_DOCDT", 0, Left(Date.Parse(CFL.GetSystemDate).ToString(dtInfo.ShortDatePattern).Replace(dtInfo.DateSeparator, ""), 8)) '& "01")
            'oDBDataH.SetValue("U_TAXDT", 0, Left(Date.Parse(CFL.GetSystemDate).ToString(dtInfo.ShortDatePattern).Replace(dtInfo.DateSeparator, ""), 8)) '& "01")
            'oDBDataH.SetValue("U_DUEDT", 0, Left(Date.Parse(CFL.GetSystemDate).ToString(dtInfo.ShortDatePattern).Replace(dtInfo.DateSeparator, ""), 8)) '& "01")

            oDBDataH.SetValue(DBField, 0, Left(Date.Parse(CFL.GetSystemDate).ToString(dtInfo.ShortDatePattern).Replace(dtInfo.DateSeparator, ""), 8)) '& "01")

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetTODAY Error : " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            dtInfo = Nothing
        End Try
    End Sub


    ''' <summary>
    ''' GetDateStringYMD
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDateStringYMD(ByVal dt As Object) As String

        Try
            Dim strDate As String = ""

            If (dt Is Nothing) Then

            ElseIf dt.GetType().Name = "String" Then
                If IsDate(dt) Then
                    strDate = DateTime.Parse(CDate(dt)).ToString("yyyyMMdd")
                Else
                    strDate = dt
                End If
            ElseIf dt.GetType().Name = "DateTime" Then
                strDate = DateTime.Parse(dt).ToString("yyyyMMdd")
            End If

            Return strDate

        Catch ex As Exception
            Return ""
        End Try



    End Function


    ''' <summary>
    ''' GetDateStringYMD
    ''' </summary>
    ''' <param name="strDt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDateStringYMD(ByVal strDt As String) As String

        Try
            Dim strDate As String = ""

            If strDt.GetType().Name = "String" Then
                If IsDate(strDt) Then
                    strDate = DateTime.Parse(CDate(strDt)).ToString("yyyyMMdd")
                Else
                    strDate = strDt
                End If

            End If

            Return strDate

        Catch ex As Exception
            Return ""
        End Try



    End Function


    ''' <summary>
    ''' GetNowDateString
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNowDateString() As String

        Dim dt As DateTime = DateTime.Now

        Dim strYear As String = dt.Year.ToString()
        Dim strMonth As String = dt.Month.ToString()
        Dim strDate As String = dt.Day.ToString()

        If (strMonth.Length = 1) Then
            strMonth = "0" + strMonth
        End If

        If (strDate.Length = 1) Then
            strDate = "0" + strDate
        End If

        Return strYear + strMonth + strDate

    End Function


    ''' <summary>
    ''' Mode_Change
    ''' </summary>
    ''' <param name="Gubun"></param>
    ''' <param name="oForm"></param>
    ''' <remarks></remarks>
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


    ''' <summary>
    ''' RemoveLastSplit
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RemoveLastSplit(ByRef str As String) As String

        Try

            If str.Length > 0 Then
                str = str.Substring(0, str.Length - 1)
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        End Try

        Return str
    End Function

    Public Sub SetGrdColumnNumber(ByRef oGrid As SAPbouiCOM.Grid, ByVal strColQty As String, ByVal strColAmt As String, ByVal strColPrc As String, ByVal strColRate As String)

        Dim xMLDoc As Xml.XmlDocument = New Xml.XmlDocument()
        xMLDoc.LoadXml(oGrid.DataTable.SerializeAsXML(BoDataTableXmlSelect.dxs_All))
        Dim arrColQty As ArrayList = New ArrayList()
        If (Not strColQty Is Nothing) Then arrColQty.AddRange(strColQty.Replace(" ", "").Split(",")) '수량
        Dim arrColAmt As ArrayList = New ArrayList()
        If (Not strColAmt Is Nothing) Then arrColAmt.AddRange(strColAmt.Replace(" ", "").Split(",")) '금액
        Dim arrColPrc As ArrayList = New ArrayList()
        If (Not strColPrc Is Nothing) Then arrColPrc.AddRange(strColPrc.Replace(" ", "").Split(",")) '단가
        Dim arrColRate As ArrayList = New ArrayList()
        If (Not strColRate Is Nothing) Then arrColRate.AddRange(strColRate.Replace(" ", "").Split(",")) '비율


        For Each node As XmlNode In xMLDoc.GetElementsByTagName("Column")
            If (arrColQty.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Quantity
                node.Attributes("MaxLength").InnerText = "0"
            ElseIf (arrColAmt.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Sum
                node.Attributes("MaxLength").InnerText = "0"
            ElseIf (arrColPrc.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Price
                node.Attributes("MaxLength").InnerText = "0"
            ElseIf (arrColRate.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Rate
                node.Attributes("MaxLength").InnerText = "0"
            End If

        Next
        oGrid.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_All, xMLDoc.InnerXml)

        If (ExistsColGrid(oGrid, "RowsHeader")) Then
            oGrid.Columns.Item("RowsHeader").Width = 20
        End If

        xMLDoc = Nothing

    End Sub


    ''' <summary>
    ''' Custom 화면에서 Serial/Batch 관리를 하기위한 임시테이블 추가
    ''' </summary>
    ''' <param name="oForm"></param>
    ''' <remarks></remarks>
    Public Sub CreateTempSNTB(ByRef oForm As SAPbouiCOM.Form)

        'OITL 임시 테이블 추가
        Dim oDT As SAPbouiCOM.DataTable

        'ITL1 임시 테이블 추가
        oDT = oForm.DataSources.DataTables.Add("ITL1")
        oDT.Columns.Add("ManagedBy", BoFieldsType.ft_Integer)
        oDT.Columns.Add("LogEntry", BoFieldsType.ft_Integer)
        oDT.Columns.Add("ItemCode", BoFieldsType.ft_AlphaNumeric, 20)
        oDT.Columns.Add("SysNumber", BoFieldsType.ft_Integer)
        oDT.Columns.Add("Quantity", BoFieldsType.ft_Quantity)
        oDT.Columns.Add("DocEntry", BoFieldsType.ft_Integer)
        oDT.Columns.Add("DocLine", BoFieldsType.ft_Integer)
        oDT.Columns.Add("DocType", BoFieldsType.ft_Integer)

    End Sub


    ''' <summary>
    ''' 일련번호 생성을 위한 템프 테이블
    ''' </summary>
    ''' <param name="oForm"></param>
    ''' <remarks></remarks>
    Public Sub CreateTempOSRN(ByRef oForm As SAPbouiCOM.Form)

        'OSRN 임시 테이블 추가
        Dim oDT As SAPbouiCOM.DataTable
        oDT = oForm.DataSources.DataTables.Add("OSRN")

        oDT.Columns.Add("DocLine", BoFieldsType.ft_AlphaNumeric, 20)    '전표라인번호
        oDT.Columns.Add("ItemCode", BoFieldsType.ft_AlphaNumeric, 20)   '품목 번호
        oDT.Columns.Add("SysNumber", BoFieldsType.ft_AlphaNumeric, 11)  '시스템 번호
        oDT.Columns.Add("DistNumber", BoFieldsType.ft_AlphaNumeric, 32) '일련번호
        oDT.Columns.Add("MnfSerial", BoFieldsType.ft_AlphaNumeric, 32)  '제조업체 일련번호
        oDT.Columns.Add("LotNumber", BoFieldsType.ft_AlphaNumeric, 32)  '로트 번호
        oDT.Columns.Add("ExpDate", BoFieldsType.ft_Date, 8)             '만료일
        oDT.Columns.Add("MnfDate", BoFieldsType.ft_Date, 8)             '제조일
        oDT.Columns.Add("InDate", BoFieldsType.ft_Date, 8)              '입력일
        oDT.Columns.Add("GrntStart", BoFieldsType.ft_Date, 8)           '제조업체 보증 시작일
        oDT.Columns.Add("GrntExp", BoFieldsType.ft_Date, 8)             '제조업체 보증 종료일
        oDT.Columns.Add("CreateDate", BoFieldsType.ft_Date, 8)          '생성일
        oDT.Columns.Add("Location", BoFieldsType.ft_AlphaNumeric, 100)  '위치
        oDT.Columns.Add("Status", BoFieldsType.ft_AlphaNumeric, 1)      '상태
        oDT.Columns.Add("Notes", BoFieldsType.ft_Text, 64000)           '세부사항
        oDT.Columns.Add("DataSource", BoFieldsType.ft_AlphaNumeric, 1)  '데이터 소스
        oDT.Columns.Add("UserSign", BoFieldsType.ft_AlphaNumeric, 6)    '사용자 서명
        oDT.Columns.Add("Transfered", BoFieldsType.ft_AlphaNumeric, 1)  '전송
        oDT.Columns.Add("Instance", BoFieldsType.ft_AlphaNumeric, 6)    '인스턴스
        oDT.Columns.Add("AbsEntry", BoFieldsType.ft_AlphaNumeric, 11)   'abs entry
        oDT.Columns.Add("ObjType", BoFieldsType.ft_AlphaNumeric, 20)    'object type
        oDT.Columns.Add("itemName", BoFieldsType.ft_AlphaNumeric, 100)  '품목 내역
        oDT.Columns.Add("U_VARIANT", BoFieldsType.ft_AlphaNumeric, 20)  '가변품목번호
        oDT.Columns.Add("KeyCode", BoFieldsType.ft_AlphaNumeric, 1000)  '구분자를 위한 Key값
        oDT.Columns.Add("GBN1", BoFieldsType.ft_AlphaNumeric, 100)      '구분자1
        oDT.Columns.Add("GBN2", BoFieldsType.ft_AlphaNumeric, 100)      '구분자2
        oDT.Columns.Add("GBN3", BoFieldsType.ft_AlphaNumeric, 100)      '구분자3

    End Sub


    ''' <summary>
    ''' 배치품목여부
    ''' </summary>
    ''' <param name="strItemCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function bBatchItem(ByVal strItemCode As String) As Boolean

        If CFL.GetValue("SELECT IFNULL(A.""ManBtchNum"", 'N') FROM OITM A WHERE A.""ItemCode"" = '" & Trim(strItemCode) & "'") = "Y" Then
            Return True
        Else
            Return False
        End If

    End Function


    ''' <summary>
    ''' 배치번호 생성을 위한 템프 테이블
    ''' </summary>
    ''' <param name="oForm"></param>
    ''' <remarks></remarks>
    Public Sub CreateTempOBTN(ByRef oForm As SAPbouiCOM.Form)

        'OSRN 임시 테이블 추가
        Dim oDT As SAPbouiCOM.DataTable
        oDT = oForm.DataSources.DataTables.Add("OBTN")

        oDT.Columns.Add("DocLine", BoFieldsType.ft_AlphaNumeric, 20)    '전표라인번호
        oDT.Columns.Add("ItemCode", BoFieldsType.ft_AlphaNumeric, 20)   '품목 번호
        oDT.Columns.Add("SysNumber", BoFieldsType.ft_AlphaNumeric, 11)  '시스템 번호
        oDT.Columns.Add("DistNumber", BoFieldsType.ft_AlphaNumeric, 32) '배치 번호
        oDT.Columns.Add("MnfSerial", BoFieldsType.ft_AlphaNumeric, 32)  '배치 속성 1
        oDT.Columns.Add("LotNumber", BoFieldsType.ft_AlphaNumeric, 32)  '배치 속성 2
        oDT.Columns.Add("ExpDate", BoFieldsType.ft_Date, 8)             '만료일
        oDT.Columns.Add("MnfDate", BoFieldsType.ft_Date, 8)             '제조일
        oDT.Columns.Add("InDate", BoFieldsType.ft_Date, 8)              '입력일
        oDT.Columns.Add("GrntStart", BoFieldsType.ft_Date, 8)           '보증 시작일
        oDT.Columns.Add("GrntExp", BoFieldsType.ft_Date, 8)             '보증 종료일
        oDT.Columns.Add("CreateDate", BoFieldsType.ft_Date, 8)          '생성일
        oDT.Columns.Add("Location", BoFieldsType.ft_AlphaNumeric, 100)  '위치
        oDT.Columns.Add("Status", BoFieldsType.ft_AlphaNumeric, 1)      '상태
        oDT.Columns.Add("Notes", BoFieldsType.ft_Text, 64000)           '세부사항
        oDT.Columns.Add("DataSource", BoFieldsType.ft_AlphaNumeric, 1)  '데이터 소스
        oDT.Columns.Add("UserSign", BoFieldsType.ft_AlphaNumeric, 6)    '사용자 서명
        oDT.Columns.Add("Transfered", BoFieldsType.ft_AlphaNumeric, 1)  '전송
        oDT.Columns.Add("Instance", BoFieldsType.ft_AlphaNumeric, 6)    '인스턴스
        oDT.Columns.Add("AbsEntry", BoFieldsType.ft_AlphaNumeric, 11)   'abs entry
        oDT.Columns.Add("ObjType", BoFieldsType.ft_AlphaNumeric, 20)    'object type
        oDT.Columns.Add("itemName", BoFieldsType.ft_AlphaNumeric, 100)  '품목 내역
        oDT.Columns.Add("U_VARIANT", BoFieldsType.ft_AlphaNumeric, 20)  '가변품목번호
        oDT.Columns.Add("QTY", BoFieldsType.ft_AlphaNumeric, 20)        '수량
        oDT.Columns.Add("KeyCode", BoFieldsType.ft_AlphaNumeric, 1000)  '구분자를 위한 Key값
        oDT.Columns.Add("GBN1", BoFieldsType.ft_AlphaNumeric, 100)      '구분자1
        oDT.Columns.Add("GBN2", BoFieldsType.ft_AlphaNumeric, 100)      '구분자2
        oDT.Columns.Add("GBN3", BoFieldsType.ft_AlphaNumeric, 100)      '구분자3

    End Sub


    ''' <summary>
    ''' Custom 화면에서 Bin 위치 관리를 하기위한 임시테이블 추가
    ''' </summary>
    ''' <param name="oForm"></param>
    ''' <remarks></remarks>
    Public Sub CreateTempOBTL(ByRef oForm As SAPbouiCOM.Form)

        'OBTL 임시 테이블 추가
        Dim oDT As SAPbouiCOM.DataTable

        'OBTL 임시 테이블 추가 
        oDT = oForm.DataSources.DataTables.Add("OBTL")
        oDT.Columns.Add("AbsEntry", BoFieldsType.ft_Integer)
        oDT.Columns.Add("ManagedBy", BoFieldsType.ft_Integer)
        oDT.Columns.Add("BinAbs", BoFieldsType.ft_Integer)
        oDT.Columns.Add("BinCode", BoFieldsType.ft_AlphaNumeric, 228)
        oDT.Columns.Add("SnBMDAbs", BoFieldsType.ft_Integer)
        oDT.Columns.Add("Quantity", BoFieldsType.ft_Quantity)
        oDT.Columns.Add("ITLEntry", BoFieldsType.ft_Integer)
        oDT.Columns.Add("DocEntry", BoFieldsType.ft_AlphaNumeric, 100)
        oDT.Columns.Add("DocLine", BoFieldsType.ft_Integer)
        oDT.Columns.Add("DocType", BoFieldsType.ft_Integer)

    End Sub


    ''' <summary>
    ''' 공장콤보세팅
    ''' </summary>
    ''' <param name="oCombo"></param>
    ''' <remarks></remarks>
    Public Function SetPLANTCombo(ByRef oCombo As SAPbouiCOM.ComboBox, Optional ByVal bDefault As Boolean = True) As String
        Dim rValue As String = ""

        Dim oRS As SAPbobsCOM.Recordset
        Dim i As Integer
        Dim xSQL As String = ""
        Dim strDfltPlant As String = ""

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""
            xSQL = xSQL & vbCrLf & " SELECT ISNULL(U_PLCD, '')		AS U_PLCD "
            xSQL = xSQL & vbCrLf & "       ,ISNULL(U_PLNM, '')		AS U_PLNM "
            xSQL = xSQL & vbCrLf & "       ,ISNULL(U_DFLTPLT, N'N')	AS U_DFLTPLT "
            xSQL = xSQL & vbCrLf & " FROM [@WJS_SAD63M] "

#If HANA = "Y" Then
            xSQL = CFL.GetConvertHANA(xSQL)
#End If

            oRS.DoQuery(xSQL)

            For i = 0 To oRS.RecordCount - 1
                oCombo.ValidValues.Add(oRS.Fields.Item("U_PLCD").Value.ToString.Trim(), oRS.Fields.Item("U_PLNM").Value.ToString().Trim())

                If Trim(oRS.Fields.Item("U_DFLTPLT").Value) = "Y" Then
                    strDfltPlant = Trim(oRS.Fields.Item("U_PLCD").Value)
                    rValue = strDfltPlant
                End If

                oRS.MoveNext()
            Next

            If (oCombo.ValidValues.Count > 0 And bDefault = True) Then

                If (strDfltPlant <> "") Then
                    oCombo.Select(strDfltPlant, BoSearchKey.psk_ByValue)
                End If

            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oRS = Nothing
        End Try

        Return rValue
    End Function

    ''' <summary>
    ''' 공장권한에 따른 Plant Combo 셋팅
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <param name="oCombo"></param>
    ''' <remarks></remarks>

    ''' <summary>
    ''' 공장콤보세팅
    ''' </summary>
    ''' <param name="oComboColumn"></param>
    ''' <remarks></remarks>
    Public Function SetPLANTCombo(ByRef oComboColumn As SAPbouiCOM.ComboBoxColumn, Optional ByVal bDefault As Boolean = True) As String
        Dim rValue As String = ""

        Dim oRS As SAPbobsCOM.Recordset
        Dim i As Integer
        Dim xSQL As String = ""
        Dim strDfltPlant As String = ""

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""
            xSQL = xSQL & vbCrLf & " SELECT ISNULL(U_PLCD, '')		AS U_PLCD "
            xSQL = xSQL & vbCrLf & "       ,ISNULL(U_PLNM, '')		AS U_PLNM "
            xSQL = xSQL & vbCrLf & "       ,ISNULL(U_DFLTPLT, N'N')	AS U_DFLTPLT "
            xSQL = xSQL & vbCrLf & " FROM [@WJS_SAD63M] "

#If HANA = "Y" Then
            xSQL = CFL.GetConvertHANA(xSQL)
#End If

            oRS.DoQuery(xSQL)

            For i = 0 To oRS.RecordCount - 1
                oComboColumn.ValidValues.Add(oRS.Fields.Item("U_PLCD").Value.ToString.Trim(), oRS.Fields.Item("U_PLNM").Value.ToString().Trim())

                If Trim(oRS.Fields.Item("U_DFLTPLT").Value) = "Y" Then
                    strDfltPlant = Trim(oRS.Fields.Item("U_PLCD").Value)
                    rValue = strDfltPlant
                End If

                oRS.MoveNext()
            Next

            'If (oComboColumn.ValidValues.Count > 0 And bDefault = True) Then

            '    If (strDfltPlant <> "") Then
            '        oComboColumn.
            '    End If

            'End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oRS = Nothing
        End Try

        Return rValue
    End Function




    ''' <summary>
    ''' 통화콤보세팅
    ''' </summary>
    ''' <param name="oComboBox"></param>
    ''' <remarks></remarks>
    Public Function SetCurrencyCombo(ByRef oComboBox As SAPbouiCOM.ComboBox, Optional ByVal bDefault As Boolean = True) As String
        Dim rValue As String = ""

        Dim oRS As SAPbobsCOM.Recordset
        Dim i As Integer
        Dim xSQL As String = ""
        Dim strDfltPlant As String = ""

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""
            xSQL = xSQL & vbCrLf & " SELECT     ""CurrCode"" AS CODE	 "
            xSQL = xSQL & vbCrLf & "       ,	""CurrName"" AS NAME "
            xSQL = xSQL & vbCrLf & " FROM OCRN "

#If HANA = "Y" Then
            xSQL = CFL.GetConvertHANA(xSQL)
#End If

            oRS.DoQuery(xSQL)

            For i = 0 To oRS.RecordCount - 1
                oComboBox.ValidValues.Add(oRS.Fields.Item("CODE").Value.ToString.Trim(), oRS.Fields.Item("NAME").Value.ToString().Trim())
                strDfltPlant = Trim(oRS.Fields.Item("CODE").Value)
                rValue = strDfltPlant
                oRS.MoveNext()
            Next

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oRS = Nothing
        End Try

        Return rValue
    End Function




    ''' <summary>
    ''' 세금그룹콤보세팅
    ''' </summary>
    ''' <param name="oComboBox"></param>
    ''' <remarks></remarks>
    Public Function SetVATGCombo(ByRef oComboBox As SAPbouiCOM.ComboBox, Optional ByVal bDefault As Boolean = True) As String
        Dim rValue As String = ""

        Dim oRS As SAPbobsCOM.Recordset
        Dim i As Integer
        Dim xSQL As String = ""
        Dim strDfltPlant As String = ""

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""
            xSQL = xSQL & vbCrLf & " SELECT ""Code"" AS CODE	 "
            xSQL = xSQL & vbCrLf & "       ,""Name"" AS NAME "
            xSQL = xSQL & vbCrLf & " FROM OVTG "

#If HANA = "Y" Then
            xSQL = CFL.GetConvertHANA(xSQL)
#End If

            oRS.DoQuery(xSQL)

            For i = 0 To oRS.RecordCount - 1
                oComboBox.ValidValues.Add(oRS.Fields.Item("CODE").Value.ToString.Trim(), oRS.Fields.Item("NAME").Value.ToString().Trim())
                strDfltPlant = Trim(oRS.Fields.Item("CODE").Value)
                rValue = strDfltPlant
                oRS.MoveNext()
            Next

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oRS = Nothing
        End Try

        Return rValue
    End Function


    ''' <summary>
    ''' QDs
    ''' </summary>
    ''' <param name="values"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function QDs(ByVal ParamArray values() As String) As String
        Dim strReturn As String = ""

        For icnt As Integer = 0 To UBound(values)
            If icnt > 0 Then
                strReturn = strReturn + ", "
            End If

            If values(icnt) Is Nothing Then
                strReturn = strReturn + "N''"
            Else
                strReturn = strReturn + "N'" & values(icnt).Replace("'", "''") & "'"
            End If

        Next
        Return strReturn
    End Function


    ''' <summary>
    ''' QDs
    ''' </summary>
    ''' <param name="oUserData"></param>
    ''' <param name="userField"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function QDs(ByVal oUserData As SAPbouiCOM.UserDataSources, ByVal ParamArray userField() As String) As String
        Dim strReturn As String = ""

        For icnt As Integer = 0 To UBound(userField)
            If icnt > 0 Then
                strReturn = strReturn + ", "
            End If

            If userField(icnt) Is Nothing Then
                strReturn = strReturn + "N''"
            Else
                strReturn = strReturn + "N'" & oUserData.Item(userField(icnt)).Value.Replace("'", "''") & "'"
            End If

        Next
        Return strReturn
    End Function


    '*********************************************
    ' Created : 2013.04.15 HMK
    ' Comment : 금액 끝전처리 방법
    ' 
    '*********************************************
    Public Function GetRoundFormat(ByVal RndAmt As Double, ByVal RndRule As String, Optional ByVal UnitRule As Integer = 0) As Double
        '// RndAmt : 끝전처리할 대상금액
        '// RndRule : 반올림규칙 (R=반올림, F=올림, C=버림)
        '// UnitRule : 몇째자리 적용인지여부 (1, 10,100,1000,10000 원)

        '--10원단위(대상금액 * 10원단위)  
        'Select Case ROUND(ROUND((105.0 + (10 * 0.5)) / 10, 0, -1) * 10, 0) - -R반올림
        'SELECT  ROUND(ROUND((105.5 + (10 * 0.9999999)) / 10 ,0,-1) * 10 ,0)  --C올림
        'SELECT  ROUND(ROUND(105.5 /10 ,0,-1) * 10,0)					--F버림

        '--100원단위(대상금액 * 100원단위)  
        'SELECT  ROUND(ROUND((1020.5 + (100 * 0.5)) / 100 ,0,-1) * 100 ,0) --R반올림
        'SELECT  ROUND(ROUND((1020.5 + (100 * 0.9999999)) / 100 ,0,-1) * 100 ,0)  --C올림
        'SELECT  ROUND(ROUND(1020.5 /100 ,0,-1) * 100,0)					--F버림

        '---소숫점자리 1째자리까지
        '        SELECT  ROUND(1498.474,1)
        '        SELECT  ROUND((1498.474 + 0.09)/0.1 ,0,-1) *0.1  --올림
        '        SELECT  ROUND(1498.474 /0.1 ,0,-1) *0.1  --버림

        Dim RoundAmt As Double
        Dim CValue As Double
        Dim DecUnit As Double

        Try
            RoundAmt = Math.Round(RndAmt, 6)

            '// 끝전처리 단위(소숫점자리 없음) 표시단위(10: 10원단위, 100: 100원단위, 1000:1000원단위)
            If UnitRule > 6 Then
                CValue = 0.999999
                Select Case RndRule
                    Case "R"    '/ 반올림
                        RoundAmt = Math.Round(Math.Truncate((RoundAmt + (UnitRule * 0.5)) / UnitRule) * UnitRule, 0)
                    Case "C"    '/ 올림
                        RoundAmt = Math.Round(Math.Truncate((RoundAmt + (UnitRule * CValue)) / UnitRule) * UnitRule, 0)
                    Case "F"    '/ 버림
                        RoundAmt = Math.Round(Math.Truncate(RoundAmt / UnitRule) * UnitRule, 0)
                End Select
            Else
                '// OADM의 금액 소숫점자릿수 표시단위(1: 0.0, 2:0.00, 3:0.000, 4:0.0000, 5:0.00000, 6:0.000000)
                Select Case UnitRule
                    Case 0
                        CValue = 0.9 : DecUnit = 1
                    Case 1
                        CValue = 0.09 : DecUnit = 0.1
                    Case 2
                        CValue = 0.009 : DecUnit = 0.01
                    Case 3
                        CValue = 0.0009 : DecUnit = 0.001
                    Case 4
                        CValue = 0.00009 : DecUnit = 0.0001
                    Case 5
                        CValue = 0.000009 : DecUnit = 0.00001
                    Case 6
                        CValue = 0.0000009 : DecUnit = 0.000001
                End Select
                Select Case RndRule
                    Case "R"    '/ 반올림
                        RoundAmt = Math.Round(RoundAmt, UnitRule)
                    Case "C"    '/ 올림
                        RoundAmt = Math.Truncate((RoundAmt + CValue) / DecUnit) * DecUnit
                    Case "F"    '/ 버림
                        RoundAmt = Math.Truncate(RoundAmt / DecUnit) * DecUnit
                End Select
            End If
            Return RoundAmt
        Catch ex As Exception
            Return RndAmt
        End Try
    End Function


    ''' <summary>
    ''' ReCalTotAmt
    ''' </summary>
    ''' <param name="oMatrix"></param>
    ''' <param name="oDBDataD"></param>
    ''' <param name="oUserData"></param>
    ''' <remarks></remarks>
    Public Sub ReCalTotAmt(ByVal oMatrix As SAPbouiCOM.Matrix, ByVal oDBDataD As SAPbouiCOM.DBDataSource, ByVal oUserData As SAPbouiCOM.UserDataSources)

        ''수량,금액(FC),금액,세액 합계 계산
        Dim i As Integer = 0, dblTotCnt As Double = 0, dblTotMnyFC As Double = 0, dblTotMny As Double = 0, dblTotTax As Double = 0

        Try

            For i = 0 To oDBDataD.Size - 1
                dblTotCnt = dblTotCnt + oDBDataD.GetValue("U_QTY", i)
                dblTotMnyFC = dblTotMnyFC + oDBDataD.GetValue("U_FCAMT", i)
                dblTotMny = dblTotMny + oDBDataD.GetValue("U_AMT", i)
                dblTotTax = dblTotTax + oDBDataD.GetValue("U_VAT", i)

            Next

            oUserData.Item("edtCNT").ValueEx = dblTotCnt
            oUserData.Item("edtMNYFC").ValueEx = dblTotMnyFC
            oUserData.Item("edtMNY").ValueEx = dblTotMny + dblTotTax
            oUserData.Item("edtTAX").ValueEx = dblTotTax

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText(CFL.GetMSG("FA0196", ModuleIni.FA) & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error) '합계계산도중 에러가 발생하였습니다. 메세지 : 
        End Try
    End Sub


    ''' <summary>
    ''' colValidate
    ''' </summary>
    ''' <param name="pval"></param>
    ''' <param name="cDBDataH"></param>
    ''' <param name="cDBDataD"></param>
    ''' <param name="oMatrix"></param>
    ''' <remarks></remarks>
    Public Sub colValidate(ByVal pval As SAPbouiCOM.ItemEvent, ByVal cDBDataH As SAPbouiCOM.DBDataSource, ByVal cDBDataD As SAPbouiCOM.DBDataSource, ByVal oMatrix As SAPbouiCOM.Matrix)

        ''수량,단가,단가(FC),금액,금액(FC)

        Dim cv_Rate As Double = cDBDataH.GetValue("U_EXRT", 0)
        Dim cForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.Item(pval.FormUID)

        Dim cv_Qty As Double = 0            ''수량
        Dim cv_Prc As Double = 0            ''단가
        Dim cv_FCPrc As Double = 0          ''단가(FC)
        Dim cv_VatRate As Double = 0        ''부가세율
        Dim cv_TaxRndRule As String = "R"     ''세금반올림규칙
        Dim cv_Round As Integer = CDbl(CFL.GetValue("SELECT ISNULL(SUMDEC,0) FROM OADM")) '통화
        Dim cv_RoundFc As Integer = CDbl(CFL.GetValue("SELECT ISNULL(PRICEDEC,0) FROM OADM")) '외화
        Try

            oMatrix.FlushToDataSource()
            'cDBDataD.Offset = pval.Row - 1

            cv_Qty = cDBDataD.GetValue("U_QTY", pval.Row - 1)

            If pval.FormType <> "2104000012" Then
                cv_Prc = cDBDataD.GetValue("U_PRC", pval.Row - 1)
                cv_FCPrc = cDBDataD.GetValue("U_FCPRC", pval.Row - 1)
            End If

            cv_VatRate = CDbl(Val(CFL.GetValue("SELECT Rate FROM OVTG WHERE Code='" & cDBDataD.GetValue("U_VATGRP", pval.Row - 1) & "' ")) / 100)
            cv_TaxRndRule = CFL.GetValue("SELECT TaxRndRule FROM OADM ")

            Select Case pval.ColUID
                Case "colQTY"   '수량* 단가(FC) = 금액(FC) , 수량 *단가 = 금액 ,세액 = 금액 * 부가세율

                    If pval.FormType = "2104000009" Then        ''자산금액 = 수량 * 매각자산단가 , 상각누계액 = 수량 * 상각누계액단가
                        cDBDataD.SetValue("U_SALAMT", pval.Row - 1, CDbl(cv_Qty * cDBDataD.GetValue("U_SALPRC", pval.Row - 1)))
                        cDBDataD.GetValue("U_SDEPAMT", pval.Row - 1)
                        cDBDataD.SetValue("U_SDEPAMT", pval.Row - 1, CDbl(cv_Qty * cDBDataD.GetValue("U_SDEPPRC", pval.Row - 1)))
                        cDBDataD.SetValue("U_FCAMT", pval.Row - 1, Math.Round(CDbl(cv_Qty * cv_FCPrc), cv_RoundFc))
                        cDBDataD.SetValue("U_AMT", pval.Row - 1, Math.Round(CDbl(cv_Prc * cv_Qty), cv_Round))
                        '   cDBDataD.SetValue("U_VAT", pval.Row - 1, CDbl(cv_Prc * cv_Qty * cv_VatRate))
                        cDBDataD.SetValue("U_VAT", pval.Row - 1, GetRoundFormat(CDbl(cv_Prc * cv_Qty * cv_VatRate), cv_TaxRndRule, 0))

                    ElseIf pval.FormType = "2104000012" Then        ''자산금액 = 수량 * 폐기자산단가 , 상각누계액 = 수량 * 상각누계액단가

                        cDBDataD.SetValue("U_DDEPAMT", pval.Row - 1, CDbl(cv_Qty * cDBDataD.GetValue("U_DDEPPRC", pval.Row - 1)))
                        cDBDataD.SetValue("U_DISAMT", pval.Row - 1, CDbl(cv_Qty * cDBDataD.GetValue("U_DISPRC", pval.Row - 1)))

                    Else
                        cDBDataD.SetValue("U_FCAMT", pval.Row - 1, Math.Round(CDbl(cv_Qty * cv_FCPrc), cv_RoundFc))
                        cDBDataD.SetValue("U_AMT", pval.Row - 1, Math.Round(CDbl(cv_Prc * cv_Qty), cv_Round))
                        ' cDBDataD.SetValue("U_VAT", pval.Row - 1, CDbl(cv_Prc * cv_Qty * cv_VatRate))
                        cDBDataD.SetValue("U_VAT", pval.Row - 1, GetRoundFormat(CDbl(cv_Prc * cv_Qty * cv_VatRate), cv_TaxRndRule, 0))
                    End If

                Case "colAMT"
                    'cDBDataD.SetValue("U_VAT", pval.Row - 1, CDbl(cDBDataD.GetValue("U_AMT", pval.Row - 1)) * CDbl(cv_VatRate))
                    cDBDataD.SetValue("U_VAT", pval.Row - 1, GetRoundFormat(CDbl(cDBDataD.GetValue("U_AMT", pval.Row - 1)) * CDbl(cv_VatRate), cv_TaxRndRule, 0))

                Case "colPRC"
                    cDBDataD.SetValue("U_AMT", pval.Row - 1, Math.Round(CDbl(cv_Prc * cv_Qty), cv_Round))
                    ' cDBDataD.SetValue("U_VAT", pval.Row - 1, CDbl(cv_Prc * cv_Qty * cv_VatRate))
                    cDBDataD.SetValue("U_VAT", pval.Row - 1, GetRoundFormat(CDbl(cv_Prc * cv_Qty * cv_VatRate), cv_TaxRndRule, 0))
                Case "colFCPRC" '단가(FC) *수량 = 금액(FC) , 
                    If checkCUR(cDBDataH) Then
                        cDBDataD.SetValue("U_FCAMT", pval.Row - 1, Math.Round(CDbl(cv_Qty * cv_FCPrc), cv_RoundFc))
                        cDBDataD.SetValue("U_PRC", pval.Row - 1, cv_FCPrc * cv_Rate)
                        cDBDataD.SetValue("U_AMT", pval.Row - 1, Math.Round(CDbl(cv_Qty * cv_FCPrc * cv_Rate), cv_Round))
                        ' cDBDataD.SetValue("U_VAT", pval.Row - 1, CDbl(cv_Qty * cv_FCPrc * cv_Rate * cv_VatRate))
                        cDBDataD.SetValue("U_VAT", pval.Row - 1, GetRoundFormat(CDbl(cv_Qty * cv_FCPrc * cv_Rate * cv_VatRate), cv_TaxRndRule, 0))
                    End If

                Case "colFCAMT"
                    If checkCUR(cDBDataH) Then
                        If pval.FormType = "2104000012" Then

                            cDBDataD.SetValue("U_AMT", pval.Row - 1, Math.Round(CDbl(cv_Rate * cDBDataD.GetValue("U_FCAMT", pval.Row - 1)), cv_Round))
                            'cDBDataD.SetValue("U_VAT", pval.Row - 1, CDbl(cv_VatRate * cv_Rate * cDBDataD.GetValue("U_FCAMT", pval.Row - 1)))
                            cDBDataD.SetValue("U_VAT", pval.Row - 1, GetRoundFormat(CDbl(cv_VatRate * cv_Rate * cDBDataD.GetValue("U_FCAMT", pval.Row - 1)), cv_TaxRndRule, 0))
                        Else
                            cDBDataD.SetValue("U_AMT", pval.Row - 1, Math.Round(CDbl(cv_Qty * cv_FCPrc * cv_Rate), cv_Round))
                            ' cDBDataD.SetValue("U_VAT", pval.Row - 1, CDbl(cv_Qty * cv_FCPrc * cv_Rate * cv_VatRate))
                            cDBDataD.SetValue("U_VAT", pval.Row - 1, GetRoundFormat(CDbl(cv_Qty * cv_FCPrc * cv_Rate * cv_VatRate), cv_TaxRndRule, 0))
                        End If
                    End If

                Case "colDDEPPRC"
                    cDBDataD.SetValue("U_DDEPAMT", pval.Row - 1, CDbl(cv_Qty * cDBDataD.GetValue("U_DDEPPRC", pval.Row - 1)))

                Case "colDISPRC"

                    cDBDataD.SetValue("U_DISAMT", pval.Row - 1, CDbl(cv_Qty * cDBDataD.GetValue("U_DISPRC", pval.Row - 1)))
                Case "colVATGRP"
                    'cDBDataD.SetValue("U_VAT", pval.Row - 1, CDbl(cDBDataD.GetValue("U_AMT", pval.Row - 1)) * CDbl(cv_VatRate))
                    cDBDataD.SetValue("U_VAT", pval.Row - 1, GetRoundFormat(CDbl(cDBDataD.GetValue("U_AMT", pval.Row - 1)) * CDbl(cv_VatRate), cv_TaxRndRule, 0))
            End Select

            cDBDataD.Offset = pval.Row - 1
            oMatrix.SetLineData(pval.Row)
            'oMatrix.LoadFromDataSource()
            ReCalTotAmt(oMatrix, cDBDataD, cForm.DataSources.UserDataSources)

        Catch ex As Exception

            cForm = Nothing
            B1Connections.theAppl.StatusBar.SetText(CFL.GetMSG("FA0198", ModuleIni.FA) & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error) '계산도중 에러가 발생했습니다.메세지 : 
        Finally

            cForm = Nothing

        End Try
    End Sub


    ''' <summary>
    ''' checkCUR
    ''' </summary>
    ''' <param name="cDBDataH"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function checkCUR(ByVal cDBDataH As SAPbouiCOM.DBDataSource) As Boolean

        ''통화 
        Dim Cur As String = cDBDataH.GetValue("U_CURR", 0)
        Dim LCur As String = CFL.GetValue("SELECT MAinCurncy FROM OADM")

        Try
            checkCUR = False

            If Cur = "" Then
                Return False
            End If

            If LCur <> Cur Then
                Return True
            End If

            Return checkCUR

        Catch ex As Exception
            checkCUR = False
            B1Connections.theAppl.StatusBar.SetText(CFL.GetMSG("FA0197", ModuleIni.FA) & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error) '통화체크도중 발생했습니다.메세지 : 
        Finally
        End Try
    End Function


    ''' <summary>
    ''' SetRowNum
    ''' </summary>
    ''' <param name="oGrid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetRowNum(ByVal oGrid As SAPbouiCOM.Grid) As Boolean

        Try

            If oGrid.DataTable Is Nothing Then Return True

            For iRow As Integer = 0 To oGrid.Rows.Count - 1

                oGrid.RowHeaders.SetText(iRow, CStr(iRow + 1))

            Next

            Return True
        Catch ex As Exception
            oApplication.StatusBar.SetText("Common Function Error(SetRowNum) : " & ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning)
            Return False

        End Try
    End Function


    ''' <summary>
    ''' RowAdd
    ''' </summary>
    ''' <param name="oForm"></param>
    ''' <param name="oMatrix"></param>
    ''' <param name="DbSrc"></param>
    ''' <remarks></remarks>
    Public Sub RowAdd(ByVal oForm As SAPbouiCOM.Form, ByVal oMatrix As SAPbouiCOM.Matrix, ByVal DbSrc As SAPbouiCOM.DBDataSource)

        Try
            oForm.Freeze(True)

            DbSrc.Clear()
            oMatrix.AddRow(1)
            oMatrix.FlushToDataSource()

            Dim i As Integer
            For i = 0 To DbSrc.Fields.Count - 1
                If (Left(DbSrc.Fields.Item(i).Name, 2) = "U_" Or DbSrc.Fields.Item(i).Name = "LineId") Then
                    DbSrc.SetValue(i, oMatrix.VisualRowCount - 1, "")
                End If

            Next

            DbSrc.Offset = oMatrix.VisualRowCount - 1
            oMatrix.SetLineData(oMatrix.VisualRowCount)

            'oMatrix.LoadFromDataSource()
            oMatrix.SelectRow(oMatrix.VisualRowCount, True, False)

            If oForm.Mode = BoFormMode.fm_OK_MODE Then
                oForm.Mode = BoFormMode.fm_UPDATE_MODE
            End If

            oForm.Freeze(False)

        Catch
            oForm.Freeze(False)
            B1Connections.theAppl.StatusBar.SetText("OnAfterRowDataMenu_Add " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        End Try


    End Sub


    ''' <summary>
    ''' RowDelete
    ''' </summary>
    ''' <param name="oForm"></param>
    ''' <param name="oMatrix"></param>
    ''' <remarks></remarks>
    Public Sub RowDelete(ByVal oForm As SAPbouiCOM.Form, ByVal oMatrix As SAPbouiCOM.Matrix)

        Dim i As Integer

        Try

            '제거를 눌러도 화면에서만 사라지고 실제 메트릭스에서는 사라지지 않음. 
            '강제로 한 줄 추가 후 제거하면 반영되어 루틴을 추가함.
            oForm.Freeze(True)

            oMatrix.AddRow()
            i = oMatrix.VisualRowCount

            Call oMatrix.DeleteRow(i)
            oMatrix.FlushToDataSource()

            oForm.Freeze(False)


        Catch ex As Exception
            oForm.Freeze(False)
            B1Connections.theAppl.StatusBar.SetText("OnAfterRowDataMenu_Delete " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        End Try


    End Sub


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
                xSql = "select U_SMLCD, U_SMLNM from [@WJS_SAD011] where CODE = '" & strGroupCd & "' "

#If HANA = "Y" Then
                xSql = CFL.GetConvertHANA(xSql)
#End If
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

    Public Sub SetDefaultPlantUserFild(ByRef oForm As SAPbouiCOM.Form, ByVal strPLCD_FLDNM As String, ByVal strPLNM_FLDNM As String)

        ' Default 공장셋팅을 위해. 사용자정의 필드화면이 띄워 졌는지 확인한다.
        If (Not B1Connections.theAppl.Menus.Item("6913").Checked) Then
            B1Connections.theAppl.Menus.Item("6913").Activate()
        End If

        Dim cForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.GetFormByTypeAndCount(oForm.TypeEx _
                                                    , oForm.TypeCount)

        If (cForm Is Nothing) Then '사용자정의 필드화면을 가져와서 없으면 리턴함.
            Return
        ElseIf (Not PLS_COMMON.ChkExtItemEnable(cForm, strPLCD_FLDNM)) Then
            Return
        Else
            Dim hs As Hashtable = PLS_COMMON.GetDfltPlant()
            cForm.Items.Item(strPLCD_FLDNM).Specific.Value = hs.Item("U_PLCD").ToString()
            cForm.Items.Item(strPLNM_FLDNM).Specific.Value = hs.Item("U_PLNM").ToString()
        End If

    End Sub

    Public Function ChkExtItemEnable(ByRef oForm As SAPbouiCOM.Form, ByVal strUid As String) As Boolean

        Dim i As Integer = 0
        Dim xmlAtt As Xml.XmlAttribute
        Dim xmlDoc As Xml.XmlDocument = New Xml.XmlDocument()
        xmlDoc.LoadXml(oForm.GetAsXML())

        For Each node As Xml.XmlNode In xmlDoc.SelectNodes("Application/forms/action/form/items/action/item")
            xmlAtt = node.Attributes.ItemOf("uid") '아이템 아이디
            If (Not xmlAtt Is Nothing) Then
                If (xmlAtt.InnerText = strUid) Then
                    xmlAtt = node.Attributes.ItemOf("visible") '활성화
                    If (Not xmlAtt Is Nothing) Then
                        If (xmlAtt.InnerText = "1") Then
                            xmlAtt = node.Attributes.ItemOf("enabled") '편집가능
                            If (Not xmlAtt Is Nothing) Then
                                If (xmlAtt.InnerText = "1") Then
                                    Return True
                                End If
                            End If
                        End If
                    End If
                End If
            End If

        Next

        Return False

    End Function

    '****************************************************************************************************
    '   함수명      :   SetBplidCombo
    '   작성자      :   최양규
    '   작성일      :   
    '   간략한 설명 :   세그먼트 설정에 따른 사업장 콤보 조회
    '   인수        :   ComboObj- 콤보객체명

    '                   AllYN   - TRUE    : 전체추가 
    '                             FALSE   : 전체없음
    '                   UseYN   - TRUE    : 사용중인 사업장만
    '                             FALSE   : 모든 사업장

    '****************************************************************************************************
    Public Sub SetBplidCombo(ByVal ComboObj As SAPbouiCOM.ComboBox, ByVal AllYN As Boolean, ByVal UseYN As Boolean)

        Dim oRS As SAPbobsCOM.Recordset
        Dim xSql As String
        Dim cv_BplTP As String
        Dim i As Integer

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            For i = 1 To ComboObj.ValidValues.Count
                ComboObj.ValidValues.Remove(0, BoSearchKey.psk_Index)
            Next

#If HANA = "Y" Then
            oRS.DoQuery(CFL.GetConvertHANA("SELECT ISNULL(U_BPLTP, 'N') AS U_BPLTP FROM [@WJS_SAD00M] "))
#Else
            oRS.DoQuery("SELECT ISNULL(U_BPLTP, 'N') AS U_BPLTP FROM [@WJS_SAD00M] ")
#End If
            cv_BplTP = oRS.Fields.Item("U_BPLTP").Value

            If cv_BplTP = "Y" Then

                If UseYN Then
                    xSql = "SELECT BPLID, BPLNAME FROM OBPL WHERE DISABLED = N'N' ORDER BY BPLID"
                Else
                    xSql = "SELECT BPLID, BPLNAME FROM OBPL ORDER BY BPLID"
                End If
            Else

                xSql = "SELECT Code, Name FROM OASC WHERE SegmentId = '1' "

            End If
#If HANA = "Y" Then
            xSql = CFL.GetConvertHANA(xSql)
#End If

            oRS.DoQuery(xSql)

            If AllYN Then
                ComboObj.ValidValues.Add("", CFL.GetCaption("전체", ModuleIni.FI))
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

    Public Function EDU_CHK_AD(Optional ByVal strCode As String = "") As Boolean

        EDU_CHK_AD = False

        If PLS_COMMON.Chk_TableonDB("@WJS_SAD01M") = False Then
            ' 테이블이 존재하지 않습니다.
            CFL.COMMON_MESSAGE("!", "@WJS_SAD01M" + CFL.GetMSG("FI0249", ModuleIni.FI))
            Exit Function
        End If

        If PLS_COMMON.Chk_TableonDB("@WJS_SAD001") = False Then
            ' 테이블이 존재하지 않습니다.
            CFL.COMMON_MESSAGE("!", "@WJS_SAD001" + CFL.GetMSG("FI0249", ModuleIni.FI))
            Exit Function
        End If

        If PLS_COMMON.Chk_TableonDB("@WJS_SAD001", "U_PRTTP") = False Then
            ' 테이블의 필드가 존재하지 않습니다.
            CFL.COMMON_MESSAGE("!", "@WJS_SAD001" + CFL.GetMSG("FI0250", ModuleIni.FI) + "U_PRTTP" + CFL.GetMSG("FI0251", ModuleIni.FI))
            Exit Function
        End If

        If PLS_COMMON.Chk_TableonDB("@WJS_SAD001", "U_PRTFILE") = False Then
            ' 테이블의 필드가 존재하지 않습니다.
            CFL.COMMON_MESSAGE("!", "@WJS_SAD001" + CFL.GetMSG("FI0250", ModuleIni.FI) + "U_PRTFILE" + CFL.GetMSG("FI0251", ModuleIni.FI))
            Exit Function
        End If

        If CFL.GetValue("SELECT 1 FROM [@WJS_SAD00M]") = "" Then
            '운영>>설정>>추가설정>>환경설정을 먼저 설정바랍니다.
            CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0188", ModuleIni.FI))
            Exit Function
        End If

        If strCode <> "" Then
            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SAD001] WHERE ISNULL(U_PRTTP,'') = '" & strCode & "' AND ISNULL(U_PRTFILE,'') <> ''") = "" Then
                '운영>>설정>>추가설정>>환경설정에서 전표인쇄 레포트파일를 설정바랍니다.
                CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0189", ModuleIni.FI) + " (" + strCode + ")")
                Exit Function
            End If

            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SAD001] WHERE ISNULL(U_PRTTP,'') = '" & strCode & "' AND ISNULL(U_PRTPROC,'') <> ''") = "" Then
                '운영>>설정>>추가설정>>환경설정에서 전표인쇄 프로시저를 설정바랍니다.
                CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0190", ModuleIni.FI) + " (" + strCode + ")")
                Exit Function
            End If

        Else
            '건별, 그룹별 체크
            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SAD001] WHERE (ISNULL(U_PRTTP,'') = 'S01' AND ISNULL(U_PRTFILE,'') <> '') OR (ISNULL(U_PRTTP,'') = 'S02' AND ISNULL(U_PRTFILE,'') <> '')") = "" Then
                '운영>>설정>>추가설정>>환경설정에서 전표인쇄 레포트파일를 설정바랍니다.
                CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0189", ModuleIni.FI))
                Exit Function
            End If

            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SAD001] WHERE (ISNULL(U_PRTTP,'') = 'S01' AND ISNULL(U_PRTPROC,'') <> '') OR (ISNULL(U_PRTTP,'') = 'S02' AND ISNULL(U_PRTPROC,'') <> '')") = "" Then
                '운영>>설정>>추가설정>>환경설정에서 전표인쇄 프로시저를 설정바랍니다.
                CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0190", ModuleIni.FI))
                Exit Function
            End If
        End If

        EDU_CHK_AD = True

    End Function

    Public Function Chk_TableonDB(ByVal cv_TableName As String, Optional ByVal cv_Field1 As String = "") As Boolean

        Dim oRS As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim xSql As String = ""

        Chk_TableonDB = True

        Try


            If cv_Field1 = "" Then
                xSql = "SELECT COUNT(*) FROM SYSOBJEPLS WHERE XTYPE = 'U' AND NAME = '" & cv_TableName & "' "
            Else
                xSql = "select COUNT(*) from INFORMATION_SCHEMA.COLUMNS where table_name = '" & cv_TableName & "' and column_name= '" & cv_Field1 & "'"
            End If
#If HANA = "Y" Then
            xSql = CFL.GetConvertHANA(xSql)
#End If
            oRS.DoQuery(xSql)

            If Not oRS.EoF Then

                If oRS.Fields.Item(0).Value <= 0 Then
                    Chk_TableonDB = False
                    Exit Try
                End If
            Else
                Chk_TableonDB = False
            End If


        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("Chk_TableonDB " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing

        End Try

    End Function

    Public Function GetDfltPlant() As Hashtable
        Dim hs As Hashtable = New Hashtable()
        Dim RS As SAPbobsCOM.Recordset
        Dim xSQL As String = ""

        Try
            RS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = " select U_PLCD, U_PLNM "
            xSQL = xSQL & vbCrLf & "  from [@WJS_SAD63M]  "
            xSQL = xSQL & vbCrLf & " where isnull(U_DFLTPLT,'N') = 'Y' "
#If HANA = "Y" Then
            xSQL = CFL.GetConvertHANA(xSQL)
#End If
            RS.DoQuery(xSQL)
            RS.MoveFirst()
            hs.Add("U_PLCD", RS.Fields.Item("U_PLCD").Value.ToString.Trim())
            hs.Add("U_PLNM", RS.Fields.Item("U_PLNM").Value.ToString.Trim())
        Catch ex As Exception
            If (Not hs.ContainsKey("U_PLCD")) Then
                hs.Add("U_PLCD", "")
            End If
            If (Not hs.ContainsKey("U_PLNM")) Then
                hs.Add("U_PLNM", "")
            End If
        Finally
            RS = Nothing
        End Try

        Return hs

    End Function
    Public Function GetDfltPlant(ByVal strUserId As String) As Hashtable
        Dim hs As Hashtable = New Hashtable()
        Dim RS As SAPbobsCOM.Recordset
        Dim xSQL As String = ""

        Try
            RS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""
            'S01 전체, S02 읽기전용 , S3 없음
            xSQL = " select U_PLANT, (select U_PLNM From [@WJS_SAD63M] where U_PLCD = a.U_PLANT) as U_PLANTNM, U_DFTYN "
            xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511] a "
            xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
            xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y' and isnull(U_DFTYN,'N') = 'Y'"


#If HANA = "Y" Then
            xSQL = CFL.GetConvertHANA(xSQL)
#End If
            RS.DoQuery(xSQL)
            RS.MoveFirst()
            hs.Add("U_PLCD", RS.Fields.Item("U_PLANT").Value.ToString.Trim())
            hs.Add("U_PLNM", RS.Fields.Item("U_PLANTNM").Value.ToString.Trim())
        Catch ex As Exception
            If (Not hs.ContainsKey("U_PLCD")) Then
                hs.Add("U_PLCD", "")
            End If
            If (Not hs.ContainsKey("U_PLNM")) Then
                hs.Add("U_PLNM", "")
            End If
        Finally
            RS = Nothing
        End Try

        Return hs

    End Function

    Public Function GetLangCode() As String

        Dim v_RTNVAL As String = ""

        Try

            If B1Connections.diCompany.language = SAPbobsCOM.BoSuppLangs.ln_English Then
                v_RTNVAL = "ENG"
            ElseIf B1Connections.diCompany.language = SAPbobsCOM.BoSuppLangs.ln_English_Cy Then
                v_RTNVAL = "ENG"
            ElseIf B1Connections.diCompany.language = SAPbobsCOM.BoSuppLangs.ln_English_Gb Then
                v_RTNVAL = "ENG"
            ElseIf B1Connections.diCompany.language = SAPbobsCOM.BoSuppLangs.ln_English_Sg Then
                v_RTNVAL = "ENG"
            ElseIf B1Connections.diCompany.language = SAPbobsCOM.BoSuppLangs.ln_Chinese Then
                v_RTNVAL = "CHN"
            ElseIf B1Connections.diCompany.language = SAPbobsCOM.BoSuppLangs.ln_Korean_Kr Then
                v_RTNVAL = "KOR"
            ElseIf B1Connections.diCompany.language = SAPbobsCOM.BoSuppLangs.ln_Japanese_Jp Then
                v_RTNVAL = "JPN"
            End If

            Return v_RTNVAL
        Catch ex As Exception

        End Try
    End Function

    Public Function FI_CHK_AD(Optional ByVal strCode As String = "") As Boolean

        FI_CHK_AD = False

        If PLS_COMMON.Chk_TableonDB("@WJS_SAD01M") = False Then
            ' 테이블이 존재하지 않습니다.
            CFL.COMMON_MESSAGE("!", "@WJS_SAD01M" + CFL.GetMSG("FI0249", ModuleIni.FI))
            Exit Function
        End If

        If PLS_COMMON.Chk_TableonDB("@WJS_SAD001") = False Then
            ' 테이블이 존재하지 않습니다.
            CFL.COMMON_MESSAGE("!", "@WJS_SAD001" + CFL.GetMSG("FI0249", ModuleIni.FI))
            Exit Function
        End If

        If PLS_COMMON.Chk_TableonDB("@WJS_SAD001", "U_PRTTP") = False Then
            ' 테이블의 필드가 존재하지 않습니다.
            CFL.COMMON_MESSAGE("!", "@WJS_SAD001" + CFL.GetMSG("FI0250", ModuleIni.FI) + "U_PRTTP" + CFL.GetMSG("FI0251", ModuleIni.FI))
            Exit Function
        End If

        If PLS_COMMON.Chk_TableonDB("@WJS_SAD001", "U_PRTFILE") = False Then
            ' 테이블의 필드가 존재하지 않습니다.
            CFL.COMMON_MESSAGE("!", "@WJS_SAD001" + CFL.GetMSG("FI0250", ModuleIni.FI) + "U_PRTFILE" + CFL.GetMSG("FI0251", ModuleIni.FI))
            Exit Function
        End If

        If CFL.GetValue("SELECT 1 FROM [@WJS_SAD00M]") = "" Then
            '운영>>설정>>추가설정>>환경설정을 먼저 설정바랍니다.
            CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0188", ModuleIni.FI))
            Exit Function
        End If

        If strCode <> "" Then
            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SAD001] WHERE ISNULL(U_PRTTP,'') = '" & strCode & "' AND ISNULL(U_PRTFILE,'') <> ''") = "" Then
                '운영>>설정>>추가설정>>환경설정에서 전표인쇄 레포트파일를 설정바랍니다.
                CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0189", ModuleIni.FI) + " (" + strCode + ")")
                Exit Function
            End If

            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SAD001] WHERE ISNULL(U_PRTTP,'') = '" & strCode & "' AND ISNULL(U_PRTPROC,'') <> ''") = "" Then
                '운영>>설정>>추가설정>>환경설정에서 전표인쇄 프로시저를 설정바랍니다.
                CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0190", ModuleIni.FI) + " (" + strCode + ")")
                Exit Function
            End If

        Else
            '건별, 그룹별 체크
            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SAD001] WHERE (ISNULL(U_PRTTP,'') = 'S01' AND ISNULL(U_PRTFILE,'') <> '') OR (ISNULL(U_PRTTP,'') = 'S02' AND ISNULL(U_PRTFILE,'') <> '')") = "" Then
                '운영>>설정>>추가설정>>환경설정에서 전표인쇄 레포트파일를 설정바랍니다.
                CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0189", ModuleIni.FI))
                Exit Function
            End If

            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SAD001] WHERE (ISNULL(U_PRTTP,'') = 'S01' AND ISNULL(U_PRTPROC,'') <> '') OR (ISNULL(U_PRTTP,'') = 'S02' AND ISNULL(U_PRTPROC,'') <> '')") = "" Then
                '운영>>설정>>추가설정>>환경설정에서 전표인쇄 프로시저를 설정바랍니다.
                CFL.COMMON_MESSAGE("!", CFL.GetMSG("FI0190", ModuleIni.FI))
                Exit Function
            End If
        End If

        FI_CHK_AD = True

    End Function

    Public Function ChkYYYYMM(ByVal agDate As String, Optional ByVal DateForm As Integer = 1) As String
        Dim v_DateSp_s As String = GetDateSplit()
        Dim v_ChkDate_s As String = Replace(agDate, v_DateSp_s, "")           '체크할 일자
        '날짜유형이 일.월.년 과 같은 형태로 세팅되어 있을시 에러
        '날짜유형에 관계없이 처리 가능하도록 변경 2013.03.04 - SBO 김정환
        'Dim v_CurDate_s As String = Replace(GetDateFormat("", CFL.GetSystemDate), v_DateSp_s, "")
        Dim v_CurDate_s As String = Replace(CFL.GetNowDate(AddOnBase.Enum_Date.m_Ccyymmdd), ".", "")
        Dim v_RetDate_s As String
        Dim v_RTNVAL As String = ""

        Try

            If agDate <> "" Then
                Select Case DateForm
                    Case 1
                        If IsNumeric(agDate) Then                           '입력한 값이 13보다 작은 현재 년의 입력한 숫자를 월로 셋팅한다.
                            If CLng(agDate) < 13 And CLng(agDate) <> 0 Then
                                Return (Mid(v_CurDate_s, 1, 4) & Right(CStr(CLng(agDate) + 100), 2))
                            Else
                                If Len(agDate) = 6 Then
                                    v_ChkDate_s = v_ChkDate_s + "01"
                                ElseIf Len(agDate) = 4 Then
                                    v_ChkDate_s = "20" + v_ChkDate_s
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

        End Try
    End Function

    Public Function GetDateSplit(Optional ByVal agDate As String = "") As String

        Dim oRS As SAPbobsCOM.Recordset

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            GetDateSplit = ""
#If HANA = "Y" Then
            Call oRS.DoQuery(CFL.GetConvertHANA("SELECT DateSep FROM OADM"))
#Else
            Call oRS.DoQuery("SELECT DateSep FROM OADM")
#End If


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


        End Try

    End Function
    Enum DateType
        m_Ddmmyy = 0
        m_Ddmmccyy = 1
        m_Mmddyy = 2
        m_Mmddccyy = 3
        m_Ccyymmdd = 4
        M_YYYYMondd = 5
        m_Yymmdd = 6
    End Enum
    Public Function GetDateFormat(ByVal agDbDate As String, Optional ByVal agDate1 As String = "", Optional ByVal agDate2 As String = "") As String

        Dim oRS As SAPbobsCOM.Recordset
        Dim AryField
        Dim v_DateFormat_s As String
        Dim v_DateSep_s As String
        Dim xSql As String

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            GetDateFormat = ""

            xSql = "SELECT DateFormat, DateSep FROM OADM"

#If HANA = "Y" Then
            xSql = CFL.GetConvertHANA(xSql)
#End If
            oRS.DoQuery(xSql)

            If Not oRS.EoF Then

                v_DateFormat_s = Trim(oRS.Fields.Item(0).Value)
                v_DateSep_s = Trim(oRS.Fields.Item(1).Value)

                GetDateFormat = ""
                If agDbDate <> "" Then                                    'DB에 DATE 값 넘겨줄때

                    AryField = Split(agDbDate, Trim(oRS.Fields.Item(1).Value))

                    Select Case v_DateFormat_s
                        Case DateType.m_Ddmmyy : GetDateFormat = AryField(2) & "-" & AryField(1) & "-" & AryField(0)
                        Case DateType.m_Ddmmccyy : GetDateFormat = AryField(2) & " - " & AryField(1) & " - " & AryField(0)
                        Case DateType.m_Mmddyy : GetDateFormat = AryField(2) & " - " & AryField(0) & " - " & AryField(1)
                        Case DateType.m_Mmddccyy : GetDateFormat = AryField(2) & " - " & AryField(0) & " - " & AryField(1)
                        Case DateType.m_Yymmdd : GetDateFormat = AryField(0) & " - " & AryField(1) & " - " & AryField(2)
                        Case DateType.m_Ccyymmdd : GetDateFormat = AryField(0) & " - " & AryField(1) & " - " & AryField(2)
                            'Case DateType.m_Ddmmyyyy : GetDateFormat = AryField(2) & "-" & AryField(1) & "-" & AryField(0)
                    End Select

                ElseIf agDbDate = "" And agDate1 <> "" Then             '리스트에 DATE값 뿌려줄때

                    If agDate1 Like "*-*" Then
                        AryField = Split(agDate1, "-")
                    Else
                        AryField = Split(agDate1, ".")
                    End If

                    'AryField = Split(agDate1, "-")
                    Select Case v_DateFormat_s
                        Case DateType.m_Ddmmyy : GetDateFormat = AryField(2) & v_DateSep_s & AryField(1) & v_DateSep_s & Mid(AryField(0), 3, 2)
                        Case DateType.m_Ddmmccyy : GetDateFormat = AryField(2) & v_DateSep_s & AryField(1) & v_DateSep_s & AryField(0)
                        Case DateType.m_Mmddyy : GetDateFormat = AryField(1) & v_DateSep_s & AryField(2) & v_DateSep_s & Mid(AryField(0), 3, 2)
                        Case DateType.m_Mmddccyy : GetDateFormat = AryField(2) & v_DateSep_s & AryField(0) & v_DateSep_s & AryField(1)
                        Case DateType.m_Yymmdd : GetDateFormat = Mid(AryField(0), 3, 2) & v_DateSep_s & AryField(1) & v_DateSep_s & AryField(2)
                        Case DateType.m_Ccyymmdd : GetDateFormat = AryField(0) & v_DateSep_s & AryField(1) & v_DateSep_s & AryField(2)
                            'Case DateType.m_Ddmmyyyy : GetDateFormat = AryField(2) & v_DateSep_s & AryField(1) & v_DateSep_s & AryField(0)
                    End Select

                ElseIf agDate2 <> "" Then
                    v_DateSep_s = ""
                    'yyyymmdd
                    'yyyy LEFT(agDate2,4)
                    'yy Left(agDate2,2)
                    ''mm Mid(agDate2,3,2)
                    'dd Right(agDate2,2)
                    Select Case v_DateFormat_s
                        Case DateType.m_Ddmmyy : GetDateFormat = Right(agDate2, 2)
                        Case DateType.m_Ddmmccyy : GetDateFormat = Right(agDate2, 4)
                        Case DateType.m_Mmddyy : GetDateFormat = Right(agDate2, 2)
                        Case DateType.m_Mmddccyy : GetDateFormat = Right(agDate2, 4)

                        Case DateType.m_Yymmdd : GetDateFormat = Left(agDate2, 2)
                        Case DateType.m_Ccyymmdd : GetDateFormat = Left(agDate2, 4)
                            'Case DateType.m_Ddmmyyyy : GetDateFormat = Right(agDate2, 4) & v_DateSep_s & Mid(agDate2, 3, 2) & v_DateSep_s & Left(agDate2, 2)
                    End Select

                End If

            End If


        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetDateFormat" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing

        End Try

    End Function

    Public Function GetCheckDate(ByVal agDate As String) As Integer

        Dim oRS As SAPbobsCOM.Recordset
        Dim xSQL As String

        Try
            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            xSQL = "SELECT * FROM (SELECT ISDATE('" & agDate & "') a) as a"
#If HANA = "Y" Then
            xSQL = "SELECT WJS_FN_ISDATE('" & agDate & "') a FROM DUMMY"
#End If
            oRS.DoQuery(xSQL)
            GetCheckDate = 0
            GetCheckDate = oRS.Fields.Item(0).Value

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetCheckDate" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing

        End Try

    End Function

    ''' <summary>
    ''' 콤보 셋팅
    ''' </summary>
    ''' <param name="ComboObj">콤보객체명</param>
    ''' <param name="xSql">쿼리 구문</param>
    ''' <param name="AddEmpty">공백 추가 여부</param>
    ''' <remarks>사업장 콤보 셋팅</remarks>
    Public Sub SetCOMBO(ByVal ComboObj As SAPbouiCOM.ComboBox, ByVal xSql As String, ByVal AddEmpty As Boolean)

        Dim oRS As SAPbobsCOM.Recordset
        Dim i As Integer
        oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Try



            For i = 1 To ComboObj.ValidValues.Count
                ComboObj.ValidValues.Remove(0, BoSearchKey.psk_Index)
            Next

            oRS.DoQuery(xSql)

            If AddEmpty Then
                ComboObj.ValidValues.Add("", "")
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
            If (Not (oRS) Is Nothing) Then Marshal.ReleaseComObject(oRS)
            oRS = Nothing

        End Try

    End Sub
    Public Sub SetCOMBO(ByVal oForm As SAPbouiCOM.Form, ByVal ComboObjName As String, ByVal xSql As String, ByVal AddEmpty As Boolean)
        Call SetCOMBO(oForm.Items.Item(ComboObjName).Specific, xSql, AddEmpty)
    End Sub

    ''' <summary>
    ''' 그리스 수량,금액,가격,비율 등의 소숫점 SBO기준 셋팅
    ''' </summary>
    ''' <param name="oGrid"></param>
    ''' <param name="strColQty"></param>
    ''' <param name="strColAmt"></param>
    ''' <param name="strColPrc"></param>
    ''' <param name="strColRate"></param>
    ''' <remarks></remarks>
    Public Sub SetGrdColumnNumber(ByRef oGrid As SAPbouiCOM.Grid, ByVal strColQty As String, ByVal strColAmt As String, ByVal strColPrc As String, ByVal strColRate As String, ByVal strColPercent As String)

        Dim xMLDoc As Xml.XmlDocument = New Xml.XmlDocument()
        xMLDoc.LoadXml(oGrid.DataTable.SerializeAsXML(BoDataTableXmlSelect.dxs_All))
        Dim arrColQty As ArrayList = New ArrayList()
        If (Not strColQty Is Nothing) Then arrColQty.AddRange(strColQty.Replace(" ", "").Split(",")) '수량
        Dim arrColAmt As ArrayList = New ArrayList()
        If (Not strColAmt Is Nothing) Then arrColAmt.AddRange(strColAmt.Replace(" ", "").Split(",")) '금액
        Dim arrColPrc As ArrayList = New ArrayList()
        If (Not strColPrc Is Nothing) Then arrColPrc.AddRange(strColPrc.Replace(" ", "").Split(",")) '단가
        Dim arrColRate As ArrayList = New ArrayList()
        If (Not strColRate Is Nothing) Then arrColRate.AddRange(strColRate.Replace(" ", "").Split(",")) '비율
        Dim arrColPercent As ArrayList = New ArrayList()
        If (Not strColPercent Is Nothing) Then arrColPercent.AddRange(strColPercent.Replace(" ", "").Split(",")) '%

        For Each node As XmlNode In xMLDoc.GetElementsByTagName("Column")
            If (arrColQty.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Quantity
                node.Attributes("MaxLength").InnerText = "0"
            ElseIf (arrColAmt.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Sum
                node.Attributes("MaxLength").InnerText = "0"
            ElseIf (arrColPrc.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Price
                node.Attributes("MaxLength").InnerText = "0"
            ElseIf (arrColRate.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Rate
                node.Attributes("MaxLength").InnerText = "0"
            ElseIf (arrColPercent.Contains(node.Attributes("Uid").InnerText)) Then
                node.Attributes("Type").InnerText = BoFieldsType.ft_Percent
                node.Attributes("MaxLength").InnerText = "0"
            End If

        Next
        oGrid.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_All, xMLDoc.InnerXml)

        If (ExistsColGrid(oGrid, "RowsHeader")) Then
            oGrid.Columns.Item("RowsHeader").Width = 20
        End If


    End Sub

    ''' <summary>
    ''' 그리드 컬럼 포함여부
    ''' </summary>
    ''' <param name="oGrid"></param>
    ''' <param name="strCol"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistsColGrid(ByRef oGrid As SAPbouiCOM.Grid, ByVal strCol As String) As Boolean


        Dim i As Integer
        For i = 0 To oGrid.Columns.Count - 1
            If (oGrid.Columns.Item(i).UniqueID = strCol) Then
                Return True
            End If
        Next

        Return False


    End Function

    Public Function chkExcelExist() As Boolean

        'excel 설치여부 레지스트리 체크(BaseAddon에 있는 체크는 2003,2007버젼을 체크함.2010버젼을 체크하는것을 넣던지. 버전과 상관없이 엑셀설치여부를 체크하는 펑션을 만든후에 대체가능)
        Dim readValue = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\excel.exe", "Path", "")
        If readValue <> "" Then
            chkExcelExist = True
        Else
            chkExcelExist = False
        End If

    End Function

    '****************************************************************************************************
    '   함수명      :   ExcelImportData
    '   작성자      :   최양규
    '   작성일      :   2012.05.11
    '   간략한 설명 :   조회된 쿼리 내역을 엑셀로 출력한다.
    '   인수        :   
    '****************************************************************************************************

    Public Function ExcelImportData(ByVal xSql As String, ByVal strTitle As String) As Boolean
        '그리드를 받아서 그리드 내용 그대로 엑셀로 찍어준다. 제일 위에 제목 넣어주고

        Dim xlApp As Excel.Application
        Dim xlBook As Excel.Workbook
        Dim xlSheet As Excel.Worksheet
        Dim oRs As SAPbobsCOM.Recordset

        Dim xlFileName As String
        Dim xlFileNameType As String
        Dim xlFileType As String
        Dim pStartPath As String
        Dim i As Integer
        Dim j As Integer

        Dim iHandle As Long

        Dim strConnectionString As String
        Dim oleCon As System.Data.OleDb.OleDbConnection
        Dim oleAdapter As System.Data.OleDb.OleDbDataAdapter
        Dim oleCommnad As System.Data.OleDb.OleDbCommand
        Dim enumerator As System.Data.OleDb.OleDbEnumerator
        Dim ProviderList As ArrayList = New ArrayList()
        Dim iCnt As Integer
        Dim dts As System.Data.DataTable
        Dim oDtSet As System.Data.DataSet = New System.Data.DataSet
        Dim ExcelCon As String = "Provider="
        Dim cv_OLEDB_s As String = ""
        Dim cv_OLEVR_d As Double
        Dim av_Title_s As String
        Dim cv_SaveFolder_s As String = ""
        Dim c_Return_b As Boolean = False

        Try

            B1Connections.theAppl.StatusBar.SetText(CFL.GetMSG("FI0052", ModuleIni.FI), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning) '데이터 조회중입니다

            '-------------------------------------------------------------------------------------------
            ' 사용자의 엑셀 환경에 따라 oleDb 접속 Provider 설정 
            enumerator = New OleDbEnumerator()
            '엑셀 Provider 여부에 따라 접속방식변경
            dts = enumerator.GetElements
            For iCnt = 1 To dts.Rows.Count()

                If dts.Rows(iCnt - 1).Item(0).ToString.IndexOf("Microsoft.ACE.OLEDB.") >= 0 Or dts.Rows(iCnt - 1).Item(0).ToString.IndexOf("Microsoft.Jet.OLEDB.") >= 0 Then
                    ProviderList.Add(dts.Rows(iCnt - 1).Item(0).ToString)
                End If
            Next

            ProviderList.Sort()

            cv_OLEVR_d = 0

            For iCnt = 1 To ProviderList.Count

                If ProviderList(iCnt - 1).ToString.IndexOf("Microsoft.ACE.OLEDB.") >= 0 Then

                    If cv_OLEVR_d < Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.ACE.OLEDB.", "")) Then
                        cv_OLEDB_s = ProviderList(iCnt - 1).ToString
                        cv_OLEVR_d = Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.ACE.OLEDB.", ""))
                    End If
                End If

            Next

            cv_OLEVR_d = 0

            If cv_OLEDB_s = "" Then
                For iCnt = 1 To ProviderList.Count

                    If ProviderList(iCnt - 1).ToString.IndexOf("Microsoft.Jet.OLEDB.") >= 0 Then
                        If cv_OLEVR_d < Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.Jet.OLEDB.", "")) Then
                            cv_OLEDB_s = ProviderList(iCnt - 1).ToString
                            cv_OLEVR_d = Convert.ToDouble(ProviderList(iCnt - 1).ToString.Replace("Microsoft.Jet.OLEDB.", ""))
                        End If
                    End If
                Next
            End If

            ExcelCon = ExcelCon & cv_OLEDB_s & ";"

            dts.Clear()

            '-------------------------------------------------------------------------------------------

            '-------------------------------------------------------------------------------------------
            ' 데이터조회후 데이터테이블로 형태 변환 _ Start  
            oRs = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            oRs.DoQuery(xSql)
            Dim strQryXml As String

            strQryXml = oRs.GetAsXML

            Dim xmlSR As System.IO.StringReader = New System.IO.StringReader(strQryXml)
            Dim theDataSet As DataSet = New DataSet
            Dim RowData As System.Data.DataTable = New System.Data.DataTable

            theDataSet.ReadXml(xmlSR)

            RowData = theDataSet.Tables(3)

            '데이터 테이블 시트용 명칭변경
            RowData.TableName = "Sheet1"
            '-------------------------------------------------------------------------------------------

            B1Connections.theAppl.StatusBar.SetText(CFL.GetMSG("FI0053", ModuleIni.FI), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success) '조회완료

            '-------------------------------------------------------------------------------------------
            ' 엑셀파일을 읽어온 후에 신규파일로 저장 
            pStartPath = System.Reflection.Assembly.GetExecutingAssembly.Location
            pStartPath = pStartPath.Substring(0, InStrRev(pStartPath, "\"))

            If ExcelCon.IndexOf("Microsoft.Jet.OLEDB.") Then
                xlFileName = "XLS\" & "excelprint.xls"
                xlFileNameType = "Excel files (*.xls)|*.xls"
                xlFileType = ".xlsx"
            Else
                xlFileName = "XLS\" & "excelprint.xlsx"
                xlFileNameType = "Excel files (*.xlsx)|*.xlsx"
                xlFileType = ".xlsx"
            End If

            av_Title_s = DateTime.Parse(CDate(System.DateTime.Now)).ToString("yyyyMMddHHmss")

            If Not IsDir(pStartPath & xlFileName) Then
                Exit Try
            End If

            xlApp = CreateObject("Excel.Application")
            xlBook = xlApp.Workbooks.Open(pStartPath & xlFileName)

            '엑셀 개체Hendle 가져옴.
            iHandle = IntPtr.Zero
            If CInt(xlApp.Version) < "10.0" Then
                iHandle = FindWindow(Nothing, xlApp.Caption)
            Else
                iHandle = xlApp.Parent.Hwnd
            End If

            cv_SaveFolder_s = CFL.FileDialog(eFileDialog.en_SaveFile, xlFileNameType, True)

            If cv_SaveFolder_s = "" Then
                xlFileName = IIf(B1Connections.diCompany.GetCompanyService.GetAdminInfo.ExcelFolderPath = "", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), B1Connections.diCompany.GetCompanyService.GetAdminInfo.ExcelFolderPath)
                xlFileName = xlFileName + IIf(xlFileName.LastIndexOf("\") = xlFileName.Length - 1, "", "\")
                If Dir(xlFileName, FileAttribute.Directory) = "" Then
                    MkDir(xlFileName)
                End If

                If strTitle = "" Or strTitle = Nothing Then
                    strTitle = "ExcelData"
                End If

                xlFileName = xlFileName + strTitle & "_" & av_Title_s & xlFileType
            Else
                xlFileName = cv_SaveFolder_s
            End If

            xlBook.SaveAs(xlFileName)

            If Not xlApp Is Nothing Then
                xlApp.Quit()
            End If

            'KillExcel(iHandle)
            '-------------------------------------------------------------------------------------------


            '------------------------------------------------------------------------------------------- 

            strConnectionString = ExcelCon + "Data Source=" + xlFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 8.0;HDR=Yes;IMEX=0;" + Convert.ToChar(34).ToString()

            '엑셀 oledb객체생성 후  쿼리문 작성 
            oleCon = New OleDbConnection()
            oleCon.ConnectionString = strConnectionString

            '엑셀 oledb객체 연결
            oleCon.Open()

            Dim colNameField As String()
            Dim colParam As String()
            Dim createTable As String()
            Dim strColumnFieldQry As String
            Dim strCreateQry As String
            Dim strParmQry As String

            ReDim createTable(RowData.Columns.Count - 2)
            ReDim colNameField(RowData.Columns.Count - 2)
            ReDim colParam(RowData.Columns.Count - 2)

            '데이터테이블 컬럼명 및 파라미터 정의
            For i = 0 To RowData.Columns.Count - 2
                colNameField(i) = String.Format("[{0}]", RowData.Columns(i).ColumnName)
                colParam(i) = "?"
                createTable(i) = String.Format("[{0}] NVARCHAR(255)  ", RowData.Columns(i).ColumnName)
                'If RowData.Columns(i).ColumnName <> "QTY" Then
                '    createTable(i) = String.Format("[{0}] NVARCHAR(255)  ", RowData.Columns(i).ColumnName)
                'Else
                '    createTable(i) = String.Format("[{0}] NUMERIC(19,6)  ", RowData.Columns(i).ColumnName)
                'End If

            Next


            strColumnFieldQry = String.Join(",", colNameField)
            strCreateQry = String.Join(",", createTable)
            strParmQry = String.Join(",", colParam)

            oleCommnad = New OleDbCommand
            oleCommnad.Connection = oleCon

            '엑셀 시트테이블 생성
            oleCommnad = New OleDbCommand(" CREATE TABLE [Sheet1$] ( " + strCreateQry.ToString + " ) ", oleCon)
            oleCommnad.ExecuteNonQuery()

            'create the adapter with the select to get 
            oleAdapter = New OleDbDataAdapter("SELECT * FROM [Sheet1$] ", oleCon)

            '데이터테이블과 엑셀 포맷 설정
            oleAdapter.FillSchema(RowData, SchemaType.Source)
            oleAdapter.Fill(RowData)

            '데이터 인서트 쿼리문 작성
            oleAdapter.InsertCommand = New OleDbCommand(String.Format("INSERT INTO [Sheet1$] ( {0} ) VALUES ( {1} )", strColumnFieldQry, strParmQry), oleCon)

            '데이터 인서트 파라미터 정의
            For i = 0 To RowData.Columns.Count - 1
                oleAdapter.InsertCommand.Parameters.Add(String.Format("@[{0}]", RowData.Columns(i).ColumnName), OleDbType.Char, 255).SourceColumn = RowData.Columns(i).ColumnName.ToString
            Next

            '데이터테이블 내역 일괄 생성
            oleAdapter.Update(RowData)

            oleCon.Close()

            c_Return_b = True

            '-------------------------------------------------------------------------------------------


            Return c_Return_b

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

            If Not xlApp Is Nothing Then
                xlApp.Quit()
            End If

            c_Return_b = False

            Return c_Return_b

        Finally

            oleCon.Close()

            If Not xlApp Is Nothing Then
                xlApp.Quit()
            End If
            KillExcel(iHandle)

            xlApp = Nothing
            xlBook = Nothing
            xlSheet = Nothing

            dts = Nothing
            oDtSet = Nothing
            oleAdapter = Nothing
            oleCon = Nothing
            oleCommnad = Nothing
            enumerator = Nothing
            ProviderList = Nothing

            GC.Collect()
            GC.WaitForPendingFinalizers()

            If (xlFileName <> "" And c_Return_b = True) Then
                Dim proc As Process = New System.Diagnostics.Process
                proc.StartInfo.FileName = "EXCEL.EXE"
                proc.StartInfo.Arguments = """" & xlFileName & """"
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Maximized
                proc.Start()
            End If

        End Try
    End Function

    Public Function IsDir(ByVal FileName As String) As Boolean

        If Dir(FileName) = "" Then
            '출력양식이 존재하지 않습니다
            B1Connections.theAppl.MessageBox("출력 양식이 존재하지 않습니다.")
            IsDir = False
            Exit Function
        End If

        IsDir = True

    End Function

    Public Sub MatrixSettingColor(ByVal ParamArray cols() As SAPbouiCOM.Column)

        For Each col As SAPbouiCOM.Column In cols
            col.BackColor = 12777465
        Next
    End Sub

    Public Sub DateEditeSetting(ByVal oForm As SAPbouiCOM.Form, _
                                ByVal type As String, _
                                ByVal ParamArray edtStr() As String)

        If Not oForm.Mode = BoFormMode.fm_ADD_MODE Then Exit Sub

        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
        Dim oStr As String = String.Empty
        Try
            oForm.Freeze(True)
            oRs = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            oStr = " EXEC [WJS_SP_FIA90903] "
            oStr = oStr & "N'" & type & "' "
            oRs.DoQuery(oStr)

            DirectCast(oForm.Items.Item(edtStr(0)).Specific, SAPbouiCOM.EditText).Value = oRs.Fields.Item(0).Value
            DirectCast(oForm.Items.Item(edtStr(1)).Specific, SAPbouiCOM.EditText).Value = oRs.Fields.Item(1).Value

            If oRs.Fields.Item(2).Value = "Y" Then

                'Dim itms As SAPbouiCOM.Items = oForm.Items

                'For Each itm As SAPbouiCOM.Item In itms
                '    oForm.ActiveItem = itm.UniqueID
                '    Exit For
                'Next

                'oForm.Items.Item(edtStr(0)).Enabled = False
                'oForm.Items.Item(edtStr(1)).Enabled = False

            Else
                oForm.Items.Item(edtStr(0)).Enabled = True
                oForm.Items.Item(edtStr(1)).Enabled = True
            End If
            oForm.Freeze(False)

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("DateEditeSetting " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            oForm.Freeze(False)
        End Try
    End Sub

    Public Function TranSave(ByVal oForm As SAPbouiCOM.Form, _
                             ByVal type As String, _
                             ByVal FTYYMM As String, _
                             ByVal TOYYMM As String, _
                             Optional ByVal YN As String = "Y") As Boolean

        'Dim oDBDataH As SAPbouiCOM.DBDataSource = oForm.DataSources.DBDataSources.Item(datatable)
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
        Dim oStr As String = String.Empty

        Try
            oRs = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            ', @U_CLDTCD VARCHAR(16) = ''
            ', @U_CLYN VARCHAR(1)
            ', @U_FICD varchar(16) = ''
            ', @U_FTYYMM varchar(8) = ''
            ', @U_TOYYMM varchar(8) = ''
            oStr = " EXEC [WJS_SP_FIA90901] "
            oStr = oStr & "'" & B1Connections.diCompany.UserSignature.ToString & "', "
            oStr = oStr & "N'" & YN & "', "
            oStr = oStr & "N'" & type & "', "
            oStr = oStr & "'" & FTYYMM & "', "
            oStr = oStr & "'" & TOYYMM & "' "
#If HANA = "Y" Then
            oStr = CFL.GetConvertHANA(oStr)
#End If
            oRs.DoQuery(oStr)

            If oRs.Fields.Item(0).Value = "0" Then

                CFL.COMMON_MESSAGE("!", oRs.Fields.Item(1).Value)


                Return False
            End If
            Return True
        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("TranSave " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function


    Public Function GetCOPLANT(Optional ByVal sCACD As String = "") As String

        Dim xSQL As String
        Dim oRS As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim v_RTNVAL As String = ""
        Try
            xSQL = " SELECT COUNT(B.U_PLCD)"
            xSQL = xSQL & vbCrLf & " FROM [@WJS_SCO01M] AS A"
            xSQL = xSQL & vbCrLf & " INNER JOIN [@WJS_SCO011] AS B ON A.Code = B.Code"
            xSQL = xSQL & vbCrLf & " WHERE A.Code = CASE WHEN N'" & sCACD & "' = '' THEN A.Code ELSE '" & sCACD & "' END "
            xSQL = xSQL & vbCrLf & " AND B.U_USEYN = N'Y'"

#If HANA = "Y" Then
            xSQL = CFL.GetConvertHANA(xSQL)
#End If
            oRS.DoQuery(xSQL)

            If Not (oRS.EoF) Then

                If oRS.Fields.Item(0).Value > 1 Then
                    v_RTNVAL = ""
                    Return v_RTNVAL
                Else

                    xSQL = "SELECT TOP 1 U_PLCD FROM [@WJS_SCO011] WHERE U_USEYN = 'Y'"
                    v_RTNVAL = CFL.GetValue(xSQL)
                    Return v_RTNVAL
                End If

            End If

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetCheckDate" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing

        End Try

    End Function
    ''' <summary>
    ''' 사업부권한에 따른 사업부 Combo 셋팅
    ''' </summary>
    ''' <param name="oForm"></param>
    ''' <param name="oCombo"></param>
    ''' <param name="strComboValue"></param>
    ''' <param name="strUserId"></param>
    ''' <remarks></remarks>
    Public Sub SetBACDCombo(ByVal oForm As SAPbouiCOM.Form, ByVal oCombo As SAPbouiCOM.ComboBox, Optional ByVal strComboValue As String = "", Optional ByVal strUserId As String = "")
        Dim oRS As SAPbobsCOM.Recordset
        Dim xSQL As String = ""
        Dim iRow As Integer = 0
        Dim udfForm As SAPbouiCOM.Form
        Dim bComboEab As Boolean = True '콤보 Enable 값

        Dim xmldoc As Xml.XmlDocument
        Dim xnode As Xml.XmlNode
        Dim nodelist As Xml.XmlNodeList
        Dim strFocusItem As String = ""

        Try

            oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            If strUserId = "" Then strUserId = B1Connections.diCompany.UserSignature.ToString

            '기존에 사업부 콤보값과 사업부 정보의 사업부 수가 다르면 세로 콤보박스를 만든다.
            If (oCombo.ValidValues.Count - 1).ToString <> CFL.GetValue("SELECT COUNT(*) FROM [@WJS_SAD50M]") Then

                '기존콤보값 삭제
                For iRow = oCombo.ValidValues.Count - 1 To 0 Step -1
                    If iRow = 0 Then Exit For
                    oCombo.ValidValues.Remove(iRow, BoSearchKey.psk_Index)
                Next

                '첫행 공백 처리
                If oCombo.ValidValues.Count = 0 Then oCombo.ValidValues.Add("", "")

                '사업부 콤보값 세팅
                xSQL = "SELECT U_BACD, U_BANM FROM [@WJS_SAD50M] ORDER BY U_BACD, U_BANM"
                oRS.DoQuery(xSQL)

                For iRow = 0 To oRS.RecordCount - 1 Step 1
                    oCombo.ValidValues.Add(Trim(oRS.Fields.Item("U_BACD").Value), Trim(oRS.Fields.Item("U_BANM").Value))
                    oRS.MoveNext()
                Next

            End If

            '사업부권한 조회
            xSQL = "SELECT A.U_BACD, ISNULL(A.U_AUTHTP, 'N') AS U_AUTHTP FROM [@WJS_UAD08M_DSB] A LEFT OUTER JOIN [@WJS_SAD50M] B ON A.U_BACD = B.U_BACD WHERE U_USERID = '" & strUserId & "'"
            oRS.DoQuery(xSQL)

            '사업부 권한에 없는 유저는 전체 권한을 준다.
            If Not oRS.EoF Then

                '콤보기본값 세팅
                If Trim(strComboValue) = "" Then strComboValue = Trim(oRS.Fields.Item("U_BACD").Value)

                '콤보를 Disable처리할시 포커스가 콤보에 있으면 오류나는 부분을 처리 하기위해 Enable된 Edit값을 찾는 로직
                If Trim(oRS.Fields.Item("U_AUTHTP").Value) <> "Y" Then

                    '콤보 Enable 값
                    bComboEab = False

                    '폼 XML을 이용하여 포커스를 이동할 Item을 찾는다.
                    xmldoc = New Xml.XmlDocument()
                    xmldoc.LoadXml(oForm.GetAsXML())

                    '포커스를 줄 Item과 관계없는 Xml은 제거 한다.
                    xnode = xmldoc.SelectSingleNode("Application/forms/action/form/datasources")
                    If (Not xnode Is Nothing) Then
                        xnode.RemoveAll()
                    End If

                    xnode = xmldoc.SelectSingleNode("Application/forms/action/form/ChooseFromListCollection")
                    If (Not xnode Is Nothing) Then
                        xnode.RemoveAll()
                    End If

                    xnode = xmldoc.SelectSingleNode("Application/forms/action/form/DataBrowser")
                    If (Not xnode Is Nothing) Then
                        xnode.RemoveAll()
                    End If

                    xnode = xmldoc.SelectSingleNode("Application/forms/action/form/FormMenu")
                    If (Not xnode Is Nothing) Then
                        xnode.RemoveAll()
                    End If

                    nodelist = xmldoc.SelectNodes("Application/forms/action/form")

                    For Each xn As Xml.XmlNode In nodelist

                        '조회 화면은 사업부콤보를 활성화 시킨다.(ObjectType 값이 있는 걸로 구분)
                        If Trim(xn.Attributes("ObjectType").Value.ToString) = "-1" Or Trim(xn.Attributes("ObjectType").Value.ToString) = "" Then

                            '콤보 Enable 값
                            bComboEab = True
                            Exit For

                        End If

                    Next

                    '콤보박스를 비활성화 할때 포커스를 줄 Item을 찾는다.
                    If bComboEab = False Then

                        nodelist = xmldoc.SelectNodes("Application/forms/action/form/items/action/item")

                        For Each xn As Xml.XmlNode In nodelist

                            '타입이 EditBox이고 활성화 되어있는 Item을 찾는다.
                            If xn.Attributes("type").Value.ToString = "16" And xn.Attributes("visible").Value.ToString = "1" And xn.Attributes("enabled").Value.ToString = "1" Then

                                strFocusItem = xn.Attributes("uid").Value.ToString
                                Exit For

                            End If

                        Next

                    End If

                End If

            End If

            '콤보 기본값 선택
            If strComboValue <> "" Then

                oCombo.Select(strComboValue, BoSearchKey.psk_ByValue)

                If Not oForm.UniqueID.StartsWith("WJS_") And oForm.UDFFormUID <> "" Then
                    udfForm = B1Connections.theAppl.Forms.Item(oForm.UDFFormUID)
                    udfForm.Update()
                End If

            End If

            '커서를 옮긴 후 Disable 시킨다.
            If strFocusItem <> "" Then
                oForm.Items.Item(strFocusItem).Click()
            End If

            '콤보 Enable, Disable 처리
            oCombo.Item.Enabled = bComboEab
            oCombo.Item.DisplayDesc = True

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("SetBACDCombo " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            oRS = Nothing
            udfForm = Nothing
        End Try

    End Sub
    Public Function FieldValueList(ByVal TableID As String, ByVal AliasID As String, _
        ByVal av_ip_s As String, ByVal av_dbname_s As String) As SAPbobsCOM.Recordset

        Dim xSQL As String

        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        xSQL = "SELECT B.FldValue, B.Descr " & vbCrLf
        If av_ip_s = "" And av_dbname_s = "" Then
            xSQL = xSQL + " FROM CUFD A WITH(NOLOCK)  INNER JOIN UFD1 B  WITH(NOLOCK) ON A.TableID = B.TableID AND A.FieldID = B.FieldID" & vbCrLf
        Else
            xSQL = xSQL + " FROM [" & av_ip_s & "].[" & av_dbname_s & "].dbo.CUFD A  WITH(NOLOCK) "
            xSQL = xSQL & " INNER JOIN [" & av_ip_s & "].[" & av_dbname_s & "].dbo.UFD1 B  WITH(NOLOCK) ON A.TableID = B.TableID AND  A.FieldID = B.FieldID" & vbCrLf
        End If
        xSQL = xSQL + " WHERE A.TableID = N'" & TableID & "' AND A.AliasID = N'" & AliasID & "' " & vbCrLf
        xSQL = xSQL + " ORDER BY B.IndexID "
#If HANA = "Y" Then
        xSQL = CFL.GetConvertHANA(xSQL)
#End If
        oRs.DoQuery(xSQL)

        FieldValueList = oRs

        oRs = Nothing

    End Function

    Public Function GetMTPName(ByVal strMTPCD As String) As String

        Return CFL.GetValue("select U_MTPNM from [@WJS_SAD67M] where U_MTPCD = " + CFL.GetQD(strMTPCD))

    End Function

    Public Function GetPlantName(ByVal strPLCD As String) As String
        Return CFL.GetValue("SELECT U_PLNM FROM [@WJS_SAD63M] WHERE U_PLCD = " + CFL.GetQD(strPLCD))
    End Function

    Public Function GetMTPAcct(ByVal strMTPCD As String, ByVal strITEMCODE As String) As String

        'Return CFL.GetValue("select U_ACCTCD from [@WJS_SAD67M] where U_MTPCD = " + CFL.GetQD(strMTPCD) + " and U_PRECD = 'U' ")

        Dim xSQL As String
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        xSQL = ""
        xSQL = xSQL & " SELECT CASE WHEN ISNULL(A4.U_GACCTCD, '') <> '' THEN A4.U_GACCTCD "
        xSQL = xSQL & " 			 WHEN ISNULL(A3.U_ACCTCD, '') <> '' THEN A3.U_ACCTCD "
        xSQL = xSQL & " 			 ELSE U_GACCTCD END "
        xSQL = xSQL & " from	OITM AS A1 "
        xSQL = xSQL & " INNER JOIN OITB AS A2 ON A1.ItmsGrpCod = A2.ItmsGrpCod "
        xSQL = xSQL & " LEFT JOIN [@WJS_SAD67M] AS A3 ON A3.U_USEYN = 'Y' and A3.U_PRECD = 'U' AND A3.U_MTPCD = " + CFL.GetQD(strMTPCD)
        xSQL = xSQL & " LEFT JOIN [@WJS_SAD82M] AS A4 ON A3.U_MTPCD = A4.U_MTPCD AND A2.BalInvntAc = A4.U_SACCTCD "
        xSQL = xSQL & " where A1.ItemCode = " + CFL.GetQD(strITEMCODE)
        oRs.DoQuery(xSQL)

        Return oRs.Fields.Item(0).Value

    End Function

    Public Function ChkColumn(ByRef oMatrix As SAPbouiCOM.Matrix, ByVal pv_Col_s As String) As Boolean
        Dim i As Integer

        For i = 0 To oMatrix.Columns.Count - 1
            If (oMatrix.Columns.Item(i).UniqueID = pv_Col_s) Then
                ChkColumn = True
                Exit Function
            End If
        Next
        ChkColumn = False
    End Function
    Public Function ConFirmCHK(ByVal oForm As SAPbouiCOM.Form, _
                               ByVal type As String, _
                               ByVal FTYYMM As String, _
                               ByVal TOYYMM As String, _
                               Optional ByVal YN As String = "Y", _
                               Optional ByVal NoMSG As Boolean = False) As Boolean

        'Dim oDBDataH As SAPbouiCOM.DBDataSource = oForm.DataSources.DBDataSources.Item(datatable)
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
        Dim oStr As String = String.Empty
        ' @DOC varchar(max)
        ',@FRMM VARCHAR(6)
        ',@TOMM VARCHAR(6)
        ',@GB VARCHAR(1)
        Try
            oRs = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            oStr = " EXEC [WJS_SP_FIA90902] "
            oStr = oStr & "N'" & type & "', "
            oStr = oStr & "'" & FTYYMM & "', "
            oStr = oStr & "'" & TOYYMM & "', "
            oStr = oStr & "N'" & YN & "' "

            oRs.DoQuery(oStr)

            If oRs.Fields.Item(0).Value = "0" Then
                If Not NoMSG Then
                    CFL.COMMON_MESSAGE("!", oRs.Fields.Item(1).Value)
                End If

                Return False
            End If
            Return True

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("ConFirmCHK " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
            Return False
        End Try
    End Function
    Public Function ChkIfLive() As Boolean

        Dim xSQL As String = ""
        xSQL = " SELECT IFNULL( " _
                + " 	( " _
                + "     SELECT LIVE " _
                + " 	FROM WJSSY.WJSIFINFO  " _
                + " 	WHERE ERPDB = '" + B1Connections.diCompany.CompanyDB + "' " _
                + " ), 'N') AS LIVE FROM DUMMY  "


        Return (CFL.GetValue(xSQL) = "Y")

    End Function


    Public Function GetCheckSelCnt(ByRef oGrid As SAPbouiCOM.Grid, ByVal strColumnID As String) As Integer
        Dim i As Integer = 0
        Dim boolSelected As Boolean = False
        Dim iSelCnt As Integer = 0

        For i = 0 To oGrid.Rows.Count - 1
            If oGrid.DataTable.GetValue(strColumnID, i) = "Y" Then
                iSelCnt = iSelCnt + 1
            End If
        Next

        Return iSelCnt

    End Function

    '암호화 관련 시작
    Public Function WJSCRYPTO_CHECK_UDF()
        '사용자 정의 필드 Check
        If (Not B1Connections.theAppl.Menus.Item("6913").Checked) Then
            B1Connections.theAppl.Menus.Item("6913").Activate()
        End If
    End Function

    Public Function WJSCRYPTO_PROCESS_CHECKAUTH() As String

        '권한 체크 Process'

        Dim xSQL As String = String.Empty
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Try
            xSQL = "SELECT TOP 1"
            xSQL = xSQL & vbCrLf & " U_AUTH FROM [@WJS_SAD13T] "
            xSQL = xSQL & vbCrLf & " WHERE U_USERID = '" & B1Connections.diCompany.UserName & "'"
            oRs.DoQuery(xSQL) '권한 체크 Process 끝'
            Return oRs.Fields.Item("U_AUTH").Value
        Catch ex As Exception
            MsgBox(ex.ToString & "권한 체크 실패")
        Finally
            oRs = Nothing
        End Try
    End Function

    Public Function WJSCRYPTO_CHECK_Edit(ByVal OriginalFormID As Integer, ByVal FormID As Integer, ByVal CheckFormType As String, ByVal EditColNM As String, ByVal TypeCount As Integer, ByVal BindValue As String)
        '개인정보 취급자 외 개인정보 Data 변경 불가
        'Form, Grid, Matrix 별로 처리 하기 위해 Select Case를 통한 처리 및 Grid, Matrix Binding을 위해 Grid 혹은 Matrix Item 값을 파라미터로 받음
        Dim oForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.GetFormByTypeAndCount(OriginalFormID, TypeCount)
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim Auth As String = "Y" '-- 권한부분 제외 'WJSCRYPTO_PROCESS_CHECKAUTH()
        If (Auth = "Y" Or String.IsNullOrEmpty(Auth)) Then
            Select Case CheckFormType
                Case "Form"
                    If (Not OriginalFormID = FormID) Then
                        Dim cForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.GetFormByTypeAndCount(FormID, TypeCount)
                        cForm.Items.Item(EditColNM).Enabled = False
                    Else
                        'oForm.Items.Item(EditColNM).Enabled = False
                        Call oForm.Items.Item(EditColNM).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, BoAutoFormMode.afm_All, BoModeVisualBehavior.mvb_False)
                    End If
                Case "Grid"
                    Dim oGrid As SAPbouiCOM.Grid = oForm.Items.Item(BindValue).Specific
                    oGrid.Columns.Item(EditColNM).Editable = False
                Case "Matrix"
                    Dim oMatrix As SAPbouiCOM.Matrix = oForm.Items.Item(BindValue).Specific
                    oMatrix.Columns.Item(EditColNM).Editable = False
            End Select
        End If
    End Function

    Public Function WJSCRYPTO_PROCESS_Encryption(ByVal OriginalFormID As Integer, ByVal FormID As Integer, ByVal CrytoColNM As String, ByVal TypeCount As Integer, ByVal PlainText As String, ByVal PKValue As String) As String

        '암호화 Process'
        Dim DbSrc As SAPbouiCOM.DBDataSource
        Dim xSQL As String = String.Empty
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.GetFormByTypeAndCount(OriginalFormID, TypeCount)
        Dim Auth As String = String.Empty
        Try

            Auth = "YN" ' -- 권한제외 WJSCRYPTO_PROCESS_CHECKAUTH() '권한을 체크합니다.

            If (Auth = "YN" Or Auth = "N") Then

                xSQL = "SELECT TOP 1" 'Meta 테이블에서 암호화시 필요한 항목들을 가져 옵니다.
                xSQL = xSQL & vbCrLf & " U_Privacy, U_CrytoColNM, U_FormNM, U_Repo, U_RepoPK FROM [@WJS_SAD15M] "
                xSQL = xSQL & vbCrLf & " WHERE U_FormID = '" & OriginalFormID & "'"
                xSQL = xSQL & vbCrLf & " AND U_CrytoColNM = '" & CrytoColNM & "'"
                oRs.DoQuery(xSQL)
                DbSrc = oForm.DataSources.DBDataSources.Item(oRs.Fields.Item("U_Repo").Value)

                Select Case oRs.Fields.Item("U_Privacy").Value.ToString '개인 정보에 따라 해당 정보 토큰 암호화
                    Case "JMNO"
                        WJSCRYPTO_PROCESS_Encryption = WJSCRYPTO_JMNO(OriginalFormID, oRs.Fields.Item("U_Privacy").Value.ToString.Trim, oRs.Fields.Item("U_Repo").Value, PlainText, oRs.Fields.Item("U_FormNM").Value, oRs.Fields.Item("U_RepoPK").Value.ToString.Trim, PKValue)
                    Case "ACNO"
                        WJSCRYPTO_PROCESS_Encryption = WJSCRYPTO_ACNO(OriginalFormID, oRs.Fields.Item("U_Privacy").Value.ToString.Trim, oRs.Fields.Item("U_Repo").Value, PlainText, oRs.Fields.Item("U_FormNM").Value, oRs.Fields.Item("U_RepoPK").Value.ToString.Trim, PKValue)
                    Case "CRNO"
                        WJSCRYPTO_PROCESS_Encryption = WJSCRYPTO_CRNO(OriginalFormID, oRs.Fields.Item("U_Privacy").Value.ToString.Trim, oRs.Fields.Item("U_Repo").Value, PlainText, oRs.Fields.Item("U_FormNM").Value, oRs.Fields.Item("U_RepoPK").Value.ToString.Trim, PKValue)
                End Select
            Else
                B1Connections.theAppl.MessageBox("암호화 권한이 없습니다. 관리자에게 문의하시길 바랍니다.")
                Return False
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            oRs = Nothing

        End Try
    End Function

    Public Function WJSCRYPTO_JMNO(ByVal FormID As Integer, _
                                      ByVal Privacy As String, _
                                      ByVal Repo As String, _
                                      ByVal Value As String, _
                                      ByVal FormNM As String, _
                                      ByVal PKName As String, _
                                      ByVal PKValue As String) As String

        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oRs2 As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim xSQL As String = ""
        Dim strCryto As String = ""
        Dim strRegno As String = ""
        Dim strC5 As String = ""
        Dim strC8 As String = ""
        Dim strC9 As String = ""
        Dim strC10 As String = ""
        Dim strC11 As String = ""
        Dim strC12 As String = ""
        Dim strC13 As String = ""

        Dim upperbound As Integer = 3
        Dim lowerbound As Integer = 1
        Dim randomValue As Integer = 0

        Dim FixDigit5 As Integer = 0
        Dim FixDigit8 As Integer = 0
        Dim FixDigit9 As Integer = 0
        Dim FixDigit10 As Integer = 0
        Dim FixDigit11 As Integer = 0
        Dim FixDigit12 As Integer = 0
        Dim FixDigit13 As Integer = 0

        Dim ValueDigit5 As Integer = 0
        Dim ValueDigit8 As Integer = 0
        Dim ValueDigit9 As Integer = 0
        Dim ValueDigit10 As Integer = 0
        Dim ValueDigit11 As Integer = 0
        Dim ValueDigit12 As Integer = 0
        Dim ValueDigit13 As Integer = 0
        Dim iCount As Integer = 999

        Dim Type As String = String.Empty

        WJSCRYPTO_JMNO = ""


        Try


            strCryto = CFL.CryptoEncryption(Value)

            If strCryto = "" Then
                Exit Try
            Else

                xSQL = ""
                xSQL = "SELECT  TOP 1 Value  FROM  WJSSY.DBO.JMNO  WHERE  WJSCRYValue  =  '" & strCryto & "'"
                oRs2.DoQuery(xSQL)

                If Not oRs2.EoF Then
                    WJSCRYPTO_JMNO = oRs2.Fields.Item("Value").Value
                Else

                    ValueDigit5 = Value.Substring(4, 1)
                    ValueDigit8 = Value.Substring(7, 1)
                    ValueDigit9 = Value.Substring(8, 1)
                    ValueDigit10 = Value.Substring(9, 1)
                    ValueDigit11 = Value.Substring(10, 1)
                    ValueDigit12 = Value.Substring(11, 1)
                    ValueDigit13 = Value.Substring(12, 1)

                    Do While iCount = 999

                        Randomize()

                        randomValue = CInt(Math.Floor((upperbound - lowerbound + 1) * Rnd())) + lowerbound

                        If randomValue = 1 Then

                            FixDigit5 = 65      '대문자A
                            FixDigit8 = 66      '대문자B
                            FixDigit9 = 99      '소문자C
                            FixDigit10 = 48     '숫자2
                            FixDigit11 = 103    '소문자G
                            FixDigit12 = 109    '소문자M
                            FixDigit13 = 49     '숫자1

                        ElseIf randomValue = 2 Then

                            FixDigit5 = 90      '대문자Z
                            FixDigit8 = 67      '대문자C
                            FixDigit9 = 80      '대문자P
                            FixDigit10 = 97     '소문자A
                            FixDigit11 = 97     '소문자A
                            FixDigit12 = 97     '소문자A
                            FixDigit13 = 48     '숫자0

                        ElseIf randomValue = 3 Then

                            FixDigit5 = 66      '대문자B
                            FixDigit8 = 68      '대문자D
                            FixDigit9 = 80      '대문자P
                            FixDigit10 = 97     '소문자A
                            FixDigit11 = 97     '소문자A
                            FixDigit12 = 97     '소문자A
                            FixDigit13 = 49     '숫자1

                        Else
                            FixDigit5 = 67      '대문자C
                            FixDigit8 = 68      '대문자D
                            FixDigit9 = 80      '대문자P
                            FixDigit10 = 97     '소문자A
                            FixDigit11 = 97     '소문자A
                            FixDigit12 = 97     '소문자A
                            FixDigit13 = 49     '숫자1
                        End If

                        strC5 = Chr(FixDigit5)
                        strC8 = Chr(FixDigit8 + ValueDigit8)
                        strC9 = Chr(FixDigit9 + ValueDigit9)
                        strC10 = Chr(FixDigit10 + ValueDigit10)
                        strC11 = Chr(FixDigit11 + ValueDigit11)
                        strC12 = Chr(FixDigit12 + ValueDigit12)
                        strC13 = WJSCRYPTO_RAND("REGNO")

                        strRegno = Value.Substring(0, 4).ToString & strC5 & Value.Substring(5, 2) & strC8 & strC9 & strC10 & strC11 & strC12 & strC13

                        xSQL = "SELECT   1"
                        xSQL = xSQL & vbCrLf & " FROM   WJSSY.DBO.JMNO "
                        xSQL = xSQL & vbCrLf & " WHERE  Value = '" & strRegno & "'"
                        oRs.DoQuery(xSQL)

                        If oRs.EoF Then
                            iCount = 1
                        Else
                            iCount = 999
                        End If

                    Loop
                    xSQL = "SELECT   1"
                    xSQL = xSQL & vbCrLf & " FROM   WJSSY.DBO.JMNO "
                    xSQL = xSQL & vbCrLf & " WHERE  FormNM = '" & FormNM & "'"
                    xSQL = xSQL & vbCrLf & " AND RepoPKValue = '" & PKValue.Trim.ToString & "'"
                    oRs.DoQuery(xSQL)
                    If (Not oRs.EoF) Then
                        Type = "Update"
                    Else
                        Type = "Add"
                    End If
                    'strCryto = CFL.CryptoEncryption(Value)

                    If strCryto <> "" And strRegno <> "" Then
                        Select Case Type
                            Case "Add"
                                xSQL = ""
                                xSQL = "INSERT  INTO  WJSSY.DBO.JMNO (Privacy, FormID, FormNM, Repo, RepoPKName, RepoPKValue, Value, WJSCRYValue, CreateDate, UserCode, CompanyDBNM)"
                                xSQL = xSQL & vbCrLf & "SELECT  '" & Privacy & "' As Privacy ,  '" & FormID & "' As FormID, '" & FormNM & "' As FormNM, '" & Repo & "' As Repo"
                                xSQL = xSQL & vbCrLf & ", '" & PKName.ToString.Trim & "' As RepoPKName , '" & PKValue.ToString.Trim & "' As RepoPKValue"
                                xSQL = xSQL & vbCrLf & ", '" & strRegno & " ' AS Value , '" & strCryto & "'  AS  WJSCRYValue"
                                xSQL = xSQL & vbCrLf & ",  GETDATE() AS CreateDate"
                                xSQL = xSQL & vbCrLf & ", '" & B1Connections.diCompany.UserName & "'  AS  UserCode"
                                xSQL = xSQL & vbCrLf & ", '" & B1Connections.diCompany.CompanyDB & "'"

                            Case "Update"
                                xSQL = ""
                                xSQL = "Update WJSSY.DBO.JMNO "
                                xSQL = xSQL & vbCrLf & "SET Value =  '" & strRegno & "'"
                                xSQL = xSQL & vbCrLf & ", WJSCRYValue = '" & strCryto & "'"
                                xSQL = xSQL & vbCrLf & "WHERE FormNM = '" & FormNM & "'"
                                xSQL = xSQL & vbCrLf & "AND RepoPKValue = '" & PKValue.Trim & "'"
                        End Select
                        oRs.DoQuery(xSQL)
                        WJSCRYPTO_JMNO = strRegno
                    End If

                End If
            End If


        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("WJSCRYPTO_JMNO Error : " & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error)

        Finally
            oRs = Nothing
            oRs2 = Nothing
        End Try

    End Function
    Public Function WJSCRYPTO_ACNO(ByVal FormID As Integer, _
                                       ByVal Privacy As String, _
                                       ByVal Repo As String, _
                                       ByVal Value As String, _
                                       ByVal FormNM As String, _
                                       ByVal PKName As String, _
                                       ByVal PKValue As String) As String


        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oRs2 As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim xSQL As String = ""
        Dim strCryto As String = ""

        Dim strAccno As String = ""
        Dim strC1 As String = ""
        Dim strC2 As String = ""
        Dim strC3 As String = ""
        Dim strC4 As String = ""
        Dim strC5 As String = ""
        Dim strC6 As String = ""
        Dim strC7 As String = ""
        Dim strC8 As String = ""
        Dim iCount As Integer = 999
        Dim iLength As Integer = 0

        Dim Type As String = String.Empty

        WJSCRYPTO_ACNO = ""


        Try

            strCryto = CFL.CryptoEncryption(Value)

            If strCryto = "" Then
                Exit Try
            Else

                xSQL = ""
                xSQL = "SELECT  TOP 1 Value  FROM  WJSSY.DBO.ACNO  WHERE  WJSCRYValue  =  '" & strCryto & "'"
                oRs2.DoQuery(xSQL)

                If Not oRs2.EoF Then
                    WJSCRYPTO_ACNO = oRs2.Fields.Item("Value").Value
                Else

                    Do While iCount = 999

                        strC1 = WJSCRYPTO_RAND("ACNO")
                        strC2 = WJSCRYPTO_RAND("ACNO")
                        strC3 = WJSCRYPTO_RAND("ACNO")
                        strC4 = WJSCRYPTO_RAND("ACNO")
                        strC5 = WJSCRYPTO_RAND("ACNO")
                        strC6 = WJSCRYPTO_RAND("ACNO")
                        strC7 = WJSCRYPTO_RAND("ACNO")
                        strC8 = WJSCRYPTO_RAND("ACNO")


                        strAccno = Value.Substring(0, 4).ToString & "EN" & strC1 & strC2 & strC3 & strC4 & strC5 & strC6 & strC7 & strC8

                        xSQL = "SELECT   1"
                        xSQL = xSQL & vbCrLf & " FROM   WJSSY.DBO.JMNO "
                        xSQL = xSQL & vbCrLf & " WHERE  Value  = '" & strAccno & "'"
                        oRs.DoQuery(xSQL)

                        If oRs.EoF Then
                            iCount = 1
                        Else
                            iCount = 999
                        End If
                    Loop
                    xSQL = "SELECT   1"
                    xSQL = xSQL & vbCrLf & " FROM   WJSSY.DBO.ACNO "
                    xSQL = xSQL & vbCrLf & " WHERE  FormNM = '" & FormNM & "'"
                    xSQL = xSQL & vbCrLf & " AND RepoPKValue = '" & PKValue.Trim & "'"
                    oRs.DoQuery(xSQL)
                    If (Not oRs.EoF) Then
                        Type = "Update"
                    Else
                        Type = "Add"
                    End If
                    'strCryto = CFL.CryptoEncryption(Value)

                    If strCryto <> "" And strAccno <> "" Then
                        Select Case Type
                            Case "Add"
                                xSQL = ""
                                xSQL = "INSERT  INTO  WJSSY.DBO.ACNO (Privacy, FormID, FormNM, Repo, RepoPKName, RepoPKValue, Value, WJSCRYValue, CreateDate, UserCode, CompanyDBNM)"
                                xSQL = xSQL & vbCrLf & "SELECT  '" & Privacy & "' As Privacy ,  '" & FormID & "' As FormID, '" & FormNM & "' As FormNM, '" & Repo & "' As Repo"
                                xSQL = xSQL & vbCrLf & ", '" & PKName.ToString.Trim & "' As RepoPKName , '" & PKValue.ToString.Trim & "' As RepoPKValue"
                                xSQL = xSQL & vbCrLf & ", '" & strAccno & " ' AS Value , '" & strCryto & "'  AS  WJSCRYValue"
                                xSQL = xSQL & vbCrLf & ",  GETDATE() AS CreateDate"
                                xSQL = xSQL & vbCrLf & ", '" & B1Connections.diCompany.UserName & "'  AS  UserCode"
                                xSQL = xSQL & vbCrLf & ", '" & B1Connections.diCompany.CompanyDB & "'"

                            Case "Update"
                                xSQL = ""
                                xSQL = "Update WJSSY.DBO.ACNO "
                                xSQL = xSQL & vbCrLf & "SET Value =  '" & strAccno & "'"
                                xSQL = xSQL & vbCrLf & ", WJSCRYValue = '" & strCryto & "'"
                                xSQL = xSQL & vbCrLf & "WHERE FormNM = '" & FormNM & "'"
                                xSQL = xSQL & vbCrLf & "AND RepoPKValue = '" & PKValue.Trim & "'"
                        End Select

                        oRs.DoQuery(xSQL)

                        WJSCRYPTO_ACNO = strAccno
                    End If
                End If
            End If



        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("WJSCRYPTO_ACNO Error : " & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error)
        Finally
            oRs = Nothing
            oRs2 = Nothing
        End Try


    End Function
    Public Function WJSCRYPTO_CRNO(ByVal FormID As Integer, _
                                   ByVal Privacy As String, _
                                   ByVal Repo As String, _
                                   ByVal Value As String, _
                                   ByVal FormNM As String, _
                                   ByVal PKName As String, _
                                   ByVal PKValue As String) As String


        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim oRs2 As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim xSQL As String = ""
        Dim strCryto As String = ""

        Dim strAccno As String = ""
        Dim strC1 As String = ""
        Dim strC2 As String = ""
        Dim strC3 As String = ""
        Dim strC4 As String = ""
        Dim strC5 As String = ""
        Dim strC6 As String = ""
        Dim strC7 As String = ""
        Dim iCount As Integer = 999
        Dim iLength As Integer = 0

        Dim Type As String = String.Empty

        WJSCRYPTO_CRNO = ""


        Try

            strCryto = CFL.CryptoEncryption(Value)

            If strCryto = "" Then
                Exit Try
            Else

                xSQL = ""
                xSQL = "SELECT  TOP 1 Value  FROM  WJSSY.DBO.CRNO  WHERE  WJSCRYValue  =  '" & strCryto & "'"
                oRs2.DoQuery(xSQL)

                If Not oRs2.EoF Then
                    WJSCRYPTO_CRNO = oRs2.Fields.Item("Value").Value
                Else

                    Do While iCount = 999

                        strC1 = WJSCRYPTO_RAND("ACNO")
                        strC2 = WJSCRYPTO_RAND("ACNO")
                        strC3 = WJSCRYPTO_RAND("ACNO")
                        strC4 = WJSCRYPTO_RAND("ACNO")
                        strC5 = WJSCRYPTO_RAND("ACNO")
                        strC6 = WJSCRYPTO_RAND("ACNO")
                        strC7 = WJSCRYPTO_RAND("ACNO")


                        strAccno = Value.Substring(0, 7).ToString & "EN" & strC1 & strC2 & strC3 & strC4 & strC5 & strC6 & strC7

                        xSQL = "SELECT   1"
                        xSQL = xSQL & vbCrLf & " FROM   WJSSY.DBO.CRNO "
                        xSQL = xSQL & vbCrLf & " WHERE  Value  = '" & strAccno & "'"
                        oRs.DoQuery(xSQL)

                        If oRs.EoF Then
                            iCount = 1
                        Else
                            iCount = 999
                        End If
                    Loop
                    xSQL = "SELECT   1"
                    xSQL = xSQL & vbCrLf & " FROM   WJSSY.DBO.CRNO "
                    xSQL = xSQL & vbCrLf & " WHERE  FormNM = '" & FormNM & "'"
                    xSQL = xSQL & vbCrLf & " AND RepoPKValue = '" & PKValue.Trim & "'"
                    oRs.DoQuery(xSQL)
                    If (Not oRs.EoF) Then
                        Type = "Update"
                    Else
                        Type = "Add"
                    End If
                    'strCryto = CFL.CryptoEncryption(Value)

                    If strCryto <> "" And strAccno <> "" Then
                        Select Case Type
                            Case "Add"
                                xSQL = ""
                                xSQL = "INSERT  INTO  WJSSY.DBO.CRNO (Privacy, FormID, FormNM, Repo, RepoPKName, RepoPKValue, Value, WJSCRYValue, CreateDate, UserCode, CompanyDBNM)"
                                xSQL = xSQL & vbCrLf & "SELECT  '" & Privacy & "' As Privacy ,  '" & FormID & "' As FormID, '" & FormNM & "' As FormNM, '" & Repo & "' As Repo"
                                xSQL = xSQL & vbCrLf & ", '" & PKName.ToString.Trim & "' As RepoPKName , '" & PKValue.ToString.Trim & "' As RepoPKValue"
                                xSQL = xSQL & vbCrLf & ", '" & strAccno & " ' AS Value , '" & strCryto & "'  AS  WJSCRYValue"
                                xSQL = xSQL & vbCrLf & ",  GETDATE() AS CreateDate"
                                xSQL = xSQL & vbCrLf & ", '" & B1Connections.diCompany.UserName & "'  AS  UserCode"
                                xSQL = xSQL & vbCrLf & ", '" & B1Connections.diCompany.CompanyDB & "'"

                            Case "Update"
                                xSQL = ""
                                xSQL = "Update WJSSY.DBO.CRNO "
                                xSQL = xSQL & vbCrLf & "SET Value =  '" & strAccno & "'"
                                xSQL = xSQL & vbCrLf & ", WJSCRYValue = '" & strCryto & "'"
                                xSQL = xSQL & vbCrLf & "WHERE FormNM = '" & FormNM & "'"
                                xSQL = xSQL & vbCrLf & "AND RepoPKValue = '" & PKValue.Trim & "'"
                        End Select

                        oRs.DoQuery(xSQL)

                        WJSCRYPTO_CRNO = strAccno
                    End If
                End If
            End If



        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("WJSCRYPTO_CRNO Error : " & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error)
        Finally
            oRs = Nothing
            oRs2 = Nothing
        End Try


    End Function

    Public Function WJSCRYPTO_PROCESS_Del(ByVal OriginalFormID As Integer, ByVal FormID As Integer, ByVal CrytoColNM As String, ByVal TypeCount As Integer, ByVal DelText As String, ByVal PKValue As String) As String
        '암호화 테이블 내 데이터 삭제
        Dim DbSrc As SAPbouiCOM.DBDataSource
        Dim xSQL As String = String.Empty
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim Auth As String = String.Empty
        Dim cForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.GetFormByTypeAndCount(FormID, TypeCount)
        Dim oForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.GetFormByTypeAndCount(OriginalFormID, TypeCount)
        Try
            xSQL = "SELECT TOP 1"
            xSQL = xSQL & vbCrLf & " U_Privacy, U_CrytoColNM, U_Repo, U_RepoPK FROM [@WJS_SAD15M] "
            xSQL = xSQL & vbCrLf & " WHERE U_FormID = '" & OriginalFormID & "'"
            xSQL = xSQL & vbCrLf & " AND U_CrytoColNM = '" & CrytoColNM & "'"
            oRs.DoQuery(xSQL)
            Dim Privacy As String = oRs.Fields.Item("U_Privacy").Value.ToString.Trim
            Select Case Privacy
                Case "JMNO"
                    Privacy = "주민등록번호"
                Case "ACNO"
                    Privacy = "계좌번호"
                Case "CRNO"
                    Privacy = "카드번호"
            End Select
            DbSrc = oForm.DataSources.DBDataSources.Item(oRs.Fields.Item("U_Repo").Value)
            Dim colNM As String = oRs.Fields.Item("U_CrytoColNM").Value
            Select Case oRs.Fields.Item("U_Privacy").Value.ToString
                Case "JMNO"
                    If (Not String.IsNullOrEmpty(cForm.Items.Item(oRs.Fields.Item("U_CrytoColNM").Value).Specific.Value.ToString)) Then
                        xSQL = "Delete"
                        xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.JMNO"
                        xSQL = xSQL & vbCrLf & " WHERE Value = '" & DelText & "'"
                        xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                        xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                        oRs.DoQuery(xSQL)
                    End If
                Case "ACNO"
                    xSQL = "DELETE"
                    xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.ACNO"
                    xSQL = xSQL & vbCrLf & " WHERE Value = '" & DelText & "'"
                    xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                    xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                    oRs.DoQuery(xSQL)
                Case "CRNO"
                    xSQL = "DELETE"
                    xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.CRNO"
                    xSQL = xSQL & vbCrLf & " WHERE Value = '" & DelText & "'"
                    xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                    xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                    oRs.DoQuery(xSQL)
            End Select
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            oRs = Nothing
        End Try

    End Function

    Public Function WJSCRYPTO_PROCESS_Decryption(ByVal OriginalFormID As Integer, ByVal FormID As Integer, ByVal CrytoColNM As String, ByVal TypeCount As Integer, ByVal CipherText As String, ByVal PKValue As String) As String
        '복호화 Process'
        Dim DbSrc As SAPbouiCOM.DBDataSource
        Dim xSQL As String = String.Empty
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim Auth As String = String.Empty
        Dim cForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.GetFormByTypeAndCount(FormID, TypeCount)
        Dim oForm As SAPbouiCOM.Form = B1Connections.theAppl.Forms.GetFormByTypeAndCount(OriginalFormID, TypeCount)
        Try
            Auth = "YN" '권한제외 WJSCRYPTO_PROCESS_CHECKAUTH()
            If (Auth = "YN" Or Auth = "Y") Then
                xSQL = "SELECT TOP 1"
                xSQL = xSQL & vbCrLf & " U_Privacy, U_CrytoColNM, U_Repo, U_RepoPK FROM [@WJS_SAD15M] "
                xSQL = xSQL & vbCrLf & " WHERE U_FormID = '" & OriginalFormID & "'"
                xSQL = xSQL & vbCrLf & " AND U_CrytoColNM = '" & CrytoColNM & "'"
                oRs.DoQuery(xSQL)
                Dim Privacy As String = oRs.Fields.Item("U_Privacy").Value.ToString.Trim
                Select Case Privacy
                    Case "JMNO"
                        Privacy = "주민등록번호"
                    Case "ACNO"
                        Privacy = "계좌번호"
                    Case "CRNO"
                        Privacy = "카드번호"
                End Select
                DbSrc = oForm.DataSources.DBDataSources.Item(oRs.Fields.Item("U_Repo").Value)
                Dim colNM As String = oRs.Fields.Item("U_CrytoColNM").Value
                Select Case oRs.Fields.Item("U_Privacy").Value.ToString
                    Case "JMNO"
                        If (Not String.IsNullOrEmpty(cForm.Items.Item(oRs.Fields.Item("U_CrytoColNM").Value).Specific.Value.ToString)) Then
                            xSQL = "SELECT WJSCRYValue"
                            xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.JMNO"
                            xSQL = xSQL & vbCrLf & " WHERE Value = '" & CipherText & "'"
                            xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                            xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                            oRs.DoQuery(xSQL)
                        End If
                    Case "ACNO"
                        xSQL = "SELECT WJSCRYValue"
                        xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.ACNO"
                        xSQL = xSQL & vbCrLf & " WHERE Value = '" & CipherText & "'"
                        xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                        xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                        oRs.DoQuery(xSQL)
                    Case "CRNO"
                        xSQL = "SELECT WJSCRYValue"
                        xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.CRNO"
                        xSQL = xSQL & vbCrLf & " WHERE Value = '" & CipherText & "'"
                        xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                        xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                        oRs.DoQuery(xSQL)
                End Select
                If Not oRs.EoF Then
                    B1Connections.theAppl.MessageBox("개인정보 ( " & Privacy & " )  복호화 된 값은 :   " & CFL.CryptoDecryption(oRs.Fields.Item("WJSCRYValue").Value.ToString))
                End If
            Else
                B1Connections.theAppl.MessageBox("복호화 권한이 없습니다.")
                Exit Try
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            oRs = Nothing
        End Try
    End Function

    Public Function WJSCRYPTO_PROCESS_AllEncryption(ByVal Privacy As String, ByVal FormNM As String, ByVal PlainText As String, ByVal PKValue As String) As String
        '일괄암호화 Process'
        Dim xSQL As String = String.Empty
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim Auth As String = String.Empty
        Try

            Auth = "YN" '--권한제외 WJSCRYPTO_PROCESS_CHECKAUTH() '권한을 체크합니다.

            If (Auth = "YN" Or Auth = "N") Then

                xSQL = "SELECT TOP 1" 'Meta 테이블에서 암호화시 필요한 항목들을 가져 옵니다.
                xSQL = xSQL & vbCrLf & " U_FormID, U_CrytoColNM, U_Repo, U_RepoPK FROM [@WJS_SAD15M] "
                xSQL = xSQL & vbCrLf & " WHERE U_Privacy = '" & Privacy & "'"
                xSQL = xSQL & vbCrLf & " AND U_FormNM = '" & FormNM & "'"
                oRs.DoQuery(xSQL)

                Select Case Privacy '개인 정보에 따라 해당 정보 토큰 암호화
                    Case "JMNO"
                        WJSCRYPTO_PROCESS_AllEncryption = WJSCRYPTO_JMNO(oRs.Fields.Item("U_FormID").Value.ToString.Trim, Privacy, oRs.Fields.Item("U_Repo").Value, PlainText, FormNM, oRs.Fields.Item("U_RepoPK").Value.ToString.Trim, PKValue)
                    Case "ACNO"
                        WJSCRYPTO_PROCESS_AllEncryption = WJSCRYPTO_ACNO(oRs.Fields.Item("U_FormID").Value.ToString.Trim, Privacy, oRs.Fields.Item("U_Repo").Value, PlainText, FormNM, oRs.Fields.Item("U_RepoPK").Value.ToString.Trim, PKValue)
                    Case "CRNO"
                        WJSCRYPTO_PROCESS_AllEncryption = WJSCRYPTO_CRNO(oRs.Fields.Item("U_FormID").Value.ToString.Trim, Privacy, oRs.Fields.Item("U_Repo").Value, PlainText, FormNM, oRs.Fields.Item("U_RepoPK").Value.ToString.Trim, PKValue)
                End Select
            Else
                B1Connections.theAppl.MessageBox("암호화 권한이 없습니다. 관리자에게 문의하시길 바랍니다.")
                Exit Try
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Function

    Public Function WJSCRYPTO_PROCESS_AllDecryption(ByVal Privacy As String, ByVal FormNM As String, ByVal CipherText As String, ByVal PKValue As String) As String
        '복호화 Process'
        Dim DbSrc As SAPbouiCOM.DBDataSource
        Dim xSQL As String = String.Empty
        Dim oRs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim Auth As String = String.Empty
        Try
            Auth = "YN" '-- 권한제외 WJSCRYPTO_PROCESS_CHECKAUTH()
            If (Auth = "YN" Or Auth = "Y") Then
                xSQL = "SELECT TOP 1" 'Meta 테이블에서 암호화시 필요한 항목들을 가져 옵니다.
                xSQL = xSQL & vbCrLf & " U_FormID, U_CrytoColNM, U_Repo, U_RepoPK FROM [@WJS_SAD15M] "
                xSQL = xSQL & vbCrLf & " WHERE U_Privacy = '" & Privacy & "'"
                xSQL = xSQL & vbCrLf & " AND U_FormNM = '" & FormNM & "'"
                oRs.DoQuery(xSQL)
                Select Case Privacy
                    Case "JMNO"
                        xSQL = "SELECT WJSCRYValue"
                        xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.JMNO"
                        xSQL = xSQL & vbCrLf & " WHERE Value = '" & CipherText & "'"
                        xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                        xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                        oRs.DoQuery(xSQL)
                    Case "ACNO"
                        xSQL = "SELECT WJSCRYValue"
                        xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.ACNO"
                        xSQL = xSQL & vbCrLf & " WHERE Value = '" & CipherText & "'"
                        xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                        xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                        oRs.DoQuery(xSQL)
                    Case "CRNO"
                        xSQL = "SELECT WJSCRYValue"
                        xSQL = xSQL & vbCrLf & " FROM WJSSY.DBO.CRNO"
                        xSQL = xSQL & vbCrLf & " WHERE Value = '" & CipherText & "'"
                        xSQL = xSQL & vbCrLf & " AND Repo = '" & oRs.Fields.Item("U_Repo").Value & "'"
                        xSQL = xSQL & vbCrLf & " AND RepoPKName = '" & oRs.Fields.Item("U_RepoPK").Value & "'"
                        oRs.DoQuery(xSQL)
                End Select
                If Not oRs.EoF Then
                    WJSCRYPTO_PROCESS_AllDecryption = CFL.CryptoDecryption(oRs.Fields.Item("WJSCRYValue").Value.ToString)
                End If
            Else
                B1Connections.theAppl.MessageBox("복호화 권한이 없습니다.")
                Exit Try
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            oRs = Nothing
        End Try
    End Function

    Public Function WJSCRYPTO_RAND(ByVal Field As String) As String
        '향후 Field 필드를 이용하여 난수 발생시 특이사항을 적용할 수 있도록 활용할 수 있다.

        Dim upperbound As Integer = 3
        Dim lowerbound As Integer = 1
        Dim randomValue As Integer = 0

        Dim upperbound_L_X As Integer = 90         '대문자 최고값(X ASCII)
        Dim lowerbound_L_X As Integer = 65         '대문자 최소값(A ASCII)
        Dim randomResult As Integer = 0            '대문자 랜덤값

        Dim upperbound_S_X As Integer = 122        '소문자 최고값(x ascii)
        Dim lowerbound_S_X As Integer = 97         '소문자 최소값(a ascii)

        Dim upperbound_9 As Integer = 57           '숫자 최고값 9
        Dim lowerbound_9 As Integer = 48           '숫자 최소값 0


        WJSCRYPTO_RAND = ""

        Try

            Randomize()

            randomValue = CInt(Math.Floor((upperbound - lowerbound + 1) * Rnd())) + lowerbound          '대문자,소문자, 숫자를 결정한다.

            If randomValue = 1 Then         '대문자
                randomResult = CInt(Math.Floor((upperbound_L_X - lowerbound_L_X + 1) * Rnd())) + lowerbound_L_X

            ElseIf randomValue = 2 Then     '소문자
                randomResult = CInt(Math.Floor((upperbound_S_X - lowerbound_S_X + 1) * Rnd())) + lowerbound_S_X

            Else                            '숫자
                randomResult = CInt(Math.Floor((upperbound_9 - lowerbound_9 + 1) * Rnd())) + lowerbound_9
            End If


            WJSCRYPTO_RAND = Chr(randomResult)


        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("WJSCRYPTO_RAND Error : " & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error)

        Finally

        End Try


    End Function

    '암화과 관련 끝


    ''' <summary>
    ''' ChkExtUserDataSource
    ''' </summary>
    ''' <param name="oForm"></param>
    ''' <param name="strUid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ChkExtUserDataSource(ByRef oForm As SAPbouiCOM.Form, ByVal strUid As String) As Boolean


        Dim i As Integer = 0
        Dim xmlAtt As Xml.XmlAttribute
        Dim xmlDoc As Xml.XmlDocument = New Xml.XmlDocument()
        xmlDoc.LoadXml(oForm.GetAsXML())

        For Each node As XmlNode In xmlDoc.SelectNodes("Application/forms/action/form/datasources/userdatasources/action/datasource")
            xmlAtt = node.Attributes.ItemOf("uid")
            If (Not xmlAtt Is Nothing) Then
                If (xmlAtt.InnerText = strUid) Then
                    Return True
                End If
            End If
        Next

        Return False

    End Function


    ''' <summary>
    ''' GetUserText
    ''' </summary>
    ''' <param name="strItemCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserText(ByVal strItemCode As String) As String
        Dim strUserText As String = ""

        Try

            strUserText = CFL.GetValue("SELECT IFNULL(A.""UserText"", '') FROM OITM A WHERE A.""ItemCode"" = '" & Trim(strItemCode) & "' ")

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("GetUserText : " & Err.Description, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error)
        End Try

        Return strUserText
    End Function
    Public Function GetMatrixColumnIdx(ByVal oMatrix As SAPbouiCOM.Matrix, ByVal strColumnId As String) As Integer
        Dim i As Integer
        For i = 1 To oMatrix.Columns.Count
            If (oMatrix.Columns.Item(i).UniqueID = strColumnId) Then
                Return i
            End If
        Next
        Return 1

    End Function
    Public Function FConvertDT(ByVal sdata As String) As Date
        ' sdata 는 8 자리로 넘어와야 한다. 20140201
        Dim dt As Date
        Dim iYYYY, iMM, iDD As Integer

        iYYYY = Int32.Parse(sdata.Substring(0, 4))
        iMM = Int32.Parse(sdata.Substring(4, 2))
        iDD = Int32.Parse(sdata.Substring(6, 2))

        dt = New Date(iYYYY, iMM, iDD)

        Return dt

    End Function
    Public Function GetJsonStringData(ByVal key As String, ByVal value As String) As String
        Dim cv_Json_s As String

        cv_Json_s = " """ & key & """ : """ + value & """  "

        Return cv_Json_s

    End Function

    Public Function ParsingJsonRetunData(ByVal strResult As String, ByRef ReturnCode As String, ByRef ReturnDesc As String) As Integer
        Dim cv_Error_i As Integer
        Dim iRow As Integer = 0

        cv_Error_i = strResult.IndexOf("returnCode")

        If cv_Error_i < 0 Then

            Return cv_Error_i

        End If

        strResult = strResult.Replace("""", "").Replace("{", "").Replace("}", "")


        Dim separators() As String = {":", ","}
        Dim cv_ResultVals_s() As String = strResult.Split(separators, StringSplitOptions.RemoveEmptyEntries)
        Dim cv_ResultVal_s As String

        For Each cv_ResultVal_s In cv_ResultVals_s

            If iRow = 1 Then
                ReturnCode = cv_ResultVal_s
            ElseIf iRow = 3 Then
                ReturnDesc = cv_ResultVal_s
            End If

            iRow = iRow + 1

        Next

        Return cv_Error_i

    End Function

    Public Function GetGridSelectedRow(ByVal oGrid As SAPbouiCOM.Grid) As Integer
        '선택되어있는 row를 리턴함
        Dim i As Integer
        Dim sel As Integer = -1
        If oGrid.DataTable.Rows.Count > 0 Then
            For i = 0 To oGrid.Rows.Count - 1
                If oGrid.Rows.IsSelected(i) Then
                    sel = i
                End If
            Next i
        End If
        GetGridSelectedRow = sel
    End Function

    '******************************************************
    '리모트 서비스 관련
    '******************************************************

    Public Function UrlEncode(ByVal Text As String) As String
        Dim i As Integer
        Dim Ansi() As Byte
        Dim AsciiCode As Short
        Dim strEncode As String


        Ansi = System.Text.Encoding.UTF8.GetBytes(Text)
        strEncode = ""


        For i = 0 To UBound(Ansi)
            AsciiCode = Ansi(i)


            Select Case AsciiCode
                Case 48 To 57, 65 To 90, 97 To 122
                    strEncode = strEncode & Chr(AsciiCode)
                Case 32
                    strEncode = strEncode & "+"
                Case Else
                    If AsciiCode < 16 Then
                        strEncode = strEncode & "%0" & Hex(AsciiCode)
                    Else
                        strEncode = strEncode & "%" & Hex(AsciiCode)
                    End If
            End Select
        Next i
        UrlEncode = strEncode
    End Function

    Public Function GetCOTEMPLET(Optional ByVal sCACD As String = "", Optional ByVal sCT As String = "") As String

        Dim xSQL As String
        Dim oRS As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim v_RTNVAL As String = ""
        Try
            xSQL = " SELECT COUNT(DISTINCT(A.U_TPL))"
            xSQL = xSQL & vbCrLf & " FROM [@WJS_SCO10M] AS A"
            xSQL = xSQL & vbCrLf & " WHERE A.U_CACD = CASE WHEN N'" & sCACD & "' = '' THEN A.U_CACD ELSE '" & sCACD & "' END "
            xSQL = xSQL & vbCrLf & " AND A.U_CT = CASE WHEN N'" & sCT & "' = '' THEN A.U_CT ELSE '" & sCT & "' END "
#If HANA = "Y" Then
            xSQL = CFL.GetConvertHANA(xSQL)
#End If
            oRS.DoQuery(xSQL)

            If Not (oRS.EoF) Then

                If oRS.Fields.Item(0).Value > 1 Then
                    v_RTNVAL = ""
                    Return v_RTNVAL
                Else

                    xSQL = "SELECT TOP 1 U_TPL FROM [@WJS_SCO10M] WHERE U_CT = CASE WHEN N'" & sCT & "' = '' THEN U_CT ELSE '" & sCT & "' END "
                    v_RTNVAL = CFL.GetValue(xSQL)
                    Return v_RTNVAL
                End If

            End If

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("GetCOTEMPLET" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing

        End Try

    End Function

#Region "PU"

    Public Function AttachPath() As Boolean
        Dim oPathAdmin As SAPbobsCOM.PathAdmin
        Dim oCompanyService As CompanyService '

        Try

            AttachPath = False

            Dim Rs As SAPbobsCOM.Recordset = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim xSQL As String = ""
            Dim PATH As String = ""
            Dim TEXTLONG As Long

            xSQL = "select AttachPath from OADP"
            Rs.DoQuery(xSQL)

            PATH = Rs.Fields.Item("AttachPath").Value.ToString.Trim()
            TEXTLONG = Len(PATH) - 9

            PATH = Mid(PATH, 1, TEXTLONG)

            PATH = PATH & Date.Today.Year.ToString & IIf(CStr(Date.Today.Month.ToString).Length = 1, "0" & Date.Today.Month.ToString, Date.Today.Month.ToString) & IIf(CStr(Date.Today.Day.ToString).Length = 1, "0" & Date.Today.Day.ToString, Date.Today.Day.ToString) & "\"

            If System.IO.Directory.Exists(PATH) = False Then '로 폴더 존재 여부를 검사하고,
                System.IO.Directory.CreateDirectory(PATH) '로 폴더를 생성합니다.
            End If

            'Directory 생성후 B1첨부폴더비교후 불일치시 첨부파일 Path변경


            oCompanyService = B1Connections.diCompany.GetCompanyService
            oPathAdmin = B1Connections.diCompany.GetCompanyService.GetPathAdmin

            If (oPathAdmin.AttachmentsFolderPath <> PATH) Then
                oPathAdmin.AttachmentsFolderPath = PATH
                oCompanyService.UpdatePathAdmin(oPathAdmin)
            End If

            AttachPath = True

        Catch ex As Exception
            Throw ex
        Finally
            oPathAdmin = Nothing
            oCompanyService = Nothing
        End Try


    End Function

    Public Function DistNumberCaption() As String
        Dim strReturn As String = ""

        strReturn = CFL.GetCaption(CFL.GetValue("SELECT ISNULL(U_DISTCAP,N'일련번호') FROM [@WJS_SCO01M]"), ModuleIni.CO)

        Return strReturn
    End Function

    Public Function DistNumberVisible() As Boolean

        Try

            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SCO111] WHERE U_STEMH = 'S06'") = "1" Then
                DistNumberVisible = True
            Else
                DistNumberVisible = False
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("DistNumberVisible" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally

        End Try

    End Function


    Public Function DistNumberVisible_Product() As Boolean

        Try

            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SCO111] WHERE U_CITGBN IN ('S02','S03') AND U_STEMH = 'S06'") = "1" Then
                DistNumberVisible_Product = True
            Else
                DistNumberVisible_Product = False
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("DistNumberVisible_Product" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally

        End Try

    End Function


    Public Function DistNumberVisible_NOTSupply() As Boolean

        Try

            If CFL.GetValue("SELECT TOP 1 1 FROM [@WJS_SCO111] WHERE U_CITGBN NOT IN ('S06') AND U_STEMH = 'S06'") = "1" Then
                DistNumberVisible_NOTSupply = True
            Else
                DistNumberVisible_NOTSupply = False
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText("DistNumberVisible_NOTSupply" & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally

        End Try

    End Function


#End Region

#Region "QM"
    Public Function CreateFileServer(ByVal strGbn As String, ByVal strFileFullPath As String, ByVal strFolder As String, Optional ByVal COREYN As Boolean = False) As String
        Dim rValue As String = ""
        Dim strServerDir As String = ""

        Try

            If COREYN Then

            Else

                strServerDir = CFL.GetValue("SELECT CONVERT(NVARCHAR(MAX), U_RMK1) FROM [@WJS_SAD021] WHERE Code = 'DSK_AD01' AND U_CD = " & CFL.GetQD(strGbn))

                If strServerDir = "" Then
                    '사용자정의 코드에 첨부파일경로가 지정되어 있지 않습니다. 첨부파일 경로를 지정 하십시오.
                    CFL.COMMON_MESSAGE("!", CFL.GetMSG("DSK350"))
                    Exit Try
                Else

                    If Not IO.Directory.Exists(strServerDir) Then
                        '첨부파일 서버로 접근이 불가능 합니다.
                        CFL.COMMON_MESSAGE("!", CFL.GetMSG("DSK349"))
                        Exit Try
                    End If

                    '화면에서 넘어온 경로가 없으면 새로운 경로 설정
                    If strFolder = "" Then strFolder = GetTimeStamp()

                    strServerDir = strServerDir & "\" & strFolder

                    '첨부파일 경로신규생성
                    If Not IO.Directory.Exists(strServerDir) Then IO.Directory.CreateDirectory(strServerDir)

                    strServerDir = strServerDir & "\" & Right(strFileFullPath, strFileFullPath.Length - (strFileFullPath.LastIndexOf("\") + 1))

                    If IO.File.Exists(strServerDir) Then
                        '이 이름을 가진 파일이 이미 있습니다. + 경로 + 파일을 교체하겠습니까?
                        If CFL.COMMON_MESSAGE("?", CFL.GetMSG("DSK347") & "  " & strServerDir & "  " & CFL.GetMSG("DSK348")) <> 1 Then Exit Try
                    End If

                End If

            End If

            rValue = strFolder
        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message.ToString)
        End Try

        Return rValue
    End Function
    ''' <summary>
    ''' 공장권한에 따른 Default PLANT 셋팅
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <param name="oCombo"></param>
    ''' <remarks></remarks>
    Public Sub SetDfltPlantCombo(ByVal strUserId As String, ByRef oCombo As SAPbouiCOM.ComboBox)
        Dim xSQL As String = ""
        If (oCombo.ValidValues.Count > 0) Then
            xSQL = " select U_PLANT "
            xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511]  "
            xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
            xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y' and isnull(U_DFTYN,'N') = 'Y' "

            'xSQL = "SELECT U_PLCD FROM [@WJS_SAD63M] WHERE ISNULL(U_DFLTPLT, N'N') = N'Y'"

            Dim strDfltPlant As String = CFL.GetValue(xSQL)

            If (strDfltPlant <> "") Then
                oCombo.Select(strDfltPlant, BoSearchKey.psk_ByValue)
            End If
        End If
    End Sub
    ''' <summary>
    ''' 공장권한에 따른 Plant Combo 셋팅
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <param name="oCombo"></param>
    ''' <remarks></remarks>
    Public Sub SetPLANTCombo(ByVal strUserId As String, ByRef oCombo As SAPbouiCOM.ComboBox)
        Dim Rs As SAPbobsCOM.Recordset

        Dim xSQL As String = ""

        Try

            Rs = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""
            'S01 전체, S02 읽기전용 , S3 없음
            xSQL = " select U_PLANT, (select U_PLNM From [@WJS_SAD63M] where U_PLCD = a.U_PLANT) as U_PLANTNM, U_DFTYN "
            xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511] a "
            xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
            xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y'"


            'xSQL = xSQL & vbCrLf & " SELECT U_PLCD AS U_PLANT, U_PLNM AS U_PLANTNM FROM [@WJS_SAD63M] "

            Rs.DoQuery(xSQL)

            Dim i As Integer
            For i = 0 To Rs.RecordCount - 1
                oCombo.ValidValues.Add(Rs.Fields.Item("U_PLANT").Value.ToString.Trim(), Rs.Fields.Item("U_PLANTNM").Value.ToString().Trim())
                Rs.MoveNext()
            Next

            If (oCombo.ValidValues.Count > 0) Then
                xSQL = " select U_PLANT "
                xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511]  "
                xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
                xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y' and isnull(U_DFTYN,'N') = 'Y' "

                'xSQL = "SELECT U_PLCD FROM [@WJS_SAD63M] WHERE ISNULL(U_DFLTPLT, N'N') = N'Y'"

                Dim strDfltPlant As String = CFL.GetValue(xSQL)

                If (strDfltPlant <> "") Then
                    oCombo.Select(strDfltPlant, BoSearchKey.psk_ByValue)
                End If
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            Rs = Nothing
        End Try

    End Sub


    ''' <summary>
    ''' 공장권한에 따른 Plant Combo 셋팅
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <param name="oCombo"></param>
    ''' <remarks></remarks>
    Public Sub SetPLANTComboList(ByRef oCombo As SAPbouiCOM.ComboBox)
        Dim Rs As SAPbobsCOM.Recordset

        Dim xSQL As String = ""

        Try

            Rs = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            xSQL = xSQL & vbCrLf & " SELECT U_PLCD AS U_PLANT, U_PLNM AS U_PLANTNM FROM [@WJS_SAD63M] "

            Rs.DoQuery(xSQL)

            Dim i As Integer
            For i = 0 To Rs.RecordCount - 1
                oCombo.ValidValues.Add(Rs.Fields.Item("U_PLANT").Value.ToString.Trim(), Rs.Fields.Item("U_PLANTNM").Value.ToString().Trim())
                Rs.MoveNext()
            Next

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            Rs = Nothing
        End Try

    End Sub


    ''' <summary>
    ''' 공장권한에 따른 Plant Combo 셋팅
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <param name="oCombo"></param>
    ''' <remarks></remarks>
    Public Sub SetPLANTCombo(ByVal strUserId As String, ByRef oCombo As SAPbouiCOM.ComboBox, ByVal strGubun1 As Boolean)
        Dim Rs As SAPbobsCOM.Recordset

        Dim xSQL As String

        Try

            Rs = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            If strGubun1 = True Then
                'xSQL = " select U_PLANT, (select U_PLNM From [@WJS_SAD63M] where U_PLCD = a.U_PLANT) as U_PLANTNM, U_DFTYN "
                'xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511] a "
                'xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
                'xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y'"

                xSQL = " SELECT U_PLCD AS U_PLANT, U_PLNM AS U_PLANTNM FROM [@WJS_SAD63M] "

                Rs.DoQuery(xSQL)

                Dim i As Integer
                For i = 0 To Rs.RecordCount - 1
                    oCombo.ValidValues.Add(Rs.Fields.Item("U_PLANT").Value.ToString.Trim(), Rs.Fields.Item("U_PLANTNM").Value.ToString().Trim())
                    Rs.MoveNext()
                Next

            End If

            If (oCombo.ValidValues.Count > 0) Then
                'xSQL = " select U_PLANT "
                'xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511]  "
                'xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
                'xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y' and isnull(U_DFTYN,'N') = 'Y' "
                'xSQL = xSQL & vbCrLf & " UNION ALL "
                'xSQL = xSQL & vbCrLf & " select U_PLANT "
                'xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511]  "
                'xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
                'xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y' "

                xSQL = "SELECT U_PLCD FROM [@WJS_SAD63M] WHERE ISNULL(U_DFLTPLT, N'N') = N'Y'"

                Dim strDfltPlant As String = CFL.GetValue(xSQL)

                If (strDfltPlant <> "") Then
                    oCombo.Select(strDfltPlant, BoSearchKey.psk_ByValue)
                End If
            End If

        Catch ex As Exception
            B1Connections.theAppl.StatusBar.SetText(Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
        Finally
            Rs = Nothing
        End Try

    End Sub


    ''' <summary>
    ''' 공장권한에 따른 Plant Combo 셋팅
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <param name="oCombo"></param>
    ''' <remarks></remarks>
    Public Sub SetPLANTCombo(ByVal strUserId As String, ByRef oCombo As SAPbouiCOM.ComboBox, ByRef DbSrc As SAPbouiCOM.DBDataSource, ByVal strFieldNm As String)
        Dim Rs As SAPbobsCOM.Recordset
        Dim xSQL As String = ""

        Try

            Rs = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""

            'xSQL = " select U_PLANT, (select U_PLNM From [@WJS_SAD63M] where U_PLCD = a.U_PLANT) as U_PLANTNM, U_DFTYN "
            'xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511] a "
            'xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
            'xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y'"

            xSQL = xSQL & vbCrLf & " SELECT U_PLCD AS U_PLANT, U_PLNM AS U_PLANTNM FROM [@WJS_SAD63M] "

            Rs.DoQuery(xSQL)

            Dim i As Integer
            For i = 0 To Rs.RecordCount - 1
                oCombo.ValidValues.Add(Rs.Fields.Item("U_PLANT").Value.ToString.Trim(), Rs.Fields.Item("U_PLANTNM").Value.ToString().Trim())
                Rs.MoveNext()
            Next

            If (oCombo.ValidValues.Count > 0) Then
                'xSQL = " select U_PLANT "
                'xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511]  "
                'xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
                'xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y' and isnull(U_DFTYN,'N') = 'Y' "
                'xSQL = xSQL & vbCrLf & " UNION ALL "
                'xSQL = xSQL & vbCrLf & " select U_PLANT "
                'xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511]  "
                'xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
                'xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y' "

                xSQL = "SELECT U_PLCD FROM [@WJS_SAD63M] WHERE ISNULL(U_DFLTPLT, N'N') = N'Y'"

                Dim strDfltPlant As String = CFL.GetValue(xSQL)

                If (strDfltPlant <> "") Then
                    DbSrc.SetValue(strFieldNm, 0, strDfltPlant)
                End If
            End If

        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally
            Rs = Nothing
        End Try

    End Sub


    ''' <summary>
    ''' 공장권한에 따른 Array 리턴
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <remarks></remarks>
    Public Function GetPLANTArray(ByVal strUserId As String) As ArrayList
        GetPLANTArray = Nothing

        Dim Rs As SAPbobsCOM.Recordset
        Dim xSQL As String = ""
        Dim arr As ArrayList = New ArrayList()

        Try

            Rs = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""

            xSQL = " select U_PLANT, (select U_PLNM From [@WJS_SAD63M] where U_PLCD = a.U_PLANT) as U_PLANTNM, U_DFTYN "
            xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511] a "
            xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
            xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y'"

            Rs.DoQuery(xSQL)

            Dim i As Integer
            For i = 0 To Rs.RecordCount - 1
                arr.Add(Rs.Fields.Item("U_PLANT").Value.ToString.Trim())
                Rs.MoveNext()
            Next

            GetPLANTArray = arr

        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally
            Rs = Nothing
            arr = Nothing
        End Try

        Return GetPLANTArray
    End Function

    ''' <summary>
    ''' 공장권한에 따른 Default 공장 리턴
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <remarks></remarks>
    Public Function GetPLANTStr(ByVal strUserId As String) As String

        Dim Rs As SAPbobsCOM.Recordset
        Dim xSQL As String = ""
        Dim sDftPLANT As String

        Try

            Rs = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            xSQL = ""

            xSQL = " select U_PLANT"
            xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511] a "
            xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
            xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y' and isnull(U_DFTYN,'N') = 'Y'"

            Rs.DoQuery(xSQL)

            sDftPLANT = Rs.Fields.Item("U_PLANT").Value.ToString.Trim()

        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally
            Rs = Nothing
        End Try

        Return sDftPLANT
    End Function


    ''' <summary>
    ''' 공장권한에 따른 Default 사용자별 공장 리스트
    ''' </summary>
    ''' <param name="strUserId"></param>
    ''' <remarks></remarks>
    Public Function GetPLANTLIST(ByVal strUserId As String) As String
        Dim xSQL As String = ""
        Try
            xSQL = ""
            xSQL = " select U_PLANT,U_PLANTNM"
            xSQL = xSQL & vbCrLf & "  from [@WJS_SAD511] a "
            xSQL = xSQL & vbCrLf & " where U_USERID = " + CFL.GetQD(strUserId)
            xSQL = xSQL & vbCrLf & " and (U_PLANTRL = 'S01' or U_PLANTRL = 'S02') and isnull(U_USEYN,'N') = 'Y'"
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally

        End Try
        Return xSQL
    End Function

    'Gantt Tree에서 사용하는 ICON - BASE64 Format
    '1:폴더(Close), 2:List, 3:폴더(Open)
    Public Const strTREEIMG As String = _
   "gBJJgBAIDAAGAAEAAQhYAf8Pf4hh0QihCJo2AEZjQAjEZFEaIEaEEaAIAkcbk0olUrlktl0vmExmUzmk1m03nE5nU7nk9n0/oFBoVDolFo1HpFJpVLplNp1PqFRqVTqlVq1XrFZrVbrldr1fsFhsVjslls1ntFptVrtltt1vuFxuVzul1u13vF5vV7vl9v1/jKXSSPf+CwmGwuDxOHxWIx2NyGMyWLymIlgJCgWDYeI5QMYwPSMWJAUzAeYtPKNZAXHRFNAKCwaiWAseIfz9fz/fr8fr/fe/f76fT5f74fL4f735T/er2e3Mej0f7z6j/eXXf7x7T/eHd7ne7rwykbJaUUi8L7DbL/LrAaz/Ly/95g+T/IivZT/JamYL/KxcGo+BgGu9hfwILhfPeJxRlubYUCoL45Nop7bH83J+wwf5yGycR/nEbRwn+axowCdBvmY5LlnrFZ/ui6Trnm7Ltu0eLvvE8Lxo0NhnRCQBqHIf47mccB/jeZJun+M5iG0+z8H+IZTGGf4nFxAgnGCb0pyxLUsigYMkCfBhtrYQEygBMpATPM00TVNM2TfNc4zdOU2zqljEH+3M8wsf5tG0bB/nObsUFgQgYxEYpTRS5DnHq6DpOpF7sRo7J4RrHDLI0OBoyARJtnQf8fSBIUiSNJD7vyIpUGJKcqy5V8ryyaZSiTAsCTJMyIIyh4AGBX1dn/XtfonYRgWBYtj19Y1iTZO7FIpCzcmQW5NH+W5BhEf5ckIBR/mASojn+ex" _
   + "6HlcR7ueVZMWqVZL2qVV2n/d5M3iS150wxSNjqahzn+SRwnYf9PVBUUgyGf4ilg/IklTVgpF1QApGGb9YS2bpci7AVAVxNNdWVZNh15j1iZFkOQWDj1mpXPCKTyf5hk8LZ/l+SAcH+ZBYERaxBgOf5tmESrinvRlzuZFlIuseUYnk7buxrTKMjybB0n+TZzvFf2AYFUMfn+I5ZRQJxWGM/xePWLZjxCKxiyJiOJnZt5/jAYL142idg5Hk1kbxZeS75k+85SlWV5Y4p6HUf58nvcrdn4f5mlOJx/mISwQO4dUCOUe9xRZFzpuq69y0pp4AD6beAFGdjparq9/4DT5/iUWhnH+KhXmQf4uF/JgzGbIAwGUcZ/7PEO34AMRhSZuuO7zkm/79vW+4/Y3ApRweWNz7E+Qw3p4HKaB/mYVIQn+a5eC5RdzOfFdHHoedIUmeNy9GQJvHdeJ4811HVatfvWicLd74WxZDJH+GMYQ3B/hzGmqANo0BzD/d4kB4sBRhjbbuWpNium9vSg5BuDyvHqEbes4Qh72E9m5H4bwf46BtCdH+NUXYN0NDVMIPg5Zzjnj0aM+06w835L4I0IQcB4hYD1OIKp/A/39D/GMPE57DhpD/DCLYZY/w0jGSyIIbTAA9DXcPAqBkDh/hlGLAh5T0YPvOg7Gh6MISNQjhJCZaJukMwpH2h4aIfB/jeGUGk4o+DkLnc0+tFp0XPIxdGIgca5RaD3jvEWI8SR0D6caFYXqAQ0C5GaP8OA" _
   + "yUiCQHEeIRL9R/xaYAG8ZiHQeh0EEJaM7d42RqjS9PLSIEzLOMIM0Zh+ZdS8l246X8vZgS+mJMOY0wpkTBl+6MRQ5UYyNjvNAf8kEWm4H+FpAY/w3C8dmHYZiIRPDpkQOVcsoDxBrGSxMuEGVeK6ndO2eCwZ3zyni3ae0bjAmKHANkaY/59z9n/P6flAqAUDoDQegYkQ8hooIP+hVDKA0PRyRkRo5lHTSoxI4f7jSHhfSWP8Ogvnvh9Ge8EVg73NClHYo6cKMQ3DKRDOtMzzZaU1ZQnROCc6dEtMeZOnplTI1Ap9UGn7oxGjnovRqjMdyKBkGQkgPAwBoj/ECNIcq1h6D6H+LEeRxKTuaDhKmC5cU2U0rMr+fBtKj1JmjRqjlGyIBpGWkQP4xhqj/FcO454vx8uNF3RoW9WR/hyd7WMulOU62IQmSoAIAgBADrXVse0dx0j7N6JkdR0hDjfHaP8PIzYEC8HrVoYQ+je2ltPaYf4th6HEDKLQY43rF2zJqGYUgrxbCLHIPIfgjRyHSESOBcoiBvI1ESN1GohhtjvH+IMbL9hADXYAH0arhw1C3GSN4GwXw0h1LgRYHxK7wkeJASIkgACSkovSTiDVtChK8V6H9Y6ZyDJnvSIC+t6CM34vPfS/t/CM33vzgK/+A794GTOAe/+CiM31EBgwQBEMHkZP6rzBhFMJgAICA=" _
   + "4r1cYtACYtDn4okeYgYlYg4sgDiMhwAhhgAH41CHoUBEjFAAjEB8QMAfvDjEBwYzQMY4jn4hgM47Yv4Z49hACS5CHsZCY/m35BCN5FCSZDZGkYYa4/4eZH4mvDjNh4YzYliH45vDiHiAg=="

    Public Sub GanttExpandAll(ByRef oGantt As EXG2ANTTLib.G2antt, ByVal bExpand As Boolean)
        With oGantt
            .BeginUpdate()
            Dim i As Integer
            With .Items
                For Each i In oGantt.Items
                    .ExpandItem(i) = bExpand
                Next
            End With
            .EndUpdate()
        End With
    End Sub
    Public Sub GanttSetTreeVw(ByRef oGantt As EXG2ANTTLib.G2antt)


        Try

            oGantt.BeginUpdate()

            oGantt.BackColor = RGB(217, 229, 242)

            '간트 셋팅
            oGantt.Chart.PaneWidth(True) = 0


            oGantt.HeaderVisible = True
            oGantt.ColumnAutoResize = False
            oGantt.LinesAtRoot = EXG2ANTTLib.LinesAtRootEnum.exLinesAtRoot
            oGantt.FullRowSelect = EXG2ANTTLib.CellSelectEnum.exItemSel
            oGantt.MarkSearchColumn = False
            oGantt.ExpandOnDblClick = False

            '간트 셋팅시작
            Dim oGanCol As EXG2ANTTLib.Column

            'Index0
            oGanCol = oGantt.Columns.Add()
            oGanCol.Editor.Locked = True
            oGanCol.AllowSort = False
            oGanCol.WidthAutoResize = True


            oGantt.HeaderVisible = False


        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message)
        Finally
            oGantt.EndUpdate()
        End Try


    End Sub
    ''' <summary>
    ''' GetTimeStamp
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTimeStamp(Optional ByVal strGBN As String = "DB") As String
        Dim rValue As String = ""

        If strGBN = "DB" Then
            rValue = CFL.GetValue("SELECT CONVERT(NVARCHAR(10), GETDATE(), 112) + REPLACE(CONVERT(NVARCHAR(20), GETDATE(), 114), ':', '')")
        Else
            rValue = CFL.GetValue("SELECT CONVERT(NVARCHAR(10), GETDATE(), 112) + REPLACE(CONVERT(NVARCHAR(20), GETDATE(), 114), ':', '')")
        End If

        Return rValue
    End Function
    Public Sub MoveFile(ByVal strGbn As String, ByVal strFileFullPath As String, ByVal strFolder As String, Optional ByVal COREYN As Boolean = False)
        Dim strServerDir As String = ""
        Dim strServerFile As String = ""

        Try

            If COREYN Then

            Else

                strServerDir = CFL.GetValue("SELECT CONVERT(NVARCHAR(MAX), U_RMK1) FROM [@WJS_SAD021] WHERE Code = 'LHC_AD01' AND U_CD = " & CFL.GetQD(strGbn))

                strServerFile = strServerDir & "\" & strFolder & "\" & Right(strFileFullPath, strFileFullPath.Length - (strFileFullPath.LastIndexOf("\") + 1))

                '서버로 파일 복사
                IO.File.Copy(strFileFullPath, strServerFile, True)

            End If

        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message.ToString)
        End Try

    End Sub
    Public Function RemoveFile(ByVal strGbn As String, ByVal strFile As String, ByVal strFolder As String, Optional ByVal COREYN As Boolean = False) As Boolean
        Dim rValue As Boolean = False
        Dim strServerDir As String = ""
        Dim strDeleteFullPath As String = ""

        Try

            If COREYN Then

            Else

                strServerDir = CFL.GetValue("SELECT CONVERT(NVARCHAR(MAX), U_RMK1) FROM [@WJS_SAD021] WHERE Code = 'DSK_AD01' AND U_CD = " & CFL.GetQD(strGbn))

                If strServerDir = "" Then
                    '사용자정의 코드에 첨부파일경로가 지정되어 있지 않습니다. 첨부파일 경로를 지정 하십시오.
                    CFL.COMMON_MESSAGE("!", CFL.GetMSG("DSK350"))
                    Exit Try
                Else

                    If Not IO.Directory.Exists(strServerDir) Then
                        '첨부파일 서버로 접근이 불가능 합니다.
                        CFL.COMMON_MESSAGE("!", CFL.GetMSG("DSK349"))
                        Exit Try
                    End If

                    strDeleteFullPath = strServerDir & "\" & strFolder & "\" & strFile

                    '파일 삭제
                    If IO.File.Exists(strDeleteFullPath) Then
                        '파일서버에 있는 파일이 바로 제거 됩니다. 제거 하시겠습니까?
                        If CFL.COMMON_MESSAGE("?", CFL.GetMSG("DSK358")) = 1 Then
                            IO.File.Delete(strDeleteFullPath)
                        Else
                            Exit Try
                        End If
                    End If

                End If

            End If

            rValue = True
        Catch ex As Exception
            CFL.COMMON_MESSAGE("!", ex.Message.ToString)
        End Try

        Return rValue
    End Function

#End Region

    '// 파일의 읽기전용 속성 변경
    Public Sub SetReadonly(ByVal sFileName As String, ByVal bReadOnly As Boolean)
        Dim fileInfo As System.IO.FileInfo
        fileInfo = New System.IO.FileInfo(sFileName)

        If fileInfo.IsReadOnly <> bReadOnly Then
            fileInfo.IsReadOnly = bReadOnly
        End If

        fileInfo = Nothing

    End Sub
    Public Function GetParentForm(ByVal strParentFormUid As String) As SAPbouiCOM.Form

        Dim i As Integer

        For i = 0 To B1Connections.theAppl.Forms.Count - 1
            If (B1Connections.theAppl.Forms.Item(i).UniqueID = strParentFormUid) Then
                Return B1Connections.theAppl.Forms.Item(i)
            End If
        Next

        Return Nothing

    End Function

#Region "KillExcel"
    Public Sub KillExcel(ByVal pintHandle As IntPtr)
        Dim intResult As Long
        Try
            'this will perform the end task activity whereby it will release the excel process from the task manager.
            intResult = EndTask(pintHandle)
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Declare Function EndTask Lib "user32.dll" Alias "EndTask" (ByVal hwnd As Long) As Long
    Public Declare Function FindWindow Lib "user32.dll" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
#End Region

    Public Sub DeleteRow(ByVal pDBDS As SAPbouiCOM.DBDataSource, ByVal pMtx As SAPbouiCOM.Matrix, ByVal pRowIndex As Integer)
        pDBDS.RemoveRecord(pRowIndex)
        pMtx.LoadFromDataSource()
    End Sub


    Public Sub ClearMatrixSpaceLine(ByVal pDbds As SAPbouiCOM.DBDataSource, ByVal pMtx As SAPbouiCOM.Matrix, ByVal DbdsColIndex As String)
        Dim i As Integer
        pMtx.FlushToDataSource()
        For i = pDbds.Size - 1 To 0 Step -1
            If pDbds.GetValue(DbdsColIndex, i).ToString().Trim() = "" Then
                pDbds.Offset = i
                DeleteRow(pDbds, pMtx, pDbds.Offset)
            End If
        Next

    End Sub

    Public Function DeleteDuplicate(ByVal inString As String, ByVal delimiter As String)

        If Len(delimiter) = 0 Then
            DeleteDuplicate = inString
            Exit Function
        End If
        Dim inArray, inLength, i, j, checkCount, lastCheck
        Dim tempCount, tempInput
        inArray = Split(inString, delimiter)
        inLength = UBound(inArray)

        For i = 0 To inLength

            For j = CInt(i + 1) To inLength - 1

                If inArray(i) = inArray(j) Then

                    checkCount = checkCount + CInt(1)

                End If

            Next

            tempCount = tempCount + checkCount

            If checkCount = 0 Then ' 중복된 것이 없음

                If i = inLength Then ' 배열의 마지막까지 비교했음

                    lastCheck = InStr(tempInput, inArray(i))

                    If lastCheck = 0 Then

                        tempInput = tempInput & inArray(i)

                    End If

                Else

                    tempInput = tempInput & inArray(i)

                    tempInput = tempInput & delimiter

                End If

            End If

            checkCount = CInt(0)

        Next

        ' delete last delimiter

        Dim lastDelimiterIndex As Integer

        lastDelimiterIndex = InStrRev(tempInput, delimiter)

        If Mid(tempInput, lastDelimiterIndex) = delimiter Then

            tempInput = Mid(tempInput, 1, lastDelimiterIndex - 1)
        End If
        DeleteDuplicate = tempInput
    End Function

    'Public _WJSIFDBNAME As String = "WJSIF_PLS"
    'Public _WJSSYDBNAME As String = "WJSSY_PLS"

    Private Function CrystalDecisions() As Object
        Throw New NotImplementedException
    End Function

    Public Function BindGrid_array(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As ArrayList, ByVal pColNms As ArrayList, ByVal pEdCols As ArrayList, ByVal pViCols As ArrayList, ByVal pAlignCols As ArrayList, ByVal pColor1Cols As ArrayList, ByVal pColor2Cols As ArrayList) As Boolean
        BindGrid_array = False

        'Dim cols() As String
        'Dim colNms() As String
        'Dim affCols() As String
        'Dim edCols() As String
        'Dim viCols() As String
        'Dim alignCols() As String
        Dim i As Integer

        Try

            If pCols.Count <> pColNms.Count Then
                Exit Function
            End If


            For i = 0 To pCols.Count - 1
                oGrid.Columns.Item(pCols.Item(i).ToString.Trim).TitleObject.Caption = CFL.GetCaption(pColNms.Item(i).ToString.Trim, ModuleIni.CO)
            Next i

            For i = 0 To pEdCols.Count - 1
                oGrid.Columns.Item(pEdCols.Item(i)).Editable = False
            Next i

            For i = 0 To pViCols.Count - 1
                oGrid.Columns.Item(pViCols.Item(i)).Visible = False
            Next i

            For i = 0 To pAlignCols.Count - 1
                oGrid.Columns.Item(pAlignCols.Item(i)).RightJustified = True
            Next i

            For i = 0 To pColor1Cols.Count - 1
                oGrid.Columns.Item(pColor1Cols.Item(i)).BackColor = 12777465
            Next i


            For i = 0 To pColor2Cols.Count - 1
                oGrid.Columns.Item(pColor2Cols.Item(i)).BackColor = 13624308
            Next i


            'If pEdCols.ToString.Trim.Length > 0 Then
            '    edCols = Split(pEdCols, ",")
            '    For i = LBound(edCols) To UBound(edCols)
            '        oGrid.Columns.Item(edCols(i)).Editable = False
            '    Next i
            'End If

            'If pViCols.ToString.Trim.Length > 0 Then
            '    viCols = Split(pViCols, ",")
            '    For i = LBound(viCols) To UBound(viCols)
            '        oGrid.Columns.Item(viCols(i)).Visible = False
            '    Next i
            'End If

            'If pAffCols.ToString.Trim.Length > 0 Then
            '    affCols = Split(pAffCols, ",")
            '    For i = LBound(affCols) To UBound(affCols)
            '        oGrid.Columns.Item(affCols(i)).AffectsFormMode = False
            '    Next i
            'End If

            'If pAlignCols.ToString.Trim.Length > 0 Then
            '    alignCols = Split(pAlignCols, ",")
            '    For i = LBound(alignCols) To UBound(alignCols)
            '        oGrid.Columns.Item(alignCols(i)).RightJustified = True
            '    Next i
            'End If

            'If pColor1Cols.ToString.Trim.Length > 0 Then
            '    alignCols = Split(pColor1Cols, ",")
            '    For i = LBound(alignCols) To UBound(alignCols)
            '        oGrid.Columns.Item(alignCols(i)).BackColor = 12777465
            '    Next i
            'End If

            'If pColor2Cols.ToString.Trim.Length > 0 Then
            '    alignCols = Split(pColor2Cols, ",")
            '    For i = LBound(alignCols) To UBound(alignCols)
            '        oGrid.Columns.Item(alignCols(i)).BackColor = 13624308
            '    Next i
            'End If

            BindGrid_array = True

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("BindGrid_array " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            'cols = Nothing
            'colNms = Nothing
            'affCols = Nothing
            'edCols = Nothing
            'viCols = Nothing
            'alignCols = Nothing

        End Try

    End Function


    Public Function SetGridTitle_Array(ByVal oGrid As SAPbouiCOM.Grid, ByVal pCols As ArrayList, ByVal pColNms As ArrayList, ByVal pEdCols As ArrayList, ByVal pViCols As ArrayList, ByVal pAlignCols As ArrayList, ByVal pColor1Cols As ArrayList, ByVal pcolor2Cols As ArrayList) As Boolean

        Dim cols() As String
        Dim colNms() As String
        Dim affCols() As String
        Dim edCols() As String
        Dim viCols() As String
        Dim alignCols() As String
        Dim xSql As String = ""
        Dim i As Integer = 0

        Try

            SetGridTitle_Array = False

            If pCols.Count <> pColNms.Count Then
                Exit Function
            End If

            For i = 0 To pCols.Count - 1
                xSql = xSql & IIf(xSql = "", "", ",") & "'' as """ & pCols(i).ToString.Trim & """ "
            Next

            xSql = IIf(xSql = "", "", "Select ") & xSql

            If xSql <> "" Then
#If HANA = "Y" Then
                xSql = xSql & " FROM DUMMY;"
#End If
                Call oGrid.DataTable.ExecuteQuery(xSql)
                Call oGrid.DataTable.Rows.Remove(0)
            End If

            For i = 0 To pCols.Count - 1
                oGrid.Columns.Item(pCols.Item(i).ToString.Trim).TitleObject.Caption = CFL.GetCaption(pColNms.Item(i).ToString.Trim, ModuleIni.CO)
            Next i

            For i = 0 To pEdCols.Count - 1
                oGrid.Columns.Item(pEdCols.Item(i)).Editable = False
            Next i

            For i = 0 To pViCols.Count - 1
                oGrid.Columns.Item(pViCols.Item(i)).Visible = False
            Next i

            For i = 0 To pAlignCols.Count - 1
                oGrid.Columns.Item(pAlignCols.Item(i)).RightJustified = True
            Next i

            For i = 0 To pColor1Cols.Count - 1
                oGrid.Columns.Item(pColor1Cols.Item(i)).BackColor = 12777465
            Next i


            For i = 0 To pcolor2Cols.Count - 1
                oGrid.Columns.Item(pcolor2Cols.Item(i)).BackColor = 13624308
            Next i

            oGrid.AutoResizeColumns()

            SetGridTitle_Array = True

        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("SetGridTitle_Array " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            cols = Nothing
            colNms = Nothing
            affCols = Nothing
            edCols = Nothing
            viCols = Nothing
            alignCols = Nothing

        End Try

    End Function

    ''' <summary>
    ''' 화면UI에서 달표현된Text를 읽어 YYYYMM으로 Return DB에 저장할때를 대비해서 사용 함.
    ''' </summary>
    ''' <param name="sdt">SBO포맷 형태의 년, 월 String</param>
    ''' <returns>YYYYMM 으로 String변환</returns>
    ''' <remarks></remarks>
    Public Function SBOMonthConvert(sdt As String) As String

        '0	DD/MM/YY																																														
        '1	DD/MM/CCYY																																														
        '2	MM/DD/YY																																														
        '3	MM/DD/CCYY																																														
        '4	CCYY/MM/DD																																														
        '5	YYYY/MM/DD																																														
        '6	YY/MM/DD																																														

        Dim strDateFormat As String = CFL.GetValue("SELECT TOP 1 DateFormat from OADM")
        Dim strDateSep As String = CFL.GetValue("SELECT TOP 1 DateSep from OADM")

        Dim MM As String = String.Empty
        Dim FrontYY As String = Left(Now.ToString("yyyyMMdd"), 2)
        Dim YY As String = String.Empty
        Dim sReturnYYYYMM As String = sdt.Replace(strDateSep, String.Empty)


        Select Case strDateFormat

            Case 0
                sReturnYYYYMM = FrontYY & Right(sdt, 2) & Left(sdt, 2)
                Return sReturnYYYYMM 'MM & strDateSep & Right(YYYY, 2)																																															
            Case 1 'DD/MM/CCYY																																															
                sReturnYYYYMM = Right(sdt, 4) & Left(sdt, 2)
                Return sReturnYYYYMM 'MM & strDateSep & YYYY																																															
            Case 2
                sReturnYYYYMM = FrontYY & Right(sdt, 2) & Left(sdt, 2)
                Return sReturnYYYYMM 'MM & strDateSep & Right(YYYY, 2)																																															
            Case 3
                sReturnYYYYMM = Right(sdt, 4) & Left(sdt, 2)
                Return sReturnYYYYMM 'MM & strDateSep & YYYY																																															
            Case 4
                Return sReturnYYYYMM 'YYYY & strDateSep & MM																																															
            Case 5
                Return sReturnYYYYMM 'YYYY & strDateSep & MM																																															
            Case 6
                sReturnYYYYMM = FrontYY & Left(sdt, 2) & Right(sdt, 2)
                Return sReturnYYYYMM 'Right(YYYY, 2) & strDateSep & MM																																															
        End Select


    End Function

    ''' <summary>
    ''' 화면UI의 string을 받아 SBO설정과 동일한지 체크 한다.
    ''' </summary>
    ''' <param name="strformat"> Empty를 입력하면 SBO의 포맷으로 변경해서 리턴한다.</param>
    ''' <param name="sdt">체크할 텍스트(년달)</param>
    ''' <returns>입력일자가 SBO포맷과 동일하면 True</returns>
    ''' <remarks></remarks>
    Public Function ValiSBOMonthConvert(ByRef strformat As String, ByRef sdt As String) As Boolean

        '0	DD/MM/YY																																														
        '1	DD/MM/CCYY																																														
        '2	MM/DD/YY																																														
        '3	MM/DD/CCYY																																														
        '4	CCYY/MM/DD																																														
        '5	YYYY/MM/DD																																														
        '6	YY/MM/DD																																														

        '0	strformat = "MM/YY"																																														
        '1	strformat = "MM/CCYY"																																														
        '2	strformat = "MM/YY"																																														
        '3	strformat = "MM/CCYY"																																														
        '4	strformat = "CCYY/MM"																																														
        '5	strformat = "YYYY/MM"																																														
        '6	strformat = "YY/MM"																																														

        Dim shorValue As Boolean = False
        If sdt = "1" Or sdt = "2" Or sdt = "3" Or sdt = "4" Or sdt = "5" Or sdt = "6" Or sdt = "7" Or sdt = "8" Or sdt = "9" Or sdt = "10" Or sdt = "11" Or sdt = "12" Then
            shorValue = True
        End If



        Dim strDateFormat As String = CFL.GetValue("SELECT TOP 1 DateFormat from OADM")
        Dim strDateSep As String = CFL.GetValue("SELECT TOP 1 DateSep from OADM")

        If Not DateSepAdd() Then
            strDateSep = ""
        End If

        Dim sNowDate As String = YYYYMMConvertSBODateformat(Now.ToString("yyyyMM"))

        If sdt = String.Empty Then
            sdt = sNowDate
            Return True
        End If

        Dim MM As String = String.Empty
        Dim YY As String = String.Empty
        Dim DateSep As String = String.Empty


        Dim FrontYY As String = Left(Now.ToString("yyyyMMdd"), 2)
        Dim sReturnYYYYMM As String = String.Empty
        If Not strDateSep = "" Then

            sdt.Replace(strDateSep, String.Empty)
        End If



        Select Case strDateFormat

            Case 0

                If shorValue Then
                    YY = Right(Now().Year, 2)
                    MM = Right("00" & sdt, 2)
                Else
                    YY = Right(sdt, 2)
                    MM = Left(sdt, 2)
                End If


                If sdt.IndexOf(strDateSep) < 0 Or strDateSep = "" Then
                    sdt = MM & strDateSep & YY
                    DateSep = strDateSep
                Else
                    DateSep = sdt.Substring(2, 1)
                End If


                strformat = "MM/YY".Replace("/", strDateSep)

            Case 1 'DD/MM/CCYY	
                If shorValue Then
                    YY = Now().Year
                    MM = Right("00" & sdt, 2)
                Else
                    YY = Right(sdt, 4)
                    MM = Left(sdt, 2)
                End If

                If sdt.IndexOf(strDateSep) < 0 Or strDateSep = "" Then
                    sdt = MM & strDateSep & YY
                    DateSep = strDateSep
                Else
                    DateSep = sdt.Substring(2, 1)
                End If



                strformat = "MM/CCYY".Replace("/", strDateSep)


            Case 2

                If shorValue Then
                    YY = Now().Year
                    MM = Right("00" & sdt, 2)
                Else
                    YY = FrontYY & Right(sdt, 2)
                    MM = Left(sdt, 2)
                End If




                If sdt.IndexOf(strDateSep) < 0 Or strDateSep = "" Then
                    sdt = MM & strDateSep & YY
                    DateSep = strDateSep
                Else
                    DateSep = sdt.Substring(2, 1)
                End If


                strformat = "MM/YY".Replace("/", strDateSep)

            Case 3
                If shorValue Then
                    YY = Now().Year
                    MM = Right("00" & sdt, 2)
                Else
                    YY = Right(sdt, 4)
                    MM = Left(sdt, 2)
                End If



                If sdt.IndexOf(strDateSep) < 0 Or strDateSep = "" Then
                    sdt = MM & strDateSep & YY
                    DateSep = strDateSep
                Else
                    DateSep = sdt.Substring(2, 1)
                End If

                strformat = "MM/CCYY".Replace("/", strDateSep)

            Case 4

                If shorValue Then
                    YY = Now().Year
                    MM = Right("00" & sdt, 2)
                Else
                    YY = Left(sdt, 4)
                    MM = Right(sdt, 2)
                End If



                If sdt.IndexOf(strDateSep) < 0 Or strDateSep = "" Then
                    sdt = YY & strDateSep & MM
                    DateSep = strDateSep
                Else
                    DateSep = sdt.Substring(4, 1)
                End If
                strformat = "CCYY/MM".Replace("/", strDateSep)

            Case 5
                If shorValue Then
                    YY = Now().Year
                    MM = Right("00" & sdt, 2)
                Else
                    YY = Left(sdt, 4)
                    MM = Right(sdt, 2)
                End If



                If sdt.IndexOf(strDateSep) < 0 Or strDateSep = "" Then
                    sdt = YY & strDateSep & MM
                    DateSep = strDateSep
                Else
                    DateSep = sdt.Substring(4, 1)
                End If
                strformat = "YYYY/MM".Replace("/", strDateSep)

            Case 6
                If shorValue Then
                    YY = Now().Year
                    MM = Right("00" & sdt, 2)
                Else
                    YY = FrontYY & Right(sdt, 2)
                    MM = Left(sdt, 2)
                End If


                If sdt.IndexOf(strDateSep) < 0 Or strDateSep = "" Then
                    sdt = YY & strDateSep & MM
                    DateSep = strDateSep
                Else
                    DateSep = sdt.Substring(2, 1)
                End If
                strformat = "YY/MM".Replace("/", strDateSep)
        End Select

        If DateSep = "" Then
            If Not IsNumeric(YY) Or Not IsNumeric(MM) Then
                sdt = sNowDate
                Return True
            End If
        Else
            If Not IsNumeric(YY.Replace(DateSep, "A")) Or Not IsNumeric(MM.Replace(DateSep, "A")) Then
                sdt = sNowDate
                Return True
            End If
        End If


        If YY.Length = 4 AndAlso (YY < 1000 Or YY > 2999) Then
            sdt = sNowDate
            Return True
        End If

        If YY.Length = 2 AndAlso (YY > 99) Then
            sdt = sNowDate
            Return True
        End If


        If strDateSep <> "" Then
            If strDateSep <> DateSep Then
                sdt = sNowDate
                Return True
            End If
        End If


        If Not (CInt(MM) > 0 And CInt(MM) < 13) Then
            sdt = sNowDate
            Return True
        End If
        Return True

    End Function


    ''' <summary>
    ''' 날짜 구분자 추가여부.
    ''' </summary>
    ''' <returns>구분자추가일 경우 True</returns>
    ''' <remarks></remarks>
    Public Function DateSepAdd() As Boolean

        Dim strQ As String = "SELECT ISNULL(MAX(U_USEYN), 'N') AS VAL FROM [@WJS_SAD011] WHERE Code = 'AD97'"
        Dim strYN As String = CFL.GetValue(strQ)

        If strYN = "Y" Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function YYYYMMConvertSBODateformat(YYYYMM As String) As String


        '0	DD/MM/YY																																														
        '1	DD/MM/CCYY																																														
        '2	MM/DD/YY																																														
        '3	MM/DD/CCYY																																														
        '4	CCYY/MM/DD																																														
        '5	YYYY/MM/DD																																														
        '6	YY/MM/DD																																														

        '0	strformat = "MM/YY"																																														
        '1	strformat = "MM/CCYY"																																														
        '2	strformat = "MM/YY"																																														
        '3	strformat = "MM/CCYY"																																														
        '4	strformat = "CCYY/MM"																																														
        '5	strformat = "YYYY/MM"																																														
        '6	strformat = "YY/MM"																																														


        Dim strDateFormat As String = CFL.GetValue("SELECT TOP 1 DateFormat from OADM")
        Dim strDateSep As String = CFL.GetValue("SELECT TOP 1 DateSep from OADM")


        Dim FrontYY As String = Left(YYYYMM, 2)
        Dim MM As String = Right(YYYYMM, 2)
        Dim YY As String = YYYYMM.Substring(2, 2)
        Dim DateSep As String = strDateSep
        Dim strResult As String = String.Empty


        Select Case strDateFormat

            Case 0
                strResult = MM & DateSep & YY

            Case 1 'DD/MM/CCYY																																															
                strResult = MM & DateSep & FrontYY & YY

            Case 2 '"MM/YY"		
                strResult = MM & DateSep & YY
            Case 3 'DD/MM/CCYY																																															
                strResult = MM & DateSep & FrontYY & YY

            Case 4 '"CCYY/MM"
                strResult = FrontYY & YY & DateSep & MM

            Case 5
                strResult = FrontYY & YY & DateSep & MM

            Case 6
                strResult = YY & DateSep & MM
        End Select

        If DateSepAdd() Then
            Return strResult
        Else
            Return strResult.Replace(DateSep, "")
        End If


    End Function
End Module
