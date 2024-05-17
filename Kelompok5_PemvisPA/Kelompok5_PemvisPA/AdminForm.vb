Imports MySql.Data.MySqlClient
Imports System.IO

Public Class AdminForm

    Private Sub btnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        Dim jenis As String = ""
        If rbAnalog.Checked Then
            jenis = "Analog"
        ElseIf rbDigital.Checked Then
            jenis = "Digital"
        ElseIf rbAuto.Checked Then
            jenis = "Auto"
        End If

        ' Periksa apakah pengguna telah memilih file gambar
        If BukaFile.FileName = "" Then
            MessageBox.Show("Pilih gambar terlebih dahulu.")
            Return
        End If

        ' Periksa ekstensi file gambar
        Dim fileExtension As String = Path.GetExtension(BukaFile.FileName)
        If fileExtension.ToLower() <> ".jpg" AndAlso fileExtension.ToLower() <> ".png" Then
            MessageBox.Show("Hanya file gambar dengan format JPG atau PNG yang diperbolehkan.")
            Return
        End If

        Dim fileName As String = Path.GetFileName(BukaFile.FileName) ' Ambil nama file gambar
        Dim folderPath As String = "D:\FOTO JAM TANGAN\" ' Folder tujuan

        ' Periksa apakah folder sudah ada, jika tidak, buat folder baru
        If Not Directory.Exists(folderPath) Then
            Try
                Directory.CreateDirectory(folderPath)
            Catch ex As Exception
                MessageBox.Show("Error creating folder: " & ex.Message)
                Return
            End Try
        End If

        Dim filePath As String = Path.Combine(folderPath, fileName) ' Path lengkap file

        ' Simpan gambar ke folder 
        File.Copy(BukaFile.FileName, filePath, True)

        ' Baca konten gambar sebagai byte array
        Dim imageBytes As Byte() = File.ReadAllBytes(filePath)

        Dim query As String = "INSERT INTO jt (Nama, Merk, Harga, Ukuran, Jumlah, Jenis, Foto) VALUES (@nama, @merk, @harga, @ukuran, @jumlah, @jenis, @foto)"

        Try
            If CONN.State = ConnectionState.Closed Then
                CONN.Open()
            End If

            Dim cmd As New MySqlCommand(query, CONN)
            cmd.Parameters.AddWithValue("@nama", txtNama.Text)
            cmd.Parameters.AddWithValue("@merk", txtMerk.Text)
            cmd.Parameters.AddWithValue("@harga", Convert.ToDouble(txtHarga.Text))
            cmd.Parameters.AddWithValue("@ukuran", txtUkuran.Text)
            cmd.Parameters.AddWithValue("@jumlah", Convert.ToInt32(txtJumlah.Text))
            cmd.Parameters.AddWithValue("@jenis", jenis)
            cmd.Parameters.AddWithValue("@foto", imageBytes) ' Simpan gambar sebagai byte array
            cmd.ExecuteNonQuery()
            MessageBox.Show("Data berhasil disimpan")
            tampilJt()

            ' Membersihkan inputan setelah berhasil menyimpan
            Kosong()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If CONN.State = ConnectionState.Open Then
                CONN.Close()
            End If
        End Try
    End Sub

    Private Sub btnUbah_Click(sender As Object, e As EventArgs) Handles btnUbah.Click
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim jenis As String = ""
            If rbAnalog.Checked Then
                jenis = "Analog"
            ElseIf rbDigital.Checked Then
                jenis = "Digital"
            ElseIf rbAuto.Checked Then
                jenis = "Auto"
            End If

            Dim query As String = "UPDATE jt SET Nama=@nama, Merk=@merk, Harga=@harga, Ukuran=@ukuran, Jumlah=@jumlah, Jenis=@jenis, Foto=@foto WHERE ID=@id"
            Dim cmd As New MySqlCommand(query, CONN)
            cmd.Parameters.AddWithValue("@nama", txtNama.Text)
            cmd.Parameters.AddWithValue("@merk", txtMerk.Text)
            cmd.Parameters.AddWithValue("@harga", Convert.ToDouble(txtHarga.Text))
            cmd.Parameters.AddWithValue("@ukuran", txtUkuran.Text)
            cmd.Parameters.AddWithValue("@jumlah", Convert.ToInt32(txtJumlah.Text))
            cmd.Parameters.AddWithValue("@jenis", jenis)
            cmd.Parameters.AddWithValue("@id", DataGridView1.SelectedRows(0).Cells("ID").Value)

            ' Periksa apakah pengguna telah memilih file gambar baru
            If Not String.IsNullOrEmpty(BukaFile.FileName) AndAlso File.Exists(BukaFile.FileName) Then
                Dim fileName As String = Path.GetFileName(BukaFile.FileName)
                Dim folderPath As String = "D:\FOTO JAM TANGAN\"
                Dim filePath As String = Path.Combine(folderPath, fileName)
                Dim imageBytes As Byte() = File.ReadAllBytes(filePath)
                cmd.Parameters.AddWithValue("@foto", imageBytes)
            Else
                ' Jika tidak, gunakan gambar yang sudah ada dalam database
                Dim imageBytes As Byte() = DirectCast(DataGridView1.SelectedRows(0).Cells("Foto").Value, Byte())
                cmd.Parameters.AddWithValue("@foto", imageBytes)
            End If

            Try
                CONN.Open()
                cmd.ExecuteNonQuery()
                MessageBox.Show("Data berhasil diubah")
                tampilJt()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                CONN.Close()
            End Try
        Else
            MessageBox.Show("Pilih data terlebih dahulu dari DataGridView")
        End If
    End Sub

    Private Sub btnHapus_Click(sender As Object, e As EventArgs) Handles btnHapus.Click
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                Dim id As Integer = Convert.ToInt32(DataGridView1.SelectedRows(0).Cells("ID").Value)
                Dim query As String = "DELETE FROM jt WHERE ID=@id"
                Dim cmd As New MySqlCommand(query, CONN)
                cmd.Parameters.AddWithValue("@id", id)

                Try
                    CONN.Open()
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Data berhasil dihapus")
                    tampilJt()
                Catch ex As Exception
                    MessageBox.Show("Error: " & ex.Message)
                Finally
                    CONN.Close()
                End Try
            End If
        Else
            MessageBox.Show("Pilih data terlebih dahulu dari DataGridView")
        End If
    End Sub


    Private Sub AdminForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        koneksi()
        RefreshID()
        tampilJt()
        Kosong()
        aturGrid()
    End Sub

    Sub RefreshID() ' ini untuk reset id pada data kembali ke 1 setiap kali data tidak ada
        Try
            ' Ambil nilai maksimum ID saat ini
            Dim queryMaxID As String = "SELECT MAX(ID) FROM jt"
            Dim cmdMaxID As New MySqlCommand(queryMaxID, CONN)
            Dim maxID As Object = cmdMaxID.ExecuteScalar()

            ' Periksa apakah nilai maxID adalah DBNull
            If maxID IsNot DBNull.Value Then
                'Atur ulang AUTO_INCREMENT ke nilai maksimum ID + 1
                Dim maxIDValue As Integer = Convert.ToInt32(maxID)
                Dim queryResetID As String = "ALTER TABLE jt AUTO_INCREMENT = @maxIDPlusOne"
                Dim cmdResetID As New MySqlCommand(queryResetID, CONN)
                cmdResetID.Parameters.AddWithValue("@maxIDPlusOne", maxIDValue + 1)
                cmdResetID.ExecuteNonQuery()
            Else
                ' Jika maxID adalah DBNull, berarti tidak ada data dalam tabel
                ' Atur ulang AUTO_INCREMENT ke 1
                Dim queryResetID As String = "ALTER TABLE jt AUTO_INCREMENT = 1"
                Dim cmdResetID As New MySqlCommand(queryResetID, CONN)
                cmdResetID.ExecuteNonQuery()
            End If
        Catch ex As Exception
            MessageBox.Show("Error refreshing ID: " & ex.Message)
        End Try
    End Sub

    Private Sub DataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView1.SelectionChanged
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
            txtNama.Text = row.Cells("Nama").Value.ToString()
            txtMerk.Text = row.Cells("Merk").Value.ToString()
            txtHarga.Text = row.Cells("Harga").Value.ToString()
            txtUkuran.Text = row.Cells("Ukuran").Value.ToString()
            txtJumlah.Text = row.Cells("Jumlah").Value.ToString()

            Dim jenis As String = row.Cells("Jenis").Value.ToString()
            If jenis = "Analog" Then
                rbAnalog.Checked = True
            ElseIf jenis = "Digital" Then
                rbDigital.Checked = True
            ElseIf jenis = "Auto" Then
                rbAuto.Checked = True
            End If

            ' Memuat gambar ke PictureBox1
            If Not IsDBNull(row.Cells("Foto").Value) Then
                Dim imageBytes As Byte() = DirectCast(row.Cells("Foto").Value, Byte())
                Using ms As New MemoryStream(imageBytes)
                    PictureBox1.Image = Image.FromStream(ms)
                End Using
            End If
        End If
    End Sub

    Sub Kosong()
        txtNama.Clear()
        txtMerk.Clear()
        txtHarga.Clear()
        txtUkuran.Clear()
        txtJumlah.Clear()
        rbAnalog.Checked = False
        rbDigital.Checked = False
        rbAuto.Checked = False
        PictureBox1.Image = Nothing
        txtNama.Focus()
    End Sub

    Sub tampilJt()
        Dim query As String = "SELECT ID, Nama, Merk, Harga, Ukuran, Jumlah, Jenis, Foto FROM jt"
        Dim cmd As New MySqlCommand(query, CONN)
        Dim adapter As New MySqlDataAdapter(cmd)
        Dim table As New DataTable()
        adapter.Fill(table)

        ' Atur ulang kolom Foto ke tipe byte array
        table.Columns("Foto").DataType = GetType(Byte())

        ' Atur data source untuk DataGridView
        DataGridView1.DataSource = table
        DataGridView1.Refresh()
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        If e.ColumnIndex = DataGridView1.Columns("Foto").Index AndAlso e.RowIndex >= 0 Then
            If Not IsDBNull(DataGridView1.Rows(e.RowIndex).Cells("Foto").Value) Then
                Try
                    Dim imageBytes As Byte() = DirectCast(DataGridView1.Rows(e.RowIndex).Cells("Foto").Value, Byte())
                    Using ms As New MemoryStream(imageBytes)
                        Dim image As Image = Image.FromStream(ms)
                        e.Value = image
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Error displaying image: " & ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Sub aturGrid()
        ' Pengaturan lebar kolom secara manual
        DataGridView1.Columns("ID").Visible = True
        DataGridView1.Columns("Nama").Width = 150
        DataGridView1.Columns("Merk").Width = 150
        DataGridView1.Columns("Harga").Width = 100
        DataGridView1.Columns("Ukuran").Width = 100
        DataGridView1.Columns("Jumlah").Width = 100
        DataGridView1.Columns("Jenis").Width = 100

        ' Penataan teks di tengah
        DataGridView1.Columns("Nama").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns("Merk").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns("Harga").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns("Ukuran").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns("Jumlah").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns("Jenis").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        ' Atur judul kolom
        DataGridView1.Columns("Nama").HeaderText = "Nama"
        DataGridView1.Columns("Merk").HeaderText = "Merk"
        DataGridView1.Columns("Harga").HeaderText = "Harga"
        DataGridView1.Columns("Ukuran").HeaderText = "Ukuran"
        DataGridView1.Columns("Jumlah").HeaderText = "Jumlah"
        DataGridView1.Columns("Jenis").HeaderText = "Jenis"

        ' Atur tinggi baris secara langsung
        DataGridView1.RowTemplate.Height = DataGridView1.RowTemplate.Height + 80

        ' Set properti ImageLayout untuk kolom gambar
        For Each column As DataGridViewColumn In DataGridView1.Columns
            If column.Name = "Foto" Then
                CType(column, DataGridViewImageColumn).ImageLayout = DataGridViewImageCellLayout.Zoom
            End If
        Next
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        ' Set filter untuk menampilkan file JPEG (JPG) atau PNG dari folder laptop
        BukaFile.Filter = "Gambar JPG (*.jpg)|*.jpg|Gambar PNG (*.png)|*.png|Semua File Gambar (*.jpg, *.png)|*.jpg;*.png|Semua File (*.*)|*.*"
        BukaFile.ShowDialog()
        PictureBox1.ImageLocation = BukaFile.FileName
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        If txtSearch.Text <> Nothing Then
            Dim query As String = "select * from jt where Nama like '%" & txtSearch.Text & "%'"
            Dim cmd As New MySqlCommand(query, CONN)
            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()
            adapter.Fill(table)
            DataGridView1.DataSource = table
            DataGridView1.ReadOnly = True
        Else
            tampilJt()
        End If
    End Sub

    Private Sub btnKeluar_Click(sender As Object, e As EventArgs) Handles btnKeluar.Click
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Me.Close()
            FMenu.Show()
        End If
    End Sub

End Class
