'---------------------------------------------------------------------
' <copyright file="LinqAnyAllTests.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports AstoriaUnitTests.Stubs
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Collections.Generic
Imports Microsoft.OData.Client
Imports System.Data.Test.Astoria
Imports System.Text


<TestClass()>
Public Class AnyAllFilterTests

    Private Shared Function _MakeArray(Of T)(ByVal ParamArray values() As T) As T()
        Return values
    End Function

    Private Shared Sub SendingRequestHandler(ByVal sender As Object, ByVal e As SendingRequest2EventArgs)
        Dim version = e.RequestMessage.GetHeader("OData-Version")
        Assert.IsTrue(version.Contains("4.0"))
    End Sub

#Region "collection model"
    Public Class MVComplexType
        Public Property Name As String
        Public Property Numbers As List(Of Integer)

    End Class

    Public Class EntityWithCollections
        Public Property ID As Integer
        Public Property CollectionOfInt As List(Of Integer)
        Public Property CollectionOfString As List(Of String)
        Public Property CollectionOfComplexType As List(Of MVComplexType)
    End Class
#End Region

    <TestCategory("Partition1")> <TestMethod()>
    Public Sub FilterCollectionWithAnyAll()

        Dim ctx = New DataServiceContext(New System.Uri("http://localhost"), ODataProtocolVersion.V4)
        'ctx.Format.UseAtom()

        Dim values = ctx.CreateQuery(Of EntityWithCollections)("Values")
        Dim testCases = _MakeArray(
            New With {
                .q = values.Where(Function(e) e.CollectionOfInt.Any()),
                .url = "Values?$filter=CollectionOfInt/any()"
            },
            New With {
                .q = values.Where(Function(e) e.CollectionOfInt.Any() And e.ID = 0),
                .url = "Values?$filter=CollectionOfInt/any() and ID eq 0"
            },
            New With {
                .q = values.Where(Function(e) e.CollectionOfInt.Any(Function(mv) mv = 2)),
                .url = "Values?$filter=CollectionOfInt/any(mv:mv eq 2)"
            },
            New With {
                .q = values.Where(Function(e) e.CollectionOfInt.Any(Function(mv) mv > e.ID) And e.ID < 100),
                .url = "Values?$filter=CollectionOfInt/any(mv:mv gt $it/ID) and ID lt 100"
            },
            New With {
                .q = values.Where(Function(e) e.CollectionOfComplexType.Any(Function(mv) e.CollectionOfString.All(Function(s) s.StartsWith(mv.Name)) Or e.ID < 100) And e.ID > 50),
                .url = "Values?$filter=CollectionOfComplexType/any(mv:$it/CollectionOfString/all(s:startswith(s,mv/Name)) or $it/ID lt 100) and ID gt 50"
            },
            New With {
                .q = values.Where(Function(e) e.CollectionOfComplexType.All(Function(mv) mv.Name.StartsWith("a") Or e.ID < 100) And e.ID > 50),
                .url = "Values?$filter=CollectionOfComplexType/all(mv:startswith(mv/Name,'a') or $it/ID lt 100) and ID gt 50"
            },
            New With {
                .q = values.Where(Function(e) e.CollectionOfComplexType.All(Function(mv) mv.Name.Contains("a") Or mv.Numbers.All(Function(n) n Mod 2 = 0)) And e.ID \ 5 = 3),
                .url = "Values?$filter=CollectionOfComplexType/all(mv:contains(mv/Name,'a') or mv/Numbers/all(n:n mod 2 eq 0)) and ID div 5 eq 3"
            })

        TestUtil.RunCombinations(testCases,
                                 Sub(testCase)
                                     Assert.AreEqual(ctx.BaseUri.AbsoluteUri & testCase.url, testCase.q.ToString(), "url == q.ToString()")
                                 End Sub)

    End Sub

#Region "Movie model"
    Public Class Movie
        Public Property ID As Integer
        Public Property Name As String
        Public Property ReleaseYear As Global.System.DateTimeOffset
        Public Property Director As Person
        Public Property Actors As List(Of Actor)
        Public Property Awards As List(Of Award)
    End Class

    Public Class Award
        Public Property ID As Integer
        Public Property Name As String
        Public Property AwardDate As Global.System.DateTimeOffset
        Public Property Movie As Movie
        Public Property Recepient As Person
    End Class

    Public Class Person
        Public Property ID As Integer
        Public Property FirstName As String
        Public Property LastName As String
        Public Property DateOfBirth As Global.System.DateTimeOffset
        Public Property DirectedMovies As List(Of Movie)
        Public Property Awards As List(Of Award)
    End Class

    Public Class Actor
        Inherits Person

        Public Property Movies As List(Of Movie)
    End Class

    Public Class MegaStar
        Inherits Actor

        Public Property MegaStartProp As String
    End Class
