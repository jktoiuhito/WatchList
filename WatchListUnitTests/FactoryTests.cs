using WatchList;
using System;
using Xunit;

namespace WatchListUnitTests
{
    public class FactoryTests
    {
        //MVC components

        public class ControllerTests
        {
            //Default values

            [Fact]
            public void Factory_Controller_IsNotNull ()
            {
                var controller = Factory.Controller;

                Assert.NotNull(controller);
            }

            [Fact]
            public void Factory_Controller_DefaultWantsExitIsFalse ()
            {
                var controller = Factory.Controller;

                Assert.False(controller.WantsExit);
            }

            [Fact]
            public void Factory_Controller_DefaultCurrentlyWatchingIsNull ()
            {
                var controller = Factory.Controller;

                var currently = controller.CurrentlyWatching;

                Assert.Null(currently);
            }

            [Fact]
            public void Factory_Controller_DefaultInBacklogIsFalse ()
            {
                var controller = Factory.Controller;

                var inBacklog = controller.InBacklog;

                Assert.False(inBacklog);
            }

            [Fact]
            public void Factory_Controller_DefaultOpenStreamIsNull ()
            {
                var controller = Factory.Controller;

                var openStream = controller.OpenStream;

                Assert.Null(openStream);
            }

            //WithWantsExit

            [Fact]
            public void Factory_Controller_WithWantsExit_SetsWantsExit ()
            {
                var controller = Factory.Controller;
                var wantsExit = true;

                var exitingController = controller.WithWantsExit(wantsExit);

                Assert.True(exitingController.WantsExit);
            }

            //WithCurrentlyWatching

            [Fact]
            public void
                Factory_Controller_WithCurrentlyWatching_SetsCurrentlyWatching ()
            {
                var controller = Factory.Controller;
                var series = Factory.Series(Factory.Name("name"));

                var watchingController =
                    controller.WithCurrentlyWatching(series);

                Assert.Equal(series, watchingController.CurrentlyWatching);

                //Can be nulled

                ISeriesTuple series2 = null;

                var notWatchingController =
                    watchingController.WithCurrentlyWatching(series2);

                Assert.Null(notWatchingController.CurrentlyWatching);
            }

            [Fact]
            public void Factory_Controller_WithInBacklog_SetsInBacklog ()
            {
                var controller = Factory.Controller;
                var inBacklog = true;

                var backlogController = controller.WithInBacklog(inBacklog);

                Assert.Equal(inBacklog, backlogController.InBacklog);
            }

            //WithOpenStream

            [Fact]
            public void
                Factory_Controller_WithOpenStream_SetsOpenStream ()
            {
                var controller = Factory.Controller;
                var uri = new Uri("https://www.example.com/");

                var watchingController = controller.WithOpenStream(uri);

                Assert.Equal(uri, watchingController.OpenStream);

                //Can be nulled

                Uri uri2 = null;

                var notWatchingController =
                    watchingController.WithOpenStream(uri2);

                Assert.Null(notWatchingController.OpenStream);
            }
        }

        public class ViewTests
        {
            //Default values

            [Fact]
            public void Factory_View_IsNotNull ()
            {
                var view = Factory.View;

                Assert.NotNull(view);
            }

            [Fact]
            public void Factory_View_DefaultTitleIs_WatchList_ ()
            {
                var view = Factory.View;
                var expectedTitle = "WatchList";

                Assert.Equal(expectedTitle, view.Title);
            }

            [Fact]
            public void Factory_View_DefaultCurrentlyWatchingIsNull ()
            {
                var view = Factory.View;
                var currently = view.CurrentlyWatching;

                Assert.Null(currently);
            }

            [Fact]
            public void Factory_View_DefaultMessageIsNull ()
            {
                var view = Factory.View;

                var message = view.Message;

                Assert.Null(message);
            }

            //WithTitle

