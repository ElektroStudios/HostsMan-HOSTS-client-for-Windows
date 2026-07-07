' ***********************************************************************
' Author           : Elektro
' Last Modified On : 18-January-2015
' ***********************************************************************
' <copyright file="HostsTools.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Usage Examples "

'Public Class HostsFileTestClass
'
'    Private Sub HostsFileTestHandler() Handles MyBase.Shown
'
'        ' Instance the HostsFile Class.
'        Dim Hosts As New HostsFile()
'
'        ' Set a new mapping.
'        Dim Mapping As New HostsFile.MappingInfo
'        With Mapping
'            .HostName = "cuantodanio.es"
'            .IP = Hosts.LOCALHOST ' "127.0.0.1"
'            .Comment = "Test mapping comment."
'        End With
'
'        With Hosts
'
'            ' Delete the Host file.
'            If .FileExists Then
'                .FileDelete()
'            End If
'
'            ' Create a new one Hosts file.
'            .FileCreate()
'
'            ' Add some new mappings.
'            .Add(Mapping)
'            .Add(HostName:="www.youtube.com", IP:=.LOCALHOST, Comment:="Test mapping comment")
'
'            ' Check whether a mapping exists.
'            If .IsMapped(Mapping) Then
'                ' Disable the mapping.
'                .Disable(Mapping)
'            End If
'
'            ' Check whether an existing mapping is disabled.
'            If .IsDisabled("www.youtube.com") Then
'                ' Remove the mapping.
'                .Remove("www.youtube.com")
'            End If
'
'            ' Open the HOSTS file with the specified text-editor.
'            .FileOpen("C:\Program Files\Sublime Text\sublime_text.exe")
'
'        End With
'
'        ' Get the IP of a mapped Hostname.
'        MessageBox.Show("cuantodanio.es: " & Hosts.GetMappingFromHostname("cuantodanio.es").IP)
'
'        ' Get all the hostname mappings
'        Dim Mappings As List(Of HostsFile.MappingInfo) = Hosts.GetMappings()
'        For Each MappingInfo As HostsFile.MappingInfo In Mappings
'
'            Dim sb As New System.Text.StringBuilder
'            With sb
'                .AppendLine(String.Format("Hostname...: {0}", MappingInfo.HostName))
'                .AppendLine(String.Format("IP Address.: {0}", MappingInfo.IP))
'                .AppendLine(String.Format("Comment....: {0}", MappingInfo.Comment))
'                .AppendLine(String.Format("Is Enabled?: {0}", Not MappingInfo.IsDisabled))
'            End With
'
'            MessageBox.Show(sb.ToString, "HostsFile Mappings", MessageBoxButtons.OK, MessageBoxIcon.Information)
'
'        Next MappingInfo
'
'        ' Get all the hostname mappings that matches an ip address
'        Dim MappingMatches As List(Of HostsFile.MappingInfo) = Hosts.GetMappingsFromIP(Hosts.LOCALHOST)
'
'    End Sub
'
'End Class

#End Region

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Net
Imports System.Text

#End Region

#Region " Hosts File "

