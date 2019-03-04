' ***********************************************************************
' Author   : Elektro
' Modified : 18-January-2015
' ***********************************************************************
' <copyright file="Main.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports HostsMan.Tools
Imports HostsMan.Tools.HostsTools
Imports System.IO
Imports HostsMan.Tools.ListViewColumnSorter
Imports System.ComponentModel

#End Region

#Region " Main "

Namespace UserInterface

    ''' <summary>
    ''' The Main UserInterface Form.
    ''' </summary>
    Public NotInheritable Class Main

#Region " Properties "

        ''' <summary>
        ''' The <see cref="WindowSticker"/> instance that sticks the Form on the Desktop screen.
        ''' </summary>
        Private windowSticker As New WindowSticker(ClientForm:=Me) With {.SnapMargin = 35}

        ''' <summary>
        ''' Gets or sets the hosts object.
        ''' </summary>
        ''' <value>The hosts object.</value>
        Private Property HostsObj As New HostsTools()

        ''' <summary>
        ''' Gets or sets the Hosts mappings.
        ''' </summary>
        ''' <value>The Hosts mappings.</value>
        Private Property Mappings As List(Of MappingInfo)

        ''' <summary>
        ''' Gets the Windows Directory path.
        ''' </summary>
        ''' <value>The Windows Directory path.</value>
        Private ReadOnly Property WinDirPath As String
            Get
                Return Environment.GetFolderPath(Environment.SpecialFolder.Windows)
            End Get
        End Property

        ''' <summary>
        ''' The ListViewColumnSorter instance.
        ''' </summary>
        Private columnSorter As New ListViewColumnSorter

        ''' <summary>
        ''' Gets or sets the mapping search type.
        ''' </summary>
        ''' <value>The mapping search type.</value>
        Private Property CurrentMappingInfoSearchType As MappingInfoSearchType

        ''' <summary>
        ''' Gets or sets the mapping search condition.
        ''' </summary>
        ''' <value>The mapping search condition.</value>
        Private Property CurrentMappingInfoSearchCondition As MappingInfoSearchCondition

#End Region

#Region " Enumerations "

        ''' <summary>
        ''' Specifies a mapping search type.
        ''' </summary>
        Private Enum MappingInfoSearchType

            ''' <summary>
            ''' Search by IP Address.
            ''' </summary>
            IP

            ''' <summary>
            ''' Search by HostName.
            ''' </summary>
            HostName

        End Enum

        ''' <summary>
        ''' Specifies a mapping search condition.
        ''' </summary>
        Private Enum MappingInfoSearchCondition

            ''' <summary>
            ''' Starting with text.
            ''' </summary>
            StartsWithText

            ''' <summary>
            ''' Containing text.
            ''' </summary>
            ContainsText

        End Enum

#End Region

#Region " Event Handlers "

#Region " Form "

        ''' <summary>
        ''' Handles the Shown event of the Main Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Main_Shown(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Shown

            Me.MinimumSize = Me.Size
            Me.InitializeListview()
            Me.InitializeComboBox()
            Me.LoadHostsFile()

        End Sub

#End Region

#Region " ToolStrip "

        ''' <summary>
        ''' Handles the MouseEnter event of the ToolStripButton controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButtons_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_About.MouseEnter,
                ToolStripButton_Add.MouseEnter,
                ToolStripButton_Clean.MouseEnter,
                ToolStripButton_Directory.MouseEnter,
                ToolStripButton_Disable.MouseEnter,
                ToolStripButton_Edit.MouseEnter,
                ToolStripButton_Enable.MouseEnter,
                ToolStripButton_Notepad.MouseEnter,
                ToolStripButton_Remove.MouseEnter,
                ToolStripButton_Rescan.MouseEnter,
                ToolStripButton_Save.MouseEnter

            Me.Cursor = Cursors.Hand

        End Sub

        ''' <summary>
        ''' Handles the MouseLeave event of the ToolStripButton controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButtons_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_About.MouseLeave,
                ToolStripButton_Add.MouseLeave,
                ToolStripButton_Clean.MouseLeave,
                ToolStripButton_Directory.MouseLeave,
                ToolStripButton_Disable.MouseLeave,
                ToolStripButton_Edit.MouseLeave,
                ToolStripButton_Enable.MouseLeave,
                ToolStripButton_Notepad.MouseLeave,
                ToolStripButton_Remove.MouseLeave,
                ToolStripButton_Rescan.MouseLeave,
                ToolStripButton_Save.MouseLeave

            Me.Cursor = Me.DefaultCursor

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Add control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Add_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Add.Click

            Dim mappingInfo As New MappingInfo

            Using mappingForm As New MappingForm(mappingInfo)

                mappingForm.Icon = Icon.FromHandle(My.Resources.Add.GetHicon)
                mappingForm.Text = "Add New Mapping"

                If mappingForm.ShowDialog() = Windows.Forms.DialogResult.OK Then

                    If Me.Mappings.Exists(Function(x As MappingInfo)
                                              Return x.HostName.Equals(mappingInfo.HostName, StringComparison.OrdinalIgnoreCase)
                                          End Function) Then

                        Using New CenteredMessageBox(ownerForm:=Me)
                            MessageBox.Show("This HostName already exists in list", Me.Text,
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Using

                    Else
                        Me.Mappings.Add(mappingInfo)
                        Me.AppendMappingsToListview(mappingInfo)
                        Me.ToolStripButton_Clean.Enabled = CBool(ListView_Mappings.Items.Count)
                        Me.ToolStripButton_Save.Enabled = True

                    End If

                End If

            End Using

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Remove control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Remove_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Remove.Click

            Me.ListView_Mappings.Enabled = False

            For Each item As ListViewItem In Me.ListView_Mappings.SelectedItems

                Dim mapping As MappingInfo = Me.FindMappingInfo(item)
                Me.Mappings.Remove(mapping)
                Me.ListView_Mappings.Items.Remove(item)

            Next item

            Me.ToolStripButton_Clean.Enabled = CBool(ListView_Mappings.Items.Count)
            Me.ToolStripButton_Save.Enabled = True
            Me.ListView_Mappings.Enabled = True

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Enable control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Enable_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Enable.Click

            Me.ListView_Mappings.Enabled = False

            For Each item As ListViewItem In Me.ListView_Mappings.SelectedItems

                Dim mapping As MappingInfo = Me.FindMappingInfo(item)
                mapping.IsDisabled = False
                item.SubItems(0).Text = "Yes"

            Next item

            Me.ListView_Mappings.Enabled = True

            Me.ToolStripButton_Enable.Enabled = False
            Me.ToolStripButton_Disable.Enabled = True
            Me.ToolStripButton_Save.Enabled = True

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Disable control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Disable_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Disable.Click

            Me.ListView_Mappings.Enabled = False

            For Each item As ListViewItem In Me.ListView_Mappings.SelectedItems

                Dim mapping As MappingInfo = Me.FindMappingInfo(item)
                mapping.IsDisabled = True
                item.SubItems(0).Text = "No"

            Next item

            Me.ListView_Mappings.Enabled = True

            Me.ToolStripButton_Enable.Enabled = True
            Me.ToolStripButton_Disable.Enabled = False
            Me.ToolStripButton_Save.Enabled = True

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Edit control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Edit_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Edit.Click

            Me.EditMapping(Me.FindMappingInfo(Me.ListView_Mappings.SelectedItems(0)))

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Rescan control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Rescan_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Rescan.Click

            If Not ListView_Mappings.Items.Count = 0 Then

                Using New CenteredMessageBox(ownerForm:=Me)

                    If MessageBox.Show("This will discard all the current changes, Are you sure?", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                        Exit Sub
                    End If

                End Using

            End If

            Me.LoadHostsFile()

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Clean control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Clean_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Clean.Click

            Using New CenteredMessageBox(ownerForm:=Me)

                If MessageBox.Show("This will remove all mappings from list, Are you sure?", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then

                    Me.Mappings.Clear()
                    Me.ListView_Mappings.Items.Clear()

                    DirectCast(sender, ToolStripButton).Enabled = False
                    Me.ToolStripButton_Remove.Enabled = CBool(ListView_Mappings.SelectedItems.Count)
                    Me.ToolStripButton_Enable.Enabled = CBool(ListView_Mappings.SelectedItems.Count)
                    Me.ToolStripButton_Disable.Enabled = CBool(ListView_Mappings.SelectedItems.Count)
                    Me.ToolStripButton_Save.Enabled = True
                    Me.ToolStripButton_Edit.Enabled = False

                End If

            End Using

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Save control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Save_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Save.Click

            Dim oldPath As String = Me.HostsObj.HostsLocation
            Dim tmpPath As String = Path.GetTempFileName

            Me.HostsObj.HostsLocation = tmpPath

            Try
                Me.HostsObj.FileCreate()

            Catch ex As Exception
                Using New CenteredMessageBox(ownerForm:=Me)
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub

            End Try

            Try
                For Each mappinginfo As MappingInfo In Mappings
                    Me.HostsObj.Add(mappinginfo)
                Next mappinginfo

            Catch ex As Exception
                Using New CenteredMessageBox(ownerForm:=Me)
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub

            Finally
                Me.HostsObj.HostsLocation = oldPath
                DirectCast(sender, ToolStripButton).Enabled = False

            End Try

            Try
                File.Replace(tmpPath, oldPath, Path.GetTempFileName)

            Catch ex As Exception
                Using New CenteredMessageBox(ownerForm:=Me)
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub

            End Try

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Directory control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Directory_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Directory.Click

            Try
                Process.Start(Path.Combine(Me.WinDirPath, "Explorer.exe"),
                              String.Format("/Select,""{0}""", Me.HostsObj.HostsLocation))

            Catch ex As Exception
                Using New CenteredMessageBox(ownerForm:=Me)
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using

            End Try

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Notepad control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Notepad_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Notepad.Click

            Try
                Me.HostsObj.FileOpen(Path.Combine(Me.WinDirPath, "Notepad.exe"))

            Catch ex As Exception
                Using New CenteredMessageBox(ownerForm:=Me)
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using

            End Try

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_About control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_About_Click(sender As Object, e As EventArgs) _
        Handles ToolStripButton_About.Click

            Using aboutform As New About
                aboutform.ShowDialog()
            End Using

        End Sub

#End Region

#Region " Seach Panel "

        ''' <summary>
        ''' Handles the SelectedIndexChanged event of the ComboBox_SearchType control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ComboBox_SearchType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ComboBox_SearchType.SelectedIndexChanged

            Me.CurrentMappingInfoSearchType = DirectCast([Enum].Parse(GetType(MappingInfoSearchType),
                                                                       DirectCast(sender, ComboBox).Text, True), 
                                                                   MappingInfoSearchType)

            ' Update search
            If Me.TextBox_Search.TextLength <> 0 Then
                Me.FindMappingByText(Me.CurrentMappingInfoSearchType, Me.CurrentMappingInfoSearchCondition, Me.TextBox_Search.Text)
            End If

        End Sub

        ''' <summary>
        ''' Handles the SelectedIndexChanged event of the ComboBox_SearchCondition control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ComboBox_SearchCondition_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ComboBox_SearchCondition.SelectedIndexChanged

            Select Case DirectCast(sender, ComboBox).Text.ToLower

                Case "starting with text..."
                    Me.CurrentMappingInfoSearchCondition = MappingInfoSearchCondition.StartsWithText

                Case "containing text..."
                    Me.CurrentMappingInfoSearchCondition = MappingInfoSearchCondition.ContainsText

                Case Else
                    Throw New Exception("Wrong CurrentMappingInfoSearchCondition name!")

            End Select

            ' Update search
            If Me.TextBox_Search.TextLength <> 0 Then
                Me.FindMappingByText(Me.CurrentMappingInfoSearchType, Me.CurrentMappingInfoSearchCondition, Me.TextBox_Search.Text)
            End If

        End Sub

        ''' <summary>
        ''' Handles the TextChanged event of the TextBox_Search control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub TextBox_Search_TextChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles TextBox_Search.TextChanged

            Me.FindMappingByText(Me.CurrentMappingInfoSearchType, Me.CurrentMappingInfoSearchCondition, DirectCast(sender, TextBox).Text)

        End Sub

#End Region

#Region " ListView "

        ''' <summary>
        ''' Handles the SelectedIndexChanged event of the ListView_Mappings control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ListView_Mappings_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ListView_Mappings.SelectedIndexChanged

            Dim lv As ListView = DirectCast(sender, ListView)

            Me.ToolStripButton_Remove.Enabled = CBool(lv.SelectedItems.Count)
            Me.ToolStripButton_Enable.Enabled = If(lv.SelectedItems.Count = 1, FindMappingInfo(ListView_Mappings.SelectedItems(0)).IsDisabled, CBool(lv.SelectedItems.Count))
            Me.ToolStripButton_Disable.Enabled = If(lv.SelectedItems.Count = 1, Not FindMappingInfo(ListView_Mappings.SelectedItems(0)).IsDisabled, CBool(lv.SelectedItems.Count))
            Me.ToolStripButton_Clean.Enabled = CBool(lv.Items.Count)
            Me.ToolStripButton_Edit.Enabled = lv.SelectedItems.Count = 1

        End Sub

        ''' <summary>
        ''' Handles the MouseDoubleClick event of the ListView_Mappings control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ListView_Mappings_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ListView_Mappings.MouseDoubleClick

            If DirectCast(e, MouseEventArgs).Button = Windows.Forms.MouseButtons.Left Then
                Me.EditMapping(Me.FindMappingInfo(Me.ListView_Mappings.SelectedItems(0)))
            End If

        End Sub

        ''' <summary>
        ''' Handles the ColumnClick event of the ListView_Mappings control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="ColumnClickEventArgs"/> instance containing the event data.</param>
        Private Sub ListView_Mappings_ColumnClick(ByVal sender As Object, ByVal e As ColumnClickEventArgs) _
        Handles ListView_Mappings.ColumnClick

            Dim lv As ListView = DirectCast(sender, ListView)

            ' Dinamycaly sets the sort-modifier to sort the column by text, number, or date.
            Me.columnSorter.SortModifier = DirectCast(lv.Columns(e.Column).Tag, SortModifiers)

            ' Determine whether clicked column is already the column that is being sorted.
            If e.Column = Me.columnSorter.Column Then

                ' Reverse the current sort direction for this column.
                If Me.columnSorter.Order = SortOrder.Ascending Then
                    Me.columnSorter.Order = SortOrder.Descending

                Else
                    Me.columnSorter.Order = SortOrder.Ascending

                End If ' Sorter.Order

            Else

                ' Set the column number that is to be sorted, default to ascending.
                Me.columnSorter.Column = e.Column
                Me.columnSorter.Order = SortOrder.Ascending

            End If ' e.Column

            ' Perform the sort with these new sort options.
            lv.Sort()

        End Sub

#End Region

#Region " ContextMenu "

        ''' <summary>
        ''' Handles the Opening event of the ContextMenuStrip_Listview control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        Private Sub ContextMenuStrip_Listview_Opening(ByVal sender As Object, ByVal e As CancelEventArgs) _
        Handles ContextMenuStrip_Listview.Opening

            e.Cancel = Not Me.ListView_Mappings.SelectedItems.Count = 1

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripMenuItem_CopyIP control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripMenuItem_CopyIP_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripMenuItem_CopyIP.Click

            Clipboard.SetText(Me.ListView_Mappings.SelectedItems(0).SubItems(1).Text)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripMenuItem_CopyHostname control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripMenuItem_CopyHostname_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripMenuItem_CopyHostname.Click

            Clipboard.SetText(Me.ListView_Mappings.SelectedItems(0).SubItems(2).Text)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripMenuItem_Navigate control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripMenuItem_Navigate_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripMenuItem_Navigate.Click

            Dim hostName As String = Me.ListView_Mappings.SelectedItems(0).SubItems(2).Text
            If Not hostName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) Then
                hostName = hostName.Insert(0, "http://")
            End If

            Try
                Process.Start(hostName)

            Catch ex As Exception
                Using New CenteredMessageBox(ownerForm:=Me)
                    MessageBox.Show(ex.Message, Me.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using

            End Try

        End Sub

#End Region

#End Region

#Region " Methods "

        ''' <summary>
        ''' Initializes the listview properties.
        ''' </summary>
        Private Sub InitializeListview()

            With Me.ListView_Mappings

                ' Set the sorter, our 'ListViewColumnSorter'.
                .ListViewItemSorter = Me.columnSorter

                ' The sorting default direction.
                .Sorting = SortOrder.Ascending

                ' Set the default sort-modifier.
                Me.columnSorter.SortModifier = SortModifiers.SortByText

                .Columns(0).Tag = SortModifiers.SortByText
                .Columns(1).Tag = SortModifiers.SortByText
                .Columns(2).Tag = SortModifiers.SortByText
                .Columns(3).Tag = SortModifiers.SortByText

            End With

        End Sub

        ''' <summary>
        ''' Initializes the ComboBoxes properties.
        ''' </summary>
        Private Sub InitializeComboBox()

            Me.ComboBox_SearchType.SelectedIndex = 1
            Me.ComboBox_SearchCondition.SelectedIndex = 1

        End Sub

        ''' <summary>
        ''' Loads the hosts file.
        ''' </summary>
        Private Sub LoadHostsFile()

            If Not Me.HostsObj.FileExists() Then

                Try
                    Me.HostsObj.FileCreate()

                Catch ex As Exception
                    Using New CenteredMessageBox(ownerForm:=Me)
                        MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Using

                    Exit Sub

                End Try

            End If

            Me.Mappings = Me.HostsObj.GetMappings()
            Me.AddMappingsToListview(Me.Mappings)
            Me.ToolStripButton_Clean.Enabled = CBool(ListView_Mappings.Items.Count)
            Me.ToolStripButton_Save.Enabled = False
            Me.ToolStripButton_Enable.Enabled = False
            Me.ToolStripButton_Disable.Enabled = False
            Me.ToolStripButton_Remove.Enabled = False
            Me.ToolStripButton_Edit.Enabled = False

        End Sub

        ''' <summary>
        ''' Finds the <see cref="MappingInfo"/> equivalent to the specified listview item.
        ''' </summary>
        ''' <param name="item">The listview item.</param>
        ''' <returns>MappingInfo.</returns>
        Private Function FindMappingInfo(ByVal item As ListViewItem) As MappingInfo

            Dim mapping As MappingInfo = (From mappingInfo As MappingInfo In Me.Mappings
                                          Where mappingInfo.IP = item.SubItems(1).Text _
                                                AndAlso mappingInfo.HostName = item.SubItems(2).Text).First

            Return mapping

        End Function

        ''' <summary>
        ''' Adds the mappings into listview.
        ''' </summary>
        ''' <param name="mappings">The mappings.</param>
        Private Sub AddMappingsToListview(ByVal mappings As IEnumerable(Of MappingInfo))

            Dim lvCol As New List(Of ListViewItem)

            For Each mappingInfo As MappingInfo In mappings
                lvCol.Add(New ListViewItem({If(Not mappingInfo.IsDisabled, "Yes", "No"),
                                               mappingInfo.IP,
                                               mappingInfo.HostName,
                                               mappingInfo.Comment}))
            Next mappingInfo

            Me.ListView_Mappings.BeginUpdate()
            Me.ListView_Mappings.Items.Clear()
            Me.ListView_Mappings.Items.AddRange(lvCol.ToArray)
            Me.ListView_Mappings.EndUpdate()

        End Sub

        ''' <summary>
        ''' Adds the mapping into listview.
        ''' </summary>
        Private Sub AppendMappingsToListview(ByVal mappingInfo As MappingInfo)

            Me.ListView_Mappings.Items.Add(New ListViewItem({If(Not mappingInfo.IsDisabled, "Yes", "No"),
                                                             mappingInfo.IP,
                                                             mappingInfo.HostName,
                                                             mappingInfo.Comment}))

        End Sub

        ''' <summary>
        ''' Edits the specified mapping.
        ''' </summary>
        ''' <param name="mappingInfo">The mapping information.</param>
        Private Sub EditMapping(ByVal mappingInfo As MappingInfo)

            Dim newMappingInfo As New MappingInfo With {
                .IP = mappingInfo.IP,
                .HostName = mappingInfo.HostName,
                .Comment = mappingInfo.Comment,
                .IsDisabled = mappingInfo.IsDisabled
            }

            Using mappingForm As New MappingForm(newMappingInfo)

                mappingForm.Icon = Icon.FromHandle(My.Resources.Edit.GetHicon)
                mappingForm.Text = "Edit Mapping"

                If mappingForm.ShowDialog() = Windows.Forms.DialogResult.OK Then

                    If Not (mappingInfo.IP = newMappingInfo.IP) _
                    OrElse Not (mappingInfo.HostName.Equals(newMappingInfo.HostName, StringComparison.OrdinalIgnoreCase)) Then

                        If Me.Mappings.Exists(Function(x As MappingInfo)
                                                  Return (x.HostName.Equals(newMappingInfo.HostName, StringComparison.OrdinalIgnoreCase))
                                              End Function) Then

                            Using New CenteredMessageBox(ownerForm:=Me)
                                MessageBox.Show("This HostName  already exists in list", Me.Text,
                                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Using

                            Exit Sub

                        Else
                            Me.ToolStripButton_Save.Enabled = True

                        End If

                    Else
                        With Me.FindMappingInfo(Me.ListView_Mappings.SelectedItems(0))
                            .IP = newMappingInfo.IP
                            .HostName = newMappingInfo.HostName
                            .Comment = newMappingInfo.Comment
                            .IsDisabled = newMappingInfo.IsDisabled
                        End With

                        Me.ListView_Mappings.Items(Me.ListView_Mappings.SelectedItems(0).Index) =
                                                      New ListViewItem({If(Not newMappingInfo.IsDisabled, "Yes", "No"),
                                                                        newMappingInfo.IP,
                                                                        newMappingInfo.HostName,
                                                                        newMappingInfo.Comment})

                        Me.ToolStripButton_Save.Enabled = True

                    End If

                End If

            End Using

        End Sub

        ''' <summary>
        ''' Finds all the ocurrences of a mapping by the specified text.
        ''' </summary>
        ''' <param name="mappingSearchType">Type of the mapping search.</param>
        ''' <param name="pattern">The search pattern.</param>
        Private Sub FindMappingByText(ByVal mappingSearchType As MappingInfoSearchType,
                                      ByVal mappingSearchCondition As MappingInfoSearchCondition,
                                      ByVal pattern As String)

            Dim mappings As List(Of MappingInfo) =
                Me.Mappings.FindAll(Function(x As MappingInfo)

                                        Select Case mappingSearchType

                                            Case Main.MappingInfoSearchType.IP
                                                Select Case mappingSearchCondition

                                                    Case MappingInfoSearchCondition.StartsWithText
                                                        Return x.IP.StartsWith(pattern)

                                                    Case MappingInfoSearchCondition.ContainsText
                                                        Return x.IP.Contains(pattern)

                                                    Case Else
                                                        Return Nothing

                                                End Select

                                            Case Main.MappingInfoSearchType.HostName
                                                Select Case mappingSearchCondition

                                                    Case MappingInfoSearchCondition.StartsWithText
                                                        Return x.HostName.ToLower.StartsWith(pattern.ToLower)

                                                    Case MappingInfoSearchCondition.ContainsText
                                                        Return x.HostName.ToLower.Contains(pattern.ToLower)

                                                    Case Else
                                                        Return Nothing

                                                End Select

                                            Case Else
                                                Return Nothing

                                        End Select

                                    End Function)

            Me.AddMappingsToListview(mappings)

        End Sub

#End Region

    End Class

End Namespace

#End Region