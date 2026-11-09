' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ThemeNativeMethods.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices
Imports System.Security

#End Region

#Region " ThemeNativeMethods "

Namespace UserInterface.Theming

    ''' <summary>
    ''' Platform Invocation (P/Invoke) methods used by the theming system.
    ''' </summary>
    <SuppressUnmanagedCodeSecurity>
    Friend NotInheritable Class ThemeNativeMethods

#Region " Constants "

        ''' <summary>
        ''' DWMWA_USE_IMMERSIVE_DARK_MODE attribute of <see cref="DwmSetWindowAttribute"/>
        ''' (Windows 10 version 20H1 (build 19041) and later, including Windows 11).
        ''' </summary>
        Friend Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

        ''' <summary>
        ''' Undocumented DWMWA_USE_IMMERSIVE_DARK_MODE attribute value of <see cref="DwmSetWindowAttribute"/>
        ''' used by Windows 10 versions 1809 (build 17763) to 1909 (build 18363).
        ''' </summary>
        Friend Const DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 As Integer = 19

        ''' <summary>
        ''' LVM_GETHEADER message: gets the handle to the header control of a list-view control.
        ''' </summary>
        Friend Const LVM_GETHEADER As Integer = &H101F

        ''' <summary>
        ''' WM_NCACTIVATE message: sent to a window when its non-client area needs to be changed
        ''' to indicate an active or inactive state.
        ''' </summary>
        Friend Const WM_NCACTIVATE As Integer = &H86

        ''' <summary>
        ''' SetWindowPos flags used to force a non-client area (frame) repaint without moving, resizing,
        ''' re-ordering or activating the window (SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED).
        ''' </summary>
        Friend Const SWP_FRAMECHANGED_ONLY As UInteger = &H1UI Or &H2UI Or &H4UI Or &H10UI Or &H20UI

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="ThemeNativeMethods"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Functions "

        ''' <summary>
        ''' Sets the value of Desktop Window Manager (DWM) non-client rendering attributes for a window.
        ''' </summary>
        ''' <param name="hwnd">The handle to the window.</param>
        ''' <param name="attribute">The attribute to set.</param>
        ''' <param name="value">The attribute value.</param>
        ''' <param name="size">The size, in bytes, of the attribute value.</param>
        ''' <returns>S_OK (zero) if succeeds; otherwise, an HRESULT error code.</returns>
        <DllImport("dwmapi.dll", SetLastError:=False, ExactSpelling:=True)>
        Friend Shared Function DwmSetWindowAttribute(ByVal hwnd As IntPtr,
                                                     ByVal attribute As Integer,
                                                     ByRef value As Integer,
                                                     ByVal size As Integer) As Integer
        End Function

        ''' <summary>
        ''' Causes a window to use a different set of visual style information than its class normally uses.
        ''' </summary>
        ''' <param name="hwnd">The handle to the window.</param>
        ''' <param name="subAppName">The application name, or <see langword="Nothing"/> to restore the default theme.</param>
        ''' <param name="subIdList">The semicolon-separated list of CLSID names, or <see langword="Nothing"/>.</param>
        ''' <returns>S_OK (zero) if succeeds; otherwise, an HRESULT error code.</returns>
        <DllImport("uxtheme.dll", SetLastError:=False, ExactSpelling:=True, CharSet:=CharSet.Unicode)>
        Friend Shared Function SetWindowTheme(ByVal hwnd As IntPtr,
                                              ByVal subAppName As String,
                                              ByVal subIdList As String) As Integer
        End Function

        ''' <summary>
        ''' Sends the specified message to a window.
        ''' </summary>
        ''' <param name="hwnd">The handle to the window.</param>
        ''' <param name="msg">The message.</param>
        ''' <param name="wParam">Additional message-specific information.</param>
        ''' <param name="lParam">Additional message-specific information.</param>
        ''' <returns>The result of the message processing.</returns>
        <DllImport("user32.dll", SetLastError:=False, EntryPoint:="SendMessageW", ExactSpelling:=True)>
        Friend Shared Function SendMessage(ByVal hwnd As IntPtr,
                                           ByVal msg As Integer,
                                           ByVal wParam As IntPtr,
                                           ByVal lParam As IntPtr) As IntPtr
        End Function

        ''' <summary>
        ''' Changes the size, position, and Z order of a window.
        ''' </summary>
        ''' <param name="hwnd">The handle to the window.</param>
        ''' <param name="hwndInsertAfter">The handle to the window to precede the positioned window in the Z order.</param>
        ''' <param name="x">The new position of the left side of the window.</param>
        ''' <param name="y">The new position of the top of the window.</param>
        ''' <param name="cx">The new width of the window.</param>
        ''' <param name="cy">The new height of the window.</param>
        ''' <param name="flags">The window sizing and positioning flags.</param>
        ''' <returns><see langword="True"/> if succeeds; otherwise, <see langword="False"/>.</returns>
        <DllImport("user32.dll", SetLastError:=True, ExactSpelling:=True)>
        Friend Shared Function SetWindowPos(ByVal hwnd As IntPtr,
                                            ByVal hwndInsertAfter As IntPtr,
                                            ByVal x As Integer,
                                            ByVal y As Integer,
                                            ByVal cx As Integer,
                                            ByVal cy As Integer,
                                            ByVal flags As UInteger) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Destroys an icon and frees any memory the icon occupied.
        ''' </summary>
        ''' <param name="hIcon">The handle to the icon to be destroyed.</param>
        ''' <returns><see langword="True"/> if succeeds; otherwise, <see langword="False"/>.</returns>
        <DllImport("user32.dll", SetLastError:=True, ExactSpelling:=True)>
        Friend Shared Function DestroyIcon(ByVal hIcon As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Retrieves information about the specified combo box (including the handle of its drop-down list).
        ''' </summary>
        ''' <param name="hwndCombo">The handle to the combo box.</param>
        ''' <param name="info">A <see cref="COMBOBOXINFO"/> structure that receives the information.
        ''' Its <see cref="COMBOBOXINFO.cbSize"/> member must be initialized before calling this function.</param>
        ''' <returns><see langword="True"/> if succeeds; otherwise, <see langword="False"/>.</returns>
        <DllImport("user32.dll", SetLastError:=True, ExactSpelling:=True)>
        Friend Shared Function GetComboBoxInfo(ByVal hwndCombo As IntPtr,
                                               ByRef info As COMBOBOXINFO) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

