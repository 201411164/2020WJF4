Option Strict Off
Option Explicit On
Imports AddOnBase

Public Class WJS_DEV_Init

    Implements IDisposable

    Dim xSQL As String

    Public Sub New()
        Me.Init()
    End Sub

    Public Sub Init()


        Dim RS As SAPbobsCOM.Recordset
        RS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Try

            CFL.LoadFromXMLMenuCst()

            'Call fnSMB_MenuSetting()

        Catch ex As Exception

            B1Connections.theAppl.MessageBox("커스터마이징모듈 생성중 오류가 발생하였습니다. 관리자에게 문의 하십시오")

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

    Public Function fnSMB_MenuSetting() As Boolean

        Dim oRS As SAPbobsCOM.Recordset
        oRS = B1Connections.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Try



        Catch ex As Exception

            B1Connections.theAppl.StatusBar.SetText("ET_BFITEM_PRESSED " & Err.Description, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)

        Finally

            oRS = Nothing

        End Try

    End Function
#Region " IDisposable Support "
    ' 삭제 가능한 패턴을 올바르게 구현하기 위해 Visual Basic에서 추가한 코드입니다.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 이 코드는 변경하지 마십시오. 위의 Dispose(ByVal disposing As Boolean)에 정리 코드를 입력하십시오.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

