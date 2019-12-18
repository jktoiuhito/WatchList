using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System;

namespace WatchList
{
    /*
     * - DONE
     *      - display that all episodes have been watched.
     *      - display 'all X episodes watched' if watched series has all
     *        episodes watched.
     *      - 'stream <uri>' should not set a message
     *      - add, watch, episodes X OR watched X, rm -> watched remains
     * 
     * - BUGS
     *      - episode count cannot be set to zero if watched is set.
     * 
     * - TODO
     *      - document exceptions on interfaces (and overall finish doc).
     * 
     * - Post-release (read: might do, might not)
     *      - 'random' should work on backlog
     *      - 'watch <series>' should work on backlog
     *      - 'rewatch' - reset the watched count to zero.
     *      - 'name <name>' sets name of series being watched.
     *      - inform if reading from BACKLOG or ALIASES failed, and where
     *        so the user can fix it.
     */

    //This is the only class with side-effects.
    //All IO is focused in one place!
    //Beautiful, isn't it?
    static class Program
    {
        static void Main ()
        {
            var root = Directory.GetCurrentDirectory();
            var BacklogPath = root + "\\BACKLOG";
            var AliasesPath = root + "\\ALIASES";
            var ListPath = root + "\\LIST.temp";

            var controller = Factory.Controller;
            var view = Factory.View;
            var model = Factory.Model
                .WithAliases(Aliases.Deserialize(ReadFile(AliasesPath)))
                .WithBacklog(Backlog.Deserialize(ReadFile(BacklogPath)))
                .WithList(List.Deserialize(ReadFile(ListPath)));

            Console.Title = "WatchList";
            Console.WriteLine(
                view.Create(model.List, (uint)Console.BufferWidth));

            while (true)
            {
                (controller, view, model) = controller.HandleInput(
                    view,
                    model,
                    Console.ReadLine());

                Task.Run(() => WriteFile(
                    ListPath, Backlog.Serialize(model.List)));
                Task.Run(() => WriteFile(
                    BacklogPath, model.Backlog.Serialize()));
                Task.Run(() => WriteFile(
                    AliasesPath, model.Aliases.Serialize()));

                Console.Clear();
                Console.WriteLine(
                    view.Create(
                        controller.InBacklog
                            ? (IEnumerable<ISeriesTuple>)model.Backlog
                            : model.List,
                        (uint)Console.BufferWidth));
                view = view.WithMessage(null);

                if (controller.WantsExit)
                    break;
                if (controller.OpenStream != null)
                {
                    Process.Start(new ProcessStartInfo(
                        controller.OpenStream.ToString())
                            { UseShellExecute = true });
                    controller = controller.WithOpenStream(null);
                }
            }
            File.Delete(ListPath);
        }

        static string ReadFile (string file)
        {
            try
            {
                return File.ReadAllText(file);
            }
            catch
            {
                return "";
            }
        }

        static void WriteFile (string file, string content) =>
            File.WriteAllText(file, content);
    }
}
