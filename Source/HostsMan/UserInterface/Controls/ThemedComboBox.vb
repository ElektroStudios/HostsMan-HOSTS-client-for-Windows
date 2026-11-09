' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemedComboBox.vb" company="Elektro Studios">
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
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices

Imports HostsMan.UserInterface.Theming

#End Region

#Region " ThemedComboBox "

Namespace UserInterface.Controls

    ''' <summary>
    ''' A <see cref="ComboBox"/> that supports the application themes.
    ''' <para></para>
    ''' In the light theme the control is drawn natively by Windows.
    ''' In the dark theme, when <see cref="ComboBox.DropDownStyle"/> is <see cref="ComboBoxStyle.DropDownList"/>,
    ''' the whole control (background, border, selected text and drop-down arrow) and the items of the
    ''' drop-down list are custom painted with the colors of the <see cref="ThemePalette"/>,
    ''' because the native flat style draws a dark rectangle with a black border in the middle of the control.
    ''' </summary>
    <DesignerCategory("Code")>
    <ToolboxItem(True)>
    Public Class ThemedComboBox : Inherits ComboBox : Implements IThemedControl

#Region " Constants "

        ''' <summary>
        ''' The horizontal padding, in pixels, of the selected item text.
        ''' </summary>
        Private Const TextPadding As Integer = 4

        ''' <summary>
        ''' The control styles used to custom paint the control.
        ''' </summary>
        Private Const CustomPaintStyles As ControlStyles = ControlStyles.UserPaint Or
                                                           ControlStyles.AllPaintingInWmPaint Or
                                                           ControlStyles.OptimizedDoubleBuffer Or
                                                           ControlStyles.ResizeRedraw

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the palette currently applied to the control.
        ''' </summary>
        ''' <value>The palette currently applied to the control.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property Palette As ThemePalette
            Get
                Return Me.palette1
            End Get
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="Palette"/> property. )
        ''' </summary>
        Private palette1 As ThemePalette = ThemePalette.Light

        ''' <summary>
        ''' Gets a value indicating whether the control is custom painted
        ''' (dark palette and <see cref="ComboBoxStyle.DropDownList"/> style).
        ''' </summary>
        ''' <value><see langword="True"/> if the control is custom painted; otherwise, <see langword="False"/>.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property IsCustomPainted As Boolean
            Get
                Return Me.palette1.IsDark AndAlso Me.DropDownStyle = ComboBoxStyle.DropDownList
            End Get
        End Property

        ''' <summary>
        ''' Gets the bounds of the drop-down button (arrow) within the client area.
        ''' </summary>
        ''' <value>The bounds of the drop-down button.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property ButtonBounds As Rectangle
            Get
                Dim buttonWidth As Integer = SystemInformation.VerticalScrollBarWidth
                Return New Rectangle(Math.Max(0, Me.ClientSize.Width - buttonWidth - 1), 1,
                                     buttonWidth, Math.Max(0, Me.ClientSize.Height - 2))
            End Get
        End Property

        ''' <summary>
        ''' A value indicating whether the mouse pointer is over the control.
        ''' </summary>
        Private isMouseOver As Boolean

#End Region

#Region " IThemedControl "

        ''' <summary>
        ''' Applies the specified palette to the control.
        ''' </summary>
        ''' <param name="palette">The palette to apply.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="palette"/> is <see langword="Nothing"/>.</exception>
        Public Sub ApplyPalette(ByVal palette As ThemePalette) Implements IThemedControl.ApplyPalette

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            Me.palette1 = palette
            Me.UpdateRenderingMode()

        End Sub

#End Region

