' ***********************************************************************
' Author           : Elektro
' Last Modified On : 08-20-2014
' ***********************************************************************
' <copyright file="ListView Column-Sorter.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Usage Examples "

'Public Class ListViewColumnSorter_TestForm : Inherits form
'
'    ''' <summary>
'    ''' The listview to sort.
'    ''' </summary>
'    Private WithEvents LV As New ListView
'
'    ''' <summary>
'    ''' The 'ListViewColumnSorter' instance.
'    ''' </summary>
'    Private Sorter As New ListViewColumnSorter
'
'    ''' <summary>
'    ''' Initializes a new instance of the <see cref="ListViewColumnSorter_TestForm"/> class.
'    ''' </summary>
'    Public Sub New()
'
'        ' This call is required by the designer.
'        InitializeComponent()
'
'        With LV ' Set the Listview properties.
'
'            ' Set the sorter, our 'ListViewColumnSorter'.
'            .ListViewItemSorter = Sorter
'
'            ' The sorting default direction.
'            .Sorting = SortOrder.Ascending
'
'            ' Set the default sort-modifier.
'            Sorter.SortModifier = ListViewColumnSorter.SortModifiers.SortByText
'
'            ' Add some columns.
'            .Columns.Add("Text").Tag = ListViewColumnSorter.SortModifiers.SortByText
'            .Columns.Add("Numbers").Tag = ListViewColumnSorter.SortModifiers.SortByNumber
'            .Columns.Add("Dates").Tag = ListViewColumnSorter.SortModifiers.SortByDate
'
'            ' Adjust the column sizes.
'            For Each col As ColumnHeader In LV.Columns
'                col.Width = 100I
'            Next
'
'            ' Add some items.
'            .Items.Add("hello").SubItems.AddRange({"1", "11/11/2000"})
'            .Items.Add("yeehaa!").SubItems.AddRange({"2", "11-11-2000"})
'            .Items.Add("El3ktr0").SubItems.AddRange({"10", "9/9/1999"})
'            .Items.Add("wow").SubItems.AddRange({"100", "21/08/2014"})
'
'            ' Visual-Style things.
'            .Dock = DockStyle.Fill
'            .View = View.Details
'            .FullRowSelect = True
'
'        End With
'
'        With Me ' Set the Form properties.
'
'            .Size = New Size(400, 200)
'            .FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
'            .MaximizeBox = False
'            .StartPosition = FormStartPosition.CenterScreen
'            .Text = "ListViewColumnSorter TestForm"
'
'        End With
'
'        ' Add the Listview to UI.
'        Me.Controls.Add(LV)
'
'    End Sub
'
'    ''' <summary>
'    ''' Handles the 'ColumnClick' event of the 'ListView1' control.
'    ''' </summary>
'    Private Sub ListView1_ColumnClick(ByVal sender As Object, ByVal e As ColumnClickEventArgs) _
'    Handles LV.ColumnClick
'
'        ' Dinamycaly sets the sort-modifier to sort the column by text, number, or date.
'        Sorter.SortModifier = sender.columns(e.Column).tag
'
'        ' Determine whether clicked column is already the column that is being sorted.
'        If e.Column = Sorter.Column Then
'
'            ' Reverse the current sort direction for this column.
'            If Sorter.Order = SortOrder.Ascending Then
'                Sorter.Order = SortOrder.Descending
'
'            Else
'                Sorter.Order = SortOrder.Ascending
'
'            End If ' Sorter.Order
'
'        Else
'
'            ' Set the column number that is to be sorted, default to ascending.
'            Sorter.Column = e.Column
'            Sorter.Order = SortOrder.Ascending
'
'        End If ' e.Column
'
'        ' Perform the sort with these new sort options.
'        sender.Sort()
'
'    End Sub
'
'End Class

#End Region

#Region " Option Statements "

Option Explicit On
Option Strict Off
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel

#End Region

#Region " ListView Column-Sorter "

