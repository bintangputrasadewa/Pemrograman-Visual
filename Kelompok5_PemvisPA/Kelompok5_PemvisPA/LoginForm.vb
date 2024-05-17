Imports MySql.Data.MySqlClient

Public Class LoginForm
    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        If String.IsNullOrEmpty(txtUsername.Text) Or String.IsNullOrEmpty(txtPassword.Text) Then
            MessageBox.Show("Username dan Password harus diisi")
            Return
        End If

        If IsUserValid(txtUsername.Text, txtPassword.Text) Then
            If txtUsername.Text = "admin" AndAlso txtPassword.Text = "123" Then
                Dim Admin As New AdminForm
                Admin.Show()
                Me.Close()
            Else
                Dim Pelanggan As New PelangganForm
                Pelanggan.Show()
                Me.Close()
            End If
        Else
            MessageBox.Show("Username atau Password salah")
        End If
    End Sub
    ' kalo blm ada akun
    Private Sub linkAkun_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkAkun.LinkClicked
        Dim Regis As New RegisForm
        Regis.Show()
        Me.Hide()
    End Sub

    Private Function IsUserValid(username As String, password As String) As Boolean
        Dim isValid As Boolean = False
        Try
            koneksi()

            Dim query As String = "SELECT COUNT(*) FROM akun WHERE username = @username AND password = @password"
            CMD = New MySqlCommand(query, CONN)
            CMD.Parameters.AddWithValue("@username", username)
            CMD.Parameters.AddWithValue("@password", password)

            Dim count As Integer = Convert.ToInt32(CMD.ExecuteScalar())

            ' Jika jumlah baris yang sesuai lebih dari 0, username dan password valid
            If count > 0 Then
                isValid = True
            End If

            ' Jika login berhasil dan username adalah "Admin", buka AdminForm
            If isValid AndAlso username = "Admin" Then
                Return True
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            CONN.Close()
        End Try
        Return isValid
    End Function

End Class