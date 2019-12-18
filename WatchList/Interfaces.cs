using System.Collections.Generic;
using System;

namespace WatchList
{
    /// <summary>
    /// Controller containing program state.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Does this controller want the program to exit?
        /// </summary>
        bool WantsExit { get; }

        /// <summary>
        /// Series currently being watched. Null if no series is being
        /// watched.
        /// </summary>
        ISeriesTuple CurrentlyWatching { get; }

        /// <summary>
        /// Is the controller currently accessing the backlog?
        /// If false, it is accessing the normal list.
        /// </summary>
        bool InBacklog { get; }

        /// <summary>
        /// This contains a URI that the user wants opened in a browser.
        /// Null if no opening is required.
        /// </summary>
        Uri OpenStream { get; }

        /// <summary>
        /// The controller with the given want to exit.
        /// </summary>
        /// <param name="wants">Does the controller want to exit.</param>
        /// <returns>Controller with the specified want to exit.</returns>
        IController WithWantsExit (bool wants);

        /// <summary>
        /// Get a controller with the given series being the currently
        /// watched one.
        /// </summary>
        /// <param name="series">Series being currently watched.</param>
        /// <returns>
        ///     Controller with the series being currently watched.
        /// </returns>
        IController WithCurrentlyWatching (ISeriesTuple series);

        /// <summary>
        /// Get a controller which is or is not in the backlog.
        /// </summary>
        /// <param name="isIn">Is the controller in the backlog?</param>
        /// <returns>
        ///     Controller with InBacklog set to the given value.
        /// </returns>
        IController WithInBacklog (bool isIn);

