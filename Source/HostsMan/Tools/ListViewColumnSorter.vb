' ***********************************************************************
' Author           : Elektro
' Last Modified On : 09-November-2026
' ***********************************************************************
' <copyright file="ListView Column-Sorter.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Usage Examples "

'Public Class ListViewColumnSorter_TestForm : Inherits Form
'
'    Private WithEvents lv As New ListView
'    Private ReadOnly sorter As New ListViewColumnSorter
'
'    Public Sub New()
'
'        With Me.lv
'            .ListViewItemSorter = Me.sorter
'            .Sorting = SortOrder.Ascending
'            .Columns.Add("Text").Tag = ListViewColumnSorter.SortModifiers.SortByText
'            .Columns.Add("Numbers").Tag = ListViewColumnSorter.SortModifiers.SortByNumber
'            .Columns.Add("Dates").Tag = ListViewColumnSorter.SortModifiers.SortByDate
'            .Items.Add("hello").SubItems.AddRange({"1", "11/11/2000"})
'            .Items.Add("El3ktr0").SubItems.AddRange({"10", "9/9/1999"})
'            .Dock = DockStyle.Fill
'            .View = View.Details
'        End With
'
'        Me.Controls.Add(Me.lv)
'
'    End Sub
'
'    Private Sub Lv_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles lv.ColumnClick
'
'        Me.sorter.SortModifier = DirectCast(Me.lv.Columns(e.Column).Tag, ListViewColumnSorter.SortModifiers)
'
'        If e.Column = Me.sorter.Column Then
'            Me.sorter.Order = If(Me.sorter.Order = SortOrder.Ascending, SortOrder.Descending, SortOrder.Ascending)
'        Else
'            Me.sorter.Column = e.Column
'            Me.sorter.Order = SortOrder.Ascending
'        End If
'
'        Me.lv.Sort()
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

Imports System.ComponentModel
Imports System.Globalization

#End Region

#Region " ListView Column-Sorter "

Namespace Tools

    ''' <summary>
    ''' Performs a sorting comparison of the items of a <see cref="ListView"/> by the text of one of its columns.
    ''' </summary>
    Public NotInheritable Class ListViewColumnSorter : Implements IComparer

#Region " Objects "

        ''' <summary>
        ''' The text comparer instance.
        ''' </summary>
        Private ReadOnly comparerText As New TextComparer()

        ''' <summary>
        ''' The numeric comparer instance.
        ''' </summary>
        Private ReadOnly comparerNumeric As New NumericComparer()

        ''' <summary>
        ''' The date comparer instance.
        ''' </summary>
        Private ReadOnly comparerDate As New DateComparer()

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the zero-based index of the column to which to apply the sorting operation (Defaults to '0').
        ''' </summary>
        ''' <value>The zero-based column index.</value>
        ''' <exception cref="ArgumentOutOfRangeException">The value is negative.</exception>
        Public Property Column As Integer
            Get
                Return Me.column1
            End Get
            Set(ByVal value As Integer)
                If value < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), value, "The column index cannot be negative.")
                End If
                Me.column1 = value
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="Column"/> property. )
        ''' </summary>
        Private column1 As Integer = 0

        ''' <summary>
        ''' Gets or sets the order of sorting to apply (Defaults to <see cref="SortOrder.None"/>).
        ''' </summary>
        ''' <value>The order of sorting.</value>
        ''' <exception cref="InvalidEnumArgumentException">The value is not a defined <see cref="SortOrder"/>.</exception>
        Public Property Order As SortOrder
            Get
                Return Me.order1
            End Get
            Set(ByVal value As SortOrder)
                If Not [Enum].IsDefined(GetType(SortOrder), value) Then
                    Throw New InvalidEnumArgumentException(NameOf(value), CInt(value), GetType(SortOrder))
                End If
                Me.order1 = value
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="Order"/> property. )
        ''' </summary>
        Private order1 As SortOrder = SortOrder.None

        ''' <summary>
        ''' Gets or sets the sort modifier (Defaults to <see cref="SortModifiers.SortByText"/>).
        ''' </summary>
        ''' <value>The sort modifier.</value>
        ''' <exception cref="InvalidEnumArgumentException">The value is not a defined <see cref="SortModifiers"/>.</exception>
        Public Property SortModifier As SortModifiers
            Get
                Return Me.sortModifier1
            End Get
            Set(ByVal value As SortModifiers)
                If Not [Enum].IsDefined(GetType(SortModifiers), value) Then
                    Throw New InvalidEnumArgumentException(NameOf(value), CInt(value), GetType(SortModifiers))
                End If
                Me.sortModifier1 = value
            End Set
        End Property
        ''' <summary>
        ''' ( Backing field of <see cref="SortModifier"/> property. )
        ''' </summary>
        Private sortModifier1 As SortModifiers = SortModifiers.SortByText

