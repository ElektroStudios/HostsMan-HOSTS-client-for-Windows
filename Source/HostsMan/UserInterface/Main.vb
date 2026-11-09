' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
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

Imports System.ComponentModel
Imports System.IO
Imports System.Security

Imports HostsMan.Tools
Imports HostsMan.Tools.HostsTools
Imports HostsMan.Tools.ListViewColumnSorter
Imports HostsMan.UserInterface.Theming

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
        Private ReadOnly windowSticker As New WindowSticker(clientForm:=Me) With {.SnapMargin = 35}

        ''' <summary>
        ''' Gets the hosts object.
        ''' </summary>
        ''' <value>The hosts object.</value>
        Private ReadOnly Property HostsObj As HostsTools = New HostsTools()

        ''' <summary>
        ''' Gets or sets the Hosts mappings being edited (the source of truth of the list view).
        ''' </summary>
        ''' <value>The Hosts mappings.</value>
        Private Property Mappings As New List(Of MappingInfo)

        ''' <summary>
        ''' The ListViewColumnSorter instance.
        ''' </summary>
        Private ReadOnly columnSorter As New ListViewColumnSorter()

        ''' <summary>
        ''' Gets or sets the mapping search type.
        ''' </summary>
        ''' <value>The mapping search type.</value>
        Private Property CurrentSearchType As MappingSearchType = MappingSearchType.HostName

        ''' <summary>
        ''' Gets or sets the mapping search condition.
        ''' </summary>
        ''' <value>The mapping search condition.</value>
        Private Property CurrentSearchCondition As MappingSearchCondition = MappingSearchCondition.ContainsText

        ''' <summary>
        ''' A value indicating whether the Hosts file was successfully loaded.
        ''' Editing is not allowed when the file could not be loaded, to avoid overwriting it with an empty list.
        ''' </summary>
        Private isHostsFileLoaded As Boolean

        ''' <summary>
        ''' A value indicating whether there are changes that have not been saved to the Hosts file.
        ''' </summary>
        Private hasUnsavedChanges As Boolean

        ''' <summary>
        ''' Gets the Windows Directory path.
        ''' </summary>
        ''' <value>The Windows Directory path.</value>
        Private Shared ReadOnly Property WinDirPath As String
            Get
                Return Environment.GetFolderPath(Environment.SpecialFolder.Windows)
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="Main"/> class.
        ''' </summary>
        Public Sub New()

            ' This call is required by the designer.
            Me.InitializeComponent()

            ' Show the program name and version in the title bar.
            Me.Text = Main.GetWindowTitle(My.Application.Info.Title, My.Application.Info.Version)

            ' Reflect the current theme in the theme switch.
            Me.ToggleSwitch_DarkTheme.Checked = (ThemeManager.CurrentTheme = AppTheme.Dark)

        End Sub

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

        ''' <summary>
        ''' Handles the FormClosing event of the Main Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        Private Sub Main_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) _
        Handles MyBase.FormClosing

            If Not Me.hasUnsavedChanges Then
                Exit Sub
            End If

            Dim answer As DialogResult
            Using New CenteredMessageBox(ownerForm:=Me)
                answer = MessageBox.Show(Me, "There are unsaved changes. Do you want to save them to the hosts file before closing?",
                                         Me.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
            End Using

            Select Case answer
                Case DialogResult.Yes
                    e.Cancel = Not Me.SaveHostsFile()
                Case DialogResult.Cancel
                    e.Cancel = True
            End Select

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

            Dim mappingInfo As New MappingInfo()

            If Not Me.ShowMappingForm(mappingInfo, "Add Mapping", My.Resources.Add) Then
                Exit Sub
            End If

            If Me.Mappings.Exists(Function(x As MappingInfo) x.ConflictsWith(mappingInfo)) Then
                Me.ShowError($"The hostname '{mappingInfo.HostName}' is already mapped to another address of the same type (IPv4 or IPv6).")
                Exit Sub
            End If

            Me.Mappings.Add(mappingInfo)
            Me.RefreshListView()
            Me.MarkAsModified()

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Remove control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Remove_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Remove.Click

            Me.ListView_Mappings.BeginUpdate()
            Try
                For Each item As ListViewItem In Me.ListView_Mappings.SelectedItems.Cast(Of ListViewItem).ToArray()
                    Me.Mappings.Remove(Main.GetMapping(item))
                    Me.ListView_Mappings.Items.Remove(item)
                Next item

            Finally
                Me.ListView_Mappings.EndUpdate()

            End Try

            Me.MarkAsModified()

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Enable control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Enable_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Enable.Click

            Me.SetSelectedMappingsDisabled(isDisabled:=False)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Disable control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Disable_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Disable.Click

            Me.SetSelectedMappingsDisabled(isDisabled:=True)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Edit control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Edit_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Edit.Click

            If Me.ListView_Mappings.SelectedItems.Count = 1 Then
                Me.EditMapping(Me.ListView_Mappings.SelectedItems(0))
            End If

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Rescan control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Rescan_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Rescan.Click

            If Me.hasUnsavedChanges AndAlso
               Not Me.Confirm("The unsaved changes will be lost. Do you want to reload the hosts file anyway?") Then
                Exit Sub
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

            If Not Me.Confirm("All the mappings will be removed from the list. Do you want to continue?") Then
                Exit Sub
            End If

            Me.Mappings.Clear()
            Me.RefreshListView()
            Me.MarkAsModified()

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Save control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Save_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Save.Click

            Me.SaveHostsFile()

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_Directory control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_Directory_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_Directory.Click

            Try
                Using Process.Start(Path.Combine(Main.WinDirPath, "Explorer.exe"), $"/Select,""{Me.HostsObj.HostsLocation}""")
                End Using

            Catch ex As Win32Exception
                Me.ShowError(ex.Message)

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
                Me.HostsObj.FileOpen(Path.Combine(Main.WinDirPath, "Notepad.exe"))

            Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is Win32Exception
                Me.ShowError(ex.Message)

            End Try

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripButton_About control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripButton_About_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripButton_About.Click

            Using aboutform As New About()
                aboutform.ShowDialog(Me)
            End Using

        End Sub

#End Region

#Region " Search Panel "

        ''' <summary>
        ''' Handles the SelectedIndexChanged event of the ComboBox_SearchType control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ComboBox_SearchType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ComboBox_SearchType.SelectedIndexChanged

            Dim index As Integer = DirectCast(sender, ComboBox).SelectedIndex
            If Not [Enum].IsDefined(GetType(MappingSearchType), index) Then
                Throw New InvalidEnumArgumentException(NameOf(index), index, GetType(MappingSearchType))
            End If

            Me.CurrentSearchType = CType(index, MappingSearchType)
            Me.RefreshListView()

        End Sub

        ''' <summary>
        ''' Handles the SelectedIndexChanged event of the ComboBox_SearchCondition control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ComboBox_SearchCondition_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ComboBox_SearchCondition.SelectedIndexChanged

            Dim index As Integer = DirectCast(sender, ComboBox).SelectedIndex
            If Not [Enum].IsDefined(GetType(MappingSearchCondition), index) Then
                Throw New InvalidEnumArgumentException(NameOf(index), index, GetType(MappingSearchCondition))
            End If

            Me.CurrentSearchCondition = CType(index, MappingSearchCondition)
            Me.RefreshListView()

        End Sub

        ''' <summary>
        ''' Handles the TextChanged event of the TextBox_Search control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub TextBox_Search_TextChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles TextBox_Search.TextChanged

            Me.RefreshListView()

        End Sub

        ''' <summary>
        ''' Handles the CheckedChanged event of the ToggleSwitch_DarkTheme control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToggleSwitch_DarkTheme_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToggleSwitch_DarkTheme.CheckedChanged

            Dim newTheme As AppTheme = If(DirectCast(sender, Global.HostsMan.UserInterface.Controls.ToggleSwitch).Checked, AppTheme.Dark, AppTheme.Light)
            If newTheme = ThemeManager.CurrentTheme Then
                Exit Sub
            End If

            ThemeManager.CurrentTheme = newTheme

            My.Settings.AppTheme = newTheme
            My.Settings.Save()

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

            Me.UpdateToolStripButtonStates()

        End Sub

        ''' <summary>
        ''' Handles the MouseDoubleClick event of the ListView_Mappings control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        Private Sub ListView_Mappings_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) _
        Handles ListView_Mappings.MouseDoubleClick

            If e.Button = MouseButtons.Left AndAlso
               Me.isHostsFileLoaded AndAlso
               Me.ListView_Mappings.SelectedItems.Count = 1 Then

                Me.EditMapping(Me.ListView_Mappings.SelectedItems(0))

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

            ' Dynamically sets the sort-modifier to sort the column by text, number, or date.
            Me.columnSorter.SortModifier = DirectCast(lv.Columns(e.Column).Tag, SortModifiers)

            ' Determine whether clicked column is already the column that is being sorted.
            If e.Column = Me.columnSorter.Column Then
                ' Reverse the current sort direction for this column.
                Me.columnSorter.Order = If(Me.columnSorter.Order = SortOrder.Ascending, SortOrder.Descending, SortOrder.Ascending)

            Else
                ' Set the column number that is to be sorted, default to ascending.
                Me.columnSorter.Column = e.Column
                Me.columnSorter.Order = SortOrder.Ascending

            End If

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

            e.Cancel = Me.ListView_Mappings.SelectedItems.Count <> 1

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripMenuItem_CopyIP control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripMenuItem_CopyIP_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripMenuItem_CopyIP.Click

            Clipboard.SetText(Main.GetMapping(Me.ListView_Mappings.SelectedItems(0)).IP)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripMenuItem_CopyHostname control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripMenuItem_CopyHostname_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripMenuItem_CopyHostname.Click

            Clipboard.SetText(Main.GetMapping(Me.ListView_Mappings.SelectedItems(0)).HostName)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the ToolStripMenuItem_Navigate control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ToolStripMenuItem_Navigate_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ToolStripMenuItem_Navigate.Click

            Dim url As String = Main.GetNavigationUrl(Main.GetMapping(Me.ListView_Mappings.SelectedItems(0)).HostName)

            Try
                Using Process.Start(url)
                End Using

            Catch ex As Win32Exception
                Me.ShowError(ex.Message)

            End Try

        End Sub

#End Region

#End Region

#Region " Friend Shared Methods "

        ''' <summary>
        ''' Creates the <see cref="ListViewItem"/> that represents the specified mapping.
        ''' The mapping is stored in the <see cref="ListViewItem.Tag"/> property.
        ''' </summary>
        ''' <param name="mappingInfo">The mapping.</param>
        ''' <returns>The <see cref="ListViewItem"/>.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="mappingInfo"/> is <see langword="Nothing"/>.</exception>
        Friend Shared Function CreateListViewItem(ByVal mappingInfo As MappingInfo) As ListViewItem

            If mappingInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappingInfo))
            End If

            Dim item As New ListViewItem({String.Empty, String.Empty, String.Empty, String.Empty}) With {.Tag = mappingInfo}
            Main.UpdateListViewItem(item)
            Return item

        End Function

        ''' <summary>
        ''' Updates the texts of the specified <see cref="ListViewItem"/> from the mapping stored in its <see cref="ListViewItem.Tag"/>.
        ''' </summary>
        ''' <param name="item">The list view item.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidOperationException">The item does not contain a mapping.</exception>
        Friend Shared Sub UpdateListViewItem(ByVal item As ListViewItem)

            Dim mappingInfo As MappingInfo = Main.GetMapping(item)

            item.SubItems(0).Text = If(mappingInfo.IsDisabled, "No", "Yes")
            item.SubItems(1).Text = mappingInfo.IP
            item.SubItems(2).Text = mappingInfo.HostName
            item.SubItems(3).Text = If(mappingInfo.Comment, String.Empty)

        End Sub

        ''' <summary>
        ''' Gets the mapping represented by the specified <see cref="ListViewItem"/>.
        ''' </summary>
        ''' <param name="item">The list view item.</param>
        ''' <returns>The mapping.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidOperationException">The item does not contain a mapping.</exception>
        Friend Shared Function GetMapping(ByVal item As ListViewItem) As MappingInfo

            If item Is Nothing Then
                Throw New ArgumentNullException(NameOf(item))
            End If

            Dim mappingInfo As MappingInfo = TryCast(item.Tag, MappingInfo)
            If mappingInfo Is Nothing Then
                Throw New InvalidOperationException("The list view item does not contain a mapping.")
            End If

            Return mappingInfo

        End Function

        ''' <summary>
        ''' Gets the text of the window title bar ("{Title} {Major}.{Minor}.{Build}").
        ''' </summary>
        ''' <param name="title">The program title.</param>
        ''' <param name="version">The program version.</param>
        ''' <returns>The window title.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="title"/> or <paramref name="version"/> is <see langword="Nothing"/>.</exception>
        Friend Shared Function GetWindowTitle(ByVal title As String, ByVal version As Version) As String

            If title Is Nothing Then
                Throw New ArgumentNullException(NameOf(title))
            End If

            If version Is Nothing Then
                Throw New ArgumentNullException(NameOf(version))
            End If

            Return $"{title} {version.ToString(3)}"

        End Function

        ''' <summary>
        ''' Gets the URL to navigate to the specified hostname.
        ''' </summary>
        ''' <param name="hostName">The hostname.</param>
        ''' <returns>The URL ("http://" is prepended if the hostname has no scheme).</returns>
        ''' <exception cref="ArgumentException"><paramref name="hostName"/> is <see langword="Nothing"/> or empty.</exception>
        Friend Shared Function GetNavigationUrl(ByVal hostName As String) As String

            If String.IsNullOrEmpty(hostName) Then
                Throw New ArgumentException("The hostname cannot be null or empty.", NameOf(hostName))
            End If

            Return If(hostName.Contains("://"), hostName, $"http://{hostName}")

        End Function

