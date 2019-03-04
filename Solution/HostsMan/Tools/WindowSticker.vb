' ***********************************************************************
' Author   : Elektro
' Modified : 08-December-2014
' ***********************************************************************
' <copyright file="WindowSticker.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Usage Examples "

' ''' <summary>
' ''' The <see cref="WindowSticker"/> instance that sticks the Form on the Desktop screen.
' ''' </summary>
'Private windowSticker As New WindowSticker(ClientForm:=Me) With {.SnapMargin = 35}

'Private Sub Form1_Load() Handles MyBase.Shown

'    WindowSticker.Dispose()
'    WindowSticker = New WindowSticker(Form2)
'    WindowSticker.ClientForm.Show()

'End Sub

#End Region

#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Runtime.InteropServices

#End Region

#Region " WindowSticker "

Namespace Tools

    ''' <summary>
    ''' Sticks a Form to a Desktop border (if the Form is near).
    ''' </summary>
    Public NotInheritable Class WindowSticker : Inherits NativeWindow : Implements IDisposable

#Region " Properties "

#Region " Public "

        ''' <summary>
        ''' Gets the client form used to stick its borders.
        ''' </summary>
        ''' <value>The client form used to stick its borders.</value>
        Public ReadOnly Property ClientForm As Form
            Get
                Return Me.clientForm1
            End Get
        End Property
        Private WithEvents clientForm1 As Form

        ''' <summary>
        ''' Gets or sets the snap margin (offset), in pixels.
        ''' (Default value is: 30))
        ''' </summary>
        ''' <value>The snap margin (offset), in pixels.</value>
        Public Property SnapMargin As Integer
            Get
                Return Me.snapMargin1
            End Get
            Set(ByVal value As Integer)
                Me.DisposedCheck()
                Me.snapMargin1 = value
            End Set
        End Property
        ''' <summary>
        ''' The snap margin (offset), in pixels.
        ''' </summary>
        Private snapMargin1 As Integer = 30I

#End Region

#Region " Private "

        ''' <summary>
        ''' Gets rectangle that contains the size of the current screen.
        ''' </summary>
        ''' <value>The rectangle that contains the size of the current screen.</value>
        Private ReadOnly Property ScreenRect As Rectangle
            Get
                Return Screen.FromControl(Me.clientForm1).Bounds
            End Get
        End Property

        ''' <summary>
        ''' Gets the working area of the current screen.
        ''' </summary>
        ''' <value>The working area of the current screen.</value>
        Private ReadOnly Property WorkingArea As Rectangle
            Get
                Return Screen.FromControl(Me.clientForm1).WorkingArea
            End Get
        End Property

        ''' <summary>
        ''' Gets the desktop taskbar height (when thet taskbar is horizontal).
        ''' </summary>
        ''' <value>The desktop taskbar height (when thet taskbar is horizontal).</value>
        Private ReadOnly Property TaskbarHeight As Integer
            Get
                Return Me.ScreenRect.Height - Me.WorkingArea.Height
            End Get
        End Property

#End Region

#End Region

#Region " Objects "

        ''' <summary>
        ''' Determines whether the form is being resized by one of its borders.
        ''' </summary>
        Private isResizing As Boolean

#End Region

#Region " Enumerations "

        ''' <summary>
        ''' Windows Message Identifiers.
        ''' </summary>
        <Description("Messages to process in WndProc")>
        Private Enum WindowsMessages As Integer

            ''' <summary>
            ''' Sent to a window that the user is resizing. 
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632647%28v=vs.85%29.aspx
            ''' </summary>
            WM_SIZING = &H214

            ''' <summary>
            ''' Sent one time to a window, after it has exited the moving or sizing modal loop. 
            ''' The window enters the moving or sizing modal loop when the user clicks the window's title bar 
            ''' or sizing border.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632623%28v=vs.85%29.aspx
            ''' </summary>
            WM_EXITSIZEMOVE = &H232

            ''' <summary>
            ''' Sent to a window whose size, position, or place in the Z order is about to change.
            ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632653%28v=vs.85%29.aspx
            ''' </summary>
            WM_WINDOWPOSCHANGING = &H46I

        End Enum

#End Region

