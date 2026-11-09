' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ColorContrast.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " ColorContrast "

Namespace TestHelpers

    ''' <summary>
    ''' Computes color luminance and contrast ratios as defined by the WCAG 2.x accessibility guidelines.
    ''' </summary>
    Friend NotInheritable Class ColorContrast

        ''' <summary>
        ''' Prevents a default instance of the <see cref="ColorContrast"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Gets the relative luminance (0 = black, 1 = white) of the specified color.
        ''' </summary>
        ''' <param name="color">The color.</param>
        ''' <returns>The relative luminance.</returns>
        Friend Shared Function GetRelativeLuminance(ByVal color As Color) As Double

            Return (0.2126 * ColorContrast.ToLinear(color.R)) +
                   (0.7152 * ColorContrast.ToLinear(color.G)) +
                   (0.0722 * ColorContrast.ToLinear(color.B))

        End Function

        ''' <summary>
        ''' Gets the contrast ratio (1 to 21) between two colors.
        ''' </summary>
        ''' <param name="colorA">The first color.</param>
        ''' <param name="colorB">The second color.</param>
        ''' <returns>The contrast ratio.</returns>
        Friend Shared Function GetContrastRatio(ByVal colorA As Color, ByVal colorB As Color) As Double

            Dim luminanceA As Double = ColorContrast.GetRelativeLuminance(colorA)
            Dim luminanceB As Double = ColorContrast.GetRelativeLuminance(colorB)

            Return (Math.Max(luminanceA, luminanceB) + 0.05) / (Math.Min(luminanceA, luminanceB) + 0.05)

        End Function

        ''' <summary>
        ''' Converts an sRGB channel value to its linear value.
        ''' </summary>
        ''' <param name="channel">The sRGB channel value (0-255).</param>
        ''' <returns>The linear value (0-1).</returns>
        Private Shared Function ToLinear(ByVal channel As Byte) As Double

            Dim value As Double = channel / 255.0
            Return If(value <= 0.03928, value / 12.92, Math.Pow((value + 0.055) / 1.055, 2.4))

        End Function

    End Class

End Namespace

#End Region
