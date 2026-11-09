' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="IconHelper.vb" company="Elektro Studios">
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

#Region " IconHelper "

Namespace UserInterface

    ''' <summary>
    ''' Provides helper methods to work with icons.
    ''' </summary>
    Friend NotInheritable Class IconHelper

        ''' <summary>
        ''' Prevents a default instance of the <see cref="IconHelper"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Creates a managed <see cref="Icon"/> from the specified bitmap without leaking the native icon handle
        ''' (<see cref="Icon.FromHandle"/> does not take ownership of the handle created by <see cref="Bitmap.GetHicon"/>).
        ''' </summary>
        ''' <param name="bitmap">The source bitmap. It is not disposed by this method.</param>
        ''' <returns>A new <see cref="Icon"/> that owns its native resources. The caller must dispose it.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="bitmap"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="Win32Exception">The native icon handle could not be destroyed.</exception>
        Friend Shared Function CreateIcon(ByVal bitmap As Bitmap) As Icon

            If bitmap Is Nothing Then
                Throw New ArgumentNullException(NameOf(bitmap))
            End If

            Dim hIcon As IntPtr = bitmap.GetHicon()
            Try
                Using tempIcon As Icon = Icon.FromHandle(hIcon)
                    Return DirectCast(tempIcon.Clone(), Icon)
                End Using

            Finally
                If Not ThemeNativeMethods.DestroyIcon(hIcon) Then
                    Throw New Win32Exception()
                End If

            End Try

        End Function

        ''' <summary>
        ''' Creates a high quality resized copy of the specified image (e.g. to show a big resource image inside a button).
        ''' </summary>
        ''' <param name="image">The source image. It is not disposed by this method.</param>
        ''' <param name="size">The size of the new image, in pixels.</param>
        ''' <returns>A new <see cref="Bitmap"/> of the specified size. The caller must dispose it.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is not a positive size.</exception>
        Friend Shared Function ResizeImage(ByVal image As Image, ByVal size As Size) As Bitmap

            If image Is Nothing Then
                Throw New ArgumentNullException(NameOf(image))
            End If

            If size.Width <= 0 OrElse size.Height <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(size), size, "The size must be positive.")
            End If

            Dim result As New Bitmap(size.Width, size.Height, Imaging.PixelFormat.Format32bppArgb)
            Try
                Using g As Graphics = Graphics.FromImage(result)
                    g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                    g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                    g.DrawImage(image, New Rectangle(Point.Empty, size))
                End Using
                Return result

            Catch
                result.Dispose()
                Throw

            End Try

        End Function

    End Class

End Namespace

#End Region
