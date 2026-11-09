' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="IThemedControl.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " IThemedControl "

Namespace UserInterface.Theming

    ''' <summary>
    ''' Defines a control that knows how to apply a <see cref="ThemePalette"/> to itself
    ''' (e.g. custom painted controls, or controls that need native theming calls).
    ''' <para></para>
    ''' <see cref="ThemeManager.ApplyTheme"/> calls <see cref="ApplyPalette"/> after
    ''' applying the generic theming rules to the control.
    ''' </summary>
    Public Interface IThemedControl

        ''' <summary>
        ''' Applies the specified palette to the control.
        ''' </summary>
        ''' <param name="palette">The palette to apply.</param>
        Sub ApplyPalette(ByVal palette As ThemePalette)

    End Interface

End Namespace

#End Region
