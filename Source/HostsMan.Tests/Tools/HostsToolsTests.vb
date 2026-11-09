' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="HostsToolsTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Text

Imports HostsMan.Tests.TestHelpers
Imports HostsMan.Tools
Imports HostsMan.Tools.HostsTools

#End Region

#Region " HostsToolsTests "

Namespace Tools

    ''' <summary>
    ''' Tests for the file operations of <see cref="HostsTools"/>.
    ''' All the tests work on an isolated temporary file, never on the real Windows Hosts file.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class HostsToolsTests

#Region " Fields "

        ''' <summary>
        ''' The temporary Hosts file of the current test.
        ''' </summary>
        Private tempFile As TempHostsFile

        ''' <summary>
        ''' The <see cref="HostsTools"/> instance under test, that targets <see cref="tempFile"/>.
        ''' </summary>
        Private hosts As HostsTools

#End Region

#Region " Initialization "

        ''' <summary>
        ''' Creates the temporary Hosts file location before each test.
        ''' </summary>
        <TestInitialize>
        Public Sub TestInitialize()

            Me.tempFile = New TempHostsFile()
            Me.hosts = Me.tempFile.CreateHostsTools()

        End Sub

        ''' <summary>
        ''' Deletes the temporary Hosts file location after each test.
        ''' </summary>
        <TestCleanup>
        Public Sub TestCleanup()

            Me.tempFile.Dispose()

        End Sub

#End Region

#Region " Constructor and Properties "

        <TestMethod>
        Public Sub Constructor_WithoutLocation_UsesDefaultHostsLocation()

            Dim sut As New HostsTools()

            Assert.AreEqual(HostsTools.DefaultHostsLocation, sut.HostsLocation)
            Assert.IsTrue(HostsTools.DefaultHostsLocation.EndsWith("drivers\etc\hosts", StringComparison.OrdinalIgnoreCase))

        End Sub

        <TestMethod>
        Public Sub Constructor_WithLocation_UsesSpecifiedLocation()

            Assert.AreEqual(Me.tempFile.FilePath, Me.hosts.HostsLocation)

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow("   ")>
        Public Sub HostsLocation_SetEmptyOrWhiteSpace_ThrowsArgumentException(ByVal value As String)

            Assert.ThrowsExactly(Of ArgumentException)(Sub() Me.hosts.HostsLocation = value)

        End Sub

        <TestMethod>
        Public Sub HostsLocation_SetNothing_ThrowsArgumentException()

            Assert.ThrowsExactly(Of ArgumentException)(Sub() Me.hosts.HostsLocation = Nothing)

        End Sub

        <TestMethod>
        Public Sub HostsEncoding_SetNothing_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Me.hosts.HostsEncoding = Nothing)

        End Sub

        <TestMethod>
        Public Sub HostsEncoding_DefaultValue_IsSystemDefaultEncoding()

            Assert.AreEqual(Encoding.Default, New HostsTools(Me.tempFile.FilePath).HostsEncoding)

        End Sub

        <TestMethod>
        Public Sub Localhost_DefaultValue_Is127001()

            Assert.AreEqual("127.0.0.1", Me.hosts.Localhost)

        End Sub

        <TestMethod>
        <DataRow("0.0.0.0")>
        <DataRow("::1")>
        Public Sub Localhost_SetValidAddress_IsStored(ByVal value As String)

            Me.hosts.Localhost = value

            Assert.AreEqual(value, Me.hosts.Localhost)

        End Sub

        <TestMethod>
        <DataRow("localhost")>
        <DataRow("256.0.0.1")>
        <DataRow("")>
        Public Sub Localhost_SetInvalidAddress_ThrowsFormatException(ByVal value As String)

            Assert.ThrowsExactly(Of FormatException)(Sub() Me.hosts.Localhost = value)

        End Sub

        <TestMethod>
        Public Sub DefaultHeader_UsesEnvironmentNewLineAndEndsWithLineBreak()

            ' Regression: the header was built from an XML literal, which produced LF-only line endings
            ' mixed with the CRLF line endings of the mappings.
            Dim header As String = HostsTools.DefaultHeader

            Assert.IsTrue(header.EndsWith(Environment.NewLine, StringComparison.Ordinal))
            Dim withoutNewLines As String = header.Replace(Environment.NewLine, String.Empty)
            Assert.AreEqual(-1, withoutNewLines.IndexOfAny({ControlChars.Cr, ControlChars.Lf}), "The header contains mixed line endings.")
            Assert.IsTrue(header.Split({Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).All(Function(line As String) line.StartsWith("#", StringComparison.Ordinal)))

        End Sub

#End Region

#Region " File Operations "

        <TestMethod>
        Public Sub FileExists_MissingFile_ReturnsFalse()

            Assert.IsFalse(Me.hosts.FileExists())

        End Sub

        <TestMethod>
        Public Sub FileCreate_WritesOnlyTheDefaultHeader()

            Me.hosts.FileCreate()

            Assert.IsTrue(Me.hosts.FileExists())
            Assert.AreEqual(HostsTools.DefaultHeader, Me.tempFile.ReadText())
            Assert.AreEqual(0, Me.hosts.GetMappings().Count)

        End Sub

        <TestMethod>
        Public Sub FileCreate_ExistingFile_OverwritesIt()

            Me.tempFile.WriteLines("127.0.0.1 example.com")

            Me.hosts.FileCreate()

            Assert.AreEqual(HostsTools.DefaultHeader, Me.tempFile.ReadText())

        End Sub

        <TestMethod>
        Public Sub FileClean_RemovesAllTheMappings()

            Me.tempFile.WriteLines("127.0.0.1 a.com", "0.0.0.0 b.com")

            Me.hosts.FileClean()

            Assert.AreEqual(0, Me.hosts.GetMappings().Count)

        End Sub

        <TestMethod>
        Public Sub FileDelete_ExistingFile_DeletesIt()

            Me.hosts.FileCreate()

            Me.hosts.FileDelete()

            Assert.IsFalse(File.Exists(Me.tempFile.FilePath))

        End Sub

        <TestMethod>
        Public Sub FileDelete_MissingFile_ThrowsFileNotFoundException()

            Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Me.hosts.FileDelete())

        End Sub

        <TestMethod>
        Public Sub FileOpen_MissingHostsFile_ThrowsFileNotFoundException()

            Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Me.hosts.FileOpen())

        End Sub

        <TestMethod>
        Public Sub FileOpen_MissingRootedProcess_ThrowsFileNotFoundException()

            Me.hosts.FileCreate()
            Dim missingProcess As String = Path.Combine(Me.tempFile.DirectoryPath, "missing-editor.exe")

            Dim ex As FileNotFoundException = Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Me.hosts.FileOpen(missingProcess))

            Assert.AreEqual(missingProcess, ex.FileName)

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow("  ")>
        Public Sub FileOpen_EmptyProcess_ThrowsArgumentException(ByVal process As String)

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of ArgumentException)(Sub() Me.hosts.FileOpen(process))

        End Sub

#End Region

#Region " Add "

        <TestMethod>
        Public Sub Add_EnabledMapping_WritesTabSeparatedLine()

            Me.hosts.FileCreate()

            Me.hosts.Add(enabled:=True, hostName:="example.com", ip:="127.0.0.1")

            CollectionAssert.Contains(Me.tempFile.ReadLines(), $"127.0.0.1{ControlChars.Tab}example.com")

        End Sub

        <TestMethod>
        Public Sub Add_DisabledMappingWithComment_WritesCommentedLine()

            Me.hosts.FileCreate()

            Me.hosts.Add(enabled:=False, hostName:="example.com", ip:="0.0.0.0", comment:="Blocked")

            CollectionAssert.Contains(Me.tempFile.ReadLines(), $"# 0.0.0.0{ControlChars.Tab}example.com{ControlChars.Tab}# Blocked")

            Dim mapping As MappingInfo = Me.hosts.GetMappings().Single()
            Assert.IsTrue(mapping.IsDisabled)
            Assert.AreEqual("0.0.0.0", mapping.IP)
            Assert.AreEqual("example.com", mapping.HostName)
            Assert.AreEqual("Blocked", mapping.Comment)

        End Sub

        <TestMethod>
        Public Sub Add_MappingInfoOverload_AddsTheMapping()

            Me.hosts.FileCreate()

            Me.hosts.Add(New MappingInfo("::1", "ipv6.example.com", "IPv6 entry", isDisabled:=False))

            Assert.IsTrue(Me.hosts.IsMapped("ipv6.example.com"))
            Assert.AreEqual("::1", Me.hosts.GetMappingFromHostname("ipv6.example.com").IP)

        End Sub

        <TestMethod>
        Public Sub Add_FileWithoutTrailingLineBreak_WritesTheMappingOnItsOwnLine()

            ' Regression: appending to a file without trailing line break must not merge two entries in the same line.
            Me.tempFile.WriteText("127.0.0.1 first.com")

            Me.hosts.Add(enabled:=True, hostName:="second.com", ip:="127.0.0.1")

            Dim hostNames As List(Of String) = Me.hosts.GetMappings().Select(Function(m As MappingInfo) m.HostName).ToList()
            CollectionAssert.AreEqual({"first.com", "second.com"}, hostNames)

        End Sub

        <TestMethod>
        Public Sub Add_SeveralMappings_DoesNotInsertBlankLines()

            Me.hosts.FileCreate()

            Me.hosts.Add(enabled:=True, hostName:="a.com", ip:="127.0.0.1")
            Me.hosts.Add(enabled:=True, hostName:="b.com", ip:="127.0.0.1")

            Dim lines As String() = Me.tempFile.ReadLines()
            Assert.IsFalse(lines.Any(Function(line As String) line.Length = 0))
            Assert.AreEqual($"127.0.0.1{ControlChars.Tab}b.com", lines.Last())

        End Sub

        <TestMethod>
        <DataRow("999.0.0.1")>
        <DataRow("127.0.0")>
        <DataRow("1")>
        <DataRow("example.com")>
        <DataRow("")>
        Public Sub Add_InvalidIPAddress_ThrowsFormatException(ByVal ip As String)

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of FormatException)(Sub() Me.hosts.Add(enabled:=True, hostName:="example.com", ip:=ip))

        End Sub

        <TestMethod>
        <DataRow("two words")>
        <DataRow("hash#tag.com")>
        <DataRow("")>
        Public Sub Add_InvalidHostName_ThrowsArgumentException(ByVal hostName As String)

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of ArgumentException)(Sub() Me.hosts.Add(enabled:=True, hostName:=hostName, ip:="127.0.0.1"))

        End Sub

        <TestMethod>
        Public Sub Add_CommentWithLineBreak_ThrowsArgumentExceptionAndDoesNotInjectLines()

            ' Regression: a comment containing a line break injected arbitrary lines into the Hosts file.
            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of ArgumentException)(
                Sub() Me.hosts.Add(enabled:=True, hostName:="example.com", ip:="127.0.0.1", comment:=$"comment{Environment.NewLine}0.0.0.0 injected.com"))

            Assert.AreEqual(HostsTools.DefaultHeader, Me.tempFile.ReadText())

        End Sub

        <TestMethod>
        Public Sub Add_HostNameAlreadyMapped_ThrowsInvalidOperationException()

            Me.tempFile.WriteLines("127.0.0.1 Example.com")

            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() Me.hosts.Add(enabled:=True, hostName:="example.COM", ip:="0.0.0.0"))

        End Sub

        <TestMethod>
        Public Sub Add_MissingFile_ThrowsFileNotFoundException()

            Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Me.hosts.Add(enabled:=True, hostName:="example.com", ip:="127.0.0.1"))

        End Sub

        <TestMethod>
        Public Sub Add_NullMappingInfo_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Me.hosts.Add(CType(Nothing, MappingInfo)))

        End Sub

#End Region

#Region " GetMappings "

        <TestMethod>
        Public Sub GetMappings_TabAndSpaceSeparatedEntries_AreParsed()

            Me.tempFile.WriteLines($"127.0.0.1{ControlChars.Tab}tab.com",
                                   "127.0.0.1 space.com",
                                   $"  0.0.0.0 {ControlChars.Tab}  mixed.com  ")

            Dim mappings As List(Of MappingInfo) = Me.hosts.GetMappings()

            CollectionAssert.AreEqual({"tab.com", "space.com", "mixed.com"}, mappings.Select(Function(m As MappingInfo) m.HostName).ToArray())
            CollectionAssert.AreEqual({"127.0.0.1", "127.0.0.1", "0.0.0.0"}, mappings.Select(Function(m As MappingInfo) m.IP).ToArray())
            Assert.IsTrue(mappings.TrueForAll(Function(m As MappingInfo) Not m.IsDisabled AndAlso m.Comment = String.Empty))

        End Sub

        <TestMethod>
        Public Sub GetMappings_SeveralHostNamesInOneLine_ReturnsOneMappingPerHostName()

            ' Regression: the second hostname of a line was parsed as the comment, and lost (unmapped) when saving.
            Me.tempFile.WriteLines("0.0.0.0 ads.example.com tracker.example.com  # Ad servers")

            Dim mappings As List(Of MappingInfo) = Me.hosts.GetMappings()

            Assert.AreEqual(2, mappings.Count)
            CollectionAssert.AreEqual({"ads.example.com", "tracker.example.com"}, mappings.Select(Function(m As MappingInfo) m.HostName).ToArray())
            Assert.IsTrue(mappings.TrueForAll(Function(m As MappingInfo) m.IP = "0.0.0.0" AndAlso m.Comment = "Ad servers"))

        End Sub

        <TestMethod>
        Public Sub GetMappings_IPv6Entries_AreParsed()

            ' Regression: IPv6 mappings were accepted by Add, but ignored by GetMappings (and lost when saving).
            Me.tempFile.WriteLines("::1 localhost", "fe80::1%4 link.local", "# ::1 disabled.local")

            Dim mappings As List(Of MappingInfo) = Me.hosts.GetMappings()

            Assert.AreEqual(3, mappings.Count)
            Assert.AreEqual("::1", mappings(0).IP)
            Assert.AreEqual("fe80::1%4", mappings(1).IP)
            Assert.IsTrue(mappings(2).IsDisabled)

        End Sub

        <TestMethod>
        <DataRow("#127.0.0.1 example.com")>
        <DataRow("# 127.0.0.1 example.com")>
        <DataRow("##   127.0.0.1" & vbTab & "example.com")>
        <DataRow("   #  127.0.0.1   example.com   ")>
        Public Sub GetMappings_DisabledEntryVariants_AreParsedAsDisabled(ByVal line As String)

            Me.tempFile.WriteLines(line)

            Dim mapping As MappingInfo = Me.hosts.GetMappings().Single()

            Assert.IsTrue(mapping.IsDisabled)
            Assert.AreEqual("127.0.0.1", mapping.IP)
            Assert.AreEqual("example.com", mapping.HostName)

        End Sub

        <TestMethod>
        <DataRow("127.0.0.1 example.com #comment", "comment")>
        <DataRow("127.0.0.1 example.com # a longer comment ", "a longer comment")>
        <DataRow("127.0.0.1 example.com#glued", "glued")>
        <DataRow("127.0.0.1" & vbTab & "example.com" & vbTab & "## double hash", "double hash")>
        <DataRow("# 127.0.0.1 example.com # disabled comment", "disabled comment")>
        Public Sub GetMappings_InlineComment_IsParsedWithoutTheHashCharacter(ByVal line As String, ByVal expectedComment As String)

            Me.tempFile.WriteLines(line)

            Dim mapping As MappingInfo = Me.hosts.GetMappings().Single()

            Assert.AreEqual("example.com", mapping.HostName)
            Assert.AreEqual(expectedComment, mapping.Comment)

        End Sub

        <TestMethod>
        Public Sub GetMappings_NonMappingLines_AreIgnoredWithoutThrowing()

            ' Regression: a commented line with only an IP address ("# 10.0.0.1") threw IndexOutOfRangeException,
            ' and malformed addresses such as "1abc.2.3.4" were accepted as IP addresses.
            Me.tempFile.WriteLines("# Copyright (c) 1993-2009 Microsoft Corp.",
                                   "#",
                                   String.Empty,
                                   "    ",
                                   "# 10.0.0.1",
                                   "10.0.0.1",
                                   "not an entry",
                                   "1abc.2.3.4 invalid.com",
                                   "300.1.1.1 invalid.com",
                                   "# For example:",
                                   "127.0.0.1 valid.com")

            Dim mappings As List(Of MappingInfo) = Me.hosts.GetMappings()

            Assert.AreEqual(1, mappings.Count)
            Assert.AreEqual("valid.com", mappings(0).HostName)

        End Sub

        <TestMethod>
        Public Sub GetMappings_WindowsDefaultHostsFile_ReturnsTheSampleEntriesAsDisabled()

            Me.tempFile.WriteLines("# Copyright (c) 1993-2009 Microsoft Corp.",
                                   "#",
                                   "# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.",
                                   "#",
                                   "# For example:",
                                   "#",
                                   "#      102.54.94.97     rhino.acme.com          # source server",
                                   "#       38.25.63.10     x.acme.com              # x client host",
                                   String.Empty,
                                   "# localhost name resolution is handled within DNS itself.",
                                   $"#{ControlChars.Tab}127.0.0.1       localhost",
                                   $"#{ControlChars.Tab}::1             localhost")

            Dim mappings As List(Of MappingInfo) = Me.hosts.GetMappings()

            Assert.AreEqual(4, mappings.Count)
            Assert.IsTrue(mappings.TrueForAll(Function(m As MappingInfo) m.IsDisabled))
            Assert.AreEqual("rhino.acme.com", mappings(0).HostName)
            Assert.AreEqual("source server", mappings(0).Comment)
            Assert.AreEqual("x client host", mappings(1).Comment)
            Assert.AreEqual("::1", mappings(3).IP)

        End Sub

        <TestMethod>
        Public Sub GetMappings_MissingFile_ThrowsFileNotFoundException()

            Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Me.hosts.GetMappings())

        End Sub

#End Region

#Region " IsMapped / IsDisabled "

        <TestMethod>
        Public Sub IsMapped_SpaceSeparatedEntry_ReturnsTrue()

            ' Regression: IsMapped only understood TAB separated entries.
            Me.tempFile.WriteLines("127.0.0.1 example.com")

            Assert.IsTrue(Me.hosts.IsMapped("example.com"))

        End Sub

        <TestMethod>
        Public Sub IsMapped_IsCaseInsensitive()

            Me.tempFile.WriteLines("127.0.0.1 Example.COM")

            Assert.IsTrue(Me.hosts.IsMapped("example.com"))

        End Sub

        <TestMethod>
        Public Sub IsMapped_DisabledEntry_ReturnsTrue()

            Me.tempFile.WriteLines("# 127.0.0.1 example.com")

            Assert.IsTrue(Me.hosts.IsMapped("example.com"))
            Assert.IsTrue(Me.hosts.IsMapped(New MappingInfo("0.0.0.0", "example.com")))

        End Sub

        <TestMethod>
        Public Sub IsMapped_SecondHostNameOfALine_ReturnsTrue()

            Me.tempFile.WriteLines("127.0.0.1 first.com second.com")

            Assert.IsTrue(Me.hosts.IsMapped("second.com"))

        End Sub

        <TestMethod>
        Public Sub IsMapped_HostNameOnlyInsideAComment_ReturnsFalse()

            Me.tempFile.WriteLines("127.0.0.1 example.com # see also other.com")

            Assert.IsFalse(Me.hosts.IsMapped("other.com"))
            Assert.IsFalse(Me.hosts.IsMapped("unknown.com"))

        End Sub

        <TestMethod>
        Public Sub IsMapped_NullHostName_ThrowsArgumentNullException()

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Me.hosts.IsMapped(CType(Nothing, String)))

        End Sub

        <TestMethod>
        Public Sub IsDisabled_OnlyDisabledEntries_ReturnsTrue()

            Me.tempFile.WriteLines("#127.0.0.1 example.com", "# ::1 example.com")

            Assert.IsTrue(Me.hosts.IsDisabled("example.com"))

        End Sub

        <TestMethod>
        Public Sub IsDisabled_EnabledEntry_ReturnsFalse()

            Me.tempFile.WriteLines("127.0.0.1 example.com")

            Assert.IsFalse(Me.hosts.IsDisabled(New MappingInfo("127.0.0.1", "example.com")))

        End Sub

        <TestMethod>
        Public Sub IsDisabled_DisabledAndEnabledEntries_ReturnsFalse()

            ' Regression: only the first entry of the hostname was evaluated.
            Me.tempFile.WriteLines("# 0.0.0.0 example.com", "127.0.0.1 example.com")

            Assert.IsFalse(Me.hosts.IsDisabled("example.com"))

        End Sub

        <TestMethod>
        Public Sub IsDisabled_NotMapped_ThrowsInvalidOperationException()

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() Me.hosts.IsDisabled("example.com"))

        End Sub

#End Region

#Region " Disable "

        <TestMethod>
        Public Sub Disable_SpaceSeparatedEntry_DisablesIt()

            ' Regression: Disable failed with "is not mapped" for space separated entries.
            Me.tempFile.WriteLines("127.0.0.1 example.com")

            Me.hosts.Disable("example.com")

            Assert.IsTrue(Me.hosts.IsDisabled("example.com"))

        End Sub

        <TestMethod>
        Public Sub Disable_PreservesTheOtherLinesUntouched()

            Dim untouchedLines As String() = {"# My custom header", String.Empty, "0.0.0.0    other.com    # keep me", "#  just a comment"}
            Me.tempFile.WriteLines(untouchedLines.Concat({"127.0.0.1 example.com"}).ToArray())

            Me.hosts.Disable("example.com")

            Dim lines As String() = Me.tempFile.ReadLines()
            CollectionAssert.AreEqual(untouchedLines, lines.Take(untouchedLines.Length).ToArray())
            Assert.AreEqual($"# 127.0.0.1{ControlChars.Tab}example.com", lines.Last())

        End Sub

        <TestMethod>
        Public Sub Disable_HostNameSharingALine_OnlyDisablesThatHostName()

            Me.tempFile.WriteLines("0.0.0.0 a.com b.com c.com # shared")

            Me.hosts.Disable("b.com")

            Dim mappings As List(Of MappingInfo) = Me.hosts.GetMappings()
            Assert.AreEqual(3, mappings.Count)
            Assert.IsFalse(Me.hosts.IsDisabled("a.com"))
            Assert.IsTrue(Me.hosts.IsDisabled("b.com"))
            Assert.IsFalse(Me.hosts.IsDisabled("c.com"))
            Assert.IsTrue(mappings.TrueForAll(Function(m As MappingInfo) m.Comment = "shared"))

        End Sub

        <TestMethod>
        Public Sub Disable_AlreadyDisabled_ThrowsInvalidOperationException()

            Me.tempFile.WriteLines("# 127.0.0.1 example.com")

            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() Me.hosts.Disable("example.com"))

        End Sub

        <TestMethod>
        Public Sub Disable_NotMapped_ThrowsInvalidOperationException()

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() Me.hosts.Disable(New MappingInfo("127.0.0.1", "example.com")))

        End Sub

        <TestMethod>
        Public Sub Disable_MissingFile_ThrowsFileNotFoundException()

            Assert.ThrowsExactly(Of FileNotFoundException)(Sub() Me.hosts.Disable("example.com"))

        End Sub

#End Region

#Region " Remove "

        <TestMethod>
        Public Sub Remove_SpaceSeparatedEntry_RemovesIt()

            Me.tempFile.WriteLines("127.0.0.1 example.com", "127.0.0.1 other.com")

            Me.hosts.Remove("example.com")

            Assert.IsFalse(Me.hosts.IsMapped("example.com"))
            Assert.IsTrue(Me.hosts.IsMapped("other.com"))

        End Sub

        <TestMethod>
        Public Sub Remove_SeveralEntriesOfTheSameHostName_RemovesAllOfThem()

            Me.tempFile.WriteLines("127.0.0.1 example.com", "::1 example.com", "# 0.0.0.0 EXAMPLE.com")

            Me.hosts.Remove(New MappingInfo("127.0.0.1", "example.com"))

            Assert.IsFalse(Me.hosts.IsMapped("example.com"))
            Assert.AreEqual(0, Me.hosts.GetMappings().Count)

        End Sub

        <TestMethod>
        Public Sub Remove_HostNameSharingALine_KeepsTheOtherHostNames()

            Me.tempFile.WriteLines("0.0.0.0 a.com b.com # shared")

            Me.hosts.Remove("a.com")

            Dim mapping As MappingInfo = Me.hosts.GetMappings().Single()
            Assert.AreEqual("b.com", mapping.HostName)
            Assert.AreEqual("0.0.0.0", mapping.IP)
            Assert.AreEqual("shared", mapping.Comment)

        End Sub

        <TestMethod>
        Public Sub Remove_PreservesTheOtherLinesUntouched()

            Dim untouchedLines As String() = {"# header", "0.0.0.0     keep.com", String.Empty}
            Me.tempFile.WriteLines(untouchedLines.Concat({"127.0.0.1 example.com"}).ToArray())

            Me.hosts.Remove("example.com")

            CollectionAssert.AreEqual(untouchedLines, Me.tempFile.ReadLines())

        End Sub

        <TestMethod>
        Public Sub Remove_NotMapped_ThrowsInvalidOperationException()

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() Me.hosts.Remove("example.com"))

        End Sub

        <TestMethod>
        Public Sub Remove_NullMappingInfo_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Me.hosts.Remove(CType(Nothing, MappingInfo)))

        End Sub

#End Region

#Region " GetMappingFromHostname / GetMappingsFromIP "

        <TestMethod>
        Public Sub GetMappingFromHostname_EntryWithoutComment_ReturnsEmptyComment()

            ' Regression: entries without comment threw IndexOutOfRangeException.
            Me.tempFile.WriteLines($"127.0.0.1{ControlChars.Tab}example.com")

            Dim mapping As MappingInfo = Me.hosts.GetMappingFromHostname("example.com")

            Assert.AreEqual("127.0.0.1", mapping.IP)
            Assert.AreEqual(String.Empty, mapping.Comment)
            Assert.IsFalse(mapping.IsDisabled)

        End Sub

        <TestMethod>
        Public Sub GetMappingFromHostname_EntryWithComment_ReturnsTheCommentWithoutHash()

            ' Regression: the comment was returned with its leading '#' character.
            Me.tempFile.WriteLines($"127.0.0.1{ControlChars.Tab}example.com{ControlChars.Tab}#My comment")

            Assert.AreEqual("My comment", Me.hosts.GetMappingFromHostname("EXAMPLE.com").Comment)

        End Sub

        <TestMethod>
        Public Sub GetMappingFromHostname_SeveralEntries_PrefersTheFirstEnabledOne()

            Me.tempFile.WriteLines("# 0.0.0.0 example.com", "127.0.0.1 example.com", "10.0.0.1 example.com")

            Dim mapping As MappingInfo = Me.hosts.GetMappingFromHostname("example.com")

            Assert.AreEqual("127.0.0.1", mapping.IP)
            Assert.IsFalse(mapping.IsDisabled)

        End Sub

        <TestMethod>
        Public Sub GetMappingFromHostname_OnlyDisabledEntries_ReturnsTheFirstOne()

            Me.tempFile.WriteLines("# 0.0.0.0 example.com", "# 127.0.0.1 example.com")

            Assert.AreEqual("0.0.0.0", Me.hosts.GetMappingFromHostname("example.com").IP)

        End Sub

        <TestMethod>
        Public Sub GetMappingFromHostname_NotMapped_ThrowsInvalidOperationException()

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() Me.hosts.GetMappingFromHostname("example.com"))

        End Sub

        <TestMethod>
        Public Sub GetMappingsFromIP_IncludesTheDisabledEntries()

            ' Regression: disabled entries ("# 127.0.0.1") were not matched because of the space after '#'.
            Me.tempFile.WriteLines("127.0.0.1 a.com", "# 127.0.0.1 b.com", "0.0.0.0 c.com")

            Dim mappings As List(Of MappingInfo) = Me.hosts.GetMappingsFromIP("127.0.0.1")

            CollectionAssert.AreEqual({"a.com", "b.com"}, mappings.Select(Function(m As MappingInfo) m.HostName).ToArray())

        End Sub

        <TestMethod>
        Public Sub GetMappingsFromIP_IPv6_ComparesAddressesSemantically()

            Me.tempFile.WriteLines("0:0:0:0:0:0:0:1 expanded.local", "::1 compact.local")

            Dim mappings As List(Of MappingInfo) = Me.hosts.GetMappingsFromIP("::1")

            Assert.AreEqual(2, mappings.Count)

        End Sub

        <TestMethod>
        Public Sub GetMappingsFromIP_InvalidAddress_ThrowsFormatException()

            Me.hosts.FileCreate()

            Assert.ThrowsExactly(Of FormatException)(Sub() Me.hosts.GetMappingsFromIP("not-an-ip"))

        End Sub

#End Region

#Region " WriteMappings "

        <TestMethod>
        Public Sub WriteMappings_RoundTrip_PreservesAllTheValues()

            Dim expected As New List(Of MappingInfo) From {
                New MappingInfo("127.0.0.1", "local.example.com", "Local", isDisabled:=False),
                New MappingInfo("0.0.0.0", "blocked.example.com", String.Empty, isDisabled:=True),
                New MappingInfo("::1", "ipv6.example.com", "IPv6 with spaces in comment", isDisabled:=False)
            }

            Me.hosts.WriteMappings(expected)
            Dim actual As List(Of MappingInfo) = Me.hosts.GetMappings()

            Assert.AreEqual(expected.Count, actual.Count)
            For i As Integer = 0 To expected.Count - 1
                Assert.AreEqual(expected(i).IP, actual(i).IP)
                Assert.AreEqual(expected(i).HostName, actual(i).HostName)
                Assert.AreEqual(expected(i).Comment, actual(i).Comment)
                Assert.AreEqual(expected(i).IsDisabled, actual(i).IsDisabled)
            Next i

            Assert.IsTrue(Me.tempFile.ReadText().StartsWith(HostsTools.DefaultHeader, StringComparison.Ordinal))

        End Sub

        <TestMethod>
        Public Sub WriteMappings_SameHostNameForIPv4AndIPv6_IsAllowed()

            ' Regression: saving from the UI used Add, which rejected any repeated hostname and aborted the save.
            Dim mappings As New List(Of MappingInfo) From {
                New MappingInfo("127.0.0.1", "localhost"),
                New MappingInfo("::1", "localhost")
            }

            Me.hosts.WriteMappings(mappings)

            Assert.AreEqual(2, Me.hosts.GetMappings().Count)

        End Sub

        <TestMethod>
        Public Sub WriteMappings_InvalidMapping_DoesNotModifyTheFile()

            Me.tempFile.WriteLines("127.0.0.1 original.com")
            Dim originalText As String = Me.tempFile.ReadText()

            Dim mappings As New List(Of MappingInfo) From {
                New MappingInfo("127.0.0.1", "valid.com"),
                New MappingInfo("127.0.0.1", "invalid host name")
            }

            Assert.ThrowsExactly(Of ArgumentException)(Sub() Me.hosts.WriteMappings(mappings))
            Assert.AreEqual(originalText, Me.tempFile.ReadText())

        End Sub

        <TestMethod>
        Public Sub WriteMappings_InvalidIPAddress_ThrowsFormatException()

            Assert.ThrowsExactly(Of FormatException)(Sub() Me.hosts.WriteMappings({New MappingInfo("1.2.3", "example.com")}))
            Assert.IsFalse(Me.hosts.FileExists())

        End Sub

        <TestMethod>
        Public Sub WriteMappings_NullElement_ThrowsArgumentException()

            Assert.ThrowsExactly(Of ArgumentException)(Sub() Me.hosts.WriteMappings({New MappingInfo("127.0.0.1", "a.com"), Nothing}))

        End Sub

        <TestMethod>
        Public Sub WriteMappings_NullCollection_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() Me.hosts.WriteMappings(Nothing))

        End Sub

        <TestMethod>
        Public Sub WriteMappings_MissingFile_CreatesIt()

            Me.hosts.WriteMappings(New List(Of MappingInfo))

            Assert.IsTrue(Me.hosts.FileExists())
            Assert.AreEqual(0, Me.hosts.GetMappings().Count)

        End Sub

        <TestMethod>
        Public Sub WriteMappings_Utf8WithoutBom_WritesNoBomAndRoundTripsNonAsciiComments()

            Me.hosts.WriteMappings({New MappingInfo("127.0.0.1", "example.com", "Café ñandú")})

            Dim bytes As Byte() = File.ReadAllBytes(Me.tempFile.FilePath)
            Assert.AreNotEqual(&HEF, CInt(bytes(0)))
            Assert.AreEqual("Café ñandú", Me.hosts.GetMappings().Single().Comment)

        End Sub

        <TestMethod>
        Public Sub WriteMappings_LargeBlockList_IsWrittenAndReadInLinearTime()

            ' Regression: saving from the UI called Add once per mapping, and every Add re-read the whole file (O(n²)),
            ' which made saving block lists of tens of thousands of entries practically impossible.
            Dim mappings As New List(Of MappingInfo)
            For i As Integer = 0 To 49999
                mappings.Add(New MappingInfo("0.0.0.0", $"ads{i}.example.com"))
            Next i

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Me.hosts.WriteMappings(mappings)
            Dim readBack As List(Of MappingInfo) = Me.hosts.GetMappings()
            watch.Stop()

            Assert.AreEqual(mappings.Count, readBack.Count)
            Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(30), $"Elapsed: {watch.Elapsed}")

        End Sub

#End Region

    End Class

End Namespace

#End Region