Namespace Tools

    ''' <summary>
    ''' Manages the Windows HOSTS file to map Hostnames to IP addresses.
    ''' </summary>
    Public NotInheritable Class HostsTools

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="HostsTools"/> class.
        ''' </summary>
        ''' <param name="HOSTSLocation">
        ''' Optionaly indicates a custom Hosts file location.
        ''' Default value is 'X:\Windows\System32\Drivers\etc\hosts'.
        ''' </param>
        Public Sub New(Optional ByVal hostsLocation As String = Nothing)

            If Not String.IsNullOrEmpty(hostsLocation) Then
                Me.hostsLocation1 = hostsLocation
            End If

        End Sub

        ''' <summary>
        ''' Prevents a default instance of the <see cref="HostsTools"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Properties "

        ''' <summary>
        ''' The Hosts file location.
        ''' </summary>
        ''' <value>The Hosts file location.</value>
        Public Property HostsLocation As String
            Set(ByVal value As String)
                hostsLocation1 = value
            End Set
            Get
                Return hostsLocation1
            End Get
        End Property
        Private ReadOnly sysDir As String = Environment.GetFolderPath(Environment.SpecialFolder.System)
        Private hostsLocation1 As String = Path.Combine(sysDir, "Drivers\etc\hosts")

        ''' <summary>
        ''' The Hosts file encoding.
        ''' The encoding must be <see cref="Encoding.Default"/> (ANSI) or <see cref="Encoding.UTF8"/> (UTF-8 without BOM), 
        ''' otherwise the entries will be ignored by Windows.
        ''' </summary>
        ''' <value>The Hosts file encoding.</value>
        Public Property HostsEncoding As Encoding
            Get
                Return hostsEncoding1
            End Get
            Set(ByVal value As Encoding)
                Me.hostsEncoding1 = value
            End Set
        End Property
        Private hostsEncoding1 As Encoding = Encoding.Default

        ''' <summary>
        ''' Gets or sets the default 'LocalHost' IP address.
        ''' In most computers the default address is '127.0.0.1'.
        ''' </summary>
        ''' <value>The default LocalHost.</value>
        Public Property Localhost As String
            Get
                Return Me.localhost1
            End Get
            Set(ByVal value As String)
                Me.localhost1 = value
            End Set
        End Property
        Private localhost1 As String = "127.0.0.1"

        ''' <summary>
        ''' Gets the default Hosts file header.
        ''' </summary>
        Private ReadOnly hostsHeader As String =
<a><![CDATA[# Copyright (c) 1993-2009 Microsoft Corp.
#
# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.
#
# This file contains the mappings of IP addresses to host names. Each
# entry should be kept on an individual line. The IP address should
# be placed in the first column followed by the corresponding host name.
# The IP address and the host name should be separated by at least one
# space.
]]></a>.Value

#End Region

#Region " Types "

#Region " MappingInfo "

        ''' <summary>
        ''' Specifies info of a HOSTS file mapping.
        ''' </summary>
        Public Class MappingInfo

            ''' <summary>
            ''' Gets or sets the hostname.
            ''' </summary>
            ''' <value>The hostname.</value>
            Public Property HostName As String

            ''' <summary>
            ''' Gets or sets the IP address.
            ''' </summary>
            ''' <value>The IP address.</value>
            Public Property IP As String

            ''' <summary>
            ''' Gets or sets the mapping comment.
            ''' </summary>
            ''' <value>The mapping comment.</value>
            Public Property Comment As String

            ''' <summary>
            ''' This value is reserved.
            ''' Gets a value indicating whether the mapping is disabled in the HOSTS file.
            ''' </summary>
            ''' <value><c>true</c> if the mapping is disabled, <c>false</c> otherwise.</value>
            Public Property IsDisabled As Boolean

        End Class

