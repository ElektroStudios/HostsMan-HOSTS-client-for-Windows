' ***********************************************************************
' Author   : Elektro
' Modified : 18-January-2015
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

Imports HostsMan.Tools.HostsTools

#End Region

#Region " AddMappingForm "

Namespace UserInterface

    ''' <summary>
    ''' Class MappingForm.
    ''' </summary>
    Public NotInheritable Class MappingForm

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the add mapping result.
        ''' </summary>
        ''' <value>The add mapping result.</value>
        Private Property MappingResult As MappingInfo

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="MappingForm"/> class.
        ''' </summary>
        ''' <param name="mappingInfo">The <see cref="MappingInfo"/> object to assign the result mapping.</param>
        Public Sub New(ByRef mappingInfo As MappingInfo)

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.MappingResult = mappingInfo

        End Sub

#End Region

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the TextChanged event of the TextBox controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub TextBoxes_TextChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MaskedTextBox_IP1.TextChanged,
                MaskedTextBox_IP2.TextChanged,
                MaskedTextBox_IP3.TextChanged,
                MaskedTextBox_IP4.TextChanged,
                TextBox_HostName.TextChanged

            Me.Button_Confirm.Enabled = Not String.IsNullOrEmpty(Me.MaskedTextBox_IP1.Text) AndAlso _
                                        Not String.IsNullOrEmpty(Me.MaskedTextBox_IP2.Text) AndAlso _
                                        Not String.IsNullOrEmpty(Me.MaskedTextBox_IP3.Text) AndAlso _
                                        Not String.IsNullOrEmpty(Me.MaskedTextBox_IP4.Text) AndAlso _
                                        Not String.IsNullOrEmpty(Me.TextBox_HostName.Text)

        End Sub

        ''' <summary>
        ''' Handles the KeyPress event of the TextBox controls.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        Private Sub TextBoxes_KeyPress(sender As Object, e As KeyPressEventArgs) _
        Handles MaskedTextBox_IP1.KeyPress,
                MaskedTextBox_IP2.KeyPress,
                MaskedTextBox_IP3.KeyPress,
                MaskedTextBox_IP4.KeyPress,
                TextBox_HostName.KeyPress,
                TextBox_Comment.KeyPress

            If e.KeyChar = Convert.ToChar(Keys.Enter) _
            AndAlso Me.Button_Confirm.Enabled Then

                Me.EndMapping()

            End If

        End Sub

        ''' <summary>
        ''' Handles the KeyPress event of the MaskedTextBox_IP1 control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        Private Sub MaskedTextBox_IP1_KeyPress(sender As Object, e As KeyPressEventArgs) _
        Handles MaskedTextBox_IP1.KeyPress

            Select Case DirectCast(sender, MaskedTextBox).SelectionStart

                Case 2
                    Me.MaskedTextBox_IP2.Select()
                    Me.MaskedTextBox_IP2.SelectionStart = 0

                Case 3
                    Me.MaskedTextBox_IP2.Select()
                    Me.MaskedTextBox_IP2.SelectionStart = 0
                    Me.MaskedTextBox_IP2.Text = e.KeyChar

            End Select

        End Sub

        ''' <summary>
        ''' Handles the KeyPress event of the MaskedTextBox_IP2 control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        Private Sub MaskedTextBox_IP2_KeyPress(sender As Object, e As KeyPressEventArgs) _
        Handles MaskedTextBox_IP2.KeyPress

            Select Case DirectCast(sender, MaskedTextBox).SelectionStart

                Case 2
                    Me.MaskedTextBox_IP3.Select()
                    Me.MaskedTextBox_IP3.SelectionStart = 0

                Case 3
                    Me.MaskedTextBox_IP3.Select()
                    Me.MaskedTextBox_IP3.SelectionStart = 0
                    Me.MaskedTextBox_IP3.Text = e.KeyChar

            End Select

        End Sub

        ''' <summary>
        ''' Handles the KeyPress event of the MaskedTextBox_IP3 control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        Private Sub MaskedTextBox_IP3_KeyPress(sender As Object, e As KeyPressEventArgs) _
        Handles MaskedTextBox_IP3.KeyPress

            Select Case DirectCast(sender, MaskedTextBox).SelectionStart

                Case 2
                    Me.MaskedTextBox_IP4.Select()
                    Me.MaskedTextBox_IP4.SelectionStart = 0

                Case 3
                    Me.MaskedTextBox_IP4.Select()
                    Me.MaskedTextBox_IP4.SelectionStart = 0
                    Me.MaskedTextBox_IP4.Text = e.KeyChar

            End Select

        End Sub

        ''' <summary>
        ''' Handles the Load event of the AddMappingForm Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub AddMappingForm_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load

            Me.InitializeControls()

        End Sub

        ''' <summary>
        ''' Handles the Shown event of the AddMappingForm Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub AddMappingForm_Shown(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Shown

            Me.MaskedTextBox_IP1.SelectionStart = 0

        End Sub

        ''' <summary>
        ''' Handles the Click event of the Button_Cancel control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Button_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button_Cancel.Click

            Me.DialogResult = Windows.Forms.DialogResult.Cancel

        End Sub

        ''' <summary>
        ''' Handles the Click event of the Button_Confirm control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Button_Confirm_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button_Confirm.Click

            Me.EndMapping()

        End Sub

#End Region

#Region " Methods "

        ''' <summary>
        ''' Initializes the IP MaskedTextBox.
        ''' </summary>
        Private Sub InitializeControls()

            Select Case String.IsNullOrEmpty(Me.MappingResult.IP)

                Case True
                    Me.MaskedTextBox_IP1.Text = "127"
                    Me.MaskedTextBox_IP2.Text = "0"
                    Me.MaskedTextBox_IP3.Text = "0"
                    Me.MaskedTextBox_IP4.Text = "1"

                Case Else
                    Dim ip() As String = Me.MappingResult.IP.Split({"."c}, StringSplitOptions.RemoveEmptyEntries)
                    Me.MaskedTextBox_IP1.Text = ip(0)
                    Me.MaskedTextBox_IP2.Text = ip(1)
                    Me.MaskedTextBox_IP3.Text = ip(2)
                    Me.MaskedTextBox_IP4.Text = ip(3)

            End Select

            Me.TextBox_HostName.Text = Me.MappingResult.HostName
            Me.TextBox_Comment.Text = Me.MappingResult.Comment
            Me.RadioButton_Disabled.Checked = Me.MappingResult.IsDisabled

        End Sub

        ''' <summary>
        ''' Ends the mapping.
        ''' </summary>
        Private Sub EndMapping()

            Me.SaveMapping()
            Me.DialogResult = Windows.Forms.DialogResult.OK

        End Sub

        ''' <summary>
        ''' Saves the mapping.
        ''' </summary>
        Private Sub SaveMapping()

            With Me.MappingResult

                .IP = String.Format("{0}.{1}.{2}.{3}",
                                    Me.MaskedTextBox_IP1.Text,
                                    Me.MaskedTextBox_IP2.Text,
                                    Me.MaskedTextBox_IP3.Text,
                                    Me.MaskedTextBox_IP4.Text)

                .HostName = Me.TextBox_HostName.Text
                .Comment = Me.TextBox_Comment.Text
                .IsDisabled = Me.RadioButton_Disabled.Checked

            End With

        End Sub

#End Region

    End Class

End Namespace

#End Region