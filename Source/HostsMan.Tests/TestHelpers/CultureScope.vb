' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="CultureScope.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Globalization
Imports System.Threading

#End Region

#Region " CultureScope "

Namespace TestHelpers

    ''' <summary>
    ''' Temporarily changes the current culture of the calling thread, and restores it when disposed.
    ''' </summary>
    Friend NotInheritable Class CultureScope : Implements IDisposable

        ''' <summary>
        ''' The original culture.
        ''' </summary>
        Private ReadOnly originalCulture As CultureInfo

        ''' <summary>
        ''' The original UI culture.
        ''' </summary>
        Private ReadOnly originalUICulture As CultureInfo

        ''' <summary>
        ''' Initializes a new instance of the <see cref="CultureScope"/> class.
        ''' </summary>
        ''' <param name="cultureName">The name of the culture to apply (e.g. "en-US").</param>
        Friend Sub New(ByVal cultureName As String)

            Me.originalCulture = Thread.CurrentThread.CurrentCulture
            Me.originalUICulture = Thread.CurrentThread.CurrentUICulture

            Dim culture As CultureInfo = CultureInfo.GetCultureInfo(cultureName)
            Thread.CurrentThread.CurrentCulture = culture
            Thread.CurrentThread.CurrentUICulture = culture

        End Sub

        ''' <summary>
        ''' Restores the original culture.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            Thread.CurrentThread.CurrentCulture = Me.originalCulture
            Thread.CurrentThread.CurrentUICulture = Me.originalUICulture

        End Sub

    End Class

End Namespace

#End Region