#Region " Overridden Methods "

        ''' <summary>
        ''' Raises the <see cref="Control.HandleCreated"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnHandleCreated(ByVal e As EventArgs)

            MyBase.OnHandleCreated(e)

            If Not Me.DesignMode Then
                Me.ApplyDropDownListTheme()
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ComboBox.DropDownStyleChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnDropDownStyleChanged(ByVal e As EventArgs)

            MyBase.OnDropDownStyleChanged(e)

            ' The base ComboBox constructor can set the DropDownStyle (raising this method) before the
            ' field initializers of this class run; the rendering mode is updated later by ApplyPalette.
            If Me.palette1 IsNot Nothing Then
                Me.UpdateRenderingMode()
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.Paint"/> event.
        ''' This method is only called when the control is custom painted.
        ''' </summary>
        ''' <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

            If Not Me.IsCustomPainted Then
                MyBase.OnPaint(e)
                Exit Sub
            End If

            Dim g As Graphics = e.Graphics
            Dim p As ThemePalette = Me.palette1
            Dim bounds As Rectangle = Me.ClientRectangle
            Dim isActive As Boolean = Me.Focused OrElse Me.DroppedDown

            ' Background.
            Dim backColor As Color = If(Not Me.Enabled, p.WindowBack,
                                        If(Me.isMouseOver OrElse Me.DroppedDown, p.ControlBack, p.InputBack))
            Using backBrush As New SolidBrush(backColor)
                g.FillRectangle(backBrush, bounds)
            End Using

            ' Border.
            Dim borderColor As Color = If(Not Me.Enabled, p.Border,
                                          If(isActive, p.Accent, If(Me.isMouseOver, p.DisabledFore, p.Border)))
            Using borderPen As New Pen(borderColor)
                g.DrawRectangle(borderPen, 0, 0, bounds.Width - 1, bounds.Height - 1)
            End Using

            ' Selected item text.
            Dim buttonRect As Rectangle = Me.ButtonBounds
            Dim textRect As New Rectangle(TextPadding, 0, Math.Max(0, buttonRect.Left - TextPadding), bounds.Height)
            Dim textColor As Color = If(Me.Enabled, p.InputFore, p.DisabledFore)
            If Me.SelectedIndex >= 0 Then
                TextRenderer.DrawText(g, Me.GetItemText(Me.SelectedItem), Me.Font, textRect, textColor,
                                      TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or
                                      TextFormatFlags.SingleLine Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)
            End If

            ' Drop-down arrow (chevron).
            Dim arrowColor As Color = If(Me.Enabled, p.ControlFore, p.DisabledFore)
            Dim centerX As Single = buttonRect.Left + (buttonRect.Width / 2.0F)
            Dim centerY As Single = buttonRect.Top + (buttonRect.Height / 2.0F)
            Dim halfWidth As Single = Math.Max(3.0F, buttonRect.Width / 5.0F)
            Dim halfHeight As Single = halfWidth / 2.0F

            g.SmoothingMode = SmoothingMode.AntiAlias
            Using arrowPen As New Pen(arrowColor, 1.6F) With {.StartCap = LineCap.Round, .EndCap = LineCap.Round, .LineJoin = LineJoin.Round}
                g.DrawLines(arrowPen, {New PointF(centerX - halfWidth, centerY - halfHeight),
                                       New PointF(centerX, centerY + halfHeight),
                                       New PointF(centerX + halfWidth, centerY - halfHeight)})
            End Using

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ComboBox.DrawItem"/> event.
        ''' This method is only called for the items of the drop-down list when the control is custom painted.
        ''' </summary>
        ''' <param name="e">The <see cref="DrawItemEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnDrawItem(ByVal e As DrawItemEventArgs)

            If Not Me.IsCustomPainted OrElse e.Index < 0 Then
                MyBase.OnDrawItem(e)
                Exit Sub
            End If

            Dim isSelected As Boolean = (e.State And DrawItemState.Selected) = DrawItemState.Selected
            Dim backColor As Color = If(isSelected, Me.palette1.SelectionBack, Me.palette1.InputBack)
            Dim foreColor As Color = If(isSelected, Me.palette1.SelectionFore, Me.palette1.InputFore)

            Using backBrush As New SolidBrush(backColor)
                e.Graphics.FillRectangle(backBrush, e.Bounds)
            End Using

            Dim textRect As New Rectangle(e.Bounds.X + TextPadding, e.Bounds.Y, Math.Max(0, e.Bounds.Width - TextPadding), e.Bounds.Height)
            TextRenderer.DrawText(e.Graphics, Me.GetItemText(Me.Items(e.Index)), Me.Font, textRect, foreColor,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or
                                  TextFormatFlags.SingleLine Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)

            MyBase.OnDrawItem(e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ComboBox.DropDown"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnDropDown(ByVal e As EventArgs)

            Me.ApplyDropDownListTheme()
            MyBase.OnDropDown(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ComboBox.DropDownClosed"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnDropDownClosed(ByVal e As EventArgs)

            MyBase.OnDropDownClosed(e)
            Me.isMouseOver = Me.ClientRectangle.Contains(Me.PointToClient(Control.MousePosition))
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ComboBox.SelectedIndexChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnSelectedIndexChanged(ByVal e As EventArgs)

            MyBase.OnSelectedIndexChanged(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.MouseEnter"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)

            MyBase.OnMouseEnter(e)
            Me.isMouseOver = True
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.MouseLeave"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)

            MyBase.OnMouseLeave(e)
            Me.isMouseOver = False
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.GotFocus"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnGotFocus(ByVal e As EventArgs)

            MyBase.OnGotFocus(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.LostFocus"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnLostFocus(ByVal e As EventArgs)

            MyBase.OnLostFocus(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.EnabledChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnEnabledChanged(ByVal e As EventArgs)

            MyBase.OnEnabledChanged(e)
            Me.Invalidate()

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Switches between the native rendering (light palette or editable styles)
        ''' and the custom rendering (dark palette with <see cref="ComboBoxStyle.DropDownList"/> style).
        ''' </summary>
        Private Sub UpdateRenderingMode()

            Dim isCustom As Boolean = Me.IsCustomPainted

            Me.FlatStyle = FlatStyle.Standard

            If Me.palette1.IsDark Then
                Me.BackColor = Me.palette1.InputBack
                Me.ForeColor = Me.palette1.InputFore
            Else
                Me.ResetBackColor()
                Me.ResetForeColor()
            End If

            ' WM_PAINT is routed to OnPaint only while the UserPaint style is set.
            Me.SetStyle(CustomPaintStyles, isCustom)

            ' The items of the drop-down list are owner-drawn in the custom mode. (Changing it recreates the handle.)
            Dim requiredDrawMode As DrawMode = If(isCustom, DrawMode.OwnerDrawFixed, DrawMode.Normal)
            If Me.DrawMode <> requiredDrawMode Then
                Me.DrawMode = requiredDrawMode
            End If

            If Me.IsHandleCreated Then
                Me.ApplyDropDownListTheme()
            End If

            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Applies the native Windows visual style that corresponds to the current palette
        ''' to the drop-down list window (dark scroll bars in the dark theme).
        ''' </summary>
        Private Sub ApplyDropDownListTheme()

            Dim info As New ThemeNativeMethods.COMBOBOXINFO()
            info.cbSize = Marshal.SizeOf(GetType(ThemeNativeMethods.COMBOBOXINFO))

            ' Best-effort cosmetic call: Windows versions without dark visual styles simply keep the default style.
            If ThemeNativeMethods.GetComboBoxInfo(Me.Handle, info) AndAlso info.hwndList <> IntPtr.Zero Then
                ThemeNativeMethods.SetWindowTheme(info.hwndList, If(Me.palette1.IsDark, "DarkMode_Explorer", Nothing), Nothing)
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
