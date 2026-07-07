' ***********************************************************************
' Author   : Elektro
' Modified : 07-December-2014
' ***********************************************************************
' <copyright file="CenteredMessageBox.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Usage Examples "

'Using New CenteredMessageBox(ownerForm:=Me,
'                             textFont:=New Font("Lucida Console", Font.SizeInPoints, FontStyle.Italic),
'                             timeOut:=2500)
'
'    MessageBox.Show("Text", "Title", MessageBoxButtons.OK, MessageBoxIcon.Information)
'
'End Using

#End Region

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms
Imports System.ComponentModel

#End Region

#Region " Centered MessageBox "

Namespace Tools

    ''' <summary>
    ''' A customized <see cref="MessageBox"/>. 
    ''' This class cannot be inherited.
    ''' </summary>
    Public NotInheritable Class CenteredMessageBox : Implements IDisposable

#Region " Properties "

        ''' <summary>
        ''' Gets the messagebox main window handle (hwnd).
        ''' </summary>
        ''' <value>The messagebox main window handle (hwnd).</value>
        Friend ReadOnly Property MessageBoxWindowHandle As IntPtr
            Get
                Return Me.messageBoxWindowHandle1
            End Get
        End Property
        ''' <summary>
        ''' The messagebox main window handle (hwnd).
        ''' </summary>
        Private messageBoxWindowHandle1 As IntPtr

        ''' <summary>
        ''' Gets the owner <see cref="Form"/> to center the <see cref="CenteredMessageBox"/>.
        ''' </summary>
        ''' <value>The owner <see cref="Form"/> to center the <see cref="CenteredMessageBox"/>.</value>
        Friend ReadOnly Property OwnerForm As Form
            Get
                Return Me.ownerForm1
            End Get
        End Property
        ''' <summary>
        ''' The owner <see cref="Form"/> to center the <see cref="CenteredMessageBox"/>
        ''' </summary>
        Private ownerForm1 As Form

        ''' <summary>
        ''' Gets the <see cref="Font"/> used to display the <see cref="CenteredMessageBox"/> text.
        ''' </summary>
        ''' <value>The <see cref="Font"/> used to display the <see cref="CenteredMessageBox"/> text.</value>
        Friend ReadOnly Property Font As Font
            Get
                Return Me.font1
            End Get
        End Property
        ''' <summary>
        ''' The <see cref="Font"/> used to display the <see cref="CenteredMessageBox"/> text.
        ''' </summary>
        Private ReadOnly font1 As Font

        ''' <summary>
        ''' Gets the time interval to auto-close this <see cref="CenteredMessageBox"/>, in milliseconds.
        ''' Default value is '0', which means Infinite.
        ''' </summary>
        Friend ReadOnly Property TimeOut As Integer
            Get
                Return Me.timeOut1
            End Get
        End Property
        ''' <summary>
        ''' The time interval to auto-close this <see cref="CenteredMessageBox"/>, in milliseconds.
        ''' Default value is '0', which means Infinite.
        ''' </summary>
        Private ReadOnly timeOut1 As Integer = 0

#End Region

#Region " Objects "

        ''' <summary>
        ''' A <see cref="Windows.Forms.Timer"/> that keeps track of <see cref="TimeOut"/> value to close this <see cref="CenteredMessageBox"/>.
        ''' </summary>
        Private WithEvents timeoutTimer As Timer

        ''' <summary>
        ''' Keeps track of the current amount of tries to find this <see cref="CenteredMessageBox"/> dialog.
        ''' </summary>
        Private tries As Integer

#End Region

#Region " P/Invoke "

        ''' <summary>
        ''' Platform Invocation methods (P/Invoke), access unmanaged code.
        ''' This class does not suppress stack walks for unmanaged code permission.
        ''' <see cref="System.Security.SuppressUnmanagedCodeSecurityAttribute"/>  must not be applied to this class.
        ''' This class is for methods that can be used anywhere because a stack walk will be performed.
        ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/ms182161.aspx
        ''' </summary>
        Protected NotInheritable Class NativeMethods

#Region " Functions "

            ''' <summary>
            ''' Retrieves the thread identifier of the calling thread.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms683183%28v=vs.85%29.aspx
            ''' </summary>
            ''' <returns>The thread identifier of the calling thread.</returns>
            <DllImport("kernel32.dll", SetLastError:=False)>
            Protected Friend Shared Function GetCurrentThreadId() As Integer
            End Function

            ''' <summary>
            ''' Enumerates all nonchild windows associated with a thread by passing the handle to each window, 
            ''' in turn, to an application-defined callback function. 
            ''' <see cref="EnumThreadWindows"/> continues until the last window is enumerated or the callback function returns <c>false</c>.
            ''' To enumerate child windows of a particular window, use the EnumChildWindows function.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633495%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="dwThreadId">The identifier of the thread whose windows are to be enumerated.</param>
            ''' <param name="lpfn">A pointer to an application-defined callback function.</param>
            ''' <param name="lParam">An application-defined value to be passed to the callback function.</param>
            ''' <returns>
            ''' <c>true</c> if the callback function returns <c>true</c> for all windows in the thread specified by dwThreadId parameter. 
            ''' <c>false</c> if the callback function returns <c>false</c> on any enumerated window, 
            ''' or if there are no windows found in the thread specified by dwThreadId parameter.</returns>
            <DllImport("user32.dll", SetLastError:=False)>
            Protected Friend Shared Function EnumThreadWindows(
                      ByVal dwThreadId As Integer,
                      ByVal lpfn As NativeMethods.EnumThreadWndProc,
                      ByVal lParam As IntPtr
            ) As <MarshalAs(UnmanagedType.Bool)> Boolean
            End Function

            ''' <summary>
            ''' Retrieves the name of the class to which the specified window belongs.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633582%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
            ''' <param name="buffer">The class name string.</param>
            ''' <param name="buflen">
            ''' The length of the lpClassName buffer, in characters. 
            ''' The buffer must be large enough to include the terminating null character; 
            ''' otherwise, the class name string is truncated to nMaxCount-1 characters.
            ''' </param>
            ''' <returns>
            ''' If the function succeeds, the return value is the number of characters copied to the buffer, 
            ''' not including the terminating null character.
            ''' If the function fails, the return value is 0.
            ''' </returns>
            <DllImport("user32.dll", SetLastError:=False, CharSet:=CharSet.Unicode)>
            Protected Friend Shared Function GetClassName(
                      ByVal hWnd As IntPtr,
                      ByVal buffer As StringBuilder,
                      ByVal buflen As Integer
            ) As Integer
            End Function

            ''' <summary>
            ''' Retrieves a handle to a control in the specified dialog box.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms645481%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="hWnd">A handle to the dialog box that contains the control.</param>
            ''' <param name="item">The identifier of the control to be retrieved.</param>
            ''' <returns>
            ''' If the function succeeds, the return value is the window handle of the specified control.
            ''' If the function fails, the return value is <see cref="IntPtr.Zero"/>, 
            ''' indicating an invalid dialog box handle or a nonexistent control
            ''' </returns>
            <DllImport("user32.dll", SetLastError:=False)>
            Protected Friend Shared Function GetDlgItem(
                      ByVal hWnd As IntPtr,
                      ByVal item As Integer
            ) As IntPtr
            End Function

            ''' <summary>
            ''' Retrieves the dimensions of the bounding rectangle of the specified window. 
            ''' The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633519%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="hWnd">A handle to the window.</param>
            ''' <param name="rc">
            ''' A pointer to a <see cref="RECT"/> structure that receives the screen coordinates of 
            ''' the upper-left and lower-right corners of the window.
            ''' </param>
            ''' <returns><c>true</c> if the function succeeds, <c>false</c> otherwise.</returns>
            <DllImport("user32.dll", SetLastError:=False)>
            Protected Friend Shared Function GetWindowRect(
                      ByVal hWnd As IntPtr,
                      ByRef rc As Rect
            ) As <MarshalAs(UnmanagedType.Bool)> Boolean
            End Function

            ''' <summary>
            ''' Destroys the specified window. 
            ''' The function sends WM_DESTROY and WM_NCDESTROY messages to the window to deactivate it and remove the keyboard focus from it. 
            ''' The function also destroys the window's menu, flushes the thread message queue, destroys timers, removes clipboard ownership,
            ''' and breaks the clipboard viewer chain (if the window is at the top of the viewer chain).
            ''' If the specified window is a parent or owner window, 
            ''' DestroyWindow automatically destroys the associated child or owned windows when it destroys the parent or owner window. 
            ''' The function first destroys child or owned windows, and then it destroys the parent or owner window.
            ''' DestroyWindow also destroys modeless dialog boxes created by the CreateDialog function.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632682%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="hwnd">Handle to the window to be destroyed.</param>
            ''' <returns><c>true</c> if the function succeeds, <c>false</c> otherwise.</returns>
            <DllImport("user32.dll", SetLastError:=False)>
            Protected Friend Shared Function DestroyWindow(
                      ByVal hwnd As IntPtr
            ) As <MarshalAs(UnmanagedType.Bool)> Boolean
            End Function

            ''' <summary>
            ''' Changes the position and dimensions of the specified window. 
            ''' For a top-level window, the position and dimensions are relative to the upper-left corner of the screen. 
            ''' For a child window, they are relative to the upper-left corner of the parent window's client area.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633534%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="hWnd">A handle to the window.</param>
            ''' <param name="x">The new position of the left side of the window.</param>
            ''' <param name="y">The new position of the top of the window.</param>
            ''' <param name="width">The new width of the window.</param>
            ''' <param name="height">The new height of the window.</param>
            ''' <param name="repaint">
            ''' Indicates whether the window is to be repainted. 
            ''' If this parameter is TRUE, the window receives a message. 
            ''' If the parameter is FALSE, no repainting of any kind occurs. 
            ''' This applies to the client area, the nonclient area (including the title bar and scroll bars), 
            ''' and any part of the parent window uncovered as a result of moving a child window.
            ''' </param>
            ''' <returns><c>true</c> if the function succeeds, <c>false</c> otherwise.</returns>
            <DllImport("user32.dll", SetLastError:=False)>
            Protected Friend Shared Function MoveWindow(
                      ByVal hWnd As IntPtr,
                      ByVal x As Integer,
                      ByVal y As Integer,
                      ByVal width As Integer,
                      ByVal height As Integer,
                      ByVal repaint As Boolean
            ) As <MarshalAs(UnmanagedType.Bool)> Boolean
            End Function

            ''' <summary>
            ''' Changes the size, position, and Z order of a child, pop-up, or top-level window. 
            ''' These windows are ordered according to their appearance on the screen. 
            ''' The topmost window receives the highest rank and is the first window in the Z order.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633545%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="hWnd">A handle to the window.</param>
            ''' <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order.</param>
            ''' <param name="x">The new position of the left side of the window, in client coordinates.</param>
            ''' <param name="y">The new position of the top of the window, in client coordinates.</param>
            ''' <param name="cx">The new width of the window, in pixels.</param>
            ''' <param name="cy">The new height of the window, in pixels.</param>
            ''' <param name="uFlags">The window sizing and positioning flags.</param>
            ''' <returns><c>true</c> if the function succeeds, <c>false</c> otherwise.</returns>
            <DllImport("user32.dll", SetLastError:=True)> _
            Protected Friend Shared Function SetWindowPos(
                      ByVal hWnd As IntPtr,
                      ByVal hWndInsertAfter As IntPtr,
                      ByVal x As Integer,
                      ByVal y As Integer,
                      ByVal cx As Integer,
                      ByVal cy As Integer,
                      ByVal uFlags As SetWindowPosFlags
            ) As <MarshalAs(UnmanagedType.Bool)> Boolean
            End Function

            ''' <summary>
            ''' Sends the specified message to a window or windows. 
            ''' The <see cref="SendMessage"/> function calls the window procedure for the specified window and 
            ''' does not return until the window procedure has processed the message.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms644950%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="hWnd">A handle to the window whose window procedure will receive the message.</param>
            ''' <param name="msg">The windows message to be sent.</param>
            ''' <param name="wParam">Additional message-specific information.</param>
            ''' <param name="lParam">Additional message-specific information.</param>
            ''' <returns>The result of the message processing; it depends on the message sent.</returns>
            <DllImport("user32.dll", SetLastError:=False)>
            Protected Friend Shared Function SendMessage(
                      ByVal hWnd As IntPtr,
                      ByVal msg As WindowsMessages,
                      ByVal wParam As IntPtr,
                      ByVal lParam As IntPtr
            ) As IntPtr
            End Function

