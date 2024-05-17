Imports MySql.Data.MySqlClient

Public Class PelangganForm
    Public Property MerkPembelian As String
    Public Property JamTanganPembelian As String
    Public Property JumlahPembelian As Integer

    Private Sub PelangganForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        koneksi() ' Memanggil fungsi koneksi dari Module1
        FillComboBox(cbMerk, "SELECT DISTINCT merk FROM jt")
        AddHandler cbMerk.SelectedIndexChanged, AddressOf cbMerk_SelectedIndexChanged
        AddHandler cbJamTangan.SelectedIndexChanged, AddressOf cbJamTangan_SelectedIndexChanged
    End Sub

    Private Sub FillComboBox(cmb As ComboBox, query As String)
        Try
            CMD = New MySqlCommand(query, CONN)
            RD = CMD.ExecuteReader()
            cmb.Items.Clear()
            While RD.Read()
                cmb.Items.Add(RD.GetString(0))
            End While
            RD.Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub cbMerk_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cbMerk.SelectedItem IsNot Nothing Then
            FillComboBox(cbJamTangan, "SELECT nama FROM jt WHERE merk = '" & cbMerk.SelectedItem.ToString() & "'")
        End If
    End Sub

    Private Sub cbJamTangan_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cbJamTangan.SelectedItem IsNot Nothing Then
            FillComboBox(cbJenis, "SELECT jenis FROM jt WHERE merk = '" & cbMerk.SelectedItem.ToString() & "' AND nama = '" & cbJamTangan.SelectedItem.ToString() & "'")
            FillComboBox(cbUkuran, "SELECT ukuran FROM jt WHERE merk = '" & cbMerk.SelectedItem.ToString() & "' AND nama = '" & cbJamTangan.SelectedItem.ToString() & "'")
        End If
    End Sub
    Private Sub btnBeli_Click(sender As Object, e As EventArgs) Handles btnBeli.Click
        ' Ambil informasi barang yang dibeli dari ComboBox yang dipilih oleh pengguna
        Dim merk As String = cbMerk.SelectedItem.ToString()
        Dim jamTangan As String = cbJamTangan.SelectedItem.ToString()
        Dim jenis As String = cbJenis.SelectedItem.ToString()
        Dim ukuran As String = cbUkuran.SelectedItem.ToString()

        MerkPembelian = merk
        JamTanganPembelian = jamTangan

        ' Verifikasi apakah barang yang dipilih tersedia dalam stok
        Dim jumlahTersedia As Integer = GetJumlahBarangTersedia(merk, jamTangan)
        If jumlahTersedia <= 0 Then
            MessageBox.Show("Barang tidak tersedia dalam stok.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Lakukan proses pembelian dengan memasukkan jumlah barang yang ingin dibeli
        Dim jumlahBeli As Integer
        If Not Integer.TryParse(txtJumlah.Text, jumlahBeli) Then
            MessageBox.Show("Jumlah barang yang dimasukkan tidak valid.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Validasi apakah jumlah yang diminta pengguna tersedia di stok
        If jumlahBeli > jumlahTersedia Then
            MessageBox.Show("Jumlah barang yang ingin Anda beli melebihi stok yang tersedia.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Update jumlah barang yang tersedia dalam stok
        KurangiJumlahBarang(merk, jamTangan, jumlahBeli)

        ' Tampilkan form StrukForm kepada pengguna untuk melihat struk pembelian
        Dim strukForm As New StrukForm(merk, jamTangan, jumlahBeli)
        strukForm.Show()
    End Sub

    Private Sub KurangiJumlahBarang(merk As String, nama As String, jumlah As Integer)
        Try
            STR = "UPDATE jt SET jumlah = jumlah - @jumlah WHERE merk = @merk AND nama = @nama"
            CMD = New MySqlCommand(STR, CONN)
            CMD.Parameters.AddWithValue("@jumlah", jumlah)
            CMD.Parameters.AddWithValue("@merk", merk)
            CMD.Parameters.AddWithValue("@nama", nama)
            CMD.ExecuteNonQuery()
            MessageBox.Show("Jumlah barang berhasil diperbarui di database.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub
    Private Function GetJumlahBarangTersedia(merk As String, nama As String) As Integer
        Dim jumlah As Integer = 0
        Try
            STR = "SELECT jumlah FROM jt WHERE merk = @merk AND nama = @nama"
            CMD = New MySqlCommand(STR, CONN)
            CMD.Parameters.AddWithValue("@merk", merk)
            CMD.Parameters.AddWithValue("@nama", nama)
            RD = CMD.ExecuteReader()
            If RD.Read() Then
                jumlah = Convert.ToInt32(RD("jumlah"))
            End If
            RD.Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
        Return jumlah
    End Function


    Private Sub btnKeluar_Click(sender As Object, e As EventArgs) Handles btnKeluar.Click
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Me.Close()
            FMenu.Show()
        End If
    End Sub

End Class