#End Region

#Region " Enumerations "

        ''' <summary>
        ''' Specifies a comparison result.
        ''' </summary>
        Public Enum ComparerResult As Integer

            ''' <summary>
            ''' 'X' is equals to 'Y'.
            ''' </summary>
            Equals = 0

            ''' <summary>
            ''' 'X' is less than 'Y'.
            ''' </summary>
            Less = -1

            ''' <summary>
            ''' 'X' is greater than 'Y'.
            ''' </summary>
            Greater = 1

        End Enum

        ''' <summary>
        ''' Indicates a Sorting Modifier.
        ''' </summary>
        Public Enum SortModifiers As Integer

            ''' <summary>
            ''' Treats the values as text.
            ''' </summary>
            SortByText = 0

            ''' <summary>
            ''' Treats the values as numbers.
            ''' </summary>
            SortByNumber = 1

            ''' <summary>
            ''' Treats the values as dates.
            ''' </summary>
            SortByDate = 2

        End Enum

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Compares two <see cref="ListViewItem"/> and returns a value indicating whether one is less than,
        ''' equal to, or greater than the other, according to <see cref="Column"/>, <see cref="Order"/> and <see cref="SortModifier"/>.
        ''' </summary>
        ''' <param name="x">The first <see cref="ListViewItem"/> to compare.</param>
        ''' <param name="y">The second <see cref="ListViewItem"/> to compare.</param>
        ''' <returns>
        ''' Less than zero if <paramref name="x"/> sorts before <paramref name="y"/>;
        ''' zero if they are equal;
        ''' greater than zero if <paramref name="x"/> sorts after <paramref name="y"/>.
        ''' </returns>
        ''' <exception cref="ArgumentNullException"><paramref name="x"/> or <paramref name="y"/> is <see langword="Nothing"/>.</exception>
        ''' <exception cref="InvalidCastException"><paramref name="x"/> or <paramref name="y"/> is not a <see cref="ListViewItem"/>.</exception>
        Private Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

            If x Is Nothing Then
                Throw New ArgumentNullException(NameOf(x))
            End If

            If y Is Nothing Then
                Throw New ArgumentNullException(NameOf(y))
            End If

            Dim itemX As ListViewItem = DirectCast(x, ListViewItem)
            Dim itemY As ListViewItem = DirectCast(y, ListViewItem)

            ' A ListView with sorting disabled keeps the insertion order.
            Dim owner As ListView = itemX.ListView
            If (owner IsNot Nothing AndAlso owner.Sorting = SortOrder.None) OrElse Me.order1 = SortOrder.None Then
                Return ComparerResult.Equals
            End If

            Dim textX As String = ListViewColumnSorter.GetColumnText(itemX, Me.column1)
            Dim textY As String = ListViewColumnSorter.GetColumnText(itemY, Me.column1)
            Dim result As Integer = Math.Sign(Me.GetComparer().Compare(textX, textY))

            Return If(Me.order1 = SortOrder.Descending, -result, result)

        End Function

        ''' <summary>
        ''' Gets the comparer that corresponds to the current <see cref="SortModifier"/>.
        ''' </summary>
        ''' <returns>The comparer.</returns>
        Private Function GetComparer() As IComparer

            Select Case Me.sortModifier1
                Case SortModifiers.SortByNumber
                    Return Me.comparerNumeric
                Case SortModifiers.SortByDate
                    Return Me.comparerDate
                Case Else
                    Return Me.comparerText
            End Select

        End Function

        ''' <summary>
        ''' Gets the text of the specified column of a <see cref="ListViewItem"/>.
        ''' </summary>
        ''' <param name="item">The item.</param>
        ''' <param name="column">The zero-based column index.</param>
        ''' <returns>The text of the column, or <see langword="Nothing"/> if the item has no sub-item for that column.</returns>
        Private Shared Function GetColumnText(ByVal item As ListViewItem, ByVal column As Integer) As String

            Return If(column < item.SubItems.Count, item.SubItems(column).Text, Nothing)

        End Function

#End Region

    End Class

End Namespace

#End Region

#Region " Comparers "

#Region " Text "

Namespace Tools

    ''' <summary>
    ''' Performs a case-insensitive text comparison, using the current culture.
    ''' </summary>
    Public NotInheritable Class TextComparer : Inherits CaseInsensitiveComparer : Implements IComparer

