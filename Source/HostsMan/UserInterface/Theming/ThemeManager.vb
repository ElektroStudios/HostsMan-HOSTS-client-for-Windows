' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemeManager.vb" company="Elektro Studios">
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

#End Region

#Region " ThemeManager "

Namespace UserInterface.Theming

    ''' <summary>
    ''' Manages the current visual theme of the application and applies it to forms and controls.
    ''' <para></para>
    ''' The light theme restores the default Windows system colors and styles of each control,
    ''' while the dark theme applies the colors of <see cref="ThemePalette.Dark"/>.
    ''' </summary>
    Public NotInheritable Class ThemeManager

#Region " Events "

        ''' <summary>
        ''' Occurs when the value of <see cref="CurrentTheme"/> changes.
        ''' </summary>
        Public Shared Event ThemeChanged As EventHandler

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the current theme of the application (Defaults to <see cref="AppTheme.Light"/>).
        ''' <para></para>
        ''' Setting a different value raises the <see cref="ThemeChanged"/> event.
        ''' </summary>
        ''' <value>The current theme.</value>
        ''' <exception cref="InvalidEnumArgumentException">The value is not a defined <see cref="AppTheme"/>.</exception>
        Public Shared Property CurrentTheme As AppTheme
            Get
                Return ThemeManager.currentTheme1
            End Get
            Set(ByVal value As AppTheme)
                If Not [Enum].IsDefined(GetType(AppTheme), value) Then
                    Throw New InvalidEnumArgumentException(NameOf(value), CInt(value), GetType(AppTheme))
                End If

                If value <> ThemeManager.currentTheme1 Then
                    ThemeManager.currentTheme1 = value
                    RaiseEvent ThemeChanged(Nothing, EventArgs.Empty)
                End If
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="CurrentTheme"/> property. )
        ''' </summary>
        Private Shared currentTheme1 As AppTheme = AppTheme.Light

        ''' <summary>
        ''' Gets the palette of the <see cref="CurrentTheme"/>.
        ''' </summary>
        ''' <value>The palette of the current theme.</value>
        Public Shared ReadOnly Property CurrentPalette As ThemePalette
            Get
                Return ThemePalette.FromTheme(ThemeManager.currentTheme1)
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="ThemeManager"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Applies the <see cref="CurrentTheme"/> to the specified control and all its child controls
        ''' (including their context menus).
        ''' </summary>
        ''' <param name="root">The root control (usually a form).</param>
        ''' <exception cref="ArgumentNullException"><paramref name="root"/> is <see langword="Nothing"/>.</exception>
        Public Shared Sub ApplyTheme(ByVal root As Control)

            ThemeManager.ApplyTheme(root, ThemeManager.CurrentTheme)

        End Sub

        ''' <summary>
        ''' Applies the specified theme to the specified control and all its child controls
        ''' (including their context menus).
        ''' <para></para>
        ''' The control tree is traversed iteratively (no recursion).
        ''' </summary>
        ''' <param name="root">The root control (usually a form).</param>
        ''' <param name="theme">The theme to apply.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="root"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="theme"/> is not a defined <see cref="AppTheme"/>.</exception>
        Public Shared Sub ApplyTheme(ByVal root As Control, ByVal theme As AppTheme)

            If root Is Nothing Then
                Throw New ArgumentNullException(NameOf(root))
            End If

            Dim palette As ThemePalette = ThemePalette.FromTheme(theme)
            Dim pending As New Stack(Of Control)
            pending.Push(root)

            root.SuspendLayout()
            Try
                While pending.Count <> 0

                    Dim current As Control = pending.Pop()
                    ThemeManager.ApplyToControl(current, palette)

                    If current.ContextMenuStrip IsNot Nothing Then
                        ThemeManager.ApplyToToolStrip(current.ContextMenuStrip, palette)
                    End If

                    For Each child As Control In current.Controls
                        pending.Push(child)
                    Next child

                End While

            Finally
                root.ResumeLayout(performLayout:=True)

            End Try

            root.Invalidate(invalidateChildren:=True)

        End Sub

        ''' <summary>
        ''' Applies the specified palette to a <see cref="ToolStrip"/> (or <see cref="ContextMenuStrip"/>),
        ''' its items, and all its nested drop-down menus.
        ''' </summary>
        ''' <param name="toolStrip">The tool strip.</param>
        ''' <param name="palette">The palette to apply.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="toolStrip"/> or <paramref name="palette"/> is <see langword="Nothing"/>.</exception>
        Public Shared Sub ApplyToToolStrip(ByVal toolStrip As ToolStrip, ByVal palette As ThemePalette)

            If toolStrip Is Nothing Then
                Throw New ArgumentNullException(NameOf(toolStrip))
            End If

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            Dim pending As New Stack(Of ToolStrip)
            pending.Push(toolStrip)

            While pending.Count <> 0

                Dim current As ToolStrip = pending.Pop()

                If palette.IsDark Then
                    current.Renderer = New ThemedToolStripRenderer(palette)
                    current.BackColor = palette.ToolStripBack
                    current.ForeColor = palette.ControlFore
                Else
                    current.RenderMode = ToolStripRenderMode.ManagerRenderMode
                    current.ResetBackColor()
                    current.ResetForeColor()
                End If

                For Each item As ToolStripItem In current.Items

                    If palette.IsDark Then
                        item.ForeColor = palette.ControlFore
                    Else
                        item.ResetForeColor()
                    End If

                    Dim dropDownItem As ToolStripDropDownItem = TryCast(item, ToolStripDropDownItem)
                    If dropDownItem IsNot Nothing AndAlso dropDownItem.HasDropDownItems Then
                        pending.Push(dropDownItem.DropDown)
                    End If

                Next item

            End While

        End Sub

        ''' <summary>
        ''' Enables or disables the dark mode of the title bar (non-client area) of the specified form,
        ''' through the DWMWA_USE_IMMERSIVE_DARK_MODE attribute of the Desktop Window Manager.
        ''' <para></para>
        ''' The form handle must be created. This method should be called from <see cref="Form.OnHandleCreated"/>,
        ''' because calling it from the <see cref="Form.Load"/> event has no effect on the first paint of the title bar.
        ''' </summary>
        ''' <param name="form">The form.</param>
        ''' <param name="enabled"><see langword="True"/> to enable the dark title bar; <see langword="False"/> to restore the default title bar.</param>
        ''' <returns>
        ''' <see langword="True"/> if the attribute was applied;
        ''' <see langword="False"/> if the operating system does not support it (Windows versions older than Windows 10 1809).
        ''' </returns>
        ''' <exception cref="ArgumentNullException"><paramref name="form"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidOperationException">The form handle is not created.</exception>
        Public Shared Function SetTitleBarDarkMode(ByVal form As Form, ByVal enabled As Boolean) As Boolean

            If form Is Nothing Then
                Throw New ArgumentNullException(NameOf(form))
            End If

            If Not form.IsHandleCreated Then
                Throw New InvalidOperationException("The form handle is not created.")
            End If

            Dim value As Integer = If(enabled, 1, 0)
            Dim hresult As Integer = ThemeNativeMethods.DwmSetWindowAttribute(form.Handle, ThemeNativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, value, 4)
            If hresult <> 0 Then
                hresult = ThemeNativeMethods.DwmSetWindowAttribute(form.Handle, ThemeNativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, value, 4)
            End If

            Return hresult = 0

        End Function

        ''' <summary>
        ''' Forces the repaint of the title bar (non-client area) of the specified form,
        ''' so a title bar theme change takes effect immediately on a visible form.
        ''' </summary>
        ''' <param name="form">The form.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="form"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidOperationException">The form handle is not created.</exception>
        Public Shared Sub RefreshTitleBar(ByVal form As Form)

            If form Is Nothing Then
                Throw New ArgumentNullException(NameOf(form))
            End If

            If Not form.IsHandleCreated Then
                Throw New InvalidOperationException("The form handle is not created.")
            End If

            ThemeNativeMethods.SetWindowPos(form.Handle, IntPtr.Zero, 0, 0, 0, 0, ThemeNativeMethods.SWP_FRAMECHANGED_ONLY)

            ' Windows 10 does not repaint the caption until the activation state changes.
            If Global.System.Windows.Forms.Form.ActiveForm Is form Then
                ThemeNativeMethods.SendMessage(form.Handle, ThemeNativeMethods.WM_NCACTIVATE, IntPtr.Zero, IntPtr.Zero)
                ThemeNativeMethods.SendMessage(form.Handle, ThemeNativeMethods.WM_NCACTIVATE, New IntPtr(1), IntPtr.Zero)
            End If

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Applies the specified palette to a single control (not to its children).
        ''' </summary>
        ''' <param name="control">The control.</param>
        ''' <param name="palette">The palette to apply.</param>
        Private Shared Sub ApplyToControl(ByVal control As Control, ByVal palette As ThemePalette)

            Dim isDark As Boolean = palette.IsDark

            Select Case True

                Case TypeOf control Is Form
                    ThemeManager.ApplyColors(control, palette.WindowBack, palette.WindowFore, isDark)

                Case TypeOf control Is ToolStrip
                    ThemeManager.ApplyToToolStrip(DirectCast(control, ToolStrip), palette)

                Case TypeOf control Is TextBoxBase
                    ThemeManager.ApplyColors(control, palette.InputBack, palette.InputFore, isDark)
                    Dim textBox As TextBox = TryCast(control, TextBox)
                    If textBox IsNot Nothing Then
                        textBox.BorderStyle = If(isDark, BorderStyle.FixedSingle, BorderStyle.Fixed3D)
                    End If

                Case TypeOf control Is ComboBox AndAlso Not TypeOf control Is IThemedControl
                    ' (Custom combo boxes, such as ThemedComboBox, are fully handled by their ApplyPalette method.)
                    DirectCast(control, ComboBox).FlatStyle = If(isDark, FlatStyle.Flat, FlatStyle.Standard)
                    ThemeManager.ApplyColors(control, palette.InputBack, palette.InputFore, isDark)

                Case TypeOf control Is ListView, TypeOf control Is ListBox, TypeOf control Is TreeView
                    ThemeManager.ApplyColors(control, palette.InputBack, palette.InputFore, isDark)

                Case TypeOf control Is Button
                    ThemeManager.ApplyToButton(DirectCast(control, Button), palette)

                Case TypeOf control Is LinkLabel
                    Dim linkLabel As LinkLabel = DirectCast(control, LinkLabel)
                    linkLabel.LinkColor = If(isDark, palette.Link, Color.Empty)
                    linkLabel.VisitedLinkColor = If(isDark, palette.Link, Color.Empty)
                    linkLabel.ActiveLinkColor = If(isDark, palette.LinkActive, Color.Empty)
                    ThemeManager.ApplyAmbientColors(control)

                Case Else
                    ThemeManager.ApplyAmbientColors(control)

            End Select

            Dim themedControl As IThemedControl = TryCast(control, IThemedControl)
            If themedControl IsNot Nothing Then
                themedControl.ApplyPalette(palette)
            End If

        End Sub

        ''' <summary>
        ''' Applies the specified palette to a <see cref="Button"/>.
        ''' </summary>
        ''' <param name="button">The button.</param>
        ''' <param name="palette">The palette to apply.</param>
        Private Shared Sub ApplyToButton(ByVal button As Button, ByVal palette As ThemePalette)

            If palette.IsDark Then
                button.FlatStyle = FlatStyle.Flat
                button.BackColor = palette.ControlBack
                button.ForeColor = palette.ControlFore
                button.UseVisualStyleBackColor = False
                button.FlatAppearance.BorderSize = 1
                button.FlatAppearance.BorderColor = palette.Border
                button.FlatAppearance.MouseOverBackColor = palette.HoverBack
                button.FlatAppearance.MouseDownBackColor = palette.PressedBack
            Else
                button.FlatStyle = FlatStyle.Standard
                button.ResetBackColor()
                button.ResetForeColor()
                button.UseVisualStyleBackColor = True
                button.FlatAppearance.BorderColor = Color.Empty
                button.FlatAppearance.MouseOverBackColor = Color.Empty
                button.FlatAppearance.MouseDownBackColor = Color.Empty
            End If

        End Sub

        ''' <summary>
        ''' Applies explicit colors to a control (dark theme), or restores its default colors (light theme).
        ''' </summary>
        ''' <param name="control">The control.</param>
        ''' <param name="backColor">The background color to apply in the dark theme.</param>
        ''' <param name="foreColor">The foreground color to apply in the dark theme.</param>
        ''' <param name="isDark">A value indicating whether the dark theme is being applied.</param>
        Private Shared Sub ApplyColors(ByVal control As Control, ByVal backColor As Color, ByVal foreColor As Color, ByVal isDark As Boolean)

            If isDark Then
                control.BackColor = backColor
                control.ForeColor = foreColor
            Else
                control.ResetBackColor()
                control.ResetForeColor()
            End If

        End Sub

        ''' <summary>
        ''' Restores the ambient colors of a control, so it inherits the colors of its parent
        ''' (used for labels, panels and other transparent-like controls in both themes).
        ''' </summary>
        ''' <param name="control">The control.</param>
        Private Shared Sub ApplyAmbientColors(ByVal control As Control)

            control.ResetBackColor()
            control.ResetForeColor()

        End Sub

#End Region

    End Class

End Namespace

#End Region