        IController WithOpenStream (Uri uri);
    }

    /// <summary>
    /// View containing displayable information.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Title displayed in the view.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Message displayed in the bottom of the view.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Series being currently watched, which data is displayed
        /// more accurately in the view.
        /// </summary>
        ISeriesTuple CurrentlyWatching { get; }

        /// <summary>
        /// The view with the given title. Whitespace is trimmed off.
        /// Title cannot be null, empty or whitespace.
        /// </summary>
        /// <param name="title">Title of the view.</param>
        /// <returns>View with the given title.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When title is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     When title is empty or whitespace.
        /// </exception>
        IView WithTitle (string title);

        IView WithMessage (string message);

        IView WithCurrentlyWatching (ISeriesTuple series);
    }

    /// <summary>
    /// Model containing data.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// List stored in the model.
        /// </summary>
        IList List { get; }

        /// <summary>
        /// Backlog stored in the model.
        /// </summary>
        IBacklog Backlog { get; }

        /// <summary>
        /// Aliases stored in the model.
        /// </summary>
        IAliases Aliases { get; }

        /// <summary>
        /// Get a model with the given list.
        /// List cannot be null.
        /// </summary>
        /// <param name="list">List.</param>
        /// <returns>Model with the given list.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When list is null.
        /// </exception>
        IModel WithList (IList list);

        /// <summary>
        /// Get a model with the given backlog.
        /// Backlog cannot be null.
        /// </summary>
        /// <param name="backlog">Backlog.</param>
        /// <returns>Model with the given backlog.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When backlog is null.
        /// </exception>
        IModel WithBacklog (IBacklog backlog);

        /// <summary>
        /// Get a model with the given aliases.
        /// Aliases cannot be null.
        /// </summary>
        /// <param name="backlog">Aliases.</param>
        /// <returns>Model with the given aliases.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When aliases is null.
        /// </exception>
        IModel WithAliases (IAliases aliases);
    }

    //Tables

    /// <summary>
    /// Backlog containing zero to many series.
    /// Only one copy of a series with the same name can be stored.
    /// </summary>
    public interface IBacklog : IEnumerable<ISeriesTuple>
    {
        /// <summary>
        /// Add series to the backlog.
        /// If a series with the given name is already contained,
        /// adds nothing.
        /// </summary>
        /// <param name="series">Series to be added to the backlog.</param>
        /// <returns>Backlog with the added series.</returns>
        IBacklog Add (ISeriesTuple series);

        /// <summary>
        /// Replace an existing series with a new one.
        /// If the old series is null or does not exist, do nothing.
        /// New series cannot be null.
        /// </summary>
        /// <param name="old">Series to be replaced.</param>
        /// <param name="new">Series to replace the old series.</param>
        /// <returns>Backlog with the series replaced.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When @new is null.
        /// </exception>
        IBacklog Replace (ISeriesTuple old, ISeriesTuple @new);

        /// <summary>
        /// Get a series stored in the backlog with a given name.
        /// Returns null if no series was found.
        /// </summary>
        /// <param name="name">Name of the series.</param>
        /// <returns>
        ///     Series as stored in the backlog. Null if none was found.
        /// </returns>
        ISeriesTuple GetSeriesWithName (IName name);
    }

    /// <summary>
    /// Different names of a same series. Each alias is contained only 
    /// once, and is associated with only one series.
    /// </summary>
    public interface IAliases : IEnumerable<IAliasTuple>
    {
        /// <summary>
        /// Add an alias to a main calling name.
        /// Alias cannot be null.
        /// </summary>
        /// <param name="alias">Alias to be added.</param>
        /// <returns>Aliases with the added alias.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When alias is null.
        /// </exception>
        IAliases Add (IAliasTuple alias);

        /// <summary>
        /// Get the main calling name of the series the given alias points to.
        /// Returns null if no alias with the given name is found.
        /// </summary>
        /// <param name="alias">An alias.</param>
        /// <returns>
        ///     Name of the series the alias points to.
        ///     Null if the alias is not contained in this collection.
        /// </returns>
        IName GetMainName (IName alias);
    }

    /// <summary>
    /// List containing zero to many series.
    /// Each series may be contained more than once.
    /// </summary>
    public interface IList : IEnumerable<ISeriesTuple>
    {
        /// <summary>
        /// Add a series to a list.
        /// </summary>
        /// <param name="series">Series to be added.</param>
        /// <returns>List with the series added.</returns>
        IList Add (ISeriesTuple series);

        /// <summary>
        /// Get the first series stored in the list with the given name.
        /// Returns null if no series were found.
        /// </summary>
        /// <param name="name">Name of the series.</param>
        /// <returns>
        ///     First series with the given name stored in the list.
        ///     Null if no series were found.
        /// </returns>
        ISeriesTuple GetSeriesWithName (IName name);

        /// <summary>
        /// Replace all existing series with a new one.
        /// If the old series does not exist, do nothing.
        /// </summary>
        /// <param name="old">Series to be replaced.</param>
        /// <param name="new">Series to replace the old series.</param>
        /// <returns>List with the series replaced.</returns>
        /// <exception cref="ArgumentNullException">
        ///     old is null, new is null
        /// </exception>
        IList ReplaceAll (ISeriesTuple old, ISeriesTuple @new);

        /// <summary>
        /// Remove all occurences of the given series from the list.
        /// </summary>
        /// <param name="series">Series to be removed from the list.</param>
        /// <returns>List with the series removed.</returns>
        IList RemoveAll (ISeriesTuple series);
    }

    //Records

    /// <summary>
    /// Series name, episode count, watched count and stream source.
    /// </summary>
    public interface ISeriesTuple
    {
        /// <summary>
        /// Name of the series.
        /// Can never be null.
        /// </summary>
        IName Name { get; }

        /// <summary>
        /// Amount of episodes.
        /// Always equal or bigger than Watched if both are set.
        /// </summary>
        uint Episodes { get; }

        /// <summary>
        /// Amount of watched episodes.
        /// Always equal or smaller than Episodes if both are set.
        /// </summary>
        uint Watched { get; }

        /// <summary>
        /// Stream source.
        /// Can be null.
        /// </summary>
        Uri Stream { get; }

        /// <summary>
        /// Get a series with the given name.
        /// Name cannot be null.
        /// </summary>
        /// <param name="name">Name of the series.</param>
        /// <returns>Series with the given name.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When name is null.
        /// </exception>
        ISeriesTuple WithName (IName name);

        /// <summary>
        /// Get a series with the given episode count.
        /// If watched count is set, must be equal or higher than it.
        /// </summary>
        /// <param name="episodes">Series episode count.</param>
        /// <returns>Series with the given episode count.</returns>
        /// <exception cref="ArgumentException">
        ///     When watched is non-zero and larger than given episode count.
        /// </exception>
        ISeriesTuple WithEpisodes (uint episodes);

        /// <summary>
        /// Get a series with the given watched episodes count.
        /// If episode count is set, must be equal or lower than it.
        /// </summary>
        /// <param name="watched">Series watched episodes count.</param>
        /// <returns>Series with the given watched episodes count.</returns>
        /// <exception cref="ArgumentException">
        ///     When episodes is non-zero and smaller than given watched count.
        /// </exception>
        ISeriesTuple WithWatched (uint watched);

        /// <summary>
        /// Get a series with the given stream source.
        /// </summary>
        /// <param name="stream">Stream source of the series.</param>
        /// <returns></returns>
        ISeriesTuple WithStream (Uri stream);
    }

    /// <summary>
    /// Alias and the name it refers to.
    /// </summary>
    public interface IAliasTuple
    {
        //TODO: document IAlias

        /// <summary>
        /// Alias referring to a name.
        /// </summary>
        IName Alias { get; }

        /// <summary>
        /// Name the alias refers to.
        /// </summary>
        IName Name { get; }

        IAliasTuple WithAlias (IName alias);

        IAliasTuple WithName (IName name);
    }

    //Values

    /// <summary>
    /// Name which is never null, empty or whitespace, and has
    /// case-insensitive comparison and hashing.
    /// </summary>
    public interface IName
    {
        /// <summary>
        /// Name of a series.
        /// Is never null, empty or whitespace.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get a name with the given name (...).
        /// Name cannot be null, empty or whitespace.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Name with the given name.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When name is null.
        ///  </exception>
        ///  <exception cref="ArgumentException">
        ///     When name is empty or whitespace.
        ///  </exception>
        IName WithName (string name);
    }
}
