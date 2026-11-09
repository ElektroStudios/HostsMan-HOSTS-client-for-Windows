' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemePalette.vb" company="Elektro Studios">
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

#Region " ThemePalette "

Namespace UserInterface.Theming

    ''' <summary>
    ''' Defines the set of colors of an <see cref="AppTheme"/>.
    ''' <para></para>
    ''' The <see cref="Light"/> palette uses the Windows <see cref="SystemColors"/>,
    ''' so it always follows the current Windows default colors.
    ''' </summary>
    Public NotInheritable Class ThemePalette

#Region " Shared Instances "

        ''' <summary>
        ''' Gets the palette of the <see cref="AppTheme.Light"/> theme (Windows default system colors).
        ''' </summary>
        ''' <value>The light palette.</value>
        Public Shared ReadOnly Property Light As ThemePalette = New ThemePalette(AppTheme.Light)

        ''' <summary>
        ''' Gets the palette of the <see cref="AppTheme.Dark"/> theme.
        ''' </summary>
        ''' <value>The dark palette.</value>
        Public Shared ReadOnly Property Dark As ThemePalette = New ThemePalette(AppTheme.Dark)

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the theme that this palette represents.
        ''' </summary>
        Public ReadOnly Property Theme As AppTheme

        ''' <summary>
        ''' Gets a value indicating whether this is a dark palette.
        ''' </summary>
        Public ReadOnly Property IsDark As Boolean
            Get
                Return Me.Theme = AppTheme.Dark
            End Get
        End Property

        ''' <summary>
        ''' Gets the background color of the windows (forms and containers).
        ''' </summary>
        Public ReadOnly Property WindowBack As Color

        ''' <summary>
        ''' Gets the foreground (text) color of the windows (forms, containers and labels).
        ''' </summary>
        Public ReadOnly Property WindowFore As Color

        ''' <summary>
        ''' Gets the background color of the buttons.
        ''' </summary>
        Public ReadOnly Property ControlBack As Color

        ''' <summary>
        ''' Gets the foreground (text) color of the buttons and tool strip items.
        ''' </summary>
        Public ReadOnly Property ControlFore As Color

        ''' <summary>
        ''' Gets the background color of a control under the mouse pointer.
        ''' </summary>
        Public ReadOnly Property HoverBack As Color

        ''' <summary>
        ''' Gets the background color of a pressed control.
        ''' </summary>
        Public ReadOnly Property PressedBack As Color

        ''' <summary>
        ''' Gets the background color of the input controls (text boxes, combo boxes and lists).
        ''' </summary>
        Public ReadOnly Property InputBack As Color

        ''' <summary>
        ''' Gets the foreground (text) color of the input controls.
        ''' </summary>
        Public ReadOnly Property InputFore As Color

        ''' <summary>
        ''' Gets the background color of the tool strips and menus.
        ''' </summary>
        Public ReadOnly Property ToolStripBack As Color

        ''' <summary>
        ''' Gets the background color of the list column headers.
        ''' </summary>
        Public ReadOnly Property HeaderBack As Color

        ''' <summary>
        ''' Gets the foreground (text) color of the list column headers.
        ''' </summary>
        Public ReadOnly Property HeaderFore As Color

        ''' <summary>
        ''' Gets the color of the borders and separators.
        ''' </summary>
        Public ReadOnly Property Border As Color

        ''' <summary>
        ''' Gets the color of the list grid lines.
        ''' </summary>
        Public ReadOnly Property GridLine As Color

        ''' <summary>
        ''' Gets the background color of the selected items of a focused list.
        ''' </summary>
        Public ReadOnly Property SelectionBack As Color

        ''' <summary>
        ''' Gets the foreground (text) color of the selected items.
        ''' </summary>
        Public ReadOnly Property SelectionFore As Color

        ''' <summary>
        ''' Gets the background color of the selected items of a list that does not have the focus.
        ''' </summary>
        Public ReadOnly Property SelectionInactiveBack As Color

        ''' <summary>
        ''' Gets the foreground (text) color of the disabled controls.
        ''' </summary>
        Public ReadOnly Property DisabledFore As Color

        ''' <summary>
        ''' Gets the accent color (e.g. the track of a checked toggle switch).
        ''' </summary>
        Public ReadOnly Property Accent As Color

        ''' <summary>
        ''' Gets the track color of an unchecked toggle switch.
        ''' </summary>
        Public ReadOnly Property SwitchOffTrack As Color

        ''' <summary>
        ''' Gets the thumb color of a toggle switch.
        ''' </summary>
        Public ReadOnly Property SwitchThumb As Color

        ''' <summary>
        ''' Gets the color of the links.
        ''' </summary>
        Public ReadOnly Property Link As Color

        ''' <summary>
        ''' Gets the color of the active (clicked) links.
        ''' </summary>
        Public ReadOnly Property LinkActive As Color

        ''' <summary>
        ''' Gets the foreground (text) color used to highlight invalid input.
        ''' </summary>
        Public ReadOnly Property ErrorFore As Color

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ThemePalette"/> class.
        ''' </summary>
        ''' <param name="theme">The theme.</param>
        Private Sub New(ByVal theme As AppTheme)

            Me.Theme = theme

            Select Case theme

                Case AppTheme.Light
                    Me.WindowBack = SystemColors.Control
                    Me.WindowFore = SystemColors.ControlText
                    Me.ControlBack = SystemColors.Control
                    Me.ControlFore = SystemColors.ControlText
                    Me.HoverBack = SystemColors.ControlLight
                    Me.PressedBack = SystemColors.ControlDark
                    Me.InputBack = SystemColors.Window
                    Me.InputFore = SystemColors.WindowText
                    Me.ToolStripBack = SystemColors.Control
                    Me.HeaderBack = SystemColors.Control
                    Me.HeaderFore = SystemColors.ControlText
                    Me.Border = SystemColors.ControlDark
                    Me.GridLine = SystemColors.ControlLight
                    Me.SelectionBack = SystemColors.Highlight
                    Me.SelectionFore = SystemColors.HighlightText
                    Me.SelectionInactiveBack = SystemColors.Control
                    Me.DisabledFore = SystemColors.GrayText
                    Me.Accent = SystemColors.Highlight
                    Me.SwitchOffTrack = SystemColors.ControlDark
                    Me.SwitchThumb = Color.White
                    Me.Link = SystemColors.HotTrack
                    Me.LinkActive = Color.Red
                    Me.ErrorFore = Color.FromArgb(192, 0, 0)

                Case AppTheme.Dark
                    Me.WindowBack = Color.FromArgb(32, 32, 32)
                    Me.WindowFore = Color.FromArgb(240, 240, 240)
                    Me.ControlBack = Color.FromArgb(51, 51, 51)
                    Me.ControlFore = Color.FromArgb(240, 240, 240)
                    Me.HoverBack = Color.FromArgb(69, 69, 69)
                    Me.PressedBack = Color.FromArgb(85, 85, 85)
                    Me.InputBack = Color.FromArgb(43, 43, 43)
                    Me.InputFore = Color.FromArgb(240, 240, 240)
                    Me.ToolStripBack = Color.FromArgb(38, 38, 38)
                    Me.HeaderBack = Color.FromArgb(51, 51, 51)
                    Me.HeaderFore = Color.FromArgb(225, 225, 225)
                    Me.Border = Color.FromArgb(85, 85, 85)
                    Me.GridLine = Color.FromArgb(60, 60, 60)
                    Me.SelectionBack = Color.FromArgb(0, 95, 184)
                    Me.SelectionFore = Color.White
                    Me.SelectionInactiveBack = Color.FromArgb(70, 70, 70)
                    Me.DisabledFore = Color.FromArgb(140, 140, 140)
                    Me.Accent = Color.FromArgb(0, 120, 212)
                    Me.SwitchOffTrack = Color.FromArgb(90, 90, 90)
                    Me.SwitchThumb = Color.White
                    Me.Link = Color.FromArgb(76, 194, 255)
                    Me.LinkActive = Color.FromArgb(153, 235, 255)
                    Me.ErrorFore = Color.FromArgb(255, 107, 107)

                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(theme), CInt(theme), GetType(AppTheme))

            End Select

        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Gets the palette of the specified theme.
        ''' </summary>
        ''' <param name="theme">The theme.</param>
        ''' <returns>The palette of the theme.</returns>
        ''' <exception cref="InvalidEnumArgumentException"><paramref name="theme"/> is not a defined <see cref="AppTheme"/>.</exception>
        Public Shared Function FromTheme(ByVal theme As AppTheme) As ThemePalette

            Select Case theme
                Case AppTheme.Light
                    Return ThemePalette.Light
                Case AppTheme.Dark
                    Return ThemePalette.Dark
                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(theme), CInt(theme), GetType(AppTheme))
            End Select

        End Function

#End Region

    End Class

End Namespace

#End Region
