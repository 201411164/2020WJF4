Option Strict Off
Option Explicit On
Imports AddOnBase

Public Class WJS_PLS_Init
    Implements IDisposable

    Dim xSQL As String

    Public Sub New()
        Me.Init()
    End Sub

    Public Sub Init()
        Try

            CFL.LoadFromXMLMenuCst()

            '사업 계획
            If (B1Connections.theAppl.Menus.Exists("WJS_SO01_PLS")) Then
                If (Not System.IO.File.Exists(B1Connections.theAppl.Menus.Item("WJS_SO01_PLS").Image())) Then
                    If System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\IMG\SO.jpg") Then
                        B1Connections.theAppl.Menus.Item("WJS_SO01_PLS").Image = System.Windows.Forms.Application.StartupPath + "\IMG\SO.jpg"
                    End If
                End If
            End If

            '품질 관리
            If (B1Connections.theAppl.Menus.Exists("WJS_QM00_PLS")) Then
                If (Not System.IO.File.Exists(B1Connections.theAppl.Menus.Item("WJS_QM00_PLS").Image())) Then
                    If System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\IMG\QM.JPG") Then
                        B1Connections.theAppl.Menus.Item("WJS_QM00_PLS").Image = System.Windows.Forms.Application.StartupPath + "\IMG\QM.JPG"
                    End If
                End If
            End If

            ''금형 관리
            If (B1Connections.theAppl.Menus.Exists("WJS_DM00_PLS")) Then
                If (Not System.IO.File.Exists(B1Connections.theAppl.Menus.Item("WJS_DM00_PLS").Image())) Then
                    If System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\IMG\DM.JPG") Then
                        B1Connections.theAppl.Menus.Item("WJS_DM00_PLS").Image = System.Windows.Forms.Application.StartupPath + "\IMG\DM.JPG"
                    End If
                End If
            End If

            Dim ADSUB As PLS_ADDEVT = New PLS_ADDEVT()

        Catch ex As Exception

            B1Connections.theAppl.MessageBox("커스터마이징모듈 생성중 오류가 발생하였습니다. 관리자에게 문의 하십시오" + ex.Message)

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
        'GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