#End Region

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Adds a new mapping.
        ''' </summary>
        ''' <param name="HostName">Indicates the Hostname.</param>
        ''' <param name="IP">Indicates the IP address.</param>
        ''' <param name="Comment">Indicates a comment for this mapping.</param>
        ''' <exception cref="System.IO.FileNotFoundException">"Hosts file not found."</exception>
        ''' <exception cref="System.FormatException">Invalid IP adress.</exception>
        ''' <exception cref="System.Exception">Hostname is already mapped.</exception>
        Public Sub Add(ByVal enabled As Boolean,
                       ByVal hostName As String,
                       ByVal ip As String,
                       Optional ByVal comment As String = Nothing)

            If Not Me.FileExists() Then ' Hosts file does not exists.
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            ElseIf Not Me.ValidateIP(ip) Then ' Invalid IP address.
                Throw New FormatException(String.Format("Address: '{0}' is not a valid IP adress.", ip))

            ElseIf Me.IsMapped(hostName) Then ' Hostname is already mapped.
                Throw New Exception(String.Format("Hostname '{0}' is already mapped.", hostName))

            Else ' Add the entry.

                ' Fix value spacing.
                Dim entryFormat As String =
                    ip & hostName.Insert(0I, ControlChars.Tab) &
                    If(Not String.IsNullOrEmpty(comment),
                       comment.Insert(0I, ControlChars.Tab & "#"c),
                       String.Empty)

                ' Write the mapping.
                File.AppendAllText(Me.hostsLocation1, Environment.NewLine & If(enabled, entryFormat, "# " & entryFormat), Me.hostsEncoding1)

            End If

        End Sub

        ''' <summary>
        ''' Adds a new mapping.
        ''' </summary>
        ''' <param name="MappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        Public Sub Add(ByVal mappingInfo As MappingInfo)

            Me.Add(Not mappingInfo.IsDisabled, mappingInfo.HostName, mappingInfo.IP, mappingInfo.Comment)

        End Sub

        ''' <summary>
        ''' Disables an existing mapping.
        ''' </summary>
        ''' <param name="HostName">Indicates the Hostname.</param>
        ''' <exception cref="System.IO.FileNotFoundException">"Hosts file not found."</exception>
        ''' <exception cref="System.Exception">Hostname is not mapped.</exception>
        ''' <exception cref="System.Exception">Hostname is already disabled.</exception>
        Public Sub Disable(ByVal hostName As String)

            If Not Me.FileExists() Then ' Hosts file does not exists.
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            ElseIf Not Me.IsMapped(hostName) Then ' Hostname is not mapped.
                Throw New Exception(String.Format("Hostname: '{0}' is not mapped.", hostName))

            ElseIf Me.IsDisabled(hostName) Then ' Hostname is already disabled.
                Throw New Exception(String.Format("Hostname: '{0}' is already disabled.", hostName))

            Else ' Disable the mapping.

                ' Retrieve the HOSTS file content.
                Dim hosts As List(Of String) = File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1).ToList

                ' Iterate the mappings.
                For x As Integer = 0I To (hosts.Count - 1I)

                    If Not String.IsNullOrEmpty(hosts(x)) AndAlso hosts(x).Contains(ControlChars.Tab) Then

                        ' Retrieve the HostName of this mapping.
                        Dim host As String = hosts(x).Split({ControlChars.Tab})(1I)

                        If host.Equals(hostName, StringComparison.OrdinalIgnoreCase) Then

                            ' Disable the mapping.
                            hosts(x) = hosts(x).Insert(0I, "#"c)
                            Exit For

                        End If ' Host.Equals(...)

                    End If ' Not String.IsNullOrEmpty(Hosts(X))...

                Next x

                File.WriteAllLines(Me.hostsLocation1, hosts, Me.hostsEncoding1)

            End If

        End Sub

        ''' <summary>
        ''' Disables an existing mapping.
        ''' </summary>
        ''' <param name="MappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        Public Sub Disable(ByVal mappingInfo As MappingInfo)

            Me.Disable(mappingInfo.HostName)

        End Sub

        ''' <summary>
        ''' Removes a mapping.
        ''' </summary>
        ''' <param name="HostName">Indicates the Hostname.</param>
        ''' <exception cref="System.IO.FileNotFoundException">"Hosts file not found."</exception>
        ''' <exception cref="System.Exception">Hostname is not mapped.</exception>
        Public Sub Remove(ByVal hostName As String)

            If Not Me.FileExists() Then ' Hosts file does not exists.
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            ElseIf Not Me.IsMapped(hostName) Then ' Hostname is not mapped.
                Throw New Exception(String.Format("Hostname: '{0}' is not mapped.", hostName))

            Else ' Remove the mapping.

                ' Retrieve the HOSTS file content.
                Dim hosts As List(Of String) = File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1).ToList

                ' Iterate the mappings.
                For x As Integer = 0I To (hosts.Count - 1I)

                    If Not String.IsNullOrEmpty(hosts(x)) AndAlso hosts(x).Contains(ControlChars.Tab) Then

                        ' Retrieve the HostName of this mapping.
                        Dim host As String = hosts(x).Split({ControlChars.Tab})(1I)

                        If host.Equals(hostName, StringComparison.OrdinalIgnoreCase) Then

                            ' Remove the mapping.
                            hosts.RemoveAt(x)
                            Exit For

                        End If ' Host.Equals(...)

                    End If ' Not String.IsNullOrEmpty(Hosts(X))...

                Next x

                File.WriteAllLines(Me.hostsLocation1, hosts, Me.hostsEncoding1)

            End If

        End Sub

        ''' <summary>
        ''' Removes a mapping.
        ''' </summary>
        ''' <param name="MappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        Public Sub Remove(ByVal mappingInfo As MappingInfo)

            Me.Remove(mappingInfo.HostName)

        End Sub

        ''' <summary>
        ''' Gets a <see cref="List(Of HostsMapping)"/> instance containing the mapping info of all mappings.
        ''' </summary>
        ''' <exception cref="System.IO.FileNotFoundException">"Hosts file not found."</exception>
        Public Function GetMappings() As List(Of MappingInfo)

            If Not Me.FileExists() Then ' Hosts file does not exists.
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            Else ' Get the mapping.

                ' Retrieve the HOSTS file content.
                Dim hosts As List(Of String) = File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1).ToList
                Dim mappings As New List(Of MappingInfo)

                ' Iterate the mappings.
                For x As Integer = 0 To (hosts.Count - 1)

                    If Not String.IsNullOrEmpty(hosts(x))  Then

                        ' Retrieve the mapping parts.
                        Dim parts As String() = hosts(x).Split({ControlChars.Tab, Convert.ToChar(Keys.Space)}, StringSplitOptions.RemoveEmptyEntries)

                        If parts.Count > 1 AndAlso
                            (parts(0) Like "#**.#**.#**.#**" OrElse parts(0) Like "****.#**.#**.#**" OrElse parts(1) Like "#**.#**.#**.#**") Then

                            Dim isDisabled As Boolean = parts(0).TrimStart.StartsWith("#"c)
                            Dim mappingInfo As New MappingInfo

                            Select Case isDisabled

                                Case True
                                    With mappingInfo
                                        .IsDisabled = True
                                        .IP = If(parts(0).Trim.Length > 1, parts(0).Replace("#"c, String.Empty), parts(1).Replace("#"c, String.Empty))
                                        .HostName = If(parts(0).Trim.Length > 1, parts(1), parts(2))
                                        .Comment = If(parts(0).Trim.Length > 1 AndAlso parts.Count > 2, String.Join(" ", parts.Skip(2)).TrimStart("#"c, " "c), String.Join(" ", parts.Skip(3)).TrimStart("#"c, " "c))
                                    End With ' MappingInfo

                                Case Else
                                    With mappingInfo
                                        .IsDisabled = False
                                        .IP = parts(0).Replace("#"c, String.Empty)
                                        .HostName = parts(1)
                                        .Comment = If(parts.Count > 2, String.Join(" ", parts.Skip(2)).TrimStart("#"c, " "c), String.Empty)
                                    End With ' MappingInfo

                            End Select

                            mappings.Add(mappingInfo)

                        End If

                    End If ' Not String.IsNullOrEmpty(Hosts(X))...

                Next x

                Return mappings

            End If

        End Function

        ''' <summary>
        ''' Gets a <see cref="MappingInfo"/> instance containing the mapping info of a Hostname.
        ''' </summary>
        ''' <param name="HostName">Indicates the Hostname.</param>
        ''' <exception cref="System.IO.FileNotFoundException">"Hosts file not found."</exception>
        ''' <exception cref="System.Exception">Hostname is not mapped.</exception>
        Public Function GetMappingFromHostname(ByVal hostname As String) As MappingInfo

            If Not Me.FileExists() Then ' Hosts file does not exists.
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            ElseIf Not Me.IsMapped(hostname) Then ' Hostname is not mapped.
                Throw New Exception(String.Format("Hostname: '{0}' is not mapped.", hostname))

            Else ' Get the mapping.

                ' Retrieve the HOSTS file content.
                Dim hosts As List(Of String) = File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1).ToList
                Dim mappingInfo As New MappingInfo

                ' Iterate the mappings.
                For x As Integer = 0I To (hosts.Count - 1I)

                    If Not String.IsNullOrEmpty(hosts(x)) AndAlso hosts(x).Contains(ControlChars.Tab) Then

                        ' Retrieve the mapping parts.
                        Dim parts As String() = hosts(x).Split({ControlChars.Tab})

                        If parts(1I).Equals(hostname, StringComparison.OrdinalIgnoreCase) Then

                            With mappingInfo
                                .HostName = parts(1I)
                                .IP = parts(0I).Replace("#"c, String.Empty)
                                .Comment = If(parts.Count > 1I, parts(2I), String.Empty)
                                .IsDisabled = parts(0I).TrimStart.StartsWith("#"c)
                            End With ' MappingInfo

                            Exit For

                        End If ' Parts(1I).Equals(Hostname)...

                    End If ' Not String.IsNullOrEmpty(Hosts(X))...

                Next x

                Return mappingInfo

            End If

        End Function

        ''' <summary>
        ''' Gets a <see cref="List(Of HostsMapping)"/> instance containing the mapping info of all mappings
        ''' matching the specified IP address.
        ''' </summary>
        ''' <exception cref="System.IO.FileNotFoundException">"Hosts file not found."</exception>
        ''' <exception cref="System.FormatException">Invalid IP adress.</exception>
        Public Function GetMappingsFromIP(ByVal ip As String) As List(Of MappingInfo)

            If Not Me.FileExists() Then ' Hosts file does not exists.
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            ElseIf Not Me.ValidateIP(ip) Then ' Invalid IP address.
                Throw New FormatException(String.Format("Address: '{0}' is not a valid IP adress.", ip))

            Else ' Get the mapping.

                ' Retrieve the HOSTS file content.
                Dim hosts As List(Of String) = File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1).ToList
                Dim mappings As New List(Of MappingInfo)

                ' Iterate the mappings.
                For x As Integer = 0I To (hosts.Count - 1I)

                    If Not String.IsNullOrEmpty(hosts(x)) AndAlso hosts(x).Contains(ControlChars.Tab) Then

                        ' Retrieve the mapping parts.
                        Dim parts As String() = hosts(x).Split({ControlChars.Tab})

                        If parts(0I).Replace("#"c, String.Empty).Equals(ip) Then

                            Dim mappingInfo As New MappingInfo
                            With mappingInfo
                                .HostName = parts(1I)
                                .IP = parts(0I).Replace("#"c, String.Empty)
                                .Comment = If(parts.Count > 1I, parts(2I), String.Empty)
                                .IsDisabled = parts(0I).TrimStart.StartsWith("#"c)
                            End With ' MappingInfo

                            mappings.Add(mappingInfo)

                        End If

                    End If ' Not String.IsNullOrEmpty(Hosts(X))...

                Next x

                Return mappings

            End If

        End Function

        ''' <summary>
        ''' Checks whether a HostName is already mapped.
        ''' </summary>
        ''' <param name="HostName">Indicates the Hostname.</param>
        ''' <returns><c>true</c> if the specified Hostname is mapped; otherwise, <c>false</c>.</returns>
        ''' <exception cref="System.IO.FileNotFoundException">"Hosts file not found."</exception>
        Public Function IsMapped(ByVal hostName As String) As Boolean

            If Not Me.FileExists() Then ' Hosts file does not exists.
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            Else
                ' Retrieve the HOSTS file content.
                Dim hosts As List(Of String) = File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1).ToList

                ' Iterate the mappings.
                For x As Integer = 0I To (hosts.Count - 1I)

                    If Not String.IsNullOrEmpty(hosts(x)) AndAlso hosts(x).Contains(ControlChars.Tab) Then

                        ' Retrieve the HostName of this mapping.
                        Dim host As String = hosts(x).Split({ControlChars.Tab})(1I)

                        If host.Equals(hostName, StringComparison.OrdinalIgnoreCase) Then
                            Return True
                        End If ' Host.Equals(HostName)...

                    End If ' Not String.IsNullOrEmpty(Hosts(X)) AndAlso...

                Next x

                Return False

            End If ' Not Me.Exists()...

        End Function

        ''' <summary>
        ''' Checks whether a HostName is already mapped.
        ''' </summary>
        ''' <param name="MappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        ''' <returns><c>true</c> if the specified Hostname is mapped; otherwise, <c>false</c>.</returns>
        Public Function IsMapped(ByVal mappingInfo As MappingInfo) As Boolean

            Return Me.IsMapped(mappingInfo.HostName)

        End Function

        ''' <summary>
        ''' Checks whether a HostName is already disabled.
        ''' </summary>
        ''' <param name="HostName">Indicates the Hostname.</param>
        ''' <returns><c>true</c> if the specified Hostname is disabled; otherwise, <c>false</c>.</returns>
        ''' <exception cref="System.IO.FileNotFoundException">"Hosts file not found."</exception>
        ''' <exception cref="System.Exception">Hostname is not mapped.</exception>
        Public Function IsDisabled(ByVal hostName As String) As Boolean

            If Not Me.FileExists() Then ' Hosts file does not exists.
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            ElseIf Not Me.IsMapped(hostName) Then ' Hostname is not mapped.
                Throw New Exception(String.Format("Hostname: '{0}' is not mapped.", hostName))

            Else
                ' Retrieve the HOSTS file content.
                Dim hosts As List(Of String) = File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1).ToList
                Dim result As Boolean = False

                ' Iterate the mappings.
                For x As Integer = 0I To (hosts.Count - 1I)

                    If Not String.IsNullOrEmpty(hosts(x)) AndAlso hosts(x).Contains(ControlChars.Tab) Then

                        ' Retrieve the HostName of this mapping.
                        Dim host As String = hosts(x).Split({ControlChars.Tab})(1I)

                        If host.Equals(hostName, StringComparison.OrdinalIgnoreCase) Then
                            result = hosts(x).TrimStart.StartsWith("#"c)
                            Exit For
                        End If ' Host.Equals(HostName)...

                    End If ' Not String.IsNullOrEmpty(Hosts(X)) AndAlso...

                Next x

                Return result

            End If

        End Function

        ''' <summary>
        ''' Checks whether a HostName is already disabled.
        ''' </summary>
        ''' <param name="MappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        ''' <returns><c>true</c> if the specified Hostname is disabled; otherwise, <c>false</c>.</returns>
        Public Function IsDisabled(ByVal mappingInfo As MappingInfo) As Boolean

            Return Me.IsDisabled(mappingInfo.HostName)

        End Function

        ''' <summary>
        ''' Checks whether the Hosts file exists.
        ''' </summary>
        ''' <returns><c>true</c> if Hosts file exists, <c>false</c> otherwise.</returns>
        Public Function FileExists() As Boolean

            Return File.Exists(Me.hostsLocation1)

        End Function

        ''' <summary>
        ''' Creates the Hosts file.
        ''' </summary>
        Public Sub FileCreate()

            If Me.FileExists() Then
                File.Delete(Me.hostsLocation1)
            End If

            File.WriteAllText(Me.hostsLocation1, Me.hostsHeader, Me.hostsEncoding1)

        End Sub

        ''' <summary>
        ''' Deletes the Hosts file.
        ''' </summary>
        ''' <exception cref="System.IO.FileNotFoundException">Hosts file not found.</exception>
        Public Sub FileDelete()

            If Not Me.FileExists() Then
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            Else
                File.Delete(Me.hostsLocation1)

            End If

        End Sub

        ''' <summary>
        ''' Cleans the Hosts file.
        ''' This removes all the mappings and adds the default file header.
        ''' </summary>
        Public Sub FileClean()

            Me.FileCreate()

        End Sub

        ''' <summary>
        ''' Opens the Hosts file with the specified process.
        ''' </summary>
        ''' <param name="Process">
        ''' Indicates the process location.
        ''' Default value is: "notepad.exe".
        ''' </param>
        ''' <exception cref="System.IO.FileNotFoundException">Hosts file not found.</exception>
        ''' <exception cref="System.IO.FileNotFoundException">Process not found.</exception>
        Public Sub FileOpen(Optional ByVal process As String = "notepad.exe")

            If Not Me.FileExists Then
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)

            ElseIf Not File.Exists(process) Then
                Throw New FileNotFoundException("Process not found.", process)

            Else
                Diagnostics.Process.Start(process, ControlChars.Quote & Me.hostsLocation1 & ControlChars.Quote)

            End If

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Validates an IP address.
        ''' </summary>
        ''' <param name="Address">The IP address.</param>
        ''' <returns><c>true</c> if IP is in the proper format, <c>false</c> otherwise.</returns>
        Private Function ValidateIP(ByVal address As String) As Boolean

            Dim ip As IPAddress = Nothing
            Return IPAddress.TryParse(address, ip)

        End Function

#End Region

    End Class

End Namespace

#End Region