Namespace Tools

    ''' <summary>
    ''' Performs a sorting comparison.
    ''' </summary>
    Public NotInheritable Class ListViewColumnSorter : Implements IComparer

#Region " Objects "

        '''' <summary>
        '''' Indicates the comparer instance.
        '''' </summary>
        Private comparer As Object = New TextComparer

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        ''' </summary>
        Public Property Column As Integer
            Get
                Return Me.column1
            End Get
            Set(ByVal value As Integer)
                Me.column1 = value
            End Set
        End Property
        Private column1 As Integer = 0I

        ''' <summary>
        ''' Gets or sets the order of sorting to apply.
        ''' </summary>
        Public Property Order As SortOrder
            Get
                Return Me.order1
            End Get
            Set(ByVal value As SortOrder)
                Me.order1 = value
            End Set
        End Property
        Private order1 As SortOrder = SortOrder.None

        ''' <summary>
        ''' Gets or sets the sort modifier.
        ''' </summary>
        ''' <value>The sort modifier.</value>
        Public Property SortModifier As SortModifiers
            Get
                Return Me.sortModifier1
            End Get
            Set(ByVal value As SortModifiers)
                Me.sortModifier1 = value
            End Set
        End Property
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
            Equals = 0I

            ''' <summary>
            ''' 'X' is less than 'Y'.
            ''' </summary>
            Less = -1I

            ''' <summary>
            ''' 'X' is greater than 'Y'.
            ''' </summary>
            Greater = 1I

        End Enum

        ''' <summary>
        ''' Indicates a Sorting Modifier.
        ''' </summary>
        Public Enum SortModifiers As Integer

            ''' <summary>
            ''' Treats the values ​​as text.
            ''' </summary>
            SortByText = 0I

            ''' <summary>
            ''' Treats the values ​​as numbers.
            ''' </summary>
            SortByNumber = 1I

            ''' <summary>
            ''' Treats valuesthe values ​​as dates.
            ''' </summary>
            SortByDate = 2I

        End Enum

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ''' </summary>
        ''' <param name="x">The first object to compare.</param>
        ''' <param name="y">The second object to compare.</param>
        ''' <returns>
        ''' A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, 
        ''' 0: <paramref name="x"/> equals <paramref name="y"/>. 
        ''' Less than 0: <paramref name="x"/> is less than <paramref name="y"/>. 
        ''' Greater than 0: <paramref name="x"/> is greater than <paramref name="y"/>.
        ''' </returns>
        Private Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

            Dim compareResult As ComparerResult = ComparerResult.Equals
            Dim lVItemX, lVItemY As ListViewItem

            ' Cast the objects to be compared
            lVItemX = DirectCast(x, ListViewItem)
            lVItemY = DirectCast(y, ListViewItem)

            Dim strX As String = If(Not lVItemX.SubItems.Count <= Me.column1,
                                   lVItemX.SubItems(Me.column1).Text,
                                   Nothing)

            Dim strY As String = If(Not lVItemY.SubItems.Count <= Me.column1,
                                    lVItemY.SubItems(Me.column1).Text,
                                    Nothing)

            Dim listViewMain As ListView = lVItemX.ListView

            ' Calculate correct return value based on object comparison
            If listViewMain.Sorting <> SortOrder.Ascending AndAlso listViewMain.Sorting <> SortOrder.Descending Then

                ' Return '0' to indicate they are equal
                Return ComparerResult.Equals

            End If

            If Me.sortModifier1.Equals(SortModifiers.SortByText) Then

                ' Compare the two items
                If lVItemX.SubItems.Count <= Me.column1 AndAlso lVItemY.SubItems.Count <= Me.column1 Then
                    compareResult = Me.comparer.Compare(Nothing, Nothing)

                ElseIf lVItemX.SubItems.Count <= Me.column1 AndAlso lVItemY.SubItems.Count > Me.column1 Then
                    compareResult = Me.comparer.Compare(Nothing, strY)

                ElseIf lVItemX.SubItems.Count > Me.column1 AndAlso lVItemY.SubItems.Count <= Me.column1 Then
                    compareResult = Me.comparer.Compare(strX, Nothing)

                Else
                    compareResult = Me.comparer.Compare(strX, strY)

                End If

            Else ' Me._SortModifier IsNot 'SortByText'

                Select Case Me.sortModifier1

                    Case SortModifiers.SortByNumber
                        If Me.comparer.GetType <> GetType(NumericComparer) Then
                            Me.comparer = New NumericComparer
                        End If

                    Case SortModifiers.SortByDate
                        If Me.comparer.GetType <> GetType(DateComparer) Then
                            Me.comparer = New DateComparer
                        End If

                    Case Else
                        If Me.comparer.GetType <> GetType(TextComparer) Then
                            Me.comparer = New TextComparer
                        End If

                End Select

                compareResult = comparer.Compare(strX, strY)

            End If ' Me._SortModifier.Equals(...)

            ' Calculate correct return value based on object comparison
            If Me.order1 = SortOrder.Ascending Then
                ' Ascending sort is selected, return normal result of compare operation
                Return compareResult

            ElseIf Me.order1 = SortOrder.Descending Then
                ' Descending sort is selected, return negative result of compare operation
                Return (-compareResult)

            Else
                ' Return '0' to indicate they are equal
                Return 0I

            End If ' Me._Order = ...

        End Function

#End Region

#Region " Hidden Methods "

        ''' <summary>
        ''' Serves as a hash function for a particular type.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Sub GetHashCode()
        End Sub

        ''' <summary>
        ''' Determines whether the specified System.Object instances are considered equal.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Sub Equals()
        End Sub

        ''' <summary>
        ''' Gets the System.Type of the current instance.
        ''' </summary>
        ''' <returns>The exact runtime type of the current instance.</returns>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Function [GetType]()
            Return Me.GetType
        End Function

        ''' <summary>
        ''' Returns a String that represents the current object.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shadows Sub ToString()
        End Sub

#End Region


    End Class

End Namespace

#End Region

#Region " Comparers "

#Region " Text "

Namespace Tools

    ''' <summary>
    ''' Performs a text comparison.
    ''' </summary>
    Public NotInheritable Class TextComparer : Inherits CaseInsensitiveComparer

