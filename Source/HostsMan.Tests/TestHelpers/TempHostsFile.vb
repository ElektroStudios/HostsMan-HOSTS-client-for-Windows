' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="TempHostsFile.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Text

Imports HostsMan.Tools

#End Region

#Region " TempHostsFile "

Namespace TestHelpers

    ''' <summary>
    ''' Provides an isolated, temporary Hosts file location for a single test.
    ''' The real Windows Hosts file is never touched by the tests.
    ''' </summary>
    Friend NotInheritable Class TempHostsFile : Implements IDisposable

        ''' <summary>
        ''' The encoding used to read and write the temporary Hosts file (UTF-8 without BOM),
        ''' so the tests behave the same regardless of the system ANSI code page.
        ''' </summary>
        Friend Shared ReadOnly Property FileEncoding As Encoding = New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)

        ''' <summary>
        ''' Gets the temporary directory that contains the Hosts file.
        ''' </summary>
        Friend ReadOnly Property DirectoryPath As String

        ''' <summary>
        ''' Gets the full path of the temporary Hosts file (it is not created until written).
        ''' </summary>
        Friend ReadOnly Property FilePath As String

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TempHostsFile"/> class.
        ''' </summary>
        Friend Sub New()

            Me.DirectoryPath = Path.Combine(Path.GetTempPath(), "HostsMan.Tests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(Me.DirectoryPath)
            Me.FilePath = Path.Combine(Me.DirectoryPath, "hosts")

        End Sub

        ''' <summary>
        ''' Creates a <see cref="HostsTools"/> instance that targets the temporary Hosts file.
        ''' </summary>
        ''' <returns>The <see cref="HostsTools"/> instance.</returns>
        Friend Function CreateHostsTools() As HostsTools

            Return New HostsTools(Me.FilePath) With {.HostsEncoding = TempHostsFile.FileEncoding}

        End Function

        ''' <summary>
        ''' Writes the specified lines to the temporary Hosts file (CRLF line endings, trailing line break).
        ''' </summary>
        ''' <param name="lines">The lines.</param>
        Friend Sub WriteLines(ParamArray lines As String())

            File.WriteAllText(Me.FilePath, $"{String.Join(ControlChars.CrLf, lines)}{ControlChars.CrLf}", TempHostsFile.FileEncoding)

        End Sub

        ''' <summary>
        ''' Writes the specified raw text to the temporary Hosts file.
        ''' </summary>
        ''' <param name="text">The text.</param>
        Friend Sub WriteText(ByVal text As String)

            File.WriteAllText(Me.FilePath, text, TempHostsFile.FileEncoding)

        End Sub

        ''' <summary>
        ''' Reads the lines of the temporary Hosts file.
        ''' </summary>
        ''' <returns>The lines.</returns>
        Friend Function ReadLines() As String()

            Return File.ReadAllLines(Me.FilePath, TempHostsFile.FileEncoding)

        End Function

        ''' <summary>
        ''' Reads the raw text of the temporary Hosts file.
        ''' </summary>
        ''' <returns>The text.</returns>
        Friend Function ReadText() As String

            Return File.ReadAllText(Me.FilePath, TempHostsFile.FileEncoding)

        End Function

        ''' <summary>
        ''' Deletes the temporary directory.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            If Directory.Exists(Me.DirectoryPath) Then
                Directory.Delete(Me.DirectoryPath, recursive:=True)
            End If

        End Sub

    End Class

End Namespace

#End Region
