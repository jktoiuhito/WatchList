using WatchList;
using System;
using Xunit;

namespace WatchListUnitTests
{
    public class AliasesTests
    {
        public class Serialize
        {
            //Argument correctness

            [Fact]
            public void Aliases_Serialize_ThrowsExceptionOnNullInput ()
            {
                IAliases aliases = null;

                Assert.Throws<ArgumentNullException>(
                    () => Aliases.Serialize(aliases));
            }

            //Output correctness

            [Fact]
            public void Aliases_Serialize_ReturnsEmptyOnEmptyAliases ()
            {
                var aliases = Factory.Aliases;

                var serialized = Aliases.Serialize(aliases);

                Assert.Empty(serialized);
            }

            [Fact]
            public void Aliases_Serialize_OutputContainsAliases ()
            {
                var alias1 = Factory.Name("alias");
                var name1 = Factory.Name("Name");
                var alias2 = Factory.Name("anse");
                var name2 = Factory.Name("Another Series");
                var aliases =
                    Factory.Aliases
                    .Add(Factory.AliasTuple(alias1, name1))
                    .Add(Factory.AliasTuple(alias2, name2));

                var serialized = Aliases.Serialize(aliases);

                Assert.Contains(alias1.Name, serialized);
                Assert.Contains(name1.Name, serialized);
            }
        }

        public class Deserialize
        {
            //Argument correctness

            [Fact]
            public void Aliases_Deserialize_ThrowsExceptionOnNullInput ()
            {
                string serialized = null;

                Assert.Throws<ArgumentNullException>(
                    () => Aliases.Deserialize(serialized));
            }

            [Fact]
            public void Aliases_Deserialize_ReturnsEmptyOnEmptyInput ()
            {
                var serialized = "";

                var deserialized = Aliases.Deserialize(serialized);

                Assert.Empty(deserialized);
            }

            [Fact]
            public void Aliases_Deserialize_ReturnsEmptyOnWhitespaceInput ()
            {
                var serialized = "   ";

                var deserialized = Aliases.Deserialize(serialized);

                Assert.Empty(deserialized);
            }

            [Fact]
            public void Aliases_Deserialize_ReturnsEmptyOnNonsenseInput ()
            {
                var serialized = "nonsense";

                var deserialized = Aliases.Deserialize(serialized);

                Assert.Empty(deserialized);
            }

            //Serialized can be deserialized

            [Fact]
            public void Aliases_Deserialize_CanDeserializeSerialized ()
            {
                var alias1 = Factory.Name("alias");
                var name1 = Factory.Name("Name");
                var aliases =
                    Factory.Aliases.Add(Factory.AliasTuple(alias1, name1));

                var serialized = Aliases.Serialize(aliases);
                var deserialized = Aliases.Deserialize(serialized);

                Assert.Equal(aliases, deserialized);
            }
        }
    }
}