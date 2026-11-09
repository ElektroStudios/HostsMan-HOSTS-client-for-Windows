' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ApplicationEvents.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.VisualBasic.ApplicationServices

Imports HostsMan.UserInterface.Theming

#End Region

#Region " MyApplication "

Namespace My

    ''' <summary>
    ''' Handles the application events.
    ''' </summary>
    Partial Friend Class MyApplication

        ''' <summary>
        ''' Handles the Startup event of the application.
        ''' <para></para>
        ''' The theme saved in the user settings is restored here, before the main form is created,
        ''' so the forms are created (and their title bars painted) directly with the right theme.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As StartupEventArgs) _
        Handles Me.Startup

            ThemeManager.CurrentTheme = My.Settings.AppTheme

        End Sub

    End Class

End Namespace

#End Region
