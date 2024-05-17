Public Class FMenu
    ' Event handler untuk item menu "Login Admin"
    Private Sub MenuAdminToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MenuAdminToolStripMenuItem.Click
        Dim loginForm As New LoginForm()
        loginForm.Show()
        Me.Hide()
    End Sub

    ' Event handler untuk item menu "Registrasi Pelanggan"
    Private Sub MenuPelangganToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles MenuPelangganToolStripMenuItem1.Click
        Dim regisForm As New RegisForm()
        regisForm.Show()
        Me.Hide()
    End Sub

    ' Event handler untuk item menu "Login Pelanggan"
    Private Sub LoginPelangganToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoginPelangganToolStripMenuItem.Click
        Dim loginForm As New LoginForm()
        loginForm.Show()
        Me.Hide()
    End Sub

    ' Event handler untuk item menu "Keluar Program"
    Private Sub KeluarProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KeluarProgramToolStripMenuItem.Click
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin keluar program?", "Konfirmasi Keluar", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            End
        End If
    End Sub


    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class
