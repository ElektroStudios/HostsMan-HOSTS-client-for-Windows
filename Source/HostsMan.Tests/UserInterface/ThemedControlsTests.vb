' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemedControlsTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports HostsMan.UserInterface.Controls
Imports HostsMan.UserInterface.Theming

#End Region

#Region " ThemedControlsTests "

Namespace UserInterface

    ''' <summary>
    ''' Tests for the themed custom controls: <see cref="ThemedListView"/>, <see cref="ToggleSwitch"/>,
    ''' <see cref="ThemedToolStripRenderer"/> and <see cref="ThemedColorTable"/>.
    ''' No window handle is created by these tests.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class ThemedControlsTests

#Region " Helpers "

        ''' <summary>
        ''' A <see cref="ToggleSwitch"/> that exposes its protected input methods.
        ''' </summary>
        Private NotInheritable Class TestableToggleSwitch : Inherits ToggleSwitch

            ''' <summary>
            ''' Simulates a mouse click.
            ''' </summary>
            Friend Sub SimulateClick()
                Me.OnClick(EventArgs.Empty)
            End Sub

            ''' <summary>
            ''' Simulates the release of a key.
            ''' </summary>
            ''' <param name="keyData">The key and modifiers.</param>
            ''' <returns>A value indicating whether the key was handled.</returns>
            Friend Function SimulateKeyUp(ByVal keyData As Keys) As Boolean
                Dim e As New KeyEventArgs(keyData)
                Me.OnKeyUp(e)
                Return e.Handled
            End Function

        End Class

#End Region

#Region " ThemedListView "

        <TestMethod>
        Public Sub ThemedListView_DefaultState_IsNativeLightRendering()

            Using lv As New ThemedListView()
                Assert.AreSame(ThemePalette.Light, lv.Palette)
                Assert.IsFalse(lv.OwnerDraw)
                Assert.IsFalse(lv.DrawsCustomGridLines)
            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedListView_DarkPalette_EnablesOwnerDrawAndReplacesTheNativeGridLines()

            Using lv As New ThemedListView() With {.GridLines = True}

                lv.ApplyPalette(ThemePalette.Dark)

                Assert.IsTrue(lv.OwnerDraw)
                Assert.IsFalse(lv.GridLines)
                Assert.IsTrue(lv.DrawsCustomGridLines)
                Assert.AreEqual(ThemePalette.Dark.InputBack, lv.BackColor)
                Assert.AreEqual(ThemePalette.Dark.InputFore, lv.ForeColor)

            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedListView_DarkPaletteTwice_KeepsTheRequestedGridLines()

            Using lv As New ThemedListView() With {.GridLines = True}

                lv.ApplyPalette(ThemePalette.Dark)
                lv.ApplyPalette(ThemePalette.Dark)
                lv.ApplyPalette(ThemePalette.Light)

                Assert.IsTrue(lv.GridLines)

            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedListView_LightAfterDark_RestoresTheNativeRendering()

            Using lv As New ThemedListView() With {.GridLines = True}
                Using reference As New ListView()

                    lv.ApplyPalette(ThemePalette.Dark)
                    lv.ApplyPalette(ThemePalette.Light)

                    Assert.IsFalse(lv.OwnerDraw)
                    Assert.IsTrue(lv.GridLines)
                    Assert.IsFalse(lv.DrawsCustomGridLines)
                    Assert.AreEqual(reference.BackColor, lv.BackColor)
                    Assert.AreEqual(reference.ForeColor, lv.ForeColor)

                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedListView_ApplyPaletteNothing_ThrowsArgumentNullException()

            Using lv As New ThemedListView()
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() lv.ApplyPalette(Nothing))
            End Using

        End Sub

        <TestMethod>
        <DataRow(HorizontalAlignment.Left, TextFormatFlags.Left)>
        <DataRow(HorizontalAlignment.Center, TextFormatFlags.HorizontalCenter)>
        <DataRow(HorizontalAlignment.Right, TextFormatFlags.Right)>
        Public Sub ThemedListView_GetTextFormatFlags_MapsTheAlignment(ByVal alignment As HorizontalAlignment, ByVal expected As TextFormatFlags)

            Dim flags As TextFormatFlags = ThemedListView.GetTextFormatFlags(alignment)

            Assert.AreEqual(expected, flags And (TextFormatFlags.HorizontalCenter Or TextFormatFlags.Right))
            Assert.IsTrue(flags.HasFlag(TextFormatFlags.SingleLine Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix))

        End Sub

#End Region

#Region " ToggleSwitch "

        <TestMethod>
        Public Sub ToggleSwitch_DefaultState()

            Using toggle As New ToggleSwitch()
                Assert.IsFalse(toggle.Checked)
                Assert.IsTrue(toggle.TabStop)
                Assert.AreEqual(AccessibleRole.CheckButton, toggle.AccessibleRole)
                Assert.AreSame(ThemePalette.Light, toggle.Palette)
                Assert.AreEqual(New Size(100, 22), toggle.Size)
            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_Checked_RaisesCheckedChangedOnlyWhenTheValueChanges()

            Using toggle As New ToggleSwitch()

                Dim raisedCount As Integer = 0
                AddHandler toggle.CheckedChanged, Sub(sender As Object, e As EventArgs) raisedCount += 1

                toggle.Checked = True
                toggle.Checked = True
                toggle.Checked = False

                Assert.AreEqual(2, raisedCount)

            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_Toggle_InvertsTheState()

            Using toggle As New ToggleSwitch()
                toggle.Toggle()
                Assert.IsTrue(toggle.Checked)
                toggle.Toggle()
                Assert.IsFalse(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_Click_TogglesTheState()

            Using toggle As New TestableToggleSwitch()
                toggle.SimulateClick()
                Assert.IsTrue(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_SpaceKey_TogglesTheState()

            Using toggle As New TestableToggleSwitch()
                Assert.IsTrue(toggle.SimulateKeyUp(Keys.Space))
                Assert.IsTrue(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        <DataRow(Keys.Enter)>
        <DataRow(Keys.Space Or Keys.Shift)>
        <DataRow(Keys.Space Or Keys.Control)>
        Public Sub ToggleSwitch_OtherKeys_DoNotToggleTheState(ByVal keyData As Keys)

            Using toggle As New TestableToggleSwitch()
                Assert.IsFalse(toggle.SimulateKeyUp(keyData))
                Assert.IsFalse(toggle.Checked)
            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_ApplyPalette_StoresThePalette()

            Using toggle As New ToggleSwitch()
                toggle.ApplyPalette(ThemePalette.Dark)
                Assert.AreSame(ThemePalette.Dark, toggle.Palette)
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() toggle.ApplyPalette(Nothing))
            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_SwitchBounds_IsRightAlignedAndVerticallyCentered()

            Using toggle As New ToggleSwitch() With {.Size = New Size(120, 30)}

                Dim bounds As Rectangle = toggle.SwitchBounds

                Assert.IsTrue(bounds.Right <= toggle.ClientSize.Width)
                Assert.IsTrue(bounds.Right >= toggle.ClientSize.Width - 2)
                Assert.AreEqual(bounds.Width, bounds.Height * 2)
                Assert.IsTrue(Math.Abs((bounds.Top - 0) - (toggle.ClientSize.Height - bounds.Bottom)) <= 1)

            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_GetPreferredSize_FitsTheCaptionAndTheSwitch()

            Using toggle As New ToggleSwitch() With {.Text = "Dark theme"}

                Dim switchSize As Size = ToggleSwitch.GetSwitchSize(toggle.Font)
                Dim captionWidth As Integer = TextRenderer.MeasureText(toggle.Text, toggle.Font).Width
                Dim preferred As Size = toggle.GetPreferredSize(Size.Empty)

                Assert.IsTrue(preferred.Width > switchSize.Width + (captionWidth \ 2))
                Assert.IsTrue(preferred.Height >= switchSize.Height)

                ' The designer size of the main form switch must fit its caption.
                Assert.IsTrue(preferred.Width <= 100, $"Preferred width: {preferred.Width}")

            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_GetSwitchSize_ScalesWithTheFont()

            Using smallFont As New Font(FontFamily.GenericSansSerif, 8.25F),
                  largeFont As New Font(FontFamily.GenericSansSerif, 16.0F)

                Dim smallSize As Size = ToggleSwitch.GetSwitchSize(smallFont)
                Dim largeSize As Size = ToggleSwitch.GetSwitchSize(largeFont)

                Assert.IsTrue(largeSize.Height > smallSize.Height)
                Assert.AreEqual(smallSize.Height * 2, smallSize.Width)

            End Using

        End Sub

#End Region

#Region " ToggleSwitch Captions "

        <TestMethod>
        Public Sub ToggleSwitch_DisplayText_DependsOnTheCheckedState()

            Using toggle As New ToggleSwitch() With {.Text = "Theme", .CheckedText = "Dark theme", .UncheckedText = "Light theme"}

                Assert.AreEqual("Light theme", toggle.DisplayText)

                toggle.Checked = True
                Assert.AreEqual("Dark theme", toggle.DisplayText)

                toggle.Checked = False
                Assert.AreEqual("Light theme", toggle.DisplayText)

            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_DisplayText_EmptyStateCaption_FallsBackToText()

            Using toggle As New ToggleSwitch() With {.Text = "Theme", .CheckedText = "Dark theme"}

                Assert.AreEqual("Theme", toggle.DisplayText)
                toggle.Checked = True
                Assert.AreEqual("Dark theme", toggle.DisplayText)

                toggle.CheckedText = Nothing
                Assert.AreEqual(String.Empty, toggle.CheckedText)
                Assert.AreEqual("Theme", toggle.DisplayText)

            End Using

        End Sub

        <TestMethod>
        Public Sub ToggleSwitch_GetPreferredSize_FitsTheWidestStateCaption()

            Using shortCaption As New ToggleSwitch() With {.Text = "x"},
                  longCaption As New ToggleSwitch() With {.Text = "x", .CheckedText = "A much longer caption"}

                Assert.IsTrue(longCaption.GetPreferredSize(Size.Empty).Width > shortCaption.GetPreferredSize(Size.Empty).Width)

            End Using

        End Sub

#End Region

#Region " ThemedComboBox "

        ''' <summary>
        ''' A <see cref="ThemedComboBox"/> that exposes its protected control styles.
        ''' </summary>
        Private NotInheritable Class TestableComboBox : Inherits ThemedComboBox

            ''' <summary>
            ''' Gets a value indicating whether the control is user painted (WM_PAINT routed to OnPaint).
            ''' </summary>
            Friend ReadOnly Property IsUserPaint As Boolean
                Get
                    Return Me.GetStyle(ControlStyles.UserPaint)
                End Get
            End Property

        End Class

        <TestMethod>
        Public Sub ThemedComboBox_DefaultState_IsNativeRendering()

            Using combo As New TestableComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}
                Assert.AreSame(ThemePalette.Light, combo.Palette)
                Assert.IsFalse(combo.IsCustomPainted)
                Assert.IsFalse(combo.IsUserPaint)
                Assert.AreEqual(DrawMode.Normal, combo.DrawMode)
            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedComboBox_DarkPaletteWithDropDownList_IsCustomPainted()

            ' Regression: the native flat style drew a dark rectangle with a black border in the middle of the control.
            Using combo As New TestableComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}

                combo.ApplyPalette(ThemePalette.Dark)

                Assert.IsTrue(combo.IsCustomPainted)
                Assert.IsTrue(combo.IsUserPaint)
                Assert.AreEqual(DrawMode.OwnerDrawFixed, combo.DrawMode)
                Assert.AreEqual(FlatStyle.Standard, combo.FlatStyle)
                Assert.AreEqual(ThemePalette.Dark.InputBack, combo.BackColor)
                Assert.AreEqual(ThemePalette.Dark.InputFore, combo.ForeColor)

            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedComboBox_DarkPaletteWithEditableStyle_KeepsTheNativeRendering()

            Using combo As New TestableComboBox() With {.DropDownStyle = ComboBoxStyle.DropDown}

                combo.ApplyPalette(ThemePalette.Dark)

                Assert.IsFalse(combo.IsCustomPainted)
                Assert.IsFalse(combo.IsUserPaint)
                Assert.AreEqual(DrawMode.Normal, combo.DrawMode)
                Assert.AreEqual(ThemePalette.Dark.InputBack, combo.BackColor)

                ' Switching to DropDownList while dark enables the custom painting.
                combo.DropDownStyle = ComboBoxStyle.DropDownList
                Assert.IsTrue(combo.IsUserPaint)

            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedComboBox_LightAfterDark_RestoresTheNativeRendering()

            Using combo As New TestableComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList},
                  reference As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}

                combo.Items.AddRange(New Object() {"A", "B"})
                combo.SelectedIndex = 1

                combo.ApplyPalette(ThemePalette.Dark)
                combo.ApplyPalette(ThemePalette.Light)

                Assert.IsFalse(combo.IsUserPaint)
                Assert.AreEqual(DrawMode.Normal, combo.DrawMode)
                ' Note: on .NET Framework (Windows) a reset ComboBox reports SystemColors.Window.
                ' (Mono's WinForms implementation reports the ambient Control color instead.)
                Assert.AreEqual(reference.BackColor, combo.BackColor)
                Assert.AreEqual(1, combo.SelectedIndex)

            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedComboBox_ThemeManager_DoesNotApplyTheNativeFlatStyle()

            Using combo As New TestableComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}

                ThemeManager.ApplyTheme(combo, AppTheme.Dark)

                Assert.AreEqual(FlatStyle.Standard, combo.FlatStyle)
                Assert.IsTrue(combo.IsCustomPainted)
                Assert.AreSame(ThemePalette.Dark, combo.Palette)

            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedComboBox_ApplyPaletteNothing_ThrowsArgumentNullException()

            Using combo As New ThemedComboBox()
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() combo.ApplyPalette(Nothing))
            End Using

        End Sub

        <TestMethod>
        Public Sub ThemedComboBox_ButtonBounds_IsAtTheRightEdge()

            Using combo As New ThemedComboBox() With {.Width = 120}

                Dim bounds As Rectangle = combo.ButtonBounds

                Assert.AreEqual(SystemInformation.VerticalScrollBarWidth, bounds.Width)
                Assert.IsTrue(bounds.Right <= combo.ClientSize.Width)
                Assert.IsTrue(bounds.Right >= combo.ClientSize.Width - 2)

            End Using

        End Sub

#End Region

#Region " Renderer and Color Table "

        <TestMethod>
        Public Sub ThemedColorTable_NothingPalette_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Call New ThemedColorTable(Nothing).ToString())

        End Sub

        <TestMethod>
        Public Sub ThemedColorTable_ReturnsThePaletteColors()

            Dim p As ThemePalette = ThemePalette.Dark
            Dim table As New ThemedColorTable(p)

            Assert.IsFalse(table.UseSystemColors)
            Assert.AreEqual(p.ToolStripBack, table.ToolStripGradientBegin)
            Assert.AreEqual(p.ToolStripBack, table.ToolStripGradientEnd)
            Assert.AreEqual(p.ToolStripBack, table.ToolStripDropDownBackground)
            Assert.AreEqual(p.ToolStripBack, table.ImageMarginGradientBegin)
            Assert.AreEqual(p.Border, table.ToolStripBorder)
            Assert.AreEqual(p.Border, table.MenuBorder)
            Assert.AreEqual(p.Border, table.SeparatorDark)
            Assert.AreEqual(p.HoverBack, table.MenuItemSelected)
            Assert.AreEqual(p.HoverBack, table.ButtonSelectedGradientBegin)
            Assert.AreEqual(p.PressedBack, table.ButtonPressedGradientBegin)
            Assert.AreEqual(p.PressedBack, table.MenuItemPressedGradientBegin)

        End Sub

        <TestMethod>
        Public Sub ThemedToolStripRenderer_UsesThePaletteWithoutRoundedEdges()

            Dim renderer As New ThemedToolStripRenderer(ThemePalette.Dark)

            Assert.AreSame(ThemePalette.Dark, renderer.Palette)
            Assert.IsFalse(renderer.RoundedEdges)
            Assert.IsInstanceOfType(Of ThemedColorTable)(renderer.ColorTable)

        End Sub

        <TestMethod>
        Public Sub ThemedToolStripRenderer_NothingPalette_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Call New ThemedToolStripRenderer(Nothing).ToString())

        End Sub

#End Region

    End Class

End Namespace

#End Region