#End Region

#Region " Private Methods "

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

            Me.ComboBox_SearchType.SelectedIndex = MappingSearchType.HostName
            Me.ComboBox_SearchCondition.SelectedIndex = MappingSearchCondition.ContainsText

        End Sub

        ''' <summary>
        ''' Loads the hosts file (creating it if it does not exist).
        ''' </summary>
        Private Sub LoadHostsFile()

            Try
                If Not Me.HostsObj.FileExists() Then
                    Me.HostsObj.FileCreate()
                End If

                Me.Mappings = Me.HostsObj.GetMappings()
                Me.isHostsFileLoaded = True

            Catch ex As Exception When TypeOf ex Is IOException OrElse
                                       TypeOf ex Is UnauthorizedAccessException OrElse
                                       TypeOf ex Is SecurityException

                Me.Mappings = New List(Of MappingInfo)
                Me.isHostsFileLoaded = False
                Me.ShowError($"The hosts file could not be loaded:{Environment.NewLine}{Environment.NewLine}{ex.Message}")

            End Try

            Me.hasUnsavedChanges = False
            Me.RefreshListView()

        End Sub

        ''' <summary>
        ''' Saves the mappings to the Hosts file.
        ''' <para></para>
        ''' The mappings are written to a temporary file in the same directory than the Hosts file,
        ''' which then atomically replaces the Hosts file (preserving its attributes and permissions).
        ''' </summary>
        ''' <returns><see langword="True"/> if the Hosts file was saved; otherwise, <see langword="False"/>.</returns>
        Private Function SaveHostsFile() As Boolean

            Dim hostsPath As String = Me.HostsObj.HostsLocation
            Dim tempPath As String = Path.Combine(Path.GetDirectoryName(hostsPath), $"{Path.GetFileName(hostsPath)}.{Guid.NewGuid():N}.tmp")

            Try
                Dim writer As New HostsTools(tempPath) With {.HostsEncoding = Me.HostsObj.HostsEncoding}
                writer.WriteMappings(Me.Mappings)

                If File.Exists(hostsPath) Then
                    File.Replace(tempPath, hostsPath, Nothing)
                Else
                    File.Move(tempPath, hostsPath)
                End If

                Me.MarkAsSaved()
                Return True

            Catch ex As UnauthorizedAccessException
                Me.ShowError($"{ex.Message}{Environment.NewLine}{Environment.NewLine}Administrator rights are required to modify the hosts file. Run HostsMan as administrator and try again.")
                Return False

            Catch ex As Exception When TypeOf ex Is IOException OrElse
                                       TypeOf ex Is SecurityException OrElse
                                       TypeOf ex Is ArgumentException OrElse
                                       TypeOf ex Is FormatException
                Me.ShowError(ex.Message)
                Return False

            Finally
                If File.Exists(tempPath) Then
                    File.Delete(tempPath)
                End If

            End Try

        End Function

        ''' <summary>
        ''' Rebuilds the list view items from <see cref="Mappings"/>, applying the current search filter
        ''' and preserving the current selection.
        ''' </summary>
        Private Sub RefreshListView()

            Dim selected As New HashSet(Of MappingInfo)(
                Me.ListView_Mappings.SelectedItems.Cast(Of ListViewItem).Select(Function(item As ListViewItem) Main.GetMapping(item)))

            Dim visibleMappings As List(Of MappingInfo) =
                MappingSearch.Filter(Me.Mappings, Me.CurrentSearchType, Me.CurrentSearchCondition, Me.TextBox_Search.Text)

            Dim items As ListViewItem() = visibleMappings.Select(Function(m As MappingInfo) Main.CreateListViewItem(m)).ToArray()
            For Each item As ListViewItem In items
                item.Selected = selected.Contains(Main.GetMapping(item))
            Next item

            Me.ListView_Mappings.BeginUpdate()
            Try
                Me.ListView_Mappings.Items.Clear()
                Me.ListView_Mappings.Items.AddRange(items)

            Finally
                Me.ListView_Mappings.EndUpdate()

            End Try

            Me.UpdateToolStripButtonStates()

        End Sub

        ''' <summary>
        ''' Updates the enabled state of the tool strip buttons according to the current state.
        ''' </summary>
        Private Sub UpdateToolStripButtonStates()

            Dim selectedMappings As List(Of MappingInfo) =
                Me.ListView_Mappings.SelectedItems.Cast(Of ListViewItem).Select(Function(item As ListViewItem) Main.GetMapping(item)).ToList()

            Dim canEdit As Boolean = Me.isHostsFileLoaded

            Me.ToolStripButton_Add.Enabled = canEdit
            Me.ToolStripButton_Remove.Enabled = canEdit AndAlso selectedMappings.Count <> 0
            Me.ToolStripButton_Enable.Enabled = canEdit AndAlso selectedMappings.Exists(Function(m As MappingInfo) m.IsDisabled)
            Me.ToolStripButton_Disable.Enabled = canEdit AndAlso selectedMappings.Exists(Function(m As MappingInfo) Not m.IsDisabled)
            Me.ToolStripButton_Edit.Enabled = canEdit AndAlso selectedMappings.Count = 1
            Me.ToolStripButton_Clean.Enabled = canEdit AndAlso Me.Mappings.Count <> 0
            Me.ToolStripButton_Save.Enabled = canEdit AndAlso Me.hasUnsavedChanges

        End Sub

        ''' <summary>
        ''' Enables or disables the selected mappings.
        ''' </summary>
        ''' <param name="isDisabled"><see langword="True"/> to disable the selected mappings; <see langword="False"/> to enable them.</param>
        Private Sub SetSelectedMappingsDisabled(ByVal isDisabled As Boolean)

            For Each item As ListViewItem In Me.ListView_Mappings.SelectedItems
                Main.GetMapping(item).IsDisabled = isDisabled
                Main.UpdateListViewItem(item)
            Next item

            Me.MarkAsModified()

        End Sub

        ''' <summary>
        ''' Edits the mapping represented by the specified list view item.
        ''' </summary>
        ''' <param name="item">The list view item.</param>
        Private Sub EditMapping(ByVal item As ListViewItem)

            Dim mappingInfo As MappingInfo = Main.GetMapping(item)
            Dim editedMappingInfo As MappingInfo = mappingInfo.Clone()

            If Not Me.ShowMappingForm(editedMappingInfo, "Edit Mapping", My.Resources.Edit) Then
                Exit Sub
            End If

            If Me.Mappings.Exists(Function(x As MappingInfo) x IsNot mappingInfo AndAlso x.ConflictsWith(editedMappingInfo)) Then
                Me.ShowError($"The hostname '{editedMappingInfo.HostName}' is already mapped to another address of the same type (IPv4 or IPv6).")
                Exit Sub
            End If

            mappingInfo.IP = editedMappingInfo.IP
            mappingInfo.HostName = editedMappingInfo.HostName
            mappingInfo.Comment = editedMappingInfo.Comment
            mappingInfo.IsDisabled = editedMappingInfo.IsDisabled

            Me.RefreshListView()
            Me.MarkAsModified()

        End Sub

        ''' <summary>
        ''' Shows the <see cref="MappingForm"/> to add or edit a mapping.
        ''' </summary>
        ''' <param name="mappingInfo">The mapping that receives the result.</param>
        ''' <param name="title">The title of the form.</param>
        ''' <param name="iconImage">The image used as the icon of the form. It is disposed by this method.</param>
        ''' <returns><see langword="True"/> if the user confirmed the mapping; otherwise, <see langword="False"/>.</returns>
        Private Function ShowMappingForm(ByVal mappingInfo As MappingInfo, ByVal title As String, ByVal iconImage As Bitmap) As Boolean

            Using iconImage
                Using formIcon As Icon = IconHelper.CreateIcon(iconImage),
                      mappingForm As New MappingForm(mappingInfo)

                    mappingForm.Icon = formIcon
                    mappingForm.Text = title
                    Return mappingForm.ShowDialog(Me) = DialogResult.OK

                End Using
            End Using

        End Function

        ''' <summary>
        ''' Marks the mappings as modified (enables the save button).
        ''' </summary>
        Private Sub MarkAsModified()

            Me.hasUnsavedChanges = True
            Me.UpdateToolStripButtonStates()

        End Sub

        ''' <summary>
        ''' Marks the mappings as saved (disables the save button).
        ''' </summary>
        Private Sub MarkAsSaved()

            Me.hasUnsavedChanges = False
            Me.UpdateToolStripButtonStates()

        End Sub

        ''' <summary>
        ''' Shows an error message box centered on this form.
        ''' </summary>
        ''' <param name="message">The error message.</param>
        Private Sub ShowError(ByVal message As String)

            Using New CenteredMessageBox(ownerForm:=Me)
                MessageBox.Show(Me, message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using

        End Sub

        ''' <summary>
        ''' Shows a Yes/No question message box centered on this form.
        ''' </summary>
        ''' <param name="message">The question.</param>
        ''' <returns><see langword="True"/> if the user answered Yes; otherwise, <see langword="False"/>.</returns>
        Private Function Confirm(ByVal message As String) As Boolean

            Using New CenteredMessageBox(ownerForm:=Me)
                Return MessageBox.Show(Me, message, Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
            End Using

        End Function

#End Region

    End Class

End Namespace

#End Region
