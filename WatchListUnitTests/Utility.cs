using WatchList;

namespace WatchListUnitTests
{
    public static class Utility
    {
        /// <summary>
        /// Utility method for calling Controller.HandleInput with the
        /// necessary inputs wrapped into a tuple.
        /// </summary>
        /// <param name="data">Controller, view and model in a tuple.</param>
        /// <param name="command">String representing user input.</param>
        /// <returns>Updated controller, view and model in a tuple.</returns>
        public static (IController controller, IView view, IModel model)
            Command (this (IController, IView, IModel) data, string command) =>
            Controller.HandleInput(data.Item1, data.Item2, data.Item3, command);

        /// <summary>
        /// Empty starting state for controller, view and model in a tuple.
        /// </summary>
        public static readonly (IController controller, IView view, IModel model)
            Empty = (Factory.Controller, Factory.View, Factory.Model);
    }
}