            [Fact]
            public void Factory_View_WithTitle_ThrowsExceptionOnNullInput ()
            {
                string title = null;

                Assert.Throws<ArgumentNullException>(
                    () => Factory.View.WithTitle(title));
            }

            [Fact]
            public void Factory_View_WithTitle_ThrowsExceptionOnEmptyInput ()
            {
                var title = "";

                Assert.Throws<ArgumentException>(
                    () => Factory.View.WithTitle(title));
            }

            [Fact]
            public void Factory_View_WithTitle_ThrowsExceptionOnWhitespaceInput ()
            {
                var title = "  ";

                Assert.Throws<ArgumentException>(
                    () => Factory.View.WithTitle(title));
            }

            [Fact]
            public void Factory_View_WithTitle_SetsTitle ()
            {
                var title = "Title";

                var view = Factory.View.WithTitle(title);

                Assert.Equal(title, view.Title);
            }

            //WithCurrentlyWatching

            [Fact]
            public void
                Factory_View_WithCurrentlyWatching_SetsCurrentlyWatching ()
            {
                var view = Factory.View;
                var series = Factory.Series(Factory.Name("name"));

                var watchingView = view.WithCurrentlyWatching(series);

                Assert.Equal(series, watchingView.CurrentlyWatching);

                //Can be nulled

                ISeriesTuple series2 = null;

                var notWatchingView =
                    watchingView.WithCurrentlyWatching(series2);

                Assert.Null(notWatchingView.CurrentlyWatching);
            }

            //WithMessage
            [Fact]
            public void Factory_View_WithMessage_SetsMessage ()
            {
                var view = Factory.View;
                var message1 = "message";
                string message2 = null;

                var messageView1 = view.WithMessage(message1);
                var messageView2 = messageView1.WithMessage(message2);

                Assert.Equal(message1, messageView1.Message);
                Assert.Equal(message2, messageView2.Message);
            }
        }

        public class ModelTests
        {
            [Fact]
            public void Factory_Model_IsNotNull ()
            {
                var model = Factory.Model;

                Assert.NotNull(model);
            }

            [Fact]
            public void Factory_Model_ListIsNotNull ()
            {
                var model = Factory.Model;
                var list = model.List;

                Assert.NotNull(list);
            }

            [Fact]
            public void Factory_Model_BacklogIsNotNull ()
            {
                var model = Factory.Model;
                var backlog = model.Backlog;

                Assert.NotNull(backlog);
            }

            [Fact]
            public void Factory_Model_AliasesIsNotNull ()
            {
                var model = Factory.Model;
                var aliases = model.Aliases;

                Assert.NotNull(aliases);
            }

            [Fact]
            public void Factory_Model_ListIsEmpty ()
            {
                var model = Factory.Model;
                var list = model.List;

                Assert.Empty(list);
            }

            [Fact]
            public void Factory_Model_BacklogIsEmpty ()
            {
                var model = Factory.Model;
                var backlog = model.Backlog;

                Assert.Empty(backlog);
            }

            [Fact]
            public void Factory_Model_AliasesIsEmpty ()
            {
                var model = Factory.Model;
                var aliases = model.Aliases;

                Assert.Empty(aliases);
            }

            //WithList

            [Fact]
            public void Factory_Model_WithList_ThrowsExceptionOnNullInput ()
            {
                var model = Factory.Model;
                IList list = null;

                Assert.Throws<ArgumentNullException>(
                    () => model.WithList(list));
            }

            [Fact]
            public void Factory_Model_WithList_SetsList ()
            {
                var model = Factory.Model;
                var list = Factory.List;

                var modelWithList = model.WithList(list);

                Assert.Equal(list, modelWithList.List);
            }

            //WithBacklog

            [Fact]
            public void Factory_Model_WithBacklog_ThrowsExceptionOnNullInput ()
            {
                var model = Factory.Model;
                IBacklog backlog = null;

                Assert.Throws<ArgumentNullException>(
                    () => model.WithBacklog(backlog));
            }

