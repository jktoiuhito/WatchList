using WatchList;
using System;
using Xunit;

namespace WatchListUnitTests
{
    public class ModelTests
    {
        //Argument correctness

        public class AddSeriesWithName
        {
            //Input correctness

            [Fact]
            public void AddSeriesWithName_ReturnsInputOnNullName ()
            {
                var model = Factory.Model;
                string name = null;

                var addedModel = Model.AddSeriesWithName(model, name);

                Assert.Equal(model, addedModel);
            }

            [Fact]
            public void AddSeriesWithName_ReturnsInputOnEmptyName ()
            {
                var model = Factory.Model;
                var name = "";

                var addedModel = Model.AddSeriesWithName(model, name);

                Assert.Equal(model, addedModel);
            }

            [Fact]
            public void AddSeriesWithName_ReturnsInputOnWhitespaceName ()
            {
                var model = Factory.Model;
                var name = "   ";

                var addedModel = Model.AddSeriesWithName(model, name);

                Assert.Equal(model, addedModel);
            }

            [Fact]
            public void AddSeriesWithName_ThrowsExceptionOnNullModel ()
            {
                IModel model = null;
                var name = "";

                Assert.Throws<ArgumentNullException>(
                    () => Model.AddSeriesWithName(model, name));
            }

            //Functionality

            [Fact]
            public void AddSeriesWithName_AddsNewSeriesToList ()
            {
                var model = Factory.Model;
                var name = "name";

                var addedModel = Model.AddSeriesWithName(model, name);

                Assert.Single(addedModel.List);
            }

            [Fact]
            public void AddSeriesWithName_AddsNewSeriesToBacklog ()
            {
                var model = Factory.Model;
                var name = "name";

                var addedModel = Model.AddSeriesWithName(model, name);

                Assert.Single(addedModel.Backlog);
            }

            [Fact]
            public void
                AddSeriesWithName_ListContainsDuplicatesAfterManyAdditions ()
            {
                var model = Factory.Model;
                var name = "name";
                var expectedCount = 2;

                var addedModel = model
                    .AddSeriesWithName(name)
                    .AddSeriesWithName(name);

                var actualCount = 0;
                foreach (var series in addedModel.List)
                    if (series.Name.Equals(name))
                        actualCount++;

                Assert.Equal(expectedCount, actualCount);
            }

            [Fact]
            public void 
                AddSeriesWithName_BacklogContainsOnlyOneAfterMultipleAdditions ()
            {
                var model = Factory.Model;
                var name = "name";

                var addedModel = model.AddSeriesWithName(name)
                    .AddSeriesWithName(name);

                Assert.Single(addedModel.Backlog);
            }

            [Fact]
            public void
                Model_AddSeriesWithName_SeriesInBacklogIsCopiedToList ()
            {
                var name = "name";
                uint episodes = 1;
                uint watched = 1;
                var stream = new Uri("https://www.example.com/");

                var model = Factory.Model.WithBacklog(
                    Factory.Backlog.Add(
                        Factory.Series(
                            Factory.Name(name))
                            .WithEpisodes(episodes)
                            .WithWatched(watched)
                            .WithStream(stream)));

                var addedModel = model.AddSeriesWithName(name);

                var enumerator = addedModel.List.GetEnumerator();
                enumerator.MoveNext();
                var seriesInList = enumerator.Current;

                Assert.True(seriesInList.Name.Equals(name));
                Assert.Equal(episodes, seriesInList.Episodes);
                Assert.Equal(watched, seriesInList.Watched);
                Assert.Equal(stream, seriesInList.Stream);
            }
        }
    }
}