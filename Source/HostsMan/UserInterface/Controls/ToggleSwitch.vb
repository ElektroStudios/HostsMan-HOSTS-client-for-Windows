' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ToggleSwitch.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

Imports HostsMan.UserInterface.Theming

#End Region

#Region " ToggleSwitch "

Namespace UserInterface.Controls

    ''' <summary>
    ''' A modern on/off toggle switch control with a caption, that supports the application themes.
    ''' <para></para>
    ''' The caption (<see cref="Control.Text"/>) is drawn at the left and the switch at the right.
    ''' The switch can be toggled with the mouse, or with the Space key when it has the focus.
    ''' </summary>
    <DefaultEvent("CheckedChanged")>
    <DefaultProperty("Checked")>
    <DesignerCategory("Code")>
    <ToolboxItem(True)>
    Public Class ToggleSwitch : Inherits Control : Implements IThemedControl

#Region " Constants "

        ''' <summary>
        ''' The gap, in pixels, between the caption and the switch.
        ''' </summary>
        Private Const CaptionGap As Integer = 6

#End Region

#Region " Events "

        ''' <summary>
        ''' Occurs when the value of the <see cref="Checked"/> property changes.
        ''' </summary>
        <Category("Behavior")>
        <Description("Occurs when the value of the Checked property changes.")>
        Public Event CheckedChanged As EventHandler

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets a value indicating whether the switch is in the on (checked) state.
        ''' </summary>
        ''' <value><see langword="True"/> if the switch is on; otherwise, <see langword="False"/>.</value>
        <Category("Appearance")>
        <DefaultValue(False)>
        <Description("Indicates whether the switch is in the on (checked) state.")>
        Public Property Checked As Boolean
            Get
                Return Me.checked1
            End Get
            Set(ByVal value As Boolean)
                If value <> Me.checked1 Then
                    Me.checked1 = value
                    If Me.IsHandleCreated Then
                        Me.AccessibilityNotifyClients(AccessibleEvents.StateChange, -1)
                    End If
                    Me.Invalidate()
                    Me.OnCheckedChanged(EventArgs.Empty)
                End If
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="Checked"/> property. )
        ''' </summary>
        Private checked1 As Boolean

        ''' <summary>
        ''' Gets or sets the caption shown while the switch is on (checked).
        ''' If empty, <see cref="Control.Text"/> is shown.
        ''' </summary>
        ''' <value>The caption shown while the switch is on.</value>
        <Category("Appearance")>
        <DefaultValue("")>
        <Description("The caption shown while the switch is on. If empty, the Text property is shown.")>
        Public Property CheckedText As String
            Get
                Return Me.checkedText1
            End Get
            Set(ByVal value As String)
                Me.checkedText1 = If(value, String.Empty)
                Me.Invalidate()
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="CheckedText"/> property. )
        ''' </summary>
        Private checkedText1 As String = String.Empty

        ''' <summary>
        ''' Gets or sets the caption shown while the switch is off (unchecked).
        ''' If empty, <see cref="Control.Text"/> is shown.
        ''' </summary>
        ''' <value>The caption shown while the switch is off.</value>
        <Category("Appearance")>
        <DefaultValue("")>
        <Description("The caption shown while the switch is off. If empty, the Text property is shown.")>
        Public Property UncheckedText As String
            Get
                Return Me.uncheckedText1
            End Get
            Set(ByVal value As String)
                Me.uncheckedText1 = If(value, String.Empty)
                Me.Invalidate()
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="UncheckedText"/> property. )
        ''' </summary>
        Private uncheckedText1 As String = String.Empty

        ''' <summary>
        ''' Gets the caption currently shown: <see cref="CheckedText"/> or <see cref="UncheckedText"/>
        ''' depending on the <see cref="Checked"/> state, or <see cref="Control.Text"/> if that caption is empty.
        ''' </summary>
        ''' <value>The caption currently shown.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property DisplayText As String
            Get
                Dim stateText As String = If(Me.checked1, Me.checkedText1, Me.uncheckedText1)
                Return If(String.IsNullOrEmpty(stateText), Me.Text, stateText)
            End Get
        End Property

        ''' <summary>
        ''' Gets the palette used to paint the control.
        ''' </summary>
        ''' <value>The palette used to paint the control.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property Palette As ThemePalette
            Get
                Return Me.palette1
            End Get
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="Palette"/> property. )
        ''' </summary>
        Private palette1 As ThemePalette = ThemePalette.Light

        ''' <summary>
        ''' Gets the bounds of the switch (track) within the client area.
        ''' </summary>
        ''' <value>The bounds of the switch.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property SwitchBounds As Rectangle
            Get
                Dim switchSize As Size = ToggleSwitch.GetSwitchSize(Me.Font)
                Return New Rectangle(Math.Max(0, Me.ClientSize.Width - switchSize.Width - 1),
                                     Math.Max(0, (Me.ClientSize.Height - switchSize.Height) \ 2),
                                     switchSize.Width, switchSize.Height)
            End Get
        End Property

        ''' <summary>
        ''' Gets the default size of the control.
        ''' </summary>
        ''' <value>The default size of the control.</value>
        Protected Overrides ReadOnly Property DefaultSize As Size
            Get
                Return New Size(100, 22)
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ToggleSwitch"/> class.
        ''' </summary>
        Public Sub New()

            MyBase.New()

            Me.SetStyle(ControlStyles.UserPaint Or
                        ControlStyles.AllPaintingInWmPaint Or
                        ControlStyles.OptimizedDoubleBuffer Or
                        ControlStyles.ResizeRedraw Or
                        ControlStyles.Selectable Or
                        ControlStyles.StandardClick Or
                        ControlStyles.SupportsTransparentBackColor, True)

            Me.SetStyle(ControlStyles.StandardDoubleClick, False)

            Me.Cursor = Cursors.Hand
            Me.TabStop = True
            Me.AccessibleRole = AccessibleRole.CheckButton

        End Sub

