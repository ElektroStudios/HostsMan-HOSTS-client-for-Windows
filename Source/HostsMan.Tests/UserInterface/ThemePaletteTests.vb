' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemePaletteTests.vb" company="Elektro Studios">
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

Imports HostsMan.Tests.TestHelpers
Imports HostsMan.UserInterface.Theming

#End Region

#Region " ThemePaletteTests "

Namespace UserInterface

    ''' <summary>
    ''' Tests for <see cref="ThemePalette"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class ThemePaletteTests

        ''' <summary>
        ''' The minimum WCAG AA contrast ratio for normal text.
        ''' </summary>
        Private Const MinTextContrast As Double = 4.5

        ''' <summary>
        ''' The minimum WCAG AA contrast ratio for user interface components.
        ''' </summary>
        Private Const MinComponentContrast As Double = 3.0

        <TestMethod>
        Public Sub FromTheme_ReturnsTheSharedInstances()

            Assert.AreSame(ThemePalette.Light, ThemePalette.FromTheme(AppTheme.Light))
            Assert.AreSame(ThemePalette.Dark, ThemePalette.FromTheme(AppTheme.Dark))

        End Sub

        <TestMethod>
        Public Sub FromTheme_UndefinedValue_ThrowsInvalidEnumArgumentException()

            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() ThemePalette.FromTheme(CType(7, AppTheme)))

        End Sub

        <TestMethod>
        Public Sub ThemeAndIsDark_AreConsistent()

            Assert.AreEqual(AppTheme.Light, ThemePalette.Light.Theme)
            Assert.IsFalse(ThemePalette.Light.IsDark)
            Assert.AreEqual(AppTheme.Dark, ThemePalette.Dark.Theme)
            Assert.IsTrue(ThemePalette.Dark.IsDark)

        End Sub

        <TestMethod>
        Public Sub LightPalette_UsesTheWindowsSystemColors()

            Dim palette As ThemePalette = ThemePalette.Light

            Assert.AreEqual(SystemColors.Control, palette.WindowBack)
            Assert.AreEqual(SystemColors.ControlText, palette.WindowFore)
            Assert.AreEqual(SystemColors.Window, palette.InputBack)
            Assert.AreEqual(SystemColors.WindowText, palette.InputFore)
            Assert.AreEqual(SystemColors.Highlight, palette.SelectionBack)
            Assert.AreEqual(SystemColors.HighlightText, palette.SelectionFore)
            Assert.AreEqual(SystemColors.GrayText, palette.DisabledFore)
            Assert.IsTrue(palette.WindowBack.IsSystemColor)
            Assert.IsTrue(palette.InputBack.IsSystemColor)

        End Sub

        <TestMethod>
        Public Sub DarkPalette_BackgroundsAreDarkAndTextsAreLight()

            Dim palette As ThemePalette = ThemePalette.Dark

            For Each background As Color In {palette.WindowBack, palette.InputBack, palette.ControlBack, palette.ToolStripBack, palette.HeaderBack}
                Assert.IsTrue(ColorContrast.GetRelativeLuminance(background) < 0.05, $"Background {background} is not dark.")
            Next background

            For Each foreground As Color In {palette.WindowFore, palette.InputFore, palette.ControlFore, palette.HeaderFore, palette.SelectionFore}
                Assert.IsTrue(ColorContrast.GetRelativeLuminance(foreground) > 0.7, $"Foreground {foreground} is not light.")
            Next foreground

        End Sub

        <TestMethod>
        Public Sub DarkPalette_TextColors_MeetTheWcagContrastRatio()

            Dim p As ThemePalette = ThemePalette.Dark

            Dim pairs As New Dictionary(Of String, Tuple(Of Color, Color)) From {
                {"WindowFore/WindowBack", Tuple.Create(p.WindowFore, p.WindowBack)},
                {"InputFore/InputBack", Tuple.Create(p.InputFore, p.InputBack)},
                {"ControlFore/ControlBack", Tuple.Create(p.ControlFore, p.ControlBack)},
                {"ControlFore/HoverBack", Tuple.Create(p.ControlFore, p.HoverBack)},
                {"ControlFore/PressedBack", Tuple.Create(p.ControlFore, p.PressedBack)},
                {"ControlFore/ToolStripBack", Tuple.Create(p.ControlFore, p.ToolStripBack)},
                {"HeaderFore/HeaderBack", Tuple.Create(p.HeaderFore, p.HeaderBack)},
                {"SelectionFore/SelectionBack", Tuple.Create(p.SelectionFore, p.SelectionBack)},
                {"SelectionFore/SelectionInactiveBack", Tuple.Create(p.SelectionFore, p.SelectionInactiveBack)},
                {"DisabledFore/WindowBack", Tuple.Create(p.DisabledFore, p.WindowBack)},
                {"Link/WindowBack", Tuple.Create(p.Link, p.WindowBack)},
                {"ErrorFore/InputBack", Tuple.Create(p.ErrorFore, p.InputBack)}
            }

            For Each pair As KeyValuePair(Of String, Tuple(Of Color, Color)) In pairs
                Dim ratio As Double = ColorContrast.GetContrastRatio(pair.Value.Item1, pair.Value.Item2)
                Assert.IsTrue(ratio >= MinTextContrast, $"{pair.Key} contrast ratio is {ratio:0.00}.")
            Next pair

        End Sub

        <TestMethod>
        Public Sub BothPalettes_SwitchThumb_IsDistinguishableFromTheTrack()

            ' The light palette depends on the user system colors, so only a minimum distinction is required.
            For Each p As ThemePalette In {ThemePalette.Light, ThemePalette.Dark}
                Dim minContrast As Double = If(p.IsDark, MinComponentContrast, 2.0)
                Assert.IsTrue(ColorContrast.GetContrastRatio(p.SwitchThumb, p.Accent) >= minContrast, $"{p.Theme}: thumb/accent")
                Assert.IsTrue(ColorContrast.GetContrastRatio(p.SwitchThumb, p.SwitchOffTrack) >= 2.0, $"{p.Theme}: thumb/off track")
            Next p

        End Sub

    End Class

End Namespace

#End Region
