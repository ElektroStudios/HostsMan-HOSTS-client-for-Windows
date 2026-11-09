' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="MappingValidator.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization
Imports System.Net
Imports System.Net.Sockets

#End Region

#Region " MappingValidator "

Namespace Tools

    ''' <summary>
    ''' Provides validation rules for the values of a HOSTS file mapping
    ''' (IP address, hostname and comment).
    ''' </summary>
    Public NotInheritable Class MappingValidator

#Region " Constants "

        ''' <summary>
        ''' The maximum length, in characters, of a hostname (RFC 1035).
        ''' </summary>
        Public Const MaxHostNameLength As Integer = 255

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="MappingValidator"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Determines whether the specified value is a valid IP address for a HOSTS file mapping.
        ''' <para></para>
        ''' IPv4 addresses must be written in the strict dotted-decimal notation (e.g. "127.0.0.1"),
        ''' because the shortened, octal or hexadecimal notations accepted by <see cref="IPAddress.TryParse(String, ByRef IPAddress)"/>
        ''' (e.g. "1", "127.1" or "0x7f.0.0.1") are not reliably understood by the Windows resolver.
        ''' <para></para>
        ''' IPv6 addresses are accepted in any notation understood by <see cref="IPAddress.TryParse(String, ByRef IPAddress)"/>.
        ''' </summary>
        ''' <param name="value">The value to validate.</param>
        ''' <returns><see langword="True"/> if <paramref name="value"/> is a valid IP address; otherwise, <see langword="False"/>.</returns>
        Public Shared Function IsValidIPAddress(ByVal value As String) As Boolean

            If String.IsNullOrEmpty(value) OrElse MappingValidator.ContainsWhiteSpaceOrControl(value) Then
                Return False
            End If

            Dim address As IPAddress = Nothing
            If Not IPAddress.TryParse(value, address) Then
                Return False
            End If

            Select Case address.AddressFamily

                Case AddressFamily.InterNetworkV6
                    Return value.Contains(":"c)

                Case AddressFamily.InterNetwork
                    Return MappingValidator.IsDottedDecimalIPv4(value)

                Case Else
                    Return False

            End Select

        End Function

        ''' <summary>
        ''' Determines whether the specified value is a valid hostname for a HOSTS file mapping.
        ''' <para></para>
        ''' A valid hostname is not empty, is not longer than <see cref="MaxHostNameLength"/> characters,
        ''' and does not contain white-spaces, control characters or the comment character ('#').
        ''' </summary>
        ''' <param name="value">The value to validate.</param>
        ''' <returns><see langword="True"/> if <paramref name="value"/> is a valid hostname; otherwise, <see langword="False"/>.</returns>
        Public Shared Function IsValidHostName(ByVal value As String) As Boolean

            Return Not String.IsNullOrEmpty(value) AndAlso
                   value.Length <= MappingValidator.MaxHostNameLength AndAlso
                   Not value.Contains("#"c) AndAlso
                   Not MappingValidator.ContainsWhiteSpaceOrControl(value)

        End Function

        ''' <summary>
        ''' Determines whether the specified value is a valid comment for a HOSTS file mapping.
        ''' <para></para>
        ''' A comment is optional (<see langword="Nothing"/> or empty is valid),
        ''' but it must not contain line breaks or other control characters (except tabs).
        ''' </summary>
        ''' <param name="value">The value to validate.</param>
        ''' <returns><see langword="True"/> if <paramref name="value"/> is a valid comment; otherwise, <see langword="False"/>.</returns>
        Public Shared Function IsValidComment(ByVal value As String) As Boolean

            If String.IsNullOrEmpty(value) Then
                Return True
            End If

            For Each c As Char In value
                If Char.IsControl(c) AndAlso c <> ControlChars.Tab Then
                    Return False
                End If
            Next c

            Return True

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Determines whether the specified value contains any white-space or control character.
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        ''' <returns><see langword="True"/> if a white-space or control character was found; otherwise, <see langword="False"/>.</returns>
        Private Shared Function ContainsWhiteSpaceOrControl(ByVal value As String) As Boolean

            For Each c As Char In value
                If Char.IsWhiteSpace(c) OrElse Char.IsControl(c) Then
                    Return True
                End If
            Next c

            Return False

        End Function

        ''' <summary>
        ''' Determines whether the specified value is an IPv4 address written in strict dotted-decimal notation
        ''' (four decimal octets in the range 0-255).
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        ''' <returns><see langword="True"/> if <paramref name="value"/> is in dotted-decimal notation; otherwise, <see langword="False"/>.</returns>
        Private Shared Function IsDottedDecimalIPv4(ByVal value As String) As Boolean

            Dim octets As String() = value.Split("."c)
            If octets.Length <> 4 Then
                Return False
            End If

            For Each octet As String In octets

                If octet.Length = 0 OrElse octet.Length > 3 Then
                    Return False
                End If

                For Each c As Char In octet
                    If c < "0"c OrElse c > "9"c Then
                        Return False
                    End If
                Next c

                If Integer.Parse(octet, NumberStyles.None, CultureInfo.InvariantCulture) > 255 Then
                    Return False
                End If

            Next octet

            Return True

        End Function

#End Region

    End Class

End Namespace

#End Region
