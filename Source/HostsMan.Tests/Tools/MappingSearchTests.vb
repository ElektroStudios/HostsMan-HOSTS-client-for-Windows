' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="MappingSearchTests.vb" company="Elektro Studios">
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
Imports HostsMan.Tools
Imports HostsMan.Tools.HostsTools

#End Region

#Region " MappingSearchTests "

Namespace Tools

    ''' <summary>
    ''' Tests for <see cref="MappingSearch"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class MappingSearchTests

        ''' <summary>
        ''' Gets a sample list of mappings.
        ''' </summary>
        Private Shared ReadOnly Property SampleMappings As List(Of MappingInfo)
            Get
                Return New List(Of MappingInfo) From {
                    New MappingInfo("127.0.0.1", "local.example.com", "Local web server"),
                    New MappingInfo("0.0.0.0", "ADS.example.com", "Blocked ads"),
                    New MappingInfo("192.168.1.10", "nas.lan", "Home NAS"),
                    New MappingInfo("FE80::1", "router.lan")
                }
            End Get
        End Property

        <TestMethod>
        <DataRow(MappingSearchType.HostName, MappingSearchCondition.StartsWithText, "ads", "ADS.example.com")>
        <DataRow(MappingSearchType.HostName, MappingSearchCondition.ContainsText, "EXAMPLE", "local.example.com|ADS.example.com")>
        <DataRow(MappingSearchType.HostName, MappingSearchCondition.StartsWithText, ".lan", "")>
        <DataRow(MappingSearchType.HostName, MappingSearchCondition.ContainsText, ".lan", "nas.lan|router.lan")>
        <DataRow(MappingSearchType.IP, MappingSearchCondition.StartsWithText, "192.", "nas.lan")>
        <DataRow(MappingSearchType.IP, MappingSearchCondition.ContainsText, ".0.", "local.example.com|ADS.example.com")>
        <DataRow(MappingSearchType.IP, MappingSearchCondition.StartsWithText, "fe80", "router.lan")>
        <DataRow(MappingSearchType.Comment, MappingSearchCondition.StartsWithText, "blocked", "ADS.example.com")>
        <DataRow(MappingSearchType.Comment, MappingSearchCondition.ContainsText, "SERVER", "local.example.com")>
        <DataRow(MappingSearchType.Comment, MappingSearchCondition.ContainsText, "o", "local.example.com|ADS.example.com|nas.lan")>
        <DataRow(MappingSearchType.Comment, MappingSearchCondition.ContainsText, "router", "")>
        Public Sub Filter_ReturnsTheMatchingMappingsInOrder(ByVal searchType As MappingSearchType,
                                                            ByVal searchCondition As MappingSearchCondition,
                                                            ByVal text As String,
                                                            ByVal expectedHostNames As String)

            Dim result As List(Of MappingInfo) = MappingSearch.Filter(MappingSearchTests.SampleMappings, searchType, searchCondition, text)

            Dim expected As String() = expectedHostNames.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries)
            CollectionAssert.AreEqual(expected, result.Select(Function(m As MappingInfo) m.HostName).ToArray())

        End Sub

        <TestMethod>
        Public Sub Filter_EmptyOrNothingText_ReturnsAllTheMappings()

            For Each text As String In {String.Empty, Nothing}
                Dim result As List(Of MappingInfo) = MappingSearch.Filter(MappingSearchTests.SampleMappings, MappingSearchType.IP, MappingSearchCondition.StartsWithText, text)
                Assert.AreEqual(MappingSearchTests.SampleMappings.Count, result.Count)
            Next text

        End Sub

        <TestMethod>
        Public Sub IsMatch_TurkishCulture_IsCultureInvariant()

            ' Regression: the search used culture-sensitive ToLower(), so in Turkish culture "INFO" did not match "info".
            Using New CultureScope("tr-TR")
                Assert.IsTrue(MappingSearch.IsMatch(New MappingInfo("127.0.0.1", "INFO.example.com"),
                                                    MappingSearchType.HostName, MappingSearchCondition.StartsWithText, "info"))
            End Using

        End Sub

        <TestMethod>
        Public Sub IsMatch_CommentSearch_MappingWithoutComment_DoesNotMatch()

            Dim mapping As New MappingInfo With {.IP = "127.0.0.1", .HostName = "example.com", .Comment = Nothing}

            Assert.IsFalse(MappingSearch.IsMatch(mapping, MappingSearchType.Comment, MappingSearchCondition.ContainsText, "x"))
            Assert.IsTrue(MappingSearch.IsMatch(mapping, MappingSearchType.Comment, MappingSearchCondition.ContainsText, String.Empty))

        End Sub

        <TestMethod>
        Public Sub IsMatch_NothingField_DoesNotThrow()

            Dim mapping As New MappingInfo With {.IP = Nothing, .HostName = Nothing}

            Assert.IsFalse(MappingSearch.IsMatch(mapping, MappingSearchType.HostName, MappingSearchCondition.ContainsText, "x"))

        End Sub

        <TestMethod>
        Public Sub IsMatch_NothingMapping_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(
                Sub() MappingSearch.IsMatch(Nothing, MappingSearchType.IP, MappingSearchCondition.ContainsText, "x"))

        End Sub

        <TestMethod>
        Public Sub IsMatch_InvalidEnumValues_ThrowInvalidEnumArgumentException()

            Dim mapping As New MappingInfo("127.0.0.1", "example.com")

            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(
                Sub() MappingSearch.IsMatch(mapping, CType(99, MappingSearchType), MappingSearchCondition.ContainsText, "x"))
            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(
                Sub() MappingSearch.IsMatch(mapping, MappingSearchType.IP, CType(99, MappingSearchCondition), "x"))

        End Sub

        <TestMethod>
        Public Sub Filter_NothingMappings_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(
                Sub() MappingSearch.Filter(Nothing, MappingSearchType.IP, MappingSearchCondition.ContainsText, "x"))

        End Sub

        <TestMethod>
        Public Sub SearchEnums_MatchTheOrderOfTheComboBoxItems()

            ' The main form maps the combo box SelectedIndex directly to these enumerations.
            Assert.AreEqual(0, CInt(MappingSearchType.IP))
            Assert.AreEqual(1, CInt(MappingSearchType.HostName))
            Assert.AreEqual(2, CInt(MappingSearchType.Comment))
            Assert.AreEqual(0, CInt(MappingSearchCondition.StartsWithText))
            Assert.AreEqual(1, CInt(MappingSearchCondition.ContainsText))

        End Sub

    End Class

End Namespace

#End Region
