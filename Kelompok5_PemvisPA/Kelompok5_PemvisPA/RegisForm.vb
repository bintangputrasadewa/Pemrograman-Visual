Imports MySql.Data.MySqlClient

Public Class RegisForm

    Private Sub btnRegis_Click(sender As Object, e As EventArgs) Handles btnRegis.Click
        ' Validasi input form
        If txtNama.Text = "" OrElse txtUser.Text = "" OrElse txtPw.Text = "" OrElse txtKonfirm.Text = "" Then
            MessageBox.Show("Harap lengkapi semua kolom.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Validasi konfirmasi password
        If txtPw.Text <> txtKonfirm.Text Then
            MessageBox.Show("Password dan konfirmasi password tidak cocok.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Validasi username sudah ada atau belum
        If IsUsernameExists(txtUser.Text) Then
            MessageBox.Show("Username sudah digunakan. Harap pilih username lain.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Tambahkan user baru ke database
        AddUser(txtNama.Text, txtUser.Text, txtPw.Text)
        MessageBox.Show("Registrasi berhasil.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ' Tutup form registrasi dan buka form login
        Me.Close()
        Dim loginForm As New LoginForm()
        loginForm.Show()
    End Sub

    Private Function IsUsernameExists(username As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM akun WHERE username = @username"

        Module1.koneksi()
        Using command As New MySqlCommand(query, Module1.CONN)
            command.Parameters.AddWithValue("@username", username)
            Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
            Return count > 0
        End Using
    End Function

    Private Sub AddUser(nama As String, username As String, password As String)
        Dim query As String = "INSERT INTO akun (nama, username, password) VALUES (@nama, @username, @password)"

        Module1.koneksi()
        Using command As New MySqlCommand(query, Module1.CONN)
            command.Parameters.AddWithValue("@nama", nama)
            command.Parameters.AddWithValue("@username", username)
            command.Parameters.AddWithValue("@password", password)
            command.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub adaAkun_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles adaAkun.LinkClicked
        LoginForm.Show()
        Me.Hide()
    End Sub
End Class