#Region " Enumerations "

        ''' <summary>
        ''' Specifies a comparison result.
        ''' </summary>
        Public Enum ComparerResult As Integer

            ''' <summary>
            ''' 'X' is equals to 'Y'.
            ''' </summary>
            Equals = 0

            ''' <summary>
            ''' 'X' is less than 'Y'.
            ''' </summary>
            Less = -1

            ''' <summary>
            ''' 'X' is greater than 'Y'.
            ''' </summary>
            Greater = 1

        End Enum

#End Region

#Region " Methods "

        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ''' <para></para>
        ''' <see langword="Nothing"/> sorts before any value, and strings sort after non-string values.
        ''' </summary>
        ''' <param name="x">The first object to compare.</param>
        ''' <param name="y">The second object to compare.</param>
        ''' <returns>A <see cref="ComparerResult"/> value (-1, 0 or 1).</returns>
        Public Shadows Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

            Dim nullResult As Integer
            If ComparerHelper.TryCompareNulls(x, y, nullResult) Then
                Return nullResult
            End If

            Dim isStringX As Boolean = TypeOf x Is String
            Dim isStringY As Boolean = TypeOf y Is String

            If isStringX AndAlso isStringY Then
                Return Math.Sign(MyBase.Compare(x, y))

            ElseIf isStringX Then
                Return ComparerResult.Greater

            ElseIf isStringY Then
                Return ComparerResult.Less

            Else
                Return ComparerResult.Equals

            End If

        End Function

#End Region

    End Class

End Namespace

#End Region

#Region " Numeric "

Namespace Tools

    ''' <summary>
    ''' Performs a numeric comparison, using the current culture to parse the values.
    ''' </summary>
    Public NotInheritable Class NumericComparer : Implements IComparer

#Region " Enumerations "

        ''' <summary>
        ''' Specifies a comparison result.
        ''' </summary>
        Public Enum ComparerResult As Integer

            ''' <summary>
            ''' 'X' is equals to 'Y'.
            ''' </summary>
            Equals = 0

            ''' <summary>
            ''' 'X' is less than 'Y'.
            ''' </summary>
            Less = -1

            ''' <summary>
            ''' 'X' is greater than 'Y'.
            ''' </summary>
            Greater = 1

        End Enum

#End Region

#Region " Methods "

        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ''' <para></para>
        ''' <see langword="Nothing"/> sorts before any value, numbers sort after non-numeric values,
        ''' and two non-numeric values are compared as text.
        ''' </summary>
        ''' <param name="x">The first object to compare.</param>
        ''' <param name="y">The second object to compare.</param>
        ''' <returns>A <see cref="ComparerResult"/> value (-1, 0 or 1).</returns>
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

            Dim nullResult As Integer
            If ComparerHelper.TryCompareNulls(x, y, nullResult) Then
                Return nullResult
            End If

            Dim textX As String = Convert.ToString(x, CultureInfo.CurrentCulture)
            Dim textY As String = Convert.ToString(y, CultureInfo.CurrentCulture)
            Dim numberX As Double
            Dim numberY As Double
            Dim isNumberX As Boolean = Double.TryParse(textX, NumberStyles.Float Or NumberStyles.AllowThousands, CultureInfo.CurrentCulture, numberX)
            Dim isNumberY As Boolean = Double.TryParse(textY, NumberStyles.Float Or NumberStyles.AllowThousands, CultureInfo.CurrentCulture, numberY)

            If isNumberX AndAlso isNumberY Then
                Return Math.Sign(numberX.CompareTo(numberY))

            ElseIf isNumberX Then
                Return ComparerResult.Greater

            ElseIf isNumberY Then
                Return ComparerResult.Less

            Else
                Return Math.Sign(String.Compare(textX, textY, StringComparison.CurrentCulture))

            End If

        End Function

