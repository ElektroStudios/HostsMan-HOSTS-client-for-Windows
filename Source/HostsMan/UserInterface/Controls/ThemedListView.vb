' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemedListView.vb" company="Elektro Studios">
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

#Region " ThemedListView "

Namespace UserInterface.Controls

    ''' <summary>
    ''' A <see cref="ListView"/> that supports the application themes.
    ''' <para></para>
    ''' In the light theme the control is drawn natively by Windows.
    ''' In the dark theme the column headers, items, selection and grid lines are owner-drawn
    ''' with the colors of the <see cref="ThemePalette"/>, and the scroll bars use the Windows dark visual style.
    ''' </summary>
    <DesignerCategory("Code")>
    <ToolboxItem(True)>
    Public Class ThemedListView : Inherits ListView : Implements IThemedControl

#Region " Constants "

        ''' <summary>
        ''' The horizontal padding, in pixels, of the owner-drawn texts.
        ''' </summary>
        Private Const TextPadding As Integer = 6

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
        ''' Gets a value indicating whether the grid lines are owner-drawn (dark theme with <see cref="ListView.GridLines"/> requested).
        ''' </summary>
        ''' <value><see langword="True"/> if the grid lines are owner-drawn; otherwise, <see langword="False"/>.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property DrawsCustomGridLines As Boolean
            Get
                Return Me.palette1.IsDark AndAlso Me.requestedGridLines
            End Get
        End Property

        ''' <summary>
        ''' The value of <see cref="ListView.GridLines"/> before the dark palette disabled the native grid lines.
        ''' </summary>
        Private requestedGridLines As Boolean

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ThemedListView"/> class.
        ''' </summary>
        Public Sub New()

            MyBase.New()
            Me.DoubleBuffered = True

        End Sub

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

            Dim wasDark As Boolean = Me.palette1.IsDark
            Me.palette1 = palette

            If palette.IsDark Then
                ' Native grid lines are drawn with a light system color; they are owner-drawn instead.
                If Not wasDark Then
                    Me.requestedGridLines = MyBase.GridLines
                End If
                MyBase.GridLines = False
                Me.BackColor = palette.InputBack
                Me.ForeColor = palette.InputFore
                Me.OwnerDraw = True

            Else
                If wasDark Then
                    MyBase.GridLines = Me.requestedGridLines
                End If
                Me.ResetBackColor()
                Me.ResetForeColor()
                Me.OwnerDraw = False

            End If

            If Me.IsHandleCreated Then
                Me.ApplyWindowTheme()
            End If

            Me.Invalidate()

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
                Me.ApplyWindowTheme()
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ListView.DrawColumnHeader"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="DrawListViewColumnHeaderEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnDrawColumnHeader(ByVal e As DrawListViewColumnHeaderEventArgs)

            If Not Me.palette1.IsDark Then
                e.DrawDefault = True
                MyBase.OnDrawColumnHeader(e)
                Exit Sub
            End If

            Dim bounds As Rectangle = e.Bounds

            Using backBrush As New SolidBrush(Me.palette1.HeaderBack),
                  borderPen As New Pen(Me.palette1.Border)

                e.Graphics.FillRectangle(backBrush, bounds)
                e.Graphics.DrawLine(borderPen, bounds.Right - 1, bounds.Top + 3, bounds.Right - 1, bounds.Bottom - 4)
                e.Graphics.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1)

            End Using

            If e.Header IsNot Nothing Then
                Dim textBounds As New Rectangle(bounds.X + TextPadding, bounds.Y, Math.Max(0, bounds.Width - (TextPadding * 2)), bounds.Height)
                TextRenderer.DrawText(e.Graphics, e.Header.Text, Me.Font, textBounds, Me.palette1.HeaderFore,
                                      ThemedListView.GetTextFormatFlags(e.Header.TextAlign))
            End If

            MyBase.OnDrawColumnHeader(e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ListView.DrawItem"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="DrawListViewItemEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnDrawItem(ByVal e As DrawListViewItemEventArgs)

            ' In details view the item is fully painted by OnDrawSubItem.
            e.DrawDefault = Not (Me.palette1.IsDark AndAlso Me.View = View.Details)
            MyBase.OnDrawItem(e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ListView.DrawSubItem"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="DrawListViewSubItemEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnDrawSubItem(ByVal e As DrawListViewSubItemEventArgs)

            If Not Me.palette1.IsDark Then
                e.DrawDefault = True
                MyBase.OnDrawSubItem(e)
                Exit Sub
            End If

            Dim bounds As Rectangle = e.Bounds
            If e.ColumnIndex = 0 AndAlso Me.Columns.Count <> 0 Then
                bounds.Width = Me.Columns(0).Width
            End If

            Dim isSelectionVisible As Boolean = e.Item.Selected AndAlso (Me.Focused OrElse Not Me.HideSelection)
            Dim backColor As Color = If(isSelectionVisible,
                                        If(Me.Focused, Me.palette1.SelectionBack, Me.palette1.SelectionInactiveBack),
                                        Me.BackColor)
            Dim foreColor As Color = If(isSelectionVisible, Me.palette1.SelectionFore, Me.ForeColor)

            Using backBrush As New SolidBrush(backColor)
                e.Graphics.FillRectangle(backBrush, bounds)
            End Using

            If Me.requestedGridLines Then
                Using gridPen As New Pen(Me.palette1.GridLine)
                    e.Graphics.DrawLine(gridPen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1)
                    e.Graphics.DrawLine(gridPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1)
                End Using
            End If

            Dim text As String = If(e.SubItem IsNot Nothing, e.SubItem.Text, e.Item.Text)
            Dim alignment As HorizontalAlignment = If(e.Header IsNot Nothing, e.Header.TextAlign, HorizontalAlignment.Left)
            Dim textBounds As New Rectangle(bounds.X + TextPadding, bounds.Y, Math.Max(0, bounds.Width - (TextPadding * 2)), bounds.Height)

            TextRenderer.DrawText(e.Graphics, text, Me.Font, textBounds, foreColor, ThemedListView.GetTextFormatFlags(alignment))

            MyBase.OnDrawSubItem(e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.GotFocus"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnGotFocus(ByVal e As EventArgs)

            MyBase.OnGotFocus(e)
            If Me.palette1.IsDark Then
                Me.Invalidate()
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.LostFocus"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnLostFocus(ByVal e As EventArgs)

            MyBase.OnLostFocus(e)
            If Me.palette1.IsDark Then
                Me.Invalidate()
            End If

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Applies the native Windows visual style that corresponds to the current palette
        ''' to the list (scroll bars) and to its header control.
        ''' </summary>
        Private Sub ApplyWindowTheme()

            Dim isDark As Boolean = Me.palette1.IsDark

            ' Best-effort cosmetic calls: Windows versions without dark visual styles simply keep the default style.
            ThemeNativeMethods.SetWindowTheme(Me.Handle, If(isDark, "DarkMode_Explorer", Nothing), Nothing)

            Dim headerHandle As IntPtr = ThemeNativeMethods.SendMessage(Me.Handle, ThemeNativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero)
            If headerHandle <> IntPtr.Zero Then
                ThemeNativeMethods.SetWindowTheme(headerHandle, If(isDark, "DarkMode_ItemsView", Nothing), Nothing)
            End If

        End Sub

        ''' <summary>
        ''' Gets the <see cref="TextFormatFlags"/> to draw a single line text with the specified alignment.
        ''' </summary>
        ''' <param name="alignment">The horizontal alignment.</param>
        ''' <returns>The <see cref="TextFormatFlags"/>.</returns>
        Friend Shared Function GetTextFormatFlags(ByVal alignment As HorizontalAlignment) As TextFormatFlags

            Dim flags As TextFormatFlags = TextFormatFlags.SingleLine Or
                                           TextFormatFlags.VerticalCenter Or
                                           TextFormatFlags.EndEllipsis Or
                                           TextFormatFlags.NoPrefix

            Select Case alignment
                Case HorizontalAlignment.Center
                    Return flags Or TextFormatFlags.HorizontalCenter
                Case HorizontalAlignment.Right
                    Return flags Or TextFormatFlags.Right
                Case Else
                    Return flags Or TextFormatFlags.Left
            End Select

        End Function

#End Region

    End Class

End Namespace

#End Region
