using System.Linq;
using System;

namespace WatchList
{
    public static class Controller
    {
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="controller">
        ///     Controller containing the current program state.
        /// </param>
        /// <param name="view">
        ///     View which might be changed by the input.
        /// </param>
        /// <param name="model">
        ///     Model containing data which is operated on.
        /// </param>
        /// <param name="input">User input.</param>
        /// <returns>
        ///     Controller, view and model in a state resulting from
        ///     the user input.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Controller, view and/or model are null.
        /// </exception>
        public static (IController controller, IView view, IModel model)
            HandleInput (
                this IController controller,
                IView view,
                IModel model,
                string input)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(input))
                return (controller, view, model);

            var commands = input.Trim().Split(' ', 2);
            var command = commands[0].Trim().ToLower();
            if (commands.Length < 2)
                return SingleCommand(controller, view, model, command);
            else
            {
                var parameter = commands[1].Trim();
                return ParameterCommand(
                    controller, view, model, command, parameter);
            }
        }

        //These switch-loops are quite stupid, with string literals
        //everywhere. But as the app will never be localized, 
        //thats not an issue.
        //It would've been cool to keep the methods pure AND get the 
        //strings from somewhere else, but that would've required the
        //creation of some localization-data object, which would've been
        //totally useless otherwise.
        //Read: implementing it now would be such a pain I won't bother.
        //Oh well, maybe next time.

        static (IController, IView, IModel) SingleCommand (
            IController controller,
            IView view,
            IModel model,
            string command)
        {
            switch (command)
            {
                case "++":
                    var series = controller.CurrentlyWatching;
                    if (series == null)
                        return (
                            controller,
                            view.WithMessage("Cannot increase watched " +
                            "count: not watching any series."),
                            model);
                    if ((series.Episodes > 0 && 
                        series.Watched == series.Episodes))
                        return (
                            controller,
                            view.WithMessage("Cannot increase watched " +
                            "count: total amount of episodes has already " +
                            "been watched."),
                            model);
                    var newSeries = series.WithWatched(series.Watched + 1);
                    return (controller.WithCurrentlyWatching(newSeries),
                        view.WithCurrentlyWatching(newSeries),
                        model.WithList(
                            model.List.ReplaceAll(series, newSeries))
                            .WithBacklog(
                            model.Backlog.Replace(series, newSeries)));
                case "--":
                    var seris = controller.CurrentlyWatching;
                    if (seris == null)
                        return (
                            controller,
                            view.WithMessage("Cannot decrease watched " +
                            "count: not watching any series."),
                            model);
                    if (seris.Watched <= 0)
                        return (controller,
                            view.WithMessage("Cannot decrease watched " +
                            "count: watched count cannot be set below zero."),
                            model);
                    var newseris = seris.WithWatched(seris.Watched - 1);
                    return (controller.WithCurrentlyWatching(newseris),
                        view.WithCurrentlyWatching(newseris),
                        model.WithList(
                            model.List.ReplaceAll(seris, newseris))
                            .WithBacklog(
                            model.Backlog.Replace(seris, newseris)));
                case "add":
                    return (controller,
                        view.WithMessage("Please specify the name or an " +
                    "alias of the series to be added to the list."),
                        model);
                case "alias":
                    return controller.CurrentlyWatching == null
                        ? (controller,
                            view.WithMessage("Cannot set alias: not " +
                            "watching any series."),
                            model)
                        : (controller,
                            view.WithMessage("Please specify the alias."),
                            model);
                case "backlog":
                    return ((controller.InBacklog
                            ? controller
                            : controller.WithInBacklog(true)),
                        view, model);
                case "episodes":
                    return controller.CurrentlyWatching == null
                        ? (controller,
                            view.WithMessage("Cannot set episode count: not " +
                            "watching any series."),
                            model)
                        : (controller,
                            view.WithMessage("Please specify the total " +
                            "amount of episodes."),
                            model);
                case "exit":
                    return (controller.WithWantsExit(true), view, model);
                case "finish":
                    if (controller.CurrentlyWatching == null)
                        return (controller,
                            view.WithMessage("Cannot finish watching: not " +
                            "watching any series."),
                            model);
                    return (
                        controller.WithCurrentlyWatching(null),
                        view.WithCurrentlyWatching(null),
                        model.WithList(
                            model.List.RemoveAll(
                                controller.CurrentlyWatching)));
                case "help":
                    return (controller,
                        view.WithMessage(
                            "General commands" +
                            "\n\n  add <name/alias>" +
                            "\n     add a series with the given name to " +
                            "the list." +
                            "\n     The name can also be an alias of " +
                            "a series." +
                            "\n     If series with the given name or " +
                            "alias does not yet exist in the backlog, " +
                            "\n     a new series with the given name is " +
                            "created and added to both the list and the " +
                            "backlog." +
                            "\n\n  exit" +
                            "\n     exit the program." +
                            "\n\n  random" +
                            "\n     pick a random series from the list " +
                            "or backlog for watching." +
                            "\n\n  rm <name/alias>" +
                            "\n     remove a series with the given name " +
                            "or alias from the list." +
                            "\n     All of its data will" +
                            " still be stored in the backlog." +
                            "\n\n  watch <name/alias>" +
                            "\n     start watching a series from the " +
                            "list or backlog with the given name or " +
                            "alias." +
                            "\n\nNavigation commands" +
                            "\n\n  backlog" +
                            "\n     change the view mode to backlog." +
                            "\n\n  list" +
                            "\n     change the view mode to list." +
                            "\n\nCommands usable when watching a series" +
                            "\n\n  ++" +
                            "\n     increase the amount of watched " +
                            "episodes of the series being watched by " +
                            "one." +
                            "\n     Does nothing if the amount of watched " +
                            "episodes is equal to the total amount of " +
                            "episodes." +
                            "\n\n  --" +
                            "\n     decrease the amount of watched " +
                            "episodes of the series being watched by one." +
                            "\n     Does nothing if the amount of " +
                            "watched episodes is zero." +
                            "\n\n  alias <alias>" +
                            "\n     adds an alias to the currently " +
                            "watched series." +
                            "\n\n  episodes <number>" +
                            "\n     sets the total episode count of the " +
                            "currently watched series." +
                            "\n     Cannot be smaller " +
                            "than the amount of watched episodes." +
                            //TODO: can the amount be set to zero?
                            "\n\n  finish" +
                            "\n     finish watching the current series " +
                            "and remove it from the list." +
                            "\n\n  stream" +
                            "\n     open the stream of the series." +
                            "\n     Does nothing if no stream-source " +
                            "is set." +
                            "\n\n  stream <uri>" +
                            "\n     set the stream-source of the series. " +
                            "\n     Can also be a local file despite " +
                            "the name." +
                            "\n\n  watched <number>" +
                            "\n     set the amount of watched episodes " +
                            "of the series being watched." +
                            "\n     If the total " +
                            "amount of episodes is set to non-zero, this " +
                            "cannot be larger than that amount."),
                        model);
                case "list":
                    return ((controller.InBacklog
                            ? controller.WithInBacklog(false)
                            : controller),
                        view, model);
                case "random":
                    var randomSeries = (model.List.Count() < 1)
                        ? null
                        : model.List.ElementAt(
                            new Random().Next(0, model.List.Count()));
                    if (randomSeries == null)
                        return (controller,
                            view.WithMessage("Cannot choose random from an " +
                            "empty list."),
                            model);
                    return (
                        controller.WithCurrentlyWatching(randomSeries),
                        view.WithCurrentlyWatching(randomSeries),
                        model);
                case "rm":
                    return (controller,
                        view.WithMessage("Please specify the name or " +
                        "an alias of the series to be removed " +
                        "from the list."),
                    model);
                case "stream":
                    if (controller.CurrentlyWatching == null)
                        return (controller,
                            view.WithMessage("Cannot open stream: not " +
                            "watching any series."),
                            model);
                    if (controller.CurrentlyWatching.Stream == null)
                        return (controller,
                            view.WithMessage("Cannot open stream: watched " +
                            "series does not have a stream source."),
                            model);
                    return (controller.WithOpenStream(
                                controller.CurrentlyWatching.Stream),
                            view,
                            model);
                case "watch":
                    return model.List.Count() <= 0
                        ? (controller,
                            view.WithMessage("Cannot watch from an empty list."),
                            model)
                        : (controller,
                            view.WithMessage("Please specify the name or an " +
                                "alias of the series you want to watch."),
                        model);
                case "watched":
                    return controller.CurrentlyWatching == null
                        ? (
                            controller,
                            view.WithMessage("Cannot set watched episode " +
                            "count: not watching any series."),
                            model)
                        : (
                            controller,
                            view.WithMessage("Please specify the amount of " +
                                "watched episodes."),
                            model);
                default:
                    return (controller,
                        view.WithMessage($"The command '{command}" +
                        "' is unsupported. Type 'help' for a list of " +
                        "supported commands."),
                        model);
            }
        }

        static (IController, IView, IModel) ParameterCommand (
            IController controller,
            IView view,
            IModel model,
            string command,
            string parameter)
        {
            switch (command)
            {
                case "add":
                    var nm = model.Aliases.GetMainName(
                        Factory.Name(parameter));
                    return (
                        controller,
                        view,
                        model.AddSeriesWithName(parameter));
                case "alias":
                    var watching = controller.CurrentlyWatching;
                    if (watching == null)
                        return (controller,
                            view.WithMessage("Cannot set alias: not " +
                            "watching any series."),
                            model);
                    return (controller,
                        view.WithMessage("Added alias '" +
                        $"{parameter.ToLower()}' to series " +
                        $"'{watching.Name.Name}'."),
                        model.WithAliases(model.Aliases.Add(
                            Factory.AliasTuple(
                                Factory.Name(parameter),
                                watching.Name))));
                case "episodes":
                    var series = controller.CurrentlyWatching;
                    if (series == null)
                        return (controller,
                            view.WithMessage("Cannot set total episode " +
                            "count: not watching any series."),
                            model);
                    if (!uint.TryParse(parameter, out var amount))
                        return (controller,
                            view.WithMessage("Cannot set total episode " +
                            "count: parameter must be a positive whole " +
                            "number."),
                            model);
                    if (series.Watched > amount)
                        return (controller,
                            view.WithMessage("Cannot set total episode " +
                            "count: amount of episodes cannot be less " +
                            "than the amount of watched episodes."),
                            model);
                    return (controller.WithCurrentlyWatching(
                                series.WithEpisodes(amount)),
                            view.WithCurrentlyWatching(
                                series.WithEpisodes(amount)),
                            model.WithBacklog(
                                model.Backlog.Replace(
                                    series, series.WithEpisodes(amount)))
                                .WithList(model.List.ReplaceAll(
                                    series, series.WithEpisodes(amount))));
                case "rm":
                    if (model.List.Count() <= 0 )
                        return (controller,
                            view.WithMessage("Cannot remove from an " +
                            "empty list."),
                            model);
                    var name = Factory.Name(parameter);
                    var realName = model.Aliases.GetMainName(name) ?? name;
                    var seriesss = model.List.GetSeriesWithName(realName);
                    if (seriesss == null)
                        return (controller,
                            view.WithMessage("Series with the name " +
                                $"or alias '{name.Name.ToLower()}' was not " +
                                "found in the list."),
                            model);
                    return (controller.CurrentlyWatching != null &&
                        controller.CurrentlyWatching.Equals(seriesss)
                        ? controller.WithCurrentlyWatching(null)
                        : controller,
                        (view.CurrentlyWatching != null &&
                        view.CurrentlyWatching.Equals(seriesss)
                        ? view.WithCurrentlyWatching(null)
                        : view),
                        model.WithList(model.List.RemoveAll(seriesss)));
                case "stream":
                    if (controller.CurrentlyWatching == null)
                        return (controller,
                            view.WithMessage("Cannot set stream: not watching " +
                            "any series."),
                            model);
                    Uri stream;
                    try { stream = new Uri(parameter); }
                    catch {
                        return (controller,
                            view.WithMessage("Cannot set stream: the URI " +
                            "is malformed."),
                            model);
                    }
                    if (controller.CurrentlyWatching == null)
                        return (controller,
                            view.WithMessage("Cannot set stream: not " +
                            "watching any series."),
                            model);
                    var sers =
                        controller.CurrentlyWatching.WithStream(stream);
                    return (controller.WithCurrentlyWatching(sers),
                        view.WithCurrentlyWatching(sers),
                        model
                            .WithBacklog(model.Backlog.Replace(
                                controller.CurrentlyWatching, sers))
                            .WithList(model.List.ReplaceAll(
                                controller.CurrentlyWatching, sers)));
                case "watch":
                    var nname = Factory.Name(parameter);
                    var realname = model.Aliases.GetMainName(nname) ??
                        nname;
                    //This could be redundant... as the series definitely
                    //won't be contained in an empty list either.
                    if (model.List.Count() <= 0)
                        return (controller,
                            view.WithMessage("Cannot watch from an empty " +
                            "list."),
                            model);
                    var seriess = model.List.GetSeriesWithName(realname);
                    if (seriess == null)
                        return (controller,
                            view.WithMessage("Series with the name or alias " +
                            $"'{realname.Name.ToLower()}' was not found in " +
                            "the list."),
                            model);
                    return (controller.WithCurrentlyWatching(seriess),
                        view.WithCurrentlyWatching(seriess),
                        model);
                case "watched":
                    var srs = controller.CurrentlyWatching;
                    if (srs == null)
                        return (controller,
                            view.WithMessage("Cannot set watched episode " +
                            "count: not watching any series."),
                            model);
                    if (!uint.TryParse(parameter, out var ammount))
                        return (controller,
                            view.WithMessage("Cannot set watched count: " +
                            "parameter must be a positive whole number."),
                            model);
                    if (srs.Episodes > 0 && srs.Episodes < ammount)
                        return (controller,
                            view.WithMessage("Cannot set watched count: " +
                            "watched count cannot be over the total " +
                            "episode count."),
                            model);
                    return (controller.WithCurrentlyWatching(
                            srs.WithWatched(ammount)),
                        view.WithCurrentlyWatching(
                            srs.WithWatched(ammount)),
                        model.WithBacklog(
                            model.Backlog.Replace(
                                srs, srs.WithWatched(ammount)))
                            .WithList(model.List.ReplaceAll(
                                srs, srs.WithWatched(ammount))));
                default:
                    return (controller,
                        view.WithMessage("The command '" +
                        $"{command + " " + parameter.ToLower()}" +
                        "' is unsupported. Type 'help' for a list of " +
                        "supported commands."),
                        model);
            }
        }
    }
}