            [Fact]
            public void Factory_Model_WithBacklog_SetsBacklog ()
            {
                var model = Factory.Model;
                var backlog = Factory.Backlog;

                var modelWithBacklog = model.WithBacklog(backlog);

                Assert.Equal(backlog, modelWithBacklog.Backlog);
            }

            //WithAliases

            [Fact]
            public void Factory_Model_WithAliases_ThrowsExceptionOnNullInput ()
            {
                var model = Factory.Model;
                IAliases aliases = null;

                Assert.Throws<ArgumentNullException>(
                    () => model.WithAliases(aliases));
            }

            [Fact]
            public void Factory_Model_WithAliases_SetsAliases ()
            {
                var model = Factory.Model;
                var aliases = Factory.Aliases;

                var modelWithAliases = model.WithAliases(aliases);

                Assert.Equal(aliases, modelWithAliases.Aliases);
            }
        }

        //Tables

        public class AliasesTests
        {
            [Fact]
            public void Factory_Aliases_IsNotNull ()
            {
                var aliases = Factory.Aliases;

                Assert.NotNull(aliases);
            }

            [Fact]
            public void Factory_Aliases_IsEmpty ()
            {
                var aliases = Factory.Aliases;

                Assert.Empty(aliases);
            }

            //Add

            [Fact]
            public void Factory_Aliases_Add_NullAliasThrowsException ()
            {
                var aliases = Factory.Aliases;
                IAliasTuple alias = null;

                Assert.Throws<ArgumentNullException>(
                    () => aliases.Add(alias));
            }

            [Fact]
            public void Factory_Aliases_Add_AddsAlias ()
            {
                var aliases = Factory.Aliases;
                var alias = Factory.AliasTuple(
                    Factory.Name("alias"), Factory.Name("name"));

                var addedAliases = aliases.Add(alias);

                Assert.Contains(alias, addedAliases);
                Assert.Single(addedAliases);
            }
            //This also proves that IEnumerable works.

            //GetMainName

            [Fact]
            public void Factory_Aliases_GetMainName_ThrowsExceptionOnNullInput ()
            {
                var aliases = Factory.Aliases;

                IName name = null;

                Assert.Throws<ArgumentNullException>(
                    () => aliases.GetMainName(name));
            }

            [Fact]
            public void Factory_Aliases_GetMainName_ReturnsNullWhenEmpty ()
            {
                var aliases = Factory.Aliases;
                var name = Factory.Name("swln");

                var realName = aliases.GetMainName(name);

                Assert.Null(realName);
            }

            [Fact]
            public void Factory_Aliases_GetMainName_ReturnsNullOnNotContained ()
            {
                var aliases = Factory.Aliases.Add(
                    Factory.AliasTuple(
                        Factory.Name("alias"),
                        Factory.Name("name")));
                var name = Factory.Name("swln");

                var realName = aliases.GetMainName(name);

                Assert.Null(realName);
            }

            [Fact]
            public void Factory_Aliases_GetMainName_ReturnsContainedName ()
            {
                var name = Factory.Name("Series With a Long Name");
                var alias = Factory.Name("swln");
                var aliases = Factory.Aliases.Add(
                    Factory.AliasTuple(alias, name));
                
                var realName = aliases.GetMainName(alias);

                Assert.Equal(name, realName);
            }
        }

        public class BacklogTests
        {
            //Default values

            [Fact]
            public void Factory_Backlog_IsNotNull ()
            {
                var backlog = Factory.Backlog;

                Assert.NotNull(backlog);
            }

            [Fact]
            public void Factory_Backlog_IsEmpty ()
            {
                var backlog = Factory.Backlog;

                Assert.Empty(backlog);
            }
            //This also proves IEnumerable works.

            //Add

            [Fact]
            public void Factory_Backlog_Add_ReturnsItselfOnNullInput ()
            {
                var backlog = Factory.Backlog;
                ISeriesTuple series = null;

                var backlogWithSeries = backlog.Add(series);

                Assert.Equal(backlog, backlogWithSeries);
            }

