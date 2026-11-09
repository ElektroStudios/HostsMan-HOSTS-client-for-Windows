' ***********************************************************************
' Author   : Elektro
' Modified : 09-November-2026
' ***********************************************************************
' <copyright file="ComparerTests.vb" company="Elektro Studios">
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

Imports HostsMan.Tests.TestHelpers
Imports HostsMan.Tools

#End Region

#Region " ComparerTests "

Namespace Tools

    ''' <summary>
    ''' Tests for <see cref="TextComparer"/>, <see cref="NumericComparer"/> and <see cref="DateComparer"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class ComparerTests

        ''' <summary>
        ''' The culture scope used by every test, so the parsing of numbers and dates is deterministic.
        ''' </summary>
        Private cultureScope As CultureScope

        ''' <summary>
        ''' Sets the en-US culture before each test.
        ''' </summary>
        <TestInitialize>
        Public Sub TestInitialize()

            Me.cultureScope = New CultureScope("en-US")

        End Sub

        ''' <summary>
        ''' Restores the original culture after each test.
        ''' </summary>
        <TestCleanup>
        Public Sub TestCleanup()

            Me.cultureScope.Dispose()

        End Sub

        ''' <summary>
        ''' Gets the comparer instances under test.
        ''' </summary>
        Private Shared ReadOnly Property AllComparers As IComparer()
            Get
                Return {New TextComparer(), New NumericComparer(), New DateComparer()}
            End Get
        End Property

        <TestMethod>
        Public Sub AllComparers_NothingValues_SortBeforeAnyValue()

            For Each comparer As IComparer In ComparerTests.AllComparers
                Assert.AreEqual(0, comparer.Compare(Nothing, Nothing), comparer.GetType().Name)
                Assert.AreEqual(-1, comparer.Compare(Nothing, "1"), comparer.GetType().Name)
                Assert.AreEqual(1, comparer.Compare("1", Nothing), comparer.GetType().Name)
            Next comparer

        End Sub

        <TestMethod>
        Public Sub AllComparers_ReturnOnlyMinusOneZeroOrOne()

            Dim samples As String() = {"a", "Z", "10", "9", "1/1/2000", "12/31/1999", "", "zzzzzzzz"}

            For Each comparer As IComparer In ComparerTests.AllComparers
                For Each x As String In samples
                    For Each y As String In samples
                        Dim result As Integer = comparer.Compare(x, y)
                        Assert.IsTrue(result >= -1 AndAlso result <= 1, $"{comparer.GetType().Name}: '{x}' vs '{y}' = {result}")
                        Assert.AreEqual(-result, comparer.Compare(y, x), $"{comparer.GetType().Name} is not antisymmetric for '{x}' and '{y}'.")
                    Next y
                Next x
            Next comparer

        End Sub

        <TestMethod>
        Public Sub TextComparer_IsCaseInsensitive()

            Dim comparer As IComparer = New TextComparer()

            Assert.AreEqual(0, comparer.Compare("HostName", "hostname"))
            Assert.AreEqual(-1, comparer.Compare("apple", "Banana"))

        End Sub

        <TestMethod>
        Public Sub TextComparer_StringsSortAfterNonStrings()

            Dim comparer As IComparer = New TextComparer()

            Assert.AreEqual(1, comparer.Compare("text", 5))
            Assert.AreEqual(-1, comparer.Compare(5, "text"))
            Assert.AreEqual(0, comparer.Compare(5, 6))

        End Sub

        <TestMethod>
        Public Sub NumericComparer_ComparesNumbersAndPutsThemAfterText()

            Dim comparer As IComparer = New NumericComparer()

            Assert.AreEqual(1, comparer.Compare("10", "9"))
            Assert.AreEqual(0, comparer.Compare("1,000", "1000"))
            Assert.AreEqual(-1, comparer.Compare("-5", "2.5"))
            Assert.AreEqual(1, comparer.Compare("1", "abc"))
            Assert.AreEqual(-1, comparer.Compare("abc", "1"))
            Assert.AreEqual(-1, comparer.Compare("abc", "abd"))

        End Sub

        <TestMethod>
        Public Sub DateComparer_ComparesDatesAndPutsThemAfterText()

            Dim comparer As IComparer = New DateComparer()

            Assert.AreEqual(1, comparer.Compare("1/2/2014", "12/31/2013"))
            Assert.AreEqual(0, comparer.Compare("1/2/2014", "01/02/2014"))
            Assert.AreEqual(1, comparer.Compare("1/2/2014", "not a date"))
            Assert.AreEqual(-1, comparer.Compare("not a date", "1/2/2014"))

        End Sub

    End Class

End Namespace

#End Region
