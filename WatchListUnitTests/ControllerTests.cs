using System.Linq;
using WatchList;
using System;
using Xunit;

namespace WatchListUnitTests
{
    public class ControllerTests
    {
        /*  
         *  Find places where the controller, view or model is
         *  supposed to change by searching CONTROLLER, VIEW or MODEL
         *  case matched.
         * 
         * All possible inputs:     Followng changes:
         *  - null                  -> no change. (=default)
         *  - empty                 -> no change. (=default)
         *  - whitespace            -> no change. (=default)
         *  - <gibberish>           -> add message (command unsupported)
         *  - ++                    -> watching ? (w == eps ? mes. : incr.) : mes.
         *  - ++ <gibberish>        -> ?
         *  - --                    -> watching ? (w == 0 ? mes. : decr.) : mes.
         *  - -- <gibberish>        -> ?
         *  - add                   -> add message (add, missing param.)
         *  - add <alias>           -> add series
         *  - add <name>            -> add series
         *  - alias                 -> message (alias, missing param.)
         *  - alias <name>          -> watching ? add alias : message (not w.)
         *  - backlog               -> set inBacklog flag
         *  - backlog <gibberish>   -> ?
         *  - episodes              -> watching ? mes. (mis. prm.) : mes. (not w.)
         *  - episodes <gibberish>  -> watching ? mes. (unpars.) : mes. (not w.)
         *  - episodes <uint>       -> watching ? set episodes : mes. (not w.)
         *  - exit                  -> set exit Flag
         *  - exit <gibberish>      -> ?
         *  - finish                -> watching ? clear watch. : mes. (not w.)
         *  - finish <gibberish>    -> ?
         *  - help                  -> message (help-file)
         *  - help <gibberish>      -> ?
         *  - list                  -> reset inBacklog flag
         *  - list <gibberish>      -> ?
         *  - random                -> empty list ? mes. (empty list) : set watched
         *  - random <gibberish>    -> ?
         *  - rm                    -> message (rm, missing param.)
         *  - rm <not found>        -> message (rm, not found name/alias)
         *  - rm <alias>            -> remove series
         *  - rm <name>             -> remove series
         *  - stream                -> watching ? (str != null ? open str. : mes. (no str.)) : mes. (not w.)
         *  - stream <gibberish>    -> watching ? mes. (malf. uri) : mes. (not w.)
         *  - stream <uri>          -> watching ? set str. : mes. (not w.)
         *  - watch                 -> message (missing param.)
         *  - watch <not found>     -> message (not found)
         *  - watch <alias>         -> set watching
         *  - watch <name>          -> set watching
         *  - watched               -> watching ? mes. (missing param.) : mes. (not w.)
         *  - watched <gibberish>   -> watching ? mes. (unpars.) : mes. (not w.)
         *  - watched <uint>        -> watching ? (w > eps ? mes. (w > eps) : set w) : mes. (not w.)
         */

        public class HandleInput
        {
            #region Argument correctness

            [Fact]
            public void Controller_HandleInput_ThrowsExceptionOnNullController ()
            {
                IController controller = null;
                var view = Factory.View;
                var model = Factory.Model;
                string input = null;

                Assert.Throws<ArgumentNullException>(
                    () => Controller.HandleInput(
                        controller, view, model, input));
            }

            [Fact]
            public void Controller_HandleInput_ThrowsExceptionOnNullView ()
            {
                var controller = Factory.Controller;
                IView view = null;
                var model = Factory.Model;
                string input = null;

                Assert.Throws<ArgumentNullException>(
                    () => Controller.HandleInput(
                        controller, view, model, input));
            }

            [Fact]
            public void Controller_HandleInput_ThrowsExceptionOnNullModel ()
            {
                var controller = Factory.Controller;
                var view = Factory.View;
                IModel model = null;
                string input = null;

                Assert.Throws<ArgumentNullException>(
                    () => Controller.HandleInput(
                        controller, view, model, input));
            }

#endregion

            //From here on, test user input.

            //Null

            [Fact]
            public void
                Controller_HandleInput_Null_AfterDefault_ChangesNothing ()
            {
                var startingState = Empty;
                string input = null;

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.view, view);
                Assert.Equal(startingState.model, model);
            }

            //Empty

            [Fact]
            public void
                Controller_HandleInput_Empty_AfterDefault_ChangesNothing ()
            {
                var startingState = Empty;
                string input = "";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.view, view);
                Assert.Equal(startingState.model, model);
            }

            //Whitespace

