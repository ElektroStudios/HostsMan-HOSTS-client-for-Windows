' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemeManagerTests.vb" company="Elektro Studios">
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

Imports HostsMan.UserInterface.Theming

#End Region

#Region " ThemeManagerTests "

Namespace UserInterface

    ''' <summary>
    ''' Tests for <see cref="ThemeManager"/>.
    ''' No window handle is created by these tests.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class ThemeManagerTests

#Region " Helpers "

        ''' <summary>
        ''' A control that records the palettes applied through <see cref="IThemedControl"/>.
        ''' </summary>
        Private NotInheritable Class RecordingControl : Inherits Control : Implements IThemedControl

            ''' <summary>
            ''' Gets the applied palettes, in order.
            ''' </summary>
            Friend ReadOnly Property AppliedPalettes As List(Of ThemePalette) = New List(Of ThemePalette)

            ''' <summary>
            ''' Records the applied palette.
            ''' </summary>
            ''' <param name="palette">The palette.</param>
            Public Sub ApplyPalette(ByVal palette As ThemePalette) Implements IThemedControl.ApplyPalette
                Me.AppliedPalettes.Add(palette)
            End Sub

        End Class

        ''' <summary>
        ''' A sample control tree similar to the application forms.
        ''' </summary>
        Private NotInheritable Class SampleTree : Implements IDisposable

            Friend ReadOnly Form As New Form()
            Friend ReadOnly Panel As New Panel() With {.BackColor = SystemColors.Control}
            Friend ReadOnly Label As New Label() With {.Text = "Label"}
            Friend ReadOnly LinkLabel As New LinkLabel() With {.Text = "Link"}
            Friend ReadOnly TextBox As New TextBox()
            Friend ReadOnly ComboBox As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}
            Friend ReadOnly Button As New Button() With {.Text = "Button", .BackColor = Color.Transparent, .UseVisualStyleBackColor = False}
            Friend ReadOnly RadioButton As New RadioButton() With {.Text = "Radio"}
            Friend ReadOnly ListView As New ListView()
            Friend ReadOnly ToolStrip As New ToolStrip()
            Friend ReadOnly ToolStripButton As New ToolStripButton("Tool")
            Friend ReadOnly ContextMenu As New ContextMenuStrip()
            Friend ReadOnly MenuItem As New ToolStripMenuItem("Menu")
            Friend ReadOnly SubMenuItem As New ToolStripMenuItem("Sub menu")
            Friend ReadOnly Recording As New RecordingControl()

            Friend Sub New()
                Me.ToolStrip.Items.Add(Me.ToolStripButton)
                Me.MenuItem.DropDownItems.Add(Me.SubMenuItem)
                Me.ContextMenu.Items.Add(Me.MenuItem)
                Me.ListView.ContextMenuStrip = Me.ContextMenu
                Me.Panel.Controls.AddRange(New Control() {Me.Label, Me.LinkLabel, Me.TextBox, Me.ComboBox, Me.Button, Me.RadioButton, Me.Recording})
                Me.Form.Controls.AddRange(New Control() {Me.ListView, Me.Panel, Me.ToolStrip})
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                Me.ContextMenu.Dispose()
                Me.Form.Dispose()
            End Sub

        End Class

#End Region

