' ***********************************************************************
' Author           : Elektro
' Last Modified On : 09-November-2026
' ***********************************************************************
' <copyright file="HostsTools.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Usage Examples "

'Public Class HostsToolsTestClass
'
'    Private Sub HostsToolsTestHandler() Handles MyBase.Shown
'
'        ' Instance the HostsTools Class.
'        Dim hosts As New HostsTools()
'
'        ' Set a new mapping.
'        Dim mapping As New HostsTools.MappingInfo With {
'            .HostName = "cuantodanio.es",
'            .IP = hosts.Localhost, ' "127.0.0.1"
'            .Comment = "Test mapping comment."
'        }
'
'        ' Delete the Hosts file.
'        If hosts.FileExists() Then
'            hosts.FileDelete()
'        End If
'
'        ' Create a new Hosts file.
'        hosts.FileCreate()
'
'        ' Add some new mappings.
'        hosts.Add(mapping)
'        hosts.Add(enabled:=True, hostName:="www.youtube.com", ip:=hosts.Localhost, comment:="Test mapping comment")
'
'        ' Check whether a mapping exists, then disable it.
'        If hosts.IsMapped(mapping) Then
'            hosts.Disable(mapping)
'        End If
'
'        ' Check whether an existing mapping is disabled, then remove it.
'        If hosts.IsDisabled("www.youtube.com") Then
'            hosts.Remove("www.youtube.com")
'        End If
'
'        ' Open the HOSTS file with the specified text-editor.
'        hosts.FileOpen("C:\Program Files\Sublime Text\sublime_text.exe")
'
'        ' Get the IP of a mapped Hostname.
'        MessageBox.Show($"cuantodanio.es: {hosts.GetMappingFromHostname("cuantodanio.es").IP}")
'
'        ' Get all the hostname mappings.
'        For Each mappingInfo As HostsTools.MappingInfo In hosts.GetMappings()
'            MessageBox.Show(mappingInfo.ToString(), "HostsTools Mappings")
'        Next mappingInfo
'
'        ' Get all the hostname mappings that matches an IP address.
'        Dim mappingMatches As List(Of HostsTools.MappingInfo) = hosts.GetMappingsFromIP(hosts.Localhost)
'
'        ' Overwrite the whole Hosts file with the specified mappings.
'        hosts.WriteMappings(mappingMatches)
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
    ''' <para></para>
    ''' The parser understands the full HOSTS file syntax: tab or space separated fields,
    ''' several hostnames per line, inline comments ("# comment"), disabled (commented) mappings,
    ''' and both IPv4 and IPv6 addresses.
    ''' </summary>
    Public NotInheritable Class HostsTools

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="HostsTools"/> class.
        ''' </summary>
        ''' <param name="hostsLocation">
        ''' Optionally indicates a custom Hosts file location.
        ''' Default value is <see cref="DefaultHostsLocation"/> ('X:\Windows\System32\drivers\etc\hosts').
        ''' </param>
        Public Sub New(Optional ByVal hostsLocation As String = Nothing)

            Me.HostsLocation = If(String.IsNullOrEmpty(hostsLocation), HostsTools.DefaultHostsLocation, hostsLocation)

        End Sub

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the default location of the Windows Hosts file ('X:\Windows\System32\drivers\etc\hosts').
        ''' </summary>
        ''' <value>The default location of the Windows Hosts file.</value>
        Public Shared ReadOnly Property DefaultHostsLocation As String =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\etc\hosts")

        ''' <summary>
        ''' Gets the default Hosts file header, written by <see cref="FileCreate()"/> and <see cref="WriteMappings"/>.
        ''' </summary>
        ''' <value>The default Hosts file header.</value>
        Public Shared ReadOnly Property DefaultHeader As String =
            String.Join(Environment.NewLine,
                        "# Copyright (c) 1993-2009 Microsoft Corp.",
                        "#",
                        "# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.",
                        "#",
                        "# This file contains the mappings of IP addresses to host names. Each",
                        "# entry should be kept on an individual line. The IP address should",
                        "# be placed in the first column followed by the corresponding host name.",
                        "# The IP address and the host name should be separated by at least one",
                        "# space.",
                        String.Empty)

        ''' <summary>
        ''' Gets or sets the Hosts file location.
        ''' </summary>
        ''' <value>The Hosts file location.</value>
        ''' <exception cref="ArgumentException">The value is <see langword="Nothing"/>, empty or white-space.</exception>
        Public Property HostsLocation As String
            Get
                Return Me.hostsLocation1
            End Get
            Set(ByVal value As String)
                If String.IsNullOrWhiteSpace(value) Then
                    Throw New ArgumentException("The Hosts file location cannot be null, empty or white-space.", NameOf(value))
                End If
                Me.hostsLocation1 = value
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="HostsLocation"/> property. )
        ''' </summary>
        Private hostsLocation1 As String

        ''' <summary>
        ''' Gets or sets the Hosts file encoding.
        ''' The encoding must be <see cref="Encoding.Default"/> (ANSI) or <see cref="UTF8Encoding"/> (UTF-8 without BOM),
        ''' otherwise the entries will be ignored by Windows.
        ''' </summary>
        ''' <value>The Hosts file encoding.</value>
        ''' <exception cref="ArgumentNullException">The value is <see langword="Nothing"/>.</exception>
        Public Property HostsEncoding As Encoding
            Get
                Return Me.hostsEncoding1
            End Get
            Set(ByVal value As Encoding)
                If value Is Nothing Then
                    Throw New ArgumentNullException(NameOf(value))
                End If
                Me.hostsEncoding1 = value
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="HostsEncoding"/> property. )
        ''' </summary>
        Private hostsEncoding1 As Encoding = Encoding.Default

        ''' <summary>
        ''' Gets or sets the default 'LocalHost' IP address.
        ''' In most computers the default address is '127.0.0.1'.
        ''' </summary>
        ''' <value>The default LocalHost.</value>
        ''' <exception cref="FormatException">The value is not a valid IP address.</exception>
        Public Property Localhost As String
            Get
                Return Me.localhost1
            End Get
            Set(ByVal value As String)
                If Not MappingValidator.IsValidIPAddress(value) Then
                    Throw New FormatException($"Address: '{value}' is not a valid IP address.")
                End If
                Me.localhost1 = value
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="Localhost"/> property. )
        ''' </summary>
        Private localhost1 As String = "127.0.0.1"

#End Region

#Region " Types "

#Region " MappingInfo "

        ''' <summary>
        ''' Specifies info of a HOSTS file mapping.
        ''' </summary>
        Public NotInheritable Class MappingInfo

            ''' <summary>
            ''' Initializes a new instance of the <see cref="MappingInfo"/> class.
            ''' </summary>
            Public Sub New()
            End Sub

            ''' <summary>
            ''' Initializes a new instance of the <see cref="MappingInfo"/> class.
            ''' </summary>
            ''' <param name="ip">The IP address.</param>
            ''' <param name="hostName">The hostname.</param>
            ''' <param name="comment">The mapping comment.</param>
            ''' <param name="isDisabled">A value indicating whether the mapping is disabled.</param>
            Public Sub New(ByVal ip As String, ByVal hostName As String,
                           Optional ByVal comment As String = "",
                           Optional ByVal isDisabled As Boolean = False)

                Me.IP = ip
                Me.HostName = hostName
                Me.Comment = comment
                Me.IsDisabled = isDisabled

            End Sub

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
            ''' Gets or sets a value indicating whether the mapping is disabled (commented) in the HOSTS file.
            ''' </summary>
            ''' <value><see langword="True"/> if the mapping is disabled, <see langword="False"/> otherwise.</value>
            Public Property IsDisabled As Boolean

            ''' <summary>
            ''' Creates a new <see cref="MappingInfo"/> that is a copy of this instance.
            ''' </summary>
            ''' <returns>A copy of this instance.</returns>
            Public Function Clone() As MappingInfo

                Return New MappingInfo(Me.IP, Me.HostName, Me.Comment, Me.IsDisabled)

            End Function

            ''' <summary>
            ''' Determines whether this mapping has the same IP address and hostname than the specified mapping.
            ''' <para></para>
            ''' The hostname comparison is case-insensitive (DNS names are case-insensitive),
            ''' and the IP address comparison is semantic (e.g. "::1" equals "0:0:0:0:0:0:0:1").
            ''' </summary>
            ''' <param name="other">The other mapping.</param>
            ''' <returns><see langword="True"/> if both mappings target the same IP address and hostname; otherwise, <see langword="False"/>.</returns>
            ''' <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="Nothing"/>.</exception>
            Public Function IsSameTarget(ByVal other As MappingInfo) As Boolean

                If other Is Nothing Then
                    Throw New ArgumentNullException(NameOf(other))
                End If

                Return String.Equals(Me.HostName, other.HostName, StringComparison.OrdinalIgnoreCase) AndAlso
                       HostsTools.AreSameAddress(Me.IP, other.IP)

            End Function

            ''' <summary>
            ''' Determines whether this mapping conflicts with the specified mapping,
            ''' that is, both map the same hostname to an address of the same family (IPv4 or IPv6).
            ''' <para></para>
            ''' A hostname can be mapped at the same time to one IPv4 address and to one IPv6 address,
            ''' but two IPv4 (or two IPv6) mappings of the same hostname are ambiguous, because Windows only uses the first one.
            ''' </summary>
            ''' <param name="other">The other mapping.</param>
            ''' <returns><see langword="True"/> if both mappings conflict; otherwise, <see langword="False"/>.</returns>
            ''' <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="Nothing"/>.</exception>
            Public Function ConflictsWith(ByVal other As MappingInfo) As Boolean

                If other Is Nothing Then
                    Throw New ArgumentNullException(NameOf(other))
                End If

                If Not String.Equals(Me.HostName, other.HostName, StringComparison.OrdinalIgnoreCase) Then
                    Return False
                End If

                Dim addressA As IPAddress = Nothing
                Dim addressB As IPAddress = Nothing
                If IPAddress.TryParse(Me.IP, addressA) AndAlso IPAddress.TryParse(other.IP, addressB) Then
                    Return addressA.AddressFamily = addressB.AddressFamily
                End If

                ' Unparseable addresses: be conservative and consider them conflicting.
                Return True

            End Function

            ''' <summary>
            ''' Returns the HOSTS file line that represents this mapping.
            ''' </summary>
            ''' <returns>The HOSTS file line that represents this mapping.</returns>
            Public Overrides Function ToString() As String

                Return HostsTools.FormatMapping(Me)

            End Function

        End Class

