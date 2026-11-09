' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="MappingSearch.vb" company="Elektro Studios">
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

Imports HostsMan.Tools.HostsTools

#End Region

#Region " MappingSearch "

Namespace Tools

    ''' <summary>
    ''' Specifies the field of a mapping on which a search is performed.
    ''' </summary>
    Public Enum MappingSearchType As Integer

        ''' <summary>
        ''' Search by IP Address.
        ''' </summary>
        IP = 0

        ''' <summary>
        ''' Search by HostName.
        ''' </summary>
        HostName = 1

        ''' <summary>
        ''' Search by Comment.
        ''' </summary>
        Comment = 2

    End Enum

    ''' <summary>
    ''' Specifies the condition of a mapping search.
    ''' </summary>
    Public Enum MappingSearchCondition As Integer

        ''' <summary>
        ''' The field starts with the search text.
        ''' </summary>
        StartsWithText = 0

        ''' <summary>
        ''' The field contains the search text.
        ''' </summary>
        ContainsText = 1

    End Enum

    ''' <summary>
    ''' Provides the search (filter) logic of the mappings list.
    ''' </summary>
    Public NotInheritable Class MappingSearch

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="MappingSearch"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Determines whether the specified mapping matches the search criteria.
        ''' <para></para>
        ''' The comparison is ordinal and case-insensitive (IPv6 addresses, hostnames and comments are compared case-insensitively).
        ''' An empty search text matches any mapping.
        ''' </summary>
        ''' <param name="mapping">The mapping.</param>
        ''' <param name="searchType">The field to search on.</param>
        ''' <param name="searchCondition">The search condition.</param>
        ''' <param name="text">The search text.</param>
        ''' <returns><see langword="True"/> if the mapping matches the search criteria; otherwise, <see langword="False"/>.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="mapping"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidEnumArgumentException">Invalid <paramref name="searchType"/> or <paramref name="searchCondition"/>.</exception>
        Public Shared Function IsMatch(ByVal mapping As MappingInfo,
                                       ByVal searchType As MappingSearchType,
                                       ByVal searchCondition As MappingSearchCondition,
                                       ByVal text As String) As Boolean

            If mapping Is Nothing Then
                Throw New ArgumentNullException(NameOf(mapping))
            End If

            Dim field As String
            Select Case searchType
                Case MappingSearchType.IP
                    field = mapping.IP
                Case MappingSearchType.HostName
                    field = mapping.HostName
                Case MappingSearchType.Comment
                    field = mapping.Comment
                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(searchType), CInt(searchType), GetType(MappingSearchType))
            End Select

            If String.IsNullOrEmpty(text) Then
                Return True
            End If

            field = If(field, String.Empty)

            Select Case searchCondition
                Case MappingSearchCondition.StartsWithText
                    Return field.StartsWith(text, StringComparison.OrdinalIgnoreCase)
                Case MappingSearchCondition.ContainsText
                    Return field.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0
                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(searchCondition), CInt(searchCondition), GetType(MappingSearchCondition))
            End Select

        End Function

        ''' <summary>
        ''' Gets the mappings that match the search criteria, preserving their order.
        ''' </summary>
        ''' <param name="mappings">The mappings.</param>
        ''' <param name="searchType">The field to search on.</param>
        ''' <param name="searchCondition">The search condition.</param>
        ''' <param name="text">The search text.</param>
        ''' <returns>The mappings that match the search criteria.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="mappings"/> is <see langword="Nothing"/>.</exception>
        Public Shared Function Filter(ByVal mappings As IEnumerable(Of MappingInfo),
                                      ByVal searchType As MappingSearchType,
                                      ByVal searchCondition As MappingSearchCondition,
                                      ByVal text As String) As List(Of MappingInfo)

            If mappings Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappings))
            End If

            Dim result As New List(Of MappingInfo)
            For Each mapping As MappingInfo In mappings
                If MappingSearch.IsMatch(mapping, searchType, searchCondition, text) Then
                    result.Add(mapping)
                End If
            Next mapping

            Return result

        End Function

#End Region

    End Class

End Namespace

#End Region