#Region " Initialization "

        ''' <summary>
        ''' Restores the default theme after each test (the theme manager state is shared).
        ''' </summary>
        <TestCleanup>
        Public Sub TestCleanup()

            ThemeManager.CurrentTheme = AppTheme.Light

        End Sub

#End Region

#Region " CurrentTheme "

        <TestMethod>
        Public Sub CurrentTheme_DefaultValue_IsLight()

            Assert.AreEqual(AppTheme.Light, ThemeManager.CurrentTheme)
            Assert.AreSame(ThemePalette.Light, ThemeManager.CurrentPalette)

        End Sub

        <TestMethod>
        Public Sub CurrentTheme_SetDifferentValue_RaisesThemeChangedOnce()

            Dim raisedCount As Integer = 0
            Dim handler As EventHandler = Sub(sender As Object, e As EventArgs) raisedCount += 1

            AddHandler ThemeManager.ThemeChanged, handler
            Try
                ThemeManager.CurrentTheme = AppTheme.Dark
                ThemeManager.CurrentTheme = AppTheme.Dark

            Finally
                RemoveHandler ThemeManager.ThemeChanged, handler

            End Try

            Assert.AreEqual(1, raisedCount)
            Assert.AreEqual(AppTheme.Dark, ThemeManager.CurrentTheme)
            Assert.AreSame(ThemePalette.Dark, ThemeManager.CurrentPalette)

        End Sub

        <TestMethod>
        Public Sub CurrentTheme_SetSameValue_DoesNotRaiseThemeChanged()

            Dim raised As Boolean = False
            Dim handler As EventHandler = Sub(sender As Object, e As EventArgs) raised = True

            AddHandler ThemeManager.ThemeChanged, handler
            Try
                ThemeManager.CurrentTheme = AppTheme.Light

            Finally
                RemoveHandler ThemeManager.ThemeChanged, handler

            End Try

            Assert.IsFalse(raised)

        End Sub

        <TestMethod>
        Public Sub CurrentTheme_SetUndefinedValue_ThrowsAndKeepsTheCurrentValue()

            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() ThemeManager.CurrentTheme = CType(5, AppTheme))
            Assert.AreEqual(AppTheme.Light, ThemeManager.CurrentTheme)

        End Sub

#End Region

#Region " ApplyTheme "

        <TestMethod>
        Public Sub ApplyTheme_NothingRoot_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyTheme(Nothing, AppTheme.Dark))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyTheme(Nothing))

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_UndefinedTheme_ThrowsInvalidEnumArgumentException()

            Using control As New Control()
                Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() ThemeManager.ApplyTheme(control, CType(9, AppTheme)))
            End Using

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_Dark_AppliesThePaletteToTheWholeTree()

            Dim p As ThemePalette = ThemePalette.Dark

            Using tree As New SampleTree()

                ThemeManager.ApplyTheme(tree.Form, AppTheme.Dark)

                Assert.AreEqual(p.WindowBack, tree.Form.BackColor)
                Assert.AreEqual(p.WindowFore, tree.Form.ForeColor)

                ' Ambient controls inherit the form colors (the designer Panel.BackColor is reset).
                Assert.AreEqual(p.WindowBack, tree.Panel.BackColor)
                Assert.AreEqual(p.WindowFore, tree.Label.ForeColor)
                Assert.AreEqual(p.WindowFore, tree.RadioButton.ForeColor)

                Assert.AreEqual(p.InputBack, tree.TextBox.BackColor)
                Assert.AreEqual(p.InputFore, tree.TextBox.ForeColor)
                Assert.AreEqual(BorderStyle.FixedSingle, tree.TextBox.BorderStyle)

                Assert.AreEqual(FlatStyle.Flat, tree.ComboBox.FlatStyle)
                Assert.AreEqual(p.InputBack, tree.ComboBox.BackColor)

                Assert.AreEqual(p.InputBack, tree.ListView.BackColor)
                Assert.AreEqual(p.InputFore, tree.ListView.ForeColor)

                Assert.AreEqual(FlatStyle.Flat, tree.Button.FlatStyle)
                Assert.AreEqual(p.ControlBack, tree.Button.BackColor)
                Assert.AreEqual(p.ControlFore, tree.Button.ForeColor)
                Assert.IsFalse(tree.Button.UseVisualStyleBackColor)
                Assert.AreEqual(p.Border, tree.Button.FlatAppearance.BorderColor)
                Assert.AreEqual(p.HoverBack, tree.Button.FlatAppearance.MouseOverBackColor)

                Assert.AreEqual(p.Link, tree.LinkLabel.LinkColor)
                Assert.AreEqual(p.LinkActive, tree.LinkLabel.ActiveLinkColor)

                Assert.IsInstanceOfType(Of ThemedToolStripRenderer)(tree.ToolStrip.Renderer)
                Assert.AreEqual(p.ToolStripBack, tree.ToolStrip.BackColor)
                Assert.AreEqual(p.ControlFore, tree.ToolStripButton.ForeColor)

                Assert.AreSame(p, tree.Recording.AppliedPalettes.Single())

            End Using

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_Dark_AlsoThemesTheContextMenusAndTheirSubMenus()

            Using tree As New SampleTree()

                ThemeManager.ApplyTheme(tree.Form, AppTheme.Dark)

                Assert.IsInstanceOfType(Of ThemedToolStripRenderer)(tree.ContextMenu.Renderer)
                Assert.AreEqual(ThemePalette.Dark.ControlFore, tree.MenuItem.ForeColor)
                Assert.IsInstanceOfType(Of ThemedToolStripRenderer)(tree.MenuItem.DropDown.Renderer)
                Assert.AreEqual(ThemePalette.Dark.ControlFore, tree.SubMenuItem.ForeColor)

            End Using

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_LightAfterDark_RestoresTheWindowsDefaults()

            Using tree As New SampleTree()
                Using reference As New SampleTree()

                    ThemeManager.ApplyTheme(tree.Form, AppTheme.Dark)
                    ThemeManager.ApplyTheme(tree.Form, AppTheme.Light)

                    Assert.AreEqual(SystemColors.Control, tree.Form.BackColor)
                    Assert.AreEqual(SystemColors.ControlText, tree.Form.ForeColor)
                    Assert.AreEqual(SystemColors.Control, tree.Panel.BackColor)

                    ' Note: on .NET Framework (Windows) a reset TextBox reports SystemColors.Window/WindowText.
                    ' (Mono's WinForms implementation reports the ambient ControlText color instead.)
                    Assert.AreEqual(reference.TextBox.BackColor, tree.TextBox.BackColor)
                    Assert.AreEqual(reference.TextBox.ForeColor, tree.TextBox.ForeColor)
                    Assert.AreEqual(BorderStyle.Fixed3D, tree.TextBox.BorderStyle)

                    Assert.AreEqual(FlatStyle.Standard, tree.ComboBox.FlatStyle)
                    Assert.AreEqual(reference.ComboBox.BackColor, tree.ComboBox.BackColor)
                    Assert.AreEqual(reference.ListView.BackColor, tree.ListView.BackColor)

                    Assert.AreEqual(FlatStyle.Standard, tree.Button.FlatStyle)
                    Assert.IsTrue(tree.Button.UseVisualStyleBackColor)
                    Assert.AreEqual(SystemColors.Control, tree.Button.BackColor)

                    Assert.AreEqual(reference.LinkLabel.LinkColor, tree.LinkLabel.LinkColor)
                    Assert.AreEqual(reference.LinkLabel.ActiveLinkColor, tree.LinkLabel.ActiveLinkColor)

                    Assert.AreEqual(ToolStripRenderMode.ManagerRenderMode, tree.ToolStrip.RenderMode)
                    Assert.IsNotInstanceOfType(Of ThemedToolStripRenderer)(tree.ToolStrip.Renderer)
                    Assert.AreEqual(ToolStripRenderMode.ManagerRenderMode, tree.ContextMenu.RenderMode)
                    Assert.AreEqual(reference.ToolStripButton.ForeColor, tree.ToolStripButton.ForeColor)

                    Assert.AreSame(ThemePalette.Light, tree.Recording.AppliedPalettes.Last())

                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_DeeplyNestedControls_DoesNotOverflowTheStack()

            ' The control tree is traversed iteratively, so deep trees are supported.
            Using root As New Panel()

                Dim current As Control = root
                For i As Integer = 1 To 500
                    Dim child As New Panel()
                    current.Controls.Add(child)
                    current = child
                Next i
                Dim deepestTextBox As New TextBox()
                current.Controls.Add(deepestTextBox)

                ThemeManager.ApplyTheme(root, AppTheme.Dark)

                Assert.AreEqual(ThemePalette.Dark.InputBack, deepestTextBox.BackColor)

            End Using

        End Sub

        <TestMethod>
        Public Sub ApplyTheme_WithoutTheme_UsesTheCurrentTheme()

            ThemeManager.CurrentTheme = AppTheme.Dark

            Using textBox As New TextBox()
                ThemeManager.ApplyTheme(textBox)
                Assert.AreEqual(ThemePalette.Dark.InputBack, textBox.BackColor)
            End Using

        End Sub

        <TestMethod>
        Public Sub ApplyToToolStrip_NothingArguments_ThrowArgumentNullException()

            Using strip As New ToolStrip()
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyToToolStrip(Nothing, ThemePalette.Dark))
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyToToolStrip(strip, Nothing))
            End Using

        End Sub

#End Region

#Region " Title Bar "

        <TestMethod>
        Public Sub SetTitleBarDarkMode_NothingForm_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.SetTitleBarDarkMode(Nothing, True))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.RefreshTitleBar(Nothing))

        End Sub

        <TestMethod>
        Public Sub SetTitleBarDarkMode_HandleNotCreated_ThrowsInvalidOperationException()

            ' The DWM attribute requires a window handle: it must be applied from OnHandleCreated, not before.
            Using form As New Form()
                Assert.ThrowsExactly(Of InvalidOperationException)(Sub() ThemeManager.SetTitleBarDarkMode(form, True))
                Assert.ThrowsExactly(Of InvalidOperationException)(Sub() ThemeManager.RefreshTitleBar(form))
                Assert.IsFalse(form.IsHandleCreated)
            End Using

        End Sub

#End Region

    End Class

End Namespace

#End Region