#End Region

    <TestCategory("Partition1")> <TestMethod()>
    Public Sub FilterNavigationWithAnyAllMovieModel()
        Dim ctx = New DataServiceContext(New System.Uri("http://localhost"), ODataProtocolVersion.V4)
        'ctx.Format.UseAtom()
        ctx.ResolveName = Function(type) "NS." & type.Name

        Dim movies = ctx.CreateQuery(Of Movie)("Movies")
        Dim testCases = _MakeArray(
            New With {
                .q = movies.Where(Function(m) m.Awards.Any()),
                .url = "Movies?$filter=Awards/any()"
            },
            New With {
                .q = movies.Where(Function(m) m.Awards.Any() And m.ID = 0),
                .url = "Movies?$filter=Awards/any() and ID eq 0"
            },
            New With {
                .q = movies.Where(Function(m) m.Awards.Any(Function(a) a.ID = 2)),
                .url = "Movies?$filter=Awards/any(a:a/ID eq 2)"
            },
            New With {
                .q = movies.Where(Function(m) m.Awards.Any(Function(a) a.ID = m.ID)),
                .url = "Movies?$filter=Awards/any(a:a/ID eq $it/ID)"
            },
            New With {
                .q = movies.Where(Function(m) m.Director.Awards.All(Function(a) a.Movie.Equals(m))),
                .url = "Movies?$filter=Director/Awards/all(a:a/Movie eq $it)"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.Any(Function(a) a.DirectedMovies.All(Function(dm) dm.Equals(m)))),
                .url = "Movies?$filter=Actors/any(a:a/DirectedMovies/all(dm:dm eq $it))"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.Any(Function(a) a.DirectedMovies.All(Function(dm) dm.Equals(m) And m.Awards.All(Function(aw) aw.Movie.Director.Equals(dm.Director))))),
                .url = "Movies?$filter=Actors/any(a:a/DirectedMovies/all(dm:dm eq $it and $it/Awards/all(aw:aw/Movie/Director eq dm/Director)))"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.Any(Function(a) TypeOf a Is MegaStar)),
                .url = "Movies?$filter=Actors/any(a:isof(a, 'NS.MegaStar'))"
            },
            New With {
                .q = movies.Where(Function(m) m.Awards.All(Function(aw) TypeOf aw.Movie.Director Is MegaStar)),
                .url = "Movies?$filter=Awards/all(aw:isof(aw/Movie/Director, 'NS.MegaStar'))"
            },
            New With {
                .q = movies.Where(Function(m) m.Awards.All(Function(aw) TypeOf m.Director Is MegaStar And Not (aw.Movie.Actors.Any(Function(a) TypeOf a Is MegaStar)))),
                .url = "Movies?$filter=Awards/all(aw:isof($it/Director, 'NS.MegaStar') and not aw/Movie/Actors/any(a:isof(a, 'NS.MegaStar')))"
            },
            New With {
                .q = movies.Where(Function(m) m.Awards.All(Function(aw) m.Director.FirstName.StartsWith("Hus") And Not (aw.Movie.Actors.Any(Function(a) TypeOf a Is MegaStar)))),
                .url = "Movies?$filter=Awards/all(aw:startswith($it/Director/FirstName,'Hus') and not aw/Movie/Actors/any(a:isof(a, 'NS.MegaStar')))"
            },
            New With {
                .q = movies.Where(Function(m) m.Awards.All(Function(aw) TypeOf m.Director Is MegaStar And CType(m.Director, MegaStar).MegaStartProp.StartsWith("Hus") And aw.Recepient.Equals(m.Director))),
                .url = "Movies?$filter=Awards/all(aw:isof($it/Director, 'NS.MegaStar') and startswith(cast($it/Director,'NS.MegaStar')/MegaStartProp,'Hus') and aw/Recepient eq $it/Director)"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.OfType(Of MegaStar).Any()),
                .url = "Movies?$filter=Actors/NS.MegaStar/any()"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.OfType(Of MegaStar).Any(Function(ms) ms.Awards.Any())),
                .url = "Movies?$filter=Actors/NS.MegaStar/any(ms:ms/Awards/any())"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.OfType(Of MegaStar).All(Function(ms) ms.Awards.Any())),
                .url = "Movies?$filter=Actors/NS.MegaStar/all(ms:ms/Awards/any())"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.All(Function(a) TypeOf a Is MegaStar And a.Awards.Any())),
                .url = "Movies?$filter=Actors/all(a:isof(a, 'NS.MegaStar') and a/Awards/any())"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.OfType(Of MegaStar).All(Function(ms) ms.Awards.Any(Function(a) a.AwardDate > New Global.System.DateTimeOffset() And ms.DirectedMovies.Any() And m.Director.Equals(ms)))),
                .url = "Movies?$filter=Actors/NS.MegaStar/all(ms:ms/Awards/any(a:a/AwardDate gt 0001-01-01T00:00:00Z and ms/DirectedMovies/any() and $it/Director eq ms))"
            },
            New With {
                .q = movies.Where(Function(m) m.Actors.OfType(Of MegaStar).All(Function(ms) ms.Awards.Any(Function(a) a.AwardDate > New Global.System.DateTimeOffset() And ms.DirectedMovies.Any(Function(dm) dm.Awards.All(Function(aw) aw.Recepient.FirstName.Equals(ms.FirstName))) And m.Director.Equals(ms)))),
                .url = "Movies?$filter=Actors/NS.MegaStar/all(ms:ms/Awards/any(a:a/AwardDate gt 0001-01-01T00:00:00Z and ms/DirectedMovies/any(dm:dm/Awards/all(aw:aw/Recepient/FirstName eq ms/FirstName)) and $it/Director eq ms))"
            },
            New With {
                .q = movies.Where(Function(m) m.Awards.Any(Function(aw) TypeOf aw.Recepient Is MegaStar And m.Actors.OfType(Of MegaStar).All(Function(a) a.DateOfBirth > New Global.System.DateTimeOffset(New Date(2010, 1, 1), New Global.System.TimeSpan(0))))),
                .url = "Movies?$filter=Awards/any(aw:isof(aw/Recepient, 'NS.MegaStar') and $it/Actors/NS.MegaStar/all(a:a/DateOfBirth gt 2010-01-01T00:00:00Z))"
            }
        )

        TestUtil.RunCombinations(testCases,
                                 Sub(testCase)
                                     Assert.AreEqual(ctx.BaseUri.AbsoluteUri & testCase.url, testCase.q.ToString(), "url == q.ToString()")
                                 End Sub)

    End Sub


End Class
