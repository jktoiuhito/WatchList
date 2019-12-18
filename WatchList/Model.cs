using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace WatchList
{
    public static class Model
    {
        /// <summary>
        /// Add series to the models list. If the series exists in the backlog,
        /// fetch the series from there.
        /// </summary>
        /// <param name="model">Model to which list to add the series.</param>
        /// <param name="name">Name of the series.</param>
        /// <returns>
        ///     Model with a list and backlog containing a series with
        ///     the given name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     When model is null.
        /// </exception>
        public static IModel AddSeriesWithName (this IModel model, string name)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (name == null)
                return model;

            IName createdName;
            try
            {
                //Fails if name is empty or whitespace.
                createdName = Factory.Name(name);
            }
            catch
            {
                return model;
            }

            //Check aliases
            var realName = model.Aliases.GetMainName(createdName);
            if (realName == null)
                realName = createdName;

            var series = model.Backlog.GetSeriesWithName(realName);
            if (series != null)
                return model.WithList(model.List.Add(series));
            else
            {
                series = Factory.Series(realName);
                return model.WithBacklog(model.Backlog.Add(series))
                    .WithList(model.List.Add(series));
            }
        }
    }
}
