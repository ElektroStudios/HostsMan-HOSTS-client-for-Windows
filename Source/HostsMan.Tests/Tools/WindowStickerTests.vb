' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="WindowStickerTests.vb" company="Elektro Studios">
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

#Region " WindowStickerTests "

Namespace Tools

    ''' <summary>
    ''' Tests for <see cref="WindowSticker"/> (without creating window handles).
    ''' </summary>
    <TestClass>
    Public NotInheritable Class WindowStickerTests

        <TestMethod>
        Public Sub Constructor_StoresTheClientFormAndTheDefaultSnapMargin()

            Using form As New Form()
                Using sticker As New WindowSticker(form)
                    Assert.AreSame(form, sticker.ClientForm)
                    Assert.AreEqual(30, sticker.SnapMargin)
                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub SnapMargin_SetValue_IsStored()

            Using form As New Form()
                Using sticker As New WindowSticker(form) With {.SnapMargin = 35}
                    Assert.AreEqual(35, sticker.SnapMargin)
                End Using
            End Using

        End Sub

        <TestMethod>
        Public Sub SnapMargin_SetAfterDispose_ThrowsObjectDisposedException()

            Using form As New Form()
                Dim sticker As New WindowSticker(form)
                sticker.Dispose()

                Assert.ThrowsExactly(Of ObjectDisposedException)(Sub() sticker.SnapMargin = 10)
                Assert.IsNull(sticker.ClientForm)
            End Using

        End Sub

    End Class

End Namespace

#End Region
