' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="MappingForm.vb" company="Elektro Studios">
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
Imports HostsMan.UserInterface.Theming

#End Region

#Region " MappingForm "

Namespace UserInterface

    ''' <summary>
    ''' The form to add or edit a Hosts file mapping.
    ''' <para></para>
    ''' The IP address accepts both IPv4 and IPv6 addresses. The confirm button is only enabled while
    ''' all the values are valid, and the invalid values are highlighted.
    ''' </summary>
    Public NotInheritable Class MappingForm

#Region " Constants "

        ''' <summary>
        ''' The IP address shown for a new mapping.
        ''' </summary>
        Friend Const DefaultIPAddress As String = "127.0.0.1"

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the mapping that receives the result when the user confirms the form.
        ''' </summary>
        ''' <value>The mapping result.</value>
        Friend ReadOnly Property MappingResult As MappingInfo

        ''' <summary>
        ''' Gets a value indicating whether the entered IP address is valid.
        ''' </summary>
        ''' <value><see langword="True"/> if the IP address is valid; otherwise, <see langword="False"/>.</value>
        Friend ReadOnly Property IsIPAddressValid As Boolean
            Get
                Return MappingValidator.IsValidIPAddress(Me.TextBox_IP.Text.Trim())
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the entered hostname is valid.
        ''' </summary>
        ''' <value><see langword="True"/> if the hostname is valid; otherwise, <see langword="False"/>.</value>
        Friend ReadOnly Property IsHostNameValid As Boolean
            Get
                Return MappingValidator.IsValidHostName(Me.TextBox_HostName.Text.Trim())
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the entered comment is valid.
        ''' </summary>
        ''' <value><see langword="True"/> if the comment is valid; otherwise, <see langword="False"/>.</value>
        Friend ReadOnly Property IsCommentValid As Boolean
            Get
                Return MappingValidator.IsValidComment(Me.TextBox_Comment.Text)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether all the entered values are valid.
        ''' </summary>
        ''' <value><see langword="True"/> if all the values are valid; otherwise, <see langword="False"/>.</value>
        Friend ReadOnly Property IsInputValid As Boolean
            Get
                Return Me.IsIPAddressValid AndAlso Me.IsHostNameValid AndAlso Me.IsCommentValid
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="MappingForm"/> class.
        ''' </summary>
        ''' <param name="mappingInfo">The <see cref="MappingInfo"/> object to edit, that receives the result mapping.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="mappingInfo"/> is <see langword="Nothing"/>.</exception>
        Public Sub New(ByVal mappingInfo As MappingInfo)

            If mappingInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(mappingInfo))
            End If

            ' This call is required by the designer.
            Me.InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.MappingResult = mappingInfo
            Me.InitializeControls()
            Me.InitializeButtonImages()

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the TextChanged event of the TextBox controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub TextBoxes_TextChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles TextBox_IP.TextChanged,
                TextBox_HostName.TextChanged,
                TextBox_Comment.TextChanged

            Me.UpdateValidationState()

        End Sub

        ''' <summary>
        ''' Handles the Shown event of the MappingForm Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub MappingForm_Shown(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Shown

            ' The theme is applied when the handle is created; re-apply the validation colors over it.
            Me.UpdateValidationState()
            Me.TextBox_IP.Select()
            Me.TextBox_IP.SelectAll()

        End Sub

        ''' <summary>
        ''' Handles the Click event of the Button_Cancel control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Button_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button_Cancel.Click

            Me.DialogResult = DialogResult.Cancel

        End Sub

        ''' <summary>
        ''' Handles the Click event of the Button_Confirm control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Button_Confirm_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button_Confirm.Click

            Me.CommitMapping()

        End Sub

#End Region

#Region " Friend Methods "

        ''' <summary>
        ''' Copies the entered values to <see cref="MappingResult"/> and closes the form with <see cref="DialogResult.OK"/>.
        ''' </summary>
        ''' <exception cref="InvalidOperationException">The entered values are not valid.</exception>
        Friend Sub CommitMapping()

            If Not Me.IsInputValid Then
                Throw New InvalidOperationException("The mapping values are not valid.")
            End If

            With Me.MappingResult
                .IP = Me.TextBox_IP.Text.Trim()
                .HostName = Me.TextBox_HostName.Text.Trim()
                .Comment = Me.TextBox_Comment.Text.Trim()
                .IsDisabled = Me.RadioButton_Disabled.Checked
            End With

            Me.DialogResult = DialogResult.OK

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Initializes the controls from <see cref="MappingResult"/>.
        ''' </summary>
        Private Sub InitializeControls()

            Me.TextBox_IP.Text = If(String.IsNullOrEmpty(Me.MappingResult.IP), MappingForm.DefaultIPAddress, Me.MappingResult.IP)
            Me.TextBox_HostName.Text = If(Me.MappingResult.HostName, String.Empty)
            Me.TextBox_Comment.Text = If(Me.MappingResult.Comment, String.Empty)
            Me.RadioButton_Enabled.Checked = Not Me.MappingResult.IsDisabled
            Me.RadioButton_Disabled.Checked = Me.MappingResult.IsDisabled

            Me.UpdateValidationState()

        End Sub

        ''' <summary>
        ''' Sets the images of the OK and Cancel buttons (the same images of the
        ''' "Enable" and "Remove" buttons of the main window), scaled to the current DPI.
        ''' The images are disposed together with the form.
        ''' <para></para>
        ''' The image size is derived from the button font height, so it scales with the screen DPI
        ''' (24 pixels at 96 DPI).
        ''' </summary>
        Private Sub InitializeButtonImages()

            Dim imageSize As Integer = MappingForm.GetButtonImageSize(Me.Button_Confirm.Font)

            Using okSource As Bitmap = My.Resources.Enable,
                  cancelSource As Bitmap = My.Resources.Remove

                Me.Button_Confirm.Image = IconHelper.ResizeImage(okSource, New Size(imageSize, imageSize))
                Me.Button_Cancel.Image = IconHelper.ResizeImage(cancelSource, New Size(imageSize, imageSize))

            End Using

            AddHandler Me.Disposed, AddressOf Me.DisposeButtonImages

        End Sub

        ''' <summary>
        ''' Gets the size, in pixels, of the images shown in the OK and Cancel buttons for the specified button font.
        ''' </summary>
        ''' <param name="font">The button font.</param>
        ''' <returns>The image size, in pixels.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="font"/> is <see langword="Nothing"/>.</exception>
        Friend Shared Function GetButtonImageSize(ByVal font As Font) As Integer

            If font Is Nothing Then
                Throw New ArgumentNullException(NameOf(font))
            End If

            Return font.Height + 4

        End Function

        ''' <summary>
        ''' Disposes the images of the OK and Cancel buttons.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub DisposeButtonImages(ByVal sender As Object, ByVal e As EventArgs)

            For Each button As Button In {Me.Button_Confirm, Me.Button_Cancel}
                Dim image As Image = button.Image
                button.Image = Nothing
                image.Dispose()
            Next button

        End Sub

        ''' <summary>
        ''' Enables the confirm button only when all the values are valid, and highlights the invalid values.
        ''' </summary>
        Private Sub UpdateValidationState()

            Dim palette As ThemePalette = ThemeManager.CurrentPalette

            ' Empty fields are not highlighted (the user did not type anything yet), but they are still invalid.
            Me.TextBox_IP.ForeColor = If(Me.IsIPAddressValid OrElse Me.TextBox_IP.TextLength = 0, palette.InputFore, palette.ErrorFore)
            Me.TextBox_HostName.ForeColor = If(Me.IsHostNameValid OrElse Me.TextBox_HostName.TextLength = 0, palette.InputFore, palette.ErrorFore)
            Me.TextBox_Comment.ForeColor = If(Me.IsCommentValid, palette.InputFore, palette.ErrorFore)

            Me.Button_Confirm.Enabled = Me.IsInputValid

        End Sub

#End Region

    End Class

End Namespace

#End Region