#Region " Enumerations "

        ''' <summary>
        ''' Specifies a comparison result.
        ''' </summary>
        Public Enum ComparerResult As Integer

            ''' <summary>
            ''' 'X' is equals to 'Y'.
            ''' </summary>
            Equals = 0I

            ''' <summary>
            ''' 'X' is less than 'Y'.
            ''' </summary>
            Less = -1I

            ''' <summary>
            ''' 'X' is greater than 'Y'.
            ''' </summary>
            Greater = 1I

        End Enum

#End Region

#Region " Methods "

        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ''' </summary>
        ''' <param name="x">The first object to compare.</param>
        ''' <param name="y">The second object to compare.</param>
        ''' <returns>
        ''' A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, 
        ''' 0: <paramref name="x"/> equals <paramref name="y"/>. 
        ''' Less than 0: <paramref name="x"/> is less than <paramref name="y"/>. 
        ''' Greater than 0: <paramref name="x"/> is greater than <paramref name="y"/>.
        ''' </returns>
        Friend Shadows Function Compare(ByVal x As Object, ByVal y As Object) As Integer

            ' Null parsing.
            If x Is Nothing AndAlso y Is Nothing Then
                Return ComparerResult.Equals ' X is equals to Y.

            ElseIf x Is Nothing AndAlso y IsNot Nothing Then
                Return ComparerResult.Less ' X is less than Y.

            ElseIf x IsNot Nothing AndAlso y Is Nothing Then
                Return ComparerResult.Greater ' X is greater than Y.

            End If

            ' String parsing:
            If (TypeOf x Is String) AndAlso (TypeOf y Is String) Then ' True and True
                Return [Enum].Parse(GetType(ComparerResult),
                                    MyBase.Compare(x, y))

            ElseIf (TypeOf x Is String) AndAlso Not (TypeOf y Is String) Then ' True and False
                Return ComparerResult.Greater ' X is greater than Y.

            ElseIf Not (TypeOf x Is String) AndAlso (TypeOf y Is String) Then ' False and True
                Return ComparerResult.Less ' X is less than Y.

            Else ' False and False
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
    ''' Performs a numeric comparison.
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
            Equals = 0I

            ''' <summary>
            ''' 'X' is less than 'Y'.
            ''' </summary>
            Less = -1I

            ''' <summary>
            ''' 'X' is greater than 'Y'.
            ''' </summary>
            Greater = 1I

        End Enum

