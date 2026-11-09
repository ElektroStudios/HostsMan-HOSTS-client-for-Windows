' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="MappingValidatorTests.vb" company="Elektro Studios">
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

#End Region

#Region " MappingValidatorTests "

Namespace Tools

    ''' <summary>
    ''' Tests for <see cref="MappingValidator"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class MappingValidatorTests

        <TestMethod>
        <DataRow("127.0.0.1")>
        <DataRow("0.0.0.0")>
        <DataRow("255.255.255.255")>
        <DataRow("192.168.001.010")>
        <DataRow("::1")>
        <DataRow("::")>
        <DataRow("2001:db8::ff00:42:8329")>
        <DataRow("FE80::1")>
        <DataRow("::ffff:192.168.1.1")>
        Public Sub IsValidIPAddress_ValidAddress_ReturnsTrue(ByVal value As String)

            Assert.IsTrue(MappingValidator.IsValidIPAddress(value))

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow(" ")>
        <DataRow("256.0.0.1")>
        <DataRow("300.1.1.1")>
        <DataRow("1")>
        <DataRow("127.1")>
        <DataRow("127.0.0")>
        <DataRow("127.0.0.1.5")>
        <DataRow("0x7f.0.0.1")>
        <DataRow("1abc.2.3.4")>
        <DataRow("-1.0.0.0")>
        <DataRow("1234.0.0.1")>
        <DataRow(" 127.0.0.1")>
        <DataRow("127.0.0.1 ")>
        <DataRow("127. 0.0.1")>
        <DataRow("localhost")>
        <DataRow("::g")>
        <DataRow("1:2:3:4:5:6:7:8:9")>
        Public Sub IsValidIPAddress_InvalidAddress_ReturnsFalse(ByVal value As String)

            Assert.IsFalse(MappingValidator.IsValidIPAddress(value))

        End Sub

        <TestMethod>
        Public Sub IsValidIPAddress_Nothing_ReturnsFalse()

            Assert.IsFalse(MappingValidator.IsValidIPAddress(Nothing))

        End Sub

        <TestMethod>
        <DataRow("localhost")>
        <DataRow("example.com")>
        <DataRow("sub-domain.example.co.uk")>
        <DataRow("_service.example.com")>
        <DataRow("xn--bcher-kva.example")>
        <DataRow("UPPER.CASE.COM")>
        Public Sub IsValidHostName_ValidHostName_ReturnsTrue(ByVal value As String)

            Assert.IsTrue(MappingValidator.IsValidHostName(value))

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow(" ")>
        <DataRow("two words.com")>
        <DataRow("tab" & vbTab & "host")>
        <DataRow("hash#host")>
        <DataRow("line" & vbLf & "break")>
        Public Sub IsValidHostName_InvalidHostName_ReturnsFalse(ByVal value As String)

            Assert.IsFalse(MappingValidator.IsValidHostName(value))

        End Sub

        <TestMethod>
        Public Sub IsValidHostName_Nothing_ReturnsFalse()

            Assert.IsFalse(MappingValidator.IsValidHostName(Nothing))

        End Sub

        <TestMethod>
        Public Sub IsValidHostName_LengthLimit_IsEnforced()

            Assert.IsTrue(MappingValidator.IsValidHostName(New String("a"c, MappingValidator.MaxHostNameLength)))
            Assert.IsFalse(MappingValidator.IsValidHostName(New String("a"c, MappingValidator.MaxHostNameLength + 1)))

        End Sub

        <TestMethod>
        <DataRow("")>
        <DataRow("A comment")>
        <DataRow("Comment # with hash")>
        <DataRow("Tab" & vbTab & "separated")>
        <DataRow("Ünïcödé çømmént")>
        Public Sub IsValidComment_ValidComment_ReturnsTrue(ByVal value As String)

            Assert.IsTrue(MappingValidator.IsValidComment(value))

        End Sub

        <TestMethod>
        Public Sub IsValidComment_Nothing_ReturnsTrue()

            Assert.IsTrue(MappingValidator.IsValidComment(Nothing))

        End Sub

        <TestMethod>
        <DataRow("line" & vbCr & "break")>
        <DataRow("line" & vbLf & "break")>
        <DataRow("line" & vbCrLf & "break")>
        <DataRow("null" & vbNullChar & "char")>
        Public Sub IsValidComment_ControlCharacters_ReturnsFalse(ByVal value As String)

            Assert.IsFalse(MappingValidator.IsValidComment(value))

        End Sub

    End Class

End Namespace

#End Region