            [Fact]
            public void Factory_Backlog_Add_AddsSeries ()
            {
                var backlog = Factory.Backlog;
                var series = Factory.Series(Factory.Name("name"));

                var backlogWithSeries = backlog.Add(series);

                Assert.Contains(series, backlogWithSeries);
            }

            [Fact]
            public void Factory_Backlog_Add_SeriesWithSameNameCanOnlyBeAddedOnce ()
            {
                var backlog = Factory.Backlog;
                var series1 = Factory.Series(Factory.Name("name"));
                var series2 = Factory.Series(Factory.Name("name"))
                    .WithEpisodes(1);

                var backlogWithSeries = backlog.Add(series1).Add(series2);

                Assert.Single(backlogWithSeries);
            }

            //GetSeriesWithName

            [Fact]
            public void
                Factory_Backlog_GetSeriesWithName_ReturnsNullOnNullName ()
            {
                var backlog = Factory.Backlog;
                IName name = null;

                var series = backlog.GetSeriesWithName(name);

                Assert.Null(series);
            }

            [Fact]
            public void
                Factory_Backlog_GetSeriesWithName_ReturnsNullOnNotContainedName ()
            {
                var backlog = Factory.Backlog;
                var name = Factory.Name("name");

                var series = backlog.GetSeriesWithName(name);

                Assert.Null(series);
            }

            [Fact]
            public void
                Factory_Backlog_GetSeriesWithName_ReturnsSeriesWithContainedName ()
            {
                var name = Factory.Name("name");
                var series = Factory.Series(name);
                var backlog = Factory.Backlog.Add(series);

                var gottenSeries = backlog.GetSeriesWithName(name);

                Assert.Equal(series, gottenSeries);
            }

            //Replace

            [Fact]
            public void Factory_Backlog_Replace_ThrowsExceptionOnNullOld ()
            {
                var backlog = Factory.Backlog;

                ISeriesTuple oldSeries = null;
                var newSeries = Factory.Series(Factory.Name("new"));

                Assert.Throws<ArgumentNullException>(
                    () => backlog.Replace(oldSeries, newSeries));
            }

            [Fact]
            public void Factory_Backlog_Replace_ThrowsExceptionOnNullNew ()
            {
                var backlog = Factory.Backlog;

                var oldSeries = Factory.Series(Factory.Name("old"));
                ISeriesTuple newSeries = null;

                Assert.Throws<ArgumentNullException>(
                    () => backlog.Replace(oldSeries, newSeries));
            }

            [Fact]
            public void Factory_Backlog_Replace_ThrowsExceptionOnNullOldNew ()
            {
                var backlog = Factory.Backlog;

                ISeriesTuple oldSeries = null;
                ISeriesTuple newSeries = null;

                Assert.Throws<ArgumentNullException>(
                    () => backlog.Replace(oldSeries, newSeries));
            }

            [Fact]
            public void Factory_Backlog_Replace_ReturnsItselfWhenEmpty ()
            {
                var backlog = Factory.Backlog;
                var oldSeries = Factory.Series(Factory.Name("old"));
                var newSeries = Factory.Series(Factory.Name("new"));

                var replacedBacklog = backlog.Replace(oldSeries, newSeries);

                Assert.Equal(backlog, replacedBacklog);
            }

            [Fact]
            public void Factory_Backlog_Replace_ReturnsItselfOnNotContained ()
            {
                var backlog = Factory.Backlog.Add(
                    Factory.Series(Factory.Name("name")));
                var oldSeries = Factory.Series(Factory.Name("old"));
                var newSeries = Factory.Series(Factory.Name("new"));

                var replacedBacklog = backlog.Replace(oldSeries, newSeries);

                Assert.Equal(backlog, replacedBacklog);
            }