#End Region

#Region " Methods "

        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ''' </summary>
        ''' <param name="x">The first object to compare.</param>
        ''' <param name="y">The second object to compare.</param>
        ''' <returns>
        ''' A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, 
        ''' 0: <paramref name="x"/> equals <paramref name="y"/>. 
        ''' Less than 0: <paramref name="x" /> is less than <paramref name="y"/>. 
        ''' Greater than 0: <paramref name="x"/> is greater than <paramref name="y"/>.
        ''' </returns>
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
        Implements IComparer.Compare

            ' Null parsing.
            If x Is Nothing AndAlso y Is Nothing Then
                Return ComparerResult.Equals ' X is equals to Y.

            ElseIf x Is Nothing AndAlso y IsNot Nothing Then
                Return ComparerResult.Less ' X is less than Y.

            ElseIf x IsNot Nothing AndAlso y Is Nothing Then
                Return ComparerResult.Greater ' X is greater than Y.

            End If

            ' The single variables to parse the text.
            Dim singleX, singleY As Single

            ' Single parsing:
            If Single.TryParse(x, singleX) AndAlso Single.TryParse(y, singleY) Then ' True and True
                Return [Enum].Parse(GetType(ComparerResult),
                                    singleX.CompareTo(singleY))

            ElseIf Single.TryParse(x, singleX) AndAlso Not Single.TryParse(y, singleY) Then ' True and False
                Return ComparerResult.Greater ' X is greater than Y.

            ElseIf Not Single.TryParse(x, singleX) AndAlso Single.TryParse(y, singleY) Then ' False and True
                Return ComparerResult.Less ' X is less than Y.

            Else ' False and False
                Return [Enum].Parse(GetType(ComparerResult),
                                    x.ToString.CompareTo(y.ToString))

            End If

        End Function

#End Region

    End Class

End Namespace

#End Region

#Region " Date "

Namespace Tools

    ''' <summary>
    ''' Performs a date comparison.
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
            Equals = 0I

            ''' <summary>
            ''' 'X' is less than 'Y'.
            ''' </summary>
            Less = -1I

            ''' <summary>
            ''' 'X' is greater than 'Y'.
            ''' </summary>
            Greater = 1I

        End Enum

#End Region

#Region " Methods "

        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        ''' </summary>
        ''' <param name="x">The first object to compare.</param>
        ''' <param name="y">The second object to compare.</param>
        ''' <returns>
        ''' A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, 
        ''' 0: <paramref name="x"/> equals <paramref name="y"/>. 
        ''' Less than 0: <paramref name="x"/> is less than <paramref name="y"/>. 
        ''' Greater than 0: <paramref name="x"/> is greater than <paramref name="y"/>.
        ''' </returns>
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare

            ' Null parsing.
            If x Is Nothing AndAlso y Is Nothing Then
                Return ComparerResult.Equals ' X is equals to Y.

            ElseIf x Is Nothing AndAlso y IsNot Nothing Then
                Return ComparerResult.Less ' X is less than Y.

            ElseIf x IsNot Nothing AndAlso y Is Nothing Then
                Return ComparerResult.Greater ' X is greater than Y.

            End If

            ' The Date variables to parse the text.
            Dim dateX, dateY As Date

            ' Date parsing:
            If Date.TryParse(x, dateX) AndAlso Date.TryParse(y, dateY) Then ' True and True
                Return [Enum].Parse(GetType(ComparerResult),
                                    dateX.CompareTo(dateY))

            ElseIf Date.TryParse(x, dateX) AndAlso Not Date.TryParse(y, dateY) Then ' True and False
                Return ComparerResult.Greater ' X is greater than Y.

            ElseIf Not Date.TryParse(x, dateX) AndAlso Date.TryParse(y, dateY) Then ' False and True
                Return ComparerResult.Less ' X is less than Y.

            Else ' False and False
                Return [Enum].Parse(GetType(ComparerResult),
                                    x.ToString.CompareTo(y.ToString))

            End If

        End Function

#End Region

    End Class

End Namespace

#End Region

#End Region