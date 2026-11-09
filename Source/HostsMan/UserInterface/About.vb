' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="About.vb" company="Elektro Studios">
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

#Region " About "

''' <summary>
''' The About form.
''' </summary>
Public NotInheritable Class About

#Region " Constants "

    ''' <summary>
    ''' The URL of the official website (GitHub repository).
    ''' </summary>
    Friend Const WebsiteUrl As String = "https://github.com/ElektroStudios/HostsMan-HOSTS-client-for-Windows"

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="About"/> class.
    ''' </summary>
    Public Sub New()

        ' This call is required by the designer.
        Me.InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Label4.Text = About.GetProductTitle(GetType(About).Assembly.GetName().Version)

    End Sub

#End Region

#Region " Friend Methods "

    ''' <summary>
    ''' Gets the product title shown in the form ("HostsMan v{Major}.{Minor}").
    ''' </summary>
    ''' <param name="version">The assembly version.</param>
    ''' <returns>The product title.</returns>
    ''' <exception cref="ArgumentNullException"><paramref name="version"/> is <see langword="Nothing"/>.</exception>
    Friend Shared Function GetProductTitle(ByVal version As Version) As String

        If version Is Nothing Then
            Throw New ArgumentNullException(NameOf(version))
        End If

        Return $"HostsMan v{version.Major}.{version.Minor}"

    End Function

#End Region

#Region " Event Handlers "

    ''' <summary>
    ''' Handles the LinkClicked event of the LinkLabel1 control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
    Private Sub LinkLabel1_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) _
    Handles LinkLabel1.LinkClicked

        Try
            Using p As New Process()
                p.StartInfo.FileName = About.WebsiteUrl
                p.StartInfo.UseShellExecute = True
                p.Start()
            End Using

        Catch ex As Win32Exception
            MessageBox.Show(Me, $"Error: {ex.Message}", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

#End Region

End Class

#End Region