#End Region

    End Class

End Namespace

#End Region

#Region " Date "

Namespace Tools

    ''' <summary>
    ''' Performs a date comparison, using the current culture to parse the values.
    ''' </summary>
    Public NotInheritable Class DateComparer : Implements IComparer

#Region " Enumerations "

        ''' <summary>
        ''' Specifies a comparison result.
        ''' </summary>
        Public Enum ComparerResult As Integer

            ''' <summary>
            ''' 'X' is equals to 'Y'.
            ''' </summary>
            Equals = 0

            ''' <summary>
            ''' 'X' is less than 'Y'.
            ''' </summary>
            Less = -1

            ''' <summary>
            ''' 'X' is greater than 'Y'.
            ''' </summary>
            Greater = 1

        End Enum

#End Region

#Region " Methods "

        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ''' <para></para>
        ''' <see langword="Nothing"/> sorts before any value, dates sort after non-date values,
        ''' and two non-date values are compared as text.
        ''' </summary>
        ''' <param name="x">The first object to compare.</param>
        ''' <param name="y">The second object to compare.</param>
        ''' <returns>A <see cref="ComparerResult"/> value (-1, 0 or 1).</returns>
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

            Dim nullResult As Integer
            If ComparerHelper.TryCompareNulls(x, y, nullResult) Then
                Return nullResult
            End If

            Dim textX As String = Convert.ToString(x, CultureInfo.CurrentCulture)
            Dim textY As String = Convert.ToString(y, CultureInfo.CurrentCulture)
            Dim dateX As Date
            Dim dateY As Date
            Dim isDateX As Boolean = Date.TryParse(textX, CultureInfo.CurrentCulture, DateTimeStyles.None, dateX)
            Dim isDateY As Boolean = Date.TryParse(textY, CultureInfo.CurrentCulture, DateTimeStyles.None, dateY)

            If isDateX AndAlso isDateY Then
                Return Math.Sign(dateX.CompareTo(dateY))

            ElseIf isDateX Then
                Return ComparerResult.Greater

            ElseIf isDateY Then
                Return ComparerResult.Less

            Else
                Return Math.Sign(String.Compare(textX, textY, StringComparison.CurrentCulture))

            End If

        End Function

#End Region

    End Class

End Namespace

#End Region

#Region " Helper "

Namespace Tools

    ''' <summary>
    ''' Provides shared logic for the comparers.
    ''' </summary>
    Friend NotInheritable Class ComparerHelper

        ''' <summary>
        ''' Prevents a default instance of the <see cref="ComparerHelper"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Compares two objects when at least one of them is <see langword="Nothing"/>.
        ''' <see langword="Nothing"/> sorts before any value.
        ''' </summary>
        ''' <param name="x">The first object.</param>
        ''' <param name="y">The second object.</param>
        ''' <param name="result">When this method returns <see langword="True"/>, contains the comparison result (-1, 0 or 1).</param>
        ''' <returns><see langword="True"/> if at least one of the objects is <see langword="Nothing"/>; otherwise, <see langword="False"/>.</returns>
        Friend Shared Function TryCompareNulls(ByVal x As Object, ByVal y As Object, ByRef result As Integer) As Boolean

            If x Is Nothing AndAlso y Is Nothing Then
                result = 0
                Return True

            ElseIf x Is Nothing Then
                result = -1
                Return True

            ElseIf y Is Nothing Then
                result = 1
                Return True

            End If

            result = 0
            Return False

        End Function

    End Class

End Namespace

#End Region

#End Region