#End Region

#Region " IThemedControl "

        ''' <summary>
        ''' Applies the specified palette to the control.
        ''' </summary>
        ''' <param name="palette">The palette to apply.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="palette"/> is <see langword="Nothing"/>.</exception>
        Public Sub ApplyPalette(ByVal palette As ThemePalette) Implements IThemedControl.ApplyPalette

            If palette Is Nothing Then
                Throw New ArgumentNullException(NameOf(palette))
            End If

            Me.palette1 = palette
            Me.Invalidate()

        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Toggles the <see cref="Checked"/> state.
        ''' </summary>
        Public Sub Toggle()

            Me.Checked = Not Me.Checked

        End Sub

        ''' <summary>
        ''' Retrieves the size of a rectangular area into which the control can be fitted (caption plus switch).
        ''' </summary>
        ''' <param name="proposedSize">The custom-sized area for the control.</param>
        ''' <returns>The preferred size of the control.</returns>
        Public Overrides Function GetPreferredSize(ByVal proposedSize As Size) As Size

            Dim switchSize As Size = ToggleSwitch.GetSwitchSize(Me.Font)
            ' The widest caption of both states is measured, so the size does not change when toggling.
            Dim textSize As Size = Drawing.Size.Empty
            For Each caption As String In {Me.Text, Me.checkedText1, Me.uncheckedText1}
                If Not String.IsNullOrEmpty(caption) Then
                    Dim captionSize As Size = TextRenderer.MeasureText(caption, Me.Font, Drawing.Size.Empty,
                                                                       TextFormatFlags.SingleLine Or TextFormatFlags.NoPadding)
                    textSize = New Size(Math.Max(textSize.Width, captionSize.Width), Math.Max(textSize.Height, captionSize.Height))
                End If
            Next caption

            Dim width As Integer = textSize.Width + If(textSize.Width > 0, CaptionGap, 0) + switchSize.Width + 2
            Dim height As Integer = Math.Max(textSize.Height, switchSize.Height) + 4

            Return New Size(width, height)

        End Function

#End Region