#End Region

#Region " Callbacks "

            ''' <summary>
            ''' An application-defined callback function used with the <see cref="EnumThreadWindows"/> function. 
            ''' It receives the window handles associated with a thread. 
            ''' The WNDENUMPROC type defines a pointer to this callback function. 
            ''' <see cref="EnumThreadWndProc"/> is a placeholder for the application-defined function name
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633496%28v=vs.85%29.aspx
            ''' </summary>
            ''' <param name="hWnd">A handle to a window associated with the thread specified in the <see cref="EnumThreadWindows"/> function.</param>
            ''' <param name="lParam">The application-defined value given in the <see cref="EnumThreadWindows"/> function.</param>
            ''' <returns>
            ''' To continue enumeration, the callback function must return <c>true</c>; 
            ''' To stop enumeration, it must return <c>false</c>.
            ''' </returns>
            Protected Friend Delegate Function EnumThreadWndProc(
                      ByVal hWnd As IntPtr,
                      ByVal lParam As IntPtr
            ) As Boolean

#End Region

#Region " Enumerations "

            ''' <summary>
            ''' Specifies a System-Defined Message.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms644927%28v=vs.85%29.aspx#system_defined
            ''' </summary>
            <Description("Enum used for 'SendMessage' function.")>
            Protected Friend Enum WindowsMessages As Integer

                ' **************************************
                ' NOTE:
                ' This enumeration is partially defined.
                ' **************************************

                ''' <summary>
                ''' Sets the font that a control is to use when drawing text.
                ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632642%28v=vs.85%29.aspx
                ''' </summary>
                WM_SETFONT = &H30

                ''' <summary>
                ''' Retrieves the font with which the control is currently drawing its text.
                ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632624%28v=vs.85%29.aspx
                ''' </summary>
                WM_GETFONT = &H31

            End Enum

            ''' <summary>
            ''' Specifies the window sizing and positioning flags.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633545%28v=vs.85%29.aspx
            ''' </summary>
            <FlagsAttribute>
            <Description("Enum used for 'SetWindowPos' function.")>
            Protected Friend Enum SetWindowPosFlags As UInteger

                ' **************************************
                ' NOTE:
                ' This enumeration is partially defined.
                ' **************************************

                ''' <summary>
                ''' Indicates any flag.
                ''' </summary>
                None = &H0UI

            End Enum

#End Region

#Region " Structures "

            ''' <summary>
            ''' Defines the coordinates of the upper-left and lower-right corners of a rectangle.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/dd162897%28v=vs.85%29.aspx
            ''' </summary>
            <Description("Structure used for 'GetWindowRect' function.")>
            Protected Friend Structure Rect

                ''' <summary>
                ''' The x-coordinate of the upper-left corner of the rectangle.
                ''' </summary>
                Friend Left As Integer

                ''' <summary>
                ''' The y-coordinate of the upper-left corner of the rectangle.
                ''' </summary>
                Friend Top As Integer

                ''' <summary>
                ''' The x-coordinate of the lower-right corner of the rectangle.
                ''' </summary>
                Friend Right As Integer

                ''' <summary>
                ''' The y-coordinate of the lower-right corner of the rectangle.
                ''' </summary>
                Friend Bottom As Integer

            End Structure

