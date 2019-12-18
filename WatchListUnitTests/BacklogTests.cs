using WatchList;
using System;
using Xunit;

namespace WatchListUnitTests
{
    public class BacklogTests
    {
        public class Serialize
        {
            //Argument correctness

            [Fact]
            public void Backlog_Serialize_ThrowsExceptionOnNullInput ()
            {
                IBacklog backlog = null;

                Assert.Throws<ArgumentNullException>(
                    () => Backlog.Serialize(backlog));
            }

            //Output correctness

            [Fact]
            public void Backlog_Serialize_ReturnsEmptyOnEmptyBacklog ()
            {
                var backlog = Factory.Backlog;

                var serialized = Backlog.Serialize(backlog);

                Assert.Empty(serialized);
            }

            [Fact]
            public void Backlog_Serialize_OutputContainsSeriesData ()
            {
                var series1 = Factory.Series(Factory.Name("Series 1"))
                    .WithEpisodes(2)
                    .WithWatched(1)
                    .WithStream(new Uri("https://www.example.com"));
                var series2 = Factory.Series(Factory.Name("Another Series"))
                    .WithEpisodes(4)
                    .WithWatched(3)
                    .WithStream(new Uri("https://another.example.com"));
                var backlog = Factory.Backlog
                    .Add(series1)
                    .Add(series2);

                var serialized = Backlog.Serialize(backlog);

                Assert.Contains(series1.Name.Name, serialized);
                Assert.Contains(series1.Episodes.ToString(), serialized);
                Assert.Contains(series1.Watched.ToString(), serialized);
                Assert.Contains(series1.Stream.ToString(), serialized);

                Assert.Contains(series2.Name.Name, serialized);
                Assert.Contains(series2.Episodes.ToString(), serialized);
                Assert.Contains(series2.Watched.ToString(), serialized);
                Assert.Contains(series2.Stream.ToString(), serialized);
            }
        }

        public class Deserialize
        {
            //Argument correctness

            [Fact]
            public void Backlog_Deserialize_ThrowsExceptionOnNullInput ()
            {
                string serialized = null;

                Assert.Throws<ArgumentNullException>(
                    () => Backlog.Deserialize(serialized));
            }

            [Fact]
            public void Backlog_Deserialize_ReturnsEmptyOnEmptyInput ()
            {
                var serialized = "";

                var deserialized = Backlog.Deserialize(serialized);

                Assert.Empty(deserialized);
            }

            [Fact]
            public void Backlog_Deserialize_ReturnsEmptyOnWhitespaceInput ()
            {
                var serialized = "   ";

                var deserialized = Backlog.Deserialize(serialized);

                Assert.Empty(deserialized);
            }

            [Fact]
            public void Backlog_Deserialize_ReturnsEmptyOnNonsenseInput ()
            {
                var serialized = "nonsense";

                var deserialized = Backlog.Deserialize(serialized);

                Assert.Empty(deserialized);
            }

            //Serialized can be deserialized

            [Fact]
            public void Backlog_Serialize_OutputContainsSeriesData ()
            {
                var backlog = Factory.Backlog
                    .Add(Factory.Series(Factory.Name("Series 1"))
                        .WithEpisodes(2)
                        .WithWatched(1)
                        .WithStream(new Uri("https://www.example.com")))
                    .Add(Factory.Series(Factory.Name("Another Series"))
                        .WithEpisodes(4)
                        .WithWatched(3)
                        .WithStream(new Uri("https://another.example.com")));

                var serialized = Backlog.Serialize(backlog);
                var deserialized = Backlog.Deserialize(serialized);

                Assert.Equal(backlog, deserialized);
            }
        }
    }
}