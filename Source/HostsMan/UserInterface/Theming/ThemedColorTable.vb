' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemedColorTable.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " ThemedColorTable "

Namespace UserInterface.Theming

    ''' <summary>
    ''' A <see cref="ProfessionalColorTable"/> whose colors are taken from a <see cref="ThemePalette"/>.
    ''' </summary>
    Public NotInheritable Class ThemedColorTable : Inherits ProfessionalColorTable

#Region " Properties "

        ''' <summary>
        ''' Gets the palette that provides the colors.
        ''' </summary>
        ''' <value>The palette.</value>
        Public ReadOnly Property Palette As ThemePalette

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ThemedColorTable"/> class.
        ''' </summary>
        ''' <param name="palette">The palette that provides the colors.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="palette"/> is <see langword="Nothing"/>.</exception>
        Public Sub New(ByVal palette As ThemePalette)

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            Me.Palette = palette
            Me.UseSystemColors = False

        End Sub

#End Region

#Region " Tool Strip "

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripGradientBegin As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripGradientMiddle As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripGradientEnd As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripBorder As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripContentPanelGradientBegin As Color
            Get
                Return Me.Palette.WindowBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripContentPanelGradientEnd As Color
            Get
                Return Me.Palette.WindowBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripPanelGradientBegin As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ToolStripPanelGradientEnd As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property OverflowButtonGradientBegin As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property OverflowButtonGradientMiddle As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property OverflowButtonGradientEnd As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property GripDark As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property GripLight As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property SeparatorDark As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property SeparatorLight As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StatusStripGradientBegin As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StatusStripGradientEnd As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

#End Region

#Region " Buttons "

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonSelectedHighlight As Color
            Get
                Return Me.Palette.HoverBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonSelectedHighlightBorder As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonSelectedGradientBegin As Color
            Get
                Return Me.Palette.HoverBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonSelectedGradientMiddle As Color
            Get
                Return Me.Palette.HoverBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonSelectedGradientEnd As Color
            Get
                Return Me.Palette.HoverBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonSelectedBorder As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonPressedHighlight As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonPressedHighlightBorder As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonPressedGradientBegin As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonPressedGradientMiddle As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonPressedGradientEnd As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonPressedBorder As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonCheckedHighlight As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonCheckedHighlightBorder As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonCheckedGradientBegin As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonCheckedGradientMiddle As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ButtonCheckedGradientEnd As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property CheckBackground As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property CheckSelectedBackground As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property CheckPressedBackground As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

#End Region

#Region " Menus "

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuBorder As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuItemBorder As Color
            Get
                Return Me.Palette.Border
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuItemSelected As Color
            Get
                Return Me.Palette.HoverBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuItemSelectedGradientBegin As Color
            Get
                Return Me.Palette.HoverBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuItemSelectedGradientEnd As Color
            Get
                Return Me.Palette.HoverBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuItemPressedGradientBegin As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuItemPressedGradientMiddle As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuItemPressedGradientEnd As Color
            Get
                Return Me.Palette.PressedBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuStripGradientBegin As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property MenuStripGradientEnd As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
            Get
                Return Me.Palette.ToolStripBack
            End Get
        End Property

#End Region

    End Class

End Namespace

#End Region
