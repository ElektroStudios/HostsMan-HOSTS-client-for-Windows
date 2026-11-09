' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="FormsTests.vb" company="Elektro Studios">
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
Imports HostsMan.UserInterface
Imports HostsMan.UserInterface.Controls
Imports HostsMan.UserInterface.Theming

Imports MainForm = HostsMan.UserInterface.Main

#End Region

#Region " FormsTests "

Namespace UserInterface

    ''' <summary>
    ''' Tests for the logic of the application forms (<see cref="MappingForm"/>, <see cref="MainForm"/> and <see cref="About"/>).
    ''' The forms are created but never shown, and no window handle is created.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class FormsTests

#Region " Initialization "

        ''' <summary>
        ''' Restores the default theme after each test (the theme manager state is shared).
        ''' </summary>
        <TestCleanup>
        Public Sub TestCleanup()

            ThemeManager.CurrentTheme = AppTheme.Light

        End Sub

#End Region

#Region " Themed Forms "

        <TestMethod>
        Public Sub AllForms_InheritFromThemedForm()

            Assert.IsTrue(GetType(ThemedForm).IsAssignableFrom(GetType(MainForm)))
            Assert.IsTrue(GetType(ThemedForm).IsAssignableFrom(GetType(MappingForm)))
            Assert.IsTrue(GetType(ThemedForm).IsAssignableFrom(GetType(About)))

        End Sub

        <TestMethod>
        Public Sub ThemedForm_ApplyPaletteWithoutHandle_SetsColorsWithoutCreatingTheHandle()

            Using form As New ThemedForm()

                form.ApplyPalette(ThemePalette.Dark)

                Assert.AreEqual(ThemePalette.Dark.WindowBack, form.BackColor)
                Assert.AreEqual(ThemePalette.Dark.WindowFore, form.ForeColor)
                Assert.IsFalse(form.IsHandleCreated)
                Assert.IsFalse(form.IsDarkTitleBarActive)

                form.ApplyPalette(ThemePalette.Light)

                Assert.AreEqual(SystemColors.Control, form.BackColor)
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() form.ApplyPalette(Nothing))

            End Using

        End Sub

#End Region