#Region " Overridden Methods "

        ''' <summary>
        ''' Raises the <see cref="CheckedChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnCheckedChanged(ByVal e As EventArgs)

            RaiseEvent CheckedChanged(Me, e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.Click"/> event, toggling the switch.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnClick(ByVal e As EventArgs)

            Me.Toggle()
            MyBase.OnClick(e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.KeyUp"/> event, toggling the switch when the Space key is released.
        ''' </summary>
        ''' <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnKeyUp(ByVal e As KeyEventArgs)

            If e.KeyCode = Keys.Space AndAlso e.Modifiers = Keys.None Then
                Me.Toggle()
                e.Handled = True
            End If

            MyBase.OnKeyUp(e)

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.TextChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)

            MyBase.OnTextChanged(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.EnabledChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnEnabledChanged(ByVal e As EventArgs)

            MyBase.OnEnabledChanged(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.GotFocus"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnGotFocus(ByVal e As EventArgs)

            MyBase.OnGotFocus(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.LostFocus"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnLostFocus(ByVal e As EventArgs)

            MyBase.OnLostFocus(e)
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Raises the <see cref="Control.Paint"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

            MyBase.OnPaint(e)

            Dim g As Graphics = e.Graphics
            Dim palette As ThemePalette = Me.palette1
            Dim switchRect As Rectangle = Me.SwitchBounds

            ' Caption (depends on the checked state).
            Dim caption As String = Me.DisplayText
            If Not String.IsNullOrEmpty(caption) Then
                Dim textRect As New Rectangle(0, 0, Math.Max(0, switchRect.Left - CaptionGap), Me.ClientSize.Height)
                Dim textColor As Color = If(Me.Enabled, Me.ForeColor, palette.DisabledFore)
                TextRenderer.DrawText(g, caption, Me.Font, textRect, textColor,
                                      TextFormatFlags.SingleLine Or TextFormatFlags.VerticalCenter Or
                                      TextFormatFlags.Right Or TextFormatFlags.EndEllipsis)
            End If

            g.SmoothingMode = SmoothingMode.AntiAlias

            ' Track.
            Dim trackColor As Color = If(Me.checked1, palette.Accent, palette.SwitchOffTrack)
            If Not Me.Enabled Then
                trackColor = palette.DisabledFore
            End If

            Using trackPath As GraphicsPath = ToggleSwitch.CreatePillPath(switchRect),
                  trackBrush As New SolidBrush(trackColor)
                g.FillPath(trackBrush, trackPath)
            End Using

            ' Thumb.
            Dim thumbMargin As Integer = Math.Max(2, switchRect.Height \ 6)
            Dim thumbDiameter As Integer = switchRect.Height - (thumbMargin * 2)
            Dim thumbX As Integer = If(Me.checked1,
                                       switchRect.Right - thumbMargin - thumbDiameter,
                                       switchRect.Left + thumbMargin)

            Using thumbBrush As New SolidBrush(palette.SwitchThumb)
                g.FillEllipse(thumbBrush, thumbX, switchRect.Top + thumbMargin, thumbDiameter, thumbDiameter)
            End Using

            ' Focus cue.
            If Me.Focused AndAlso Me.ShowFocusCues Then
                g.SmoothingMode = SmoothingMode.None
                Dim focusRect As Rectangle = Rectangle.Inflate(switchRect, 2, 2)
                focusRect.Intersect(Me.ClientRectangle)
                ControlPaint.DrawFocusRectangle(g, focusRect, Me.ForeColor, Me.BackColor)
            End If

        End Sub

        ''' <summary>
        ''' Creates a new accessibility object for the control.
        ''' </summary>
        ''' <returns>A new <see cref="AccessibleObject"/> for the control.</returns>
        Protected Overrides Function CreateAccessibilityInstance() As AccessibleObject

            Return New ToggleSwitchAccessibleObject(Me)

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Gets the size of the switch (track) for the specified font.
        ''' </summary>
        ''' <param name="font">The font of the control.</param>
        ''' <returns>The size of the switch.</returns>
        Friend Shared Function GetSwitchSize(ByVal font As Font) As Size

            Dim height As Integer = Math.Max(14, font.Height + 2)
            Return New Size(height * 2, height)

        End Function

        ''' <summary>
        ''' Creates a pill-shaped (fully rounded rectangle) path.
        ''' </summary>
        ''' <param name="bounds">The bounds of the pill.</param>
        ''' <returns>The pill-shaped path.</returns>
        Private Shared Function CreatePillPath(ByVal bounds As Rectangle) As GraphicsPath

            Dim diameter As Integer = bounds.Height
            Dim path As New GraphicsPath()
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 90, 180)
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 180)
            path.CloseFigure()
            Return path

        End Function

#End Region

#Region " Accessibility "

        ''' <summary>
        ''' Provides accessibility information for a <see cref="ToggleSwitch"/>.
        ''' </summary>
        Protected Class ToggleSwitchAccessibleObject : Inherits ControlAccessibleObject

            ''' <summary>
            ''' Initializes a new instance of the <see cref="ToggleSwitchAccessibleObject"/> class.
            ''' </summary>
            ''' <param name="owner">The owner control.</param>
            Public Sub New(ByVal owner As ToggleSwitch)

                MyBase.New(owner)

            End Sub

            ''' <summary>
            ''' Gets the state of the accessible object (including <see cref="AccessibleStates.Checked"/>).
            ''' </summary>
            ''' <value>The state of the accessible object.</value>
            Public Overrides ReadOnly Property State As AccessibleStates
                Get
                    Dim result As AccessibleStates = MyBase.State
                    If DirectCast(Me.Owner, ToggleSwitch).Checked Then
                        result = result Or AccessibleStates.Checked
                    End If
                    Return result
                End Get
            End Property

            ''' <summary>
            ''' Gets the default action of the accessible object.
            ''' </summary>
            ''' <value>The default action.</value>
            Public Overrides ReadOnly Property DefaultAction As String
                Get
                    Return "Toggle"
                End Get
            End Property

            ''' <summary>
            ''' Performs the default action (toggles the switch).
            ''' </summary>
            Public Overrides Sub DoDefaultAction()

                DirectCast(Me.Owner, ToggleSwitch).Toggle()

            End Sub

        End Class

#End Region

    End Class

End Namespace

#End Region
