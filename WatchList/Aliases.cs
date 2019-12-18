using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;

namespace WatchList
{
    public static class Aliases
    {
        /// <summary>
        /// Get aliases in a serializable, human-readable format.
        /// </summary>
        /// <param name="aliases">Aliases to be serialized.</param>
        /// <returns>
        ///     Aliases in a serializable, human-readable format.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     When aliases is null.
        /// </exception>
        public static string Serialize (this IAliases aliases)
        {
            if (aliases == null)
                throw new ArgumentNullException("aliases");
            if (aliases.Count() == 0)
                return "";

            var document = new XDocument();
            var root = new XElement("Aliases");
            document.Add(root);

            foreach (var aliasTuple in aliases)
                root.Add(
                    new XElement("AliasRecord",
                        new XElement("Alias", aliasTuple.Alias.Name),
                        new XElement("Name", aliasTuple.Name.Name)));

            return document.ToString();
        }

        /// <summary>
        /// Deserialize aliases from a string.
        /// </summary>
        /// <param name="serialized">Aliases in a serialized format.</param>
        /// <returns>Deserialized aliases.</returns>
        /// <exception cref="ArgumentNullException">
        ///     When serialized is null.
        /// </exception>
        public static IAliases Deserialize (string serialized)
        {
            if (serialized == null)
                throw new ArgumentNullException("serialized");
            if (serialized.Trim().Equals(""))
                return Factory.Aliases;

            try
            {
                var document = XDocument.Parse(serialized);
                var aliases = Factory.Aliases;
                var aliasRecords = document.Root.Elements("AliasRecord");

                foreach (var aliasRecord in aliasRecords)
                {
                    try
                    {
                        aliases = aliases.Add(Factory.AliasTuple(
                            Factory.Name(aliasRecord.Element("Alias").Value),
                            Factory.Name(aliasRecord.Element("Name").Value)));
                    }
                    catch { }
                }
                return aliases;
            }
            catch
            {
                return Factory.Aliases;
            }
        }
    }
}