            [Fact]
            public void Factory_Backlog_Replace_ReplacesSeries ()
            {
                var oldSeries = Factory.Series(Factory.Name("old"));
                var backlog = Factory.Backlog.Add(oldSeries);
                var newSeries = Factory.Series(Factory.Name("new"));

                var replacedBacklog = backlog.Replace(oldSeries, newSeries);

                Assert.Single(replacedBacklog);
                Assert.Contains(newSeries, replacedBacklog);
            }
        }

        public class ListTests
        {
            //Default values

            [Fact]
            public void Factory_List_IsNotNull ()
            {
                var list = Factory.List;

                Assert.NotNull(list);
            }

            [Fact]
            public void Factory_List_IsEmpty ()
            {
                var list = Factory.List;

                Assert.Empty(list);
            }
            //This also proves IEnumerable works.

            //Add

            [Fact]
            public void Factory_List_Add_ReturnsItselfOnNullInput ()
            {
                var list = Factory.List;

                var listWithSeries = list.Add(null);

                Assert.Equal(list, listWithSeries);
            }

            [Fact]
            public void Factory_List_Add_AddsSeriesToModel ()
            {
                var list = Factory.List;
                var series = Factory.Series(Factory.Name("name"));

                var listWithSeries = list.Add(series);

                Assert.Contains(series, listWithSeries);
            }

            //GetSeriesWithName

            [Fact]
            public void
                Factory_List_GetSeriesWithName_ThrowsExceptionOnNullInput ()
            {
                var list = Factory.List;

                IName name = null;

                Assert.Throws<ArgumentNullException>(
                    () => list.GetSeriesWithName(name));
            }

            [Fact]
            public void
                Factory_List_GetSeriesWithName_ReturnsNullWhenEmpty ()
            {
                var list = Factory.List;
                var name = Factory.Name("name");

                var series = list.GetSeriesWithName(name);

                Assert.Null(series);
            }

            [Fact]
            public void
                Factory_List_GetSeriesWithName_ReturnsNullOnNotContained ()
            {
                var list =
                    Factory.List.Add(Factory.Series(Factory.Name("Series")));
                var name = Factory.Name("name");

                var series = list.GetSeriesWithName(name);

                Assert.Null(series);
            }

            [Fact]
            public void
                Factory_List_GetSeriesWithName_ReturnsContainedSeries ()
            {
                var name = Factory.Name("Series");
                var series = Factory.Series(name);
                var list =
                    Factory.List.Add(series);

                var returnedSeries = list.GetSeriesWithName(name);

                Assert.Equal(series, returnedSeries);
            }

            //ReplaceAll

            [Fact]
            public void Factory_List_ReplaceAll_ThrowsExceptionOnNullOld ()
            {
                var list = Factory.List;
                ISeriesTuple oldSeries = null;
                var newSeries = Factory.Series(Factory.Name("new"));

                Assert.Throws<ArgumentNullException>(
                    () => list.ReplaceAll(oldSeries, newSeries));
            }

            [Fact]
            public void Factory_List_ReplaceAll_ThrowsExceptionOnNullNew ()
            {
                var list = Factory.List;
                var oldSeries = Factory.Series(Factory.Name("old"));
                ISeriesTuple newSeries = null;

                Assert.Throws<ArgumentNullException>(
                    () => list.ReplaceAll(oldSeries, newSeries));
            }

            [Fact]
            public void Factory_List_ReplaceAll_ThrowsExceptionOnNullOldNew ()
            {
                var list = Factory.List;
                ISeriesTuple oldSeries = null;
                ISeriesTuple newSeries = null;

                Assert.Throws<ArgumentNullException>(
                    () => list.ReplaceAll(oldSeries, newSeries));
            }

            [Fact]
            public void
                Factory_List_ReplaceAll_ReturnsItselfWhenEmpty ()
            {
                var list = Factory.List;
                var oldSeries = Factory.Series(Factory.Name("old"));
                var newSeries = Factory.Series(Factory.Name("new"));

                var replacedList = list.ReplaceAll(oldSeries, newSeries);

                Assert.Equal(list, replacedList);
            }

