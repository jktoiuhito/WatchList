using System.Collections.Immutable;
using System.Collections.Generic;
using System.Collections;
using System;

namespace WatchList
{
    /// <summary>
    /// Produces necessary objects.
    /// </summary>
    public static class Factory
    {
        //MVC components

        /// <summary>
        /// A default-value controller.
        /// </summary>
        public static readonly IController Controller =
            new ControllerImplementation();

        /// <summary>
        /// A default-value view.
        /// </summary>
        public static readonly IView View = 
            new ViewImplementation().WithTitle("WatchList");

        /// <summary>
        /// A default-value (empty) model.
        /// </summary>
        public static readonly IModel Model =
            new ModelImplementation(
                new ListImplementation(
                    ImmutableArray<ISeriesTuple>.Empty),
                new BacklogImplementation(
                    ImmutableDictionary<IName, ISeriesTuple>.Empty),
                new AliasesImplementation(
                    ImmutableDictionary<IName, IAliasTuple>.Empty));

        //Tables

        /// <summary>
        /// A default-value (empty) backlog.
        /// </summary>
        public static readonly IBacklog Backlog =
            new BacklogImplementation(
                ImmutableDictionary<IName, ISeriesTuple>.Empty);

        /// <summary>
        /// A default-value (empty) aliases.
        /// </summary>
        public static readonly IAliases Aliases =
            new AliasesImplementation(
                ImmutableDictionary<IName, IAliasTuple>.Empty);

        /// <summary>
        /// A default-value (empty) list.
        /// </summary>
        public static readonly IList List = 
            new ListImplementation(ImmutableArray<ISeriesTuple>.Empty);

        //Records

        /// <summary>
        /// Construct a new series.
        /// Name cannot be null.
        /// </summary>
        /// <param name="name">Name of the series.</param>
        /// <returns>Properly constructed series.</returns>
        public static ISeriesTuple Series (IName name) =>
            new SeriesTupleImplementation().WithName(name);

        /// <summary>
        /// Construct an alias.
        /// Parameters cannot be null.
        /// </summary>
        /// <param name="alias">Alias to the name.</param>
        /// <param name="name">Name.</param>
        /// <returns>Properly constructed alias.</returns>
        public static IAliasTuple AliasTuple (IName alias, IName name) =>
            new AliasTupleImplementation(alias, name);

        //Values

        /// <summary>
        /// Construct a new name.
        /// Parameter cannot be null, empty or whitespace.
        /// </summary>
        /// <param name="name">Name of the name.</param>
        /// <returns>Properly contstructed name.</returns>
        public static IName Name (string name) =>
            new NameImplementation().WithName(name);

        #region Implementation details

        //MVC components

        readonly struct ControllerImplementation : IController
        {
            public bool WantsExit { get; }

            public ISeriesTuple CurrentlyWatching { get; }

            public bool InBacklog { get; }

            public Uri OpenStream { get; }

            public IController WithWantsExit (bool wants) =>
                (wants == WantsExit)
                ? this
                : new ControllerImplementation(
                    wants, CurrentlyWatching, InBacklog, OpenStream);

            public IController WithCurrentlyWatching (ISeriesTuple series) =>
                (series == CurrentlyWatching)
                ? this
                : new ControllerImplementation(
                    WantsExit, series, InBacklog, OpenStream);

            public IController WithInBacklog (bool isIn) =>
                isIn == InBacklog
                ? this
                : new ControllerImplementation(
                    WantsExit, CurrentlyWatching, isIn, OpenStream);

            public IController WithOpenStream (Uri uri) =>
                uri == OpenStream
                ? this
                : new ControllerImplementation(
                    WantsExit, CurrentlyWatching, InBacklog, uri);

            private ControllerImplementation (
                bool exit, ISeriesTuple watching, bool inBacklog, Uri open)
            {
                WantsExit = exit;
                CurrentlyWatching = watching;
                InBacklog = inBacklog;
                OpenStream = open;
            }
        }