#End Region

#Region " ParsedLine "

        ''' <summary>
        ''' Represents a parsed HOSTS file mapping line.
        ''' A single line can map several hostnames to the same IP address.
        ''' </summary>
        Private NotInheritable Class ParsedLine

            ''' <summary>
            ''' Gets or sets the IP address.
            ''' </summary>
            Friend Property IP As String

            ''' <summary>
            ''' Gets or sets the hostnames mapped in the line.
            ''' </summary>
            Friend Property HostNames As List(Of String)

            ''' <summary>
            ''' Gets or sets the inline comment (without the leading '#').
            ''' </summary>
            Friend Property Comment As String

            ''' <summary>
            ''' Gets or sets a value indicating whether the line is commented (disabled).
            ''' </summary>
            Friend Property IsDisabled As Boolean

            ''' <summary>
            ''' Determines whether this line maps the specified hostname.
            ''' </summary>
            ''' <param name="hostName">The hostname.</param>
            ''' <returns><see langword="True"/> if the line maps <paramref name="hostName"/>; otherwise, <see langword="False"/>.</returns>
            Friend Function ContainsHostName(ByVal hostName As String) As Boolean

                Return Me.HostNames.Exists(Function(host As String) host.Equals(hostName, StringComparison.OrdinalIgnoreCase))

            End Function

        End Class

#End Region

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Adds a new mapping at the end of the Hosts file.
        ''' </summary>
        ''' <param name="enabled">Indicates whether the mapping is enabled (<see langword="False"/> writes it commented).</param>
        ''' <param name="hostName">Indicates the Hostname.</param>
        ''' <param name="ip">Indicates the IP address.</param>
        ''' <param name="comment">Indicates a comment for this mapping.</param>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        ''' <exception cref="FormatException">Invalid IP address.</exception>
        ''' <exception cref="ArgumentException">Invalid hostname or comment.</exception>
        ''' <exception cref="InvalidOperationException">Hostname is already mapped.</exception>
        Public Sub Add(ByVal enabled As Boolean,
                       ByVal hostName As String,
                       ByVal ip As String,
                       Optional ByVal comment As String = Nothing)

            Me.EnsureFileExists()
            HostsTools.ValidateMappingValues(ip, hostName, comment)

            If Me.IsMapped(hostName) Then
                Throw New InvalidOperationException($"Hostname '{hostName}' is already mapped.")
            End If

            Dim content As String = File.ReadAllText(Me.hostsLocation1, Me.hostsEncoding1)
            Dim needsLeadingNewLine As Boolean = content.Length <> 0 AndAlso Not content.EndsWith(ControlChars.Lf, StringComparison.Ordinal)
            Dim line As String = HostsTools.FormatMapping(New MappingInfo(ip, hostName, comment, isDisabled:=Not enabled))

            File.AppendAllText(Me.hostsLocation1,
                               $"{If(needsLeadingNewLine, Environment.NewLine, String.Empty)}{line}{Environment.NewLine}",
                               Me.hostsEncoding1)

        End Sub

        ''' <summary>
        ''' Adds a new mapping at the end of the Hosts file.
        ''' </summary>
        ''' <param name="mappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="mappingInfo"/> is <see langword="Nothing"/>.</exception>
        Public Sub Add(ByVal mappingInfo As MappingInfo)

            If mappingInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappingInfo))
            End If

            Me.Add(Not mappingInfo.IsDisabled, mappingInfo.HostName, mappingInfo.IP, mappingInfo.Comment)

        End Sub

        ''' <summary>
        ''' Disables (comments) all the enabled mappings of the specified Hostname.
        ''' <para></para>
        ''' If the Hostname shares a line with other hostnames, the line is split
        ''' so that only the specified Hostname is disabled.
        ''' </summary>
        ''' <param name="hostName">Indicates the Hostname.</param>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        ''' <exception cref="InvalidOperationException">Hostname is not mapped, or it is already disabled.</exception>
        Public Sub Disable(ByVal hostName As String)

            Me.EnsureFileExists()
            HostsTools.EnsureHostNameNotNull(hostName)

            If Not Me.IsMapped(hostName) Then
                Throw New InvalidOperationException($"Hostname: '{hostName}' is not mapped.")
            End If

            If Me.IsDisabled(hostName) Then
                Throw New InvalidOperationException($"Hostname: '{hostName}' is already disabled.")
            End If

            Me.RewriteHostNameLines(hostName, disable:=True)

        End Sub

        ''' <summary>
        ''' Disables (comments) all the enabled mappings of the specified Hostname.
        ''' </summary>
        ''' <param name="mappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="mappingInfo"/> is <see langword="Nothing"/>.</exception>
        Public Sub Disable(ByVal mappingInfo As MappingInfo)

            If mappingInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappingInfo))
            End If

            Me.Disable(mappingInfo.HostName)

        End Sub

        ''' <summary>
        ''' Removes all the mappings (enabled and disabled) of the specified Hostname.
        ''' <para></para>
        ''' If the Hostname shares a line with other hostnames, only the specified Hostname is removed from that line.
        ''' </summary>
        ''' <param name="hostName">Indicates the Hostname.</param>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        ''' <exception cref="InvalidOperationException">Hostname is not mapped.</exception>
        Public Sub Remove(ByVal hostName As String)

            Me.EnsureFileExists()
            HostsTools.EnsureHostNameNotNull(hostName)

            If Not Me.IsMapped(hostName) Then
                Throw New InvalidOperationException($"Hostname: '{hostName}' is not mapped.")
            End If

            Me.RewriteHostNameLines(hostName, disable:=False)

        End Sub

        ''' <summary>
        ''' Removes all the mappings (enabled and disabled) of the specified Hostname.
        ''' </summary>
        ''' <param name="mappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="mappingInfo"/> is <see langword="Nothing"/>.</exception>
        Public Sub Remove(ByVal mappingInfo As MappingInfo)

            If mappingInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappingInfo))
            End If

            Me.Remove(mappingInfo.HostName)

        End Sub

        ''' <summary>
        ''' Gets a <see cref="List(Of MappingInfo)"/> containing the mapping info of all mappings, in file order.
        ''' <para></para>
        ''' A line that maps several hostnames produces one <see cref="MappingInfo"/> per hostname.
        ''' </summary>
        ''' <returns>The mappings of the Hosts file.</returns>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        Public Function GetMappings() As List(Of MappingInfo)

            Me.EnsureFileExists()

            Dim mappings As New List(Of MappingInfo)
            For Each line As String In File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1)
                mappings.AddRange(HostsTools.ParseLine(line))
            Next line

            Return mappings

        End Function

        ''' <summary>
        ''' Gets a <see cref="MappingInfo"/> instance containing the mapping info of a Hostname.
        ''' <para></para>
        ''' If the Hostname is mapped more than once, the first enabled mapping is returned;
        ''' if all its mappings are disabled, the first disabled mapping is returned.
        ''' </summary>
        ''' <param name="hostname">Indicates the Hostname.</param>
        ''' <returns>The mapping info of the Hostname.</returns>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        ''' <exception cref="InvalidOperationException">Hostname is not mapped.</exception>
        Public Function GetMappingFromHostname(ByVal hostname As String) As MappingInfo

            HostsTools.EnsureHostNameNotNull(hostname)

            Dim matches As List(Of MappingInfo) =
                Me.GetMappings().FindAll(Function(m As MappingInfo) m.HostName.Equals(hostname, StringComparison.OrdinalIgnoreCase))

            If matches.Count = 0 Then
                Throw New InvalidOperationException($"Hostname: '{hostname}' is not mapped.")
            End If

            Dim enabledMatch As MappingInfo = matches.Find(Function(m As MappingInfo) Not m.IsDisabled)
            Return If(enabledMatch, matches(0))

        End Function

        ''' <summary>
        ''' Gets a <see cref="List(Of MappingInfo)"/> containing the mapping info of all mappings
        ''' (enabled and disabled) matching the specified IP address.
        ''' <para></para>
        ''' The comparison is semantic, so for example "::1" matches "0:0:0:0:0:0:0:1".
        ''' </summary>
        ''' <param name="ip">Indicates the IP address.</param>
        ''' <returns>The mappings that match the IP address.</returns>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        ''' <exception cref="FormatException">Invalid IP address.</exception>
        Public Function GetMappingsFromIP(ByVal ip As String) As List(Of MappingInfo)

            Me.EnsureFileExists()

            If Not MappingValidator.IsValidIPAddress(ip) Then
                Throw New FormatException($"Address: '{ip}' is not a valid IP address.")
            End If

            Return Me.GetMappings().FindAll(Function(m As MappingInfo) HostsTools.AreSameAddress(m.IP, ip))

        End Function

        ''' <summary>
        ''' Checks whether a HostName is mapped (either enabled or disabled).
        ''' </summary>
        ''' <param name="hostName">Indicates the Hostname.</param>
        ''' <returns><see langword="True"/> if the specified Hostname is mapped; otherwise, <see langword="False"/>.</returns>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        ''' <exception cref="ArgumentNullException"><paramref name="hostName"/> is <see langword="Nothing"/>.</exception>
        Public Function IsMapped(ByVal hostName As String) As Boolean

            Me.EnsureFileExists()
            HostsTools.EnsureHostNameNotNull(hostName)

            For Each line As String In File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1)
                Dim parsed As ParsedLine = HostsTools.TryParseLine(line)
                If parsed IsNot Nothing AndAlso parsed.ContainsHostName(hostName) Then
                    Return True
                End If
            Next line

            Return False

        End Function

        ''' <summary>
        ''' Checks whether a HostName is mapped (either enabled or disabled).
        ''' </summary>
        ''' <param name="mappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        ''' <returns><see langword="True"/> if the specified Hostname is mapped; otherwise, <see langword="False"/>.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="mappingInfo"/> is <see langword="Nothing"/>.</exception>
        Public Function IsMapped(ByVal mappingInfo As MappingInfo) As Boolean

            If mappingInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappingInfo))
            End If

            Return Me.IsMapped(mappingInfo.HostName)

        End Function

        ''' <summary>
        ''' Checks whether a HostName is disabled, that is, it is mapped but none of its mappings is enabled.
        ''' </summary>
        ''' <param name="hostName">Indicates the Hostname.</param>
        ''' <returns><see langword="True"/> if the specified Hostname is disabled; otherwise, <see langword="False"/>.</returns>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        ''' <exception cref="InvalidOperationException">Hostname is not mapped.</exception>
        Public Function IsDisabled(ByVal hostName As String) As Boolean

            HostsTools.EnsureHostNameNotNull(hostName)

            Dim matches As List(Of MappingInfo) =
                Me.GetMappings().FindAll(Function(m As MappingInfo) m.HostName.Equals(hostName, StringComparison.OrdinalIgnoreCase))

            If matches.Count = 0 Then
                Throw New InvalidOperationException($"Hostname: '{hostName}' is not mapped.")
            End If

            Return matches.TrueForAll(Function(m As MappingInfo) m.IsDisabled)

        End Function

        ''' <summary>
        ''' Checks whether a HostName is disabled, that is, it is mapped but none of its mappings is enabled.
        ''' </summary>
        ''' <param name="mappingInfo">A <see cref="MappingInfo"/> instance containing the mapping info.</param>
        ''' <returns><see langword="True"/> if the specified Hostname is disabled; otherwise, <see langword="False"/>.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="mappingInfo"/> is <see langword="Nothing"/>.</exception>
        Public Function IsDisabled(ByVal mappingInfo As MappingInfo) As Boolean

            If mappingInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappingInfo))
            End If

            Return Me.IsDisabled(mappingInfo.HostName)

        End Function

        ''' <summary>
        ''' Overwrites the Hosts file with the <see cref="DefaultHeader"/> followed by the specified mappings.
        ''' <para></para>
        ''' All the mappings are validated before writing, so the file is not modified if any mapping is invalid.
        ''' The Hosts file is created if it does not exist.
        ''' </summary>
        ''' <param name="mappings">The mappings to write.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="mappings"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="ArgumentException">A mapping is <see langword="Nothing"/>, or it has an invalid hostname or comment.</exception>
        ''' <exception cref="FormatException">A mapping has an invalid IP address.</exception>
        Public Sub WriteMappings(ByVal mappings As IEnumerable(Of MappingInfo))

            If mappings Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappings))
            End If

            Dim sb As New StringBuilder(HostsTools.DefaultHeader)
            sb.AppendLine()

            Dim index As Integer = 0
            For Each mapping As MappingInfo In mappings
                If mapping Is Nothing Then
                    Throw New ArgumentException($"The mapping at index {index} is null.", NameOf(mappings))
                End If
                HostsTools.ValidateMappingValues(mapping.IP, mapping.HostName, mapping.Comment)
                sb.AppendLine(HostsTools.FormatMapping(mapping))
                index += 1
            Next mapping

            File.WriteAllText(Me.hostsLocation1, sb.ToString(), Me.hostsEncoding1)

        End Sub

        ''' <summary>
        ''' Checks whether the Hosts file exists.
        ''' </summary>
        ''' <returns><see langword="True"/> if Hosts file exists, <see langword="False"/> otherwise.</returns>
        Public Function FileExists() As Boolean

            Return File.Exists(Me.hostsLocation1)

        End Function

        ''' <summary>
        ''' Creates the Hosts file containing only the <see cref="DefaultHeader"/>.
        ''' If the file already exists, it is overwritten.
        ''' </summary>
        Public Sub FileCreate()

            File.WriteAllText(Me.hostsLocation1, HostsTools.DefaultHeader, Me.hostsEncoding1)

        End Sub

        ''' <summary>
        ''' Deletes the Hosts file.
        ''' </summary>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        Public Sub FileDelete()

            Me.EnsureFileExists()
            File.Delete(Me.hostsLocation1)

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
        ''' <param name="process">
        ''' Indicates the process location, or the name of an executable that can be found in the PATH.
        ''' Default value is: "notepad.exe".
        ''' </param>
        ''' <exception cref="FileNotFoundException">Hosts file not found, or the process (a rooted path) not found.</exception>
        ''' <exception cref="ArgumentException"><paramref name="process"/> is <see langword="Nothing"/>, empty or white-space.</exception>
        Public Sub FileOpen(Optional ByVal process As String = "notepad.exe")

            Me.EnsureFileExists()

            If String.IsNullOrWhiteSpace(process) Then
                Throw New ArgumentException("The process cannot be null, empty or white-space.", NameOf(process))
            End If

            If Path.IsPathRooted(process) AndAlso Not File.Exists(process) Then
                Throw New FileNotFoundException("Process not found.", process)
            End If

            Dim psi As New Diagnostics.ProcessStartInfo() With {
                .FileName = process,
                .Arguments = $"""{Me.hostsLocation1}""",
                .UseShellExecute = True
            }

            Using p As Diagnostics.Process = Diagnostics.Process.Start(psi)
            End Using
        End Sub

