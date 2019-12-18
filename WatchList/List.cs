using System.Xml.Linq;
using System;

namespace WatchList
{
    class List
    {
        //Untested !! (but its copypasted from Backlog.Serialize,
        //which is tested.)
        public static IList Deserialize (string serialized)
        {
            if (serialized == null)
                throw new ArgumentNullException(nameof(serialized));
            if (serialized.Trim().Equals(""))
                return Factory.List;

            try
            {
                var document = XDocument.Parse(serialized);
                var list = Factory.List;
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

                        list = list.Add(createdSeries);
                    }
                    catch { throw; }
                }
                return list;
            }
            catch
            {
                return Factory.List;
            }
        }
    }
}