#Region " MappingForm "

        <TestMethod>
        Public Sub MappingForm_NothingMapping_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Call New MappingForm(Nothing).Dispose())

        End Sub

        <TestMethod>
        Public Sub MappingForm_NewMapping_ShowsTheDefaultsAndCannotBeConfirmed()

            Using form As New MappingForm(New MappingInfo())

                Assert.AreEqual(MappingForm.DefaultIPAddress, form.TextBox_IP.Text)
                Assert.AreEqual(String.Empty, form.TextBox_HostName.Text)
                Assert.IsTrue(form.RadioButton_Enabled.Checked)
                Assert.IsFalse(form.RadioButton_Disabled.Checked)
                Assert.IsFalse(form.Button_Confirm.Enabled, "A mapping without hostname must not be confirmable.")

            End Using

        End Sub

        <TestMethod>
        Public Sub MappingForm_ExistingIPv6Mapping_IsLoadedIntoTheControls()

            ' Regression: the IP address was split into four IPv4 octet boxes, so editing an IPv6 mapping threw IndexOutOfRangeException.
            Dim mapping As New MappingInfo("2001:db8::1", "ipv6.example.com", "note", isDisabled:=True)

            Using form As New MappingForm(mapping)

                Assert.AreEqual("2001:db8::1", form.TextBox_IP.Text)
                Assert.AreEqual("ipv6.example.com", form.TextBox_HostName.Text)
                Assert.AreEqual("note", form.TextBox_Comment.Text)
                Assert.IsTrue(form.RadioButton_Disabled.Checked)
                Assert.IsFalse(form.RadioButton_Enabled.Checked)
                Assert.IsTrue(form.Button_Confirm.Enabled)

            End Using

        End Sub

        <TestMethod>
        <DataRow("300.1.1.1", "example.com", "")>
        <DataRow("127.0.0", "example.com", "")>
        <DataRow("127.0.0.1", "two words", "")>
        <DataRow("127.0.0.1", "hash#host", "")>
        <DataRow("127.0.0.1", "", "")>
        <DataRow("", "example.com", "")>
        <DataRow("127.0.0.1", "example.com", "multi" & vbCrLf & "line")>
        Public Sub MappingForm_InvalidInput_DisablesTheConfirmButton(ByVal ip As String, ByVal hostName As String, ByVal comment As String)

            ' Regression: the old masked IP boxes accepted octets greater than 255 (e.g. "300"),
            ' and the hostname was not validated, which made the save operation fail later.
            Using form As New MappingForm(New MappingInfo())

                form.TextBox_IP.Text = ip
                form.TextBox_HostName.Text = hostName
                form.TextBox_Comment.Text = comment

                Assert.IsFalse(form.IsInputValid)
                Assert.IsFalse(form.Button_Confirm.Enabled)
                Assert.ThrowsExactly(Of InvalidOperationException)(Sub() form.CommitMapping())

            End Using

        End Sub

        <TestMethod>
        Public Sub MappingForm_InvalidIPAddress_IsHighlightedWithTheErrorColor()

            ThemeManager.CurrentTheme = AppTheme.Dark

            Using form As New MappingForm(New MappingInfo())

                form.TextBox_IP.Text = "999.0.0.1"
                Assert.AreEqual(ThemePalette.Dark.ErrorFore, form.TextBox_IP.ForeColor)

                form.TextBox_IP.Text = "10.0.0.1"
                Assert.AreEqual(ThemePalette.Dark.InputFore, form.TextBox_IP.ForeColor)

            End Using

        End Sub

        <TestMethod>
        Public Sub MappingForm_CommitMapping_CopiesTheTrimmedValuesAndReturnsOK()

            Dim mapping As New MappingInfo()

            Using form As New MappingForm(mapping)

                form.TextBox_IP.Text = "  0.0.0.0 "
                form.TextBox_HostName.Text = " ads.example.com  "
                form.TextBox_Comment.Text = "  Blocked  "
                form.RadioButton_Disabled.Checked = True

                Assert.IsTrue(form.Button_Confirm.Enabled)
                form.CommitMapping()

                Assert.AreEqual(DialogResult.OK, form.DialogResult)
                Assert.AreSame(mapping, form.MappingResult)

            End Using

            Assert.AreEqual("0.0.0.0", mapping.IP)
            Assert.AreEqual("ads.example.com", mapping.HostName)
            Assert.AreEqual("Blocked", mapping.Comment)
            Assert.IsTrue(mapping.IsDisabled)

        End Sub

        <TestMethod>
        Public Sub MappingForm_Buttons_HaveTextImageAndSmallerFont()

            Using form As New MappingForm(New MappingInfo())

                Assert.AreEqual("OK", form.Button_Confirm.Text)
                Assert.AreEqual("Cancel", form.Button_Cancel.Text)

                For Each button As Button In {form.Button_Confirm, form.Button_Cancel}
                    Assert.IsNotNull(button.Image, button.Name)
                    Assert.AreEqual(button.Image.Width, button.Image.Height, button.Name)
                    Assert.AreEqual(MappingForm.GetButtonImageSize(button.Font), button.Image.Width, button.Name)
                    Assert.IsTrue(button.Image.Width > button.Font.Height, button.Name)
                    Assert.AreEqual(TextImageRelation.ImageBeforeText, button.TextImageRelation, button.Name)
                    Assert.AreEqual(12.75F, button.Font.SizeInPoints, 0.01F, button.Name)
                    Assert.IsTrue(button.Image.Height < button.Height, button.Name)
                Next button

            End Using

        End Sub

        <TestMethod>
        Public Sub MappingForm_Dispose_DisposesTheButtonImages()

            Dim form As New MappingForm(New MappingInfo())
            Dim okImage As Image = form.Button_Confirm.Image

            form.Dispose()

            Assert.IsNull(form.Button_Confirm.Image)
            Assert.IsNull(form.Button_Cancel.Image)
            Assert.ThrowsExactly(Of ArgumentException)(Sub() okImage.GetBounds(GraphicsUnit.Pixel))

        End Sub

        <TestMethod>
        Public Sub MappingForm_EnterAndEscapeKeys_AreWiredToTheButtons()

            Using form As New MappingForm(New MappingInfo())
                Assert.AreSame(form.Button_Confirm, form.AcceptButton)
                Assert.AreSame(form.Button_Cancel, form.CancelButton)
            End Using

        End Sub

