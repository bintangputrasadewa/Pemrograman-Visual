Imports MySql.Data.MySqlClient
Imports System.Drawing.Printing

Public Class StrukForm
    Private WithEvents PrintDocument1 As New PrintDocument()
    Private merk As String
    Private jamTangan As String
    Private jumlahBeli As Integer
    Private harga As Decimal

    Public Sub New(merk As String, jamTangan As String, jumlah As Integer)
        InitializeComponent()
        Me.merk = merk
        Me.jamTangan = jamTangan
        Me.jumlahBeli = jumlah
        Me.harga = GetHarga(jamTangan)
        GenerateStruk()
    End Sub

    Private Function GetHarga(namaJamTangan As String) As Decimal
        Dim harga As Decimal = 0
        Try
            Dim query As String = "SELECT harga FROM jt WHERE nama = @nama"
            Using cmd As New MySqlCommand(query, Module1.CONN)
                cmd.Parameters.AddWithValue("@nama", namaJamTangan)
                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        harga = Convert.ToDecimal(reader("harga"))
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
        Return harga
    End Function

    Private Sub GenerateStruk()
        Dim struk As New System.Text.StringBuilder()

        ' Header struk
        struk.AppendLine(New String("-", 69))
        struk.AppendLine("JAMBRED STORE".PadLeft(37))
        struk.AppendLine(New String("-", 69))
        struk.AppendLine(Environment.NewLine)

        ' Header tabel
        struk.AppendLine("NAMA BARANG".PadRight(20) & "JUMLAH".PadRight(10) & "HARGA".PadLeft(17))
        struk.AppendLine(New String("-", 69))

        ' Isi tabel
        Dim hargaSatuan As Decimal = GetHarga(jamTangan)
        struk.AppendLine(jamTangan.PadRight(20) & jumlahBeli.ToString().PadRight(13) & hargaSatuan.ToString("C2"))

        struk.AppendLine(New String("-", 69))

        ' Total harga
        Dim totalHarga As Decimal = hargaSatuan * jumlahBeli
        struk.AppendLine("TOTAL HARGA ($):".PadLeft(49) & totalHarga.ToString("C2").PadLeft(10))

        ' Pesan CEO
        struk.AppendLine(Environment.NewLine)
        struk.AppendLine("CEO JAMBRED STORE".PadLeft(58))
        struk.AppendLine("Gerry Saputra".PadLeft(62))
        struk.AppendLine(New String("-", 69))

        ' Tampilkan struk di TextBox multiline
        txtStruk.Text = struk.ToString()
    End Sub
    Private Sub btnCetak_Click(sender As Object, e As EventArgs) Handles btnCetak.Click
        PrintPreviewDialog1.Document = PrintDocument1
        PrintPreviewDialog1.ShowDialog()
    End Sub

    Private Sub PrintDocument1_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim headerFont As New Font("Arial", 16, FontStyle.Bold)
        Dim tableFont As New Font("Arial", 12, FontStyle.Bold)
        Dim totalFont As New Font("Arial", 12, FontStyle.Bold)
        Dim ceoFont As New Font("Arial", 12, FontStyle.Bold)

        ' Header struk
        Dim headerText As String = "JAMBRED STORE"
        Dim margin As Integer = 50
        Dim lineLength As Integer = e.PageBounds.Width - (2 * margin)

        Dim headerX As Single = (e.PageBounds.Width - e.Graphics.MeasureString(headerText, headerFont).Width) / 2
        e.Graphics.DrawString(New String("-"c, lineLength), headerFont, Brushes.Black, margin, 50)
        e.Graphics.DrawString(headerText, headerFont, Brushes.Black, headerX, 70)
        e.Graphics.DrawString(New String("-"c, lineLength), headerFont, Brushes.Black, margin, 100)

        ' Spasi
        Dim currentY As Single = 140

        ' Header tabel (diubah menjadi tebal)
        Dim headerString As String = "NAMA BARANG".PadRight(20) & "JUMLAH".PadRight(10) & "HARGA".PadLeft(17)
        Dim headerXTable As Single = (e.PageBounds.Width - e.Graphics.MeasureString(headerString, tableFont).Width) / 2
        e.Graphics.DrawString(headerString, tableFont, Brushes.Black, headerXTable, currentY)
        currentY += 20
        e.Graphics.DrawString(New String("-"c, lineLength), tableFont, Brushes.Black, margin, currentY)

        ' Isi tabel
        Dim namaBarang As String = "Rolex Kan Abang"
        Dim jumlah As Integer = 12
        Dim hargaSatuan As Decimal = 111.0
        Dim isiString As String = namaBarang.PadRight(20) & jumlah.ToString().PadRight(10) & hargaSatuan.ToString("C2").PadLeft(17)
        Dim isiX As Single = (e.PageBounds.Width - e.Graphics.MeasureString(isiString, tableFont).Width) / 2
        currentY += 20
        e.Graphics.DrawString(isiString, tableFont, Brushes.Black, isiX, currentY)

        ' Garis pembatas
        currentY += 20
        e.Graphics.DrawString(New String("-"c, lineLength), tableFont, Brushes.Black, margin, currentY)

        ' Total harga
        Dim totalHarga As Decimal = hargaSatuan * jumlah
        Dim totalString As String = "TOTAL HARGA ($):".PadLeft(49) & totalHarga.ToString("C2").PadLeft(10)
        Dim totalX As Single = e.PageBounds.Width - e.Graphics.MeasureString(totalString, totalFont).Width - margin ' Posisi X relatif terhadap tepi kertas
        currentY += 20
        e.Graphics.DrawString(totalString, totalFont, Brushes.Black, totalX, currentY)

        ' Pesan tambahan
        Dim ceoString As String = "CEO JAMBRED STORE"
        Dim ceoX As Single = e.PageBounds.Width - e.Graphics.MeasureString(ceoString, ceoFont).Width - margin ' Posisi X relatif terhadap tepi kertas
        currentY += 40
        e.Graphics.DrawString(ceoString, ceoFont, Brushes.Black, ceoX, currentY)
        currentY += 20
        Dim gerryString As String = "Gerry Saputra"
        Dim gerryX As Single = e.PageBounds.Width - e.Graphics.MeasureString(gerryString, ceoFont).Width - margin ' Posisi X relatif terhadap tepi kertas
        e.Graphics.DrawString(gerryString, ceoFont, Brushes.Black, gerryX, currentY)

        ' Garis akhir
        currentY += 40
        e.Graphics.DrawString(New String("-"c, lineLength), headerFont, Brushes.Black, margin, currentY)
    End Sub

    Private Sub btnKeluar_Click(sender As Object, e As EventArgs) Handles btnKeluar.Click
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Me.Close()
            FMenu.Show()
        End If
    End Sub

End Class