        readonly struct ViewImplementation : IView
        {
            public string Title { get; }

            public ISeriesTuple CurrentlyWatching { get; }

            public string Message { get; }

            public IView WithTitle (string title) =>
                title == Title
                ? this
                : new ViewImplementation(
                    title, CurrentlyWatching, Message);

            public IView WithCurrentlyWatching (ISeriesTuple series) =>
                series == CurrentlyWatching
                ? this
                : new ViewImplementation(Title, series, Message);

            public IView WithMessage (string message) =>
                message == Message
                ? this
                : new ViewImplementation(Title, CurrentlyWatching, message);

            private ViewImplementation (
                string title,
                ISeriesTuple watching,
                string message)
            {
                if (title == null)
                    throw new ArgumentNullException("title");
                if (title.Equals(""))
                    throw new ArgumentException("cannot be empty", "title");
                if (title.Trim().Equals(""))
                    throw new ArgumentException(
                        "cannot be whitespace", "title");

                Title = title.Trim();
                CurrentlyWatching = watching;
                Message = message;
            }
        }

        readonly struct ModelImplementation : IModel
        {
            public IList List { get; }

            public IBacklog Backlog { get; }

            public IAliases Aliases { get; }

            public IModel WithBacklog (IBacklog backlog) =>
                new ModelImplementation(List, backlog, Aliases);

            public IModel WithList (IList list) =>
                new ModelImplementation(list, Backlog, Aliases);

            public IModel WithAliases (IAliases aliases) =>
                new ModelImplementation(List, Backlog, aliases);

            //Visible only inside Factory.
            public ModelImplementation (
                IList list, IBacklog backlog, IAliases aliases)
            {
                List = list ??
                    throw new ArgumentNullException("list");
                Backlog = backlog ??
                    throw new ArgumentNullException("backlog");
                Aliases = aliases ??
                    throw new ArgumentNullException("aliases");
            }
        }

        //Collections

        readonly struct BacklogImplementation : IBacklog
        {
            readonly IImmutableDictionary<IName, ISeriesTuple> _backlog;

            public IBacklog Add (ISeriesTuple series) =>
                series == null
                ? this
                : _backlog.ContainsKey(series.Name)
                    ? this
                    : new BacklogImplementation(
                        _backlog.Add(series.Name, series));

            public ISeriesTuple GetSeriesWithName (IName name)
            {
                foreach (var series in _backlog.Values)
                    if (series.Name.Equals(name))
                        return series;
                return null;
            }

            public IBacklog Replace (ISeriesTuple old, ISeriesTuple @new) =>
                old == null
                ? throw new ArgumentNullException("old")
                : @new == null
                ? throw new ArgumentNullException("@new")
                : _backlog.ContainsKey(old.Name)
                    ? new BacklogImplementation(
                        _backlog.Remove(old.Name).Add(@new.Name, @new))
                    : this;

            public IEnumerator<ISeriesTuple> GetEnumerator () =>
                _backlog.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

            public BacklogImplementation (
                IImmutableDictionary<IName, ISeriesTuple> backlog) =>
                _backlog = backlog ??
                    throw new ArgumentNullException("backlog");
        }

        readonly struct AliasesImplementation : IAliases
        {
            readonly IImmutableDictionary<IName, IAliasTuple> _aliases;

            public IAliases Add (IAliasTuple alias)
            {
                if (alias == null)
                    throw new ArgumentNullException("alias");
                return new AliasesImplementation(_aliases.Add(alias.Alias, alias));
            }

            public IName GetMainName (IName alias) =>
                _aliases.TryGetValue(alias, out var tuple)
                ? tuple.Name
                : null;

            public IEnumerator<IAliasTuple> GetEnumerator () =>
                _aliases.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

            //Visible only inside Factory.
            public AliasesImplementation (
                IImmutableDictionary<IName, IAliasTuple> aliases) =>
                _aliases = aliases ?? 
                    throw new ArgumentNullException("aliases");
        }