            [Fact]
            public void
                Factory_List_ReplaceAll_ReturnsItselfOnSeriesNotContained ()
            {
                var list = Factory.List.Add(
                    Factory.Series(Factory.Name("A Series")));
                var oldSeries = Factory.Series(Factory.Name("old"));
                var newSeries = Factory.Series(Factory.Name("new"));

                var replacedList = list.ReplaceAll(oldSeries, newSeries);

                Assert.Equal(list, replacedList);
            }

            //Eliminates condition for a stack-overflow.
            [Fact]
            public void
                Factory_List_ReplaceAll_ReturnsItselfWhenOldIsNewIsContained ()
            {
                var oldSeries = Factory.Series(Factory.Name("old"));
                var newSeries = Factory.Series(Factory.Name("old"));
                var list = Factory.List.Add(oldSeries);

                var replacedList = list.ReplaceAll(oldSeries, newSeries);

                Assert.Equal(list, replacedList);
            }

            [Fact]
            public void
                Factory_List_ReplaceAll_ReplacesAll ()
            {
                var oldSeries = Factory.Series(Factory.Name("old"));
                var list = Factory.List
                    .Add(oldSeries)
                    .Add(oldSeries);
                var newSeries = Factory.Series(Factory.Name("new"));

                var replacedList = list.ReplaceAll(oldSeries, newSeries);

                foreach (var series in replacedList)
                    Assert.Equal(newSeries, series);
            }

            //RemoveAll

            [Fact]
            public void Factory_List_RemoveAll_ThrowsExceptionOnNullInput ()
            {
                var list = Factory.List;

                ISeriesTuple series = null;

                Assert.Throws<ArgumentNullException>(
                    () => list.RemoveAll(series));
            }

            [Fact]
            public void
                Factory_List_RemoveAll_ReturnsItselfWhenEmpty ()
            {
                var list = Factory.List;
                var series = Factory.Series(Factory.Name("name"));

                var removedList = list.RemoveAll(series);

                Assert.Equal(list, removedList);
            }

            [Fact]
            public void
                Factory_List_RemoveAll_ReturnsItselfOnSeriesNotContained ()
            {
                var list = Factory.List.Add(
                    Factory.Series(Factory.Name("A Series")));
                var series = Factory.Series(Factory.Name("name"));

                var removedList = list.RemoveAll(series);

                Assert.Equal(list, removedList);
            }

            [Fact]
            public void
                Factory_List_RemoveAll_RemovesAll ()
            {
                var series = Factory.Series(Factory.Name("name"));
                var list = Factory.List
                    .Add(series)
                    .Add(series);

                var removedList = list.RemoveAll(series);

                Assert.Empty(removedList);
            }
        }

        //Records

        public class SeriesTupleTests
        {
            //Constructor

            [Fact]
            public void SeriesTuple_NullNameThrowsException ()
            {
                IName name = null;

                Assert.Throws<ArgumentNullException>(
                    () => Factory.Series(name));
            }

            [Fact]
            public void SeriesTuple_NameIsGivenName ()
            {
                var name = Factory.Name("name");

                var series = Factory.Series(name);

                Assert.Equal(name, series.Name);
            }

            //Default values

            [Fact]
            public void SeriesTuple_DefaultEpisodesIsZero ()
            {
                var name = Factory.Name("name");
                uint episodes = 0;

                var series = Factory.Series(name);

                Assert.Equal(episodes, series.Episodes);
            }

            [Fact]
            public void SeriesTuple_DefaultWatchedIsZero ()
            {
                var name = Factory.Name("name");
                uint watched = 0;

                var series = Factory.Series(name);

                Assert.Equal(watched, series.Watched);
            }

