# .NET Programming

### 연결

```vb
 Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sConStr As String = "Initial Catalog = " & TextBox1.Text & ";Data Source = " & "77106152-PC\MSSQLSERVER_2017" & ";Integrated Security = " & TextBox3.Text & ";"
        'Dim sConStr As String = "Server=77106152-PC\MSSQLSERVER_2017;Database=SBODemoKR;User Id=sa;Password=root"    
    
            myConn = New SqlConnection(sConStr)
        If myConn.State = ConnectionState.Open Then
            MsgBox("이미 연결되어 있습니다")
        Else
            myConn.Open()
            If myConn.State = ConnectionState.Open Then
                MsgBox("연결 성공")
            Else
                MsgBox("연결실패")
            End If
        End If
    End Sub
```

### 해제 

```vb
Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click   
	If myConn.State = ConnectionState.Closed Then
        MsgBox("이미 해제되어 있습니다.")
    Else
        myConn.Close()
        If myConn.State = ConnectionState.Closed Then
            MsgBox("해제 성공")
        Else
            MsgBox("해제 실패")
        End If
    End If
```
### 조회

```vb
Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Using myCmd As New SqlCommand()
            With myCmd
                .Connection = myConn
                .CommandType = CommandType.Text
                .CommandText = "SELECT cast(number as  nvarchar(20)), name, cast(createtime as nvarchar(20)) FROM myTable"
            End With
            Try
                myReader = myCmd.ExecuteReader()
                Do While myReader.Read()
                    results = results & myReader.GetString(0) & vbTab & myReader.GetString(1) & vbTab & myReader.GetString(2) & vbLf
                Loop
                MsgBox(results)
                results = ""
            Catch ex As Exception
                MessageBox.Show(ex.Message.ToString(), "Error Message")
            Finally
                results = ""
                myReader.Close()
                myReader = Nothing
            End Try
        End Using
    End Sub
```

### 입력

```vb
 Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim a As Integer
        Dim count1 As Integer
        Using myCmd As New SqlCommand()
            With myCmd
                .Connection = myConn
                .CommandType = CommandType.Text
                .CommandText = "insert into mytable (number, name, createtime) values (@param1, @param2, @param3)"
                .Parameters.Add("@param1", SqlDbType.Int).Value = count1
                .Parameters.Add("@param2", SqlDbType.NChar, 20).Value = "kb"
                .Parameters.Add("@param3", SqlDbType.Date).Value = "2020-09-28"
                Try
                    a = .ExecuteNonQuery
                Catch ex As Exception
                    MessageBox.Show(ex.Message.ToString(), "Error Message")
                Finally
                    If count1 <> Nothing Then
                        count1 = count1 + 1
                    Else
                        count1 = 1
                    End If
                End Try

                If a <> -1 Then
                    MessageBox.Show("입력완료", "Info msg")
                Else
                    MessageBox.Show("입력실패", "error msg")
                End If
            End With
        End Using
    End Sub
```

### 수정

```vb
 Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim returnVAl As Integer
        Using myCmd As New SqlCommand()
            With myCmd
                .Connection = myConn
                .CommandType = CommandType.Text
                .CommandText = "Update myTable set name = @param1 from mytable"
                .Parameters.Add("@param1", SqlDbType.Int).Value = "2"
                Try
                    returnVAl = .ExecuteNonQuery
                Catch ex As Exception
                    MessageBox.Show(ex.Message.ToString, "Error Msg")
                End Try
                If returnVAl <> -1 Then
                    MessageBox.Show("Update Completed", "Info msg")
                Else
                    MessageBox.Show("UPdate Failed", "Error MSG")
                End If
            End With
        End Using
    End Sub
```

### 삭제

```vb
Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim returnVal As Integer
        Dim inputVal As String
        If TextBox4.Text <> "" Then
            inputVal = TextBox4.Text
        Else
            inputVal = "NULL"
        End If
        Using myCmd As New SqlCommand()
            With myCmd
                .Connection = myConn
                .CommandType = CommandType.Text
                .CommandText = "delete from myTable where name = @param1"
                .Parameters.Add("@param1", SqlDbType.NChar, 20).Value = inputVal
                Try
                    returnVal = .ExecuteNonQuery
                Catch ex As Exception
                    MessageBox.Show(ex.Message.ToString.ToString, "error Msg")
                Finally
                    TextBox4.Text = ""

                End Try
                If returnVal <> -1 Then
                    MessageBox.Show("Delete Completed", "INFO")
                Else
                    MessageBox.Show("Delete failed", "Error")
                End If

            End With
        End Using
    End Sub
```