            [Fact]
            public void
                Controller_HandleInput_Whitespace_AfterDefautl_ChangesNothing ()
            {
                var startingState = Empty;
                string input = "  ";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.view, view);
                Assert.Equal(startingState.model, model);
            }

            #region ++

            [Fact]
            public void
                Controller_HandleInput_PlusPlus_AfterDefault_SetsMessage ()
            {
                var startingState = Empty;
                var input = "++";
                var expectedMessage = "Cannot increase watched count: " +
                    "not watching any series.";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);

                //VIEW changes.
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);
            }

            [Fact]
            public void
                Controller_HandleInput_PlusPlus_AfterWatch_SetsMessageIfCurrentlyWatchingWatchedEqualsEpisodes ()
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("episodes 1")
                    .Command("watched 1")
                    .Command("finish")
                    .Command("add Series")
                    .Command("watch Series");
                var input = "++";
                var expectedMessage = "Cannot increase watched count: total " +
                    "amount of episodes has already been watched.";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);

                //VIEW changes.
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);
            }

            [Fact]
            public void
                Controller_HandleInput_PlusPlus_AfterRandom_SetsMessageIfCurrentlyWatchingWatchedEqualsEpisodes ()
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("episodes 1")
                    .Command("watched 1")
                    .Command("finish")
                    .Command("add Series")
                    .Command("random");
                var input = "++";
                var expectedMessage = "Cannot increase watched count: total " +
                    "amount of episodes has already been watched.";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);

                //VIEW changes.
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);
            }

            [Fact]
            public void
                Controller_HandleInput_PlusPlus_AfterWatch_IncreasesWatchedCountOfCurrentlyWatchedByOne ()
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var input = "++";
                uint expectedWatched = 1;

                var (controller, view, model) = startingState.Command(input);
                
                //CONTROLLER changes.
                Assert.Equal(
                    expectedWatched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Episodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Stream,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    expectedWatched,
                    view.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Episodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Stream,
                    view.CurrentlyWatching.Stream);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Equal(
                    expectedWatched,
                    model.List.Single().Watched);
                Assert.Equal(
                    expectedWatched,
                    model.Backlog.Single().Watched);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Fact]
            public void
                Controller_HandleInput_PlusPlus_AfterRandom_IncreasesWatchedCountOfCurrentlyWatchedByOne ()
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var input = "++";
                uint expectedWatched = 1;

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    expectedWatched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Episodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Stream,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    expectedWatched,
                    view.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Episodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Stream,
                    view.CurrentlyWatching.Stream);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Equal(
                    expectedWatched,
                    model.List.Single().Watched);
                Assert.Equal(
                    expectedWatched,
                    model.Backlog.Single().Watched);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            #endregion

            #region --

            [Fact]
            public void
            Controller_HandleInput_MinusMinus_AfterDefault_SetsMessage ()
            {
                var startingState = Empty;
                var input = "--";
                var expectedMessage = "Cannot decrease watched count: not " +
                    "watching any series.";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);

                //VIEW changes.
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);
            }

            [Fact]
            public void
                Controller_HandleInput_MinusMinus_AfterWatch_SetsMessageIfCurrentlyWatchingWatchedCountIsZero ()
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var input = "--";
                var expectedMessage = "Cannot decrease watched count: " +
                    "watched count cannot be set below zero.";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);

                //VIEW changes.
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);
            }

            [Fact]
            public void
                Controller_HandleInput_MinusMinus_AfterRandom_SetsMessageIfCurrentlyWatchingWatchedCountIsZero ()
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var input = "--";
                var expectedMessage = "Cannot decrease watched count: " +
                    "watched count cannot be set below zero.";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);

                //VIEW changes.
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);
            }

            [Fact]
            public void
                Controller_HandleInput_MinusMinus_AfterWatched_DecreasesWatchedCountOfCurrentlyWatchedByOne ()
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("watched 1")
                    .Command("finish")
                    .Command("add Series")
                    .Command("watch Series");
                var input = "--";
                uint expectedWatched = 0;

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    expectedWatched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Episodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Stream,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    expectedWatched,
                    view.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Episodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Stream,
                    view.CurrentlyWatching.Stream);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Equal(
                    expectedWatched,
                    model.List.Single().Watched);
                Assert.Equal(
                    expectedWatched,
                    model.Backlog.Single().Watched);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Fact]
            public void
                Controller_HandleInput_MinusMinus_AfterRandom_DecreasesWatchedCountOfCurrentlyWatchedByOne ()
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("watched 1")
                    .Command("finish")
                    .Command("add Series")
                    .Command("random");
                var input = "--";
                uint expectedWatched = 0;

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    expectedWatched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Episodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Stream,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    expectedWatched,
                    view.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Episodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Stream,
                    view.CurrentlyWatching.Stream);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Equal(
                    expectedWatched,
                    model.List.Single().Watched);
                Assert.Equal(
                    expectedWatched,
                    model.Backlog.Single().Watched);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            #endregion

            #region Add <series>

            [Theory]
            [InlineData("add")]
            [InlineData(" ADD")]
            [InlineData("Add ")]
            [InlineData(" aDD ")]
            public void
                Controller_HandleInput_Add_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Please specify the name or an " +
                    "alias of the series to be added to the list.";

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);
            }

            [Theory]
            [InlineData("add ", "series")]
            [InlineData(" ADD ", " SERIES")]
            [InlineData("Add ", "Series ")]
            [InlineData(" aDD ", " sERies ")]
            public void
                Controller_HandleInput_AddName_AfterDefault_AddsSeriesToModel
                (string command, string series)
            {
                var startingState = Empty;

                var (controller, view, model) = 
                    startingState.Command(command + series);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.view, view);

                //MODEL changes
                Assert.NotNull(
                    model.List.GetSeriesWithName(Factory.Name(series)));
                Assert.Single(model.List);
                Assert.NotNull(
                    model.Backlog.GetSeriesWithName(Factory.Name(series)));
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("add swln")]
            [InlineData(" ADD SWLN")]
            [InlineData("Add Swln ")]
            [InlineData(" aDd sWLN ")]
            public void Controller_HandleInput_AddAlias_AfterFinish_CanAddWithAliases (string input)
            {
                var expectedName = Factory.Name("Series With a Long Name");
                var startingState = Empty
                    .Command("add Series With a Long Name")
                    .Command("watch Series With a Long Name")
                    .Command("alias swln")
                    .Command("finish");

                var (controller, view, model) = startingState.Command(input);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.view, view);

                //MODEL changes
                Assert.NotNull(model.List.GetSeriesWithName(expectedName));
                Assert.Equal(startingState.model.Backlog, model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

#endregion

            #region Alias <alias>

            [Theory]
            [InlineData("alias")]
            [InlineData(" ALIAS")]
            [InlineData("Alias ")]
            [InlineData(" ALIas ")]
            public void
                Controller_HandleInput_Alias_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set alias: not watching any " +
                    "series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("alias")]
            [InlineData(" ALIAS")]
            [InlineData("Alias ")]
            [InlineData(" ALIas ")]
            public void
                Controller_HandleInput_Alias_AfterWatch_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Please specify the alias.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("alias")]
            [InlineData(" ALIAS")]
            [InlineData("Alias ")]
            [InlineData(" ALIas ")]
            public void
                Controller_HandleInput_Alias_AfterRandom_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Please specify the alias.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("alias alias")]
            [InlineData(" ALIAS ALIAS")]
            [InlineData("Alias Alias")]
            [InlineData(" ALIas aliAS")]
            public void
                Controller_HandleInput_AliasName_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set alias: not watching any " +
                    "series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("alias ", "alias")]
            [InlineData(" ALIAS ", "ALIAS")]
            [InlineData("Alias ", "Alias ")]
            [InlineData(" ALIas ", "aliAS ")]
            public void Controller_HandleInput_AliasName_AfterWatch_AddsAlias 
                (string command, string alias)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Added alias 'alias' to series " +
                    "'Series'.";

                var (controller, view, model) =
                    startingState.Command(command + alias);

                //MODEL changes
                Assert.Single(model.Aliases);
                Assert.NotNull(model.Aliases.GetMainName(Factory.Name(alias)));
                Assert.Equal(startingState.model.Backlog, model.Backlog);
                Assert.Equal(startingState.model.List, model.List);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
            }

            [Theory]
            [InlineData("alias ", "alias")]
            [InlineData(" ALIAS ", "ALIAS")]
            [InlineData("Alias ", "Alias ")]
            [InlineData(" ALIas ", "aliAS ")]
            public void Controller_HandleInput_AliasName_AfterRandom_AddsAlias
                (string command, string alias)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Added alias 'alias' to series " +
                    "'Series'.";

                var (controller, view, model) =
                    startingState.Command(command + alias);

                //MODEL changes
                Assert.Single(model.Aliases);
                Assert.NotNull(model.Aliases.GetMainName(Factory.Name(alias)));
                Assert.Equal(startingState.model.Backlog, model.Backlog);
                Assert.Equal(startingState.model.List, model.List);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
            }

            #endregion

            #region Backlog and List

            [Theory]
            [InlineData("backlog")]
            [InlineData(" BACKLOG")]
            [InlineData("Backlog ")]
            [InlineData(" BACkLOg ")]
            public void
                Controller_HandleInput_Backlog_AfterDefault_SetsInBacklog
                (string input)
            {
                var startingState = Empty;

                var (controller, view, model) =
                    startingState.Command(input);

                //CONTROLLER changes
                Assert.True(controller.InBacklog);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching,
                    controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                Assert.Equal(startingState.model, model);
                Assert.Equal(startingState.view, view);
            }

            [Theory]
            [InlineData("list")]
            [InlineData(" LIST")]
            [InlineData("List ")]
            [InlineData(" lIST ")]
            public void
                Controller_HandleInput_List_AfterDefault_ResetsInBacklog
                (string input)
            {
                var startingState = Empty;

                var (controller, view, model) =
                    startingState.Command(input);

                //CONTROLLER changes
                Assert.False(controller.InBacklog);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching,
                    controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                Assert.Equal(startingState.model, model);
                Assert.Equal(startingState.view, view);
            }

            #endregion

            #region Episodes <uint>

            [Theory]
            [InlineData("episodes")]
            [InlineData(" EPISODES")]
            [InlineData("Episodes ")]
            [InlineData(" ePIsODEs ")]
            public void
                Controller_HandleInput_Episodes_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set episode count: not " +
                    "watching any series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes")]
            [InlineData(" EPISODES")]
            [InlineData("Episodes ")]
            [InlineData(" ePIsODEs ")]
            public void
                Controller_HandleInput_Episodes_AfterWatch_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Please specify the total amount of " +
                    "episodes.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes")]
            [InlineData(" EPISODES")]
            [InlineData("Episodes ")]
            [InlineData(" ePIsODEs ")]
            public void
                Controller_HandleInput_Episodes_AfterRandom_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Please specify the total amount of " +
                    "episodes.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes 1")]
            [InlineData(" EPISODES 1")]
            [InlineData("Episodes 1 ")]
            [InlineData(" ePIsODEs 1 ")]
            public void
                Controller_HandleInput_EpisodesNum_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set total episode count: not " +
                    "watching any series.";

                var (controller, view, model) = 
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes 1")]
            [InlineData(" EPISODES 1")]
            [InlineData("Episodes 1 ")]
            [InlineData(" ePIsODEs 1 ")]
            public void
                Controller_HandleInput_EpisodesNum_AfterWatchedNum_SetsMessageWhenSetEpisodesWouldBeLessThanWatched
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("watched 2");
                var expectedMessage = "Cannot set total episode count: " +
                    "amount of episodes cannot be less than the " +
                    "amount of watched episodes.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes 1")]
            [InlineData(" EPISODES 1")]
            [InlineData("Episodes 1 ")]
            [InlineData(" ePIsODEs 1 ")]
            public void
                Controller_HandleInput_EpisodesNum_AfterWatch_SetsMessageWhenSetEpisodesWouldBeLessThanWatched
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("watched 2")
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Cannot set total episode count: " +
                    "amount of episodes cannot be less than the " +
                    "amount of watched episodes.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes 1")]
            [InlineData(" EPISODES 1")]
            [InlineData("Episodes 1 ")]
            [InlineData(" ePIsODEs 1 ")]
            public void
                Controller_HandleInput_EpisodesNum_AfterRandom_SetsMessageWhenSetEpisodesWouldBeLessThanWatched
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("watched 2")
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Cannot set total episode count: " +
                    "amount of episodes cannot be less than the " +
                    "amount of watched episodes.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes 1")]
            [InlineData(" EPISODES 1")]
            [InlineData("Episodes 1 ")]
            [InlineData(" ePIsODEs 1 ")]
            public void
                Controller_HandleInput_EpisodesNum_AfterWatch_SetsEpisodesOfCurrentlyWatchedSeries
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                uint expectedEpisodes = 1;

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    expectedEpisodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Stream,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Watched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    expectedEpisodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Stream,
                    view.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Watched,
                    view.CurrentlyWatching.Watched);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Equal(
                    expectedEpisodes,
                    model.List.Single().Episodes);
                Assert.Equal(
                    expectedEpisodes,
                    model.Backlog.Single().Episodes);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("episodes 1")]
            [InlineData(" EPISODES 1")]
            [InlineData("Episodes 1 ")]
            [InlineData(" ePIsODEs 1 ")]
            public void
                Controller_HandleInput_EpisodesNum_AfterRandom_SetsEpisodesOfCurrentlyWatchedSeries
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                uint expectedEpisodes = 1;

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    expectedEpisodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Stream,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Watched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    expectedEpisodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Stream,
                    view.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Watched,
                    view.CurrentlyWatching.Watched);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Equal(
                    expectedEpisodes,
                    model.List.Single().Episodes);
                Assert.Equal(
                    expectedEpisodes,
                    model.Backlog.Single().Episodes);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            //TODO: test that can always be made zero.

            [Theory]
            [InlineData("episodes one")]
            [InlineData(" EPISODES ONE")]
            [InlineData("Episodes One ")]
            [InlineData(" ePIsODEs oNe ")]
            [InlineData("episodes -1")]
            [InlineData(" EPISODES -1")]
            [InlineData("Episodes -1 ")]
            [InlineData(" ePIsODEs -1 ")]
            [InlineData("episodes 1.01")]
            [InlineData(" EPISODES 1.01")]
            [InlineData("Episodes 1.01 ")]
            [InlineData(" ePIsODEs 1.01 ")]
            public void
                Controller_HandleInput_EpisodesGib_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set total episode count: " +
                    "not watching any series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes one")]
            [InlineData(" EPISODES ONE")]
            [InlineData("Episodes One ")]
            [InlineData(" ePIsODEs oNe ")]
            [InlineData("episodes -1")]
            [InlineData(" EPISODES -1")]
            [InlineData("Episodes -1 ")]
            [InlineData(" ePIsODEs -1 ")]
            [InlineData("episodes 1.01")]
            [InlineData(" EPISODES 1.01")]
            [InlineData("Episodes 1.01 ")]
            [InlineData(" ePIsODEs 1.01 ")]
            public void
                Controller_HandleInput_EpisodesGib_AfterWatch_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Cannot set total episode count: " +
                    "parameter must be a positive whole number.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("episodes one")]
            [InlineData(" EPISODES ONE")]
            [InlineData("Episodes One ")]
            [InlineData(" ePIsODEs oNe ")]
            [InlineData("episodes -1")]
            [InlineData(" EPISODES -1")]
            [InlineData("Episodes -1 ")]
            [InlineData(" ePIsODEs -1 ")]
            [InlineData("episodes 1.01")]
            [InlineData(" EPISODES 1.01")]
            [InlineData("Episodes 1.01 ")]
            [InlineData(" ePIsODEs 1.01 ")]
            public void
                Controller_HandleInput_EpisodesGib_AfterRandom_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Cannot set total episode count: " +
                    "parameter must be a positive whole number.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            #endregion

            #region Exit

            [Theory]
            [InlineData("exit")]
            [InlineData(" EXIT")]
            [InlineData("ExiT ")]
            [InlineData(" eXiT ")]
            public void
                Controller_HandleInput_Exit_AfterDefault_SetsWantsExit
                (string input)
            {
                var startingState = Empty;

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.True(controller.WantsExit);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching,
                    controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);

                Assert.Equal(startingState.view, view);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("exit useless")]
            [InlineData(" EXIT PARAMETER")]
            [InlineData("ExiT UseLesS paRAmeTeR")]
            [InlineData(" eXiT uSpaRa")]
            public void
                Controller_HandleInput_ExitGib_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = $"The command '{input.Trim().ToLower()}' " +
                    "is unsupported. Type 'help' for a list of supported " +
                    "commands.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.model, model);
                Assert.Equal(startingState.controller, controller);
            }