            [Fact]
            public void SeriesTuple_DefaultStreamIsNull ()
            {
                var name = Factory.Name("name");

                var series = Factory.Series(name);

                Assert.Null(series.Stream);
            }

            //WithName

            [Fact]
            public void SeriesTuple_WithName_ThrowsExceptionOnNullInput ()
            {
                var name = Factory.Name("name");
                var series = Factory.Series(name);

                IName nullName = null;

                Assert.Throws<ArgumentNullException>(
                    () => series.WithName(nullName));
            }

            [Fact]
            public void SeriesTuple_WithName_SetsName ()
            {
                var name = Factory.Name("name");
                var series = Factory.Series(name);

                var newName = Factory.Name("new name");
                var namedSeries = series.WithName(newName);

                Assert.Equal(newName, namedSeries.Name);
            }

            //WithEpisodes

            [Fact]
            public void SeriesTuple_WithEpisodes_SetsEpisodes ()
            {
                var series = Factory.Series(Factory.Name("name"));
                uint episodes = 1;

                var episodeSeries = series.WithEpisodes(episodes);

                Assert.Equal(episodes, episodeSeries.Episodes);
            }

            [Fact]
            public void
                SeriesTuple_WithEpisodes_ThrowsExceptionIfLessThanWatched ()
            {
                uint watched = 2;
                var series = 
                    Factory.Series(Factory.Name("name"))
                    .WithWatched(watched);
                uint episodes = 1;

                Assert.Throws<ArgumentException>(
                    () => series.WithEpisodes(episodes));
            }

            //WithWatched

            [Fact]
            public void SeriesTuple_WithWatched_SetsWatched ()
            {
                var series = Factory.Series(Factory.Name("name"));
                uint watched = 1;

                var watchedSeries = series.WithWatched(watched);

                Assert.Equal(watched, watchedSeries.Watched);
            }

            [Fact]
            public void
                SeriesTuple_WithWatched_ThrowsExceptionIfMoreThanEpisodes ()
            {
                uint episodes = 1;
                var series =
                    Factory.Series(Factory.Name("name"))
                    .WithEpisodes(episodes);
                uint watched = 2;

                Assert.Throws<ArgumentException>(
                    () => series.WithWatched(watched));
            }

            //WithStream

            [Fact]
            public void SeriesTuple_WithStream_SetsStream ()
            {
                var series = Factory.Series(Factory.Name("name"));
                var stream = new Uri("https://www.example.com/");

                var streamSeries = series.WithStream(stream);

                Assert.Equal(stream, streamSeries.Stream);
            }

            [Fact]
            public void SeriesTuple_WithStream_SetsNullStream ()
            {
                var series = Factory.Series(Factory.Name("name"));
                Uri stream = null;

                var streamSeries = series.WithStream(stream);

                Assert.Equal(stream, streamSeries.Stream);
            }
        }

        public class AliasTupleTests
        {
            //Constructor

            [Fact]
            public void Factory_AliasTuple_ThrowsExceptionOnNullAlias ()
            {
                IName alias = null;
                var name = Factory.Name("name");

                Assert.Throws<ArgumentNullException>(
                    () => Factory.AliasTuple(alias, name));
            }

            [Fact]
            public void Factory_AliasTuple_ThrowsExceptionOnNullName ()
            {
                var alias = Factory.Name("alias");
                IName name = null;

                Assert.Throws<ArgumentNullException>(
                    () => Factory.AliasTuple(alias, name));
            }

            [Fact]
            public void Factory_AliasTuple_AliasIsGivenAlias ()
            {
                var alias = Factory.Name("alias");
                var name = Factory.Name("name");

                var aliasTuple = Factory.AliasTuple(alias, name);

                Assert.Equal(alias, aliasTuple.Alias);
            }

            [Fact]
            public void Factory_AliasTuple_NameIsGivenName ()
            {
                var alias = Factory.Name("alias");
                var name = Factory.Name("name");

                var aliasTuple = Factory.AliasTuple(alias, name);

                Assert.Equal(name, aliasTuple.Name);
            }

