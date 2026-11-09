' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemedToolStripRenderer.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " ThemedToolStripRenderer "

Namespace UserInterface.Theming

    ''' <summary>
    ''' A <see cref="ToolStripProfessionalRenderer"/> that paints tool strips, menus and context menus
    ''' with the colors of a <see cref="ThemePalette"/>.
    ''' </summary>
    Public NotInheritable Class ThemedToolStripRenderer : Inherits ToolStripProfessionalRenderer

#Region " Properties "

        ''' <summary>
        ''' Gets the palette used to paint.
        ''' </summary>
        ''' <value>The palette.</value>
        Public ReadOnly Property Palette As ThemePalette

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ThemedToolStripRenderer"/> class.
        ''' </summary>
        ''' <param name="palette">The palette used to paint.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="palette"/> is <see langword="Nothing"/>.</exception>
        Public Sub New(ByVal palette As ThemePalette)

            MyBase.New(New ThemedColorTable(palette))
            Me.Palette = palette
            Me.RoundedEdges = False

        End Sub

#End Region

#Region " Overridden Methods "

        ''' <summary>
        ''' Raises the <see cref="ToolStripRenderer.RenderItemText"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="ToolStripItemTextRenderEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnRenderItemText(ByVal e As ToolStripItemTextRenderEventArgs)

            e.TextColor = If(e.Item.Enabled, Me.Palette.ControlFore, Me.Palette.DisabledFore)
            MyBase.OnRenderItemText(e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ToolStripRenderer.RenderArrow"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="ToolStripArrowRenderEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnRenderArrow(ByVal e As ToolStripArrowRenderEventArgs)

            e.ArrowColor = If(e.Item Is Nothing OrElse e.Item.Enabled, Me.Palette.ControlFore, Me.Palette.DisabledFore)
            MyBase.OnRenderArrow(e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ToolStripRenderer.RenderSeparator"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="ToolStripSeparatorRenderEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnRenderSeparator(ByVal e As ToolStripSeparatorRenderEventArgs)

            Dim bounds As New Rectangle(Point.Empty, e.Item.Size)

            Using pen As New Pen(Me.Palette.Border)
                If e.Vertical Then
                    Dim x As Integer = bounds.Width \ 2
                    e.Graphics.DrawLine(pen, x, bounds.Top + 4, x, bounds.Bottom - 4)
                Else
                    Dim y As Integer = bounds.Height \ 2
                    Dim left As Integer = If(TypeOf e.ToolStrip Is ToolStripDropDownMenu, e.ToolStrip.Padding.Left + 26, bounds.Left + 4)
                    e.Graphics.DrawLine(pen, left, y, bounds.Right - 4, y)
                End If
            End Using

        End Sub

        ''' <summary>
        ''' Raises the <see cref="ToolStripRenderer.RenderToolStripBorder"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="ToolStripRenderEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnRenderToolStripBorder(ByVal e As ToolStripRenderEventArgs)

            Dim bounds As New Rectangle(Point.Empty, e.ToolStrip.Size)

            Using pen As New Pen(Me.Palette.Border)
                If TypeOf e.ToolStrip Is ToolStripDropDown Then
                    e.Graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1)
                Else
                    e.Graphics.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1)
                End If
            End Using

        End Sub

#End Region

    End Class

End Namespace

#End Region