#End Region

#Region " Structures "

        ''' <summary>
        ''' Defines a rectangle by the coordinates of its upper-left and lower-right corners.
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Friend Structure RECT

            ''' <summary>The x-coordinate of the upper-left corner.</summary>
            Friend Left As Integer

            ''' <summary>The y-coordinate of the upper-left corner.</summary>
            Friend Top As Integer

            ''' <summary>The x-coordinate of the lower-right corner.</summary>
            Friend Right As Integer

            ''' <summary>The y-coordinate of the lower-right corner.</summary>
            Friend Bottom As Integer

        End Structure

        ''' <summary>
        ''' Contains combo box status information.
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Friend Structure COMBOBOXINFO

            ''' <summary>The size, in bytes, of the structure.</summary>
            Friend cbSize As Integer

            ''' <summary>The coordinates of the edit box.</summary>
            Friend rcItem As RECT

            ''' <summary>The coordinates of the drop-down button.</summary>
            Friend rcButton As RECT

            ''' <summary>The state of the drop-down button.</summary>
            Friend stateButton As Integer

            ''' <summary>The handle to the combo box.</summary>
            Friend hwndCombo As IntPtr

            ''' <summary>The handle to the edit box.</summary>
            Friend hwndItem As IntPtr

            ''' <summary>The handle to the drop-down list.</summary>
            Friend hwndList As IntPtr

        End Structure

#End Region

    End Class

End Namespace

#End Region
