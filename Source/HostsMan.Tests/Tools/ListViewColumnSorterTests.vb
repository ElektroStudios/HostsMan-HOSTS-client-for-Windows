' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ListViewColumnSorterTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections
Imports System.ComponentModel

Imports HostsMan.Tests.TestHelpers
Imports HostsMan.Tools
Imports HostsMan.Tools.ListViewColumnSorter

#End Region

#Region " ListViewColumnSorterTests "

Namespace Tools

    ''' <summary>
    ''' Tests for <see cref="ListViewColumnSorter"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class ListViewColumnSorterTests

#Region " Helpers "

        ''' <summary>
        ''' Creates a detached list view item with the specified column texts.
        ''' </summary>
        ''' <param name="texts">The column texts.</param>
        ''' <returns>The list view item.</returns>
        Private Shared Function CreateItem(ParamArray texts As String()) As ListViewItem

            Return New ListViewItem(texts)

        End Function

        ''' <summary>
        ''' Sorts the specified column values with the specified sorter and returns them in the sorted order.
        ''' </summary>
        ''' <param name="sorter">The sorter.</param>
        ''' <param name="values">The values of the column 0.</param>
        ''' <returns>The sorted values.</returns>
        Private Shared Function SortColumn0(ByVal sorter As ListViewColumnSorter, ParamArray values As String()) As String()

            Dim items As ListViewItem() = values.Select(Function(value As String) ListViewColumnSorterTests.CreateItem(value)).ToArray()
            Array.Sort(items, DirectCast(sorter, IComparer))
            Return items.Select(Function(item As ListViewItem) item.Text).ToArray()

        End Function

#End Region

#Region " Properties "

        <TestMethod>
        Public Sub DefaultValues_AreColumn0OrderNoneAndSortByText()

            Dim sorter As New ListViewColumnSorter()

            Assert.AreEqual(0, sorter.Column)
            Assert.AreEqual(SortOrder.None, sorter.Order)
            Assert.AreEqual(SortModifiers.SortByText, sorter.SortModifier)

        End Sub

        <TestMethod>
        Public Sub Column_Negative_ThrowsArgumentOutOfRangeException()

            Dim sorter As New ListViewColumnSorter()

            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() sorter.Column = -1)

        End Sub

        <TestMethod>
        Public Sub Order_UndefinedValue_ThrowsInvalidEnumArgumentException()

            Dim sorter As New ListViewColumnSorter()

            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() sorter.Order = CType(42, SortOrder))

        End Sub

        <TestMethod>
        Public Sub SortModifier_UndefinedValue_ThrowsInvalidEnumArgumentException()

            Dim sorter As New ListViewColumnSorter()

            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Sub() sorter.SortModifier = CType(42, SortModifiers))

        End Sub

        <TestMethod>
        Public Sub GetTypeAndToString_DoNotRecurseInfinitely()

            ' Regression: the class shadowed GetType() with a method that called itself (StackOverflowException),
            ' and shadowed ToString() with a Sub that returned nothing.
            Dim sorter As New ListViewColumnSorter()

            Assert.AreEqual(GetType(ListViewColumnSorter), sorter.GetType())
            Assert.AreEqual(GetType(ListViewColumnSorter).FullName, sorter.ToString())
            Assert.IsTrue(sorter.Equals(sorter))

        End Sub

#End Region

#Region " Compare "

        <TestMethod>
        Public Sub Compare_OrderNone_ReturnsZero()

            Dim sorter As IComparer = New ListViewColumnSorter() With {.Order = SortOrder.None}

            Assert.AreEqual(0, sorter.Compare(ListViewColumnSorterTests.CreateItem("a"), ListViewColumnSorterTests.CreateItem("b")))

        End Sub

        <TestMethod>
        Public Sub Compare_OwnerListViewWithSortingNone_ReturnsZero()

            Using lv As New ListView() With {.Sorting = SortOrder.None}
                Dim itemA As ListViewItem = lv.Items.Add("a")
                Dim itemB As ListViewItem = lv.Items.Add("b")
                Dim sorter As IComparer = New ListViewColumnSorter() With {.Order = SortOrder.Ascending}

                Assert.AreEqual(0, sorter.Compare(itemA, itemB))
            End Using

        End Sub

        <TestMethod>
        Public Sub SortByText_AscendingAndDescending_IsCaseInsensitive()

            Using New CultureScope("en-US")
                Dim sorter As New ListViewColumnSorter() With {.Order = SortOrder.Ascending}
                CollectionAssert.AreEqual({"alpha", "Bravo", "charlie"}, ListViewColumnSorterTests.SortColumn0(sorter, "charlie", "alpha", "Bravo"))

                sorter.Order = SortOrder.Descending
                CollectionAssert.AreEqual({"charlie", "Bravo", "alpha"}, ListViewColumnSorterTests.SortColumn0(sorter, "charlie", "alpha", "Bravo"))
            End Using

        End Sub

        <TestMethod>
        Public Sub SortByNumber_SortsNumerically()

            Using New CultureScope("en-US")
                Dim sorter As New ListViewColumnSorter() With {.Order = SortOrder.Ascending, .SortModifier = SortModifiers.SortByNumber}

                CollectionAssert.AreEqual({"2", "9", "10", "100"}, ListViewColumnSorterTests.SortColumn0(sorter, "10", "9", "100", "2"))
            End Using

        End Sub

        <TestMethod>
        Public Sub SortByDate_SortsChronologically()

            Using New CultureScope("en-US")
                Dim sorter As New ListViewColumnSorter() With {.Order = SortOrder.Ascending, .SortModifier = SortModifiers.SortByDate}

                CollectionAssert.AreEqual({"9/9/1999", "11/11/2000", "1/2/2014"},
                                          ListViewColumnSorterTests.SortColumn0(sorter, "1/2/2014", "9/9/1999", "11/11/2000"))
            End Using

        End Sub

        <TestMethod>
        Public Sub SortByText_AfterSortByNumber_UsesTheTextComparer()

            ' Regression: once a numeric (or date) sort was performed, the text sort kept using the numeric comparer.
            Using New CultureScope("en-US")
                Dim sorter As New ListViewColumnSorter() With {.Order = SortOrder.Ascending, .SortModifier = SortModifiers.SortByNumber}
                ListViewColumnSorterTests.SortColumn0(sorter, "10", "9")

                sorter.SortModifier = SortModifiers.SortByText

                CollectionAssert.AreEqual({"10", "9"}, ListViewColumnSorterTests.SortColumn0(sorter, "9", "10"))
            End Using

        End Sub

        <TestMethod>
        Public Sub Compare_UsesTheConfiguredColumn()

            Dim sorter As IComparer = New ListViewColumnSorter() With {.Order = SortOrder.Ascending, .Column = 2}

            Dim result As Integer = sorter.Compare(ListViewColumnSorterTests.CreateItem("a", "x", "zzz"),
                                                   ListViewColumnSorterTests.CreateItem("b", "x", "aaa"))

            Assert.AreEqual(1, result)

        End Sub

        <TestMethod>
        Public Sub Compare_ItemWithoutTheColumn_SortsFirst()

            Dim sorter As IComparer = New ListViewColumnSorter() With {.Order = SortOrder.Ascending, .Column = 1}

            Dim result As Integer = sorter.Compare(ListViewColumnSorterTests.CreateItem("a"),
                                                   ListViewColumnSorterTests.CreateItem("b", "value"))

            Assert.AreEqual(-1, result)

        End Sub

        <TestMethod>
        Public Sub Compare_Nothing_ThrowsArgumentNullException()

            Dim sorter As IComparer = New ListViewColumnSorter() With {.Order = SortOrder.Ascending}
            Dim item As ListViewItem = ListViewColumnSorterTests.CreateItem("a")

            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() sorter.Compare(Nothing, item))
            Assert.ThrowsExactly(Of ArgumentNullException)(Sub() sorter.Compare(item, Nothing))

        End Sub

        <TestMethod>
        Public Sub Compare_NotAListViewItem_ThrowsInvalidCastException()

            Dim sorter As IComparer = New ListViewColumnSorter() With {.Order = SortOrder.Ascending}

            Assert.ThrowsExactly(Of InvalidCastException)(Sub() sorter.Compare("a", "b"))

        End Sub

#End Region

    End Class

End Namespace

#End Region