#End Region

        End Class

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="CenteredMessageBox"/> class.
        ''' </summary>
        ''' <param name="ownerForm">The form that owns this <see cref="CenteredMessageBox"/>.</param>
        ''' <param name="TextFont">The <see cref="Font"/> used to display the text of this <see cref="CenteredMessageBox"/>.</param>
        ''' <param name="TimeOut">
        ''' The time interval to auto-close this <see cref="CenteredMessageBox"/>, in milliseconds;
        ''' Default value is '0', which means Infinite.
        ''' </param>
        Public Sub New(ByVal ownerForm As Form,
                       Optional textFont As Font = Nothing,
                       Optional timeOut As Integer = 0I)

            Me.ownerForm1 = ownerForm
            Me.font1 = textFont
            Me.timeOut1 = timeOut
            Me.ownerForm1.BeginInvoke(New MethodInvoker(AddressOf Me.FindDialog))

        End Sub

        ''' <summary>
        ''' Prevents a default instance of the <see cref="CenteredMessageBox"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Finds the <see cref="CenteredMessageBox"/> dialog window.
        ''' </summary>
        Private Sub FindDialog()

            ' Enumerate windows to find the message box
            If Me.tries < 0 Then
                Return
            End If

            Dim callback As New NativeMethods.EnumThreadWndProc(AddressOf Me.CheckWindow)

            If NativeMethods.EnumThreadWindows(NativeMethods.GetCurrentThreadId(), callback, IntPtr.Zero) Then

                If Threading.Interlocked.Increment(Me.tries) < 10 Then
                    Me.ownerForm1.BeginInvoke(New MethodInvoker(AddressOf Me.FindDialog))
                End If

            End If

            If Me.timeOut1 > 0 Then

                Me.timeoutTimer = New Timer With
                                  {
                                      .Interval = Me.timeOut1,
                                      .Enabled = True
                                  }

                Me.timeoutTimer.Start()

            End If

        End Sub

        ''' <summary>
        ''' Checks whether the specified window is our <see cref="CenteredMessageBox"/> dialog.
        ''' </summary>
        ''' <param name="hWnd">A handle to the window to check.</param>
        ''' <param name="lParam">The application-defined value given in the <see cref="NativeMethods.EnumThreadWindows"/> function.</param>
        ''' <returns>
        ''' <c>true</c> the specified window is our <see cref="CenteredMessageBox"/> dialog, <c>false</c> otherwise.
        ''' </returns>
        Private Function CheckWindow(ByVal hWnd As IntPtr,
                                     ByVal lParam As IntPtr) As Boolean

            ' Checks if <hWnd> is a dialog
            Dim sb As New StringBuilder(260)
            NativeMethods.GetClassName(hWnd, sb, sb.Capacity)
            If sb.ToString() <> "#32770" Then
                Return True
            End If

            ' Get the control that displays the text.
            Dim hText As IntPtr = NativeMethods.GetDlgItem(hWnd, &HFFFFI)
            Me.messageBoxWindowHandle1 = hWnd

            ' Get the dialog Rect.
            Dim frmRect As New Rectangle(Me.ownerForm1.Location, Me.ownerForm1.Size)
            Dim dlgRect As NativeMethods.Rect
            NativeMethods.GetWindowRect(hWnd, dlgRect)

            ' Set the custom Font (if any).
            If hText <> IntPtr.Zero Then

                Me.SetFont(font:=Me.font1,
                           hwnd:=hText,
                           rect:=frmRect)

            End If

            ' Center the dialog window in the specified Form.
            Me.CenterDialog(hwnd:=hWnd,
                            dialogRect:=dlgRect,
                            formRect:=frmRect)

            ' Stop the EnumThreadWndProc callback by sending False.
            Return False

        End Function

        ''' <summary>
        ''' Sets the font of this <see cref="CenteredMessageBox"/> window.
        ''' </summary>
        ''' <param name="font">The <see cref="Font"/> used to display the <see cref="CenteredMessageBox"/> text.</param>
        ''' <param name="hwnd">A handle to the <see cref="CenteredMessageBox"/> window.</param>
        ''' <param name="rect">A <see cref="Rectangle"/> to positionate the text.</param>
        Private Sub SetFont(ByVal font As Font,
                            ByVal hwnd As IntPtr,
                            ByVal rect As Rectangle)

            Select Case font IsNot Nothing

                Case True
                    ' Set the text position.
                    NativeMethods.SetWindowPos(hWnd:=hwnd,
                                               hWndInsertAfter:=IntPtr.Zero,
                                               x:=65,
                                               y:=35,
                                               cx:=rect.Width,
                                               cy:=font.Height,
                                               uFlags:=NativeMethods.SetWindowPosFlags.None)

                    ' Set the new font.
                    NativeMethods.SendMessage(hWnd:=hwnd,
                                              msg:=NativeMethods.WindowsMessages.WM_SETFONT,
                                              wParam:=font.ToHfont,
                                              lParam:=New IntPtr(1))

                Case Else
                    ' Do Nothing.

                    ' Get the dialog font.
                    ' dim fnt as Font = Font.FromHfont(NativeMethods.SendMessage(hWnd:=hwnd,
                    '                                                            msg:=NativeMethods.WindowsMessages.WM_GETFONT,
                    '                                                            wParam:=IntPtr.Zero,
                    '                                                            lParam:=IntPtr.Zero))

            End Select

        End Sub

        ''' <summary>
        ''' Centers the <see cref="CenteredMessageBox"/> dialog in the specified <see cref="Form"/>.
        ''' </summary>
        ''' <param name="hwnd">A handle to the <see cref="CenteredMessageBox"/> window.</param>
        ''' <param name="dialogRect">The dialog <see cref="NativeMethods.Rect"/> structure.</param>
        ''' <param name="formRect">The form <see cref="Rectangle"/> structure.</param>
        Private Sub CenterDialog(ByVal hwnd As IntPtr,
                                 ByVal dialogRect As NativeMethods.Rect,
                                 ByVal formRect As Rectangle)

            ' Resize and positionate the messagebox window.
            NativeMethods.MoveWindow(hwnd,
                                     x:=formRect.Left + (formRect.Width - dialogRect.Right + dialogRect.Left) \ 2I,
                                     y:=formRect.Top + (formRect.Height - dialogRect.Bottom + dialogRect.Top) \ 2I,
                                     width:=(dialogRect.Right - dialogRect.Left),
                                     height:=(dialogRect.Bottom - dialogRect.Top),
                                     repaint:=True)

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the Tick event of the TimeoutTimer control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub TimeoutTimer_Tick(ByVal sender As Object, ByVal e As EventArgs) _
        Handles timeoutTimer.Tick

            NativeMethods.DestroyWindow(Me.messageBoxWindowHandle1)
            Me.Dispose()

        End Sub

#End Region

#Region " IDisposable "

        ''' <summary>
        ''' To detect redundant calls when disposing.
        ''' </summary>
        Private isDisposed As Boolean = False

        ''' <summary>
        ''' Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            Me.Dispose(isDisposing:=True)
            GC.SuppressFinalize(obj:=Me)

        End Sub

        ''' <summary>
        ''' Releases unmanaged and - optionally - managed resources.
        ''' </summary>
        ''' <param name="IsDisposing">
        ''' <c>true</c> to release both managed and unmanaged resources; 
        ''' <c>false</c> to release only unmanaged resources.
        ''' </param>
        Protected Sub Dispose(ByVal isDisposing As Boolean)

            If Not Me.isDisposed Then

                If isDisposing Then

                    Me.tries = -1
                    Me.ownerForm1 = Nothing

                    If Me.font1 IsNot Nothing Then
                        Me.font1.Dispose()
                    End If

                End If

            End If

            Me.isDisposed = True

        End Sub

#End Region

    End Class

End Namespace

#End Region