' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemedForm.vb" company="Elektro Studios">
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

#Region " ThemedForm "

Namespace UserInterface.Controls

    ''' <summary>
    ''' A <see cref="Form"/> that automatically applies the <see cref="ThemeManager.CurrentTheme"/>
    ''' to itself and its controls, including the dark title bar, and that reacts to theme changes at run-time.
    ''' </summary>
    Public Class ThemedForm : Inherits Form : Implements IThemedControl

#Region " Properties "

        ''' <summary>
        ''' Gets a value indicating whether the dark title bar was applied by the operating system
        ''' the last time the palette was applied (<see langword="False"/> on unsupported Windows versions).
        ''' </summary>
        ''' <value><see langword="True"/> if the dark title bar is active; otherwise, <see langword="False"/>.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property IsDarkTitleBarActive As Boolean

        ''' <summary>
        ''' Gets a value indicating whether the form is hosted by a designer (Visual Studio).
        ''' </summary>
        ''' <value><see langword="True"/> if the form is in design mode; otherwise, <see langword="False"/>.</value>
        Private ReadOnly Property IsInDesignMode As Boolean
            Get
                Return Me.DesignMode OrElse LicenseManager.UsageMode = LicenseUsageMode.Designtime
            End Get
        End Property

        ''' <summary>
        ''' A value indicating whether this form is subscribed to the <see cref="ThemeManager.ThemeChanged"/> event.
        ''' </summary>
        Private isSubscribed As Boolean

#End Region

#Region " IThemedControl "

        ''' <summary>
        ''' Applies the specified palette to the form itself: background, foreground and title bar.
        ''' <para></para>
        ''' This method is called by <see cref="ThemeManager.ApplyTheme(Control, AppTheme)"/>.
        ''' </summary>
        ''' <param name="palette">The palette to apply.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="palette"/> is <see langword="Nothing"/>.</exception>
        Public Overridable Sub ApplyPalette(ByVal palette As ThemePalette) Implements IThemedControl.ApplyPalette

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            If palette.IsDark Then
                Me.BackColor = palette.WindowBack
                Me.ForeColor = palette.WindowFore
            Else
                Me.ResetBackColor()
                Me.ResetForeColor()
            End If

            If Me.IsHandleCreated AndAlso Not Me.IsInDesignMode Then
                Me._IsDarkTitleBarActive = ThemeManager.SetTitleBarDarkMode(Me, palette.IsDark) AndAlso palette.IsDark
            End If

        End Sub

#End Region

#Region " Overridden Methods "

        ''' <summary>
        ''' Raises the <see cref="Control.HandleCreated"/> event.
        ''' <para></para>
        ''' The theme (and the DWM dark title bar attribute) is applied here, because applying it
        ''' from the <see cref="Form.Load"/> event has no effect on the title bar.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnHandleCreated(ByVal e As EventArgs)

            MyBase.OnHandleCreated(e)

            If Me.IsInDesignMode Then
                Exit Sub
            End If

            ThemeManager.ApplyTheme(Me, ThemeManager.CurrentTheme)

            If Not Me.isSubscribed Then
                AddHandler ThemeManager.ThemeChanged, AddressOf Me.ThemeManager_ThemeChanged
                Me.isSubscribed = True
            End If

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.HandleDestroyed"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnHandleDestroyed(ByVal e As EventArgs)

            If Me.isSubscribed AndAlso Not Me.RecreatingHandle Then
                RemoveHandler ThemeManager.ThemeChanged, AddressOf Me.ThemeManager_ThemeChanged
                Me.isSubscribed = False
            End If

            MyBase.OnHandleDestroyed(e)

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the <see cref="ThemeManager.ThemeChanged"/> event.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub ThemeManager_ThemeChanged(ByVal sender As Object, ByVal e As EventArgs)

            If Me.IsDisposed OrElse Not Me.IsHandleCreated Then
                Exit Sub
            End If

            If Me.InvokeRequired Then
                Me.BeginInvoke(New MethodInvoker(AddressOf Me.ApplyCurrentTheme))
            Else
                Me.ApplyCurrentTheme()
            End If

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Applies the <see cref="ThemeManager.CurrentTheme"/> to the form and forces the repaint of its title bar.
        ''' </summary>
        Private Sub ApplyCurrentTheme()

            ThemeManager.ApplyTheme(Me, ThemeManager.CurrentTheme)

            If Me.Visible Then
                ThemeManager.RefreshTitleBar(Me)
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