#End Region

#Region " Public Shared Methods "

        ''' <summary>
        ''' Parses a single Hosts file line.
        ''' </summary>
        ''' <param name="line">The Hosts file line.</param>
        ''' <returns>
        ''' One <see cref="MappingInfo"/> per hostname mapped in the line,
        ''' or an empty list if the line is not a mapping (blank line, comment, malformed line...).
        ''' </returns>
        ''' <exception cref="ArgumentNullException"><paramref name="line"/> is <see langword="Nothing"/>.</exception>
        Public Shared Function ParseLine(ByVal line As String) As List(Of MappingInfo)

            If line Is Nothing Then
                Throw New ArgumentNullException(NameOf(line))
            End If

            Dim result As New List(Of MappingInfo)
            Dim parsed As ParsedLine = HostsTools.TryParseLine(line)
            If parsed IsNot Nothing Then
                For Each host As String In parsed.HostNames
                    result.Add(New MappingInfo(parsed.IP, host, parsed.Comment, parsed.IsDisabled))
                Next host
            End If

            Return result

        End Function

        ''' <summary>
        ''' Formats the specified mapping as a Hosts file line ("[# ]IP&lt;TAB&gt;HostName[&lt;TAB&gt;# Comment]").
        ''' </summary>
        ''' <param name="mappingInfo">The mapping to format.</param>
        ''' <returns>The Hosts file line.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="mappingInfo"/> is <see langword="Nothing"/>.</exception>
        Public Shared Function FormatMapping(ByVal mappingInfo As MappingInfo) As String

            If mappingInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappingInfo))
            End If

            Return HostsTools.FormatLine(mappingInfo.IsDisabled, mappingInfo.IP, {mappingInfo.HostName}, mappingInfo.Comment)

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Throws a <see cref="FileNotFoundException"/> if the Hosts file does not exist.
        ''' </summary>
        ''' <exception cref="FileNotFoundException">Hosts file not found.</exception>
        Private Sub EnsureFileExists()

            If Not Me.FileExists() Then
                Throw New FileNotFoundException("Hosts file not found.", Me.hostsLocation1)
            End If

        End Sub

        ''' <summary>
        ''' Throws an <see cref="ArgumentNullException"/> if the specified hostname is <see langword="Nothing"/>.
        ''' </summary>
        ''' <param name="hostName">The hostname.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="hostName"/> is <see langword="Nothing"/>.</exception>
        Private Shared Sub EnsureHostNameNotNull(ByVal hostName As String)

            If hostName Is Nothing Then
                Throw New ArgumentNullException(NameOf(hostName))
            End If

        End Sub

        ''' <summary>
        ''' Validates the values of a mapping.
        ''' </summary>
        ''' <param name="ip">The IP address.</param>
        ''' <param name="hostName">The hostname.</param>
        ''' <param name="comment">The comment.</param>
        ''' <exception cref="FormatException">Invalid IP address.</exception>
        ''' <exception cref="ArgumentException">Invalid hostname or comment.</exception>
        Private Shared Sub ValidateMappingValues(ByVal ip As String, ByVal hostName As String, ByVal comment As String)

            If Not MappingValidator.IsValidIPAddress(ip) Then
                Throw New FormatException($"Address: '{ip}' is not a valid IP address.")
            End If

            If Not MappingValidator.IsValidHostName(hostName) Then
                Throw New ArgumentException($"Hostname: '{hostName}' is not a valid hostname.", NameOf(hostName))
            End If

            If Not MappingValidator.IsValidComment(comment) Then
                Throw New ArgumentException("The comment cannot contain line breaks or control characters.", NameOf(comment))
            End If

        End Sub

        ''' <summary>
        ''' Determines whether two IP address strings represent the same address.
        ''' </summary>
        ''' <param name="ipA">The first IP address.</param>
        ''' <param name="ipB">The second IP address.</param>
        ''' <returns><see langword="True"/> if both represent the same address; otherwise, <see langword="False"/>.</returns>
        Private Shared Function AreSameAddress(ByVal ipA As String, ByVal ipB As String) As Boolean

            Dim addressA As IPAddress = Nothing
            Dim addressB As IPAddress = Nothing

            If IPAddress.TryParse(ipA, addressA) AndAlso IPAddress.TryParse(ipB, addressB) Then
                Return addressA.Equals(addressB)
            End If

            Return String.Equals(ipA, ipB, StringComparison.OrdinalIgnoreCase)

        End Function

        ''' <summary>
        ''' Formats a Hosts file line.
        ''' </summary>
        ''' <param name="isDisabled">A value indicating whether the line is commented.</param>
        ''' <param name="ip">The IP address.</param>
        ''' <param name="hostNames">The hostnames.</param>
        ''' <param name="comment">The comment.</param>
        ''' <returns>The Hosts file line.</returns>
        Private Shared Function FormatLine(ByVal isDisabled As Boolean,
                                           ByVal ip As String,
                                           ByVal hostNames As IEnumerable(Of String),
                                           ByVal comment As String) As String

            Dim prefix As String = If(isDisabled, "# ", String.Empty)
            Dim suffix As String = If(String.IsNullOrWhiteSpace(comment), String.Empty, $"{ControlChars.Tab}# {comment.Trim()}")

            Return $"{prefix}{ip}{ControlChars.Tab}{String.Join(" ", hostNames)}{suffix}"

        End Function

        ''' <summary>
        ''' Tries to parse a Hosts file line.
        ''' </summary>
        ''' <param name="line">The line to parse.</param>
        ''' <returns>The parsed line, or <see langword="Nothing"/> if the line is not a mapping.</returns>
        Private Shared Function TryParseLine(ByVal line As String) As ParsedLine

            Dim text As String = line.Trim()
            If text.Length = 0 Then
                Return Nothing
            End If

            ' Disabled (commented) mapping.
            Dim isDisabled As Boolean = False
            If text(0) = "#"c Then
                isDisabled = True
                text = text.TrimStart("#"c).Trim()
                If text.Length = 0 Then
                    Return Nothing
                End If
            End If

            ' Inline comment.
            Dim comment As String = String.Empty
            Dim commentIndex As Integer = text.IndexOf("#"c)
            If commentIndex >= 0 Then
                comment = text.Substring(commentIndex + 1).TrimStart("#"c).Trim()
                text = text.Substring(0, commentIndex).Trim()
            End If

            ' Fields (split by any white-space).
            Dim fields As String() = text.Split(New Char() {}, StringSplitOptions.RemoveEmptyEntries)
            If fields.Length < 2 OrElse Not MappingValidator.IsValidIPAddress(fields(0)) Then
                Return Nothing
            End If

            Dim hostNames As New List(Of String)
            For i As Integer = 1 To fields.Length - 1
                If Not MappingValidator.IsValidHostName(fields(i)) Then
                    Return Nothing
                End If
                hostNames.Add(fields(i))
            Next i

            Return New ParsedLine With {
                .IP = fields(0),
                .HostNames = hostNames,
                .Comment = comment,
                .IsDisabled = isDisabled
            }

        End Function

        ''' <summary>
        ''' Rewrites the Hosts file lines that map the specified hostname, either removing or disabling the hostname.
        ''' Lines that do not map the hostname are preserved untouched.
        ''' </summary>
        ''' <param name="hostName">The hostname.</param>
        ''' <param name="disable">
        ''' <see langword="True"/> to disable the enabled mappings of the hostname;
        ''' <see langword="False"/> to remove all the mappings of the hostname.
        ''' </param>
        Private Sub RewriteHostNameLines(ByVal hostName As String, ByVal disable As Boolean)

            Dim result As New List(Of String)

            For Each line As String In File.ReadAllLines(Me.hostsLocation1, Me.hostsEncoding1)

                Dim parsed As ParsedLine = HostsTools.TryParseLine(line)

                If parsed Is Nothing OrElse Not parsed.ContainsHostName(hostName) OrElse (disable AndAlso parsed.IsDisabled) Then
                    result.Add(line)
                    Continue For
                End If

                Dim targets As List(Of String) =
                    parsed.HostNames.FindAll(Function(host As String) host.Equals(hostName, StringComparison.OrdinalIgnoreCase))
                Dim remaining As List(Of String) =
                    parsed.HostNames.FindAll(Function(host As String) Not host.Equals(hostName, StringComparison.OrdinalIgnoreCase))

                If remaining.Count <> 0 Then
                    result.Add(HostsTools.FormatLine(parsed.IsDisabled, parsed.IP, remaining, parsed.Comment))
                End If

                If disable Then
                    result.Add(HostsTools.FormatLine(True, parsed.IP, {targets(0)}, parsed.Comment))
                End If

            Next line

            File.WriteAllLines(Me.hostsLocation1, result, Me.hostsEncoding1)

        End Sub

#End Region

    End Class

End Namespace

#End Region