            //WithAlias

            [Fact]
            public void
                Factory_AliasTuple_WithAlias_ThrowsExceptionOnNullAlias ()
            {
                var aliasTuple = Factory.AliasTuple(
                    Factory.Name("alias"),
                    Factory.Name("name"));

                IName newAlias = null;

                Assert.Throws<ArgumentNullException>(
                    () => aliasTuple.WithAlias(newAlias));
            }

            [Fact]
            public void
                Factory_AliasTuple_WithAlias_SetsAlias ()
            {
                var aliasTuple = Factory.AliasTuple(
                    Factory.Name("alias"),
                    Factory.Name("name"));

                var newAlias = Factory.Name("new alias");
                var newAliasTuple = aliasTuple.WithAlias(newAlias);

                Assert.Equal(newAlias, newAliasTuple.Alias);
            }

            //WithName

            [Fact]
            public void
                Factory_AliasTuple_WithName_ThrowsExceptionOnNullName ()
            {
                var aliasTuple = Factory.AliasTuple(
                    Factory.Name("alias"),
                    Factory.Name("name"));

                IName newName = null;

                Assert.Throws<ArgumentNullException>(
                    () => aliasTuple.WithName(newName));
            }

            [Fact]
            public void
                Factory_AliasTuple_WithName_SetsName ()
            {
                var aliasTuple = Factory.AliasTuple(
                    Factory.Name("alias"),
                    Factory.Name("name"));

                var newName = Factory.Name("new name");
                var newAliasTuple = aliasTuple.WithName(newName);

                Assert.Equal(newName, newAliasTuple.Name);
            }
        }

        //Values

        public class NameTests
        {
            //Default values

            [Fact]
            public void Factory_Name_ThrowsExceptionOnNullInput ()
            {
                string name = null;

                Assert.Throws<ArgumentNullException>(
                    () => Factory.Name(name));
            }

            [Fact]
            public void Factory_Name_ThrowsExceptionOnEmptyInput ()
            {
                var name = "";

                Assert.Throws<ArgumentException>(
                    () => Factory.Name(name));
            }

            [Fact]
            public void Factory_Name_ThrowsExceptionOnWhitespaceInput ()
            {
                var name = "  ";

                Assert.Throws<ArgumentException>(
                    () => Factory.Name(name));
            }

            [Fact]
            public void Factory_Name_NameIsGivenName ()
            {
                var name = "name";
                var seriesname = Factory.Name(name);

                Assert.Equal(name, seriesname.Name);
            }

            //Equals

            [Theory]
            [InlineData("name", " name")]
            [InlineData("name", "Name ")]
            [InlineData("name", "NAME")]
            [InlineData("name", " name ")]
            public void
                Factory_Name_Equals_TrueIrrelevantOfCaseWhitespace (
                string inname1, string inname2)
            {
                var name1 = Factory.Name(inname1);
                var name2 = Factory.Name(inname2);

                Assert.Equal(name1, name2);
            }

            [Theory]
            [InlineData("name", " name")]
            [InlineData("name", "Name ")]
            [InlineData("name", "NAME")]
            [InlineData("name", " name ")]
            public void
                Factory_Name_Equals_TrueIrrelevantOfCaseWhitespace_OnStrings (
                string inname, string string_name)
            {
                var IName_name = Factory.Name(inname);

                Assert.True(IName_name.Equals(string_name));
            }

            //GetHashCode

            [Theory]
            [InlineData("name", " name")]
            [InlineData("name", "Name ")]
            [InlineData("name", "NAME")]
            [InlineData("name", " name ")]
            public void
                Factory_Name_GetHashCode_IsSameIrrelevantOfCaseWhitespace (
                string inname1, string inname2)
            {
                var name1 = Factory.Name(inname1);
                var name2 = Factory.Name(inname2);

                Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
            }
        }
    }
}
