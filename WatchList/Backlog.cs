using System.Xml.Linq;
using System.Linq;
using System;
using System.Collections.Generic;

namespace WatchList
{
    public static class Backlog
    {
        /// <summary>
        /// Get backlog in a serializable, human-readable format.
        /// </summary>
        /// <param name="aliases">Backlog to be serialized.</param>
        /// <returns>
        ///     Backlog in a serializable, human-readable format.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     When backlog is null.
        /// </exception>
        public static string Serialize (this IBacklog backlog) =>
            Serialize((IEnumerable<ISeriesTuple>)backlog);

        public static string Serialize (IEnumerable<ISeriesTuple> list)
        {
            if (list == null)
                throw new ArgumentNullException("backlog");
            if (list.Count() < 1)
                return "";

            var document = new XDocument();
            var root = new XElement("Backlog");
            document.Add(root);

            foreach (var series in list)
                root.Add(
                    new XElement("Series",
                        new XElement("Name", series.Name.Name),
                        new XElement("Episodes", series.Episodes),
                        new XElement("Watched", series.Watched),
                        new XElement("Stream", series.Stream)
                        ));

            return document.ToString();
        }

        /// <summary>
        /// Deserialize backlog from a string.
        /// </summary>
        /// <param name="serialized">Backlog in a serialized format.</param>
        /// <returns>Deserialized backlog.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When serialized is null.
        /// </exception>
        public static IBacklog Deserialize (string serialized)
        {
            if (serialized == null)
                throw new ArgumentNullException("serialized");
            if (serialized.Trim().Equals(""))
                return Factory.Backlog;

            try
            {
                var document = XDocument.Parse(serialized);
                var backlog = Factory.Backlog;
                var series = document.Root.Elements("Series");

                foreach (var serie in series)
                {
                    try
                    {
                        var createdSeries = Factory.Series(
                            Factory.Name(serie.Element("Name").Value));

                        try
                        {
                            createdSeries = createdSeries.WithEpisodes(
                                uint.Parse(serie.Element("Episodes").Value));
                        }
                        catch { }
                        try
                        {
                            createdSeries = createdSeries.WithWatched(
                                uint.Parse(serie.Element("Watched").Value));
                        }
                        catch { }
                        try
                        {
                            createdSeries = createdSeries.WithStream(
                                new Uri(serie.Element("Stream").Value));
                        }
                        catch { }

                        backlog = backlog.Add(createdSeries);
                    }
                    catch { throw; }
                }
                return backlog;
            }
            catch
            {
                return Factory.Backlog;
            }
        }
    }
}