#endregion

            #region Finish

            [Theory]
            [InlineData("finish")]
            [InlineData(" FINISH")]
            [InlineData("Finish ")]
            [InlineData(" FinIsH ")]
            public void
                Controller_HandleInput_Finish_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = $"Cannot finish watching: not watching " +
                    $"any series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.model, model);
                Assert.Equal(startingState.controller, controller);
            }

            [Theory]
            [InlineData("finish")]
            [InlineData(" FINISH")]
            [InlineData("Finish ")]
            [InlineData(" FinIsH ")]
            public void
                Controller_HandleInput_Finish_AfterWatch_ResetsCurrentlyWatchingAndRemovesFromList
                (string input)
            {
                //Also removes all from list.
                var startingState = Empty
                    .Command("add Series")
                    .Command("add Series")
                    .Command("add Series")
                    .Command("watch Series");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Null(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Null(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            #endregion

            #region Random

            [Theory]
            [InlineData("random")]
            [InlineData(" RANDOM")]
            [InlineData("Random ")]
            [InlineData(" RaNDom ")]
            public void
                Controller_HandleInput_Random_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;

                var (controller, view, model) =
                    startingState.Command(input);

                var expectedMessage =
                    "Cannot choose random from an empty list.";

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching, 
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("random")]
            [InlineData(" RANDOM")]
            [InlineData("Random ")]
            [InlineData(" RaNDom ")]
            public void
                Controller_HandleInput_Random_AfterAdd_SetsCurrentlyWatching
                (string input)
            {
                var name = "Series";
                var startingState = Empty
                    .Command("add " + name);

                var (controller, view, model) =
                    startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(name, controller.CurrentlyWatching.Name.Name);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(name, view.CurrentlyWatching.Name.Name);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.model, model);
            }

#endregion

            #region rm <series>

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
                Controller_HandleInput_Rm_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage =
                    "Cannot remove from an empty list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm")]
            [InlineData(" RM")]
            [InlineData("Rm ")]
            [InlineData(" rM ")]
            public void
                Controller_HandleInput_Rm_AfterAdd_SetsMessageIfNoParameters
                (string input)
            {
                //Also after watch and random

                var startingState = Empty
                    .Command("add Series");
                var expectedMessage =
                    "Please specify the name or an alias of the series " +
                    "to be removed from the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm")]
            [InlineData(" RM")]
            [InlineData("Rm ")]
            [InlineData(" rM ")]
            public void
                Controller_HandleInput_Rm_AfterWatch_SetsMessageIfNoParameters
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage =
                    "Please specify the name or an alias of the series " +
                    "to be removed from the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm")]
            [InlineData(" RM")]
            [InlineData("Rm ")]
            [InlineData(" rM ")]
            public void
                Controller_HandleInput_Rm_AfterRandom_SetsMessageIfNoParameters
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage =
                    "Please specify the name or an alias of the series " +
                    "to be removed from the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
                Controller_HandleInput_RmName_AfterAdd_SetsMessageIfSeriesIsNotInList
                (string input)
            {
                var startingState = Empty
                    .Command("add Series");
                var expectedMessage = "Series with the name or" +
                    " alias 'name' was not found in the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
                Controller_HandleInput_RmName_AfterWatch_SetsMessageIfSeriesIsNotInList
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Series with the name or" +
                    " alias 'name' was not found in the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
                Controller_HandleInput_RmName_AfterRandom_SetsMessageIfSeriesIsNotInList
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Series with the name or" +
                    " alias 'name' was not found in the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
                Controller_HandleInput_RmName_AfterAdd_RemovesSeriesFromList
                (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("add Name")
                    .Command("add Name");

                var (controller, view, model) = startingState.Command(input);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.view, view);
            }

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
                Controller_HandleInput_RmName_AfterWatch_RemovesSeriesFromListAndWatched
                (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("add Name")
                    .Command("add Name")
                    .Command("watch Name");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Null(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Null(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
                Controller_HandleInput_RmName_AfterRandom_RemovesSeriesFromListAndWatched
                (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("add Name")
                    .Command("add Name")
                    .Command("random");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Null(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Null(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            //Test cases for a found bug

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
            Controller_HandleInput_RmName_AfterEpisodes_RemovesSeriesFromListAndWatched
            (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("random")
                    .Command("episodes 1");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Null(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Null(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
            Controller_HandleInput_RmName_AfterWatched_RemovesSeriesFromListAndWatched
            (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("random")
                    .Command("watched 1");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Null(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Null(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("rm")]
            [InlineData(" RM")]
            [InlineData("Rm ")]
            [InlineData(" rM ")]
            public void
                Controller_HandleInput_RmAlias_AfterAdd_SetsMessageIfNoParameters
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("alias alias")
                    .Command("rm Series")
                    .Command("add Series");
                var expectedMessage =
                    "Please specify the name or an alias of the series " +
                    "to be removed from the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm")]
            [InlineData(" RM")]
            [InlineData("Rm ")]
            [InlineData(" rM ")]
            public void
                Controller_HandleInput_RmAlias_AfterWatch_SetsMessageIfNoParameters
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("alias alias")
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage =
                    "Please specify the name or an alias of the series " +
                    "to be removed from the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm")]
            [InlineData(" RM")]
            [InlineData("Rm ")]
            [InlineData(" rM ")]
            public void
                Controller_HandleInput_RmAlias_AfterRandom_SetsMessageIfNoParameters
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("alias alias")
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("random");
                var expectedMessage =
                    "Please specify the name or an alias of the series " +
                    "to be removed from the list.";

                var (controller, view, model) =
                    startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("rm name")]
            [InlineData(" RM NAME")]
            [InlineData("Rm Name ")]
            [InlineData(" rM nAMe ")]
            public void
                Controller_HandleInput_RmAlias_AfterAdd_RemovesSeriesFromList
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("alias name")
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("add Series")
                    .Command("add Series");

                var (controller, view, model) = startingState.Command(input);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.view, view);
            }

            [Theory]
            [InlineData("rm swln")]
            [InlineData(" RM SWLN")]
            [InlineData("Rm Swln ")]
            [InlineData(" rM sWLn ")]
            public void
                Controller_HandleInput_RmAlias_AfterWatch_RemovesSeriesFromListAndWatching
                (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("watch Name")
                    .Command("alias swln")
                    .Command("rm Name")
                    .Command("add Name")
                    .Command("add Name")
                    .Command("add Name")
                    .Command("watch Name");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Null(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Null(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("rm swln")]
            [InlineData(" RM SWLN")]
            [InlineData("Rm Swln ")]
            [InlineData(" rM sWLn ")]
            public void
                Controller_HandleInput_RmAlias_AfterRandom_RemovesSeriesFromListAndWatching
                (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("watch Name")
                    .Command("alias swln")
                    .Command("rm Name")
                    .Command("add Name")
                    .Command("add Name")
                    .Command("add Name")
                    .Command("random");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Null(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Null(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Empty(model.List);
                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("rm swln")]
            [InlineData(" RM SWLN")]
            [InlineData("Rm Swln ")]
            [InlineData(" rM sWLn ")]
            public void
                Controller_HandleInput_RmAlias_AfterAlias_RemovesSeriesFromListAndWatching
                (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("add Name")
                    .Command("add Name")
                    .Command("watch Name")
                    .Command("alias swln");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Null(controller.CurrentlyWatching);

                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Null(view.CurrentlyWatching);

                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Empty(model.List);

                Assert.Single(model.Backlog);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }



            #endregion

            #region Stream

            [Theory]
            [InlineData("stream")]
            [InlineData(" STREAM")]
            [InlineData("Stream ")]
            [InlineData(" STrEam ")]
            public void
                Controller_HandleInput_Stream_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot open stream: not watching any " +
                    "series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("stream")]
            [InlineData(" STREAM")]
            [InlineData("Stream ")]
            [InlineData(" STrEam ")]
            public void
                Controller_HandleInput_Stream_AfterWatch_SetsMessageIfCurrentlyWatchedSeriesHasNoStream
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Cannot open stream: watched series " +
                    "does not have a stream source.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("stream")]
            [InlineData(" STREAM")]
            [InlineData("Stream ")]
            [InlineData(" STrEam ")]
            public void
                Controller_HandleInput_Stream_AfterRandom_SetsMessageIfCurrentlyWatchedSeriesHasNoStream
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Cannot open stream: watched series " +
                    "does not have a stream source.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("stream")]
            [InlineData(" STREAM")]
            [InlineData("Stream ")]
            [InlineData(" STrEam ")]
            public void
                Controller_HandleInput_Stream_AfterStreamUri_SetsOpenStream
                (string input)
            {
                var uri = new Uri("https://www.example.com/");
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("stream " + uri);

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Equal(uri, controller.OpenStream);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching,
                    controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                Assert.Equal(startingState.model, model);
                Assert.Equal(startingState.view, view);
            }

            [Theory]
            [InlineData("stream")]
            [InlineData(" STREAM")]
            [InlineData("Stream ")]
            [InlineData(" STrEam ")]
            public void
                Controller_HandleInput_Stream_AfterWatchName_SetsOpenStream
                (string input)
            {
                var uri = new Uri("https://www.example.com/");
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("stream " + uri)
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("watch Series");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Equal(uri, controller.OpenStream);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching,
                    controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                Assert.Equal(startingState.model, model);
                Assert.Equal(startingState.view, view);
            }

            [Theory]
            [InlineData("stream")]
            [InlineData(" STREAM")]
            [InlineData("Stream ")]
            [InlineData(" STrEam ")]
            public void
                Controller_HandleInput_Stream_AfterRandom_SetsOpenStream
                (string input)
            {
                var uri = new Uri("https://www.example.com/");
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("stream " + uri)
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("random");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes
                Assert.Equal(uri, controller.OpenStream);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching,
                    controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                Assert.Equal(startingState.model, model);
                Assert.Equal(startingState.view, view);
            }

            #endregion

            #region Stream <uri>

            [Theory]
            [InlineData("stream unparsable")]
            [InlineData(" STREAM UNPARSABLE")]
            [InlineData("Stream Unparsable ")]
            [InlineData(" sTREam UNparSABLE ")]
            public void
                Controller_HandleInput_StreamGib_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set stream: not watching any " +
                    "series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("stream unparsable")]
            [InlineData(" STREAM UNPARSABLE")]
            [InlineData("Stream Unparsable ")]
            [InlineData(" sTREam UNparSABLE ")]
            public void
                Controller_HandleInput_StreamGib_AfterWatchName_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Cannot set stream: the URI is " +
                    "malformed.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("stream unparsable")]
            [InlineData(" STREAM UNPARSABLE")]
            [InlineData("Stream Unparsable ")]
            [InlineData(" sTREam UNparSABLE ")]
            public void
                Controller_HandleInput_StreamGib_AfterRandom_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Cannot set stream: the URI is " +
                    "malformed.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("stream https://www.example.com/")]
            [InlineData(" STREAM HTTPS://WWW.EXAMPLE.COM/")]
            [InlineData("Stream Https://Www.Example.Com/ ")]
            [InlineData(" sTREaM HTTps://wWW.exAMPLE.COm/ ")]
            public void
                Controller_HandleInput_StreamUri_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set stream: not watching any " +
                    "series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("stream https://www.example.com/")]
            [InlineData(" STREAM HTTPS://WWW.EXAMPLE.COM/")]
            [InlineData("Stream Https://Www.Example.Com/ ")]
            [InlineData(" sTREaM HTTps://wWW.exAMPLE.COm/ ")]
            public void
                Controller_HandleInput_StreamUri_AfterWatchName_SetsStreamOfWatchedSeries
                (string input)
            {
                var uri = new Uri("https://www.example.com/");
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    uri,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Episodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Watched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    uri,
                    view.CurrentlyWatching.Stream);

                Assert.Equal(
                    startingState.view.CurrentlyWatching.Episodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Watched,
                    view.CurrentlyWatching.Watched);
                Assert.Equal(startingState.view.Title, view.Title);
                Assert.Equal(startingState.view.Message, view.Message);

                //MODEL changes.
                Assert.Equal(uri, model.List.Single().Stream);
                Assert.Equal(uri, model.Backlog.Single().Stream);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("stream https://www.example.com/")]
            [InlineData(" STREAM HTTPS://WWW.EXAMPLE.COM/")]
            [InlineData("Stream Https://Www.Example.Com/ ")]
            [InlineData(" sTREaM HTTps://wWW.exAMPLE.COm/ ")]
            public void
                Controller_HandleInput_StreamUri_AfterRandom_SetsStreamOfWatchedSeries
                (string input)
            {
                var uri = new Uri("https://www.example.com/");
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    uri,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Episodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Watched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    uri,
                    view.CurrentlyWatching.Stream);

                Assert.Equal(
                    startingState.view.CurrentlyWatching.Episodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Watched,
                    view.CurrentlyWatching.Watched);
                Assert.Equal(startingState.view.Title, view.Title);
                Assert.Equal(startingState.view.Message, view.Message);

                //MODEL changes.
                Assert.Equal(uri, model.List.Single().Stream);
                Assert.Equal(uri, model.Backlog.Single().Stream);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            #endregion

            #region Watch <series>

            [Theory]
            [InlineData("watch")]
            [InlineData(" WATCH")]
            [InlineData("Watch ")]
            [InlineData(" wATCH ")]
            public void Controller_HandleInput_Watch_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot watch from an empty list.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watch")]
            [InlineData(" WATCH")]
            [InlineData("Watch ")]
            [InlineData(" wATCH ")]
            public void Controller_HandleInput_Watch_AfterAddName_SetsMessage
                (string input)
            {
                var startingState = Empty
                    .Command("add Series");
                var expectedMessage = "Please specify the name or an " +
                    "alias of the series you want to watch.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watch name")]
            [InlineData(" WATCH NAME")]
            [InlineData("Watch Name")]
            [InlineData(" wATCH nAME")]
            public void
                Controller_HandleInput_WatchName_AfterDefault_SetsMessafe
                (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot watch from an empty list.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watch name")]
            [InlineData(" WATCH NAME")]
            [InlineData("Watch Name")]
            [InlineData(" wATCH nAME")]
            public void
                Controller_HandleInput_WatchName_AfterAddName_SetsMessageOnSeriesNotInList
                (string input)
            {
                var startingState = Empty
                    .Command("add Series");
                var expectedMessage = "Series with the name or alias 'name' " +
                    "was not found in the list.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watch name")]
            [InlineData(" WATCH NAME")]
            [InlineData("Watch Name")]
            [InlineData(" wATCH nAME")]
            public void
                Controller_HandleInput_WatchName_AfterAddName_SetsCurrentlyWatching
                (string input)
            {
                var startingState = Empty
                    .Command("add Name");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.NotNull(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.NotNull(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watch swln")]
            [InlineData(" WATCH SWLN")]
            [InlineData("Watch Swln")]
            [InlineData(" wATCH sWLN")]
            public void
                Controller_HandleInput_WatchAlias_AfterAddName_SetsMessageOnSeriesNotInList
                (string input)
            {
                var startingState = Empty
                    .Command("add Series");
                var expectedMessage = "Series with the name or alias 'swln' " +
                    "was not found in the list.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watch swln")]
            [InlineData(" WATCH SWLN")]
            [InlineData("Watch Swln")]
            [InlineData(" wATCH sWLN")]
            public void
                Controller_HandleInput_WatchAlias_AfterAddName_SetsCurrentlyWatching
                (string input)
            {
                var startingState = Empty
                    .Command("add Name")
                    .Command("watch Name")
                    .Command("alias swln")
                    .Command("rm Name")
                    .Command("add Name");

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.NotNull(controller.CurrentlyWatching);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.NotNull(view.CurrentlyWatching);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.model, model);
            }

#endregion

            #region Watched <uint>

            [Theory]
            [InlineData("watched")]
            [InlineData(" WATCHED")]
            [InlineData("Watched ")]
            [InlineData(" WatCHED ")]
            public void
                Controller_HandleInput_Watched_AfterDefault_SetsMessage
            (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set watched episode count: not " +
                    "watching any series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched")]
            [InlineData(" WATCHED")]
            [InlineData("Watched ")]
            [InlineData(" WatCHED ")]
            public void
                Controller_HandleInput_Watched_AfterWatchName_SetsMessage
            (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Please specify the amount of watched " +
                    "episodes.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched")]
            [InlineData(" WATCHED")]
            [InlineData("Watched ")]
            [InlineData(" WatCHED ")]
            public void
                Controller_HandleInput_Watched_AfterRandom_SetsMessage
            (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Please specify the amount of watched " +
                    "episodes.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched one")]
            [InlineData(" WATCHED ONE")]
            [InlineData("Watched One")]
            [InlineData(" WatCHED oNe")]
            [InlineData("watched -1")]
            [InlineData(" WATCHED -1")]
            [InlineData("Watched -1")]
            [InlineData(" WatCHED -1")]
            [InlineData("watched 1.01")]
            [InlineData(" WATCHED 1.01")]
            [InlineData("Watched 1.01")]
            [InlineData(" WatCHED 1.01")]
            public void
                Controller_HandleInput_WatchedGib_AfterDefault_SetsMessage
            (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set watched episode count: not " +
                    "watching any series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched one")]
            [InlineData(" WATCHED ONE")]
            [InlineData("Watched One")]
            [InlineData(" WatCHED oNe")]
            [InlineData("watched -1")]
            [InlineData(" WATCHED -1")]
            [InlineData("Watched -1")]
            [InlineData(" WatCHED -1")]
            [InlineData("watched 1.01")]
            [InlineData(" WATCHED 1.01")]
            [InlineData("Watched 1.01")]
            [InlineData(" WatCHED 1.01")]
            public void
                Controller_HandleInput_WatchedGib_AfterWatchName_SetsMessage
            (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Cannot set watched count: parameter " +
                    "must be a positive whole number.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched one")]
            [InlineData(" WATCHED ONE")]
            [InlineData("Watched One")]
            [InlineData(" WatCHED oNe")]
            [InlineData("watched -1")]
            [InlineData(" WATCHED -1")]
            [InlineData("Watched -1")]
            [InlineData(" WatCHED -1")]
            [InlineData("watched 1.01")]
            [InlineData(" WATCHED 1.01")]
            [InlineData("Watched 1.01")]
            [InlineData(" WatCHED 1.01")]
            public void
                Controller_HandleInput_WatchedGib_AfterRandom_SetsMessage
            (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Cannot set watched count: parameter " +
                    "must be a positive whole number.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched 1")]
            [InlineData(" WATCHED 1")]
            [InlineData("Watched 1")]
            [InlineData(" WatCHED 1")]
            public void
            Controller_HandleInput_WatchedNum_AfterDefault_SetsMessage
            (string input)
            {
                var startingState = Empty;
                var expectedMessage = "Cannot set watched episode count: not " +
                    "watching any series.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched 2")]
            [InlineData(" WATCHED 2")]
            [InlineData("Watched 2")]
            [InlineData(" WatCHED 2")]
            public void
            Controller_HandleInput_WatchedNum_AfterWatchName_SetsMessageWhenWatchedWouldBeMoreThanSetEpisodes
            (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("episodes 1")
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("watch Series");
                var expectedMessage = "Cannot set watched count: watched " +
                    "count cannot be over the total episode count.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched 2")]
            [InlineData(" WATCHED 2")]
            [InlineData("Watched 2")]
            [InlineData(" WatCHED 2")]
            public void
            Controller_HandleInput_WatchedNum_AfterRandom_SetsMessageWhenWatchedWouldBeMoreThanSetEpisodes
            (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series")
                    .Command("episodes 1")
                    .Command("rm Series")
                    .Command("add Series")
                    .Command("random");
                var expectedMessage = "Cannot set watched count: watched " +
                    "count cannot be over the total episode count.";

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }

            [Theory]
            [InlineData("watched 1")]
            [InlineData(" WATCHED 1")]
            [InlineData("Watched 1 ")]
            [InlineData(" WatCHED 1 ")]
            public void
                Controller_HandleInput_WatchedNum_AfterWatchName_SetsWatchedOfCurrentlyWatchedSeries
            (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("watch Series");
                uint expectedWatched = 1;

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    expectedWatched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Episodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Stream,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    expectedWatched,
                    view.CurrentlyWatching.Watched);

                Assert.Equal(
                    startingState.view.CurrentlyWatching.Episodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Stream,
                    view.CurrentlyWatching.Stream);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Equal(expectedWatched, model.List.Single().Watched);
                Assert.Equal(expectedWatched, model.Backlog.Single().Watched);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            [Theory]
            [InlineData("watched 1")]
            [InlineData(" WATCHED 1")]
            [InlineData("Watched 1 ")]
            [InlineData(" WatCHED 1 ")]
            public void
                Controller_HandleInput_WatchedNum_AfterRandom_SetsWatchedOfCurrentlyWatchedSeries
            (string input)
            {
                var startingState = Empty
                    .Command("add Series")
                    .Command("random");
                uint expectedWatched = 1;

                var (controller, view, model) = startingState.Command(input);

                //CONTROLLER changes.
                Assert.Equal(
                    expectedWatched,
                    controller.CurrentlyWatching.Watched);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Episodes,
                    controller.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Name,
                    controller.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.controller.CurrentlyWatching.Stream,
                    controller.CurrentlyWatching.Stream);
                Assert.Equal(
                    startingState.controller.InBacklog,
                    controller.InBacklog);
                Assert.Equal(
                    startingState.controller.OpenStream,
                    controller.OpenStream);
                Assert.Equal(
                    startingState.controller.WantsExit,
                    controller.WantsExit);

                //VIEW changes.
                Assert.Equal(
                    expectedWatched,
                    view.CurrentlyWatching.Watched);

                Assert.Equal(
                    startingState.view.CurrentlyWatching.Episodes,
                    view.CurrentlyWatching.Episodes);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Name,
                    view.CurrentlyWatching.Name);
                Assert.Equal(
                    startingState.view.CurrentlyWatching.Stream,
                    view.CurrentlyWatching.Stream);
                Assert.Equal(startingState.view.Message, view.Message);
                Assert.Equal(startingState.view.Title, view.Title);

                //MODEL changes.
                Assert.Equal(expectedWatched, model.List.Single().Watched);
                Assert.Equal(expectedWatched, model.Backlog.Single().Watched);
                Assert.Equal(startingState.model.Aliases, model.Aliases);
            }

            #endregion

            //Help

            [Theory]
            [InlineData("help")]
            [InlineData(" HELP")]
            [InlineData("Help ")]
            [InlineData(" HeLp ")]
            public void Controller_HandleInput_Help_AfterDefault_SetsMessage
                (string input)
            {
                var startingState = Empty;

                var pieces = new string[]
                {
                    "General commands",
                        "add <name/alias>",
                        "exit",
                        "random",
                        "rm <name/alias>",
                        "watch <name/alias>",
                    "Navigation commands",
                        "backlog",
                        "list",
                    "Commands usable when watching a series",
                        "++",
                        "--",
                        "alias <alias>",
                        "episodes <number>",
                        "finish",
                        "stream",
                        "stream <uri>",
                        "watched <number>"
                };

                var (controller, view, model) = startingState.Command(input);

                //VIEW changes
                foreach (var piece in pieces)
                    Assert.Contains(piece, view.Message);

                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }
            
            //Unsupported

            [Theory]
            [InlineData("unsupported")]
            [InlineData("an unsupported command")]
            public void
                Controller_HandleInput_Gib_SetsMessage
                (string input)
            {
                var startingState = Empty;
                var (controller, view, model) = startingState.Command(input);

                var expectedMessage = "The command '" + input
                    + "' is unsupported. Type 'help' for a " +
                    "list of supported commands.";

                //VIEW changes
                Assert.Equal(expectedMessage, view.Message);
                Assert.Equal(
                    startingState.view.CurrentlyWatching,
                    view.CurrentlyWatching);
                Assert.Equal(startingState.view.Title, view.Title);

                Assert.Equal(startingState.controller, controller);
                Assert.Equal(startingState.model, model);
            }
        }

        /// <summary>
        /// Empty starting state for controller, view and model in a tuple.
        /// </summary>
        static readonly (IController controller, IView view, IModel model)
            Empty = Utility.Empty;
    }
}
