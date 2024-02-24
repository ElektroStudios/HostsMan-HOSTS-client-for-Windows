Public Class About

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        Try
            Using p As New Process
                p.StartInfo.FileName = "https://github.com/ElektroStudios/HostsMan-HOSTS-client-for-Windows"
                p.StartInfo.UseShellExecute = True
                p.Start()
            End Using

        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        End Try

    End Sub

End Class