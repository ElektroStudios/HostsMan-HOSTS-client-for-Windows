' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="HostsLineFormatTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports HostsMan.Tools
Imports HostsMan.Tools.HostsTools

#End Region

#Region " HostsLineFormatTests "

Namespace Tools

    ''' <summary>
    ''' Tests for the line parsing and formatting of <see cref="HostsTools"/>, and for <see cref="MappingInfo"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class HostsLineFormatTests

#Region " ParseLine "

        <TestMethod>
        Public Sub ParseLine_Nothing_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() HostsTools.ParseLine(Nothing))

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow("#")>
        <DataRow("# just a comment")>
        <DataRow("127.0.0.1")>
        <DataRow("# 127.0.0.1")>
        <DataRow("hostname-without-ip")>
        <DataRow("localhost 127.0.0.1")>
        Public Sub ParseLine_NotAMapping_ReturnsEmptyList(ByVal line As String)

            Assert.AreEqual(0, HostsTools.ParseLine(line).Count)

        End Sub

        <TestMethod>
        Public Sub ParseLine_SeveralHostNames_SharesIPCommentAndState()

            Dim mappings As List(Of MappingInfo) = HostsTools.ParseLine("# 0.0.0.0 a.com b.com c.com # comment")

            Assert.AreEqual(3, mappings.Count)
            Assert.IsTrue(mappings.TrueForAll(Function(m As MappingInfo) m.IP = "0.0.0.0" AndAlso m.IsDisabled AndAlso m.Comment = "comment"))

        End Sub

#End Region

#Region " FormatMapping "

        <TestMethod>
        Public Sub FormatMapping_Nothing_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() HostsTools.FormatMapping(Nothing))

        End Sub

        <TestMethod>
        <DataRow("127.0.0.1", "example.com", "", False, "127.0.0.1" & vbTab & "example.com")>
        <DataRow("127.0.0.1", "example.com", "   ", False, "127.0.0.1" & vbTab & "example.com")>
        <DataRow("0.0.0.0", "example.com", "", True, "# 0.0.0.0" & vbTab & "example.com")>
        <DataRow("::1", "example.com", " note ", False, "::1" & vbTab & "example.com" & vbTab & "# note")>
        <DataRow("0.0.0.0", "example.com", "note", True, "# 0.0.0.0" & vbTab & "example.com" & vbTab & "# note")>
        Public Sub FormatMapping_ReturnsTheExpectedLine(ByVal ip As String, ByVal hostName As String, ByVal comment As String,
                                                        ByVal isDisabled As Boolean, ByVal expected As String)

            Assert.AreEqual(expected, HostsTools.FormatMapping(New MappingInfo(ip, hostName, comment, isDisabled)))

        End Sub

        <TestMethod>
        Public Sub FormatMapping_NothingComment_HasNoCommentSuffix()

            Dim mapping As New MappingInfo With {.IP = "127.0.0.1", .HostName = "example.com", .Comment = Nothing}

            Assert.AreEqual($"127.0.0.1{ControlChars.Tab}example.com", HostsTools.FormatMapping(mapping))

        End Sub

        <TestMethod>
        <DataRow("127.0.0.1", "example.com", "", False)>
        <DataRow("0.0.0.0", "ads.example.com", "Blocked by HostsMan", True)>
        <DataRow("2001:db8::1", "ipv6.example.com", "IPv6", False)>
        <DataRow("192.168.1.10", "nas.lan", "Comment with # hash inside", True)>
        Public Sub FormatMappingThenParseLine_RoundTrips(ByVal ip As String, ByVal hostName As String, ByVal comment As String,
                                                         ByVal isDisabled As Boolean)

            Dim original As New MappingInfo(ip, hostName, comment, isDisabled)

            Dim parsed As MappingInfo = HostsTools.ParseLine(HostsTools.FormatMapping(original)).Single()

            Assert.AreEqual(ip, parsed.IP)
            Assert.AreEqual(hostName, parsed.HostName)
            Assert.AreEqual(comment, parsed.Comment)
            Assert.AreEqual(isDisabled, parsed.IsDisabled)

        End Sub

#End Region

#Region " MappingInfo "

        <TestMethod>
        Public Sub MappingInfo_ToString_ReturnsTheFormattedLine()

            Dim mapping As New MappingInfo("127.0.0.1", "example.com", "note", isDisabled:=True)

            Assert.AreEqual(HostsTools.FormatMapping(mapping), mapping.ToString())

        End Sub

        <TestMethod>
        Public Sub MappingInfo_Clone_ReturnsAnIndependentCopy()

            Dim original As New MappingInfo("127.0.0.1", "example.com", "note", isDisabled:=True)

            Dim copy As MappingInfo = original.Clone()
            copy.IP = "0.0.0.0"
            copy.HostName = "other.com"
            copy.Comment = "changed"
            copy.IsDisabled = False

            Assert.AreNotSame(original, copy)
            Assert.AreEqual("127.0.0.1", original.IP)
            Assert.AreEqual("example.com", original.HostName)
            Assert.AreEqual("note", original.Comment)
            Assert.IsTrue(original.IsDisabled)

        End Sub

        <TestMethod>
        <DataRow("127.0.0.1", "example.com", "127.0.0.1", "EXAMPLE.COM", True)>
        <DataRow("::1", "example.com", "0:0:0:0:0:0:0:1", "example.com", True)>
        <DataRow("127.0.0.1", "example.com", "0.0.0.0", "example.com", False)>
        <DataRow("127.0.0.1", "example.com", "127.0.0.1", "other.com", False)>
        Public Sub MappingInfo_IsSameTarget_ComparesHostNameAndAddress(ByVal ipA As String, ByVal hostA As String,
                                                                      ByVal ipB As String, ByVal hostB As String,
                                                                      ByVal expected As Boolean)

            Assert.AreEqual(expected, New MappingInfo(ipA, hostA).IsSameTarget(New MappingInfo(ipB, hostB)))

        End Sub

        <TestMethod>
        <DataRow("127.0.0.1", "example.com", "0.0.0.0", "Example.com", True)>
        <DataRow("::1", "example.com", "fe80::1", "example.com", True)>
        <DataRow("127.0.0.1", "example.com", "::1", "example.com", False)>
        <DataRow("127.0.0.1", "example.com", "127.0.0.1", "other.com", False)>
        Public Sub MappingInfo_ConflictsWith_DetectsSameHostNameAndAddressFamily(ByVal ipA As String, ByVal hostA As String,
                                                                                ByVal ipB As String, ByVal hostB As String,
                                                                                ByVal expected As Boolean)

            Assert.AreEqual(expected, New MappingInfo(ipA, hostA).ConflictsWith(New MappingInfo(ipB, hostB)))

        End Sub

        <TestMethod>
        Public Sub MappingInfo_IsSameTargetAndConflictsWith_Nothing_ThrowArgumentNullException()

            Dim mapping As New MappingInfo("127.0.0.1", "example.com")

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() mapping.IsSameTarget(Nothing))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() mapping.ConflictsWith(Nothing))

        End Sub

#End Region

    End Class

End Namespace

#End Region
