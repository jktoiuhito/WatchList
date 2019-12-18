using System.Collections.Generic;
using WatchList;
using System;
using Xunit;

namespace WatchListUnitTests
{
    public class ViewTests
    {
        public class Create
        {
            //Argument correctness

            [Fact]
            public void View_Create_ThrowsExceptionOnNullView ()
            {
                IView view = null;
                var list = Factory.List;
                uint width = 80;

                Assert.Throws<ArgumentNullException>(
                    () => View.Create(view, list, width));
            }

            [Fact]
            public void View_Create_ThrowsExceptionOnNullModel ()
            {
                var view = Factory.View;
                IEnumerable<ISeriesTuple> model = null;
                uint width = 80;

                Assert.Throws<ArgumentNullException>(
                    () => View.Create(view, model, width));
            }

            [Fact]
            public void View_Create_ThrowsExceptionOnZeroWidth ()
            {
                var view = Factory.View;
                var model = Factory.List;
                uint width = 0;

                Assert.Throws<ArgumentException>(
                    () => View.Create(view, model, width));
            }

            //Output correctness

            [Fact]
            public void View_Create_OutputContainsViewTitle ()
            {
                var view = Factory.View;
                var list = Factory.List;
                uint width = 80;
                var shouldContain = view.Title;

                var output = View.Create(view, list, width);

                Assert.Contains(shouldContain, output);
            }

            [Fact]
            public void View_Create_OutputContainsMessage ()
            {
                var message = "message";
                var view = Factory.View.WithMessage(message);
                var list = Factory.List;
                uint width = 80;

                var output = View.Create(view, list, width);

                Assert.Contains(message, output);
            }

            // - listed series

            [Fact]
            public void View_Create_OutputContainsNamesOfListedSeries ()
            {
                var view = Factory.View;
                var name1 = "name";
                var name2 = "Series seireS";
                var model = Factory.List
                    .Add(Factory.Series(Factory.Name(name1)))
                    .Add(Factory.Series(Factory.Name(name2)));
                uint width = 80;

                var output = View.Create(view, model, width);

                Assert.Contains(name1, output);
                Assert.Contains(name2, output);
            }

            // - next episode (watched + 1)

            [Fact]
            public void View_Create_OutputContainsNextEpisode ()
            {
                var view = Factory.View;
                uint watched1 = 0;
                uint watched2 = 2;
                var list = Factory.List
                    .Add(
                        Factory.Series(Factory.Name("name1"))
                        .WithWatched(watched1))
                    .Add(
                        Factory.Series(Factory.Name("name2"))
                        .WithWatched(watched2));
                uint width = 80;

                var output = View.Create(view, list, width);
                var next1 = watched1 + 1;
                var next2 = watched2 + 1;

                Assert.Contains(next1.ToString(), output);
                Assert.Contains(next2.ToString(), output);
            }

            [Fact]
            public void
                View_Create_Output_NextEpisode_Is_All_IfWatchedEqualsEpisodes ()
            {
                var view = Factory.View;
                uint watchedAndEpisodes = 1;
                var list = Factory.List.Add(
                    Factory.Series(Factory.Name("name"))
                        .WithEpisodes(watchedAndEpisodes)
                        .WithWatched(watchedAndEpisodes));
                uint width = 80;
                var shouldContain = "All / 1";

                var output = View.Create(view, list, width);

                Assert.Contains(shouldContain, output);
            }

            [Fact]
            public void
                View_Create_Output_TellsAllEpisodesWatched_IfWatchedEqualsEpisodes ()
            {
                uint watchedAndEpisodes = 1;
                var series = Factory.Series(Factory.Name("name"))
                        .WithEpisodes(watchedAndEpisodes)
                        .WithWatched(watchedAndEpisodes);
                var view = Factory.View.WithCurrentlyWatching(series);
                var list = Factory.List.Add(series);
                uint width = 80;
                var shouldContain = "All 1 episodes have been watched.";

                var output = View.Create(view, list, width);

                Assert.Contains(shouldContain, output);
            }

            // - episodes

            [Fact]
            public void
                View_Create_OutputContainsQuestionMarkOnUnknownEpisodeCounts ()
            {
                var expected = "?";
                var view = Factory.View;
                var list = Factory.List.Add(
                    Factory.Series(
                        Factory.Name("name")));
                uint width = 80;

                var output = View.Create(view, list, width);

                Assert.Contains(expected, output);
            }

            [Fact]
            public void View_Create_OutputContainsAmountOfEpisodes ()
            {
                var view = Factory.View;
                uint episodes1 = 1;
                uint episodes2 = 2;
                var list = Factory.List
                    .Add(
                        Factory.Series(Factory.Name("name1"))
                        .WithEpisodes(episodes1))
                    .Add(
                        Factory.Series(Factory.Name("name2"))
                        .WithEpisodes(episodes2));
                uint width = 80;

                var output = View.Create(view, list, width);

                Assert.Contains(episodes1.ToString(), output);
                Assert.Contains(episodes2.ToString(), output);
            }

            // - stream

            [Fact]
            public void View_Create_OutputContains_Yes_IfStreamExists ()
            {
                var view = Factory.View;
                var list = Factory.List.Add(
                    Factory.Series(Factory.Name("name1"))
                    .WithStream(new Uri("https://www.example.com")));
                uint width = 80;
                var expected = "[Stream]";

                var output = View.Create(view, list, width);

                Assert.Contains(expected, output);
            }

            // - watching

            [Fact]
            public void View_Create_OutputContainsNameOfCurrentlyWatchedSeries ()
            {
                var name = "name";
                var series = Factory.Series(Factory.Name(name));
                var view = Factory.View.WithCurrentlyWatching(series);
                var list = Factory.List;
                uint width = 80;

                var output = View.Create(view, list, width);

                Assert.Contains(name, output);
            }

            [Fact]
            public void
                View_Create_OutputContainsEpisodesOfCurrentlyWatchedSeries ()
            {
                uint episodes = 1;
                var series = Factory.Series(Factory.Name("name"))
                    .WithEpisodes(episodes);
                var view = Factory.View.WithCurrentlyWatching(series);
                var list = Factory.List;
                uint width = 80;

                var output = View.Create(view, list, width);

                Assert.Contains(episodes.ToString(), output);
            }

            [Fact]
            public void
                View_Create_OutputContainsNextEpisodeOfCurrentlyWatchedSeries ()
            {
                uint watched = 1;
                var series = Factory.Series(Factory.Name("name"))
                    .WithWatched(watched);
                var view = Factory.View.WithCurrentlyWatching(series);
                var list = Factory.List;
                uint width = 80;

                var output = View.Create(view, list, width);

                Assert.Contains((watched + 1).ToString(), output);
            }

            [Fact]
            public void
                View_Create_OutputContainsStreamOfCurrentlyWatchedSeries ()
            {
                var stream = new Uri("https://www.example.com/");
                var series = Factory.Series(Factory.Name("name"))
                    .WithStream(stream);
                var view = Factory.View.WithCurrentlyWatching(series);
                var list = Factory.List;
                uint width = 80;

                var output = View.Create(view, list, width);

                Assert.Contains(stream.ToString(), output);
            }
        }
    }
}