#Region " Structures "

        ''' <summary>
        ''' Contains information about the size and position of a window.
        ''' MSDN Documentation: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632612%28v=vs.85%29.aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Private Structure WindowPos

            ''' <summary>
            ''' A handle to the window.
            ''' </summary>
            Public Hwnd As IntPtr

            ''' <summary>
            ''' The position of the window in Z order (front-to-back position). 
            ''' This member can be a handle to the window behind which this window is placed, 
            ''' or can be one of the special values listed with the 'SetWindowPos' function. 
            ''' </summary>
            Public HwndInsertAfter As IntPtr

            ''' <summary>
            ''' The position of the left edge of the window.
            ''' </summary>
            Public X As Integer

            ''' <summary>
            ''' The position of the top edge of the window.
            ''' </summary>
            Public Y As Integer

            ''' <summary>
            ''' The window width, in pixels.
            ''' </summary>
            Public Width As Integer

            ''' <summary>
            ''' The window height, in pixels.
            ''' </summary>
            Public Height As Integer

            ''' <summary>
            ''' Flag containing the window position.
            ''' </summary>
            Public Flags As Integer

        End Structure

#End Region

#Region " Constructor "

        ''' <summary>
        ''' Initializes a new instance of WindowSticker class.
        ''' </summary>
        ''' <param name="ClientForm">The client form to assign this NativeWindow.</param>
        Public Sub New(ByVal clientForm As Form)

            ' Assign the Formulary.
            Me.clientForm1 = clientForm

        End Sub

        ''' <summary>
        ''' Prevents a default instance of the <see cref="WindowSticker"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Assign the handle of the target Form to this NativeWindow,
        ''' necessary to override target Form's WndProc.
        ''' </summary>
        Private Sub SetFormHandle(ByVal sender As Object, ByVal e As EventArgs) Handles clientForm1.HandleCreated, clientForm1.Load, clientForm1.Shown

            If (Me.clientForm1 IsNot Nothing) AndAlso (Not MyBase.Handle.Equals(Me.clientForm1.Handle)) Then

                MyBase.AssignHandle(Me.clientForm1.Handle)

            End If

        End Sub

        ''' <summary>
        ''' Releases the Handle.
        ''' </summary>
        Private Sub OnHandleDestroyed(ByVal sender As Object, ByVal e As EventArgs) Handles clientForm1.HandleDestroyed

            MyBase.ReleaseHandle()

        End Sub

#End Region

#Region " WndProc "

        ''' <summary>
        ''' Invokes the default window procedure associated with this window to process messages.
        ''' </summary>
        ''' <param name="m">
        ''' A <see cref="T:System.Windows.Forms.Message" /> that is associated with the current Windows message.
        ''' </param>
        Protected Overrides Sub WndProc(ByRef m As Message)

            Select Case m.Msg

                Case WindowsMessages.WM_SIZING
                    Me.isResizing = True

                Case WindowsMessages.WM_EXITSIZEMOVE
                    Me.isResizing = False

                Case WindowsMessages.WM_WINDOWPOSCHANGING
                    If Not Me.isResizing = True Then
                        Me.SnapToDesktopBorder(clientForm:=Me.clientForm1, handle:=m.LParam, snapMargin:=Me.snapMargin1)
                    End If

            End Select

            MyBase.WndProc(m)

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Sticks a Form to a desktop border (it its near).
        ''' </summary>
        ''' <param name="ClientForm">The client form used to stick its borders.</param>
        ''' <param name="Handle">A pointer to a 'WINDOWPOS' structure that contains information about the window's new size and position.</param>
        ''' <param name="snapMargin">the snap margin (offset), in pixels.</param>
        Private Sub SnapToDesktopBorder(ByVal clientForm As Form,
                                        ByVal handle As IntPtr,
                                        Optional ByVal snapMargin As Integer = 0I)

            Dim newPosition As WindowPos = CType(Marshal.PtrToStructure(handle, GetType(WindowPos)), WindowPos)

            If (newPosition.Y = 0) OrElse (newPosition.X = 0) Then
                ' Nothing to do.
                Exit Sub
            End If

            ' Top border (check if taskbar is on top or bottom via WorkingRect.Y)
            If (newPosition.Y >= -snapMargin AndAlso (Me.WorkingArea.Y > 0 AndAlso newPosition.Y <= (Me.TaskbarHeight + Me.SnapMargin))) _
            OrElse (Me.WorkingArea.Y <= 0 AndAlso newPosition.Y <= (snapMargin)) Then

                If Me.TaskbarHeight > 0 Then
                    ' Horizontal Taskbar
                    newPosition.Y = Me.WorkingArea.Y
                Else
                    ' Vertical Taskbar
                    newPosition.Y = 0
                End If

            End If

            ' Left border
            If (newPosition.X >= Me.WorkingArea.X - Me.SnapMargin) _
            AndAlso (newPosition.X <= Me.WorkingArea.X + Me.SnapMargin) Then

                newPosition.X = Me.WorkingArea.X

            End If

            ' Right border.
            If (newPosition.X + clientForm.Width <= Me.WorkingArea.Right + Me.SnapMargin) _
            AndAlso (newPosition.X + clientForm.Width >= Me.WorkingArea.Right - Me.SnapMargin) Then

                newPosition.X = (Me.WorkingArea.Right - clientForm.Width)

            End If

            ' Bottom border.
            If (newPosition.Y + clientForm.Height <= Me.WorkingArea.Bottom + Me.SnapMargin) _
            AndAlso (newPosition.Y + clientForm.Height >= Me.WorkingArea.Bottom - Me.SnapMargin) Then

                newPosition.Y = (Me.WorkingArea.Bottom - clientForm.Height)

            End If

            ' Marshal it back.
            Marshal.StructureToPtr([structure]:=newPosition, ptr:=handle, fDeleteOld:=True)

        End Sub

#End Region

#Region " Hidden Methods "

        ''' <summary>
        ''' Determines whether the specified System.Object instances are the same instance.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Private Shadows Sub ReferenceEquals()
        End Sub

        ''' <summary>
        ''' Assigns a handle to this window.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Sub AssignHandle(handle As IntPtr)
            MyBase.AssignHandle(handle)
        End Sub

        ''' <summary>
        ''' Creates a window and its handle with the specified creation parameters.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Sub CreateHandle(cp As CreateParams)
            MyBase.CreateHandle(cp)
        End Sub

        ''' <summary>
        ''' Destroys the window and its handle.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Sub DestroyHandle()
            MyBase.DestroyHandle()
        End Sub

        ''' <summary>
        ''' Releases the handle associated with this window.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Sub ReleaseHandle()
            MyBase.ReleaseHandle()
        End Sub

        ''' <summary>
        ''' Retrieves the window associated with the specified handle.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Private Shadows Sub FromHandle()
        End Sub

        ''' <summary>
        ''' Serves as a hash function for a particular type.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Function GetHashCode() As Integer
            Return MyBase.GetHashCode
        End Function

        ''' <summary>
        ''' Gets the System.Type of the current instance.
        ''' </summary>
        ''' <returns>The exact runtime type of the current instance.</returns>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Function [GetType]() As Type
            Return MyBase.GetType
        End Function

        ''' <summary>
        ''' Retrieves the current lifetime service object that controls the lifetime policy for this instance.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Function GetLifeTimeService() As Object
            Return MyBase.GetLifetimeService
        End Function

        ''' <summary>
        ''' Obtains a lifetime service object to control the lifetime policy for this instance.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Function InitializeLifeTimeService() As Object
            Return MyBase.InitializeLifetimeService
        End Function

        ''' <summary>
        ''' Creates an object that contains all the relevant information required to generate a proxy used to communicate with a remote object.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Function CreateObjRef(requestedType As Type) As System.Runtime.Remoting.ObjRef
            Return MyBase.CreateObjRef(requestedType)
        End Function

        ''' <summary>
        ''' Determines whether the specified System.Object instances are considered equal.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj)
        End Function

        ''' <summary>
        ''' Returns a String that represents the current object.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Function ToString() As String
            Return MyBase.ToString
        End Function

        ''' <summary>
        ''' Invokes the default window procedure associated with this window.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Sub DefWndProc(ByRef m As Message)
        End Sub

#End Region

#Region " IDisposable "

        ''' <summary>
        ''' To detect redundant calls when disposing.
        ''' </summary>
        Private isDisposed As Boolean = False

        ''' <summary>
        ''' Prevent calls to methods after disposing.
        ''' </summary>
        ''' <exception cref="System.ObjectDisposedException"></exception>
        Private Sub DisposedCheck()

            If Me.isDisposed Then
                Throw New ObjectDisposedException(Me.GetType().FullName)
            End If

        End Sub

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
                    Me.clientForm1 = Nothing
                    MyBase.ReleaseHandle()
                    MyBase.DestroyHandle()
                End If

            End If

            Me.isDisposed = True

        End Sub

#End Region

    End Class

End Namespace

#End Region