#End Region

#Region " Main "

        <TestMethod>
        Public Sub Main_CreateListViewItem_ShowsTheMappingAndStoresItInTheTag()

            Dim mapping As New MappingInfo("127.0.0.1", "example.com", "note", isDisabled:=False)

            Dim item As ListViewItem = MainForm.CreateListViewItem(mapping)

            Assert.AreSame(mapping, item.Tag)
            CollectionAssert.AreEqual({"Yes", "127.0.0.1", "example.com", "note"},
                                      item.SubItems.Cast(Of ListViewItem.ListViewSubItem).Select(Function(s As ListViewItem.ListViewSubItem) s.Text).ToArray())

        End Sub

        <TestMethod>
        Public Sub Main_CreateListViewItem_DisabledMappingWithoutComment()

            Dim mapping As New MappingInfo With {.IP = "0.0.0.0", .HostName = "example.com", .Comment = Nothing, .IsDisabled = True}

            Dim item As ListViewItem = MainForm.CreateListViewItem(mapping)

            Assert.AreEqual("No", item.SubItems(0).Text)
            Assert.AreEqual(String.Empty, item.SubItems(3).Text)

        End Sub

        <TestMethod>
        Public Sub Main_UpdateListViewItem_ReflectsTheMappingChanges()

            Dim mapping As New MappingInfo("127.0.0.1", "example.com")
            Dim item As ListViewItem = MainForm.CreateListViewItem(mapping)

            mapping.IP = "::1"
            mapping.HostName = "changed.com"
            mapping.IsDisabled = True
            MainForm.UpdateListViewItem(item)

            Assert.AreEqual("No", item.SubItems(0).Text)
            Assert.AreEqual("::1", item.SubItems(1).Text)
            Assert.AreEqual("changed.com", item.SubItems(2).Text)

        End Sub

        <TestMethod>
        Public Sub Main_GetMapping_ReturnsTheTaggedInstanceEvenForDuplicatedTexts()

            ' Regression: items were matched with their mapping by comparing the IP and hostname texts,
            ' so two identical entries always resolved to the first mapping.
            Dim first As New MappingInfo("127.0.0.1", "example.com")
            Dim second As New MappingInfo("127.0.0.1", "example.com")

            Assert.AreSame(first, MainForm.GetMapping(MainForm.CreateListViewItem(first)))
            Assert.AreSame(second, MainForm.GetMapping(MainForm.CreateListViewItem(second)))

        End Sub

        <TestMethod>
        Public Sub Main_GetMapping_InvalidItems_Throw()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MainForm.GetMapping(Nothing))
            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() MainForm.GetMapping(New ListViewItem("no tag")))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MainForm.CreateListViewItem(Nothing))

        End Sub

        <TestMethod>
        <DataRow("example.com", "http://example.com")>
        <DataRow("localhost", "http://localhost")>
        <DataRow("https://secure.example.com", "https://secure.example.com")>
        Public Sub Main_GetNavigationUrl_PrependsTheSchemeWhenMissing(ByVal hostName As String, ByVal expected As String)

            Assert.AreEqual(expected, MainForm.GetNavigationUrl(hostName))

        End Sub

        <TestMethod>
        Public Sub Main_GetNavigationUrl_EmptyHostName_ThrowsArgumentException()

            Assert.ThrowsExactly(Of ArgumentException)(Sub() MainForm.GetNavigationUrl(String.Empty))

        End Sub

        <TestMethod>
        Public Sub Main_Constructor_ThemeSwitchReflectsTheCurrentTheme()

            ThemeManager.CurrentTheme = AppTheme.Dark

            Using form As New MainForm()
                Assert.IsTrue(form.ToggleSwitch_DarkTheme.Checked)
                Assert.IsInstanceOfType(Of ThemedListView)(form.ListView_Mappings)
                Assert.IsFalse(form.IsHandleCreated)
            End Using

            ThemeManager.CurrentTheme = AppTheme.Light

            Using form As New MainForm()
                Assert.IsFalse(form.ToggleSwitch_DarkTheme.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub Main_GetWindowTitle_UsesTheTitleAndAThreeFieldsVersion()

            Assert.AreEqual("HostsMan 1.2.0", MainForm.GetWindowTitle("HostsMan", New Version(1, 2, 0, 0)))
            Assert.AreEqual("HostsMan 1.2.3", MainForm.GetWindowTitle("HostsMan", New Version(1, 2, 3, 4)))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MainForm.GetWindowTitle(Nothing, New Version(1, 0)))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() MainForm.GetWindowTitle("HostsMan", Nothing))

        End Sub

        <TestMethod>
        Public Sub Main_SearchTypeComboBox_ItemsMatchTheSearchTypes()

            Using form As New MainForm()

                Dim items As String() = form.ComboBox_SearchType.Items.Cast(Of String).ToArray()

                Assert.AreEqual([Enum].GetValues(GetType(MappingSearchType)).Length, items.Length)
                Assert.AreEqual("IP address", items(MappingSearchType.IP))
                Assert.AreEqual("Hostname", items(MappingSearchType.HostName))
                Assert.AreEqual("Comment", items(MappingSearchType.Comment))
                Assert.AreEqual([Enum].GetValues(GetType(MappingSearchCondition)).Length, form.ComboBox_SearchCondition.Items.Count)
                Assert.IsInstanceOfType(Of ThemedComboBox)(form.ComboBox_SearchType)
                Assert.IsInstanceOfType(Of ThemedComboBox)(form.ComboBox_SearchCondition)

            End Using

        End Sub

        <TestMethod>
        Public Sub Main_ListViewFont_IsOnePixelBiggerThanTheFormFont()

            Using form As New MainForm()

                ' At 96 DPI: 8.25 pt = 11 px (form font) and 9 pt = 12 px (list font).
                Dim formPixels As Single = form.Font.SizeInPoints * 96.0F / 72.0F
                Dim listPixels As Single = form.ListView_Mappings.Font.SizeInPoints * 96.0F / 72.0F

                Assert.AreEqual(formPixels + 1.0F, listPixels, 0.01F)

            End Using

        End Sub

        <TestMethod>
        Public Sub Main_ThemeSwitchCaption_ShowsTheCurrentTheme()

            Using form As New MainForm()

                Assert.AreEqual("Light theme", form.ToggleSwitch_DarkTheme.DisplayText)
                Assert.AreEqual("Dark theme", form.ToggleSwitch_DarkTheme.CheckedText)

            End Using

        End Sub

        <TestMethod>
        Public Sub Main_SearchPanelLayout_ThemeSwitchDoesNotOverlapTheSearchBox()

            Using form As New MainForm()

                Dim searchBox As Rectangle = form.TextBox_Search.Bounds
                Dim toggle As Rectangle = form.ToggleSwitch_DarkTheme.Bounds

                Assert.AreSame(form.Panel_Search, form.ToggleSwitch_DarkTheme.Parent)
                Assert.IsFalse(searchBox.IntersectsWith(toggle))
                Assert.IsTrue(toggle.Right <= form.Panel_Search.ClientSize.Width)
                Assert.AreEqual(AnchorStyles.Top Or AnchorStyles.Right, form.ToggleSwitch_DarkTheme.Anchor)

            End Using

        End Sub

#End Region

#Region " About "

        <TestMethod>
        Public Sub About_GetProductTitle_UsesTheMajorAndMinorVersion()

            Assert.AreEqual("HostsMan v1.2", About.GetProductTitle(New Version(1, 2, 3, 4)))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() About.GetProductTitle(Nothing))

        End Sub

        <TestMethod>
        Public Sub About_Constructor_ShowsTheAssemblyVersion()

            ' Regression: the version label was hard-coded ("HostsMan v1.1") and got outdated.
            Dim expected As String = About.GetProductTitle(GetType(About).Assembly.GetName().Version)

            Using form As New About()
                Assert.AreEqual(expected, form.Label4.Text)
            End Using

        End Sub

#End Region

    End Class

End Namespace

#End Region
