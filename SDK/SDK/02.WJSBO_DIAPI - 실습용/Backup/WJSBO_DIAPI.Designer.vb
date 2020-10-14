<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WJSBO_DIAPI
    Inherits System.Windows.Forms.Form

    'Form은 Dispose를 재정의하여 구성 요소 목록을 정리합니다.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows Form 디자이너에 필요합니다.
    Private components As System.ComponentModel.IContainer

    '참고: 다음 프로시저는 Windows Form 디자이너에 필요합니다.
    '수정하려면 Windows Form 디자이너를 사용하십시오.  
    '코드 편집기를 사용하여 수정하지 마십시오.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Button = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Button
        '
        Me.Button.Font = New System.Drawing.Font("굴림", 48.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(129, Byte))
        Me.Button.Location = New System.Drawing.Point(29, 27)
        Me.Button.Name = "Button"
        Me.Button.Size = New System.Drawing.Size(232, 216)
        Me.Button.TabIndex = 0
        Me.Button.Text = "Batch 실행"
        Me.Button.UseVisualStyleBackColor = True
        '
        'WJSBO_BATCH
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 268)
        Me.Controls.Add(Me.Button)
        Me.Name = "WJSBO_BATCH"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "WJSBOBatchServiceExe"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Button As System.Windows.Forms.Button

    Public Sub New()

        ' 이 호출은 Windows Form 디자이너에 필요합니다.
        InitializeComponent()

        ' InitializeComponent() 호출 뒤에 초기화 코드를 추가하십시오.

    End Sub
End Class
