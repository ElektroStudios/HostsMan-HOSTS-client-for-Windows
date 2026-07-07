Namespace UserInterface

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Main
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
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
            Me.ListView_Mappings = New System.Windows.Forms.ListView()
            Me.ColumnHeader_Enabled = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.ColumnHeader_IP = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.ColumnHeader_HostName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.ColumnHeader_Comment = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.ContextMenuStrip_Listview = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.ToolStripMenuItem_CopyIP = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripMenuItem_CopyHostname = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripMenuItem_Navigate = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStrip_MainMenu = New System.Windows.Forms.ToolStrip()
            Me.ToolStripButton_Add = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton_Remove = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton_Enable = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton_Disable = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton_Edit = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolStripButton_Rescan = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton_Clean = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton_Save = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolStripButton_Directory = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripButton_Notepad = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolStripButton_About = New System.Windows.Forms.ToolStripButton()
            Me.Panel_Search = New System.Windows.Forms.Panel()
            Me.ComboBox_SearchCondition = New System.Windows.Forms.ComboBox()
            Me.ComboBox_SearchType = New System.Windows.Forms.ComboBox()
            Me.TextBox_Search = New System.Windows.Forms.TextBox()
            Me.Label_Search = New System.Windows.Forms.Label()
            Me.ContextMenuStrip_Listview.SuspendLayout()
            Me.ToolStrip_MainMenu.SuspendLayout()
            Me.Panel_Search.SuspendLayout()
            Me.SuspendLayout()
            '
            'ListView_Mappings
            '
            Me.ListView_Mappings.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader_Enabled, Me.ColumnHeader_IP, Me.ColumnHeader_HostName, Me.ColumnHeader_Comment})
            Me.ListView_Mappings.ContextMenuStrip = Me.ContextMenuStrip_Listview
            Me.ListView_Mappings.Dock = System.Windows.Forms.DockStyle.Fill
            Me.ListView_Mappings.FullRowSelect = True
            Me.ListView_Mappings.GridLines = True
            Me.ListView_Mappings.Location = New System.Drawing.Point(0, 104)
            Me.ListView_Mappings.Name = "ListView_Mappings"
            Me.ListView_Mappings.Size = New System.Drawing.Size(622, 335)
            Me.ListView_Mappings.Sorting = System.Windows.Forms.SortOrder.Ascending
            Me.ListView_Mappings.TabIndex = 0
            Me.ListView_Mappings.UseCompatibleStateImageBehavior = False
            Me.ListView_Mappings.View = System.Windows.Forms.View.Details
            '
            'ColumnHeader_Enabled
            '
            Me.ColumnHeader_Enabled.Text = "Enabled"
            '
            'ColumnHeader_IP
            '
            Me.ColumnHeader_IP.Text = "IP Address"
            Me.ColumnHeader_IP.Width = 100
            '
            'ColumnHeader_HostName
            '
            Me.ColumnHeader_HostName.Text = "HostName"
            Me.ColumnHeader_HostName.Width = 215
            '
            'ColumnHeader_Comment
            '
            Me.ColumnHeader_Comment.Text = "Mapping Comment"
            Me.ColumnHeader_Comment.Width = 215
            '
            'ContextMenuStrip_Listview
            '
            Me.ContextMenuStrip_Listview.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem_CopyIP, Me.ToolStripMenuItem_CopyHostname, Me.ToolStripMenuItem_Navigate})
            Me.ContextMenuStrip_Listview.Name = "ContextMenuStrip_Listview"
            Me.ContextMenuStrip_Listview.Size = New System.Drawing.Size(163, 70)
            '
            'ToolStripMenuItem_CopyIP
            '
            Me.ToolStripMenuItem_CopyIP.Image = Global.HostsMan.My.Resources.Resources.Clipboard
            Me.ToolStripMenuItem_CopyIP.Name = "ToolStripMenuItem_CopyIP"
            Me.ToolStripMenuItem_CopyIP.Size = New System.Drawing.Size(162, 22)
            Me.ToolStripMenuItem_CopyIP.Text = "Copy IP"
            '
            'ToolStripMenuItem_CopyHostname
            '
            Me.ToolStripMenuItem_CopyHostname.Image = Global.HostsMan.My.Resources.Resources.Clipboard
            Me.ToolStripMenuItem_CopyHostname.Name = "ToolStripMenuItem_CopyHostname"
            Me.ToolStripMenuItem_CopyHostname.Size = New System.Drawing.Size(162, 22)
            Me.ToolStripMenuItem_CopyHostname.Text = "Copy HostName"
            '
            'ToolStripMenuItem_Navigate
            '
            Me.ToolStripMenuItem_Navigate.Image = Global.HostsMan.My.Resources.Resources.Navigate
            Me.ToolStripMenuItem_Navigate.Name = "ToolStripMenuItem_Navigate"
            Me.ToolStripMenuItem_Navigate.Size = New System.Drawing.Size(162, 22)
            Me.ToolStripMenuItem_Navigate.Text = "Navigate"
            '
            'ToolStrip_MainMenu
            '
            Me.ToolStrip_MainMenu.BackColor = System.Drawing.SystemColors.Control
            Me.ToolStrip_MainMenu.ImageScalingSize = New System.Drawing.Size(48, 48)
            Me.ToolStrip_MainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton_Add, Me.ToolStripButton_Remove, Me.ToolStripButton_Enable, Me.ToolStripButton_Disable, Me.ToolStripButton_Edit, Me.ToolStripSeparator1, Me.ToolStripButton_Rescan, Me.ToolStripButton_Clean, Me.ToolStripButton_Save, Me.ToolStripSeparator2, Me.ToolStripButton_Directory, Me.ToolStripButton_Notepad, Me.ToolStripSeparator3, Me.ToolStripButton_About})
            Me.ToolStrip_MainMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow
            Me.ToolStrip_MainMenu.Location = New System.Drawing.Point(0, 0)
            Me.ToolStrip_MainMenu.Name = "ToolStrip_MainMenu"
            Me.ToolStrip_MainMenu.Size = New System.Drawing.Size(622, 74)
            Me.ToolStrip_MainMenu.TabIndex = 2
            '
            'ToolStripButton_Add
            '
            Me.ToolStripButton_Add.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Add.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Add.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Add.Image = Global.HostsMan.My.Resources.Resources.Add
            Me.ToolStripButton_Add.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Add.Name = "ToolStripButton_Add"
            Me.ToolStripButton_Add.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Add.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Add.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Add.ToolTipText = "Add New Mapping"
            '
            'ToolStripButton_Remove
            '
            Me.ToolStripButton_Remove.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Remove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Remove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Remove.Enabled = False
            Me.ToolStripButton_Remove.Image = Global.HostsMan.My.Resources.Resources.Remove
            Me.ToolStripButton_Remove.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Remove.Name = "ToolStripButton_Remove"
            Me.ToolStripButton_Remove.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Remove.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Remove.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Remove.ToolTipText = "Remove Selected Mapping(s)"
            '
            'ToolStripButton_Enable
            '
            Me.ToolStripButton_Enable.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Enable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Enable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Enable.Enabled = False
            Me.ToolStripButton_Enable.Image = Global.HostsMan.My.Resources.Resources.Enable
            Me.ToolStripButton_Enable.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Enable.Name = "ToolStripButton_Enable"
            Me.ToolStripButton_Enable.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Enable.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Enable.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Enable.ToolTipText = "Enable Selected Mapping(s)"
            '
            'ToolStripButton_Disable
            '
            Me.ToolStripButton_Disable.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Disable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Disable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Disable.Enabled = False
            Me.ToolStripButton_Disable.Image = Global.HostsMan.My.Resources.Resources.Disable
            Me.ToolStripButton_Disable.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Disable.Name = "ToolStripButton_Disable"
            Me.ToolStripButton_Disable.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Disable.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Disable.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Disable.ToolTipText = "Disable Selected Mapping(s)"
            '
            'ToolStripButton_Edit
            '
            Me.ToolStripButton_Edit.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Edit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Edit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Edit.Enabled = False
            Me.ToolStripButton_Edit.Image = Global.HostsMan.My.Resources.Resources.Edit
            Me.ToolStripButton_Edit.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Edit.Name = "ToolStripButton_Edit"
            Me.ToolStripButton_Edit.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Edit.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Edit.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Edit.ToolTipText = "Edit Selected Mapping"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.BackColor = System.Drawing.Color.Black
            Me.ToolStripSeparator1.Margin = New System.Windows.Forms.Padding(1)
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 23)
            '
            'ToolStripButton_Rescan
            '
            Me.ToolStripButton_Rescan.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Rescan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Rescan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Rescan.Image = Global.HostsMan.My.Resources.Resources.Refresh
            Me.ToolStripButton_Rescan.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Rescan.Name = "ToolStripButton_Rescan"
            Me.ToolStripButton_Rescan.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Rescan.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Rescan.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Rescan.ToolTipText = "Rescan Hosts File"
            '
            'ToolStripButton_Clean
            '
            Me.ToolStripButton_Clean.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Clean.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Clean.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Clean.Enabled = False
            Me.ToolStripButton_Clean.Image = Global.HostsMan.My.Resources.Resources.Clean
            Me.ToolStripButton_Clean.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Clean.Name = "ToolStripButton_Clean"
            Me.ToolStripButton_Clean.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Clean.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Clean.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Clean.ToolTipText = "Clean All Mappings"
            '
            'ToolStripButton_Save
            '
            Me.ToolStripButton_Save.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Save.Enabled = False
            Me.ToolStripButton_Save.Image = Global.HostsMan.My.Resources.Resources.Save
            Me.ToolStripButton_Save.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Save.Name = "ToolStripButton_Save"
            Me.ToolStripButton_Save.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Save.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Save.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Save.ToolTipText = "Save Changes To Hosts File"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.BackColor = System.Drawing.Color.Black
            Me.ToolStripSeparator2.Margin = New System.Windows.Forms.Padding(1)
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 23)
            '
            'ToolStripButton_Directory
            '
            Me.ToolStripButton_Directory.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Directory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Directory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Directory.Image = Global.HostsMan.My.Resources.Resources.Directory
            Me.ToolStripButton_Directory.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Directory.Name = "ToolStripButton_Directory"
            Me.ToolStripButton_Directory.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Directory.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Directory.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Directory.ToolTipText = "Open Hosts Directory"
            '
            'ToolStripButton_Notepad
            '
            Me.ToolStripButton_Notepad.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_Notepad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_Notepad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_Notepad.Image = Global.HostsMan.My.Resources.Resources.Notepad
            Me.ToolStripButton_Notepad.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_Notepad.Name = "ToolStripButton_Notepad"
            Me.ToolStripButton_Notepad.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_Notepad.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_Notepad.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_Notepad.ToolTipText = "Open Hosts With Notepad"
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Margin = New System.Windows.Forms.Padding(1)
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 23)
            '
            'ToolStripButton_About
            '
            Me.ToolStripButton_About.BackColor = System.Drawing.Color.Transparent
            Me.ToolStripButton_About.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.ToolStripButton_About.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ToolStripButton_About.Image = Global.HostsMan.My.Resources.Resources.About
            Me.ToolStripButton_About.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripButton_About.Name = "ToolStripButton_About"
            Me.ToolStripButton_About.Size = New System.Drawing.Size(52, 52)
            Me.ToolStripButton_About.TextAlign = System.Drawing.ContentAlignment.BottomCenter
            Me.ToolStripButton_About.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
            Me.ToolStripButton_About.ToolTipText = "About..."
            '
            'Panel_Search
            '
            Me.Panel_Search.BackColor = System.Drawing.SystemColors.Control
            Me.Panel_Search.Controls.Add(Me.ComboBox_SearchCondition)
            Me.Panel_Search.Controls.Add(Me.ComboBox_SearchType)
            Me.Panel_Search.Controls.Add(Me.TextBox_Search)
            Me.Panel_Search.Controls.Add(Me.Label_Search)
            Me.Panel_Search.Dock = System.Windows.Forms.DockStyle.Top
            Me.Panel_Search.Location = New System.Drawing.Point(0, 74)
            Me.Panel_Search.Name = "Panel_Search"
            Me.Panel_Search.Size = New System.Drawing.Size(622, 30)
            Me.Panel_Search.TabIndex = 3
            '
            'ComboBox_SearchCondition
            '
            Me.ComboBox_SearchCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox_SearchCondition.FormattingEnabled = True
            Me.ComboBox_SearchCondition.Items.AddRange(New Object() {"Starting with text...", "Containing text..."})
            Me.ComboBox_SearchCondition.Location = New System.Drawing.Point(145, 4)
            Me.ComboBox_SearchCondition.Name = "ComboBox_SearchCondition"
            Me.ComboBox_SearchCondition.Size = New System.Drawing.Size(114, 21)
            Me.ComboBox_SearchCondition.TabIndex = 5
            '
            'ComboBox_SearchType
            '
            Me.ComboBox_SearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboBox_SearchType.FormattingEnabled = True
            Me.ComboBox_SearchType.Items.AddRange(New Object() {"IP", "HostName"})
            Me.ComboBox_SearchType.Location = New System.Drawing.Point(54, 4)
            Me.ComboBox_SearchType.Name = "ComboBox_SearchType"
            Me.ComboBox_SearchType.Size = New System.Drawing.Size(85, 21)
            Me.ComboBox_SearchType.TabIndex = 4
            '
            'TextBox_Search
            '
            Me.TextBox_Search.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextBox_Search.Location = New System.Drawing.Point(265, 5)
            Me.TextBox_Search.Name = "TextBox_Search"
            Me.TextBox_Search.Size = New System.Drawing.Size(354, 20)
            Me.TextBox_Search.TabIndex = 4
            '
            'Label_Search
            '
            Me.Label_Search.AutoSize = True
            Me.Label_Search.Location = New System.Drawing.Point(7, 8)
            Me.Label_Search.Name = "Label_Search"
            Me.Label_Search.Size = New System.Drawing.Size(41, 13)
            Me.Label_Search.TabIndex = 0
            Me.Label_Search.Text = "Search"
            '
            'Main
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.ClientSize = New System.Drawing.Size(622, 439)
            Me.Controls.Add(Me.ListView_Mappings)
            Me.Controls.Add(Me.Panel_Search)
            Me.Controls.Add(Me.ToolStrip_MainMenu)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.Name = "Main"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "HostsMan"
            Me.ContextMenuStrip_Listview.ResumeLayout(False)
            Me.ToolStrip_MainMenu.ResumeLayout(False)
            Me.ToolStrip_MainMenu.PerformLayout()
            Me.Panel_Search.ResumeLayout(False)
            Me.Panel_Search.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ListView_Mappings As System.Windows.Forms.ListView
        Friend WithEvents ToolStrip_MainMenu As System.Windows.Forms.ToolStrip
        Friend WithEvents ToolStripButton_Add As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton_Directory As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton_Remove As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton_Enable As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton_Clean As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripButton_Save As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton_Notepad As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton_Disable As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents Panel_Search As System.Windows.Forms.Panel
        Friend WithEvents Label_Search As System.Windows.Forms.Label
        Friend WithEvents ColumnHeader_IP As System.Windows.Forms.ColumnHeader
        Friend WithEvents ColumnHeader_HostName As System.Windows.Forms.ColumnHeader
        Friend WithEvents ColumnHeader_Comment As System.Windows.Forms.ColumnHeader
        Friend WithEvents ToolStripButton_Rescan As System.Windows.Forms.ToolStripButton
        Friend WithEvents TextBox_Search As System.Windows.Forms.TextBox
        Friend WithEvents ColumnHeader_Enabled As System.Windows.Forms.ColumnHeader
        Friend WithEvents ToolStripButton_Edit As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripButton_About As System.Windows.Forms.ToolStripButton
        Friend WithEvents ComboBox_SearchType As System.Windows.Forms.ComboBox
        Friend WithEvents ComboBox_SearchCondition As System.Windows.Forms.ComboBox
        Friend WithEvents ContextMenuStrip_Listview As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents ToolStripMenuItem_CopyIP As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem_CopyHostname As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem_Navigate As System.Windows.Forms.ToolStripMenuItem

    End Class

End Namespace