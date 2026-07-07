Imports HostsMan.UserInterface

Namespace UserInterface

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class MappingForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MappingForm))
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.TextBox_HostName = New System.Windows.Forms.TextBox()
            Me.TextBox_Comment = New System.Windows.Forms.TextBox()
            Me.Button_Confirm = New System.Windows.Forms.Button()
            Me.Button_Cancel = New System.Windows.Forms.Button()
            Me.MaskedTextBox_IP1 = New System.Windows.Forms.MaskedTextBox()
            Me.MaskedTextBox_IP2 = New System.Windows.Forms.MaskedTextBox()
            Me.MaskedTextBox_IP3 = New System.Windows.Forms.MaskedTextBox()
            Me.MaskedTextBox_IP4 = New System.Windows.Forms.MaskedTextBox()
            Me.RadioButton_Enabled = New System.Windows.Forms.RadioButton()
            Me.RadioButton_Disabled = New System.Windows.Forms.RadioButton()
            Me.SuspendLayout()
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(12, 15)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(58, 13)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "IP Address"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(12, 41)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(57, 13)
            Me.Label2.TabIndex = 5
            Me.Label2.Text = "HostName"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(12, 67)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(51, 13)
            Me.Label3.TabIndex = 7
            Me.Label3.Text = "Comment"
            '
            'TextBox_HostName
            '
            Me.TextBox_HostName.Location = New System.Drawing.Point(78, 38)
            Me.TextBox_HostName.Name = "TextBox_HostName"
            Me.TextBox_HostName.Size = New System.Drawing.Size(322, 20)
            Me.TextBox_HostName.TabIndex = 6
            '
            'TextBox_Comment
            '
            Me.TextBox_Comment.Location = New System.Drawing.Point(78, 64)
            Me.TextBox_Comment.Name = "TextBox_Comment"
            Me.TextBox_Comment.Size = New System.Drawing.Size(322, 20)
            Me.TextBox_Comment.TabIndex = 8
            '
            'Button_Confirm
            '
            Me.Button_Confirm.BackColor = System.Drawing.Color.Transparent
            Me.Button_Confirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Button_Confirm.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Confirm.Enabled = False
            Me.Button_Confirm.FlatAppearance.BorderSize = 0
            Me.Button_Confirm.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Button_Confirm.Location = New System.Drawing.Point(242, 90)
            Me.Button_Confirm.Name = "Button_Confirm"
            Me.Button_Confirm.Size = New System.Drawing.Size(158, 40)
            Me.Button_Confirm.TabIndex = 10
            Me.Button_Confirm.Text = "Confirm"
            Me.Button_Confirm.UseVisualStyleBackColor = False
            '
            'Button_Cancel
            '
            Me.Button_Cancel.BackColor = System.Drawing.Color.Transparent
            Me.Button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.Button_Cancel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Button_Cancel.FlatAppearance.BorderSize = 0
            Me.Button_Cancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Button_Cancel.Location = New System.Drawing.Point(78, 90)
            Me.Button_Cancel.Name = "Button_Cancel"
            Me.Button_Cancel.Size = New System.Drawing.Size(158, 40)
            Me.Button_Cancel.TabIndex = 9
            Me.Button_Cancel.Text = "Cancel"
            Me.Button_Cancel.UseVisualStyleBackColor = False
            '
            'MaskedTextBox_IP1
            '
            Me.MaskedTextBox_IP1.AsciiOnly = True
            Me.MaskedTextBox_IP1.BeepOnError = True
            Me.MaskedTextBox_IP1.Culture = New System.Globalization.CultureInfo("en-US")
            Me.MaskedTextBox_IP1.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
            Me.MaskedTextBox_IP1.Location = New System.Drawing.Point(78, 12)
            Me.MaskedTextBox_IP1.Mask = "000"
            Me.MaskedTextBox_IP1.Name = "MaskedTextBox_IP1"
            Me.MaskedTextBox_IP1.Size = New System.Drawing.Size(32, 20)
            Me.MaskedTextBox_IP1.TabIndex = 1
            Me.MaskedTextBox_IP1.Text = "127"
            '
            'MaskedTextBox_IP2
            '
            Me.MaskedTextBox_IP2.AsciiOnly = True
            Me.MaskedTextBox_IP2.BeepOnError = True
            Me.MaskedTextBox_IP2.Culture = New System.Globalization.CultureInfo("en-US")
            Me.MaskedTextBox_IP2.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
            Me.MaskedTextBox_IP2.Location = New System.Drawing.Point(120, 12)
            Me.MaskedTextBox_IP2.Mask = "000"
            Me.MaskedTextBox_IP2.Name = "MaskedTextBox_IP2"
            Me.MaskedTextBox_IP2.Size = New System.Drawing.Size(32, 20)
            Me.MaskedTextBox_IP2.TabIndex = 2
            Me.MaskedTextBox_IP2.Text = "0"
            '
            'MaskedTextBox_IP3
            '
            Me.MaskedTextBox_IP3.AsciiOnly = True
            Me.MaskedTextBox_IP3.BeepOnError = True
            Me.MaskedTextBox_IP3.Culture = New System.Globalization.CultureInfo("en-US")
            Me.MaskedTextBox_IP3.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
            Me.MaskedTextBox_IP3.Location = New System.Drawing.Point(162, 12)
            Me.MaskedTextBox_IP3.Mask = "000"
            Me.MaskedTextBox_IP3.Name = "MaskedTextBox_IP3"
            Me.MaskedTextBox_IP3.Size = New System.Drawing.Size(32, 20)
            Me.MaskedTextBox_IP3.TabIndex = 3
            Me.MaskedTextBox_IP3.Text = "0"
            '
            'MaskedTextBox_IP4
            '
            Me.MaskedTextBox_IP4.AsciiOnly = True
            Me.MaskedTextBox_IP4.BeepOnError = True
            Me.MaskedTextBox_IP4.Culture = New System.Globalization.CultureInfo("en-US")
            Me.MaskedTextBox_IP4.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
            Me.MaskedTextBox_IP4.Location = New System.Drawing.Point(204, 12)
            Me.MaskedTextBox_IP4.Mask = "000"
            Me.MaskedTextBox_IP4.Name = "MaskedTextBox_IP4"
            Me.MaskedTextBox_IP4.Size = New System.Drawing.Size(32, 20)
            Me.MaskedTextBox_IP4.TabIndex = 4
            Me.MaskedTextBox_IP4.Text = "1"
            '
            'RadioButton_Enabled
            '
            Me.RadioButton_Enabled.AutoSize = True
            Me.RadioButton_Enabled.Checked = True
            Me.RadioButton_Enabled.Cursor = System.Windows.Forms.Cursors.Hand
            Me.RadioButton_Enabled.Location = New System.Drawing.Point(12, 90)
            Me.RadioButton_Enabled.Name = "RadioButton_Enabled"
            Me.RadioButton_Enabled.Size = New System.Drawing.Size(64, 17)
            Me.RadioButton_Enabled.TabIndex = 12
            Me.RadioButton_Enabled.TabStop = True
            Me.RadioButton_Enabled.Text = "Enabled"
            Me.RadioButton_Enabled.UseVisualStyleBackColor = True
            '
            'RadioButton_Disabled
            '
            Me.RadioButton_Disabled.AutoSize = True
            Me.RadioButton_Disabled.Cursor = System.Windows.Forms.Cursors.Hand
            Me.RadioButton_Disabled.Location = New System.Drawing.Point(12, 113)
            Me.RadioButton_Disabled.Name = "RadioButton_Disabled"
            Me.RadioButton_Disabled.Size = New System.Drawing.Size(66, 17)
            Me.RadioButton_Disabled.TabIndex = 13
            Me.RadioButton_Disabled.Text = "Disabled"
            Me.RadioButton_Disabled.UseVisualStyleBackColor = True
            '
            'MappingForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(412, 142)
            Me.Controls.Add(Me.RadioButton_Disabled)
            Me.Controls.Add(Me.RadioButton_Enabled)
            Me.Controls.Add(Me.MaskedTextBox_IP4)
            Me.Controls.Add(Me.MaskedTextBox_IP3)
            Me.Controls.Add(Me.MaskedTextBox_IP2)
            Me.Controls.Add(Me.MaskedTextBox_IP1)
            Me.Controls.Add(Me.Button_Confirm)
            Me.Controls.Add(Me.Button_Cancel)
            Me.Controls.Add(Me.TextBox_Comment)
            Me.Controls.Add(Me.TextBox_HostName)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.Label1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "MappingForm"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Mapping Form"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents TextBox_HostName As System.Windows.Forms.TextBox
        Friend WithEvents TextBox_Comment As System.Windows.Forms.TextBox
        Friend WithEvents Button_Cancel As System.Windows.Forms.Button
        Friend WithEvents Button_Confirm As System.Windows.Forms.Button
        Friend WithEvents MaskedTextBox_IP1 As System.Windows.Forms.MaskedTextBox
        Friend WithEvents MaskedTextBox_IP2 As System.Windows.Forms.MaskedTextBox
        Friend WithEvents MaskedTextBox_IP3 As System.Windows.Forms.MaskedTextBox
        Friend WithEvents MaskedTextBox_IP4 As System.Windows.Forms.MaskedTextBox
        Friend WithEvents RadioButton_Enabled As System.Windows.Forms.RadioButton
        Friend WithEvents RadioButton_Disabled As System.Windows.Forms.RadioButton
    End Class

End Namespace
