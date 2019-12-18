using System.Collections.Generic;
using System.Text;
using System;

namespace WatchList
{
    public static class View
    {
        /// <summary>
        /// Create a string displayable in a console window from the
        /// given view and series.
        /// </summary>
        /// <param name="view">View which properties to use.</param>
        /// <param name="seriesEnumeration">
        ///     Series used as data of the view.
        /// </param>
        /// <param name="width">Maximum width of the resulting string.</param>
        /// <returns>String formatted for displaying in consoles.</returns>
        public static string Create (
            this IView view,
            IEnumerable<ISeriesTuple> seriesEnumeration,
            uint width)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (seriesEnumeration == null)
                throw new ArgumentNullException("model");
            if (width < 1)
                throw new ArgumentException("value cannot be zero", "width");

            var builder = new StringBuilder();

            //Title

            builder
                .Append(Ornament(view.Title, '=', width))
                .Append("\n");

            //Series

            int maxLengthName = 0;

            foreach (var series in seriesEnumeration)
                if (series.Name.Name.Length > maxLengthName)
                    maxLengthName = series.Name.Name.Length;

            var padding = 5;
            foreach (var series in seriesEnumeration)
            {
                builder.Append(" * ")
                    .Append(series.Name.Name)
                    .Append(' ', maxLengthName - series.Name.Name.Length)
                    .Append(' ', padding)
                    .Append("On episode ")
                    .Append(
                        series.Episodes > 0 &&
                        series.Watched == series.Episodes
                        ? "All"
                        : (series.Watched + 1).ToString())
                    .Append(" / ")
                    .Append(
                    (series.Episodes < 1)
                        ? "?"
                        : series.Episodes.ToString())
                    .Append(' ', padding)
                    .Append(series.Stream == null ? "" : " [Stream]");
                builder.Append("\n");
            }

            //Watching

            if (view.CurrentlyWatching != null)
            {
                var watched = view.CurrentlyWatching;

                builder
                    .Append("\n")
                    .Append(Ornament(
                        watched.Name.Name,
                        '=',
                        width))
                    .Append("\n")
                    .Append(Ornament(
                        watched.Episodes > 0 &&
                        watched.Episodes == watched.Watched
                        ? $"All {watched.Episodes} episodes have been watched."
                        : "Currently on episode " +
                        (watched.Watched + 1) +
                        ((watched.Episodes > 0)
                            ? " out of " + watched.Episodes + "."
                            : "."),
                        ' ',
                        width))
                    .Append("\n\n")
                    .Append((watched.Stream == null)
                        ? ""
                        : Ornament(
                            "Stream located at " +
                            watched.Stream, ' ', width)
                            .ToString());
            }

            //Message

            if (view.Message != null)
                builder.Append("\n").Append(view.Message).Append("\n");

            //Finished!

            return builder.ToString();
        }

        static StringBuilder Ornament (
            string toOrnament, char ornament, uint outputWidth) =>
            (outputWidth > toOrnament.Length + 2)
            ? new StringBuilder()
                .Append(ornament, ((int)outputWidth - toOrnament.Length - 2) / 2)
                .Append(" ")
                .Append(toOrnament)
                .Append(" ")
                .Append(ornament, ((int)outputWidth - toOrnament.Length - 2) / 2)
            : new StringBuilder(toOrnament);
    }
}