        readonly struct ListImplementation : IList
        {
            readonly IImmutableList<ISeriesTuple> _list;

            public IList Add (ISeriesTuple series) =>
                (series == null)
                ? this
                : new ListImplementation(_list.Add(series));

            public ISeriesTuple GetSeriesWithName (IName name)
            {
                if (name == null)
                    throw new ArgumentNullException("name");
                foreach (var series in _list)
                    if (series.Name.Equals(name))
                        return series;
                return null;
            }

            public IList RemoveAll (ISeriesTuple series) =>
                (series == null)
                ? throw new ArgumentNullException("series")
                : (_list.IndexOf(series) == -1)
                    ? this
                    : new ListImplementation(
                        _list.RemoveAll((s) => s.Equals(series)));

            public IList ReplaceAll (ISeriesTuple old, ISeriesTuple @new) =>
                old == null
                ? throw new ArgumentNullException(nameof(old))
                : @new == null
                ? throw new ArgumentNullException(nameof(@new))
                : old.Equals(@new)
                ? this
                : _list.IndexOf(old) < 0
                    ? this
                    : new ListImplementation(_list.Replace(old, @new))
                        .ReplaceAll(old, @new);

            public IEnumerator<ISeriesTuple> GetEnumerator () =>
                _list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

            //Only visible inside Factory.
            public ListImplementation (IImmutableList<ISeriesTuple> list) =>
                _list = list ?? throw new ArgumentNullException("list");
        }

        //Rows

        readonly struct SeriesTupleImplementation : ISeriesTuple
        {
            public IName Name { get; }

            public uint Episodes { get; }

            public uint Watched { get; }

            public Uri Stream { get; }

            public ISeriesTuple WithName (IName name) =>
                new SeriesTupleImplementation(name, Episodes, Watched, Stream);

            public ISeriesTuple WithEpisodes (uint episodes) =>
                new SeriesTupleImplementation(Name, episodes, Watched, Stream);

            public ISeriesTuple WithWatched (uint watched) =>
                new SeriesTupleImplementation(Name, Episodes, watched, Stream);

            public ISeriesTuple WithStream (Uri stream) =>
                new SeriesTupleImplementation(Name, Episodes, Watched, stream);

            private SeriesTupleImplementation (
                IName name,
                uint episodes,
                uint watched,
                Uri stream)
            {
                Name = name ?? throw new ArgumentNullException("name");
                if (episodes > 0 && episodes < watched)
                    throw new ArgumentException(
                        "episodes cannot be less than watched");
                Episodes = episodes;
                Watched = watched;
                Stream = stream;
            }
        }

        readonly struct AliasTupleImplementation : IAliasTuple
        {
            public IName Alias { get; }

            public IName Name { get; }

            public IAliasTuple WithAlias (IName alias) =>
                new AliasTupleImplementation(alias, Name);

            public IAliasTuple WithName (IName name) =>
                new AliasTupleImplementation(Alias, name);

            //Visible only inside Factory.
            public AliasTupleImplementation (IName alias, IName name)
            {
                Alias = alias ?? throw new ArgumentNullException(
                    "alias", "value cannot be null");
                Name = name ?? throw new ArgumentNullException(
                    "name", "value cannot be null");
            }
        }

        //Values

        readonly struct NameImplementation : IName
        {
            public string Name { get; }

            public IName WithName (string name) =>
                new NameImplementation(name);

            public override bool Equals (object obj)
            {
                if (obj == null)
                    return false;
                if (obj.GetType() == typeof(string))
                {
                    var other = (string)obj;
                    return Name.ToLower().Equals(other.Trim().ToLower());
                }
                if (obj.GetType() == GetType())
                {
                    var other = (IName)obj;
                    return Name.ToLower().Equals(other.Name.ToLower());
                }
                return false;
            }

            public override int GetHashCode () => Name.ToLower().GetHashCode();

            private NameImplementation (string name)
            {
                if (name == null)
                    throw new ArgumentNullException("name");
                if (name.Equals(""))
                    throw new ArgumentException("cannot be empty", "name");
                if (name.Trim().Equals(""))
                    throw new ArgumentException(
                        "cannot be whitespace", "name");

                Name = name.Trim();
            }
        }

        #endregion
    }
}
